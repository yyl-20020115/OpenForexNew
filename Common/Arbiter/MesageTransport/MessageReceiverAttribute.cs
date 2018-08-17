using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    public class MessageReceiverAttribute : Attribute
    {
        /// <summary>
        /// Allows only the type specified.
        /// </summary>
        public MessageReceiverAttribute()
        {
        }
    }
}
