
//namespace ForexPlatform
//{
//    /// <summary>
//    /// </summary>
//    [Serializable]
//    [UserFriendlyName("Simulation Order Execution")]
//    public class LocalOrderExecutionProvider : Operational, ISourceOrderExecution, IDeserializationCallback
//    {
//        public Account[] Accounts
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public Account DefaultAccount
//        {
//            get { throw new NotImplementedException(); }
//        }

//        //string _name;
//        //public string Name
//        //{
//        //    get { return _name; }
//        //    set { _name = value; }
//        //}

//        //#region IOrderExecutioner Members

//        //public event OperationalStatusChangedDelegate OperationalStatusChangedEvent;

//        //volatile OperationalStateEnum _operationalState = OperationalStateEnum.UnInitialized;
//        //public OperationalStateEnum OperationalState
//        //{
//        //    get { return _operationalState; }
//        //}

//        ///// <summary>
//        ///// LiveOrderExecutioner does not support time control.
//        ///// </summary>
//        //public ITimeControlled TimeControl
//        //{
//        //    get { return this; }
//        //}

//        //public ReadOnlyCollection<ActiveOrder> OpenAndPendingOrders
//        //{
//        //    get
//        //    {
//        //        lock (this)
//        //        {
//        //            List<ActiveOrder> operationResult = new List<ActiveOrder>();
//        //            foreach (ActiveOrder order in _orders)
//        //            {
//        //                if (order.State == OrderInformation.OrderStateEnum.Executed
//        //                    || order.State == OrderInformation.OrderStateEnum.Submitted)
//        //                {
//        //                    operationResult.AddElement(order);
//        //                }
//        //            }

//        //            return operationResult.AsReadOnly();
//        //        }
//        //    }
//        //}

//        //public BaseCurrency BaseCurrency
//        //{
//        //    get { return _sessionInfo.BaseCurrency; }
//        //}

//        //public ReadOnlyCollection<ActiveOrder> TradeEntities { get { lock (this) { return _orders.AsReadOnly(); } } }

//        //#endregion

//        //int _orderIdIndex = 0;

//        //decimal _initialLeverage = 50;
//        //public decimal InitialLeverage
//        //{
//        //    get { return _initialLeverage; }
//        //    set { _initialLeverage = value; }
//        //}

//        //public int TotalStepsCount
//        //{
//        //    get { return 0; }
//        //}

//        //public int CurrentStep
//        //{
//        //    get { return 0; }
//        //}

//        //LocalSimulationExecutionAccount _executionAccount = new LocalSimulationExecutionAccount();
//        //public Account Account
//        //{
//        //    get { return _executionAccount; }
//        //}

//        //decimal _initialBalance = 10000;
//        //public decimal InitialBalance
//        //{
//        //    get { return _initialBalance; }
//        //    set { _initialBalance = value; }
//        //}

//        //volatile string _accountCompany = "NA";
//        //public string AccountCompany
//        //{
//        //    get { return _accountCompany; }
//        //    set { _accountCompany = value; }
//        //}

//        //public event CurrentStepChangedDelegate CurrentStepChangedEvent;

//        ///// <summary>
//        /////
//        ///// </summary>
//        //public LocalOrderExecutionProvider(Info account)
//        //{
//        //    _sessionInfo = account;
//        //}

//        //public void Dispose()
//        //{
//        //    lock (this)
//        //    {
//        //        foreach (ActiveOrder order in _orders)
//        //        {
//        //            order.Dispose();
//        //        }
//        //        _orders.Clear();
//        //        _executionAccount.Dispose();
//        //        _executionAccount = null;
//        //        _sessionDataProvider = null;
//        //    }
//        //}

//        //#region IDeserializationCallback Members

//        //public void OnDeserialization(object sender)
//        //{
//        //    _orders = new List<ActiveOrder>();
//        //}

//        //#endregion


