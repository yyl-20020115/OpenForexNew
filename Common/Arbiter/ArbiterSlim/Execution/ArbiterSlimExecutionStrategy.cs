using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Base class for thread execution strategies, for the Arbter Slim client framework.
    /// </summary>
    public abstract class ArbiterSlimExecutionStrategy
    {
        ArbiterSlimActiveClientStub _client;
        /// <summary>
        /// Instance of the client stub, this strategy serves on.
        /// </summary>
        internal ArbiterSlimActiveClientStub Client
        {
            get { return _client; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int MaxConcurrentExecutions
        {
            get { return 0; }
            set { }
        }

        public virtual bool SupportsMaxConcurrentExecutionsControl
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int RunningExecutions
        {
            get { return 0; }
        }

        public virtual bool SupportsRunningExecutionsReport
        {
            get { return false; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ArbiterSlimExecutionStrategy(ArbiterSlimActiveClientStub client)
        {
            SystemMonitor.CheckThrow(client != null, "Client not assigned to execution strategy.");
            _client = client;
        }

        /// <summary>
        /// Enlist item for execution.
        /// </summary>
        public virtual void Execute(Envelope envelope)
        {
            if (envelope.DirectExecution)
            {
                _client.PerformExecution(envelope);
            }
            else
            {
                OnExecute(envelope);
            }
        }

        /// <summary>
        /// Implementations handle this to do the actual execution.
        /// </summary>
        protected abstract void OnExecute(Envelope envelope);
    }
}
