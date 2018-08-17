using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Arbiter
{
    public abstract class Conversation : TimeOutable, IExecutor
    {
        ArbiterClientId _ownerID;

        /// <summary>
        /// The owner of a conversation (the one who started it) receives notifications for the status (conversation timeout etc.)
        /// </summary>
        public ArbiterClientId OwnerID
        {
            get { return _ownerID; }
        }

        volatile ExecutionManager _executionManager;
        protected ExecutionManager ExecutionManager
        {
            get 
            { 
                return _executionManager; 
            }
        }

        ManualResetEvent _manualCompletionEvent = new ManualResetEvent(false);
        public ManualResetEvent ManualCompletionEvent
        {
            get
            {
                return _manualCompletionEvent;
            }
        }

        public event HandlerDelegate<Conversation> CompletionEvent;

        /// <summary>
        /// 
        /// </summary>
        public Conversation(ExecutionManager executionManager, ArbiterClientId ownerID, TimeSpan timeOut)
            : base(timeOut)
        {
            _ownerID = ownerID;
            _executionManager = executionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void RaiseCompletionEvents()
        {
            lock (_manualCompletionEvent)
            {
                _manualCompletionEvent.Set();
            }
            lock (_manualCompletionEvent)
            {
                if (CompletionEvent != null)
                {
                    CompletionEvent(this);
                }
            }
        }

        #region IExecutor Members

        public abstract void EntityExecutionStarted(ExecutionEntity entity);
        
        public abstract void EntityExecutionFinished(ExecutionEntity entity);
        
        public abstract void EntityTimedOut(ExecutionEntity entity);
        
        public abstract void EntityExecutionFailed(ExecutionEntity entity, Exception exception);

        #endregion
    }
}
