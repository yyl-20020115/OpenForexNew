using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Message notifies of an update to sources.
    /// </summary>
    [Serializable]
    public class SourcesUpdateMessage : ResponseMessage
    {
        List<SourceInfo> _sources = new List<SourceInfo>();
        
        /// <summary>
        /// 
        /// </summary>
        public List<SourceInfo> Sources
        {
            get { return _sources; }
            set { _sources = value; }
        }

        public List<ComponentId> SourcesIds
        {
            get
            {
                List<ComponentId> ids = new List<ComponentId>();
                foreach (SourceInfo info in _sources)
                {
                    ids.Add(info.ComponentId);
                }

                return ids;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public SourcesUpdateMessage()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public SourcesUpdateMessage(List<SourceInfo> sources, bool operationResult)
            : base(operationResult)
        {
            _sources = sources;
        }
    }
}
