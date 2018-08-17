using System;
using System.Runtime.Serialization;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Servers as central point for all the classes, controls, etc; related to trading manually.
    /// Extended Advanced version of the manual trading.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Component Trading")]
    public class ComponentTradingExpert : Expert
    {
        volatile ExpertHost _host;
        public ExpertHost Host
        {
            get { return _host; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ComponentTradingExpert(ISourceAndExpertSessionManager sessionManager, string name)
            : base(sessionManager, name)
        {
            _host = (ExpertHost)sessionManager;
        }

        protected override bool OnUnInitialize()
        {
            return base.OnUnInitialize();
        }
    }
}
