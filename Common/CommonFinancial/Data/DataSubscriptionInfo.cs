using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Information regarding a given dataDelivery subscription.
    /// </summary>
    [Serializable]
    public struct DataSubscriptionInfo
    {
        volatile bool _quoteSubscription;
        /// <summary>
        /// 
        /// </summary>
        public bool QuoteSubscription
        {
            get { return _quoteSubscription; }
            set { _quoteSubscription = value; }
        }

        volatile bool _tickSubscription;
        /// <summary>
        /// 
        /// </summary>
        public bool TickSubscription
        {
            get { return _tickSubscription; }
            set { _tickSubscription = value; }
        }

        ListUnique<TimeSpan> _dataBarSubscriptions;
        /// <summary>
        /// 
        /// </summary>
        public ListUnique<TimeSpan> DataBarSubscriptions
        {
            get { return _dataBarSubscriptions; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DataSubscriptionInfo Empty
        {
            get
            {
                DataSubscriptionInfo info = new DataSubscriptionInfo(false, false, new TimeSpan[] { });
                return info;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (TickSubscription == false && QuoteSubscription == false && DataBarSubscriptions.Count == 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSubscriptionInfo(bool quoteSubscription, bool tickSubsription, TimeSpan[] dataBarSubscriptions)
        {
            _quoteSubscription = quoteSubscription;
            
            _tickSubscription = tickSubsription;

            _dataBarSubscriptions = new ListUnique<TimeSpan>();
            
            if (dataBarSubscriptions != null)
            {
                _dataBarSubscriptions.AddRange(dataBarSubscriptions);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DataSubscriptionInfo Combine(DataSubscriptionInfo info, bool subscribeCombine, DataSubscriptionInfo otherInfo)
        {
            if (subscribeCombine)
            {// Combination mode.
                info._tickSubscription = info._tickSubscription || otherInfo._tickSubscription;
                info._quoteSubscription = info._quoteSubscription || otherInfo._quoteSubscription;
                info._dataBarSubscriptions.AddRange(otherInfo._dataBarSubscriptions);
            }
            else
            {// Substraction mode.
                info._tickSubscription = info._tickSubscription && info._tickSubscription == false;
                info._quoteSubscription = info._quoteSubscription && info._quoteSubscription == false;
                info._dataBarSubscriptions.RemoveRange(otherInfo._dataBarSubscriptions);
            }

            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responce"></param>
        /// <returns></returns>
        public bool AcceptsUpdate(Quote? quote)
        {
            return _quoteSubscription;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AcceptsUpdate(DataHistoryUpdate response)
        {
            if (response.BarDataAssigned && _dataBarSubscriptions.Contains(response.Period))
            {
                return true;
            }

            if (response.TickDataAssigned && _tickSubscription)
            {
                return true;
            }

            return false;
        }
        
    }
}
