using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Request a subscription for quote and / or bar history updates.
    /// </summary>
    [Serializable]
    public class DataSubscriptionRequestMessage : DataSessionRequestMessage
    {
        bool _subscribe = true;

        public bool Subscribe
        {
            get { return _subscribe; }
            set { _subscribe = value; }
        }

        DataSubscriptionInfo? _information = new DataSubscriptionInfo();
        /// <summary>
        /// If orderInfo is null (and sessionInformation orderInfo is empty), all subscriptions to this client will be removed.
        /// </summary>
        public DataSubscriptionInfo? Information
        {
            get { return _information; }
            set { _information = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSubscriptionRequestMessage(DataSessionInfo sessionInfo, bool subscribe, DataSubscriptionInfo? info)
            : base(sessionInfo)
        {
            _information = info;
            _subscribe = subscribe;
        }
    }
}
