using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CommonSupport
{
    /// <summary>
    /// Filter allows to filter items based on an Enum type input.
    /// </summary>
    [Serializable]
    public class EnumItemTracerFilter : TracerFilter
    {
        [Serializable]
        class EnumStruct
        {
            public Type EnumType;
            public bool[] Values;
        }

        ListUnique<Assembly> _assemblies = new ListUnique<Assembly>();

        List<EnumStruct> _allEnums = new List<EnumStruct>();

        List<EnumStruct> _filteredOutEnums = new List<EnumStruct>();
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public EnumItemTracerFilter()
        {
            // Find all candidate enum types and assemblies they reside in.
            List<Type> possibleEnumTypes = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(typeof(Enum), ReflectionHelper.GetApplicationEntryAssemblyAndReferencedAssemblies());
            for (int i = possibleEnumTypes.Count - 1; i >= 0; i--)
            {
                object[] attributes = possibleEnumTypes[i].GetCustomAttributes(typeof(TracerEnumAttribute), true);
                if (attributes == null || attributes.Length == 0)
                {
                    possibleEnumTypes.RemoveAt(i);
                }
                else
                {
                    _assemblies.Add(possibleEnumTypes[i].Assembly);
                }
            }
        }
        
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public EnumItemTracerFilter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }

        static EnumStruct GetEnumStruct(List<EnumStruct> list, Type type)
        {
            foreach (EnumStruct enumStruct in list)
            {
                if (enumStruct.EnumType == type)
                {
                    return enumStruct;
                }
            }
            return null;
        }

        public void SetEnumTypeFiltered(Type enumType, bool filtered)
        {

            EnumStruct enumStruct;
            lock (this)
            {
               enumStruct = GetEnumStruct(_allEnums, enumType);
            }

            if (enumStruct == null)
            {
                enumStruct = new EnumStruct();
                enumStruct.EnumType = enumType;
                enumStruct.Values = new bool[Enum.GetNames(enumType).Length];
                for (int i = 0; i < enumStruct.Values.Length; i++)
                {
                    enumStruct.Values[i] = true;
                }

                lock (this)
                {
                    _allEnums.Add(enumStruct);
                }
            }

            lock (this)
            {
                if (filtered)
                {
                    if (_filteredOutEnums.Contains(enumStruct) == false)
                    {
                        _filteredOutEnums.Add(enumStruct);
                    }
                }
                else
                {
                    _filteredOutEnums.Remove(enumStruct);
                }
            }

            RaiseFilterUpdatedEvent(false);
        }

        public bool IsEnumFiltered(Type enumType)
        {
            lock (this)
            {
                return GetEnumStruct(_filteredOutEnums, enumType) != null;
            }
        }

        public bool IsEnumValueFiltered(Type enumType, int valueIndex)
        {
            lock (this)
            {
                EnumStruct enumStruct = GetEnumStruct(_allEnums, enumType);
                return enumStruct.Values[valueIndex];
            }
        }

        public void SetEnumTypeValueFiltered(Type enumType, int valueIndex, bool filtered)
        {
            lock (this)
            {
                EnumStruct enumStruct = GetEnumStruct(_allEnums, enumType);
                enumStruct.Values[valueIndex] = filtered;
            }

            RaiseFilterUpdatedEvent(false);
        }

        public override bool FilterItem(TracerItem item)
        {
            if (item is EnumTracerItem == false)
            {
                return true;
            }

            lock (this)
            {
                foreach (EnumStruct enumStruct in _filteredOutEnums)
                {
                    if (((EnumTracerItem)item).EnumType == enumStruct.EnumType)
                    {
                        return false;
                    }
                }

                EnumStruct enumStruct2 = GetEnumStruct(_allEnums, ((EnumTracerItem)item).EnumType);
                if (enumStruct2 != null)
                {
                    return enumStruct2.Values[((EnumTracerItem)item).ValueIndex];
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
