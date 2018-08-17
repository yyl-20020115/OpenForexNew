using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Base class for platform components that are related to trading orders, dataDelivery information etc.
    /// </summary>
    [Serializable]
    public abstract class TradePlatformComponent : PlatformComponent, ISourceManager
    {
        #region Member Variables

        object _syncRoot = new object();

        List<ArbiterClientId?> _platformMessagePath;

        SourceInfo _dataStoreSourceInfo = new SourceInfo(new ComponentId(Guid.NewGuid(), "Data Store", null),
            SourceTypeEnum.DataProvider | SourceTypeEnum.BackTesting);

        SourceInfo _backtestingExecutionSourceInfo = new SourceInfo(new ComponentId(Guid.NewGuid(), "Backtesting Order Execution", null), 
                    SourceTypeEnum.OrderExecution | SourceTypeEnum.BackTesting);

        Dictionary<ComponentId, Dictionary<Symbol, IQuoteProvider>> _quoteProviders = new Dictionary<ComponentId,Dictionary<Symbol,IQuoteProvider>>();

        Dictionary<ComponentId, Dictionary<Symbol, Dictionary<TimeSpan, IDataBarHistoryProvider>>> _dataBarProviders = new Dictionary<ComponentId,Dictionary<Symbol,Dictionary<TimeSpan,IDataBarHistoryProvider>>>();
        
        Dictionary<ComponentId, Dictionary<Symbol, IDataTickHistoryProvider>> _dataTickProviders = new Dictionary<ComponentId,Dictionary<Symbol,IDataTickHistoryProvider>>();
        
        Dictionary<ComponentId, ISourceOrderExecution> _orderExecutionProviders = new Dictionary<ComponentId,ISourceOrderExecution>();

        Dictionary<ComponentId, ISourceDataDelivery> _dataDeliveries = new Dictionary<ComponentId, ISourceDataDelivery>();

        Dictionary<ComponentId, Dictionary<Symbol, DataSessionInfo?>> _cachedDataSessions = new Dictionary<ComponentId,Dictionary<Symbol,DataSessionInfo?>>();
        
        #endregion

        public List<ISourceOrderExecution> SourceOrderExecutionProvidersList
        {
            get
            {
                lock (_syncRoot)
                {
                    return GeneralHelper.EnumerableToList<ISourceOrderExecution>(_orderExecutionProviders.Values);
                }
            }
        }

        public List<ISourceDataDelivery> SourceDataDeliveriesList
        {
            get
            {
                lock (_syncRoot)
                {
                    return GeneralHelper.EnumerableToList<ISourceDataDelivery>(_dataDeliveries.Values);
                }
            }
        }

        public List<IDataBarHistoryProvider> DataBarProvidersList
        {
            get
            {
                List<IDataBarHistoryProvider> result = new List<IDataBarHistoryProvider>();
                lock (_syncRoot)
                {
                    foreach (ComponentId id in _dataBarProviders.Keys)
                    {
                        foreach (Symbol symbol in _dataBarProviders[id].Keys)
                        {
                            result.AddRange(GeneralHelper.EnumerableToList<IDataBarHistoryProvider>(_dataBarProviders[id][symbol].Values));
                        }
                    }

                }
                return result;
            }
        }

        public List<IQuoteProvider> QuoteProvidersList
        {
            get
            {
                List<IQuoteProvider> result = new List<IQuoteProvider>();
                lock (_syncRoot)
                {
                    foreach (ComponentId id in _quoteProviders.Keys)
                    {
                        result.AddRange(GeneralHelper.EnumerableToList<IQuoteProvider>(_quoteProviders[id].Values));
                    }
                }
                return result;
            }
        }

        public List<IDataTickHistoryProvider> DataTickProvidersList
        {
            get
            {
                List<IDataTickHistoryProvider> result = new List<IDataTickHistoryProvider>();
                lock (_syncRoot)
                {
                    foreach (ComponentId id in _dataTickProviders.Keys)
                    {
                        result.AddRange(GeneralHelper.EnumerableToList<IDataTickHistoryProvider>(_dataTickProviders[id].Values));
                    }
                }

                return result;
            }
        }

        #region Events

        [field: NonSerialized]
        public event GeneralHelper.GenericDelegate<TradePlatformComponent> SourcesUpdateEvent;

        #endregion

        #region Instance Control

        /// <summary>
        /// 
        /// </summary>
        public TradePlatformComponent(string name, bool singleThreadMode)
            : base(name, singleThreadMode)
        {
            base.Filter.AllowOnlyAddressedMessages = false;
            base.Filter.AllowedNonAddressedMessageTypes.Add(typeof(SourcesUpdateMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        public TradePlatformComponent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _quoteProviders = (Dictionary<ComponentId, Dictionary<Symbol, IQuoteProvider>>)info.GetValue("quoteProviders", typeof(Dictionary<ComponentId, Dictionary<Symbol, IQuoteProvider>>));

            _dataBarProviders = (Dictionary<ComponentId, Dictionary<Symbol, Dictionary<TimeSpan, IDataBarHistoryProvider>>>)info.GetValue("dataBarProviders", typeof(Dictionary<ComponentId, Dictionary<Symbol, Dictionary<TimeSpan, IDataBarHistoryProvider>>>));

            _dataTickProviders = (Dictionary<ComponentId, Dictionary<Symbol, IDataTickHistoryProvider>>)info.GetValue("dataTickProviders", typeof(Dictionary<ComponentId, Dictionary<Symbol, IDataTickHistoryProvider>>));

            _orderExecutionProviders = (Dictionary<ComponentId, ISourceOrderExecution>)info.GetValue("orderExecutionProviders", typeof(Dictionary<ComponentId, ISourceOrderExecution>));

            _dataDeliveries = (Dictionary<ComponentId, ISourceDataDelivery>)info.GetValue("dataDeliveries", typeof(Dictionary<ComponentId, ISourceDataDelivery>));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("quoteProviders", _quoteProviders);

            info.AddValue("dataBarProviders", _dataBarProviders);

            info.AddValue("dataTickProviders", _dataTickProviders);

            info.AddValue("orderExecutionProviders", _orderExecutionProviders);

            info.AddValue("dataDeliveries", _dataDeliveries);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool OnInitialize(Platform platform)
        {
            List<ArbiterClientId?> platformMessagePath = new List<ArbiterClientId?>();
            platformMessagePath.Add(platform.SubscriptionClientID);

            lock (_syncRoot)
            {

                if (_platformMessagePath != null)
                {// Platform must maintain its ID.
                    for (int i = 0; i < _platformMessagePath.Count; i++)
                    {
                        if (platformMessagePath.Count <= i ||
                            platformMessagePath[i].Value.Equals(_platformMessagePath[i].Value) == false)
                        {
                            SystemMonitor.Error("New platform instance, different addressing. Existing sessions will be mis-assigned.");
                            return false;
                        }
                    }
                }
                else
                {// Newly created.
                    _platformMessagePath = platformMessagePath;
                }
            }

            this.Send(new RequestSourcesMessage() { RequestResponse = false });

            global::Arbiter.Arbiter arbiter = this.Arbiter;
            if (arbiter == null)
            {
                SystemMonitor.Error("New platform instance, different addressing. Existing sessions will be mis-assigned.");
                return false;
            }

            // Now register all arbiter enabled components to the Arbiter.
            // Data deliveries go first.
            foreach (ISourceDataDelivery delivery in SourceDataDeliveriesList)
            {
                if (delivery is IArbiterClient)
                {
                    arbiter.AddClient((IArbiterClient)delivery);
                }
            }

            foreach (ISourceOrderExecution provider in SourceOrderExecutionProvidersList)
            {
                if (provider is IArbiterClient)
                {
                    arbiter.AddClient((IArbiterClient)provider);
                }
            }

            foreach (ISourceDataDelivery delivery in SourceDataDeliveriesList)
            {
                delivery.Initialize();
            }

            foreach (ISourceOrderExecution provider in SourceOrderExecutionProvidersList)
            {
                provider.Initialize();
            }

            return base.OnInitialize(platform);
        }

        protected override bool OnUnInitialize()
        {
            // Unregister all arbiter enabled components to the Arbiter.
            foreach (ISourceOrderExecution provider in SourceOrderExecutionProvidersList)
            {
                provider.UnInitialize();
                if (provider is IArbiterClient)
                {
                    this.Arbiter.RemoveClient((IArbiterClient)provider);
                }
            }

            foreach (ISourceDataDelivery delivery in SourceDataDeliveriesList)
            {
                delivery.UnInitialize();
                if (delivery is IArbiterClient)
                {
                    this.Arbiter.RemoveClient((IArbiterClient)delivery);
                }
            }

            foreach (IQuoteProvider provider in QuoteProvidersList)
            {
                //
            }

            foreach (IDataTickHistoryProvider provider in DataTickProvidersList)
            {
                //
            }

            foreach (IDataBarHistoryProvider provider in DataBarProvidersList)
            {
                //
            }

            return base.OnUnInitialize();
        }

        #endregion

        #region Implementation

        protected TransportInfo GetSourceTransportInfoToMe(ComponentId sourceId)
        {
            SourcesUpdateMessage response = this.SendAndReceive<SourcesUpdateMessage>(new GetSourceInfoMessage(sourceId));

            if (response == null || response.OperationResult == false)
            {
                return null;
            }

            TransportInfo info = null;
            if (response.Sources.Count > 0 && response.Sources[0].TransportInfo != null)
            {
                info = response.Sources[0].TransportInfo.Clone();

                info.PopForwardTransportInfo();
                info.AddForwardTransportId(this.SubscriptionClientID);
            }

            return info;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        protected bool GetSourcePath(ComponentId sourceId, out List<ArbiterClientId?> sourcePath)
        {
            SystemMonitor.CheckError(sourceId != _dataStoreSourceInfo.ComponentId, "Data store source has no path.");
            SystemMonitor.CheckError(sourceId != _backtestingExecutionSourceInfo.ComponentId, "Local execution source has no path.");

            // Go around the messaging mechanism for this one, since it seems to overload the system somehow.
            SourceManagementComponent management = (SourceManagementComponent)Platform.GetFirstComponentByType(typeof(SourceManagementComponent));
            if (management == null)
            {
                sourcePath = null;
                return false;
            }

            SourceInfo? info = management.GetSourceInfo(sourceId);
            if (info.HasValue == false)
            {
                sourcePath = null;
                return false;
            }

            sourcePath = info.Value.TransportInfo.CreateRespondingClientList();
            return true;

            //sourcePath = null;

            //SourcesUpdateMessage response = this.SendAndReceive<SourcesUpdateMessage>(
            //    new GetSourceInfoMessage(sourceId));

            //if (response == null || response.OperationResult == false)
            //{
            //    return false;
            //}

            //if (response.Sources.Count > 0)
            //{
            //    if (response.Sources[0].TransportInfo != null)
            //    {
            //        sourcePath = response.Sources[0].TransportInfo.CreateRespondingClientList();
            //    }
            //    else
            //    {
            //        sourcePath = null;
            //    }

            //    return true;
            //}

            //return false;
        }

        [MessageReceiver]
        protected void Receive(SourcesUpdateMessage message)
        {
            if (SourcesUpdateEvent != null)
            {
                SourcesUpdateEvent(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ISourceDataDelivery ObtainDataDelivery(ComponentId sourceId)
        {
            ISourceDataDelivery result = GetDataDelivery(sourceId);

            if (result != null)
            {
                return result;
            }

            result = CreateDataDelivery(sourceId);

            if (result != null)
            {
                result.Initialize();
                AddElement(sourceId, result);

                return result;
            }

            return null;
        }


        /// <summary>
        /// Create a dataDelivery delivery component, corresponding to the given sourceSourceId and account
        /// </summary>
        /// <param name="sourceSourceId"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        protected ISourceDataDelivery CreateDataDelivery(ComponentId sourceId)
        {
            if (sourceId == _dataStoreSourceInfo.ComponentId)
            {
                List<PlatformComponent> components = Platform.GetComponentsByType(typeof(DataStoreManagementComponent));
                if (components.Count == 0)
                {
                    SystemMonitor.Error("Failed to find data store management component.");
                    return null;
                }

                DataStoreDataDelivery delivery = new DataStoreDataDelivery(_dataStoreSourceInfo.ComponentId);
                if (delivery.Initialize() == false)
                {
                    SystemMonitor.OperationError("Failed to initialize data store data delivery.");
                    return null;
                }

                return delivery;
            }

            SourceInfo? sourceInfo = GetSourceInfo(sourceId, SourceTypeEnum.DataProvider);
            if (sourceInfo.HasValue == false)
            {
                SystemMonitor.OperationError("Source info not found for source [" + sourceId.Print() + "].", TracerItem.PriorityEnum.High);
                return null;
            }

            if ((sourceInfo.Value.SourceType & SourceTypeEnum.DataProvider) == 0)
            {
                SystemMonitor.OperationError("Can not create data delivery to source that does not implement data provider [" + sourceId.Print() + "].", TracerItem.PriorityEnum.Critical);
                return null;
            }

            TransportInfo info = GetSourceTransportInfoToMe(sourceId);
            if (info == null)
            {
                return null;
            }
            
            // This is a remote dataDelivery source, create a corresponding delivery.
            DataSourceClientStub stubDelivery = new DataSourceClientStub();
            Arbiter.AddClient(stubDelivery);
            stubDelivery.SetInitialParameters(sourceId, info);

            return stubDelivery;
        }

        /// <summary>
        /// Create a and setup a dataDelivery provider for this sourceSourceId, and to the given sessionInformation.
        /// </summary>
        /// <param name="sourceSourceId"></param>
        /// <returns></returns>
        public SessionDataProvider CreateSessionDataProvider(ComponentId sourceId, PlatformExpertSession session/*, bool isBackTest*/)
        {
            // Create dataDelivery delivery (mandatory).
            ISourceDataDelivery dataDelivery = ObtainDataDelivery(sourceId);

            if (dataDelivery == null)
            {
                SystemMonitor.OperationError("Failed to create data delivery.");
                return null;
            }

            SessionDataProvider dataProvider = new SessionDataProvider();

            // Setup the provider with the delivery.
            if (dataProvider.SetInitialParameters(this, sourceId, session) == false)
            {
                SystemMonitor.OperationError("Failed to create session data provider.");
                return null;
            }

            return dataProvider;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public ISourceOrderExecution ObtainOrderExecutionProvider(ComponentId sourceId, ComponentId dataSourceId)
        {
            ISourceOrderExecution result = GetOrderExecutionProvider(sourceId);
            if (result != null)
            {
                return result;
            }

            result = CreateExecutionProvider(sourceId, dataSourceId);

            if (result != null)
            {
                AddElement(sourceId, result);
                result.Initialize();
            }

            return result;
        }

        /// <summary>
        /// Create an execution provider to match the given sourceSourceId.
        /// </summary>
        /// <param name="sourceSourceId"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public ISourceOrderExecution CreateExecutionProvider(ComponentId sourceId, ComponentId dataSourceId)
        {
            SourceTypeEnum? mode = GetSourceTypeFlags(sourceId, SourceTypeEnum.OrderExecution);

            if (sourceId == _backtestingExecutionSourceInfo.ComponentId)
            {
                BackTestOrderExecutionProvider provider = new BackTestOrderExecutionProvider(sourceId);
                if (provider.SetInitialParameters(this, ObtainDataDelivery(dataSourceId)) == false)
                {
                    SystemMonitor.OperationError("Failed to initialize backtesting data provider.");
                    return null;
                }

                return provider;
            }
            else
            {
                SourceInfo? sourceInfo = GetSourceInfo(sourceId, SourceTypeEnum.OrderExecution);
                if (sourceInfo.HasValue == false)
                {
                    SystemMonitor.OperationError("Source info not found for source [" + sourceId.Print() + "].", TracerItem.PriorityEnum.High);
                    return null;
                }

                if ((sourceInfo.Value.SourceType & SourceTypeEnum.OrderExecution) == 0)
                {
                    SystemMonitor.OperationError("Can not create order execution to source that does not implement data provider [" + sourceId.Print() + "].", TracerItem.PriorityEnum.Critical);
                    return null;
                }
                
                PlatformSourceOrderExecutionProvider platformExecutionProvider = new PlatformSourceOrderExecutionProvider();
                Arbiter.AddClient(platformExecutionProvider);
                platformExecutionProvider.SetInitialParameters(this, sourceId, dataSourceId);
                return platformExecutionProvider;
            }
        }

        /// <summary>
        /// Helper, specify if local source is local or remote.
        /// </summary>
        protected bool IsLocalSource(ComponentId sourceId)
        {
            return sourceId == _dataStoreSourceInfo.ComponentId || _backtestingExecutionSourceInfo.ComponentId == sourceId
                || sourceId.IdentifiedComponentType == typeof(BackTestDataDelivery);
        }

        /// <summary>
        /// Get all sources that match the given criteria; multiple criteria can be combined in one sourceType
        /// using bitwise operators (for ex. operationResult = operationResult | SourceTypeEnum.Remote).
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="partialMatch">Should we try to match filtering criteria fully or partially.</param>
        /// <returns></returns>
        public Dictionary<Symbol, TimeSpan[]> SearchSymbols(ComponentId sourceId, string symbolMatch)
        {
            if (IsLocalSource(sourceId))
            {
                ISourceDataDelivery delivery = ObtainDataDelivery(sourceId);
                return delivery.SearchSymbols(symbolMatch);
            }

            List<ArbiterClientId?> sourcePath;
            if (GetSourcePath(sourceId, out sourcePath) == false)
            {
                SystemMonitor.OperationError("Failed to establish source path.");
                return new Dictionary<Symbol, TimeSpan[]>();
            }

            RequestSymbolsMessage request = new RequestSymbolsMessage() { SymbolMatch = symbolMatch };
            ResponseMessage result = this.SendAndReceiveForwarding<ResponseMessage>(sourcePath, request);
            if (result != null && result.OperationResult)
            {
                return ((RequestSymbolsResponseMessage)result).SymbolsPeriods;
            }

            return new Dictionary<Symbol, TimeSpan[]>();
        }

        /// <summary>
        /// Get dataDelivery sessions for baseCurrency, source must be dataDelivery source.
        /// </summary>
        public DataSessionInfo? GetSymbolDataSessionInfo(ComponentId sourceId, Symbol symbol)
        {
            lock (_cachedDataSessions)
            {
                if (_cachedDataSessions.ContainsKey(sourceId) == false)
                {
                    _cachedDataSessions.Add(sourceId, new Dictionary<Symbol, DataSessionInfo?>());
                }

                if (_cachedDataSessions[sourceId].ContainsKey(symbol) == false)
                {
                    _cachedDataSessions[sourceId].Add(symbol, null);
                }
                else if (_cachedDataSessions[sourceId][symbol].HasValue)
                {
                    return _cachedDataSessions[sourceId][symbol];
                }
            }

            if (IsLocalSource(sourceId))
            {
                ISourceDataDelivery delivery = ObtainDataDelivery(sourceId);
                if (delivery == null)
                {
                    SystemMonitor.OperationError("Failed to establish local delivery.");
                    return null;
                }

                RuntimeDataSessionInformation session = delivery.GetSymbolRuntimeSessionInformation(symbol);
                if (session == null)
                {
                    SystemMonitor.OperationError("Failed to establish symbol session [" + symbol.Name + "].");
                    return null;
                }
                return session.Info;
            }

            List<ArbiterClientId?> sourcePath;
            if (GetSourcePath(sourceId, out sourcePath) == false)
            {
                SystemMonitor.OperationError("Failed to establish source path.");
                return null;
            }

            RequestSymbolsRuntimeInformationMessage request = new RequestSymbolsRuntimeInformationMessage(new Symbol[] { symbol });
            ResponseMessage response = this.SendAndReceiveForwarding<ResponseMessage>(sourcePath, request);

            if (response != null && response.OperationResult)
            {
                SessionsRuntimeInformationMessage responseMessage = (SessionsRuntimeInformationMessage)response;
                if (responseMessage.Informations.Count > 0)
                {
                    lock (_cachedDataSessions)
                    {
                        _cachedDataSessions[sourceId][symbol] = responseMessage.Informations[0].Info;
                    }

                    return responseMessage.Informations[0].Info;
                }
            }

            return null;
        }

        /// <summary>
        /// Get sources by filtering criteria.
        /// </summary>
        public List<ComponentId> GetSources(SourceTypeEnum filteringSourceType, bool partialMatch)
        {
            List<ComponentId> result = new List<ComponentId>();

            SourcesUpdateMessage response = this.SendAndReceive<SourcesUpdateMessage>(
               new RequestSourcesMessage(filteringSourceType, partialMatch));

            if (response != null && response.OperationResult != false && response.SourcesIds.Count > 0)
            {
                result.AddRange(response.SourcesIds);
            }

            // Add local sources last, since they are of less interest usually.
            if (_dataStoreSourceInfo.MatchesSearchCritera(filteringSourceType, partialMatch))
            {
                result.Add(_dataStoreSourceInfo.ComponentId);
            }

            if (_backtestingExecutionSourceInfo.MatchesSearchCritera(filteringSourceType, partialMatch))
            {
                result.Add(_backtestingExecutionSourceInfo.ComponentId);
            }

            return result;
        }

        /// <summary>
        /// Get source orderInfo, by aplying filter, since more than one source may be on same id, but always from different type.
        /// </summary>
        /// <returns></returns>
        public SourceInfo? GetSourceInfo(ComponentId sourceId, SourceTypeEnum? filter)
        {
            if (sourceId == _dataStoreSourceInfo.ComponentId)
            {
                return _dataStoreSourceInfo;
            }

            if (sourceId == _backtestingExecutionSourceInfo.ComponentId)
            {
                return _backtestingExecutionSourceInfo;
            }

            SourcesUpdateMessage response = this.SendAndReceive<SourcesUpdateMessage>(
               new GetSourceInfoMessage(sourceId));

            if (response == null || response.OperationResult == false
                || response.Sources.Count < 1)
            {
                return null;
            }

            List<SourceTypeEnum> types = new List<SourceTypeEnum>();
            foreach (SourceInfo info in response.Sources)
            {
                if (filter.HasValue == false || (info.SourceType & filter.Value) != 0)
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the source type for source id, by aplpying filter, since more than one source may be on same id, but always from different type.
        /// </summary>
        public SourceTypeEnum? GetSourceTypeFlags(ComponentId sourceId, SourceTypeEnum? filter)
        {
            SourceInfo? info = GetSourceInfo(sourceId, filter);
            if (info.HasValue)
            {
                return info.Value.SourceType;
            }

            return null;
        }

        /// <summary>
        /// Is the source, compatible with this baseCurrency and dataDelivery source.
        /// </summary>
        public bool IsDataSourceSymbolCompatible(ComponentId sourceId, ComponentId dataSourceId, Symbol symbol, out int compatibilityLevel)
        {
            compatibilityLevel = 0;
            List<ArbiterClientId?> sourcePath;
            if (GetSourcePath(sourceId, out sourcePath) == false)
            {
                SystemMonitor.OperationError("Failed to establish source path.");
                return false;
            }

            GetDataSourceSymbolCompatibleResponseMessage response = this.SendAndReceiveForwarding<GetDataSourceSymbolCompatibleResponseMessage>(
                sourcePath, new GetDataSourceSymbolCompatibleMessage(dataSourceId, symbol));

            if (response == null || response.OperationResult == false)
            {
                return false;
            }
            
            compatibilityLevel = response.CompatibilityLevel;
            return response.IsCompatible;
        }

        #endregion

        /// <summary>
        /// Register ISourceOrderExecution.
        /// </summary>
        protected bool AddElement(ComponentId id, ISourceOrderExecution provider)
        {
            if (id.IsEmpty || provider == null)
            {
                SystemMonitor.Warning("Invalid Id or order execution provider instance.");
                return false;
            }

            lock (_syncRoot)
            {
                if (_orderExecutionProviders.ContainsKey(id))
                {
                    SystemMonitor.Warning("Failed to add order execution provider, since already added with this Id.");
                    return false;
                }

                _orderExecutionProviders.Add(id, provider);
            }

            return true;
        }

        /// <summary>
        /// Register ISourceDataDelivery.
        /// </summary>
        protected bool AddElement(ComponentId id, ISourceDataDelivery delivery)
        {
            if (id.IsEmpty || delivery == null)
            {
                SystemMonitor.Warning("Invalid Id or data delivery instance.");
                return false;
            }

            lock (_syncRoot)
            {
                if (_dataDeliveries.ContainsKey(id))
                {
                    SystemMonitor.Warning("Failed to add data delivery, since already added with this Id.");
                    return false;
                }

                foreach (ComponentId deliveryId in _dataDeliveries.Keys)
                {
                    SystemMonitor.CheckThrow(id.Name.ToLower() == id.Name.ToLower(), "Data Delivery with this name Id already present [" + id.Name + "].");
                }

                _dataDeliveries.Add(id, delivery);
            }

            return true;
        }

        /// <summary>
        /// Register IQuoteProvider.
        /// </summary>
        protected bool AddElement(ComponentId id, Symbol symbol, IQuoteProvider provider)
        {
            if (id.IsEmpty || symbol.IsEmpty || provider == null)
            {
                SystemMonitor.Warning("Invalid Id, Symbol or quote provider instance.");
                return false;
            }

            lock (_syncRoot)
            {
                if (_quoteProviders.ContainsKey(id) && _quoteProviders[id].ContainsKey(symbol))
                {
                    SystemMonitor.Warning("Failed to add order execution provider, since already added with this Id.");
                    return false;
                }

                if (_quoteProviders.ContainsKey(id) == false)
                {
                    _quoteProviders.Add(id, new Dictionary<Symbol, IQuoteProvider>());
                }

                _quoteProviders[id].Add(symbol, provider);
            }

            return true;
        }

        /// <summary>
        /// Register IDataBarHistoryProvider.
        /// </summary>
        protected bool AddElement(ComponentId id, Symbol symbol, TimeSpan period, IDataBarHistoryProvider provider)
        {
            if (id.IsEmpty || provider == null || symbol.IsEmpty)
            {
                SystemMonitor.Warning("Invalid Id, symbol or quote provider instance.");
                return false;
            }

            lock (_syncRoot)
            {
                if (_dataBarProviders.ContainsKey(id) && _dataBarProviders[id].ContainsKey(symbol)
                    && _dataBarProviders[id][symbol].ContainsKey(period))
                {
                    SystemMonitor.Warning("Failed to add order execution provider, since already added with this Id.");
                    return false;
                }

                if (_dataBarProviders.ContainsKey(id) == false)
                {
                    _dataBarProviders.Add(id, new Dictionary<Symbol, Dictionary<TimeSpan, IDataBarHistoryProvider>>());
                }

                if (_dataBarProviders[id].ContainsKey(symbol) == false)
                {
                    _dataBarProviders[id].Add(symbol, new Dictionary<TimeSpan, IDataBarHistoryProvider>());
                }

                _dataBarProviders[id][symbol].Add(period, provider);
            }

            return true;
        }

        /// <summary>
        /// Register IDataTickHistoryProvider.
        /// </summary>
        protected bool AddElement(ComponentId id, Symbol symbol, IDataTickHistoryProvider provider)
        {
            if (id.IsEmpty || symbol.IsEmpty || provider == null)
            {
                SystemMonitor.Warning("Invalid Id, Symbol or quote provider instance.");
                return false;
            }

            lock (_syncRoot)
            {
                if (_dataTickProviders.ContainsKey(id) && _dataTickProviders[id].ContainsKey(symbol))
                {
                    SystemMonitor.Warning("Failed to add order execution provider, since already added with this Id.");
                    return false;
                }

                if (_dataTickProviders.ContainsKey(id) == false)
                {
                    _dataTickProviders.Add(id, new Dictionary<Symbol, IDataTickHistoryProvider>());
                }

                _dataTickProviders[id].Add(symbol, provider);
            }

            return true;
        }


        #region ISourceManager Members

        public IQuoteProvider ObtainQuoteProvider(ComponentId source, Symbol symbol)
        {
            if (source.IsEmpty || symbol.IsEmpty)
            {
                SystemMonitor.Warning("Source or symbol empty.");
                return null;
            }

            lock (_syncRoot)
            {
                if (_quoteProviders.ContainsKey(source) &&
                    _quoteProviders[source].ContainsKey(symbol))
                {
                    return _quoteProviders[source][symbol];
                }
            }

            ISourceDataDelivery delivery = ObtainDataDelivery(source);

            if (delivery == null)
            {
                SystemMonitor.OperationError("Failed to establish data delivery for quote provider.");
                return null;
            }

            RuntimeDataSessionInformation session = delivery.GetSymbolRuntimeSessionInformation(symbol);

            if (session == null)
            {
                SystemMonitor.OperationError("Failed to establish runtime session information for symbol [" + symbol.Name + "]");
                return null;
            }

            QuoteProvider provider = new QuoteProvider(session.Info);
            
            // Make sure to *add before setting up* the initial paramters,
            // since a call from them can get back to this method, and cause a stack overflow.
            AddElement(source, symbol, provider);

            provider.SetInitialParameters(delivery);

            return provider;
        }

        public IDataBarHistoryProvider ObtainDataBarHistoryProvider(ComponentId source, Symbol symbol, TimeSpan period)
        {
            if (source.IsEmpty || symbol.IsEmpty)
            {
                SystemMonitor.Warning("Source or symbol empty.");
                return null;
            }

            lock (_syncRoot)
            {
                if (_dataBarProviders.ContainsKey(source)
                    && _dataBarProviders[source].ContainsKey(symbol)
                    && _dataBarProviders[source][symbol].ContainsKey(period))
                {
                    return _dataBarProviders[source][symbol][period];
                }
            }

            ISourceDataDelivery delivery = GetDataDelivery(source);

            if (delivery == null)
            {
                SystemMonitor.OperationError("Failed to establish data delivery for quote provider.");
                return null;
            }

            RuntimeDataSessionInformation session = delivery.GetSymbolRuntimeSessionInformation(symbol);

            if (session == null)
            {
                SystemMonitor.OperationError("Failed to establish runtime session information for symbol [" + symbol.Name + "]");
            }

            DataBarHistoryProvider provider = new DataBarHistoryProvider(delivery, session.Info, period, null);
            
            // By default assign a volume indicator.
            provider.Indicators.AddIndicator(new VolumeCustom());

            AddElement(source, symbol, period, provider);

            return provider;
        }

        public IDataTickHistoryProvider ObtainDataTickHistoryProvider(ComponentId source, Symbol symbol)
        {
            if (source.IsEmpty || symbol.IsEmpty)
            {
                SystemMonitor.Warning("Source or symbol empty.");
                return null;
            }

            lock (_syncRoot)
            {
                if (_dataTickProviders.ContainsKey(source)
                    && _dataTickProviders[source].ContainsKey(symbol))
                {
                    return _dataTickProviders[source][symbol];
                }
            }

            ISourceDataDelivery delivery = ObtainDataDelivery(source);

            if (delivery == null)
            {
                SystemMonitor.OperationError("Failed to establish data delivery for quote provider.");
                return null;
            }

            RuntimeDataSessionInformation session = delivery.GetSymbolRuntimeSessionInformation(symbol);

            if (session == null)
            {
                SystemMonitor.OperationError("Failed to establish runtime session information for symbol [" + symbol.Name + "]");
                return null;
            }

            DataTickHistoryProvider provider = new DataTickHistoryProvider(delivery, session.Info);
            AddElement(source, symbol, provider);

            return provider;
        }

        public ISourceOrderExecution GetOrderExecutionProvider(ComponentId source)
        {
            SystemMonitor.CheckWarning(source.IsEmpty == false, "Empty source id request.");

            lock (_syncRoot)
            {
                if (_orderExecutionProviders.ContainsKey(source))
                {
                    return _orderExecutionProviders[source];
                }
            }

            return null;
        }

        public ISourceDataDelivery GetDataDelivery(ComponentId source)
        {
            SystemMonitor.CheckWarning(source.IsEmpty == false, "Empty source id request.");

            lock (_syncRoot)
            {
                if (_dataDeliveries.ContainsKey(source))
                {
                    return _dataDeliveries[source];
                }
            }

            return null;
        }

        public SortedDictionary<int, List<ComponentId>> GetCompatibleOrderExecutionSources(ComponentId dataSourceId, Symbol symbol, SourceTypeEnum sourceType)
        {
            SortedDictionary<int, List<ComponentId>> result = new SortedDictionary<int, List<ComponentId>>();

            if (symbol.IsEmpty || dataSourceId.IsEmpty)
            {
                return result;
            }


            List<ComponentId> candidatesSources = GetSources(sourceType, false);

            foreach (ComponentId id in candidatesSources)
            {
                int compatibilityLevel = 0;
                bool compatible = false;

                SourceTypeEnum? type = GetSourceTypeFlags(id, SourceTypeEnum.OrderExecution);
                if (type.HasValue == false)
                {
                    continue;
                }

                type = type & SourceTypeEnum.BackTesting;
                if (type == SourceTypeEnum.BackTesting)
                {
                    compatibilityLevel = 1;
                    compatible = true;
                }
                else
                {
                    compatible = IsDataSourceSymbolCompatible(id, dataSourceId,
                        symbol, out compatibilityLevel);
                }

                // If componentId has not matched any continue condition, remote it from list.
                if (compatible)
                {
                    if (result.ContainsKey(compatibilityLevel) == false)
                    {
                        result[compatibilityLevel] = new List<ComponentId>();
                    }
                    result[compatibilityLevel].Add(id);
                }
            }

            return result;
        }

        #endregion

    }
}
