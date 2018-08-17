using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Use the IPersistent directly, or this class provided for ease of use.
    /// </summary>
    public abstract class PersistentEx : IPersistentEx
    {
        IPersistentManager _manager;

        protected IPersistentManager PersistencyManager
        {
            get { lock (this) { return _manager; } }
        }

        public bool PersistencyIsInitialzed
        {
            get { lock (this) { return _manager != null; } }
        }

        public abstract string PersistencyId
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public PersistentEx()
        {
        }

        public abstract bool OnSaveState(IPersistentManager manager, PersistentData data);
        public abstract bool OnRestoreState(IPersistentManager manager, PersistentData data);

        /// <summary>
        /// Delegate allows the Consistent to notify parent it requires an update.
        /// </summary>
        /// <param name="delegateInstance"></param>
        public void InitializePersistency(IPersistentManager manager)
        {
            _manager = manager;
        }

        public void SaveToManager()
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(_manager != null);
                if (_manager != null)
                {
                    _manager.SaveObjectState(this);
                }
            }
        }

        public void RestoreFromManager()
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(_manager != null);
                if (_manager != null)
                {
                    _manager.RestoreObjectState(this);
                }
            }
        }

    }
}
