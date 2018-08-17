using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SubscribeToOperationalStateChangesMessage : RequestMessage
    {
        bool _subscribe = true;
        public bool Subscribe
        {
            get { return _subscribe; }
        }

        /// <summary>
        /// 
        /// </summary>
        public SubscribeToOperationalStateChangesMessage(bool subscribe)
        {
            _subscribe = subscribe;
        }
    }
}
