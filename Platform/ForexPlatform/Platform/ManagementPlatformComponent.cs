using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ForexPlatform
{
    /// <summary>
    /// Base class for tradeEntities components.
    /// </summary>
    [Serializable]
    public abstract class ManagementPlatformComponent : PlatformComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleThreadMode"></param>
        public ManagementPlatformComponent(bool singleThreadMode)
            : base(singleThreadMode)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ManagementPlatformComponent(string name, bool singleThreadMode)
            : base(name, singleThreadMode)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ManagementPlatformComponent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


    }
}
