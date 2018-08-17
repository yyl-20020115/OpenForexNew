using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using CommonSupport;
using ForexPlatform;
using Arbiter;

namespace MT4Adapter
{
    /// <summary>
    /// Data adapter layer.
    /// </summary>
    public abstract class MT4RemoteAdapterData : MT4RemoteAdapterCommon
    {
        const int DefaultValuesRetrieved = 500;

        /// <summary>
        /// Expert ID vs Session instance.
        /// </summary>
        Dictionary<Symbol, CombinedDataSubscriptionInformation> _dataSessions = new Dictionary<Symbol, CombinedDataSubscriptionInformation>();

        /// <summary>
        /// 
        /// </summary>
        public MT4RemoteAdapterData(Uri serverUri)
            : base(serverUri)
        {

        }

        void SendToDataSubscribers(CombinedDataSubscriptionInformation session, QuoteUpdateMessage quoteMessage,  
            DataHistoryUpdateMessage updateMessage)
        {
            TracerHelper.TraceEntry();

            lock (this)
            {
                // TODO: make sure proper subscription based filtering is applied here too.
                foreach (KeyValuePair<TransportInfo, DataSubscriptionInfo> pair in session.SubscriptionsUnsafe.Values)
                {

                    if (quoteMessage != null && pair.Value.AcceptsUpdate(quoteMessage.Quote))
                    {
                        TracerHelper.Trace("Sending [" + quoteMessage.GetType().Name + "] to [" + pair.Key.OriginalSenderId.Value.Id.Name + "].");
                        SendResponding(pair.Key, quoteMessage);
                    }

                    if (updateMessage != null && pair.Value.AcceptsUpdate(updateMessage.Update))
                    {
                        TracerHelper.Trace("Sending [" + updateMessage.GetType().Name + "] to [" + pair.Key.OriginalSenderId.Value.Id.Name + "].");
                        SendResponding(pair.Key, updateMessage);
                    }
                }
            }
        }

        [MessageReceiver]
        protected virtual SessionsRuntimeInformationMessage Receive(RequestSymbolsRuntimeInformationMessage message)
        {
            List<RuntimeDataSessionInformation> result = new List<RuntimeDataSessionInformation>();
            if (OperationalState == OperationalStateEnum.Operational)
            {
                foreach (Symbol symbol in message.Symbols)
                {
                    lock (this)
                    {
                        if (_dataSessions.ContainsKey(symbol) && _dataSessions[symbol] != null)
                        {
                            result.Add(_dataSessions[symbol].SessionInformation);
                            continue;
                        }
                    }
                }
            }

            return new SessionsRuntimeInformationMessage(result, OperationalState == OperationalStateEnum.Operational);
        }

        [MessageReceiver]
        protected RequestSymbolsResponseMessage Receive(RequestSymbolsMessage message)
        {
            RequestSymbolsResponseMessage response = new RequestSymbolsResponseMessage(true);
            foreach(Symbol symbol in _dataSessions.Keys)
            {
                if (_dataSessions[symbol].SessionInformation.Info.Symbol.MatchesSearchCriteria(message.SymbolMatch))
                {
                    response.SymbolsPeriods.Add(_dataSessions[symbol].SessionInformation.Info.Symbol, _dataSessions[symbol].SessionInformation.AvailableDataBarPeriods.ToArray());
                }
            }

            return response;
        }

        [MessageReceiver]
        protected virtual DataSubscriptionResponseMessage Receive(DataSubscriptionRequestMessage message)
        {
            if (message.TransportInfo.OriginalSenderId.HasValue == false)
            {
                SystemMonitor.Error("Received a message with no original sender. Dropped.");
                return null;
            }

            if (message.SessionInfo.IsEmtpy)
            {// General (un)subscription requestMessage, not specific to a sessionInformation.

                if (message.Subscribe)
                {
                    SystemMonitor.OperationError("Unexpected combination of empty session and subscribe request, ignored.");
                }
                else
                {// Unsubscribe to each that has a orderInfo for this original sender.
                    lock (this)
                    {
                        foreach (CombinedDataSubscriptionInformation combined in _dataSessions.Values)
                        {
                            if (combined.FullUnsubscribe(message.TransportInfo.OriginalSenderId.Value) == false)
                            {
                                SystemMonitor.OperationError("Failed to unsubscribe [" + message.TransportInfo.OriginalSenderId.Value.Id.Name + "].");
                            }
                        }
                    }
                }
            }
            else
            {
                lock (this)
                {
                    if (_dataSessions.ContainsKey(message.SessionInfo.Symbol) == false
                        || _dataSessions[message.SessionInfo.Symbol].SessionInformation.Info.Equals(message.SessionInfo) == false)
                    {
                        SystemMonitor.Warning("Subsribe request for non existing session.");
                        return new DataSubscriptionResponseMessage(message.SessionInfo, false);
                    }

                    CombinedDataSubscriptionInformation combined = _dataSessions[message.SessionInfo.Symbol];
                    if (combined != null)
                    {
                        TracerHelper.Trace("Subscribing... " + message.TransportInfo.OriginalSenderId.Value.Id.Name);
                        combined.HandleRequest(message.TransportInfo, message.Subscribe, message.Information);
                    }
                    else
                    {
                        SystemMonitor.OperationError("Combined subscription info not found.");
                    }
                }
            }

            // Finalizing / responding section.
            if (message.RequestResponse)
            {
                return new DataSubscriptionResponseMessage(message.SessionInfo, true);
            }

            return null;
        }

