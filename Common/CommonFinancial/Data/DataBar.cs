using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Struct is the price dataDelivery for a given price bar. A bar always has a time, 
    /// bars can be separated by fixed time periods, or by variable ones.
    /// </summary>
    [Serializable]
    public struct DataBar : IComparable<DataBar>
    {
        public enum DataValueEnum
        {
            High,
            Low,
            Average,
            Open,
            Volume,
            Close,
            BodyHeight,
        }

        long _dateTime;
        /// <summary>
        /// The Date and Time the bar was *opened*.
        /// </summary>
        public DateTime DateTime
        {
            get { return DateTime.FromFileTime(_dateTime); }
            set { _dateTime = value.ToFileTime(); }
        }

        public decimal Average
        {
            get
            {
                decimal sum = Open + Low + High + Close;
                return sum / 4m;
            }
        }

        public decimal AverageOpenClose
        {
            get
            {
                decimal sum = Open + Close;
                return sum / 2m;
            }
        }

        decimal _open;
        /// <summary>
        /// The (ask) price the bar was opened.
        /// </summary>
        public decimal Open
        {
            get { return _open; }
            set { _open = value; }
        }
        
        decimal _close;
        /// <summary>
        /// The (ask) price the bar was closed.
        /// </summary>
        public decimal Close
        {
            get { return _close; }
            set { _close = value; }
        }
        
        decimal _low;
        /// <summary>
        /// The lowest price in the bar.
        /// </summary>
        public decimal Low
        {
            get { return _low; }
            set { _low = value; }
        }
        
        decimal _high;
        /// <summary>
        /// The highest price in the bar.
        /// </summary>
        public decimal High
        {
            get { return _high; }
            set { _high = value; }
        }

        decimal _volume;
        /// <summary>
        /// Volume.
        /// </summary>
        public decimal Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasDataValues
        {
            get { return _open != 0 && _close != 0 && _low != 0 && _high != 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal LowerOpenClose
        {
            get
            {
                return Math.Min(Open, Close);
            }
        }

        /// <summary>
        /// The higher value of Open or close.
        /// </summary>
        public decimal HigherOpenClose
        {
            get
            {
                return Math.Max(Open, Close);
            }
        }

        /// <summary>
        /// Absolute height of the top shadow.
        /// </summary>
        public decimal TopShadow
        {
            get
            {
                return High - Math.Max(Open, Close);
            }
        }

        public decimal BarBottomShadowLength
        {
            get
            {
                return Math.Min(Open, Close) - Low;
            }
        }

        /// <summary>
        /// Absolute height (high to low) of the bar.
        /// </summary>
        public decimal AbsoluteHeight
        {
            get
            {
                return Math.Abs(High - Low);
            }
        }

        /// <summary>
        /// Did the bar close higher that it opened.
        /// </summary>
        public bool IsRising
        {
            get
            {
                return Close > Open;
            }
        }

        /// <summary>
        /// Height of the bars body (close to open).
        /// </summary>
        public decimal BodyHeight
        {
            get
            {
                return Close - Open;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal AbsoluteBodyHeight
        {
            get
            {
                return Math.Abs(Open - Close);
            }
        }

        /// <summary>
        /// Is the current databar empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.CompareTo(Empty) == 0;
            }
        }

        static DataBar _empty = new DataBar();
        /// <summary>
        /// This generates an empty dataDelivery bar.
        /// </summary>
        public static DataBar Empty
        {
            get
            {
                return _empty;
            }
        }

        /// <summary>
        /// Construct empty BarData, for this dateTime.
        /// </summary>
        public DataBar(DataBar other)
        {
            _dateTime = other._dateTime;
            _volume = other._volume;
            _open = other._open;
            _close = other._close;
            _high = other._high;
            _low = other._low;
        }

        /// <summary>
        /// Construct empty BarData, for this dateTime.
        /// </summary>
        public DataBar(DateTime dateTime)
        {
            _dateTime = dateTime.ToFileTime();
            _volume = decimal.MinValue;
            _open = decimal.MinValue;
            _close = decimal.MinValue;
            _high = decimal.MinValue;
            _low = decimal.MinValue;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataBar(DateTime dateTime, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            _dateTime = dateTime.ToFileTime();
            _open = open;
            _close = close;
            _low = low;
            _high = high;
            _volume = volume;
        }

        /// <summary>
        /// The bar will encompas the values of the input ones, extending its values where needed.
        /// </summary>
        static public DataBar CombinedBar(DataBar[] bars)
        {
            DataBar result = new DataBar(bars[0]);

            decimal _volumeSum = 0;
            foreach(DataBar bar in bars)
            {
                result._low = Math.Min(bar.Low, result.Low);
                result._high = Math.Max(bar.High, result.High);
                _volumeSum += bar.Volume;
            }

            result._close = bars[bars.Length - 1].Close;
            result._volume = _volumeSum / (decimal)bars.Length;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal GetValue(DataValueEnum valueSource)
        {
            if (HasDataValues == false)
            {
                return decimal.MinValue;
            }

            switch (valueSource)
            {
                case DataValueEnum.High:
                    return this.High;
                case DataValueEnum.Low:
                    return this.Low;
                case DataValueEnum.Average:
                    return this.Average;
                case DataValueEnum.Open:
                    return this.Open;
                case DataValueEnum.Volume:
                    return this.Volume;
                case DataValueEnum.Close:
                    return this.Close;
                case DataValueEnum.BodyHeight:
                    return this.BodyHeight;
            }

            SystemMonitor.NotImplementedWarning("Value enum type not known [" + valueSource.ToString() + "]");
            return 0;
        }

        public string[] ToStrings()
        {
            return new string[] { DateTime.FromFileTime(_dateTime).ToString(), Open.ToString(), Close.ToString(), Low.ToString(), High.ToString(), Volume.ToString() };
        }

        public override string ToString()
        {
            return base.ToString() + "[DateTime: " + DateTime.FromFileTime(_dateTime).ToString() + " Open: " + Open + " Close: " + Close + " Min: " + Low + " Max: " + High + " Volume: " + _volume + "]";
        }


        public static DataBar[] GenerateTestSeries(int length)
        {
            DataBar[] result = new DataBar[length];
            for (int i = 0; i < result.Length; i++)
            {
                if (i == 0)
                {
                    result[i] = new DataBar(DateTime.Now, GeneralHelper.Random(50), GeneralHelper.Random(50), GeneralHelper.Random(50), GeneralHelper.Random(50), 10);
                }
                else
                {
                    decimal open = result[i - 1].Close + GeneralHelper.Random(-2, 3);
                    decimal close = open + GeneralHelper.Random(-30, 31);
                    result[i] = new DataBar(result[i - 1].DateTime + TimeSpan.FromSeconds(1), open, Math.Max(open, close) + GeneralHelper.Random(10), Math.Min(open, close) - GeneralHelper.Random(15), close, 10);
                }

            }

            return result;
        }


        #region IComparable<DataBar> Members

        /// <summary>
        /// Compare 2 instances of DataBar structure.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(DataBar other)
        {
            int result = _dateTime.CompareTo(other._dateTime);
            if (result != 0)
            {
                return result;
            }

            result = _volume.CompareTo(other._volume);
            if (result != 0)
            {
                return result;
            }

            result = _open.CompareTo(other._open);
            if (result != 0)
            {
                return result;
            }

            result = _close.CompareTo(other._close);
            if (result != 0)
            {
                return result;
            }

            result = _high.CompareTo(other._high);
            if (result != 0)
            {
                return result;
            }

            return _low.CompareTo(other._low);
        }

        #endregion
    }
}
