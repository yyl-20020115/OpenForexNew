using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Assign attribute to control classes that need to join the shortcut handling system.
    /// </summary>
    public class AutomatedControlEventHandlerAttribute : Attribute
    {
        Type[] _handlerClassTypes;

        /// <summary>
        /// 
        /// </summary>
        public Type[] HandlerClassTypes
        {
            get { return _handlerClassTypes; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerClassType"></param>
        public AutomatedControlEventHandlerAttribute(Type handlerClassType)
        {
            SystemMonitor.CheckThrow(handlerClassType.IsSubclassOf(typeof(AutomatedControlEventHandler)), "Only shortcut handler classes can be used as attribute parameter.");

            _handlerClassTypes = new Type[] { handlerClassType };
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handlerClassType"></param>
        public AutomatedControlEventHandlerAttribute(Type[] handlerClassTypes)
        {
            foreach (Type handlerClassType in handlerClassTypes)
            {
                SystemMonitor.CheckThrow(handlerClassType.IsSubclassOf(typeof(AutomatedControlEventHandler)), "Only shortcut handler classes can be used as attribute parameter.");
            }

            _handlerClassTypes = handlerClassTypes;
        }

        /// <summary>
        /// Helper, extract Attributes types values from a class.
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public static Type[] GetClassHandlerTypes(Type classType)
        {
            object[] attributes = classType.GetCustomAttributes(false);
            List<Type> result = new List<Type>();

            foreach (object attribute in attributes)
            {
                if (attribute is AutomatedControlEventHandlerAttribute)
                {
                    result.AddRange(((AutomatedControlEventHandlerAttribute)attribute).HandlerClassTypes);
                }
            }

            return result.ToArray();
        }
    }
}
