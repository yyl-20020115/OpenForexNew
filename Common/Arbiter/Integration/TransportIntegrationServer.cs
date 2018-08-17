using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using Arbiter.MessageContainerTransport;
using System.Collections.ObjectModel;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// If you want to point a specific client conneciton - use its ArbiterClientID in a requestMessage you have received from it.
    /// If you want to point with a forward transport, use the AdditionalForwardTransportSessionStack.
    /// If you want to point with a forward transport, but want to skip the use the AdditionalForwardTransportSessionStack and push a "null" for the server to pop.
    /// </summary>
    public sealed class TransportIntegrationServer : TransportIntegrationBase
    {
        MessageContainerTransportServer _transportServer;
        protected override IMessageContainerTransport Transport
        {
            get { return _transportServer; }
        }

        List<string> _clients = new List<string>();
        public ReadOnlyCollection<string> Clients
        {
            get { lock (this) { return _clients.AsReadOnly(); } }
        }

        Uri _serverUriBase;

        public delegate void ClientUpdateDelegate(string clientId);
        public event ClientUpdateDelegate ClientConnectedEvent;
        public event ClientUpdateDelegate ClientDisconnectedEvent;

        public TransportIntegrationServer(Uri serverUriBase)
            : base(serverUriBase, "ArbiterIntegration.Server", true)
        {
            Filter.Enabled = true;
            Filter.AllowOnlyAddressedMessages = true;

            _serverUriBase = serverUriBase;
        }

        public override bool ArbiterInitialize(Arbiter arbiter)
        {
            bool result = base.ArbiterInitialize(arbiter);

            _transportServer = new MessageContainerTransportServer();
            if (_transportServer.Initialize(_serverUriBase) == false)
            {
                base.ArbiterUnInitialize();

                _transportServer = null;
                return false;
            }

            _transportServer.MessageContainerReceivedEvent += new HandlerDelegate<String, MessageContainer>(_transport_MessageReceivedEvent);
            _transportServer.ClientConnected += new HandlerDelegate<string>(_transportServer_ClientConnected);
            _transportServer.ClientDisConnected += new HandlerDelegate<string>(_transportServer_ClientDisconnected);

            return result;
        }

        public override bool ArbiterUnInitialize()
        {
            bool result = base.ArbiterUnInitialize();

            if (_transportServer != null)
            {
                _transportServer.UnInitialize();
                _transportServer = null;
            }

            return result;
        }

        void _transportServer_ClientConnected(string id)
        {
            lock (this)
            {
                _clients.Add(id);
            }
            
            if (ClientConnectedEvent != null)
            {
                ClientConnectedEvent(id);
            }
        }

        void _transportServer_ClientDisconnected(string id)
        {
            lock (this)
            {
                _clients.Remove(id);
            }

            if (ClientDisconnectedEvent != null)
            {
                ClientDisconnectedEvent(id);
            }
        }

        /// <summary>
        /// Change default TransportClient behaviour.
        /// Handle a requestMessage received from local Arbiter and send down the pipe.
        /// </summary>
        /// <param name="requestMessage"></param>
        protected override void OnMessageReceived(TransportMessage message)
        {
            TracerHelper.TraceEntry();
            if (message.TransportInfo == null)
            {
                SystemMonitor.Error("Transport message stack can not be null.");
            }

            if (message.TransportInfo == null ||
                message.TransportInfo.TransportInfoCount == 0 ||
                message.TransportInfo.CurrentTransportInfo.Value.ReceiverID == null ||
                message.TransportInfo.CurrentTransportInfo.Value.ReceiverID.HasValue == false ||
                message.TransportInfo.CurrentTransportInfo.Value.ReceiverID.Value.Equals(this.SubscriptionClientID) == false)
            {// This requestMessage was sent to me, not to one of the clients on the server.
                
                SystemMonitor.Error("Error. Send messages to server-client instances.");
                return;
            }

            // We shall now establish the tag defining which will be the client connection receiving the requestMessage (or all).
            // by default send to all.
            string tagID = "*";
            
            if (message.IsRequest)
            {// Request.
                
                if (message.TransportInfo.ForwardTransportInfoCount != 0)
                {
                    if (message.TransportInfo.CurrentForwardTransportInfoAddress.HasValue)
                    {// Has value - take it into account, if no value is present, just use the "*" pop, 
                     // and use the rest of the AdditionalForwardTransportSessionStack.
                        tagID = message.TransportInfo.CurrentForwardTransportInfoAddress.Value.SessionTag;
                    }

                    // Pop in both cases - if it is null or it is value.
                    message.TransportInfo.PopForwardTransportInfo();
                }
            }
            else
            {// Responce.
                // Clear the transporting information from the last node to here.
                message.TransportInfo.PopTransportInfo();
                
                // Now this is the additional marking in the case with the server - the marking of the PipeChannel.
                SystemMonitor.CheckError(message.TransportInfo.TransportInfoCount > 0 &&
                    message.TransportInfo.CurrentTransportInfo.Value.SenderID.Value.Id.Name == "PipeChannelID");

                // Responce requestMessage, read and pop out the channel ID entry .
                tagID = message.TransportInfo.PopTransportInfo().Value.SenderID.Value.SessionTag;
            }

            MessageContainer container = new MessageContainer(message);
            TracerHelper.Trace("[" + message.GetType().Name + "], tag [" + tagID + "] length [" + container.MessageStreamLength + "]");
            GeneralHelper.FireAndForget(delegate() { _transportServer.SendMessageContainer(tagID, container); });
        }

        /// <summary>
        /// Message received from the "Pipe".
        /// </summary>
        void _transport_MessageReceivedEvent(string operationContextSessionID, MessageContainer messageContainer)
        {
            TransportMessage message = (TransportMessage)messageContainer.CreateMessageInstance();
            TracerHelper.Trace("[" + message.GetType().Name + "], length [" + messageContainer.MessageStreamLength + "]");

            if (message.IsRequest)
            {// Request requestMessage.

                {// Mark the request requestMessage with additional fragment in the transportation stack.
                    ArbiterClientId senderID = new ArbiterClientId("PipeChannelID", null, null);
                    senderID.SessionTag = operationContextSessionID;
                    message.TransportInfo.AddTransportInfoUnit(new TransportInfoUnit(Guid.NewGuid(), senderID, this.SubscriptionClientID));
                }

                if (MandatoryRequestMessageReceiverID.HasValue)
                {// There is a default receiver assigned for all messages from this integrator.
                    DoSendCustom(true, Guid.NewGuid(), null, 0, MandatoryRequestMessageReceiverID.Value, this.SubscriptionClientID, message, TimeSpan.Zero);
                }
                else
                {// No explicit mandatory receiver.

                    if (message.TransportInfo.ForwardTransportInfoCount > 0)
                    {
                        // First pop the Session stack, than send the cleared requestMessage.
                        ArbiterClientId? receiverID = message.TransportInfo.PopForwardTransportInfo();
                        DoSendCustom(true, Guid.NewGuid(), null, 0, receiverID, this.SubscriptionClientID, message, TimeSpan.Zero);
                        
                    }
                    else
                    {
                        DoSendCustom(true, Guid.NewGuid(), null, 0, null, this.SubscriptionClientID, message, TimeSpan.Zero);
                    }
                }

            }
            else
            {// Responce requestMessage - send back to whoever sent it to us before.

                // And clear off the initial request information, we are responding now to.
                Guid sessionID = message.TransportInfo.CurrentTransportInfo.Value.Id;
                ArbiterClientId? receiverID = message.TransportInfo.CurrentTransportInfo.Value.SenderID;
                message.TransportInfo.PopTransportInfo();

                this.DoSendCustom(false, sessionID, null, 0,
                    receiverID, this.SubscriptionClientID, message, TimeSpan.Zero);
            }
        }
    }
}
