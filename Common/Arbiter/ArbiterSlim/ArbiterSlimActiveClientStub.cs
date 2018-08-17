using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Runtime.Serialization;
using System.Threading;
using System.Reflection;

namespace Arbiter
{
    /// <summary>
    /// Default stub standalone implementation of an Arbiter Slim client.
    /// </summary>
    public class ArbiterSlimActiveClientStub : Operational, IArbiterSlimClient
    {
        volatile IArbiterClient _client;
        /// <summary>
        /// 
        /// </summary>
        public IArbiterClient Client
        {
            get { return _client; }
        }

        volatile ArbiterSlim _arbiterSlim = null;
        /// <summary>
        /// 
        /// </summary>
        public ArbiterSlim ArbiterSlim
        {
            get { return _arbiterSlim; }
        }

        private volatile int _arbiterSlimIndex = -1;
        /// <summary>
        /// Assigned upon entering the Arbiter.
        /// </summary>
        public int ArbiterSlimIndex
        {
            get { return _arbiterSlimIndex; }
        }

        ArbiterSlimExecutionStrategy _executionStrategy;
        /// <summary>
        /// Execution strategy defines how threads are managed on executing the tasks in the client.
        /// </summary>
        public ArbiterSlimExecutionStrategy ExecutionStrategy
        {
            get { return _executionStrategy; }
        }

        /// <summary>
        /// *WARNING* this event is executed on an incoming thread from the arbiter, and must never be blocked, since it will compomise the model.
        /// This is NOT executed on the new execution thread, use EnvelopeExecuteEvent for this.
        /// </summary>
        public event EnvelopeUpdateDelegate EnvelopeReceivedEvent;

        public event EnvelopeUpdateDelegate EnvelopeExecuteEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ArbiterSlimActiveClientStub(IArbiterClient client)
        {
            _client = client;

            //_executionStrategy = new FrameworkThreadPoolExecutionStrategy(this);
            _executionStrategy = new ThreadPoolFastExecutionStrategy(this, true);

            Construct();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ArbiterSlimActiveClientStub(IArbiterClient client, ArbiterSlimExecutionStrategy executionStrategy)
        {
            _client = client;
            _executionStrategy = executionStrategy;

            SystemMonitor.CheckThrow(executionStrategy != null, "Execution strategy not assigned.");

            Construct();
        }

        /// <summary>
        /// Construction helper.
        /// </summary>
        void Construct()
        {
            _arbiterSlim = null;
            _arbiterSlimIndex = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool OnAddToArbiter(ArbiterSlim arbiter, int arbiterIndex)
        {
            if (_arbiterSlim != null)
            {
                return false;
            }

            lock (this)
            {// Lock needed to assure operations go together.
                _arbiterSlimIndex = arbiterIndex;
                _arbiterSlim = arbiter;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnRemoveFromArbiter()
        {
            lock (this)
            {// Lock needed to assure operations go together.
                _arbiterSlim = null;
                _arbiterSlimIndex = -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Send(Envelope envelope)
        {
            ArbiterSlim slim = _arbiterSlim;
            if (slim != null)
            {
                envelope.SenderArbiterSlimIndex = this.ArbiterSlimIndex;
                return _arbiterSlim.Send(this._arbiterSlimIndex, envelope);
            }

            return false;
        }

        /// <summary>
        /// Receive an envelope for executing.
        /// </summary>
        /// <param name="canExecute">Can directly execute on this thread.</param>
        public bool Receive(Envelope envelope)
        {
            EnvelopeUpdateDelegate envelopeReceivedDelegate = EnvelopeReceivedEvent;
            if (envelopeReceivedDelegate != null)
            {
                envelopeReceivedDelegate(this, envelope);
            }

            ArbiterSlimExecutionStrategy executionStrategy = _executionStrategy;
            if (executionStrategy != null)
            {
                executionStrategy.Execute(envelope);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void PerformExecution(Envelope envelope)
        {
            EnvelopeUpdateDelegate envelopeExecutedDelegate = EnvelopeExecuteEvent;
            if (envelopeExecutedDelegate != null)
            {
                envelopeExecutedDelegate(this, envelope);
            }
        }

    }
}
