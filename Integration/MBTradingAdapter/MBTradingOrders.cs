using System;
using System.Collections.Generic;
using System.Threading;
using CommonFinancial;
using CommonSupport;
using ForexPlatform;
using MBTCOMLib;
using MBTORDERSLib;

namespace MBTradingAdapter
{
    /// <summary>
    /// Class manages order in MBTrading integration.
    /// </summary>
    public class MBTradingOrders : Operational, OrderExecutionSourceStub.IImplementation, IDisposable
    {
        MBTradingAdapter _adapter;

        MbtOrderClient _orderClient;

        MbtComMgr _commManager;

        MBTradingConnectionManager _manager;

        long _accountsLoadingCount = -1;
        /// <summary>
        /// 
        /// </summary>
        bool AllAccountsLoaded
        {
            get { return _accountsLoadingCount == 0; }
        }

        OperationPerformerStub _operationStub;

        enum TifEnum
        {
            VALUE_DAY = 10011,
            VALUE_DAYPLUS = 10009,
            VALUE_GTC = 10008,
            VALUE_IOC = 10010
        }

        enum MBTOrderTypeEnum : long
        {
            VALUE_DISCRETIONARY = 10043,
            VALUE_LIMIT = 10030,
            VALUE_LIMIT_CLOSE = 10057,
            VALUE_LIMIT_OPEN = 10056,
            VALUE_LIMIT_STOPMKT = 10064,
            VALUE_LIMIT_TRAIL = 10054,
            VALUE_LIMIT_TTO = 10050,
            VALUE_MARKET = 10031,
            VALUE_MARKET_CLOSE = 10039,
            VALUE_MARKET_OPEN = 10038,
            VALUE_MARKET_STOP = 10069,
            VALUE_MARKET_TRAIL = 10055,
            VALUE_MARKET_TTO = 10051,
            VALUE_PEGGED = 10062,
            VALUE_RESERVE = 10040,
            VALUE_RSV_DISC = 10044,
            VALUE_RSV_PEGGED = 10066,
            VALUE_RSV_TTO = 10052,
            VALUE_STOPLMT_STOP = 10072,
            VALUE_STOPLMT_TRAIL = 10068,
            VALUE_STOPLMT_TTO = 10067,
            VALUE_STOP_LIMIT = 10033,
            VALUE_STOP_MARKET = 10032,
            VALUE_STOP_TRAIL = 10065,
            VALUE_STOP_TTO = 10053,
            VALUE_TRAILING_STOP = 10034,
            VALUE_TTO_ORDER = 10037,
            VALUE_VWAP = 10063,
        }

        BackgroundMessageLoopOperator _messageLoopOperator;

        AccountInfo? _accountInfo = null;

        Timer _timer;

        /// <summary>
        /// Internal custom operation class.
        /// </summary>
        class PlaceOrderOperation : OperationInformation
        {
            public string ResultMessage;

            public OrderInfo? OrderResponse
            {
                get { return (OrderInfo?)base.Response; }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Complete(string resultMessage, OrderInfo? order)
            {
                ResultMessage = resultMessage;
                base.Complete(order);
            }
        }

