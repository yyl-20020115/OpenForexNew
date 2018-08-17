using System;

namespace Arbiter
{
    /// <summary>
    /// Base class for Transport messages. Transport messages allow to utilize the advanced
    /// transporting functionalities related to the Arbiter, and unless required otherwise
    /// all messages should inherit this class.
    /// </summary>
    [Serializable]
    public class TransportMessage : Message
    {
        private bool _isRequest;
        public bool IsRequest
        {
            get { return _isRequest; }
            set { _isRequest = value; }
        }

        TransportInfo _transportInfo = new TransportInfo();
        public TransportInfo TransportInfo
        {
            get { return _transportInfo; }
            set { _transportInfo = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TransportMessage()
        {
        }
    }
}
