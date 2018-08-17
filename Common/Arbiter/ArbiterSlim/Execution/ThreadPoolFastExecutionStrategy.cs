using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Execution strategy implemented using the custom fast thread pool implementation.
    /// </summary>
    public class ThreadPoolFastExecutionStrategy : ArbiterSlimExecutionStrategy
    {
        bool _useCommonArbiterPool;

        volatile ThreadPoolFast _pool;

        FastInvokeHelper.FastInvokeHandlerDelegate _singleFastInvokeDelegate = null;

        ThreadPoolFast ThreadPool
        {
            get
            {
                if (_pool == null)
                {
                    lock (this)
                    {
                        if (_pool == null)
                        {
                            if (_useCommonArbiterPool)
                            {
                                // Refactor caution!
                                if (Client.ArbiterSlim != null)
                                {
                                    _pool = Client.ArbiterSlim.ThreadPool;
                                }
                            }
                            else
                            {
                                _pool = new ThreadPoolFast(Client.Client != null ? Client.Client.Name + ".Slim.ThreadPoolFast" : "NA.Slim.ThreadPoolFast");
                                _pool.MaximumTotalThreadsAllowed = 3;
                                _pool.MinimumThreadsCount = 0;
                            }
                        }
                    }
                }

                return _pool;
            }
        }

        public override int MaxConcurrentExecutions
        {
            get 
            { 
                ThreadPoolFast pool = ThreadPool;
                if (pool != null)
                {
                    return pool.MaximumTotalThreadsAllowed;
                }
                return 0;
            }
            
            set 
            {
                if (_useCommonArbiterPool == false)
                {
                    ThreadPoolFast pool = ThreadPool;
                    if (pool != null)
                    {
                        pool.MaximumTotalThreadsAllowed = value;
                    }
                }
            }
        }

        public override bool SupportsMaxConcurrentExecutionsControl
        {
            get { return _useCommonArbiterPool == false; }
        }

        public override bool SupportsRunningExecutionsReport
        {
            get { return _useCommonArbiterPool == false; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ThreadPoolFastExecutionStrategy(ArbiterSlimActiveClientStub client, bool useCommonArbiterPool)
            : base(client)
        {
            _useCommonArbiterPool = useCommonArbiterPool;

            GeneralHelper.GenericDelegate<ThreadPoolFast, Envelope> delegateInstance = new GeneralHelper.GenericDelegate<ThreadPoolFast, Envelope>(PerformExecution);
            _singleFastInvokeDelegate = FastInvokeHelper.GetMethodInvoker(delegateInstance.Method, false);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void PerformExecution(ThreadPoolFast threadPool, object envelope)
        {
            Client.PerformExecution(envelope as Envelope);
        }

        protected override void OnExecute(Envelope envelope)
        {
            ThreadPoolFast pool = ThreadPool;
            if (pool != null)
            {
                pool.QueueFastDelegate(this, _singleFastInvokeDelegate, envelope);
                //ThreadPoolFastEx.TargetInfo targetInfo = new ThreadPoolFastEx.TargetInfo(string.Empty, this,
                //    _singleFastInvokeDelegate, threadPool, envelope);

                //threadPool.QueueTargetInfo(targetInfo);
            }
        }
    }
}
