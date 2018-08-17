using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Hot swapping - this is the fastes way of all to access a client, 
    /// without holding an actual reference, and no locks either.
    /// The client Id must contain the index of the list.
    /// This index will never change, since we shall only add items to the list.
    /// 
    /// To evade locking - when adding new items, simply replace the list with a new one.
    /// </summary>
    public class HotSwapList<TType> : IList<TType>
    {
        volatile List<TType> _instance = new List<TType>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public HotSwapList()
        {
        }

        /// <summary>
        /// Add item only if it does not already exist.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the add was performed, or false if it already exists.</returns>
        public bool AddUnique(TType item)
        {
            lock (this)
            {
                if (_instance.Contains(item))
                {
                    return false;
                }

                Add(item);
            }

            return true;
        }

        /// <summary>
        /// Try to obtain a value with this index, return false if we fail and no modification to value done.
        /// </summary>
        /// <param name="index">The index of the item retrieved.</param>
        /// <param name="value">The resulting retrieve value.</param>
        /// <returns>True if the value was retrieved, otherwise false.</returns>
        public bool TryGetValue(int index, ref TType value)
        {
            List<TType> instance = _instance;
            if (instance.Count > index)
            {
                value = instance[index];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clear all items and add the current badge.
        /// </summary>
        public void SetToRange(IEnumerable<TType> items)
        {
            lock (this)
            {
                List<TType> instance = new List<TType>();
                instance.AddRange(items);
                _instance = instance;
            }
        }

        public void AddRange(IEnumerable<TType> items)
        {
            lock (this)
            {
                List<TType> instance = new List<TType>(_instance);
                instance.AddRange(items);
                _instance = instance;
            }
        }
        
        /// <summary>
        /// Remove all instances that are equal, or the same as, this item.
        /// </summary>
        /// <returns>Count of items removed.</returns>
        public int RemoveAll(TType item)
        {
            int result = 0;
            lock (this)
            {
                List<TType> instance = new List<TType>(_instance);
                
                while (instance.Remove(item))
                {
                    result++;
                }

                if (result != 0)
                {
                    _instance = instance;
                }
            }

            return result;
        }


        #region IList<TType> Members

        public int IndexOf(TType item)
        {
            return _instance.IndexOf(item);
        }

        public void Insert(int index, TType item)
        {
            lock (this)
            {
                List<TType> items = new List<TType>(_instance);
                items.Insert(index, item);
                _instance = items;
            }
        }

        /// <summary>
        /// Implementation has internal check security,
        /// so no exceptions occur.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            lock (this)
            {
                if (_instance.Count > index)
                {
                    List<TType> items = new List<TType>(_instance);
                    items.RemoveAt(index);
                    _instance = items;
                }
            }
        }

        /// <summary>
        /// *Warning* setting a value if very slow, since it redoes the hotswaps
        /// the entire collection too, so use with caution.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TType this[int index]
        {
            get
            {
                return _instance[index];
            }

            set
            {
                lock (this)
                {
                    List<TType> items = new List<TType>(_instance);
                    items[index] = value;
                    _instance = items;
                }
            }
        }

        #endregion

        #region ICollection<TType> Members

        public void Add(TType item)
        {
            lock (this)
            {
                List<TType> items = new List<TType>(_instance);
                items.Add(item);
                _instance = items;
            }
        }

        public bool Remove(TType item)
        {
            lock (this)
            {
                if (_instance.Contains(item) == false)
                {
                    return false;
                }

                List<TType> items = new List<TType>(_instance);
                items.Remove(item);
                _instance = items;
            }

            return true;
        }

        public void Clear()
        {
            lock (this)
            {
                _instance = new List<TType>();
            }
        }

        public bool Contains(TType item)
        {
            return _instance.Contains(item);
        }

        public void CopyTo(TType[] array, int arrayIndex)
        {
            _instance.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _instance.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }


        #endregion

        #region IEnumerable<TType> Members

        public IEnumerator<TType> GetEnumerator()
        {
            return _instance.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _instance.GetEnumerator();
        }

        #endregion
    }
}
