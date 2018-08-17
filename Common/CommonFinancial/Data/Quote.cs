using System;
using System.Collections.Generic;
using System.Text;

namespace CommonFinancial
{
    /// <summary>
    /// A single market quote
    /// </summary>
    [Serializable]
    public struct Quote
    {
        /// <summary>
        /// Current ask level. May be NaN(MinValue) when no value established.
        /// </summary>
        private Decimal? _ask;

        public Decimal? Ask
        {
            get { return _ask; }
            set { _ask = value; }
        }

        /// <summary>
        /// Current bid level. May be NaN(MinValue) when no value established.
        /// </summary>
        Decimal? _bid;

        public Decimal? Bid
        {
            get { return _bid; }
            set { _bid = value; }
        }

        Decimal? _volume;

        /// <summary>
        /// 
        /// </summary>
        public Decimal? Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        Decimal? _open;
        /// <summary>
        /// 
        /// </summary>
        public Decimal? Open
        {
            get { return _open; }
            set { _open = value; }
        }

        Decimal? _high;
        /// <summary>
        /// 
        /// </summary>
        public Decimal? High
        {
            get { return _high; }
            set { _high = value; }
        }

        Decimal? _low;
        /// <summary>
        /// 
        /// </summary>
        public Decimal? Low
        {
            get { return _low; }
            set { _low = value; }
        }

        /// <summary>
        /// Current spread level. May be NaN(MinValue) when no value established.
        /// </summary>
        public Decimal? Spread 
        { 
            get
            {
                decimal? ask = _ask;
                decimal? bid = _bid;
             
                if (ask.HasValue && bid.HasValue)
                {
                    return ask.Value - bid.Value;
                }
                else
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// The current time on the quotation provider.
        /// </summary>
        DateTime? _time;

        public DateTime? Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public bool IsFullySet
        {
            get
            {
                return _ask.HasValue && _bid.HasValue && _high.HasValue && _low.HasValue && _open.HasValue
                    && _time.HasValue && _volume.HasValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Quote(decimal? ask, decimal? bid, decimal? volume, DateTime? time)
        {
            _ask = ask;
            _bid = bid;
            _volume = volume;
            _time = time;

            _low = null;
            _high = null;
            _open = null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public Quote(decimal? ask, decimal? bid, decimal? open, decimal? close, decimal? high, decimal? low,
            decimal? volume, DateTime? time)
        {
            _ask = ask;
            _bid = bid;
            _high = high;
            _low = low;
            _open = open;
            _volume = volume;
            _time = time;
        }

        /// <summary>
        /// Special constructor allows to build a quote from data bar.
        /// </summary>
        /// <param name="bar"></param>
        public Quote(DataBar bar, decimal spread)
        {
            _ask = bar.Close;
            _bid = bar.Close - spread;
            _volume = bar.Volume;
            _time = bar.DateTime;
            
            _low = null;
            _high = null;
            _open = null;
        }
    }
}
