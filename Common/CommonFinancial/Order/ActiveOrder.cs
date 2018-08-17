using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// The base class for all orders. ActiveOrder encapsulates all information regarding a placed, pending, open or closed order.
    /// Also logics and information regarding stop loss levels or take profit levels is contained here.
    /// Employs automated serialization.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Trading Order")]
    public class ActiveOrder : Order, IDeserializationCallback
    {
        decimal _orderMaximumResultAchieved = 0;
        /// <summary>
        /// The maximum positive result the order has achieved, since its opening.
        /// Applicable only to open orders.
        /// </summary>
        public decimal OrderMaximumResultAchieved
        {
            get
            {
                lock (this)
                {
                    return _orderMaximumResultAchieved;
                }
            }
        }
        
        volatile string _defaultComment = "";
        /// <summary>
        /// Default comment to apply to order.
        /// </summary>
        public string DefaultComment
        {
            get { return _defaultComment; }
            set { _defaultComment = value; }
        }

        DateTime? _localOpenTime = null;
        /// <summary>
        /// Open time at local platform (OFxP).
        /// </summary>
        public DateTime? LocalOpenTime
        {
            get { lock (this) { return _localOpenTime; } }
        }

        #region Events and Delegates

        public delegate void OrderOperationDelegate(ActiveOrder order, bool result, string operationResultMessage);

        #endregion

        #region Instance Control

        /// <summary>
        /// Constructor, allows direct order initialization.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="initialize">Initialize order on construction. If init fails, exception will occur.</param>
        public ActiveOrder(ISourceManager manager, ISourceOrderExecution executionProvider,
            ComponentId dataSourceId, bool initialize)
            : base(manager, executionProvider, dataSourceId)
        {
            State = OrderStateEnum.UnInitialized;

            if (initialize)
            {
                SystemMonitor.CheckThrow(Initialize());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnDeserialization(object sender)
        {
            State = OrderStateEnum.UnInitialized;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Initialization.
        /// </summary>
        public bool Initialize()
        {
            SystemMonitor.CheckThrow(State == OrderStateEnum.UnInitialized, "Order already initialized.");

            ISourceOrderExecution executionProvider = _executionProvider;

            if (Symbol.IsEmpty == false && QuoteProvider != null)
            {// This will only work for deserialized orders, or ones that already have adopted info.
                QuoteProvider.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
            }

            State = OrderStateEnum.Initialized;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {
            if (QuoteProvider != null)
            {
                QuoteProvider.QuoteUpdateEvent -= new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
            }

            State = OrderStateEnum.UnInitialized;
        }

        #endregion

        /// <summary>
        /// Handle dataDelivery updates; used by children types to implement placement strategies.
        /// </summary>
        protected virtual void Quote_QuoteUpdateEvent(IQuoteProvider provider)
        {
        }

        /// <summary>
        /// Will create the corresponding order, based to the passed in order information.
        /// Used to create corresponding orders to ones already existing in the platform.
        /// </summary>
        public override bool AdoptInfo(OrderInfo info)
        {
            bool subscribeToProvider = QuoteProvider == null;

            base.AdoptInfo(info);

            ISourceOrderExecution executionProvider = _executionProvider;
            if (executionProvider == null || SessionInfo.HasValue == false)
            {
                return false;
            }

            if (subscribeToProvider && QuoteProvider != null)
            {
                QuoteProvider.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
            }

            decimal lotSize = SessionInfo.Value.LotSize;

            lock (this)
            {
                //if (orderInfo.CurrentProfit.HasValue)
                //{
                //    _currentRawResult = orderInfo.CurrentProfit.Value / lotSize;
                //}
                //else
                //{
                //    _currentRawResult = 0;
                //}

                if (info.CurrentProfit.HasValue)
                {
                    _orderMaximumResultAchieved = info.CurrentProfit.Value / lotSize;
                }

                _localOpenTime = null;
            }

            return true;
        }

        ///// <summary>
        ///// Calculate order result.
        ///// </summary>
        //public override decimal? GetResult(ResultModeEnum mode)
        //{
        //    ISourceOrderExecution executionProvider = _executionProvider;

        //    if (executionProvider == null || executionProvider.OperationalState != OperationalStateEnum.Operational
        //        || QuoteProvider == null || QuoteProvider.OperationalState != OperationalStateEnum.Operational
        //        || SessionInfo.HasValue == false
        //        || this.Account == null)
        //    {
        //        return null;
        //    }

        //    if (State != OrderStateEnum.Executed)
        //    {
        //        // TODO : verify the calculation and usage of the results in other states.
        //        // There was a blocking call on the Order.GetResult that prevented all 
        //        // other states from receiving results.
        //        return null;
        //    }

        //    return Order.GetResult(mode, this.OpenPrice, this.ClosePrice, this.CurrentVolume, this.Symbol, this.State, this.Type, 
        //        CurrencyConversionManager.Instance, this.Account.Info.BaseCurrency, SessionInfo.Value.LotSize,
        //        SessionInfo.Value.DecimalPlaces, QuoteProvider.Ask, QuoteProvider.Bid);

        //    //decimal? currentRawResult = null;
        //    //if (State == OrderStateEnum.Executed)
        //    //{
        //    //    if (State != OrderStateEnum.Executed || OpenPrice.HasValue == false)
        //    //    {// Failed to get result.
        //    //        currentRawResult = 0;
        //    //    }
        //    //    else
        //    //    {
        //    //        // Update result.
        //    //        currentRawResult = GetRawResult(this.OpenPrice, this.CurrentVolume, this.State, this.Type,
        //    //            executionProvider.QuoteProvider.Ask, executionProvider.QuoteProvider.Bid, mode != ResultModeEnum.Pips);
        //    //    }

        //    //    lock (this)
        //    //    {
        //    //        //_currentRawResult = currentRawResult;
        //    //        _orderMaximumResultAchieved = Math.Max(_orderMaximumResultAchieved, currentRawResult.HasValue ? currentRawResult.Value : 0);
        //    //    }
        //    //}

        //    //if (currentRawResult.HasValue == false)
        //    //{
        //    //    return null;
        //    //}

        //    //int decimalPlaces = (int)executionProvider.Info.DecimalPlaces;
        //    //decimal lotSize = executionProvider.Info.LotSize;
        //    //decimal currency = currentRawResult.Value * lotSize;

        //    //lock (this)
        //    //{
        //    //    if (mode == ResultModeEnum.Pips)
        //    //    {
        //    //        if (State == OrderStateEnum.Closed)
        //    //        {// When closed we need to compensate the 
        //    //            if (IsBuy)
        //    //            {
        //    //                return (_info.ClosePrice - _info.OpenPrice) * (decimal)Math.Pow(10, decimalPlaces);
        //    //            }
        //    //            else
        //    //            {
        //    //                return (_info.OpenPrice - _info.ClosePrice) * (decimal)Math.Pow(10, decimalPlaces);
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            return currentRawResult.Value * (decimal)Math.Pow(10, decimalPlaces);
        //    //        }
        //    //    }
        //    //    else if (mode == ResultModeEnum.Raw)
        //    //    {
        //    //        return currentRawResult;
        //    //    }
        //    //    else if (mode == ResultModeEnum.Currency)
        //    //    {
        //    //        return currency;
        //    //    }
        //    //}

        //    //SystemMonitor.NotImplementedCritical("Mode not supported.");
        //    //return 0;
        //}

        ///// <summary>
        ///// Change order execution related parameters.
        ///// </summary>
        ///// <param name="remoteStopLoss">Applicable to open or pending orders only, pass null to skip, decimal.MinValue (or zero) to signify no value.</param>
        ///// <param name="remoteTakeProfit">Applicable to open or pending orders only, pass null to skip, decimal.MinValue (or zero) to signify no value.</param>
        ///// <param name="operationResultMessage">Applicable ONLY to pending order. Pass null otherwise or to leave unchanged.</param>
        ///// <returns></returns>
        //public override bool ModifyRemoteParameters(decimal? remoteStopLoss, decimal? remoteTakeProfit, decimal? remoteTargetOpenPrice, 
        //    out string operationResultMessage)
        //{
        //    if (this.IsOpenOrPending == false)
        //    {
        //        operationResultMessage = "Wrong order state.";
        //        return false;
        //    }

        //    if (State != OrderStateEnum.Submitted && remoteTargetOpenPrice.HasValue)
        //    {
        //        operationResultMessage = "Wrong order state for this operation (operation only applicable to pending orders).";
        //        return false;
        //    }

        //    if (remoteStopLoss == StopLoss && TakeProfit == remoteTakeProfit)
        //    {
        //        // remoteTargetOpenPrice only counts if you do it on a pending order.
        //        if ((State == OrderStateEnum.Submitted && remoteTargetOpenPrice == OpenPrice)
        //            || (State != OrderStateEnum.Submitted))
        //        {// No changes needed.
        //            operationResultMessage = "No changes needed.";
        //            return true;
        //        }
        //    }
            
        //    operationResultMessage = "Session not assigned.";
        //    ISourceOrderExecution executionProvider = _executionProvider;
        //    string modifiedId;
        //    if (executionProvider == null || OrderExecutionProvider.ModifyOrder(Account.Info, this, remoteStopLoss, remoteTakeProfit, remoteTargetOpenPrice, 
        //        out modifiedId, out operationResultMessage) == false)
        //    {
        //        return false;
        //    }

        //    return true;

        //    //lock (this)
        //    //{
        //    //    //_info.Id = modifiedId;
        //    //    if (remoteStopLoss.HasValue)
        //    //    {
        //    //        _info.StopLoss = remoteStopLoss;
        //    //    }

        //    //    if (remoteTakeProfit.HasValue)
        //    //    {
        //    //        _info.TakeProfit = remoteTakeProfit;
        //    //    }

        //    //    if (remoteTargetOpenPrice.HasValue)
        //    //    {
        //    //        _info.OpenPrice = remoteTargetOpenPrice.Value;
        //    //    }
        //    //}

        //    //RaiseOrderUpdatedEvent(UpdateTypeEnum.Modified);
        //}

        /// <summary>
        /// Print order information.
        /// </summary>
        public override string Print(bool fullPrint)
        {
            if (Symbol != Symbol.Empty)
            {
                return string.Format("Symbol {0} Type {1}, Open {2} at {3}", Symbol.Name, Type.ToString(), OpenPrice.ToString(), LocalOpenTime.ToString());
            }
            else
            {
                return "Order not initialized.";
            }
        }

        #region User Operations

        /// <summary>
        /// Submit a buy order. Note it may take some time for the order to execute (or a failure might occur).
        /// </summary>
        /// <param name="closeVolume"></param>
        /// <returns></returns>
        public virtual bool SubmitBuyOrder(int volume)
        {
            return Submit(OrderTypeEnum.BUY_MARKET, volume);
        }

        /// <summary>
        /// Submit a buy order. Note it may take some time for the order to execute (or a failure might occur).
        /// </summary>
        /// <param name="closeVolume"></param>
        /// <returns></returns>
        public virtual bool SubmitSellOrder(int volume)
        {
            return Submit(OrderTypeEnum.SELL_MARKET, volume);
        }

        /// <summary>
        /// Will use current platform prices for the operation.
        /// </summary>
        public virtual bool Submit(OrderTypeEnum orderType, int volume, out string operationResultMessage)
        {
            // It is possible to place an order even though the dataDelivery provider is not available.
            decimal? price = null;
            
            if (QuoteProvider == null)
            {
                if (orderType == OrderTypeEnum.SELL_MARKET || orderType == OrderTypeEnum.SELL_LIMIT_MARKET || orderType == OrderTypeEnum.SELL_STOP_MARKET)
                {// Sell.
                    price = QuoteProvider.Bid;
                }
                else
                {// Buy
                    price = QuoteProvider.Ask;
                }
            }

            return Submit(orderType, volume, this.DefaultExecutionSlippage, price, 0, 0, DefaultComment, out operationResultMessage);
        }

        /// <summary>
        /// Will use current platform prices for the operation.
        /// </summary>
        public virtual bool Submit(OrderTypeEnum orderType, int volume)
        {
            decimal? price = null;
            
            if (QuoteProvider != null)
            {
                if (orderType == OrderTypeEnum.SELL_MARKET || orderType == OrderTypeEnum.SELL_LIMIT_MARKET || orderType == OrderTypeEnum.SELL_STOP_MARKET)
                {// Sell.
                    price = QuoteProvider.Bid;
                }
                else
                {
                    price = QuoteProvider.Ask; // Buy
                }
            }

            return Submit(orderType, volume, this.DefaultExecutionSlippage, price, DefaultComment);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Submit(OrderTypeEnum orderType, int volume, decimal? allowedSlippage,
            decimal? desiredPrice, string comment)
        {
            string message;
            return Submit(orderType, volume, allowedSlippage, desiredPrice, 0, 0, comment, out message);
        }
        
        /// <summary>
        /// This allows more specific control over the operation.
        /// </summary>
        public bool Submit(OrderTypeEnum orderType, int volume, decimal? allowedSlippage, 
            decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss, string comment, out string operationResultMessage)
        {
            SystemMonitor.CheckThrow(volume > 0, "Misuse of the Order class.");

            if (State != OrderStateEnum.Initialized)
            {
                operationResultMessage = "Misuse of the Order class [Order not initialized; or Must not place trade, that has already been placed].";
                SystemMonitor.Warning(operationResultMessage);
                return false;
            }

            ISourceOrderExecution executionProvider = _executionProvider;

            operationResultMessage = "Session not assigned.";

            if (desiredPrice.HasValue == false)
            {
                desiredPrice = OrderInfo.TypeIsBuy(orderType) ? QuoteProvider.Bid : QuoteProvider.Ask;
            }

            if (executionProvider == null)
            {// Placement of order failed.
                State = OrderStateEnum.Failed;
                SystemMonitor.Report("Order was not executed [" + operationResultMessage + "].");
                return false;
            }

            string id = OrderExecutionProvider.SubmitOrder(Account.Info, this, Symbol, orderType, volume,
                    allowedSlippage, desiredPrice, takeProfit, stopLoss, comment, out operationResultMessage);

            if (string.IsNullOrEmpty(id))
            {
                State = OrderStateEnum.Failed;
                SystemMonitor.OperationError("Order was not executed [" + operationResultMessage + "].");
                return false;
            }

            lock(this)
            {
                _info.Type = orderType;
                _initialVolume = volume;
                _info.Id = id;
                _info.Volume = volume;

                _info.StopLoss = stopLoss;
                _info.TakeProfit = takeProfit;

                State = OrderStateEnum.Submitted;
                _localOpenTime = DateTime.Now;
            }

            Account.TradeEntities.AddOrder(this);

            if (State == OrderStateEnum.Executed)
            {
                RaiseOrderUpdatedEvent(UpdateTypeEnum.Executed);
            }
            else
            {
                RaiseOrderUpdatedEvent(UpdateTypeEnum.Submitted);
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool BeginDecreaseVolume(int volumeDecrease, decimal? allowedSlippage, decimal? desiredPrice,
            out string operationResultMessage, OrderOperationDelegate operationFinishedDelegate)
        {
            operationResultMessage = string.Empty;
            
            GeneralHelper.FireAndForget(delegate()
            {
                string tmp;
                bool result = DecreaseVolume(volumeDecrease, allowedSlippage, desiredPrice, out tmp);

                if (operationFinishedDelegate != null)
                {
                    operationFinishedDelegate(this, result, tmp);
                }
            });

            return true;
        }


        /// <summary>
        /// This allows a part of the order to be closed, or all.
        /// </summary>
        public override bool DecreaseVolume(int volumeDecrease, decimal? allowedSlippage, decimal? desiredPrice, 
            out string operationResultMessage)
        {
            if (volumeDecrease == 0)
            {
                operationResultMessage = string.Empty;
                return true;
            }

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

            if (volumeDecrease < 0)
            {
                operationResultMessage = "Positive volume decrease required.";
                return false;
            }

            if (CurrentVolume < volumeDecrease)
            {
                operationResultMessage = "Misuse of the Order class [Can not close more volume than already open].";
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
            if (_info.Volume == volumeDecrease)
            {// Close/Cancel order.
                if (State == OrderStateEnum.Executed)
                {
                    operationResult = OrderExecutionProvider.CloseOrder(Account.Info, this, allowedSlippage, desiredPrice, out operationPrice, out closeTime, out modifiedId, out operationResultMessage);
                }
                else
                {
                    operationPrice = decimal.MinValue;
                    operationResult = OrderExecutionProvider.CancelPendingOrder(Account.Info, this, out modifiedId, out operationResultMessage);
                }
            }
            else
            {// Decrease order closeVolume.
                operationResult = OrderExecutionProvider.DecreaseOrderVolume(Account.Info, this, volumeDecrease, allowedSlippage, desiredPrice, out operationPrice, out modifiedId, out operationResultMessage);
            
            }

            if (operationResult == false)
            {
                SystemMonitor.Report("Order volume decrease has failed in executioner.");
                return false;
            }

            if (string.IsNullOrEmpty(modifiedId))
            {// Since the original order has changed its ticket number; and we have failed to establish the new one - we can no longer track it so unregister.
                SystemMonitor.OperationWarning("Failed to establish new modified order ticket; order will be re-aquired.", TracerItem.PriorityEnum.High);
                Account.TradeEntities.RemoveOrder(this);

                return true;
            }

            if (State == OrderStateEnum.Executed)
            {
                if (modifiedId != this.Id)
                {
                    Account.TradeEntities.RemoveOrder(this);

                    OrderInfo newUpdatedInfo = _info;
                    newUpdatedInfo.Id = modifiedId;
                    newUpdatedInfo.Volume = _info.Volume - volumeDecrease;

                    ActiveOrder updatedOrder = new ActiveOrder(Manager, _executionProvider, _dataSourceId, true);
                    updatedOrder.AdoptInfo(newUpdatedInfo);
                    _executionProvider.TradeEntities.AddOrder(updatedOrder);

                    // Request updated order info for this and new one and remove current one.
                    if (_executionProvider != null && _executionProvider.DefaultAccount != null && string.IsNullOrEmpty(modifiedId) == false)
                    {
                        _executionProvider.BeginOrdersInformationUpdate(_executionProvider.DefaultAccount.Info, new string[] { this.Id, modifiedId }, out operationResultMessage);
                    }
                }
                else
                {
                    _info.Volume = _info.Volume - volumeDecrease;

                    if (_info.Volume == 0)
                    {
                        State = OrderStateEnum.Closed;
                        _info.CloseTime = closeTime;
                        _info.ClosePrice = operationPrice;
                    }

                }
            }
            else if (State == OrderStateEnum.Submitted)
            {
                lock (this)
                {
                    _initialVolume -= volumeDecrease;
                    _info.Volume = _initialVolume;

                    if (_info.Volume == 0)
                    {
                        State = OrderStateEnum.Canceled;
                    }
                }
            }

            if (State == OrderStateEnum.Closed)
            {// Closed.
                RaiseOrderUpdatedEvent(UpdateTypeEnum.Closed);
            }
            else
            {// Still open.
                RaiseOrderUpdatedEvent(UpdateTypeEnum.VolumeChanged);
            }

            return true;
        }

        ///// <summary>
        ///// Currently, increase is allowed to pending orders only.
        ///// </summary>
        //public bool IncreaseVolume(int volumeIncrease, decimal? allowedSlippage, decimal? desiredPrice,
        //    out string operationResultMessage)
        //{
        //    return false;

        //    //if (volumeIncrease == 0)
        //    //{
        //    //    operationResultMessage = "OK.";
        //    //    return true;
        //    //}

        //    //if (volumeIncrease < 0)
        //    //{
        //    //    operationResultMessage = "Positive minVolume decrease required.";
        //    //    return false;
        //    //}

        //    //if (State != OrderStateEnum.Submitted)
        //    //{
        //    //    operationResultMessage = "Volume increase allowed to pending orders only.";
        //    //    return false;
        //    //}

        //    //decimal operationPrice;

        //    //ISourceOrderExecution executionProvider = _executionProvider;

        //    //if (executionProvider == null)
        //    //{
        //    //    operationResultMessage = "Session not assigned.";
        //    //    return false;
        //    //}

        //    //string modifiedId;
        //    //if (OrderExecutionProvider.IncreaseOrderVolume(Account.Info, this, volumeIncrease, allowedSlippage, desiredPrice, 
        //    //    out operationPrice, out modifiedId, out operationResultMessage) == false)
        //    //{
        //    //    SystemMonitor.Report("Order minVolume decrease has failed in executioner.");
        //    //    return false;
        //    //}

        //    //if (modifiedId == this.Id)
        //    //{
        //    //    RaiseOrderUpdatedEvent(UpdateTypeEnum.VolumeChanged);
        //    //    lock (this)
        //    //    {
        //    //        _info.Id = modifiedId;
        //    //        _initialVolume = _initialVolume + volumeIncrease;
        //    //        _info.Volume = _initialVolume;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    // Request updated order info for this and new one and remove current one.
        //    //    if (string.IsNullOrEmpty(modifiedId) == false)
        //    //    {
        //    //        _executionProvider.BeginOrdersInformationUpdate(_executionProvider.DefaultAccount.Info, new string[] { this.Id, modifiedId }, out operationResultMessage);
        //    //    }

        //    //    Account.TradeEntities.RemoveOrder(this);
        //    //}

        //    //return true;
        //}

        /// <summary>
        /// Applicable to pending/delayed orders only.
        /// </summary>
        /// <returns></returns>
        public bool Cancel(out string operationResultMessage)
        {
            if (State != OrderStateEnum.Submitted)
            {
                operationResultMessage = "Not pending order can not be canceled.";
                SystemMonitor.OperationError(operationResultMessage);
                return false;
            }

            string modifiedId;
            if (OrderExecutionProvider.CancelPendingOrder(Account.Info, this, out modifiedId, out operationResultMessage))
            {
                State = OrderStateEnum.Canceled;
                //_info.Id = modifiedId;

                RaiseOrderUpdatedEvent(UpdateTypeEnum.Canceled);

                return true;
            }

            return false;
        }



        /// <summary>
        /// Use to see how order is performing at current stage, considerVolume == false to see absolute values.
        /// This will consider accumulated results so far only if considerVolume is true.
        /// ConsiderVolume is to signify should the calculation observe order closeVolume at all.
        /// autoLotSizeCompensation - since lot sizes seem to change, this compensates with a starting point of 100,000
        /// (for ex. the GBPJPY with value 141.000 the lot is size reduced 100 times)
        /// </summary>
        //protected decimal? GetRawResultAtPrice(decimal? ask, decimal? bid, bool considerVolume)
        //{
        //    if (ask.HasValue == false || bid.HasValue == false)
        //    {
        //        return null;
        //    }

        //    ISourceOrderExecution executionProvider = _executionProvider;

        //    if (State != OrderStateEnum.Executed || executionProvider == null
        //        || executionProvider.OperationalState != OperationalStateEnum.Operational
        //        || OpenPrice.HasValue == false)
        //        //executionProvider..Account == null || executionProvider.Account.OperationalState != OperationalStateEnum.Operational)
        //    {// Failed to get result.
        //        return 0;
        //    }

        //    lock (this)
        //    {
        //        decimal difference = 0;
        //        if (IsBuy)
        //        {
        //            difference = bid.Value - OpenPrice.Value;
        //        }
        //        else
        //        {
        //            difference = OpenPrice.Value - ask.Value;
        //        }

        //        if (considerVolume)
        //        {
        //            //return Math.Round(_totalResult + CurrentVolume * difference, 10);
        //            return Math.Round(CurrentVolume * difference, 10);
        //        }
        //        else
        //        {
        //            return Math.Round(difference, 10);
        //        }
        //    }
        //}

        #endregion


    }
}
