using System;
using System.Collections.Generic;
using System.Threading;
using CommonFinancial;
using CommonSupport;
using ForexPlatform;
using FXCore;

namespace FXCMAdapter
{
    /// <summary>
    /// Class manages order in FXCM integration.
    /// Based on contributions by "drginm".
    /// </summary>
    public class FXCMOrders : Operational, OrderExecutionSourceStub.IImplementation, IDisposable
    {
        FXCMAdapter _adapter;
        OrderExecutionSourceStub _stub;

        Dictionary<string, OrderInfo> _orders = new Dictionary<string, OrderInfo>();

        /// <summary>
        /// Needed to use when no position info returned, meaning all positions closed.
        /// </summary>
        Dictionary<string, PositionInfo> _positionsInfos = new Dictionary<string, PositionInfo>();

        FXCMConnectionManager Manager
        {
            get
            {
                FXCMAdapter adapter = _adapter;
                if (adapter == null)
                {
                    return null;
                }

                return adapter.Manager;
            }
        }

        Timer _updateTimer = null;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public FXCMOrders(FXCMAdapter adapter, OrderExecutionSourceStub stub)
        {
            _adapter = adapter;

            StatusSynchronizationEnabled = true;
            StatusSynchronizationSource = adapter;

            _stub = stub;
            _stub.Initialize(this);

        }

        /// <summary>
        /// 
        /// </summary>
        public bool Initialize()
        {
            FXCMConnectionManager manager = Manager;
            manager.AccountUpdatedEvent += new FXCMConnectionManager.ItemUpdateDelegate(Manager_AccountUpdateEvent);
            manager.OrderUpdatedEvent += new FXCMConnectionManager.OrderUpdateDelegate(Manager_OrderUpdatedEvent);
            //if (_updateTimer == null)
            //{
            //    _updateTimer = new Timer(TimerCallbackMethod, null, 1000, 1000);
            //}
            return true;
        }

        public void UnInitialize()
        {
            FXCMConnectionManager manager = Manager;
            manager.AccountUpdatedEvent -= new FXCMConnectionManager.ItemUpdateDelegate(Manager_AccountUpdateEvent);
            manager.OrderUpdatedEvent -= new FXCMConnectionManager.OrderUpdateDelegate(Manager_OrderUpdatedEvent);
            
            //Timer timer = _updateTimer;
            //if (timer != null)
            //{
            //    timer.Dispose();
            //}
        }

        public void Dispose()
        {
            _adapter = null;
            _stub = null;
        }

        /// <summary>
        /// 
        /// </summary>
        void TimerCallbackMethod(object state)
        {
            UpdateAccounts();
            UpdatePositions();
        }

        /// <summary>
        /// Event executed on a new thread.
        /// </summary>
        void Manager_OrderUpdatedEvent(FXCMConnectionManager manager, string accountId, OrderInfo orderInfo)
        {
            OrderExecutionSourceStub stub = _stub;
            if (stub == null)
            {
                SystemMonitor.OperationWarning(string.Format("Failed to update order information, since account [{0}] not found.", accountId));
                return;
            }
            
            AccountInfo? accountInfo = stub.GetAccountInfo(accountId);
            if (accountInfo.HasValue)
            {
                stub.UpdateOrderInfo(accountInfo.Value, Order.UpdateTypeEnum.Update, orderInfo);
            }
            else
            {
                SystemMonitor.OperationWarning(string.Format("Failed to find account [{0}] for order [{1}].", accountId, orderInfo.Id));
            }

            UpdatePositions();
        }

        /// <summary>
        /// Event executed on a new thread.
        /// </summary>
        void Manager_AccountUpdateEvent(FXCMConnectionManager manager, string accountId)
        {
            UpdateAccounts();
            UpdatePositions();
        }

