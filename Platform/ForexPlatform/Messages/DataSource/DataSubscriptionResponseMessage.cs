using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DataSubscriptionResponseMessage : DataSessionResponseMessage
    {
        DataSubscriptionInfo _assignedSubscriptionInformation = new DataSubscriptionInfo();

        public DataSubscriptionInfo AssignedSubscriptionInformation
        {
            get { return _assignedSubscriptionInformation; }
            set { _assignedSubscriptionInformation = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSubscriptionResponseMessage(DataSessionInfo sessionInfo, bool operationResult)
            : base(sessionInfo, operationResult)
        { 
        }

    }
}
