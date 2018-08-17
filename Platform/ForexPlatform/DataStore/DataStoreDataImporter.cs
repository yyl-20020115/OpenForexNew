using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Helper class, helps convert data to the data store suitable formats,
    /// so that it can be stored etc.
    /// </summary>
    public abstract class DataStoreDataImporter
    {
        DataStore _dataStore;

        /// <summary>
        /// 
        /// </summary>
        public DataStoreDataImporter(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public void Quote(string sessionName, DateTime? sessionStart, Quote quote)
        {

        }
    }
}
