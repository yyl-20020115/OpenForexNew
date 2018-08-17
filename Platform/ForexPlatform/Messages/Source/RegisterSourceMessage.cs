using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Use requestMessage to register a source.
    /// </summary>
    [Serializable]
    public class RegisterSourceMessage : RequestMessage
    {
        SourceTypeEnum? _type;

        /// <summary>
        /// If this has no value, all sources on this ComponentId path shall be unregistered.
        /// </summary>
        public SourceTypeEnum? SourceType
        {
            get { return _type; }
            set { _type = value; }
        }

        bool _register = true;
        /// <summary>
        /// When true, register, when false, unregister.
        /// </summary>
        public bool Register
        {
            get { return _register; }
            set { _register = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public RegisterSourceMessage(SourceTypeEnum type, bool register)
        {
            _type = type;
            _register = register;
        }
        
        /// <summary>
        /// Use to unregister, otherwise use the other constructor.
        /// </summary>
        public RegisterSourceMessage(bool register)
        {
            _register = register;
        }
    }
}
