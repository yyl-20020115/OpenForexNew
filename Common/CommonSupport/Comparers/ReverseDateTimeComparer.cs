using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Compare the items in reverse order.
    /// </summary>
    public class ReverseDateTimeComparer : IComparer<DateTime>
    {
        #region IComparer<DateTime> Members

        public int Compare(DateTime x, DateTime y)
        {
            return y.CompareTo(x);
        }

        #endregion
    }
}
