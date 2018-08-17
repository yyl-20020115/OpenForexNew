using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Arbiter;
using CommonSupport;
using ForexPlatformPersistence;

namespace ForexPlatform
{
    /// <summary>
    /// The main component class in the platform, a component is the typical part of the platform.
    /// There are a few groups of components (like system components, community etc.), and also
    /// custom components.
    /// A component uses complex persistance model to make sure it is most flexible.
    /// Each component instance is stored in the DB, with its type name and some custom binary information.
    /// </summary>
    [Serializable]
    [DBPersistence(false)]
    public abstract class PlatformComponent : OperationalTransportClient, IDisposable, IDBPersistent
    {
        volatile Platform _platform;
        
        /// <summary>
        /// Platform containing this component. May be null if not yet initialized.
        /// </summary>
        public Platform Platform
        {
            get { return _platform; }
        }

        /// <summary>
        /// Has this component been initialized.
        /// </summary>
        public virtual bool IsInitialized
        {
            get { return OperationalState == OperationalStateEnum.Initialized
                || OperationalState == OperationalStateEnum.Operational;
            }
        }
        
        /// <summary>
        /// Is this component *instance* persistable to Database storage.
        /// Per instance control is required, so this is not a part of the control attribute.
        /// </summary>
        public virtual bool IsPersistableToDB
        {
            get { return true; }
        }

        SerializationInfoEx _uiSerializationInfo = new SerializationInfoEx();
        /// <summary>
        /// User interface data related to this operator.
        /// </summary>
        public SerializationInfoEx UISerializationInfo
        {
            get { lock (this) { return _uiSerializationInfo; } }
            set { lock (this) { _uiSerializationInfo = value; } }
        }

        #region IDBPersistent Members
        long? _id;
        /// <summary>
        /// This field persisted. Shows the Id of this component, unique for the platform.
        /// </summary>
        [DBPersistence(true)]
        public long? Id
        {
            get { lock (this) { return _id; } }
            set { lock (this) { _id = value; } }
        }

        //volatile bool _loaded = true;
        [DBPersistence(true)]
        public bool Loaded
        {
            get { return true; }
            set { /*_loaded = value;*/ }
        }

        [DBPersistence(DBPersistenceAttribute.PersistenceModeEnum.ReadOnly)]
        [Browsable(false)]
        public object Data
        {
            get
            {
                return SerializationHelper.Serialize(this);
            }
        }

        public event GeneralHelper.GenericDelegate<IDBPersistent> PersistenceDataUpdatedEvent;

        #endregion

        [DBPersistence(DBPersistenceAttribute.PersistenceModeEnum.ReadOnly)]
        public string Type
        {
            get
            {
                return this.GetType().AssemblyQualifiedName;
            }
        }

        public delegate void PlatformComponentUpdateDelegate(PlatformComponent component);
        [field: NonSerialized]
        public event PlatformComponentUpdateDelegate Initializing;
        [field: NonSerialized]
        public event PlatformComponentUpdateDelegate UnInitializing;

        /// <summary>
        /// Component will try to use UserFriendlyNameAttribute to establish a name for the current component, or if not available use class type name.
        /// </summary>
        public PlatformComponent(bool singleThreadMode)
            : base(singleThreadMode)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformComponent(string name, bool singleThreadMode)
            : base(name, singleThreadMode)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="context"></param>
        public PlatformComponent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _uiSerializationInfo = (SerializationInfoEx)info.GetValue("uiSerializationData", typeof(SerializationInfoEx));
        }

        /// <summary>
        /// Serialization baseMethod.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            lock (this)
            {
                base.GetObjectData(info, context);
                info.AddValue("uiSerializationData", _uiSerializationInfo);
            }
        }

        /// <summary>
        /// Set initial state information to this component, allow to accept dataDelivery before initialization begins.
        /// </summary>
        /// <param name="dataDelivery"></param>
        /// <returns></returns>
        public bool SetInitialState(PlatformSettings data)
        {
            lock (this)
            {
                return OnSetInitialState(data);
            }
        }

        /// <summary>
        /// Start operation.
        /// A single instance must be able to handle multiple LoadFromFile-Uninitialize cycles.
        /// And also must be able to operate a LoadFromFile-Uninitialize-SaveState-Deserialize-LoadFromFile etc. cycles.
        /// </summary>
        public bool Initialize(Platform platform)
        {
            if (Initializing != null)
            {
                Initializing(this);
            }

            lock (this)
            {// Multiple init-uninit cycles allowed.
                SystemMonitor.CheckThrow(_platform == null || _platform == platform);

                _platform = platform;

                if (OnInitialize(platform) == false)
                {
                    _platform = null;
                    return false;
                }
            }

            SystemMonitor.CheckError(OperationalState == OperationalStateEnum.Operational || OperationalState == OperationalStateEnum.Initialized, "Component not initalized(or operational) after init[true].");
            return true;
        }

        /// <summary>
        /// Stop operation.
        /// </summary>
        public void UnInitialize()
        {
            if (IsInitialized == false)
            {
                return;
            }

            if (UnInitializing != null)
            {
                UnInitializing(this);
            }

            lock(this)
            {
                OnUnInitialize();
            }

            SystemMonitor.CheckError(OperationalState != OperationalStateEnum.Operational && OperationalState != OperationalStateEnum.Initialized, "Component not uninitialized(or not operational) after UnInit.");
        }

        /// <summary>
        /// Dispose all references and resources and release the object once and for all.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                OnDispose();

                // Let the platform reference go, only after absolutely everything else has been released.
                _platform = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void RaisePersistenceDataUpdatedEvent()
        {
            if (PersistenceDataUpdatedEvent != null)
            {
                PersistenceDataUpdatedEvent(this);
            }
            else
            {
                SystemMonitor.Error("Component [" + this.Name + "] has no PersistenceDataUpdatedEvent assigned. Persistence missed.");
            }
        }

        protected virtual bool OnSetInitialState(PlatformSettings data)
        {
            return true;
        }

        protected virtual bool OnInitialize(Platform platform)
        {
            return true;
        }

        protected virtual bool OnUnInitialize()
        {
          
            return true;
        }

        protected virtual void OnDispose()
        {
        }

    }
}
