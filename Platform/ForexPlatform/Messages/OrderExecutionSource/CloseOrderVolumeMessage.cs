using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// This is also used as a close requestMessage.
    /// </summary>
    [Serializable]
    public class CloseOrderVolumeMessage : AccountRequestMessage
    {
        string _orderId;
        public string OrderId
        {
            get { return _orderId; }
        }

        string _orderTag;
        public string OrderTag
        {
            get { return _orderTag; }
        }

        Symbol _symbol;

        public Symbol Symbol
        {
            get { return _symbol; }
        }

        /// <summary>
        /// -1 defaults to closing the entire order.
        /// </summary>
        decimal _volumeDecreasal = -1;
        
        /// <summary>
        /// How much closeVolume to close.
        /// </summary>
        public decimal VolumeDecreasal
        {
            get { return _volumeDecreasal; }
        }

        decimal? _price;

        /// <summary>
        /// Preffered closing price.
        /// </summary>
        public decimal? Price
        {
            get { return _price; }
            set { _price = value; }
        }

        decimal? _slippage;

        /// <summary>
        /// 
        /// </summary>
        public decimal? Slippage
        {
            get { return _slippage; }
        }

        /// <summary>
        /// Close order.
        /// </summary>
        public CloseOrderVolumeMessage(AccountInfo accountInfo, Symbol symbol, string orderId, string orderTag, decimal? price, decimal? slippage)
            : base(accountInfo)
        {
            _symbol = symbol;
            _orderId = orderId;
            _price = price;
            _slippage = slippage;
            _orderTag = orderTag;
        }

        /// <summary>
        /// Close order partially.
        /// </summary>
        public CloseOrderVolumeMessage(AccountInfo accountInfo, Symbol symbol, string orderId, string orderTag, decimal volumeDecreasal, decimal? price, decimal? slippage)
            : base(accountInfo)
        {
            _symbol = symbol;
            _orderId = orderId;
            _volumeDecreasal = volumeDecreasal;
            _price = price;
            _slippage = slippage;
            _orderTag = orderTag;
        }

    }
}
