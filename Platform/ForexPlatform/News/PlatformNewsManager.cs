using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using ForexPlatformPersistence;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ForexPlatform
{
    /// <summary>
    /// Implements the functionalities needed to allow the news manager to operate in the
    /// platform environment and in collaboration with other parts.
    /// </summary>
    [Serializable]
    public class PlatformNewsManager : NewsManager
    {
        [NonSerialized]
        SQLiteADOPersistenceHelper _persistenceHelper;

        [NonSerialized]
        Platform _platform;

        /// <summary>
        /// The platform this manager belongs to.
        /// </summary>
        public Platform Platform
        {
            get { return _platform; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformNewsManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformNewsManager(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Helper, creates the correspoding persistence helper.
        /// </summary>
        protected static SQLiteADOPersistenceHelper CreatePersistenceHelper(PlatformSettings settings)
        {
            SQLiteADOPersistenceHelper helper = new SQLiteADOPersistenceHelper();
            if (helper.Initialize(settings.GetMappedPath("EventsDBPath"), true) == false)
            {
                return null;
            }

            if (helper.ContainsTable("Events") == false)
            {// Create the table structure.
                object result = helper.ExecuteCommand(ForexPlatformPersistence.Properties.Settings.Default.EventsDBSchema);
            }

            helper.SetupTypeMapping(typeof(EventSource), "EventSources");
            helper.SetupTypeMapping(typeof(EventBase), "Events");

            return helper;
        }

        /// <summary>
        /// Helper, automates the loading of sources items from DB.
        /// </summary>
        /// <param name="source"></param>
        void LoadSourceItemsFromPersistence(EventSource source)
        {
            List<EventBase> items =
                _persistenceHelper.SelectDynamicType<EventBase>(new MatchExpression("SourceId", source.Id), null);

            Dictionary<string, List<EventBase>> itemsByChannel = new Dictionary<string, List<EventBase>>();

            foreach (RssNewsEvent item in items)
            {
                item.Channel = source.GetChannelByName(item.ChannelId, true);
                if (itemsByChannel.ContainsKey(item.ChannelId) == false)
                {
                    itemsByChannel.Add(item.ChannelId, new List<EventBase>());
                }

                itemsByChannel[item.ChannelId].Add(item);
            }

            foreach (string name in itemsByChannel.Keys)
            {
                // Handle the relation to persistence.
                EventSourceChannel channel = source.GetChannelByName(name, false);
                channel.AddItems(itemsByChannel[name]);
            }
        }

        /// <summary>
        /// Helper, obtain a source by its address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected EventSource GetSourceByAddress(string address)
        {
            foreach(EventSource source in base.NewsSourcesArray)
            {
                if (source.Address == address)
                {
                    return source;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(Platform platform)
        {
            SystemMonitor.CheckWarning(NewsSourcesUnsafe.Count == 0, "Manager already has assigned sources.");

            _platform = platform;

            _persistenceHelper = CreatePersistenceHelper(platform.Settings);
            if (_persistenceHelper == null)
            {
                SystemMonitor.Error("Failed to initialize the persistence helper for Platform News Manager.");
                return;
            }

            // Make sure this load is before the accept events, so that they do not cause adding failure.
            List<EventSource> sources = _persistenceHelper.SelectDynamicType<EventSource>(null, null);

            base.SourceAddedEvent += new NewsSourceUpdateDelegate(PlatformNewsManager_SourceAddedEvent);
            base.SourceRemovedEvent += new NewsSourceUpdateDelegate(PlatformNewsManager_SourceRemovedEvent);

			if (sources != null)
			{
				foreach (EventSource source in sources)
				{// Add the source to subscribe to it, than load items.
					base.AddSource(source);

                    if (source.Enabled)
                    {
                        LoadSourceItemsFromPersistence(source);
                    }
                }
			}

            if (platform != null)
            {// Load the default sources.
                foreach (string feedAddress in platform.Settings.DefaultRSSFeeds)
                {
                    if (GetSourceByAddress(feedAddress) == null)
                    {
                        RssNewsSource newSource = new RssNewsSource();
                        newSource.Initialize(feedAddress);

                        AddSource(newSource);
                    }
                }
            }
                

            GeneralHelper.FireAndForget(UpdateFeeds);
        }

        /// <summary>
        /// Handle enable changed, to load items from DB for source (since it may not have any loaded;
        /// implementing an load "On demand" mechanism"); also store change of Enabled to DB.
        /// </summary>
        /// <param name="source"></param>
        protected override void source_EnabledChangedEvent(EventSource source)
        {
            if (_persistenceHelper == null)
            {
                SystemMonitor.OperationWarning("Can not operate, since persistence helper not available.");
                return;
            }

            if (source.Enabled)
            {// Extract items from DB, since it may have none at this point.
                LoadSourceItemsFromPersistence(source);
            }

            // Update source to DB.
            source_PersistenceDataUpdatedEvent(source);

            base.source_EnabledChangedEvent(source);
        }

        /// <summary>
        /// Uninitialize the manager from operation.
        /// </summary>
        public virtual void UnInitialize()
        {
        }

        void PlatformNewsManager_SourceAddedEvent(NewsManager manager, EventSource source)
        {
            if (_persistenceHelper == null)
            {
                SystemMonitor.OperationWarning("Can not operate, since persistence helper not available.");
                return;
            }

            if (source.IsPersistedToDB == false)
            {// Already persisted to DB.
                if (_persistenceHelper.InsertDynamicTyped<EventSource>(source) == false)
                {
                    SystemMonitor.OperationError("Failed to add source to DB.");
                }
            }

            source.PersistenceDataUpdatedEvent += new PersistenceDataUpdatedDelegate(source_PersistenceDataUpdatedEvent);
            source.ItemsAddedEvent += new EventSource.ItemsUpdateDelegate(source_ItemsAddingAcceptEvent);
            source.ItemsUpdatedEvent += new EventSource.ItemsUpdateDelegate(source_ItemsUpdatedEvent);

            // AddElement the items already in the source.
            List<EventBase> items = source.GetAllItemsFlat<EventBase>();
            source_ItemsAddingAcceptEvent(source, items);
        }

        void source_ItemsUpdatedEvent(EventSource source, IEnumerable<EventBase> items)
        {
            if (_persistenceHelper == null)
            {
                SystemMonitor.OperationWarning("Can not operate, since persistence helper not available.");
                return;
            }

            _persistenceHelper.UpdateToDB<EventBase>(items, null);
        }

        void PlatformNewsManager_SourceRemovedEvent(NewsManager manager, EventSource source)
        {
            if (_persistenceHelper == null)
            {
                SystemMonitor.OperationWarning("Can not operate, since persistence helper not available.");
                return;
            }

            if (_persistenceHelper.Delete<EventSource>(new EventSource[] { source }) == false)
            {
                SystemMonitor.OperationError("Failed to delete source from DB.");
            }

            source.PersistenceDataUpdatedEvent -= new PersistenceDataUpdatedDelegate(source_PersistenceDataUpdatedEvent);
            source.ItemsAddedEvent -= new EventSource.ItemsUpdateDelegate(source_ItemsAddingAcceptEvent);
            source.ItemsUpdatedEvent -= new EventSource.ItemsUpdateDelegate(source_ItemsUpdatedEvent);

            _persistenceHelper.Delete<EventSource>(source);

            _persistenceHelper.Delete<EventBase>(new MatchExpression("SourceId", source.Id));
        }

        void source_ItemsAddingAcceptEvent(EventSource source, IEnumerable<EventBase> items)
        {
            if (_persistenceHelper == null)
            {
                SystemMonitor.OperationWarning("Can not operate, since persistence helper not available.");
                return;
            }

            List<EventBase> rssItems = new List<EventBase>();
            foreach (EventBase item in items)
            {
                if (item.IsPersistedToDB == false)
                {
                    rssItems.Add(item);
                }
            }

            if (rssItems.Count > 0)
            {
                _persistenceHelper.InsertDynamicTyped<EventBase>(rssItems, new KeyValuePair<string, object>("SourceId", source.Id));
            }
        }

        void source_PersistenceDataUpdatedEvent(IDBPersistent source)
        {
            if (_persistenceHelper == null)
            {
                SystemMonitor.OperationWarning("Can not operate, since persistence helper not available.");
                return;
            }

            if (_persistenceHelper.UpdateToDB((EventSource)source, null) == false)
            {
                SystemMonitor.OperationError("Failed to update source.");
            }
        }

    }
}
