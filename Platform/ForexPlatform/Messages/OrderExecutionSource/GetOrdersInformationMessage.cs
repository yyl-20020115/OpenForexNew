using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Message allows obtaining information of orders.
    /// </summary>
    [Serializable]
    public class GetOrdersInformationMessage : AccountRequestMessage
    {
        List<string> _orderTickets;
        /// <summary>
        /// 
        /// </summary>
        public List<string> OrderTickets
        {
            get { return _orderTickets; }
        }

        /// <summary>
        /// 
        /// </summary>
        public GetOrdersInformationMessage(AccountInfo info, string[] orderTickets)
            : base(info)
        {
            _orderTickets = new List<string>(orderTickets);
        }
    }
}
