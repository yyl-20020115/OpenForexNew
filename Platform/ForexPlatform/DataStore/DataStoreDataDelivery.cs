using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Delivers historical dataDelivery directly from the central application Data Store.
    /// </summary>
    [Serializable]
    public class DataStoreDataDelivery : Operational, ISourceDataDelivery
    {
        ComponentId _sourceId;
        /// <summary>
        /// 
        /// </summary>
        public ComponentId SourceId
        {
            get
            {
                return _sourceId;
            }
        }

        public ITimeControl TimeControl
        {
            get
            {
                return null;
            }
        }

        [field: NonSerialized]
        public event QuoteUpdateDelegate QuoteUpdateEvent;
        
        [field: NonSerialized]
        public event DataHistoryUpdateDelegate DataHistoryUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public DataStoreDataDelivery(ComponentId sourceId)
        {
            _sourceId = sourceId;
            ChangeOperationalState(OperationalStateEnum.Constructed);
        }

        /// <summary>
        /// Deserialization routine.
        /// </summary>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            ChangeOperationalState(OperationalStateEnum.Constructed);
        }

        public bool Initialize()
        {
            ChangeOperationalState(OperationalStateEnum.Operational);
            return true;
        }

        public void UnInitialize()
        {
            ChangeOperationalState(OperationalStateEnum.UnInitialized);
        }

        /// <summary>
        /// Release any permanently held refences and resources.
        /// </summary>
        public void Dispose()
        {
            UnInitialize();
        }

        public Dictionary<Symbol, TimeSpan[]> SearchSymbols(string symbolMatchPattern)
        {
            Dictionary<Symbol, TimeSpan[]> result = new Dictionary<Symbol,TimeSpan[]>();
            if (DataStore.Instance == null || DataStore.Instance.OperationalState != OperationalStateEnum.Operational)
            {
                SystemMonitor.OperationWarning("Data store not assigned or not operational.");
                return result;
            }

            foreach (RuntimeDataSessionInformation info in DataStore.Instance.Sessions)
            {
                result.Add(info.Info.Symbol, info.AvailableDataBarPeriods.ToArray());
            }

            return result;
        }

        public RuntimeDataSessionInformation GetSymbolRuntimeSessionInformation(Symbol symbol)
        {
            List<RuntimeDataSessionInformation> result = GetSymbolsRuntimeSessionInformations(new Symbol[] { symbol });
            if (result.Count > 0)
            {
                return result[0];
            }

            return null;
        }

        public List<RuntimeDataSessionInformation> GetSymbolsRuntimeSessionInformations(Symbol[] symbols)
        {
            List<Symbol> inputSymbols = new List<Symbol>(symbols);

            List<RuntimeDataSessionInformation> result = new List<RuntimeDataSessionInformation>();
            if (DataStore.Instance == null || DataStore.Instance.OperationalState != OperationalStateEnum.Operational)
            {
                return result;
            }

            foreach (RuntimeDataSessionInformation session in DataStore.Instance.Sessions)
            {
                if (inputSymbols.Contains(session.Info.Symbol))
                {
                    result.Add(session);
                }
            }

            return result;
        }

        /// <summary>
        /// As this is a static dataDelivery only provider, quote updates are not accepted.
        /// </summary>
        public bool RequestQuoteUpdate(DataSessionInfo sessionInfo, bool waitResult)
        {
            return false;
        }

        /// <summary>
        /// Request bar dataDelivery from entry.
        /// </summary>
        public bool RequestDataHistoryUpdate(DataSessionInfo sessionInfo, DataHistoryRequest request, bool waitResult)
        {
            DataStoreEntry entry = DataStore.Instance.GetEntryBySessionInfo(sessionInfo);

            if (this.OperationalState != OperationalStateEnum.Operational
                || entry == null)
            {
                SystemMonitor.OperationError("Data history request received while not operational, or invalid session requrested.");
                return false;
            }

            if (entry.Period != request.Period)
            {
                SystemMonitor.OperationError("Data history request received but period not recognized.");
                return false;
            }

            if (request.MaxValuesRetrieved.HasValue == false)
            {
                request.MaxValuesRetrieved = int.MaxValue;
            }

            if (request.StartIndex.HasValue == false)
            {
                request.StartIndex = -1;
            }

            GeneralHelper.GenericReturnDelegate<bool> operationDelegate = delegate()
            {
                if (request.IsTickBased)
                {
                    DataReaderWriter<DataTick> readerWriter = entry.GetDataTickReaderWriter();

                    List<DataTick> dataTicks;
                    DataHistoryUpdate update = new DataHistoryUpdate(request.Period, new DataTick[] { });
                    if (readerWriter.Read(request.StartIndex.Value, request.MaxValuesRetrieved.Value, out dataTicks))
                    {
                        update.DataTicksUnsafe.AddRange(dataTicks);

                        if (DataHistoryUpdateEvent != null)
                        {
                            DataHistoryUpdateEvent(this, sessionInfo, update);
                        }

                        return true;
                    }
                }
                else
                {
                    DataReaderWriter<DataBar> readerWriter = entry.GetDataBarReaderWriter();
                    if (readerWriter == null)
                    {
                        SystemMonitor.OperationError("Failed to establish file reader writer for entry.");
                        return false;
                    }

                    List<DataBar> dataBars;
                    DataHistoryUpdate update = new DataHistoryUpdate(request.Period, new DataBar[] { });

                    bool readResult = false;
                    if (request.StartIndex.Value < 0)
                    {// Instruction is to read the last count items.
                        readResult = readerWriter.ReadLast(
                            request.MaxValuesRetrieved.Value, out dataBars);
                    }
                    else
                    {
                        readResult = readerWriter.Read(request.StartIndex.Value,
                            request.MaxValuesRetrieved.Value, out dataBars);
                    }

                    if (readResult)
                    {
                        update.DataBarsUnsafe.AddRange(dataBars);

                        if (DataHistoryUpdateEvent != null)
                        {
                            DataHistoryUpdateEvent(this, sessionInfo, update);
                        }

                        return true;
                    }
                }

                return false;
            };

            if (waitResult)
            {
                return operationDelegate();
            }
            else
            {
                GeneralHelper.FireAndForget(operationDelegate);
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SubscribeToData(DataSessionInfo session, bool subscribe, DataSubscriptionInfo subscription)
        {
            return true;
        }

    }
}
