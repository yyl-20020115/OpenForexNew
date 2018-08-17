using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CommonSupport
{
    /// <summary>
    /// Helper class handles common operations related to windows forms.
    /// </summary>
    public static class WinFormsHelper
    {
        static InvokeWatchDog _invokeWatchDog = new InvokeWatchDog();

        /// <summary>
        /// 
        /// </summary>
        public struct AsyncResultInfo
        {
            public string ControlName;
            public string MethodName;
            
            public IAsyncResult AsyncResult;
            public DateTime PublishTime;
            
            /// <summary>
            /// 
            /// </summary>
            public AsyncResultInfo(IAsyncResult asyncResult, Delegate d, Control control)
            {
                AsyncResult = asyncResult;
                PublishTime = DateTime.Now;

                MethodName = d.Method.Name;
                ControlName = control.GetType().Name + ";" + control.Name;
            }

        }

        static Dictionary<Control, Dictionary<MethodInfo, MethodInvocationInformation>> _filteredInvokes = new Dictionary<Control, Dictionary<MethodInfo, MethodInvocationInformation>>();

        /// <summary>
        /// Provide a way to obtain the filtered invokes for operation, and unlock, since they tend to lock for long periods.
        /// </summary>
        static List<KeyValuePair<Control, Dictionary<MethodInfo, MethodInvocationInformation>>> FilteredInvokesPairs
        {
            get
            {
                lock (_filteredInvokes)
                {
                    return GeneralHelper.EnumerableToList<KeyValuePair<Control, Dictionary<MethodInfo, MethodInvocationInformation>>>
                        (_filteredInvokes);
                }
            }
        }

        static System.Timers.Timer _periodicInvokeTimer = new System.Timers.Timer();

        /// <summary>
        /// Explicit static constructor to tell C# compiler not to mark type 
        /// as BeforeFieldInit. Required for thread safety of static elements.
        /// </summary>
        static WinFormsHelper()
        {
            _periodicInvokeTimer.AutoReset = false;
            _periodicInvokeTimer.Interval = 100;
            _periodicInvokeTimer.Elapsed += new System.Timers.ElapsedEventHandler(_periodicInvokeTimer_Elapsed);
            _periodicInvokeTimer.Start();
        }

        static void _periodicInvokeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (KeyValuePair<Control, Dictionary<MethodInfo, MethodInvocationInformation>> pair in FilteredInvokesPairs)
            {
                foreach (KeyValuePair<MethodInfo, MethodInvocationInformation> subPair in pair.Value)
                {
                    subPair.Value.CheckCall();
                }
            }

            _periodicInvokeTimer.Start();
        }

        /// <summary>
        /// Since the filtered invoke mechanism keeps references to controls that were invoked,
        /// this method allows you to clear any left over ones, that are not still executing.
        /// </summary>
        public static void CleanUpFilteredInvokesReferences()
        {
            foreach (KeyValuePair<Control, Dictionary<MethodInfo, MethodInvocationInformation>> pair in FilteredInvokesPairs)
            {
                bool activeOperationFound = false;
                foreach (KeyValuePair<MethodInfo, MethodInvocationInformation> subPair in pair.Value)
                {
                    if (subPair.Value.IsCallCompleted == false)
                    {
                        activeOperationFound = true;
                        break;
                    }
                }

                if (activeOperationFound == false)
                {// Control cleared for removing.
                    lock (_filteredInvokes)
                    {
                        _filteredInvokes.Remove(pair.Key);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Color GetBrushBasicColor(Brush brush, Color defaultColor)
        {
            if (brush is SolidBrush)
            {
                return ((SolidBrush)brush).Color;
            }
            else if (brush is LinearGradientBrush)
            {
                return ((LinearGradientBrush)brush).LinearColors[0];
            }

            return defaultColor;
        }

        /// <summary>
        /// Helper, moves all toolstrip items from one toolstrip to another.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void MoveToolStripItems(ToolStrip source, ToolStrip destination)
        {
            foreach (ToolStripItem item in GeneralHelper.EnumerableToList<ToolStripItem>(source.Items))
            {
                destination.Items.Add(item);
            }
        }

        /// <summary>
        /// Activates the invoke watch dog mechanism; use in cases of failing or missin Invoke() calls.
        /// </summary>
        public static void ActivateInvokeWatchDog()
        {
            _invokeWatchDog.Start();
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public static bool BeginFilteredManagedInvoke(Control control, TimeSpan minimumInvocationInterval, GeneralHelper.DefaultDelegate d)
        {
            return BeginFilteredManagedInvoke(control, minimumInvocationInterval, (Delegate)d);
        }

        /// <summary>
        /// Helper.
        /// </summary>
        /// <param name="assurePending">Makes sure at least one call is still pending on the call.</param>
        public static bool BeginFilteredManagedInvoke(Control control, GeneralHelper.DefaultDelegate d)
        {
            return BeginFilteredManagedInvoke(control, TimeSpan.FromMilliseconds(250), (Delegate)d);
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public static bool BeginFilteredManagedInvoke(Control control, Delegate d, params object[] args)
        {
            return BeginFilteredManagedInvoke(control, TimeSpan.FromMilliseconds(250), (Delegate)d, args);
        }

        /// <summary>
        /// Filtered invokes make sure only one instance of the given method call is in action at the given moment.
        /// This is usefull for calls that are made very many times, but we wish to allow the control to only
        /// process as much as it can in real time (not delaying any calls for later).
        /// Filtering is done on a method AND instance basis, so if you have 2 instances of same class alive,
        /// they will not interupt each others calls and each of them will receive what is due.
        /// </summary>
        /// <param name="minimumInterInvocationInterval">The interval between two consecutive invokes of the method; pass null for default; TimeSpan.Zero for immediate (based on a 100ms timer, so delay up to 100ms).</param>
        /// <returns>Result shows if the given invoke was placed, or if there is another one pending executing and this one was dropped.</returns>
        public static bool BeginFilteredManagedInvoke(Control control, TimeSpan minimumInterInvocationInterval, Delegate d, params object[] args)
        {
            MethodInvocationInformation information = null;
            Dictionary<MethodInfo, MethodInvocationInformation> dictionary = null;

            // Obtain the corresponding invocation information item.
            lock (_filteredInvokes)
            {
                if (_filteredInvokes.ContainsKey(control) == false)
                {
                    _filteredInvokes.Add(control, new Dictionary<MethodInfo, MethodInvocationInformation>());
                }

                // Also keep the dictionary operations inside the lock.
                dictionary = _filteredInvokes[control];

                if (dictionary.ContainsKey(d.Method) == false)
                {
                    information = new MethodInvocationInformation(control, d);
                    dictionary.Add(d.Method, information);
                }
                else
                {
                    information = dictionary[d.Method];
                }
            }

            return information.Invoke(minimumInterInvocationInterval, args);
        }

        /// <summary>
        /// Will perform managed invoke if needed, or a direct one if not.
        /// </summary>
        public static void DirectOrManagedInvoke(Control invocationControl, GeneralHelper.DefaultDelegate d)
        {
            if (invocationControl.InvokeRequired)
            {
                BeginManagedInvoke(invocationControl, d);
            }
            else
            {
                d.Invoke();
            }
        }


        /// <summary>
        /// Helper.
        /// </summary>
        public static IAsyncResult BeginManagedInvoke(Control invocationControl, GeneralHelper.DefaultDelegate d)
        {
            return BeginManagedInvoke(invocationControl, (Delegate)d);
        }

        /// <summary>
        /// Helper, automates invocation on a control; also supports a monitoring mechanism agains blocking invokes.
        /// </summary>
        public static IAsyncResult BeginManagedInvoke(Control invocationControl, Delegate d, params object[] args)
        {
            // This better be off, since each call makes the diagnostics call it too, so it is a sort of a circular call
            // (although the timer mechanism protects it).
            //SystemMonitor.Report(invocationControl.Name + "." + d.Method.Name + "." + args.Length);

            if (invocationControl.IsHandleCreated
                && invocationControl.IsDisposed == false
                && invocationControl.Disposing == false)
            {
                IAsyncResult result = invocationControl.BeginInvoke(d, args);
                _invokeWatchDog.Add(new AsyncResultInfo(result, d, invocationControl));
                return result;
            }
            else
            {
                //SystemMonitor.Report(string.Format("Failed to invoke delegate[{0}] on control [{1}], since control not ready.", d.Method.Name, invocationControl.Name), TracerItem.PriorityEnum.High);
            }

            return null;
        }

        /// <summary>
        /// Helper, automates the saving of items of virtual list to a CSV files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SaveVirtualListItemsToCSV(ListView virtualListViewEx1, RetrieveVirtualItemEventHandler handler)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Select save file";
                dialog.AddExtension = true;
                dialog.RestoreDirectory = true;
                dialog.DefaultExt = "csv";
                dialog.Filter = "Text file (*.csv)|*.csv";

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                StringBuilder builder = WinFormsHelper.VirtualListItemsToCSV(virtualListViewEx1, handler);
                using (FileWriterHelper helper = new FileWriterHelper())
                {
                    if (helper.Initialize(dialog.FileName) == false)
                    {
                        MessageBox.Show("Failed to save file.");
                        return;
                    }

                    helper.Write(builder.ToString());
                }
            }
        }


        /// <summary>
        /// Helper, saving virtual list items to string builder.
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static StringBuilder VirtualListItemsToCSV(ListView listView, RetrieveVirtualItemEventHandler handler)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < listView.VirtualListSize; i++)
            {
                RetrieveVirtualItemEventArgs args = new RetrieveVirtualItemEventArgs(i);
                handler(listView, args);

                if (args.Item != null)
                {
                    RenderListViewItem(args.Item, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Helper, convert list items to CSV file.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static StringBuilder ListItemsToCSV(ListView view)
        {
            StringBuilder result = new StringBuilder();
            foreach (ListViewItem item in view.Items)
            {
                RenderListViewItem(item, result);
            }

            return result;
        }

        static void RenderListViewItem(ListViewItem item, StringBuilder builder)
        {
            builder.Append(Environment.NewLine);
            for (int i = 0; i < item.SubItems.Count; i++)
            {
                ListViewItem.ListViewSubItem subItem = item.SubItems[i];
                if (i != 0)
                {
                    builder.Append(", ");
                }

                builder.Append(subItem.Text);
            }
        }

        /// <summary>
        /// Show a message box in a standartized way.
        /// </summary>
        public static DialogResult ShowMessageBox(string text, string caption, 
            MessageBoxButtons? buttons, MessageBoxIcon? icon)
        {
            if (buttons.HasValue == false)
            {
                buttons = MessageBoxButtons.OK;
            }

            if (icon.HasValue == false)
            {
                icon = MessageBoxIcon.None;
            }

            return MessageBox.Show(text, Application.ProductName + " - " + caption, buttons.Value, icon.Value);
        }
    }
}
