using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Arbiter conversation - an abstraction defining how the operation of deliverying the message
    /// is performed.
    /// </summary>
    public class ConversationPointToPoint : Conversation
    {
        /// <summary>
        /// ID of the conversation owner.
        /// </summary>
        public ArbiterClientId SenderID
        {
            get { return base.OwnerID; }
        }

        ArbiterClientId _receiverID;
        /// <summary>
        /// ID of the conversation receiver.
        /// </summary>
        public ArbiterClientId ReceiverID
        {
            get { return _receiverID; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConversationPointToPoint(ExecutionManager executionManager, Message message, ArbiterClientId senderID, ArbiterClientId receiverID, TimeSpan timeOut)
            : base(executionManager, senderID, timeOut)
        {
            _receiverID = receiverID;

            ExecutionEntityWithReply entity = new ExecutionEntityWithReply(this, receiverID, timeOut,
                MessageContainer.DuplicateMessage(message, false));

            ExecutionManager.AddExecutionEntity(entity);
        }

        /// <summary>
        /// Entity execution has started.
        /// </summary>
        public override void EntityExecutionStarted(ExecutionEntity entity)
        {
        }

        /// <summary>
        /// Entity execution has finished.
        /// </summary>
        public override void EntityExecutionFinished(ExecutionEntity entity)
        {
            ExecutionEntityWithReply entityWithReply = (ExecutionEntityWithReply)entity;
            if (entityWithReply.ReplyMessage != null)
            {// The other side replied.
                
                ArbiterClientId messageReceiver = ReceiverID;

                if (_receiverID.Equals(entity.ReceiverID))
                {// Swap direction, receiver must now send.
                    messageReceiver = SenderID;
                }

                ExecutionEntityWithReply replyEntity = new ExecutionEntityWithReply(this, messageReceiver, entityWithReply.TimeOut,
                    MessageContainer.DuplicateMessage(entityWithReply.ReplyMessage, false));

                ExecutionManager.AddExecutionEntity(replyEntity);
            }
            else
            {// We received a nothing, conversation done, die.
                this.Die();
            }
        }

        /// <summary>
        /// Entity has timed out.
        /// </summary>
        /// <param name="entity"></param>
        public override void EntityTimedOut(ExecutionEntity entity)
        {
            this.SetTimedOut();
        }

        public override void EntityExecutionFailed(ExecutionEntity entity, Exception exception)
        {
            SystemMonitor.OperationError("Entity execution failed [" + exception.ToString() + "].", TracerItem.PriorityEnum.Medium);
            this.Die();
        }
    }
}
