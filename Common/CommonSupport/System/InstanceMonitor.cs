using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Class provides a way for a specific class instance to control
    /// its access to tracing and system monitoring resources, and
    /// configure the conditions of this access.
    /// </summary>
    public class InstanceMonitor
    {
        volatile bool _enabledOperationWarning = true;
        /// <summary>
        /// 
        /// </summary>
        public bool EnabledOperationWarning
        {
            get { return _enabledOperationWarning; }
            set { _enabledOperationWarning = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public InstanceMonitor()
        {
        }

        /// <summary>
        /// Report operation warning; it is a normal occurence in the work of the system. It can be caused
        /// for example by the lack of access to a resource or some error in a data stream.
        /// </summary>
        /// <param name="warningMessage"></param>
        public void OperationWarning(string warningMessage)
        {
            if (_enabledOperationWarning)
            {
                SystemMonitor.OperationWarning(warningMessage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OperationWarning(string warningMessage, TracerItem.PriorityEnum priority)
        {
            if (_enabledOperationWarning)
            {
                SystemMonitor.OperationWarning(warningMessage, priority);
            }
        }

        /// <summary>
        /// A Warning notifies that in some part of the systems operation a recovarable error has occured.
        /// </summary>
        /// <param name="warningMessage"></param>
        public void Warning(string warningMessage)
        {
            SystemMonitor.Warning(warningMessage);
        }

        /// <summary>
        /// Report operation error; it is a lighter version of a error, and may be expected to 
        /// occur during normal operation of the application (for. ex. a given non critical 
        /// resource was not retrieved, operation has timed out etc.)
        /// </summary>
        /// <param name="errorMessage"></param>
        public void OperationError(string errorMessage)
        {
            SystemMonitor.OperationError(errorMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMessage"></param>
        public void OperationError(string errorMessage, TracerItem.PriorityEnum priority)
        {
            SystemMonitor.OperationError(errorMessage, priority);
        }

        /// <summary>
        /// Report an serious error. Those errors are usually a sign something in the work
        /// of the application has gone seriously wrong, and operation can not continue
        /// properly (for ex. unexpected exception, access to critical resources etc.)
        /// </summary>
        /// <param name="errorMessage"></param>
        public void Error(string errorMessage)
        {
            SystemMonitor.Error(errorMessage);
        }
    }
}
