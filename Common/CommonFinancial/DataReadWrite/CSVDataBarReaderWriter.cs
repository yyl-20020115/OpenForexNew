using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace CommonFinancial
{
    /// <summary>
    /// Data reader for CSV files.
    /// </summary>
    public class CSVDataBarReaderWriter : DataReaderWriter<DataBar>
    {
        /// <summary>
        /// The amount of errors allowed on a parsing session.
        /// </summary>
        static public int MaximumParsingErrors = 100;

        public enum DataFormat
        {
            CSVHistoricalFileDefault,
            CSVYahooFinanceQuotes
        }

        DataFormat _dataFormat;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public CSVDataBarReaderWriter(string filePath, DataFormat format)
            : base(filePath)
        {
            _dataFormat = format;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool ReadPaged(int startingRow, int rowCount, int rowsPerPage, out int rowsRead)
        {
            Symbol = "[" + Path.GetFileName(FilePath) + "]";

            rowsRead = 0;
            bool loadResult = false;

            using (TextReader tr = new StreamReader(FilePath))
            {
                List<DataBar> datas = new List<DataBar>();

                TimeSpan period = TimeSpan.MinValue;
                int pageRowsRead = 0;


                while ((loadResult = LoadCSVFromReader(_dataFormat, tr, startingRow, rowsPerPage, out period, out pageRowsRead, datas))
                    && datas.Count == rowsPerPage && (rowCount <= 0 || rowCount > rowsRead))
                {
                    Period = period;

                    startingRow = 0;
                    rowsRead += pageRowsRead;

                    if (base.RaiseDataPageReadEvent(datas) == false)
                    {// User requested brake.
                        break;
                    }

                    datas.Clear();
                }

                if (datas.Count > 0)
                {// Launch the last package.
                    rowsRead += pageRowsRead;
                    base.RaiseDataPageReadEvent(datas);
                }
            }

            return loadResult;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Read(int startingRow, int rowCount, out List<DataBar> data)
        {
            data = new List<DataBar>();
            using (TextReader tr = new StreamReader(FilePath))
            {
                TimeSpan period;
                int rowsRead = 0;
                bool result = LoadCSVFromReader(_dataFormat, tr, startingRow, rowCount, out period, out rowsRead, data);
                Period = period;
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Name of the file to load from</param>
        /// <param name="startingRow">Row at which the loading starts</param>
        /// <param name="rowsLimit">How many rows to load maximum (pass 0 for no limit).</param>
        /// <param name="prevalentPeriod">The time period between the bars, that is most common.</param>
        /// <param name="datas">Output dataDelivery, pass null to just read and not store the dataDelivery.</param>
        public static bool LoadCSVFromReader(CSVDataBarReaderWriter.DataFormat format, TextReader tr, int startingRow, 
            int rowCount, out TimeSpan prevalentPeriod, out int rowsRead, List<DataBar> dataBars)
        {
            prevalentPeriod = TimeSpan.Zero;

            int parsingErrors = 0;

            rowsRead = 0;
            try
            {
                for (rowsRead = 0; rowsRead < startingRow + rowCount || rowCount == 0; rowsRead++)
                {
                    string lineStr = tr.ReadLine();
                    if (lineStr == null)
                    {
                        break;
                    }

                    if (lineStr.Contains("\""))
                    {// Since there is a \" inside the stream, the comma separation is no longer this simple and straightforward
                        SystemMonitor.OperationWarning("Element found in CSV stream that is not supported by parsing.");
                        continue;
                    }

                    if (rowsRead >= startingRow)
                    {// Parse.
                        DataBar? data = ParseCSVLine(format, lineStr);
                        if (data != null && dataBars != null)
                        {
                            dataBars.Add(data.Value);
                        }
                        else
                        {
                            parsingErrors++;
                        }
                    }

                    if (parsingErrors > MaximumParsingErrors)
                    {// Too many errors and the parsing has been aborted.
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                SystemMonitor.OperationWarning("Failed to parse time in stream, line [" + rowsRead.ToString() + "], message [" + exception.Message + "].");
                if (dataBars != null)
                {
                    dataBars.Clear();
                }

                return false;
            }

            if (dataBars != null)
            {// If dataBars is null this means we ran a simple test run on the data.
                prevalentPeriod = FinancialHelper.GetPrevalentPeriod(dataBars);
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool Write(int startingRow, int rowCount, ReadOnlyCollection<DataBar> bars)
        {
            if (_dataFormat != CommonFinancial.CSVDataBarReaderWriter.DataFormat.CSVHistoricalFileDefault)
            {
                SystemMonitor.NotImplementedWarning();
                return false;
            }

            if (startingRow != 0)
            {
                SystemMonitor.NotImplementedWarning();
                return false;
            }

            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                int count = 0;
                foreach (DataBar bar in bars)
                {
                    if (count > rowCount)
                    {
                        break;
                    }
                    if (bar.HasDataValues)
                    {
                        writer.WriteLine(bar.DateTime.Day.ToString() + "." + bar.DateTime.Month.ToString() + "." + bar.DateTime.Year.ToString()
                            + "," + bar.DateTime.Hour.ToString("00") + ":" + bar.DateTime.Minute.ToString("00")
                            + "," + bar.Open.ToString() + "," + bar.High.ToString()
                            + "," + bar.Low.ToString() + "," + bar.Close.ToString()
                            + "," + bar.Volume.ToString());
                    }

                    count++;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static DateTime ParseCSVDateTime(string date, string time)
        {
            // Must convert to "," since "." is part of the syntax of regular expressions and causes wrong split.
            string[] dateParts = Regex.Split(date.Replace(".", ","), ",");
            string[] timeParts = Regex.Split(time, ":");

            if (dateParts.Length < 3 || timeParts.Length < 2)
            {
                throw new Exception("DateTime format error");
            }

            return new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]),
                int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
        }

        /// <summary>
        /// Helper.
        /// </summary>
        static DataBar? ParseCSVLine(CommonFinancial.CSVDataBarReaderWriter.DataFormat format, string lineStr)
        {
            try
            {
                // Parse 1 line - if the CSV format changes, modify this here as well
                string[] lineSplit = Regex.Split(lineStr, ",");

                if (format == CommonFinancial.CSVDataBarReaderWriter.DataFormat.CSVHistoricalFileDefault)
                {
                    DateTime dateTime = ParseCSVDateTime(lineSplit[0], lineSplit[1]);

                    decimal volume = decimal.Parse(lineSplit[6], new System.Globalization.NumberFormatInfo());
                    return new DataBar(dateTime, decimal.Parse(lineSplit[2], GeneralHelper.UniversalNumberFormatInfo),
                        decimal.Parse(lineSplit[3], GeneralHelper.UniversalNumberFormatInfo),
                        decimal.Parse(lineSplit[4], GeneralHelper.UniversalNumberFormatInfo),
                        decimal.Parse(lineSplit[5], GeneralHelper.UniversalNumberFormatInfo), volume);
                }
                else if (format == CommonFinancial.CSVDataBarReaderWriter.DataFormat.CSVYahooFinanceQuotes)
                {
                    DateTime dateTime = DateTime.Parse(lineSplit[0]);

                    decimal volume = decimal.Parse(lineSplit[5], new System.Globalization.NumberFormatInfo());
                    decimal adjClose = decimal.Parse(lineSplit[6], new System.Globalization.NumberFormatInfo());

                    return new DataBar(dateTime, decimal.Parse(lineSplit[1], GeneralHelper.UniversalNumberFormatInfo), decimal.Parse(lineSplit[2], GeneralHelper.UniversalNumberFormatInfo), decimal.Parse(lineSplit[3], GeneralHelper.UniversalNumberFormatInfo), decimal.Parse(lineSplit[4], GeneralHelper.UniversalNumberFormatInfo), volume);

                }
                else
                {// Unknown format.
                    SystemMonitor.NotImplementedCritical("Unknow format");
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError(ex.Message + "[" + lineStr + "]");
            }

            return null;
        }


    }
}
