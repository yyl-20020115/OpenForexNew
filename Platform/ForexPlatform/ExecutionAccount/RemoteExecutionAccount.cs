using System;
using System.Collections.Generic;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Account takes care of handling synchronization with a remote order execution account
    /// (for ex. on a broker integration or external source).
    /// Some part of the updating calculation is done locally, to save updates transfer 
    /// (can not request update on each tick).
    /// </summary>
    [Serializable]
    public class RemoteExecutionAccount : Account
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RemoteExecutionAccount(AccountInfo info)
            : base(info)
        {
        }

        protected override void Orders_OrdersAddedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            base.Orders_OrdersAddedEvent(provider, account, order);
            // Run in a separate thread since it takes time to request from server.
            BeginUpdate();
        }

        protected override void Orders_OrdersRemovedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            base.Orders_OrdersRemovedEvent(provider, account, order);
            // Run in a separate thread since it takes time to request from server.
            BeginUpdate();
        }

        protected override void Orders_OrderUpdatedEvent(ITradeEntityManagement provider, AccountInfo account, Order[] orders, ActiveOrder.UpdateTypeEnum[] updatesType)
        {
            base.Orders_OrderUpdatedEvent(provider, account, orders, updatesType);
            
            // Run in a separate thread since it takes time to request from server.
            //GeneralHelper.FireAndForget(new GeneralHelper.GenericReturnDelegate<bool>(Update));
        }

        protected override void DataProvider_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            base.DataProvider_OperationalStatusChangedEvent(operational, previousOperationState);
            if (operational.OperationalState == OperationalStateEnum.Operational)
            {// Run in a separate thread since it takes time to request from server.
                BeginUpdate();
            }
        }

        protected override void _provider_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            base._provider_OperationalStatusChangedEvent(operational, previousOperationState);
            if (operational.OperationalState == OperationalStateEnum.Operational)
            {
                // Run in a separate thread since it takes time to request from server.
                BeginUpdate();
            }
        }


        /// <summary>
        /// Send the synchronization request over to the remote executor.
        /// </summary>
        /// <param name="updateOrdersIds"></param>
        /// <param name="operationResultMessage"></param>
        /// <returns></returns>
        //public bool SynchronizeOrders(string[] updateOrdersIds, out string operationResultMessage)
        //{
        //    if (OrderExecutionProvider == null)
        //    {
        //        operationResultMessage = "Account uninitialized.";
        //        return false;
        //    }

        //    PlatformSourceOrderExecutionProvider executionProvider = OrderExecutionProvider as PlatformSourceOrderExecutionProvider;
        //    if (executionProvider == null)
        //    {
        //        operationResultMessage = "Remote account coupled with non remote order execution provider. Operation can not be performed.";
        //        SystemMonitor.Warning(operationResultMessage);
        //        return false;
        //    }

        //    return executionProvider.SynchronizeOrders(updateOrdersIds, out operationResultMessage);
        //}
    }
}
