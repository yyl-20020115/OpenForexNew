using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Send requestMessage to any platform component to instruct it to uninitialize.
    /// </summary>
    [Serializable]
    public class UnInitializeComponentMessage : TransportMessage
    {
        ComponentId? _componentId;

        public ComponentId? ComponentId
        {
            get { return _componentId; }
        }

        /// <summary>
        /// 
        /// </summary>
        public UnInitializeComponentMessage()
        {
        }

        /// <summary>
        /// If the uninitializer is not the component itself, specify the componentId
        /// </summary>
        public UnInitializeComponentMessage(ComponentId? componentId)
        {
            _componentId = componentId;
        }
    }
}
