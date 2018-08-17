using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using CommonFinancial;
using CommonSupport;
using ForexPlatformPersistence;

namespace ForexPlatform
{
    /// <summary>
    /// Manages local storage and manipulation of historical quote dataDelivery.
    /// Class typically operates in Singleton mode.
    /// The manager takes care of accessing, modifying etc. stored historical dataDelivery.
    /// It is not a part of a Platform component, since multiple components may need
    /// to work with it simultaniously, and need to have shared synchronized objects access.
    /// </summary>
    public class DataStore : Operational
    {
        ListUnique<OnlineEntrySource> _onlineEntrySources = new ListUnique<OnlineEntrySource>();

        /// <summary>
        /// The sessionInformation orderInfo reuses the entry Guid, so that it can be easily persisted further.
        /// </summary>
        BiDictionary<DataStoreEntry, RuntimeDataSessionInformation> _entriesAndSessions = new BiDictionary<DataStoreEntry, RuntimeDataSessionInformation>();

        /// <summary>
        /// Online dataDelivery source entries.
        /// </summary>
        public ReadOnlyCollection<OnlineEntrySource> OnlineEntrySources
        {
            get { return _onlineEntrySources.AsReadOnly(); }
        }

        /// <summary>
        /// Existing local dataDelivery entries.
        /// </summary>
        public DataStoreEntry[] Entries
        {
            get
            {
                lock (_entriesAndSessions)
                {
                    return GeneralHelper.EnumerableToArray<DataStoreEntry>(_entriesAndSessions.Keys);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RuntimeDataSessionInformation[] Sessions
        {
            get 
            {
                lock (_entriesAndSessions)
                {
                    return GeneralHelper.EnumerableToArray<RuntimeDataSessionInformation>(_entriesAndSessions.Values);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int EntriesCount
        {
            get { return _entriesAndSessions.Count; }
        }

        volatile SQLiteADOPersistenceHelper _persistenceHelper;

        string _dataStoreFilesFolder = string.Empty;

        public const string FileNameGuidSeparator = "``";

        private decimal _defaultSessionLotSize = 10000;
        /// <summary>
        /// Default lot size for sessions created from dataDelivery store entries.
        /// </summary>
        public decimal DefaultSessionLotSize
        {
            get { return _defaultSessionLotSize; }
            set { _defaultSessionLotSize = value; }
        }

        static Mutex _singletonMutex = new Mutex();
        static DataStore _singletonInstance = null;
        /// <summary>
        /// Singleton implementation.
        /// </summary>
        public static DataStore Instance
        {
            get
            {
                if (_singletonInstance == null)
                {// This double check is done in order to evade a mutex wait on each entry,
                    // it is only done on creation, to evade the opportunity of several threads
                    // entering and creating at the same time.
                    _singletonMutex.WaitOne();
                    if (_singletonInstance == null)
                    {
                        _singletonInstance = new DataStore();
                    }
                    _singletonMutex.ReleaseMutex();
                }

                return _singletonInstance;
            }
        }

        public delegate void EntryUpdateDelegate(DataStore manager, DataStoreEntry entry);
        public delegate void OnlineEntrySourceUpdateDelegate(DataStore manager, OnlineEntrySource entrySource);
        
        public event EntryUpdateDelegate EntryAddedEvent;
        public event EntryUpdateDelegate EntryRemovedEvent;

        public event OnlineEntrySourceUpdateDelegate OnlineEntrySourceAddedEvent;
        public event OnlineEntrySourceUpdateDelegate OnlineEntrySourceRemovedEvent;

        /// <summary>
        /// Protected constructor, since currently operating under singleton mode.
        /// </summary>
        protected DataStore()
        {
            ChangeOperationalState(OperationalStateEnum.Initializing);
            _onlineEntrySources.SingleEntryMode = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected static SQLiteADOPersistenceHelper CreatePersistenceHelper(PlatformSettings settings)
        {
            SQLiteADOPersistenceHelper helper = new SQLiteADOPersistenceHelper();
            if (helper.Initialize(settings.GetMappedPath("DataStoreDBPath"), true) == false)
            {
                return null;
            }

            if (helper.ContainsTable("DataStoreEntries") == false)
            {// Create the table structure.
                helper.ExecuteCommand(ForexPlatformPersistence.Properties.Settings.Default.DataStoreDBSchema);
            }

            helper.SetupTypeMapping(typeof(DataStoreEntry), "DataStoreEntries");

            return helper;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Initialize(PlatformSettings settings)
        {
            lock (this)
            {
                _persistenceHelper = CreatePersistenceHelper(settings);

                _dataStoreFilesFolder = settings.GetMappedPath("DataStoreFolder");
            }

            // Loading online data sources, currently not operating.
            //GeneralHelper.FireAndForget(
            //    delegate()
            //    {
            //        try
            //        {// Download online dataDelivery source entries.
            //            WebRequest request = WebRequest.Create(settings.DataStoreOnlineSourcesXml);
            //            request.Timeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;

            //            TextReader reader = new StreamReader(request.GetResponse().GetResponseStream());

            //            // If you get an error here in DEBUG MODE, just ignore it, it is a bug in VS 2008.
            //            XmlSerializer serializer = new XmlSerializer(typeof(OnlineEntrySource[]));

            //            OnlineEntrySource[] sources = (OnlineEntrySource[])serializer.Deserialize(reader);

            //            lock (_onlineEntrySources)
            //            {// _onlineEntrySources contains serialized existing sources.
            //                _onlineEntrySources.AddRange(sources);
            //                sources = _onlineEntrySources.ToArray();
            //            }

            //            foreach (OnlineEntrySource source in sources)
            //            {
            //                if (OnlineEntrySourceAddedEvent != null)
            //                {
            //                    OnlineEntrySourceAddedEvent(this, source);
            //                }
            //            }

            //        }
            //        catch (Exception ex)
            //        {
            //            SystemMonitor.OperationError("Failed to obtain online data souces [" + ex.Message + "].", TracerItem.PriorityEnum.Low);
            //        }
            //    }
            //);

            if (Directory.Exists(_dataStoreFilesFolder) == false)
            {
                if (Directory.CreateDirectory(_dataStoreFilesFolder) == null)
                {
                    SystemMonitor.OperationError("Failed to create Data Store folder [" + settings.GetMappedPath("DataStoreFolder") + "]");
                    return false;
                }
            }

            List<DataStoreEntry> entries = _persistenceHelper.Select<DataStoreEntry>(null, null);
            foreach (DataStoreEntry entry in entries)
            {
                DoAddEntry(entry);
            }

            ChangeOperationalState(OperationalStateEnum.Operational);

            return true;
        }

        /// <summary>
        /// Obtain entry based on the sessionInformation orderInfo that corresponds to it.
        /// </summary>
        /// <param name="sessionInformation"></param>
        /// <returns></returns>
        public DataStoreEntry GetEntryBySessionInfo(DataSessionInfo sessionInfo)
        {
            DataStoreEntry entry = null;
            lock (_entriesAndSessions)
            {
                foreach (RuntimeDataSessionInformation information in _entriesAndSessions.Values)
                {
                    if (information.Info.Equals(sessionInfo))
                    {
                        if (_entriesAndSessions.GetByValueSafe(information, ref entry))
                        {
                            return entry;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Create an entry and initialize it with dataDelivery from a local file,
        /// and add it to the currently managed list of entries.
        /// </summary>
        /// <returns></returns>
        public DataStoreEntry AddEntryFromLocalFile(string filePath)
        {
            string newFilePath;
            string filesFolder = _dataStoreFilesFolder;
            if (string.IsNullOrEmpty(filesFolder))
            {
                SystemMonitor.Error("Files folder for data store manager not initialized.");
                return null;
            }

            if (File.Exists(filePath) == false)
            {
                SystemMonitor.OperationWarning(string.Format("File to create entry from not found [{0}].", filePath));
                return null;
            }


            // The "``" symbol is used for separator, since no trading symbol is supposed to have this in its name.
            newFilePath = Path.Combine(filesFolder, Path.GetFileNameWithoutExtension(filePath) + FileNameGuidSeparator + Guid.NewGuid().ToString() + Path.GetExtension(filePath)); ;

            bool sameFile = (Path.GetDirectoryName(filePath) == Path.GetDirectoryName(newFilePath)
                && Path.GetFileName(filePath) == Path.GetFileName(newFilePath));

            if (sameFile == false)
            {
                try
                {
                    File.Copy(filePath, newFilePath);
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationError(ex.Message);
                }
            }

            DataStoreEntry entry = new DataStoreEntry();
            entry.Initialize(_dataStoreFilesFolder);

            if (entry.LoadFromFile(newFilePath, FileNameGuidSeparator) == false)
            {
                if (sameFile == false)
                {
                    try
                    {
                        File.Delete(newFilePath);
                    }
                    catch (Exception ex)
                    {
                        SystemMonitor.OperationError(ex.Message);
                    }
                }
                return null;
            }

            entry.Description = string.Format("Entry generated from file [{0}].", Path.GetFileName(filePath));

            this.AddEntry(entry);

            return entry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool DoAddEntry(DataStoreEntry entry)
        {
            entry.Initialize(_dataStoreFilesFolder);

            lock(_entriesAndSessions)
            {
                if (_entriesAndSessions.ContainsKey(entry))
                {
                    SystemMonitor.OperationWarning("Entry already added.");
                    return false;
                }

                if (entry.Symbol.IsEmpty)
                {
                    SystemMonitor.Warning("Entry added before initialized.");
                    return false;
                }

                // The sessionInformation reuses the entry Guid, so that it can be easily persisted further.
                DataSessionInfo entrySessionInfo = new DataSessionInfo(entry.Guid, "Data Store Session [" + entry.Symbol.Name + "]",
                    entry.Symbol, DefaultSessionLotSize, entry.DecimalDigits);

                RuntimeDataSessionInformation session = new RuntimeDataSessionInformation(entrySessionInfo, entry.Period.Value);
                _entriesAndSessions.Add(entry, session);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AddEntry(DataStoreEntry entry)
        {
            if (DoAddEntry(entry))
            {
                lock (this)
                {
                    _persistenceHelper.Insert<DataStoreEntry>(entry);
                }

                if (EntryAddedEvent != null)
                {
                    EntryAddedEvent(this, entry);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemoveEntry(DataStoreEntry entry)
        {
            lock (_entriesAndSessions)
            {
                if (_entriesAndSessions.RemoveByKey(entry) == false)
                {
                    return false;
                }
            }
            
            lock(this)
            {
                _persistenceHelper.Delete<DataStoreEntry>(entry);
            }

            entry.ClearData();

            if (EntryRemovedEvent != null)
            {
                EntryRemovedEvent(this, entry);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemoveOnlineEntrySource(OnlineEntrySource source)
        {
            lock (this)
            {
                if (_onlineEntrySources.Remove(source) == false)
                {
                    return false;
                }
            }

            if (OnlineEntrySourceRemovedEvent != null)
            {
                OnlineEntrySourceRemovedEvent(this, source);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {
            ChangeOperationalState(OperationalStateEnum.UnInitialized);

            //OnlineEntrySource source = new OnlineEntrySource("s1");
            //source.Period = TimeSpan.FromDays(2);
            //source.SymbolName = "asdasd";
            //source.Source = "asdasd";
            //_onlineEntrySources.AddElement(source);
            //_onlineEntrySources.AddElement(new OnlineEntrySource("asdwqdw2"));
            //TextWriter s = new StreamWriter(@"C:\test.xml");

            //XmlSerializer serializer = new XmlSerializer(typeof(OnlineEntrySource[]));
            //serializer.SaveState(s, _onlineEntrySources.ToArray());
        }

    }
}
