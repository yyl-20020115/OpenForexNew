using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Tell a module to change its operational state.
    /// </summary>
    [Serializable]
    public class ChangeOperationalStateMessage : TransportMessage
    {
        bool _isRequest;
        /// <summary>
        /// Is this a notification or a request.
        /// </summary>
        public bool IsRequest
        {
            get { return _isRequest; }
            set { _isRequest = value; }
        }

        OperationalStateEnum _operationalState = OperationalStateEnum.UnInitialized;
        /// <summary>
        /// The state we are requested to go to.
        /// </summary>
        public OperationalStateEnum OperationalState
        {
            get { return _operationalState; }
            set { _operationalState = value; }
        }

        /// <summary>
        /// constructor.
        /// </summary>
        public ChangeOperationalStateMessage()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ChangeOperationalStateMessage(OperationalStateEnum state, bool isRequest)
        {
            _operationalState = state;
            _isRequest = isRequest;
        }
    }
}
