using System;
using System.Collections.Generic;
using Arbiter;
using CommonSupport;
using ForexPlatformPersistence;
using System.IO;
using System.Reflection;
using CommonFinancial;

namespace ForexPlatform
{
  /// <summary>
  /// Platform is the main class, the platform instance that holds and manages all components.
  /// </summary>
  [DBPersistence(false)]
  public class Platform : TransportClient, IDBPersistent
  {
    long? _id = null;
    /// <summary>
    /// DB Id of this platform.
    /// </summary>
    [DBPersistence(true)]
    public long? Id
    {
      get { return _id; }
      set { _id = value; }
    }

    /// <summary>
    /// Name of this platform instance.
    /// </summary>
    [DBPersistenceAttribute(true)]
    public new string Name
    {
      get { return base.Name; }
      set { base.Name = value; }
    }

    Guid _guid;
    /// <summary>
    /// Unique GUID of this platform instance.
    /// </summary>
    [DBPersistenceAttribute(true)]
    public Guid Guid
    {
      get { return _guid; }
      set { _guid = value; }
    }

    volatile bool _isLoading = false;
    /// <summary>
    /// Is the platform loading components.
    /// </summary>
    public bool IsLoading
    {
      get { return _isLoading; }
    }

    SecureDataManager _securityDataManager = new SecureDataManager();

    SerializationInfoEx _serializationData = new SerializationInfoEx();
    /// <summary>
    /// Persistance for all additional platform dataDelivery, that does not have a corresponding column in the DB.
    /// It is all binary serialized here and put inside a "blob" entry in the DB.
    /// </summary>
    [DBPersistence(DBPersistenceAttribute.PersistenceTypeEnum.Binary)]
    public SerializationInfoEx Data
    {
      get { lock (this) { return _serializationData; } }
      set { lock (this) { _serializationData = value; } }
    }

    SerializationInfoEx _uiSerializationInfo = new SerializationInfoEx();
    /// <summary>
    /// Persistance support for common UI settings and folders.
    /// </summary>
    [DBPersistenceAttribute(false)]
    public SerializationInfoEx UISerializationInfo
    {
      get { lock (this) { return _uiSerializationInfo; } }
    }

    SerializationInfoEx _componentSpecificSerializationInfo = new SerializationInfoEx();
    /// <summary>
    /// Persistance support for components, so that they preserve settings over registrer/unregister cycles.
    /// Make sure to store in fields that are of this format {ComponentTypeName}.{FieldName}
    /// </summary>
    [DBPersistenceAttribute(false)]
    public SerializationInfoEx ComponentSpecificSerializationInfo
    {
      get { lock (this) { return _componentSpecificSerializationInfo; } }
    }

    volatile SQLiteADOPersistenceHelper _persistenceHelper;
    SQLiteADOPersistenceHelper PersistenceHelper
    {
      get
      {
        return _persistenceHelper;
      }
    }

    /// <summary>
    /// This includes all component types grouped.
    /// </summary>
    Dictionary<ComponentId, PlatformComponent> _components = new Dictionary<ComponentId, PlatformComponent>();
    public IEnumerable<PlatformComponent> ComponentsUnsafe
    {
      get { lock (this) { return _components.Values; } }
    }

    volatile PlatformSettings _settings;
    /// <summary>
    /// Settings coming from the XML configuration file. General settings accessible to the user
    /// event when the platform is offline or not operational.
    /// </summary>
    public PlatformSettings Settings
    {
      get { return _settings; }
    }

    public SourceContainer Sources
    {
      get
      {
        SourceManagementComponent smc = this.GetFirstComponentByType(typeof(SourceManagementComponent)) as SourceManagementComponent;
        if (smc == null)
        {
          this.RegisterComponent(smc = new SourceManagementComponent());
        }
        return smc.Sources;
      }
    }

    public bool RegisterSource(SourceInfo source)
    {
      return this.Sources.Register(source);
    }
    public bool UnRegisterSource(SourceTypeEnum? sourceType, TransportInfo info)
    {
      return this.Sources.UnRegister(sourceType, info);
    }

