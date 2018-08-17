using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Arbiter.MessageContainerTransport
{
    /// <summary>
    /// This is a mirror interfaces to the IArbiterClient interface.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IMessageContainerTransport))]
    public interface IMessageContainerTransport
    {
        [OperationContract(IsOneWay = false)]
        bool Ping();

        /// <summary>
        /// Register client in server.
        /// </summary>
        [OperationContract(IsOneWay = false)]
        string ClientRegister();

        /// <summary>
        /// Typical execution.
        /// </summary>
        /// <param name="execution"></param>
        [OperationContract(IsOneWay = true)]
        void ReceiveMessageContainer(MessageContainer messageContainer);
    }
}
