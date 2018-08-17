using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Class manages the operation of system variables.
    /// </summary>
    public class SystemVariables
    {
        volatile bool _enabled = true;
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        volatile bool _showOriginatingClassNames = false;
        /// <summary>
        /// Should the names of the System variables contain the names of their originative classes.
        /// </summary>
        public bool ShowOriginatingClassNames
        {
            get { return _showOriginatingClassNames; }
            set { _showOriginatingClassNames = value; }
        }

        DateTime _lastUpdateTime = DateTime.MinValue;

        TimeSpan _updateEventInvocationTimeout = TimeSpan.FromSeconds(1);
        /// <summary>
        /// The minimal timeout between the invokes of two consecutive VariableUpdateEvent.
        /// VariableUpdateEvent is not guaranteed to (shall not) be called on each update.
        /// </summary>
        public TimeSpan UpdateEventInvocationTimeout
        {
            get { return _updateEventInvocationTimeout; }
            set { _updateEventInvocationTimeout = value; }
        }

        Dictionary<string, double?> _systemVariables = new Dictionary<string, double?>();

        /// <summary>
        /// Obtain the value of a system variable.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double? this[string key]
        {
            get
            {
                if (_enabled == false)
                {
                    return null;
                }

                lock (_systemVariables)
                {
                    if (_systemVariables.ContainsKey(key))
                    {
                        return _systemVariables[key];
                    }
                }

                return null;
            }

            set
            {
                if (_enabled == false)
                {
                    return;
                }

                lock (_systemVariables)
                {
                    if (_systemVariables.ContainsKey(key))
                    {
                        _systemVariables[key] = value;
                    }
                    else
                    {
                        _systemVariables.Add(key, value);
                    }
                }

                RaiseVariablesUpdatedEvent();
            }
        }

        public delegate void VariablesUpdatedDelegate(SystemVariables variables);
        
        /// <summary>
        /// CAUTION: this event is called on update of variables, make sure 
        /// NOT to BLOCK it, since the entire system may deadlock.
        /// We can not afford to spawn new threads to run it since it is
        /// core thread monitoring functionality.
        /// </summary>
        public event VariablesUpdatedDelegate VariablesUpdatedEvent;

        /// <summary>
        /// 
        /// </summary>
        public SystemVariables()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetNamesValues(out List<string> names, out List<double?> values)
        {
            lock (_systemVariables)
            {
                names = GeneralHelper.EnumerableToList(_systemVariables.Keys);
                values = GeneralHelper.EnumerableToList(_systemVariables.Values);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void RaiseVariablesUpdatedEvent()
        {
            if (DateTime.Now - _lastUpdateTime > _updateEventInvocationTimeout)
            {
                _lastUpdateTime = DateTime.Now;
                if (VariablesUpdatedEvent != null)
                {
                    VariablesUpdatedEvent(this);
                }
            }
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public void SetValue(object owner, string name, double? value)
        {
            if (_enabled == false)
            {
                return;
            }
            
            string variableName;
            if (_showOriginatingClassNames)
            {
                variableName = owner.GetType().Name + "." + name;
            }
            else
            {
                variableName = name;
            }

            this[variableName] = value;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public double? GetValue(object owner, string name)
        {
            if (_enabled == false)
            {
                return null;
            }

            string variableName;
            if (_showOriginatingClassNames)
            {
                variableName = owner.GetType().Name + "." + name;
            }
            else
            {
                variableName = name;
            }

            return this[variableName];
        }

        /// <summary>
        /// Set the value of a system variable (will create it if not present).
        /// </summary>
        public void SetValue(string name, double? value)
        {
            if (_enabled == false)
            {
                return;
            }

            this[name] = value;
        }

        public double? GetValue(string name)
        {
            if (_enabled == false)
            {
                return null;
            }

            return this[name];
        }
    }
}
