//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Arbiter
//{
//    public class DummyClient : ArbiterClientBase
//    {
//        public event HandlerDelegate<Message> MessageReceivedEvent;

//        public DummyClient()
//            : base("DummyClient", true)
//        {
//            _messageFilter.Enabled = false;
//        }

//        public void Send(Message requestMessage)
//        {
//            Arbiter.CreateConversation(this._id, requestMessage, TimeSpan.Zero);
//        }

//        public override void ReceiveExecution(ExecutionEntity entity)
//        {
//            if (MessageReceivedEvent != null)
//            {
//                MessageReceivedEvent(entity.Message);
//            }
//        }

//        public override void ReceiveExecutionWithReply(ExecutionEntityWithReply entity)
//        {
//            if (MessageReceivedEvent != null)
//            {
//                MessageReceivedEvent(entity.Message);
//            }
//        }

//        public override void ReceiveConversationTimedOut(Conversation conversation)
//        {
            
//        }
//    }
//}
