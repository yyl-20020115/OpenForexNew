using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// Base class for messages passed trough the Arbiter mechanism.
    /// (Make sure all your requestMessage implementations are public and expose public properties 
    /// to get them properly serialized in the logger.)
    /// </summary>
    [Serializable]
    public abstract class Message : System.EventArgs
    {
        //bool _transportByReference = false;
        ///// <summary>
        ///// By default messages are transported by cloning them; 
        ///// should a message be marked as transport by reference
        ///// set this to true.
        ///// </summary>
        //public bool TransportByReference
        //{
        //    get { return _transportByReference; }
        //    set { _transportByReference = value; }
        //}

        /// <summary>
        /// Constructor.
        /// </summary>
        public Message()
        {
        }
    }
}
