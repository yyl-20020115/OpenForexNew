using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace CommonSupport
{
    /// <summary>
    /// Class is a helper for a general application level tracing support.
    /// It uses a conventional Tracer class to store its traced items, but also
    /// can output to system trace.
    /// </summary>
    public static class TracerHelper
    {
        /// <summary>
        /// Default tracer. Assign to make all default static tracing pass trough here.
        /// </summary>
        volatile static Tracer _tracer = null;
        public static Tracer Tracer
        {
            get { return _tracer; }
            set { _tracer = value; }
        }

        /// <summary>
        /// Static constructor.
        /// By default the tracer is created and configured with a tracer item keeper sink.
        /// </summary>
        static TracerHelper()
        {
            _tracer = new Tracer();
            _tracer.Add(new TracerItemKeeperSink(_tracer));

            GeneralHelper.ApplicationClosingEvent += new GeneralHelper.DefaultDelegate(GeneralHelper_ApplicationClosingEvent);
        }

        static void GeneralHelper_ApplicationClosingEvent()
        {
            //if (_tracer != null)
            //{
            //    _tracer.Dispose();
            //    _tracer = null;
            //}
        }

        static List<Type> OwnerTypes = new List<Type>(new Type[] { typeof(TracerHelper), typeof(SystemMonitor) });

        /// <summary>
        /// Perform actual item tracing.
        /// </summary>
        /// <param name="tracer"></param>
        /// <param name="itemType"></param>
        /// <param name="message"></param>
        static public void DoTrace(Tracer tracer, TracerItem.TypeEnum itemType, TracerItem.PriorityEnum priority, string message)
        {
            if (tracer != null && tracer.Enabled)
            {
                string threadId = Thread.CurrentThread.ManagedThreadId.ToString();
                string threadName = Thread.CurrentThread.Name;

                MethodBase method = ReflectionHelper.GetExternalCallingMethod(3, OwnerTypes);

                MethodTracerItem item = new MethodTracerItem(itemType, priority, message, method);
                tracer.Add(item);
            }
        }

        /// <summary>
        /// This is mean to be used with not properly managed code (for ex. Managed C++ wrapper)
        /// that does not allow for the gathering of baseMethod, thread and assembly information
        /// with reflection.
        /// </summary>
        /// <param name="?"></param>
        public static void TraceSimple(TracerItem.TypeEnum itemType, string message)
        {
            Tracer tracer = _tracer;
            if (tracer.Enabled && tracer != null)
            {
                TracerItem item = new TracerItem(itemType, TracerItem.PriorityEnum.Default, message);
                tracer.Add(item);
            }
        }

        public static void TraceEntry(Tracer tracer)
        {
            DoTrace(tracer, TracerItem.TypeEnum.MethodEntry, TracerItem.PriorityEnum.Default, string.Empty);
        }

        public static void TraceEntry(Tracer tracer, string message)
        {
            DoTrace(tracer, TracerItem.TypeEnum.MethodEntry, TracerItem.PriorityEnum.Default, message);
        }

        public static void TraceEntry(string message)
        {
            DoTrace(_tracer, TracerItem.TypeEnum.MethodEntry, TracerItem.PriorityEnum.Default, message);
        }

        public static void TraceEntry()
        {
            DoTrace(_tracer, TracerItem.TypeEnum.MethodEntry, TracerItem.PriorityEnum.Default, string.Empty);
        }

        public static void TraceExit(Tracer tracer)
        {
            DoTrace(tracer, TracerItem.TypeEnum.MethodExit, TracerItem.PriorityEnum.Default, string.Empty);
        }

        public static void TraceExit(string message)
        {
            DoTrace(_tracer, TracerItem.TypeEnum.MethodExit, TracerItem.PriorityEnum.Default, message);
        }

        public static void TraceWarning(Tracer tracer, string message)
        {
            DoTrace(tracer, TracerItem.TypeEnum.Warning, TracerItem.PriorityEnum.High, message);
        }

        public static void TraceOperationError(Tracer tracer, string message)
        {
            DoTrace(tracer, TracerItem.TypeEnum.Operation | TracerItem.TypeEnum.Error, TracerItem.PriorityEnum.VeryHigh, message);
        }

        public static void TraceError(Tracer tracer, string message)
        {
            DoTrace(tracer, TracerItem.TypeEnum.Error, TracerItem.PriorityEnum.VeryHigh, message);
        }

        public static void Trace(Tracer tracer, string message)
        {
            DoTrace(tracer, TracerItem.TypeEnum.Trace, TracerItem.PriorityEnum.Default, message);
        }

        public static void TraceExit()
        {
            DoTrace(_tracer, TracerItem.TypeEnum.MethodExit, TracerItem.PriorityEnum.Default, string.Empty);
        }

        public static void TraceWarning(string message)
        {
            DoTrace(_tracer, TracerItem.TypeEnum.Warning, TracerItem.PriorityEnum.High, message);
        }

        public static void TraceOperationWarning(string message)
        {
            DoTrace(_tracer, TracerItem.TypeEnum.Warning | TracerItem.TypeEnum.Operation, TracerItem.PriorityEnum.High, message);
        }

        public static void TraceOperationError(string message)
        {
            DoTrace(_tracer, TracerItem.TypeEnum.Error | TracerItem.TypeEnum.Operation, TracerItem.PriorityEnum.VeryHigh, message);
        }

        public static void TraceError(string message)
        {
            DoTrace(_tracer, TracerItem.TypeEnum.Error, TracerItem.PriorityEnum.VeryHigh, message);
        }

        public static void Trace(string message)
        {
            DoTrace(_tracer, TracerItem.TypeEnum.Trace, TracerItem.PriorityEnum.Default, message);
        }

        public static void Trace(string message, TracerItem.PriorityEnum priority)
        {
            DoTrace(_tracer, TracerItem.TypeEnum.Trace, priority, message);
        }

        public static void Trace(Tracer tracer, string message, TracerItem.PriorityEnum priority)
        {
            DoTrace(tracer, TracerItem.TypeEnum.Trace, priority, message);
        }

		/// <summary>
		/// Full feature call, no tracer.
		/// </summary>
		/// <param name="tracer"></param>
		/// <param name="type"></param>
		/// <param name="message"></param>
		/// <param name="priority"></param>
		public static void Trace(TracerItem.TypeEnum type, string message, TracerItem.PriorityEnum priority)
		{
			DoTrace(Tracer, type, priority, message);
		}

		/// <summary>
		/// Full feature call, tracer included.
		/// </summary>
		public static void Trace(Tracer tracer, TracerItem.TypeEnum type, string message, TracerItem.PriorityEnum priority)
		{
			DoTrace(tracer, type, priority, message);
		}
	}
}
