using System;
using System.Collections.Generic;
using System.Text;
using ForexPlatform;
using CommonSupport;
using CommonFinancial;

namespace FXCMAdapter
{
    /// <summary>
    /// Native data periods for the FXCM are "t1", "m1", "m5", "m15", "m30", "H1", "D1",
    /// </summary>
    public class FXCMData : Operational, DataSourceStub.IImplementation, IDisposable
    {
        FXCMAdapter _adapter;

        DataSourceStub _dataSourceStub;
        /// <summary>
        /// 
        /// </summary>
        public DataSourceStub Stub
        {
            get { return _dataSourceStub; }
        }

        static List<TimeSpan> DefaultAvailablePeriods = new List<TimeSpan>(new TimeSpan[] {
                                                                TimeSpan.FromDays(1),
                                                                TimeSpan.FromHours(1),
                                                                TimeSpan.FromMinutes(30),
                                                                TimeSpan.FromMinutes(15),
                                                                TimeSpan.FromMinutes(5),
                                                                TimeSpan.FromMinutes(1),
                                                        });

        static string[] ForexSymbols = new string[] 
                                {
                                    "EUR/USD",
                                    "USD/JPY",
                                    "GBP/USD",
                                    "USD/CHF",
                                    "EUR/CHF",
                                    "AUD/USD",
                                    "USD/CAD",
                                    "NZD/USD",
                                    "EUR/GBP",
                                    "EUR/JPY",
                                    "GBP/JPY",
                                    "GBP/CHF",
                                };

        FXCMConnectionManager Manager
        {
            get
            {
                FXCMAdapter adapter = _adapter;
                if (adapter == null)
                {
                    return null;
                }

                return adapter.Manager;
            }
        }

        #region Instance control

        /// <summary>
        /// Constructor.
        /// </summary>
        public FXCMData(FXCMAdapter adapter, DataSourceStub stub)
        {
            _adapter = adapter;

            _dataSourceStub = stub;
            _dataSourceStub.Initialize(this);

            StatusSynchronizationEnabled = true;
            StatusSynchronizationSource = adapter;

            foreach (string symbol in ForexSymbols)
            {
                _dataSourceStub.AddSuggestedSymbol(new Symbol(Symbol.SymbolGroup.Forex, symbol));
            }
        }

        public void Dispose()
        {
            _adapter = null;
            _dataSourceStub = null;
        }

        public bool Initialize()
        {
            FXCMConnectionManager manager = Manager;
            manager.QuoteUpdatedEvent += new FXCMConnectionManager.QuoteUpdateDelegate(manager_QuoteUpdatedEvent);

            return true;
        }

        public void UnInitialize()
        {
            FXCMConnectionManager manager = Manager;
            manager.QuoteUpdatedEvent -= new FXCMConnectionManager.QuoteUpdateDelegate(manager_QuoteUpdatedEvent);
        }

        #endregion

        void manager_QuoteUpdatedEvent(FXCMConnectionManager manager, string symbolName, DataTick dataTick)
        {
            Symbol symbol = new Symbol(Symbol.SymbolGroup.Forex, symbolName);

            DataSourceStub dataSourceStub = _dataSourceStub;
            if (dataSourceStub == null)
            {
                return;
            }

            RuntimeDataSessionInformation sessionInformation = dataSourceStub.GetSymbolSessionInformation(symbol);
            if (sessionInformation == null)
            {
                return;
            }

            CombinedDataSubscriptionInformation info = dataSourceStub.GetUnsafeSessionSubscriptions(sessionInformation.Info);
            if (info != null && info.GetCombinedDataSubscription().QuoteSubscription)
            {
                dataSourceStub.UpdateQuote(sessionInformation.Info, new Quote(dataTick.Ask, dataTick.Bid, null, dataTick.DateTime), null);
                foreach (TimeSpan supportedPeriod in DefaultAvailablePeriods)
                {
                    dataSourceStub.UpdateDataHistory(sessionInformation.Info, new DataHistoryUpdate(supportedPeriod, new DataTick[] { dataTick }));
                }
            }
        }

        
        protected bool IsPeriodSupported(TimeSpan period)
        {
            return DefaultAvailablePeriods.Contains(period);
        }

