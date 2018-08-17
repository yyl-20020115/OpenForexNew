using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonSupport
{
    /// <summary>
    /// Implements a simple bi-directional dictionary (or a bi-map).
    /// </summary>
    /// <typeparam name="ValueType1"></typeparam>
    /// <typeparam name="ValueType2"></typeparam>
    public class BiDictionary<ValueTypeKey, ValueTypeValue> 
    {

        Dictionary<ValueTypeKey, ValueTypeValue> _dictionary1 = new Dictionary<ValueTypeKey, ValueTypeValue>();
        Dictionary<ValueTypeValue, ValueTypeKey> _dictionary2 = new Dictionary<ValueTypeValue, ValueTypeKey>();

        /// <summary>
        /// [] accessor.
        /// </summary>
        public ValueTypeValue this[ValueTypeKey value]
        {
            get { return _dictionary1[value]; }
        }

        /// <summary>
        /// Count of elements in collection.
        /// </summary>
        public int Count
        {
            get { return _dictionary1.Count; }
        }

        public IEnumerable<KeyValuePair<ValueTypeKey, ValueTypeValue>> Pairs
        {
            get
            {
                return _dictionary1;
            }
        }

        public IEnumerable<ValueTypeKey> Keys
        {
            get { return _dictionary1.Keys; }
        }

        public IEnumerable<ValueTypeValue> Values
        {
            get { return _dictionary1.Values; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BiDictionary()
        {
        }

        /// <summary>
        /// Add element.
        /// </summary>
        public bool Add(ValueTypeKey keyValue, ValueTypeValue valueValue)
        {
            if (_dictionary1.ContainsKey(keyValue) || _dictionary2.ContainsKey(valueValue))
            {
                return false;
            }

            _dictionary1.Add(keyValue, valueValue);
            _dictionary2.Add(valueValue, keyValue);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public ValueTypeValue GetByKey(ValueTypeKey key)
        {
            return _dictionary1[key];
        }

        /// <summary>
        /// 
        /// </summary>
        public object GetByKeyNullabe(ValueTypeKey key)
        {
            if (_dictionary1.ContainsKey(key) == false)
            {
                return null;
            }

            return _dictionary1[key];
        }


        /// <summary>
        /// Safe.
        /// </summary>
        public bool GetByKeySafe(ValueTypeKey key, ref ValueTypeValue value)
        {
            if (_dictionary1.ContainsKey(key) == false)
            {
                return false;
            }

            value = _dictionary1[key];
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public ValueTypeKey GetByValue(ValueTypeValue value)
        {
            return _dictionary2[value];
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ContainsValue(ValueTypeValue value)
        {
            return _dictionary1.ContainsValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ContainsKey(ValueTypeKey key)
        {
            return _dictionary1.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public object GetByValueNullabe(ValueTypeValue value)
        {
            if (_dictionary2.ContainsKey(value) == false)
            {
                return null;
            }

            return _dictionary2[value];
        }


        /// <summary>
        /// Safe.
        /// </summary>
        public bool GetByValueSafe(ValueTypeValue key, ref ValueTypeKey value)
        {
            if (_dictionary2.ContainsKey(key) == false)
            {
                return false;
            }

            value = _dictionary2[key];
            return true;
        }

        public void Clear()
        {
            _dictionary1.Clear();
            _dictionary2.Clear();
        }

        public bool RemoveByKey(ValueTypeKey value)
        {
            if (_dictionary1.ContainsKey(value))
            {
                _dictionary2.Remove(_dictionary1[value]);
                return _dictionary1.Remove(value);
            }

            return false;
        }

        public bool RemoveByValue(ValueTypeValue value)
        {
            if (_dictionary2.ContainsKey(value))
            {
                _dictionary1.Remove(_dictionary2[value]);
                return _dictionary2.Remove(value);
            }

            return false;
        }

    }
}
