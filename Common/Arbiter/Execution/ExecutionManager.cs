using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using CommonSupport;
using System.Reflection;

namespace Arbiter
{
    /// <summary>
    /// To resolve the danger of getting stuck, the Execution manager will provide 
    /// new threads if the majority of the current thread have gone asleep.
    /// A new member - global threads maximum will be assigned.
    /// </summary>
    public class ExecutionManager : IDisposable
    {
        volatile Arbiter _arbiter;
        public Arbiter Arbiter
        {
            get { return _arbiter; }
        }

        /// <summary>
        /// The last time a requestMessage of all executioners busy was shown.
        /// </summary>
        //DateTime _executionersBusyWarningShownTime = DateTime.MinValue;
        DateTime _maxPendingItemsWarningShownTime = DateTime.MinValue;

        TimeSpan _warningsTimeSpan = TimeSpan.FromSeconds(30);
        /// <summary>
        /// The minimum time interval between showing 2 warning messages of all executioners busy.
        /// </summary>
        public TimeSpan AllExecutionersBusyWarningTimeSpan
        {
            get { lock (this) { return _warningsTimeSpan; } }
            set { lock (this) { _warningsTimeSpan = value; } }
        }

        public int MaxExecutionersAllowed
        {
            get { return _threadPool.MaximumThreadsCount; }
            set { _threadPool.MaximumThreadsCount = value; }
        }

        volatile int _maxPendingExecutionItems = 1000;
        /// <summary>
        /// Any item pending above this number will be discarded and a warning shown and recorded.
        /// </summary>
        public int MaxPendingExecutionItems
        {
            get { return _maxPendingExecutionItems; }
            set { _maxPendingExecutionItems = value; }
        }

        volatile int _maxExecutionersPerEntity = 10;
        public int MaxExecutionersPerEntity
        {
            get { return _maxExecutionersPerEntity; }
            set { _maxExecutionersPerEntity = value; }
        }

        volatile List<ExecutionEntity> _pendingEntities = new List<ExecutionEntity>();

        ThreadPoolFastEx _threadPool = null;
        
        Dictionary<ArbiterClientId, int> _clientsRunningExecutioners = new Dictionary<ArbiterClientId, int>();

