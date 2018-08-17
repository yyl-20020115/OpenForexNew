using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CommonSupport
{
    public class EnumTracerItem : TracerItem
    {
        Enum _enumValue;
        public Type EnumType
        {
            get { return _enumValue.GetType(); }
        }

        public override Assembly Assembly
        {
            get { return EnumType.Assembly; }
        }

        public string ValueName
        {
            get { return _enumValue.ToString(); }
        }

        /// <summary>
        /// Since Enum supports IConvertible, this is the proper way to convert.
        /// Also possible is this (int)(object)(_enumCode)
        /// </summary>
        public int ValueIndex
        {
            get { return Convert.ToInt32(_enumValue); }
        }

        /// <summary>
        /// 
        /// </summary>
        public EnumTracerItem(TypeEnum itemType, TracerItem.PriorityEnum priority, string message, Enum enumValue)
            : base(itemType, priority, message)
        {
            _enumValue = enumValue;
        }

    }
}
