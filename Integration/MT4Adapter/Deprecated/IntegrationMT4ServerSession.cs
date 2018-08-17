//using System;
//using System.Collections.Generic;
//using System.Text;
//using Arbiter;
//using CommonSupport;
//using System.Threading;
//using System.Globalization;
//using ForexPlatform;
//using CommonFinancial;
//using System.Reflection;

//namespace MT4Adapter
//{
//    /// TODO : Also if session has not been called for some time, it must set a time out flag,
//    /// so that the manager can remove it from active sessions.
//    public class IntegrationMT4ServerSession : TransportClient
//    {
//        //const string SeparatorSymbol = ";";

//        ////volatile RuntimeDataSessionInformation _sessionInformation;
//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public RuntimeDataSessionInformation Information
//        //{
//        //    get { return _sessionInformation; }
//        //}

//        //const int DefaultValuesRetrieved = 500;

//        ///// <summary>
//        ///// This is the transport stack of the currently subscribed client.
//        ///// It can be timed out, or it can be unsubscribed.
//        ///// </summary>
//        //TransportInfo _subscribedTransportMessageInfo = null;
//        //public TransportInfo SubscribedTransportMessageInfo
//        //{
//        //    get { lock (this) { return _subscribedTransportMessageInfo; } }
//        //    set { lock (this) { _subscribedTransportMessageInfo = value; } }
//        //}

//        //int _totalOrderInformationsOperationID = -1;

//        //List<OperationMessage> _pendingMessages = new List<OperationMessage>();

//        //List<OrderInfo> _pendingInformations = new List<OrderInfo>();

//        //TimeSpan _period;

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public IntegrationMT4ServerSession(string expertID, string symbol, int minutes, decimal lotSize, int decimalPlaces) 
//        //    : base("MT4IntegrationSession[" + expertID + "]", false)
//        //{
//        //    lock (this)
//        //    {
//        //        _period = TimeSpan.FromMinutes(minutes);

//        //        DataSessionInfo sessionInfo = new DataSessionInfo(Guid.NewGuid(), expertID,
//        //            new Symbol("Unknown", symbol), lotSize, decimalPlaces);

//        //        _sessionInformation = new RuntimeDataSessionInformation(sessionInfo);
//        //        _sessionInformation.AvailableDataBarPeriods.Add(TimeSpan.FromMinutes(minutes));
//        //    }
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="message"></param>
//        //void SendToSubscriber(TransportMessage message)
//        //{
//        //    TracerHelper.Trace("<< " + message.GetType());

//        //    if (_subscribedTransportMessageInfo != null)
//        //    {
//        //        this.SendResponding(_subscribedTransportMessageInfo, message);
//        //    }

//        //    TracerHelper.TraceExit();
//        //}

//        ///// <summary>
//        ///// This is also called when we change subscriber, so do not destroy vital functioning parts here.
//        ///// </summary>
//        //public void UnInitialize()
//        //{// Send to clients.
//        //    SystemMonitor.NotImplementedWarning("Session unsubscription not sent.");
//        //    lock (this)
//        //    {// TODO: implement unsubscribtion notification.
//        //        //SendToSubscriber(new SessionsUpdatesMessage(SessionsUpdatesMessage.UpdateTypeEnum.Removed, _sessionInformation));
//        //        _subscribedTransportMessageInfo = null;
//        //    }
//        //}

//        ///// <summary>
//        ///// Helper, unused.
//        ///// </summary>
//        ///// <param name="inputMessage"></param>
//        ///// <returns></returns>
//        //protected bool VerifySubscriber(TransportMessage inputMessage)
//        //{
//        //    if (_subscribedTransportMessageInfo == null)
//        //    {
//        //        return false;
//        //    }

//        //    lock (this)
//        //    {
//        //        return _subscribedTransportMessageInfo.CheckOriginalSender(inputMessage.TransportInfo);
//        //    }
//        //}

