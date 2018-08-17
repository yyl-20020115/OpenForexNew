using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Runtime.Serialization;
using System.Threading;

namespace CommonFinancial
{
    /// <summary>
    /// Back testing order execution provider. Extends the default execution with time control.
    /// Handles order execution requests coming from an expert sessionInformation. It will not pass the orders
    /// to any actual execution source, rather keep them locally for test analysis only.
    /// Also supports time control to be able to cope with simulation modes.
    /// Makes use of the automatic serialization mode.
    /// 
    /// Operation logics contributions by "bolo"
    /// </summary>
    [Serializable]
    [UserFriendlyName("Back Test Order Execution")]
    public class BackTestOrderExecutionProvider : Operational, ISourceOrderExecution, IDeserializationCallback
    {
        decimal _runningAccountBalance = 10000;

        BackTestAccount _account = null;

        TradeEntityKeeper _tradeEntities = new TradeEntityKeeper();

        ISourceManager _manager;

        ISourceDataDelivery _dataDelivery;

        ComponentId _sourceId;

        TimeControl _timeControl;

        int _pendingOrderId = 1;

        public enum ResultsAproximationModeEnum
        {
            OptimisticResult,
            PesimisticResult,
            MostProbableResult,
         
            // TODO: To implement this, tick by tick dataDelivery is needed.
            //ExactResult
        }

        volatile ResultsAproximationModeEnum _resultsApproximationMode = ResultsAproximationModeEnum.PesimisticResult;
        public ResultsAproximationModeEnum ResultsApproximationMode
        {
            get { return _resultsApproximationMode; }
            set { _resultsApproximationMode = value; }
        }

        #region IOrderSink Members

        /// <summary>
        /// Not needed under the backtest execution models.
        /// </summary>
        public decimal SlippageMultiplicator
        {
            get
            {
                return 1;
            }

            set
            {
                SystemMonitor.NotImplementedWarning();
            }
        }

        #endregion


        public Account[] Accounts
        {
            get { return new Account[] { _account }; }
        }

        public Account DefaultAccount
        {
            get { return _account; }
        }

        public ITradeEntityManagement TradeEntities
        {
            get { return _tradeEntities; }
        }

        public ComponentId SourceId
        {
            get { return _sourceId; }
        }

        public ITimeControl TimeControl
        {
            get { return _timeControl; }
        }

        public bool SupportsActiveOrderManagement
        {
            get { return false; }
        }

        //public bool IsBusy
        //{
        //    get { return false; }
        //}

        Quote? _lastDataQuote = null;

        DataBar? _lastDataBar = null;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        Dictionary<string, Order> _orders = new Dictionary<string, Order>();

        [field: NonSerialized]
        public event OrdersUpdateDelegate OrdersUpdatedEvent;

        [field: NonSerialized]
        public event PositionUpdateDelegate PositionsUpdateEvent;

        [field: NonSerialized]
        public event AccountInfoUpdateDelegate AccountInfoUpdateEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackTestOrderExecutionProvider(ComponentId sourceId)
        {
            _sourceId = sourceId;
            _sourceId.IdentifiedComponentType = typeof(BackTestOrderExecutionProvider);

            _account = new BackTestAccount(new AccountInfo(Guid.NewGuid(), _runningAccountBalance, 0,
                string.Empty, new Symbol("USD"), _runningAccountBalance, _runningAccountBalance, 100, _runningAccountBalance, "Simulation Account", string.Empty, 0, string.Empty));

            ChangeOperationalState(OperationalStateEnum.Constructed);
        }

        /// <summary>
        /// One time init by passing references essential for operation.
        /// </summary>
        public bool SetInitialParameters(ISourceManager manager, ISourceDataDelivery dataDelivery)
        {
            _manager = manager;
            _dataDelivery = dataDelivery;

            _account.SetInitialParameters(_manager, this, _dataDelivery);
            _tradeEntities.SetInitialParameters(_manager, this, _dataDelivery);

            _timeControl = new TimeControl();
            _timeControl.CanStepBack = false;
            _timeControl.CanStepForward = true;
            _timeControl.CanRestart = false;
            _timeControl.TotalStepsCount = int.MaxValue;

            ChangeOperationalState(OperationalStateEnum.Initializing);

            return true;
        }

        /// <summary>
        /// Init every time upon starting.
        /// </summary>
        public bool Initialize()
        {

            _account.Initialize(this);
            _tradeEntities.Initialize();
            
            _dataDelivery.QuoteUpdateEvent += new QuoteUpdateDelegate(_dataDelivery_QuoteUpdateEvent);
            _dataDelivery.DataHistoryUpdateEvent += new DataHistoryUpdateDelegate(_dataDelivery_DataHistoryUpdateEvent);

            ChangeOperationalState(OperationalStateEnum.Operational);
            return true;
        }

