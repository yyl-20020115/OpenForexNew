using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;

namespace ForexPlatform
{
    /// <summary>
    /// Responce requestMessage to dataDelivery source baseCurrency compatibility query.
    /// </summary>
    [Serializable]
    public class GetDataSourceSymbolCompatibleResponseMessage : ResponseMessage
    {
        int _compatibilityLevel = 0;
        /// <summary>
        /// The higher the compatibility level, the more compatible it is.
        /// Native order execution source for the dataDelivery source must be highest compatibility.
        /// </summary>
        public int CompatibilityLevel
        {
            get { return _compatibilityLevel; }
            set { _compatibilityLevel = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCompatible
        {
            get { return _compatibilityLevel != 0; }
            set { _compatibilityLevel = 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        public GetDataSourceSymbolCompatibleResponseMessage(bool operationResult)
            : base(operationResult)
        {
        }
    }
}
