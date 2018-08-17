using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Runtime.Serialization;

namespace Arbiter
{
    [Serializable]
    public class TransportMessageFilter : MessageFilter, IDeserializationCallback
    {
        bool _allowOnlyAddressedMessages = true;
        public bool AllowOnlyAddressedMessages
        {
            get { return _allowOnlyAddressedMessages; }
            set { _allowOnlyAddressedMessages = value; }
        }

        ArbiterClientId _ownerID;

        /// <summary>
        /// 
        /// </summary>
        public TransportMessageFilter(ArbiterClientId ownerID) : base(true)
        {
            _ownerID = ownerID;
        }



        /// <summary>
        /// 
        /// </summary>
        protected virtual bool IsAddressedToMe(TransportMessage message)
        {
            return (message.TransportInfo.CurrentTransportInfo.Value.ReceiverID != null &&
                message.TransportInfo.CurrentTransportInfo.Value.ReceiverID.HasValue && this._ownerID.Equals(message.TransportInfo.CurrentTransportInfo.Value.ReceiverID.Value));
        }

        /// <summary>
        /// Do the filtering based on the transportSessionGuid but allow all that are starting a session now.
        /// </summary>
        /// <param name="requestMessage"></param>
        public override bool MessageAllowed(Message messageInput)
        {
            SystemMonitor.CheckError(messageInput is TransportMessage);
            TransportMessage message = (TransportMessage)messageInput;

            if (this.Enabled == false)
            {
                return true;
            }

            bool isTypeAllowed = base.MessageAllowed(message);
            bool isAddressed = (message.TransportInfo.CurrentTransportInfo.Value.ReceiverID != null);
            bool isAddressedToMe = IsAddressedToMe(message); 

            if (AllowOnlyAddressedMessages)
            {// Addressed mode.
                if (isAddressed == false || isAddressedToMe == false)
                {
                    return false;
                }
            }
            else
            {// Non addressed mode.
                if (isAddressed && isAddressedToMe == false)
                {// Addressed to someone else.
                    return false;
                }
            }

            // Addressing solved - it allowed the requestMessage to pass on.
            if (message.IsRequest)
            {// Request.
                return isTypeAllowed;
            }
            else
            {// Responce - accept.
                return true;
            }
        }


        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            
        }

        #endregion
    }
}
