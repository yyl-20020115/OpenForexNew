using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using CommonFinancial;
using Arbiter;

namespace ForexPlatform
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SourceContainer
    {
        List<SourceInfo> _sources = new List<SourceInfo>();
        /// <summary>
        /// 
        /// </summary>
        public List<SourceInfo> SourcesUnsafe
        {
            get { return _sources; }
        }

        public List<ComponentId> DataSources
        {
            get
            {
                List<ComponentId> result = new List<ComponentId>();
                lock (this)
                {
                    foreach (SourceInfo source in _sources)
                    {
                        if ((source.SourceType | SourceTypeEnum.DataProvider) == SourceTypeEnum.DataProvider)
                        {
                            result.Add(source.ComponentId);
                        }
                    }
                }
                return result;
            }
        }

        public List<ComponentId> OrderExecutionSources
        {
            get
            {
                List<ComponentId> result = new List<ComponentId>();
                lock (this)
                {
                    foreach (SourceInfo source in _sources)
                    {
                        if ((source.SourceType | SourceTypeEnum.OrderExecution) == SourceTypeEnum.OrderExecution)
                        {
                            result.Add(source.ComponentId);
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SourceInfo[] SourcesArray
        {
            get
            {
                lock (this)
                {
                    return _sources.ToArray();
                }
            }
        }


        public int Count
        {
            get { return _sources.Count; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SourceContainer()
        {
        }

        /// <summary>
        /// Register a new source.
        /// </summary>
        public bool Register(SourceInfo source)
        {
            lock (this)
            {
                // See if this source already added.
                // NOTE: using IComparable on the structure does not work inside the list.
                foreach (SourceInfo info in _sources)
                {
                    if (info.ComponentId == source.ComponentId
                        && info.SourceType == source.SourceType)
                    {
                        return false;
                    }
                }

                _sources.Add(source);
            }

            return true;
        }

        public void Clear()
        {
            lock (this)
            {
                _sources.Clear();
            }
        }

        public bool ContainsComponentId(ComponentId id)
        {
            lock (this)
            {
                foreach (SourceInfo info in _sources)
                {
                    if (info.ComponentId == id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<SourceInfo> SearchSources(SourceTypeEnum? filteringSourceType, bool partialMatch)
        {
            List<SourceInfo> result = new List<SourceInfo>();
            lock (this)
            {
                foreach(SourceInfo source in _sources)
                {
                    if (source.MatchesSearchCritera(filteringSourceType, partialMatch))
                    {
                        result.Add(source);
                    }
                }
            }

            return result;
        }

        public SourceInfo? GetSourceById(ComponentId id)
        {
            List<SourceInfo> ids = GetSourcesById(id);
            if (ids.Count > 0)
            {
                return ids[0];
            }

            return null;
        }

        /// <summary>
        /// Helper, return information for the modes of a given source.
        /// </summary>
        public SourceTypeEnum GetSourceTypeFlags(ComponentId sourceId)
        {
            SourceTypeEnum result = SourceTypeEnum.None;
            lock (this)
            {
                SourceInfo? info = GetSourceById(sourceId);
                if (info.HasValue)
                {
                    return info.Value.SourceType;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<SourceInfo> GetSourcesById(ComponentId id)
        {
            List<SourceInfo> result = new List<SourceInfo>();

            lock (this)
            {
                foreach (SourceInfo source in _sources)
                {
                    if (source.ComponentId == id)
                    {
                        result.Add(source);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UnRegister(SourceTypeEnum? sourceType, TransportInfo info)
        {
            lock (this)
            {
                for (int i = _sources.Count - 1; i >= 0; i--)
                {
                    if (_sources[i].ComponentId == info.OriginalSenderId.Value.Id)
                    {
                        if (sourceType.HasValue == false || sourceType.Value == _sources[i].SourceType)
                        {
                            _sources.RemoveAt(i);
                        }
                    }
                }
            }

            return true;
        }
    }
}
