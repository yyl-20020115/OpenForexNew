using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace CommonSupport
{
    /// <summary>
    /// Class extends tha custom fast thread pool with a few easy to use features (yet slow, so be cautious).
    /// </summary>
    public class ThreadPoolFastEx : ThreadPoolFast
    {
        static List<Type> OwnerTypes = new List<Type>(new Type[] { typeof(ThreadPoolFastEx) });

        Dictionary<MethodInfo, FastInvokeHelper.FastInvokeHandlerDelegate> _methodDelegates = new Dictionary<MethodInfo, FastInvokeHelper.FastInvokeHandlerDelegate>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of this thread pool</param>
        public ThreadPoolFastEx(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Helper, obtain the correspoding fast delegate of a method info.
        /// </summary>
        public FastInvokeHelper.FastInvokeHandlerDelegate GetMessageHandler(MethodInfo methodInfo)
        {
            lock (_methodDelegates)
            {
                if (_methodDelegates.ContainsKey(methodInfo))
                {
                    return _methodDelegates[methodInfo];
                }

                FastInvokeHelper.FastInvokeHandlerDelegate resultHandler = FastInvokeHelper.GetMethodInvoker(methodInfo, true);
                _methodDelegates[methodInfo] = resultHandler;
                return resultHandler;
            }
        }

        /// <summary>
        /// This is dreadfully slow and can overload CPU with only 3000 calls per second!
        /// </summary>
        /// <returns></returns>
        string ObtainCallerName()
        {
            if (Debugger.IsAttached)
            {
                MethodBase method = ReflectionHelper.GetExternalCallingMethod(2, OwnerTypes);
                if (method != null)
                {
                    return method.Name;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Add a new delegate call to be executed.
        /// This call gives a performance of approx 300 - 400 K calls per second, so it is not very fast.
        /// For more speed, use the QueueFastDelegate and the QueueTargetInfo methods (1.5-3.5 Mil on a Dual Core).
        /// </summary>
        public void Queue(Delegate d, params object[] args)
        {
            if (d == null)
            {
                return;
            }

            // This is 3000 calls per seconds SLOW.
            string callerName = ObtainCallerName();

            // The "d.Method" call is AWFULLY SLOW.
            QueueTargetInfo(new TargetInfo(callerName, d.Target, GetMessageHandler(d.Method), false, this, args));
        }



    }  
}
