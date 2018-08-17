using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CommonSupport
{
    /// <summary>
    /// Gives some common predefined code for the implementation of the IOperational interface.
    /// </summary>
    [Serializable]
    public class Operational : IOperational, IDeserializationCallback
    {
        protected volatile OperationalStateEnum _operationalState = OperationalStateEnum.UnInitialized;
        
        /// <summary>
        /// The current operational state of the object.
        /// </summary>
        public OperationalStateEnum OperationalState
        {
            get { return _operationalState; }
        }

        volatile bool _statusSynchronizationEnabled = true;
        /// <summary>
        /// Is the status synchronization enabled.
        /// </summary>
        protected bool StatusSynchronizationEnabled
        {
            get { return _statusSynchronizationEnabled; }
            set { _statusSynchronizationEnabled = value; }
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

        [field: NonSerialized]
        public event OperationalStateChangedDelegate OperationalStateChangedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Operational()
        {
        }

        public virtual void OnDeserialization(object sender)
        {
            StatusSynchronizationSource = _statusSynchronizationSource;
        }

        /// <summary>
        /// Change the component operational state.
        /// </summary>
        /// <param name="operationalState"></param>
        public virtual void ChangeOperationalState(OperationalStateEnum operationalState)
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

            TracerHelper.Trace(this.GetType().Name + " was " + previousState.ToString() + " is now " + operationalState.ToString());

            _operationalState = operationalState;
            if (OperationalStateChangedEvent != null)
            {
                OperationalStateChangedEvent(this, previousState);
            }
        }

        /// <summary>
        /// Follow synchrnozation source status.
        /// </summary>
        void _statusSynchronizationSource_OperationalStatusChangedEvent(IOperational parameter1, OperationalStateEnum parameter2)
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
        /// Raise event helper.
        /// </summary>
        /// <param name="previousState"></param>
        protected void RaiseOperationalStatusChangedEvent(OperationalStateEnum previousState)
        {
            if (OperationalStateChangedEvent != null)
            {
                OperationalStateChangedEvent(this, previousState);
            }
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public static bool IsInitOrOperational(OperationalStateEnum state)
        {
            return state == OperationalStateEnum.Initialized || state == OperationalStateEnum.Operational;
        }

    }
}
