using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// Multiple threads are entering those methods.
    /// </summary>
    public interface IExecutor
    {
        void EntityExecutionStarted(ExecutionEntity entity);
        void EntityExecutionFinished(ExecutionEntity entity);
        void EntityExecutionFailed(ExecutionEntity entity, Exception exception);
        void EntityTimedOut(ExecutionEntity entity);
    }
}
