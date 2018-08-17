using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonSupport
{
    /// <summary>
    /// Class implements basic bahviour for a time control.
    /// It can be inherited and used with baseMethod overriding or it can be
    /// controlled trough the events and methods.
    /// Class does not synchronize steps with slaves; only warnings are reported upon failure
    /// of a slave to step trough.
    /// </summary>
    [Serializable]
    public class TimeControl : ITimeControl
    {
        ListUnique<ITimeControl> _slaveControls = new ListUnique<ITimeControl>();

        volatile int _currentStep = 0;

        public delegate bool TimeControlStepUpdateDelegate(ITimeControl control);
        public delegate bool TimeControlStepsUpdateDelegate(ITimeControl control, int steps);
        public delegate ValueType TimeControlValueUpdateRequestDelegate<ValueType>(ITimeControl control);

        [field: NonSerialized]
        public event TimeControlValueUpdateRequestDelegate<bool> CanStepBackValueUpdateEvent;

        [field: NonSerialized]
        public event TimeControlValueUpdateRequestDelegate<bool> CanStepForwardValueUpdateEvent;

        [field: NonSerialized]
        public event TimeControlValueUpdateRequestDelegate<int?> CurrentStepValueUpdateEvent;

        [field: NonSerialized]
        public event TimeControlValueUpdateRequestDelegate<int?> TotalStepsValueUpdateEvent;

        [field: NonSerialized]
        public event TimeControlValueUpdateRequestDelegate<TimeSpan?> PeriodValueUpdateEvent;

        /// <summary>
        /// Events provide a way for a subscriber to control the behaviour of the control.
        /// </summary>
        [field: NonSerialized]
        public event TimeControlStepsUpdateDelegate StepForwardEvent;

        [field: NonSerialized]
        public event TimeControlStepsUpdateDelegate StepBackEvent;

        [field: NonSerialized]
        public event TimeControlStepsUpdateDelegate StepToEvent;

        [field: NonSerialized]
        public event TimeControlStepUpdateDelegate StepToEndEvent;

        [field: NonSerialized]
        public event TimeControlStepUpdateDelegate RestartEvent;

        /// <summary>
        /// Provide a thread safe array of slaves.
        /// </summary>
        protected ITimeControl[] SlaveControlsArray
        {
            get
            {
                lock (_slaveControls)
                {
                    return _slaveControls.ToArray();
                }
            }
        }

        [field: NonSerialized]
        public event ITimeControlStepUpdateDelegate CurrentStepChangedEvent;

        /// <summary>
        /// 
        /// </summary>
        public TimeControl()
        {
        
        }

        /// <summary>
        /// Change curret step to this new step.
        /// </summary>
        /// <param name="newStep"></param>
        /// <returns></returns>
        public void ChangeCurrentStep(int newStep)
        {
            _currentStep = newStep;
            if (CurrentStepChangedEvent != null)
            {
                CurrentStepChangedEvent(this);
            }
        }

        #region ITimeControl Members

        int? _totalStepsCount = 0;
        public virtual int? TotalStepsCount
        {
            get 
            {
                if (TotalStepsValueUpdateEvent != null)
                {
                    return TotalStepsValueUpdateEvent(this);
                }

                return _totalStepsCount; 
            }

            set 
            { 
                _totalStepsCount = value; 
            }
        }

        public virtual int? CurrentStep
        {
            get 
            {
                if (CurrentStepValueUpdateEvent != null)
                {
                    return CurrentStepValueUpdateEvent(this);
                }

                return _currentStep; 
            }
        }

        volatile bool _canRestart = true;
        public bool CanRestart 
        {
            get { return _canRestart; }
            set { _canRestart = value; }
        }

        volatile bool _canStepBack = true;
        public virtual bool CanStepBack
        {
            get 
            {
                if (CanStepBackValueUpdateEvent != null)
                {
                    return CanStepBackValueUpdateEvent(this);
                }
                return _canStepBack; 
            }
            set { _canStepBack = value; }
        }

        volatile bool _canStepForwards = true;
        public virtual bool CanStepForward
        {
            get 
            {
                if (CanStepForwardValueUpdateEvent != null)
                {
                    return CanStepForwardValueUpdateEvent(this);
                }

                return _canStepForwards;
            }
            set { _canStepForwards = value; }
        }

        TimeSpan? _period = null;
        public virtual TimeSpan? Period
        {
            get 
            {
                if (PeriodValueUpdateEvent != null)
                {
                    return PeriodValueUpdateEvent(this);
                }
                return _period; 
            }

            set { _period = value; }
        }


        public virtual ReadOnlyCollection<ITimeControl> SlaveControls
        {
            get { lock (_slaveControls) { return _slaveControls.AsReadOnly(); } }
        }

        public virtual bool AddSlaveControl(ITimeControl control)
        {
            lock (_slaveControls) { return _slaveControls.Add(control); }
        }

        public virtual bool RemoveSlaveControl(ITimeControl control)
        {
            lock (_slaveControls) { return _slaveControls.Remove(control); }
        }


        public virtual bool StepForward()
        {
            return StepForward(1);
        }

        public virtual bool StepForward(int steps)
        {
            if (TotalStepsCount < CurrentStep + steps)
            {
                return false;
            }

            if (StepForwardEvent != null && StepForwardEvent(this, steps) == false)
            {
                return false;
            }

            foreach (ITimeControl control in SlaveControlsArray)
            {
                if (control.StepForward(steps) == false)
                {
                    SystemMonitor.Warning("Slave control has failed to follow master time control.");
                }
            }

            ChangeCurrentStep(_currentStep + steps);
            return true;
        }

        public virtual bool StepBack()
        {
            return StepBack(1);
        }

        public virtual bool StepBack(int steps)
        {
            if (CanStepBack == false ||
                (StepBackEvent != null && StepBackEvent(this, steps) == false))
            {
                return false;
            }

            foreach (ITimeControl control in SlaveControlsArray)
            {
                if (control.StepBack(steps) == false)
                {
                    SystemMonitor.Warning("Slave control has failed to follow master time control.");
                }
            }

            ChangeCurrentStep(_currentStep - steps);
            return true;

        }

        public virtual bool StepTo(int index)
        {
            if (StepToEvent != null && StepToEvent(this, index) == false)
            {
               return false;
            }

            foreach (ITimeControl control in SlaveControlsArray)
            {
                if (control.StepTo(index) == false)
                {
                    SystemMonitor.Warning("Slave control has failed to follow master time control.");
                }
            }

            ChangeCurrentStep(index);
            return true;
        }

        public virtual bool StepToEnd()
        {
            if (StepToEndEvent != null && StepToEndEvent(this) == false)
            {
                return false;
            }

            foreach (ITimeControl control in SlaveControlsArray)
            {
                if (control.StepToEnd() == false)
                {
                    SystemMonitor.Warning("Slave control has failed to follow master time control.");
                }
            }

            if (TotalStepsCount.HasValue)
            {
                ChangeCurrentStep(TotalStepsCount.Value);
            }
            else
            {
                ChangeCurrentStep(int.MaxValue);
            }
            return true;
        }

        public virtual bool Restart()
        {
            if (CanRestart == false 
                || (RestartEvent != null && RestartEvent(this) == false))
            {
                return false;
            }

            foreach (ITimeControl control in SlaveControlsArray)
            {
                if (control.Restart() == false)
                {
                    SystemMonitor.Warning("Slave control has failed to follow master time control.");
                }
            }

            ChangeCurrentStep(0);
            return true;
        }

        #endregion
    }
}
