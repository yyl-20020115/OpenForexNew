using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// 
    /// </summary>
    public class SortedListEx<TKey, TValue> : SortedList<TKey, TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool RemoveFirstValue(TValue value)
        {
            int index = this.IndexOfValue(value);
            if (index < 0)
            {
                return false;
            }

            this.RemoveAt(index);
            return true;
        }
    }
}
