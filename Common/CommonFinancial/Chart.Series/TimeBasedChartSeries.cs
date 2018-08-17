using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Time based series must consider time gaps.
    /// </summary>
    [Serializable]
    public abstract class TimeBasedChartSeries : ChartSeries
    {
        volatile bool _showTimeGaps = false;
        /// <summary>
        /// If enabled, time gaps are shown but only on zoom 1 (units unification = 1).
        /// </summary>
        public bool ShowTimeGaps
        {
            get { return _showTimeGaps; }
            set { _showTimeGaps = value; }
        }

        protected TimeSpan? _period;
        public TimeSpan? Period
        {
            get { lock (this) { return _period; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeBasedChartSeries(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="context"></param>
        public TimeBasedChartSeries(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info.GetBoolean("periodHasValue"))
            {
                _period = (TimeSpan)info.GetValue("period", typeof(TimeSpan));
            }
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("periodHasValue", _period.HasValue);
            if (_period.HasValue)
            {
                info.AddValue("period", _period.Value);
            }
        }

        /// <summary>
        /// This is only valid for time based series.
        /// </summary>
        public abstract DateTime? GetTimeAtIndex(int index);

    }
}
