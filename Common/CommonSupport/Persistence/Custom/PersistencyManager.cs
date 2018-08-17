using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Configuration;

namespace CommonSupport
{
    public interface IPersistentManager
    {
        void RegisterObject(IPersistentEx consistent);

        bool SaveObjectState(IPersistentEx consistent);
        bool RestoreObjectState(IPersistentEx consistent);
    }

    /// <summary>
    /// Class helps implement a custom consistency system of inteligently saving objects states.
    /// Warning - objects registered in the consistency manager are referenced, and will persist in memory.
    /// </summary>
    [Serializable]
    public class PersistentManager : IPersistentManager
    {
        const string FileExtension = "persistency";

        string _fullFileName;
        
        public string FileName
        {
            get { return _fullFileName.Substring(_fullFileName.LastIndexOf('\\') + 1, _fullFileName.Length -  _fullFileName.LastIndexOf('\\') - 1); }
        }

        // Object ID vs Object data
        Dictionary<string, PersistentData> _restoredObjects = new Dictionary<string, PersistentData>();

        Dictionary<string, PersistentData> _pendingSaveObjects = new Dictionary<string, PersistentData>();
        
        // This will verify that once an object registers with an ID, no other tries to adopt it.
        // It operates in relation to real current objects only.
        Dictionary<string, IPersistentEx> _verificationObjects = new Dictionary<string, IPersistentEx>();

        //bool _autoSaveOnObjectUpdate = true;
        /// <summary>
        /// Should the manager invoke save to file each time a single object is saved.
        /// </summary>
        //public bool AutoSaveOnObjectUpdate
        //{
        //    get { return _autoSaveOnObjectUpdate; }
        //    set { _autoSaveOnObjectUpdate = value; }
        //}

        /// <summary>
        /// 
        /// </summary>
        public PersistentManager(string fileName)
        {
            _fullFileName = fileName.ToLower();
            if (_fullFileName.EndsWith(FileExtension) == false)
            {
                _fullFileName += "." + FileExtension;
            }
        }

        string ActualPersistentId(IPersistentEx consistent)
        {
            return consistent.GetType().Name + "." + consistent.PersistencyId;
        }

        public void RegisterObject(IPersistentEx persistent)
        {
            lock (this)
            {
                if (persistent.PersistencyIsInitialzed == false)
                {
                    TracerHelper.Trace("[" + ActualPersistentId(persistent) + "] invoked by: " + ReflectionHelper.GetFullCallingMethodName(2));

                    persistent.InitializePersistency(this);

                    if (_pendingSaveObjects.ContainsKey(ActualPersistentId(persistent)) == false)
                    {
                        _pendingSaveObjects.Add(ActualPersistentId(persistent), new PersistentData());
                    }
                    
                    SystemMonitor.CheckThrow(_verificationObjects.ContainsKey(ActualPersistentId(persistent)) == false);
                    _verificationObjects.Add(ActualPersistentId(persistent), persistent);
                }

                System.Diagnostics.Debug.Assert(_verificationObjects.ContainsKey(ActualPersistentId(persistent)));
            }
        }

        public void SaveToFile()
        {
            TracerHelper.Trace("[" + _fullFileName + "] invoked by: " + ReflectionHelper.GetFullCallingMethodName(2));

            lock (this)
            {
                if (File.Exists(_fullFileName))
                {// Rename the old file - keep it as archive.
                    TimeSpan span = DateTime.Now - new DateTime(DateTime.Now.Year, 1, 1);
                    string customTimeValue = "." + DateTime.Now.Year + "." + (int)span.TotalSeconds;
                    string newFileName = _fullFileName.ToLower().Replace("." + FileExtension, customTimeValue) + "." + FileExtension;

                    while (File.Exists(newFileName))
                    {
                        newFileName = newFileName.Replace("." + FileExtension, "X" + "." + FileExtension);
                    }
                        
                    File.Move(_fullFileName, newFileName);
                }

                try
                {
                    FileInfo fi = new FileInfo(_fullFileName);
                    if (fi.Directory.Exists == false)
                    {
                        fi.Directory.Create();
                    }

                    using (FileStream stream = new FileStream(_fullFileName, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, _pendingSaveObjects);
                        stream.Close();
                    }
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.Fail("Stream error.");
                    TracerHelper.TraceError("Error occured while creating file stream [" + exception.Message + "].");
                }
            }
        }

        public bool RestoreFromFile()
        {
            TracerHelper.Trace("[" + _fullFileName + "] invoked by: " + ReflectionHelper.GetFullCallingMethodName(2));

            lock (this)
            {
                _restoredObjects.Clear();

                if (File.Exists(_fullFileName) == false)
                {
                    return false;
                }

                try
                {
                    using (FileStream stream = new FileStream(_fullFileName, FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        _restoredObjects = (Dictionary<string, PersistentData>)formatter.Deserialize(stream);
                        stream.Close();
                    }

                    // Transfer the restored info in the pending info places, where no info exists at all, to persist objects
                    // that are not currently active and will be restorable in the furure.
                    foreach (string id in _restoredObjects.Keys)
                    {
                        if (_pendingSaveObjects.ContainsKey(id) == false)
                        {
                            _pendingSaveObjects.Add(id, _restoredObjects[id]);
                        }
                    }

                    return true;
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.Fail("Stream error.");
                    TracerHelper.TraceError("Error occured while parsing file stream [" + exception.Message + "].");
                }
            }
            return false;
        }

        /// <summary>
        /// Allows the user of this interface to force an update to an object.
        /// Also the object itself can raise an request to be updated (saved).
        /// </summary>
        public bool SaveObjectState(IPersistentEx persistent)
        {
            TracerHelper.Trace("[" + ActualPersistentId(persistent) + "] invoked by: " + ReflectionHelper.GetFullCallingMethodName(2));

            lock (this)
            {
                RegisterObject(persistent);
                PersistentData data = _pendingSaveObjects[ActualPersistentId(persistent)];
                if (persistent.OnSaveState(this, data) == false)
                {
                    return false;
                }

                // Only if the receiver has confirmed the saving operation, replace the current data.
                _pendingSaveObjects[ActualPersistentId(persistent)] = data;
            }

            return true;
        }

        /// <summary>
        /// Allows the user of this interface to force an update to an object.
        /// Also the object itself can raise an request to be updated (saved).
        /// </summary>
        public bool RestoreObjectState(IPersistentEx persistent)
        {
            TracerHelper.Trace("[" + ActualPersistentId(persistent) + "] invoked by :" + ReflectionHelper.GetFullCallingMethodName(3));

            lock (this)
            {
                string persistentName = ActualPersistentId(persistent);
                SystemMonitor.CheckThrow(_verificationObjects.ContainsKey(persistentName) == false || _verificationObjects[persistentName] == persistent);

                if (_restoredObjects.ContainsKey(persistentName) == false 
                    /*|| _restoredObjects[persistentName].Values.Count == 0*/)
                {// Object restoration infromation not found.
                    return false;
                }

                PersistentData dataCopy = _restoredObjects[persistentName].Clone();
                bool result = persistent.OnRestoreState(this, dataCopy);

                if (result)
                {// Object restored successfully, remove pending restoration information.
                    _restoredObjects.Remove(persistentName);
                }

                return result;
            }
        }

    }
}
