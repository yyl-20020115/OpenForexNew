using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Class contains and manages bar dataDelivery, coming from a dataDelivery delivery.
    /// </summary>
    [Serializable]
    public sealed class DataBarHistoryProvider : Operational, IDataBarHistoryProvider, IDisposable
    {
        ISourceDataDelivery _dataDelivery;

        volatile int _defaultHistoryBarsCount = 10000;
        /// <summary>
        /// How many bars of history to provide by default.
        /// The default is limited to 100,000 to evade loading too high load on the system,
        /// altough it is possible to load and use up to 2,000,000 items or more.
        /// </summary>
        public int DefaultHistoryBarsCount
        {
            get { return _defaultHistoryBarsCount; }
            set 
            { 
                _defaultHistoryBarsCount = value;

                if (_dataDelivery != null && _sessionInfo.IsEmtpy == false)
                {
                    RuntimeDataSessionInformation session = _dataDelivery.GetSymbolRuntimeSessionInformation(_sessionInfo.Symbol);
                    if (session != null && session.AvailableDataBarPeriods.Contains(_period.Value))
                    {
                        _dataDelivery.RequestDataHistoryUpdate(_sessionInfo,
                            new DataHistoryRequest(_period.Value, _defaultHistoryBarsCount), false);
                    }
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        List<DataBar> _dataBars = new List<DataBar>();

        private int _dataBarsLimit = 16 * 1024 * 1024;

        public int DataBarLimit
        {
          get
          {
            return this._dataBarsLimit;
          }
          set
          {
            this._dataBarsLimit = value;
          }
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<DataBar> DataBars
        {
            get { return _dataBars; }
            set { _dataBars = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        DateTime _lastDataUpdate = DateTime.MinValue;

        /// <summary>
        /// Used when a matching of a given time to a bar dataDelivery is needed.
        /// Matches a time to an index of bar dataDelivery on this time.
        /// </summary>
        [NonSerialized]
        Dictionary<DateTime, int> _cachedDataBarIndexSearches = new Dictionary<DateTime, int>();

        /// <summary>
        /// 
        /// </summary>
        DateTime LastDataUpdate
        {
            get { return _lastDataUpdate; }
        }

        public int BarCount
        {
            get { return _dataBars.Count; }
        }

        TimeSpan? _period;
        public TimeSpan? Period
        {
            get
            {
                return _period;
            }
        }

        public DateTime? FirstBarTime
        {
            get
            {
                lock (this)
                {
                    if (_dataBars.Count > 0)
                    {
                        return _dataBars[0].DateTime;
                    }
                }

                return null;
            }
        }

        public DateTime? LastBarTime
        {
            get
            {
                lock (this)
                {
                    if (_dataBars.Count > 0)
                    {
                        return _dataBars[_dataBars.Count - 1].DateTime;
                    }
                }
                return null;
            }
        }

        public ReadOnlyCollection<DataBar> BarsUnsafe
        {
            get { return _dataBars.AsReadOnly(); }
        }

        public DataBar? Current
        {
            get
            {
                lock (this)
                {
                    if (_dataBars.Count == 0)
                    {
                        return null;
                    }

                    return _dataBars[_dataBars.Count - 1];
                }
            }
        }

        volatile IndicatorManager _indicators = null;
        /// <summary>
        /// 
        /// </summary>
        public IndicatorManager Indicators
        {
            get { return _indicators; }
        }

        DataSessionInfo _sessionInfo;
        /// <summary>
        /// 
        /// </summary>
        public DataSessionInfo DataSessionInfo
        {
            get { return _sessionInfo; }
        }

        [NonSerialized]
        DataBarFilter _barFilter;

        [field: NonSerialized]
        public event DataBarHistoryUpdateDelegate DataBarHistoryUpdateEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataBarHistoryProvider(ISourceDataDelivery dataDelivery, DataSessionInfo session, TimeSpan period, int? defaultHistoryBarsCount)
        {
            _sessionInfo = session;

            if (defaultHistoryBarsCount.HasValue)
            {
                _defaultHistoryBarsCount = defaultHistoryBarsCount.Value;
            }

            _dataDelivery = dataDelivery;
            _indicators = new IndicatorManager(this);
            _period = period;

            Construct();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            _dataBars = new List<DataBar>();
            _lastDataUpdate = DateTime.MinValue;
            _cachedDataBarIndexSearches = new Dictionary<DateTime, int>();

            Construct();
        }

        /// <summary>
        /// 
        /// </summary>
        void Construct()
        {
            _barFilter = new DataBarFilter(this);
            //_barFilter.Enabled = false;

            if (_dataDelivery == null)
            {
                return;
            }

            _dataDelivery.DataHistoryUpdateEvent += new DataHistoryUpdateDelegate(_dataDelivery_DataHistoryUpdateDelegate);
            _dataDelivery.QuoteUpdateEvent += new QuoteUpdateDelegate(_dataDelivery_QuoteUpdateEvent);
            _dataDelivery.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_dataDelivery_OperationalStatusChangedEvent);
            
            if (_dataDelivery.OperationalState == OperationalStateEnum.Operational)
            {
                _dataDelivery_OperationalStatusChangedEvent(_dataDelivery, OperationalStateEnum.NotOperational);
            }

            _indicators.Initialize();

            StatusSynchronizationSource = _dataDelivery;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {
            if (_indicators != null)
            {
                _indicators.UnInitialize();
            }

            if (_dataDelivery != null)
            {
                _dataDelivery.DataHistoryUpdateEvent -= new DataHistoryUpdateDelegate(_dataDelivery_DataHistoryUpdateDelegate);
                _dataDelivery.QuoteUpdateEvent -= new QuoteUpdateDelegate(_dataDelivery_QuoteUpdateEvent);
            }
        }

        void _dataDelivery_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            if (operational.OperationalState != OperationalStateEnum.Operational)
            {
                return;
            }

            if (_sessionInfo.IsEmtpy)
            {
                SystemMonitor.OperationError("Data bar provider has no valid session assiged.");
                return;
            }

            // Re-map the session orderInfo.
            RuntimeDataSessionInformation information = _dataDelivery.GetSymbolRuntimeSessionInformation(_sessionInfo.Symbol);
            if (information == null)
            {
                SystemMonitor.OperationError("Failed to map session information for data provider.");
                _sessionInfo = DataSessionInfo.Empty;
                return;
            }
            else
            {
                _sessionInfo = information.Info;
            }

            if (_dataDelivery.SubscribeToData(_sessionInfo, true, new DataSubscriptionInfo(false, false, new TimeSpan[] { Period.Value })) == false)
            {
                SystemMonitor.OperationError("Failed to subscribe to bar data updates.");
                return;
            }

            RuntimeDataSessionInformation session = _dataDelivery.GetSymbolRuntimeSessionInformation(_sessionInfo.Symbol);
            if (session != null && session.AvailableDataBarPeriods.Contains(_period.Value))
            {
                _dataDelivery.RequestDataHistoryUpdate(_sessionInfo, new DataHistoryRequest(_period.Value, _defaultHistoryBarsCount), false);
            }
        }

        /// <summary>
        /// Create a dataDelivery bar based on quote.
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public DataBar UpdateCurrentBar(DataBar currentBar, Quote quote)
        {
            if (quote.Bid.HasValue == false || quote.Ask.HasValue == false)
            {
                return currentBar;
            }
            
            currentBar.Low = Math.Min(quote.Bid.Value, currentBar.Low);
            currentBar.High = Math.Max(quote.Bid.Value, currentBar.High);

            currentBar.Close = quote.Bid.Value;

            if (quote.Volume.HasValue)
            {
                currentBar.Volume = quote.Volume.Value;
            }

            return currentBar;
        }

        public DataBar? BarFromQuote(DataBar? previousBar, DateTime time, Quote quote)
        {
            if ((quote.IsFullySet == false && previousBar.HasValue == false)
                || (quote.Ask.HasValue == false || quote.Bid.HasValue == false))
            {// We need at least previous bar if quote is not fully set.
                return null;
            }

            decimal open = quote.Open.HasValue ? quote.Open.Value : previousBar.Value.Close;
            decimal close = quote.Bid.Value;

            DataBar result = new DataBar(time,
                open,
                Math.Max(open, close),
                Math.Min(open, close),
                close,
                0);

            return result;
        }

        /// <summary>
        /// Follow quotes update.
        /// </summary>
        void _dataDelivery_QuoteUpdateEvent(ISourceDataDelivery dataDelivery, DataSessionInfo session, Quote? quote)
        {
            if (_sessionInfo.Equals(session) == false || _period.HasValue == false
                || quote.HasValue == false)
            {
                return;
            }

            DataBar? lastBar = null;
            if (_dataBars.Count > 0)
            {
                lastBar = _dataBars[_dataBars.Count - 1];
            }
            else
            {// If there are no existing bars, we do not operate, to evade mixing up start / end period etc.
                return;
            }

            TimeSpan? period = _period;

            DataBarUpdateType? updateType = null;

            // The time of a bar is its open time, so everything after a "period" closeVolume of time after it is part of that bar.
            if (lastBar.Value.DateTime + period < quote.Value.Time)
            {// We need to append a new bar.
                DateTime newBarDateTime = lastBar.Value.DateTime;
                int i = 0;
                while (newBarDateTime.Add(period.Value) < quote.Value.Time)
                {// Establish end time of new bar (max 1000 steps to evade potential lockups).
                    newBarDateTime = newBarDateTime.Add(period.Value);
                    i++;

                    if (i > 1000)
                    {// We have tried and failed to establish proper new period so just abort.
                        SystemMonitor.OperationError("We have tried and failed to establish proper new period so quote aborted.");
                        return;
                    }
                }

                DataBar? newBar = BarFromQuote(lastBar, newBarDateTime, quote.Value);
                if (newBar.HasValue == false)
                {// Failed to establish bar from quote.
                    return;
                }

                updateType = DataBarUpdateType.NewPeriod;

                lock (this)
                {// Add the new bar.
                    _lastDataUpdate = DateTime.Now;
                    _dataBars.Add(newBar.Value);

                  //NOTICE: guard for size
                    if (_dataBars.Count > this.DataBarLimit * 2)
                    {
                      this._dataBars.RemoveRange(0, this._dataBars.Count - this.DataBarLimit);
                    }
                }
            }
            else
            {
                updateType = DataBarUpdateType.CurrentBarUpdate;

                lock (this)
                {// Just update the current last bar.
                    _lastDataUpdate = DateTime.Now;
                    _dataBars[_dataBars.Count - 1] = UpdateCurrentBar(lastBar.Value, quote.Value);
                }
            }

            if (updateType.HasValue && DataBarHistoryUpdateEvent != null)
            {
                DataBarHistoryUpdateEvent(this, updateType.Value, 1);
            }

        }

        /// <summary>
        /// Data delivery has received a dataDelivery update.
        /// </summary>
        void _dataDelivery_DataHistoryUpdateDelegate(ISourceDataDelivery dataDelivery, DataSessionInfo session, DataHistoryUpdate update)
        {
            if (_sessionInfo.Equals(session) == false)
            {
                return;
            }

            if (update.Period != _period.Value)
            {// This update is aimed at another provider.
                return;
            }

            DataBarUpdateType? updateType = null;

            if (_dataBars.Count == 0)
            {
                updateType = DataBarUpdateType.Initial;
            }

            _barFilter.FilterUpdate(dataDelivery, session, update);

            int updatedBarsCount = 0;
            lock (this)
            {
                _lastDataUpdate = DateTime.Now;

                // Add new bars - search for new later bars.
                for (int i = 0; i < update.DataBarsUnsafe.Count; i++)
                {
                    if (_dataBars.Count == 0 || update.DataBarsUnsafe[i].DateTime > _dataBars[_dataBars.Count - 1].DateTime)
                    {
                        if (updateType.HasValue == false)
                        {
                            updateType = DataBarUpdateType.HistoryUpdate;
                        }

                        updatedBarsCount++;

                      //NOTICE: clean at double space

                        if (_dataBars.Count >= this.DataBarLimit * 2)
                        {
                          for (int u = 0; u < _dataBars.Count - this.DataBarLimit; u++)
                          {
                            _cachedDataBarIndexSearches.Remove(_dataBars[u].DateTime);
                          }
                          _dataBars.RemoveRange(0, _dataBars.Count - this.DataBarLimit);
                        }

                      _dataBars.Add(update.DataBarsUnsafe[i]);

                        _cachedDataBarIndexSearches.Add(update.DataBarsUnsafe[i].DateTime, _dataBars.Count - 1);
                    }
                }

                bool preTimeUpdate = false;
                // Add new bars - search for previous bars - a separate cycle needed since we need to move backwards on this.
                for (int i = update.DataBarsUnsafe.Count - 1; i >= 0; i--)
                {
                    if (_dataBars.Count > 0 && update.DataBarsUnsafe[i].DateTime < _dataBars[0].DateTime)
                    {// This is a bar from previous history, we do not know about - insert first place.
                        _dataBars.Insert(0, update.DataBarsUnsafe[i]);
                        preTimeUpdate = true;
                    }
                }

                // Also check the last 5 units for any requotes that might have been sent,
                // this happens when price changes and we get updates for the last unit.
                for (int i = 0; i < 5 && update.DataBarsUnsafe.Count - 1 - i > 0 && _dataBars.Count - 1 - i > 0; i++)
                {
                    if (update.DataBarsUnsafe[update.DataBarsUnsafe.Count - 1 - i].DateTime == _dataBars[_dataBars.Count - 1 - i].DateTime
                        /*&& update.DataBarsUnsafe[update.DataBarsUnsafe.Count - 1 - i].Equals(_dataBars[_dataBars.Count - 1 - i]) == false*/)
                    {
                        updatedBarsCount++;

                        // Since this update is only when the date times are the same, the helper cache dictionary needs not be updated.
                        _dataBars[_dataBars.Count - 1 - i] = update.DataBarsUnsafe[update.DataBarsUnsafe.Count - 1 - i];

                        if (updateType.HasValue == false)
                        {
                            updateType = DataBarUpdateType.HistoryUpdate;
                        }
                    }
                }

                if (preTimeUpdate)
                {// Make a full update if we have inserted something in the beggining.
                    updateType = DataBarUpdateType.HistoryUpdate;
                    updatedBarsCount = _dataBars.Count;
                }
            }

            if (updateType.HasValue && DataBarHistoryUpdateEvent != null)
            {
                DataBarHistoryUpdateEvent(this, updateType.Value, updatedBarsCount);
            }
        }


        #region IDataBarHistoryProvider Members

        public decimal[] GetValues(DataBar.DataValueEnum valueEnum)
        {
            return GetValues(valueEnum, 0, int.MaxValue);
        }

        public decimal[] GetValues(DataBar.DataValueEnum valueEnum, int startingIndex, int indexCount)
        {
            decimal[] result;
            lock (this)
            {
                int count = indexCount;
                if (count == 0)
                {
                    count = _dataBars.Count - startingIndex;
                    GeneralHelper.Verify(count >= 0, "Invalid starting index.");
                }

                result = new decimal[Math.Min(count, _dataBars.Count)];
                for (int i = startingIndex; i < startingIndex + count && i < _dataBars.Count; i++)
                {
                    result[i - startingIndex] = _dataBars[i].GetValue(valueEnum);
                }
            }

            return result;
        }

        public double[] GetValuesAsDouble(DataBar.DataValueEnum valueEnum)
        {
            return GetValuesAsDouble(valueEnum, 0, int.MaxValue);
        }

        public double[] GetValuesAsDouble(DataBar.DataValueEnum valueEnum, int startingIndex, int indexCount)
        {
            double[] result;
            lock (this)
            {
                int count = indexCount;
                if (count == 0)
                {
                    count = _dataBars.Count - startingIndex;
                    GeneralHelper.Verify(count >= 0, "Invalid starting index.");
                }

                result = new double[Math.Min(count, _dataBars.Count)];
                for (int i = startingIndex; i < startingIndex + count && i < _dataBars.Count; i++)
                {
                    result[i - startingIndex] = (double)_dataBars[i].GetValue(valueEnum);
                }
            }

            return result;
        }

        /// <summary>
        /// Obtain the index of the dataDelivery bar, with the given time.
        /// Time gaps must be considered.
        /// Result is -1 to indicate not found.
        /// Since this baseMethod is called very extensively on drawing, it employes a caching mechanism.
        /// </summary>
        public int GetIndexAtTime(DateTime time)
        {
            lock (this)
            {
                if (_dataBars.Count == 0 && _dataDelivery == null
                    && (_period.HasValue == false ||
                        _period.Value == TimeSpan.Zero))
                {
                    return 0;
                }

                if (_cachedDataBarIndexSearches.ContainsKey(time))
                {
                    return _cachedDataBarIndexSearches[time];
                }

                TimeSpan difference = (time - _dataBars[0].DateTime);
                if (difference.TotalSeconds < 0)
                {// Before time.
                    return -1;
                }
                else if (difference.TotalSeconds <= _period.Value.TotalSeconds)
                {// 0 index.
                    _cachedDataBarIndexSearches.Add(time, 0);
                    return 0;
                }

                // Estimated index has skipped time gaps. so now start looking back from it to find the proper period.
                int estimatedIndex = 1 + (int)Math.Floor(difference.TotalSeconds / _period.Value.TotalSeconds);
                estimatedIndex = Math.Min(_dataBars.Count - 1, estimatedIndex);
                for (int i = estimatedIndex; i >= 1; i--)
                {
                    if (_dataBars[i - 1].DateTime <= time
                        && _dataBars[i].DateTime >= time)
                    {
                        _cachedDataBarIndexSearches.Add(time, i);
                        return i;
                    }
                }
            }
            return -1;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            UnInitialize();
            ChangeOperationalState(OperationalStateEnum.Disposed);

            _dataBars = null;
            _dataDelivery = null;
            _indicators = null;
            _period = null;
        }

        #endregion
    }
}
