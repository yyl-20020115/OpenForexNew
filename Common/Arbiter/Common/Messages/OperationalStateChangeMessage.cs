using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// A module changed its operational state.
    /// </summary>
    [Serializable]
    public class OperationalStateChangeMessage : TransportMessage
    {
        OperationalStateEnum _state = OperationalStateEnum.UnInitialized;
        public OperationalStateEnum State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public OperationalStateChangeMessage()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public OperationalStateChangeMessage(OperationalStateEnum state)
        {
            _state = state;
        }
    }
}
