using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// Used for filtering messages.
    /// Evade linking to any other objects.
    /// </summary>
    [Serializable]
    public class MessageFilter
    {
        List<Type> _allowedNonAddressedMessageTypes = new List<Type>();
        public List<Type> AllowedNonAddressedMessageTypes
        {
            get { return _allowedNonAddressedMessageTypes; }
        }

        private bool _enabled = true;
        /// <summary>
        /// Disabled filter will pass trough all messages.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private bool _allowChildrenTypes = false;
        /// <summary>
        /// Should the filter accept messages that are children types in relation to the types inputed.
        /// </summary>
        public bool AllowChildrenTypes
        {
            get { return _allowChildrenTypes; }
            set { _allowChildrenTypes = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MessageFilter(bool enabled)
        {
            _enabled = enabled;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public virtual bool MessageAllowed(Message message)
        {
            if (!_enabled)
            {
                return true;
            }

            Type requiredType = message.GetType();

            foreach (Type messageType in _allowedNonAddressedMessageTypes)
            {
                if (messageType == requiredType)
                {// Type matched.
                    return true;
                }

                if (_allowChildrenTypes && requiredType.IsSubclassOf(messageType))
                {// Subclasses allowed and this is one.
                    return true;
                }
            }

            return false;
        }
    }
}
