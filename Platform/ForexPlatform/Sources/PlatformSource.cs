using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Base class for custom (dataDelivery/order) sources.
    /// </summary>
    [Serializable]
    [ComponentManagement(false, true, 30, true)]
    public abstract class PlatformSource : PlatformComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public PlatformSource()
            : base(false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformSource(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
