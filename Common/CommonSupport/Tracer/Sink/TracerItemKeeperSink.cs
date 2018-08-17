using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonSupport
{
    /// <summary>
    /// Class stored tracer item items for further inspection.
    /// </summary>
    public class TracerItemKeeperSink : TracerItemSink
    {
        volatile int _maxItems = 200000;
        /// <summary>
        /// The maximum number of items, from the all items pool, to store in memory.
        /// Set to 0 to specify no limit.
        /// </summary>
        public int MaxItems
        {
            get { return _maxItems; }
            set { _maxItems = value; }
        }

        List<TracerItem> _items = new List<TracerItem>();

        /// <summary>
        /// Gathering and storing items by type is costly, so perform only when needed.
        /// Set to null, to stop collecting items by type.
        /// Gathering and storing items by type is costly, so perform only when needed (needed by the TraceStatusStripOperator).
        /// </summary>
        Dictionary<TracerItem.TypeEnum, List<TracerItem>> _itemsByType = new Dictionary<TracerItem.TypeEnum, List<TracerItem>>();

        /// <summary>
        /// Items passed trough filtering and were approved.
        /// Unsafe collection means the owner TracerItemKeeperSink class needs 
        /// to be locked before safe iteration.
        /// </summary>
        volatile List<TracerItem> _filteredItems = new List<TracerItem>();

        /// <summary>
        /// Fitlered items count.
        /// </summary>
        public int FilteredItemsCount
        {
            get { return _filteredItems.Count; }
        }

        public delegate void TracerUpdateDelegate(TracerItemKeeperSink tracer);
        public delegate void ItemUpdateDelegate(TracerItemKeeperSink tracer, TracerItem item);

        [field: NonSerialized]
        public event ItemUpdateDelegate ItemAddedEvent;

        [field: NonSerialized]
        public event TracerUpdateDelegate ItemsFilteredEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TracerItemKeeperSink(Tracer tracer)
            : base(tracer)
        {
            SetupItemsByType();
        }

        /// <summary>
        /// Clear existing and filtered items.
        /// </summary>
        public override void Clear()
        {
            lock (this)
            {
                _items.Clear();
                _filteredItems.Clear();
                _itemsByType.Clear();
                SetupItemsByType();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int GetItemsByTypeCount(TracerItem.TypeEnum type)
        {
            lock (this)
            {
                if (_itemsByType == null)
                {
                    return 0;
                }

                return _itemsByType[type].Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TracerItem GetFilteredItem(int index)
        {
            lock (this)
            {
                if (_filteredItems.Count > index)
                {
                    return _filteredItems[index];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        void SetupItemsByType()
        {
            lock (this)
            {
                foreach (TracerItem.TypeEnum value in Enum.GetValues(typeof(TracerItem.TypeEnum)))
                {
                    if (_itemsByType.ContainsKey(value) == false)
                    {
                        _itemsByType.Add(value, new List<TracerItem>());
                    }
                }
            }
        }

        /// <summary>
        /// Put all items trough filtering again, to have a fresh set of FilteredItems.
        /// </summary>
        public void ReFilterItems()
        {
            Tracer tracer = Tracer;
            if (tracer == null)
            {
                return;
            }

            TracerFilter[] filtersArray = FiltersArray;
            List<TracerItem> filteredItems = new List<TracerItem>();

            // Not using a copy here causes 2 problems
            // - "collection modified" while filtering, possibly due to Filter raising some event on the same thread (FilterUpdated), that modifies deposits a new item in the _items.
            // - a possible dead lock, since we need to keep locked while calling filters Filter() methods.
            List<TracerItem> items;
            lock (this)
            {
                items = new List<TracerItem>(_items.Count);
                items.AddRange(_items);
            }

            foreach (TracerItem item in items)
            {
                if (Tracer.FilterItem(filtersArray, item))
                {
                    filteredItems.Add(item);
                }
            }

            lock (this)
            {
                _filteredItems = filteredItems;
            }

            if (ItemsFilteredEvent != null)
            {
                ItemsFilteredEvent(this);
            }

            foreach (TracerFilter filter in filtersArray)
            {// Allow the filters to raise delayed update events.
                filter.PerformDelayedUpdateEvent();
            }
        }

        protected override void filter_FilterUpdatedEvent(TracerFilter filter)
        {
            // This causes the filtering to be executed on the event raising thread.
            GeneralHelper.FireAndForget(ReFilterItems);

            base.filter_FilterUpdatedEvent(filter);
        }

        /// <summary>
        /// Lock on this, before calling.
        /// </summary>
        /// <param name="?"></param>
        bool LimitItemsSetSize(List<TracerItem> items)
        {
            if (_maxItems > 0 && items.Count > _maxItems)
            {// Remove the first 10%, only low importance items.
                items.RemoveRange(0, (int)((float)_maxItems / 10f));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool OnReceiveItem(TracerItem item, bool isFilteredOutByTracer, bool isFilteredOutBySink)
        {
            if (isFilteredOutByTracer)
            {
                return true;
            }

            lock (this)
            {
                if (LimitItemsSetSize(_items))
                {
                    LimitItemsSetSize(_filteredItems);
                }

                _items.Add(item);

                if (isFilteredOutBySink == false)
                {
                    _filteredItems.Add(item);
                }

                if (_itemsByType != null)
                {
                    foreach (TracerItem.TypeEnum type in item.Types)
                    {
                        LimitItemsSetSize(_itemsByType[type]);

                        _itemsByType[type].Add(item);
                    }
                }
            }

            if (isFilteredOutBySink == false && ItemAddedEvent != null)
            {
                ItemAddedEvent(this, item);
            }

            return true;

        }
    }
}
