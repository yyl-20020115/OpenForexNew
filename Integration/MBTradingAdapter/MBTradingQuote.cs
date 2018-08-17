using System;
using System.Collections.Generic;
using System.Text;
using MBTQUOTELib;
using CommonFinancial;
using CommonSupport;


namespace MBTradingAdapter
{
    /// <summary>
    /// 
    /// </summary>
    public class MBTradingQuote : IMbtQuotesNotify, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public class SessionQuoteInformation
        {
            public Symbol Symbol;
            public Quote? Quote;
			public Commission? Commission; // commission - multiplied to the quote values;
        }

        volatile MbtQuotes _quotesClient;

        bool IsInitialized
        {
            get { return _quotesClient != null; }
        }

        volatile Dictionary<string, SessionQuoteInformation> _sessionsHotSwap = new Dictionary<string, SessionQuoteInformation>();

        /// <summary>
        /// This is a hot swappable collection, so read safely (but do not modify).
        /// </summary>
        public Dictionary<string, SessionQuoteInformation> SessionsQuotes
        {

            get { return _sessionsHotSwap; }

        }

        BackgroundMessageLoopOperator _messageLoopOperator;

        MBTradingConnectionManager _manager;

        public delegate void QuoteUpdateDelegate(MBTradingQuote keeper, SessionQuoteInformation information);
        public event QuoteUpdateDelegate QuoteUpdateEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MBTradingQuote(BackgroundMessageLoopOperator messageLoopOperator)
        {// Calls to the COM must be done in the requestMessage loop operator.
            _messageLoopOperator = messageLoopOperator;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_quotesClient != null)
                {
                    _quotesClient.UnadviseAll(this);
                    _quotesClient = null;
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError(ex.Message);
            }

            _messageLoopOperator = null;
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool Initialize(MBTradingConnectionManager manager, MbtQuotes quotesClient)
        {
            SystemMonitor.CheckError(_messageLoopOperator.InvokeRequred == false, "Init must better be called on message loop method.");

            _manager = manager;
            if (_quotesClient != null)
            {
                return false;
            }

            _quotesClient = quotesClient;
            return true;
        }

