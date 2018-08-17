using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using CommonSupport.Events;

namespace CommonSupport
{
    /// <summary>
    /// Base class for event sources; events are occurences that have a time, text and other
    /// basic parameters assigned to them, and are usually notification of something happening
    /// at the given moment (for ex. a news item).
    /// </summary>
    [DBPersistence(false)]
    public class EventSource : PersistentOperational
    {
        volatile bool _enabled = true;
        /// <summary>
        /// Is the source enabled.
        /// </summary>
        [DBPersistence(true)]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (EnabledChangedEvent != null)
                {
                    EnabledChangedEvent(this);
                }
            }
        }

        protected volatile string _address = string.Empty;
        /// <summary>
        /// Address of this source.
        /// </summary>
        [DBPersistence(true)]
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        protected volatile string _name = string.Empty;
        /// <summary>
        /// Name of the source.
        /// </summary>
        [DBPersistence(true)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected volatile string _description = string.Empty;
        /// <summary>
        /// Description of this event source.
        /// </summary>
        [DBPersistence(true)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Ugly container, but makes sure that items belong to corresponding channel, 
        /// and each channel has its items sort accessible by date time.
        /// The string is the channel name, the DateTime is the time of the channel 
        /// and items that are corresponding to this time.
        /// </summary>
        protected Dictionary<string, EventSourceChannel> _channels =
            new Dictionary<string, EventSourceChannel>();

        /// <summary>
        /// Obtain the names of all channels.
        /// </summary>
        public List<EventSourceChannel> Channels
        {
            get
            {
                lock (_channels)
                {
                    return GeneralHelper.EnumerableToList(_channels.Values);
                }
            }
        }

        /// <summary>
        /// Names of the channels in this source.
        /// </summary>
        public List<string> ChannelsNames
        {
            get
            {
                lock (_channels)
                {
                    return GeneralHelper.EnumerableToList(_channels.Keys);
                }
            }
        }

        /// <summary>
        /// Count of channels in the source.
        /// </summary>
        public int ChannelsCount
        {
            get
            {
                return _channels.Count;
            }
        }

        /// <summary>
        /// Additional data field used for storing internal operation information.
        /// </summary>
        [DBPersistence(DBPersistenceAttribute.PersistenceTypeEnum.Binary)]
        public virtual SerializationInfoEx Data
        {
            get
            {
                SerializationInfoEx info = new SerializationInfoEx();
                info.AddValue("Channels", Channels);
                return info;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                lock(_channels)
                {
                    _channels.Clear();
                }

                if (value.HasValue("Channels"))
                {
                    foreach (EventSourceChannel channel in value.GetValue<List<EventSourceChannel>>("Channels"))
                    {
                        channel.Initialize(this);
                        AddChannel(channel);
                    }
                }
            }
        }

        #region Events and Delegates

        public delegate void EnabledChangedDelegate(EventSource source);

        public delegate void ItemsUpdateDelegate(EventSource source, IEnumerable<EventBase> items);

        public event EnabledChangedDelegate EnabledChangedEvent;
        //public event OperationalStateChangedDelegate OperationalStateChangedEvent;

        public event ItemsUpdateDelegate ItemsAddedEvent;
        public event ItemsUpdateDelegate ItemsUpdatedEvent;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public EventSource()
        {
        }

        /// <summary>
        /// Perform update of data.
        /// </summary>
        public void Update()
        {
            if (this.Enabled)
            {
                OnUpdate();
            }
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected virtual void OnUpdate()
        {
        }

        /// <summary>
        /// Add a new channel to this source.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enabled"></param>
        protected void AddChannel(EventSourceChannel channel)
        {
            lock (_channels)
            {
                _channels[channel.Name] = channel;
            }

            channel.ItemsAddedEvent += new EventSourceChannel.ItemsUpdateDelegate(channel_ItemsAddedEvent);
            channel.ItemsUpdatedEvent += new EventSourceChannel.ItemsUpdateDelegate(channel_ItemsUpdatedEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        protected void RemoveChannel(EventSourceChannel channel)
        {
            lock (_channels)
            {
                if (_channels.Remove(channel.Name))
                {
                    channel.ItemsAddedEvent -= new EventSourceChannel.ItemsUpdateDelegate(channel_ItemsAddedEvent);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void channel_ItemsAddedEvent(EventSource source, EventSourceChannel channel, IEnumerable<EventBase> items)
        {
            if (ItemsAddedEvent != null)
            {
                ItemsAddedEvent(source, items);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void channel_ItemsUpdatedEvent(EventSource source, EventSourceChannel channel, IEnumerable<EventBase> items)
        {
            if (ItemsUpdatedEvent != null)
            {
                ItemsUpdatedEvent(source, items);
            }
        }

        /// <summary>
        /// It is needed to specify the item type here, since otherwise when the result is empty,
        /// it can not be cast to the actual type in real time and causes an exception.
        /// The problem is casting an array ot base type to children type is not possible, when the array
        /// is coming from a list of base types converted with ToArray().
        /// </summary>
        public List<ItemType> GetAllItemsFlat<ItemType>()
            where ItemType : EventBase
        {
            List<ItemType> result = new List<ItemType>();
            
            foreach (EventSourceChannel channel in Channels)
            {
                if (channel.Enabled)
                {
                    lock (channel)
                    {
                        foreach (EventBase eventItem in channel.ItemsUnsafe.Values)
                        {
                            result.Add((ItemType)eventItem);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// It is needed to specify the item type here, since otherwise when the result is empty,
        /// it can not be cast to the actual type in real time and causes an exception.
        /// The problem is casting an array ot base type to children type is not possible, when the array
        /// is coming from a list of base types converted with ToArray().
        /// </summary>
        public SortedList<ItemType, ItemType> GetAllItems<ItemType>()
            where ItemType : EventBase
        {
            SortedList<ItemType, ItemType> result = new SortedList<ItemType,ItemType>();

            foreach (EventSourceChannel channel in Channels)
            {
                if (channel.Enabled)
                {
                    lock (channel)
                    {
                        foreach (ItemType item in channel.ItemsUnsafe.Values)
                        {
                            //string id = item.GetFullId();
                            if (result.ContainsKey(item))
                            {// If there is id duplication, resolve with additional guid.
                                SystemMonitor.OperationError("Item hash code duplication [" + item.GetFullId() + "], item skipped.");
                                //id += "." + Guid.NewGuid().ToString();
                            }

                            result.Add(item, item);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EventSourceChannel GetChannelByName(string name, bool create)
        {
            lock (_channels)
            {
                if (_channels.ContainsKey(name) == false)
                {
                    if (create)
                    {
                        EventSourceChannel channel = new EventSourceChannel(name, true);
                        channel.Initialize(this);

                        _channels.Add(name, channel);
                    }
                    else
                    {
                        return null;
                    }
                }

                return _channels[name];
            }
        }


    }
}
