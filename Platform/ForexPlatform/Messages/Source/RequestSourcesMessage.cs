using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RequestSourcesMessage : RequestMessage
    {
        SourceTypeEnum? _sourceType = null;

        public SourceTypeEnum? SourceType
        {
            get { return _sourceType; }
            set { _sourceType = value; }
        }

        bool _partialMatch = false;

        public bool PartialMatch
        {
            get { return _partialMatch; }
            set { _partialMatch = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public RequestSourcesMessage()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public RequestSourcesMessage(SourceTypeEnum? sourceType, bool partialMatch)
        {
            _sourceType = sourceType;
            _partialMatch = partialMatch;
        }
    }
}
