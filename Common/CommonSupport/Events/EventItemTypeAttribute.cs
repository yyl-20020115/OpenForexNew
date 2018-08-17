using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Specify the type of news item the source uses.
    /// </summary>
    public class EventItemTypeAttribute : Attribute
    {
        Type _type;
        public Type TypeValue
        {
            get { return _type; }
        }

        /// <summary>
        /// 
        /// </summary>
        public EventItemTypeAttribute(Type type)
        {
            _type = type;
        }
    }
}
