using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using CommonSupport;
using CommonFinancial.Webservicex.CurrencyConvertor;

namespace CommonFinancial
{
    /// <summary>
    /// Class manages universal information of converting one (currency) symbol to another.
    /// It uses a web service to deliver rates for values that are otherwise not found.
    /// Also implements a singleton model, not mandatory, but the globlal object is always created.
    /// </summary>
    public class CurrencyConversionManager : IDisposable
    {
        /// <summary>
        /// Class used for singleton implementation.
        /// </summary>
        internal static class Builder
        {
            public static CurrencyConversionManager _instance = new CurrencyConversionManager();
            /// <summary>
            /// 
            /// </summary>
            static Builder()
            {
            }
        }

        /// <summary>
        /// Use for singleton usage.
        /// </summary>
        public static CurrencyConversionManager Instance
        {
            get
            {
                return Builder._instance;
            }
        }

        /// <summary>
        /// A single entry of how one currency converts to another.
        /// </summary>
        public class ConversionEntry
        {
            string _symbol1;
            public string Symbol1
            {
                get { return _symbol1; }
            }

            string _symbol2;
            public string Symbol2
            {
                get { return _symbol2; }
            }

            double? _value;
            public double? Value
            {
                get { return _value; }
            }

            DateTime _lastUpdate = DateTime.MinValue;
            public DateTime LastUpdate
            {
              get { return _lastUpdate; }
            }

            ManualResetEvent _updateEvent = new ManualResetEvent(false);
            public ManualResetEvent UpdateEvent
            {
                get { return _updateEvent; }
            }

            /// <summary>
            /// 
            /// </summary>
            public ConversionEntry(string symbol1, string symbol2)
            {
                _symbol1 = symbol1;
                _symbol2 = symbol2;
            }

            public void Update(double value)
            {
                _lastUpdate = DateTime.Now;
                _value = value;
                _updateEvent.Set();
            }

            public bool IsTooOld(TimeSpan ageAllowed)
            {
                return (DateTime.Now - _lastUpdate) > ageAllowed;
            }
        }

        Dictionary<string, Dictionary<string, ConversionEntry>> _entries = new Dictionary<string, Dictionary<string, ConversionEntry>>();

        TimeSpan _defaultEntryAgeAllowed = TimeSpan.FromMinutes(20);
        /// <summary>
        /// How long is an entry valid.
        /// </summary>
        public TimeSpan DefaultEntryAgeAllowed
        {
            get { return _defaultEntryAgeAllowed; }
            set { _defaultEntryAgeAllowed = value; }
        }

        CurrencyConvertor _currencyService = new CurrencyConvertor();
        Dictionary<string, Currency> _serviceCurrencies = new Dictionary<string, Currency>();

        public delegate void EntryUpdateDelegate(CurrencyConversionManager manager, ConversionEntry entry);
        /// <summary>
        /// Event raised on the Main UI thread of the application, due to WebService implementation (COM?!).
        /// </summary>
        public event EntryUpdateDelegate EntryUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public CurrencyConversionManager()
        {
            string[] names = Enum.GetNames(typeof(Currency));
            Array values = Enum.GetValues(typeof(Currency));

            for (int i = 0; i < names.Length; i++)
            {
                _serviceCurrencies.Add(names[i], (Currency)values.GetValue(i));
            }

            _currencyService.ConversionRateCompleted += new ConversionRateCompletedEventHandler(_currencyService_ConversionRateCompleted);
        }

