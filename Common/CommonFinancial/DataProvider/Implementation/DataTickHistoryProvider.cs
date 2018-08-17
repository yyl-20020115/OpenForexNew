using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Manages and stores dataDelivery ticks coming in from a dataDelivery delivery.
    /// </summary>
    [Serializable]
    public class DataTickHistoryProvider : Operational, IDataTickHistoryProvider
    {
        ISourceDataDelivery _dataDelivery;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        protected List<DataTick> _dataTicks = new List<DataTick>();

        /// <summary>
        /// Caching, see _cachedDataBarIndexSearches.
        /// </summary>
        [NonSerialized]
        Dictionary<DateTime, int> _cachedDataTickIndexSearches = new Dictionary<DateTime, int>();

        [field: NonSerialized]
        public event DataTickHistoryUpdateDelegate DataTickHistoryUpdateEvent;

        DateTime _lastDataUpdate = DateTime.MinValue;

        DataSessionInfo _session;

        /// <summary>
        /// 
        /// </summary>
        public DataTickHistoryProvider(ISourceDataDelivery dataDelivery, DataSessionInfo session)
        {
            _dataDelivery = dataDelivery;
            _session = session;

            Initialize();
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            _dataTicks = new List<DataTick>();
            Initialize();
        }

        void Initialize()
        {
            if (_dataDelivery != null)
            {
                _dataDelivery.DataHistoryUpdateEvent += new DataHistoryUpdateDelegate(_dataDelivery_DataHistoryUpdateDelegate);

                _dataDelivery.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_dataDelivery_OperationalStatusChangedEvent);
                if (_dataDelivery.OperationalState == OperationalStateEnum.Operational)
                {
                    _dataDelivery_OperationalStatusChangedEvent(_dataDelivery, OperationalStateEnum.NotOperational);
                }
            }

        }

        void _dataDelivery_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            if (operational.OperationalState != OperationalStateEnum.Operational)
            {
                return;
            }

            RuntimeDataSessionInformation info = _dataDelivery.GetSymbolRuntimeSessionInformation(_session.Symbol);

            if (info != null)
            {
                if (info.TickDataAvailabe)
                {
                    _dataDelivery.RequestDataHistoryUpdate(_session, new DataHistoryRequest(TimeSpan.Zero, null), false);
                }
            }
            else
            {
                SystemMonitor.OperationError("Failed to estblish runtime data session information.");
            }

        }

        public void UnInitialize()
        {
            if (_dataDelivery != null)
            {
                _dataDelivery.DataHistoryUpdateEvent -= new DataHistoryUpdateDelegate(_dataDelivery_DataHistoryUpdateDelegate);
            }
        }

        void _dataDelivery_DataHistoryUpdateDelegate(ISourceDataDelivery dataDelivery, DataSessionInfo session, DataHistoryUpdate update)
        {
            if (session.Equals(_session) == false || update.DataTicksUnsafe.Count == 0) 
            {
                return;
            }

            DataTickUpdateType? updateType = null;
            if (_dataTicks.Count == 0)
            {
                updateType = DataTickUpdateType.HistoryUpdate;
            }

            lock (this)
            {
                _lastDataUpdate = DateTime.Now;

                for (int i = 0; i < update.DataTicksUnsafe.Count; i++)
                {
                    if (_dataTicks.Count == 0 || update.DataTicksUnsafe[i].DateTime > _dataTicks[_dataTicks.Count - 1].DateTime)
                    {
                        if (updateType.HasValue == false)
                        {
                            updateType = DataTickUpdateType.HistoryUpdate;
                        }

                        _dataTicks.Add(update.DataTicksUnsafe[i]);
                        _cachedDataTickIndexSearches.Add(update.DataTicksUnsafe[i].DateTime, i);
                    }
                }

                // Also check the last 5 units for any requotes that might have been sent,
                // this happens when price changes and we get updates for the last unit.
                for (int i = 0; i < 5 && update.DataTicksUnsafe.Count - 1 - i > 0 && _dataTicks.Count - 1 - i > 0; i++)
                {
                    if (update.DataTicksUnsafe[update.DataTicksUnsafe.Count - 1 - i].DateTime == _dataTicks[_dataTicks.Count - 1 - i].DateTime
                        && update.DataTicksUnsafe[update.DataTicksUnsafe.Count - 1 - i].Equals(_dataTicks[_dataTicks.Count - 1 - i]) == false)
                    {
                        // Since this update is only when the date times are the same, the helper cache dictionary needs not be updated.
                        _dataTicks[_dataTicks.Count - 1 - i] = update.DataTicksUnsafe[update.DataTicksUnsafe.Count - 1 - i];

                        if (updateType.HasValue == false)
                        {
                            updateType = DataTickUpdateType.HistoryUpdate;
                        }
                    }
                }
            }

            if (updateType.HasValue && DataTickHistoryUpdateEvent != null)
            {
                DataTickHistoryUpdateEvent(this, updateType.Value);
            }
        }



        #region IDataTickHistoryProvider Members

        public int TickCount
        {
            get { return _dataTicks.Count; }
        }

        public DateTime? FirstTime
        {
            get
            {
                lock (this)
                {
                    if (_dataTicks.Count != 0)
                    {
                        return _dataTicks[0].DateTime;
                    }
                }

                return null;
            }
        }

        public DateTime? LastTime
        {
            get
            {
                lock (this)
                {
                    if (_dataTicks.Count != 0)
                    {
                        return _dataTicks[_dataTicks.Count - 1].DateTime;
                    }
                }

                return null;
            }
        }

        public ReadOnlyCollection<DataTick> TicksUnsafe
        {
            get { return _dataTicks.AsReadOnly(); }
        }

        public DataTick? Current
        {
            get
            {
                lock (this)
                {
                    if (_dataTicks.Count == 0)
                    {
                        return null;
                    }

                    return _dataTicks[_dataTicks.Count - 1];
                }
            }
        }

        public decimal[] GetValues(DataTick.DataValueEnum valueEnum)
        {
            return GetValues(valueEnum, 0, int.MaxValue);
        }

        public decimal[] GetValues(DataTick.DataValueEnum valueEnum, int startingIndex, int indexCount)
        {
            decimal[] result;
            lock (this)
            {
                int count = indexCount;
                if (count == 0)
                {
                    count = _dataTicks.Count - startingIndex;
                    GeneralHelper.Verify(count >= 0, "Invalid starting index.");
                }

                result = new decimal[Math.Min(count, _dataTicks.Count)];
                for (int i = startingIndex; i < startingIndex + count && i < _dataTicks.Count; i++)
                {
                    result[i - startingIndex] = _dataTicks[i].GetValue(valueEnum);
                }
            }

            return result;
        }

        public double[] GetValuesAsDouble(DataTick.DataValueEnum valueEnum)
        {
            return GetValuesAsDouble(valueEnum, 0, int.MaxValue);
        }

        public double[] GetValuesAsDouble(DataTick.DataValueEnum valueEnum, int startingIndex, int indexCount)
        {
            double[] result;
            lock (this)
            {
                int count = indexCount;
                if (count == 0)
                {
                    count = _dataTicks.Count - startingIndex;
                    GeneralHelper.Verify(count >= 0, "Invalid starting index.");
                }

                result = new double[Math.Min(count, _dataTicks.Count)];
                {
                    for (int i = startingIndex; i < startingIndex + count && i < _dataTicks.Count; i++)
                    {
                        result[i - startingIndex] = (double)_dataTicks[i].GetValue(valueEnum);
                    }
                }
            }

            return result;
        }

        public int GetIndexAtTime(DateTime time)
        {
            lock (this)
            {
                if (_dataTicks.Count == 0 && _dataDelivery == null)
                {
                    return 0;
                }

                if (_cachedDataTickIndexSearches.ContainsKey(time))
                {
                    return _cachedDataTickIndexSearches[time];
                }

                TimeSpan timeDifference = (time - _dataTicks[0].DateTime);
                if (timeDifference.TotalSeconds < 0)
                {// Before time.
                    return -1;
                }
                else if (timeDifference.TotalSeconds == 0)
                {// 0 index.
                    _cachedDataTickIndexSearches.Add(time, 0);
                    return 0;
                }

                // Search for estimated location, basing our criteria on the supposition ticks are statistically equally 
                // distributed over the entire time period.
                double estimatedTickLocation = 0;
                if (LastTime.HasValue && FirstTime.HasValue)
                {
                    estimatedTickLocation = timeDifference.TotalMilliseconds / (LastTime.Value - FirstTime.Value).TotalMilliseconds;
                }

                int estimatedIndex = (int)(_dataTicks.Count * estimatedTickLocation);
                if (_dataTicks.Count >= estimatedIndex || estimatedIndex < 1)
                {
                    return -1;
                }

                bool searchDirection = _dataTicks[estimatedIndex].DateTime < time;
                bool found = false;
                while (found == false && estimatedIndex > 1 && estimatedIndex < _dataTicks.Count - 1)
                {
                    found = _dataTicks[estimatedIndex].DateTime < time && _dataTicks[estimatedIndex - 1].DateTime > time;
                    estimatedIndex = searchDirection ? estimatedIndex + 1 : estimatedIndex - 1;
                }

                if (found)
                {
                    // TODO: some more precision can be added here if we check, whether it is closer to the current ot the previous/next one.

                    _cachedDataTickIndexSearches[time] = estimatedIndex;

                    return estimatedIndex;
                }
            }
            return -1;
        }
        
        #endregion
    }
}