        void _dataDelivery_DataHistoryUpdateEvent(ISourceDataDelivery dataDelivery, DataSessionInfo session, DataHistoryUpdate update)
        {
            if (update.BarDataAssigned)
            {
                lock (this)
                {
                    _lastDataBar = update.DataBarsUnsafe[update.DataBarsUnsafe.Count - 1];
                }
            }

        }

        void _dataDelivery_QuoteUpdateEvent(ISourceDataDelivery dataDelivery, DataSessionInfo session, Quote? quote)
        {
            _lastDataQuote = quote;
            
            BeginAccountInfoUpdate(_account.Info);

            BeginManagedOrdersUpdate(quote);
        }

        public void UnInitialize()
        {
            _tradeEntities.UnInitialize();

            _dataDelivery.QuoteUpdateEvent -= new QuoteUpdateDelegate(_dataDelivery_QuoteUpdateEvent);
            _dataDelivery.DataHistoryUpdateEvent -= new DataHistoryUpdateDelegate(_dataDelivery_DataHistoryUpdateEvent);

            ChangeOperationalState(OperationalStateEnum.UnInitialized);
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
            _sourceId.IdentifiedComponentType = typeof(BackTestOrderExecutionProvider);

            _orders = new Dictionary<string, Order>();

            ChangeOperationalState(OperationalStateEnum.Constructed);
        }

        protected void RaiseOrderUpdateEvent(OrderInfo orderInfo, Order.UpdateTypeEnum updateType)
        {
            if (OrdersUpdatedEvent != null)
            {
                OrdersUpdatedEvent(this, _account.Info, 
                    new[] { orderInfo.Id }, new[] { orderInfo }, new[] { updateType });
            }
        }

        public bool GetAvailableAccountInfos(out AccountInfo[] accounts)
        {
            accounts = new[] { _account.Info };
            return true;
        }

        public bool GetOrdersInformation(AccountInfo accountInfo, string[] orderIds, out OrderInfo[] informations,
            out string operationResultMessage)
        {
            if (_account.Info != accountInfo)
            {
                informations = new OrderInfo[] { };
                operationResultMessage = "Account info not recognized.";
                return false;
            }

            List<OrderInfo> orders = new List<OrderInfo>();
            foreach (string id in orderIds)
            {
                if (_orders.ContainsKey(id))
                {
                    orders.Add(_orders[id].Info);
                }
            }

            informations = orders.ToArray();
            operationResultMessage = string.Empty;
            return true;
        }

        public bool BeginOrdersInformationUpdate(AccountInfo accountInfo, string[] orderIds, out string operationResultMessage)
        {
            OrderInfo[] ordersInfos;
            if (GetOrdersInformation(accountInfo, orderIds, out ordersInfos, out operationResultMessage))
            {
                Order.UpdateTypeEnum[] ordersUpdates = new Order.UpdateTypeEnum[ordersInfos.Length];
                for (int i = 0; i < ordersUpdates.Length; i++)
			    {
    			    ordersUpdates[i] = Order.UpdateTypeEnum.Update;
	    		}

                if (OrdersUpdatedEvent != null)
                {
                    GeneralHelper.FireAndForget(delegate()
                    {
                        OrdersUpdatedEvent(this, accountInfo, orderIds, ordersInfos, ordersUpdates);
                    });
                }
            }

            operationResultMessage = string.Empty;
            return true;
        }

