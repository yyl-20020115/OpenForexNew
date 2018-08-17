using System;
using System.Collections.Generic;
using CommonSupport;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Trading position class that implements a position based on placing orders.
    /// </summary>
    [Serializable]
    public abstract class Position : IPosition, IDisposable, IDeserializationCallback
    {
        protected volatile ISourceManager _manager;

        [NonSerialized]
        private volatile Tracer _tracer = TracerHelper.Tracer;
        /// <summary>
        /// Tracer to target traces from this position to.
        /// </summary>
        public Tracer Tracer
        {
            get { return _tracer; }
            set { _tracer = value; }
        }

        volatile ISourceOrderExecution _orderProvider;
        /// <summary>
        /// 
        /// </summary>
        public ISourceOrderExecution OrderExecutionProvider
        {
            get { return _orderProvider; }
        }

        volatile bool _isProcessing = false;
        /// <summary>
        /// Is the order currently processing some (synchronous) request.
        /// </summary>
        public bool IsProcessing
        {
            get { return _isProcessing; }
        }

        protected volatile ISourceDataDelivery _dataDelivery;
        /// <summary>
        /// 
        /// </summary>
        public ISourceDataDelivery DataDelivery
        {
            get { return _dataDelivery; }
        }

        /// <summary>
        /// The quote provider (if available) corresponding to this position.
        /// </summary>
        public IQuoteProvider QuoteProvider
        {
            get
            {
                ISourceManager manager = _manager;
                ISourceDataDelivery dataDelivery = _dataDelivery;

                if (manager != null && dataDelivery != null)
                {
                    return manager.ObtainQuoteProvider(dataDelivery.SourceId, Symbol);
                }

                return null;
            }
        }

        protected PositionInfo _info = new PositionInfo();
        /// <summary>
        /// 
        /// </summary>
        public PositionInfo Info
        {
            get { lock (this) { return _info; } }
        }

        public Symbol Symbol
        {
            get { return _info.Symbol; }
        }

        /// <summary>
        /// A position can have positive or negative minVolume, indicating its direction.
        /// </summary>
        public decimal Volume
        {
            get
            {
                if (_info.Volume.HasValue)
                {
                    return _info.Volume.Value;
                }
                return 0;
            }
        }

        public decimal? BasePrice
        {
            get { return _info.Basis; }
        }

        decimal? _price = null;
        /// <summary>
        /// Current position price.
        /// </summary>
        public decimal? Price
        {
            get { return _price; }
        }

        public decimal Result
        {
            get 
            {
                if (_info.Result.HasValue)
                {
                    return _info.Result.Value;
                }

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsBusy
        {
            get { return false; }
        }

        #region Events

        [field: NonSerialized]
        public event UpdateDelegate UpdateEvent;

        [field: NonSerialized]
        public event OperationUpdateDelegate ExecuteEvent;

        [field: NonSerialized]
        public event OperationUpdateDelegate SubmitEvent;

        [field: NonSerialized]
        public event OperationUpdateDelegate FailEvent;

        #endregion

        #region Construction and Instance Control

        /// <summary>
        /// Constructor.
        /// </summary>
        public Position()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SetInitialParameters(ISourceManager manager,
            ISourceOrderExecution provider, 
            ISourceDataDelivery dataDelivery, Symbol symbol)
        {
            _manager = manager;
            _orderProvider = provider;
            _dataDelivery = dataDelivery;

            _info.Symbol = symbol;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Initialize()
        {
            _dataDelivery.QuoteUpdateEvent += new QuoteUpdateDelegate(_dataDelivery_QuoteUpdateEvent);

            _orderProvider.TradeEntities.OrdersAddedEvent += new OrderManagementOrdersUpdateDelegate(TradeEntities_OrdersAddedEvent);
            _orderProvider.TradeEntities.OrdersRemovedEvent += new OrderManagementOrdersUpdateDelegate(TradeEntities_OrdersRemovedEvent);
            _orderProvider.TradeEntities.OrdersUpdatedEvent += new OrderManagementOrdersUpdateTypeDelegate(TradeEntities_OrdersUpdatedEvent);
            
            return true;
        }

        void _dataDelivery_QuoteUpdateEvent(ISourceDataDelivery dataDelivery, DataSessionInfo session, Quote? quote)
        {
            if (session.Symbol == this.Symbol)
            {
                if (quote.HasValue)
                {
                    lock(this)
                    {
                        if (this.Volume < 0)
                        {
                            _price = quote.Value.Ask;
                        }
                        else
                        {
                            _price = quote.Value.Bid;
                        }
                    }
                }
                else
                {
                    _price = null;
                }

                RecalculateParameters(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {
            if (_dataDelivery != null)
            {
                _dataDelivery.QuoteUpdateEvent -= new QuoteUpdateDelegate(_dataDelivery_QuoteUpdateEvent);
            }

            if (_orderProvider != null)
            {
                _orderProvider.TradeEntities.OrdersAddedEvent -= new OrderManagementOrdersUpdateDelegate(TradeEntities_OrdersAddedEvent);
                _orderProvider.TradeEntities.OrdersRemovedEvent -= new OrderManagementOrdersUpdateDelegate(TradeEntities_OrdersRemovedEvent);
                _orderProvider.TradeEntities.OrdersUpdatedEvent -= new OrderManagementOrdersUpdateTypeDelegate(TradeEntities_OrdersUpdatedEvent);
            }
        }

        public void Dispose()
        {
            _manager = null;
            _orderProvider = null;
            _dataDelivery = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateInfo(PositionInfo info)
        {
            lock (this)
            {
                _info.Update(info);
            }

            RecalculateParameters(false);

            if (this.UpdateEvent != null)
            {
                UpdateEvent(this);
            }

            return true;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        void TradeEntities_OrdersAddedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> orders)
        {
            foreach(Order order in orders)
            {
                if (order.Symbol == Symbol)
                {
                    RecalculateParameters(true);
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void TradeEntities_OrdersRemovedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> orders)
        {
            foreach (Order order in orders)
            {
                if (order.Symbol == Symbol)
                {
                    RecalculateParameters(true);
                    break;
                }
            }
        }

        void TradeEntities_OrdersUpdatedEvent(ITradeEntityManagement provider, AccountInfo account, Order[] orders, Order.UpdateTypeEnum[] updatesType)
        {
            bool recalculateNeeded = false;
            for (int i = 0; i < orders.Length; i++)
            {
                if (orders[i].Symbol == Symbol)
                {
                    recalculateNeeded = true;

                    if (updatesType[i] == Order.UpdateTypeEnum.Submitted)
                    {
                        if (SubmitEvent != null)
                        {
                            SubmitEvent(this, orders[i].Id);
                        }
                    }

                    if (updatesType[i] == Order.UpdateTypeEnum.Executed)
                    {
                        if (ExecuteEvent != null)
                        {
                            ExecuteEvent(this, orders[i].Id);
                        }
                    }
                }
            }

            if (recalculateNeeded)
            {
                RecalculateParameters(true);
            }
        }

        protected abstract void OnRecalculateParameters(ISourceOrderExecution provider, bool fullRecalculation);

        /// <summary>
        /// 
        /// </summary>
        void RecalculateParameters(bool fullRecalculation)
        {
            if (fullRecalculation)
            {
                ISourceOrderExecution provider = _orderProvider;
                if (provider == null || Symbol.IsEmpty)
                {
                    Symbol symbol = Symbol;
                    _info = PositionInfo.Empty;
                    _info.Symbol = symbol;

                    return;
                }

                OnRecalculateParameters(provider, fullRecalculation);
            }

            lock (this)
            {
                if (_price.HasValue)
                {
                    _info.MarketValue = _price * Volume;
                }
            }

            if (UpdateEvent != null)
            {
                UpdateEvent(this);
            }
        }

        #region OrderBasedPosition Management

        ///// <summary>
        ///// Will try to asynchronously reverse the given execution info position part.
        ///// Similar to the ExecuteMarketReverse baseMethod, only this is not synchronous.
        ///// </summary>
        //public virtual string SubmitReverse(PositionExecutionInfo orderInfo, decimal? slippage, bool manipulateExistingOrders, out string operationResultMessage)
        //{
        //    if (orderInfo.IsEmpty || orderInfo.Result != PositionExecutionInfo.ExecutionResultEnum.Success)
        //    {
        //        operationResultMessage = "Execution info not properly assigned for reversal.";
        //        return string.Empty;
        //    }

        //    OrderTypeEnum orderType = OrderTypeEnum.UNKNOWN;
        //    if (orderInfo.OrderType == OrderTypeEnum.BUY_MARKET)
        //    {
        //        orderType = OrderTypeEnum.SELL_MARKET;
        //    }
        //    else if (orderInfo.OrderType == OrderTypeEnum.SELL_MARKET)
        //    {
        //        orderType = OrderTypeEnum.BUY_MARKET;
        //    }
        //    else
        //    {
        //        operationResultMessage = "Only market position can be reversed.";
        //        SystemMonitor.OperationError(operationResultMessage);
        //        return string.Empty;
        //    }

        //    return Submit(orderType, (int)orderInfo.ExecutedVolume, 
        //        null, slippage, null, null, manipulateExistingOrders, out operationResultMessage);
        //}

        ///// <summary>
        ///// Will try to synchronously execute a reverse of the given execution info.
        ///// This is a suitable replacement for a specific "order close", i.e. allowing
        ///// you to reverse a specific previous execution on this position.
        ///// </summary>
        ///// <param name="orderInfo"></param>
        ///// <param name="allowExistingActiveOrdersManipulation">Is the operation allowed to close a matching existing order, or should we always open a new one. This is suitable for Active orders brokers, that dissallow hedged orders at the same time.</param>
        ///// <returns></returns>
        //public virtual string ExecuteMarketReverse(PositionExecutionInfo orderInfo, decimal? slippage, TimeSpan timeOut, bool manipulateExistingOrders,
        //    out PositionExecutionInfo newExecutionInfo, out string operationResultMessage)
        //{
        //    newExecutionInfo = PositionExecutionInfo.Empty;

        //    if (orderInfo.IsEmpty == false && orderInfo.Result == PositionExecutionInfo.ExecutionResultEnum.Success)
        //    {
        //        OrderTypeEnum orderType = OrderTypeEnum.UNKNOWN;
        //        if (orderInfo.OrderType == OrderTypeEnum.BUY_MARKET)
        //        {
        //            orderType = OrderTypeEnum.SELL_MARKET;
        //        }
        //        else if (orderInfo.OrderType == OrderTypeEnum.SELL_MARKET)
        //        {
        //            orderType = OrderTypeEnum.BUY_MARKET;
        //        }
        //        else
        //        {
        //            operationResultMessage = "Only market position can be reversed.";
        //            SystemMonitor.OperationError(operationResultMessage);
        //            return string.Empty;
        //        }

        //        return ExecuteMarket(orderType, (int)orderInfo.ExecutedVolume, null, slippage, null, null, timeOut, out newExecutionInfo, out operationResultMessage);
        //    }
        //    else
        //    {
        //        operationResultMessage = "Can not reverse execution info with result [" + orderInfo.Result.ToString() + "], no action taken.";
        //        SystemMonitor.OperationError(operationResultMessage);
        //        return string.Empty;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool OnExecuteMarketBalanced(ISourceOrderExecution provider, int volumeModification, decimal? desiredPrice,
            decimal? slippage, TimeSpan timeOut, out PositionExecutionInfo executionInfo, out string operationResultMessage)
        {
            OrderTypeEnum orderType = OrderTypeEnum.BUY_MARKET;
            if (volumeModification < 0)
            {
                orderType = OrderTypeEnum.SELL_MARKET;
            }

            return string.IsNullOrEmpty(ExecuteMarket(orderType, Math.Abs(volumeModification), desiredPrice, slippage, null, null,
                timeOut, out executionInfo, out operationResultMessage)) == false;
        }

        /// <summary>
        /// This may contain a few operations on active orders, in order to match the rule "no opposing active orders"
        /// </summary>
        public virtual string ExecuteMarketBalanced(int volumeModification, decimal? desiredPrice, decimal? slippage, TimeSpan timeOut, 
            out PositionExecutionInfo executionInfo, out string operationResultMessage)
        {
            executionInfo = PositionExecutionInfo.Empty;

            ISourceOrderExecution provider = _orderProvider;
            if (provider == null || provider.DefaultAccount == null)
            {
                operationResultMessage = "Position not properly initialized.";
                return string.Empty;
            }

            // Trace pre-execution info.
            string traceMessage = string.Format("Submit Volume Modification [{0}] Price [{1}] Slippage [{2}] TimeOut [{3}]",
                volumeModification.ToString(), GeneralHelper.ToString(desiredPrice), GeneralHelper.ToString(slippage), timeOut.ToString());

            traceMessage += GenerateConditionsInfo();

            TracerHelper.Trace(_tracer, traceMessage, TracerItem.PriorityEnum.High);

            if (OnExecuteMarketBalanced(provider, volumeModification, desiredPrice, slippage, timeOut, out executionInfo, out operationResultMessage))
            {
                return Guid.NewGuid().ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract string OnExecuteMarket(ISourceOrderExecution provider, OrderTypeEnum orderType, int volume, 
            decimal? price, decimal? slippage, decimal? takeProfit, decimal? stopLoss, TimeSpan timeOut,
            out PositionExecutionInfo executionInfo, out string operationResultMessage);

        /// <summary>
        /// Places and tries to execute a market order synchronously. Since it might be a modification of an 
        /// existing active order, no specific order Id is returned - instead a bool indicating operation result.
        /// </summary>
        /// <param name="manipulateExistingOrders">[where applicable] Is the operation allowed to close a matching existing order, or should we always open a new one. This is suitable for Active orders brokers, that dissallow hedged orders at the same time.</param>
        public string ExecuteMarket(OrderTypeEnum orderType, int volume, decimal? price, decimal? slippage,
            decimal? takeProfit, decimal? stopLoss, TimeSpan timeOut, out PositionExecutionInfo executionInfo, 
            out string operationResultMessage)
        {
            operationResultMessage = string.Empty;
            executionInfo = PositionExecutionInfo.Empty;

            ISourceOrderExecution provider = _orderProvider;
            if (provider == null || provider.DefaultAccount == null)
            {
                operationResultMessage = "Position not properly initialized.";
                return string.Empty;
            }

            // Trace pre-execution info.
            string traceMessage = string.Format("Submit Type[{0}] Volume [{1}] Price [{2}] Slippage [{3}] TP [{4}] SL [{5}] TimeOut [{6}]", orderType.ToString(),
                volume.ToString(), GeneralHelper.ToString(price), GeneralHelper.ToString(slippage), GeneralHelper.ToString(takeProfit), 
                GeneralHelper.ToString(stopLoss), timeOut.ToString());

            traceMessage += GenerateConditionsInfo();

            TracerHelper.Trace(_tracer, traceMessage, TracerItem.PriorityEnum.High);

            _isProcessing = true;
            
            string result = OnExecuteMarket(provider, orderType, volume, price, slippage, takeProfit, stopLoss, timeOut,
                out executionInfo, out operationResultMessage);

            SystemMonitor.CheckError(result == executionInfo.ExecutionId, operationResultMessage);

            _isProcessing = false;

            return result;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        protected OrderTypeEnum GetReverseOrderType(OrderTypeEnum orderType)
        {
            if (orderType == OrderTypeEnum.SELL_MARKET)
            {
                return OrderTypeEnum.BUY_MARKET;
            }

            if (orderType == OrderTypeEnum.BUY_MARKET)
            {
                return OrderTypeEnum.SELL_MARKET;
            }

            return OrderTypeEnum.UNKNOWN;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        /// <returns></returns>
        protected string GenerateConditionsInfo()
        {
            IQuoteProvider quoteProvider = this.QuoteProvider;
            if (quoteProvider != null)
            {
                return string.Format(" Conditions [Ask {0}, Bid {1}, Last Quote {2}]",
                    GeneralHelper.ToString(quoteProvider.Ask), GeneralHelper.ToString(quoteProvider.Bid), GeneralHelper.ToString(quoteProvider.LastQuoteTime));
            }
            else
            {
                return " No conditions info (quote provider not available).";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract string OnSubmit(ISourceOrderExecution provider, OrderTypeEnum orderType, int volume, decimal? price, 
            decimal? slippage, decimal? takeProfit, decimal? stopLoss, out string operationResultMessage);


        /// <summary>
        /// Full submit of orders with a full set of parameters.
        /// </summary>
        /// <returns>The Id of the placement operation, allowing to trace its further execution or Empty if placement fails.</returns>
        public string Submit(OrderTypeEnum orderType, int volume, decimal? price, decimal? slippage,
            decimal? takeProfit, decimal? stopLoss, out string operationResultMessage)
        {
            ISourceOrderExecution provider = OrderExecutionProvider;
            if (provider == null || provider.DefaultAccount == null)
            {
                operationResultMessage = "Position not properly initialized.";
                return string.Empty;
            }

            if (price.HasValue == false)
            {
                price = _price;
            }

            // Trace pre-execution info.
            string traceMessage = string.Format("Submit Type[{0}] Volume [{1}] Price [{2}] Slippage [{3}] TP [{4}] SL [{5}]", orderType.ToString(),
                volume.ToString(), GeneralHelper.ToString(price), GeneralHelper.ToString(slippage), GeneralHelper.ToString(takeProfit), GeneralHelper.ToString(stopLoss));

            traceMessage += GenerateConditionsInfo();

            TracerHelper.Trace(_tracer, traceMessage, TracerItem.PriorityEnum.High);

            return OnSubmit(provider, orderType, volume, price, slippage, takeProfit, stopLoss, out operationResultMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExecuteMarketBalancedClose(int? closeVolume, TimeSpan timeOut, out PositionExecutionInfo executionResult, out string operationResultMessage)
        {
            operationResultMessage = string.Empty;
            executionResult = PositionExecutionInfo.Empty;

            ISourceOrderExecution provider = _orderProvider;
            if (provider == null || provider.DefaultAccount == null)
            {
                operationResultMessage = "Position not initialized.";
                return string.Empty;
            }

            if (Volume == 0)
            {
                operationResultMessage = "Position has no open volume (amount).";
                return string.Empty;
            }

            if (closeVolume.HasValue == false)
            {
                closeVolume = -(int)Volume;
            }
            else if (Volume > 0)
            {// Reverse of direction needed, since we make it directional.
                closeVolume = -closeVolume.Value;
            }

            if (Math.Abs(closeVolume.Value) > Math.Abs(Volume))
            {
                operationResultMessage = "Volume (amount) to close too big.";
                return string.Empty;
            }

            return ExecuteMarketBalanced(closeVolume.Value, null, null,
                timeOut, out executionResult, out operationResultMessage);
        }

        /// <summary>
        /// Submit a request for a market close partial or full closeVolume of the current position.
        /// </summary>
        /// <returns>The id of the operation or Empty if oepration placement fails.</returns>
        public string SubmitClose(int? closeVolume, out string operationResultMessage)
        {
            operationResultMessage = string.Empty;
            ISourceOrderExecution provider = _orderProvider;
            if (provider == null || provider.DefaultAccount == null)
            {
                operationResultMessage = "Position not initialized.";
                return string.Empty;
            }

            if (Volume == 0)
            {
                operationResultMessage = "Position has no open volume (amount).";
                return string.Empty;
            }

            if (closeVolume.HasValue == false)
            {
                closeVolume = -(int)Volume;
            }

            closeVolume = Math.Abs(closeVolume.Value);

            if (Math.Abs(closeVolume.Value) > Math.Abs(Volume))
            {
                operationResultMessage = "Volume (amount) to close too big.";
                return string.Empty;
            }

            OrderTypeEnum orderType = OrderTypeEnum.BUY_MARKET;
            if (this.Volume > 0)
            {
                orderType = OrderTypeEnum.SELL_MARKET;
            }

            return Submit(orderType, Math.Abs(closeVolume.Value), _price, null, null, null, out operationResultMessage);
        }

        /// <summary>
        /// Open orders in position management
        /// </summary>
        public bool CancelPending(string openOrderId, out string operationResultMessage)
        {
            ISourceOrderExecution provider = _orderProvider;
            if (provider == null || provider.DefaultAccount == null)
            {
                operationResultMessage = "Position not initialized.";
                return false;
            }

            Order order = provider.TradeEntities.GetOrderById(openOrderId);

            if (order.State != OrderStateEnum.Submitted)
            {
                operationResultMessage = "Order state not 'submitted', so not able to cancel as a pending order";
                return false;
            }

            ActiveOrder activeOrder = order as ActiveOrder;
            if (null != activeOrder)
            {
                return activeOrder.Cancel(out operationResultMessage);
            }

            PassiveOrder passiveOrder = order as PassiveOrder;
            if (null != passiveOrder)
            {
                return passiveOrder.CloseOrCancel(null, null, out operationResultMessage);
            }

            operationResultMessage = "Not able to cancel order";
            return false;
        }

        #endregion



        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            _tracer = TracerHelper.Tracer;
        }

        #endregion
    }
}
