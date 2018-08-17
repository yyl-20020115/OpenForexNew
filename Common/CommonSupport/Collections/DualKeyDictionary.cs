//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CommonSupport
//{
//    /// <summary>
//    /// Collection allows the usage of multiple keys to locate a certain value.
//    /// </summary>
//    public class DualKeyDictionary<TKey1, TKey2, TValue>
//    {
//        Dictionary<TKey1, TValue> _dictionary1 = new Dictionary<TKey1, TValue>();
//        Dictionary<TKey2, TValue> _dictionary2 = new Dictionary<TKey2, TValue>();
                
//        /// <summary>
//        /// [] accessor; works on first dictionary.
//        /// </summary>
//        public TValue this[TKey1 value]
//        {
//            get { return _dictionary1[value]; }
//        }

//        /// <summary>
//        /// Count of elements in collection.
//        /// </summary>
//        public int Count
//        {
//            get { return _dictionary1.Count; }
//        }

//        public IEnumerable<TKey1> Keys1
//        {
//            get { return _dictionary1.Keys; }
//        }

//        public IEnumerable<TKey2> Keys2
//        {
//            get { return _dictionary2.Keys; }
//        }

//        public IEnumerable<TValue> Values
//        {
//            get { return _dictionary1.Values; }
//        }

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        public DualKeyDictionary()
//        {
//        }

//        /// <summary>
//        /// Add element.
//        /// </summary>
//        public bool Add(TKey1 key1Value, TKey2 key2Value, TValue valueValue)
//        {
//            if (_dictionary1.ContainsKey(key1Value) || _dictionary2.ContainsKey(key2Value))
//            {
//                return false;
//            }

//            _dictionary1.Add(keyValue, valueValue);
//            _dictionary2.Add(valueValue, keyValue);

//            return true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public TValue GetByKey(TKey1 key)
//        {
//            return _dictionary1[key];
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public object GetByKeyNullabe(TKey1 key)
//        {
//            if (_dictionary1.ContainsKey(key) == false)
//            {
//                return null;
//            }

//            return _dictionary1[key];
//        }


//        /// <summary>
//        /// Safe.
//        /// </summary>
//        public bool GetByKeySafe(TKey1 key, ref TValue value)
//        {
//            if (_dictionary1.ContainsKey(key) == false)
//            {
//                return false;
//            }

//            value = _dictionary1[key];
//            return true;
//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        public TKey1 GetByValue(TValue value)
//        {
//            return _dictionary2[value];
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public bool ContainsValue(TValue value)
//        {
//            return _dictionary1.ContainsValue(value);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public bool ContainsKey(TKey1 key)
//        {
//            return _dictionary1.ContainsKey(key);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public object GetByValueNullabe(TValue value)
//        {
//            if (_dictionary2.ContainsKey(value) == false)
//            {
//                return null;
//            }

//            return _dictionary2[value];
//        }


//        /// <summary>
//        /// Safe.
//        /// </summary>
//        public bool GetByValueSafe(TValue key, ref TKey1 value)
//        {
//            if (_dictionary2.ContainsKey(key) == false)
//            {
//                return false;
//            }

//            value = _dictionary2[key];
//            return true;
//        }

//        public void Clear()
//        {
//            _dictionary1.Clear();
//            _dictionary2.Clear();
//        }

//        public bool RemoveByKey(TKey1 value)
//        {
//            if (_dictionary1.ContainsKey(value))
//            {
//                _dictionary2.Remove(_dictionary1[value]);
//                return _dictionary1.Remove(value);
//            }

//            return false;
//        }

//        public bool RemoveByValue(TValue value)
//        {
//            if (_dictionary2.ContainsKey(value))
//            {
//                _dictionary1.Remove(_dictionary2[value]);
//                return _dictionary2.Remove(value);
//            }

//            return false;
//        }


//    }
//}
