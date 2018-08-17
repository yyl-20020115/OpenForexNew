using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using ForexPlatform;
using CommonFinancial;
using Arbiter;
using System.Threading;

namespace MT4Adapter
{
    /// <summary>
    /// 5 layer of the adapter handles - handles order execution requests.
    /// </summary>
    public class MT4RemoteAdapterOrders : MT4RemoteAdapterData
    {
        AccountInfo _accountInfo = new AccountInfo(Guid.NewGuid(), 0, 0, string.Empty, Symbol.Empty, 0, 0, 0, 0, string.Empty, string.Empty, 0, string.Empty);

        Dictionary<ArbiterClientId, TransportInfo> _subscribers = new Dictionary<ArbiterClientId,TransportInfo>();

        Dictionary<string, OrderInfo?> _orders = new Dictionary<string, OrderInfo?>();

        /// <summary>
        /// Here are stored all ids of orders that we need to get information about.
        /// </summary>
        List<string> _pendingOrdersInformations = new List<string>();

        /// <summary>
        /// Each (X) seconds, the information of all open/pending orders will be updated 
        /// and sent to user.
        /// </summary>
        TimeSpan? _activeOrdersUpdateInterval = TimeSpan.FromSeconds(60);

        /// <summary>
        /// The last time active orders were updated.
        /// </summary>
        DateTime _lastActiveOrdersUpdate = DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        public MT4RemoteAdapterOrders(Uri uri)
            : base(uri)
        {
            if (_activeOrdersUpdateInterval.Value.TotalSeconds < 10)
            {
                _activeOrdersUpdateInterval = TimeSpan.FromSeconds(10);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected void SendToSubscribers(TransportMessage message)
        {
            TracerHelper.TraceEntry(message.GetType().Name + " to " + _subscribers.Count + " subscribers");
            lock (this)
            {
                foreach (KeyValuePair<ArbiterClientId, TransportInfo> pair in _subscribers)
                {
                    SendResponding(pair.Value, message);
                }
            }
        }

        // << Result format ; string& symbol, double& minVolume, double& desiredPrice (0 for none), int& slippage, double& takeProfit, double& stopLoss, int& orderType, int& operationID, string& comment
        public string RequestNewOrder()
        {
            int operationId;
            try
            {
                OrderMessage message = base.GetPendingMessage<OrderMessage>(true, out operationId);

                if (message == null)
                {
                    return string.Empty;
                }

                // Process comment.
                message.Comment = message.Comment.Replace(SeparatorSymbol, ",");

                // Convert slippage to points.
                int slippage = ConvertSlippage(message.Symbol, message.Slippage);

                // Process desired price.
                if (message.DesiredPrice.HasValue == false)
                {
                    message.DesiredPrice = 0;
                }

                // Process TP.
                if (message.TakeProfit.HasValue == false)
                {
                    message.TakeProfit = 0;
                }

                // Process SL.
                if (message.StopLoss.HasValue == false)
                {
                    message.StopLoss = 0;
                }

                // Process minVolume.
                CombinedDataSubscriptionInformation session = base.GetDataSession(message.Symbol);
                if (session == null)
                {
                    SystemMonitor.Error("Failed to establish symbol [" + message.Symbol.Name + "] session.");
                    if (message.PerformSynchronous == false)
                    {
                        base.CompleteOperation(operationId, new ResponseMessage(false, "Failed to establish symbol session."));
                    }
                    return string.Empty;
                }

                decimal volume = ConvertVolume(session.SessionInformation.Info.LotSize, message.Volume);

                string result = 
                    message.Symbol.Name + SeparatorSymbol +
                    volume.ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol +
                    message.DesiredPrice.Value.ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol +
                    slippage.ToString() + SeparatorSymbol +
                    message.TakeProfit.Value.ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol +
                    message.StopLoss.Value.ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol +
                    ((int)message.OrderType).ToString() + SeparatorSymbol +
                    operationId + SeparatorSymbol + 
                    message.Comment;

                TracerHelper.Trace(result);

                return result;
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }


            return string.Empty;
        }

        // << Result format : int& orderTicket, int& operationID, double& minVolume, double& price, int& slippage
        public string RequestCloseOrder()
        {
            try
            {
                int operationId;
                CloseOrderVolumeMessage message = base.GetPendingMessage<CloseOrderVolumeMessage>(true, out operationId);

                if (message == null)
                {
                    return string.Empty;
                }

                // Convert slippage to points.
                int slippage = ConvertSlippage(message.Symbol, message.Slippage);

                // Process price.
                if (message.Price.HasValue == false)
                {
                    message.Price = 0;
                }

                // Process minVolume.
                CombinedDataSubscriptionInformation session = base.GetDataSession(message.Symbol);
                if (session == null)
                {
                    SystemMonitor.Error("Failed to establish symbol [" + message.Symbol.Name + "] session.");
                    if (message.PerformSynchronous)
                    {
                        base.CompleteOperation(operationId, new ResponseMessage(false, "Failed to establish symbol session."));
                    }
                    return string.Empty;
                }

                decimal volumeDecreasal = ConvertVolume(session.SessionInformation.Info.LotSize, (int)message.VolumeDecreasal);
                
                string result = 
                    message.OrderId.ToString() + SeparatorSymbol +
                    operationId.ToString() + SeparatorSymbol +
                    volumeDecreasal.ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol + 
                    message.Price.Value.ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol + 
                    slippage.ToString();

                TracerHelper.Trace(result);

                return result;
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }


            return string.Empty;
        }

        // for the double (and int) values, 0 means do not change, -1 means set to "not assigned"
        // << Result format : int& orderTicket, int& operationID, double& stopLoss, double& takeProfit, double& targetOpenPrice, int& expiration
        public string RequestModifyOrder()
        {
            try
            {
                int operationId;
                ModifyOrderMessage message = base.GetPendingMessage<ModifyOrderMessage>(true, out operationId);

                if (message == null)
                {
                    return string.Empty;
                }

                string result =
                    message.Symbol.Name.ToString() + SeparatorSymbol +
                    message.OrderId.ToString() + SeparatorSymbol +
                    operationId.ToString() + SeparatorSymbol + 
                    TranslateModificationValue(message.StopLoss).ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol + 
                    TranslateModificationValue(message.TakeProfit).ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol + 
                    TranslateModificationValue(message.TargetOpenPrice).ToString(GeneralHelper.UniversalNumberFormatInfo) + SeparatorSymbol + 
                    TranslateModificationValue(message.Expiration).ToString();

                TracerHelper.Trace(result);

                return result;
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

            return string.Empty;
        }

        // The result of this is a MULTI CALL FOR A SINGLE OPERATION, but this works normally.
        // Most complex operations, since it is multi call, before returning result to user.
        // << Result format : int& orderTicket, int operationId (currently always -1).
        public string RequestOrderInformation()
        {
            try
            {
                lock (this)
                {
                    if (_activeOrdersUpdateInterval.HasValue
                        && (DateTime.Now - _lastActiveOrdersUpdate) >= _activeOrdersUpdateInterval.Value)
                    {// Perform the periodical order update operation.
                        _lastActiveOrdersUpdate = DateTime.Now;

                        foreach (KeyValuePair<string, OrderInfo?> pair in _orders)
                        {
                            if (pair.Value.HasValue 
                                && (pair.Value.Value.State == OrderStateEnum.Executed
                                || pair.Value.Value.State == OrderStateEnum.Submitted))
                            {
                                _pendingOrdersInformations.Add(pair.Key);
                            }
                        }
                    }

                    if (_pendingOrdersInformations.Count > 0)
                    {
                        string id = _pendingOrdersInformations[0];
                        _pendingOrdersInformations.RemoveAt(0);
                        
                        return id + SeparatorSymbol + "-1";
                    }
                }
            }
            catch (Exception ex)
            {
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

            return string.Empty;
        }

        // << Result format : operationID if > 0
        public int RequestAccountInformation()
        {
            try
            {
                int operationId;
                AccountInformationMessage message = GetPendingMessage<AccountInformationMessage>(true, out operationId);

                if (message != null)
                {
                    TracerHelper.Trace(message.AccountInfo.Name);
                    return operationId;
                }
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

            return 0;
        }

        // >>
        public void OrderOpened(string symbol, int operationID, int orderTicket, decimal openingPrice, 
            int orderOpenTime, bool operationResult, string operationResultMessage)
        {
            TracerHelper.Trace(symbol);

            try
            {
                OperationInformation operation = base.GetOperationById(operationID);

                DateTime? time = GeneralHelper.GenerateDateTimeSecondsFrom1970(orderOpenTime);
                if (time.HasValue == false)
                {
                    if (operation != null)
                    {
                        base.CompleteOperation(operationID, new ResponseMessage(false, "Failed to convert time for order."));
                    }

                    SystemMonitor.Error("Failed to convert time for order.");
                    return;
                }

                string orderId = orderTicket.ToString();
                if (orderTicket < 0)
                {// The system needs orderId empty to recognize the result as failure.
                    orderId = string.Empty;
                    operationResult = false;
                }

                if (operation != null)
                {
                    ResponseMessage message = null;

                    lock (this)
                    {
                        if (operation.Request is SubmitOrderMessage)
                        {
                            message = new SubmitOrderResponseMessage(_accountInfo, orderId, true);
                            message.OperationResultMessage = operationResultMessage;
                            message.OperationResult = operationResult;
                        }
                        else if (operation.Request is ExecuteMarketOrderMessage)
                        {
                            OrderInfo info = new OrderInfo(orderId, CreateSymbol(symbol), OrderTypeEnum.UNKNOWN, OrderStateEnum.Executed, int.MinValue);
                            info.OpenTime = time;
                            info.OpenPrice = openingPrice;

                            message = new ExecuteMarketOrderResponseMessage(_accountInfo, info, operationResult);
                            message.OperationResultMessage = operationResultMessage;
                        }
                        else
                        {
                            SystemMonitor.Error("Failed to establish placed order request type.");
                            message = new ResponseMessage(false);
                        }
                    }

                    base.CompleteOperation(operationID, message);
                }

                if (string.IsNullOrEmpty(orderId) == false)
                {
                    // Do an update of this order.
                    lock (this)
                    {
                        _pendingOrdersInformations.Add(orderId);
                    }
                }
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise 
                // they bring the entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }
        }

        // >>
        public void OrderClosed(string symbol, int operationID, int orderTicket, int orderNewTicket, decimal closingPrice, int orderCloseTime, bool operationResult, string operationResultMessage)
        {
            TracerHelper.Trace(symbol);

            try
            {
                OperationInformation operation = base.GetOperationById(operationID);

                string orderNewTicketString = orderNewTicket.ToString();
                if (orderNewTicket < 1)
                {// Anything below 1 (0, -1 etc) is considered empty.
                    orderNewTicketString = string.Empty;
                }

                DateTime? closeTime = GeneralHelper.GenerateDateTimeSecondsFrom1970(orderCloseTime);

                if (closeTime.HasValue == false)
                {
                    if (operation != null)
                    {
                        base.CompleteOperation(operationID, new ResponseMessage(false, "Failed to convert order close time."));
                    }
                    SystemMonitor.Error("Failed to convert order close time.");
                    return;
                }

                if (operation != null)
                {
                    CloseOrderVolumeResponseMessage message;
                    lock (this)
                    {
                        message = new CloseOrderVolumeResponseMessage(_accountInfo, orderTicket.ToString(), orderNewTicketString, closingPrice, closeTime.Value, operationResult);
                        message.OperationResultMessage = operationResultMessage;
                    }

                    base.CompleteOperation(operationID, message);
                }
                else
                {
                    SystemMonitor.Error("Failed to finish order close operation as expected.");
                }

                // Do an update of this order.
                lock (this)
                {
                    _pendingOrdersInformations.Add(orderTicket.ToString());
                }
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }
        }

        // >>
        public void OrderModified(string symbol, int operationID, int orderTicket, int orderNewTicket, bool operationResult, string operationResultMessage)
        {
            TracerHelper.Trace(symbol);
            try
            {
                OperationInformation operation = base.GetOperationById(operationID);

                if (operation != null)
                {
                    ModifyOrderResponseMessage message;
                    lock (this)
                    {
                        message = new ModifyOrderResponseMessage(_accountInfo, orderTicket.ToString(), orderNewTicket.ToString(), operationResult);
                        message.OperationResultMessage = operationResultMessage;
                    }

                    base.CompleteOperation(operationID, message);
                }

                lock (this)
                {
                    _pendingOrdersInformations.Add(orderTicket.ToString());
                }
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }
        }

        // >>
        public void AllOrders(int operationID, string symbol, int[] openMagicIDs, int[] openTickets,
            int[] historicalMagicIDs, int[] historicalTickets, bool operationResult)
        {
            TracerHelper.Trace(symbol);

            try
            {
                lock (this)
                {
                    List<string> knownOrdersIds = GeneralHelper.EnumerableToList<string>(_orders.Keys);

                    for (int i = 0; i < openTickets.Length; i++)
                    {
                        string orderId = openTickets[i].ToString();
                        knownOrdersIds.Remove(orderId);

                        if (_orders.ContainsKey(orderId) == false
                            || _orders[orderId].HasValue == false 
                            || _orders[orderId].Value.State != OrderStateEnum.Executed)
                        {
                            _orders[orderId] = null;
                            _pendingOrdersInformations.Add(orderId);
                        }
                    }

                    for (int i = 0; i < historicalTickets.Length; i++)
                    {
                        string orderId = historicalTickets[i].ToString();
                        knownOrdersIds.Remove(orderId);

                        if (_orders.ContainsKey(orderId) == false
                            || _orders[orderId].HasValue == false
                            || _orders[orderId].Value.State != OrderStateEnum.Closed)
                        {
                            _orders[orderId] = null;
                            _pendingOrdersInformations.Add(orderId);
                        }
                    }

                    foreach (string id in knownOrdersIds)
                    {// Make sure to remove non existing ones.
                        _orders.Remove(id);
                    }
                }

            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }
        }

        static int ConvertVolume(decimal lotSize, decimal lot)
        {
            return (int)(lot * lotSize);
        }

        static decimal ConvertVolume(decimal lotSize, int units)
        {
            return (decimal)units / lotSize;
        }

        static OrderTypeEnum ConvertOrderType(int orderType)
        {
            switch(orderType)
            {
                case 0:
                    return OrderTypeEnum.BUY_MARKET;
                case 1:
                    return OrderTypeEnum.SELL_MARKET;
                case 2:
                    return OrderTypeEnum.BUY_LIMIT_MARKET;
                case 3:
                    return OrderTypeEnum.SELL_LIMIT_MARKET;
                case 4:
                    return OrderTypeEnum.BUY_STOP_MARKET;
                case 5:
                    return OrderTypeEnum.SELL_STOP_MARKET;
            }


            return OrderTypeEnum.UNKNOWN;
        }

        // >>
        public void OrderInformation(int orderTicket, int operationID, string orderSymbol, int orderType, decimal volume,
                                         decimal inputOpenPrice, decimal inputClosePrice, decimal inputOrderStopLoss, decimal inputOrderTakeProfit,
                                         decimal currentProfit, decimal orderSwap, int inputOrderPlatformOpenTime,
                                         int inputOrderPlatformCloseTime, int inputOrderExpiration, decimal orderCommission,
                                         string orderComment, int orderCustomID, bool operationResult, string operationResultMessage)
        {
            TracerHelper.TraceEntry(string.Format("ticketId[{0}], customId[{1}], operationId[{2}], symbol[{3}], SL[{4}, TP[{5}]", orderTicket.ToString(),
                orderCustomID.ToString(), operationID.ToString(), orderSymbol, inputOrderStopLoss.ToString(), inputOrderTakeProfit.ToString()));

            try
            {
                #region Preprocess Data

                decimal? openPrice = Convert(inputOpenPrice);
                decimal? closePrice = Convert(inputClosePrice);
                decimal? orderStopLoss = Convert(inputOrderStopLoss);
                decimal? orderTakeProfit = Convert(inputOrderTakeProfit);
                int? orderPlatformOpenTime = Convert(inputOrderPlatformOpenTime);
                int? orderPlatformCloseTime = Convert(inputOrderPlatformCloseTime);
                int? orderExpiration = Convert(inputOrderExpiration);

                // Perform dataDelivery fixes to convert to proper cases.
                bool isOpen = orderPlatformCloseTime.HasValue == false || orderPlatformCloseTime == 0;

                OrderStateEnum orderState = OrderStateEnum.Unknown;
                // According to documentataion this is the way to establish if order is closed, see here : http://docs.mql4.com/trading/OrderSelect
                if (isOpen)
                {
                    if (CommonFinancial.OrderInfo.TypeIsDelayed((OrderTypeEnum)orderType) == false
                       && orderPlatformOpenTime > 0)
                    {
                        orderState = OrderStateEnum.Executed;
                    }
                    else
                    {
                        orderState = OrderStateEnum.Submitted;
                    }
                }
                else
                {
                    orderState = OrderStateEnum.Closed;
                }

                if (orderState == OrderStateEnum.Executed)
                {// Since the integration might report close price for opened orders, at the current closing price.
                    closePrice = null;
                }

                DateTime? openTime = GeneralHelper.GenerateDateTimeSecondsFrom1970(orderPlatformOpenTime);
                DateTime? closeTime = GeneralHelper.GenerateDateTimeSecondsFrom1970(orderPlatformCloseTime);
                DateTime? expirationTime = GeneralHelper.GenerateDateTimeSecondsFrom1970(orderExpiration);

                CombinedDataSubscriptionInformation sessionSubscriptionInfo = GetDataSession(orderSymbol);

                if (sessionSubscriptionInfo == null)
                {
                    SystemMonitor.Error("Corresponding symbol [" + orderSymbol + "] session info not found.");
                    return;
                }

                #endregion

                bool isNewlyAcquired = false;
                OrderInfo? orderInformation = null;

                if (orderTicket > -1)
                {// Store call information for later use.
                    orderInformation = new OrderInfo(
                        orderTicket.ToString(), sessionSubscriptionInfo.SessionInformation.Info.Symbol, ConvertOrderType(orderType), orderState,
                        ConvertVolume(sessionSubscriptionInfo.SessionInformation.Info.LotSize, volume), openPrice, closePrice,
                        orderStopLoss, orderTakeProfit, currentProfit,
                        orderSwap, openTime, closeTime, openTime,
                        expirationTime, orderCommission, orderComment, orderCustomID.ToString());

                    lock (this)
                    {

                        isNewlyAcquired = _orders.ContainsKey(orderTicket.ToString()) == false || _orders[orderTicket.ToString()].HasValue == false;
                        _orders[orderTicket.ToString()] = orderInformation;
                    }

                    TracerHelper.TraceEntry(string.Format("Storing information, id[{0}], symb[{1}], customId[{2}], operationId[{3}].", orderTicket.ToString(), orderSymbol, orderCustomID.ToString(), operationID));
                }
                else
                {// This used to be flush call (send all stored to user), but currently not used.
                    SystemMonitor.NotImplementedError("Case not implemented.");
                }

                //if (isNewlyAcquired)
                {// Send a notification to subscribers, an order orderInfo was acquired.
                    OrdersInformationUpdateResponseMessage message = new OrdersInformationUpdateResponseMessage(_accountInfo,
                        new OrderInfo[] { orderInformation.Value }, true);

                    SendToSubscribers(message);
                }

            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }


        }

        // >>
        public void AccountInformation(int operationID, decimal accountBalance, decimal accountCredit,
            string accountCompany, string accountCurrency,
            decimal accountEquity, decimal accountFreeMargin, decimal accountLeverage,
            decimal accountMargin, string accountName, int accountNumber,
            decimal accountProfit, string accountServer, bool operationResult, string operationResultMessage)
        {
            TracerHelper.TraceEntry();
            try
            {
                lock (this)
                {
                    _accountInfo.Balance = Math.Round(accountBalance, 4);
                    _accountInfo.Credit = Math.Round(accountCredit, 4);
                    _accountInfo.Company = accountCompany;
                    _accountInfo.BaseCurrency = new Symbol(accountCurrency);
                    _accountInfo.Equity = Math.Round(accountEquity, 4);
                    _accountInfo.FreeMargin = Math.Round(accountFreeMargin, 4);
                    _accountInfo.Leverage = Math.Round(accountLeverage, 4);
                    _accountInfo.Margin = Math.Round(accountMargin, 4);
                    _accountInfo.Name = accountName;
                    _accountInfo.Id = accountNumber.ToString();
                    _accountInfo.Profit = Math.Round(accountProfit, 4);
                    _accountInfo.Server = accountServer;
                }

                AccountInformationUpdateMessage message = new AccountInformationUpdateMessage(_accountInfo, operationResult);
                message.OperationResultMessage = operationResultMessage;

                if (base.GetOperationById(operationID) != null)
                {
                    base.CompleteOperation(operationID, message);
                }
                else
                {// This is an operationless update.
                    SendToSubscribers(message);
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.Error(ex.Message);
            }
        }

        [MessageReceiver]
        ResponseMessage Receive(SubscribeToSourceAccountsUpdatesMessage message)
        {
            if (message.TransportInfo.OriginalSenderId.HasValue == false)
            {
                SystemMonitor.Error("Failed to establish original sender id.");
                return null;
            }

            if (message.Subscribe)
            {
                List<OrderInfo> orderInfos = new List<OrderInfo>();
                lock (this)
                {
                    _subscribers[message.TransportInfo.OriginalSenderId.Value] = message.TransportInfo;

                    // Send an update of current orderInfo to new subscriber.
                    foreach (KeyValuePair<string, OrderInfo?> pair in _orders)
                    {
                        if (pair.Value.HasValue)
                        {
                            orderInfos.Add(pair.Value.Value);
                        }
                    }
                }

                if (orderInfos.Count > 0)
                {
                    TransportInfo transportInfo = message.TransportInfo.Clone();

                    GeneralHelper.FireAndForget(delegate()
                    {// Make sure the result of the current call is returned before sending an initial update.
                        Thread.Sleep(500);

                        OrdersInformationUpdateResponseMessage updateMessage = new OrdersInformationUpdateResponseMessage(_accountInfo,
                            orderInfos.ToArray(), true);

                        this.SendResponding(transportInfo, updateMessage);
                    });
                }
            }
            else
            {
                lock (this)
                {
                    _subscribers.Remove(message.TransportInfo.OriginalSenderId.Value);
                }
            }

            if (message.RequestResponse)
            {
                return new ResponseMessage(true);
            }

            return null;
        }

        [MessageReceiver]
        OrdersInformationUpdateResponseMessage Receive(GetOrdersInformationMessage message)
        {
            TracerHelper.TraceEntry();

            List<OrderInfo> result = new List<OrderInfo>();
            lock (this)
            {
                foreach (string id in message.OrderTickets)
                {
                    if (_orders.ContainsKey(id) && _orders[id].HasValue)
                    {
                        result.Add(_orders[id].Value);
                    }
                    else
                    {
                        _pendingOrdersInformations.Add(id);
                    }
                }
            }

            TracerHelper.TraceExit();

            OrdersInformationUpdateResponseMessage response = new OrdersInformationUpdateResponseMessage(_accountInfo, result.ToArray(), true);

            if (message.RequestResponse)
            {
                return response;
            }

            SendToSubscribers(response);
            return null;
        }

        [MessageReceiver]
        GetAvailableAccountsResponseMessage Receive(GetAvailableAccountsMessage message)
        {
            if (message.RequestResponse)
            {
                return new GetAvailableAccountsResponseMessage(new AccountInfo[] { _accountInfo }, true);
            }

            SendToSubscribers(new GetAvailableAccountsResponseMessage(new AccountInfo[] { _accountInfo }, true));
            return null;
        }

        #region Arbiter Messages

        //[MessageReceiver]
        //AccountResponceMessage Receive(OrdersInformationMessage message)
        //{
        //    IImplementation implementation = _implementation;
        //    if (implementation == null || OperationalState != OperationalStateEnum.Operational)
        //    {
        //        if (message.RequestResponce)
        //        {
        //            return new AccountResponceMessage(message.AccountInfo, false);
        //        }

        //        return null;
        //    }

        //    string operationResultMessage;
        //    OrderInfo[] orderInfos;
        //    bool operationResult = implementation.GetOrdersInfos(message.AccountInfo, message.OrderTickets, out orderInfos, out operationResultMessage);

        //    OrdersInformationUpdateResponceMessage responce = new OrdersInformationUpdateResponceMessage(message.AccountInfo, 
        //        orderInfos, operationResult);
        //    responce.ResultMessage = operationResultMessage;

        //    if (message.RequestResponce)
        //    {
        //        return responce;
        //    }
        //    else
        //    {
        //        if (operationResult)
        //        {
        //            SendResponding(message.TransportInfo, responce);
        //        }
        //    }
        //    return null;
        //}


        //[MessageReceiver]
        //GetExecutionSourceParametersResponceMessage Receive(GetExecutionSourceParametersMessage message)
        //{
        //    return new GetExecutionSourceParametersResponceMessage(_supportsActiveOrderManagement);
        //}

        //[MessageReceiver]
        //AccountResponceMessage Receive(ModifyOrderMessage message)
        //{
        //    IImplementation implementation = _implementation;
        //    if (implementation == null || OperationalState != OperationalStateEnum.Operational)
        //    {
        //        return new AccountResponceMessage(message.AccountInfo, false);
        //    }

        //    string modifiedId, operationResultMessage;
        //    ModifyOrderResponceMessage responce;

        //    if (implementation.ModifyOrder(message.AccountInfo, message.OrderId, message.StopLoss, message.TakeProfit, message.TargetOpenPrice,
        //        out modifiedId, out operationResultMessage))
        //    {
        //        responce = new ModifyOrderResponceMessage(message.AccountInfo, 
        //            message.OrderId, modifiedId, true);
        //    }
        //    else
        //    {
        //        responce = new ModifyOrderResponceMessage(message.AccountInfo, 
        //            message.OrderId, modifiedId, false);
        //    }

        //    responce.ResultMessage = operationResultMessage;
        //    return responce;
        //}

        //[MessageReceiver]
        //AccountResponceMessage Receive(CloseOrderVolumeMessage message)
        //{
        //    IImplementation implementation = _implementation;
        //    if (implementation == null || OperationalState != OperationalStateEnum.Operational)
        //    {
        //        return new AccountResponceMessage(message.AccountInfo, false);
        //    }

        //    decimal closingPrice;
        //    DateTime closingTime;
        //    string modifiedId;
        //    string operationResultMessage;

        //    CloseOrderVolumeResponceMessage responce;
        //    if (implementation.CloseOrder(message.AccountInfo, message.OrderId, message.Slippage, message.Price, out closingPrice, out closingTime,
        //        out modifiedId, out operationResultMessage))
        //    {
        //        responce = new CloseOrderVolumeResponceMessage(message.AccountInfo, message.OrderId, modifiedId,
        //            closingPrice, closingTime, true);
        //    }
        //    else
        //    {
        //        responce = new CloseOrderVolumeResponceMessage(message.AccountInfo, message.OrderId, string.Empty,
        //            decimal.Zero, DateTime.MinValue, false);
        //    }

        //    responce.ResultMessage = operationResultMessage;
        //    return responce;
        //}

        //[MessageReceiver]
        //ResponceMessage Receive(SubscribeToSourceAccountsUpdatesMessage message)
        //{
        //    lock (this)
        //    {
        //        if (message.Subscribe)
        //        {
        //            _subscribers.Add(message.TransportInfo);
        //        }
        //        else
        //        {
        //            _subscribers.Remove(message.TransportInfo);
        //        }
        //    }

        //    return new ResponceMessage(true);
        //}

        //[MessageReceiver]
        //AccountResponceMessage Receive(AccountInformationMessage message)
        //{
        //    IImplementation implementation = _implementation;
        //    if (implementation != null && OperationalState == OperationalStateEnum.Operational)
        //    {
        //        AccountInfo? updatedInfo = implementation.GetAccountInfoUpdate(message.AccountInfo);
        //        if (updatedInfo.HasValue)
        //        {
        //            if (message.RequestResponce)
        //            {
        //                return new AccountResponceMessage(updatedInfo.Value, true);
        //            }
        //            else
        //            {
        //                SendResponding(message.TransportInfo, new AccountResponceMessage(updatedInfo.Value, true));
        //            }
        //        }
        //    }

        //    if (message.RequestResponce)
        //    {
        //        return new AccountResponceMessage(message.AccountInfo, false);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //[MessageReceiver]
        //GetDataSourceSymbolCompatibleResponceMessage Receive(GetDataSourceSymbolCompatibleMessage message)
        //{
        //    IImplementation implementation = _implementation;
        //    if (implementation == null || OperationalState != OperationalStateEnum.Operational)
        //    {
        //        return new GetDataSourceSymbolCompatibleResponceMessage(false);
        //    }
        //    else
        //    {
        //        int result = implementation.IsDataSourceSymbolCompatible(message.DataSourceId, message.Symbol);
        //        return new GetDataSourceSymbolCompatibleResponceMessage(true) { CompatibilityLevel = result };
        //    }
        //}

        //[MessageReceiver]
        //GetAvailableAccountsResponceMessage Receive(GetAvailableAccountsMessage message)
        //{
        //    IImplementation implementation = _implementation;
        //    if (implementation == null || OperationalState != OperationalStateEnum.Operational)
        //    {
        //        return new GetAvailableAccountsResponceMessage(new AccountInfo[] { }, false);
        //    }

        //    return new GetAvailableAccountsResponceMessage(implementation.GetAvailableAccounts(), true);
        //}

        #endregion
    }
}
