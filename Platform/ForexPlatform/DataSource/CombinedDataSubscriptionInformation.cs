using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using Arbiter;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Contains information of multiple subscriptions to a single sessionInformation.
    /// </summary>
    public class CombinedDataSubscriptionInformation
    {
        volatile RuntimeDataSessionInformation _sessionInformation;

        /// <summary>
        /// Runtime dataDelivery session information for this subscription.
        /// </summary>
        public RuntimeDataSessionInformation SessionInformation
        {
            get { return _sessionInformation; }
        }

        Dictionary<ArbiterClientId, KeyValuePair<TransportInfo, DataSubscriptionInfo>> _subscriptions = new Dictionary<ArbiterClientId, KeyValuePair<TransportInfo, DataSubscriptionInfo>>();
        /// <summary>
        /// A collection of all subscribtions, based on original sender Id. Make sure to lock before accessing.
        /// </summary>
        public Dictionary<ArbiterClientId, KeyValuePair<TransportInfo, DataSubscriptionInfo>> SubscriptionsUnsafe
        {
            get { lock (this) { return _subscriptions; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public CombinedDataSubscriptionInformation(RuntimeDataSessionInformation sessionInformation)
        {
            _sessionInformation = sessionInformation;
        }

        /// <summary>
        /// Combines all required subscriptions by all members together in one orderInfo.
        /// </summary>
        public DataSubscriptionInfo GetCombinedDataSubscription()
        {
            DataSubscriptionInfo result = DataSubscriptionInfo.Empty;
            lock(this)
            {
                foreach (KeyValuePair<TransportInfo, DataSubscriptionInfo> pair in _subscriptions.Values)
                {
                    result = DataSubscriptionInfo.Combine(result, true, pair.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void HandleRequest(TransportInfo transportInfo, bool subscribe, DataSubscriptionInfo? info)
        {
            if (transportInfo.OriginalSenderId.HasValue == false)
            {
                SystemMonitor.Error("Original sender not available in transport info.");
                return;
            }

            lock (this)
            {
                if (info.HasValue == false)
                {
                    _subscriptions.Remove(transportInfo.OriginalSenderId.Value);
                }
                else
                {
                    if (_subscriptions.ContainsKey(transportInfo.OriginalSenderId.Value))
                    {
                        _subscriptions[transportInfo.OriginalSenderId.Value] = new KeyValuePair<TransportInfo,DataSubscriptionInfo>(transportInfo, 
                            DataSubscriptionInfo.Combine(_subscriptions[transportInfo.OriginalSenderId.Value].Value, subscribe, info.Value));
                    }
                    else
                    {
                        _subscriptions[transportInfo.OriginalSenderId.Value] = new KeyValuePair<TransportInfo, DataSubscriptionInfo>(transportInfo, info.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FullUnsubscribe(ArbiterClientId? originalSenderId)
        {
            if (originalSenderId.HasValue == false)
            {
                SystemMonitor.Error("Original sender not available in transport info.");
                return false;
            }

            lock (this)
            {
                return _subscriptions.Remove(originalSenderId.Value);
            }
        }
    }
}
