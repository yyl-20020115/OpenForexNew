using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Message allows modifications of existing orders parameters.
    /// </summary>
    [Serializable]
    public class ModifyOrderMessage : AccountRequestMessage
    {
        string _orderId;
        public string OrderId
        {
            get { return _orderId; }
        }

        Symbol _symbol;
        public Symbol Symbol
        {
            get { return _symbol; }
        }

        decimal? _takeProfit = decimal.MinValue;
        public decimal? TakeProfit
        {
            get { return _takeProfit; }
        }

        decimal? _stopLoss = decimal.MinValue;
        public decimal? StopLoss
        {
            get { return _stopLoss; }
        }

        decimal? _targetOpenPrice = decimal.MinValue;
        public decimal? TargetOpenPrice
        {
            get { return _targetOpenPrice; }
        }

        Int64? _expiration = null;
        public Int64? Expiration
        {
            get { return _expiration; }
        }

        /// <summary>
        /// Pass double.Nan for any parameter to assign it to "not assigned", pass null to leave unchanged.
        /// </summary>
        public ModifyOrderMessage(AccountInfo account, Symbol symbol, string orderId, decimal? stopLoss, decimal? takeProfit, decimal? targetOpenPrice, DateTime? expiration)
            : base(account)
        {
            _symbol = symbol;
            _orderId = orderId;
            _takeProfit = takeProfit;
            _stopLoss = stopLoss;
            _targetOpenPrice = targetOpenPrice;

            if (_expiration.HasValue)
            {
                _expiration = GeneralHelper.GenerateSecondsDateTimeFrom1970(expiration.Value);
            }
        }
    }
}
