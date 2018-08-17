using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;

namespace ForexPlatform
{
    /// <summary>
    /// Request symbols corresponding to this baseCurrency.
    /// </summary>
    [Serializable]
    public class RequestSymbolsMessage : RequestMessage
    {
        int _resultLimit = 200;
        /// <summary>
        /// Return a maximum of this number symbols matching the criteria.
        /// </summary>
        public int ResultLimit
        {
            get { return _resultLimit; }
            set { _resultLimit = value; }
        }

        string _symbolMatch = "";
        /// <summary>
        /// Match criteria.
        /// </summary>
        public string SymbolMatch
        {
            get { return _symbolMatch; }
            set { _symbolMatch = value; }
        }
    }
}