    public delegate void ActiveComponentUpdateDelegate(PlatformComponent component, bool isInitialUpdate);
    /// <summary>
    /// A component was added to the platform.
    /// </summary>
    public event ActiveComponentUpdateDelegate ActiveComponentAddedEvent;
    /// <summary>
    /// A component was remoted from the platform.
    /// </summary>
    public event ActiveComponentUpdateDelegate ActiveComponentRemovedEvent;

    public delegate void ActiveComponentChangedOperationalStateDelegate(Platform platform, PlatformComponent component, OperationalStateEnum previousState);
    /// <summary>
    /// A component of the platform has changed its operations state.
    /// </summary>
    public event ActiveComponentChangedOperationalStateDelegate ActiveComponentChangedOperationalStateEvent;

    public delegate void ComponentDeserializationFailedDelegate(long componentId, string componentTypeName);
    /// <summary>
    /// A component has failed to deserialize (used by UI to allow handling of this occurence).
    /// </summary>
    public event ComponentDeserializationFailedDelegate ComponentDeserializationFailedEvent;

    /// <summary>
    /// IDBPersistent member.
    /// </summary>
    //public event GeneralHelper.GenericDelegate<IDBPersistent> PersistenceDataUpdatedEvent;

    /// <summary>
    /// Name of the platform must be unique.
    /// </summary>
    public Platform(string name)
      : base(name, false)
    {
      TracerHelper.TraceEntry();

      Arbiter.Arbiter arbiter = new Arbiter.Arbiter("Platform");
      //TracerHelper.TracingEnabled = false;
      arbiter.AddClient(this);
    }

    /// <summary>
    /// Helper, creates the persistence helper for the platform.
    /// </summary>
    protected static SQLiteADOPersistenceHelper CreatePlatformPersistenceHelper(PlatformSettings settings)
    {
      SQLiteADOPersistenceHelper helper = new SQLiteADOPersistenceHelper();
      if (helper.Initialize(settings.GetMappedPath("PlatformDBPath"), true) == false)
      {
        return null;
      }

      if (helper.ContainsTable("Platforms") == false)
      {// Create the table structure.
        helper.ExecuteCommand(ForexPlatformPersistence.Properties.Settings.Default.PlatformDBSchema);
      }

      helper.SetupTypeMapping(typeof(Platform), "Platforms");
      helper.SetupTypeMapping(typeof(PlatformComponent), "PlatformComponents");

      return helper;
    }

    /// <summary>
    /// 
    /// </summary>
    protected void LoadModules(PlatformSettings platformSettings)
    {

      List<string> loadingModules = new List<string>();
      string[] optionalModules = platformSettings.GetString("OptionalModules").Replace("\r", string.Empty).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
      loadingModules.AddRange(optionalModules);

      string[] externalModules = platformSettings.GetString("ExternalModules").Replace("\r", string.Empty).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
      loadingModules.AddRange(externalModules);

      // Load the external optional references for the solution.
      // Loaded assemblies are attached as runtime references to the main application assembly.
      foreach (string referenceName in loadingModules)
      {
        string filePath = GeneralHelper.MapRelativeFilePathToExecutingDirectory(referenceName);
        string operationResultMessage = string.Empty;

        Assembly assembly = null;
        try
        {
          // If you get a *warning* here (LoadFromContext) ignore it. 
          // Using LoadFrom is important, since it will also allow to find 
          // the newly loaded assemblies references, if they are in its folder.
          assembly = Assembly.LoadFrom(filePath);

          Type[] types = assembly.GetTypes();
        }
        catch (Exception ex)
        {
          operationResultMessage = ex.Message;
        }

        if (assembly != null)
        {
          ReflectionHelper.AddDynamicReferencedAssembly(Assembly.GetEntryAssembly(), assembly);
        }
        else
        {
          SystemMonitor.OperationWarning("Failed to load external dynamic module [" + referenceName + ", " + filePath + "; " + operationResultMessage + "]", TracerItem.PriorityEnum.Low);
        }
      }

      //AppDomain.CurrentDomain.DynamicDirectory
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      //AppDomain.CurrentDomain.

    }

