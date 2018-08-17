using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Passive orders can not be controlled after being opened, 
    /// or can not be controlled at all, once submited.
    /// </summary>
    public class PassiveOrder : Order
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PassiveOrder(ISourceManager manager, ComponentId dataSourceId, ComponentId orderExecutionSourceId)
            : base(manager, manager.ObtainOrderExecutionProvider(orderExecutionSourceId, dataSourceId), dataSourceId)
        {
            SystemMonitor.CheckError(dataSourceId.IsEmpty == false && orderExecutionSourceId.IsEmpty == false, "Source Id not available to order.");
        }

        /// <summary>
        /// Will close open order (?!) or cancel pending one.
        /// </summary>
        /// <param name="desiredPrice"></param>
        /// <param name="slippage"></param>
        /// <param name="operationResultMessage"></param>
        /// <returns></returns>
        public bool CloseOrCancel(decimal? desiredPrice, decimal? slippage, out string operationResultMessage)
        {
            if (this.OpenPrice.HasValue == false)
            {
                operationResultMessage = "Invalid order open price.";
                return false;
            }

            if (State != OrderStateEnum.Executed && State != OrderStateEnum.Submitted)
            {
                operationResultMessage = "Close/Decrease volume can be done only to open/pending orders.";
                return false;
            }

            decimal operationPrice;
            bool operationResult = false;

            ISourceOrderExecution executionProvider = _executionProvider;

            if (executionProvider == null)
            {
                operationResultMessage = "Execution provider not assigned.";
                return false;
            }

            DateTime closeTime = DateTime.MinValue;

            string modifiedId;
            // Close/Cancel order.
            if (State == OrderStateEnum.Executed)
            {
                operationResult = executionProvider.CloseOrder(_executionProvider.DefaultAccount.Info, this,
                    slippage, desiredPrice, out operationPrice, out closeTime, out modifiedId, out operationResultMessage);
            }
            else if (State == OrderStateEnum.Submitted)
            {
                operationPrice = decimal.MinValue;
                operationResult = executionProvider.CancelPendingOrder(_executionProvider.DefaultAccount.Info, this, out modifiedId, out operationResultMessage);
            }
            else
            {
                operationResultMessage = "Order not in expected state.";
                return false;
            }

            if (operationResult == false)
            {
                SystemMonitor.Report("Order volume decrease has failed in executioner.");
                return false;
            }

            if (string.IsNullOrEmpty(modifiedId))
            {// Since the original order has changed its ticket number; and we have failed to establish the new one - we can no longer track it so unregister.
                SystemMonitor.OperationWarning("Failed to establish new modified order ticket; order will be re-aquired.", TracerItem.PriorityEnum.High);
                executionProvider.TradeEntities.RemoveOrder(this);

                return true;
            }

            if (State == OrderStateEnum.Executed)
            {

                if (modifiedId != this.Id)
                {
                    operationResultMessage = "Failed to close order, id returned is different.";
                    return false;

                    //executionProvider.TradeEntities.RemoveOrder(this);

                    //OrderInfo newUpdatedInfo = _info;
                    //newUpdatedInfo.Id = modifiedId;
                    //newUpdatedInfo.Volume = 0;

                    //ActiveOrder updatedOrder = new ActiveOrder(_manager, _executionProvider, _quoteProvider, _dataSourceId, Symbol, true);
                    //updatedOrder.AdoptInfo(newUpdatedInfo);
                    //_executionProvider.TradeEntities.AddOrder(updatedOrder);

                    // Request updated order info for this and new one and remove current one.
                    //if (_executionProvider != null && _executionProvider.DefaultAccount != null && string.IsNullOrEmpty(modifiedId) == false)
                    //{
                    //    _executionProvider.BeginOrdersInformationUpdate(_executionProvider.DefaultAccount.Info, new string[] { this.Id, modifiedId }, out operationResultMessage);
                    //}
                }
                else
                {
                    _info.Volume = 0;

                    State = OrderStateEnum.Closed;
                    _info.CloseTime = closeTime;
                    _info.ClosePrice = operationPrice;
                }
            }
            else if (State == OrderStateEnum.Submitted)
            {
                lock (this)
                {
                    //_initialVolume -= volumeDecrease;
                    _info.Volume = 0;
                    State = OrderStateEnum.Canceled;
                }
            }

            if (State == OrderStateEnum.Closed)
            {// Closed.
                RaiseOrderUpdatedEvent(UpdateTypeEnum.Closed);
            }
            else if (State == OrderStateEnum.Canceled)
            {// Still open.
                RaiseOrderUpdatedEvent(UpdateTypeEnum.Canceled);
            }

            return true;
        }

    }
}
