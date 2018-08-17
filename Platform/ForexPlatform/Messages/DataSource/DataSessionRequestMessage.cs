using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Base class for operation messages, related to a given sessionInformation.
    /// </summary>
    [Serializable]
    public class DataSessionRequestMessage : RequestMessage
    {
        DataSessionInfo _sessionInfo;
        public DataSessionInfo SessionInfo
        {
            get { return _sessionInfo; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSessionRequestMessage(DataSessionInfo sessionInfo)
        {
            _sessionInfo = sessionInfo;
        }
    }
}
