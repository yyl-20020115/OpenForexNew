using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CommonSupport;

namespace CommonSupport
{
    /// <summary>
    /// Monitors an MT4 instance, using the log file, and notifies when orders are placed on it.
    /// </summary>
    public class TextFileMonitor : IDisposable
    {
        object _syncRoot = new object();

        public string FilePath { get; protected set; }

        FileSystemWatcher _watcher = null;
        FileStream _fileStream = null;
        StreamReader _reader = null;
        long _lastFilePos = 0;

        #region Delegates and Events

        public delegate void DataUpdateDelegate(TextFileMonitor monitor, string data);
        public event DataUpdateDelegate NewLineReadEvent;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextFileMonitor()
        {
            _watcher = new FileSystemWatcher();
            _watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
            _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_watcher != null)
                {
                    _watcher.Dispose();
                    _watcher = null;
                }

                if (_reader != null)
                {
                    // Do not dispose, since it will dump the stream as well.
                    _reader = null;
                }

                if (_fileStream != null)
                {
                    _fileStream.Dispose();
                    _fileStream = null;
                }
            }
        }

        public void ForceCheck()
        {
            ProcessUpdate(WatcherChangeTypes.Changed);
        }

        public bool Initialize(string filePath)
        {
            lock (_syncRoot)
            {
                if (string.IsNullOrEmpty(FilePath) == false)
                {
                    return false;
                }

                FilePath = filePath;
                if (_watcher != null)
                {
                    _watcher.Path = Path.GetDirectoryName(filePath);
                    _watcher.Filter = Path.GetFileName(filePath);
                    _watcher.EnableRaisingEvents = true;
                }


                // Create new FileInfo object and get the Length.
                FileInfo fileInfo = new FileInfo(FilePath);

                _lastFilePos = fileInfo.Length;
            }

            return true;
        }

        void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            ProcessUpdate(e.ChangeType);
        }

        void ProcessUpdate(WatcherChangeTypes changeType)
        {
            lock (_syncRoot)
            {
                // File created, deleted, or first call, setup the reader.
                if (_fileStream == null
                    || changeType == WatcherChangeTypes.Deleted
                    || changeType == WatcherChangeTypes.Created)
                {
                    if (_reader != null)
                    {
                        _reader.Dispose();
                        _reader = null;
                    }

                    if (_fileStream != null)
                    {
                        _fileStream.Dispose();
                        _fileStream = null;
                    }

                    if (changeType == WatcherChangeTypes.Deleted)
                    {
                        return;
                    }

                    try
                    {
                        _fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        _reader = new StreamReader(_fileStream);
                    }
                    catch (Exception ex)
                    {
                        SystemMonitor.Error(ex.Message);

                        _reader = null;
                        _fileStream.Dispose();
                        _fileStream = null;
                    }
                }

                if (_fileStream == null || _fileStream.CanRead == false)
                {
                    return;
                }

                if (_fileStream.Length < _lastFilePos - 10)
                {// File was rewritten start from beggining.
                    _lastFilePos = 0;
                }

                try
                {
                    _fileStream.Seek(_lastFilePos, SeekOrigin.Begin);

                    string line = _reader.ReadLine();
                    while (line != null)
                    {
                        DataUpdateDelegate delegateInstance = NewLineReadEvent;
                        if (delegateInstance != null)
                        {
                            delegateInstance(this, line);
                        }

                        line = _reader.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationError("Failed to read file", ex);
                }

                _lastFilePos = _fileStream.Position;
            }
        }


    }
}
