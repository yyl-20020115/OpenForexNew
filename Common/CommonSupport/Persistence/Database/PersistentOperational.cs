using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport.Events
{
    /// <summary>
    /// Base abstract class for elements that need to be both operational and IDBPersistent.
    /// </summary>
    public abstract class PersistentOperational : Operational, IDBPersistent
    {
        #region IDBPersistent

        long? _id = null;
        /// <summary>
        /// DB id of this item.
        /// </summary>
        [DBPersistence(true)]
        public long? Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Has the object been persisted to DB yet.
        /// </summary>
        public bool IsPersistedToDB
        {
            get { return _id.HasValue; }
        }

        [field: NonSerialized]
        public event PersistenceDataUpdatedDelegate PersistenceDataUpdatedEvent;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public PersistentOperational()
        {
        }

        /// <summary>
        /// Helper.
        /// </summary>
        protected void RaisePersistenceDataUpdatedEvent()
        {
            if (PersistenceDataUpdatedEvent != null)
            {
                PersistenceDataUpdatedEvent(this);
            }
        }

    }
}
