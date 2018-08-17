using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using CommonSupport;
using Ionic.Zip;

namespace ForexPlatform
{
    /// <summary>
    /// Class describes information for online dataDelivery store entry.
    /// Class can undergo 2 types of serialization, both Binary (stored inside an DataStoreManager)
    /// and XML (stored on a web placed XML file, for common access).
    /// </summary>
    [Serializable]
    [XmlRoot("OnlineEntrySource")]
    public class OnlineEntrySource : IDeserializationCallback, IComparable<OnlineEntrySource>
    {
        volatile string _uri = string.Empty;
        
        /// <summary>
        /// Uri for the file resource.
        /// </summary>
        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        volatile string _source = string.Empty;

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        volatile string _symbolName = string.Empty;

        /// <summary>
        /// BaseCurrency of entry.
        /// </summary>
        public string SymbolName
        {
            get { return _symbolName; }
            set { _symbolName = value; }
        }

        TimeSpan _period = TimeSpan.Zero;
        /// <summary>
        /// Period of entry bars.
        /// </summary>
        public string PeriodString
        {
            get { return _period.ToString(); }
            set { _period = TimeSpan.Parse(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public TimeSpan Period
        {
            get { return _period; }
            set { _period = value; }
        }

        [NonSerialized]
        volatile bool _isDownloading = false;

        /// <summary>
        /// Entry is in download operation.
        /// </summary>
        public bool IsDownloading
        {
            get { return _isDownloading; }
        }

        /// <summary>
        /// Paths to downloaded files (can be more than one).
        /// </summary>
        List<string> _downloadedTempFilesPaths = new List<string>();

        [XmlIgnore]
        public ReadOnlyCollection<string> DownloadedTempFilesPaths
        {
            get { lock (this) { return _downloadedTempFilesPaths.AsReadOnly(); } }
        }
        
        [NonSerialized]
        volatile int _downloadProgressPercentage = 0;
        /// <summary>
        /// How much of the dataDelivery for this entry has been downloaded.
        /// </summary>
        public int DownloadProgressPercentage
        {
            get { return _downloadProgressPercentage; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DownloadSucceeded
        {
            get
            {
                return _downloadProgressPercentage == 100;
            }
        }

        public delegate void EntrySourceUpdateDelegate(OnlineEntrySource entrySource);
        
        [field:NonSerialized]
        public event EntrySourceUpdateDelegate DataDownloadedEvent;

        [field: NonSerialized]
        public event EntrySourceUpdateDelegate DataDownloadUpdateEvent;

        /// <summary>
        /// Parameterless constructor for serialization purposes.
        /// </summary>
        public OnlineEntrySource()
        { 
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public OnlineEntrySource(string uri)
        {
            _uri = uri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        public void OnDeserialization(object sender)
        {
            _isDownloading = false;
            _downloadProgressPercentage = 0;
        }

        /// <summary>
        /// Download dataDelivery.
        /// </summary>
        /// <param name="downloadFolder">Pass string.Empty to download to auto-temporary folder.</param>
        public bool BeginDownload(bool extractDownloadResult, string downloadFolder)
        {
            lock(this)
            {
                if (_isDownloading || _downloadedTempFilesPaths.Count > 0 || _downloadProgressPercentage != 0)
                {
                    SystemMonitor.Warning("Online data entry downloading or data not processed.");
                    return false;
                }

                _isDownloading = true;

                string operationTempFolder = downloadFolder;

                if (string.IsNullOrEmpty(operationTempFolder))
                {
                    operationTempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                }

                if (Directory.Exists(operationTempFolder) == false)
                {
                    Directory.CreateDirectory(operationTempFolder);
                }

                string localDownloadFilePath = Path.Combine(operationTempFolder, Path.GetFileName(_uri));

                WebClient client = new WebClient();
                client.DownloadFileCompleted += delegate(object sender, AsyncCompletedEventArgs e)
                {
                    _downloadProgressPercentage = 100;
                    _downloadedTempFilesPaths.Clear();
                    _isDownloading = false;

                    if (extractDownloadResult && Path.GetExtension(localDownloadFilePath).ToLower() == ".zip")
                    {// Unzip the downloaded file.
                        try
                        {
                            ZipFile file = new ZipFile(localDownloadFilePath);
                            file.ExtractAll(operationTempFolder, true);
                            foreach (string fileName in file.EntryFileNames)
                            {
                                _downloadedTempFilesPaths.Add(Path.Combine(operationTempFolder, fileName));
                            }
                        }
                        catch (Exception ex)
                        {
                            _downloadProgressPercentage = 0;
                            SystemMonitor.OperationError("Failed to extract file from [" + _uri + ", " + ex.Message + "].");
                        }

                    }
                    else
                    {
                        _downloadedTempFilesPaths.Add(localDownloadFilePath);
                    }

                    if (DataDownloadedEvent != null)
                    {
                        DataDownloadedEvent(this);
                    }

                };

                client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
                {
                    _downloadProgressPercentage = e.ProgressPercentage;

                    if (DataDownloadUpdateEvent != null)
                    {
                        DataDownloadUpdateEvent(this);
                    }
                };

                client.DownloadFileAsync(new Uri(_uri), localDownloadFilePath);
            }

            return true;
        }

        /// <summary>
        /// Call this when the actual downloading process has finished.
        /// Clear all information regarding the current download and prepare to start a new one.
        /// </summary>
        /// <returns></returns>
        public bool EndDownload(bool deleteTempFiles)
        {
            if (_isDownloading)
            {
                return false;
            }

            if (deleteTempFiles)
            {
                lock (this)
                {// Clear temporary downloaded files.
                    foreach (string filePath in _downloadedTempFilesPaths)
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            SystemMonitor.OperationError(string.Format("Failed to delete downloaded file [{0}] [{1}].", filePath, ex.Message));
                        }
                    }
                }
            }

            _downloadedTempFilesPaths.Clear();
            _downloadProgressPercentage = 0;

            return true;
        }


        #region IComparable<OnlineEntrySource> Members

        public int CompareTo(OnlineEntrySource other)
        {
            int result = this._source.CompareTo(other._source);
            if (result != 0)
            {
                return result;
            }

            result = this._period.CompareTo(other._period);
            if (result != 0)
            {
                return result;
            }

            result = this._symbolName.CompareTo(other._symbolName);
            if (result != 0)
            {
                return result;
            }

            return this._uri.CompareTo(other.Uri);
        }

        #endregion
    }
}
