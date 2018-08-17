using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Threading;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// The base class for any Execution Account. An account handles all information regarding 
    /// Margin, Leverage, Profit, etc. It also tracks all orders placed to make sure they comply with
    /// requirements (for ex. free margin requirement).
    /// </summary>
    [Serializable]
    public abstract class Account : Operational, IDeserializationCallback, IDisposable
    {
        protected IOrderSink _implementation;

        DateTime _lastInfoUpdate = DateTime.MinValue;
        TimeSpan _minimumUpdateInterval = TimeSpan.FromSeconds(2);

        protected AccountInfo _accountInfo;
        /// <summary>
        /// 
        /// </summary>
        public AccountInfo Info
        {
            get { return _accountInfo; }
        }

		int _marginCallPercents = 50;
		/// <summary>
		/// percents below which we have a margin call;
		/// </summary>
		public int MarginCallPercents
		{
			get { return _marginCallPercents; }
			set { _marginCallPercents = value; }
		}

        ISourceManager _manager;

        ISourceDataDelivery _dataDelivery;

        ISourceOrderExecution _provider = null;
        /// <summary>
        /// 
        /// </summary>
        public ISourceOrderExecution OrderExecutionProvider
        {
            get { return _provider; }
        }

        AccountStatistics _statistics = new AccountStatistics();
        /// <summary>
        /// Statistics.
        /// </summary>
        public AccountStatistics Statistics
        {
            get { return _statistics; }
        }

        /// <summary>
        /// Information for past and present orders. May be null, 
        /// when accout does not give information about orders.
        /// </summary>
        public ITradeEntityManagement TradeEntities
        {
            get
            {
                if (_provider != null)
                {
                    return _provider.TradeEntities;
                }

                return null;
            }
        }

        /// <summary>
        /// Delegate for the event of account information update.
        /// </summary>
        public delegate void AccountUpdateDelegate(Account account);

        [NonSerialized]
        Mutex _updateMutex = new Mutex(false);

        [field: NonSerialized]
        public event AccountUpdateDelegate UpdatedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Account(AccountInfo info)
        {
            _accountInfo = info;
        }

        public virtual void Dispose()
        {
            //if (_tradeEntities != null)
            //{
            //    _tradeEntities.UnInitialize();
            //    _tradeEntities = null;
            //}

            if (_statistics != null)
            {
                _statistics.Dispose();
                _statistics = null;
            }
         
            _dataDelivery = null;
            _manager = null;
            
            if (_provider != null)
            {
                _provider.AccountInfoUpdateEvent -= new AccountInfoUpdateDelegate(_provider_AccountInfoUpdateEvent);
                _provider.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(_provider_OperationalStatusChangedEvent);

                _provider = null;
            }
        }

        /// <summary>
        /// Initialize execution account with its owner provider.
        /// </summary>
        /// <param name="session"></param>
        public bool SetInitialParameters(ISourceManager manager, ISourceOrderExecution orderExecutionProvider, ISourceDataDelivery dataDelivery)
        {
            SystemMonitor.CheckError(_provider == null, "Order account already initialized.");

            _manager = manager;
            //why is the provider not ready
            _provider = orderExecutionProvider;
            _dataDelivery = dataDelivery;

            SystemMonitor.CheckWarning(_manager != null && _provider != null && _dataDelivery != null, "Account not properly initialized.");

            return true;
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            _updateMutex = new Mutex(false);
        }

        /// <summary>
        /// Allows to feed in all the core information for an account from the orderInfo structure.
        /// </summary>
        /// <param name="orderInfo"></param>
        protected void AdoptInfo(AccountInfo info)
        {
            lock (this)
            {
                _accountInfo = info;
            }

            ChangeOperationalState(OperationalStateEnum.Operational);
            RaiseUpdateEvent();
        }

        void _provider_AccountInfoUpdateEvent(IOrderSink provider, AccountInfo accountInfo)
        {
            lock (this)
            {
                _accountInfo.Update(accountInfo);
            }

            RaiseUpdateEvent();
        }

        /// <summary>
        /// Helper, raising update event.
        /// </summary>
        protected void RaiseUpdateEvent()
        {
            if (UpdatedEvent != null)
            {
                UpdatedEvent(this);
            }
        }

        /// <summary>
        /// Start asynchronous update.
        /// </summary>
        public virtual void BeginUpdate()
        {
            if (_updateMutex.WaitOne(10, true) == false)
            {// Already updating.
                return;
            }

            if (DateTime.Now - _lastInfoUpdate < _minimumUpdateInterval)
            {// Last update too soon.
                _updateMutex.ReleaseMutex();
                return;
            }
            
            _lastInfoUpdate = DateTime.Now;
            bool result = _implementation.BeginAccountInfoUpdate(this.Info);

            _updateMutex.ReleaseMutex();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Initialize(IOrderSink implementation)
        {
            SystemMonitor.CheckError(_implementation == null, "Account already initialized.");
            _implementation = implementation;

            lock (this)
            {
                // Once the session is initialized - subscribe to its members.
                if (_provider != null)
                {
                    _provider.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_provider_OperationalStatusChangedEvent);
                    _provider.AccountInfoUpdateEvent += new AccountInfoUpdateDelegate(_provider_AccountInfoUpdateEvent);
                }

                if (TradeEntities != null)
                {
                    //if (_tradeEntities.Initialize() == false)
                    //{
                    //    SystemMonitor.OperationError("Failed to initialize order management.");
                    //    return false;
                    //}
                    //_tradeEntities.SynchronizeOrdersEvent += new TradeEntityKeeper.SynchronizeOrdersDelegate(_orderManagement_UpdateOrdersEvent);

                    TradeEntities.OrdersAddedEvent += new OrderManagementOrdersUpdateDelegate(Orders_OrdersAddedEvent);
                    TradeEntities.OrdersRemovedEvent += new OrderManagementOrdersUpdateDelegate(Orders_OrdersRemovedEvent);
                    TradeEntities.OrdersUpdatedEvent += new OrderManagementOrdersUpdateTypeDelegate(Orders_OrderUpdatedEvent);
                }

                if (_provider != null && _dataDelivery != null)
                {
                    _dataDelivery.OperationalStateChangedEvent += new OperationalStateChangedDelegate(DataProvider_OperationalStatusChangedEvent);
                    _dataDelivery.QuoteUpdateEvent += new QuoteUpdateDelegate(dataDelivery_QuoteUpdateEvent);
                }
                _statistics.Initialize(this);

                //SystemMonitor.OperationWarning("Initial order synchronization not done.");
                //GeneralHelper.FireAndForget(delegate()
                //{// Run an initial order syncrhonization.
                //    Thread.Sleep(2500);
                //    string operationResultMessage;
                //    //_orderManagement_UpdateOrdersEvent(_tradeEntities, null, out operationResultMessage);
                //    _provider.Account.SynchronizeOrders(null, out operationResultMessage);
                //});
            }

            return true;
        }

        protected virtual void dataDelivery_QuoteUpdateEvent(ISourceDataDelivery dataDelivery, DataSessionInfo session, Quote? quote)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {
            lock (this)
            {
                // Once the session is initialized - subscribe to its members.
                if (_provider != null)
                {
                    _provider.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(_provider_OperationalStatusChangedEvent);
                }

                if (TradeEntities != null)
                {
                    //_tradeEntities.UnInitialize();

                    //_tradeEntities.SynchronizeOrdersEvent -= new TradeEntityKeeper.SynchronizeOrdersDelegate(_orderManagement_UpdateOrdersEvent);
                    TradeEntities.OrdersAddedEvent -= new OrderManagementOrdersUpdateDelegate(Orders_OrdersAddedEvent);
                    TradeEntities.OrdersRemovedEvent -= new OrderManagementOrdersUpdateDelegate(Orders_OrdersRemovedEvent);
                    TradeEntities.OrdersUpdatedEvent -= new OrderManagementOrdersUpdateTypeDelegate(Orders_OrderUpdatedEvent);
                }

                if (_dataDelivery != null)
                {
                    _dataDelivery.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(DataProvider_OperationalStatusChangedEvent);
                    _dataDelivery.QuoteUpdateEvent -= new QuoteUpdateDelegate(dataDelivery_QuoteUpdateEvent);
                }

                _implementation = null;
                _statistics.UnInitialize();
            }
        }


        protected virtual void Orders_OrderUpdatedEvent(ITradeEntityManagement provider, AccountInfo account, Order[] orders, Order.UpdateTypeEnum[] updatesType)
        {
            
        }

        protected virtual void Orders_OrdersRemovedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            
        }

        protected virtual void Orders_OrdersAddedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            
        }

        protected virtual void DataProvider_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            
        }

        protected virtual void _provider_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            
        }

        // Margin = Ask of Position * Lot Size / Margin Requirement >> Converted to account base currency.

            //        // Calculate positions advanced parameters.
            //decimal grossUnrealizedPnL = 0;
            //decimal grossCommission = 0;
            //decimal grossPosition = 0;

            //foreach (MbtPosition position in orderClient.Positions)
            //{
            //    Quote? quote = null;

            //    lock (quotes)
            //    {// Try to get quote for the symbol of this position.
            //        if (quotes.SessionsQuotesUnsafe.ContainsKey(position.Symbol))
            //        {
            //            quote = quotes.SessionsQuotesUnsafe[position.Symbol].Quote;
            //        }
            //    }

            //    if (quote.HasValue == false)
            //    {// We need to put subscription for this symbol too.
            //        string symbolMarket;

            //        if (quotes.GetSingleSymbolQuote(position.Symbol, TimeSpan.FromSeconds(3), out symbolMarket).HasValue)
            //        {// BaseCurrency found and established from broker.
            //            quotes.SubscribeSymbolSession(new Symbol(symbolMarket, position.Symbol));
            //        }
            //    }

            //    if (quote.HasValue == false || quote.Value.Ask.HasValue == false
            //        || quote.Value.Bid.HasValue == false)
            //    {
            //        SystemMonitor.OperationWarning("Failed to locate quote for position [" + position.Symbol + "]");
            //        continue;
            //    }

            //    if (position.IntradayPosition == 0)
            //    {
            //        continue;
            //    }

            //    if (position.Account == account)
            //    {
            //        grossCommission += (decimal)position.Commission;

            //        decimal unrealizedPnL = 0;
            //        if (position.IntradayPosition > 0)
            //        {
            //            unrealizedPnL = quote.Value.Bid.Value - (decimal)position.IntradayPrice;
            //        }
            //        else if (position.IntradayPosition < 0)
            //        {
            //            unrealizedPnL = quote.Value.Ask.Value - (decimal)position.IntradayPrice;
            //        }
                    
            //        unrealizedPnL = unrealizedPnL * position.IntradayPosition;

            //        grossPosition += Math.Abs(position.IntradayPosition);

            //        grossUnrealizedPnL += unrealizedPnL;
            //    }
            //}
    }
}
