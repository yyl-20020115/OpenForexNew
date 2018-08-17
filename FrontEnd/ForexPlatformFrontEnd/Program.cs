using System;
using System.Reflection;
using System.Windows.Forms;
using CommonSupport;
using ForexPlatformFrontEnd.Properties;
using System.Globalization;
using System.Threading;

namespace ForexPlatformFrontEnd
{
  static class Program
  {

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      // Assigning our custom Culture to the application.
      Application.CurrentCulture = GeneralHelper.DefaultCultureInfo;
      GeneralHelper.AssignThreadCulture();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
      Application.ThreadExit += new EventHandler(Application_ThreadExit);

      if (args.Length == 0)
      {
        MessageBox.Show("Use ForexPlatformClient.exe to run the platform.", "Application Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }

      // The application starts in global diagnostics mode by default. When the platform initializes, it restores it setting on that.
      // No major warnings/errors are expected in normal operation before the initialization of the platform.
      SystemMonitor.GlobalDiagnosticsMode = false;

      PerformanceCounterHelper.CountersAllowed = Settings.Default.AllowPerformanceCounters;

      try
      {
        if (args[0].ToLower() == "ManagedLaunch".ToLower())
        {// Default managed starting procedure.
          try
          {
            // Single instance mode check.
            bool createdNew;
            GeneralHelper.CreateCheckApplicationMutex(Application.ProductName, out createdNew);

            if (createdNew == false)
            {
              if (Settings.Default.SingleInstanceMode)
              {
                MessageBox.Show("Application already running and single instance mode set (config file).", Application.ProductName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
              }
              else
              {
                if (MessageBox.Show("Another instance of the application is already running, do you wish to continue?", Application.ProductName + " Note", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                  return;
                }
              }
            }

            // Log file.
            string logFile = Settings.Default.TraceLogFile;

            if (string.IsNullOrEmpty(logFile) == false)
            {
              TracerHelper.Tracer.Add(new FileTracerItemSink(TracerHelper.Tracer,
                   GeneralHelper.MapRelativeFilePathToExecutingDirectory(logFile)));
            }

            if (createdNew == false)
            {
              TracerHelper.Trace("Running as second (multiple) instance.");
            }

            Form mainForm = new OpenForexPlatformBeta();
            Application.Run(mainForm);
          }
          catch (Exception ex)
          {
            SystemMonitor.Error(ex.GetType().Name + "; " + ex.Message);
          }
          finally
          {
            GeneralHelper.DestroyApplicationMutex();
          }
        }
        else if (args[0].ToLower() == "experthost")
        {// Start as an expert host.
          string uri =Properties.Resources.DefaultURI;

          string expertName = "Expert";

          Type expertType = null;

          if (args.Length > 1)
          {
            uri = args[1];
          }
          if (args.Length > 2)
          {
            try
            {
              expertType = Type.ReflectionOnlyGetType(args[2], true, true);

            }
            catch 
            {
            }
          }

          if (args.Length > 3)
          {
            expertName = args[3];
          }

          RemoteExpertHostForm hostForm = new RemoteExpertHostForm(uri, expertType, expertName);

          Application.Run(hostForm);
        }
        else
        {
          MessageBox.Show("Starting parameters not recognized. Process will not start.", "Error in starting procedure.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
      finally
      {
        GeneralHelper.SetApplicationClosing();
      }
    }

    static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      //string message = e.Exception.Message;
      //if (e.Exception.InnerException != null && string.IsNullOrEmpty(e.Exception.InnerException.Message))
      //{
      //    message += ", inner [" + e.Exception.InnerException.Message + "]";
      //}

      SystemMonitor.Error("Application exception ", e.Exception);
    }

    static void Application_ThreadExit(object sender, EventArgs e)
    {

    }
  }
}