using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using Arbiter;

namespace ForexPlatform
{
    /// <summary>
    /// Message sent from server to client when there is an update in trading quote information.
    /// </summary>
    [Serializable]
    public class QuoteUpdateMessage : DataSessionResponseMessage
    {
        Quote? _quote;
        /// <summary>
        /// 
        /// </summary>
        public Quote? Quote
        {
            get { return _quote; }
        }

        /// <summary>
        /// Full constructor.
        /// </summary>
        public QuoteUpdateMessage(DataSessionInfo sessionInfo, Quote? quote, bool operationResult)
            : base(sessionInfo, operationResult)
        {
            _quote = quote;
        }
    }
}
