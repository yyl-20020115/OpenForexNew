using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
  /// <summary>
  /// Expert host hosts the execution of an expert, providing it with a bridge to the resources that the
  /// platform provides, takes care of serialization, sessionInformation tradeEntities etc.
  /// </summary>
  [Serializable]
  public abstract class ExpertHost : TradePlatformComponent, ISourceAndExpertSessionManager
  {
    #region Members and Properties

    /// <summary>
    /// Only used in the initial creation, once the expert is created this type is not needed,
    /// and it is not persisted.
    /// </summary>
    volatile Type _expertType;
    public Type ExpertType
    {
      get { return _expertType; }
    }

    /// <summary>
    /// Persisted.
    /// </summary>
    volatile protected Expert _expert;
    public Expert Expert
    {
      get { return _expert; }
    }

    public PlatformManagedExpert PlatformManagedExpert
    {
      get
      {
        return _expert as PlatformManagedExpert;
      }
    }

    /// <summary>
    /// Persisted.
    /// </summary>
    List<PlatformExpertSession> _sessions = new List<PlatformExpertSession>();
    public ExpertSession[] SessionsArray
    {
      get { lock (this) { return _sessions.ToArray(); } }
    }

    /// <summary>
    /// 
    /// </summary>
    public int SessionCount
    {
      get { return _sessions.Count; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string ExpertName
    {
      get
      {
        Expert expert = _expert;
        if (expert != null)
        {
          return _expert.Name;
        }
        else
        {
          return string.Empty;
        }
      }
    }

    #endregion

    #region Events

    [field: NonSerialized]
    public event GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager> SessionsUpdateEvent;

    [field: NonSerialized]
    public event GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager, ExpertSession> SessionCreatedEvent;

    [field: NonSerialized]
    public event GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager, ExpertSession> SessionDestroyedEvent;

    [field: NonSerialized]
    public event GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager, ExpertSession> SessionDestroyingEvent;

    #endregion

    #region Construction and Instance Control

    /// <summary>
    /// Local execution. Expert to be executed locally, within the platform process 
    /// space and on the platforms arbiter.
    /// </summary>
    protected ExpertHost(string name, Type expertType)
      : base(name, false)
    {
      TracerHelper.Trace(this.Name);
      _expertType = expertType;

      base.DefaultTimeOut = TimeSpan.FromSeconds(10);
    }

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    public ExpertHost(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      _expertType = (Type)info.GetValue("expertType", typeof(Type));
      _expert = (Expert)info.GetValue("expert", typeof(Expert));
      _sessions = (List<PlatformExpertSession>)info.GetValue("existingSessions", _sessions.GetType());
    }

    /// <summary>
    /// Serialization routine.
    /// </summary>
    /// <param name="orderInfo"></param>
    /// <param name="context"></param>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("expert", _expert);
      info.AddValue("expertType", _expertType);
      info.AddValue("existingSessions", _sessions);
    }

    protected override void OnDispose()
    {
      base.OnDispose();

      // Destroy all sessions.
      while (_sessions.Count > 0)
      {
        UnRegisterExpertSession(_sessions[0]);
      }

      _expert.Dispose();
      _expert = null;
    }

    #endregion

    #region Messages Host Related

    protected override bool OnInitialize(Platform platform)
    {
      TracerHelper.TraceEntry();

      if (base.OnInitialize(platform) == false)
      {
        return false;
      }

      string expertName = this.Name + ".expert";

      // Clean expertname since we might be sending it trough command line.
      expertName = expertName.Replace(" ", "_");
      expertName = expertName.Replace("\"", "");

      lock (this)
      {
        if (_expert == null)
        {// Create the expert.

          SystemMonitor.CheckThrow(_expertType.IsSubclassOf(typeof(Expert)), "Invalid expert type passed in.");
          ConstructorInfo constructor = _expertType.GetConstructor(new Type[] { typeof(ISourceAndExpertSessionManager), typeof(string) });

          if (constructor == null)
          {// Try the second option for construction.
            constructor = _expertType.GetConstructor(new Type[] { typeof(ISourceAndExpertSessionManager) });
          }

          if (constructor == null)
          {
            SystemMonitor.Error("Failed to find corresponding constructor for expert type [" + _expertType.ToString() + "].");
            return false;
          }

          if (constructor.GetParameters().Length == 2)
          {
            _expert = (Expert)constructor.Invoke(new object[] { this, expertName });
          }
          else
          {
            _expert = (Expert)constructor.Invoke(new object[] { this });
          }
        }

        if (_expert.Initialize() == false)
        {
          SystemMonitor.Error("Expert host failed to connect to platform.");
          return false;
        }

        if (_expert != null)
        {
          _expert.PersistenceDataUpdateEvent += new Expert.ExpertUpdateDelegate(_expert_PersistenceDataUpdateEvent);
        }
      }

      foreach (PlatformExpertSession session in SessionsArray)
      {
        if (session.Initialize(null) == false)
        {
          SystemMonitor.OperationWarning("Failed to initialize session.");
        }
      }

      ChangeOperationalState(OperationalStateEnum.Operational);

      TracerHelper.TraceExit();
      return true;
    }

    void _expert_PersistenceDataUpdateEvent(Expert expert)
    {
      RaisePersistenceDataUpdatedEvent();
    }
    
    protected override bool OnUnInitialize()
    {
      base.OnUnInitialize();

      lock (this)
      {
        if (_expert != null)
        {
          _expert.UnInitialize();
          _expert.PersistenceDataUpdateEvent -= new Expert.ExpertUpdateDelegate(_expert_PersistenceDataUpdateEvent);
        }
      }

      foreach (PlatformExpertSession session in SessionsArray)
      {
        session.UnInitialize();
      }

      // [??] No need to change operational state, since it is derived from expert, and we uninitialized it.
      ChangeOperationalState(OperationalStateEnum.NotOperational);

      return true;
    }

    /// <summary>
    /// Helper, gets an expert sessionInformation by its sessionInformation orderInfo.
    /// </summary>
    /// <param name="orderInfo"></param>
    /// <returns></returns>
    public ExpertSession GetExpertSession(DataSessionInfo info)
    {
      lock (this)
      {
        foreach (ExpertSession session in _sessions)
        {
          if (session.Info.CompareTo(info) == 0)
          {
            return session;
          }
        }
      }

      return null;
    }

    #endregion

    #region ISourceManager Members

    /// <summary>
    /// Create an expert sessionInformation, by coupling the dataDelivery and order execution providers.
    /// </summary>
    /// <param name="dataProvider">Do not initialize the dataDelivery OrderExecutionProvider before passing it here, and do not add it to arbiter.</param>
    /// <param name="orderExecutionProvider">May be null, for dataDelivery only sessions; do not initialize the dataDelivery OrderExecutionProvider before passing it here, and do not add it to arbiter.</param>
    /// <returns></returns>
    public bool RegisterExpertSession(ExpertSession session)
    {
      lock (this)
      {
        _sessions.Add((PlatformExpertSession)session);
      }

      if (SessionCreatedEvent != null)
      {
        SessionCreatedEvent(this, session);
      }

      if (SessionsUpdateEvent != null)
      {
        SessionsUpdateEvent(this);
      }

      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void UnRegisterExpertSession(ExpertSession session)
    {
      lock (this)
      {
        if (SessionDestroyingEvent != null)
        {
          SessionDestroyingEvent(this, session);
        }

        if (session.OperationalState == OperationalStateEnum.Operational
            || session.OperationalState == OperationalStateEnum.Initialized)
        {
          SystemMonitor.CheckError(Arbiter != null, "Make sure entering here only when the entire host is still valid and only a session needs to be destroyed.");
          ((PlatformExpertSession)session).UnInitialize();
        }

        session.Dispose();

        _sessions.Remove((PlatformExpertSession)session);
      }

      if (SessionDestroyedEvent != null)
      {
        SessionDestroyedEvent(this, session);
      }

      if (SessionsUpdateEvent != null)
      {
        SessionsUpdateEvent(this);
      }
    }

    public ExpertSession GetExpertSessionByExecutionSource(ComponentId executionSourceId, Symbol symbol)
    {
      lock (this)
      {
        foreach (ExpertSession session in _sessions)
        {
          if (session.OrderExecutionProvider != null
              && session.OrderExecutionProvider.SourceId == executionSourceId
              && session.Info.Symbol == symbol)
          {
            return session;
          }
        }
      }

      return null;
    }

    public ExpertSession GetExpertSession(ComponentId source, Symbol symbol)
    {
      lock (this)
      {
        foreach (ExpertSession session in _sessions)
        {
          if (session.DataProvider.SourceId == source &&
              session.Info.Symbol == symbol)
          {
            return session;
          }
        }
      }

      return null;
    }

    #endregion

    public virtual ISessionDataProvider CreateRemoteExpertSession(DataSessionInfo info, ComponentId sourceId)
    {
      return this.CreateRemoteExpertSession(info, sourceId, sourceId);
    }
    public virtual ISessionDataProvider CreateRemoteExpertSession(DataSessionInfo info, ComponentId dataSourceId, ComponentId orderExecuteId)
    {
      string message = string.Empty;

      ExpertSession session = this.CreateExpertSession(info, dataSourceId, orderExecuteId, false, out message);

      if (session != null)
      {
        if (this.RegisterExpertSession(session))
        {
          return session.DataProvider;
        }
      }
      return null;
    }

    public PlatformExpertSession CreateExpertSession(DataSessionInfo? selectedSession,
        ComponentId dataSourceId, ComponentId? orderExecuteId, bool isBackTest, out string operationResultMessage)
    {
      // Create sessionInformation (mandatory)
      PlatformExpertSession session = new PlatformExpertSession(selectedSession.Value);

      ComponentId actualDataSourceId = dataSourceId;

      if (isBackTest)
      {// We shall create a special backtesting data delivery for this session, and register it
        // and use its componentId.
        BackTestDataDelivery delivery = new BackTestDataDelivery();
        if (delivery.SetInitialParameters(this, dataSourceId, session) == false
            || delivery.Initialize() == false)
        {
          operationResultMessage = "Failed to create and setup back test data delivery.";
          return null;
        }

        // Register the new delivery with its own ID.
        this.AddElement(delivery.SourceId, delivery);

        actualDataSourceId = delivery.SourceId;
      }

      // Create dataDelivery provider (mandatory).
      SessionDataProvider dataProvider = CreateSessionDataProvider(actualDataSourceId, session);
      if (dataProvider == null)
      {// Failed to create data provider.
        // When using back test data delivery, data provider creation not expected to fail so no need to handle this case.
        operationResultMessage = "Failed to create required session data provider for this source and session.";
        return null;
      }

      dataProvider.Initialize(selectedSession.Value);

      // Create order executor and order history (optional).
      ISourceOrderExecution executionProvider = null;
      if (orderExecuteId.HasValue)
      {
        executionProvider = ObtainOrderExecutionProvider(orderExecuteId.Value, actualDataSourceId);
      }

      if (session.SetInitialParameters(dataProvider, executionProvider) == false)
      {
        operationResultMessage = "Failed to set sessin initial parameters.";
        return null;
      }

      if (session.Initialize(selectedSession.Value) == false)
      {
        operationResultMessage = "Failed to initialize session.";
        session.Dispose();
        return null;
      }

      operationResultMessage = string.Empty;
      return session;
    }
  }
}