//        #region MT4 Integration Calls
//        // Result Operation ID if > 0
//        //public int RequestAllOrders()
//        //{
//        //    //try
//        //    //{
//        //    //    lock (this)
//        //    //    {
//        //    //        for (int i = 0; i < _pendingMessages.Count; i++)
//        //    //        {
//        //    //            if (_pendingMessages[i] is GetAllOrdersMessage)
//        //    //            {
//        //    //                TracerHelper.Trace(_sessionInformation.Info.Name);

//        //    //                int result = ((GetAllOrdersMessage)_pendingMessages[i]).OperationID;
//        //    //                System.Diagnostics.Debug.Assert(result != 0);
//        //    //                _pendingMessages.RemoveAt(i);
//        //    //                return result;
//        //    //            }
//        //    //        }
//        //    //    }
//        //    //}
//        //    //catch (Exception ex)
//        //    //{// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //    //    // entire package (MT4 included) down with a bad error.
//        //    //    SystemMonitor.Error(ex.Message);
//        //    //}

//        //    return 0;
//        //}

//        ///// <summary>
//        ///// Since the MT4 uses its slippage in points, we need to convert.
//        ///// </summary>
//        ///// <param name="normalValue"></param>
//        ///// <returns></returns>
//        //protected int ConvertSlippage(decimal? normalValue)
//        //{
//        //    if (normalValue.HasValue == false)
//        //    {// -1 encoded for full value.
//        //        return -1;
//        //    }

//        //    int result = 0;
//        //    result = (int)((double)normalValue * Math.Pow(10, _sessionInformation.Info.DecimalPlaces));
//        //    return result;
//        //}

//        //// << Result format ; double& volume, double& desiredPrice (0 for none), int& slippage, double& takeProfit, double& stopLoss, int& orderType, int& operationID, string& comment
//        //public string RequestNewOrder()
//        //{
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            for (int i = 0; i < _pendingMessages.Count; i++)
//        //            {
//        //                if (_pendingMessages[i] is OpenOrderMessage)
//        //                {
//        //                    TracerHelper.Trace(_sessionInformation.Info.Name);

//        //                    OpenOrderMessage message = ((OpenOrderMessage)_pendingMessages[i]);
//        //                    _pendingMessages.RemoveAt(i);

//        //                    message.Comment = message.Comment.Replace(SeparatorSymbol, ",");

//        //                    // Convert slippage to points.
//        //                    int slippage = ConvertSlippage(message.Slippage);

//        //                    if (message.DesiredPrice.HasValue == false)
//        //                    {
//        //                        message.DesiredPrice = 0;
//        //                    }

//        //                    if (message.TakeProfit.HasValue == false)
//        //                    {
//        //                        message.TakeProfit = 0;
//        //                    }

//        //                    if (message.StopLoss.HasValue == false)
//        //                    {
//        //                        message.StopLoss = 0;
//        //                    }

//        //                    return message.Volume.ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol +
//        //                        message.DesiredPrice.Value.ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol +
//        //                        slippage.ToString() + SeparatorSymbol +
//        //                        message.TakeProfit.Value.ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol +
//        //                        message.StopLoss.Value.ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol +
//        //                        ((int)message.OrderType).ToString() + SeparatorSymbol +
//        //                        message.OperationID + SeparatorSymbol
//        //                        + message.Comment;
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }


//        //    return "";
//        //}

//        //// << Result format : int& orderTicket, int& operationID, double& volume, double price, int slippage
//        //public string RequestCloseOrder()
//        //{
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            for (int i = 0; i < _pendingMessages.Count; i++)
//        //            {
//        //                if (_pendingMessages[i] is CloseOrderVolumeMessage)
//        //                {
//        //                    TracerHelper.Trace(_sessionInformation.Info.Name);

//        //                    CloseOrderVolumeMessage message = (CloseOrderVolumeMessage)_pendingMessages[i];
//        //                    _pendingMessages.RemoveAt(i);

