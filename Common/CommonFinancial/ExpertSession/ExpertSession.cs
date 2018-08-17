using System;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// For each pair-timeframe couple (EURUSD.M60 for ex.) an ExpertSession is needed. 
    /// The ExpertSession represents all information related to this given trading couple.
    /// As well as any IndicatorsUnsafe or Evaluators calculated based on this sessions dataDelivery.
    /// The dataDelivery inside the session is provided by the SessionDataProvider(ISessionDataProvider) and the
    /// order execution capabilities by the OrderExecutionProvider(ISourceOrderExecution).
    /// </summary>
    [Serializable]
    public class ExpertSession : Operational, IDisposable
    {
        DataSessionInfo _sessionInfo;
        /// <summary>
        /// Session orderInfo. Contains essential trading session information.
        /// The actual sessions that the Data OrderExecutionProvider and the ActiveOrder Execution provider
        /// user have different ids than this session, since on restarts and reconnects
        /// new sessions and generated and the requests to those mapped correspondigly.
        /// </summary>
        public DataSessionInfo Info
        {
            get { lock (this) { return _sessionInfo; } }
        }

        volatile ISessionDataProvider _sessionDataProvider;
        /// <summary>
        /// Session dataDelivery provider.
        /// </summary>
        public ISessionDataProvider DataProvider
        {
            get { return _sessionDataProvider; }
        }

        volatile ISourceOrderExecution _orderExecutionProvider;
        /// <summary>
        /// Session execution provider.
        /// </summary>
        public ISourceOrderExecution OrderExecutionProvider
        {
            get { return _orderExecutionProvider; }
        }

        Position _position = null;
        /// <summary>
        /// Trade position related to this session (if available).
        /// </summary>
        public Position Position
        {
            get
            {
                return _position;
            }
        }

        volatile object _tag = null;
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExpertSession(DataSessionInfo sessionInfo)
        {
            _sessionInfo = sessionInfo;
        }

        /// <summary>
        /// Startup initialization procedure, called only once in the lifecycle of an object 
        /// (only after initial construction, not upon serialization-deserialization).
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <param name="orderExectionProvider"></param>
        public bool SetInitialParameters(ISessionDataProvider dataProvider, ISourceOrderExecution orderExectionProvider)
        {
            SystemMonitor.CheckThrow(_sessionDataProvider == null && _orderExecutionProvider == null, "Session already initialized.");
            _sessionDataProvider = dataProvider;
            _orderExecutionProvider = orderExectionProvider;

            return true;
        }

        public void Dispose()
        {
            if (_sessionDataProvider != null)
            {
                _sessionDataProvider.Dispose();
                _sessionDataProvider = null;
            }

            if (_orderExecutionProvider != null)
            {
                _orderExecutionProvider = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Initialize(DataSessionInfo? sessionInfo)
        {
            if (sessionInfo.HasValue == false)
            {
                sessionInfo = _sessionInfo;
            }

            if (IsInitOrOperational(_sessionDataProvider.OperationalState) == false)
            {
                if (_sessionDataProvider.Initialize(sessionInfo) == false)
                {
                    SystemMonitor.Error("Data provider for expert session can not be initialized.");
                    ChangeOperationalState(OperationalStateEnum.NotOperational);
                    return false;
                }
            }

            ChangeOperationalState(OperationalStateEnum.Initializing);

            _sessionDataProvider.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_subItem_OperationalStatusChangedEvent);

            if (_orderExecutionProvider != null)
            {
                SystemMonitor.CheckReport(_orderExecutionProvider.OperationalState == OperationalStateEnum.Operational, "Order execution provider not ready on session init.");
                _orderExecutionProvider.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_subItem_OperationalStatusChangedEvent);
            }

            _subItem_OperationalStatusChangedEvent(null, OperationalStateEnum.Unknown);

            return true;
        }

        public virtual void UnInitialize()
        {
            ChangeOperationalState(OperationalStateEnum.UnInitialized);

            lock (this)
            {
                if (_sessionDataProvider != null)
                {
                    _sessionDataProvider.UnInitialize();
                    _sessionDataProvider.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(_subItem_OperationalStatusChangedEvent);
                }

                if (_orderExecutionProvider != null)
                {
                    _orderExecutionProvider.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(_subItem_OperationalStatusChangedEvent);
                }
            }

        }

        /// <summary>
        /// Helper method, gives full account name of session.
        /// </summary>
        /// <returns></returns>
        public string GetFullAccountName()
        {
            if (OrderExecutionProvider != null &&
                OrderExecutionProvider.OperationalState != OperationalStateEnum.UnInitialized
                && OrderExecutionProvider.DefaultAccount != null)
            {
                AccountInfo account = OrderExecutionProvider.DefaultAccount.Info;
                return account.Server + ", " + account.Company + ", " + account.Name + "." + account.Id
                    + ", " + account.BaseCurrency.Name;
            }

            return "Unknown";
        }

        void _subItem_OperationalStatusChangedEvent(IOperational subItem, OperationalStateEnum previousState)
        {
            if (_sessionDataProvider != null && _sessionDataProvider.OperationalState == OperationalStateEnum.Operational
                && (_orderExecutionProvider == null || _orderExecutionProvider.OperationalState == OperationalStateEnum.Constructed))
            {
                if (_orderExecutionProvider != null && _orderExecutionProvider.TradeEntities != null)
                {
                    // Create the default position for this session.
                    _position = _orderExecutionProvider.TradeEntities.ObtainPositionBySymbol(_sessionInfo.Symbol);

                    _orderExecutionProvider.ChangeOperationalState( OperationalStateEnum.Operational );
                }

                ChangeOperationalState(OperationalStateEnum.Operational);
            }
            else
            {
                ChangeOperationalState(OperationalStateEnum.NotOperational);
            }
        }
    }
}
