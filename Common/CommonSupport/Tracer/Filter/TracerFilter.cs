using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CommonSupport
{
    /// <summary>
    /// Base class for tracer filters. A filter is capable of filtering out trace
    /// items based on some criteria.
    /// </summary>
    public abstract class TracerFilter : ISerializable
    {
        Tracer _tracer;
        public Tracer Tracer
        {
            get { return _tracer; }
        }

        volatile bool _delayedUpdatePending = false;

        public delegate void FilterUpdatedDelegate(TracerFilter filter);
        public event FilterUpdatedDelegate FilterUpdatedEvent;

        #region Instance Control

        /// <summary>
        /// Constructor.
        /// </summary>
        public TracerFilter()
        {
        }

        /// <summary>
        /// Deserialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public TracerFilter(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// Serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Initialize(Tracer tracer)
        {
            if (_tracer != null)
            {
                SystemMonitor.OperationError("Filter already initialized.");
                return false;
            }

            _tracer = tracer;
            return true;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherFilter"></param>
        public virtual void CopyDataFrom(TracerFilter otherFilter)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        internal void PerformDelayedUpdateEvent()
        {
            if (_delayedUpdatePending)
            {
                _delayedUpdatePending = false;
                RaiseFilterUpdatedEvent(false);
            }
        }

        /// <summary>
        /// Raise event related to filter updated.
        /// </summary>
        /// <param name="delayedUpdate">Should we wait for the singaling call, that calls for all updates to be done (delayed update), or raise right away. Delayed usefull to prevent FilterItem lock issues.</param>
        protected void RaiseFilterUpdatedEvent(bool delayedUpdate)
        {
            if (delayedUpdate)
            {
                _delayedUpdatePending = true;
            }
            else
            {
                if (FilterUpdatedEvent != null)
                {
                    FilterUpdatedEvent(this);
                }
            }
        }

        /// <summary>
        /// Will return true if item is allowed to pass filter.
        /// </summary>
        public abstract bool FilterItem(TracerItem item);

    }
}
