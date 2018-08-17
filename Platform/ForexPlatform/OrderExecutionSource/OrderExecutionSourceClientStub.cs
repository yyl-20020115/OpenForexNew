using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using Arbiter;
using CommonSupport;
using System.Runtime.Serialization;

namespace ForexPlatform
{
    /// <summary>
    /// Class automates the responce communication with an order execution source, and
    /// allows to consume the operation of the source by simple implementing the Implementation interface.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Order Execution Client Stub")]
    public class OrderExecutionSourceClientStub : OperationalTransportClient, IOrderSink
    {
        /// <summary>
        /// 
        /// </summary>
        TransportInfo SourceTransportInfo
        {
            get { return base.RemoteStatusSynchronizationSource; }
        }

        //GetExecutionSourceParametersResponseMessage _sourceInfoMessage = null;

        public bool SupportsActiveOrderManagement
        {
            get
            {
              //NOTICE: fixed, cast to GetExecutionSourceParametersResponseMessage blocks
              return true;

                //if (_sourceInfoMessage == null)
                //{
                //    ResponseMessage responseMessage =
                //        this.SendAndReceiveResponding<ResponseMessage>(SourceTransportInfo, new GetExecutionSourceParametersMessage());

                //    if (responseMessage == null || responseMessage.OperationResult == false)
                //    {
                //        SystemMonitor.OperationError("Client stub failed to receive a proper response from source stub.", TracerItem.PriorityEnum.Medium);
                //        return false;
                //    }

                //    lock (this)
                //    {
                      
                //      _sourceInfoMessage = (GetExecutionSourceParametersResponseMessage)o;
                //    }
                //}

                //lock (this)
                //{
                //    if (_sourceInfoMessage != null)
                //    {
                //        return _sourceInfoMessage.SupportActiveOrderManagement;
                //    }
                //    else
                //    {
                //        SystemMonitor.OperationWarning("Source info not yet available to establish supported order type.", TracerItem.PriorityEnum.Medium);
                //        return false;
                //    }
                //}
            }

        }

        //volatile protected bool _isBusy = false;

        ComponentId _sourceId;
        public ComponentId SourceId
        {
            get { return _sourceId; }
        }

        #region IOrderSink Members

        decimal _slippageMultiplicator = 1;
        public decimal SlippageMultiplicator
        {
            get { return _slippageMultiplicator; }
            set { _slippageMultiplicator = value; }
        }

        #endregion

        [field:NonSerialized]
        public event OrdersUpdateDelegate OrdersUpdatedEvent;

        [field: NonSerialized]
        public event AccountInfoUpdateDelegate AccountInfoUpdateEvent;