//        //public bool LoadFromFile(ISessionDataProvider dataProvider)
//        //{
//        //    SystemMonitor.CheckThrow(dataProvider == null || dataProvider.Info.CompareTo(_sessionInfo) == 0, "Mixed constructor and initialization sessions.");

//        //    lock (this)
//        //    {
//        //        _sessionDataProvider = dataProvider;

//        //        _executionAccount.InitializeInfo(_initialBalance, 0, _accountCompany, _sessionDataProvider.Info.BaseCurrency.Name, _initialBalance,
//        //           0, _initialLeverage, 0, _sessionDataProvider.Info.BaseCurrency.Name + " Simulation Account", 1, 0, "Local");

//        //        _executionAccount.LoadFromFile(this);

//        //        this.Name = "Simulation ActiveOrder Executioner of " + _sessionDataProvider.Info.Name;

//        //        if (_sessionDataProvider != null && _sessionDataProvider.Quote != null)
//        //        {
//        //            _sessionDataProvider.Quote.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
//        //        }

//        //        foreach (ActiveOrder order in _orders)
//        //        {
//        //            order.LoadFromFile();
//        //        }
//        //    }

//        //    OperationalStateEnum previousState = _operationalState;
//        //    _operationalState = OperationalStateEnum.Operational;
//        //    if (OperationalStatusChangedEvent != null)
//        //    {
//        //        OperationalStatusChangedEvent(this, previousState);
//        //    }

//        //    return OperationalState == OperationalStateEnum.Operational;
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="provider"></param>
//        //void Quote_QuoteUpdateEvent(IQuoteProvider provider)
//        //{
//        //    foreach (ActiveOrder order in _orders)
//        //    {
//        //        if (order.State != OrderInformation.OrderStateEnum.Executed
//        //            && order.State != OrderInformation.OrderStateEnum.Submitted)
//        //        {
//        //            continue;
//        //        }

//        //        if (order.IsDelayed && order.State == OrderInformation.OrderStateEnum.Submitted)
//        //        {// Pending order, check for opening conditions.
//        //            switch (order.Type)
//        //            {
//        //                case OrderTypeEnum.BUY_LIMIT_MARKET:
//        //                    // Submitted buy above the current price.
//        //                    if (order.OpenPrice <= provider.Ask)
//        //                    {
//        //                        order.AcceptPendingExecuted(provider.Ask);
//        //                    }
//        //                    break;
//        //                case OrderTypeEnum.BUY_STOP_MARKET:
//        //                    // Submitted buy below the current price.
//        //                    if (order.OpenPrice >= provider.Ask)
//        //                    {
//        //                        order.AcceptPendingExecuted(provider.Ask);
//        //                    }
//        //                    break;
//        //                case OrderTypeEnum.SELL_LIMIT_MARKET:
//        //                    // Submitted sell above the current price.
//        //                    if (order.OpenPrice <= provider.Bid)
//        //                    {
//        //                        order.AcceptPendingExecuted(provider.Bid);
//        //                    }
//        //                    break;
//        //                case OrderTypeEnum.SELL_STOP_MARKET:
//        //                    // Submitted sell below the current price.
//        //                    if (order.OpenPrice >= provider.Bid)
//        //                    {
//        //                        order.AcceptPendingExecuted(provider.Bid);
//        //                    }
//        //                    break;
//        //            }

//        //            // A pending order is executed if it is between low and high.
//        //            if (_sessionDataProvider.DataBarHistory != null 
//        //                && order.State == OrderInformation.OrderStateEnum.Submitted
//        //                && _sessionDataProvider.DataBarHistory.CurrentDataBar.HasValue &&
//        //                order.OpenPrice <= _sessionDataProvider.DataBarHistory.CurrentDataBar.Value.High &&
//        //                order.OpenPrice >= _sessionDataProvider.DataBarHistory.CurrentDataBar.Value.Low)
//        //            {
//        //                order.AcceptPendingExecuted(order.OpenPrice + provider.Spread);
//        //            }
//        //        }


