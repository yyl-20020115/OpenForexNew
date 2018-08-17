using System.Windows.Forms;
using System.Collections.Generic;

namespace CommonSupport
{
    /// <summary>
    /// Class displays important system diagnostics information.
    /// </summary>
    public partial class DiagnosticsControl : CommonBaseControl
    {
        /// <summary>
        /// 
        /// </summary>
        public DiagnosticsControl()
        {
            InitializeComponent();

            SystemMonitor.Variables.VariablesUpdatedEvent += new SystemVariables.VariablesUpdatedDelegate(Variables_VariablesUpdatedEvent);
        }

        public override void  UnInitializeControl()
        {
            base.UnInitializeControl();
            SystemMonitor.Variables.VariablesUpdatedEvent -= new SystemVariables.VariablesUpdatedDelegate(Variables_VariablesUpdatedEvent);
        }

        void Variables_VariablesUpdatedEvent(SystemVariables variables)
        {
            WinFormsHelper.BeginManagedInvoke(this, UpdateUI);
        }

        void UpdateUI()
        {
            List<string> names;
            List<double?> values;

            SystemMonitor.Variables.GetNamesValues(out names, out values);

            for (int i = 0; i < names.Count; i++)
            {
                if (listViewVariables.Items.Count <= i)
                {
                    listViewVariables.Items.Add(new ListViewItem(new string[] { "-", "0" }));
                }

                SetItemAsVarialbe(listViewVariables.Items[i], names[i], values[i]);
            }
        }

        void SetItemAsVarialbe(ListViewItem item, string name, double? value)
        {
            if (item.SubItems[0].Text != name)
            {
                item.SubItems[0].Text = name;
            }

            string valueString = GeneralHelper.ToString(value);
            if (item.SubItems[1].Text != valueString)
            {
                item.SubItems[1].Text = valueString;
            }
            
        }

        private void ApplicationDiagnosticsInformationControl_SizeChanged(object sender, System.EventArgs e)
        {
            listViewVariables.Columns[0].Width = this.Width - listViewVariables.Columns[1].Width - 5;
        }
    }
}
