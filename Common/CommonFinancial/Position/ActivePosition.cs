using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Position class for managing active orders.
    /// </summary>
    [Serializable]
    public class ActivePosition : Position
    {
        volatile bool _manipulateExistingOrders = true;
        /// <summary>
        /// Is the position allowed to manipulate existing orders.
        /// </summary>
        public bool ManipulateExistingOrders
        {
            get { return _manipulateExistingOrders; }
            set { _manipulateExistingOrders = value; }
        }

        volatile bool _useOrdersForPositionInfo = true;
        /// <summary>
        /// 
        /// </summary>
        public bool UseOrdersForPositionInfo
        {
            get { return _useOrdersForPositionInfo; }
            set { _useOrdersForPositionInfo = value; }
        }

        /// <summary>
        /// Orders selected so far for active manipulation.
        /// They shall all be from owners Order Execution Provider
        /// and from this Position's symbol.
        /// </summary>
        List<ActiveOrder> _activeSelectedOrders = new List<ActiveOrder>();

        /// <summary>
        /// 
        /// </summary>
        public override bool IsBusy
        {
            get { return _activeSelectedOrders.Count > 0; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActivePosition()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnRecalculateParameters(ISourceOrderExecution provider, bool fullRecalculation)
        {
            SystemMonitor.CheckError(provider.SupportsActiveOrderManagement, "Wrong position type for this provider.");

            Order[] orders = provider.TradeEntities.GetOrdersBySymbol(Symbol);
            lock (this)
            {
                _info.Volume = 0;
                _info.PendingBuyVolume = 0;
                _info.PendingSellVolume = 0;
                _info.Result = 0;

                foreach (ActiveOrder order in GeneralHelper.SafeChildTypeIteration<Order, ActiveOrder>(orders))
                {
                    if (order.State == OrderStateEnum.Executed)
                    {
                        _info.Volume += order.CurrentDirectionalVolume;
                        _info.Result += order.GetResult(Order.ResultModeEnum.Currency);
                    }
                    else if (order.State == OrderStateEnum.Submitted)
                    {
                        if (order.IsBuy)
                        {
                            _info.PendingBuyVolume += order.CurrentVolume;
                        }
                        else
                        {
                            _info.PendingSellVolume += order.CurrentVolume;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        ActiveOrder ObtainManipulationOrder(ISourceOrderExecution provider, OrderTypeEnum orderType, int minVolume, 
            out bool suitableOrdersAvailable)
        {
            suitableOrdersAvailable = false;

            lock (provider.TradeEntities)
            {
                foreach (ActiveOrder activeOrder in provider.TradeEntities.GetOrdersByStateUnsafe(OrderStateEnum.Executed))
                {
                    if (activeOrder.Symbol == Symbol 
                        && minVolume <= activeOrder.CurrentVolume
                        && (activeOrder.Type == orderType))
                    {// Found a opposing order, close it.

                        suitableOrdersAvailable = true;

                        lock (_activeSelectedOrders)
                        {
                            if (_activeSelectedOrders.Contains(activeOrder) == false)
                            {// Only if this order is not currently processed, take it for processing.
                                _activeSelectedOrders.Add(activeOrder);
                                return activeOrder;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Helper, remove order from manipulation list.
        /// </summary>
        void ReleaseManipulationOrder(ActiveOrder order)
        {
            lock (_activeSelectedOrders)
            {
                _activeSelectedOrders.Remove(order);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        decimal? ProcessPrice(IQuoteProvider quoteProvider, OrderTypeEnum orderType, decimal? price)
        {
            if (price.HasValue == false && quoteProvider.Ask.HasValue && quoteProvider.Bid.HasValue)
            {
                if (OrderInfo.TypeIsBuy(orderType))
                {
                    return quoteProvider.Ask;
                }
                else
                {
                    return quoteProvider.Bid;
                }
            }

            return price;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string OnExecuteMarket(ISourceOrderExecution provider, OrderTypeEnum orderType, int volume, 
            decimal? price, decimal? slippage, decimal? takeProfit, decimal? stopLoss, TimeSpan timeOut,
            out PositionExecutionInfo executionInfo, out string operationResultMessage)
        {
            SystemMonitor.CheckError(provider.SupportsActiveOrderManagement, "Wrong position type for this provider.");

            executionInfo = PositionExecutionInfo.Empty;
            
            IQuoteProvider quoteProvider = QuoteProvider;
            if (quoteProvider == null)
            {
                operationResultMessage = "Failed to establish quote provider for [" + _dataDelivery.SourceId.Name + ", " + Symbol.Name + "].";
                SystemMonitor.Error(operationResultMessage);
                return string.Empty;
            }

            price = ProcessPrice(quoteProvider, orderType, price);

            // New order shall be created.
            ActiveOrder order = new ActiveOrder(_manager, provider, _dataDelivery.SourceId, true);
            
            OrderInfo? infoReference;
            
            // Using the extended operationTimeOut to 55 seconds.
            bool result = provider.SynchronousExecute(provider.DefaultAccount.Info, order, _info.Symbol,
                orderType, volume, slippage, price, takeProfit, stopLoss, string.Empty, timeOut, out infoReference, out operationResultMessage);

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
        protected override string OnSubmit(ISourceOrderExecution provider, OrderTypeEnum orderType, int volume, decimal? price,
            decimal? slippage, decimal? takeProfit, decimal? stopLoss, out string operationResultMessage)
        {
            SystemMonitor.CheckError(provider.SupportsActiveOrderManagement, "Wrong position type for this provider.");

            IQuoteProvider quotes = QuoteProvider;

            if (quotes == null)
            {
                operationResultMessage = "Failed to retrieve quote provider.";
                return string.Empty;
            }

            ActiveOrder order = new ActiveOrder(_manager, provider, _dataDelivery.SourceId, true);

            price = ProcessPrice(quotes, orderType, price);

            string id = provider.SubmitOrder(provider.DefaultAccount.Info, order, _info.Symbol,
                orderType, volume, slippage, price, takeProfit, stopLoss, string.Empty, out operationResultMessage);

            if (string.IsNullOrEmpty(id))
            {
                return string.Empty;
            }

            OrderInfo info = new OrderInfo(id, Symbol, orderType, OrderStateEnum.Submitted, volume,
                price, null, null, null, null, null, null, null, null, null, null, string.Empty, null);

            order.AdoptInfo(info);
            provider.TradeEntities.AddOrder(order);

            return id;

        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool OnExecuteMarketBalanced(ISourceOrderExecution provider, int volumeModification, decimal? desiredPrice, 
            decimal? slippage, TimeSpan timeOut, out PositionExecutionInfo executionInfo, out string operationResultMessage)
        {
            if (_manipulateExistingOrders == false)
            {
                return base.OnExecuteMarketBalanced(provider, volumeModification, desiredPrice, slippage, timeOut, out executionInfo, out operationResultMessage);
            }

            OrderTypeEnum orderType = OrderTypeEnum.BUY_MARKET;
            if (volumeModification < 0)
            {
                orderType = OrderTypeEnum.SELL_MARKET;
                volumeModification = Math.Abs(volumeModification);
            }

            executionInfo = PositionExecutionInfo.Empty;
            if (_activeSelectedOrders.Count > 0)
            {// *ONE BY ONE*
                operationResultMessage = "Another operation performed on active orders is already in progress.";
                return false;
            }

            int originalVolumeModification = volumeModification;

            List<KeyValuePair<double, decimal>> closePrices = new List<KeyValuePair<double, decimal>>();

            bool suitableOrdersAvailable;

            ActiveOrder orderSelected = ObtainManipulationOrder(provider, GetReverseOrderType(orderType), Math.Abs(originalVolumeModification), 
                out suitableOrdersAvailable);

            if (orderSelected == null)
            {// There are some orders with lower volume, that otherwise match the requirements.
                // This will result in partial execution at best.
                orderSelected = ObtainManipulationOrder(provider, GetReverseOrderType(orderType), 0, out suitableOrdersAvailable);
            }

            if (orderSelected != null && volumeModification > 0)
            {
                if (volumeModification >= orderSelected.CurrentVolume)
                {
                    int orderVolume = orderSelected.CurrentVolume;
                    string closeMessage;
                    if (orderSelected.Close(slippage, null, out closeMessage))
                    {
                        volumeModification -= orderVolume;
                        if (orderSelected.ClosePrice.HasValue)
                        {
                            closePrices.Add(new KeyValuePair<double, decimal>(orderVolume, orderSelected.ClosePrice.Value));
                        }
                        else
                        {
                            SystemMonitor.Error("Order [{" + orderSelected.Id + "}] closed but close price not assigned.");
                        }
                    }
                    else
                    {
                        ReleaseManipulationOrder(orderSelected);
                        operationResultMessage = "Failed to close corresponding reverse active order [" + closeMessage + "].";
                        return false;
                    }
                }
                else
                {
                    if (orderSelected.DecreaseVolume(volumeModification, slippage, null))
                    {
                        volumeModification = 0;
                    }
                }

                ReleaseManipulationOrder(orderSelected);
                //orderSelected = null;
            }

            if (suitableOrdersAvailable && volumeModification > 0
                && originalVolumeModification == volumeModification)
            {// Complete failure to close anything, and there are some suitable.
                executionInfo = PositionExecutionInfo.Empty;
                operationResultMessage = "Suitable reverse market orders are available, but currently manipulated, so hedging rules forbid reverse orders placement at this moment.";
                return false;
            }

            if (orderSelected == null && volumeModification > 0)
            {// We need to in the reverse direction, only if there is nothing in the current direction.
                PositionExecutionInfo marketExecutionInfo;
                string tmp;

                string executionResult = ExecuteMarket(orderType, volumeModification, null, slippage, null, null,
                    timeOut, out marketExecutionInfo, out tmp);

                if (string.IsNullOrEmpty(executionResult) == false)
                {// Success.
                    volumeModification -= (int)marketExecutionInfo.VolumeExecuted;
                    if (marketExecutionInfo.ExecutedPrice.HasValue)
                    {
                        closePrices.Add(new KeyValuePair<double, decimal>(marketExecutionInfo.VolumeExecuted, marketExecutionInfo.ExecutedPrice.Value));
                    }
                    else
                    {
                        SystemMonitor.Error("MarketExecutionInfo for a valid execution [{" + executionResult + "}] does not have ExecutedPrice assigned.");
                    }
                }
                else
                {
                    operationResultMessage = tmp;
                    return false;
                }
            }

            // Calculate the close price, combination from the operations.
            decimal closePrice = 0;
            double totalVolume = 0;

            if (FinancialHelper.CalculateAveragePrice(closePrices, out closePrice, out totalVolume) == false)
            {
                SystemMonitor.Error("Failed to calculate average price for market balanced execution.");
                closePrice = 0;
            }

            if (volumeModification > 0)
            {// Failure.
                if (originalVolumeModification == volumeModification)
                {// Complete failure.
                    executionInfo = PositionExecutionInfo.Empty;
                    operationResultMessage = "Failed to execute market operation.";
                    return false;
                }
                else
                {// Partial execution success.
                    executionInfo = new PositionExecutionInfo(Guid.NewGuid().ToString(), _dataDelivery.SourceId, provider.SourceId, Symbol, orderType,
                        closePrice, originalVolumeModification, originalVolumeModification - volumeModification, DateTime.Now, PositionExecutionInfo.ExecutionResultEnum.PartialSuccess);
                }
            }
            else
            {// Success.
                executionInfo = new PositionExecutionInfo(Guid.NewGuid().ToString(), _dataDelivery.SourceId, provider.SourceId, Symbol, orderType,
                    closePrice, originalVolumeModification, originalVolumeModification, DateTime.Now, PositionExecutionInfo.ExecutionResultEnum.Success);
            }

            operationResultMessage = string.Empty;
            return true;
        }
    }
}
