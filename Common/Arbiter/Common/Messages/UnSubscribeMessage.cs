using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;

namespace Arbiter
{
    /// <summary>
    /// Use requestMessage to subscribe and unsubscribe to session.
    /// </summary>
    [Serializable]
    public class UnSubscribeMessage : RequestMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public UnSubscribeMessage()
        {
        }
    }
}
