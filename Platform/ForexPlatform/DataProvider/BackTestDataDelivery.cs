using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Implements a data delivery sistem allowing to back test since it is time controllable in the past.
    /// Current implementation only supports one symbol/session.
    /// </summary>
    [Serializable]
    public class BackTestDataDelivery : Operational, ISourceDataDelivery
    {
        ComponentId _sourceId = new ComponentId(Guid.NewGuid(), "BackTestDataDelivery", typeof(BackTestDataDelivery));

        public ComponentId SourceId
        {
            get { return _sourceId; }
        }

        ISourceManager _manager;

        TimeControl _timeControl = null;
        /// <summary>
        /// 
        /// </summary>
        public ITimeControl TimeControl
        {
            get { return _timeControl; }
        }

        volatile DataBarHistoryProvider _sourceDataBarProvider = null;

        volatile ISourceDataDelivery _sourceDataDelivery = null;

        DataSessionInfo? _pendingSessionInfo = null;

        DataHistoryRequest? _pendingRequest = null;

        TimeSpan _period = TimeSpan.Zero;

        DataSessionInfo _sessionInfo = DataSessionInfo.Empty;

        volatile int _step = 12;
        
        /// <summary>
        /// How many steps to provide initially.
        /// </summary>
        public int InitialSteps
        {
            get { return _step; }
            set { _step = value; }
        }

        volatile int _spreadPoints = 4;
        /// <summary>
        /// How much points (pip) the spread is.
        /// </summary>
        public int SpreadPoints
        {
            get { return _spreadPoints; }
            set { _spreadPoints = value; }
        }

        [NonSerialized]
        bool _initialStepsRestored = false;

        [field:NonSerialized]
        public event QuoteUpdateDelegate QuoteUpdateEvent;

        [field: NonSerialized]
        public event DataHistoryUpdateDelegate DataHistoryUpdateEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackTestDataDelivery()
        {
            _initialStepsRestored = false;
            ChangeOperationalState(OperationalStateEnum.Constructed);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            _initialStepsRestored = false;

            // This is needed, so make sure to apply it each time we start, since it is not persisted.
            _sourceId.IdentifiedComponentType = typeof(BackTestDataDelivery);

            _sourceDataBarProvider.DataBarHistoryUpdateEvent += new DataBarHistoryUpdateDelegate(_sourceDataBarProvider_DataBarHistoryUpdateEvent);
        }

        public bool SetInitialParameters(ISourceManager manager, ComponentId sourceSourceId, ExpertSession session)
        {
            _manager = manager;

            _sessionInfo = session.Info;

            _sourceDataDelivery = manager.ObtainDataDelivery(sourceSourceId);
            //_sourceDataDelivery.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_sourceDataDelivery_OperationalStateChangedEvent);

            StatusSynchronizationSource = _sourceDataDelivery;

            return true;
        }

        public bool SubscribeToData(DataSessionInfo session, bool subscribe, DataSubscriptionInfo subscription)
        {
            return _sourceDataDelivery.SubscribeToData(session, subscribe, subscription);
        }

        public Dictionary<Symbol, TimeSpan[]> SearchSymbols(string symbolMatchPattern)
        {
            return _sourceDataDelivery.SearchSymbols(symbolMatchPattern);
        }

        void ConstructSourceDataBarProvider(TimeSpan period)
        {
            if (_sourceDataBarProvider == null)
            {
                if (_sourceDataDelivery.OperationalState != OperationalStateEnum.Operational)
                {
                    SystemMonitor.OperationError("Can not build data bar provider, source data delivery not ready.");
                    return;
                }

                if (_sourceDataDelivery.GetSymbolRuntimeSessionInformation(_sessionInfo.Symbol).AvailableDataBarPeriods.Contains(period) == false)
                {
                    SystemMonitor.OperationError("Can not build data bar provider, period not supported.");
                    return;
                }

                _period = period;
                _timeControl.Period = period;

                _sourceDataBarProvider = new DataBarHistoryProvider(_sourceDataDelivery, _sessionInfo, period, 100000);

                _sourceDataBarProvider.DataBarHistoryUpdateEvent += new DataBarHistoryUpdateDelegate(_sourceDataBarProvider_DataBarHistoryUpdateEvent);
            }
        }

        public bool Initialize()
        {
            lock (this)
            {
                _timeControl = new TimeControl();

                _timeControl.CanStepForward = true;
                _timeControl.CanStepBack = false;
                _timeControl.CanRestart = false;

                _timeControl.Period = _period;

                _timeControl.TotalStepsValueUpdateEvent += delegate(ITimeControl control)
                {
                    if (_sourceDataBarProvider == null)
                    {
                        return 0;
                    }

                    return _sourceDataBarProvider.BarCount;
                };

                _timeControl.CurrentStepChangedEvent += delegate(ITimeControl control)
                {
                    _step = _timeControl.CurrentStep.Value;
                };

                _timeControl.StepForwardEvent += delegate(ITimeControl control, int steps)
                {
                    if (_sourceDataBarProvider == null)
                    {
                        SystemMonitor.Warning("Current data bar provider not assigned.");
                        return true;
                    }

                    lock (this)
                    {
                        bool step = (_sourceDataBarProvider.BarCount > _timeControl.CurrentStep.Value);
                        if (step == false || steps == 0)
                        {
                            return false;
                        }
                    }

                    List<DataBar> newBars = new List<DataBar>();
                    lock (_sourceDataBarProvider)
                    {
                        ReadOnlyCollection<DataBar> barsUnsafe = _sourceDataBarProvider.BarsUnsafe;

                        for (int i = 0; i < steps && _timeControl.CurrentStep.Value + i < barsUnsafe.Count; i++)
                        {
                            newBars.Add(barsUnsafe[_timeControl.CurrentStep.Value + i]);
                        }
                    }

                    if (newBars.Count > 0)
                    {// Feed the update steps one by one.

                        foreach (DataBar bar in newBars)
                        {
                            if (DataHistoryUpdateEvent != null)
                            {// This will insert those bars in the surfaceKeeper.
                                DataHistoryUpdateEvent(this, _sessionInfo, new DataHistoryUpdate(_timeControl.Period.Value, new DataBar[] { bar }));
                            }

                            if (QuoteUpdateEvent != null)
                            {
                                QuoteUpdateEvent(this, _sessionInfo, new Quote(bar, _spreadPoints * _sessionInfo.DecimalIncrement));
                            }
                        }
                    }


                    //if (DataHistoryUpdateEvent != null)
                    //{// This will insert those bars in the surfaceKeeper.
                    //    DataHistoryUpdateEvent(this, _sessionInfo, new DataHistoryUpdate(_timeControl.Period.Value, bars));
                    //}

                    //if (QuoteUpdateEvent != null)
                    //{
                    //    QuoteUpdateEvent(this, _sessionInfo, new Quote(bars[bars.Count - 1], _spreadPoints * _sessionInfo.DecimalIncrement));
                    //}

                    return true;
                };

                _timeControl.StepToEvent += delegate(ITimeControl control, int index)
                {
                    if (_sourceDataBarProvider == null)
                    {
                        SystemMonitor.OperationWarning("Current data bar provider not assigned.");
                        return true;
                    }

                    List<DataBar> newBars = new List<DataBar>();
                    lock (this)
                    {
                        if (index <= _timeControl.CurrentStep || index > _sourceDataBarProvider.BarCount)
                        {// Step invalid.
                            SystemMonitor.OperationWarning(string.Format("Step to invalid [{0} out of {1}, current {2}].", index.ToString(), _sourceDataBarProvider.BarCount.ToString(), _timeControl.CurrentStep.Value));
                            return false;
                        }
                    }

                    lock (_sourceDataBarProvider)
                    {
                        ReadOnlyCollection<DataBar> barsUnsafe = _sourceDataBarProvider.BarsUnsafe;
                        for (int i = _timeControl.CurrentStep.Value; i < index; i++)
                        {
                            newBars.Add(barsUnsafe[i]);
                        }
                    }

                    if (newBars.Count > 0)
                    {// Feed the update steps one by one.

                        foreach (DataBar bar in newBars)
                        {
                            if (DataHistoryUpdateEvent != null)
                            {// This will insert those bars in the surfaceKeeper.
                                DataHistoryUpdateEvent(this, _sessionInfo, new DataHistoryUpdate(_timeControl.Period.Value, new DataBar[] { bar }));
                            }

                            if (QuoteUpdateEvent != null)
                            {
                                QuoteUpdateEvent(this, _sessionInfo, new Quote(bar, _spreadPoints * _sessionInfo.DecimalIncrement));
                            }
                        }
                    }

                    return true;
                };

                //_sourceDataBarProvider.DataBarHistoryUpdateEvent += new DataBarHistoryUpdateDelegate(_sourceDataBarProvider_DataBarHistoryUpdateEvent);
            }

            RestoreLastStep();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        void RestoreLastStep()
        {
            bool restoreSteps = false;

            lock (this)
            {
                restoreSteps = _initialStepsRestored == false && (_sourceDataBarProvider != null && _timeControl != null && _sourceDataBarProvider.OperationalState == OperationalStateEnum.Operational
                        && _sourceDataBarProvider.BarsUnsafe.Count > 0 && _timeControl.CurrentStep != _step);

                _initialStepsRestored = _initialStepsRestored || restoreSteps;
            }

            if (restoreSteps)
            {
                if (_step > _sourceDataBarProvider.BarCount)
                {
                    _step = _sourceDataBarProvider.BarCount;
                }

                if (_step > 0)
                {
                    _timeControl.StepTo(_step);
                }
            }

        }

        /// <summary>
        /// Since we are in back test delivery, it is possible to receive this before we are initialized, 
        /// since the underlying data bar provider gets launched before this class does.
        /// </summary>
        void _sourceDataBarProvider_DataBarHistoryUpdateEvent(IDataBarHistoryProvider provider, DataBarUpdateType updateType, int updatedBarsCount)
        {
            SystemMonitor.CheckError(provider == _sourceDataBarProvider, "Data provider update not expected.");

            RestoreLastStep();

            if (_pendingSessionInfo.HasValue && _pendingRequest.HasValue)
            {// Executed only the first time.
                _pendingSessionInfo = null;
                _pendingRequest = null;

                RequestDataHistoryUpdate(_pendingSessionInfo.Value, _pendingRequest.Value, false);
            }
        }

        public List<RuntimeDataSessionInformation> GetSymbolsRuntimeSessionInformations(Symbol[] symbols)
        {
            return _sourceDataDelivery.GetSymbolsRuntimeSessionInformations(symbols);
        }

        public RuntimeDataSessionInformation GetSymbolRuntimeSessionInformation(Symbol symbol)
        {
            RuntimeDataSessionInformation info = _sourceDataDelivery.GetSymbolRuntimeSessionInformation(symbol);
            
            if (info != null)
            {// Session info changes on each run (symbol stays the same), so keep it updated.
                _sessionInfo = info.Info;
            }

            return info;
        }
        
        public bool RequestQuoteUpdate(DataSessionInfo sessionInfo, bool waitResult)
        {
            Quote? quote = null;
            lock (this)
            {
                if (_sourceDataBarProvider == null || _sourceDataBarProvider.BarCount == 0)
                {// No steps done yet.
                    return true;
                }

            }
            
            lock (_sourceDataBarProvider)
            {
                if (_sourceDataBarProvider.BarCount < _step)
                {
                    SystemMonitor.Error("Unexpected case.");
                    return false;
                }
                else
                {
                    if (_step - 1 >= 0 && _step - 1 < _sourceDataBarProvider.BarCount)
                    {
                        quote = new Quote(_sourceDataBarProvider.BarsUnsafe[_step - 1], _spreadPoints * _sessionInfo.DecimalIncrement);
                    }
                }
            }

            if (quote.HasValue == false)
            {// No dataDelivery.
                return false;
            }

            if (QuoteUpdateEvent != null)
            {
                QuoteUpdateEvent(this, sessionInfo, quote);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RequestDataHistoryUpdate(DataSessionInfo sessionInfo, DataHistoryRequest request, bool waitResult)
        {
            if (_sourceDataBarProvider == null)
            {
                ConstructSourceDataBarProvider(request.Period);
                //SystemMonitor.Warning("Source data bar provider not created yet.");
                return true;
            }

            if (request.StartIndex.HasValue == false)
            {
                request.StartIndex = -1;
            }

            if (request.MaxValuesRetrieved.HasValue == false)
            {
                request.MaxValuesRetrieved = int.MaxValue;
            }

            if (request.Period != _sourceDataBarProvider.Period)
            {
                SystemMonitor.NotImplementedCritical("Mode not supported.");
                return false;
            }

            if (request.IsTickBased)
            {
                SystemMonitor.NotImplementedCritical("Mode not supported.");
                return false;
            }

            List<DataBar> bars = new List<DataBar>();
            if (request.StartIndex < 0)
            {
                for (int i = Math.Max(0, _sourceDataBarProvider.BarCount - request.MaxValuesRetrieved.Value);
                    i < _sourceDataBarProvider.BarCount; i++)
                {
                    lock (_sourceDataBarProvider)
                    {
                        bars.Add(_sourceDataBarProvider.BarsUnsafe[i]);
                    }
                }
            }
            else
            {
                for (int i = request.StartIndex.Value;
                    i < request.MaxValuesRetrieved && i < _sourceDataBarProvider.BarCount; i++)
                {
                    lock (_sourceDataBarProvider)
                    {
                        bars.Add(_sourceDataBarProvider.BarsUnsafe[i]);
                    }
                }
            }

            if (this.DataHistoryUpdateEvent != null)
            {
                this.DataHistoryUpdateEvent(this, sessionInfo, new DataHistoryUpdate(request.Period, bars));
            }

            return true;
        }

    }
}
