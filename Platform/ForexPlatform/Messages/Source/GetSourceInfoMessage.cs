using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GetSourceInfoMessage : RequestMessage
    {
        ComponentId _id;
        public ComponentId Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public GetSourceInfoMessage(ComponentId id)
        {
            _id = id;
        }
    }
}
