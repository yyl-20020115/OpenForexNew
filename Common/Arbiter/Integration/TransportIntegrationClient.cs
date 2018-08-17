using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Arbiter.MessageContainerTransport;
using CommonSupport;

namespace Arbiter
{
    public sealed class TransportIntegrationClient : TransportIntegrationBase
    {
        MessageContainerTransportClient _transportClient;

        public delegate void ConnectionStatusChangedDelegate(bool connected);
        public event ConnectionStatusChangedDelegate ConnectionStatusChangedEvent;

        protected override IMessageContainerTransport Transport
        {
            get { return _transportClient; }
        }

        public bool IsConnected
        {
            get { return _transportClient.IsConnected; }
        }

        Uri _serverAddressUri;

        /// <summary>
        /// 
        /// </summary>
        public TransportIntegrationClient(Uri serverAddressUri) 
            : base(serverAddressUri, "ArbiterIntegration.Client", true)
        {
            Filter.Enabled = true;
            Filter.AllowOnlyAddressedMessages = true;

            _serverAddressUri = serverAddressUri;
        }

        void _transportClient_ConnectionStatusChangedEvent(bool connected)
        {
            if (ConnectionStatusChangedEvent != null)
            {
                ConnectionStatusChangedEvent(connected);
            }
        }

        public override bool ArbiterInitialize(Arbiter arbiter)
        {
            bool result = base.ArbiterInitialize(arbiter);

            lock (this)
            {
                if (_transportClient != null)
                {
                    throw new Exception("Already initialized.");
                }

                _transportClient = new MessageContainerTransportClient();
                _transportClient.ConnectionStatusChangedEvent += new MessageContainerTransportClient.ConnectionStatusChangedDelegate(_transportClient_ConnectionStatusChangedEvent);
                _transportClient.MessageContainerReceivedEvent += new HandlerDelegate<MessageContainer>(_transport_MessageReceivedEvent);
            }

            _transportClient.Initialize(_serverAddressUri);

            return result;
        }

        public override bool ArbiterUnInitialize()
        {
            bool result = base.ArbiterUnInitialize();
            lock (this)
            {
                if (_transportClient != null)
                {
                    _transportClient.UnInitialize();
                    _transportClient = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Change default TransportClient behaviour.
        /// Handle a requestMessage received from local Arbiter and send down the pipe.
        /// </summary>
        /// <param name="requestMessage"></param>
        protected override void OnMessageReceived(TransportMessage message)
        {
            if (message.IsRequest == false)
            {// Responce requestMessage.
                // Clear off the transporting information from the last node to here.
                message.TransportInfo.PopTransportInfo();
            }

            MessageContainer container = new MessageContainer(message);
            TracerHelper.Trace("[" + message.GetType().Name + "], length [" + container.MessageStreamLength + "]");

            if (_transportClient != null)
            {
                _transportClient.SendMessageContainer(container);
            }
            else
            {
                SystemMonitor.OperationError("Message received after Integration Client uninitialized [" + message.GetType().Name + "].", TracerItem.PriorityEnum.Medium);
            }
        }

        /// <summary>
        /// Handle a requestMessage received trough the transport pipe.
        /// </summary>
        /// <param name="messageContainer"></param>
        void _transport_MessageReceivedEvent(MessageContainer messageContainer)
        {
            TransportMessage message = (TransportMessage)messageContainer.CreateMessageInstance();
            TracerHelper.Trace("[" + message.GetType().Name + "], length [" + messageContainer.MessageStreamLength + "]");

            if (message.IsRequest)
            {// Request requestMessage.

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

        public void ChangeServerAddressUri(Uri serverAddress)
        {
            _transportClient.ChangeServerAddressUri(serverAddress);
        }
    }
}
