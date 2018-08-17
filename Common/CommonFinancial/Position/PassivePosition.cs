using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Position class for management of passive orders.
    /// </summary>
    [Serializable]
    public class PassivePosition : Position
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PassivePosition()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnRecalculateParameters(ISourceOrderExecution provider, bool fullRecalculation)
        {
            //decimal pendingVolume = 0;
            //Order[] orders = provider.TradeEntities.GetOrdersBySymbol(Symbol);
            //foreach (PassiveOrder order in orders)
            //{
            //    if (order.State == OrderStateEnum.Submitted)
            //    {
            //        pendingVolume += order.CurrentVolume;
            //    }
            //}

            //lock (this)
            //{
            //    _info.PendingBuyVolume = pendingVolume;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string OnExecuteMarket(ISourceOrderExecution provider, OrderTypeEnum orderType, int volume, 
            decimal? price, decimal? slippage, decimal? takeProfit, decimal? stopLoss, TimeSpan timeOut, 
            out PositionExecutionInfo executionInfo, out string operationResultMessage)
        {
            SystemMonitor.CheckError(provider.SupportsActiveOrderManagement == false, "Wrong position type for this provider.");

            executionInfo = PositionExecutionInfo.Empty;
            PassiveOrder order;
            lock (this)
            {
                order = new PassiveOrder(_manager, _dataDelivery.SourceId, provider.SourceId);
            }

            OrderInfo? infoReference;

            bool result = provider.SynchronousExecute(provider.DefaultAccount.Info, order, _info.Symbol,
                orderType, volume, slippage, price, takeProfit, stopLoss, string.Empty, out infoReference, out operationResultMessage);

            if (result && infoReference.HasValue)
            {
                OrderInfo infoAssign = infoReference.Value;
                if (infoAssign.Type == OrderTypeEnum.UNKNOWN)
                {
                    infoAssign.Type = orderType;
                }

                if (infoAssign.Volume == int.MinValue
                    || infoAssign.Volume == int.MaxValue)
                {// Volume was not retrieved by integration.
                    infoAssign.Volume = volume;
                }

                if (infoAssign.OpenPrice.HasValue)
                {
                    executionInfo = new PositionExecutionInfo(infoReference.Value.Id, _dataDelivery.SourceId, provider.SourceId, Symbol,
                        infoAssign.Type, infoAssign.OpenPrice.Value, volume, volume, 
                        infoAssign.OpenTime, PositionExecutionInfo.ExecutionResultEnum.Success);
                }
                else
                {
                    SystemMonitor.Error("Received execution result, but price not assigned.");
                }

                order.AdoptInfo(infoAssign);

                provider.TradeEntities.AddOrder(order);

                return infoReference.Value.Id;
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string OnSubmit(ISourceOrderExecution provider, OrderTypeEnum orderType, int volume, 
            decimal? price, decimal? slippage, decimal? takeProfit, decimal? stopLoss, out string operationResultMessage)
        {
            PassiveOrder order = new PassiveOrder(_manager, _dataDelivery.SourceId, provider.SourceId);

            string id = provider.SubmitOrder(provider.DefaultAccount.Info, order, _info.Symbol,
                orderType, volume, slippage, price, takeProfit, stopLoss, string.Empty, out operationResultMessage);

            if (string.IsNullOrEmpty(id))
            {
                return string.Empty;
            }

            if (order.Info.State != OrderStateEnum.Executed)
            {// It is possible that the submit executes the order instantly, so make sure this is not the case.
                OrderInfo info = new OrderInfo(id, Symbol, orderType, OrderStateEnum.Submitted, volume,
                    price, null, stopLoss, takeProfit, null, null, null, null, null, null, null, string.Empty, null);
                order.AdoptInfo(info);
            }

            provider.TradeEntities.AddOrder(order);

            return id;

        }

    }
}