        TimeOutMonitor _timeOutMonitor = new TimeOutMonitor();

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisposed
        {
            get { return _timeOutMonitor == null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ExecutionManager(Arbiter arbiter)
        {
            _arbiter = arbiter;
            _threadPool = new ThreadPoolFastEx(typeof(ExecutionManager).Name);
            //_threadPool.MaximumSimultaniouslyRunningThreadsAllowed = 20;
            
            // Clear them promptly, since otherwise we seem to get stuck.
            _threadPool.ThreadIdle = TimeSpan.FromSeconds(1);

            _threadPool.MaximumThreadsCount = 25;

            _timeOutMonitor.EntityTimedOutEvent += new HandlerDelegate<TimeOutable>(_timeOutMonitor_EntityTimedOutEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ThreadPoolFastEx pool = _threadPool;
            if (pool != null)
            {
                pool.Dispose();
            }

            lock (this)
            {
                _pendingEntities.Clear();
                _pendingEntities = null;

                //_executionersAndClientIDs.Clear();
                _clientsRunningExecutioners.Clear();

                _timeOutMonitor.Dispose();
                _timeOutMonitor = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter0"></param>
        void _timeOutMonitor_EntityTimedOutEvent(TimeOutable parameter0)
        {
            SystemMonitor.Error("_timeOutMonitor_EntityTimedOutEvent");
            ExecutionEntity entity = (ExecutionEntity)parameter0;
            entity.Conversation.EntityTimedOut(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void AddExecutionEntity(ExecutionEntity entity)
        {
            //TracerHelper.TraceEntry();
            lock (this)
            {
                if (_pendingEntities.Count > _maxPendingExecutionItems)
                {
                    TracerHelper.TraceError("Too many pending entities in system. Some older entities are being dropped.");
                    if ((DateTime.Now - _maxPendingItemsWarningShownTime) > _warningsTimeSpan)
                    {
                        _maxPendingItemsWarningShownTime = DateTime.Now;
                        SystemMonitor.Error("Too many pending entities in system. Some older entities are being dropped.");
                    }

                    // Loose the oldest entity in line.
                    _timeOutMonitor.RemoveEntity(_pendingEntities[0]);
                    _pendingEntities.RemoveAt(0);
                }

                _timeOutMonitor.AddEntity(entity);
                _pendingEntities.Add(entity);
            }

            // Continue execution chain.
            UpdatePendingExecution();
        }

        /// <summary>
        /// Obtain the next execution entity and remove it from pending.
        /// Children to override to manage the way operations are performed, if they need to.
        /// This is where currently the MultiThreads per client protection is performed.
        /// </summary>
        protected virtual ExecutionEntity PopNextExecutionEntity()
        {
            lock (this)
            {
                // Check total executioners count.
                if (_threadPool.FreeThreadsCount < 2)
                {
                    //if ((DateTime.Now - _executionersBusyWarningShownTime) >= _warningsTimeSpan)
                    {
                        SystemMonitor.OperationError("All of the [" + _threadPool.MaximumThreadsCount + "] arbiter [" + _arbiter.Name + "] executioners are busy, entity execution delayed.", TracerItem.PriorityEnum.Medium);
                        //_executionersBusyWarningShownTime = DateTime.Now;
                    }

                    return null;
                }

                // List of IDs we tried to execute upon already, and presumably failed.
                List<ArbiterClientId> processedIds = new List<ArbiterClientId>();

                // Try looking for a new entity.
                for (int i = 0; i < _pendingEntities.Count; i++)
                {// Look for an entity that we are allowed to execute now.

                    ExecutionEntity entity = _pendingEntities[i];
                    IArbiterClient iClient = _arbiter.GetClientByID(entity.ReceiverID);

                    if (processedIds.Contains(entity.ReceiverID))
                    {// Alredy tried on this entity, skip.
                        continue;
                    }
                    else
                    {// First time we see this entity.
                        processedIds.Add(entity.ReceiverID);
                    }

                    bool isTransportResponseMessage = (entity.Message is TransportMessage &&
                            ((TransportMessage)(entity.Message)).IsRequest == false);

                    int runningOnClient = 0;
                    if (_clientsRunningExecutioners.ContainsKey(entity.ReceiverID))
                    {
                        runningOnClient = _clientsRunningExecutioners[entity.ReceiverID];
                    }

                    if (isTransportResponseMessage == false && iClient != null 
                        && iClient.SingleThreadMode && runningOnClient > 0/*_executionersAndClientIDs.ContainsValue(entity.ReceiverID)*/)
                    {// We are not allowed to run this, as there is already an executioner there.
                        // And it is not a responce requestMessage.
                        continue;
                    }

                    if (runningOnClient >= _maxExecutionersPerEntity)
                    {// This entity is already consuming too many threads. It must release some before using more.
                        //if ((DateTime.Now - _executionersBusyWarningShownTime) >= _warningsTimeSpan)
                        {
                            //_executionersBusyWarningShownTime = DateTime.Now;
                            SystemMonitor.OperationError("An entity [" + entity.ReceiverID.Id.Name + "] is using all its threads allowed. Further entity executions will be delayed.", TracerItem.PriorityEnum.Medium);
                        }

                        continue;
                    }

                    //TraceHelper.Trace("PopNextExecutionEntity [" + iClient.ToString() + "][" + iClient.SingleThreadMode + "," + _executionersAndClientIDs.ContainsValue(entity.ReceiverID) + "][" + entity.Message.ToString() + "]");

                    _pendingEntities.Remove(entity);

                    // OK, this entity is good to go.
                    return entity;
                }

                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual void UpdatePendingExecution()
        {
            ExecutionEntity entity;

            while (_pendingEntities != null 
                && (entity = PopNextExecutionEntity()) != null)
            {
                TracerHelper.TraceEntry(" pending [" + _pendingEntities.Count + "] started executioners [" 
                    + _threadPool.ActiveRunningThreadsCount.ToString() + "]");

                ThreadPoolFastEx pool = _threadPool;
                if (pool != null)
                {
                    pool.Queue(new GeneralHelper.GenericDelegate<ExecutionEntity>(worker_DoWork), entity);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void worker_DoWork(ExecutionEntity entity)
        {
            lock (this)
            {
                if (_clientsRunningExecutioners.ContainsKey(entity.ReceiverID) == false)
                {
                    _clientsRunningExecutioners[entity.ReceiverID] = 1;
                }
                else
                {
                    _clientsRunningExecutioners[entity.ReceiverID] = _clientsRunningExecutioners[entity.ReceiverID] + 1;
                }
            }

            TracerHelper.Trace(" Enlisted entity at [" + entity.ReceiverID.Id.Name + "] entity starting [" + entity.Message.GetType().Name + ", " + 
                entity.ReceiverID.Id.Name + "], total count [" + _threadPool.ActiveRunningThreadsCount.ToString() + "]");

            DateTime startTime = DateTime.Now;

            // Notify executor we are running this entity.
            entity.Conversation.EntityExecutionStarted(entity);

            try
            {
                IArbiterClient receiver = _arbiter.GetClientByID(entity.ReceiverID);
                if (receiver != null)
                {
                    SystemMonitor.CheckError(((TransportMessage)entity.Message).TransportInfo.TransportInfoCount > 0);

                    // Do the entity.
                    if (entity is ExecutionEntityWithReply)
                    {
                        ExecutionEntityWithReply replyEntity = (ExecutionEntityWithReply)entity;
                        SystemMonitor.CheckError(replyEntity.ReplyMessage == null && replyEntity.ReplyTimeOut == TimeSpan.Zero);
                        receiver.ReceiveExecutionWithReply(replyEntity);
                    }
                    else
                    {
                        SystemMonitor.CheckError(entity.GetType() == typeof(ExecutionEntity));
                        receiver.ReceiveExecution(entity);
                    }
                    entity.Conversation.EntityExecutionFinished(entity);
                }
            }
            catch (TargetInvocationException exception)
            {
                if (exception.InnerException is ThreadInterruptedException)
                {// ThreadInterruptedException's are OK, since we use them to awake sleeping threads when closing down.
                    SystemMonitor.Report(exception.ToString() + "[" + exception.InnerException.Message + "]");
                    entity.Conversation.EntityExecutionFailed(entity, exception);
                }
                else
                {
                    SystemMonitor.OperationError(exception.ToString() + "[" + exception.InnerException.Message + "]");
                    entity.Conversation.EntityExecutionFailed(entity, exception);
                }
            }
            catch (ThreadInterruptedException exception)
            {
                // ThreadInterruptedException's are OK, since we use them to awake sleeping threads when closing down.
                SystemMonitor.Report(exception.ToString() + "[" + exception.Message + "]");
                entity.Conversation.EntityExecutionFailed(entity, exception);
            }
            catch (Exception exception)
            {
                SystemMonitor.Error(exception.ToString());
                entity.Conversation.EntityExecutionFailed(entity, exception);
            }
            finally
            {
                entity.Die();
            }

            lock (this)
            {
                if (_clientsRunningExecutioners.ContainsKey(entity.ReceiverID))
                {
                    int newClientsValue = _clientsRunningExecutioners[entity.ReceiverID] - 1;
                    if (newClientsValue <= 0)
                    {
                        _clientsRunningExecutioners.Remove(entity.ReceiverID);
                    }
                    else
                    {
                        _clientsRunningExecutioners[entity.ReceiverID] = newClientsValue;
                    }
                }
                else
                {
                    if (IsDisposed == false)
                    {
                        SystemMonitor.Error("ClientsRunningExecutioners not properly maintained.");
                    }
                }

                //TracerHelper.TraceExit("entity finished for [" + (DateTime.Now - startTime).Milliseconds + "]ms [" + entity.Message.GetType().Name + ", " + entity.ReceiverID.Id.Name + "], total count [" + _threadPool.ActiveRunningThreadsCount.ToString() + "]");
            }

            // Continue execution chain.
            UpdatePendingExecution();
        }

    }
}
