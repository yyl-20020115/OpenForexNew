using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Message requests the delivery of historical bar/tick values.
    /// </summary>
    [Serializable]
    public class RequestDataHistoryMessage : DataSessionRequestMessage
    {
        DataHistoryRequest _request;

        public DataHistoryRequest Request
        {
            get { return _request; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expertID"></param>
        public RequestDataHistoryMessage(DataSessionInfo sessionInfo, DataHistoryRequest request)
            : base(sessionInfo)
        {
            _request = request;
        }
    }
}
