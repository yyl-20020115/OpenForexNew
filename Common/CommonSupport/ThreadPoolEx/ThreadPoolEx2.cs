using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace CommonSupport
{
    /// <summary>
    /// This is a version of the thread pool using the .NET framework thread pool. It performs overall poorly.
    /// </summary>
    public class ThreadPoolEx2
    {
        /// <summary>
        /// 
        /// </summary>
        public ThreadPoolEx2()
        {
            ThreadPool.SetMaxThreads(55, 125);
        }

        public void Queue(Delegate d, params object[] args)
        {
            ThreadPool.QueueUserWorkItem(WaitCallbackInstance, new object[] { d, args });
        }

        void WaitCallbackInstance(object state)
        {
            GeneralHelper.AssignThreadCulture();

            object[] parameters = (object[])state;
            Delegate d = (Delegate)parameters[0];
            object[] callParams = (object[])parameters[1];

            d.DynamicInvoke(callParams);
        }

    }
}
