//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Arbiter
//{
//    public class MessagePath
//    {
//        List<ArbiterClientId?> _path = new List<ArbiterClientId?>();

//        public static MessagePath Empty
//        {
//            get { return new MessagePath(); }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public MessagePath()
//        {
//        }

//        public void SetMessageAddressing(TransportMessage requestMessage, ArbiterClientId? receiverId, ArbiterClientId? senderId, Guid sessionGuid)
//        {
//            if (requestMessage.IsRequest)
//            {
//                for (int i = _path.Count - 1; i >= 0; i++)
//                {// Push the elements in reverse order, so that the first added in the forward
//                    // addressing is the one on top of the stack, second is second etc.
//                    requestMessage.AdditionalForwardTransportSessionStack.Push(_path[i]);
//                }
//            }

//        }

//    }
//}