//        //                    // Convert slippage to points.
//        //                    int slippage = ConvertSlippage(message.Slippage);

//        //                    if (message.Price.HasValue == false)
//        //                    {
//        //                        message.Price = 0;
//        //                    }

//        //                    string result = message.OrderId.ToString() + SeparatorSymbol
//        //                        + message.OperationID.ToString() + SeparatorSymbol
//        //                        + message.VolumeDecreasal.ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol
//        //                        + message.Price.Value.ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol
//        //                        + slippage.ToString();

//        //                    TracerHelper.Trace(result);

//        //                    return result;
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }


//        //    return "";
//        //}

//        ///// <summary>
//        ///// Helper.
//        ///// </summary>
//        //protected decimal TranslateModificationValue(decimal? doubleValue)
//        //{
//        //    decimal result = 0;
//        //    if (doubleValue.HasValue)
//        //    {
//        //        if (doubleValue.Value == decimal.MaxValue || doubleValue.Value == decimal.MinValue)
//        //        {// Nan means, set to not assigned.
//        //            result = -1;
//        //        }
//        //        else
//        //        {// Normal value assignment.
//        //            result = doubleValue.Value;
//        //        }
//        //    }

//        //    return result;
//        //}

//        ///// <summary>
//        ///// Helper.
//        ///// </summary>
//        //protected long TranslationModificationValue(long? intValue)
//        //{
//        //    long result = 0;
//        //    if (intValue.HasValue)
//        //    {
//        //        if (long.MinValue == intValue.Value)
//        //        {// Nan means, set to not assigned.
//        //            result = -1;
//        //        }
//        //        else
//        //        {// Normal value assignment.
//        //            result = intValue.Value;
//        //        }
//        //    }

//        //    return result;
//        //}

//        //// for the double (and int) values, 0 means do not change, -1 means set to "not assigned"
//        //// << Result format : int& orderTicket, int& operationID, double& stopLoss, double& takeProfit, double& targetOpenPrice, int& expiration
//        //public string RequestModifyOrder()
//        //{
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            for (int i = 0; i < _pendingMessages.Count; i++)
//        //            {
//        //                if (_pendingMessages[i] is ModifyOrderMessage)
//        //                {
//        //                    TracerHelper.Trace(_sessionInformation.Info.Name);

//        //                    ModifyOrderMessage message = (ModifyOrderMessage)_pendingMessages[i];
//        //                    _pendingMessages.RemoveAt(i);


//        //                    string result = message.OrderId.ToString() + SeparatorSymbol
//        //                        + message.OperationID.ToString() + SeparatorSymbol
//        //                        + TranslateModificationValue(message.StopLoss).ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol
//        //                        + TranslateModificationValue(message.TakeProfit).ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol
//        //                        + TranslateModificationValue(message.TargetOpenPrice).ToString(GeneralHelper.NumberFormatInfo) + SeparatorSymbol
//        //                        + TranslateModificationValue(message.Expiration).ToString();

//        //                    TracerHelper.Trace(result);
//        //                    return result;
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //    return "";
//        //}

//        ///// <summary>
//        ///// Helper.
//        ///// </summary>
//        //MessageType GetPendingMessage<MessageType>()
//        //    where MessageType : OperationMessage
//        //{
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            for (int i = 0; i < _pendingMessages.Count; i++)
//        //            {
//        //                //TracerHelper.Trace(_pendingMessages[i].GetType().ToString());
//        //                Type t = typeof(MessageType);
//        //                if (t == _pendingMessages[i].GetType())
//        //                {
//        //                    return (MessageType)_pendingMessages[i];
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //    return null;
//        //}

//        //// MULTI CALL FOR A SINGLE OPERATION.
//        //// Most complex operations, since it is multi call, before returning result to user.
//        //// << Result format : int& orderTicket
//        //public string RequestOrderInformation()
//        //{
//        //    OrdersInformationMessage message = GetPendingMessage<OrdersInformationMessage>();
//        //    if (message == null)
//        //    {
//        //        return "";
//        //    }

