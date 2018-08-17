using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// 
    /// </summary>
    internal class MethodInvocationInformation
    {
        DateTime _lastInvocation = DateTime.MinValue;

        volatile bool _isCallPending = false;

        public bool IsCallCompleted
        {
            get
            {
                if (_currentExecutionResult != null && _currentExecutionResult.IsCompleted == false)
                {
                    return false;
                }

                return true;
            }
        }

        object[] _lastInvocationParameters = null;

        TimeSpan _minimumCallInterval = TimeSpan.MaxValue;

        Control _control = null;

        Delegate _delegate = null;

        IAsyncResult _currentExecutionResult = null;

        /// <summary>
        /// 
        /// </summary>
        protected bool LastInvocationTimedOut
        {
            get
            {
                lock (this)
                {
                    return (DateTime.Now - _lastInvocation) >= _minimumCallInterval;
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MethodInvocationInformation(Control control, Delegate d)
        {
            _control = control;
            _delegate = d;
        }

        /// <summary>
        /// Invoked by the timer based invocation monitor, allows to execute pending calls, after a time interval has passed.
        /// </summary>
        public void CheckCall()
        {
            lock (this)
            {
                if (_isCallPending == false || LastInvocationTimedOut == false)
                {// No call pending, or last invocation too soon.
                    return;
                }
            }

            Invoke(TimeSpan.MaxValue, _lastInvocationParameters);
        }

        /// <summary>
        /// Submit an invoke request, if currently an invoke is done, this will be put as pending.
        /// </summary>
        public bool Invoke(TimeSpan minimumCallInterval, params object[] parameters)
        {
            bool lastInvocationTimedOut = LastInvocationTimedOut;

            lock (this)
            {
                if ((_currentExecutionResult != null && _currentExecutionResult.IsCompleted == false)
                    || (_isCallPending && lastInvocationTimedOut == false))
                {// Current call in progress, or a call already pending and not time for execution yet.

                    _isCallPending = true;
                    _lastInvocationParameters = parameters;
                    _minimumCallInterval = TimeSpan.FromMilliseconds(Math.Min(minimumCallInterval.TotalMilliseconds, _minimumCallInterval.TotalMilliseconds));

                    return false;
                }

                _isCallPending = false;
                _lastInvocationParameters = null;
                _lastInvocation = DateTime.Now;

                // Reset the minimum call interval.
                _minimumCallInterval = TimeSpan.MaxValue;
            }

            // Make sure to do this outside of lock, since it may cause a deadlock.
            _currentExecutionResult = WinFormsHelper.BeginManagedInvoke(_control, _delegate, parameters);
            //System.Diagnostics.Trace.WriteLine(_control.Name, _delegate.Method.Name);

            return true;
        }
    }
}
