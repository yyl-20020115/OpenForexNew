using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    public delegate void PersistenceDataUpdatedDelegate(IDBPersistent persistent);

    /// <summary>
    /// Interface defines the base interface for DBPersistent items,
    /// allowing them to be processed in the custom persistence mechanism.
    /// </summary>
    public interface IDBPersistent
    {
        /// <summary>
        /// Mark not persistenct, since its persistence is automated.
        /// </summary>
        [DBPersistence(false)]
        long? Id { get; set;  }

        /// <summary>
        /// Event raised when item has changed its persistable data.
        /// </summary>
        // public event PersistenceDataUpdatedDelegate PersistenceDataUpdatedEvent;
    
    }
}
