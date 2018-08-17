using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Class defines common functionality for tracer item sink implementations.
    /// </summary>
    public abstract class TracerItemSink : ITracerItemSink
    {
        volatile bool _enabled = true;
        /// <summary>
        /// Sink enabled / disabled.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        bool _processNonFilteredOutItems = true;
        /// <summary>
        /// Sink accepts all or only filtered items.
        /// </summary>
        public bool ProcessNonFilteredOutItems
        {
            get { return _processNonFilteredOutItems; }
            set { _processNonFilteredOutItems = value; }
        }

        volatile Tracer _tracer;
        /// <summary>
        /// Sinks owner tracer.
        /// </summary>
        public Tracer Tracer
        {
            get { return _tracer; }
        }

        /// <summary>
        /// Filters specific for this sink.
        /// </summary>
        ListUnique<TracerFilter> _filters = new ListUnique<TracerFilter>();

        public TracerFilter[] FiltersArray
        {
            get
            {
                lock (this)
                {
                    return _filters.ToArray();
                }
            }
        }

        public delegate void SinkUpdateDelegate(TracerItemSink tracer);

        [field: NonSerialized]
        public event SinkUpdateDelegate FilterUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public TracerItemSink(Tracer tracer)
        {
            _tracer = tracer;
        }

        /// <summary>
        /// Override in child class to handle item acqustion.
        /// </summary>
        protected abstract bool OnReceiveItem(TracerItem item, bool isFilteredOutByTracer, bool isFilteredOutBySink);

        /// <summary>
        /// 
        /// </summary>
        public virtual bool ReceiveItem(TracerItem item, bool isFilteredOut)
        {
            if (_enabled && (isFilteredOut || _processNonFilteredOutItems))
            {
                bool filteredBySink = false;
                filteredBySink = !Tracer.FilterItem(FiltersArray, item);

                return OnReceiveItem(item, isFilteredOut, filteredBySink);
            }

            return true;
        }

        /// <summary>
        /// Add sink specific filter.
        /// It is up to the sinks implementation to how the filter will be used.
        /// </summary>
        public bool AddFilter(TracerFilter filter)
        {
            lock (this)
            {
                if (_filters.Add(filter))
                {
                    filter.FilterUpdatedEvent += new TracerFilter.FilterUpdatedDelegate(filter_FilterUpdatedEvent);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearFilters()
        {
            foreach (TracerFilter filter in FiltersArray)
            {
                RemoveFilter(filter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemoveFilter(TracerFilter filter)
        {
            lock (this)
            {
                if (_filters.Remove(filter))
                {
                    filter.FilterUpdatedEvent -= new TracerFilter.FilterUpdatedDelegate(filter_FilterUpdatedEvent);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Clear()
        {
        }

        protected virtual void filter_FilterUpdatedEvent(TracerFilter filter)
        {
            if (FilterUpdateEvent != null)
            {
                FilterUpdateEvent(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            Clear();
            _tracer = null;
        }
    }
}
 