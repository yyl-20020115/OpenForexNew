using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Base class for elements that persist in the DB trough this mechanism.
    /// </summary>
    [Serializable]
    public abstract class DBPersistent : IDBPersistent
    {
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

        [field:NonSerialized]
        public event PersistenceDataUpdatedDelegate PersistenceDataUpdatedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DBPersistent()
        {
        }

        /// <summary>
        /// Allows the children to invoke the persistency event.
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
