using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Message allows obtaining information of orders.
    /// </summary>
    [Serializable]
    public class PositionsInformationMessage : AccountRequestMessage
    {
        Symbol[] _positionsSymbols = null;
        public Symbol[] PositionsSymbols
        {
            get { return _positionsSymbols; }
            set { _positionsSymbols = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PositionsInformationMessage(AccountInfo info, Symbol[] positionsSymbols)
            : base(info)
        {
            _positionsSymbols = positionsSymbols;
        }
    }
}
