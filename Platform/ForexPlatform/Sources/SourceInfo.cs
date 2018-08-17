using System;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Structure contains information regarding a source. A source can be dataDelivery or order execution and 
    /// to consume its capabilities a messaging mechanism is used.
    /// </summary>
    [Serializable]
    public struct SourceInfo
    {
        SourceTypeEnum _sourceType;
        /// <summary>
        /// Type description of this source.
        /// </summary>
        public SourceTypeEnum SourceType
        {
            get { return _sourceType; }
        }

        TransportInfo _transportInfo;
        /// <summary>
        /// Messaging transportation information to the source.
        /// </summary>
        public TransportInfo TransportInfo
        {
            get { return _transportInfo; }
        }

        ComponentId _componentId;
        /// <summary>
        /// Id of the component for this source.
        /// </summary>
        public ComponentId ComponentId
        {
            get
            {
                return _componentId;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SourceInfo(SourceTypeEnum sourceType, TransportInfo info)
        {
            _sourceType = sourceType;
            _transportInfo = info;
            _componentId = _transportInfo.OriginalSenderId.Value.Id;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SourceInfo(ComponentId componentId, SourceTypeEnum sourceType)
        {
            _componentId = componentId;
            _transportInfo = null;
            _sourceType = sourceType;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool MatchesSearchCritera(SourceTypeEnum? filteringSourceType, bool partialMatch)
        {
            if (filteringSourceType.HasValue == false || SourceType == filteringSourceType)
            {// Full match.
                return true;
            }
            else if (partialMatch)
            {// Look for partial match.
                if ((SourceType & filteringSourceType) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        //#region IComparable<SourceInfo> Members

        //public int CompareTo(SourceInfo other)
        //{
        //    return this.ComponentId.CompareTo(other.ComponentId);
        //}

        //#endregion
    }
}
