using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;

namespace CommonFinancial
{
    /// <summary>
    /// Class provides additional runtime session information, adding to the Info by default.
    /// </summary>
    [Serializable]
    public class RuntimeDataSessionInformation
    {
        DataSessionInfo _info;
        /// <summary>
        /// Core identifiable session information.
        /// </summary>
        public DataSessionInfo Info
        {
            get { return _info; }
        }

        List<TimeSpan> _availableDataBarPeriods = new List<TimeSpan>();
        /// <summary>
        /// 
        /// </summary>
        public List<TimeSpan> AvailableDataBarPeriods
        {
            get { return _availableDataBarPeriods; }
        }

        volatile bool _quotesAvailable;
        /// <summary>
        /// 
        /// </summary>
        public bool QuotesAvailable
        {
            get { return _quotesAvailable; }
            set { _quotesAvailable = value; }
        }

        volatile bool _tickDataAvailabe;
        /// <summary>
        /// 
        /// </summary>
        public bool TickDataAvailabe
        {
            get { return _tickDataAvailabe; }
            set { _tickDataAvailabe = value; }
        }

        [NonSerialized]
        object _tag = null;

        /// <summary>
        /// 
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RuntimeDataSessionInformation(DataSessionInfo info)
        {
            _info = info;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RuntimeDataSessionInformation(DataSessionInfo info, IEnumerable<TimeSpan> spans)
        {
            _info = info;
            _availableDataBarPeriods.AddRange(spans);
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="spans"></param>
        public RuntimeDataSessionInformation(DataSessionInfo info, TimeSpan period)
        {
            _info = info;
            _availableDataBarPeriods.Add(period);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualAvailableDataBarPeriods(RuntimeDataSessionInformation other)
        {
            if (other.AvailableDataBarPeriods.Count != AvailableDataBarPeriods.Count)
            {
                return false;
            }

            for (int i = 0; i < other.AvailableDataBarPeriods.Count; i++)
            {
                if (AvailableDataBarPeriods[i] != other.AvailableDataBarPeriods[i])
                {
                    return false;
                }
            }

            return true;
        }

        //#region IComparable<Info> Members

        //public int CompareTo(Info other)
        //{
        //    return _info.CompareTo(other);
        //}

        //#endregion
    }
}
