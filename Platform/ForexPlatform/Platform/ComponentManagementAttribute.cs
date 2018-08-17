using System;
using System.Collections.Generic;
using System.Text;

namespace ForexPlatform
{
    /// <summary>
    /// Controls how a platform component is managed by the Platform.
    /// </summary>
    public class ComponentManagementAttribute : Attribute
    {
        int? _componentLevel = null;
        /// <summary>
        /// ComponentLevel is a per class type property. Separate component classes must override and provide
        /// their levels.
        /// Each component in a platform is a cetain level. Lower level shows this component must be started
        /// first and stopped last. Higher level means component relies on more of the other components, and is 
        /// started later on.
        /// The model is higher levels depend on components from lower levels. This differentiation is needed to make
        /// sure underlying components are started before those using them, and stopped after that.
        /// By default the value provided is highest level, meaning this component is not being depended upon by no other.
        /// </summary>
        public int ComponentLevel
        {
            get 
            {
                if (_componentLevel.HasValue)
                {
                    return _componentLevel.Value;
                }
                return int.MaxValue;
            }
        }

        bool? _multipleInstancesAllowed = null;

        /// <summary>
        /// Are multiple instances allowed of this componend, in one platform.
        /// </summary>
        public bool AllowMultipleInstances
        {
            get
            {
                if (_multipleInstancesAllowed.HasValue)
                {
                    return _multipleInstancesAllowed.Value && (_isMandatory.HasValue == false || _isMandatory.Value == false);
                }

                return true;
            }
        }

        bool? _isMandatory = null;
        /// <summary>
        /// Mandatory components are always created in a platform and are vital for operation.
        /// </summary>
        public bool IsMandatory
        {
            get 
            {
                if (_isMandatory.HasValue)
                {
                    return _isMandatory.Value;
                }

                return false;
            }
        }

        bool _requestPreStartSetup = true;
        /// <summary>
        /// Should the component be shown a properties box allowing to set up parameters before adding to platform.
        /// </summary>
        public bool RequestPreStartSetup
        {
            get { return _requestPreStartSetup; }
            set { _requestPreStartSetup = value; }
        }

        /// <summary>
        /// Default values for all.
        /// </summary>
        public ComponentManagementAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ComponentManagementAttribute(int componentLevel, bool multipleInstancesAllowed)
        {
            _componentLevel = componentLevel;
            _multipleInstancesAllowed = multipleInstancesAllowed;
        }

        /// <summary>
        /// 
        /// </summary>
        public ComponentManagementAttribute(bool mandatory, bool requestPreStartSetup, int componentLevel, bool multipleInstancesAllowed)
        {
            _requestPreStartSetup = requestPreStartSetup;
            _isMandatory = mandatory;
            _componentLevel = componentLevel;
            _multipleInstancesAllowed = multipleInstancesAllowed;
        }

        /// <summary>
        /// Will always return an instance (with default values if none assigned).
        /// </summary>
        public static ComponentManagementAttribute GetTypeAttribute(Type classType)
        {
            object[] attributes = classType.GetCustomAttributes(typeof(ComponentManagementAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return (ComponentManagementAttribute)attributes[0];
            }

            // Try parent classes.
            attributes = classType.GetCustomAttributes(typeof(ComponentManagementAttribute), true);
            if (attributes != null && attributes.Length > 0)
            {
                return (ComponentManagementAttribute)attributes[0];
            }

            return new ComponentManagementAttribute();
        }
    }
}
