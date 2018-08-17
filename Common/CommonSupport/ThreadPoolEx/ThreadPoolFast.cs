using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace CommonSupport
{
    /// <summary>
    /// A very fast implementation of the thread pool.
    /// *WARNING* This class has been extremely optimized, line by line, so even very small changes can distrupt the fine
    /// threading model of execution. Upon doing any changes make sure to execute the Speed tests mutliple times, to be sure
    /// no damage has been dome.
    /// </summary>
    public class ThreadPoolFast : IDisposable
    {
        //const string DefaultThreadName = "ThreadPoolEx.Thread";

        /// <summary>
        /// Internal data storage class - for a running thread.
        /// </summary>
        class ThreadInfo
        {
            internal volatile int ThreadId;
            internal volatile Thread Thread;
            internal bool MustDispose = false;
            internal AutoResetEvent Event = new AutoResetEvent(false);
        }

        /// <summary>
        /// Internal data storage class - for a queued thread entity.
        /// </summary>
        public class TargetInfo
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            public TargetInfo(string invokerName, object target, FastInvokeHelper.FastInvokeHandlerDelegate delegateInstance,
                bool poolAsFirstParameter, ThreadPoolFast pool, params object[] args)
            {
                DelegateInstance = delegateInstance;
                Target = target;
                InvokerName = invokerName;

                if (poolAsFirstParameter)
                {
                    Args = new object[] { pool, args };
                }
                else
                {
                    Args = args;
                }

                //if (args != null && args.Length == 1)
                //{// Single parameter pass.
                //    Args = new object[] { pool, args[0] };
                //}
                //else
                //{
                //    Args = new object[] { pool, args };
                //}
            }

            public object Invoke()
            {
                return DelegateInstance(Target, Args);
            }

            readonly object Target;
            readonly string InvokerName;
            readonly FastInvokeHelper.FastInvokeHandlerDelegate DelegateInstance;
            readonly object[] Args;
        }

        #region Statitics

        int _totalThreadsAwakens = 0;
        /// <summary>
        /// 
        /// </summary>
        public int TotalThreadsAwakens
        {
            get { return _totalThreadsAwakens; }
            set { _totalThreadsAwakens = value; }
        }

        int _totalThreadsStarted = 0;
        /// <summary>
        /// 
        /// </summary>
        public int TotalThreadsStarted
        {
            get { return _totalThreadsStarted; }
        }

        #endregion

        InstanceMonitor _instanceSystem = new InstanceMonitor();
        /// <summary>
        /// 
        /// </summary>
        public InstanceMonitor InstanceSystem
        {
            get { return _instanceSystem; }
        }

        volatile bool _running = true;
        protected bool IsRunning
        {
            get
            {
                return _running && GeneralHelper.ApplicationClosing == false;
            }
        }

        TimeSpan _threadIdle = TimeSpan.FromSeconds(25);
        /// <summary>
        /// How long a thread waits for new tasks before going away.
        /// </summary>
        public TimeSpan ThreadIdle
        {
            get { return _threadIdle; }
            set { _threadIdle = value; }
        }

        volatile string _name = string.Empty;
        /// <summary>
        /// Name of this thread pool.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Total threads (running, sleeping, suspended, etc.)
        /// </summary>
        volatile int _maximumTotalThreadsAllowed = 20;
        public int MaximumThreadsCount
        {
            get { return _maximumTotalThreadsAllowed; }
            set { _maximumTotalThreadsAllowed = value; }
        }

        volatile int _minimumThreadsCount = 1;
        /// <summary>
        /// How many threads should be kept always ready and 
        /// alive waiting for pending tasks to come.
        /// This will increase the thread count in general, but
        /// provide a faster responce when a task comes in.
        /// Typical speed values: 
        /// - new thread = 0.2ms
        /// - existing thread = 0.05ms
        /// </summary>
        public int MinimumThreadsCount
        {
            get { return _minimumThreadsCount; }
            set { _minimumThreadsCount = value; }
        }

        volatile ApartmentState _threadsApartmentState = ApartmentState.STA;
        /// <summary>
        /// The default ApartmentState to use for the threads.
        /// </summary>
        public ApartmentState ThreadsApartmentState
        {
            get { return _threadsApartmentState; }
            set { _threadsApartmentState = value; }
        }

        /// <summary>
        /// Number of thread slots available.
        /// </summary>
        public int ActiveRunningThreadsCount
        {
            get
            {
                return Math.Max(0, _threadsHotSwap.Count - _sleepingThreads.Count);
            }
        }

        public int SleepingThreadsCount
        {
            get
            {
                return _sleepingThreads.Count;
            }
        }

        /// <summary>
        /// Number of thread slots available.
        /// </summary>
        public int FreeThreadsCount
        {
            get
            {
                return Math.Max(0, _maximumTotalThreadsAllowed - ActiveRunningThreadsCount);
            }
        }

        public int QueuedItemsCount
        {
            get { return _queue.Count; }
        }

        int _finalDisposeTimeoutMilliseconds = 15000;

        /// <summary>
        /// Creating a new threads takes about 100-200ms, so this helps to prevent system from creation overload.
        /// </summary>
        long _minimumThreadCreationIntervalMilliseconds = 500;

        /// <summary>
        /// ManagedThreadNumber vs ThreadInfo.
        /// </summary>
        volatile Dictionary<int, ThreadInfo> _threadsHotSwap = new Dictionary<int, ThreadInfo>();

        Stack<ThreadInfo> _sleepingThreads = new Stack<ThreadInfo>();

        long _lastQueueItemProcessedMillisecond = 0;

        long _lastThreadCreatedMillisecond = 0;

        /// <summary>
        /// *Reserving* space for the list here, makes it VERY VERY MUCH *SLOWER*, since all the reserved items are moved, each
        /// time we do an insert or remove, SO DO NOT DO IT.
        /// </summary>
        protected Queue<TargetInfo> _queue = new Queue<TargetInfo>();

        protected AutoResetEvent _queueProcessEvent = new AutoResetEvent(false);

        Thread _queueProcessorThread;

        #region Instance Control

        /// <summary>
        /// Constructor.
        /// </summary>
        public ThreadPoolFast(string name)
        {
            _name = name;
            GeneralHelper.ApplicationClosingEvent += new GeneralHelper.DefaultDelegate(GeneralHelper_ApplicationClosingEvent);

            _queueProcessorThread = new Thread(new ThreadStart(QueueProcessor));
            _queueProcessorThread.Name = name + ".QueueProcessor";
            _queueProcessorThread.Start();
        }

        /// <summary>
        /// Helper, performs common actions on stopping a still running thread.
        /// </summary>
        /// <param name="thread"></param>
        public static void StopThread(Thread thread, bool systemMonitorReport, int preInterruptTimeout, int preAbortTimeout)
        {
            if (thread == null)
            {
                return;
            }

            if (thread.ThreadState != System.Threading.ThreadState.Running
                && thread.ThreadState != System.Threading.ThreadState.WaitSleepJoin)
            {
                return;
            }

            if (preInterruptTimeout > 0)
            {
                Thread.Sleep(preInterruptTimeout);
            }

            if (thread.ThreadState != System.Threading.ThreadState.Running
                && thread.ThreadState != System.Threading.ThreadState.WaitSleepJoin)
            {
                return;
            }

            if (systemMonitorReport)
            {
                SystemMonitor.OperationWarning(string.Format("Interrupting  thread [{0}, {1}].", thread.ManagedThreadId, thread.Name));
            }

            // Will awaken, if asleep, or cause exception if goes to sleep.
            thread.Interrupt();
            if (preAbortTimeout > 0)
            {
                Thread.Sleep(preAbortTimeout);
            }

            if (thread.ThreadState != System.Threading.ThreadState.Running
                && thread.ThreadState != System.Threading.ThreadState.WaitSleepJoin)
            {
                return;
            }

            if (systemMonitorReport)
            {
                SystemMonitor.OperationWarning(string.Format("Aborting thread [{0}, {1}].", thread.ManagedThreadId, thread.Name));
            }

            thread.Abort();
        }
        
        public void Dispose()
        {
        }

        /// <summary>
        /// Free all threads, both asleep, and those that do not wish to end peacefully.
        /// Also stop the queue processor only in case it is still running.
        /// </summary>
        protected void Dispose(bool disposeQueueProcessor)
        {
            _running = false;

            if (_sleepingThreads.Count > 0)
            {
                lock (_sleepingThreads)
                {
                    while (_sleepingThreads.Count > 0)
                    {// Wake up all sleeping threads and kill them.
                        ThreadInfo info = _sleepingThreads.Pop();
                        info.MustDispose = true;
                        info.Event.Set();
                    }
                }
            }

            Stopwatch disposeWatch = new Stopwatch();
            disposeWatch.Start();

            if (_threadsHotSwap.Count > 0)
            {
                lock (this)
                {
                    Dictionary<int, ThreadInfo> threadsHotSwap = new Dictionary<int, ThreadInfo>(_threadsHotSwap);
                    while (threadsHotSwap.Count > 0)
                    {
                        Dictionary<int, ThreadInfo>.ValueCollection.Enumerator enumerator = threadsHotSwap.Values.GetEnumerator();
                        if (enumerator.MoveNext() == false)
                        {
                            return;
                        }

                        Thread thread = enumerator.Current.Thread;
                        if (thread.ThreadState == System.Threading.ThreadState.Running
                            || thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                        {
                            // Some thread is still working, see if we can wait any further.
                            if (_finalDisposeTimeoutMilliseconds > disposeWatch.ElapsedMilliseconds)
                            {// Continue waiting for some more time.
                                Thread.Sleep(500);
                                continue;
                            }
                        }

                        StopThread(thread, false, 0, 500);

                        threadsHotSwap.Remove(thread.ManagedThreadId);
                    }

                    _threadsHotSwap = threadsHotSwap;

                    // Execute under the lock.
                    if (disposeQueueProcessor && _queueProcessorThread != null)
                    {
                        // Finally stop the queue processor in case it is still running.
                        StopThread(_queueProcessorThread, false, 500, 500);
                        _queueProcessorThread = null;
                    }
                }
            }
        }

        void GeneralHelper_ApplicationClosingEvent()
        {
            GeneralHelper.ApplicationClosingEvent -= new GeneralHelper.DefaultDelegate(GeneralHelper_ApplicationClosingEvent);
            Dispose();

            TracerHelper.TraceSimple(TracerItem.TypeEnum.Report, "Thread pool fast GeneralHelper_ApplicationClosingEvent");
        }

        #endregion

        #region Input

        /// <summary>
        /// Enqueue a target and Fast Invoke delegate instance for execution.
        /// *IMPORTANT* make sure to store the delegateInstance and reuse it over multiple calls!
        /// </summary>
        public void QueueFastDelegate(object target, FastInvokeHelper.FastInvokeHandlerDelegate delegateInstance,
            params object[] args)
        {
            QueueFastDelegate(target, false, delegateInstance, args);
        }

        /// <summary>
        /// Enqueue a target and Fast Invoke delegate instance for execution.
        /// *IMPORTANT* make sure to store the delegateInstance and reuse it over multiple calls!
        /// </summary>
        public void QueueFastDelegate(object target, bool poolAsFirstParameter, FastInvokeHelper.FastInvokeHandlerDelegate delegateInstance, 
            params object[] args)
        {
            ThreadPoolFastEx.TargetInfo targetInfo = new ThreadPoolFastEx.TargetInfo(string.Empty,
                target, delegateInstance, poolAsFirstParameter, this, args);

            QueueTargetInfo(targetInfo);
        }

        /// <summary>
        /// Enqueue a fully assigned target info item for execution.
        /// </summary>
        protected void QueueTargetInfo(TargetInfo info)
        {
            if (IsRunning == false)
            {
                return;
            }

            lock (_queue)
            {
                _queue.Enqueue(info);
            }

            int activeRunningThreadsCount = ActiveRunningThreadsCount;
            bool notEnoughRunning = activeRunningThreadsCount < MinimumThreadsCount;
            if (activeRunningThreadsCount == 0 || notEnoughRunning)
            {
                _queueProcessEvent.Set();
            }
        }

        #endregion

        /// <summary>
        /// Routine running the queue processor thread.
        /// </summary>
        void QueueProcessor()
        {
            GeneralHelper.AssignThreadCulture();

            try
            {
                while (IsRunning)
                {
                    _queueProcessEvent.WaitOne(1);
                    ProcessThreads();
                }

                //lock (this)
                //{// Make self null, since otherwise the dispose will try to shut us down
                //    // while we are executing on it (shut ourselves).
                //    _queueProcessorThread = null;
                //}

                // Dispose, since in cases where the ApplicationClosingEvent is not raised
                // the pools threads will remain active.
                Dispose(false);
            }
            catch (Exception)
            {// Not much we can do here.
            }
        }

        /// <summary>
        /// Helper, process the items gathered in the execution queue.
        /// </summary>
        void ProcessThreads()
        {
            if (_queue.Count != 0)
            {
                int queueSize = _queue.Count;

                int awaken = 0;
                while (awaken < queueSize)
                {
                    if (AwakeSleepingThread() != null)
                    {
                        awaken++;
                        //break;
                    }
                    else
                    {// No more sleeping threads.
                        break;
                    }
                }

                if (awaken == 0)
                {// Running threads are below limit and nobody is sleeping, so run a new one.
                    CreateThread();
                }
            }
            else if (SleepingThreadsCount > 0 
                && _threadIdle.TotalMilliseconds < GeneralHelper.ApplicationStopwatchMilliseconds - _lastQueueItemProcessedMillisecond
                && _threadsHotSwap.Count > MinimumThreadsCount)
            {// Thread sleep timeout (execute only on timeout, and when combined threads count above minimum required).
                
                ThreadInfo info = null;
                lock (_sleepingThreads)
                {
                    if(_sleepingThreads.Count > 0)
                    {
                        info = _sleepingThreads.Pop();
                        info.MustDispose = true;
                        info.Event.Set();
                    }
                }

                if (info != null)
                {
                    RemoveThread(info.Thread);

                    // Finally awaken.
                    info.Event.Set();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ThreadInfo AwakeSleepingThread()
        {
            if (_sleepingThreads.Count == 0)
            {
                return null;
            }

            //if (ActiveRunningThreadsCount >= MaximumTotalThreadsAllowed)
            //{
            //    return null;
            //}

            ThreadInfo threadInfo = null;
            lock (_sleepingThreads)
            {
                if (_sleepingThreads.Count > 0)
                {
                    threadInfo = _sleepingThreads.Pop();
                }
            }

            if (threadInfo == null)
            {
                return null;
            }

            // Wake up the thread so it can do some work.
            threadInfo.Event.Set();
            
            Interlocked.Increment(ref _totalThreadsAwakens);

            return threadInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        ThreadInfo CreateThread()
        {
            if ((GeneralHelper.ApplicationStopwatchMilliseconds - _lastThreadCreatedMillisecond) < _minimumThreadCreationIntervalMilliseconds)
            {// Minimum inter thread creation time not met.
                return null;
            }

            if (IsRunning == false || _threadsHotSwap.Count >= MaximumThreadsCount)
            {
                return null;
            }

            _lastThreadCreatedMillisecond = GeneralHelper.ApplicationStopwatchMilliseconds;

            Thread newThread = new Thread(new ParameterizedThreadStart(ThreadExecute));
            newThread.SetApartmentState(_threadsApartmentState);
            newThread.Name = this._name + ".WorkerThread";

            ThreadInfo threadInfo;
            lock (this)
            {
                Dictionary<int, ThreadInfo> newThreads = new Dictionary<int, ThreadInfo>(_threadsHotSwap);
                threadInfo = new ThreadInfo() { Thread = newThread, ThreadId = newThread.ManagedThreadId };
                newThreads.Add(newThread.ManagedThreadId, threadInfo);

                // Hot Swap.
                _threadsHotSwap = newThreads;
            }

            Interlocked.Increment(ref _totalThreadsStarted);

            //newThread.Name = DefaultThreadName;
            newThread.Start(threadInfo);

            return threadInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        bool RemoveThread(Thread thread)
        {
            bool result;
            lock (this)
            {
                Dictionary<int, ThreadInfo> newThreads = new Dictionary<int, ThreadInfo>(_threadsHotSwap);
                result = newThreads.Remove(thread.ManagedThreadId);

                if (result)
                {
                    // Hot Swap.
                    _threadsHotSwap = newThreads;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        void ThreadExecute(object threadInfoParam)
        {
            ThreadInfo threadInfo = threadInfoParam as ThreadInfo;

            GeneralHelper.AssignThreadCulture();

            while (IsRunning)
            {
                TargetInfo targetInfo = null;
                if (_queue.Count != 0)
                {
                    lock (_queue)
                    {
                        if (_queue.Count > 0)
                        {
                            targetInfo = _queue.Dequeue();
                        }
                    }

                    Interlocked.Exchange(ref _lastQueueItemProcessedMillisecond, GeneralHelper.ApplicationStopwatchMilliseconds);
                }

                if (targetInfo == null)
                {
                    lock (_sleepingThreads)
                    {
                        // Keep this locked.
                        if (IsRunning == false)
                        {// Do not enter sleeping mode, if we are already stopped.
                            return;
                        }
                        _sleepingThreads.Push(threadInfo);
                    }

                    threadInfo.Event.WaitOne();

                    if (threadInfo.MustDispose)
                    {// Instructed to dispose.
                        return;
                    }
                }
                else
                {
                    try
                    {
                        object invokeResult = targetInfo.Invoke();
                    }
                    catch (Exception ex)
                    {
                        InstanceSystem.OperationError(SystemMonitor.ProcessExceptionMessage("[" + _name + "] Thread executed caused an exception ", ex));
                    }
                }
            }
        }
    }
}
