using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Class encapsulates the serialization/deserialization of messages, thus allowing to have them transported.
    /// </summary>
    [Serializable]
    public sealed class MessageContainer
    {
        MemoryStream _messageStream = new MemoryStream();
        public long MessageStreamLength
        {
            get { return _messageStream.Length; }
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageContainer()
        { 
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageContainer(Message message)
        {
            SerializeMessage(message);
        }

        /// <summary>
        /// Serializes the message to the internal stream and keeps it there for future usage.
        /// </summary>
        public void SerializeMessage(Message message)
        {
            lock (_messageStream)
            {
                _messageStream.SetLength(0);
                if (message != null)
                {
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(_messageStream, message);
                    }
                    catch (Exception exception)
                    {
                        TracerHelper.TraceError("Exception in message serialization [" + exception.Message + "].");
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Create a message instance based on the information in the internal stream.
        /// </summary>
        public Message CreateMessageInstance()
        {
            lock (_messageStream)
            {
                _messageStream.Position = 0;
                if (_messageStream.Length > 0)
                {
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        Message message = (Message)formatter.Deserialize(_messageStream);
                        return message;
                    }
                    catch (Exception exception)
                    {
                        TracerHelper.TraceError("Exception in message deserialization [" + exception.Message + "].");
                        throw;
                    }
                }
            }

            TracerHelper.TraceError("No message serialized to deserialize.");
            return null;
        }

        /// <summary>
        /// Helper static method, allowing to duplicate a message instance.
        /// It will duplicate the message, ONLY if the message is marked as "TransportByReference == false"
        /// or mandatoryDuplication is true.
        /// </summary>
        public static Message DuplicateMessage(Message message, bool mandatoryDuplication)
        {
            //if (mandatoryDuplication == false && message.TransportByReference)
            //{
            //    return message;
            //}

            //if (message is ICloneable)
            //{
            //    return (Message)((ICloneable)message).Clone();
            //}

            MessageContainer container;
            try
            {
                container = new MessageContainer();
                container.SerializeMessage(message);
            }
            catch (Exception exception)
            {
                TracerHelper.TraceError("Exception in message duplication [" + exception.Message + "].");
                throw;
            }

            return container.CreateMessageInstance();
        }

    }
}
