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
    public class SubmitOrderMessage : OrderMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public SubmitOrderMessage(AccountInfo accountInfo, Symbol symbol, OrderTypeEnum orderType, int volume, decimal? price, decimal? slippage, 
            decimal? takeProfit, decimal? stopLoss, string comment)
            : base(accountInfo, symbol, orderType, volume, price, slippage, takeProfit, stopLoss, comment)
        {
        }
    }
}
