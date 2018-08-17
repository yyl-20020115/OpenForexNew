using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ForexPlatformPersistence;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Envelope stores a message.
    /// </summary>
    [Serializable]
    public class Envelope
    {
        volatile bool _transportByReference = false;
        /// <summary>
        /// Should the message be transported by reference, or be cloned.
        /// </summary>
        public bool TransportByReference
        {
            get { return _transportByReference; }
        }

        volatile object _message = null;
        /// <summary>
        /// The actual message.
        /// </summary>
        public object Message
        {
            get { return _message; }
        }

        volatile int _receiverArbiterSlimIndex = -1;
        /// <summary>
        /// Arbiter slim index of the receiving entity.
        /// </summary>
        public int ReceiverArbiterSlimIndex
        {
            get { return _receiverArbiterSlimIndex; }
            set { _receiverArbiterSlimIndex = value; }
        }

        volatile int _senderArbiterSlimIndex = -1;
        /// <summary>
        /// Arbiter slim index of the sender entity.
        /// </summary>
        public int SenderArbiterSlimIndex
        {
            get { return _senderArbiterSlimIndex; }
            set { _senderArbiterSlimIndex = value; }
        }

        private bool _directExecution = false;
        /// <summary>
        /// Is the envelope to be directly executed at host.
        /// </summary>
        public bool DirectExecution
        {
            get { return _directExecution; }
            set { _directExecution = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Envelope()
        {
        }

        /// <summary>
        /// Should we transport by reference, or we should clone the message.
        /// </summary>
        public Envelope(int receiverArbiterSlimIndex, bool transportByReference, object message)
        {
            _receiverArbiterSlimIndex = receiverArbiterSlimIndex;
            _transportByReference = transportByReference;
            _message = message;
        }

        #region ICloneable Members

        public virtual Envelope Clone()
        {
            Envelope newObject = (Envelope)this.MemberwiseClone();
            
            if (_transportByReference == false && _message != null)
            {
                if (_message is ICloneable)
                {
                    newObject._message = ((ICloneable)_message).Clone();
                }
                else if (_message.GetType().IsClass)
                {// We need to use the slow cloning mechanism.
                    newObject._message = SerializationHelper.BinaryClone(_message);
                    SystemMonitor.CheckOperationError(newObject._message != null, "Failed to serialize message [" + _message.GetType().Name + "].");
                }

                // Non class items are supposed to be copied by referencing.
            }

            return newObject;
        }

        #endregion
    }
}
