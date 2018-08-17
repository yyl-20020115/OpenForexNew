using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Market watch component, allows to visualize a market watch control to show the current trading values of multiple symbols
    /// at the same time.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Market Watch")]
    [ComponentManagement(false, false, int.MaxValue, true)]
    public class MarketWatchComponent : TradePlatformComponent
    {
        ISourceDataDelivery _delivery = null;
        public ISourceDataDelivery Delivery
        {
            get { return _delivery; }
        }

        ComponentId? _dataSourceId = null;
        public ComponentId? DataSourceId
        {
            get { return _dataSourceId; }
        }

        Dictionary<Symbol, SymbolInformation> _symbolQuotes = new Dictionary<Symbol, SymbolInformation>();

        public class SymbolInformation
        {
            static TimeSpan Timeout = TimeSpan.FromSeconds(3);

            Symbol _symbol;
            public Symbol Symbol
            {
                get { return _symbol; }
            }

            Quote? _quote;
            public Quote? Quote
            {
                get { return _quote; }
            }

            bool _isUpFromPrevious = true;
            public bool IsUpFromPrevious
            {
                get { return _isUpFromPrevious; }
            }

            public bool IsUpDownTimedOut
            {
                get
                {
                    return (DateTime.Now - _lastUpdate) > Timeout;
                }
            }

            DateTime _lastUpdate = DateTime.MinValue;
            public DateTime LastUpdate
            {
                get { return _lastUpdate; }
            }

            /// <summary>
            /// 
            /// </summary>
            public SymbolInformation(Symbol symbol)
            {
                _symbol = symbol;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="quote"></param>
            /// <param name="isUp"></param>
            public void Update(Quote? quote, bool isUp)
            {
                _quote = quote;
                _isUpFromPrevious = isUp;
                _lastUpdate = DateTime.Now;
            }
        }

        public IEnumerable<SymbolInformation> SymbolsUnsafe
        {
            get { return _symbolQuotes.Values; }
        }

        public delegate void QuotesUpdateDelegate(MarketWatchComponent component, SymbolInformation info);
        public event QuotesUpdateDelegate QuotesUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public MarketWatchComponent()
            : base(UserFriendlyNameAttribute.GetTypeAttributeName(typeof(MarketWatchComponent)), false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public MarketWatchComponent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _dataSourceId = (ComponentId)info.GetValue("dataSourceId", typeof(ComponentId));
            if (_dataSourceId.Value.IsEmpty)
            {
                _dataSourceId = null;
                return;
            }

            Symbol[] symbols = (Symbol[])info.GetValue("symbols", typeof(Symbol[]));
            foreach (Symbol symbol in symbols)
            {
                _symbolQuotes.Add(symbol, new SymbolInformation(symbol));
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("dataSourceId", _dataSourceId.HasValue ? _dataSourceId.Value : ComponentId.Empty);
            info.AddValue("symbols", GeneralHelper.EnumerableToArray<Symbol>(_symbolQuotes.Keys));
        }

        public bool AddSymbol(ComponentId dataSourceId, Symbol symbol, out string operationResultMessage)
        {
            if (_dataSourceId.HasValue && _dataSourceId.Value != dataSourceId)
            {
                operationResultMessage = "One source per watch component currently supported.";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            _dataSourceId = dataSourceId;
            if (_delivery == null)
            {
                _delivery = base.ObtainDataDelivery(_dataSourceId.Value);
            }

            RuntimeDataSessionInformation info = _delivery.GetSymbolRuntimeSessionInformation(symbol);
            if (info == null)
            {
                operationResultMessage = "Failed to obtain symbol runtime session information.";
                return false;
            }

            _delivery.SubscribeToData(info.Info, true, new DataSubscriptionInfo(true, false, null));
            _delivery.QuoteUpdateEvent += new QuoteUpdateDelegate(delivery_QuoteUpdateEvent);

            operationResultMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemoveSymbol(Symbol symbol)
        {
            ISourceDataDelivery delivery = _delivery;
            if (delivery != null)
            {
                RuntimeDataSessionInformation info = delivery.GetSymbolRuntimeSessionInformation(symbol);
                if (info == null)
                {
                    return false;
                }

                delivery.SubscribeToData(info.Info, false, new DataSubscriptionInfo(true, false, null));
            }

            lock (this)
            {
                _symbolQuotes.Remove(symbol);
                if (_symbolQuotes.Count == 0)
                {// Release the delivery on last.
                    //_delivery.UnInitialize();
                    //_delivery.Dispose();
                    //_delivery = null;
                    _dataSourceId = null;
                }
            }

            return true;
        }

        void delivery_QuoteUpdateEvent(ISourceDataDelivery dataDelivery, DataSessionInfo session, Quote? quote)
        {
            bool isUp = true;
            SymbolInformation information;
            lock (this)
            {
                if (_symbolQuotes.ContainsKey(session.Symbol) == false
                    || _symbolQuotes[session.Symbol] == null)
                {
                    information = new SymbolInformation(session.Symbol);
                    _symbolQuotes.Add(session.Symbol, information);
                }
                else
                {
                    information = _symbolQuotes[session.Symbol];
                    if (information.Quote.HasValue && quote.HasValue)
                    {
                        if (information.Quote.Value.Ask == quote.Value.Ask
                            && information.Quote.Value.Bid == quote.Value.Bid)
                        {
                            return;
                        }

                        isUp = information.Quote.Value.Ask < quote.Value.Ask;
                    }
                }

                information.Update(quote, isUp);
            }

            if (QuotesUpdateEvent != null)
            {
                QuotesUpdateEvent(this, information);
            }
        }

        protected override bool OnInitialize(Platform platform)
        {
            ChangeOperationalState(OperationalStateEnum.Operational);

            base.OnInitialize(platform);

            GeneralHelper.FireAndForget(delegate()
            {
                Thread.Sleep(2000);

                if (_dataSourceId.HasValue)
                {
                    _delivery = base.ObtainDataDelivery(_dataSourceId.Value);
                    if (_delivery != null)
                    {
                        _delivery.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_delivery_OperationalStatusChangedEvent);
                        if (_delivery.OperationalState == OperationalStateEnum.Operational)
                        {
                            _delivery_OperationalStatusChangedEvent(_delivery, OperationalStateEnum.Unknown);
                        }
                    }
                }
            });


            return true;
        }

        void _delivery_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            if (operational.OperationalState == OperationalStateEnum.Operational)
            {
                foreach (Symbol symbol in _symbolQuotes.Keys)
                {
                    string message;
                    AddSymbol(_dataSourceId.Value, symbol, out message);
                }
            }
            
        }

        protected override bool OnUnInitialize()
        {
            if (_delivery != null)
            {
                _delivery.UnInitialize();
            }

            ChangeOperationalState(OperationalStateEnum.NotOperational);
            return base.OnUnInitialize();
        }

    }
}