    /// <summary>
    /// The initialization of a platform consumes 2 main dataDelivery sources.
    /// The settings is used for primary, startup information (like where 
    /// is the persistence DB file etc.) and the information inside the 
    /// persistence is than on used to create components etc.
    /// </summary>
    public bool Initialize(PlatformSettings platformSettings)
    {
      TracerHelper.TraceEntry();

      SystemMonitor.CheckThrow(_settings == null, "Platform already initialized.");

      lock (this)
      {
        _settings = platformSettings;
        if (_persistenceHelper == null)
        {
          _persistenceHelper = CreatePlatformPersistenceHelper(platformSettings);
        }
      }

      if (_persistenceHelper == null)
      {
        return false;
      }

      LoadModules(_settings);

      if (PersistenceHelper.Count<Platform>(new MatchExpression("Name", this.Name)) == 0)
      {// This is a new platform.
        lock (this)
        {
          _guid = Guid.NewGuid();
        }

        if (PersistenceHelper.Insert<Platform>(this, null) == false)
        {
          SystemMonitor.Error("Failed to persist new platform [" + this.Name + "]");
          return false;
        }
      }
      else
      {// This is existing.
        // Now try to load self from persistance storage.
        bool selectionResult = PersistenceHelper.SelectScalar<Platform>(this, new MatchExpression("Name", this.Name));

        if (selectionResult == false)
        {// Failed to load self from DB.
          return false;
        }
      }

      lock (this)
      {
        if (_serializationData.ContainsValue("diagnosticsMode"))
        {
          _settings.DiagnosticsMode = _serializationData.GetBoolean("diagnosticsMode");
        }

        if (_serializationData.ContainsValue("uiSerializationInfo"))
        {
          // The main serialization dataDelivery stores the UI orderInfo.
          _uiSerializationInfo = _serializationData.GetValue<SerializationInfoEx>("uiSerializationInfo");
        }

        if (_serializationData.ContainsValue("componentSpecificSerializationInfo"))
        {
          // The main serialization dataDelivery stores the UI orderInfo.
          _componentSpecificSerializationInfo = _serializationData.GetValue<SerializationInfoEx>("componentSpecificSerializationInfo");
        }
      }

      //_server = new Arbiter.TransportIntegrationServer(_platformUri);
      //Arbiter.AddClient(_server);

      GeneralHelper.FireAndForget(delegate()
      {// LoadFromFile components.
        // Registering of components is better done outside the lock, 
        // since components may launch requests to platform at initializations.

        _isLoading = true;

        // Components are stored in the PlatformComponents database, they are being serialized and the entire object is 
        // persisted in the DB, as well as the type information for it and a reference to the platform instance it belongs to.
        List<long> failedSerializationsIds = new List<long>();
        List<PlatformComponent> components = PersistenceHelper.SelectSerializedType<PlatformComponent>(
            new MatchExpression("PlatformId", this.Id), "Data", null, failedSerializationsIds);

        SortedList<int, List<PlatformComponent>> componentsByLevel = GetComponentsByLevel(components);

        GatherMandatoryComponents(componentsByLevel);

        foreach (int level in componentsByLevel.Keys)
        {// Register lower level components first.
          foreach (PlatformComponent component in componentsByLevel[level])
          {
            if (DoRegisterComponent(component, true) == false
                && component.Id.HasValue && ComponentDeserializationFailedEvent != null)
            {
              ComponentDeserializationFailedEvent(component.Id.Value, component.GetType().Name);
            }
          }
        }

        // Handle failed deserializations.
        foreach (int id in failedSerializationsIds)
        {
          string typeName = "Unknown";

          try
          {// Extract the type of this entry.
            List<object[]> result = PersistenceHelper.SelectColumns<PlatformComponent>(
                new MatchExpression("Id", id), new string[] { "Type" }, 1);

            Type type = Type.GetType(result[0][0].ToString());

            if (type != null)
            {
              typeName = type.Name;
            }
          }
          catch (Exception ex)
          {
            SystemMonitor.Error("Failed to extract type information [" + ex.Message + "].");
          }

          if (ComponentDeserializationFailedEvent != null)
          {
            ComponentDeserializationFailedEvent(id, typeName);
          }
        }

        _isLoading = false;
      });

      return true;
    }

