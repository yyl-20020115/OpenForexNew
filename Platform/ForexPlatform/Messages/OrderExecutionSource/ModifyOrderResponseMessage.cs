using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Message is responce to a request to modify order parameters.
    /// </summary>
    [Serializable]
    public class ModifyOrderResponseMessage : AccountResponseMessage
    {
        string _orderId;
        public string OrderId
        {
            get { return _orderId; }
        }

        string _orderModifiedId;
        /// <summary>
        /// ActiveOrder changed ticket when closed or modified.
        /// </summary>
        public string OrderModifiedId
        {
            get { return _orderModifiedId; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ModifyOrderResponseMessage(AccountInfo sessionInfo, string orderId,
            string orderModifiedId, bool operationResult)
            : base(sessionInfo, operationResult)
        {
            _orderId = orderId;
            _orderModifiedId = orderModifiedId;
        }
    }
}
