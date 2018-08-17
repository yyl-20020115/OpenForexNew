using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// One tick of trading dataDelivery.
    /// </summary>
    [Serializable]
    public struct DataTick
    {
        public enum DataValueEnum
        {
            Ask,
            Bid,
            Volume
        }

        long _dateTime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTime
        {
            get { return DateTime.FromFileTime(_dateTime); }
            set { _dateTime = value.ToFileTime(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal Ask { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Bid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Provide enum based access to values.
        /// </summary>
        public decimal GetValue(DataValueEnum value)
        {
            switch (value)
            {
                case DataValueEnum.Ask:
                    return Ask;
                case DataValueEnum.Bid:
                    return Bid;
                case DataValueEnum.Volume:
                    return Volume;
            }

            SystemMonitor.Error("Value [" + value.ToString() + "] not recognized.");

            return 0;
        }
    }
}
