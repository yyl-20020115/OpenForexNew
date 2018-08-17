using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// Class allows the application of a background message loop - similar to this of the main win form application thread.
    /// This is very usefull when in need to deal with COM events etc. since they require an active message loop to operate.
    /// Class allows to specify calls to be made, in its internal thread, by adding invocation delegates.
    /// Those calls will be made to a given baseMethod, of an object with specified parameters, but in this classes thread.
    /// TODO: this can be integrated into the extended thread pool class, allowing it to be a bit more flexible,
    /// however it is also a bit different since it relies on work on one thread, not many.
    /// 
    /// NEEDS TO BE OPTIMIZED, SINCE CURRENTLY IT RUNS ON A 50MS WINDOWS TIMER.
    /// </summary>
    public class BackgroundMessageLoopOperator : IDisposable
    {
        /// <summary>
        /// Structure contains data for a single invocation.
        /// </summary>
        protected class InvocationData
        {
            public ManualResetEvent CompletedEvent = new ManualResetEvent(false);

            public Delegate DelegateInstance;
            public object[] Parameters;
            public object Result;

            /// <summary>
            /// 
            /// </summary>
            public InvocationData(Delegate delegateInstance, object[] parameters)
            {
                DelegateInstance = delegateInstance;
                Parameters = parameters;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Perform()
            {
                try
                {
                    Result = DelegateInstance.Method.Invoke(DelegateInstance.Target, Parameters);
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException)
                    {
                        throw;
                    }
                    else
                    {
                        string message = "Invoke threw an exception [" + DelegateInstance.Method.Name + " , " + ex.Message + "]";
                        if (ex.InnerException != null)
                        {
                            message += ", Inner[" + ex.InnerException.GetType().Name + ", " + ex.InnerException.Message + "]";
                        }

                        SystemMonitor.OperationError(message);
                    }
                }

                CompletedEvent.Set();
            }
        }

        bool _keepRunning = true;

        volatile Thread _workerInternalThread = null;

        bool IsStarted
        {
            get
            {
                return _workerInternalThread != null;
            }
        }

        List<InvocationData> _pendingInvokes = new List<InvocationData>();

        volatile bool _blockingInvokeInProgress = false;

        /// <summary>
        /// Is invoke required on current thread.
        /// </summary>
        public bool InvokeRequred
        {
            get { return Thread.CurrentThread != _workerInternalThread; }
        }

        /// <summary>
        /// 
        /// </summary>
        public BackgroundMessageLoopOperator(bool autoStart)
        {
            if (autoStart)
            {
                Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Start()
        {
            if (_workerInternalThread != null)
            {// Only 1 call ever allowed.
                return false;
            }

            GeneralHelper.ApplicationClosingEvent += new GeneralHelper.DefaultDelegate(GeneralHelper_ApplicationClosingEvent);

            lock (this)
            {
                _workerInternalThread = new Thread(Run);
                _workerInternalThread.Name = "Background Message Loop Operator";

                // Make sure to leave ApartmentState by default (MTA) as otherwise LoaderLock may occur.
                // Yes, BUT COM events work better (at all) in STA!?! otherwise crashing, so ignore LoaderLock.
                _workerInternalThread.SetApartmentState(ApartmentState.STA);
                _workerInternalThread.Start();
            }

            return true;
        }

        void GeneralHelper_ApplicationClosingEvent()
        {
            TracerHelper.TraceSimple(TracerItem.TypeEnum.Report, "Background message operator GeneralHelper_ApplicationClosingEvent");
            Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Stop()
        {
            GeneralHelper.ApplicationClosingEvent -= new GeneralHelper.DefaultDelegate(GeneralHelper_ApplicationClosingEvent);
            _keepRunning = false;
            return true;
        }

        /// <summary>
        /// Helper, to allow the usage of anonymous delegates.
        /// </summary>
        /// <param name="delegateInstance"></param>
        /// <param name="parameters"></param>
        public void BeginInvoke(GeneralHelper.DefaultDelegate delegateInstance)
        {
            BeginInvoke((Delegate)delegateInstance);
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginInvoke(Delegate delegateInstance, params object[] parameters)
        {
            if (IsStarted == false)
            {
                Start();
            }

            lock (this)
            {
                _pendingInvokes.Add(new InvocationData(delegateInstance, parameters));
            }
        }

        /// <summary>
        /// Helper, to allow the use of anonymous delegates.
        /// </summary>
        public object Invoke(GeneralHelper.DefaultReturnDelegate delegateInstance)
        {
            object result;
            Invoke((Delegate)delegateInstance, TimeSpan.MaxValue, out result);
            return result;
        }

        /// <summary>
        /// Helper, to allow the use of anonymous delegates.
        /// </summary>
        public object Invoke(GeneralHelper.DefaultDelegate delegateInstance)
        {
            object result;
            Invoke(delegateInstance, TimeSpan.MaxValue, out result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Invoke(GeneralHelper.DefaultDelegate delegateInstance, out object result, TimeSpan timeOut)
        {
            return Invoke((Delegate)delegateInstance, timeOut, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Invoke(GeneralHelper.DefaultDelegate delegateInstance, TimeSpan timeOut)
        {
            object dummy;
            return Invoke((Delegate)delegateInstance, timeOut, out dummy);
        }

        /// <summary>
        /// Invoke synchronously.
        /// </summary>
        /// <param name="delegateInstance"></param>
        /// <param name="parameters"></param>
        public bool Invoke(Delegate delegateInstance, TimeSpan timeOut, out object result, params object[] parameters)
        {
            if (IsStarted == false)
            {
                Start();
            }

            result = null;
            if (_blockingInvokeInProgress)
            {
                //SystemMonitor.Report("Another blocking invoke is already in progress.");
            }

            if (Thread.CurrentThread == _workerInternalThread)
            {// Invoke called from within the invoke thread, just execute directly to 
                // evade "locking" problem (one invoke spawning another, and the other waiting for invocation).
                result = delegateInstance.Method.Invoke(delegateInstance.Target, parameters);
                return true;
            }

            _blockingInvokeInProgress = true;
            
            InvocationData data;
            lock (this)
            {
                data = new InvocationData(delegateInstance, parameters);
                _pendingInvokes.Add(data);
            }

            if (timeOut == TimeSpan.MaxValue)
            {
                if (data.CompletedEvent.WaitOne() == false)
                {
                    return false;
                }
            }
            else
            {
                if (data.CompletedEvent.WaitOne(timeOut) == false)
                {
                    return false;
                }
            }
            
            _blockingInvokeInProgress = false;
            result = data.Result;

            return true;
        }

        /// <summary>
        /// Main operation thread.
        /// </summary>
        protected void Run()
        {
            GeneralHelper.AssignThreadCulture();

            while (_keepRunning)
            {
                try
                {
                    Application.DoEvents();
                    ProcessRequests();
                    Thread.Sleep(16);
                }
                catch (Exception ex)
                {
                    SystemMonitor.Error("Exception in messages processing thread [" + ex.Message + "]");
                }
            }
        }

        void ProcessRequests()
        {
            InvocationData data = null;
            do
            {
                lock (this)
                {
                    if (_pendingInvokes.Count > 0)
                    {
                        data = _pendingInvokes[0];
                        _pendingInvokes.RemoveAt(0);
                    }
                    else
                    {
                        data = null;
                        break;
                    }
                }

                try
                {
                    if (data != null)
                    {
                        //TracerHelper.Trace("Performing request.");
                        data.Perform();
                    }
                }
                catch (Exception ex)
                {
                    SystemMonitor.Error("Pending operation caused an exception [" + ex.Message + "].");
                }
            }
            while (_keepRunning && data != null);
        }

        public void Dispose()
        {
            Stop();

            ThreadPoolFast.StopThread(_workerInternalThread, true, 200, 500);

            //if (_workerInternalThread != null && _workerInternalThread.ThreadState == ThreadState.Running)
            //{
            //    // Allow a few ms for the thread to try and stop as it should.
            //    Thread.Sleep(500);

            //    if (_workerInternalThread.ThreadState == ThreadState.Running)
            //    {
            //        SystemMonitor.Warning("Aborting background message processing thread.");
            //        _workerInternalThread.Abort();
            //    }
            //}
        }
    }
}