        #region Helpers

        /// <summary>
        /// Since the MT4 uses its slippage in points, we need to convert.
        /// </summary>
        /// <param name="normalValue"></param>
        /// <returns></returns>
        protected int ConvertSlippage(Symbol symbol, decimal? normalValue)
        {
            RuntimeDataSessionInformation session = null;

            //string convertedSymbol = ConvertSymbol(symbol);
            lock (this)
            {
                if (_dataSessions.ContainsKey(symbol))
                {
                    session = _dataSessions[symbol].SessionInformation;
                }
            }

            if (normalValue.HasValue == false)
            {// -1 encoded for full value.
                return -1;
            }

            if (session == null)
            {
                SystemMonitor.Error("Session for symbol [" + symbol + "] not created yet. Slippage conversion failed.");
                return -1;
            }

            return (int)((double)normalValue * Math.Pow(10, session.Info.DecimalPlaces));
        }

        protected CombinedDataSubscriptionInformation GetDataSession(Symbol symbol)
        {
            lock (this)
            {
                if (_dataSessions.ContainsKey(symbol))
                {
                    return _dataSessions[symbol];
                }
            }

            return null;
        }

        protected CombinedDataSubscriptionInformation GetDataSession(string symbol)
        {
            Symbol symbolItem = CreateSymbol(symbol);
            return GetDataSession(symbolItem);
        }

        /// <summary>
        /// Helper, converts raw dataDelivery to dataDelivery bar structures.
        /// </summary>
        /// <returns></returns>
        public List<DataBar> GenerateDataBars(TimeSpan period,
            Int64[] times, decimal[] opens, decimal[] closes, decimal[] highs, decimal[] lows, decimal[] volumes)
        {
            List<DataBar> result = new List<DataBar>();

            for (int i = 0; i < times.Length && i < opens.Length && i < highs.Length
                && i < closes.Length && i < volumes.Length; i++)
            {
                DateTime dateTime = GeneralHelper.GenerateDateTimeSecondsFrom1970(times[i]).Value;
                result.Add(new DataBar(dateTime, opens[i], highs[i], lows[i], closes[i], volumes[i]));
            }

            return result;
        }

        #endregion

        #region MT4 Incoming Calls

