using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Class represents main dataDelivery provisional functionality to an expert (expert session),
    /// or any entity that selects to use the framework.
    /// One dataDelivery provider gives dataDelivery for one trading instrument.
    /// </summary>
    [Serializable]
    public class SessionDataProvider : Operational, ISessionDataProvider
    {
        /// <summary>
        /// The delivery provides the dataDelivery to the dataDelivery access.
        /// </summary>
        private volatile ISourceDataDelivery _dataDelivery = null;
        public ISourceDataDelivery DataDelivery
        {
            get { return _dataDelivery; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected volatile ISourceManager _manager = null;

        /// <summary>
        /// Quotes
        /// </summary>
        protected volatile IQuoteProvider _quote = null;

        /// <summary>
        /// Ticks
        /// </summary>
        protected volatile IDataTickHistoryProvider _tickProvider = null;

        /// <summary>
        /// 
        /// </summary>
        protected volatile IDataBarHistoryProvider _dataBarProvider = null;

        /// <summary>
        /// Bars
        /// </summary>
        protected Dictionary<TimeSpan, IDataBarHistoryProvider> _dataBarProviders = new Dictionary<TimeSpan, IDataBarHistoryProvider>();

        DataSessionInfo _sessionInfo;

        /// <summary>
        /// Name of provider instance.
        /// </summary>
        public string Name
        {
            get { return "Data Provider for [" + SessionInfo.Name + "]"; }
        }

        /// <summary>
        /// Provides access to the current price information (may be null if only historical information is available).
        /// </summary>
        public virtual IQuoteProvider Quotes 
        {
            get
            {
                return _quote;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IDataTickHistoryProvider DataTicks
        {
            get
            {
                return _tickProvider;
            }
        }

        /// <summary>
        /// Currently selected bar history provider.
        /// Provides access to quote history (if available). May be null (in cases there is no dataDelivery history).
        /// </summary>
        public virtual IDataBarHistoryProvider DataBars 
        {
            get
            {
                return _dataBarProvider;
            }
        }

        /// <summary>
        /// Time control interface (optional, may be null).
        /// </summary>
        public virtual ITimeControl TimeControl
        {
            get
            {
                return _dataDelivery.TimeControl;
            }
        }

        #region ISessionDataProvider Members

        public DataSessionInfo SessionInfo
        {
            get { return _sessionInfo; }
        }

        public RuntimeDataSessionInformation RuntimeSessionInformation
        {
            get
            {
                return _dataDelivery.GetSymbolRuntimeSessionInformation(_sessionInfo.Symbol);
            }
        }

        public TimeSpan[] AvailableDataBarProviderPeriods
        {
            get
            {
                return _dataDelivery.GetSymbolRuntimeSessionInformation(_sessionInfo.Symbol).AvailableDataBarPeriods.ToArray();
            }
        }

        ComponentId _sourceId;
        public ComponentId SourceId
        {
            get
            {
                return _sourceId;
            }
        }

        #endregion

        [field:NonSerialized]
        public event DataProviderUpdateDelegate CurrentDataBarProviderChangedEvent;

        [field: NonSerialized]
        public event DataProviderBarProviderUpdateDelegate DataBarProviderCreatedEvent;

        [field: NonSerialized]
        public event DataProviderBarProviderUpdateDelegate DataBarProviderDestroyedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SessionDataProvider()
        { 
        }
       
        /// <summary>
        /// 
        /// </summary>
        public virtual bool SetInitialParameters(ISourceManager manager, ComponentId sourceId, ExpertSession session)
        {
            _sessionInfo = session.Info;

            _manager = manager;

            _sourceId = sourceId;

            _dataDelivery = manager.ObtainDataDelivery(sourceId);

            _quote = manager.ObtainQuoteProvider(sourceId, _sessionInfo.Symbol);

            _tickProvider = manager.ObtainDataTickHistoryProvider(sourceId, _sessionInfo.Symbol);

            _dataBarProvider = null;

            bool result = _dataDelivery != null && _quote != null && _tickProvider != null;

            SystemMonitor.CheckError(result, "Failed to initialize data provider.");

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Initialize(DataSessionInfo? sessionInfo)
        {
            //if (_dataDelivery != null && _dataDelivery.Initialize() == false)
            //{
            //    SystemMonitor.Warning("Failed to initialize dataDelivery delivery.");
            //    return false;   
            //}

            if (_dataDelivery == null)
            {
                SystemMonitor.Error("Data delivery not assigned.");
                return false;
            }

            base.StatusSynchronizationSource = _dataDelivery;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void UnInitialize()
        {
            //if (_quote != null)
            //{
            //    _quote.UnInitialize();
            //}

            //if (_dataDelivery != null)
            //{
            //    _dataDelivery.UnInitialize();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public bool SetCurrentDataBarProvider(TimeSpan? period)
        {
            if (period.HasValue == false)
            {
                _dataBarProvider = null;
                if (CurrentDataBarProviderChangedEvent != null)
                {
                    CurrentDataBarProviderChangedEvent(this);
                }
                return true;
            }

            lock(this)
            {
                if (_dataBarProviders.ContainsKey(period.Value) == false)
                {
                    SystemMonitor.OperationError("Period provider not available.");
                    return false;
                }

                _dataBarProvider = _dataBarProviders[period.Value];
            }

            if (CurrentDataBarProviderChangedEvent != null)
            {
                CurrentDataBarProviderChangedEvent(this);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public IDataBarHistoryProvider GetDataBarProvider(TimeSpan period)
        {
            lock (this)
            {
                if (_dataBarProviders.ContainsKey(period) == false)
                {
                    SystemMonitor.OperationError("Period provider not available.");
                    return null;
                }

                return _dataBarProviders[period];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public bool ObtainDataBarProvider(TimeSpan period)
        {
            lock (this)
            {
                RuntimeDataSessionInformation sessionInfo = _dataDelivery.GetSymbolRuntimeSessionInformation(SessionInfo.Symbol);

                if (sessionInfo == null)
                {
                    SystemMonitor.OperationError("Failed to retrieve runtime session information for symbol [" + SessionInfo.Symbol.Name + "].");
                    return false;
                }

                if (false == sessionInfo.AvailableDataBarPeriods.Contains(period))
                {
                    SystemMonitor.OperationError("Period not available for data bar provider creation.");
                    return false;
                }
            }

            IDataBarHistoryProvider provider;
            lock (this)
            {
                if (_dataBarProviders.ContainsKey(period))
                {
                    SystemMonitor.OperationError("Period data bar already created.");
                    return false;
                }

                provider = _manager.ObtainDataBarHistoryProvider(_dataDelivery.SourceId, 
                    _sessionInfo.Symbol, period);

                _dataBarProviders.Add(period, provider);
            }

            if (_dataBarProvider == null)
            {
                SetCurrentDataBarProvider(period);
            }

            if (DataBarProviderCreatedEvent != null)
            {
                DataBarProviderCreatedEvent(this, provider);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public bool ReleaseDataBarProvider(TimeSpan period)
        {
            IDataBarHistoryProvider provider;
            lock (this)
            {
                if (_dataBarProviders.ContainsKey(period) == false)
                {
                    return false;
                }
                
                provider = _dataBarProviders[period];
                _dataBarProviders.Remove(period);
            }

            if (_dataBarProvider == provider)
            {
                SetCurrentDataBarProvider(null);
            }

            if (DataBarProviderDestroyedEvent != null)
            {
                DataBarProviderDestroyedEvent(this, provider);
            }

            provider.Dispose();
            return true;
        }


        #region IDisposable Members

        public virtual void Dispose()
        {
            UnInitialize();

            _manager = null;

            _dataDelivery = null;

            _quote = null;

            _tickProvider = null;

            _dataBarProvider = null;

            ChangeOperationalState(OperationalStateEnum.Disposed);
        }

        #endregion


    }
}
