using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CommonSupport
{
    /// <summary>
    /// Tracer item coming from a baseMethod tracing information.
    /// </summary>
    public class MethodTracerItem : TracerItem
    {
        /// <summary>
        /// Method that posted the item.
        /// </summary>
        MethodBase _methodBase;
        public MethodBase MethodBase
        {
            get { return _methodBase; }
        }

        /// <summary>
        /// Assembly that posted this entry.
        /// </summary>
        public override System.Reflection.Assembly Assembly
        {
            get { return _methodBase.DeclaringType.Assembly; }
        }

        volatile string _threadName = string.Empty;
        /// <summary>
        /// Name of the thread that placed this trace entry (if provided).
        /// </summary>
        public string ThreadName
        {
            get { return _threadName; }
        }

        volatile string _threadId = string.Empty;
        /// <summary>
        /// Id of the thread that placed this entry (if provided).
        /// </summary>
        public string ThreadId
        {
            get { return _threadId; }
            set { _threadId = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MethodTracerItem(TypeEnum itemType, TracerItem.PriorityEnum priority, string message, MethodBase methodInfo)
            : base(itemType, priority, message)
        {
            _methodBase = methodInfo;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MethodTracerItem(TypeEnum itemType, TracerItem.PriorityEnum priority, string message, MethodBase methodInfo, string threadName, string threadId)
            : base(itemType, priority, message)
        {
            _methodBase = methodInfo;
            _threadId = threadId;
            _threadName = threadName;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string PrintMessage()
        {
            string assemblyName = _methodBase.DeclaringType.Assembly.GetName().Name;
            string fullMethodName = assemblyName + "." + _methodBase.DeclaringType.Name + "." + _methodBase.Name;

            string threadInfo = string.Format("{0},{1}", _threadId, _threadName);
            threadInfo = threadInfo.Trim(',');

            return threadInfo + " " + fullMethodName + " " + base.PrintMessage();
        }
    }
}