        public string SubmitOrder(AccountInfo account, Order order, Symbol symbol,
            OrderTypeEnum orderType, int volume, decimal? allowedSlippage, decimal? desiredPrice,
            decimal? takeProfit, decimal? stopLoss, string comment, out string operationResultMessage)
        {
            OrderInfo? orderInfo;
            if (SynchronousExecute(account, order, symbol, orderType, volume, allowedSlippage, desiredPrice,
                takeProfit, stopLoss, comment, out orderInfo, out operationResultMessage))
            {
                order.AdoptInfo(orderInfo.Value);
                return orderInfo.Value.Id;
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SynchronousExecute(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType,
            int volume, decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss,
            string comment, out OrderInfo? info, out string operationResultMessage)
        {
            return SynchronousExecute(account, order, symbol, orderType,
                volume, allowedSlippage, desiredPrice, takeProfit, stopLoss,
                comment, TimeSpan.MaxValue, out info, out operationResultMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SynchronousExecute(AccountInfo account, Order order, Symbol symbol,
            OrderTypeEnum orderType, int volume, decimal? allowedSlippage, decimal? desiredPrice,
            decimal? takeProfit, decimal? stopLoss, string comment, TimeSpan operationTimeOut, out OrderInfo? info, out string operationResultMessage)
        {
            info = null;

            OrderInfo updatedInfo = order.Info;
            updatedInfo.OpenPrice = null;
            updatedInfo.OpenTime = null;
            updatedInfo.State = OrderStateEnum.Unknown;

            IQuoteProvider quotes;
            IDataBarHistoryProvider bars;
            if (GetProviders(symbol, out quotes, out bars) == false)
            {
                operationResultMessage = "Failed to establish corresponding providers.";
                return false;
            }

            if (quotes.OperationalState != OperationalStateEnum.Operational ||
                quotes.Ask.HasValue == false || quotes.Bid.HasValue == false || quotes.Time.HasValue == false)
            {
                operationResultMessage = "Data provider not operational or not providing valid dataDelivery.";
                return false;
            }

            switch (orderType)
            {
                case OrderTypeEnum.BUY_LIMIT_MARKET:
                case OrderTypeEnum.SELL_LIMIT_MARKET:
                    updatedInfo.State = OrderStateEnum.Submitted;
                    updatedInfo.OpenPrice = desiredPrice;
                    break;
                case OrderTypeEnum.BUY_MARKET:
                case OrderTypeEnum.SELL_MARKET:
                    {
                        decimal? currentPrice = quotes.GetOrderOpenQuote(orderType);

                        // Check if slippage requirements failed.
                        if (desiredPrice.HasValue && allowedSlippage.HasValue && currentPrice.HasValue
                            && allowedSlippage > 0 && Math.Abs(currentPrice.Value - desiredPrice.Value) > allowedSlippage)
                        {
                            operationResultMessage = "Slippage criteria not met.";
                            return false;
                        }

                        updatedInfo.State = OrderStateEnum.Executed;
                        updatedInfo.OpenPrice = currentPrice;
                        updatedInfo.OpenTime = quotes.Time.Value;
                    }
                    break;
                default:
                    operationResultMessage = "Order type not currently supported in back testing mode.";
                    return false;
            }

            updatedInfo.StopLoss = stopLoss;
            updatedInfo.TakeProfit = takeProfit;
            updatedInfo.Comment = comment;
            updatedInfo.Type = orderType;
            updatedInfo.Symbol = symbol;
            updatedInfo.Volume = volume;
            Interlocked.Increment(ref _pendingOrderId);
            updatedInfo.Id = _pendingOrderId.ToString();

            info = updatedInfo;
            order.Info = updatedInfo;

            lock (this)
            {
                _orders.Add(updatedInfo.Id, order);
            }

            BeginAccountInfoUpdate(_account.Info);
            BeginManagedOrdersUpdate(_lastDataQuote);

            operationResultMessage = string.Empty;
            return true; 
        }

        public void BeginManagedOrdersUpdate(Quote? quote)
        {
            //List<Order> orders = _provider.Account.TradeEntities.GetOrdersByState(OrderStateEnum.Executed | OrderStateEnum.Submitted);

            if (quote.HasValue == false)
            {
                SystemMonitor.OperationError("Quote not assigned.");
                return;
            }

            decimal? ask = quote.Value.Ask;
            decimal? bid = quote.Value.Bid;
            decimal? spread = quote.Value.Spread;

            if (ask.HasValue == false || bid.HasValue == false || spread.HasValue == false)
            {
                return;
            }

            foreach (Order order in _orders.Values)
            {
                if (order.State != OrderStateEnum.Executed
                    && order.State != OrderStateEnum.Submitted)
                {
                    continue;
                }

                if (order.IsDelayed && order.State == OrderStateEnum.Submitted)
                {// Pending order, check for opening conditions.
                    switch (order.Type)
                    {
                        case OrderTypeEnum.BUY_LIMIT_MARKET:
                            // Submitted buy below the current price.
                            if (order.OpenPrice >= ask)
                            {
                                DateTime? time = order.QuoteProvider != null && order.QuoteProvider.CurrentQuote.HasValue? order.QuoteProvider.CurrentQuote.Value.Time : null;
                                order.AcceptPendingExecuted(ask.Value, time);
                                string operationResultMessage;
                                UpdatePosition(order.Symbol, order.CurrentDirectionalVolume, out operationResultMessage);
                            }
                            break;
                        //case OrderTypeEnum.BUY_STOP_MARKET:
                        //    // Submitted buy below the current price.
                        //    if (order.OpenPrice >= ask)
                        //    {
                        //        order.AcceptPendingExecuted(ask.Value);
                        //    }
                        //    break;
                        case OrderTypeEnum.SELL_LIMIT_MARKET:
                            // Submitted sell above the current price.
                            if (order.OpenPrice <= bid)
                            {
                                DateTime? time = order.QuoteProvider != null && order.QuoteProvider.CurrentQuote.HasValue ? order.QuoteProvider.CurrentQuote.Value.Time : null;
                                order.AcceptPendingExecuted(bid.Value, time);
                                string operationResultMessage;
                                UpdatePosition(order.Symbol, order.CurrentDirectionalVolume, out operationResultMessage);
                            }
                            break;
                        //case OrderTypeEnum.SELL_STOP_MARKET:
                        //    // Submitted sell below the current price.
                        //    if (order.OpenPrice >= bid)
                        //    {
                        //        order.AcceptPendingExecuted(bid.Value);
                        //    }
                        //    break;
                    }


                    // Don't think this is needed as it should be satisfied by the above??
                    //
                    // A pending order is executed if it is between low and high.
                    //if (_lastDataBar.HasValue
                    //    && order.OpenPrice.HasValue
                    //    && order.State == OrderStateEnum.Submitted
                    //    && order.OpenPrice <= _lastDataBar.Value.High
                    //    && order.OpenPrice >= _lastDataBar.Value.Low)
                    //{
                    //    order.AcceptPendingExecuted(order.OpenPrice.Value + spread.Value);
                    //}
                }


                if (order.StopLoss.HasValue
                    && order.StopLoss.Value != 0)
                {// Check Stop Loss level.
                    if ((order.IsBuy && bid <= order.StopLoss) ||
                        (order.IsBuy == false && ask >= order.StopLoss))
                    {
                        if (order.State == OrderStateEnum.Executed)
                        {
                            string tmp;
                            ((PassiveOrder)order).CloseOrCancel(null, null, out tmp);
                            continue;
                        }
                    }
                }

                if (order.TakeProfit.HasValue
                    && order.TakeProfit.Value != 0)
                {// Check Take Profit level.
                    if ((order.IsBuy && bid >= order.TakeProfit) ||
                        (order.IsBuy == false && ask <= order.TakeProfit))
                    {
                        if (order.State == OrderStateEnum.Executed)
                        {
                            string tmp;
                            ((PassiveOrder)order).CloseOrCancel(null, null, out tmp);
                            continue;
                        }
                    }
                }

               
                if ((CheckForConflictsInsideBar(order)
                        || CheckStopLossInsideBar(order)
                        || CheckTakeProfitInsideBar(order))
                        && order.State == OrderStateEnum.Executed)
                {
                    string tmp;
                    ((PassiveOrder)order).CloseOrCancel(null, null, out tmp);
                    continue;
                }
            } // foreach
        }

        /// <summary>
        /// Update position parameters.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="volumeChange">*Directional* volume change, 0 for no change.</param>
        /// <param name="operationResultMessage"></param>
        /// <returns></returns>
        bool UpdatePosition(Symbol symbol, int volumeChange, out string operationResultMessage)
        {
            if (_manager == null || _dataDelivery == null || _dataDelivery.SourceId.IsEmpty)
            {
                operationResultMessage = "Not initialized properly for operation.";
                return false;
            }

            // Update the position corresponding to this order.
            Position position = _tradeEntities.GetPositionBySymbol(symbol, true);
            if (position == null)
            {
                operationResultMessage = "Failed to find corresponding position.";
                return false;
            }

            IQuoteProvider quotes = position.QuoteProvider;
            if (quotes == null)
            {
                operationResultMessage = "Failed to find corresponding quotation provider.";
                return false;
            }

            PositionInfo updatedPositionInfo = position.Info;
            if (updatedPositionInfo.Volume.HasValue == false)
            {
                updatedPositionInfo.Volume = 0;
            }

            updatedPositionInfo.PendingBuyVolume = 0;
            updatedPositionInfo.PendingSellVolume = 0;

            if (volumeChange != 0)
            {
                //decimal initialPositionResult = updatedPositionInfo.Result.Value;

                decimal? operationBasis = quotes.GetOrderOpenQuote(volumeChange > 0);
                if (operationBasis.HasValue == false)
                {
                    operationResultMessage = "Failed to establish order operation basis price.";
                    return false;
                }
                else
                {
                    //SystemMonitor.CheckError(updatedPositionInfo.Basis.HasValue, "Position has no properly assigned basis price.");
                }

                if (RecalculatePositionBasis(ref updatedPositionInfo, volumeChange, operationBasis.Value) == false)
                {
                    operationResultMessage = "Failed to recalculate position parameters.";
                    return false;
                }
            }

            decimal? pendingPrice = quotes.GetOrderCloseQuote(updatedPositionInfo.Volume > 0);
            
            if (updatedPositionInfo.Volume.HasValue && updatedPositionInfo.Volume != 0 
                && pendingPrice.HasValue && updatedPositionInfo.Basis.HasValue)
            {
                updatedPositionInfo.Result = (pendingPrice.Value - updatedPositionInfo.Basis.Value) * updatedPositionInfo.Volume.Value;
            }
            else
            {
                updatedPositionInfo.Result = 0;
            }

            bool positionClosing = (position.Volume > 0 && volumeChange < 0) || (position.Volume < 0 && volumeChange > 0);

            if (volumeChange != 0 && positionClosing && position.Info.Result.HasValue && updatedPositionInfo.Result.HasValue)
            {// Update the account upon closing some (or all) of the position volume.
                _runningAccountBalance += position.Info.Result.Value - updatedPositionInfo.Result.Value;
            }

            position.UpdateInfo(updatedPositionInfo);

            operationResultMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        static bool RecalculatePositionBasis(ref PositionInfo position, int volumeChange, decimal newBasis)
        {
            if (position.Volume == 0)
            {
                position.Basis = newBasis;
                position.Volume = volumeChange;
                return true;
            }

            if ((position.Volume > 0 && volumeChange > 0)
                || (position.Volume < 0 && volumeChange < 0))
            {// Same direction, basis recalculated as corresponding average.
                decimal currentSum = position.Volume.Value * position.Basis.Value;
                decimal newSum = volumeChange * newBasis;

                position.Volume = position.Volume + volumeChange;
                position.Basis = Math.Round((currentSum + newSum) / position.Volume.Value, 4);
            }
            else
            {// Different direction.
                if (Math.Abs(position.Volume.Value) < Math.Abs(volumeChange))
                {// New volume has overtaken existing volume.
                    position.Basis = newBasis;
                    position.Volume = position.Volume + volumeChange;
                }
                else
                {// New volume is only part of existing volume, basis remains unchaged.
                    position.Volume = position.Volume + volumeChange;
                }
            }

            if (position.Volume == 0)
            {
                position.Basis = 0;
            }

            return true;
        }

        public bool ModifyOrder(AccountInfo account, Order order, decimal? stopLoss, decimal? takeProfit,
            decimal? targetOpenPrice, out string modifiedId, out string operationResultMessage)
        {
            if (order.State != OrderStateEnum.Executed && order.State != OrderStateEnum.Suspended)
            {
                modifiedId = string.Empty;
                operationResultMessage = "Only submitted and executed orders can be modified.";
                return false;
            }
            
            modifiedId = order.Info.Id;
            operationResultMessage = string.Empty;

            OrderInfo modifiedInfo = order.Info;

            modifiedInfo.StopLoss = stopLoss;
            modifiedInfo.TakeProfit = takeProfit;
            modifiedInfo.OpenPrice = targetOpenPrice;

            RaiseOrderUpdateEvent(modifiedInfo, Order.UpdateTypeEnum.Modified);

            return true;
        }

        public bool DecreaseOrderVolume(AccountInfo account, Order order, decimal volumeDecreasal,
            decimal? allowedSlippage, decimal? desiredPrice, out decimal decreasalPrice,
            out string modifiedId, out string operationResultMessage)
        {
            decreasalPrice = 0;

            if (order.State != OrderStateEnum.Submitted)
            {
                modifiedId = string.Empty;
                operationResultMessage = "Only submitted orders can be modified in volume.";
                return false;
            }

            SystemMonitor.NotImplementedWarning();

            modifiedId = order.Info.Id;
            operationResultMessage = string.Empty;
            return true;

            #region Old Active Orders Implementation
            //            modifiedId = order.Id;
            //            decreasalPrice = 0;
            //            if (_provider.Account.TradeEntities.ContainsOrder(order) == false)
            //            {
            //                operationResultMessage = "Order [" + order.Guid + "] not registered in provider.";
            //                SystemMonitor.OperationError(operationResultMessage);
            //                return false;
            //            }

            //            decreasalPrice = decimal.MinValue;

            //            ISessionDataProvider dataProvider = SessionDataProvider;
            //            if (dataProvider == null || dataProvider.Quotes == null)
            //            {
            //                operationResultMessage = "Data provider not assigned or initialized properly.";
            //                SystemMonitor.OperationError(operationResultMessage);
            //                return false;
            //            }

            //            decimal? currentPrice = dataProvider.Quotes.Bid;
            //            if (order.IsSell)
            //            {// Sell order.
            //                currentPrice = dataProvider.Quotes.Ask;
            //            }

            //            if (desiredPrice.HasValue && allowedSlippage.HasValue
            //                && allowedSlippage > 0 &&
            //                (currentPrice.HasValue && Math.Abs(currentPrice.Value - desiredPrice.Value) > allowedSlippage))
            //            {//
            //                operationResultMessage = "Slippage criteria not met.";
            //                return false;
            //            }

            //            if (volumeDecreasal == order.CurrentVolume)
            //            {// Closing order.
            //                DateTime closingDateTime;
            //                CloseOrder(_provider.Account.Info, order, allowedSlippage, desiredPrice, out decreasalPrice, out closingDateTime, out modifiedId, out operationResultMessage);
            //                return true;
            //            }

            //            if (currentPrice.HasValue == false)
            //            {// 
            //                operationResultMessage = "Can not decrease order since dataDelivery provider has no Ask/Bid values.";
            //                return false;
            //            }

            //            operationResultMessage = string.Empty;
            //            // TODO: implement a proper decreasal model here, just like the cloing one.
            //            // This is a rough estimation.
            //            decreasalPrice = currentPrice.Value;

            //            RaiseOrderUpdateEvent(order.Info, Order.UpdateTypeEnum.VolumeChanged);

            //            return true;

            #endregion
        }

        public bool IncreaseOrderVolume(AccountInfo account, Order order, decimal volumeIncrease,
            decimal? allowedSlippage, decimal? desiredPrice, out decimal increasalPrice, out string modifiedId,
            out string operationResultMessage)
        {
            increasalPrice = 0;

            if (order.State != OrderStateEnum.Submitted)
            {
                modifiedId = string.Empty;
                operationResultMessage = "Only submitted orders can be modified in volume.";
                return false;
            }

            SystemMonitor.NotImplementedWarning();

            modifiedId = order.Info.Id;
            operationResultMessage = string.Empty;
            return true;

            #region Old Active Orders Implementation

            //            increasalPrice = 0;

            //            modifiedId = order.Id;

            //            lock (this)
            //            {
            //                if (_provider.Account.TradeEntities.GetOrderById(order.Id) == null)
            //                {
            //                    operationResultMessage = "Order [" + order.Guid + "] not registered in provider.";
            //                    SystemMonitor.OperationError(operationResultMessage);
            //                    return false;
            //                }
            //            }

            //            increasalPrice = decimal.MinValue;

            //            ISessionDataProvider dataProvider = SessionDataProvider;
            //            if (dataProvider == null || dataProvider.Quotes == null
            //                 || dataProvider.DataBars == null)
            //            {
            //                operationResultMessage = "Data provider not assigned.";
            //                SystemMonitor.OperationError(operationResultMessage);
            //                return false;
            //            }

            //            if (desiredPrice.HasValue == false)
            //            {
            //                desiredPrice = dataProvider.Quotes.Ask;
            //            }

            //            if (volumeIncrease == 0)
            //            {
            //                increasalPrice = desiredPrice.Value;
            //                operationResultMessage = "No change.";
            //                return true;
            //            }

            //            decimal? currentPrice = dataProvider.Quotes.Bid;
            //            if (order.IsBuy == false)
            //            {// Sell order.
            //                currentPrice = dataProvider.Quotes.Ask;
            //            }

            //            if (desiredPrice.HasValue && allowedSlippage.HasValue
            //                && allowedSlippage > 0 && (currentPrice.HasValue && Math.Abs(currentPrice.Value - desiredPrice.Value) > allowedSlippage))
            //            {// Fail.
            //                operationResultMessage = "Slippage criteria not met.";
            //                return false;
            //            }

            //            if (currentPrice.HasValue == false)
            //            {
            //                operationResultMessage = "Current price not assigned.";
            //                return false;
            //            }

            //            operationResultMessage = string.Empty;
            //            increasalPrice = currentPrice.Value;

            //            RaiseOrderUpdateEvent(order.Info, Order.UpdateTypeEnum.VolumeChanged);
            //            return true;

            #endregion
        }

        public bool CancelPendingOrder(AccountInfo account, Order order, out string modifiedId,
            out string operationResultMessage)
        {
            modifiedId = order.Id;

            if (_orders.ContainsValue(order) == false)
            {
                operationResultMessage = "Order [" + order.Id + "] not registered in provider.";
                SystemMonitor.OperationError(operationResultMessage);
                return false;
            }

            if (order.State != OrderStateEnum.Submitted)
            {
                operationResultMessage = "Order [" + order.Id + "] is not in submited state.";
                SystemMonitor.OperationError(operationResultMessage);
                return false;
            }

            operationResultMessage = string.Empty;
            //RaiseOrderUpdateEvent(order.Info, Order.UpdateTypeEnum.Canceled);
            return true;
        }

        public bool CheckStopLossInsideBar(Order order)
        {
            if (_lastDataBar.HasValue == false)
            {
                SystemMonitor.OperationError("Data provider not assigned.");
                return false;
            }

            // Check if we have reached the stoploss.
            if ((order.IsBuy && order.StopLoss != 0 && _lastDataBar.Value.Low < order.StopLoss)
                || (!order.IsBuy && order.StopLoss != 0 && !order.IsDelayed && _lastDataBar.Value.High > order.StopLoss))
            { // StopLoss has been triggered inside bar.
                return true;
            }

            return false;
        }

        public bool CheckTakeProfitInsideBar(Order order)
        {
            if (_lastDataBar.HasValue == false)
            {
                SystemMonitor.OperationError("Data provider not assigned.");
                return false;
            }

            // Check if we have reached the takeprofit.
            if (
                (order.IsBuy && order.TakeProfit != 0 && _lastDataBar.Value.High > order.TakeProfit) 
                || (!order.IsBuy && order.TakeProfit != 0 && _lastDataBar.Value.Low < order.TakeProfit)
                )
            {// TakeProfit has been triggered inside bar.
                return true;
            }

            return false;
        }

        public bool CheckForConflictsInsideBar(Order order)
        {
            if (_lastDataBar.HasValue == false)
            {
                SystemMonitor.OperationError("Data provider not assigned.");
                return false;
            }
            if (order.State != OrderStateEnum.Executed)
            {
                return false;
            }
            if (order.TakeProfit == 0 || order.StopLoss == 0)
            {
                return false;
            }

            //Check if we have a conflict : SL > lastbar.Low and TP < lastbar.High
            // if so there is a collision
            return (order.IsBuy &&
                    order.StopLoss > _lastDataBar.Value.Low &&
                    order.TakeProfit < _lastDataBar.Value.High) ||
                   (!order.IsBuy &&
                    order.StopLoss < _lastDataBar.Value.High &&
                    order.TakeProfit > _lastDataBar.Value.Low);
        }

        /// <summary>
        /// 
        /// </summary>
        bool GetProviders(Symbol symbol, out IQuoteProvider quotes, out IDataBarHistoryProvider bars)
        {
            quotes = _manager.ObtainQuoteProvider(_dataDelivery.SourceId, symbol);
            RuntimeDataSessionInformation sessionInformation = _dataDelivery.GetSymbolRuntimeSessionInformation(symbol);

            if (sessionInformation.AvailableDataBarPeriods == null || sessionInformation.AvailableDataBarPeriods.Count == 0)
            {
                quotes = null;
                bars = null;
                SystemMonitor.OperationError("Can not close order since no suitable data provider sessions found.");
                return false;
            }

            bars = _manager.ObtainDataBarHistoryProvider(_dataDelivery.SourceId, symbol, sessionInformation.AvailableDataBarPeriods[0]);

            if (quotes == null || quotes.Ask.HasValue == false || bars == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CloseOrder(AccountInfo account, Order order, decimal? allowedSlippage,
            decimal? desiredPrice, out decimal closingPrice, out DateTime closingTime,
            out string modifiedId, out string operationResultMessage)
        {
            closingPrice = decimal.MinValue;
            closingTime = DateTime.MinValue;
            modifiedId = order.Id;

            if (_orders.ContainsValue(order) == false)
            {
                operationResultMessage = "Order [" + order.Id + "] not registered in provider.";
                SystemMonitor.OperationError(operationResultMessage);
                return false;
            }

            if (_lastDataBar.HasValue == false || _lastDataQuote.HasValue == false)
            {
                operationResultMessage = "Data bar/quote information not available.";
                SystemMonitor.OperationError(operationResultMessage);
                return false;
            }

            decimal? currentPrice = _lastDataQuote.Value.Bid;
            if (order.IsBuy == false)
            {// Sell order.
                currentPrice = _lastDataQuote.Value.Ask;
            }

            if (desiredPrice.HasValue == false)
            {
                desiredPrice = currentPrice;
            }

            if (UpdatePosition(order.Symbol, -order.Info.DirectionalVolume, out operationResultMessage) == false)
            {
                return false;
            }

            //if (dataProvider.DataBars.Current.HasValue && currentPrice.HasValue)
            {//Check if we have reached the stoploss or takeprofit in last bar

                //Default case.
                closingPrice = currentPrice.Value;

                if (order.StopLoss.HasValue && CheckStopLossInsideBar(order))
                {// StopLoss has been triggered inside bar.
                    closingPrice = order.StopLoss.Value;
                }

                if (order.TakeProfit.HasValue && CheckTakeProfitInsideBar(order))
                {// TakeProfit has been triggered inside bar.
                    closingPrice = order.TakeProfit.Value;
                }

                //Check if both : StopLoss and TakeProfit has been triggered inside bar
                if (CheckForConflictsInsideBar(order))
                {//We have a collision here, If SL > lastbar.Low and TP < lastbar.High we do not know what have been triggered first : the SL or the TP
                    //To solve the situation, 4 posiblities are offered for now : best case, worst case, probabilistic case, exact case
                    switch (_resultsApproximationMode)
                    {
                        case ResultsAproximationModeEnum.PesimisticResult:
                            if (order.StopLoss.HasValue)
                            {
                                closingPrice = order.StopLoss.Value;
                            }
                            break;
                        case ResultsAproximationModeEnum.OptimisticResult:
                            if (order.TakeProfit.HasValue)
                            {
                                closingPrice = order.TakeProfit.Value;
                            }
                            break;
                        case ResultsAproximationModeEnum.MostProbableResult:
                            //Probabilistic aproximation : If open is nearest stoploss than takeprofit, stoploss win otherwise takeprofit win
                            if (order.TakeProfit.HasValue && order.StopLoss.HasValue)
                            {
                                decimal diffToStopLoss = Math.Abs(order.StopLoss.Value - _lastDataBar.Value.Open);
                                decimal diffToTakeProfit = Math.Abs(_lastDataBar.Value.Open - order.TakeProfit.Value);
                                if (diffToStopLoss > diffToTakeProfit)
                                {//Most probable that takeprofit was triggered
                                    closingPrice = order.TakeProfit.Value;
                                }
                                else
                                {//Most probable that stoploss was triggered
                                    closingPrice = order.StopLoss.Value;
                                }
                            }
                            break;
                        //case ResultsAproximationModeEnum.ExactResult:
                        //    // Not implemented yet
                        //    // To implement this part, we need tick by tick dataDelivery to know exactly what has happened inside bar
                        //    closingPrice = currentPrice;
                        //    break;
                    }
                }
            }
            //else if (currentPrice.HasValue)
            //{
            //    closingPrice = currentPrice.Value;
            //}
            //else
            //{
            //    operationResultMessage = "Current price has no value.";
            //    return false;
            //}

            if (desiredPrice.HasValue && allowedSlippage.HasValue
                && allowedSlippage > 0 && Math.Abs(currentPrice.Value - desiredPrice.Value) > allowedSlippage)
            {
                operationResultMessage = "Slippage criteria not met.";
                return false;
            }

            closingTime = _lastDataQuote.Value.Time.Value;
            operationResultMessage = string.Empty;

            RaiseOrderUpdateEvent(order.Info, Order.UpdateTypeEnum.Closed);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool CheckStopLossInsideBar(IDataBarHistoryProvider provider, Order order)
        {
            if (provider.Current.HasValue)
            {//Check if we have reached the stoploss
                if ((order.IsBuy && order.StopLoss != 0 &&
                       provider.Current.Value.Low < order.StopLoss) ||
                    (!order.IsBuy && order.StopLoss != 0 &&
                        !order.IsDelayed &&
                    provider.Current.Value.High > order.StopLoss))
                {//StopLoss has been triggered inside bar
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public void Dispose()
        {
            _manager = null;
            _dataDelivery = null;
        }

        #region IOrderSink Members

        public bool BeginAccountInfoUpdate(AccountInfo accountInfo)
        {
            accountInfo.Balance = _runningAccountBalance;
            decimal totalPositionsMarketValue = 0;

            string updateResultMessage;

            List<Position> positions = _tradeEntities.Positions;

            lock (this)
            {
                foreach (Position position in positions)
                {
                    UpdatePosition(position.Symbol, 0, out updateResultMessage);

                    accountInfo.Balance += position.Result;

                    if (position.Info.MarketValue.HasValue)
                    {
                        totalPositionsMarketValue += position.Info.MarketValue.Value;
                    }
                }
            }

            accountInfo.Margin = _runningAccountBalance;

            accountInfo.Profit = accountInfo.Balance - _runningAccountBalance;
            accountInfo.Credit = 0;
            accountInfo.Equity = accountInfo.Balance;
            accountInfo.FreeMargin = accountInfo.CalcuateFreeMargin(totalPositionsMarketValue);

            _account.AdoptInfo(accountInfo);
            return true;
        }

        #endregion


    }
}
