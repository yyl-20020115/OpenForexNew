using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Implement an item sink tracing item information to system diagnostics trace.
    /// </summary>
    public class SystemTracerItemSink : TracerItemSink
    {
        /// <summary>
        /// 
        /// </summary>
        public SystemTracerItemSink(Tracer tracer)
            : base(tracer)
        {
        }

        protected override bool OnReceiveItem(TracerItem item, bool isFilteredOutByTracer, bool isFilteredOutBySink)
        {
            System.Diagnostics.Trace.WriteLine(item.PrintPrefix(' ') + item.PrintMessage());
            return true;
        }
    }
}
