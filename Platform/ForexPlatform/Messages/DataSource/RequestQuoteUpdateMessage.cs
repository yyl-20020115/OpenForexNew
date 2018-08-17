using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Request an update of quotes from server / source.
    /// </summary>
    [Serializable]
    public class RequestQuoteUpdateMessage : DataSessionRequestMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        public RequestQuoteUpdateMessage(DataSessionInfo sessionInfo)
            : base(sessionInfo)
        {
        }
    }
}
