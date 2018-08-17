//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using System.Runtime.Serialization;
//using System.ComponentModel;

//namespace Arbiter
//{
//    /// <summary>
//    /// This type of transport client supports performing operations trough messages on a remote location.
//    /// Operations are not persisted.
//    /// The OperationalTransportClient refers to Operation states and is not directly related to the 
//    /// Operations performing capabilities of this class.
//    /// </summary>
//    public class OperationPerformingTransportClient : OperationalTransportClient, OperationPerformerStub.IImplementation
//    {
//        volatile List<ArbiterClientId?> _operationHandlerPath = new List<ArbiterClientId?>();
//        /// <summary>
//        /// 
//        /// </summary>
//        [Browsable(false)]
//        protected List<ArbiterClientId?> OperationHandlerPathUnsafe
//        {
//            get { return _operationHandlerPath; }
//        }

//        OperationPerformerStub _operationStub;

//        /// <summary>
//        /// Default constructor.
//        /// </summary>
//        public OperationPerformingTransportClient(string name)
//            : base(name, false)
//        {
//            this.DefaultTimeOut = TimeSpan.FromSeconds(60);
//            _operationStub = new OperationPerformerStub(this);
//        }

//        /// <summary>
//        /// Deserialization constructor.
//        /// </summary>
//        public OperationPerformingTransportClient(SerializationInfo info, StreamingContext context)
//            : base(info, context)
//        {
//            _operationStub = new OperationPerformerStub(this);
//        }

//        /// <summary>
//        /// The path to the target of operation execution.
//        /// Will return null if not assigned.
//        /// </summary>
//        protected List<ArbiterClientId?> GetOperationHandlerPathCopy()
//        {
//            lock (this)
//            {
//                if (_operationHandlerPath == null || _operationHandlerPath.Count == 0)
//                {// No receiver, cancel operation.
//                    SystemMonitor.Warning("_targetTransportationInfo not assigned.");
//                    return null;
//                }

//                return new List<ArbiterClientId?>(_operationHandlerPath);
//            }
//        }


//        /// <summary>
//        /// Helper. Send and receive the operation messages. 
//        /// Operations must always be addressed, the _forwardTransportation must be assigned.
//        /// </summary>
//        protected TransportMessage SendAndReceiveMessage(TransportMessage requestMessage)
//        {
//            List<ArbiterClientId?> forwardTransport = GetOperationHandlerPathCopy();
//            if (forwardTransport == null)
//            {
//                return null;
//            }

//            return this.SendAndReceiveForwarding<TransportMessage>(forwardTransport, requestMessage);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        [MessageReceiver]
//        public void Receive(OperationResponceMessage message)
//        {

//            if (message.OperationID < 0)
//            {
//                SystemMonitor.Warning("Message not handled properly [" + message.GetType().Name + "]");
//            }

//            _operationStub.CompleteOperation(message.OperationID.ToString(), message);
//        }


//        #region IImplementation Members

//        public bool StartOperation(OperationInformation operationInfo)
//        {
//            OperationMessage requestMessage = ((ArbiterOperationInformation)operationInfo).RequestMessage;
//            OperationResponceMessage result = (OperationResponceMessage)SendAndReceiveMessage(requestMessage);

//            if (result == null || result.OperationResult == false)
//            {// Operation placement failed or timed out - remove from pending operations and close.
//                if (result == null)
//                {
//                    SystemMonitor.OperationError("Operation for [" + requestMessage.GetType().Name + "] timed out.", TracerItem.PriorityEnum.Medium);
//                }
//                else
//                {
//                    SystemMonitor.OperationError("Operation for [" + requestMessage.GetType().Name + "] failed [" + result.OperationResultMessage + "].", TracerItem.PriorityEnum.High);
//                }

//                return false;
//            }

//            return true;
//        }

//        #endregion
//    }
//}
