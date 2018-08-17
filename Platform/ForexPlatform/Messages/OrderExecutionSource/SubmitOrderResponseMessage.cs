using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Message send as a responce to Open ActiveOrder request.
    /// </summary>
    [Serializable]
    public class SubmitOrderResponseMessage : AccountResponseMessage
    {
        string _orderId;
        public string OrderId
        {
            get { return _orderId; }
        }

        /// <summary>
        /// 
        /// </summary>
        public SubmitOrderResponseMessage(AccountInfo accountInfo, string orderId, bool operationResult)
            : base(accountInfo, operationResult)
        {
            _orderId = orderId;
        }
    }
}
