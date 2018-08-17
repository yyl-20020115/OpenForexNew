using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Each entry is represented by one locally stored file, that contains its dataDelivery.
    /// </summary>
    [DBPersistence(true)]
    public sealed class DataStoreEntry : DBPersistent
    {
        public enum EntryDataTypeEnum
        {
            DataBar,
            DataTick
        }

        volatile EntryDataTypeEnum _dataType;
        /// <summary>
        /// The type of the dataDelivery in this entry.
        /// </summary>
        public EntryDataTypeEnum DataType
        {
            get { return _dataType; }
        }

        Symbol _symbol;
        [DBPersistence(DBPersistenceAttribute.PersistenceTypeEnum.Binary)]
        public Symbol Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        Guid _guid = Guid.Empty;
        /// <summary>
        /// 
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        volatile string _description = string.Empty;
        /// <summary>
        /// Description of this entry, any special information related to it.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        TimeSpan? _period = null;
        /// <summary>
        /// Time frame (period). Value 0, means entry is tick based.
        /// Value null means not established.
        /// </summary>
        public TimeSpan? Period
        {
            get { lock (this) { return _period; } }
            set { lock (this) { _period = value; } }
        }

        volatile string _copyright = string.Empty;
        /// <summary>
        /// Data copyright information.
        /// </summary>
        public string Copyright
        {
            get { return _copyright; }
            set { _copyright = value; }
        }

        volatile string _fileName = string.Empty;
        /// <summary>
        /// Local storage file name for this entry.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        volatile int _quoteCount = 0;
        /// <summary>
        /// Count of quotes in this entry.
        /// </summary>
        public int QuoteCount
        {
            get { return _quoteCount; }
            set { _quoteCount = value; }
        }

        volatile int _decimalDigits = 0;
        /// <summary>
        /// Decimal values digits of stored dataDelivery.
        /// </summary>
        public int DecimalDigits
        {
            get { return _decimalDigits; }
            set { _decimalDigits = value; }
        }

        DateTime _startTime = DateTime.MinValue;
        public DateTime StartTime
        {
            get { lock (this) { return _startTime; } }
            set { lock (this) { _startTime = value; } }
        }

        DateTime _endTime = DateTime.MinValue;
        public DateTime EndTime
        {
            get { lock (this) { return _endTime; } }
            set { lock (this) { _endTime = value; } }
        }

        public string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    return string.Empty;
                }

                return Path.Combine(_dataStoreFolder, _fileName);
            }
        }

        volatile string _dataStoreFolder = string.Empty;

        public delegate void EntryUpdateDelegate(DataStoreEntry entry);
        public event EntryUpdateDelegate EntryUpdatedEvent;

        /// <summary>
        /// Constructor used by automated DB persistence mechanism.
        /// </summary>
        /// <param name="dataType"></param>
        public DataStoreEntry()
        {
            _guid = Guid.NewGuid();
        }

        /// <summary>
        /// 
        /// </summary>
        public DataStoreEntry(EntryDataTypeEnum dataType)
        {
            _dataType = dataType;
            _guid = Guid.NewGuid();
        }

        public bool Initialize(string dataStoreFolder)
        {
            _dataStoreFolder = dataStoreFolder;
            return true;
        }

        /// <summary>
        /// Delete any local storage dataDelivery for this entry.
        /// </summary>
        public void ClearData()
        {
            try
            {
                if (string.IsNullOrEmpty(_fileName) == false
                    && File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    _fileName = null;
                }
            }
            catch (Exception ex)
            {
                _fileName = null;
                SystemMonitor.OperationError(ex.Message);
            }
        }

        /// <summary>
        /// Loads the data entry from a file.
        /// </summary>
        /// <param name="FileNameGuidSeparator">Separator symbol separating the Guid part from the symbol part. Pass null/string.Empty if none.</param>
        public bool LoadFromFile(string filePath, string fileNameGuidSeparator)
        {
            _fileName = Path.GetFileName(filePath);

            _period = null;
            _copyright = string.Empty;

            string symbolName = Path.GetFileNameWithoutExtension(filePath);
            if (string.IsNullOrEmpty(fileNameGuidSeparator) == false)
            {
                symbolName = symbolName.Substring(0, symbolName.IndexOf(fileNameGuidSeparator));
            }

            if (FinancialHelper.EstablishForexPairAndPeriod(symbolName, out symbolName) == false)
            {
                SystemMonitor.OperationWarning(string.Format("Failed to establish symbol information for file [{0}].", filePath));
            }

            if (DataType == EntryDataTypeEnum.DataBar)
            {
                DataReaderWriter<DataBar> readerWriter = GetDataBarReaderWriter();
                if (readerWriter == null)
                {
                    SystemMonitor.OperationWarning(string.Format("Failed to create reader/writer for file [{0}].", filePath));
                    _fileName = null;
                    return false;
                }

                _decimalDigits = -1;

                readerWriter.DataPageReadEvent += delegate(DataReaderWriter<DataBar> reader, List<DataBar> pageData)
                {
                    if (_period.HasValue == false)
                    {
                        _period = FinancialHelper.GetPrevalentPeriod(pageData);
                    }

                    if (pageData.Count > 0 && (pageData[0].DateTime < _startTime || _startTime == DateTime.MinValue))
                    {
                        _startTime = pageData[0].DateTime;
                    }

                    if (pageData.Count > 0 && (pageData[pageData.Count - 1].DateTime > _endTime || _endTime == DateTime.MinValue))
                    {
                        _endTime = pageData[pageData.Count - 1].DateTime;
                    }

                    if (_decimalDigits < 0)
                    {
                        _decimalDigits = FinancialHelper.EstablishDecimalDigits(pageData);
                    }

                    return true;
                };

                int totalRowsRead = 0;
                if (readerWriter.ReadDataPaged(int.MaxValue, out totalRowsRead) == false)
                {
                    _quoteCount = 0;
                    _symbol = Symbol.Empty;
                    return false;
                }

                _quoteCount = totalRowsRead;

                if (readerWriter is HSTDataBarReaderWriter)
                {// If this is an Hst, read the additional information from it.
                    _copyright = ((HSTDataBarReaderWriter)readerWriter).Copyright;
                    symbolName = ((HSTDataBarReaderWriter)readerWriter).Symbol;
                    _decimalDigits = ((HSTDataBarReaderWriter)readerWriter).DecimalDigits;
                    _period = ((HSTDataBarReaderWriter)readerWriter).Period;
                }

                _symbol = new Symbol(string.Empty, symbolName, FileName.Replace(DataStore.FileNameGuidSeparator, " "));

            }
            else
            {
                SystemMonitor.NotImplementedCritical("Mode not supported.");
            }

            return true;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataDelivery"></param>
        public bool AddData(ReadOnlyCollection<DataBar> data)
        {
            if (DataType != EntryDataTypeEnum.DataBar)
            {// Wrong dataDelivery type for entry.
                return false;
            }

            if (_quoteCount != 0)
            {// Entry already has dataDelivery, TODO: append dataDelivery mode.
                return false;
            }

            DataReaderWriter<DataBar> readerWriter = GetDataBarReaderWriter();
            
            bool result = readerWriter.Write(data);

            if (result && EntryUpdatedEvent != null)
            {
                EntryUpdatedEvent(this);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataDelivery"></param>
        /// <returns></returns>
        public bool AddData(ReadOnlyCollection<DataTick> data)
        {
            if (DataType != EntryDataTypeEnum.DataTick)
            {// Wrong dataDelivery type for entry.
                return false;
            }

            if (_quoteCount != 0)
            {// Entry already has dataDelivery, TODO: append dataDelivery mode.
                return false;
            }

            DataReaderWriter<DataTick> readerWriter = GetDataTickReaderWriter();

            bool result = readerWriter.Write(data);

            if (result && EntryUpdatedEvent != null)
            {
                EntryUpdatedEvent(this);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataReaderWriter<DataTick> GetDataTickReaderWriter()
        {
            if (DataType != EntryDataTypeEnum.DataTick)
            {
                return null;
            }

            SystemMonitor.NotImplementedCritical("Data tick mode not supported.");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataReaderWriter<DataBar> GetDataBarReaderWriter()
        {
            if (DataType != EntryDataTypeEnum.DataBar)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_fileName) == false)
            {
                return FinancialHelper.CreateDataBarFileReaderWriter(FilePath);
            }
            else
            {
                return null;
            }
        }



    }
}