//        //        if (GeneralHelper.IsValid(order.StopLoss)
//        //            && order.StopLoss != 0)
//        //        {// Check Stop Loss level.
//        //            if ((order.IsBuy && provider.Bid <= order.StopLoss) ||
//        //                (order.IsBuy == false && provider.Ask >= order.StopLoss))
//        //            {
//        //                if (order.State == OrderInformation.OrderStateEnum.Executed)
//        //                {
//        //                    order.Close();
//        //                }
//        //            }
//        //        }
//        //        if (GeneralHelper.IsValid(order.TakeProfit)
//        //            && order.TakeProfit != 0)
//        //        {// Check Take Profit level.
//        //            if ((order.IsBuy && provider.Bid >= order.TakeProfit) ||
//        //                (order.IsBuy == false && provider.Ask <= order.TakeProfit))
//        //            {
//        //                if (order.State == OrderInformation.OrderStateEnum.Executed)
//        //                {
//        //                    order.Close();
//        //                }
//        //            }
//        //        }

//        //        if ((CheckForConflictsInsideBar(order)
//        //            || CheckStopLossInsideBar(order)
//        //            || CheckTakeProfitInsideBar(order))
//        //            && order.State == OrderInformation.OrderStateEnum.Executed)
//        //        {
//        //            order.Close();
//        //        }
//        //    }
//        //}

//        //public void UnInitialize()
//        //{
//        //    OperationalStateEnum previousState = _operationalState;
//        //    lock (this)
//        //    {
//        //        _operationalState = OperationalStateEnum.UnInitialized;
//        //        _executionAccount.UnInitialize();
//        //        if (_sessionDataProvider != null && _sessionDataProvider.Quote != null)
//        //        {
//        //            _sessionDataProvider.Quote.QuoteUpdateEvent -= new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
//        //        }

//        //        foreach (ActiveOrder order in _orders)
//        //        {
//        //            order.UnInitialize();
//        //        }
//        //    }

//        //    if (OperationalStatusChangedEvent != null)
//        //    {
//        //        OperationalStatusChangedEvent(this, previousState);
//        //    }
//        //}

//        ///// <summary>
//        ///// Obtains all the orders from the order executioner.
//        ///// </summary>
//        //public bool ObtainAllOrders(out string operationResultMessage)
//        //{
//        //    operationResultMessage = string.Empty;
//        //    return true;
//        //}

//        //protected void RegisterOrder(ActiveOrder order)
//        //{
//        //    lock (this)
//        //    {
//        //        SystemMonitor.CheckThrow(_orders.Contains(order) == false, "ActiveOrder already added.");
//        //        _orders.AddElement(order);

//        //        order.Id.ExecutionPlatformId = _orderIdIndex.ToString();
//        //        _orderIdIndex++;

//        //        order.OrdersUpdatedEvent += new ActiveOrder.OrderUpdatedDelegate(order_OrderUpdatedEvent);
//        //    }

//        //    if (OrderAddedEvent != null)
//        //    {
//        //        OrderAddedEvent(this, order);
//        //    }
//        //}

//        //protected void UnRegisterOrder(ActiveOrder order)
//        //{
//        //    lock (this)
//        //    {
//        //        SystemMonitor.CheckThrow(_orders.Contains(order), "ActiveOrder not present.");
//        //        _orders.Remove(order);
//        //        order.OrdersUpdatedEvent -= new ActiveOrder.OrderUpdatedDelegate(order_OrderUpdatedEvent);
//        //    }

//        //    if (OrderRemovedEvent != null)
//        //    {
//        //        OrderRemovedEvent(this, order);
//        //    }
//        //}

//        //void order_OrderUpdatedEvent(ActiveOrder order, ActiveOrder.UpdateTypeEnum updateType)
//        //{
//        //    if (OrdersUpdatedEvent != null)
//        //    {
//        //        OrdersUpdatedEvent(this, order, updateType);
//        //    }
//        //}

