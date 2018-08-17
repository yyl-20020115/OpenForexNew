using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    public interface IRanged
    {
        int StartIndex
        {
            get;
            set;
        }

        int Count
        {
            get;
            set;
        }

        int MaxRange
        {
            get;
        }
    }
}
