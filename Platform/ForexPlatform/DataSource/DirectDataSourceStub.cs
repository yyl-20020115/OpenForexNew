//using System;
//using System.Collections.Generic;
//using System.Text;
//using Arbiter;
//using System.Runtime.Serialization;
//using CommonSupport;
//using CommonFinancial;

//namespace ForexPlatform
//{
//    /// <summary>
//    /// Directly links the source with the client, skipping the messaging framework.
//    /// </summary>
//    [Serializable]
//    public class DirectDataSourceClientStub : 
//    {
//        //// BaseCurrency + Periods, Quotes, Ticks, Tag.
//        //Dictionary<Symbol, CombinedDataSubscriptionInformation> _symbolsRunningSessions = new Dictionary<Symbol, CombinedDataSubscriptionInformation>();

//        //List<Symbol> _suggestedSymbols = new List<Symbol>();

//        //volatile DataSourceStub.IImplementation _implementation;

//        //#region Construction And State Control

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public DirectDataSourceClientStub(string name, bool isHighPriority)
//        //    : base(name, isHighPriority ? SourceTypeEnum.DataProvider | SourceTypeEnum.Live | SourceTypeEnum.HighPriority
//        //    : SourceTypeEnum.DataProvider | SourceTypeEnum.Live)
//        //{
//        //    ChangeOperationalState(OperationalStateEnum.Constructed);
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public DirectDataSourceClientStub(SerializationInfo info, StreamingContext context)
//        //    : base(info, context)
//        //{
//        //    _symbolsRunningSessions.Clear();

//        //    ChangeOperationalState(OperationalStateEnum.Constructed);
//        //}

//        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
//        //{
//        //    base.GetObjectData(info, context);
//        //}

//        //public bool Initialize(DataSourceStub.IImplementation implementation)
//        //{
//        //    _implementation = implementation;

//        //    StatusSynchronizationSource = implementation;

//        //    return true;
//        //}

//        //#endregion

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public void AddSuggestedSymbol(Symbol symbol)
//        //{
//        //    lock (this)
//        //    {
//        //        if (_suggestedSymbols.Contains(symbol) == false)
//        //        {
//        //            _suggestedSymbols.Add(symbol);
//        //        }
//        //    }
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public void AddSession(RuntimeDataSessionInformation sessionInformation)
//        //{
//        //    lock (this)
//        //    {
//        //        _symbolsRunningSessions[sessionInformation.Info.Symbol] = new CombinedDataSubscriptionInformation(sessionInformation);
//        //    }
//        //}

//        ///// <summary>
//        ///// Find a baseCurrency based on a baseCurrency name.
//        ///// </summary>
//        //public Symbol? MapSymbolToRunningSession(string symbolName)
//        //{
//        //    lock (this)
//        //    {
//        //        foreach (Symbol symbol in _symbolsRunningSessions.Keys)
//        //        {
//        //            if (symbol.Name.ToLower() == symbolName.ToLower())
//        //            {
//        //                return symbol;
//        //            }
//        //        }
//        //    }

//        //    return null;
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public bool IsSuggestedSymbol(Symbol symbol)
//        //{
//        //    lock (this)
//        //    {
//        //        return _suggestedSymbols.Contains(symbol);
//        //    }
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public List<Symbol> SearchSuggestedSymbols(string symbolSearch, int resultLimit)
//        //{
//        //    List<Symbol> result = new List<Symbol>();
//        //    lock (this)
//        //    {
//        //        foreach (Symbol symbol in _suggestedSymbols)
//        //        {
//        //            if (result.Count >= resultLimit)
//        //            {
//        //                break;
//        //            }

//        //            if (symbol.MatchesSearchCriteria(symbolSearch))
//        //            {
//        //                result.Add(symbol);
//        //            }
//        //        }
//        //    }

//        //    return result;
//        //}


