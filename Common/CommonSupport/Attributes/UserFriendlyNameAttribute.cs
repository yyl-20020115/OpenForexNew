using System;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Attribute allows to mark classes with extended user friendly names.
    /// </summary>
    public class UserFriendlyNameAttribute : Attribute
    {
        string _name;
        public string Name
        {
            get { return _name; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public UserFriendlyNameAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        static public string GetTypeAttributeName(Type classType)
        {
            string name = GeneralHelper.SeparateCapitalLetters(classType.Name);
            GetTypeAttributeValue(classType, ref name);
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        static public bool GetTypeAttributeValue(Type classType, ref string name)
        {
            object[] attributes = classType.GetCustomAttributes(typeof(UserFriendlyNameAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                name = ((UserFriendlyNameAttribute)attributes[0]).Name;
                return true;
            }
            return false;
        }
    }
}
