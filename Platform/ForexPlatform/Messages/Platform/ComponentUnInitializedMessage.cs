using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;

namespace ForexPlatform
{
    [Serializable]
    public class ComponentUnInitializedMessage : TransportMessage
    {
        ArbiterClientId _componentId;
        /// <summary>
        /// Arbiter Id of the unregistered component.
        /// </summary>
        public ArbiterClientId ComponentId
        {
            get { return _componentId; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ComponentUnInitializedMessage(ArbiterClientId componentId)
        {
            _componentId = componentId;
        }
    }
}
