using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Threading;

namespace CommonSupport
{
    /// <summary>
    /// Main Tracing manement class. Allows the tracing of messages, filtering, etc.
    /// Tracer does not store items, as it only passes them further to the sinks.
    /// To store items from tracer use the TracerItemKeeperSink.
    /// </summary>
    public class Tracer : IDisposable
    {
        volatile bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public enum TimeDisplayFormatEnum
        {
            DateTime,
            ApplicationTicks,
            Combined
        }

        volatile TimeDisplayFormatEnum _timeDisplayFormat = TimeDisplayFormatEnum.ApplicationTicks;
        /// <summary>
        /// 
        /// </summary>
        public TimeDisplayFormatEnum TimeDisplayFormat
        {
            get { return _timeDisplayFormat; }
            set { _timeDisplayFormat = value; }
        }

        long _totalItemsCount = 0;
        /// <summary>
        /// Count of all items ever passed trough this tracer.
        /// May be bigger than actual number of items stored in it.
        /// </summary>
        public long TotalItemsCount
        {
            get { return _totalItemsCount; }
        }

        ListUnique<ITracerItemSink> _itemSinks = new ListUnique<ITracerItemSink>();
        /// <summary>
        /// 
        /// </summary>
        public ITracerItemSink[] ItemSinksArray
        {
            get { lock (this) { return _itemSinks.ToArray(); } }
        }

        ListUnique<TracerFilter> _filters = new ListUnique<TracerFilter>();
         //<summary>
         //A collection of all the filters currently applied to this tracer.
         //</summary>
        public TracerFilter[] FiltersArray
        {
            get { lock (this) { return _filters.ToArray(); } }
        }

        public delegate void TracerClearDelegate(Tracer tracer, bool completeClear);
        public delegate void ItemUpdateDelegate(Tracer tracer, TracerItem item);
        
        public event ItemUpdateDelegate ItemAddedEvent;
        public event TracerClearDelegate TracerClearedEvent;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Tracer()
        {
        }

        /// <summary>
        /// This will insert the tracer in the thread's data slots, making it possible
        /// for thread specific tracing to be done.
        /// </summary>
        /// <param name="thread"></param>
        public void AttachToThread(Thread thread)
        {
            SystemMonitor.NotImplementedCritical("Operation not supported.");
        }

        /// <summary>
        /// Remove self as the trade's default tracer, inserted in the thread's data slot.
        /// </summary>
        /// <param name="thread"></param>
        public void DetachFromThread(Thread thread)
        {
            SystemMonitor.NotImplementedCritical("Operation not supported.");
        }

        /// <summary>
        /// Helper, returns item sink by its type (if present in tracer).
        /// </summary>
        /// <param name="sinkType"></param>
        /// <returns></returns>
        public ITracerItemSink GetSinkByType(Type sinkType)
        {
            foreach (ITracerItemSink sink in ItemSinksArray)
            {
                if (sink.GetType() == sinkType)
                {
                    return sink;
                }
            }

            return null;
        }

        /// <summary>
        /// Will clear class filters and sinks (or optionaly also remove them).
        /// </summary>
        /// <param name="completeClear">Will also remove the sinks and filters. Pass false to only clear them.</param>
        public void Clear(bool completeClear)
        {
            ITracerItemSink[] sinks;
            lock (this)
            {
                sinks = _itemSinks.ToArray();
            }

            foreach (ITracerItemSink sink in sinks)
            {
                sink.Clear();
                if (completeClear)
                {
                    sink.Dispose();
                }
            }

            if (completeClear)
            {
                lock (this)
                {
                    _itemSinks.Clear();
                    _filters.Clear();
                }
            }

            if (TracerClearedEvent != null)
            {
                TracerClearedEvent(this, completeClear);
            }
        }

        /// <summary>
        /// Filter single item, passing it trough all filters.
        /// Will return true if item is allowed to pass filter.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="item"></param>
        public static bool FilterItem(IEnumerable<TracerFilter> filters, TracerItem item)
        {
            foreach (TracerFilter filter in filters)
            {
                if (filter.FilterItem(item) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Filter single item, passing it trough all filters.
        /// </summary>
        public bool FilterItem(TracerItem item)
        {
            return FilterItem(FiltersArray, item);
        }

        /// <summary>
        /// Add a new tracer filter to tracer.
        /// Make sure this is correct, since once filter in place, items will not be passed to sinks at all.
        /// </summary>
        public bool Add(TracerFilter filter)
        {
            filter.Initialize(this);
            lock (this)
            {
                return _filters.Add(filter);
            }
        }

        /// <summary>
        /// Add a tracer item sink to tracer.
        /// </summary>
        /// <param name="sink"></param>
        /// <returns></returns>
        public bool Add(ITracerItemSink sink)
        {
            lock (this)
            {
                return _itemSinks.Add(sink);
            }
        }

        /// <summary>
        /// Remove tracer item sink from tracer.
        /// </summary>
        /// <param name="sink"></param>
        /// <returns></returns>
        public bool Remove(ITracerItemSink sink)
        {
            bool result;
            lock (this)
            {
                result = _itemSinks.Remove(sink);
            }

            if (result)
            {
                sink.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Add a new tracer item to tracer.
        /// </summary>
        public void Add(TracerItem tracerItem)
        {
            if (this.Enabled == false)
            {
                return;
            }

            tracerItem.Index = _totalItemsCount;
            Interlocked.Increment(ref _totalItemsCount);

            bool filtered = FilterItem(tracerItem);
            foreach (ITracerItemSink sink in ItemSinksArray)
            {
                sink.ReceiveItem(tracerItem, !filtered);
            }

            if (ItemAddedEvent != null)
            {
                ItemAddedEvent(this, tracerItem);
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            Clear(true);
        }

        #endregion
    }
}
