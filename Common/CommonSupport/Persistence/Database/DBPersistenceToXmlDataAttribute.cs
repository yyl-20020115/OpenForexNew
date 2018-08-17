using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Extends the base attribute by instructing
    /// </summary>
    public class DBPersistenceToXmlDataAttribute : DBPersistenceAttribute
    {
        //volatile bool _persistToXmlDataColumn = false;
        ///// <summary>
        ///// Should this field be persisted to the "Data" column, 
        ///// combined with all other fields with the same flag in an XML.
        ///// </summary>
        //public bool PersistToXMLDataColumn
        //{
        //    get { return _persistToXmlDataColumn; }
        //    set { _persistToXmlDataColumn = value; }
        //}

        /// <summary>
        /// 
        /// </summary>
        public DBPersistenceToXmlDataAttribute()
            : base(PersistenceTypeEnum.Default, PersistenceModeEnum.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public DBPersistenceToXmlDataAttribute(PersistenceModeEnum mode)
            : base(PersistenceTypeEnum.Default, mode)
        {
        }
    }
}
