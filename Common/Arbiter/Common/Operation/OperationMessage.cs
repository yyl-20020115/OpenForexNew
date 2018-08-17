using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;

namespace Arbiter
{
    /// <summary>
    /// Base class for messages that represent an operation.
    /// </summary>
    [Serializable]
    public class OperationMessage : RequestMessage
    {
        volatile bool _performSynchronous = true;
        /// <summary>
        /// Should the operation be performed synchronously.
        /// </summary>
        public bool PerformSynchronous
        {
            get { return _performSynchronous; }
            set { _performSynchronous = value; }
        }

        int _operationID = -1;
        public int OperationId
        {
            get { return _operationID; }
            set { _operationID = value; }
        }

        /// <summary>
        /// We do not add the operation directly here, 
        /// since we might be using the operation helper.
        /// </summary>
        public OperationMessage()
        {
        }
    }
}
