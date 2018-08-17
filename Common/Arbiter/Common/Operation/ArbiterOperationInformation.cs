//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using CommonSupport;

//namespace Arbiter
//{
//    /// <summary>
//    /// Represents a requestMessage operation performed by someone.
//    /// It has a OperationMessage and a OperationResultMessage.
//    /// It synchronizes the performing of the operation.
//    /// </summary>
//    public class ArbiterOperationInformation : OperationInformation
//    {
//        OperationMessage _requestMessage;
//        public OperationMessage RequestMessage
//        {
//            get { lock (this) { return _requestMessage; } }
//        }

//        public new OperationResponceMessage Responce
//        {
//            get { lock (this) { return (OperationResponceMessage)_responce; } }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public ArbiterOperationInformation(OperationMessage requestMessage)
//        {
//            _requestMessage = requestMessage;
//        }

//        public void Complete(OperationResponceMessage responceMessage)
//        {
//            SystemMonitor.CheckError(responceMessage.OperationID == _requestMessage.OperationId);
//            base.Complete(responceMessage);
//        }
//    }
//}
