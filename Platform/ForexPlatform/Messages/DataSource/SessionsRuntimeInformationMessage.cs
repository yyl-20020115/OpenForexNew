using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using Arbiter;

namespace ForexPlatform
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SessionsRuntimeInformationMessage : ResponseMessage
    {
        List<RuntimeDataSessionInformation> _informations;
        public List<RuntimeDataSessionInformation> Informations
        {
          get { return _informations; }
          set { _informations = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public SessionsRuntimeInformationMessage(List<RuntimeDataSessionInformation> informations, bool operationResult)
            : base(operationResult)
        {
            _informations = informations;
        }
    }
}
