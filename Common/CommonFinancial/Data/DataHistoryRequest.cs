using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// A universal request to retrieve dataDelivery history.
    /// </summary>
    [Serializable]
    public struct DataHistoryRequest
    {
        TimeSpan _period;
        public TimeSpan Period
        {
            get { return _period; }
            set { _period = value; }
        }

        int? _startIndex;
        /// <summary>
        /// TODO: NOT CURRENTLY USED, APPLY
        /// Where to start the delivery operation.
        /// If -1, means start at LAST bar/tick (latest date time).
        /// If 0, means start at FIRST bar/tick (earliest date time).
        /// If value, means start at this index, counted from the first bar/tick.
        /// </summary>
        public int? StartIndex
        {
            get { return _startIndex; }
            set { _startIndex = value; }
        }

        int? _maxValuesRetrieved;
        /// <summary>
        /// Maximum number of items to be retrieved.
        /// </summary>
        public int? MaxValuesRetrieved
        {
            get { return _maxValuesRetrieved; }

            set 
            { 
                _maxValuesRetrieved = value;

                if (_maxValuesRetrieved.HasValue && _maxValuesRetrieved == 0)
                {
                    SystemMonitor.OperationError("Requesting at least one data history bar.");
                    _maxValuesRetrieved = 1;
                }
            }
        }

        public bool IsMinuteBased
        {
            get
            {
                return _period.TotalMinutes >= 1 && _period.TotalDays < 1;
            }
        }

        public bool IsDayBased
        {
            get
            {
                return _period.TotalDays >= 1;
            }
        }

        public bool IsTickBased
        {
            get
            {
                return _period == TimeSpan.Zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistoryRequest(TimeSpan period, int? maxValueCount)
        {
            _startIndex = null;
            _period = period;
            _maxValuesRetrieved = maxValueCount.HasValue ? maxValueCount.Value : int.MaxValue;
        }
    }
}