//        //void DataProvider_ValuesUpdateEvent(IQuoteProvider dataProvider, DataBarUpdateType updateType, int count, int stepsRemaining)
//        //{

//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="order"></param>
//        ///// <param name="orderType"></param>
//        ///// <param name="closeVolume"></param>
//        ///// <param name="allowedSlippage">Pass NaN to disregard.</param>
//        ///// <param name="desiredPrice">Pass NaN to disregard.</param>
//        ///// <param name="takeProfit"></param>
//        ///// <param name="stopLoss"></param>
//        ///// <param name="comment"></param>
//        ///// <param name="openingPrice"></param>
//        ///// <param name="sourceOpenTime"></param>
//        ///// <param name="operationResultMessage"></param>
//        ///// <returns></returns>
//        //public bool ExecuteMarketOrderMessage(ActiveOrder order, OrderTypeEnum orderType, decimal closeVolume, decimal allowedSlippage, decimal desiredPrice, decimal takeProfit, decimal stopLoss,
//        //    string comment, out decimal openingPrice, out DateTime sourceOpenTime, out OrderInformation.OrderStateEnum resultState, out string operationResultMessage)
//        //{
//        //    openingPrice = decimal.MinValue;
//        //    sourceOpenTime = DateTime.MinValue;
//        //    resultState = OrderInformation.OrderStateEnum.Unknown;

//        //    if (_sessionDataProvider == null || _sessionDataProvider.Quote == null)
//        //    {
//        //        operationResultMessage = "Data provider not assigned or initialized properly.";
//        //        SystemMonitor.OperationError(operationResultMessage);
//        //        return false;
//        //    }

//        //    decimal currentPrice = _sessionDataProvider.Quote.Ask;
//        //    if (ActiveOrder.TypeIsBuy(orderType) == false)
//        //    {
//        //        currentPrice = _sessionDataProvider.Quote.Bid;
//        //    }

//        //    if (_sessionDataProvider.OperationalState != OperationalStateEnum.Operational ||
//        //        GeneralHelper.IsInvalid(currentPrice))
//        //    {
//        //        operationResultMessage = "Data provider not operational or not providing valid dataDelivery.";
//        //        return false;
//        //    }

//        //    if (GeneralHelper.IsValid(desiredPrice) && GeneralHelper.IsValid(allowedSlippage)
//        //        && allowedSlippage > 0 && Math.Abs(currentPrice - desiredPrice) > allowedSlippage)
//        //    {//
//        //        operationResultMessage = "Slippage criteria not met.";
//        //        return false;
//        //    }

//        //    RegisterOrder(order);
//        //    operationResultMessage = string.Empty;

//        //    sourceOpenTime = _sessionDataProvider.Quote.SourceTime;

//        //    if (orderType == OrderTypeEnum.BUY_MARKET
//        //        || orderType == OrderTypeEnum.SELL_MARKET)
//        //    {// Immediate order.
//        //        resultState = OrderInformation.OrderStateEnum.Executed;
//        //        openingPrice = currentPrice;
//        //    }
//        //    else
//        //    {// Delayed pending order.
//        //        resultState = OrderInformation.OrderStateEnum.Submitted;
//        //        openingPrice = desiredPrice;
//        //    }

//        //    //if (takeProfit != 0 || stopLoss != 0)
//        //    //{
//        //    //    SystemMonitor.NotImplementedWarning();
//        //    //}
//        //    return true;
//        //}

//        //public bool DecreaseOrderVolume(ActiveOrder order, decimal volumeDecreasal, decimal allowedSlippage, decimal desiredPrice,
//        //    out decimal decreasalPrice, out string operationResultMessage)
//        //{
//        //    SystemMonitor.CheckError(_orders.Contains(order), "ActiveOrder not registered in provider.");

//        //    decreasalPrice = decimal.MinValue;

//        //    ISessionDataProvider dataProvider = _sessionDataProvider;

