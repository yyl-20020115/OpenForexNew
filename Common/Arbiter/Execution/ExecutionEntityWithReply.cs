using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    public class ExecutionEntityWithReply : ExecutionEntity
    {
        Message _replyMessage;
        public Message ReplyMessage
        {
            get { return _replyMessage; }
        }

        TimeSpan _replyTimeOut;
        public TimeSpan ReplyTimeOut
        {
            get { return _replyTimeOut; }
        }

        public ExecutionEntityWithReply(Conversation conversation, ArbiterClientId receiverID, TimeSpan timeOut, Message message)
            : base(conversation, receiverID, timeOut, message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="timeOut"></param>
        public void SetReply(Message message, TimeSpan timeOut)
        {
            _replyMessage = message;
            _replyTimeOut = timeOut;
        }
    }

}