//        //    try
//        //    {

//        //        lock (this)
//        //        {
//        //            TracerHelper.Trace(_sessionInformation.Info.Name + ", " + message.OrderTickets.Count.ToString());

//        //            if (_pendingInformations.Count == 0 && message.OrderTickets.Count == 0)
//        //            {// We just received a new empty message - just call order information with empty parameters, to signify a return.
//        //                OrderInfo(-1, message.OperationID, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, true, "No orders retrieved.");
//        //                return "";
//        //            }

//        //            if (message.OrderTickets.Count == 0)
//        //            {// Break call. Operation placing finished, notify caller with an empty result and remove operation from list.
//        //                _pendingMessages.Remove(message);
//        //                return "";
//        //            }
//        //            else
//        //            {
//        //                string ticket = message.OrderTickets[0];
//        //                message.OrderTickets.RemoveAt(0);
//        //                return ticket + SeparatorSymbol + message.OperationID.ToString();
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //    return "";
//        //}

//        //// << Result format : operationID if > 0
//        //public int RequestCloseAllOrders()
//        //{
//        //    //try
//        //    //{
//        //    //    lock (this)
//        //    //    {
//        //    //        for (int i = 0; i < _pendingMessages.Count; i++)
//        //    //        {
//        //    //            if (_pendingMessages[i] is CloseAllOrdersMessage)
//        //    //            {
//        //    //                TracerHelper.Trace(_sessionInformation.Info.Name);

//        //    //                int result = ((CloseAllOrdersMessage)_pendingMessages[i]).OperationID;
//        //    //                _pendingMessages.RemoveAt(i);

//        //    //                return result;
//        //    //            }
//        //    //        }
//        //    //    }
//        //    //}
//        //    //catch (Exception ex)
//        //    //{// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //    //    // entire package (MT4 included) down with a bad error.
//        //    //    SystemMonitor.Error(ex.Message);
//        //    //}

//        //    return 0;
//        //}

//        //// << Result format : operationID if > 0; preffered count
//        //public string RequestValues()
//        //{
//        //    try
//        //    {
//        //        RequestDataHistoryMessage dataHistoryMessage = GetPendingMessage<RequestDataHistoryMessage>();
//        //        if (dataHistoryMessage != null)
//        //        {
//        //            TracerHelper.Trace(_sessionInformation.Info.Name);
//        //            lock (this)
//        //            {
//        //                int operationId = dataHistoryMessage.OperationID;
//        //                int prefferedValueCount = DefaultValuesRetrieved;
//        //                if (dataHistoryMessage.Request.MaxValuesRetrieved.HasValue)
//        //                {
//        //                    prefferedValueCount = ((RequestDataHistoryMessage)_pendingMessages[i]).Request.MaxValuesRetrieved.Value;
//        //                }

//        //                _pendingMessages.Remove(dataHistoryMessage);
//        //                return operationId.ToString() + SeparatorSymbol + prefferedValueCount.ToString();
//        //            }
//        //        }

//        //        RequestQuoteUpdateMessage quoteUpdateMessage = GetPendingMessage<RequestQuoteUpdateMessage>();
//        //        if (quoteUpdateMessage != null)
//        //        {
//        //            TracerHelper.Trace(_sessionInformation.Info.Name);
//        //            lock (this)
//        //            {
//        //                int operationId = quoteUpdateMessage.OperationID;
//        //                int prefferedValueCount = 0;

//        //                _pendingMessages.Remove(quoteUpdateMessage);
//        //                return operationId.ToString() + SeparatorSymbol + prefferedValueCount.ToString();
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //    return string.Empty;
//        //}

//        //// << Result format : operationID if > 0
//        //public int RequestAccountInformation()
//        //{
//        //    try
//        //    {
//        //        AccountInformationMessage message = GetPendingMessage<AccountInformationMessage>();
//        //        if (message != null)