        /// <summary>
        /// 
        /// </summary>
        public void Quotes(string symbol, int operationId, double ask, double bid, double open, double close,
            double low, double high, double volume, double time)
        {
            TracerHelper.TraceEntry();

            CombinedDataSubscriptionInformation session = GetDataSession(symbol);
            if (session == null)
            {
                SystemMonitor.Error("Failed to find symbol session [" + symbol + "], quotes not sent.");
                return;
            }

            try
            {
                QuoteUpdateMessage message = new QuoteUpdateMessage(session.SessionInformation.Info, 
                        new Quote((decimal)ask, (decimal)bid, (decimal)open, (decimal)close, (decimal)high, (decimal)low, (decimal)volume,
                            GeneralHelper.GenerateDateTimeSecondsFrom1970((long)time)), true);

                SendToDataSubscribers(session, message, null);
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

        }

        /// <summary>
        /// OperationId is not mandatory - but is should be there when the update was requested by a special recepient.
        /// </summary>
        public void TradingValuesUpdate(string symbol, int operationId, double time, int period, int availableBarsCount,
            Int64[] times, decimal[] opens, decimal[] closes, decimal[] highs, decimal[] lows, decimal[] volumes)
        {
            TracerHelper.TraceEntry();

            CombinedDataSubscriptionInformation session = GetDataSession(symbol);
            if (session == null)
            {
                SystemMonitor.Error("Failed to find symbol session [" + symbol + "], quotes not sent.");
                return;
            }

            try
            {
                // History update.
                TimeSpan periodValue = TimeSpan.FromMinutes(period);

                DataHistoryUpdate update = new DataHistoryUpdate(periodValue,
                    GenerateDataBars(periodValue, times, opens, closes, highs, lows, volumes));

                update.AvailableHistorySize = availableBarsCount;

                DataHistoryUpdateMessage message = new DataHistoryUpdateMessage(session.SessionInformation.Info, update, true);

                SendToDataSubscribers(session, null, message);
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

            TracerHelper.TraceExit();
        }

        protected Symbol CreateSymbol(string symbol)
        {
            return new Symbol(string.Empty, symbol);
        }

        public bool InitializeIntegrationSession(string symbol, 
            decimal modePoint, decimal modeDigits, decimal modeSpread, decimal modeStopLevel, decimal modeLotSize, decimal modeTickValue,
            decimal modeTickSize, decimal modeSwapLong, decimal modeSwapShort, decimal modeStarting, decimal modeExpiration,
            decimal modeTradeAllowed, decimal modeMinLot, decimal modeLotStep, decimal modeMaxLot, decimal modeSwapType,
            decimal modeProfitCalcMode, decimal modeMarginCalcMode, decimal modeMarginInit, decimal modeMarginMaintenance,
            decimal modeMarginHedged, decimal modeMarginRequired, decimal modeFreezeLevel)
        {
            TracerHelper.Trace(symbol);
            CombinedDataSubscriptionInformation session = GetDataSession(symbol);
            
            if (session == null)
            {
                DataSessionInfo sessionInfo = new DataSessionInfo(Guid.NewGuid(), symbol,
                    CreateSymbol(symbol), modeLotSize, (int)modeDigits);

                RuntimeDataSessionInformation runtimeSession = new RuntimeDataSessionInformation(sessionInfo);

                session = new CombinedDataSubscriptionInformation(runtimeSession);

                lock (this)
                {
                    _dataSessions.Add(sessionInfo.Symbol, session);
                }
            }

            return true;
        }

        public bool AddSessionPeriod(string symbol, int period)
        {
            TracerHelper.TraceEntry();

            TimeSpan span = TimeSpan.FromMinutes(period);

            CombinedDataSubscriptionInformation session = GetDataSession(symbol);
            if (session == null)
            {
                SystemMonitor.Error("Session not found [" + symbol + "], periods not set.");
                return false;
            }

            lock (this)
            {
                if (session.SessionInformation.AvailableDataBarPeriods.Contains(span) == false)
                {
                    session.SessionInformation.AvailableDataBarPeriods.Add(span);
                }
            }

            return true;
        }

        // << Result format : operationID if > 0; preffered count; string symbol
        public string RequestValues(string expertId)
        {
            TracerHelper.TraceEntry(expertId);

            string symbolName = ExpertIdToSymbolName(expertId);

            try
            {
                // We need to look trough all the request messages to find a one suitable for the requesting session.
                RequestDataHistoryMessage dataHistoryMessage = null;
                string operationId = string.Empty;
                lock (this)
                {
                    foreach (OperationInformation info in GetPendingMessagesOperations<RequestDataHistoryMessage>(false))
                    {
                        RequestDataHistoryMessage message = (RequestDataHistoryMessage)info.Request;
                        if (info.IsStarted == false && message.SessionInfo.Symbol.Name.ToLower() == symbolName.ToLower())
                        {// Message match.
                            info.Start();
                            operationId = info.Id;
                            dataHistoryMessage = message;
                            break;
                        }
                    }
                }

                if (dataHistoryMessage != null)
                {
                    TracerHelper.Trace("RequestDataHistoryMessage >> " + dataHistoryMessage.SessionInfo.Name);
                    
                    int prefferedValueCount = DefaultValuesRetrieved;
                    if (dataHistoryMessage.Request.MaxValuesRetrieved.HasValue)
                    {
                        prefferedValueCount = dataHistoryMessage.Request.MaxValuesRetrieved.Value;
                    }

                    string result = operationId.ToString() + SeparatorSymbol + prefferedValueCount.ToString() + SeparatorSymbol + dataHistoryMessage.SessionInfo.Symbol.Name;
                    TracerHelper.Trace(result);
                    return result;
                }

                // We need to look trough all the request messages to find a one suitable for the requesting session.
                RequestQuoteUpdateMessage quoteUpdateMessage = null;
                operationId = string.Empty;
                lock (this)
                {
                    foreach (OperationInformation info in GetPendingMessagesOperations<RequestQuoteUpdateMessage>(false))
                    {
                        RequestQuoteUpdateMessage message = (RequestQuoteUpdateMessage)info.Request;
                        if (info.IsStarted == false && message.SessionInfo.Symbol.Name.ToLower() == symbolName.ToLower())
                        {// Message match.
                            info.Start();
                            operationId = info.Id;
                            quoteUpdateMessage = message;
                            break;
                        }
                    }
                }

                if (quoteUpdateMessage != null)
                {
                    TracerHelper.Trace(quoteUpdateMessage.SessionInfo.Name);
                    
                    int prefferedValueCount = 0;
                    string result = operationId.ToString() + SeparatorSymbol + prefferedValueCount.ToString() + SeparatorSymbol + quoteUpdateMessage.SessionInfo.Symbol.Name;

                    TracerHelper.Trace(result);
                    return result;
                }
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

            return string.Empty;
        }

        #endregion

    }
}
