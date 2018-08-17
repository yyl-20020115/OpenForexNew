using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Execute a market order on a source synchronously.
    /// </summary>
    [Serializable]
    public class ExecuteMarketOrderMessage : OrderMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public ExecuteMarketOrderMessage(AccountInfo accountInfo, Symbol symbol, OrderTypeEnum orderType, int volume, decimal? price, decimal? slippage, 
            decimal? takeProfit, decimal? stopLoss, string comment)
            : base(accountInfo, symbol, orderType, volume, price, slippage, takeProfit, stopLoss, comment)
        {
        }

    }
}
