using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RequestSymbolsResponseMessage : ResponseMessage
    {
        Dictionary<Symbol, TimeSpan[]> _symbolsPeriods = new Dictionary<Symbol, TimeSpan[]>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Symbol, TimeSpan[]> SymbolsPeriods
        {
            get { return _symbolsPeriods; }
            set { _symbolsPeriods = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationResult"></param>
        public RequestSymbolsResponseMessage(bool operationResult)
            : base(operationResult)
        {
        }
    }
}
