using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    // Also support for external trace results receiver.
    public interface ITracerObject
    {
        string Print();
        
        /// <summary>
        /// This providers the tracer object with a delegate - it is to call this delegate
        /// when it want's to initiate a print (use like a print event).
        /// </summary>
        void SetPrintCallback(Delegate printCallback);
    }
}