//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public RuntimeDataSessionInformation GetSymbolSessionInformation(Symbol symbol)
//        //{
//        //    lock (this)
//        //    {
//        //        if (_symbolsRunningSessions.ContainsKey(symbol))
//        //        {
//        //            return _symbolsRunningSessions[symbol].SessionInformation;
//        //        }
//        //    }
//        //    return null;
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //public List<RuntimeDataSessionInformation> GetSymbolsSessionInformation(Symbol[] symbols)
//        //{
//        //    List<RuntimeDataSessionInformation> informations = new List<RuntimeDataSessionInformation>();
//        //    lock (this)
//        //    {
//        //        foreach (Symbol symbol in symbols)
//        //        {
//        //            if (_symbolsRunningSessions.ContainsKey(symbol))
//        //            {
//        //                RuntimeDataSessionInformation info = _symbolsRunningSessions[symbol].SessionInformation;
//        //                if (info != null)
//        //                {
//        //                    informations.Add(info);
//        //                }
//        //            }
//        //        }
//        //    }

//        //    return informations;
//        //}

//        //#region Public Methods

//        //public CombinedDataSubscriptionInformation GetUnsafeSessionSubscriptions(DataSessionInfo session)
//        //{
//        //    DataSourceStub.IImplementation implementation = _implementation;
//        //    if (implementation == null)
//        //    {
//        //        return null;
//        //    }

//        //    bool isSymbolRunningSession;
//        //    lock (this)
//        //    {
//        //        isSymbolRunningSession = (_symbolsRunningSessions.ContainsKey(session.Symbol) == false);
//        //    }

//        //    if (isSymbolRunningSession)
//        //    {
//        //        SystemMonitor.OperationWarning("Received a request for unknow session, creating a new session.");
//        //        // Make sure to leave the sessionInformation orderInfo request outside of locks.
//        //        RuntimeDataSessionInformation sessionInformation = implementation.GetSymbolSessionRuntimeInformation(session.Symbol);
//        //        lock (this)
//        //        {
//        //            _symbolsRunningSessions.Add(sessionInformation.Info.Symbol, new CombinedDataSubscriptionInformation(sessionInformation));
//        //        }
//        //    }

//        //    lock (this)
//        //    {
//        //        //if (_sessionsSubscriptions.ContainsKey(session) == false)
//        //        //{
//        //        //    _sessionsSubscriptions[session] = new CombinedDataSubscriptionInformation(session);
//        //        //}

//        //        return _symbolsRunningSessions[session.Symbol];
//        //    }
//        //}

//        //public void UpdateDataHistory(DataSessionInfo session, DataHistoryUpdate update)
//        //{
//        //    if (OperationalState != OperationalStateEnum.Operational)
//        //    {
//        //        SystemMonitor.Warning("Stub used while not operational, operation ignored.");
//        //        return;
//        //    }

//        //    DataHistoryUpdateMessage message = new DataHistoryUpdateMessage(session, update, true);

//        //    CombinedDataSubscriptionInformation combined;
//        //    lock (this)
//        //    {
//        //        combined = GetUnsafeSessionSubscriptions(session);
//        //    }

//        //    lock (combined)
//        //    {
//        //        foreach (KeyValuePair<TransportInfo, DataSubscriptionInfo> info in combined.SubscriptionsUnsafe.Values)
//        //        {
//        //            if (info.Value.AcceptsUpdate(update))
//        //            {
//        //                SendResponding(info.Key, message);
//        //            }
//        //        }
//        //    }
//        //}

//        //public void UpdateQuote(DataSessionInfo session, Quote? quote, TimeSpan? period)
//        //{
//        //    if (OperationalState != OperationalStateEnum.Operational)
//        //    {
//        //        SystemMonitor.OperationWarning("Stub used while not operational, operation ignored.", TracerItem.PriorityEnum.Low);
//        //        return;
//        //    }

//        //    QuoteUpdateMessage message = new QuoteUpdateMessage(session, quote, true);

//        //    CombinedDataSubscriptionInformation combined;
//        //    lock (this)
//        //    {
//        //        combined = GetUnsafeSessionSubscriptions(session);
//        //    }

//        //    bool hasActiveSubscriber = false;

