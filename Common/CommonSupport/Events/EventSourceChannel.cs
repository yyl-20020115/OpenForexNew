using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CommonSupport
{
    /// <summary>
    /// Class represents a single channel in the source.
    /// </summary>
    [Serializable]
    public class EventSourceChannel : IDeserializationCallback
    {
        [NonSerialized]
        volatile EventSource _source = null;
        /// <summary>
        /// Source of this channel.
        /// </summary>
        public EventSource Source
        {
            get { return _source; }
        }

        volatile bool _itemsUpdateEnabled = false;
        /// <summary>
        /// Should adding new items duplicated with existing be updated or dropped.
        /// </summary>
        public bool ItemsUpdateEnabled
        {
            get { return _itemsUpdateEnabled; }
            set { _itemsUpdateEnabled = value; }
        }

        volatile bool _enabled = false;
        /// <summary>
        /// Has this channel been enabled.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }

            set 
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    if (ChannelUpdatedEvent != null)
                    {
                        ChannelUpdatedEvent(this);
                    }
                }
            }
        }

        volatile string _address = string.Empty;
        /// <summary>
        /// Address of this channel.
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        volatile string _name = string.Empty;
        /// <summary>
        /// Name of this channel.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        bool _enableLatestEventTitleDuplicationProtection = true;
        /// <summary>
        /// Enable or disable the protection of having items with duplicated titles (based on a latest days timespan).
        /// </summary>
        public bool EnableLatestEventTitleDuplicationProtection
        {
            get { return _enableLatestEventTitleDuplicationProtection; }
            set { _enableLatestEventTitleDuplicationProtection = value; }
        }

        TimeSpan _latestEventsFilterPeriod = TimeSpan.FromDays(10);
        /// <summary>
        /// What is the period for latest events duplication filtering.
        /// </summary>
        public TimeSpan LatestEventsTitleDuplicationFilterPeriod
        {
            get { lock (this) { return _latestEventsFilterPeriod; } }
            set { lock (this) { _latestEventsFilterPeriod = value; } }
        }

        /// <summary>
        /// Cache if items for the last few days, with titles.
        /// </summary>
        [NonSerialized]
        Dictionary<string, EventBase> _latestEvents = new Dictionary<string, EventBase>();

        [NonSerialized]
        SortedDictionary<EventBase, EventBase> _items = new SortedDictionary<EventBase, EventBase>();

        /// <summary>
        /// Items of this channel, thread unsafe.
        /// </summary>
        public SortedDictionary<EventBase, EventBase> ItemsUnsafe
        {
            get { return _items; }
        }

        #region Events and Delegates

        public delegate void ChannelUpdateDelegate(EventSourceChannel channel);
        public delegate void ItemsUpdateDelegate(EventSource source, EventSourceChannel channel, IEnumerable<EventBase> items);
        
        [field:NonSerialized]
        public event ChannelUpdateDelegate ChannelUpdatedEvent;

        [field: NonSerialized]
        public event ItemsUpdateDelegate ItemsAddedEvent;
        
        [field: NonSerialized]
        public event ItemsUpdateDelegate ItemsUpdatedEvent;

        #endregion

        #region Instance Control

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public EventSourceChannel(string name, bool enabled)
        {
            _name = name;
            _enabled = enabled;
        }

        /// <summary>
        /// Initialize this channel with the source it belongs to.
        /// </summary>
        public bool Initialize(EventSource source)
        {
            SystemMonitor.CheckError(_source == null, "Source already assigned.");
            _source = source;
            return true;
        }

        public void OnDeserialization(object sender)
        {
            _latestEvents = new Dictionary<string, EventBase>();
            _items = new SortedDictionary<EventBase, EventBase>();
        }

        #endregion

        /// <summary>
        /// Add items to channel.
		/// The items added may be with no assiged Id, or a duplication of other items.
        /// </summary>
        public virtual void AddItems(IEnumerable<EventBase> items)
        {
            SystemMonitor.CheckError(_source != null, "Source not assigned.");
            
            if (this.Enabled == false)
            {
                SystemMonitor.OperationWarning("Will not add items to disabled channel.");
                return;
            }

			List<EventBase> itemsAdded = new List<EventBase>();

            foreach (EventBase item in items)
            {
                if (item.DateTime.HasValue == false)
                {
                    SystemMonitor.OperationError("Event with no date time assigned can not be processed.");
                    continue;
                }

                lock (this)
                {
                    if (_items.ContainsKey(item) &&
                        _itemsUpdateEnabled == false)
                    {// Already an item with this Id is known.
                        continue;
                    }

                    if (_enableLatestEventTitleDuplicationProtection 
                        && ((DateTime.Now - item.DateTime) < _latestEventsFilterPeriod))
                    {
                        if (_latestEvents.ContainsKey(item.Title) == false)
                        {// Gather items from the last X days.
                            _latestEvents.Add(item.Title, item);
                        }
                        else
                        {// Item wit this title already known.
                            continue;
                        }
                    }

                    _items[item] = item;
                }

                if (ItemsAddedEvent != null)
				{
					itemsAdded.Add(item);
				}
            }

            if (ItemsAddedEvent != null)
            {
                ItemsAddedEvent(_source, this, itemsAdded);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void HandleItemsUpdated(IEnumerable<EventBase> items)
        {
            if (ItemsUpdatedEvent != null)
            {
                ItemsUpdatedEvent(_source, this, items);
            }
        }

    }
}
