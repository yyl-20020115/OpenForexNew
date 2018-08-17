using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using Arbiter;

namespace ForexPlatform
{
    /// <summary>
    /// Allows to subscribe/unsubscribe to accountInfos of a source, and orders and position updates related to.
    /// </summary>
    [Serializable]
    public class SubscribeToSourceAccountsUpdatesMessage : RequestMessage
    {
        volatile bool _subscribe;
        public bool Subscribe
        {
            get { return _subscribe; }
            set { _subscribe = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public SubscribeToSourceAccountsUpdatesMessage(bool subscribe)
        {
            _subscribe = subscribe;
        }
    }
}
