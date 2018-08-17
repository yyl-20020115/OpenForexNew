using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Message allows to obtain the operating parameters of the source.
    /// </summary>
    [Serializable]
    public class GetExecutionSourceParametersMessage : RequestMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public GetExecutionSourceParametersMessage()
        {
        }
    }
}
