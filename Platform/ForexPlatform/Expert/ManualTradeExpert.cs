using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommonSupport;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Servers as central point for all the classes, controls, etc; related to trading manually.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Chart & Trade")]
    public class ManualTradeExpert : Expert
    {
        /// <summary>
        /// 
        /// </summary>
        public ManualTradeExpert(ISourceAndExpertSessionManager sessionManager, string name)
            : base(sessionManager, name)
        {
            sessionManager.SessionCreatedEvent += new GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager, ExpertSession>(sessionManager_SessionCreatedEvent);
        }

        void sessionManager_SessionCreatedEvent(ISourceManager parameter1, ExpertSession parameter2)
        {
            _name = base.Manager.SessionsArray[0].Info.Name;
        }

    }
}
