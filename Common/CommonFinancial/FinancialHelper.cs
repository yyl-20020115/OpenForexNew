using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.IO;

namespace CommonFinancial
{
    /// <summary>
    /// Class contains general helper functionality for financial classes and operation.
    /// </summary>
    public static class FinancialHelper
    {
        public static string[] CommonTradingPairs = new string[] {
                                                "AUD/CAD",
                                                "AUD/CHF",
                                                "AUD/JPY",
                                                "AUD/DKK",
                                                "AUD/NZD",
                                                "AUD/PLN",
                                                "AUD/SGD",
                                                "AUD/USD",
                                                "CAD/CHF",
                                                "CAD/JPY",
                                                "CHF/CAD",
                                                "CHF/JPY",
                                                "CHF/SGD",
                                                "EUR/AUD",
                                                "EUR/CAD",
                                                "EUR/CHF",
                                                "EUR/CCK",
                                                "EUR/DKK",
                                                "EUR/GBP",
                                                "EUR/HKD",
                                                "EUR/HUF",
                                                "EUR/JPY",
                                                "EUR/LVL",
                                                "EUR/NOK",
                                                "EUR/NZD",
                                                "EUR/PLN",
                                                "EUR/SEK",
                                                "EUR/SGD",
                                                "EUR/SKK",
                                                "EUR/USD",
                                                "EUR/ZAR",
                                                "GBP/AUD",
                                                "GBP/CAD",
                                                "GBP/CHF",
                                                "GBP/DKK",
                                                "GBP/JPY",
                                                "GBP/NOK",
                                                "GBP/NZD",
                                                "GBP/SEK",
                                                "GBP/SGD",
                                                "GBP/USD",
                                                "GBP/ZAR",
                                                "NZD/CAD",
                                                "NZD/CHF",
                                                "NZD/JPY",
                                                "NZD/SGD",
                                                "NZD/USD",
                                                "SGD/JPY",
                                                "USD/CAD",
                                                "USD/CHF",
                                                "USD/CCK",
                                                "USD/DKK",
                                                "USD/HKD",
                                                "USD/HRK",
                                                "USD/HUF",
                                                "USD/JPY",
                                                "USD/LVL",
                                                "USD/LTL",
                                                "USD/MXN",
                                                "USD/NOK",
                                                "USD/PLN",
                                                "USD/SEK",
                                                "USD/SGD",
                                                "USD/SKK",
                                                "USD/ZAR" };

        static public string[] CommonMinuteIntervals = { "1", "2", "5", "10", "15", "30", "60", "120", "240", "480", "1440" };
        static public string[] CommonHourIntervals = { "1", "2", "4", "5", "6", "8", "12", "24", "48", "168" };
        static public string[] CommonDayIntervals = { "1", "2", "5", "7" };

        ///// <summary>
        ///// Helper method, allows to split a combined currency pair in 2.
        ///// </summary>
        ///// <param name="currencyPair"></param>
        ///// <param name="currency1"></param>
        ///// <param name="currency2"></param>
        //static void GetPairParts(string currencyPair, out string currency1, out string currency2)
        //{
        //    Symbol.
        //    int slashIndex = currencyPair.IndexOf("/");
        //    currency1 = currencyPair.Substring(0, slashIndex);
        //    currency2 = currencyPair.Substring(slashIndex + 1, currencyPair.Length - slashIndex - 1);
        //}

