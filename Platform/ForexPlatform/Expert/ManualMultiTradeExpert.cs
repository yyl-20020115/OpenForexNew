using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommonSupport;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Manual trading on multiple sessions at the same time.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Multi Pair Manual Trading")]
    public class ManualMultiTradeExpert : Expert
    {
        /// <summary>
        /// 
        /// </summary>
        public ManualMultiTradeExpert(ISourceAndExpertSessionManager sessionManager, string name)
            : base(sessionManager, name)
        {
        }

    }
}
