using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Define how a sink for tracer items showd look like.
    /// A typcal sink stores tracer items to some external output
    /// like a file or system output.
    /// A sink can receive all items (both filter out and passed).
    /// </summary>
    public interface ITracerItemSink : IDisposable
    {
        /// <summary>
        /// Accept an item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isFiltered">Has item failed on filtering conditions.</param>
        /// <returns></returns>
        bool ReceiveItem(TracerItem item, bool isFilteredOut);

        /// <summary>
        /// Clear data stored in sink.
        /// </summary>
        void Clear();
    }
}
