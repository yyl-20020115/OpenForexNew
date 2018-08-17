using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Indicate with this attribute that persistance is desired on any child class of IDBPersistent
    /// - if applied on class indicates the default persistency for classes properties.
    /// - if applied to property indicates specific instructions how to map property to DB
    /// </summary>
    public class DBPersistenceAttribute : Attribute
    {
        /// <summary>
        /// The type of persistance to apply.
        /// </summary>
        public enum PersistenceTypeEnum
        {
            None,
            Default,
            Binary
        }

        /// <summary>
        /// The mode of the persistence.
        /// </summary>
        public enum PersistenceModeEnum
        {
            Default, // Both read and write access.
            ReadOnly, // Read only access (get param).
        }

        PersistenceTypeEnum _persistenceType = PersistenceTypeEnum.Default;
        /// <summary>
        /// Type of the persistence.
        /// </summary>
        public PersistenceTypeEnum PersistenceType
        {
            get { return _persistenceType; }
        }

        PersistenceModeEnum _persistenceMode = PersistenceModeEnum.Default;
        /// <summary>
        /// Mode of the persistence.
        /// </summary>
        public PersistenceModeEnum PersistenceMode
        {
            get { return _persistenceMode; }
        }

        /// <summary>
        /// Default persistance constructor.
        /// </summary>
        public DBPersistenceAttribute(bool persist)
        {
            if (persist == false)
            {
                _persistenceType = PersistenceTypeEnum.None;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DBPersistenceAttribute(PersistenceTypeEnum persistType)
        {
            _persistenceType = persistType;
        }

        /// <summary>
        /// 
        /// </summary>
        public DBPersistenceAttribute(PersistenceModeEnum persistenceMode)
        {
            _persistenceMode = persistenceMode;
        }

        /// <summary>
        /// 
        /// </summary>
        public DBPersistenceAttribute(PersistenceTypeEnum persistType, PersistenceModeEnum persistenceMode)
        {
            _persistenceType = persistType;
            _persistenceMode = persistenceMode;
        }

    }
}
