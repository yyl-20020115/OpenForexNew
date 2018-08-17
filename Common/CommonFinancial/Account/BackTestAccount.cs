using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Serves as an account for handling orders that are executed locally in a simulation mode.
    /// An account tracks the amounths of money, equity, etc. See base class for more details.
    /// </summary>
    [Serializable]
    public class BackTestAccount : Account
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BackTestAccount(AccountInfo info)
            : base(info)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public new void AdoptInfo(AccountInfo info)
        {
            base.AdoptInfo(info);
        }

        protected override void _provider_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            base._provider_OperationalStatusChangedEvent(operational, previousOperationState);
            DoUpdate();
        }

        protected override void DataProvider_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            base.DataProvider_OperationalStatusChangedEvent(operational, previousOperationState);
            DoUpdate();
        }

        protected override void Orders_OrdersAddedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            base.Orders_OrdersAddedEvent(provider, account, order);
            DoUpdate();
        }

        protected override void Orders_OrdersRemovedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            base.Orders_OrdersRemovedEvent(provider, account, order);
            DoUpdate();
        }

        protected override void Orders_OrderUpdatedEvent(ITradeEntityManagement provider, AccountInfo account, Order[] orders, Order.UpdateTypeEnum[] updatesType)
        {
            base.Orders_OrderUpdatedEvent(provider, account, orders, updatesType);
            DoUpdate();
        }


        protected override void dataDelivery_QuoteUpdateEvent(ISourceDataDelivery dataDelivery, DataSessionInfo session, Quote? quote)
        {
            base.dataDelivery_QuoteUpdateEvent(dataDelivery, session, quote);
            DoUpdate();
        }

        /// <summary>
        /// The base class has an overload protection mechanism, however we do not need it here.
        /// </summary>
        public override void BeginUpdate()
        {
            bool result = _implementation.BeginAccountInfoUpdate(this.Info);
        }

        protected void DoUpdate()
        {
            BeginUpdate();

            //SystemMonitor.NotImplementedCritical();

            //ISourceOrderExecution provider = OrderExecutionProvider;
            //if (provider == null)
            //{
            //    return;
            //}

            //decimal profit = 0;
            //decimal balance;
            //lock (this)
            //{
            //    balance = _accountInfo.Balance;
            //}

            //ITradeEntityManagement history = OrderExecutionProvider.Account.TradeEntities;

            //if (history != null)
            //{
            //    lock (history)
            //    {
            //        foreach (Order order in history.OrdersUnsafe)
            //        {
            //            if (order.State == OrderStateEnum.Executed)
            //            {// Open orders shown in profit.
            //                Decimal? result = order.GetResult(Order.ResultModeEnum.Currency);
            //                if (result.HasValue)
            //                {
            //                    profit += result.Value;
            //                }

            //            }
            //            else if (order.State == OrderStateEnum.Closed)
            //            {// Closed orders end up in balance.
            //                Decimal? result = order.GetResult(Order.ResultModeEnum.Currency);
            //                if (result.HasValue)
            //                {
            //                    balance += result.Value;
            //                }
            //            }
            //        }
            //    }
            //}

            //lock (this)
            //{
            //    AccountInfo orderInfo = _accountInfo;
            //    orderInfo.Profit = profit;
            //    orderInfo.Balance = balance;
            //    orderInfo.Equity = balance + profit;

            //    _accountInfo = orderInfo;
            //}

            //RaiseUpdateEvent();
        }

        /// <summary>
        /// We store orders locally, so just confirm.
        /// </summary>
        /// <param name="updateOrdersIds"></param>
        /// <param name="operationResultMessage"></param>
        /// <returns></returns>
        //public bool SynchronizeOrders(string[] updateOrdersIds, out string operationResultMessage)
        //{
        //    operationResultMessage = string.Empty;
        //    return true;
        //}
    }
}
