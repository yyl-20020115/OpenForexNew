using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;

namespace CommonFinancial
{
    /// <summary>
    /// Contains historical trade dataDelivery in responce to a history request.
    /// </summary>
    [Serializable]
    public class DataHistoryUpdate
    {
        TimeSpan _period;
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Period
        {
            get { return _period; }
            set { _period = value; }
        }

        int? _availableHistorySize = null;
        /// <summary>
        /// How much bars of dataDelivery is available for delivery.
        /// </summary>
        public int? AvailableHistorySize
        {
            get { return _availableHistorySize; }
            set { _availableHistorySize = value; }
        }

        /// <summary>
        /// Bars, if any.
        /// </summary>
        List<DataBar> _dataBars = new List<DataBar>();
        public List<DataBar> DataBarsUnsafe
        {
            get { return _dataBars; }
        }

        /// <summary>
        /// Ticks, if any.
        /// </summary>
        List<DataTick> _dataTicks = new List<DataTick>();
        public List<DataTick> DataTicksUnsafe
        {
            get { return _dataTicks; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool BarDataAssigned
        {
            get { lock (this) { return _dataBars.Count > 0; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TickDataAssigned
        {
            get { lock (this) { return _dataTicks.Count > 0; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistoryUpdate()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistoryUpdate(TimeSpan period, IEnumerable<DataBar> bars)
        {
            _period = period;
            if (bars != null)
            {
                _dataBars.AddRange(bars);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistoryUpdate(TimeSpan period, IEnumerable<DataTick> ticks)
        {
            _period = TimeSpan.Zero;
            if (ticks != null)
            {
                _dataTicks.AddRange(ticks);
            }
        }
    }
}