        [field: NonSerialized]
        public event PositionUpdateDelegate PositionsUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public OrderExecutionSourceClientStub()
            : base("OrderExecutionSourceClientStub", false)
        {
            this.DefaultTimeOut = TimeSpan.FromSeconds(18);
            ChangeOperationalState(CommonSupport.OperationalStateEnum.Constructed);
            StatusSynchronizationEnabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public OrderExecutionSourceClientStub(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ChangeOperationalState(OperationalStateEnum.Constructed);
            StatusSynchronizationEnabled = true;
            _sourceId = (ComponentId)info.GetValue("sourceId", typeof(ComponentId));
            try
            {
                _slippageMultiplicator = info.GetDecimal("slippageMultiplicator");
            }
            catch (SerializationException)
            {
                _slippageMultiplicator = 1;
            }
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("sourceId", _sourceId);
            info.AddValue("slippageMultiplicator", _slippageMultiplicator);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SetInitialParameters(ComponentId sourceId, TransportInfo sourceTransportInfo)
        {
            if (SourceTransportInfo != null)
            {
                SystemMonitor.OperationError("Already initialized.");
                return false;
            }

          //NOTICE:
            //this.ChangeOperationalState(OperationalStateEnum.Operational);

            SystemMonitor.CheckWarning(sourceId == sourceTransportInfo.OriginalSenderId.Value.Id, "Possible source mis match.");

            _sourceId = sourceId;

            if (base.SetRemoteStatusSynchronizationSource(sourceTransportInfo) == false)
            {
                return false;
            }

            return true;
        }

        ///// <summary>
        ///// Obtain orders Ids from the server.
        ///// </summary>
        //public bool GetAllOrdersIds(AccountInfo accountInfo, out string[] activeOrdersIds, out string[] inactiveOrdersIds,
        //    out string operationResultMessage)
        //{
        //    TracerHelper.Trace(this.Name);

        //    activeOrdersIds = new string[] { };
        //    inactiveOrdersIds = new string[] { };

        //    if (OperationalState != OperationalStateEnum.Operational)
        //    {
        //        operationResultMessage = "Attempted operations on non operational order executioner.";
        //        SystemMonitor.Error(operationResultMessage);
        //        return false;
        //    }

        //    ResponceMessage responceMessage =
        //        this.SendAndReceiveResponding<ResponceMessage>(SourceTransportInfo, new GetAllOrdersIDsMessage(accountInfo));

        //    if (responceMessage == null)
        //    {
        //        operationResultMessage = "Timeout";
        //        return false;
        //    }

        //    if (responceMessage.OperationResult == false || responceMessage is GetOrdersIDsResponceMessage == false)
        //    {
        //        operationResultMessage = responceMessage.OperationResultMessage;
        //        return false;
        //    }

        //    GetOrdersIDsResponceMessage castedResponceMessage = (GetOrdersIDsResponceMessage)responceMessage;

        //    activeOrdersIds = new string[castedResponceMessage.OpenTickets.Length];
        //    for (int i = 0; i < castedResponceMessage.OpenTickets.Length; i++)
        //    {
        //        activeOrdersIds[i] = castedResponceMessage.OpenTickets[i].ToString();
        //    }

        //    inactiveOrdersIds = new string[castedResponceMessage.HistoricalTickets.Length];
        //    for (int i = 0; i < castedResponceMessage.HistoricalTickets.Length; i++)
        //    {
        //        inactiveOrdersIds[i] = castedResponceMessage.HistoricalTickets[i].ToString();
        //    }

        //    operationResultMessage = "Orders obtained properly.";
        //    return true;
        //}

        /// <summary>
        /// Helper.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="updateType"></param>
        void RaiseOrderUpdateEvent(AccountInfo info, string previousOrderId, OrderInfo order, ActiveOrder.UpdateTypeEnum updateType)
        {
            if (OrdersUpdatedEvent != null)
            {
                OrdersUpdatedEvent(this, info, new string[] { previousOrderId }, new OrderInfo[] { order }, new ActiveOrder.UpdateTypeEnum[] { updateType });
            }
        }

        void RaisePositionsUpdateEvent(AccountInfo info, PositionInfo[] positionsInfo)
        {
            if (PositionsUpdateEvent != null)
            {
                PositionsUpdateEvent(this, info, positionsInfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool BeginOrdersInformationUpdate(AccountInfo accountInfo, string[] orderIds,
            out string operationResultMessage)
        {
            if (OperationalState != OperationalStateEnum.Operational)
            {
                operationResultMessage = "Attempted operations on non operational order executioner.";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            if (orderIds == null || orderIds.Length == 0)
            {
                operationResultMessage = "No informations required, none retrieved.";
                return true;
            }

            GetOrdersInformationMessage message = new GetOrdersInformationMessage(accountInfo, orderIds);
            message.RequestResponse = false;
            message.PerformSynchronous = false;

            this.SendResponding(SourceTransportInfo, message);

            //if (responceMessage == null)
            //{
            //    operationResultMessage = "Getting order information failed due to time out.";
            //    return false;
            //}
            //else
            //    if (responceMessage.OperationResult == false)
            //    {
            //        operationResultMessage = responceMessage.OperationResultMessage;
            //        return false;
            //    }

            operationResultMessage = string.Empty;
            return true;
        }


        /// <summary>
        /// Helper.
        /// </summary>
        public OrderInfo? GetOrderInformation(AccountInfo accountInfo, string orderId, out string operationResultMessage)
        {
            OrderInfo[] resultInformations;
            bool result = GetOrdersInformation(accountInfo, new string[] { orderId }, out resultInformations, out operationResultMessage);
            if (resultInformations != null && resultInformations.Length > 0)
            {
                return resultInformations[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Obtain infos for these orders.
        /// </summary>
        /// <param name="orderIds"></param>
        /// <param name="informations"></param>
        /// <param name="operationResultMessage"></param>
        /// <returns></returns>
        public bool GetOrdersInformation(AccountInfo accountInfo, string[] orderIds, out OrderInfo[] informations, out string operationResultMessage)
        {
            //TracerHelper.Trace(this.Name);

            throw new NotImplementedException();

            //informations = new OrderInfo[] { };
            //if (OperationalState != OperationalStateEnum.Operational)
            //{
            //    operationResultMessage = "Attempted operations on non operational order executioner.";
            //    SystemMonitor.Error(operationResultMessage);
            //    return false;
            //}

            //if (orderIds == null || orderIds.Length == 0)
            //{
            //    operationResultMessage = "No informations required, none retrieved.";
            //    return true;
            //}

            //GetOrdersInformationMessage message = new GetOrdersInformationMessage(accountInfo, orderIds);
            //message.RequestResponce = false;
            //message.PerformSynchronous = false;

            //OrdersInformationUpdateResponceMessage responceMessage = this.SendAndReceiveResponding<OrdersInformationUpdateResponceMessage>
            //    (SourceTransportInfo, message);

            //if (responceMessage == null)
            //{
            //    operationResultMessage = "Getting order information failed due to time out.";
            //    return false;
            //}
            //else
            //    if (responceMessage.OperationResult == false)
            //    {
            //        operationResultMessage = responceMessage.OperationResultMessage;
            //        return false;
            //    }

            //operationResultMessage = "Order information retrieved.";
            //informations = responceMessage.OrderInformations;
            //return true;
        }

        protected void RaiseAccountInfoUpdateEvent(AccountInfo info)
        {
            if (AccountInfoUpdateEvent != null)
            {
                AccountInfoUpdateEvent(this, info);
            }
        }

        #region Arbiter Messages

        [MessageReceiver]
        void Receive(AccountInformationUpdateMessage message)
        {
            RaiseAccountInfoUpdateEvent(message.AccountInfo);
        }

        [MessageReceiver]
        void Receive(OrdersInformationUpdateResponseMessage message)
        {
            if (OrdersUpdatedEvent != null)
            {
                OrdersUpdatedEvent(this, message.AccountInfo, new string[] { }, message.OrderInformations, message.OrdersUpdates);
            }
        }

        [MessageReceiver]
        void Receive(PositionsInformationUpdateResponseMessage message)
        {
            RaisePositionsUpdateEvent(message.AccountInfo, message.PositionsInformations);
        }

        #endregion

        /// <summary>
        /// Helper, process the slippage trough the slippage multiplicator.
        /// </summary>
        /// <param name="allowedSlippage"></param>
        /// <returns></returns>
        decimal? ProcessSlippage(decimal? allowedSlippage)
        {
            if (allowedSlippage.HasValue)
            {
                return allowedSlippage * _slippageMultiplicator;
            }
            else
            {
                return null;
            }
        }

        #region IOrderSink

        public virtual bool Initialize()
        {
            this.Name = UserFriendlyNameAttribute.GetTypeAttributeName(typeof(OrderExecutionSourceClientStub));
            return true;
        }

        public virtual void UnInitialize()
        {
            StatusSynchronizationEnabled = false;

        }

        /// <summary>
        /// 
        /// </summary>
        public string SubmitOrder(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType,
            int volume, decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss, 
            string comment, out string operationResultMessage)
        {
            TracerHelper.Trace(this.Name);

            if (OperationalState != OperationalStateEnum.Operational)
            {
                operationResultMessage = "Attempted operations on non operational order executioner.";
                SystemMonitor.Error(operationResultMessage);
                return null;
            }

            if (account.IsEmpty
                || string.IsNullOrEmpty(account.Id)
                || string.IsNullOrEmpty(account.Name))
            {
                operationResultMessage = "Account info on order execution provider not properly assigned.";
                return null;
            }

            allowedSlippage = ProcessSlippage(allowedSlippage);

            SubmitOrderMessage request = new SubmitOrderMessage(account,
                symbol, orderType, volume, desiredPrice, allowedSlippage, takeProfit, stopLoss, comment);

            request.RequestResponse = true;
            request.PerformSynchronous = true;

            ResponseMessage response = this.SendAndReceiveResponding<ResponseMessage>
                (SourceTransportInfo, request);

            if (response == null)
            {// Time out.
                operationResultMessage = "Failed receive result for order request. In this scenario inconsistency may occur!";
                SystemMonitor.Error(operationResultMessage);
                return null;
            }

            if (response.OperationResult == false)
            {
                operationResultMessage = response.OperationResultMessage;
                return null;
            }

            SubmitOrderResponseMessage responseMessage = (SubmitOrderResponseMessage)response;
            operationResultMessage = "Order submited.";

            //RaiseOrderUpdateEvent(account, order.Info, Order.UpdateTypeEnum.Submitted);

            return responseMessage.OrderId;
        }

        /// <summary>
        /// Redefine for the operationTimeOut, by providing the DefaultTimeOut.
        /// </summary>
        public bool SynchronousExecute(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType,
            int volume, decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss,
            string comment, out OrderInfo? info, out string operationResultMessage)
        {
            return SynchronousExecute(account, order, symbol, orderType,
                volume, allowedSlippage, desiredPrice, takeProfit, stopLoss,
                comment, this.DefaultTimeOut, out info, out operationResultMessage);
        }

        /// <summary>
        /// Main SynchronousExecute method.
        /// </summary>
        public bool SynchronousExecute(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType, 
            int volume, decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss, 
            string comment, TimeSpan operationTimeOut, out OrderInfo? info, out string operationResultMessage)
        {
            TracerHelper.Trace(this.Name);

            info = null;
            operationResultMessage = string.Empty;
            
            if (OperationalState != OperationalStateEnum.Operational)
            {
                operationResultMessage = "Attempted operations on non operational order executioner.";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            if (account.IsEmpty
                || string.IsNullOrEmpty(account.Id)
                || string.IsNullOrEmpty(account.Name))
            {
                operationResultMessage = "Account info on order execution provider not properly assigned.";
                return false;
            }

            allowedSlippage = ProcessSlippage(allowedSlippage);

            ExecuteMarketOrderMessage request = new ExecuteMarketOrderMessage(account,
                symbol, orderType, volume, desiredPrice, allowedSlippage, takeProfit, stopLoss, comment);

            request.PerformSynchronous = true;

            ResponseMessage response = this.SendAndReceiveResponding<ResponseMessage>
                (SourceTransportInfo, request, operationTimeOut);

            if (response == null)
            {// Time out.
                operationResultMessage = "Failed receive result for order request. In this scenario inconsistency may occur!";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            if (response.OperationResult == false)
            {
                operationResultMessage = response.OperationResultMessage;
                return false;
            }


            //if (orderType == OrderTypeEnum.BUY_MARKET
            //    || orderType == OrderTypeEnum.SELL_MARKET)
            //{// Immediate order.
            //    resultState = OrderStateEnum.Executed;
            //}
            //else
            //{// Delayed pending order.
            //    resultState = OrderStateEnum.Submitted;
            //}

            ExecuteMarketOrderResponseMessage responseMessage = (ExecuteMarketOrderResponseMessage)response;
            operationResultMessage = "Order opened.";

            info = responseMessage.Info;

            //RaiseOrderUpdateEvent(account, order.Info, ActiveOrder.UpdateTypeEnum.Submitted);
            
            return true;
        }

        public bool ModifyOrder(AccountInfo account, Order order, decimal? stopLoss, decimal? takeProfit, 
            decimal? targetOpenPrice, out string modifiedId, out string operationResultMessage)
        {
            TracerHelper.Trace(this.Name);
            modifiedId = order.Id;

            if (OperationalState != OperationalStateEnum.Operational)
            {
                operationResultMessage = "Attempted operations on non operational order executioner.";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            ModifyOrderMessage message = new ModifyOrderMessage(account, order.Symbol, order.Id, stopLoss, takeProfit, targetOpenPrice, null);
            message.PerformSynchronous = true;

            ResponseMessage responseMessage = this.SendAndReceiveResponding<ResponseMessage>(
                SourceTransportInfo, message);

            if (responseMessage == null)
            {// Time out.
                operationResultMessage = "Timeout, failed receive result for order modification request. In this scenario inconsistency may occur!";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            if (responseMessage.OperationResult == false)
            {
                operationResultMessage = responseMessage.OperationResultMessage;
                return false;
            }

            ModifyOrderResponseMessage castedResponseMessage = (ModifyOrderResponseMessage)responseMessage;
            SystemMonitor.CheckError(string.IsNullOrEmpty(castedResponseMessage.OrderModifiedId) == false, "Modified not assigned.");
            modifiedId = castedResponseMessage.OrderModifiedId;
            operationResultMessage = "Order modified.";

            RaiseOrderUpdateEvent(account, castedResponseMessage.OrderId, order.Info, ActiveOrder.UpdateTypeEnum.Modified);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DecreaseOrderVolume(AccountInfo account, Order order, decimal volumeDecreasal, decimal? allowedSlippage, 
            decimal? desiredPrice, out decimal decreasalPrice, out string modifiedId, out string operationResultMessage)
        {
            TracerHelper.Trace(this.Name);
            modifiedId = order.Id;
            decreasalPrice = decimal.MinValue;

            if (OperationalState != OperationalStateEnum.Operational)
            {
                operationResultMessage = "Attempted operations on non operational order executioner.";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            allowedSlippage = ProcessSlippage(allowedSlippage);

            CloseOrderVolumeMessage message = new CloseOrderVolumeMessage(account, order.Symbol, order.Id, 
                order.Tag, volumeDecreasal, desiredPrice, allowedSlippage);
            message.PerformSynchronous = true;

            ResponseMessage response = this.SendAndReceiveResponding<ResponseMessage>
                (SourceTransportInfo, message);

            if (response == null)
            {// Time out.
                operationResultMessage = "Failed receive result for order request. In this scenario inconsistency may occur!";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            if (response.OperationResult == false)
            {
                operationResultMessage = response.OperationResultMessage;
                return false;
            }

            CloseOrderVolumeResponseMessage responseMessage = (CloseOrderVolumeResponseMessage)response;

            operationResultMessage = "Order volume decreased.";
            decreasalPrice = responseMessage.ClosingPrice;

            // When modified, order changes its Id.
            modifiedId = responseMessage.OrderModifiedId;

            RaiseOrderUpdateEvent(account, responseMessage.OrderId, order.Info, ActiveOrder.UpdateTypeEnum.VolumeChanged);

            return true; 
        }

        public bool IncreaseOrderVolume(AccountInfo account, Order order, decimal volumeIncrease, decimal? allowedSlippage, decimal? desiredPrice, out decimal increasalPrice, out string modifiedId, out string operationResultMessage)
        {
            operationResultMessage = "Remote Order Execution Provider does not support volume increase.";
            allowedSlippage = ProcessSlippage(allowedSlippage);

            SystemMonitor.OperationError(operationResultMessage);
            increasalPrice = 0;
            modifiedId = string.Empty;
            return false;
        }

        public bool CancelPendingOrder(AccountInfo account, Order order, out string modifiedId, out string operationResultMessage)
        {
            decimal closingPrice;
            DateTime closingDateTime;
            if (DoCloseOrder(account, order, -1, 0, out closingPrice, out closingDateTime, out modifiedId, out operationResultMessage))
            {
                RaiseOrderUpdateEvent(account, string.Empty, order.Info, ActiveOrder.UpdateTypeEnum.Canceled);
                return true;
            }

            return false;
        }

        public bool CloseOrder(AccountInfo account, Order order, decimal? allowedSlippage, decimal? desiredPrice, out decimal closingPrice, out DateTime closingTime, out string modifiedId, out string operationResultMessage)
        {
            if (DoCloseOrder(account, order, allowedSlippage, desiredPrice, out closingPrice,
                out closingTime, out modifiedId, out operationResultMessage))
            {
                RaiseOrderUpdateEvent(account, string.Empty, order.Info, ActiveOrder.UpdateTypeEnum.Closed);
                return true;
            }

            return false;

        }

        public bool BeginPositionsInfoUpdate(AccountInfo account, Symbol[] symbols)
        {
            if (SourceTransportInfo == null)
            {
                SystemMonitor.OperationWarning("Failed to obtain position information - SourceTransportInfo not assigned.");
                return false;
            }

            this.SendResponding(SourceTransportInfo, new PositionsInformationMessage(account, symbols) { RequestResponse = false });

            return true;
        }

        public bool BeginAccountInfoUpdate(AccountInfo accountInfo)
        {
            TracerHelper.Trace(this.Name);

            if (SourceTransportInfo == null)
            {
                SystemMonitor.OperationWarning("Failed to obtain account information - SourceTransportInfo not assigned.");
                return false;
            }

            this.SendResponding(SourceTransportInfo, new AccountInformationMessage(accountInfo) { RequestResponse = false });

            return true;
        }

        /// <summary>
        /// Subscribe to operations on this source accounts. Needed if we are to place orders properly etc.
        /// </summary>
        /// <returns></returns>
        public bool UpdateAccountsSubscription(bool subscribe)
        {
            // Send a subscribe for this account too.
            SubscribeToSourceAccountsUpdatesMessage subscribeRequest = new SubscribeToSourceAccountsUpdatesMessage(subscribe);

            ResponseMessage subscribeResponse = this.SendAndReceiveResponding<ResponseMessage>(
                SourceTransportInfo, subscribeRequest);

            if (subscribeResponse == null || subscribeResponse.OperationResult == false)
            {
                SystemMonitor.OperationError("Failed to subscribe to account.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool GetAvailableAccountInfos(out AccountInfo[] accounts)
        {
            accounts = new AccountInfo[] { };
            if (SourceTransportInfo == null)
            {
                SystemMonitor.OperationWarning("Failed to obtain account information - SourceTransportInfo not assigned.");
                return false;
            }

            GetAvailableAccountsMessage request = new GetAvailableAccountsMessage();
            ResponseMessage response = this.SendAndReceiveResponding<ResponseMessage>(SourceTransportInfo, request);

            if (response == null || response.OperationResult == false)
            {
                return false;
            }

            accounts = ((GetAvailableAccountsResponseMessage)response).Accounts;
            return true;
        }

        #endregion

        /// <summary>
        /// Helper.
        /// </summary>
        bool DoCloseOrder(AccountInfo account, Order order, decimal? allowedSlippage, decimal? desiredPrice, out decimal closingPrice,
            out DateTime closingTime, out string modifiedId, out string operationResultMessage)
        {
            TracerHelper.Trace(this.Name);

            closingPrice = decimal.MinValue;
            closingTime = DateTime.MinValue;
            modifiedId = order.Id;

            if (OperationalState != OperationalStateEnum.Operational)
            {
                operationResultMessage = "Attempted operations on non operational order executioner.";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            allowedSlippage = ProcessSlippage(allowedSlippage);

            CloseOrderVolumeMessage message = new CloseOrderVolumeMessage(account, order.Symbol, order.Id, 
                order.Tag, desiredPrice, allowedSlippage);
            message.PerformSynchronous = true;

            ResponseMessage response = this.SendAndReceiveResponding<ResponseMessage>
                (SourceTransportInfo, message);

            if (response == null)
            {// Time out.
                operationResultMessage = "Failed receive result for order request. In this scenario inconsistency may occur!";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            if (response.OperationResult)
            {
                CloseOrderVolumeResponseMessage responseMessage = (CloseOrderVolumeResponseMessage)response;
                operationResultMessage = "Order closed.";
                closingPrice = responseMessage.ClosingPrice;
                closingTime = responseMessage.ClosingDateTime;

                SystemMonitor.CheckError(order.Id == responseMessage.OrderId.ToString(), "Order id mismatch [" + order.Id + " / " + responseMessage.OrderId + "].");

                modifiedId = responseMessage.OrderModifiedId.ToString();
                return true;
            }

            operationResultMessage = response.OperationResultMessage;
            return false;
        }

    }
}
