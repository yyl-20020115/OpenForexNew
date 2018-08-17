using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CommonSupport
{
    /// <summary>
    /// Filter applies filtering by item type fo the tracer items passed in.
    /// </summary>
    [Serializable]
    public class TypeTracerFilter : TracerFilter
    {
        Dictionary<TracerItem.TypeEnum, bool> _itemTypes = new Dictionary<TracerItem.TypeEnum, bool>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracer"></param>
        public TypeTracerFilter()
        {
            // Load the basic types.
            Array items = Enum.GetValues(typeof(TracerItem.TypeEnum));
            for (int i = 0; i < items.Length; i++)
            {
                _itemTypes.Add((TracerItem.TypeEnum)items.GetValue(i), true);
            }
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public TypeTracerFilter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Perform filtering of an item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool FilterItem(TracerItem item)
        {
            lock (this)
            {
                foreach (TracerItem.TypeEnum itemType in item.Types)
                {
                    if (_itemTypes[itemType])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool GetItemTypeFiltering(TracerItem.TypeEnum itemType)
        {
            lock (this)
            {
                if (_itemTypes.ContainsKey(itemType))
                {
                    return _itemTypes[itemType];
                }
            }

            SystemMonitor.Warning("Unknown item type introduced.");
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetItemTypesFiltering(TracerItem.TypeEnum[] itemTypes, bool[] filterings)
        {
            bool modified = false;
            lock (this)
            {
                for (int i = 0; i < itemTypes.Length; i++)
                {
                    if (_itemTypes.ContainsKey(itemTypes[i])
                        && _itemTypes[itemTypes[i]] == filterings[i])
                    {
                        continue;
                    }

                    modified = true;
                    _itemTypes[itemTypes[i]] = filterings[i];
                }

            }

            if (modified)
            {
                RaiseFilterUpdatedEvent(false);
            }
        }
    }
}
