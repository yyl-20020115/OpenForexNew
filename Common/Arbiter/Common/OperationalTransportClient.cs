using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// A transport client that has given operation states.
    /// The state is not persisted and always starts from NotOperational.
    /// </summary>
    [Serializable]
    public class OperationalTransportClient : TransportClient, IOperational
    {
        volatile OperationalStateEnum _operationalState = OperationalStateEnum.Unknown;
        public OperationalStateEnum OperationalState
        {
            get { return _operationalState; }
        }

        /// <summary>
        /// Is the OperationalState == Operational.
        /// </summary>
        public bool IsOperational
        {
            get { return _operationalState == OperationalStateEnum.Operational; }
        }

        volatile bool _statusSynchronizationEnabled = true;
        protected bool StatusSynchronizationEnabled
        {
            get { return _statusSynchronizationEnabled; }
            
            set 
            { 
                _statusSynchronizationEnabled = value;
                TracerHelper.TraceEntry(this.GetType().Name + ", Synchronization enabled: " + value.ToString());
            }
        }

        volatile IOperational _statusSynchronizationSource = null;

        /// <summary>
        /// If the current unit must synchronize status with a given source, set this variable
        /// and the synchronization is done automatically.
        /// </summary>
        protected IOperational StatusSynchronizationSource
        {
            get { return _statusSynchronizationSource; }
            set
            {
                if (value != null)
                {
                    TracerHelper.TraceEntry(this.GetType().Name + ", Synchronization source assigned: " + value.GetType().Name);
                }
                else
                {
                    TracerHelper.TraceEntry(this.GetType().Name + ", Synchronization source assigned: null");
                }

                if (_statusSynchronizationSource != null)
                {
                    _statusSynchronizationSource.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(_statusSynchronizationSource_OperationalStatusChangedEvent);
                    _statusSynchronizationSource = null;
                }

                _statusSynchronizationSource = value;
                if (_statusSynchronizationSource != null)
                {
                    _statusSynchronizationSource.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_statusSynchronizationSource_OperationalStatusChangedEvent);

                    if (_operationalState != _statusSynchronizationSource.OperationalState)
                    {
                        ChangeOperationalState(_statusSynchronizationSource.OperationalState);
                    }
                }
            }
        }

        volatile TransportInfo _remoteStatusSynchronizationSource = null;
        
        /// <summary>
        /// Allows to follow the states of a remote operational client.
        /// </summary>
        public TransportInfo RemoteStatusSynchronizationSource
        {
            get { return _remoteStatusSynchronizationSource; }
        }

        TransportInfo _persistedRemoteStatusSynchronizationSource = null;

        /// <summary>
        /// Subsribers receiving notifications of status changes of current client.
        /// </summary>
        List<TransportInfo> _operationStateChangeSubscribers = new List<TransportInfo>();

        public event OperationalStateChangedDelegate OperationalStateChangedEvent;

        public OperationalTransportClient()
          :this(false)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleThreadMode"></param>
        public OperationalTransportClient(bool singleThreadMode)
            : base(singleThreadMode)
        {
            this.DefaultTimeOut = TimeSpan.FromSeconds(20);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleThreadMode"></param>
        public OperationalTransportClient(string name, bool singleThreadMode)
            : base(name, singleThreadMode)
        {
            this.DefaultTimeOut = TimeSpan.FromSeconds(20);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleThreadMode"></param>
        public OperationalTransportClient(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _statusSynchronizationEnabled = info.GetBoolean("statusSynchronizationEnabled");
            _persistedRemoteStatusSynchronizationSource = (TransportInfo)info.GetValue("remoteStatusSynchronizationSource", typeof(TransportInfo));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("statusSynchronizationEnabled", _statusSynchronizationEnabled);
            info.AddValue("remoteStatusSynchronizationSource", _remoteStatusSynchronizationSource);
            base.GetObjectData(info, context);
        }

        public override bool ArbiterInitialize(Arbiter arbiter)
        {
            if (base.ArbiterInitialize(arbiter))
            {
                if (_persistedRemoteStatusSynchronizationSource != null)
                {
                    SetRemoteStatusSynchronizationSource(_persistedRemoteStatusSynchronizationSource);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Follow synchrnozation source status.
        /// </summary>
        protected virtual void _statusSynchronizationSource_OperationalStatusChangedEvent(IOperational parameter1, OperationalStateEnum parameter2)
        {
            if (StatusSynchronizationEnabled)
            {
                TracerHelper.Trace(this.GetType().Name + " is following its source " + parameter1.GetType().Name + " to state " + parameter1.OperationalState.ToString());
                this.ChangeOperationalState(parameter1.OperationalState);
            }
            else
            {
                TracerHelper.Trace(this.GetType().Name + " is not following its source " + parameter1.GetType().Name + " to new state because synchronization is disabled.");
            }
        }
        
        /// <summary>
        /// Change the component operational state.
        /// </summary>
        /// <param name="operationalState"></param>
        public void ChangeOperationalState(OperationalStateEnum operationalState)
        {
            OperationalStateEnum previousState;
            lock (this)
            {
                if (operationalState == _operationalState)
                {
                    return;
                }

                previousState = _operationalState;
            }

            TracerHelper.Trace(this.GetType().Name + " is now " + operationalState.ToString() + " has [" + _operationStateChangeSubscribers.Count + "] subscribers.");

            _operationalState = operationalState;
            if (OperationalStateChangedEvent != null)
            {
                OperationalStateChangedEvent(this, previousState);
            }

            // Send to monitoring subscribers.
            lock (this)
            {
                foreach (TransportInfo info in _operationStateChangeSubscribers)
                {
                    TracerHelper.Trace("Sending operational state [" + operationalState.ToString() + "] to [" + info.OriginalSenderId.Value.Id.Print() + "].");
                    this.SendResponding(info, new ChangeOperationalStateMessage(this.OperationalState, false));
                }
            }
        }

        /// <summary>
        /// Will unsubscribe to previous one.
        /// </summary>
        protected bool SetRemoteStatusSynchronizationSource(TransportInfo sourceTransportInfo)
        {
            lock (this)
            {
                if (_remoteStatusSynchronizationSource != null)
                {
                    SubscribeToOperationalStateChangesMessage message = new SubscribeToOperationalStateChangesMessage(false);
                    message.RequestResponse = false;
                    SendResponding(_remoteStatusSynchronizationSource, message);
                }

                _remoteStatusSynchronizationSource = sourceTransportInfo;
            }

            bool result = true;
            if (sourceTransportInfo != null)
            {
                ResponseMessage response = SendAndReceiveResponding<ResponseMessage>(sourceTransportInfo, 
                    new SubscribeToOperationalStateChangesMessage(true));

                result = response != null && response.OperationResult;
            }

            TracerHelper.TraceEntry(this.GetType().Name + ", Remote synchronization source " + sourceTransportInfo.OriginalSenderId.Value.Id.Name + " assinged - " + result.ToString());
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        [MessageReceiver]
        ResponseMessage Receive(SubscribeToOperationalStateChangesMessage message)
        {

            bool result = false;
            lock(this)
            {
                if (message.Subscribe)
                {// Subscribe.
                    
                    TracerHelper.TraceEntry("Subscribing - " + message.TransportInfo.OriginalSenderId.Value.Id.Name);

                    if (_operationStateChangeSubscribers.Contains(message.TransportInfo) == false)
                    {
                        _operationStateChangeSubscribers.Add(message.TransportInfo);
                        result = true;
                    }

                    // Send an initial notification.
                    this.SendResponding(message.TransportInfo, new ChangeOperationalStateMessage(this.OperationalState, false));
                }
                else
                {// Unsubscribe.
                    TracerHelper.TraceEntry("Unsubscribing - " + message.TransportInfo.OriginalSenderId.Value.Id.Name);
                    result = _operationStateChangeSubscribers.Remove(message.TransportInfo);
                }
            }

            if (message.RequestResponse)
            {
                return new ResponseMessage(result);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [MessageReceiver]
        void Receive(ChangeOperationalStateMessage message)
        {// Result can be returned to requestor.
            TracerHelper.TraceEntry();

            // Make sure to compare only the *original senders* as the rest is not guaranteed to be the same,
            // since remote status synchronization source is fed to class from outside.
            if (message.IsRequest == false)
            {// This is a notification
                if (message.TransportInfo.OriginalSenderId.Equals(_remoteStatusSynchronizationSource.OriginalSenderId))
                {// Message received from status synch source, change state to synchronize.
                    if (StatusSynchronizationEnabled)
                    {
                        TracerHelper.Trace(this.GetType().Name + " is following its status synchronization source to new state [" + message.OperationalState.ToString() + "].");
                        ChangeOperationalState(message.OperationalState);
                    }
                    else
                    {
                        TracerHelper.Trace(this.GetType().Name + " is not following its status synchronization source to new state because synchronization is disabled.");
                    }
                }
                else
                {
                    SystemMonitor.Warning("Stat change notification received, but not from status source. Ignored.");
                }
            }
            else
            {
                TracerHelper.Trace(this.GetType().Name + " is following request from " + message.TransportInfo.CurrentTransportInfo.Value.SenderID.Value.Id.Name + " to " + message.OperationalState);
                
                bool result = OnChangeOperationalStateRequest(message.OperationalState);
                SystemMonitor.CheckWarning(result, "Component [" + this.Name + "] has not changed its operational state upon request.");
            }
        }

        /// <summary>
        /// A request to change state has been received.
        /// </summary>
        /// <param name="newState"></param>
        protected virtual bool OnChangeOperationalStateRequest(OperationalStateEnum newState)
        {
            return false;
        }

    }
}
