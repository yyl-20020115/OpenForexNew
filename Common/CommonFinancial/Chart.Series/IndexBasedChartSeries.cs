using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Base class for chart series that are index based (as opposed to time based series).
    /// </summary>
    [Serializable]
    public abstract class IndexBasedChartSeries : ChartSeries
    {
        /// <summary>
        /// 
        /// </summary>
        public IndexBasedChartSeries()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public IndexBasedChartSeries(string name) 
            : base(name)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public IndexBasedChartSeries(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