//        //    if (dataProvider == null || dataProvider.Quote == null)
//        //    {
//        //        operationResultMessage = "Data provider not assigned or initialized properly.";
//        //        SystemMonitor.OperationError(operationResultMessage);
//        //        return false;
//        //    }

//        //    decimal currentPrice = dataProvider.Quote.Bid;
//        //    if (order.IsBuy == false)
//        //    {// Sell order.
//        //        currentPrice = dataProvider.Quote.Ask;
//        //    }

//        //    if (GeneralHelper.IsValid(desiredPrice) && GeneralHelper.IsValid(allowedSlippage)
//        //        && allowedSlippage > 0 && Math.Abs(currentPrice - desiredPrice) > allowedSlippage)
//        //    {//
//        //        operationResultMessage = "Slippage criteria not met.";
//        //        return false;
//        //    }

//        //    if (volumeDecreasal == order.CurrentVolume)
//        //    {// Closing order.
//        //        DateTime closingDateTime;
//        //        CloseOrder(order, allowedSlippage, desiredPrice, out decreasalPrice, out closingDateTime, out operationResultMessage);
//        //        return true;
//        //    }

//        //    operationResultMessage = string.Empty;
//        //    decreasalPrice = currentPrice;

//        //    return true;
//        //}


//        //private bool CheckTakeProfitInsideBar(ActiveOrder order)
//        //{
//        //    ISessionDataProvider dataProvider = _sessionDataProvider;

//        //    if (dataProvider == null || dataProvider.DataBarHistory == null)
//        //    {
//        //        SystemMonitor.OperationError("Data provider not assigned.");
//        //        return false;
//        //    }

//        //    if (dataProvider.DataBarHistory.CurrentDataBar.HasValue)
//        //    {//Check if we have reached the takeprofit
//        //        if ((order.IsBuy && order.TakeProfit != 0 &&
//        //            dataProvider.DataBarHistory.CurrentDataBar.Value.High > order.TakeProfit) || 
//        //            (!order.IsBuy && order.TakeProfit != 0 &&
//        //            dataProvider.DataBarHistory.CurrentDataBar.Value.Low < order.TakeProfit))
//        //        {//TakeProfit has been triggered inside bar
//        //            return true;
//        //        }
//        //        else return false;
//        //    }
//        //    else return false;
//        //}

//        //private bool CheckForConflictsInsideBar(ActiveOrder order)
//        //{
//        //    ISessionDataProvider dataProvider = _sessionDataProvider;

//        //    if (dataProvider == null || dataProvider.DataBarHistory == null)
//        //    {
//        //        SystemMonitor.OperationError("Data provider not assigned.");
//        //        return false;
//        //    }

//        //    if (dataProvider.DataBarHistory.CurrentDataBar.HasValue)
//        //    {//Check if we have a conflict : SL > lastbar.Low and TP < lastbar.High
//        //        if ((order.IsBuy &&
//        //            order.TakeProfit != 0 &&
//        //            order.StopLoss != 0 &&
//        //            order.StopLoss > dataProvider.DataBarHistory.CurrentDataBar.Value.Low &&
//        //            order.TakeProfit < dataProvider.DataBarHistory.CurrentDataBar.Value.High) ||
//        //            (!order.IsBuy &&
//        //            order.TakeProfit != 0 &&
//        //            order.StopLoss != 0 &&
//        //            order.StopLoss < dataProvider.DataBarHistory.CurrentDataBar.Value.High &&
//        //            order.TakeProfit > dataProvider.DataBarHistory.CurrentDataBar.Value.Low))
//        //        {//We have a collision here
//        //            return true;
//        //        }
//        //        else
//        //        {
//        //            return false;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        return false;
//        //    }
//        //}

//        //public bool CloseOrder(ActiveOrder order, decimal allowedSlippage, decimal desiredPrice,
//        //    out decimal closingPrice, out DateTime closingTime, out string operationResultMessage)
//        //{
//        //    return true;
//        //}

