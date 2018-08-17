using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Trading baseCurrency information structure.
    /// </summary>
    [Serializable]
    public struct Symbol : IComparable<Symbol>
    {
        public const char DefaultSeparatorSymbol = '/';

        /// <summary>
        /// Group here is optional, and there could be symbols of Forex or Stocks
        /// but with different group symbol (like FX etc.)
        /// </summary>
        public enum SymbolGroup
        {
            Forex,
            Futures,
            Stocks
        }

        public enum Currency
        {

            /// <remarks/>
            AFA,

            /// <remarks/>
            ALL,

            /// <remarks/>
            DZD,

            /// <remarks/>
            ARS,

            /// <remarks/>
            AWG,

            /// <remarks/>
            AUD,

            /// <remarks/>
            BSD,

            /// <remarks/>
            BHD,

            /// <remarks/>
            BDT,

            /// <remarks/>
            BBD,

            /// <remarks/>
            BZD,

            /// <remarks/>
            BMD,

            /// <remarks/>
            BTN,

            /// <remarks/>
            BOB,

            /// <remarks/>
            BWP,

            /// <remarks/>
            BRL,

            /// <remarks/>
            GBP,

            /// <remarks/>
            BND,

            /// <remarks/>
            BIF,

            /// <remarks/>
            XOF,

            /// <remarks/>
            XAF,

            /// <remarks/>
            KHR,

            /// <remarks/>
            CAD,

            /// <remarks/>
            CVE,

            /// <remarks/>
            KYD,

            /// <remarks/>
            CLP,

            /// <remarks/>
            CNY,

            /// <remarks/>
            COP,

            /// <remarks/>
            KMF,

            /// <remarks/>
            CRC,

            /// <remarks/>
            HRK,

            /// <remarks/>
            CUP,

            /// <remarks/>
            CYP,

            /// <remarks/>
            CZK,

            /// <remarks/>
            DKK,

            /// <remarks/>
            DJF,

            /// <remarks/>
            DOP,

            /// <remarks/>
            XCD,

            /// <remarks/>
            EGP,

            /// <remarks/>
            SVC,

            /// <remarks/>
            EEK,

            /// <remarks/>
            ETB,

            /// <remarks/>
            EUR,

            /// <remarks/>
            FKP,

            /// <remarks/>
            GMD,

            /// <remarks/>
            GHC,

            /// <remarks/>
            GIP,

            /// <remarks/>
            XAU,

            /// <remarks/>
            GTQ,

            /// <remarks/>
            GNF,

            /// <remarks/>
            GYD,

            /// <remarks/>
            HTG,

            /// <remarks/>
            HNL,

            /// <remarks/>
            HKD,

            /// <remarks/>
            HUF,

            /// <remarks/>
            ISK,

            /// <remarks/>
            INR,

            /// <remarks/>
            IDR,

            /// <remarks/>
            IQD,

            /// <remarks/>
            ILS,

            /// <remarks/>
            JMD,

            /// <remarks/>
            JPY,

            /// <remarks/>
            JOD,

            /// <remarks/>
            KZT,

            /// <remarks/>
            KES,

            /// <remarks/>
            KRW,

            /// <remarks/>
            KWD,

            /// <remarks/>
            LAK,

            /// <remarks/>
            LVL,

            /// <remarks/>
            LBP,

            /// <remarks/>
            LSL,

            /// <remarks/>
            LRD,

            /// <remarks/>
            LYD,

            /// <remarks/>
            LTL,

            /// <remarks/>
            MOP,

            /// <remarks/>
            MKD,

            /// <remarks/>
            MGF,

            /// <remarks/>
            MWK,

            /// <remarks/>
            MYR,

            /// <remarks/>
            MVR,

            /// <remarks/>
            MTL,

            /// <remarks/>
            MRO,

            /// <remarks/>
            MUR,

            /// <remarks/>
            MXN,

            /// <remarks/>
            MDL,

            /// <remarks/>
            MNT,

            /// <remarks/>
            MAD,

            /// <remarks/>
            MZM,

            /// <remarks/>
            MMK,

            /// <remarks/>
            NAD,

            /// <remarks/>
            NPR,

            /// <remarks/>
            ANG,

            /// <remarks/>
            NZD,

            /// <remarks/>
            NIO,

            /// <remarks/>
            NGN,

            /// <remarks/>
            KPW,

            /// <remarks/>
            NOK,

            /// <remarks/>
            OMR,

            /// <remarks/>
            XPF,

            /// <remarks/>
            PKR,

            /// <remarks/>
            XPD,

            /// <remarks/>
            PAB,

            /// <remarks/>
            PGK,

            /// <remarks/>
            PYG,

            /// <remarks/>
            PEN,

            /// <remarks/>
            PHP,

            /// <remarks/>
            XPT,

            /// <remarks/>
            PLN,

            /// <remarks/>
            QAR,

            /// <remarks/>
            ROL,

            /// <remarks/>
            RUB,

            /// <remarks/>
            WST,

            /// <remarks/>
            STD,

            /// <remarks/>
            SAR,

            /// <remarks/>
            SCR,

            /// <remarks/>
            SLL,

            /// <remarks/>
            XAG,

            /// <remarks/>
            SGD,

            /// <remarks/>
            SKK,

            /// <remarks/>
            SIT,

            /// <remarks/>
            SBD,

            /// <remarks/>
            SOS,

            /// <remarks/>
            ZAR,

            /// <remarks/>
            LKR,

            /// <remarks/>
            SHP,

            /// <remarks/>
            SDD,

            /// <remarks/>
            SRG,

            /// <remarks/>
            SZL,

            /// <remarks/>
            SEK,

            /// <remarks/>
            CHF,

            /// <remarks/>
            SYP,

            /// <remarks/>
            TWD,

            /// <remarks/>
            TZS,

            /// <remarks/>
            THB,

            /// <remarks/>
            TOP,

            /// <remarks/>
            TTD,

            /// <remarks/>
            TND,

            /// <remarks/>
            TRL,

            /// <remarks/>
            USD,

            /// <remarks/>
            AED,

            /// <remarks/>
            UGX,

            /// <remarks/>
            UAH,

            /// <remarks/>
            UYU,

            /// <remarks/>
            VUV,

            /// <remarks/>
            VEB,

            /// <remarks/>
            VND,

            /// <remarks/>
            YER,

            /// <remarks/>
            YUM,

            /// <remarks/>
            ZMK,

            /// <remarks/>
            ZWD,

            /// <remarks/>
            TRY,
        }

        volatile string _source;
        /// <summary>
        /// Name of the source providing this symbol, optional and applicable
        /// where multiple sources provide service trough a single provider.
        /// </summary>
        public string Source
        {
            get { return _source; }
        }

        volatile string _group;
        public string Group
        {
            get { return _group; }
        }

        public string Market
        {
            get { return _group; }
        }

        volatile string _name;
        public string Name
        {
            get { return _name; }
        }

        bool _isForexPair;
        public bool IsForexPair
        {
            get { return _isForexPair; }
        }

        string _forexCurrency1;
        /// <summary>
        /// Only available in Forex Pair symbols.
        /// </summary>
        public string ForexCurrency1
        {
            get { return _forexCurrency1; }
        }

        string _forexCurrency2;
        /// <summary>
        /// Only available in Forex Pair symbols.
        /// </summary>
        public string ForexCurrency2
        {
            get { return _forexCurrency2; }
        }

        /// <summary>
        /// The two involved currencies, in case this is a forex pair.
        /// </summary>
        public string[] ForexCurrencies
        {
            get
            {
                return new string[] { ForexCurrency1, ForexCurrency2 };
            }
        }

        /// <summary>
        /// Empty baseCurrency instance.
        /// </summary>
        public static Symbol Empty
        {
            get { return new Symbol(string.Empty, string.Empty); }
        }
    
        #region Operators

        /// <summary>
        /// 
        /// </summary>
        public static bool operator ==(Symbol a, Symbol b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator !=(Symbol a, Symbol b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Symbol == false)
            {
                return false;
            }

            return this.CompareTo((Symbol)obj) == 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(_name) || this == Empty; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseCurrency"></param>
        public Symbol(string symbol)
        {
            _group = string.Empty;
            _source = string.Empty;
            _name = symbol;

            _isForexPair = false;
            _forexCurrency1 = string.Empty;
            _forexCurrency2 = string.Empty;
            
            Construct();
        }

        /// <summary>
        /// 
        /// </summary>
        public Symbol(string group, string symbol, string source)
        {
            _name = symbol;
            _group = group;
            _source = source;

            _isForexPair = false;
            _forexCurrency1 = string.Empty;
            _forexCurrency2 = string.Empty;

            Construct();
        }

        /// <summary>
        /// 
        /// </summary>
        public Symbol(string group, string symbol)
        {
            _name = symbol;
            _group = group;
            _source = string.Empty;

            _isForexPair = false;
            _forexCurrency1 = string.Empty;
            _forexCurrency2 = string.Empty;

            Construct();
        }

        /// <summary>
        /// Use to create forex pairs symbols.
        /// </summary>
        public Symbol(SymbolGroup group, string symbol)
        {
            _name = symbol;
            _group = group.ToString();
            _source = string.Empty;

            _isForexPair = false;
            _forexCurrency1 = string.Empty;
            _forexCurrency2 = string.Empty;

            Construct();
        }

        /// <summary>
        /// Use to create forex pairs symbols.
        /// </summary>
        public Symbol(SymbolGroup group, string currency1, string currency2)
        {
            SystemMonitor.CheckThrow(group == SymbolGroup.Forex);

            _name = currency1 + "/" + currency2;
            _group = group.ToString();
            _source = string.Empty;

            _isForexPair = true;
            _forexCurrency1 = currency1;
            _forexCurrency2 = currency2;

            Construct();
        }

        void Construct()
        {
            if (string.IsNullOrEmpty(_name) == false && string.IsNullOrEmpty(_forexCurrency1) &&
                string.IsNullOrEmpty(_forexCurrency2))
            {
                _isForexPair = SplitForexSymbol(out _forexCurrency1, out _forexCurrency2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetForexPairQuoteCurrency(string symbol)
        {
            return GetForexPairQuoteCurrency(symbol, DefaultSeparatorSymbol);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetForexPairQuoteCurrency(string symbol, char separator)
        {
            string[] splits = symbol.Split(separator);
            if (splits.Length != 2)
            {
                SystemMonitor.OperationError("Failed to parse forex pair symbol [" + symbol + ", separator " + separator + "].");
                return null;
            }

            return splits[1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetForexPairBaseCurrency(string symbol)
        {
            return GetForexPairBaseCurrency(symbol, DefaultSeparatorSymbol);
        }

        /// <summary>
        /// Helper, automates the establishment of the base currency of a forex pair.
        /// (for ex. EUR/USD = USD)
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetForexPairBaseCurrency(string symbol, char separator)
        {
            string[] splits = symbol.Split(separator);
            if (splits.Length != 2)
            {
                SystemMonitor.OperationError("Failed to parse forex pair symbol [" + symbol + ", separator " + separator + "].");
                return null;
            }

            return splits[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Symbol? CreateForexPairSymbol(string symbol)
        {
            return CreateForexPairSymbol(symbol, DefaultSeparatorSymbol);
        }

        /// <summary>
        /// Helper, helps create a Symbol from a forex pair mixed symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Symbol? CreateForexPairSymbol(string symbol, char separator)
        {
            string[] splits = symbol.Split(separator);
            if (splits.Length != 2)
            {
                return null;
            }

            return new Symbol(SymbolGroup.Forex, splits[0], splits[1]);
        }

        /// <summary>
        /// This will try to split the current symbol in 2 currencies for a forex pair.
        /// </summary>
        /// <param name="currency1"></param>
        /// <param name="currency2"></param>
        /// <returns></returns>
        public bool SplitForexSymbol(out string currency1, out string currency2)
        {
            currency1 = string.Empty;
            currency2 = string.Empty;
            if (string.IsNullOrEmpty(this.Name))
            {
                return false;
            }

            string name = this.Name.ToUpper();
            string[] currencyNames = Enum.GetNames(typeof(Currency));
            foreach (string currencyName in currencyNames)
            {
                if (name.StartsWith(currencyName.ToUpper()))
                {// Found 1, try the other.
                    string subName = name.Substring(currencyName.Length);

                    foreach (string currencyName2 in currencyNames)
                    {
                        if (subName.Contains(currencyName2))
                        {// Found 2, we have a forex pair.
                            currency1 = currencyName;
                            currency2 = currencyName2;
                            return true;
                        }
                    }

                    // Failed to find second part.
                    return false;
                }
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool MatchesSearchCriteria(string nameMatch)
        {
            if (nameMatch == "*" || string.IsNullOrEmpty(nameMatch))
            {
                return true;
            }
            return _name.ToLower().Contains(nameMatch.ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Symbol [" + _name + "]";
        }

        #region IComparable<BaseCurrency> Members

        public int CompareTo(Symbol other)
        {
            int compare = GeneralHelper.CompareNullable(_name, other._name);
            if (compare != 0)
            {
                return compare;
            }

            compare = GeneralHelper.CompareNullable(_source, other._source);
            if (compare != 0)
            {
                return compare;
            }

            return compare;

            // NOTE: Having this enabled causes a nasty bug in the DataSourceStub implementation.
            //return GeneralHelper.CompareNullable(_group, other._group);
        }

        #endregion

    }
}
