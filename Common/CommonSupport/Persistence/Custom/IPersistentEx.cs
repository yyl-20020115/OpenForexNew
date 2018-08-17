using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{

    public interface IPersistentEx
    {
        /// <summary>
        /// In the entire class graph / hierarchy that is being saved, those IDs must be unique.
        /// </summary>
        string PersistencyId
        {
            get;
        }

        /// <summary>
        /// Has the consistency initialization been done.
        /// </summary>
        bool PersistencyIsInitialzed
        {
            get;
        }

        void InitializePersistency(IPersistentManager manager);
        
        // Receiver to return true to confirm it has modified the data and want's it apply, false to deny this operation and keep data untouched.
        bool OnSaveState(IPersistentManager manager, PersistentData data);

        // Receiver to return true to confirm it has modified the data and want's it apply, false to deny this operation and keep data untouched.
        bool OnRestoreState(IPersistentManager manager, PersistentData data);
    }
}