        /// <summary>
        /// Internal custom operation class.
        /// </summary>
        class CancelOrderOperation : OperationInformation
        {
            public string ResultMessage;
            /// <summary>
            /// 
            /// </summary>
            public OrderInfo? OrderResponse
            {
                get { return (OrderInfo?)base.Response; }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Complete(string resultMessage, OrderInfo? order)
            {
                ResultMessage = resultMessage;
                base.Complete(order);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MBTradingOrders(BackgroundMessageLoopOperator messageLoopOperator)
        {
            _messageLoopOperator = messageLoopOperator;
            _operationStub = new OperationPerformerStub();

            ChangeOperationalState(OperationalStateEnum.Constructed);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Initialize(MBTradingAdapter adapter, MBTradingConnectionManager manager, MbtComMgr communicationManager)
        {
            SystemMonitor.CheckError(_messageLoopOperator.InvokeRequred == false, "Init must better be called on message loop method.");

            StatusSynchronizationEnabled = true;
            StatusSynchronizationSource = manager;

            try
            {
                _adapter = adapter;
                _commManager = communicationManager;
                _orderClient = _commManager.OrderClient;

                // *Never* subscribe to COM events of a property, always make sure to hold the object alive
                // http://www.codeproject.com/Messages/2189754/Re-Losing-COM-events-handler-in-Csharp-client.aspx

                _commManager.OnAlertAdded += new IMbtComMgrEvents_OnAlertAddedEventHandler(_commManager_OnAlertAdded);

                _manager = manager;

                _orderClient.SilentMode = true;

                _timer = new Timer(TimerUpdate, null, 10000, 1000);

                // *** ALWAYS CONSUME ORDERS EVENTS AND OPERATIONS TROUGH THE _orderClient VARIABLE, NEVER like this "_commManager.OrderClient"
                _orderClient.OnAccountLoaded += new _IMbtOrderClientEvents_OnAccountLoadedEventHandler(_ordersClient_OnAccountLoaded);
                _orderClient.OnBalanceUpdate += new _IMbtOrderClientEvents_OnBalanceUpdateEventHandler(_ordersClient_OnBalanceUpdate);
                _orderClient.OnLogonSucceed += new _IMbtOrderClientEvents_OnLogonSucceedEventHandler(_ordersClient_OnLogonSucceed);
                _orderClient.OnHistoryAdded += new _IMbtOrderClientEvents_OnHistoryAddedEventHandler(_orderClient_OnHistoryAdded);
                _orderClient.OnSubmit += new _IMbtOrderClientEvents_OnSubmitEventHandler(_orderClient_OnSubmit);
                _orderClient.OnAcknowledge += new _IMbtOrderClientEvents_OnAcknowledgeEventHandler(_orderClient_OnAcknowledge);
                _orderClient.OnPositionAdded += new _IMbtOrderClientEvents_OnPositionAddedEventHandler(_orderClient_OnPositionAdded);

                _orderClient.OnExecute += new _IMbtOrderClientEvents_OnExecuteEventHandler(_orderClient_OnExecute);
                _orderClient.OnReplacePlaced += new _IMbtOrderClientEvents_OnReplacePlacedEventHandler(_orderClient_OnReplacePlaced);

            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError("Failed to initialize", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {

            try
            {
                if (_orderClient != null)
                {
                    // An exception is generated here, upon trying to unsubscribe these events, so we shall leave them be.
                    // the wrapper class around the actual COM does something and releases too early ??

                    // ALWAYS CONSUME ORDERS EVENTS AND OPERATIONS TROUGH THE _orderClient VARIABLE, NEVER like this "_commManager.OrderClient"

                    _orderClient.OnAccountLoaded -= new _IMbtOrderClientEvents_OnAccountLoadedEventHandler(_ordersClient_OnAccountLoaded);
                    _orderClient.OnBalanceUpdate -= new _IMbtOrderClientEvents_OnBalanceUpdateEventHandler(_ordersClient_OnBalanceUpdate);
                    _orderClient.OnLogonSucceed -= new _IMbtOrderClientEvents_OnLogonSucceedEventHandler(_ordersClient_OnLogonSucceed);
                    _orderClient.OnHistoryAdded -= new _IMbtOrderClientEvents_OnHistoryAddedEventHandler(_orderClient_OnHistoryAdded);
                    _orderClient.OnSubmit -= new _IMbtOrderClientEvents_OnSubmitEventHandler(_orderClient_OnSubmit);
                    _orderClient.OnAcknowledge -= new _IMbtOrderClientEvents_OnAcknowledgeEventHandler(_orderClient_OnAcknowledge);
                    _orderClient.OnPositionAdded -= new _IMbtOrderClientEvents_OnPositionAddedEventHandler(_orderClient_OnPositionAdded);

                    _orderClient.OnExecute -= new _IMbtOrderClientEvents_OnExecuteEventHandler(_orderClient_OnExecute);
                    _orderClient.OnReplacePlaced -= new _IMbtOrderClientEvents_OnReplacePlacedEventHandler(_orderClient_OnReplacePlaced);

                    _orderClient = null;
                }

                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                if (_commManager != null)
                {
                    _commManager.OnAlertAdded -= new IMbtComMgrEvents_OnAlertAddedEventHandler(_commManager_OnAlertAdded);
                    _commManager = null;
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError(ex.Message);
            }
            finally
            {
                _orderClient = null;
                _timer = null;
                _commManager = null;
                _manager = null;
                _adapter = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ChangeOperationalState(OperationalStateEnum operationalState)
        {
            if (operationalState == OperationalStateEnum.Operational && AllAccountsLoaded == false)
            {// If we are still loading the accounts susped the change to operational for a while.
                base.ChangeOperationalState(OperationalStateEnum.Initializing);
            }
            else
            {
                base.ChangeOperationalState(operationalState);
            }
        }

        void TimerUpdate(object state)
        {
            if (_messageLoopOperator == null)
            {
                return;
            }

            _messageLoopOperator.BeginInvoke(delegate()
            {
                MbtOrderClient orderClient = _orderClient;
                if (orderClient == null || orderClient.Accounts == null)
                {
                    return;
                }

                foreach (MbtAccount account in orderClient.Accounts)
                {
                    PerformUpdate(account, true, true, true);
                }
            });
        }

        void _commManager_OnAlertAdded(MbtAlert pAlert)
        {
            TracerHelper.Trace("[" + pAlert.Severity.ToString() + "] " + pAlert.Message);
        }

        void _orderClient_OnReplacePlaced(MbtOpenOrder pOrd)
        {

        }

        void _orderClient_OnExecute(MbtOpenOrder pOrd)
        {
            if (pOrd.Acknowledged == false)
            {
                return;
            }

            //PlaceOrderOperation operation;
            //lock (this)
            //{
            //    // Operations stored in stub by order token / Id.
            //    operation = (PlaceOrderOperation)_operationStub.GetOperationById(pOrd.Token);
            //}

            OrderInfo? orderInfo = ConvertToOrderInfo(pOrd);

            if (orderInfo.HasValue == false)
            {
                SystemMonitor.Warning("Failed to convert order for order [" + pOrd.Token + "].");
                return;
            }

            TryCompleteOrderOperation(pOrd.Token, true, orderInfo);

            //if (operation == null)
            //{
            //    SystemMonitor.Error("Operation with this ID [" + pOrd.Token + "] not found.");
            //}
            //else
            //{
            //    //string x = orderInfo.Value.OpenPrice.ToString();

            //    if (orderInfo.HasValue == false)
            //    {
            //        SystemMonitor.Warning("Failed to convert order for order [" + pOrd.Token + "].");
            //        operation.Complete("Failed to convert order type. Operation failed.", null);
            //        return;
            //    }

            //    operation.Complete(string.Empty, orderInfo.Value);
            //}

            if (_accountInfo.HasValue == false)
            {
                SystemMonitor.Error("Order update received, but account not established.");
            }
            else
            {
                _adapter.OrderExecutionSourceStub.UpdateOrderInfo(_accountInfo.Value, Order.UpdateTypeEnum.Executed, orderInfo.Value);
            }
        }

        void _orderClient_OnPositionAdded(MbtPosition pPos)
        {

        }

        void _orderClient_OnAcknowledge(MbtOpenOrder pOrd)
        {
            if (pOrd.Acknowledged == false)
            {
                return;
            }

            OrderInfo? orderInfo = ConvertToOrderInfo(pOrd);

            //if (orderInfo.HasValue && orderInfo.Value.State == OrderStateEnum.Canceled)
            //{// There might be a pending operation on this order.
            //    TryCompleteOrderOperation(pOrd.Token, false, orderInfo);
            //}

            if (_accountInfo.HasValue == false)
            {
                SystemMonitor.Error("Order update received, but account not established.");
            }
            else
            {
                _adapter.OrderExecutionSourceStub.UpdateOrderInfo(_accountInfo.Value, Order.UpdateTypeEnum.Submitted, orderInfo.Value);
            }

        }

        void _orderClient_OnSubmit(MbtOpenOrder pOrd)
        {

        }

        void _orderClient_OnHistoryAdded(MbtOrderHistory pHist)
        {
            Order.UpdateTypeEnum update;
            OrderInfo? info = ConvertToOrderInfo(pHist, out update);

            if (info.HasValue && info.Value.State == OrderStateEnum.Canceled)
            {
                TryCompleteOrderOperation(pHist.Token, false, info);
            }

            if (info.HasValue)
            {
                this._adapter.OrderExecutionSourceStub.UpdateOrderInfo(_accountInfo.Value, update, info.Value);
            }
            else
            {
                SystemMonitor.OperationWarning("Order history update skipped [" + pHist.OrderNumber + "/" + pHist.Token + " - " + pHist.Event + ", " + pHist.Message + "].", TracerItem.PriorityEnum.Low);
            }
        }

        void _ordersClient_OnLogonSucceed()
        {
            _accountsLoadingCount = _orderClient.Accounts.Count;
            _orderClient.Accounts.LoadAll();

            //foreach (MbtMATGroup group in _commManager.MATGroups)
            //{
            //    foreach (MbtMATAccount account in group)
            //    {
            //    }
            //}
        }

        void _ordersClient_OnBalanceUpdate(MbtAccount pAcct)
        {
            if (_accountInfo.HasValue && pAcct.Account == _accountInfo.Value.Id
                && pAcct.RoutingID == _accountInfo.Value.Name)
            {
                PerformUpdate(pAcct, true, false, false);
            }
        }

        void _orders_OnLogonSucceed()
        {
        }

        void _ordersClient_OnAccountLoaded(MbtAccount pAcct)
        {
            _accountsLoadingCount--;
            if (_accountsLoadingCount == 0)
            {// All accountInfos loaded.
                PerformUpdate(pAcct, true, true, true);

                if (OperationalState != OperationalStateEnum.Operational &&
                    _manager.OperationalState == OperationalStateEnum.Operational)
                {// We are ready to follow the manager to operational state now.
                    ChangeOperationalState(OperationalStateEnum.Operational);
                }
            }
        }

        /// <summary>
        /// Helper.
        /// </summary>
        void TryCompleteOrderOperation(string token, bool printWarning, OrderInfo? orderInfo)
        {
            PlaceOrderOperation operation;
            lock (this)
            {
                // Operations stored in stub by order token / Id.
                operation = (PlaceOrderOperation)_operationStub.GetOperationById(token);
            }

            if (operation == null)
            {
                if (printWarning)
                {
                    SystemMonitor.Error("Operation with this ID [" + token + "] not found.");
                }
                return;
            }

            if (orderInfo.HasValue == false)
            {
                if (printWarning)
                {
                    SystemMonitor.Warning("Failed to convert order for order [" + token + "].");
                }
                operation.Complete("Failed to convert order type. Operation failed.", null);

                return;
            }

            operation.Complete(string.Empty, orderInfo.Value);
        }

        /// <summary>
        /// Helper, allows to extract account info from the MBT account structure.
        /// </summary>
        void ConvertAccount(MbtAccount account, decimal openPnL, ref AccountInfo info)
        {
            // All account parameters.
            //double credit = account.Credit;
            //double currentBP = account.CurrentBP;
            //double currentEquity = account.CurrentEquity;
            //double c1 = account.CurrentExcess;
            //double c2 = account.CurrentOvernightBP;
            //double c3 = account.DailyRealizedPL;
            //double c4 = account.MMRUsed;
            //double c5 = account.MMRMultiplier;
            //double c6 = account.MorningBP;
            //double c7 = account.MorningCash;
            //double c8 = account.MorningEquity;
            //double c9 = account.MorningExcess;
            //double c10 = account.MornOvernightBP;
            //double c11 = account.OvernightExcess;


            info.Name = account.RoutingID;
            info.Id = account.Account;
            info.Server = account.Branch;
            info.Leverage = (decimal)account.MMRMultiplier;
            info.BaseCurrency = new Symbol(Symbol.SymbolGroup.Forex, account.BaseCurrency);
            info.Credit = (decimal)Math.Round(account.Credit, IntegrationAdapter.AdvisedAccountDecimalsPrecision);
            info.Equity = (decimal)account.CurrentEquity + (decimal)Math.Round(openPnL, IntegrationAdapter.AdvisedAccountDecimalsPrecision);
            info.Balance = (decimal)Math.Round((decimal)account.CurrentEquity, IntegrationAdapter.AdvisedAccountDecimalsPrecision);
            info.Profit = (decimal)Math.Round(account.DailyRealizedPL, IntegrationAdapter.AdvisedAccountDecimalsPrecision);
            info.Margin = null;
        }

        /// <summary>
        /// This will work properly only for USD *BASED ACCOUNTS*.
        /// Code based on the forexOpenPnL.txt VB6 code file from the SDK.
        /// See here for details: http://finance.groups.yahoo.com/group/mbtsdk/message/6120
        /// </summary>
        /// <returns></returns>
        bool CalculatePositionOpenPnLAndBasis(MbtPosition position, Quote? positionSymbolQuote, out double openPnL, out double basis)
        {
            openPnL = 0;
            basis = 0;

            if (positionSymbolQuote.HasValue == false ||
                positionSymbolQuote.Value.Ask.HasValue == false || positionSymbolQuote.Value.Bid.HasValue == false)
            {
                return false;
            }

            long aggregatedPosition = position.AggregatePosition;
            if (aggregatedPosition == 0)
            {
                return true;
            }
            
            // This also assumes aggregatedPosition is not zero!
            basis = ((position.OvernightPrice * position.OvernightPosition) + (position.IntradayPrice * position.IntradayPosition)) / aggregatedPosition;
            Symbol? symbol = Symbol.CreateForexPairSymbol(position.Symbol);

            if (symbol.HasValue == false)
            {
                return false;
            }

            bool isLong = aggregatedPosition > 0;

            double ask = (double)positionSymbolQuote.Value.Ask.Value;
            double bid = (double)positionSymbolQuote.Value.Bid.Value;

            if (symbol.Value.ForexCurrency1 == "USD")
            {// Based C1.
                if (isLong)
                {// Long. long position - use Bid ' ((bid - basis) / bid) * qty
                    openPnL = ((bid - basis) / bid) * aggregatedPosition;
                    return true;
                }
                else
                {// Short. short position - use Ask ' ((ask - basis) / ask) * qty
                    openPnL = ((ask - basis) / ask) * aggregatedPosition;
                    return true;
                }
            }
            else if (symbol.Value.ForexCurrency2 == "USD")
            {// Based C2.
                if (isLong)
                {// Long. ' long position - use Bid ' (bid - basis) * qty
                    openPnL = (bid - basis) * aggregatedPosition;
                    return true;
                }
                else
                {// Short. ' short position - use Ask ' (ask - basis) * qty
            		openPnL = (ask - basis) * aggregatedPosition;
                    return true;
                }
            }
            else
            {// Not based.

                // This is the code that covers this scenario (from the forexOpenPnL.txt from the SDK), 
                // however it requires the usage of other pairs data... so not implemented right now.
                //SystemMonitor.OperationWarning(string.Format("Position PnL not calculated for symbol [{0}] since pair not USD based.", position.Symbol));

                return false;

                //' Neither C1 or C2 is our base. therefore, find a "related symbol" for
                //' C2 that will relate C2 back to our base "USD", and use the related
                //' symbol's Bid/Ask as part of the calculation. Do this by creating a
                //' temp variable "f" with the value of the Bid/Ask, inverted if necessary.
                //Dim f As Double
                //    f = 0
                //    For j = 0 To mlSymCnt
                //' Find C1 base (e.g. EUR/CHF produces USD/CHF)
                //        If Left(msSyms(j), 3) = "USD" And Right(s, 3) = Right(msSyms(j), 3) Then
                //' use Ask for short, Bid for long, and invert
                //            f = 1 / (IIf(aggregatedPosition < 0, mdCAsk(j), mdCBid(j)))
                //'			dOpenPnL = (IIf(aggregatedPosition < 0, a, b) - dBasis) * f * aggregatedPosition
                //            Exit For ' found it !
                //        End If
                //    Next
                //    If f = 0 Then
                //' If C1 base not found, find C2 base (e.g. EUR/GBP produces GBP/USD)
                //        For j = 0 To mlSymCnt
                //            If Right(msSyms(j), 3) = "USD" And Right(s, 3) = Left(msSyms(j), 3) Then
                //' use Ask for short, Bid for long, but don't invert
                //                f = IIf(aggregatedPosition < 0, mdCAsk(j), mdCBid(j))
                //'				dOpenPnL = (IIf(aggregatedPosition < 0, a, b) - dBasis) * f * aggregatedPosition
                //                Exit For ' found it !
                //            End If
                //        Next
                //    End If
                //    If f = 0 Then ' for some reason, none found (you should find out why) !
                //        Debug.Print "ERROR"
                //    Else
                //' (bidOrAsk - basis) * f * qty
                //' f contains the Bid/Ask of the related symbol, inverted if necessary
                //        dOpenPnL = (IIf(aggregatedPosition < 0, a, b) - dBasis) * f * aggregatedPosition
                //    End If
                //End If

            }
        }

        /// <summary>
        /// Make sure to call on Invoke'd thread.
        /// </summary>
        /// <param name="account"></param>
        void PerformUpdate(MbtAccount account, bool updateAccount, bool updateOrders, bool updatePositions)
        {
            //SystemMonitor.CheckError(_messageLoopOperator.InvokeRequred == false, "Invoke must not be required in baseMethod call.");

            MbtOrderClient orderClient = _orderClient;
            MBTradingConnectionManager manager = _manager;
            MBTradingQuote quotes = null;

            if (orderClient == null || manager == null)
            {
                return;
            }

            quotes = manager.Quotes;
            if (quotes == null)
            {
                return;
            }

            AccountInfo info;
            Dictionary<string, OrderInfo> pendingOrderInfos = new Dictionary<string, OrderInfo>();
            List<PositionInfo> positionsInfos = new List<PositionInfo>();
            lock (this)
            {
                if (updateOrders)
                {
                    // We need to send up only the latest of each orders histories, since prev ones are for previous states.
                    foreach (MbtOrderHistory history in _orderClient.OrderHistories)
                    {
                        Order.UpdateTypeEnum updateType;
                        OrderInfo? orderInfo = ConvertToOrderInfo(history, out updateType);
                        if (orderInfo.HasValue)
                        {
                            pendingOrderInfos[orderInfo.Value.Id] = orderInfo.Value;
                        }
                    }

                    // Make sure open orders orderInfo is always on top.
                    foreach (MbtOpenOrder pOrd in _orderClient.OpenOrders)
                    {
                        OrderInfo? orderInfo = ConvertToOrderInfo(pOrd);
                        if (orderInfo.HasValue)
                        {
                            pendingOrderInfos[orderInfo.Value.Id] = orderInfo.Value;
                        }
                    }
                }

                decimal openPositionsPnL = 0;
                if (updatePositions)
                {
                    foreach (MbtPosition position in _orderClient.Positions)
                    {
                        PositionInfo? positionInfo = ConvertToPositionInfo(position, quotes);
                        if (positionInfo.HasValue)
                        {
                            openPositionsPnL += positionInfo.Value.Result.HasValue ? positionInfo.Value.Result.Value : 0;
                            positionsInfos.Add(positionInfo.Value);
                        }
                    }
                }

                if (_accountInfo.HasValue)
                {
                    info = _accountInfo.Value;
                }
                else
                {
                    info = new AccountInfo();
                    info.Guid = Guid.NewGuid();
                }

                ConvertAccount(account, openPositionsPnL, ref info);

                _accountInfo = info;
            }

            MBTradingAdapter adapter = _adapter;
            if (adapter != null)
            {
                OrderExecutionSourceStub stub = adapter.OrderExecutionSourceStub;
                if (stub != null)
                {
                    if (updateAccount)
                    {
                        stub.UpdateAccountInfo(_accountInfo.Value);
                    }

                    if (updateOrders)
                    {
                        stub.UpdateOrdersInfo(_accountInfo.Value,
                            GeneralHelper.GenerateSingleValueArray<Order.UpdateTypeEnum>(pendingOrderInfos.Count, Order.UpdateTypeEnum.Update),
                             GeneralHelper.EnumerableToArray<OrderInfo>(pendingOrderInfos.Values));
                    }

                    if (updatePositions)
                    {
                        stub.UpdatePositionsInfo(_accountInfo.Value, positionsInfos.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Helper, get account based on name from orderInfo.
        /// </summary>
        MbtAccount GetAccountByInfo(AccountInfo info)
        {
            foreach (MbtAccount account in _orderClient.Accounts)
            {
                if (account.Account == info.Id && account.RoutingID == info.Name)
                {
                    return account;
                }
            }

            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            UnInitialize();
            _messageLoopOperator = null;
        }

        #endregion

        #region IImplementation Members

        AccountInfo? OrderExecutionSourceStub.IImplementation.GetAccountInfoUpdate(AccountInfo accountInfo)
        {
            lock (this)
            {
                return _accountInfo;
            }
        }

        bool OrderExecutionSourceStub.IImplementation.GetOrdersInfos(AccountInfo accountInfo, List<string> ordersIds, out OrderInfo[] ordersInfos, out string operationResultMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        bool ConvertToMBTOrderType(OrderTypeEnum orderType, decimal? desiredPrice, decimal? allowedSlippage, out int type, out int buySell, out int lTimeInForce)
        {
            type = 0;
            buySell = 0;

            // By default set it up for Market/Limit type.
            type = (int)MBTOrderTypeEnum.VALUE_MARKET;
            lTimeInForce = (int)TifEnum.VALUE_GTC;

            if (desiredPrice.HasValue && allowedSlippage.HasValue)
            {// In order to place Market order with slippage, we use Limit order (same thing).
                lTimeInForce = (int)TifEnum.VALUE_IOC; // Immediate or cancel.
                type = (int)MBTOrderTypeEnum.VALUE_LIMIT;
            }

            switch (orderType)
            {
                case OrderTypeEnum.BUY_MARKET:
                    // Limit orders allow to assign limit execution price (aka slippage).
                    //type = (int)MBTOrderTypeEnum.VALUE_LIMIT;
                    buySell = 10000;
                    break;
                case OrderTypeEnum.SELL_MARKET:
                    // Limit orders allow to assign limit execution price (aka slippage).
                    //type = (int)MBTOrderTypeEnum.VALUE_LIMIT;
                    buySell = 10001;
                    break;
         
                case OrderTypeEnum.BUY_LIMIT_MARKET:
                    type = (int)MBTOrderTypeEnum.VALUE_STOP_MARKET;
                    buySell = 10000;
                    break;
                case OrderTypeEnum.SELL_LIMIT_MARKET:
                    type = (int)MBTOrderTypeEnum.VALUE_STOP_MARKET;
                    buySell = 10001;
                    break;
                case OrderTypeEnum.BUY_STOP_MARKET:
                    type = (int)MBTOrderTypeEnum.VALUE_STOP_MARKET;
                    buySell = 10000;
                    break;
                case OrderTypeEnum.SELL_STOP_MARKET:
                    type = (int)MBTOrderTypeEnum.VALUE_STOP_MARKET;
                    buySell = 10001;
                    break;

                case OrderTypeEnum.UNKNOWN:
                default:
                    SystemMonitor.NotImplementedWarning();
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        OrderTypeEnum ConvertFromMBTOrderType(int type, int buySell)
        {
            if (type == (int)MBTOrderTypeEnum.VALUE_MARKET
                || type == 0)
            {
                //' Buy/Sell action values
                //Public Const VALUE_BUY = 10000
                //Public Const VALUE_SELL = 10001
                //Public Const VALUE_SELLSHT = 10002

                switch (buySell)
                {
                    case 10000:
                        return OrderTypeEnum.BUY_MARKET;
                    case 10001: 
                    case 10002: 
                        return OrderTypeEnum.SELL_MARKET;
                }
            }

            if (type == (int)MBTOrderTypeEnum.VALUE_LIMIT)
            {
                switch (buySell)
                {
                    case 10000:
                        return OrderTypeEnum.BUY_LIMIT_MARKET;
                    case 10001:
                    case 10002:
                        return OrderTypeEnum.SELL_LIMIT_MARKET;
                }
            }

            if (type == (int)MBTOrderTypeEnum.VALUE_STOP_MARKET)
            {
                switch (buySell)
                {
                    case 10000:
                        return OrderTypeEnum.BUY_STOP_MARKET;
                    case 10001:
                    case 10002:
                        return OrderTypeEnum.SELL_STOP_MARKET;
                }
            }

            return OrderTypeEnum.UNKNOWN;
        }

        DateTime ConvertDateTime(string dateTimeString)
        {
            DateTime dateTime;
            if (DateTime.TryParse(dateTimeString, out dateTime) == false)
            {
                SystemMonitor.OperationError("Failed to parse order date time [" + dateTimeString + "].");
                dateTime = DateTime.Now;
            }
            return dateTime;
        }

        Symbol? TryObtainSymbol(string symbolName)
        {
            Symbol? symbol = _adapter.GetSymbolByName(symbolName, true);
            
            if (symbol.HasValue == false)
            {
                return null;
            }

            return symbol;
        }

        /// <summary>
        /// Convert history to order orderInfo.
        /// </summary>
        /// <param name="OrderInfo"></param>
        /// <returns></returns>
        OrderInfo? ConvertToOrderInfo(MbtOrderHistory pHist, out Order.UpdateTypeEnum updateType)
        {
            updateType = Order.UpdateTypeEnum.Update;

            if (string.IsNullOrEmpty(pHist.Event))
            {
                return null;
            }

            OrderInfo result = new OrderInfo(pHist.Token);

            string eventInfo = pHist.Event.ToLower();

            if (eventInfo.Contains("enter") || (eventInfo.Contains("session") && eventInfo.Contains("open")))
            {// "Enter", "Session open" events skipped.
                return null;
            }

            if (eventInfo.Contains("live"))
            {
                result.State = OrderStateEnum.Submitted;
            }
            else if (eventInfo.Contains("executed"))
            {
                result.State = OrderStateEnum.Executed;
            }
            else if (eventInfo.Contains("suspended"))
            {
                result.State = OrderStateEnum.Suspended;
            }
            else if (eventInfo == "cancel"
                    || (eventInfo.Contains("cancel") && eventInfo.Contains("reject"))
                    || (eventInfo.Contains("order") && eventInfo.Contains("reject"))
                    || (eventInfo.Contains("order") && eventInfo.Contains("cancel")))
            {// Order Reject or Cancel.
                result.State = OrderStateEnum.Canceled;
                updateType = Order.UpdateTypeEnum.Canceled;
            }
            else if (eventInfo.Contains("suspended"))
            {// Suspended
                result.State = OrderStateEnum.Suspended;
                updateType = Order.UpdateTypeEnum.Modified;
            }

            string eventMessage = pHist.Message;

            Symbol? symbol = TryObtainSymbol(pHist.Symbol);
            if (symbol.HasValue == false)
            {
                return null;
            }

            result.Tag = pHist.OrderNumber;
            result.OpenPrice = (decimal)pHist.Price;
            result.StopLoss = (decimal)pHist.StopLimit;
            result.TakeProfit = (decimal)pHist.StopLimit;
            result.OpenTime = ConvertDateTime(pHist.Date + " " + pHist.Time);
            result.Volume = pHist.Quantity;

            result.Symbol = symbol.Value;

            result.Type = ConvertFromMBTOrderType(pHist.OrderType, pHist.BuySell);
            if (result.Type == OrderTypeEnum.UNKNOWN)
            {
                SystemMonitor.OperationWarning("Failed to recognize order type [" + pHist.OrderType.ToString() + "].");
                //return null;
            }

            ApplyOrderInfoCommission(ref result);

            return result;
        }

        /// <summary>
        /// Helper, converts from MBT to OFxP object.
        /// </summary>
        PositionInfo? ConvertToPositionInfo(MbtPosition position, MBTradingQuote quotes)
        {
            PositionInfo result = new PositionInfo();

            Symbol? symbol = _adapter.GetSymbolByName(position.Symbol, true);
            if (symbol.HasValue == false)
            {
                return null;
            }

            result.Symbol = symbol.Value;
            result.Volume = position.AggregatePosition;/*position.CloseableShares;*/
            result.Commission = (decimal)Math.Round(position.Commission, IntegrationAdapter.AdvisedAccountDecimalsPrecision);
            result.PendingBuyVolume = position.PendingBuyShares;
            result.PendingSellVolume = position.PendingSellShares;
            result.MarketValue = 0;
            result.ClosedResult = (decimal)Math.Round(position.RealizedPNL, 4);

            if (quotes.SessionsQuotes.ContainsKey(symbol.Value.Name))
            {
                double openPnL, basis;
                if (CalculatePositionOpenPnLAndBasis(position, quotes.SessionsQuotes[symbol.Value.Name].Quote, out openPnL, out basis))
                {
                    result.Result = (decimal)Math.Round(openPnL, IntegrationAdapter.AdvisedAccountDecimalsPrecision);
                    result.Basis = (decimal)basis;
                }
            }

            return result;
        }

        void ApplyOrderInfoCommission(ref OrderInfo info)
        {
            MBTradingConnectionManager manager = _manager;
            if (manager != null && manager.Adapter != null && manager.Adapter.ApplyOrderCommission)
            {// Finally, calculate commission when all other parameters of the order are established.
                Commission? orderCommission = Commission.GenerateSymbolCommission(manager.Adapter, info.Symbol);
                if (orderCommission.HasValue)
                {
                    orderCommission.Value.ApplyCommissions(manager.Adapter.CommissionPrecisionDecimals, ref info);
                }
                else
                {// Failed to establish commission for this value.
                    SystemMonitor.OperationWarning(string.Format("Failed to establish commission for order [{0}].", info.Symbol));
                }
            }
        }

        /// <summary>
        /// Convert real order to order orderInfo.
        /// </summary>
        /// <param name="pOrd"></param>
        /// <returns></returns>
        OrderInfo? ConvertToOrderInfo(MbtOpenOrder pOrd)
        {
            OrderInfo result = new OrderInfo(pOrd.Token);
            result.Tag = pOrd.OrderNumber;

            result.OpenPrice = (decimal)pOrd.Price;

            bool replaceable = pOrd.Replaceable;

            string dateTimeString = pOrd.Date + " " + pOrd.Time;

            DateTime dateTime;
            if (DateTime.TryParse(dateTimeString, out dateTime) == false)
            {
                SystemMonitor.Error("Failed to parse order date time [" + dateTimeString + "].");
                dateTime = DateTime.Now;
            }

            string currentEvent = pOrd.CurrentEvent.ToLower();
            if (currentEvent.Contains("live"))
            {
                result.State = OrderStateEnum.Submitted;
            }
            else if (currentEvent.Contains("executed"))
            {
                result.State = OrderStateEnum.Executed;
            }
            else if (currentEvent.Contains("suspended"))
            {
                result.State = OrderStateEnum.Suspended;
            }
            else
            {
                if (pOrd.Acknowledged)
                {
                    result.State = OrderStateEnum.Submitted;
                }
            }

            if (result.State == OrderStateEnum.Unknown)
            {
            }

            result.OpenTime = dateTime;

            result.StopLoss = (decimal)pOrd.StopLimit;
            result.TakeProfit = (decimal)pOrd.StopLimit;

            Symbol? symbol = _adapter.GetSymbolByName(pOrd.Symbol, false);
            if (symbol.HasValue == false)
            {// Start usage for this symbol.
                return null;
            }

            result.Symbol = symbol.Value;
            result.Type = ConvertFromMBTOrderType(pOrd.OrderType, pOrd.BuySell);

            if (result.Type == OrderTypeEnum.UNKNOWN)
            {
                SystemMonitor.OperationWarning("Failed to recognize order type [" + pOrd.OrderType.ToString()  + "].");
                //return null;
            }

            result.Volume = pOrd.Quantity;

            ApplyOrderInfoCommission(ref result);

            return result;
        }

        /// <summary>
        /// Submits the order over to the orders interface, make sure to call in Invocation thread.
        /// </summary>
        string DoSubmitOrder(AccountInfo accountInfo, Symbol symbol, OrderTypeEnum orderType, int volume, Decimal? allowedSlippage, Decimal? desiredPrice,
            Decimal? takeProfit, Decimal? stopLoss, string comment, out PlaceOrderOperation operation, out string operationResultMessage)
        {
            SystemMonitor.CheckError(_messageLoopOperator.InvokeRequred == false, "Invoke required.");

            operationResultMessage = "Operation not supported.";
            operation = null;

            MbtAccount pAcct = GetAccountByInfo(accountInfo);

            if (pAcct == null)
            {
                operationResultMessage = "Failed to retrieve account.";
                SystemMonitor.OperationWarning(operationResultMessage);
                return null;
            }

            if (orderType != OrderTypeEnum.SELL_MARKET && orderType != OrderTypeEnum.BUY_MARKET)
            {
                operationResultMessage = "Order type [" + orderType.ToString() + "] not supported or tested by this provider.";
                return null;
                //if (desiredPrice.HasValue)
                //{
                //    dStopPrice = (double)desiredPrice.Value;
                //}
                //else
                //{
                //    SystemMonitor.Error("Desired price not assigned, on placing order type [" + orderType.ToString() + " ], not submitted.");
                //    return null;
                //}
            }

            // ---
            int iVolume = volume;

            int iOrdType, iBuySell;
            
            double dPrice = desiredPrice.HasValue ? (double)desiredPrice.Value : 0;
            double dPrice2 = 0;
            int lTimeInForce = -1;

            if (ConvertToMBTOrderType(orderType, desiredPrice, allowedSlippage, out iOrdType, out iBuySell, out lTimeInForce) == false)
            {
                operationResultMessage = "Failed to convert type of order.";
                SystemMonitor.OperationWarning(operationResultMessage);
                return null;
            }

            if (allowedSlippage.HasValue && dPrice != 0)
            {// Put the slippage in as a limit price.
                // This forms the "limit" price we are willing to pay for this order.
                if (OrderInfo.TypeIsBuy(orderType))
                {
                    dPrice = dPrice + (double)allowedSlippage.Value;
                }
                else
                {
                    dPrice = dPrice - (double)allowedSlippage.Value;
                }
            }

            string message = string.Empty;

            lock (this)
            {// Make sure to keep the entire package here locked, since the order operation get placed after the submit
                // so we need to make sure we shall catch the responce in OnSubmit() too.

                //if (_orderClient.Submit(iBuySell, iVolume, symbol.Name, dPrice, dStopPrice, (int)TifEnum.VALUE_GTC, 10020, iOrdType,
                //    10042, 0, pAcct, "MBTX", string.Empty, 0, 0, DateTime.FromBinary(0), DateTime.FromBinary(0), 0, 0, 0, 0, -1, ref message) == false)
                //{// Error requestMessage.
                //    operationResultMessage = message;
                //    return null;
                //}

                // Instead of using Market Orders, we shall use Limit Orders, since they allow to set an execution limit price.
                // The VALUE_IOC instructs to execute or cancel the order, instead of GTC (Good Till Cancel)
                if (_orderClient.Submit(iBuySell, iVolume, symbol.Name, dPrice, dPrice2, lTimeInForce, 10020, iOrdType,
                    10042, 0, pAcct, "MBTX", string.Empty, 0, 0, DateTime.FromBinary(0), DateTime.FromBinary(0), 0, 0, 0, 0, -1, ref message) == false)
                {// Error requestMessage.
                    operationResultMessage = message;
                    return null;
                }

                operation = new PlaceOrderOperation() { Id = message };
                // The message, or operation Id is the order token (further stored in OrderInfo.id)
                _operationStub.RegisterOperation(operation, false);
            }

            return message;
        }

        bool OrderExecutionSourceStub.IImplementation.ExecuteMarketOrder(AccountInfo accountInfo, Symbol symbol, OrderTypeEnum orderType, int volume, Decimal? allowedSlippage, Decimal? desiredPrice,
            Decimal? takeProfit, Decimal? stopLoss, string comment, out OrderInfo? orderPlaced, out string operationResultMessage)
        {
            string operationResultMessageCopy = string.Empty;

            PlaceOrderOperation operation = null;
            GeneralHelper.GenericReturnDelegate<bool> operationDelegate = delegate() 
            {
                string submitResult = DoSubmitOrder(accountInfo, symbol, orderType, volume, allowedSlippage, desiredPrice,
                    takeProfit, stopLoss, comment, out operation, out operationResultMessageCopy);

                return string.IsNullOrEmpty(submitResult) == false;
            };

            orderPlaced = null;

            object result;
            if (_messageLoopOperator.Invoke(operationDelegate, TimeSpan.FromSeconds(60), out result) == false)
            {// Timed out.
                operationResultMessage = "Timeout placing order.";
                return false;
            }

            if ((bool)result == false)
            {// Operation error.
                operationResultMessage = operationResultMessageCopy;
                return false;
            }

            object operationResult;
            if (operation.WaitResult<object>(TimeSpan.FromSeconds(60), out operationResult) == false)
            {
                operationResultMessage = "Order place timeout.";
                return false;
            }

            orderPlaced = (OrderInfo?)operationResult;

            if (operationResult == null || orderPlaced.HasValue == false)
            {
                operationResultMessage = "Order place failed.";
                return false;
            }

            // Operation OK.
            operationResultMessage = string.Empty;
            orderPlaced = operation.OrderResponse;

            if (orderPlaced.HasValue == false)
            {
                return false;
            }

            return true;
        }

        bool OrderExecutionSourceStub.IImplementation.ModifyOrder(AccountInfo accountInfo, string orderId, decimal? stopLoss, decimal? takeProfit, decimal? targetOpenPrice, out string modifiedId, out string operationResultMessage)
        {
            modifiedId = string.Empty;
            operationResultMessage = "Operation not supported.";
            return false;
        }

        bool OrderExecutionSourceStub.IImplementation.DecreaseOrderVolume(AccountInfo accountInfo, string orderId, decimal volumeDecreasal, decimal? allowedSlippage, decimal? desiredPrice, out decimal decreasalPrice, out string modifiedId, out string operationResultMessage)
        {
            decreasalPrice = 0;
            modifiedId = string.Empty;
            operationResultMessage = "Operation not supported.";
            return false;
        }

        bool OrderExecutionSourceStub.IImplementation.IncreaseOrderVolume(AccountInfo accountInfo, string orderId, decimal volumeIncrease, decimal? allowedSlippage, decimal? desiredPrice, out decimal increasalPrice, out string modifiedId, out string operationResultMessage)
        {
            increasalPrice = 0;
            modifiedId = string.Empty;
            operationResultMessage = "Operation not supported.";
            return false;
        }

        bool OrderExecutionSourceStub.IImplementation.CloseOrCancelOrder(AccountInfo accountInfo, string orderId, 
            string orderTag, decimal? allowedSlippage, decimal? desiredPrice, out decimal closingPrice, 
            out DateTime closingTime, out string modifiedId, out string operationResultMessage)
        {
            operationResultMessage = string.Empty;

            closingPrice = 0;
            closingTime = DateTime.MinValue;
            modifiedId = orderId;

            string operationResultMessageLocal = string.Empty;

            OperationInformation operation = null;
            GeneralHelper.GenericReturnDelegate<string> operationDelegate = delegate()
            {
                operationResultMessageLocal = "Operation not supported.";
                operation = null;

                MbtAccount pAcct = GetAccountByInfo(accountInfo);

                if (pAcct == null)
                {
                    operationResultMessageLocal = "Failed to retrieve account.";
                    SystemMonitor.OperationWarning(operationResultMessageLocal);
                    return null;
                }

                string message = string.Empty;
                lock (this)
                {// Make sure to keep the entire package here locked, since the order operation get placed after the submit
                    // so we need to make sure we shall catch the responce in OnSubmit() too.
                    if (_orderClient.Cancel(orderTag, ref message) == false)
                    {// Error requestMessage.
                        operationResultMessageLocal = message;
                        return null;
                    }

                    operation = new CancelOrderOperation() { Id = message };
                    // The message, or operation Id is the order token (further stored in OrderInfo.id)
                    _operationStub.RegisterOperation(operation, false);
                }

                return message;
            };

            object result;
            if (_messageLoopOperator.Invoke(operationDelegate, TimeSpan.FromSeconds(60), out result) == false)
            {// Timed out.
                operationResultMessage = "Timeout submiting order cancelation.";
                return false;
            }

            if (string.IsNullOrEmpty((string)result))
            {// Operation error.
                operationResultMessage = operationResultMessageLocal;
                return false;
            }

            // Return the ID of the submitted order.
            return true;
        }

        int OrderExecutionSourceStub.IImplementation.IsDataSourceSymbolCompatible(ComponentId dataSourceId, Symbol symbol)
        {
            if (_adapter != null && _adapter.DataSourceId.HasValue)
            {
                if (_adapter.DataSourceId.Value.Id == dataSourceId)
                {
                    return int.MaxValue;
                }
            }

            return 0;
        }

        bool OrderExecutionSourceStub.IImplementation.IsPermittedSymbol(AccountInfo accountInfo, Symbol symbol)
        {
            GeneralHelper.GenericReturnDelegate<bool> operation = delegate()
            {
                MbtAccount acc = GetAccountByInfo(accountInfo);
                if (acc == null)
                {
                    return false;
                }

                return acc.IsPermedForSymbol(symbol.Name);
            };

            object result;
            if (_messageLoopOperator.Invoke(operation, TimeSpan.FromSeconds(30), out result) == false)
            {
                return false;
            }

            return (bool)result;
        }

        /// <summary>
        /// 
        /// </summary>
        AccountInfo[] OrderExecutionSourceStub.IImplementation.GetAvailableAccounts()
        {
            if (_accountInfo.HasValue)
            {
                return new AccountInfo[] { _accountInfo.Value };
            }

            return new AccountInfo[] { };
        }


        string OrderExecutionSourceStub.IImplementation.SubmitOrder(AccountInfo accountInfo, Symbol symbol, 
            OrderTypeEnum orderType, int volume, decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, 
            decimal? stopLoss, string comment, out string operationResultMessage)
        {
            operationResultMessage = string.Empty;
            string operationResultMessageCopy = string.Empty;

            PlaceOrderOperation operation = null;
            GeneralHelper.GenericReturnDelegate<string> operationDelegate = delegate()
            {
                string submitResult = DoSubmitOrder(accountInfo, symbol, orderType, volume, allowedSlippage, desiredPrice,
                    takeProfit, stopLoss, comment, out operation, out operationResultMessageCopy);

                return submitResult;
            };

            object result;
            if (_messageLoopOperator.Invoke(operationDelegate, TimeSpan.FromSeconds(60), out result) == false)
            {// Timed out.
                operationResultMessage = "Timeout submiting order.";
                return null;
            }

            if (string.IsNullOrEmpty((string)result))
            {// Operation error.
                operationResultMessage = operationResultMessageCopy;
                return null;
            }

            // Return the ID of the submitted order.
            return (string)result;
        }

        #endregion
    }
}
