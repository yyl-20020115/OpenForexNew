using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Operation responce requestMessage, that is related to an operation on a given sessionInformation.
    /// </summary>
    [Serializable]
    public class DataSessionResponseMessage : ResponseMessage
    {
        DataSessionInfo _sessionInfo;
        public DataSessionInfo SessionInfo
        {
            get { return _sessionInfo; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSessionResponseMessage(DataSessionInfo sessionInfo, bool operationResult)
            : base(operationResult)
        {
            _sessionInfo = sessionInfo;
        }

    }
}
