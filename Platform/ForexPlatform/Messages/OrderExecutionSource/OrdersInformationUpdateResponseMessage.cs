using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;
using ForexPlatform;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Responding requestMessage, contains information for orders.
    /// </summary>
    [Serializable]
    public class OrdersInformationUpdateResponseMessage : AccountResponseMessage
    {
        OrderInfo[] _orderInformations;
        public OrderInfo[] OrderInformations
        {
            get { return _orderInformations; }
        }

        ActiveOrder.UpdateTypeEnum[] _ordersUpdates;
        public ActiveOrder.UpdateTypeEnum[] OrdersUpdates
        {
            get { return _ordersUpdates; }
            set { _ordersUpdates = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public OrdersInformationUpdateResponseMessage(AccountInfo accountInfo, 
            OrderInfo[] orderInformations, ActiveOrder.UpdateTypeEnum[] ordersUpdates, bool operationResult)
            : base(accountInfo, operationResult)
        {
            _ordersUpdates = ordersUpdates;
            _orderInformations = orderInformations;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public OrdersInformationUpdateResponseMessage(AccountInfo accountInfo, 
            OrderInfo[] orderInformations, bool operationResult)
            : base(accountInfo, operationResult)
        {
            _orderInformations = orderInformations;

            _ordersUpdates = new Order.UpdateTypeEnum[_orderInformations.Length];
            for (int i = 0; i < _orderInformations.Length; i++)
            {
                _ordersUpdates[i] = Order.UpdateTypeEnum.Update;
            }
        }
    }
}
