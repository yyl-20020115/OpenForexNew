using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    public class SessionException : Exception
    {
        public SessionException(string message)
            : base(message)
        {
        }
    }
}
