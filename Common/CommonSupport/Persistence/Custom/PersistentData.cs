using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    [Serializable]
    public class PersistentData
    {
        Dictionary<string, object> _values = new Dictionary<string, object>();
        public Dictionary<string, object> Values
        {
            get { lock (this) { return _values; } }
        }

        public object this[string value]
        {
            get { lock (this) { return _values[value]; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public PersistentData()
        {
        }

        //public string ReadString(string fieldName)
        //{
        //    lock (this)
        //    {
        //        if (_values.ContainsKey(fieldName))
        //        {
        //            return string.Empty;
        //        }

        //        return _values[fieldName];
        //    }
        //}

        public PersistentData Clone()
        {
            PersistentData result = new PersistentData();
            lock (this)
            {
                foreach (string key in _values.Keys)
                {
                    result._values.Add(key, _values[key]);
                }
            }
            return result;
        }
    }
}