        protected string GetPeriodId(TimeSpan period)
        {
            string sPeriodId = "t1";

            if (period.Days == 1)
            {
                sPeriodId = "D1";
            }
            else if (period.Hours == 1)
            {
                sPeriodId = "H1";
            }
            else if (period.Minutes == 1)
            {
                sPeriodId = "m1";
            }
            else if (period.Minutes == 5)
            {
                sPeriodId = "m5";
            }
            else if (period.Minutes == 15)
            {
                sPeriodId = "m15";
            }
            else if (period.Minutes == 30)
            {
                sPeriodId = "m30";
            }
            else if (period.Ticks == 1)
            {
                sPeriodId = "t1";
            }

            return sPeriodId;
        }

        #region IImplementation Members

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Symbol, TimeSpan[]> SearchSymbols(string symbolMatch, int resultLimit)
        {
            Dictionary<Symbol, TimeSpan[]> result = new Dictionary<Symbol, TimeSpan[]>();
            foreach (Symbol symbol in _dataSourceStub.SearchSuggestedSymbols(symbolMatch, resultLimit))
            {
                result.Add(symbol, DefaultAvailablePeriods.ToArray());
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public RuntimeDataSessionInformation GetSymbolSessionRuntimeInformation(Symbol inputSymbol)
        {
            RuntimeDataSessionInformation information = _dataSourceStub.GetSymbolSessionInformation(inputSymbol);
            FXCMConnectionManager manager = Manager;
            if (information == null && manager != null)
            {
                return new RuntimeDataSessionInformation(new DataSessionInfo(Guid.NewGuid(), inputSymbol.Name,
                    inputSymbol, 1000, (int)manager.GetInstrumentData(inputSymbol.Name, "Digits")), DefaultAvailablePeriods);
            }
            else
            {
                return information;
            }
        }

        public DataHistoryUpdate GetDataHistoryUpdate(DataSessionInfo session, DataHistoryRequest request)
        {
            if (!IsPeriodSupported(request.Period))
            {
                SystemMonitor.OperationWarning("Source queried for historic information of wrong period.");
                return null;
            }

            List<DataBar> bars = GetSymbolData(session.Symbol, request.Period);
            if (bars != null)
            {
                return new DataHistoryUpdate(request.Period, bars);
            }

            return null;
        }

        public Quote? GetQuoteUpdate(DataSessionInfo session)
        {
            FXCMConnectionManager manager = Manager;

            if (manager != null && string.IsNullOrEmpty(session.Symbol.Name) == false)
            {
                object ask = manager.GetInstrumentData(session.Symbol.Name, "Ask");
                object bid = manager.GetInstrumentData(session.Symbol.Name, "Bid");
                object time = manager.GetInstrumentData(session.Symbol.Name, "Time");

                if (ask == null || bid == null || time == null)
                {
                    return null;
                }

                return new Quote((decimal)((double)ask), (decimal)((double)bid), null, (DateTime)time);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SessionDataSubscriptionUpdate(DataSessionInfo session, bool subscribe, DataSubscriptionInfo? info)
        {
            SystemMonitor.CheckError(session.IsEmtpy == false, "Method needs valid session info assigned to operate.");

            DataSourceStub dataSourceStub = _dataSourceStub;
            if (dataSourceStub == null)
            {
                return;
            }

            CombinedDataSubscriptionInformation combined = dataSourceStub.GetUnsafeSessionSubscriptions(session);
            if (combined == null)
            {
                SystemMonitor.OperationError("Update of a non-existing session.");
                return;
            }
        }

        #endregion

        private List<DataBar> GetSymbolData(Symbol symbol, TimeSpan period)
        {
            DateTime _startDate = DateTime.Today.AddMonths(-6);
            DateTime _endDate = DateTime.Now.AddDays(1);

            List<DataBar> resultingData = null;

            try
            {
                resultingData = GetHistory(symbol.Name, period, _startDate, _endDate);
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError("Failed to retrieve stock quotes data [" + symbol + ", " + ex.Message + "]");
            }

            return resultingData;
        }

        protected List<DataBar> GetHistory(string symbol, TimeSpan period, DateTime lowerBound, DateTime upperBound)
        {
            return _adapter.Manager.GetHistory(symbol, GetPeriodId(period), lowerBound, upperBound);
        }


    }
}
