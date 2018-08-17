using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using CommonFinancial;
using System.Net;

namespace ForexPlatform
{
    /// <summary>
    /// Adapter for consuming the yahoo finance data.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Yahoo Historical Quotes")]
    public class YahooFinanceAdapter : StubIntegrationAdapter, DataSourceStub.IImplementation
    {
        DateTime _startDate = new DateTime(1900, 01, 01);
        DateTime _endDate = DateTime.Now;

        /// <summary>
        /// All yahoo stock dataDelivery is 1 day.
        /// </summary>
        TimeSpan Period = TimeSpan.FromDays(1);

        /// <summary>
        /// Constructor.
        /// </summary>
        public YahooFinanceAdapter()
        {
            DataSourceStub stub = new DataSourceStub("Yahoo Finance Data Source", false);
            stub.Initialize(this);
            base.SetStub(stub);
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public YahooFinanceAdapter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _dataSourceStub.Initialize(this);
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool OnStart(out string operationResultMessage)
        {
            operationResultMessage = string.Empty;

            if (File.Exists(Platform.Settings.BATSSymbolsFilePath) == false)
            {
                operationResultMessage = "Failed to initialize " + this.GetType().Name + " [Symbols file not found: " + Platform.Settings.BATSSymbolsFilePath + "]";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            ChangeOperationalState(OperationalStateEnum.Operational);

            GeneralHelper.FireAndForget(LoadSymbols);

            return true;
        }

        /// <summary>
        /// Helper, performs the actual loading of symbols from the BATS file.
        /// </summary>
        void LoadSymbols()
        {
            try
            {
                if (Platform == null || Platform.Settings == null)
                {
                    SystemMonitor.Error("Failed to load symbols, platform instance not available.");
                    return;
                }

                string symbolsFile = Platform.Settings.BATSSymbolsFilePath;
                System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                document.Load(symbolsFile);

                XmlNode batsNode = document.ChildNodes[1];
                XmlNode symbolsNode = batsNode.ChildNodes[0];
                List<string> symbolsNames = new List<string>();

                foreach (XmlNode node in symbolsNode)
                {
                    string symbolName = (string)node.Attributes["name"].Value;

                    // BATS and Yahoo symbols vary, so compensate.
                    symbolName = symbolName.Replace("-", "-P").Replace(".", "-").Replace("~", "-TEST").Replace("+", "-WT");

                    symbolsNames.Add(symbolName);
                }

                foreach (string symbolName in symbolsNames)
                {// For each new file name create a corresponding sessionInformation.
                    Symbol symbol = new Symbol("Stock", symbolName);
                    _dataSourceStub.AddSuggestedSymbol(symbol);
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError("Failed to load symbols [" + ex.Message + "].");
            }

            ChangeOperationalState(OperationalStateEnum.Operational);
        }
        

        /// <summary>
        /// 
        /// </summary>
        protected override bool OnStop(out string operationResultMessage)
        {
            ChangeOperationalState(OperationalStateEnum.NotOperational);

            operationResultMessage = string.Empty;
            return true;
        }

        private List<DataBar> GetSymbolData(Symbol symbol)
        {
            // TODO : place here an intelligent update caching system.
            List<DataBar> resultingData = new List<DataBar>();

            // A typical historical address looks like this (baseCurrency is "AA")
            // http://ichart.finance.yahoo.com/table.csv?s=AA&d=11&e=8&f=2008&g=d&a=0&b=2&c=1962&ignore=.csv

            StringBuilder queryBuilder = new StringBuilder("http://ichart.finance.yahoo.com/table.csv");
            queryBuilder.AppendFormat("?s={0}&d={1}&e={2}&f={3}&a={4}&b={5}&c={6}", symbol.Name,
                _endDate.Month - 1, _endDate.Day, _endDate.Year,
                _startDate.Month - 1, _startDate.Day, _startDate.Year);

            string address = queryBuilder.ToString();
            if (Uri.IsWellFormedUriString(queryBuilder.ToString(), UriKind.RelativeOrAbsolute) == false)
            {
                //address = Uri.EscapeUriString(address);
                //address = Uri.EscapeDataString(address);
            }

            using (WebClient client = new WebClient())
            {
                try
                {
                    string csvstring = client.DownloadString(address);
                    TimeSpan timePeriod;
                    using (System.IO.StringReader stringReader = new StringReader(csvstring))
                    {
                        // First line is the format orderInfo.
                        string formatInfo = stringReader.ReadLine();
                        int rows;

                        CSVDataBarReaderWriter.LoadCSVFromReader(CSVDataBarReaderWriter.DataFormat.CSVYahooFinanceQuotes, stringReader, 0, 0,
                            out timePeriod, out rows, resultingData);
                    }
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationError("Failed to retrieve stock quotes data [" + symbol + ", " + queryBuilder.ToString() + ", " + ex.Message + "]");
                    return resultingData;
                }
            }

            // Yahoo provides the stock dataDelivery with the first row being the most current.
            resultingData.Reverse();

            return resultingData;
        }

        #region IImplementation Members


        public DataHistoryUpdate GetDataHistoryUpdate(DataSessionInfo session, DataHistoryRequest request)
        {
            if (request.Period != Period)
            {
                SystemMonitor.OperationWarning("Source queried for historic information of wrong period.");
                return null;
            }

            List<DataBar> bars = GetSymbolData(session.Symbol);
            if (bars != null)
            {
                return new DataHistoryUpdate(Period, bars);
            }

            return null;
        }

        public Quote? GetQuoteUpdate(DataSessionInfo session)
        {// No quotes from this provider.
            return null;
        }

        public Dictionary<Symbol, TimeSpan[]> SearchSymbols(string symbolMatch, int resultLimit)
        {
            Dictionary<Symbol, TimeSpan[]> result = new Dictionary<Symbol, TimeSpan[]>();
            foreach (Symbol symbol in _dataSourceStub.SearchSuggestedSymbols(symbolMatch, resultLimit))
            {
                result.Add(symbol, new TimeSpan[] { Period });
            }

            return result;
        }

        public RuntimeDataSessionInformation GetSymbolSessionRuntimeInformation(Symbol symbol)
        {
            RuntimeDataSessionInformation information = _dataSourceStub.GetSymbolSessionInformation(symbol);
            if (information == null)
            {
                // It is also possible to have a symbol not in the initial BATS list.
                return new RuntimeDataSessionInformation(new DataSessionInfo(Guid.NewGuid(), "Historical Data " + symbol.Name, symbol, 1000, 4), Period);
            }
            else
            {
                return information;
            }
        }

        public void SessionDataSubscriptionUpdate(DataSessionInfo session, bool subscribe, DataSubscriptionInfo? info)
        {

        }
        #endregion
    }
}
