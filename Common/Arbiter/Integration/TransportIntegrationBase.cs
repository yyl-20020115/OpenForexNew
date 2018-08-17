using System;
using System.Collections.Generic;
using System.Text;
using Arbiter.MessageContainerTransport;

namespace Arbiter
{
    public abstract class TransportIntegrationBase : TransportClient
    {
        protected abstract IMessageContainerTransport Transport
        {
            get;
        }

        /// <summary>
        /// Sole receiver stands for only have 1 class receive addressed all the messages that come trough this integration.
        /// </summary>
        ArbiterClientId? _mandatoryRequestMessageReceiverID;
        public ArbiterClientId? MandatoryRequestMessageReceiverID
        {
            get { return _mandatoryRequestMessageReceiverID; }
            set { _mandatoryRequestMessageReceiverID = value; }
        }

        Uri _address;
        public Uri Address
        {
            get { return _address; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="singleThreadMode"></param>
        public TransportIntegrationBase(Uri address, string name, bool singleThreadMode)
            : base(name, singleThreadMode)
        {
            _address = address;
        }



    }
}
