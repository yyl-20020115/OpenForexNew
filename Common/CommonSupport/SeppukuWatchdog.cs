using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Threading;

namespace CommonSupport
{
    /// <summary>
    /// The watchdog will follow the lifespan of the application, and when activated:
    /// in case the application starts the closing procedure, but fails to close in a certain
    /// time frame, will kill the application.
    /// </summary>
    public static class SeppukuWatchdog
    {
        static object _syncRoot = new object();
        static DateTime _closeStarted = DateTime.MaxValue;

        static System.Timers.Timer _watchTimer = null;

        static TimeSpan _postCloseTimeOut = TimeSpan.FromSeconds(25);
        
        /// <summary>
        /// What is the timeout to wait for, after starting the close process, before killing ourselves.
        /// </summary>
        public static TimeSpan PostCloseTimeOut
        {
            get { return SeppukuWatchdog._postCloseTimeOut; }
            set { SeppukuWatchdog._postCloseTimeOut = value; }
        }

        /// <summary>
        /// Activate the watchdog, with the current settings.
        /// </summary>
        public static void Activate()
        {
            lock (_syncRoot)
            {
                if (_watchTimer == null)
                {
                    _watchTimer = new System.Timers.Timer(1000);
                    _watchTimer.Elapsed += new ElapsedEventHandler(_watchTimer_Elapsed);
                    _watchTimer.Start();
                }
            }
        }

        static void _watchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (GeneralHelper.ApplicationClosing == false)
            {
                return;
            }

            lock (_syncRoot)
            {
                if (_closeStarted == DateTime.MaxValue)
                {
                    _closeStarted = DateTime.Now;
                }
                else if (DateTime.Now - _closeStarted >= _postCloseTimeOut)
                {// Time to die.
                    _watchTimer.Stop();
                    _watchTimer.Elapsed -= new ElapsedEventHandler(_watchTimer_Elapsed);

                    System.Diagnostics.Trace.TraceWarning(string.Format("Process [{0}] performing suicide due to close timeout."), Process.GetCurrentProcess().ProcessName);
                    
                    Thread.Sleep(100);
                    Process.GetCurrentProcess().Kill();
                }
                
            }
        }
    }
}
