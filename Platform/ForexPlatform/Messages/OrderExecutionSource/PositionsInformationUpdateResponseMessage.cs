using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;
using ForexPlatform;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Responding requestMessage, contains information for orders.
    /// </summary>
    [Serializable]
    public class PositionsInformationUpdateResponseMessage : AccountResponseMessage
    {
        PositionInfo[] _positionsInformations;

        public PositionInfo[] PositionsInformations
        {
            get { return _positionsInformations; }
            set { _positionsInformations = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PositionsInformationUpdateResponseMessage(AccountInfo accountInfo, 
            PositionInfo[] positionsInformations, bool operationResult)
            : base(accountInfo, operationResult)
        {
            _positionsInformations = positionsInformations;
        }
    }
}