//        //    lock (combined)
//        //    {
//        //        foreach (KeyValuePair<TransportInfo, DataSubscriptionInfo> pair in combined.SubscriptionsUnsafe.Values)
//        //        {
//        //            if (pair.Value.AcceptsUpdate(quote))
//        //            {
//        //                hasActiveSubscriber = true;
//        //                SendResponding(pair.Key, message);
//        //            }
//        //        }
//        //    }

//        //    SystemMonitor.CheckOperationWarning(hasActiveSubscriber, "Quote update entered for session [" + session.Guid.ToString() + "] symbol [" + session.Symbol.Name + "] and no active subscriber found.");
//        //}

//        //#endregion

//        //void SendDataHistoryUpdate(TransportInfo[] receivers, DataSessionInfo session, DataHistoryRequest request)
//        //{
//        //    DataSourceStub.IImplementation implementation = _implementation;
//        //    if (implementation == null)
//        //    {
//        //        return;
//        //    }

//        //    DataHistoryUpdate responce = implementation.GetDataHistoryUpdate(session, request);
//        //    SendRespondingToMany(receivers, new DataHistoryUpdateMessage(session, responce, responce != null));
//        //}

//        //void SendQuoteUpdate(TransportInfo[] receivers, DataSessionInfo session)
//        //{
//        //    DataSourceStub.IImplementation implementation = _implementation;
//        //    if (implementation == null)
//        //    {
//        //        return;
//        //    }

//        //    QuoteUpdateMessage message = new QuoteUpdateMessage(session, implementation.GetQuoteUpdate(session), true);
//        //    SendRespondingToMany(receivers, message);
//        //}


//        //#region Arbiter Messages

//        //[MessageReceiver]
//        //protected virtual RequestSymbolsResponceMessage Receive(RequestSymbolsMessage message)
//        //{
//        //    RequestSymbolsResponceMessage responce = new RequestSymbolsResponceMessage(true);

//        //    DataSourceStub.IImplementation implementation = _implementation;
//        //    if (implementation != null && OperationalState == OperationalStateEnum.Operational)
//        //    {// Synchronous.
//        //        responce.SymbolsPeriods = implementation.SearchSymbols(message.SymbolMatch, message.ResultLimit);
//        //    }
//        //    else
//        //    {
//        //        responce.OperationResult = false;
//        //    }

//        //    return responce;
//        //}

//        //[MessageReceiver]
//        //protected virtual SessionsRuntimeInformationMessage Receive(RequestSymbolsRuntimeInformationMessage message)
//        //{
//        //    DataSourceStub.IImplementation implementation = _implementation;

//        //    List<RuntimeDataSessionInformation> result = new List<RuntimeDataSessionInformation>();
//        //    if (implementation != null && OperationalState == OperationalStateEnum.Operational)
//        //    {
//        //        foreach (Symbol symbol in message.Symbols)
//        //        {
//        //            lock (this)
//        //            {
//        //                if (_symbolsRunningSessions.ContainsKey(symbol) && _symbolsRunningSessions[symbol] != null)
//        //                {
//        //                    result.Add(_symbolsRunningSessions[symbol].SessionInformation);
//        //                    continue;
//        //                }
//        //            }

//        //            // Failed to find in already existing, query the implementation to create us a new one.
//        //            RuntimeDataSessionInformation sessionInformation = implementation.GetSymbolSessionRuntimeInformation(symbol);
//        //            if (sessionInformation != null)
//        //            {
//        //                lock (this)
//        //                {
//        //                    _symbolsRunningSessions[sessionInformation.Info.Symbol] = new CombinedDataSubscriptionInformation(sessionInformation);
//        //                }
//        //                result.Add(sessionInformation);
//        //            }
//        //        }
//        //    }

//        //    return new SessionsRuntimeInformationMessage(result,
//        //        implementation != null && OperationalState == OperationalStateEnum.Operational);
//        //}

//        //#endregion

//        //#region Arbiter Messages

