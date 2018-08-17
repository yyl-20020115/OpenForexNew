using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using CommonSupport;
using Arbiter;
using System.Runtime.Serialization;

namespace ForexPlatform
{
    /// <summary>
    /// OrderExecutionProvider handles managing orders on a remote source, integrated in the platforms
    /// communication mechanism.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Remote Order Execution Provider")]
    public class PlatformSourceOrderExecutionProvider : OrderExecutionSourceClientStub, ISourceOrderExecution
    {
        volatile TradePlatformComponent _manager;

        volatile TradeEntityKeeper _tradeEntities;

        ComponentId _dataSourced;

        public ITradeEntityManagement TradeEntities
        {
            get { return _tradeEntities; }
        }

        Account[] _accounts = new Account[] { };

        #region ISourceOrderExecution Members

        public Account[] Accounts
        {
            get { lock (this) { return _accounts; } }
        }

        volatile Account _defaultAccount = null;

        public Account DefaultAccount
        {
            get
            {
                return _defaultAccount;
            }
        }

        public ITimeControl TimeControl
        {
            get { return null; }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool IsBusy
        //{
        //    get { return base._isBusy; }
        //    set { base._isBusy = value; }
        //}

        #endregion

        #region Instance Control

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlatformSourceOrderExecutionProvider()
        {
            _tradeEntities = new TradeEntityKeeper();

            this.OperationalStateChangedEvent += new OperationalStateChangedDelegate(PlatformSourceOrderExecutionProvider_OperationalStateChangedEvent);
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public PlatformSourceOrderExecutionProvider(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _tradeEntities = (TradeEntityKeeper)info.GetValue("tradeEntities", typeof(TradeEntityKeeper));
            _manager = (TradePlatformComponent)info.GetValue("manager", typeof(TradePlatformComponent));
            _dataSourced = (ComponentId)info.GetValue("dataSourceId", typeof(ComponentId));

            this.OperationalStateChangedEvent += new OperationalStateChangedDelegate(PlatformSourceOrderExecutionProvider_OperationalStateChangedEvent);
        }

        /// <summary>
        /// Serialization.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("tradeEntities", _tradeEntities);
            info.AddValue("manager", _manager);
            info.AddValue("dataSourceId", _dataSourced);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Initialize()
        {
            base.Initialize();

            _tradeEntities.Initialize();

            PlatformSourceOrderExecutionProvider_OperationalStateChangedEvent(this, OperationalStateEnum.Unknown);

            return true;
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

            base.UpdateAccountsSubscription(false);

            _tradeEntities.UnInitialize();
        }

        public void Dispose()
        {
            if (_tradeEntities != null)
            {
                _tradeEntities.Dispose();
                _tradeEntities = null;
            }
        }

        void PlatformSourceOrderExecutionProvider_OperationalStateChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            if (OperationalState == OperationalStateEnum.Operational && _accounts.Length == 0)
            {
                if (base.UpdateAccountsSubscription(true) == false)
                {
                    SystemMonitor.Error("Failed to subscribe to source. Further operations will not proceed as expected.");
                }

                AccountInfo[] accountInfos;
                if (base.GetAvailableAccountInfos(out accountInfos) && accountInfos.Length > 0)
                {
                    lock (this)
                    {
                        _accounts = new Account[accountInfos.Length];
                        for (int i = 0; i < accountInfos.Length; i++)
                        {
                            _accounts[i] = new RemoteExecutionAccount(accountInfos[i]);
                            _accounts[i].SetInitialParameters(_manager, this, _manager.GetDataDelivery(_dataSourced));
                            _accounts[i].Initialize(this);
                        }

                        if (_accounts.Length > 0)
                        {
                            _defaultAccount = _accounts[0];
                        }
                        else
                        {
                            _defaultAccount = null;
                        }
                    }
                }
                else
                {
                    SystemMonitor.Warning("Failed to establish accounts (default account) on Order Execution Provider.");
                }
            }
        }


        #endregion


        /// <summary>
        /// 
        /// </summary>
        public bool SetInitialParameters(TradePlatformComponent manager, ComponentId sourceId,
            ComponentId dataSourced)
        {
            SystemMonitor.CheckError(_manager == null, "Session member already assigned.");
            _manager = manager;
            _dataSourced = dataSourced;

            _tradeEntities.SetInitialParameters(manager, this, manager.GetDataDelivery(_dataSourced));

            SourceInfo? info = _manager.GetSourceInfo(sourceId, SourceTypeEnum.OrderExecution);
            if (info.HasValue)
            {
                return base.SetInitialParameters(sourceId, info.Value.TransportInfo);
            }

            SystemMonitor.Warning("Failed to establish source information [" + sourceId.Print() + "]");
            return false;
        }

        ///// <summary>
        ///// Obtains all the orders from the order executioner.
        ///// </summary>
        //public bool SynchronizeOrders(string[] updateOrdersIds, out string operationResultMessage)
        //{
        //    TracerHelper.TraceEntry();

        //    if (OperationalState != OperationalStateEnum.Operational)
        //    {
        //        operationResultMessage = "Executioner not operational.";
        //        return false;
        //    }

        //    if (DefaultAccount == null || DefaultAccount.TradeEntities == null)
        //    {
        //        operationResultMessage = "Remote order execution operation can not continue due to lack of required refereces.";
        //        SystemMonitor.OperationWarning(operationResultMessage);
        //        return false;
        //    }

        //    ITradeEntityManagement tradeEntities = DefaultAccount.TradeEntities;

        //    List<string> ordersIds = new List<string>();
        //    //List<string> historicalOrdersIds = new List<string>();

        //    if (updateOrdersIds != null && updateOrdersIds.Length != 0)
        //    {// Update only the orders from the request.
        //        ordersIds.AddRange(updateOrdersIds);
        //    }
        //    else
        //    {// Obtain full list of orders from the source.

        //        SystemMonitor.NotImplementedWarning();

        //        //string[] openPlatformIds = new string[] { };
        //        //string[] historicalPlatformsIds = new string[] { };

        //        //if (base.GetAllOrdersIds(DefaultAccount.Info, out openPlatformIds, out historicalPlatformsIds, out operationResultMessage) == false)
        //        //{
        //        //    return false;
        //        //}

        //        //ordersIds.AddRange(openPlatformIds);
        //        //historicalOrdersIds.AddRange(historicalPlatformsIds);

        //        //// Since we obtained all, filter existing.
        //        //foreach (string orderId in ordersIds.ToArray())
        //        //{
        //        //    Order order = tradeEntities.GetOrderById(orderId);
        //        //    if (order != null && order.IsOpenOrPending)
        //        //    {
        //        //        ordersIds.Remove(orderId);
        //        //    }
        //        //}

        //        //foreach (string orderId in historicalOrdersIds.ToArray())
        //        //{
        //        //    Order order = tradeEntities.GetOrderById(orderId);
        //        //    if (order != null && order.IsOpenOrPending == false)
        //        //    {
        //        //        historicalOrdersIds.Remove(orderId);
        //        //    }
        //        //}
        //    }

        //    operationResultMessage = "Orders obtained successfully.";
        //    bool operationResult = true;

        //    if (ordersIds.Count == 0 /*&& historicalOrdersIds.Count == 0*/)
        //    {// Nothing to update.
        //        return operationResult;
        //    }

        //    // AddElement the hitorical to ordersIds, than process them all.
        //    //ordersIds.AddRange(historicalOrdersIds);

        //    // Get informations for each.
        //    OrderInfo[] informations;
        //    if (GetOrdersInformation(DefaultAccount.Info, ordersIds.ToArray(), out informations, out operationResultMessage) == false)
        //    {// One failure is enough to establish the entire operation as failed.
        //        operationResult = false;
        //        operationResultMessage = "Some orders were not obtained properly.";
        //    }

        //    if (informations == null)
        //    {
        //        return operationResult;
        //    }

        //    foreach (OrderInfo orderInfo in informations)
        //    {// Check if the order already exists.

        //        Order order = TradeEntities.GetOrderById(orderInfo.Id);

        //        //if (order == null)
        //        //{// Not existing we need to create a brand new one.
        //        //    SystemMonitor.Warning("Order instance not created.");
        //        //    order = new ActiveOrder(_manager, ExecutionSourceId, orderInfo.Symbol, false);
        //        //    if (((ActiveOrder)order).LoadFromFile() == false)
        //        //    {
        //        //        SystemMonitor.Error("Failed to initialize order.");
        //        //        continue;
        //        //    }
        //        //}

        //        if (order != null && order.AdoptInfo(orderInfo) == false)
        //        {
        //            //order.UnInitialize();

        //            //operationResult = false;
        //            //operationResultMessage = "Some orders were not obtained properly.";

        //            continue;
        //        }

        //        // Existing order will simply be skipped, but make sure to add for new orders case.
        //        TradeEntities.AddOrder(order);
        //    }

        //    return operationResult;
        //}


        //#region ISourceOrderExecution Members

        //Account[] ISourceOrderExecution.Accounts
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //Account ISourceOrderExecution.DefaultAccount
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //ITradeEntityManagement ISourceOrderExecution.TradeEntities
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //ComponentId ISourceOrderExecution.SourceId
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //ITimeControl ISourceOrderExecution.TimeControl
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //#endregion

        //#region IOrderSink Members

        //event OrdersUpdateDelegate IOrderSink.OrdersUpdatedEvent
        //{
        //    add { throw new NotImplementedException(); }
        //    remove { throw new NotImplementedException(); }
        //}

        //event PositionUpdateDelegate IOrderSink.PositionsUpdateEvent
        //{
        //    add { throw new NotImplementedException(); }
        //    remove { throw new NotImplementedException(); }
        //}

        //event AccountInfoUpdateDelegate IOrderSink.AccountInfoUpdateEvent
        //{
        //    add { throw new NotImplementedException(); }
        //    remove { throw new NotImplementedException(); }
        //}

        //bool IOrderSink.SupportsActiveOrderManagement
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //decimal? IOrderSink.SlippageMultiplicator
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //bool IOrderSink.Initialize()
        //{
        //    throw new NotImplementedException();
        //}

        //void IOrderSink.UnInitialize()
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.BeginAccountInfoUpdate(AccountInfo accountInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.GetAvailableAccountInfos(out AccountInfo[] accounts)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.GetOrdersInformation(AccountInfo accountInfo, string[] orderIds, out OrderInfo[] informations, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.BeginOrdersInformationUpdate(AccountInfo accountInfo, string[] orderIds, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //string IOrderSink.SubmitOrder(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType, int volume, decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss, string comment, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.SynchronousExecute(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType, int volume, decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss, string comment, out OrderInfo? info, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.ModifyOrder(AccountInfo account, Order order, decimal? stopLoss, decimal? takeProfit, decimal? targetOpenPrice, out string modifiedId, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.DecreaseOrderVolume(AccountInfo account, Order order, decimal volumeDecreasal, decimal? allowedSlippage, decimal? desiredPrice, out decimal decreasalPrice, out string modifiedId, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.IncreaseOrderVolume(AccountInfo account, Order order, decimal volumeIncrease, decimal? allowedSlippage, decimal? desiredPrice, out decimal increasalPrice, out string modifiedId, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.CancelPendingOrder(AccountInfo account, Order order, out string modifiedId, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //bool IOrderSink.CloseOrder(AccountInfo account, Order order, decimal? allowedSlippage, decimal? desiredPrice, out decimal closingPrice, out DateTime closingTime, out string modifiedId, out string operationResultMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

        //#region IOperational Members

        //OperationalStateEnum IOperational.OperationalState
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //event OperationalStateChangedDelegate IOperational.OperationalStateChangedEvent
        //{
        //    add { throw new NotImplementedException(); }
        //    remove { throw new NotImplementedException(); }
        //}

        //#endregion

        //#region IDisposable Members

        //void IDisposable.Dispose()
        //{
        //    //throw new NotImplementedException();
        //}

        //#endregion
    }
}
