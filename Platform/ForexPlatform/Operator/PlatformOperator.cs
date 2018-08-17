using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;
using System.Runtime.Serialization;

namespace ForexPlatform
{
    /// <summary>
    /// Class serves as basis for all platform operators. Operators are modules that extend the functionalities
    /// of the platform, like entry points for integrations for ex.
    /// </summary>
    [Serializable]
    public abstract class PlatformOperator : PlatformComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public PlatformOperator(bool singleThreadMode)
            : base(singleThreadMode)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="context"></param>
        public PlatformOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
