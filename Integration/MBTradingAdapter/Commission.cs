using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using MBTradingAdapter.Properties;

namespace MBTradingAdapter
{
    /// <summary>
    /// Handles commission operations.
    /// </summary>
    public struct Commission
    {
        public enum OperationEnum { None = 0, Multiply = 1, Add = 2 };

        OperationEnum Operation;

        decimal AskValue;
        decimal BidValue;

        decimal RawValue;
        
        /// <summary>
        /// Value related to orders commission.
        /// </summary>
        decimal OrderValue;

        /// <summary>
        /// 
        /// </summary>
        public void ApplyCommissions(int commissionPrecisionDecimals, ref OrderInfo order)
        {

            bool isBuy = OrderInfo.TypeIsBuy(order.Type);

            if (Operation == OperationEnum.Add)
            {
                if (isBuy)
                {// Buy.
                    order.OpenPrice = Math.Round(order.OpenPrice.Value + this.OrderValue, commissionPrecisionDecimals);
                }
                else
                {// Sell.
                    order.OpenPrice = Math.Round(order.OpenPrice.Value - this.OrderValue, commissionPrecisionDecimals);
                }
            }
            else if (Operation == OperationEnum.Multiply)
            {
                if (isBuy)
                {// Buy.
                    order.OpenPrice = Math.Round(order.OpenPrice.Value * this.OrderValue, commissionPrecisionDecimals);
                }
                else
                {// Sell.
                    order.OpenPrice = Math.Round(order.OpenPrice.Value / this.OrderValue, commissionPrecisionDecimals);
                }
            }
        }

        /// <summary>
        /// Apply commission values
        /// </summary>
        public void ApplyCommissions(MBTradingAdapter adapter, ref Quote quote)
        {
            
            if (quote.Ask.HasValue)
            {
                switch (Operation)
                {
                    case Commission.OperationEnum.Multiply:
                        {
                            quote.Ask = Math.Round(quote.Ask.Value * AskValue, adapter.CommissionPrecisionDecimals);
                        }
                        break;
                    case Commission.OperationEnum.Add:
                        {
                            quote.Ask = Math.Round(quote.Ask.Value + AskValue, adapter.CommissionPrecisionDecimals);
                        }
                        break;
                    default: break;
                }
            }

            if (quote.Bid.HasValue)
            {
                switch (Operation)
                {
                    case Commission.OperationEnum.Multiply:
                        {
                            quote.Bid = Math.Round(quote.Bid.Value * BidValue, adapter.CommissionPrecisionDecimals);
                        }
                        break;
                    case Commission.OperationEnum.Add:
                        {
                            quote.Bid = Math.Round(quote.Bid.Value + BidValue, adapter.CommissionPrecisionDecimals);
                        }
                        break;
                    default: 
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency1"></param>
        /// <param name="currency2"></param>
        /// <returns></returns>
        public static bool HasForexSymbol(string currency1, string currency2)
        {
            string symbol = currency1.ToUpper() + "/" + currency2.ToUpper();

            foreach (string item in MBTradingAdapter.ForexSymbols)
            {
                if (item == symbol)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Commission? GenerateSymbolCommission(MBTradingAdapter adapter, Symbol symbol)
        {
            // This is the commission value per each $10000 (USD)
            decimal commissionValue = adapter.CommissionValue;

            bool checkDealRate = false;

            // We have 4 cases - see http://www.mbtrading.com/commissions.aspx?page=Forex
            //
            if (symbol.ForexCurrency1 == "USD")
            {
                // first case - USD/JPY - US dollar based currency
                checkDealRate = false;
            }
            else if (symbol.ForexCurrency2 == "USD")
            {
                // second case - EUR/USD - Non-US dollar based currency
                checkDealRate = true;
            }
            else if (HasForexSymbol(symbol.ForexCurrency1, "USD"))
            {
                // third case - EUR/CHF - Exotic: US dollar is available as cross currency (EUR/USD)
                checkDealRate = true;
            }
            else if (HasForexSymbol("USD", symbol.ForexCurrency1))
            {
                // fourth case - CHF/JPY - Exotic: US dollar cross (CHF/USD) isn't available but base is (USD/CHF)
                checkDealRate = false;
            }
            else
            {// We should not be here.
                return null; 
            }


            Commission commission = new Commission() { RawValue = commissionValue * 0.0001m };
            decimal value = commission.RawValue / 2;

            if (checkDealRate)
            {
                commission.Operation = Commission.OperationEnum.Multiply;
                //decimal value = commission.RawValue / 2;
                //value /= 2;
                commission.AskValue = 1.0m + value;
                commission.BidValue = 1.0m - value;
                
                commission.OrderValue = 1.0m + commission.RawValue;
            }
            else
            {
                commission.Operation = Commission.OperationEnum.Add;
                //decimal value = commissionValue * 0.0001m;
                //value /= 2;
                commission.AskValue = value;
                commission.BidValue = -value;
                
                commission.OrderValue = commission.RawValue;
            }

            return commission;
        }
    }
}
