using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using CommonSupport;
using System.IO;

namespace ForexPlatform
{
    /// <summary>
    /// Class manages settings for the Platform instance.
    /// </summary>
    public class PlatformSettings
    {
        SettingsBase _settings;

        /// <summary>
        /// Is the platform running in diagnostics mode (show all warning and errors dialogs).
        /// </summary>
        public bool DiagnosticsMode
        {
            get { return SystemMonitor.GlobalDiagnosticsMode; }
            set { SystemMonitor.GlobalDiagnosticsMode = value; }
        }

        /// <summary>
        /// Default MT4 integration address.
        /// </summary>
        public string DefaultMT4IntegrationAddress
        {
            get { lock (this) { return (string)_settings["DefaultMT4IntegrationAddress"]; } }
        }

        /// <summary>
        /// Is the platform running in diagnostics mode (show all warning and errors dialogs).
        /// </summary>
        public bool DeveloperMode
        {
            get { lock (this) { return (bool)_settings["DeveloperMode"]; } }
        }

        public string TraceLogFile
        {
            get { lock (this) { return (string)_settings["TraceLogFile"]; } }
        }

        Uri _platformUri;
        /// <summary>
        /// Main URI of the address the platform runs in server mode. This is currently not used
        /// and reserved for usage to allow parts of the platform to run as separate processes (and PCs).
        /// </summary>
        public Uri PlatformUri
        {
            get { lock (this) { return _platformUri; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DataStoreOnlineSourcesXml
        {
            get { lock (this) { return (string)_settings["DataStoreOnlineSourcesXml"]; } } 
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExternalModulesFolder
        {
            get { lock (this) { return (string)_settings["ExternalModulesFolder"]; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExternalModulesSettingsPath
        {
            get { lock (this) { return (string)_settings["ExternalModulesSettingsPath"]; } }
        }

        /// <summary>
        /// Default RSS feeds addresses.
        /// </summary>
        public string[] DefaultRSSFeeds
        {
            get
            {
                lock (this)
                {
                    return (((string)_settings["DefaultRSSFeeds"]).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }

        /// <summary>
        /// Path to the file containing BATS symbols definitions.
        /// </summary>
        public string BATSSymbolsFilePath
        {
            get
            {
                lock (this)
                {
                    return Path.Combine(GetMappedPath("FilesFolder"), "batsSymbols.xml");
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings"></param>
        public PlatformSettings(SettingsBase settings)
        {
            _settings = settings;
            _platformUri = new Uri((string)settings["PlatformUriAddress"]);
        }

        /// <summary>
        /// Obtain a path from settings, mapped to startup dir to evade current directory dependency.
        /// </summary>
        /// <param name="settingsName"></param>
        /// <returns></returns>
        public string GetMappedPath(string propertyName)
        {
            lock (this)
            {
                return GeneralHelper.MapRelativeFilePathToExecutingDirectory((string)_settings[propertyName]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetString(string fieldName)
        {
            lock (this) 
            {
                return (string)_settings[fieldName];
            }
        }
    }
}
