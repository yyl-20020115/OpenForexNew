using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonFinancial
{
    /// <summary>
    /// Base class for all reader/writer implementations.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public abstract class DataReaderWriter<DataType>
    {
        volatile string _filePath = string.Empty;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        volatile string _copyright = string.Empty;
        public string Copyright
        {
            get { return _copyright; }
            set { _copyright = value; }
        }

        volatile string _symbol = string.Empty;
        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        volatile int _decimalDigits = 0;
        public int DecimalDigits
        {
            get { return _decimalDigits; }
            set { _decimalDigits = value; }
        }

        TimeSpan? _period = null;
        /// <summary>
        /// If value is TimeSpan.Zero, means dataDelivery read is non periodical (for ex. tick based)
        /// if null, means it is not established.
        /// </summary>
        public TimeSpan? Period
        {
            get { return _period; }
            set { _period = value; }
        }

        public delegate bool DataPageReadDelegate(DataReaderWriter<DataType> reader, List<DataType> pageData);
        /// <summary>
        /// Occurs when reading has read another "dataDelivery page".
        /// Return false to signify "stop reading".
        /// </summary>
        public event DataPageReadDelegate DataPageReadEvent;

        /// <summary>
        /// 
        /// </summary>
        public DataReaderWriter(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageData"></param>
        /// <returns></returns>
        protected bool RaiseDataPageReadEvent(List<DataType> pageData)
        {
            if (DataPageReadEvent != null)
            {
                return DataPageReadEvent(this, pageData);
            }

            return false;
        }

        /// <summary>
        /// Read dataDelivery by pages, predefine helper.
        /// </summary>
        public virtual bool ReadDataPaged(int rowsPerPage, out int totalRowsRead)
        {
            return ReadPaged(0, int.MaxValue, rowsPerPage, out totalRowsRead);
        }

        /// <summary>
        /// Read dataDelivery by pages (for very big files).
        /// </summary>
        public abstract bool ReadPaged(int startingRow, int rowCount, int rowsPerPage, out int totalRowsRead);

        /// <summary>
        /// Read dataDelivery directly.
        /// </summary>
        public abstract bool Read(int startingRow, int rowCount, out List<DataType> data);

        /// <summary>
        /// In case the implementing child is capable of reading only the last elements, it is best to implement
        /// and override this method.
        /// </summary>
        public virtual bool ReadLast(int rowCount, out List<DataType> data)
        {
            if (Read(0, int.MaxValue, out data))
            {
                if (rowCount < data.Count)
                {
                    data.RemoveRange(0, data.Count - rowCount);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Read all dataDelivery directly.
        /// </summary>
        public virtual List<DataType> Read()
        {
            List<DataType> result;
            Read(0, int.MaxValue, out result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bars"></param>
        /// <returns></returns>
        public virtual bool Write(ReadOnlyCollection<DataType> bars)
        {
            return Write(0, bars.Count, bars);
        }

        /// <summary>
        /// Do writing of a file.
        /// </summary>
        /// <param name="bars"></param>
        /// <returns></returns>
        public abstract bool Write(int startingRow, int rowCount, ReadOnlyCollection<DataType> bars);
    }
}
