//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using CommonSupport;
//using Arbiter;
//using System.Reflection;

//namespace ForexPlatform
//{
//    /// <summary>
//    /// ExpertHost (link mode) - It can provide a link to a remote separate process space host.
//    /// </summary>
//    /// 
//    [UserFriendlyName("Remote Expert")]
//    public class RemoteExpertHostProxy : PlatformComponent, /*IExpertHost,*/ IDisposable
//    {
//        Process _remoteHostProcess;

//        //string _expertName;
//        //public string ExpertName
//        //{
//        //    get { return _expertName; }
//        //}

//        public IntPtr ProcessMainWindowHandle
//        {
//            get { lock (this) { return _remoteHostProcess.MainWindowHandle; } }
//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        public RemoteExpertHostProxy(Type expertType, Uri uri)
//            : base(false)
//        {// Run expert host as new process

//            //_expertName = expertName;

//            _remoteHostProcess = new Process();
//            _remoteHostProcess.StartInfo.Arguments = "\"" + uri.ToString() + "\"" + " " + expertType.ToString() + " " + "expertName";
//            _remoteHostProcess.StartInfo.FilePath = Assembly.GetEntryAssembly().Location;
//            _remoteHostProcess.Exited += new EventHandler(process_Exited);

//            if (_remoteHostProcess.Start() == false)
//            {
//                _remoteHostProcess = null;
//                SystemMonitor.Error("Failed to start host process.");
//            }
//        }

//        public void Dispose()
//        {
//            lock (this)
//            {
//                if (_remoteHostProcess != null && _remoteHostProcess.HasExited == false)
//                {
//                    _remoteHostProcess.Kill();
//                }
//            }
//        }

//        void process_Exited(object sender, EventArgs e)
//        {// A process has exited.
//            //if (HostUnInitializedEvent != null)
//            //{
//            //    HostUnInitializedEvent(this);
//            //}

//            lock (this)
//            {
//                _remoteHostProcess = null;
//            }
//        }

//        protected override bool OnUnInitialize()
//        {
//            bool operationResult = base.OnUnInitialize();

//            lock (this)
//            {
//                _remoteHostProcess.CloseMainWindow();
//                _remoteHostProcess.Close();
//            }

//            return operationResult;
//        }

//    }
//}
