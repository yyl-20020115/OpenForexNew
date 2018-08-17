using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    [Serializable]
    public abstract class AModifiable : IDisposable
    {
        public delegate void ModifiedDelegate();
        
        [NonSerialized]
        ModifiedDelegate _modifiedHandler;
        public event ModifiedDelegate Modified
        {
            add
            {
                _modifiedHandler += value;
            }
            remove
            {
                _modifiedHandler -= value;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose()
        {
            // Clear all the events.
            _modifiedHandler = null;
        }


        protected void InvokeModified()
        {
            if (_modifiedHandler != null)
            {
                _modifiedHandler();
            }
        }
    }
}
