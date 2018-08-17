using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    public class ExecutionEntity : TimeOutable
    {
        ArbiterClientId _receiverID;
        public ArbiterClientId ReceiverID
        {
            get { return _receiverID; }
        }

        Message _message;
        public Message Message
        {
            get { return _message; }
        }

        IExecutor _executor;
        public IExecutor Conversation
        {
            get { return _executor; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="timeOut"></param>
        /// <param name="requestMessage"></param>
        public ExecutionEntity(IExecutor executor, ArbiterClientId receiverID, TimeSpan timeOut, Message message) : base(timeOut)
        {
            _executor = executor;
            _receiverID = receiverID;
            _message = message;
        }
    }

}
