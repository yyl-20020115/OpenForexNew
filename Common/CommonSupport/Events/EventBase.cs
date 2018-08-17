using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommonSupport
{
    /// <summary>
    /// Base class for processable events (occurences).
    /// Not related to .NET events in any way.
    /// </summary>
    [Serializable]
    public abstract class EventBase : DBPersistent, IComparable
    {
        protected volatile string _eventId = string.Empty;

        /// <summary>
        /// Id of the event (optional), may be Empty.
        /// </summary>
        [DBPersistence(true)]
        public string EventId
        {
            get { return _eventId; }
            set { _eventId = value; }
        }

        protected volatile string _title = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [DBPersistence(true)]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        protected volatile string _description = string.Empty;

        /// <summary>
        /// Description of this event (optional).
        /// </summary>
        [DBPersistence(true)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        protected DateTime? _dateTime = null;
        /// <summary>
        /// Date and time this event occured.
        /// </summary>
        [DBPersistence(true)]
        public DateTime? DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        protected DateTime? _endDateTime = null;
        /// <summary>
        /// Applicable for lasting events, that have a period.
        /// </summary>
        [DBPersistence(true)]
        public DateTime? EndDateTime
        {
            get { return _endDateTime; }
            set { _endDateTime = value; }
        }

		protected volatile bool _isVisible = true;
        /// <summary>
        /// 
        /// </summary>
        [DBPersistence(true)]
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

		protected volatile bool _isRead = false;
        /// <summary>
        /// Has this item been read.
        /// </summary>
        [DBPersistence(true)]
        public bool IsRead
        {
            get { return _isRead; }
            set { _isRead = value; }
        }

		protected volatile bool _isFavourite = false;
        /// <summary>
        /// Is this item a favourite item (optional).
        /// </summary>
        [DBPersistence(true)]
        public bool IsFavourite
        {
            get { return _isFavourite; }
            set { _isFavourite = value; }
        }

		protected volatile string _link;
        /// <summary>
        /// Link of this event, optional and usully used when event is an internet/network item.
        /// </summary>
        [DBPersistence(true)]
        public string Link
        {
            get { return _link; }
            set { _link = value; }
        }

        /// <summary>
        /// Helper property, allows to consume the link as Uri.
        /// </summary>
        [DBPersistence(false)]
        public Uri LinkUri
        {
            get { return new Uri(_link); }
        }

		protected long _sourceId = 0;

        /// <summary>
        /// Id of the source of the channel of this event.
        /// </summary>
        [DBPersistence(true)]
        public long SourceId
        {
            get
            {
                return GeneralHelper.AtomicRead(ref _sourceId);
            }

            set 
            {
                Interlocked.Exchange(ref _sourceId, value);
            }
        }

        protected volatile string _channelId = string.Empty;
        /// <summary>
        /// Id of the channel of this event.
        /// </summary>
        [DBPersistence(true)]
        public string ChannelId
        {
            get { return _channelId; }
            set { _channelId = value; }
        }

        /// <summary>
        /// Applicable for lasting events, that have a period.
        /// </summary>
        [DBPersistence(false)]
        public TimeSpan? TimeSpan
        {
            get
            {
                if (_dateTime.HasValue && _endDateTime.HasValue)
                {
                    return _endDateTime - _dateTime;
                }

                return null;
            }
        }

		protected volatile EventSourceChannel _channel;
        /// <summary>
        /// The channel this item belongs to.
        /// </summary>
        [DBPersistence(false)]
        public EventSourceChannel Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EventBase()
        {
        }

        /// <summary>
        /// Initialize item to be part of this channel.
        /// </summary>
        /// <param name="source"></param>
        public void Initialize(EventSourceChannel channel)
        {
            _channel = channel;
            _channelId = channel.Name;
        }

        /// <summary>
        /// Obtain a full id of this item (uniquely identifying the item).
        /// </summary>
        public string GetFullId()
        {
            return 
                _dateTime.HasValue == false || _dateTime.Value == System.DateTime.MinValue ? "0" : _dateTime.Value.ToFileTime() 
                + "." + _eventId == null ? string.Empty : _eventId 
                + "." + _title == null ? string.Empty : _title 
                + "." + _link == null ? string.Empty : _link 
                + "." + ChannelId;
        }

        ///// <summary>
        ///// Obtain event full id, consisting of date time and event id and some other parts,
        ///// and calculate the hash of it.
        ///// </summary>
        ///// <returns></returns>
        //protected virtual int GetFullIdHash()
        //{
        //    return GetFullId().GetHashCode();
        //}

        /// <summary>
        /// 
        /// </summary>
        public virtual int CompareTo(object other)
        {
            if (other is EventBase == false)
            {
                throw new NotImplementedException();
            }

            EventBase otherEvent = other as EventBase;

            int result = 0;
            if (_dateTime.HasValue && otherEvent._dateTime.HasValue)
            {
                result = _dateTime.Value.CompareTo(otherEvent._dateTime.Value);
            }

            if (result != 0)
            {
                return result;
            }

            return GetFullId().CompareTo(otherEvent.GetFullId());

            //int compare = 0;

            //if (_dateTime.HasValue != other.DateTime.HasValue)
            //{
            //    return _dateTime.HasValue ? 1 : -1;
            //}

            //if (_dateTime.HasValue && other.DateTime.HasValue)
            //{
            //    _dateTime.Value.CompareTo(other.DateTime);
            //}

            //if (compare != 0)
            //{
            //    return compare;
            //}

            //compare = _description.CompareTo(other.Description);
            //if (compare != 0)
            //{
            //    return compare;
            //}
            //compare = _title.CompareTo(other.Title);
            //if (compare != 0)
            //{
            //    return compare;
            //}
            //compare = Uri.AbsolutePath.CompareTo(other.Uri.AbsolutePath);
            //return compare;

            //throw new NotImplementedException();
        }

    }
}
