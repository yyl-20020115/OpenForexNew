using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommonSupport;

namespace Arbiter
{
    public class ConversationMultiPoint : Conversation
    {
        public ArbiterClientId SenderID
        {
            get { return base.OwnerID; }
        }

        List<ExecutionEntity> _executingEntities = new List<ExecutionEntity>();

        List<ArbiterClientId> _receivers = new List<ArbiterClientId>();
        public List<ArbiterClientId> Receivers
        {
            get 
            {
                lock (_receivers)
                {
                    return _receivers;
                }
            }
        }

        /// 
        /// </summary>
        public ConversationMultiPoint(ExecutionManager executionManager, Message message, ArbiterClientId senderID, IEnumerable<ArbiterClientId> receivers, TimeSpan timeOut)
            : base(executionManager, senderID, timeOut)
        {
            _receivers.AddRange(receivers);

            foreach (ArbiterClientId receiverID in receivers)
            {
                ExecutionEntity entity = new ExecutionEntity(this, receiverID, TimeSpan.Zero, MessageContainer.DuplicateMessage(message, false));
                _executingEntities.Add(entity);
                ExecutionManager.AddExecutionEntity(entity);
            }
        }

        #region IExecutor Members

        public override void EntityExecutionStarted(ExecutionEntity entity)
        {
            lock (_executingEntities)
            {
                SystemMonitor.CheckError(_executingEntities.Contains(entity), "Removing entity not present in conversation.");
            }
        }
        
        public override void EntityExecutionFinished(ExecutionEntity entity)
        {
            lock (_executingEntities)
            {
                SystemMonitor.CheckError(_executingEntities.Contains(entity), "Removing entity not present in conversation.");
                _executingEntities.Remove(entity);

                if (_executingEntities.Count == 0)
                {// OK, we are done.
                    this.Die();
                }
            }
        }

        public override void EntityTimedOut(ExecutionEntity entity)
        {
            lock (_executingEntities)
            {
                SystemMonitor.CheckError(_executingEntities.Contains(entity), "Removing entity not present in conversation.");
                _executingEntities.Remove(entity);
            }
            this.SetTimedOut();
        }

        public override void EntityExecutionFailed(ExecutionEntity entity, Exception exception)
        {
            SystemMonitor.Error("Execution of entity failed [" + exception.ToString() + "].");
            lock (_executingEntities)
            {
                SystemMonitor.CheckError(_executingEntities.Contains(entity), "Removing entity not present in conversation.");
                _executingEntities.Remove(entity);
            }
        }
        #endregion
    }
}