    public bool UnInitialize()
    {
      TracerHelper.TraceEntry();

      SortedList<int, List<PlatformComponent>> componentsByLevel;
      lock (this)
      {
        componentsByLevel = GetComponentsByLevel(_components.Values);
      }

      // Un initialize higher level components first.
      // Make sure component uninit is outside of locks, to evade dead locks.
      for (int i = 0; i < componentsByLevel.Keys.Count; i++)
      {
        int level = componentsByLevel.Keys[i];
        foreach (PlatformComponent component in componentsByLevel[level])
        {
          UnInitializeComponent(component);
        }
      }

      lock (this)
      {
        // Store additional.
        _serializationData.Clear();
        _serializationData.AddValue("diagnosticsMode", _settings.DiagnosticsMode);
        _serializationData.AddValue("uiSerializationInfo", _uiSerializationInfo);
        _serializationData.AddValue("componentSpecificSerializationInfo", _componentSpecificSerializationInfo);

        PersistenceHelper.UpdateToDB<Platform>(this, null);

        _components.Clear();
        this.Arbiter.Dispose();
      }

      return true;
    }

    /// <summary>
    /// Remove self from DB.
    /// </summary>
    public void DeleteFromPersistence()
    {
      lock (this)
      {
        PersistenceHelper.Delete<PlatformComponent>(new MatchExpression("PlatformId", this.Id));
        PersistenceHelper.Delete<Platform>(new MatchExpression("Name", this.Name));
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsMandatoryComponent(PlatformComponent component)
    {
      return GetMandatoryComponentsTypes().Contains(component.GetType());
    }

    /// <summary>
    /// 
    /// </summary>
    protected List<Type> GetMandatoryComponentsTypes()
    {
      // Establish mandatory component types.
      List<Type> mandatoryComponentTypes = new List<Type>();
      List<Type> candidateTypes = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(typeof(PlatformComponent), ReflectionHelper.GetApplicationEntryAssemblyAndReferencedAssemblies(), true, false);
      foreach (Type type in candidateTypes)
      {
        if (ComponentManagementAttribute.GetTypeAttribute(type).IsMandatory)
        {
          mandatoryComponentTypes.Add(type);
        }
      }

      return mandatoryComponentTypes;
    }

    /// <summary>
    /// 
    /// </summary>
    protected void GatherMandatoryComponents(SortedList<int, List<PlatformComponent>> componentsByLevel)
    {
      List<Type> mandatoryComponentTypes = GetMandatoryComponentsTypes();

      // Find if a mandatory already exists and remove it if it does.
      foreach (int level in componentsByLevel.Keys)
      {
        foreach (PlatformComponent component in componentsByLevel[level])
        {
          mandatoryComponentTypes.Remove(component.GetType());
        }
      }

      // Create and add any mandatories to the sorted list.
      foreach (Type type in mandatoryComponentTypes)
      {
        ConstructorInfo info = type.GetConstructor(new Type[] { });
        if (info == null)
        {
          SystemMonitor.Warning("Mandatory type [" + type.Name + "] has no default parameterless constructor.");
          continue;
        }

        int level = ComponentManagementAttribute.GetTypeAttribute(type).ComponentLevel;
        PlatformComponent component = (PlatformComponent)info.Invoke(new object[] { });

        if (componentsByLevel.ContainsKey(level) == false)
        {
          componentsByLevel[level] = new List<PlatformComponent>();
        }

        // By default they remain invisible.
        component.UISerializationInfo.AddValue("componentVisible", false);

        componentsByLevel[level].Add(component);
      }
    }

    /// <summary>
    /// Will sort but also create any mandatory components from mandatory types.
    /// Components sorted by component level. Lower level components are closer to the platform,
    /// like sources, etc. Higher level components are user/custom components. Lowest level components
    /// are started first, and stopped last.
    /// </summary>
    public SortedList<int, List<PlatformComponent>> GetComponentsByLevel(IEnumerable<PlatformComponent> components)
    {
      SortedList<int, List<PlatformComponent>> componentsByLevel = new SortedList<int, List<PlatformComponent>>();
      lock (this)
      {
        foreach (PlatformComponent component in components)
        {
          int level = ComponentManagementAttribute.GetTypeAttribute(component.GetType()).ComponentLevel;
          if (componentsByLevel.ContainsKey(level) == false)
          {
            componentsByLevel[level] = new List<PlatformComponent>();
          }
          componentsByLevel[level].Add(component);
        }
      }

      return componentsByLevel;
    }

    /// <summary>
    /// Get the current operational state of a component.
    /// </summary>
    /// <param name="componentId"></param>
    /// <returns></returns>
    public OperationalStateEnum? GetComponentOperationalState(ComponentId id)
    {
      lock (this)
      {
        if (_components.ContainsKey(id))
        {
          return _components[id].OperationalState;
        }
      }

      return null;
    }

    /// <summary>
    /// Check if some other component is operational, by arbiter client componentId.
    /// </summary>
    /// <param name="componentId"></param>
    /// <returns></returns>
    public OperationalStateEnum? GetComponentOperationalState(ArbiterClientId? id)
    {
      if (id.HasValue == false)
      {
        SystemMonitor.Error("Component Id not valid.");
        return null;
      }

      ArbiterClientId idValue = id.Value;

      // Should a direct referencing be completely ommited, this call can be replaced with a requestMessage query to the platform.
      PlatformComponent component = GetComponentByIdentification(idValue.Id, true);
      if (component == null)
      {
        return null;
      }

      return component.OperationalState;
    }


    public ArbiterClientId[] GetComponentsArbiterIds(Type componentType)
    {
      List<PlatformComponent> components = GetComponentsByType(componentType);
      ArbiterClientId[] result = new ArbiterClientId[components.Count];

      for (int i = 0; i < components.Count; i++)
      {
        result[i] = components[i].SubscriptionClientID;
      }

      return result;
    }

    /// <summary>
    /// Get the first contained component of this type.
    /// </summary>
    /// <param name="parentType"></param>
    /// <returns></returns>
    public PlatformComponent GetFirstComponentByType(Type componentOrParentType)
    {
      List<PlatformComponent> result = GetComponentsByType(componentOrParentType);
      if (result != null && result.Count > 0)
      {
        return result[0];
      }
      return null;
    }

    /// <summary>
    /// Get components by (parent) type.
    /// </summary>
    /// <param name="componentOrParentType"></param>
    /// <returns></returns>
    public List<PlatformComponent> GetComponentsByType(Type componentOrParentType)
    {
      List<PlatformComponent> result = new List<PlatformComponent>();
      lock (this)
      {
        foreach (PlatformComponent component in _components.Values)
        {
          Type componentType = component.GetType();

          if (componentOrParentType.IsInterface)
          {// Interface checking is different.
            List<Type> interfaces = new List<Type>(componentType.GetInterfaces());
            if (interfaces.Contains(componentOrParentType))
            {
              result.Add(component);
            }
          }
          else
          {// Normal class check.
            if (componentType == componentOrParentType
                || componentType.IsSubclassOf(componentOrParentType))
            {
              result.Add(component);
            }
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Prepare the object for operation. Access to this allows externals to use the 2 
    /// step component registration process.
    /// Called to bring up a component for operation. Does not add the component
    /// permanently to the platform.
    /// </summary>
    public bool InitializeComponent(PlatformComponent component)
    {
      TracerHelper.Trace(component.Name);

      try
      {
        if (component.IsInitialized == false)
        {
          Arbiter.AddClient(component);
          // This allows the component to persist while initializing.
          component.PersistenceDataUpdatedEvent += new GeneralHelper.GenericDelegate<IDBPersistent>(HandleComponentPersistenceDataUpdatedEvent);
        }

        if (component.IsInitialized || component.Initialize(this))
        {
          return true;
        }
      }
      catch (Exception ex)
      {
        SystemMonitor.Error(string.Format("Exception occured during initializing component [{0}, {1}]", component.Name, ex.Message));
      }

      // Failed to initialize component.
      component.PersistenceDataUpdatedEvent -= new GeneralHelper.GenericDelegate<IDBPersistent>(HandleComponentPersistenceDataUpdatedEvent);
      Arbiter.RemoveClient(component);
      return false;
    }

    /// <summary>
    /// Bring down the object from operation. Access to this allows externals to 
    /// use the 2 step component registration process.
    /// Called each time a components is brough down from operation.
    /// It does not remove component from platform permanently.
    /// </summary>
    public bool UnInitializeComponent(PlatformComponent component)
    {
      TracerHelper.Trace(component.Name);
      if (component.IsInitialized)
      {
        component.UnInitialize();
      }

      if (_components.ContainsKey(component.SubscriptionClientID.Id) &&
          component.IsPersistableToDB && PersistenceHelper.UpdateToDB<PlatformComponent>(component) == false)
      {
        SystemMonitor.Error("Failed to update component [" + component.Name + "] to DB.");
      }

      component.PersistenceDataUpdatedEvent -= new GeneralHelper.GenericDelegate<IDBPersistent>(HandleComponentPersistenceDataUpdatedEvent);
      Arbiter.RemoveClient(component);

      return true;
    }

    /// <summary>
    /// Check if component type is OK for adding (since there are limitations, like the one instance limitation).
    /// </summary>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public bool CanAcceptComponent(Type componentType)
    {
      bool allowMultipleInstances = ComponentManagementAttribute.GetTypeAttribute(componentType).AllowMultipleInstances;
      if (GetComponentsByType(componentType).Count > 0 && allowMultipleInstances == false)
      {// Instance already exists and no more instances allowed.
        return false;
      }
      return true;
    }

    /// <summary>
    /// AddElement the object to the list of active objects and call event to notify all listeners, a new object has been added.
    /// Permanently adds the component to the platform.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool RegisterComponent(PlatformComponent component)
    {
      return DoRegisterComponent(component, false);
    }

    /// <summary>
    /// 
    /// </summary>
    protected bool DoRegisterComponent(PlatformComponent component, bool isInitial)
    {
      TracerHelper.Trace(component.Name);

      if (CanAcceptComponent(component.GetType()) == false)
      {
        SystemMonitor.Error("Failed to add component instance since only one instance of this type allowed.");
        return false;
      }

      if (component.SubscriptionClientID.Id.Guid == Guid.Empty)
      {
        SystemMonitor.Error("Component [" + component.GetType().Name + "] has no valid Guid Id assigned.");
        return false;
      }

      if (InitializeComponent(component) == false)
      {
        return false;
      }

      lock (this)
      {
        if (_components.ContainsValue(component))
        {
          SystemMonitor.OperationWarning("Component [" + component.GetType().Name + "] already added.");
          return true;
        }

        if (_components.ContainsKey(component.SubscriptionClientID.Id))
        {
          SystemMonitor.Error("Component with this Id [" + component.SubscriptionClientID.Id.Name + ", " + component.SubscriptionClientID.Id.ToString() + "] already added.");
          return false;
        }

        _components.Add(component.SubscriptionClientID.Id, component);
      }

      component.OperationalStateChangedEvent += new OperationalStateChangedDelegate(component_OperationalStatusChangedEvent);

      if (component.IsPersistableToDB && PersistenceHelper != null && PersistenceHelper.IsPersisted<PlatformComponent>(component) == false)
      {// New component not present in DB, persist.
        if (PersistenceHelper.Insert<PlatformComponent>(component, new KeyValuePair<string, object>("PlatformId", this.Id)) == false)
        {
          SystemMonitor.Error("Failed to insert component [" + component.Name + "] to DB.");
        }
      }

      if (ActiveComponentAddedEvent != null)
      {
        ActiveComponentAddedEvent(component, isInitial);
      }

      return true;
    }

    void component_OperationalStatusChangedEvent(IOperational component, OperationalStateEnum previousState)
    {
      SystemMonitor.CheckError(_components.ContainsValue((PlatformComponent)component), "Component not present.");
      if (ActiveComponentChangedOperationalStateEvent != null)
      {
        ActiveComponentChangedOperationalStateEvent(this, (PlatformComponent)component, previousState);
      }
    }

    void HandleComponentPersistenceDataUpdatedEvent(IDBPersistent dbComponent)
    {
      PlatformComponent component = (PlatformComponent)dbComponent;
      if (component.IsPersistableToDB)
      {
        PersistenceHelper.UpdateToDB<PlatformComponent>((PlatformComponent)component);
      }
    }

    /// <summary>
    /// Helper. Allows the un registering of component from the platform, without direct referencing.
    /// </summary>
    public bool UnInitializeComponentByIdentification(ComponentId componentId)
    {
      // For un init, only check the components initialized.
      PlatformComponent component = GetComponentByIdentification(componentId, false);
      if (component == null)
      {
        SystemMonitor.Warning("Component not found.");
        return false;
      }

      this.UnInitializeComponent(component);
      return true;
    }

    /// <summary>
    /// Helper. Allows the un registering of component from the platform, without direct referencing.
    /// </summary>
    public bool UnRegisterComponentByIdentification(ComponentId componentId)
    {
      // For unregistration look into all components, not only initialized.
      PlatformComponent component = GetComponentByIdentification(componentId, true);
      if (component == null)
      {
        //SystemMonitor.Warning("Component not found.");
        return false;
      }

      return this.UnRegisterComponent(component);
    }

    /// <summary>
    /// Obtain component reference from its arbiter client subscription Id.
    /// </summary>
    /// <param name="componentId"></param>
    /// <param name="provideUnInitializedComponents">Provide normal + uninitialized components.</param>
    /// <returns></returns>
    public PlatformComponent GetComponentByIdentification(ComponentId componentId, bool provideUnInitializedComponents)
    {
      PlatformComponent component = null;

      lock (this)
      {
        if (_components.ContainsKey(componentId) == false)
        {
          return null;
        }

        component = _components[componentId];
      }

      if (provideUnInitializedComponents || component.IsArbiterInitialized)
      {
        return component;
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// Called when a component is removed permanently from the platfrom.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool UnRegisterComponent(PlatformComponent component)
    {
      if (component == null)
      {
        return false;
      }

      TracerHelper.Trace(component.Name);

      lock (this)
      {
        if (_components.ContainsValue(component) == false)
        {
          SystemMonitor.OperationWarning("Component not registered.");
          return true;
        }

      }

      component.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(component_OperationalStatusChangedEvent);

      UnInitializeComponent(component);

      component.Dispose();

      lock (this)
      {
        _components.Remove(component.SubscriptionClientID.Id);
      }

      if (component.IsPersistableToDB
          && PersistenceHelper.Delete<PlatformComponent>(component) == false)
      {
        SystemMonitor.Error("Failed to remove component [" + component.Name + "] from DB.");
      }

      if (ActiveComponentRemovedEvent != null)
      {
        ActiveComponentRemovedEvent(component, false);
      }

      return true;
    }

    public bool RemovePersistedComponentById(long componentId)
    {
      if (PersistenceHelper.Delete<PlatformComponent>(new MatchExpression("Id", componentId)) != 1)
      {
        SystemMonitor.Error("Failed to remove persisted component [" + componentId + "].");
        return false;
      }
      return true;
    }

    #region Arbiter Messages

    /// <summary>
    /// Put a component in uninitialized state.
    /// </summary>
    /// <param name="requestMessage"></param>
    [MessageReceiver]
    void Receive(UnInitializeComponentMessage message)
    {
      if (message.ComponentId != null)
      {
        PlatformComponent component = GetComponentByIdentification(message.ComponentId.Value, false);
        if (component != null)
        {
          UnInitializeComponent(component);
        }
        else
        {
          SystemMonitor.OperationError("Component not initialized.");
        }
      }
    }


    #endregion

  }
}