        void UpdateAccounts()
        {
            OrderExecutionSourceStub stub = _stub;
            FXCMConnectionManager manager = Manager;
            if (stub == null || manager == null)
            {
                return;
            }

            // The newly produced accounts have no Guids.
            List<AccountInfo> accounts = manager.GetAvailableAccounts(IntegrationAdapter.AdvisedAccountDecimalsPrecision);
            foreach (AccountInfo info in accounts)
            {// Update / insert accounts.
                AccountInfo actualInfo = info;

                AccountInfo? existingInfo = stub.GetAccountInfo(info.Id);
                if (existingInfo.HasValue == false)
                {// New info, assign a Guid.
                    actualInfo.Guid = Guid.NewGuid();
                }
                else
                {
                    actualInfo.Guid = existingInfo.Value.Guid;
                }

                stub.UpdateAccountInfo(actualInfo);
            }
        }
        
        void UpdatePositions()
        {
            OrderExecutionSourceStub stub = _stub;
            FXCMConnectionManager manager = Manager;
            if (stub == null || manager == null)
            {
                return;
            }
            List<AccountInfo> accounts = stub.Accounts;
            if (accounts.Count == 1)
            {// Update all positions on the account.

                // When a position is not returned, it means no position on that symbol.
                List<PositionInfo> positionsInfosList = manager.GetPositions();
                
                Dictionary<string, PositionInfo> inputPositionsInfos = new Dictionary<string,PositionInfo>();

                lock(this)
                {
                    foreach(PositionInfo info in positionsInfosList)
                    {
                        inputPositionsInfos.Add(info.Symbol.Name, info);

                        // Update history with latest input.
                        _positionsInfos[info.Symbol.Name] = info;
                    }
                    
                    positionsInfosList.Clear();

                    // Check if any positions were not reported (meaning they have 0 value on them).
                    foreach (PositionInfo info in GeneralHelper.EnumerableToList<PositionInfo>(_positionsInfos.Values))
                    {
                        if (inputPositionsInfos.ContainsKey(info.Symbol.Name) == false 
                            && (info.Volume.HasValue == false || info.Volume != 0))
                        {// 0 position, update.
                            PositionInfo updatedInfo = info;
                            updatedInfo.Volume = 0;
                            _positionsInfos[updatedInfo.Symbol.Name] = updatedInfo;
                            positionsInfosList.Add(updatedInfo);
                        }
                        else
                        {
                            positionsInfosList.Add(info);
                        }

                    }
                }

                stub.UpdatePositionsInfo(accounts[0], positionsInfosList.ToArray());
            }
            else
            {
                SystemMonitor.OperationError("Failed to establish account for positions update.");
            }
        }

        #region IImplementation Members

        public AccountInfo? GetAccountInfoUpdate(AccountInfo accountInfo)
        {
            // Update the info in the stub.
            UpdateAccounts();

            // Let the stub handle this, it contains the latest info anyway.
            return null;
        }

        public AccountInfo[] GetAvailableAccounts()
        {
            // Update the info in the stub.
            UpdateAccounts();
            
            // Let the stub respond, since it now contains the latest info anyway.
            return null;
        }

        public bool GetOrdersInfos(AccountInfo accountInfo, List<string> ordersIds, 
            out OrderInfo[] ordersInfos, out string operationResultMessage)
        {
            operationResultMessage = string.Empty;

            List<OrderInfo> infos = new List<OrderInfo>();
            lock (this)
            {
                foreach (string id in ordersIds)
                {
                    if (_orders.ContainsKey(id))
                    {
                        infos.Add(_orders[id]);
                    }
                }
            }

            ordersInfos = infos.ToArray();
            return true;
        }

        /// <summary>
        /// Submit an order.
        /// </summary>
        public string SubmitOrder(AccountInfo account, Symbol symbol, OrderTypeEnum orderType, int volume,
            decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss,
            string comment, out string operationResultMessage)
        {
            OrderInfo? order;
            if (ExecuteMarketOrder(account, symbol, orderType, volume, allowedSlippage, desiredPrice, takeProfit,
                stopLoss, comment, out order, out operationResultMessage) == false || order.HasValue == false)
            {
                return string.Empty;
            }
            
            return order.Value.Id;
        }

