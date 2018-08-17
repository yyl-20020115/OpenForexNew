using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Arbiter.Transport;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Base class for Transport extended Arbiter clients. The transport extensions include very many
    /// additional features over the existing Arbiter mechanisms, so make sure to use this wherever possible.
    /// </summary>
    [Serializable]
    public abstract class TransportClient : ArbiterClientBase
    {
        /// <summary>
        /// The invocation mechanism used is FastInvoke, as described in the FastInvokeHelper.
        /// This speeds up the dynamic invocation many times (50+).
        /// </summary>
        Dictionary<Type, FastInvokeHelper.FastInvokeHandlerDelegate> _methodsAndMessageTypes = new Dictionary<Type, FastInvokeHelper.FastInvokeHandlerDelegate>();

        /// <summary>
        /// The transport sessions are tracked here.
        /// </summary>
        Dictionary<Guid, SessionResults> _communicationSessions = new Dictionary<Guid, SessionResults>();

        /// <summary>
        /// What time out is for messages, when not specified in sending function.
        /// Zero stands for no timeout.
        /// </summary>
        TimeSpan _defaultTimeOut = TimeSpan.Zero;
        public TimeSpan DefaultTimeOut
        {
            get { return _defaultTimeOut; }
            set { _defaultTimeOut = value; }
        }

        [Browsable(false)]
        public TransportMessageFilter Filter
        {
            get
            {
                return (TransportMessageFilter)(_messageFilter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TransportClient(bool singleThreadMode)
            : base(singleThreadMode)
        {
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public TransportClient(string idName, bool singleThreadMode)
            : base(idName, singleThreadMode)
        {
            Initialize();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="singleThreadOnly"></param>
        public TransportClient(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _defaultTimeOut = (TimeSpan)info.GetValue("defaultTimeOut", typeof(TimeSpan));
            Initialize();
        }

        public override bool ArbiterUnInitialize()
        {
            lock (_communicationSessions)
            {
                foreach (SessionResults results in _communicationSessions.Values)
                {// Clear all pending sessions with a null responce.
                    results.ReceiveResponse(null);
                }
            }

            return base.ArbiterUnInitialize();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("defaultTimeOut", _defaultTimeOut);
        }

        /// <summary>
        /// 
        /// </summary>
        void Initialize()
        {
            if (_messageFilter == null || _messageFilter is TransportMessageFilter == false)
            {
                _messageFilter = new TransportMessageFilter(this.SubscriptionClientID);
            }

            Type type = this.GetType();
            while(type != typeof(TransportClient))
            {// Gather current type members, but also gather parents private types, since those will not be available to the child class and will be missed.
                // Also not that the dictionary key mechanism throws if same baseMethod is entered twise - so it is a added safety feature.
                MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (MethodInfo methodInfo in infos)
                {
                    object[] customAttributes = methodInfo.GetCustomAttributes(false);

                    if (type != this.GetType() && methodInfo.IsPrivate == false)
                    {// Since this is one of the members of the parent classes, make sure to just gather privates.
                        // because all of the parent's protected and public methods are available from the child class.
                        continue;
                    }
   
                    if (customAttributes.Length == 0 || customAttributes[0] is MessageReceiverAttribute == false)
                    {
                        continue;
                    }

                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsSubclassOf(typeof(Message)) &&
                        (methodInfo.ReturnType == typeof(void) || methodInfo.ReturnType.IsSubclassOf(typeof(Message)) == true))
                    {
                        _methodsAndMessageTypes.Add(parameters[0].ParameterType, FastInvokeHelper.GetMethodInvoker(methodInfo, true));
                    }
                    else
                    {// Warn that a baseMethod marked with the Attribute was found but will not be assigned.
                        SystemMonitor.Error("A method [" + methodInfo.DeclaringType + "::" + methodInfo.Name + "] marked with MessageReceiverAttribute does not match parameter and return type criteria.");
                    }
                }

                type = type.BaseType;
            }
        }

        public override void ReceiveExecution(ExecutionEntity entity)
        {
            SystemMonitor.CheckError(entity.Message.GetType().IsSubclassOf(typeof(TransportMessage)));
            OnMessageReceived((TransportMessage)entity.Message);
        }

        public override void ReceiveExecutionWithReply(ExecutionEntityWithReply entity)
        {
            SystemMonitor.CheckError(entity.Message.GetType().IsSubclassOf(typeof(TransportMessage)));
            OnMessageReceived((TransportMessage)entity.Message);
        }

        public override void ReceiveConversationTimedOut(Conversation conversation)
        {
            SystemMonitor.NotExpectedCritical();
        }

        #region Direct Call Functions

        //protected Message DirectCall(ArbiterClientId receiverId, Message message)
        //{
        //    // Preliminary verification.
        //    if (Arbiter == null || this.SubscriptionClientID.IsEmpty)
        //    {
        //        SystemMonitor.OperationWarning("Using a client [" + this.GetType().Name + " with no Arbiter assigned or SubscriptionClientID empty.");
        //        return null;
        //    }

        //    return Arbiter.DirectCall(this.SubscriptionClientID, receiverId, message);
        //}

        #endregion

        #region Send Functions

        /// <summary>
        /// Make a direct send operation, no timeout.
        /// </summary>
        /// <param name="requestMessage"></param>
        protected void Send(TransportMessage message)
        {
            DoSendCustom(true, Guid.NewGuid(), null, 0, null, this.SubscriptionClientID, message, DefaultTimeOut);
        }

        /// <summary>
        /// Perform a synchronous send and receive operation. 
        /// If the operation times out and exception will be thrown!
        /// </summary>
        protected TExpectedMessageClass SendAndReceive<TExpectedMessageClass>(TransportMessage message, TimeSpan timeOut)
            where TExpectedMessageClass : TransportMessage
        {
            return SendAndReceiveAddressed<TExpectedMessageClass>(null, message, timeOut);
        }

        /// <summary>
        /// 
        /// </summary>
        protected TExpectedMessageClass SendAndReceive<TExpectedMessageClass>(TransportMessage message)
            where TExpectedMessageClass : TransportMessage
        {
            return SendAndReceiveAddressed<TExpectedMessageClass>(null, message, DefaultTimeOut);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SendAddressed(ArbiterClientId receiverID, TransportMessage message)
        {
            DoSendCustom(true, Guid.NewGuid(), null, 0, receiverID, this.SubscriptionClientID, message, DefaultTimeOut);
        }

        /// <summary>
        /// 
        /// </summary>
        protected TExpectedMessageClass SendAndReceiveAddressed<TExpectedMessageClass>(
            ArbiterClientId? receiverID, TransportMessage message, TimeSpan timeOut)
            where TExpectedMessageClass : TransportMessage
        {
            TransportMessage[] results =
                DoSendCustom(true, Guid.NewGuid(), typeof(TExpectedMessageClass), 1, receiverID, this.SubscriptionClientID, message, timeOut);

            if (results != null && results.Length > 0)
            {
                return (TExpectedMessageClass)results[0];
            }
            else
            {
                return null;
            }
        }

        protected TExpectedMessageClass SendAndReceiveAddressed<TExpectedMessageClass>(
            ArbiterClientId? receiverID, TransportMessage message)
            where TExpectedMessageClass : TransportMessage
        {
            TransportMessage[] results =
                DoSendCustom(true, Guid.NewGuid(), typeof(TExpectedMessageClass), 1, receiverID, this.SubscriptionClientID, message, DefaultTimeOut);

            if (results != null && results.Length > 0)
            {
                return (TExpectedMessageClass)results[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected void SendForwarding(IEnumerable<ArbiterClientId?> messagePath, TransportMessage message)
        {
            DoSendAndReceiveForwarding(messagePath, message, DefaultTimeOut, null);
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected void SendForwardingToMany(IEnumerable<IEnumerable<ArbiterClientId?>> messagePaths, TransportMessage message)
        {
            foreach (IEnumerable<ArbiterClientId?> messagePath in messagePaths)
            {
                DoSendAndReceiveForwarding(messagePath, message, DefaultTimeOut, null);
            }
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected TExpectedMessageClass SendAndReceiveForwarding<TExpectedMessageClass>(IEnumerable<ArbiterClientId?> messagePath,
            TransportMessage message) where TExpectedMessageClass : TransportMessage
        {
            return (TExpectedMessageClass)DoSendAndReceiveForwarding(messagePath, message, DefaultTimeOut, typeof(TExpectedMessageClass));
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected TExpectedMessageClass SendAndReceiveForwarding<TExpectedMessageClass>(IEnumerable<ArbiterClientId?> messagePath,
            TransportMessage message, TimeSpan timeOut) where TExpectedMessageClass : TransportMessage
        {
            return (TExpectedMessageClass)DoSendAndReceiveForwarding(messagePath, message, timeOut, typeof(TExpectedMessageClass));
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected void SendResponding(TransportInfo inputMessageTransportInfo, 
            TransportMessage message)
        {
            List<ArbiterClientId?> forwardingInfo = inputMessageTransportInfo.CreateRespondingClientList();
            DoSendAndReceiveForwarding(forwardingInfo, message, DefaultTimeOut, null);
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected void SendRespondingToMany(IEnumerable<TransportInfo> inputMessageTransportInfos,
            TransportMessage message)
        {
            foreach (TransportInfo info in inputMessageTransportInfos)
            {
                List<ArbiterClientId?> forwardingInfo = info.CreateRespondingClientList();
                DoSendAndReceiveForwarding(forwardingInfo, message, DefaultTimeOut, null);
            }
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected TExpectedMessageClass SendAndReceiveResponding<TExpectedMessageClass>(
            TransportInfo incomingMessageTransportInfo, TransportMessage message)
                where TExpectedMessageClass : TransportMessage
        {
            return SendAndReceiveResponding<TExpectedMessageClass>(incomingMessageTransportInfo, message, DefaultTimeOut);
        }

        /// <summary>
        /// The "Responding" set of functions allows to easily send a new request to a location 
        /// that has sent you a requestMessage, even if this location is remote (trough server-client nodes).
        /// </summary>
        protected TExpectedMessageClass SendAndReceiveResponding<TExpectedMessageClass>(TransportInfo inputMessageTransportInfo, 
            TransportMessage message, TimeSpan timeOut)
                where TExpectedMessageClass : TransportMessage
        {
            List<ArbiterClientId?> forwardingInfo = inputMessageTransportInfo.CreateRespondingClientList();
            return (TExpectedMessageClass)DoSendAndReceiveForwarding(forwardingInfo, message, timeOut, typeof(TExpectedMessageClass));
        }

        #endregion

        /// <summary>
        /// Helper, for children classes to send messages further.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ResponseMessage ProxyRequestMessage(RequestMessage message)
        {
            if (message.RequestResponse)
            {
                return this.SendAndReceive<ResponseMessage>(message);
            }

            return null;
        }


        /// <summary>
        /// Advanced sending.
        /// </summary>
        private TransportMessage DoSendAndReceiveForwarding(IEnumerable<ArbiterClientId?> messagePath,
            TransportMessage message, TimeSpan timeOut, Type responseType)
        {
            message.TransportInfo.SetForwardTransportInfo(messagePath);

            //ArbiterClientId? immediateReceiver = this.SubscriptionClientID;
            //if (requestMessage.TransportInfo.CurrentForwardTransportInfoAddress.Value.Guid == this.SubscriptionClientID.Guid)
            //{
            //    requestMessage.TransportInfo.PopForwardTransportInfo();
            //}

            TransportMessage[] results =
                DoSendCustom(true, Guid.NewGuid(), responseType, 1, message.TransportInfo.PopForwardTransportInfo(), this.SubscriptionClientID, message, timeOut);

            if (results != null && results.Length > 0)
            {
                return results[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Allows to send a message to receiver, by "hiding" the current node, this way all responce messages
        /// are targeted to the original sender (incl. responce of this message if it is request). 
        /// This allows to implement a message proxy.
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="message"></param>
        protected void ProxySend(ArbiterClientId receiverId, TransportMessage message)
        {
            TransportInfoUnit? info = message.TransportInfo.PopTransportInfo();
            if (info == null)
            {
                SystemMonitor.Error("Failed to establish message pending transport info.");
                return;
            }

            DoSendCustom(message.IsRequest, info.Value.Id, null, 0, receiverId, info.Value.SenderID, message, DefaultTimeOut);
        }

        /// <summary>
        /// Use in combination with ProxySend to implement a full proxy element.
        /// This is good for responces, that are coming from a receiver and contain
        /// information of where to go next in their Forwarding info.
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="message"></param>
        protected void ProxyForwardingSend(TransportMessage message)
        {
            ArbiterClientId? forwardTransport = message.TransportInfo.PopForwardTransportInfo();
            if (forwardTransport == null || forwardTransport.Value.IsEmpty)
            {
                SystemMonitor.Error("Failed to establish forwarded transport info.");
                return;
            }

            DoSendCustom(message.IsRequest, Guid.NewGuid(), null, 0, forwardTransport.Value, SubscriptionClientID, message, DefaultTimeOut);
        }

        /// <summary>
        /// Central sending function.
        /// </summary>
        /// <param name="receiverID">The ID of the receiver module. Can be <b>null</b> and this sends to all in the Arbiter.</param>
        protected TransportMessage[] DoSendCustom(bool isRequest, Guid sessionGuid, 
            Type expectedResponseMessageClassType, int responsesRequired, 
            ArbiterClientId? receiverId, ArbiterClientId? senderId, TransportMessage message, TimeSpan timeOut)
        {
            //TracerHelper.TraceEntry();

            SessionResults session = null;

            if (receiverId.HasValue && receiverId.Value.IsEmpty /*receiverId.Value.CompareTo(ArbiterClientId.Empty) == 0*/)
            {
                SystemMonitor.Error("Can not send an item to empty receiver. Use null to specify broadcast.");
                return null;
            }

            // Preliminary verification.
            if (Arbiter == null)
            {
                SystemMonitor.OperationWarning("Using a client [" + this.GetType().Name + ":" + senderId.Value.ClientName + " to " + (receiverId.HasValue ? receiverId.Value.Id.Name : string.Empty) + " , " + message.GetType().Name + "] with no Arbiter assigned.");
                return null;
            }

            message.IsRequest = isRequest;

            TransportInfoUnit infoUnit = new TransportInfoUnit(sessionGuid, senderId, receiverId);
            message.TransportInfo.AddTransportInfoUnit(infoUnit);

            bool sessionEventResult = false;

            if (expectedResponseMessageClassType != null)
            {// Responce waiting session.
                session = new SessionResults(responsesRequired, expectedResponseMessageClassType);
                lock (_communicationSessions)
                {// Register the session.
                    _communicationSessions.Add(sessionGuid, session);
                }
            }

            SystemMonitor.CheckError(message.TransportInfo.CurrentTransportInfo != null);
            Conversation conversation;
            if (receiverId == null)
            {
                // We shall not use the next level time out mechanism.
                conversation = Arbiter.CreateConversation(senderId.Value, message, TimeSpan.Zero);
            }
            else
            {// Addressed conversation.
                // We shall not use the next level time out mechanism.
                conversation = Arbiter.CreateConversation(senderId.Value, receiverId.Value, message, TimeSpan.Zero);
            }

            if (conversation != null && expectedResponseMessageClassType != null)
            {// Responce waiting session (only if conversation was properly created).

                if (timeOut == TimeSpan.Zero)
                {// Wait forever.
                    sessionEventResult = session.SessionEndEvent.WaitOne();
                }
                else
                {// Wait given period.
                    sessionEventResult = session.SessionEndEvent.WaitOne(timeOut, false);
                }

                lock (_communicationSessions)
                {// Remote the session.
                    _communicationSessions.Remove(sessionGuid);
                }
            }

            message.TransportInfo.PopTransportInfo();

            if (expectedResponseMessageClassType == null)
            {// No responce waiting, just return.
                //TracerHelper.TraceExit();
                return null;
            }

            // Responce waiting session.
            if (sessionEventResult == false)
            {// Timed out - only send and receives can time out, as the other ones do not have sessions!!
                TracerHelper.TraceError("Session has timed out [" + message.GetType().Name + "].");
                return null;
            }

            //TracerHelper.TraceExit();
            return session.Responses.ToArray();
        }

        /// <summary>
        /// This will includes thread safety, so no need for the child to implement thread safety code.
        /// </summary>
        protected virtual void OnMessageReceived(TransportMessage message)
        {
            if (message.IsRequest)
            {// Request.
                HandleRequestMessage(message);
            }
            else
            {// Responce.
                HandleResponseMessage(message);
            }
        }

        /// <summary>
        /// Helper class, to establish the delegate that handles this message.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        protected FastInvokeHelper.FastInvokeHandlerDelegate GetMessageHandler(Message message)
        {
            FastInvokeHelper.FastInvokeHandlerDelegate resultHandler = null;

            Type messageType = message.GetType();
            lock (_methodsAndMessageTypes)
            {
                Type currentType = messageType;
                while (currentType != typeof(Message) && currentType != typeof(object))
                {// Look for a handler of this requestMessage type or for any of the parent types.
                    if (_methodsAndMessageTypes.ContainsKey(currentType))
                    {
                        resultHandler = _methodsAndMessageTypes[currentType];
                        break;
                    }

                    currentType = currentType.BaseType; // FIX, Asen: it used to be messageType.BaseType.
                }
            }

            return resultHandler;
        }

        /// <summary>
        /// This is an entry point for the direct call mechanism. So optimization is on speed and not functionality.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override Message ReceiveDirectCall(Message message)
        {
            FastInvokeHelper.FastInvokeHandlerDelegate handler = GetMessageHandler(message);
            if (handler != null)
            {
                return (Message)handler(this, new object[] { message });
            }
            else
            {// We failed to find a handler for this type or for any of the parent ones.
                TracerHelper.TraceError("Client instance did not handle a request message received, client [" + this.GetType().Name + "], message [" + message.GetType().Name + "].");
                return null;
            }

            //Message responceMessage = (Message)invokeMethodInfo.Invoke(this, new object[] { message });
            //return responceMessage;
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual void HandleResponseMessage(TransportMessage message)
        {
            SessionResults respondedSession = null;

            // First - check if this is a responce to a currently pending session.
            lock (_communicationSessions)
            {
                string requestMessageInfo = string.Empty;
                if (message is ResponseMessage)
                {
                    requestMessageInfo = ((ResponseMessage)message).RequestMessageTypeName;
                }

                if (_communicationSessions.ContainsKey(message.TransportInfo.CurrentTransportInfo.Value.Id) == false)
                {
                    TracerHelper.TraceError("Response received to a session that is not pending [from " + message.TransportInfo.OriginalSenderId.Value.Id.Print() + " to " + this.SubscriptionClientID.Id.Print() + "]. Message [" + message.GetType().Name + " responding to " + requestMessageInfo + "] dumped.");
                    return;
                }

                respondedSession = _communicationSessions[message.TransportInfo.CurrentTransportInfo.Value.Id];
            }

            // Clear off the sessioning Transport Info that just brough it back as well.
            message.TransportInfo.PopTransportInfo();

            respondedSession.ReceiveResponse(message);
        }


        /// <summary>
        /// Handle a request message.
        /// </summary>
        protected virtual void HandleRequestMessage(TransportMessage message)
        {
            FastInvokeHelper.FastInvokeHandlerDelegate handler = GetMessageHandler(message);

            if (handler == null)
            {// We failed to find a handler for this type or for any of the parent ones.
                TracerHelper.TraceError("Client instance did not handle a request message received, client [" + this.GetType().Name + "], message [" + message.GetType().Name + "].");
                return;
            }

            // Before giving the requestMessage to the user class, make a copy of its transport info
            // so that if the user messes it up, we still can deliver the responce properly.
            TransportInfo requestMessageTransportInfo = message.TransportInfo.Clone();
            
            TransportMessage responseMessage = (TransportMessage)handler(this, new object[] { message });
            if (responseMessage == null)
            {// No result.
                return;
            }

            if (responseMessage is ResponseMessage)
            {
                ((ResponseMessage)responseMessage).RequestMessageTypeName = message.GetType().Name;
            }

            Guid sessionGuid = requestMessageTransportInfo.CurrentTransportInfo.Value.Id;
            ArbiterClientId? senderID = requestMessageTransportInfo.CurrentTransportInfo.Value.SenderID;

            requestMessageTransportInfo.PopTransportInfo();
            
            // Transfer inherited underlaying transport stack.
            responseMessage.TransportInfo = requestMessageTransportInfo;

            // Send the responce requestMessage back.
            DoSendCustom(false, sessionGuid, null, 0, senderID, this.SubscriptionClientID, responseMessage, TimeSpan.Zero);
        }

    }
}
