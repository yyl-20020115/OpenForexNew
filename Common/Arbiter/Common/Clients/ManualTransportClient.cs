using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// Class is usefull when need to use the Arbiter infrastructure and does not want to inherit transport client
    /// and just send and receive message trough a class referenced.
    /// </summary>
    public class ManualTransportClient : TransportClient
    {
        public delegate TransportMessage MessageReceiveWithResponseDelegate(TransportMessage message);

        /// <summary>
        /// Event raised when message received (no reply).
        /// </summary>
        public event HandlerDelegate<TransportMessage> MessageReceivedEvent;

        /// <summary>
        /// Event raise when message receive (reply possible, return null for no reply).
        /// </summary>
        public event MessageReceiveWithResponseDelegate MessageReceiveWithResponseEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ManualTransportClient()
            : base("ManualTransportClient", true)
        {
            Filter.AllowOnlyAddressedMessages = true;
            Filter.AllowChildrenTypes = false;
            Filter.AllowedNonAddressedMessageTypes.Clear();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ManualTransportClient(string name)
            : base("ManualTransportClient." + name, true)
        {
            Filter.AllowOnlyAddressedMessages = true;
            Filter.AllowChildrenTypes = false;
            Filter.AllowedNonAddressedMessageTypes.Clear();
        }

        public new void Send(TransportMessage message)
        {
            base.Send(message);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public Message DirectCall(ArbiterClientId receiverID, TransportMessage message)
        //{
        //    return base.DirectCall(receiverID, message);
        //}

        public new void SendAddressed(ArbiterClientId receiverID, TransportMessage message)
        {
            base.SendAddressed(receiverID, message);
        }

        public TExpectedMessageClass SendAndReceiveAddressed<TExpectedMessageClass>(
            ArbiterClientId receiverID, TransportMessage message, TimeSpan timeOut)
            where TExpectedMessageClass : TransportMessage
        {
            return base.SendAndReceiveAddressed<TExpectedMessageClass>(receiverID, message, timeOut);
        }

        public new TExpectedMessageClass SendAndReceive<TExpectedMessageClass>(TransportMessage message, TimeSpan timeOut)
            where TExpectedMessageClass : TransportMessage
        {
            return base.SendAndReceive<TExpectedMessageClass>(message, timeOut);
        }

        public new void SendResponding(TransportInfo incomingMessageTransportInfo, TransportMessage message)
        {
            base.SendResponding(incomingMessageTransportInfo, message);
        }

        [MessageReceiver]
        protected TransportMessage Receive(TransportMessage message)
        {
            if (MessageReceivedEvent != null)
            {
                MessageReceivedEvent(message);
            }

            if (MessageReceiveWithResponseEvent != null)
            {
                return MessageReceiveWithResponseEvent(message);
            }


            return null;
        }

        //protected override void HandleRequestMessage(TransportMessage message)
        //{

        //}

    }
}
