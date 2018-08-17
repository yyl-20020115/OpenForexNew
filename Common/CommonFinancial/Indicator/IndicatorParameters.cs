using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Class manages the operation and containment of financial technical indicator parameters.
    /// </summary>
    [Serializable]
    public class IndicatorParameters : ICloneable
    {
        /// <summary>
        /// Core values are actually used to control the calculation process.
        /// </summary>
        Dictionary<string, object> _coreValues = new Dictionary<string, object>();

        /// <summary>
        /// Dynamic values are optional, and control additional indicator functionalities
        /// (for ex. "fixedLines")
        /// </summary>
        Dictionary<string, object> _dynamicValues = new Dictionary<string, object>();

        ///// <summary>
        ///// Allow full dynamic access to *core* input parameters only.
        ///// </summary>
        ///// <param name="parameterName"></param>
        ///// <returns></returns>
        //public object this[string parameterName]
        //{
        //    get
        //    {
        //        lock (this)
        //        {
        //            if (_coreValues.ContainsKey(parameterName) == false)
        //            {
        //                return null;
        //            }

        //            return _coreValues[parameterName];
        //        }
        //    }

        //    set
        //    {
        //        lock (this)
        //        {
        //            _coreValues[parameterName] = value;
        //        }

        //        if (ParameterUpdatedValueEvent != null)
        //        {
        //            ParameterUpdatedValueEvent(parameterName, value); 
        //        }
        //    }
        //}

        /// <summary>
        /// A list of the *dynamic* values of this indicator. Use for reading purposes, since its a copy.
        /// Dynamic values are optional, and control additional indicator functionalities (for ex. "fixedLines")
        /// </summary>
        public List<object> DynamicValues
        {
            get
            {
                lock (this)
                {
                    return GeneralHelper.EnumerableToList<object>(_dynamicValues.Values);
                }
            }
        }

        /// <summary>
        /// A list of the *dynamic* names of the values of this indicator. Use for reading purposes, since its a copy.
        /// </summary>
        public List<string> DynamicNames
        {
            get
            {
                lock (this)
                {
                    return GeneralHelper.EnumerableToList<string>(_dynamicValues.Keys);
                }
            }
        }

        /// <summary>
        /// Count of the *dynamic* values for this indicator, see CoreValues for details.
        /// </summary>
        public int DynamicValuesCount
        {
            get
            {
                return _dynamicValues.Count;
            }
        }

        /// <summary>
        /// A list of the *core* values of this indicator. Use for reading purposes, since its a copy.
        /// Core values are actually used to control the calculation process.
        /// </summary>
        public List<object> CoreValues
        {
            get
            {
                lock (this)
                {
                    return GeneralHelper.EnumerableToList<object>(_coreValues.Values);
                }
            }
        }

        /// <summary>
        /// A list of the *core* names of the values of this indicator. Use for reading purposes, since its a copy.
        /// </summary>
        public List<string> CoreNames
        {
            get
            {
                lock (this)
                {
                    return GeneralHelper.EnumerableToList<string>(_coreValues.Keys);
                }
            }
        }

        /// <summary>
        /// Count of the *core* values for this indicator, see CoreValues for details.
        /// </summary>
        public int CoreValuesCount
        {
            get
            {
                return _coreValues.Count;
            }
        }

        public delegate void ParameterUpdatedValueDelegate(string name, object value);
        /// <summary>
        /// Event fired when one of the parameters contained has been updated.
        /// </summary>
        [field:NonSerialized]
        public event ParameterUpdatedValueDelegate ParameterUpdatedValueEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public IndicatorParameters()
        {
        }

        /// <summary>
        /// Set core values.
        /// </summary>
        public void SetCore(string[] names, object[] values)
        {
            SystemMonitor.CheckThrow(names.Length == values.Length);
            lock (this)
            {
                _coreValues.Clear();
                for (int i = 0; i < names.Length; i++)
                {
                    _coreValues.Add(names[i], values[i]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object GetCore(string name)
        {
            lock (this)
            {
                if (_coreValues.ContainsKey(name))
                {
                    return _coreValues[name];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public object GetDynamic(string name)
        {
            lock (this)
            {
                if (_dynamicValues.ContainsKey(name))
                {
                    return _dynamicValues[name];
                }
            }

            return null;
        }

        /// <summary>
        /// Set a dynamic value.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="values"></param>
        public void SetDynamic(string name, object value)
        {
            lock (this)
            {
                _dynamicValues[name] = value;
            }

            if (ParameterUpdatedValueEvent != null)
            {
                ParameterUpdatedValueEvent(name, value);
            }
        }

        /// <summary>
        /// Set a single core value.
        /// </summary>
        public void SetCore(string name, object value)
        {
            lock (this)
            {
                _coreValues[name] = value;
            }

            if (ParameterUpdatedValueEvent != null)
            {
                ParameterUpdatedValueEvent(name, value);
            }
        }

        /// <summary>
        /// Set dynamic values.
        /// </summary>
        public void SetDynamic(string[] names, object[] values)
        {
            SystemMonitor.CheckThrow(names.Length == values.Length);
            lock (this)
            {
                _dynamicValues.Clear();
                for (int i = 0; i < names.Length; i++)
                {
                    _dynamicValues.Add(names[i], values[i]);
                }
            }
        }

        /// <summary>
        /// Does a dynamic value with this name exist.
        /// </summary>
        public bool ContainsDynamicValue(string name)
        {
            lock (this)
            {
                return _dynamicValues.ContainsKey(name);
            }
        }

        /// <summary>
        /// Does a core value with this name exist.
        /// </summary>
        public bool ContainsCoreValue(string name)
        {
            lock (this)
            {
                return _coreValues.ContainsKey(name);
            }
        }

        #region ICloneable Members

        public object Clone()
        {
            IndicatorParameters result = new IndicatorParameters();
            lock (this)
            {
                foreach (string name in _coreValues.Keys)
                {
                    result._coreValues.Add(name, _coreValues[name]);
                }

                foreach (string name in _dynamicValues.Keys)
                {
                    result._dynamicValues.Add(name, _dynamicValues[name]);
                }
            }
            return result;
        }

        #endregion
    }
}
