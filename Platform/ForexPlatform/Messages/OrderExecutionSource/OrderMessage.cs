using System;
using System.Collections.Generic;
using System.Text;
using ForexPlatform;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Message allows to open/place a new order on a source.
    /// </summary>
    [Serializable]
    public class OrderMessage : AccountRequestMessage
    {
        OrderTypeEnum _orderType;
        public OrderTypeEnum OrderType
        {
            get { return _orderType; }
        }

        Symbol _symbol;
        public Symbol Symbol
        {
            get { return _symbol; }
        }

        int _volume;
        public int Volume
        {
            get { return _volume; }
        }

        decimal? _desiredPrice;
        /// <summary>
        /// Preffered price for the trade.
        /// </summary>
        public decimal? DesiredPrice
        {
            get { return _desiredPrice; }
            set { _desiredPrice = value; }
        }

        decimal? _slippage;
        public decimal? Slippage
        {
            get { return _slippage; }
            set { _slippage = value; }
        }

        decimal? _takeProfit;
        public decimal? TakeProfit
        {
            get { return _takeProfit; }
            set { _takeProfit = value; }
        }

        decimal? _stopLoss;
        public decimal? StopLoss
        {
            get { return _stopLoss; }
            set { _stopLoss = value; }
        }

        string _comment;
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public OrderMessage(AccountInfo accountInfo, Symbol symbol, OrderTypeEnum orderType, int volume, decimal? price, decimal? slippage, 
            decimal? takeProfit, decimal? stopLoss, string comment)
            : base(accountInfo)
        {
            _symbol = symbol;
            _orderType = orderType;
            _volume = volume;
            _desiredPrice = price;
            _slippage = slippage;
            _takeProfit = takeProfit;
            _stopLoss = stopLoss;
            _comment = comment;

        }
    }
}
