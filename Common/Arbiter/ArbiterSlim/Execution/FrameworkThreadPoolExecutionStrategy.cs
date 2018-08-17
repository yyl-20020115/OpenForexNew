using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Arbiter
{
    /// <summary>
    /// Implementation of the execution strategy using the default .net framework thread pool.
    /// </summary>
    public class FrameworkThreadPoolExecutionStrategy : ArbiterSlimExecutionStrategy
    {
        WaitCallback _waitCallback;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FrameworkThreadPoolExecutionStrategy(ArbiterSlimActiveClientStub client)
            : base(client)
        {
            _waitCallback = new WaitCallback(WaitCallbackFunc);
        }

        /// <summary>
        /// Enlist item for execution.
        /// </summary>
        protected override void OnExecute(Envelope envelope)
        {
            ThreadPool.QueueUserWorkItem(_waitCallback);
        }
        
        /// <summary>
        /// Execute item.
        /// </summary>
        protected void WaitCallbackFunc(object parameter)
        {
            Client.PerformExecution(parameter as Envelope);
        }

    }
}
