using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Message sent as responce to closing order closeVolume (partial or full).
    /// </summary>
    [Serializable]
    public class CloseOrderVolumeResponseMessage : AccountResponseMessage
    {
        volatile string _orderId;
        /// <summary>
        /// Id of the order placed.
        /// </summary>
        public string OrderId
        {
            get { return _orderId; }
        }

        volatile string _orderModifiedId;
        /// <summary>
        /// ActiveOrder changed ticket when closed or modified.
        /// May be null.
        /// </summary>
        public string OrderModifiedId
        {
            get { return _orderModifiedId; }
        }

        decimal _closingPrice = decimal.MinValue;
        public decimal ClosingPrice
        {
            get { return _closingPrice; }
        }

        DateTime _closingDateTime;
        public DateTime ClosingDateTime
        {
            get { return _closingDateTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        public CloseOrderVolumeResponseMessage(AccountInfo sessionInfo, string orderId,
            string orderModifiedId, decimal closingPrice, DateTime closingDateTime, bool operationResult)
            : base(sessionInfo, operationResult)
        {
            _orderId = orderId;
            _orderModifiedId = orderModifiedId;
            _closingPrice = closingPrice;
            _closingDateTime = closingDateTime;
        }
    }
}
