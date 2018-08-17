using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using Arbiter;

namespace ForexPlatform
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RequestSymbolsRuntimeInformationMessage : RequestMessage
    {
        Symbol[] _symbols = new Symbol[] { };

        public Symbol[] Symbols
        {
            get { return _symbols; }
            set { _symbols = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public RequestSymbolsRuntimeInformationMessage(Symbol[] symbols)
        {
            _symbols = symbols;
        }
    }
}
