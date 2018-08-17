using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// 
    /// </summary>
    public class OperationPerformerStub
    {
        /// <summary>
        /// Start from 1 or more!
        /// </summary>
        int _operationID = 1;

        /// <summary>
        /// Operation custom ID and operation.
        /// </summary>
        Dictionary<string, OperationInformation> _pendingOperations = new Dictionary<string, OperationInformation>();

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<OperationInformation> PendingOperationsUnsafe
        {
            get { lock (this) { return _pendingOperations.Values; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<OperationInformation> PendingOperationsArray
        {
            get 
            {
                lock (this)
                {
                    return GeneralHelper.EnumerableToList(_pendingOperations.Values);
                }
            }
        }

        /// <summary>
        /// Interface defines methods for implementations of stub.
        /// </summary>
        public interface IImplementation
        {
            /// <summary>
            /// 
            /// </summary>
            bool StartOperation(OperationInformation operation);
        }

        TimeSpan _defaultTimeOut = TimeSpan.FromSeconds(30);

        public TimeSpan DefaultTimeOut
        {
            get { return _defaultTimeOut; }
            set { _defaultTimeOut = value; }
        }

        IImplementation _implementation;

        public delegate void OperationUpdateDelegate(OperationPerformerStub stub, OperationInformation operation);
        public event OperationUpdateDelegate OperationCompleteEvent;

        /// <summary>
        /// 
        /// </summary>
        public OperationPerformerStub(IImplementation implementation)
        {
            _implementation = implementation;
        }

        /// <summary>
        /// Stub can be used in standalone mode, with no implementation.
        /// </summary>
        public OperationPerformerStub()
        {
            _implementation = null;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        int GetNextOperationCustomID()
        {
            lock (this)
            {
                return _operationID++;
            }
        }

        string ImplementationName
        {
            get
            {
                if (_implementation != null)
                {
                    return _implementation.GetType().Name; 
                }
                return "-";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public OperationInformation GetOperationById(string id)
        {
            lock (this)
            {
                if (_pendingOperations.ContainsKey(id))
                {
                    return _pendingOperations[id];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool StartOperation(string operationId)
        {
            lock (this)
            {
                if (_pendingOperations.ContainsKey(operationId))
                {
                    return _pendingOperations[operationId].Start();
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CompleteOperation(string operationId, object result)
        {
            lock (this)
            {
                if (_pendingOperations.ContainsKey(operationId))
                {
                    _pendingOperations[operationId].Complete(result);
                    return true;
                }
            }

            // Timed out and was removed.
            SystemMonitor.OperationError("Operation response received [" + ImplementationName + ", " + result.GetType().Name + "], but request operation already timed out.");
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RegisterOperation(OperationInformation operationInfo, bool assignId)
        {
            if (assignId && string.IsNullOrEmpty(operationInfo.Id) == false)
            {
                SystemMonitor.Warning("Id of operation already assigned.");
                return false;
            }

            if (assignId)
            {
                operationInfo.Id = GetNextOperationCustomID().ToString();
            }

            lock (this)
            {
                if (_pendingOperations.ContainsKey(operationInfo.Id))
                {
                    SystemMonitor.Error("An operation for this order id already running.");
                    return false;
                }

                // Register now to be sure, that whenever responce and OrderResponce come they will be handled OK.
                _pendingOperations.Add(operationInfo.Id, operationInfo);
                operationInfo.OperationCompleteEvent += new OperationInformation.OperationUpdateDelegate(operationInfo_OperationCompleteEvent);
            }

            return true;
        }

        /// <summary>
        /// If implementation is null, it will only register the operation.
        /// </summary>
        public bool PlaceOperation(OperationInformation operationInfo, bool assignId)
        {
            if (RegisterOperation(operationInfo, assignId) == false)
            {
                return false;
            }

            if (_implementation != null && _implementation.StartOperation(operationInfo) == false)
            {
                SystemMonitor.OperationWarning("Operation [" + _implementation.GetType().Name + ", " + operationInfo.Id + "] failed to start.");
                return false;
            }

            return true;
        }

        void operationInfo_OperationCompleteEvent(OperationInformation operation)
        {
            lock (this)
            {
                // Remove operation from pending operation, timed out or not.
                _pendingOperations.Remove(operation.Id);
            }

            if (OperationCompleteEvent != null)
            {
                OperationCompleteEvent(this, operation);
            }
        }

        /// <summary>
        /// Helper baseMethod.
        /// </summary>
        public bool PerformOperation<ResultType>(OperationInformation operationInfo, TimeSpan? timeOut, bool assignId, out ResultType result)
            where ResultType : class
        {
            result = null;

            if (PlaceOperation(operationInfo, assignId) == false)
            {
                return false;
            }

            return operationInfo.WaitResult<ResultType>(timeOut.HasValue ? timeOut.Value : this.DefaultTimeOut, out result);
        }
    }
}
