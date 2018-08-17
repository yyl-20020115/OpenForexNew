using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommonSupport
{
    /// <summary>
    /// Base class for operations.
    /// </summary>
    public class OperationInformation
    {
        ManualResetEvent _event;
        public ManualResetEvent CompletionEvent
        {
            get { return _event; }
        }

        volatile string _id = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        protected volatile object _response = null;
        public virtual object Response
        {
            get { return _response; }
        }

        private volatile object _request = null;
        public object Request
        {
            get { return _request; }
            set { _request = value; }
        }

        volatile bool _isStarted = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsStarted
        {
            get { return _isStarted; }
        }

        volatile bool _isComplete = false;

        public delegate void OperationUpdateDelegate(OperationInformation operation);
        public event OperationUpdateDelegate OperationCompleteEvent;

        /// <summary>
        /// 
        /// </summary>
        public OperationInformation()
        {
            _event = new ManualResetEvent(false);
        }

        /// <summary>
        /// Wait for operation to finish, a given timeout period.
        /// </summary>
        public bool WaitResult<ExpectedResultType>(TimeSpan timeout, out ExpectedResultType result)
            where ExpectedResultType : class
        {
            lock (this)
            {
                if (_response != null)
                {
                    result = (ExpectedResultType)_response;
                    return true;
                }
            }

            result = null;
            if (timeout == TimeSpan.MaxValue)
            {
                if (_event.WaitOne(-1, true))
                {// Result properly received in assigned time frame.
                    result = (ExpectedResultType)_response;
                    return true;
                }
            }
            else
            {
                if (_event.WaitOne((int)timeout.TotalMilliseconds, true))
                {// Result properly received in assigned time frame.
                    result = (ExpectedResultType)_response;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Start()
        {
            lock (this)
            {
                if (_isStarted)
                {
                    SystemMonitor.Error("Operation already complete.");
                    return false;
                }

                _isStarted = true;
            }

            return true;
        }

        /// <summary>
        /// Complete the operation and set the result.
        /// </summary>
        public virtual void Complete(object response)
        {
            lock (this)
            {
                if (_isComplete)
                {
                    SystemMonitor.Error("Operation already complete.");
                    return;
                }

                _isComplete = true;
                _response = response;
                _event.Set();
            }

            if (OperationCompleteEvent != null)
            {
                OperationCompleteEvent(this);
            }
        }

    }
}