        /// <summary>
        /// Establish the Forex pair baseCurrency name and (if possible) period of quotes based on file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public bool EstablishForexPairAndPeriod(string complexSymbolName, out string symbolName)
        {
            //symbolName = "";
            //foreach (char c in complexSymbolName)
            //{
            //    if (char.IsLetter(c) || char.IsNumber(c))
            //    {
            //        symbolName += c;
            //    }
            //}

            symbolName = complexSymbolName;

            //// Try to establish time interval based in file name.
            //period = TimeSpan.Zero;
            //foreach (string dayInterval in CommonDayIntervals)
            //{
            //    if (complexSymbolName.Contains("D" + dayInterval))
            //    {
            //        period = TimeSpan.FromDays(int.Parse(dayInterval));
            //        break;
            //    }
            //}

            //if (period == TimeSpan.Zero)
            //{
            //    foreach (string hourInterval in CommonHourIntervals)
            //    {
            //        if (complexSymbolName.Contains("H" + hourInterval))
            //        {
            //            period = TimeSpan.FromHours(int.Parse(hourInterval));
            //            break;
            //        }
            //    }
            //}

            //if (period == TimeSpan.Zero)
            //{
            //    foreach (string minuteInterval in CommonMinuteIntervals)
            //    {
            //        if (complexSymbolName.Contains("M" + minuteInterval) 
            //            /*|| complexSymbolName.Contains(minuteInterval)*/)
            //        {
            //            period = TimeSpan.FromMinutes(int.Parse(minuteInterval));
            //            break;
            //        }
            //    }
            //}

            foreach (string pairName in CommonTradingPairs)
            {
                Symbol? symbol = Symbol.CreateForexPairSymbol(pairName, '/');

                if (complexSymbolName.Contains(symbol.Value.ForexCurrency1) && complexSymbolName.Contains(symbol.Value.ForexCurrency2))
                {
                    symbolName = pairName;
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Will create the corresponding, best fitting
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public DataReaderWriter<DataBar> CreateDataBarFileReaderWriter(string fileName)
        {
            if (File.Exists(fileName) == false)
            {
                SystemMonitor.OperationWarning("File to read from, does not exist.");
                return null;
            }

            if (Path.GetExtension(fileName).ToLower() == ".hst")
            {
                return new HSTDataBarReaderWriter(fileName);
            }
            else if (Path.GetExtension(fileName).ToLower() == ".csv")
            {
                return new CSVDataBarReaderWriter(fileName, CSVDataBarReaderWriter.DataFormat.CSVHistoricalFileDefault);
            }

            SystemMonitor.OperationWarning("File reader for this file not found.");
            return null;
        }


        /// <summary>
        /// Get the after decimal point digits for those bars.
        /// </summary>
        /// <param name="barDatas"></param>
        /// <returns></returns>
        static public int EstablishDecimalDigits(IEnumerable<DataBar> dataBars)
        {
            int result = 0;
            foreach (DataBar data in dataBars)
            {
                string value = data.Close.ToString(GeneralHelper.UniversalNumberFormatInfo).Replace(".", ",");
                if (value.IndexOf(",") >= 0)
                {
                    result = Math.Max(result, value.Length - 1 - value.IndexOf(","));
                }
            }

            return result;
        }

        /// <summary>
        /// Establish the most often met period between those dataDelivery bars. Do not use on Tick based dataDelivery.
        /// </summary>
        /// <param name="dataBars"></param>
        /// <returns></returns>
        static public TimeSpan GetPrevalentPeriod(List<DataBar> dataBars)
        {
            TimeSpan prevalentPeriod = TimeSpan.Zero;

            // Find out what is the most often occuring period - use it (for ex. consider weekends, holidays).
            SortedDictionary<double, int> periodOccurences = new SortedDictionary<double, int>();

            for (int j = 1; j < dataBars.Count; j++)
            {
                TimeSpan period = dataBars[j].DateTime - dataBars[j - 1].DateTime;
                if (periodOccurences.ContainsKey(period.TotalMinutes) == false)
                {
                    periodOccurences.Add(period.TotalMinutes, 1);
                }
                else
                {
                    periodOccurences[period.TotalMinutes]++;
                }
            }

            // Now find the one with the most occurences.
            int totalOccurences = 0;
            foreach (double periodMinutes in periodOccurences.Keys)
            {// Keep going looking for the period with most occurences.
                if (periodOccurences[periodMinutes] > totalOccurences)
                {
                    prevalentPeriod = TimeSpan.FromMinutes(periodMinutes);
                    totalOccurences = periodOccurences[periodMinutes];
                }
            }

            return prevalentPeriod;
        }

        /// <summary>
        /// Helper, calculates average price, total volume.
        /// </summary>
        public static bool CalculateAveragePrice(List<KeyValuePair<double, decimal>> volumesPrices, out decimal averagePrice, out double totalVolume)
        {
            averagePrice = 0;
            totalVolume = 0;
            foreach (KeyValuePair<double, decimal> pair in volumesPrices)
            {
                totalVolume += pair.Key;
                averagePrice += (decimal)pair.Key * pair.Value;
            }
            
            if (totalVolume > 0)
            {
                averagePrice = averagePrice / (decimal)totalVolume;
            }

            return totalVolume > 0;
        }

    }
}
