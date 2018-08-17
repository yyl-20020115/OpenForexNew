using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Timers;

namespace CommonSupport
{
    /// <summary>
    /// Manages news delivery. Implements custom serialization procedure.
    /// Implements custom serialization routine.
    /// </summary>
    [Serializable]
    public class NewsManager : ISerializable, IDisposable
    {
        Timer _updateTimer = new Timer();

        /// <summary>
        /// 
        /// </summary>
        public bool AutoUpdateEnabled
        {
            get { return _updateTimer.Enabled; }
            set { _updateTimer.Enabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan AutoUpdateInterval
        {
            get 
            {
                return TimeSpan.FromMilliseconds(_updateTimer.Interval);
            }

            set
            {
                _updateTimer.Interval = value.TotalMilliseconds;
            }
        }

        volatile bool _isUpdating = false;
        /// <summary>
        /// Is the manager updating its sources.
        /// </summary>
        public bool IsUpdating
        {
            get { return _isUpdating; }
        }

        List<EventSource> _newsSources = new List<EventSource>();
        /// <summary>
        /// A read only collections (thread unsafe, lock manager) of news sources
        /// assigned to this manager.
        /// </summary>
        public ReadOnlyCollection<EventSource> NewsSourcesUnsafe
        {
            get { lock (this) { return _newsSources.AsReadOnly(); } }
        }

        /// <summary>
        /// An array of the news sources assigned to this manager.
        /// </summary>
        public EventSource[] NewsSourcesArray
        {
            get { lock (this) { return _newsSources.ToArray(); } }
        }

        #region Delegates and Events 

        public delegate void GeneralUpdateDelegate(NewsManager manager);
        public delegate void NewsSourceUpdateDelegate(NewsManager manager, EventSource source);
        public delegate void NewsSourceItemsUpdateDelegate(NewsManager manager, EventSource source, IEnumerable<EventBase> events);
        
        public event NewsSourceUpdateDelegate SourceAddedEvent;
        public event NewsSourceUpdateDelegate SourceRemovedEvent;

        public event NewsSourceItemsUpdateDelegate SourceItemsAddedEvent;
        public event NewsSourceItemsUpdateDelegate SourceItemsUpdatedEvent;

        public event GeneralUpdateDelegate UpdatingStartedEvent;
        public event GeneralUpdateDelegate UpdatingFinishedEvent;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public NewsManager()
        {
            // Update every 2 minutes.
            _updateTimer.Interval = 1000 * 60 * 2;
            _updateTimer.Enabled = true;
            _updateTimer.Elapsed += new ElapsedEventHandler(_updateTimer_Elapsed);
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public NewsManager(SerializationInfo info, StreamingContext context)
        {
            _updateTimer.Interval = info.GetInt64("timerInterval");
            _updateTimer.Enabled = info.GetBoolean("timerEnabled");

            _updateTimer.Elapsed += new ElapsedEventHandler(_updateTimer_Elapsed);
        }

        public virtual void Dispose()
        {
        }

        #region ISerializable Members

        /// <summary>
        /// Serialization baseMethod.
        /// </summary>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("timerInterval", _updateTimer.Interval);
            info.AddValue("timerEnabled", _updateTimer.Enabled);
        }

        #endregion

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateFeeds();
        }

        /// <summary>
        /// Add a source to this manager.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual bool AddSource(EventSource source)
        {
            lock (this)
            {
                if (_newsSources.Contains(source) || string.IsNullOrEmpty(source.Address))
                {// Already contained or invalid address.
                    return false;
                }

                foreach (EventSource iteratedSource in _newsSources)
                {
                    if (iteratedSource.Address == source.Address)
                    {// A source with this address already exists.
                        return false;
                    }
                }

                _newsSources.Add(source);
            
                source.ItemsAddedEvent += new EventSource.ItemsUpdateDelegate(source_ItemsAddedEvent);
                source.ItemsUpdatedEvent += new EventSource.ItemsUpdateDelegate(source_ItemsUpdatedEvent);
                source.EnabledChangedEvent += new EventSource.EnabledChangedDelegate(source_EnabledChangedEvent);
            }

            if (SourceAddedEvent != null)
            {
                SourceAddedEvent(this, source);
            }

            return true;
        }

        void source_ItemsAddedEvent(EventSource source, IEnumerable<EventBase> items)
        {
            if (SourceItemsAddedEvent != null)
            {
                SourceItemsAddedEvent(this, source, items);
            }
        }

        void source_ItemsUpdatedEvent(EventSource source, IEnumerable<EventBase> items)
        {
            if (SourceItemsUpdatedEvent != null)
            {
                SourceItemsUpdatedEvent(this, source, items);
            }
        }

        public virtual bool RemoveSource(EventSource source)
        {
            lock (this)
            {
                if (_newsSources.Remove(source) == false)
                {// Not found.
                    return false;
                }

                source.ItemsAddedEvent -= new EventSource.ItemsUpdateDelegate(source_ItemsAddedEvent);
                source.ItemsUpdatedEvent -= new EventSource.ItemsUpdateDelegate(source_ItemsUpdatedEvent);
                source.EnabledChangedEvent -= new EventSource.EnabledChangedDelegate(source_EnabledChangedEvent);
            }

            if (SourceRemovedEvent != null)
            {
                SourceRemovedEvent(this, source);
            }

            return true;
        }

        protected virtual void source_EnabledChangedEvent(EventSource source)
        {

        }

        /// <summary>
        /// Perform the actual update of feeds.
        /// </summary>
        public void UpdateFeeds()
        {
            if (_isUpdating)
            {// Already updating.
                return;
            }

            _isUpdating = true;

            EventSource[] sources = NewsSourcesArray;

            if (UpdatingStartedEvent != null)
            {
                UpdatingStartedEvent(this);
            }

            foreach (EventSource source in sources)
            {
                try
                {
                    source.Update();
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationWarning("Failed to update news source ["+ source.Name +", " + ex.Message +"].");
                }
            }

            _isUpdating = false;

            if (UpdatingFinishedEvent != null)
            {
                UpdatingFinishedEvent(this);
            }
        }

    }
}
