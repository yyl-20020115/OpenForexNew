using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

namespace CommonSupport
{
    /// <summary>
    /// Class helps in writing information to files.
    /// </summary>
    [Serializable]
    public class FileWriterHelper : IDisposable/*, IDeserializationCallback*/
    {
        #region Members

        [NonSerialized]
        object _syncRoot = new object();

        [NonSerialized]
        volatile StreamWriter _writer = null;

        [NonSerialized]
        volatile FileStream _stream = null;

        [NonSerialized]
        int _logFileNumber = 0;

        bool _applyDateTimeReplace = true;

        /// <summary>
        /// Should a {DateTime} token be replaced in file names.
        /// </summary>
        public bool ApplyDateTimeReplace
        {
            get { return _applyDateTimeReplace; }
            set { _applyDateTimeReplace = value; }
        }

        [NonSerialized]
        volatile string _actualFilePath = string.Empty;

        volatile string _initialFilePath = string.Empty;

        /// <summary>
        /// File path of file writing to.
        /// </summary>
        public string InitialFilePath
        {
            get { return _initialFilePath; }
        }

        volatile int _maximumFileSize = 1024 * 1024 * 1000;
        /// <summary>
        /// Maximum file size, default 1000 MB.
        /// </summary>
        public int MaximumFileSize
        {
            get { return _maximumFileSize; }
            set { _maximumFileSize = value; }
        }

        /// <summary>
        /// Control the way data is flushed to the file.
        /// </summary>
        public enum FlushPolicyEnum
        {
            FlushEachEntry,
            FlushAutomatic,
            FlushPeriodic
        }

        volatile FlushPolicyEnum _flushPolicy = FlushPolicyEnum.FlushAutomatic;
        /// <summary>
        /// How often to flush the file writer.
        /// </summary>
        public FlushPolicyEnum FlushPolicy
        {
            get { return _flushPolicy; }
            set { _flushPolicy = value; }
        }

        TimeSpan _periodicFlushInterval = TimeSpan.FromSeconds(15);
        /// <summary>
        /// The time interval to use for periodic data flush.
        /// </summary>
        public TimeSpan PeriodicFlushInterval
        {
            get { return _periodicFlushInterval; }
            set { _periodicFlushInterval = value; }
        }
        
        [NonSerialized]
        DateTime _lastFlush = DateTime.Now;

        #endregion

        #region Events

        public delegate void DataWrittenDelegate(FileWriterHelper helper, StreamWriter writer, string data);
        public event DataWrittenDelegate DataWrittenEvent;

        #endregion

        #region Instance Control

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileWriterHelper()
        {
            Construct();
        }
        
        /// <summary>
        /// Use the attribute to make sure Construct runs BEFORE any parent deserialization routine 
        /// (since they use the Callback mechanism, and it is invoked last; otherwise if this class
        /// uses callback as well it gets called AFTER owner callback and this may mess things up).
        /// </summary>
        /// <param name="sender"></param>
        [OnDeserializing]
        public void OnDeserializing(StreamingContext context)
        {
            Construct();
        }

        protected void Construct()
        {
            _syncRoot = new object();

            lock (_syncRoot)
            {
                DataWrittenEvent += new DataWrittenDelegate(FileWriterHelper_DataWrittenEvent);
                _actualFilePath = string.Empty;
                _lastFlush = DateTime.MinValue;
                _writer = null;
                _stream = null;
                _logFileNumber = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Initialize(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                SystemMonitor.OperationError("Failed to initialize File Tracer Item sink with file [" + filePath + "]");
                return false;
            }

            UnInitialize();

            _initialFilePath = filePath;
            filePath = GeneralHelper.ReplaceFileNameCompatibleDateTime(filePath, DateTime.Now);

            lock (_syncRoot)
            {
                try
                {
                    if (Directory.Exists(Path.GetDirectoryName(filePath)) == false)
                    {
                        DirectoryInfo info = Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        if (info == null)
                        {// Failed to create folder.
                            SystemMonitor.OperationError("Failed to create directory of file [" + filePath + "].");
                            return false;
                        }
                    }

                    _actualFilePath = filePath;
                    _stream = new FileStream(_actualFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                    _writer = new StreamWriter(_stream);
                }
                catch (Exception ex)
                {
                    UnInitialize();

                    SystemMonitor.OperationError(ex.Message);
                    return false;
                }
                finally
                {
                    if (_writer == null)
                    {
                        _initialFilePath = string.Empty;
                        _actualFilePath = null;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {
            lock (_syncRoot)
            {// This is required since we might receive an item after/during dispose.
                try
                {
                    if (_writer != null)
                    {
                        _writer.Flush();
                        _writer.Close();
                        _writer.Dispose();
                    }

                    if (_stream != null)
                    {
                        // Writer takes care of the underlying stream.
                        //_stream.Flush();
                        //_stream.Close();
                        _stream.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationError(ex.Message);
                }
                finally
                {
                    _writer = null;
                    _stream = null;
                }

                _initialFilePath = string.Empty;
                _actualFilePath = string.Empty;
            }
        }

        public void Dispose()
        {
            UnInitialize();
        }

        #endregion

        #region Implementation

        void FileWriterHelper_DataWrittenEvent(FileWriterHelper helper, StreamWriter writer, string data)
        {
            lock (_syncRoot)
            {
                if (_flushPolicy == FlushPolicyEnum.FlushEachEntry)
                {
                    writer.Flush();
                }
                else if (_flushPolicy == FlushPolicyEnum.FlushAutomatic)
                {
                    writer.AutoFlush = true;
                }
                else if (_flushPolicy == FlushPolicyEnum.FlushPeriodic)
                {
                    if (DateTime.Now - _lastFlush > _periodicFlushInterval)
                    {
                        _lastFlush = DateTime.Now;
                        writer.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        StreamWriter ObtainStreamWriter()
        {
            StreamWriter writer = _writer;

            if (writer != null && writer.BaseStream.Position > MaximumFileSize)
            {// Maximum file size reached, start new one.
                lock (_syncRoot)
                {
                    Interlocked.Increment(ref _logFileNumber);

                    string filePath = _actualFilePath + _logFileNumber.ToString();
                    Initialize(filePath);
                    writer = _writer;
                }
            }

            return writer;
        }

        #endregion

        #region Public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Write(string data)
        {
            StreamWriter writer = ObtainStreamWriter();
            if (writer == null)
            {
                return false;
            }

            lock (_syncRoot)
            {
                writer.Write(data);
            }

            if (DataWrittenEvent != null)
            {
                DataWrittenEvent(this, writer, data);
            }

            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool WriteLine(string line)
        {
            StreamWriter writer = ObtainStreamWriter();
            if (writer == null)
            {
                return false;
            }

            lock (_syncRoot)
            {
                writer.WriteLine(line);
            }

            if (DataWrittenEvent != null)
            {
                DataWrittenEvent(this, writer, line);
            }

            return true;
        }

        #endregion

    }
}