        /// <summary>
        /// 
        /// </summary>
        void BeginServiceEntryUpdate(ConversionEntry entry)
        {
            Currency currency1, currency2;

            lock(this)
            {
                if (_serviceCurrencies.ContainsKey(entry.Symbol1) == false ||
                    _serviceCurrencies.ContainsKey(entry.Symbol2) == false)
                {
                    SystemMonitor.Warning("Symbol not supported.");
                    return;
                }

                entry.UpdateEvent.Reset();

                currency1 = _serviceCurrencies[entry.Symbol1];
                currency2 = _serviceCurrencies[entry.Symbol2];

                // Begin operation.
                _currencyService.ConversionRateAsync(currency1, currency2, new Currency[] { currency1, currency2 });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void _currencyService_ConversionRateCompleted(object sender, ConversionRateCompletedEventArgs e)
        {
            Currency[] state = (Currency[])e.UserState;
            double value = e.Result;

            if (value == 0)
            {
                SystemMonitor.Warning("Value 0 not accepted from service.");
                return;
            }

            ConversionEntry entry = null;
            lock (this)
            {
                if (_entries.ContainsKey(state[0].ToString())
                    && _entries[state[0].ToString()].ContainsKey(state[1].ToString()))
                {
                    entry = _entries[state[0].ToString()][state[1].ToString()];
                    entry.Update(value);
                }
            }

            if (EntryUpdateEvent != null && entry != null)
            {
                EntryUpdateEvent(this, entry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ConversionEntry CreateEntry(string symbol1, string symbol2, double? value)
        {
            ConversionEntry entry = new ConversionEntry(symbol1, symbol2);
            if (value.HasValue)
            {
                entry.Update(value.Value);
            }

            lock (this)
            {
                if (_entries.ContainsKey(symbol1) == false)
                {
                    _entries.Add(symbol1, new Dictionary<string,ConversionEntry>());
                }

                if (_entries[symbol1].ContainsKey(symbol2))
                {
                    SystemMonitor.Warning("Entry already exists.");
                    return _entries[symbol1][symbol2];
                }

                _entries[symbol1].Add(symbol2, entry);
            }

            return entry;
        }

        #region Public

        /// <summary>
        /// Make sure not to sleep on the UI thread, since the receival event from the web service needs it.
        /// </summary>
        public double? GetRate(string symbol1, string symbol2, TimeSpan timeOut, bool obtainForFutureReference)
        {
            if (Application.OpenForms.Count > 0)
            {
                if (Application.OpenForms[0].InvokeRequired == false && timeOut > TimeSpan.Zero)
                {
                    SystemMonitor.Error("When calling from main UI thread time out must be zero, as otherwise it locks the expected web service update event.");
                    return null;
                }
            }

            if (string.IsNullOrEmpty(symbol1) || string.IsNullOrEmpty(symbol2))
            {
                SystemMonitor.Warning("Invalid symbol name.");
                return null;
            }

            symbol1 = symbol1.ToUpper();
            symbol2 = symbol2.ToUpper();

            if (symbol1 == symbol2)
            {
                return 1;
            }

            lock (this)
            {
                double? value = GetEntryUpdatedValue(symbol1, symbol2, timeOut);

                if (value.HasValue)
                {
                    return value;
                }

                value = GetEntryUpdatedValue(symbol2, symbol1, timeOut);
                if (value.HasValue)
                {
                    if (double.IsInfinity(value.Value) || double.IsNaN(value.Value))
                    {
                        SystemMonitor.Warning("Invalid entry value.");
                        return null;
                    }

                    if (value.Value == 0)
                    {
                        return 0;
                    }

                    return 1 / value.Value;
                }
            }

            if (obtainForFutureReference)
            {
                CreateEntry(symbol1, symbol2, null);
                return GetEntryUpdatedValue(symbol1, symbol2, timeOut);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateValue(string symbol1, string symbol2, double value)
        {
            if (string.IsNullOrEmpty(symbol1) || string.IsNullOrEmpty(symbol2))
            {
                SystemMonitor.Warning("Invalid symbol name.");
                return;
            }

            if (value == 0)
            {
                SystemMonitor.Warning("Value 0 not accepted.");
                return;
            }

            symbol1 = symbol1.ToUpper();
            symbol2 = symbol2.ToUpper();

            if (symbol1 == symbol2)
            {
                return;
            }

            ConversionEntry entry = null;
            double updateValue = value;
            lock (this)
            {
                if (_entries.ContainsKey(symbol1) && _entries[symbol1].ContainsKey(symbol2))
                {// Straight.
                    entry = _entries[symbol1][symbol2];
                    updateValue = value;
                }
                else if (_entries.ContainsKey(symbol2) && _entries[symbol2].ContainsKey(symbol1))
                {// Reverse.
                    entry = _entries[symbol2][symbol1];
                    updateValue = 1 / value;
                }
            }

            if (entry != null)
            {
                entry.Update(updateValue);
            }
            else
            {
                entry = CreateEntry(symbol1, symbol2, value);
            }

            if (EntryUpdateEvent != null)
            {
                EntryUpdateEvent(this, entry);
            }

        }

        #endregion

        /// <summary>
        /// Will not create new entry if not found.
        /// </summary>
        double? GetEntryUpdatedValue(string symbol1, string symbol2, TimeSpan timeOut)
        {
            ConversionEntry entry;
            lock (this)
            {
                if (_entries.ContainsKey(symbol1) == false || 
                    _entries[symbol1].ContainsKey(symbol2) == false)
                {
                    return null;
                }

                entry = _entries[symbol1][symbol2];
            }

            if (entry.IsTooOld(_defaultEntryAgeAllowed))
            {// Go for an update of entry value.
                
                // Symbol match not found.
                BeginServiceEntryUpdate(entry);

                bool waitResult;
                if (timeOut == TimeSpan.MaxValue)
                {
                    waitResult = entry.UpdateEvent.WaitOne();
                }
                else
                {
                    waitResult = entry.UpdateEvent.WaitOne(timeOut);
                }

                return entry.Value;
            }

            return entry.Value;
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (_currencyService != null)
            {
                _currencyService.ConversionRateCompleted -= new ConversionRateCompletedEventHandler(_currencyService_ConversionRateCompleted);
                _currencyService.Dispose();
                _currencyService = null;
            }
        }

        #endregion
    }
}