        /// <summary>
        /// Call is non confirmative;
        /// Forwarded to requestMessage looped thread.
        /// </summary>
        /// <param name="baseCurrency"></param>
        public bool SubscribeSymbolSession(Symbol symbol, Commission? commission)
        {
            if (IsInitialized == false)
            {
                return false;
            }

            lock (this)
            {
                if (_sessionsHotSwap.ContainsKey(symbol.Name))
                {// BaseCurrency already subscribed.
                    return true;
                }


                Dictionary<string, SessionQuoteInformation> sessionsHotSwap = new Dictionary<string, SessionQuoteInformation>(_sessionsHotSwap);
                sessionsHotSwap.Add(symbol.Name, new SessionQuoteInformation() { Symbol = symbol, Commission = commission });
                
                _sessionsHotSwap = sessionsHotSwap;

            }
            
            MbtQuotes client = _quotesClient;
            if (client != null)
            {
                _messageLoopOperator.BeginInvoke(delegate()
                {
                    client.AdviseSymbol(this, symbol.Name, (int)enumQuoteServiceFlags.qsfLevelOne);
                });
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UnSubscribeSymbolSession(string symbol)
        {
            if (IsInitialized == false)
            {
                return false;
            }

            lock (this)
            {
                if (_sessionsHotSwap.Remove(symbol) == false)
                {
                    return false;
                }
            }

            bool result = true;
            _messageLoopOperator.Invoke(delegate()
            {
                MbtQuotes client = _quotesClient;
                if (client == null)
                {
                    result = false;
                    return;
                }

                client.UnadviseSymbol(this, symbol, (int)enumQuoteServiceFlags.qsfLevelOne);
            });

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public SessionQuoteInformation GetSymbolSessionInformation(string symbol)
        {
            if (IsInitialized == false)
            {
                return null;
            }

            lock(this)
            {
                if (_sessionsHotSwap.ContainsKey(symbol))
                {
                    return _sessionsHotSwap[symbol];
                }
            }

            return null;
        }

        public Quote? GetSingleSymbolQuote(string symbolName, TimeSpan timeOut, out string symbolMarket)
        {
            symbolMarket = string.Empty;
            if (IsInitialized == false)
            {
                return null;
            }

            string market = string.Empty;
            Quote? result = null;
            
            _messageLoopOperator.Invoke(delegate()
            {
                MbtQuotes client = _quotesClient;
                if (client == null)
                {
                    return;
                }

                QUOTERECORD record = client.GetSingleQuote(symbolName, (int)timeOut.TotalMilliseconds);
                if (string.IsNullOrEmpty(record.bstrSymbol) == false
                    && record.bstrSymbol.ToUpper() == symbolName.ToUpper())
                {// Match found.
                    Quote quoteResult = new Quote();

					Commission? commission = null;
					Dictionary<string, SessionQuoteInformation> sessionsHotSwap = _sessionsHotSwap;
					if (sessionsHotSwap.ContainsKey(record.bstrSymbol))
					{
						commission = sessionsHotSwap[record.bstrSymbol].Commission;
					}

					ConvertQuoteRecord(ref quoteResult, commission, record);
                    market = record.bstrMarket;
                    result = quoteResult;
                }
                else
                {
                    // Failed to find baseCurrency.
                    SystemMonitor.OperationWarning("Failed to find requested symbol.");
                }

            });

            if (result.HasValue)
            {
                symbolMarket = market;
            }

            return result;
        }

        #region IMbtQuotesNotify Members

        void IMbtQuotesNotify.OnLevel2Data(ref LEVEL2RECORD pRec)
        {
            throw new NotImplementedException();
        }

        void IMbtQuotesNotify.OnOptionsData(ref OPTIONSRECORD pRec)
        {
            throw new NotImplementedException();
        }

        void ConvertQuoteRecord(ref Quote quote, Commission? commission, QUOTERECORD pRec)
        {
			quote.Ask = (0 == pRec.dAsk) ? (decimal?)null : (decimal)pRec.dAsk;
			quote.Bid = (0 == pRec.dBid) ? (decimal?)null : (decimal)pRec.dBid;

            MBTradingConnectionManager manager = _manager;
            if (commission.HasValue && (manager != null && manager.Adapter != null && manager.Adapter.ApplyQuoteCommission))
            {
                commission.Value.ApplyCommissions(manager.Adapter, ref quote);
            }

            quote.High = (0 == pRec.dHigh) ? (decimal?)null : (decimal)pRec.dHigh;
            quote.Low = (0 == pRec.dLow) ? (decimal?)null : (decimal)pRec.dLow;

            quote.Open = (decimal)pRec.dOpen;
            quote.Volume = pRec.lVolume;
            quote.Time = pRec.UTCDateTime;
        }

        void IMbtQuotesNotify.OnQuoteData(ref QUOTERECORD pRec)
        {
            SessionQuoteInformation information;
            Quote quote;
			Commission? commission = null;
            lock (this)
            {
                if (_sessionsHotSwap.ContainsKey(pRec.bstrSymbol) == false)
                {
                    SystemMonitor.OperationWarning("Quote received for session not found.");
                    return;
                }

                information = _sessionsHotSwap[pRec.bstrSymbol];
                if (information.Quote.HasValue == false)
                {
                    information.Quote = new Quote();
                }

                quote = information.Quote.Value;
				commission = information.Commission;
            }

			ConvertQuoteRecord(ref quote, commission, pRec);

            lock (this)
            {
                information.Quote = quote;
            }

            if (QuoteUpdateEvent != null)
            {
                QuoteUpdateEvent(this, information);
            }
        }

        void IMbtQuotesNotify.OnTSData(ref TSRECORD pRec)
        {
            throw new NotImplementedException();
        }


		//Commission? LoadMbtCommission(string symbolName)
		//{
		//    string folder = GeneralHelper.ExecutingDirectory + "\\..\\Files\\";
		//    string filePath = folder + "MBTCommissions.xml";

		//    try 
		//    {
		//        System.Xml.XmlDocument document = new System.Xml.XmlDocument();
		//        document.Load(filePath);
		//        XmlNode commissionsNode = document.ChildNodes[1];

		//        string name, askOperation, askNodeText, bidOperation, bidNodeText;
		//        foreach (XmlNode node in commissionsNode)
		//        {
		//            name = node.ChildNodes[0].InnerText;
					
		//            if (name == symbolName) 
		//            {
		//                askOperation = node.ChildNodes[1].InnerText;
		//                askNodeText = node.ChildNodes[2].InnerText;
		//                bidOperation = node.ChildNodes[3].InnerText;
		//                bidNodeText = node.ChildNodes[4].InnerText;

		//                Commission.enumOperation askOp = this.GetCommissionOperationCode(askOperation);
		//                Commission.enumOperation bidOp = this.GetCommissionOperationCode(bidOperation);

		//                if (askOp != Commission.enumOperation.opNone && bidOp != Commission.enumOperation.opNone)
		//                {
		//                    return new Commission() {
		//                        askOperation = askOp,
		//                        askValue = Convert.ToDecimal(askNodeText),
		//                        bidOperation = bidOp,
		//                        bidValue = Convert.ToDecimal(bidNodeText)
		//                    };
		//                }
		//            }
		//        }

		//    }
		//    catch (Exception ex)
		//    {
		//        SystemMonitor.OperationError(ex.Message);
		//    }

		//    return null;
		//}

		//protected Commission.enumOperation GetCommissionOperationCode(string operation)
		//{
		//    switch (operation)
		//    {
		//        case "multiply": return Commission.enumOperation.opMult; 
		//        case "add": return Commission.enumOperation.opAdd; 
		//        default: return Commission.enumOperation.opNone; 
		//    }
		//}

        #endregion
    }
}