        public bool ExecuteMarketOrder(AccountInfo accountInfo, Symbol symbol, OrderTypeEnum orderType, int volume,
            decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss,
            string comment, out OrderInfo? orderPlaced, out string operationResultMessage)
        {
            orderPlaced = null;

            FXCMConnectionManager manager = Manager;
            if (manager == null)
            {
                operationResultMessage = "Order system not ready to operate.";
                return false;
            }

            orderPlaced = manager.SubmitOrder(accountInfo, symbol, orderType, volume, allowedSlippage, desiredPrice,
                                    takeProfit, stopLoss, out operationResultMessage);


            if (orderPlaced.HasValue)
            {
                lock (this)
                {
                    _orders[orderPlaced.Value.Id] = orderPlaced.Value;
                }
                return true;
            }
            else
            {
                return false;
            }

            //operationResultMessage = string.Empty;
            //string operationResultMessageCopy = string.Empty;

            //bool isBuy = OrderInfo.TypeIsBuy(orderType);

            //OrderInfo? order = null;
            //GeneralHelper.GenericReturnDelegate<bool> operationDelegate = delegate()
            //{
            //    double realValue = (double)_adapter.GetInstrumentData(symbol.Name, "Ask");
            //    if (!isBuy)
            //    {
            //        realValue = (double)_adapter.GetInstrumentData(symbol.Name, "Bid");
            //    }

            //    try
            //    {
            //        order = CreateOrder(accountInfo,
            //                                symbol,
            //                                orderType,
            //                                volume,
            //                                allowedSlippage,
            //                                desiredPrice,
            //                                takeProfit,
            //                                stopLoss);
            //    }
            //    catch (Exception ex)
            //    {
            //        operationResultMessageCopy = ex.Message;
            //        return false;
            //    }

            //    return true;
            //};

            //orderPlaced = order;

            //object result;
            //if (_messageLoopOperator.Invoke(operationDelegate, TimeSpan.FromSeconds(120), out result) == false)
            //{// Timed out.
            //    operationResultMessage = "Timeout submiting order.";
            //    return false;
            //}

            //if (!(bool)result)
            //{// Operation error.
            //    operationResultMessage = operationResultMessageCopy;
            //    return false;
            //}

            //orderPlaced = order;

            //return true;
        }

        public bool ModifyOrder(AccountInfo accountInfo, string orderId, decimal? stopLoss, decimal? takeProfit, decimal? targetOpenPrice,
            out string modifiedId, out string operationResultMessage)
        {
            modifiedId = string.Empty;
            operationResultMessage = "The operation is not supported by this provider.";
            return false;
        }

        public bool DecreaseOrderVolume(AccountInfo accountInfo, string orderId, decimal volumeDecreasal, decimal? allowedSlippage,
            decimal? desiredPrice, out decimal decreasalPrice, out string modifiedId, out string operationResultMessage)
        {
            decreasalPrice = 0;
            modifiedId = string.Empty;
            operationResultMessage = "The operation is not supported by this provider.";
            return false;
        }

        public bool IncreaseOrderVolume(AccountInfo accountInfo, string orderId, decimal volumeIncrease, decimal? allowedSlippage,
            decimal? desiredPrice, out decimal increasalPrice, out string modifiedId, out string operationResultMessage)
        {
            increasalPrice = 0;
            modifiedId = string.Empty;
            operationResultMessage = "The operation is not supported by this provider.";
            return false;
        }

        public bool CloseOrCancelOrder(AccountInfo accountInfo, string orderId, string orderTag,
            decimal? allowedSlippage, decimal? desiredPrice, out decimal closingPrice,
            out DateTime closingTime, out string modifiedId, out string operationResultMessage)
        {
            closingPrice = 0;
            closingTime = DateTime.MinValue;
            modifiedId = string.Empty;
            operationResultMessage = "The operation is not supported by this provider.";
            return false;
        }

        public bool IsPermittedSymbol(AccountInfo accountInfo, Symbol symbol)
        {
            return _adapter.Data.Stub.IsSuggestedSymbol(symbol);
        }

        public int IsDataSourceSymbolCompatible(ComponentId dataSourceId, Symbol symbol)
        {
            FXCMAdapter adapter = _adapter;
            if (adapter != null 
                && adapter.DataSourceId.HasValue
                && adapter.DataSourceId.Value.Id == dataSourceId
                && adapter.Data.Stub.IsSuggestedSymbol(symbol))
            {
                return int.MaxValue;
            }
            else
            {
                return 0;
            }
        }



        #endregion
    }
}
