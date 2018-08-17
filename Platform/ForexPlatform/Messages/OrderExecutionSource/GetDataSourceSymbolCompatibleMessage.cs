using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Use requestMessage to ask if a source is compatible with provided sessionInformation.
    /// </summary>
    [Serializable]
    public class GetDataSourceSymbolCompatibleMessage : RequestMessage
    {
        Symbol _symbol;
        public Symbol Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        ComponentId _dataSourceId;
        public ComponentId DataSourceId
        {
            get { return _dataSourceId; }
            set { _dataSourceId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public GetDataSourceSymbolCompatibleMessage(ComponentId dataSourceId, Symbol symbol)
        {
            _symbol = symbol;
            _dataSourceId = dataSourceId;
        }
    }
}
