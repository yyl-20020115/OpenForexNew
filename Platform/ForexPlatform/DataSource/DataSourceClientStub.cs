using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Arbiter;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Class is a preprovided connection implementation to a dataDelivery source (messages, states etc).
    /// It is a lightweight client, fairly stateless.
    /// </summary>
    [Serializable]
    public class DataSourceClientStub : OperationalTransportClient, ISourceDataDelivery
    {
        /// <summary>
        /// 
        /// </summary>
        TransportInfo SourceTransportInfo
        {
            get { return base.RemoteStatusSynchronizationSource; }
        }

        ComponentId _sourceId;
        /// <summary>
        /// 
        /// </summary>
        public ComponentId SourceId
        {
            get { return _sourceId; }
        }

        public ITimeControl TimeControl
        {
            get
            {
                return null;
            }
        }

        [field:NonSerialized]
        public event QuoteUpdateDelegate QuoteUpdateEvent;

        [field: NonSerialized]
        public event DataHistoryUpdateDelegate DataHistoryUpdateEvent;

        #region Construction And State Control
        
        /// <summary>
        /// 
        /// </summary>
        public DataSourceClientStub()
            : base(false)
        {
            StatusSynchronizationEnabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSourceClientStub(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StatusSynchronizationEnabled = true;
            _sourceId = (ComponentId)info.GetValue("sourceId", typeof(ComponentId));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("sourceId", _sourceId);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            StatusSynchronizationEnabled = false;
            if (Arbiter != null)
            {
                Arbiter.RemoveClient(this);
            }
        }

        /// <summary>
        /// Start the operation of the stub.
        /// </summary>
        public bool Initialize()
        {
            if (OperationalState != OperationalStateEnum.Operational)
            {
              //FIXED:
              //ChangeOperationalState(OperationalStateEnum.Initialized);
              ChangeOperationalState(OperationalStateEnum.Operational);
            }

            return true;
        }

        /// <summary>
        /// Stop the operation of the stub.
        /// </summary>
        public void UnInitialize()
        {
            StatusSynchronizationEnabled = false;
            if (OperationalState == OperationalStateEnum.Operational)
            {
                DataSubscriptionRequestMessage request = new DataSubscriptionRequestMessage(DataSessionInfo.Empty, false, null);
                request.RequestResponse = false;
                SendResponding(SourceTransportInfo, request);
            }

            ChangeOperationalState(OperationalStateEnum.UnInitialized);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SetInitialParameters(ComponentId sourceId, TransportInfo sourceTransportInfo)
        {
            if (SourceTransportInfo != null)
            {
                SystemMonitor.OperationError("Already initialized.");
                return false;
            }

            _sourceId = sourceId;

            SystemMonitor.CheckError(_sourceId == sourceTransportInfo.OriginalSenderId.Value.Id, "Possible source mismatch.");

            if (base.SetRemoteStatusSynchronizationSource(sourceTransportInfo) == false)
            {
                return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Symbol, TimeSpan[]> SearchSymbols(string symbolMatchPattern)
        {
            if (OperationalState != OperationalStateEnum.Operational)
            {
                return new Dictionary<Symbol, TimeSpan[]>();
            }

            RequestSymbolsMessage request = new RequestSymbolsMessage() { SymbolMatch = symbolMatchPattern };

            ResponseMessage response = SendAndReceiveResponding<ResponseMessage>(SourceTransportInfo, request);
            if (response != null && response.OperationResult)
            {
                return ((RequestSymbolsResponseMessage)response).SymbolsPeriods;
            }

            return new Dictionary<Symbol, TimeSpan[]>();
        }
        
        #region Arbiter Messages

        [MessageReceiver]
        protected virtual void Receive(DataHistoryUpdateMessage message)
        {
            if (OperationalState == OperationalStateEnum.Operational && message.OperationResult && DataHistoryUpdateEvent != null)
            {
                DataHistoryUpdateEvent(this, message.SessionInfo, message.Update);
            }
        }

        [MessageReceiver]
        protected virtual void Receive(QuoteUpdateMessage message)
        {
            if (OperationalState == OperationalStateEnum.Operational && message.OperationResult && QuoteUpdateEvent != null)
            {
                QuoteUpdateEvent(this, message.SessionInfo, message.Quote);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public RuntimeDataSessionInformation GetSymbolRuntimeSessionInformation(Symbol symbol)
        {
            List<RuntimeDataSessionInformation> results = GetSymbolsRuntimeSessionInformations(new Symbol[] { symbol });
            if (results == null || results.Count < 1)
            {
                return null;
            }
            return results[0];
        }

        /// <summary>
        /// 
        /// </summary>
        public List<RuntimeDataSessionInformation> GetSymbolsRuntimeSessionInformations(Symbol[] symbols)
        {
            List<RuntimeDataSessionInformation> result = new List<RuntimeDataSessionInformation>();
            if (OperationalState != OperationalStateEnum.Operational)
            {
                SystemMonitor.Warning("Using stub when not operational.");
                return result;
            }
            
            RequestSymbolsRuntimeInformationMessage request = new RequestSymbolsRuntimeInformationMessage(symbols);

            ResponseMessage response = this.SendAndReceiveResponding<ResponseMessage>(
                SourceTransportInfo, request);

            if (response == null || response.OperationResult == false)
            {
                SystemMonitor.OperationError("Symbol session runtime information obtain failed due to timeout.");
                return result;
            }

            return ((SessionsRuntimeInformationMessage)response).Informations;
        }

        public bool SubscribeToData(DataSessionInfo session, bool subscribe, DataSubscriptionInfo info)
        {
            if (OperationalState != OperationalStateEnum.Operational)
            {
                return false;
            }

            DataSubscriptionRequestMessage request = new DataSubscriptionRequestMessage(session, subscribe, info);
            DataSessionResponseMessage response = SendAndReceiveResponding<DataSessionResponseMessage>(SourceTransportInfo, request);

            return response != null && response.OperationResult;
        }

        public bool RequestQuoteUpdate(DataSessionInfo sessionInfo, bool waitResult)
        {
            if (OperationalState != OperationalStateEnum.Operational)
            {
                return false;
            }

            RequestQuoteUpdateMessage requestMessage = new RequestQuoteUpdateMessage(sessionInfo) { RequestResponse = false };
            requestMessage.RequestResponse = waitResult;
            if (waitResult)
            {
                ResponseMessage response = SendAndReceiveResponding<ResponseMessage>(SourceTransportInfo, requestMessage);
                return response != null && response.OperationResult;
            }
            else
            {
                SendResponding(SourceTransportInfo, requestMessage);
                return true;
            }
        }

        public bool RequestDataHistoryUpdate(DataSessionInfo sessionInfo, DataHistoryRequest request, bool waitResult)
        {
            if (OperationalState != OperationalStateEnum.Operational)
            {
                return false;
            }

            RequestDataHistoryMessage requestMessage = new RequestDataHistoryMessage(sessionInfo, request) { RequestResponse = false };
            requestMessage.RequestResponse = waitResult;
            if (waitResult)
            {
                ResponseMessage response = SendAndReceiveResponding<ResponseMessage>(SourceTransportInfo, requestMessage, this.DefaultTimeOut);
                return response != null && response.OperationResult;
            }
            else
            {
                SendResponding(SourceTransportInfo, requestMessage);
                return true;
            }
        }



    }
}