//        //        lock (this)
//        //        {
//        //            TracerHelper.Trace(_sessionInformation.Info.Name);

//        //            int operationId = message.OperationID;
//        //            _pendingMessages.Remove(message);

//        //            return operationId;
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //    return 0;
//        //}

//        //// >>
//        //public void OrderOpened(int operationID, int orderTicket, decimal openingPrice, int orderOpenTime, bool operationResult, string operationResultMessage)
//        //{
//        //    TracerHelper.Trace(_sessionInformation.Info.Name);
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            OpenOrderResponceMessage message = new OpenOrderResponceMessage( _sessionInformation.Info, operationID, orderTicket.ToString(),
//        //                openingPrice, orderOpenTime, operationResult);

//        //            message.ExceptionMessage = operationResultMessage;
//        //            SendToSubscriber(message);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }
//        //}

//        //// >>
//        //public void OrderClosed(int operationID, int orderTicket, int orderNewTicket, decimal closingPrice, int orderCloseTime, bool operationResult, string operationResultMessage)
//        //{
//        //    TracerHelper.Trace(_sessionInformation.Info.Name);

//        //    try
//        //    {

//        //        string orderNewTicketString = orderNewTicket.ToString();
//        //        if (orderNewTicket < 1)
//        //        {// Anything below 1 (0, -1 etc) is considered empty.
//        //            orderNewTicketString = string.Empty;
//        //        }

//        //        lock (this)
//        //        {
//        //            CloseOrderVolumeResponceMessage message =
//        //                new CloseOrderVolumeResponceMessage(_sessionInformation.Info, operationID, orderTicket.ToString(), orderNewTicketString,
//        //                    closingPrice, orderCloseTime, operationResult);
//        //            message.ExceptionMessage = operationResultMessage;
//        //            SendToSubscriber(message);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //}

//        //// >>
//        //public void OrderModified(int operationID, int orderTicket, int orderNewTicket, bool operationResult, string operationResultMessage)
//        //{
//        //    TracerHelper.Trace(_sessionInformation.Info.Name);
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            ModifyOrderResponceMessage message =
//        //                new ModifyOrderResponceMessage(_sessionInformation.Info, operationID, orderTicket.ToString(), orderNewTicket.ToString(), operationResult);
//        //            message.ExceptionMessage = operationResultMessage;
//        //            SendToSubscriber(message);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }
//        //}

//        //// >>
//        //public void AllOrders(int operationID,
//        //    int[] openCustomIDs, int[] openTickets,
//        //    int[] historicalCustomIDs, int[] historicalTickets, 
//        //    bool operationResult)
//        //{
//        //    TracerHelper.Trace(_sessionInformation.Info.Name);
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            GetAllOrdersResponceMessage message = new GetAllOrdersResponceMessage(_sessionInformation.Info, operationID,
//        //                openCustomIDs, openTickets,
//        //                historicalCustomIDs, historicalTickets, operationResult);
//        //            SendToSubscriber(message);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }
//        //}

//        //// >>
//        //public void AllOrdersClosed(int operationID, int closedOrdersCount, bool operationResult, string operationResultMessage)
//        //{
//        //    TracerHelper.Trace(_sessionInformation.Info.Name);
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            CloseAllOrdersResponceMessage message = new CloseAllOrdersResponceMessage(_sessionInformation.Info, operationID, closedOrdersCount, operationResult);
//        //            message.ExceptionMessage = operationResultMessage;
//        //            SendToSubscriber(message);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //}

//        //// >>
//        //public void ErrorOccured(int operationResult, string errorMessage)
//        //{
//        //    TracerHelper.Trace(_sessionInformation.Info.Name);
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            SessionErrorOccuredMessage message = new SessionErrorOccuredMessage(_sessionInformation.Info, operationResult, errorMessage);
//        //            SendToSubscriber(message);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //}

//        ///// <summary>
//        ///// Helper.
//        ///// </summary>
//        //protected decimal? Convert(decimal valueType)
//        //{
//        //    if (valueType == 0)
//        //    {
//        //        return null;
//        //    }
//        //    return valueType;
//        //}
        
//        ///// <summary>
//        ///// Helper.
//        ///// </summary>
//        //protected int? Convert(int valueType)
//        //{
//        //    if (valueType == 0)
//        //    {
//        //        return null;
//        //    }
//        //    return valueType;
//        //}

       
//        //// >>
//        //public void OrderInfo(int orderTicket, int operationID, string orderSymbol, int orderType, decimal volume,
//        //                                 decimal inputOpenPrice, decimal inputClosePrice, decimal inputOrderStopLoss, decimal inputOrderTakeProfit,
//        //                                 decimal currentProfit, decimal orderSwap, int inputOrderPlatformOpenTime,
//        //                                 int inputOrderPlatformCloseTime, int inputOrderExpiration, decimal orderCommission,
//        //                                 string orderComment, int orderCustomID, bool operationResult, string operationResultMessage)
//        //{
//        //    TracerHelper.TraceEntry();

//        //    try
//        //    {

//        //        decimal? openPrice = Convert(inputOpenPrice);
//        //        decimal? closePrice = Convert(inputClosePrice);
//        //        decimal? orderStopLoss = Convert(inputOrderStopLoss);
//        //        decimal? orderTakeProfit = Convert(inputOrderTakeProfit);
//        //        int? orderPlatformOpenTime = Convert(inputOrderPlatformOpenTime);
//        //        int? orderPlatformCloseTime = Convert(inputOrderPlatformCloseTime);
//        //        int? orderExpiration = Convert(inputOrderExpiration);

//        //        // Perform data fixes to convert to proper cases.
//        //        bool isOpen = orderPlatformCloseTime.HasValue == false || orderPlatformCloseTime == 0;

//        //        OrderInfo.StateEnum orderState = CommonFinancial.OrderInfo.StateEnum.Unknown;
//        //        // According to documentataion this is the way to establish if order is closed, see here : http://docs.mql4.com/trading/OrderSelect
//        //        if (isOpen)
//        //        {
//        //            if (CommonFinancial.OrderInfo.TypeIsDelayed((OrderTypeEnum)orderType) == false
//        //               && orderPlatformOpenTime > 0)
//        //            {
//        //                orderState = CommonFinancial.OrderInfo.StateEnum.Opened;
//        //            }
//        //            else
//        //            {
//        //                orderState = CommonFinancial.OrderInfo.StateEnum.PlacedPending;
//        //            }
//        //        }
//        //        else
//        //        {
//        //            orderState = CommonFinancial.OrderInfo.StateEnum.Closed;
//        //        }

//        //        if (orderState == CommonFinancial.OrderInfo.StateEnum.Opened)
//        //        {// Since the integration might report close price for opened orders, at the current closing price.
//        //            closePrice = null;
//        //        }

//        //        lock (this)
//        //        {
//        //            if (orderTicket > -1)
//        //            {// Store call information for later use.
//        //                TracerHelper.TraceEntry("Storing information.");

//        //                if (_totalOrderInformationsOperationID != -1 && operationID != _totalOrderInformationsOperationID)
//        //                {
//        //                    SystemMonitor.Error("Order information procedure was mixed, operationID not expected.");
//        //                }
//        //                else
//        //                {
//        //                    _totalOrderInformationsOperationID = operationID;
//        //                }

//        //                OrderInfo orderInformation = new OrderInfo(
//        //                    orderTicket.ToString(), orderSymbol, orderType, orderState,
//        //                    volume, openPrice, closePrice,
//        //                    orderStopLoss, orderTakeProfit, currentProfit,
//        //                    orderSwap, orderPlatformOpenTime, orderPlatformCloseTime,
//        //                    orderExpiration, orderCommission, orderComment, orderCustomID.ToString());

//        //                _pendingInformations.Add(orderInformation);
//        //            }
//        //            else
//        //            {// This is the flush call - send all stored to user.

//        //                TracerHelper.TraceEntry("Sending information.");

//        //                OrdersInformationResponceMessage message = new OrdersInformationResponceMessage(_sessionInformation.Info, operationID, _pendingInformations.ToArray(), operationResult);

//        //                message.ExceptionMessage = operationResultMessage;
//        //                SendToSubscriber(message);

//        //                // Clear for new operations.
//        //                _totalOrderInformationsOperationID = -1;
//        //                _pendingInformations.Clear();
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //}

//        //// >>
//        //public void AccountInformation(int operationID, decimal accountBalance, decimal accountCredit, 
//        //    string accountCompany, string accountCurrency,
//        //    decimal accountEquity, decimal accountFreeMargin, decimal accountLeverage,
//        //    decimal accountMargin, string accountName, int accountNumber,
//        //    decimal accountProfit, string accountServer, bool operationResult, string operationResultMessage)
//        //{
//        //    TracerHelper.TraceEntry();
//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            AccountInformationUpdateMessage message = new
//        //                AccountInformationUpdateMessage(_sessionInformation.Info, operationID, operationResult,
//        //                new AccountInfo(Math.Round(accountBalance, 4), Math.Round(accountCredit, 4), accountCompany,
//        //                    new Symbol(accountCurrency), Math.Round(accountEquity, 4), Math.Round(accountFreeMargin, 4),
//        //                    Math.Round(accountLeverage, 4), Math.Round(accountMargin, 4), accountName,
//        //                    accountNumber.ToString(), accountProfit, accountServer));

//        //            message.ExceptionMessage = operationResultMessage;
//        //            SendToSubscriber(message);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        SystemMonitor.Error(ex.Message);
//        //    }
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public void Quotes(int operationId, double ask, double bid, double open, double close, 
//        //    double low, double high, double volume, Int64 time)
//        //{
//        //    TracerHelper.TraceEntry();
            
//        //    try
//        //    {
//        //        TransportMessage message;
//        //        lock (this)
//        //        {
//        //            message = new QuoteUpdateMessage(_sessionInformation.Info, operationId,
//        //                new Quote((decimal)ask, (decimal)bid, (decimal)open, (decimal)close, (decimal)high, (decimal)low, (decimal)volume, 
//        //                    GeneralHelper.GenerateDateTimeSecondsFrom1970(time)), true);
//        //        }

//        //        SendToSubscriber(message);
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //}

//        ///// <summary>
//        ///// OperationId is not mandatory - but is should be there when the update was requested by a special recepient.
//        ///// </summary>
//        //public void TradingValuesUpdate(int operationId, Int64 time, int availableBarsCount,
//        //    Int64[] times, decimal[] opens, decimal[] closes, decimal[] highs, decimal[] lows, decimal[] volumes)
//        //{
//        //    TracerHelper.TraceEntry();
//        //    try
//        //    {
//        //        DataHistoryUpdateMessage message;
//        //        lock (this)
//        //        {// History update.
//        //            message = new DataHistoryUpdateMessage(_sessionInformation.Info, operationId,
//        //                _period, times, opens, closes, highs, lows, volumes);
//        //            message.AvailableHistorySize = availableBarsCount;
//        //        }

//        //        SendToSubscriber(message);
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //}

//        #endregion

//        #region Arbiter Messages

//        //[MessageReceiver]
//        //public SessionOperationResponceMessage Receive(SessionOperationMessage message)
//        //{
//        //    TracerHelper.TraceEntry();

//        //    try
//        //    {
//        //        lock (this)
//        //        {
//        //            TracerHelper.Trace(message.GetType().Name);
//        //            _pendingMessages.Add(message);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
//        //        // entire package (MT4 included) down with a bad error.
//        //        SystemMonitor.Error(ex.Message);
//        //    }

//        //    return new SessionOperationResponceMessage(message.DataSessionInfo, message.OperationID, true);
//        //}

//        #endregion

//    }
//}
