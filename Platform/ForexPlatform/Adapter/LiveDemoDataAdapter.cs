using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using CommonSupport;
using CommonFinancial;
using System.Timers;

namespace ForexPlatform
{
    /// <summary>
    /// Class provides a live demo data source, that can be used for testing with its random data.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Live Demo Adapter")]
    public class LiveDemoDataAdapter : StubIntegrationAdapter, DataSourceStub.IImplementation
    {
        DateTime _dateTime;

        Timer _liveDataTimer;

        TimeSpan _period = TimeSpan.FromMinutes(15);

        RuntimeDataSessionInformation _sessionInformation;

        List<DataBar> _history = new List<DataBar>();

        /// <summary>
        /// The interval of the timer for the generation of new items.
        /// </summary>
        public double TimerIntervalMilliseconds
        {
            get
            {
                if (_liveDataTimer != null)
                {
                    return _liveDataTimer.Interval;
                }

                return 0;
            }

            set 
            {
                if (_liveDataTimer != null)
                {
                    _liveDataTimer.Interval = value;
                }
            }
        }

        /// <summary>
        /// Info of the last data bar generated.
        /// </summary>
        private DataBar LastBar
        {
            get
            {
                if (_history.Count > 0)
                {
                    return _history[_history.Count - 1];
                }
                return DataBar.Empty;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LiveDemoDataAdapter()
        {
            DataSourceStub stub = new DataSourceStub("Live Demo Data Source", false);
            _sessionInformation = new RuntimeDataSessionInformation(new DataSessionInfo(Guid.NewGuid(), "DEMO150", new Symbol("Unknown", "DEMO10D15"), 10000, 4), _period);
            base.SetStub(stub);

            Construct();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LiveDemoDataAdapter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _sessionInformation = (RuntimeDataSessionInformation)info.GetValue("sessionInformation", typeof(RuntimeDataSessionInformation));
            
            Construct();
            // Make sure to run Construct first, since it creates the timer instance.
            _liveDataTimer.Interval = info.GetDouble("timerInterval");
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("sessionInformation", _sessionInformation);
            info.AddValue("timerInterval", _liveDataTimer.Interval);
            base.GetObjectData(info, context);
        }

        protected void Construct()
        {
            _dataSourceStub.Initialize(this);
            _dataSourceStub.AddSuggestedSymbol(_sessionInformation.Info.Symbol);
            _dataSourceStub.AddSession(_sessionInformation);

            _dateTime = DateTime.Now;

            _liveDataTimer = new Timer(750);
            _liveDataTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerTimeout);
            _liveDataTimer.AutoReset = false;

            _liveDataTimer.Start();

            for (int i = 0; i < 100; i++)
            {
                GenerateNextRandomBar();
            }
        }


        protected override bool OnStart(out string operationResultMessage)
        {
            operationResultMessage = string.Empty;
            ChangeOperationalState(OperationalStateEnum.Operational);

            return true;
        }

        protected override bool OnStop(out string operationResultMessage)
        {
            operationResultMessage = string.Empty;
            ChangeOperationalState(OperationalStateEnum.NotOperational);
            _liveDataTimer.Stop();

            return true;
        }

        /// <summary>
        /// Helper, generate a new random data bar.
        /// </summary>
        DataBar GenerateNextRandomBar()
        {
            DataBar result;
            if (_history.Count == 0)
            {
                decimal startPrice = 0.9m;
                result = new DataBar() { DateTime = _dateTime, Close = startPrice, Open = startPrice - 0.005m, High = startPrice + 0.005m, Low = startPrice - 0.012m, Volume = 12 };
            }
            else
            {
                _dateTime = _dateTime.Add(_period);

                decimal movement = Math.Round(GeneralHelper.Random(-0.01m, 0.01m), 4);
                decimal open = LastBar.Close;
                decimal close = open + movement;
                decimal high = Math.Max(open, close) + Math.Round(GeneralHelper.Random(0, 0.005m), 4);
                decimal low = Math.Min(open, close) + Math.Round(GeneralHelper.Random(-0.005m, 0), 4);
                decimal volume = GeneralHelper.Random(1, 20);
                result = new DataBar(_dateTime, open, high, low, close, volume);
            }

            _history.Add(result);
            return result;
        }


        /// <summary>
        /// On timer timeout send a newly generated bar to receivers.
        /// </summary>
        public void TimerTimeout(object source, System.Timers.ElapsedEventArgs e)
        {
            GenerateNextRandomBar();

            CombinedDataSubscriptionInformation info = _dataSourceStub.GetUnsafeSessionSubscriptions(_sessionInformation.Info);

            if (info != null && info.GetCombinedDataSubscription().QuoteSubscription)
            {
                _dataSourceStub.UpdateQuote(_sessionInformation.Info, new Quote(LastBar.Open, LastBar.Open - 0.002m, null, DateTime.Now), null);
                _dataSourceStub.UpdateDataHistory(_sessionInformation.Info, new DataHistoryUpdate(_period, new DataBar[] { LastBar }));
            }

            if (OperationalState != OperationalStateEnum.UnInitialized &&
                OperationalState != OperationalStateEnum.Disposed)
            {
                _liveDataTimer.Start();
            }
        }

        #region Implementation Members

        public DataHistoryUpdate GetDataHistoryUpdate(DataSessionInfo session, DataHistoryRequest request)
        {
            if (request.IsTickBased)
            {
                SystemMonitor.NotImplementedWarning();
                return new DataHistoryUpdate(request.Period, new DataTick[] { });
            }

            if (request.Period != _period)
            {
                return new DataHistoryUpdate(request.Period, new DataBar[] { });
            }

            lock (this)
            {
                return new DataHistoryUpdate(request.Period, _history.ToArray());
            }
        }

        public Quote? GetQuoteUpdate(DataSessionInfo session)
        {
            return new Quote(LastBar.Open, LastBar.Close, null, null);
        }

        public Dictionary<Symbol, TimeSpan[]> SearchSymbols(string symbolMatch, int resultLimit)
        {
            Dictionary<Symbol, TimeSpan[]> result = new Dictionary<Symbol, TimeSpan[]>();

            lock (this)
            {
                if (_sessionInformation.Info.Symbol.MatchesSearchCriteria(symbolMatch))
                {
                    result.Add(_sessionInformation.Info.Symbol, new TimeSpan[] { _period });
                }
            }

            return result;
        }

        public RuntimeDataSessionInformation GetSymbolSessionRuntimeInformation(Symbol symbol)
        {
            return _dataSourceStub.GetSymbolSessionInformation(symbol);
        }

        public void SessionDataSubscriptionUpdate(DataSessionInfo session, bool subscribe, DataSubscriptionInfo? info)
        {
        }

        #endregion
    }
}
