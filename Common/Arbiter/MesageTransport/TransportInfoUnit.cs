using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// A single transportation info.
    /// </summary>
    [Serializable]
    public struct TransportInfoUnit
    {
        Guid _id;
        public Guid Id
        {
            get { return _id; }
        }

        public string SenderName
        {
            get
            {
                if (_senderID.HasValue)
                {
                    return _senderID.Value.Id.Name;
                }

                return string.Empty;
            }
        }

        public string ReceiverName
        {
            get
            {
                if (_receiverID.HasValue)
                {
                    return _receiverID.Value.Id.Name;
                }

                return string.Empty;
            }
        }

        ArbiterClientId? _senderID;
        public ArbiterClientId? SenderID
        {
            get { return _senderID; }
        }

        ArbiterClientId? _receiverID;
        public ArbiterClientId? ReceiverID
        {
            get { return _receiverID; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TransportInfoUnit(Guid id, ArbiterClientId? senderID, ArbiterClientId? receiverID)
        {
            _id = id;
            _senderID = senderID;
            _receiverID = receiverID;
        }


        public bool EqualsSenderAndReceiver(TransportInfoUnit other)
        {
            return
                _senderID.HasValue == other._senderID.HasValue &&
                _senderID.Value.Equals(other.SenderID.Value) &&
                _receiverID.HasValue == other._receiverID.HasValue &&
                _receiverID.Value.Equals(other._receiverID.Value);
        }
    }
}