//        //[MessageReceiver]
//        //protected virtual ResponceMessage Receive(RequestDataHistoryMessage message)
//        //{
//        //    GeneralHelper.FireAndForget(delegate()
//        //    {// Asynchronous.
//        //        SendDataHistoryUpdate(new TransportInfo[] { message.TransportInfo }, message.SessionInfo, message.Request);
//        //    });

//        //    if (message.RequestResponce)
//        //    {
//        //        return new ResponceMessage(true);
//        //    }

//        //    return null;
//        //}

//        //[MessageReceiver]
//        //protected virtual ResponceMessage Receive(RequestQuoteUpdateMessage message)
//        //{
//        //    GeneralHelper.FireAndForget(delegate()
//        //    {// Asynchronous.
//        //        SendQuoteUpdate(new TransportInfo[] { message.TransportInfo }, message.SessionInfo);
//        //    });

//        //    if (message.RequestResponce)
//        //    {
//        //        return new ResponceMessage(true);
//        //    }

//        //    return null;
//        //}

//        //[MessageReceiver]
//        //protected virtual DataSubscriptionResponceMessage Receive(DataSubscriptionRequestMessage message)
//        //{
//        //    if (message.TransportInfo.OriginalSenderId.HasValue == false)
//        //    {
//        //        SystemMonitor.Error("Received a message with no original sender. Dropped.");
//        //        return null;
//        //    }

//        //    ForexPlatform.DataSourceStub.IImplementation implementation = _implementation;
//        //    if (message.SessionInfo.IsEmtpy)
//        //    {// General (un)subscription requestMessage, not specific to a sessionInformation.

//        //        if (message.Subscribe)
//        //        {
//        //            SystemMonitor.OperationError("Unexpected combination of empty session and subscribe request, ignored.");
//        //        }
//        //        else
//        //        {// Unsubscribe to each that has a orderInfo for this original sender.
//        //            lock (this)
//        //            {
//        //                foreach (CombinedDataSubscriptionInformation combined in _symbolsRunningSessions.Values)
//        //                {
//        //                    if (combined.FullUnsubscribe(message.TransportInfo.OriginalSenderId.Value))
//        //                    {// For every sessionInformation that has something
//        //                        GeneralHelper.FireAndForget(delegate()
//        //                        {
//        //                            if (implementation != null)
//        //                            {// First allow the implementation to unsubscribe since it needs the combined dataDelivery.
//        //                                implementation.SessionDataSubscriptionUpdate(combined.SessionInformation.Info, message.Subscribe, message.Information);
//        //                            }

//        //                        });
//        //                    }
//        //                }
//        //            }
//        //        }
//        //    }
//        //    else
//        //    {
//        //        if (_symbolsRunningSessions.ContainsKey(message.SessionInfo.Symbol) == false
//        //            || _symbolsRunningSessions[message.SessionInfo.Symbol].SessionInformation.Info.Equals(message.SessionInfo) == false)
//        //        {
//        //            SystemMonitor.Warning("Subsribe request for non existing session.");
//        //            return new DataSubscriptionResponceMessage(message.SessionInfo, false);
//        //        }

//        //        CombinedDataSubscriptionInformation combined = GetUnsafeSessionSubscriptions(message.SessionInfo);
//        //        if (combined != null)
//        //        {
//        //            combined.HandleRequest(message.TransportInfo, message.Subscribe, message.Information);
//        //            if (implementation != null)
//        //            {
//        //                GeneralHelper.FireAndForget(delegate()
//        //                {
//        //                    implementation.SessionDataSubscriptionUpdate(message.SessionInfo, message.Subscribe, message.Information);
//        //                });
//        //            }
//        //        }
//        //        else
//        //        {
//        //            SystemMonitor.OperationError("Combined subscription info not found.");
//        //        }
//        //    }

//        //    // Finalizing / responding section.
//        //    if (message.RequestResponce)
//        //    {
//        //        return new DataSubscriptionResponceMessage(message.SessionInfo, true);
//        //    }

//        //    return null;
//        //}

//        //#endregion
//    }
//}
