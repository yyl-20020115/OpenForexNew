using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonFinancial;
using CommonSupport;
using System.IO;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// UI Control for the Data Store platform component.
    /// </summary>
    public partial class DataStoreManagementComponentControl : PlatformComponentControl
    {
        DataStoreManagementComponent DataStoreComponent
        {
            get { return (DataStoreManagementComponent)base.Component; }
        }

        WaitControl _waitControl = new WaitControl();

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataStoreManagementComponentControl(DataStoreManagementComponent store)
            : base(store)
        {
            InitializeComponent();

            splitContainer1.Panel1.Controls.Add(_waitControl);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DataStoreComponent.OperationalStateChangedEvent += new OperationalStateChangedDelegate(DataStore_OperationalStatusChangedEvent);
            DataStoreComponent.DataStore.EntryAddedEvent += new DataStore.EntryUpdateDelegate(Manager_EntryAddedEvent);
            DataStoreComponent.DataStore.EntryRemovedEvent += new DataStore.EntryUpdateDelegate(Manager_EntryRemovedEvent);
            DataStoreComponent.DataStore.OnlineEntrySourceAddedEvent += new DataStore.OnlineEntrySourceUpdateDelegate(Manager_OnlineEntrySourceAddedEvent);
            DataStoreComponent.DataStore.OnlineEntrySourceRemovedEvent += new DataStore.OnlineEntrySourceUpdateDelegate(Manager_OnlineEntrySourceRemovedEvent);

            UpdateUI();
        }

        void Manager_OnlineEntrySourceAddedEvent(DataStore manager, OnlineEntrySource entrySource)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        void Manager_OnlineEntrySourceRemovedEvent(DataStore manager, OnlineEntrySource entrySource)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        public override void UnInitializeControl()
        {
            DataStoreComponent.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(DataStore_OperationalStatusChangedEvent);
            DataStoreComponent.DataStore.EntryAddedEvent -= new DataStore.EntryUpdateDelegate(Manager_EntryAddedEvent);
            DataStoreComponent.DataStore.EntryRemovedEvent -= new DataStore.EntryUpdateDelegate(Manager_EntryRemovedEvent);

            DataStoreComponent.DataStore.OnlineEntrySourceAddedEvent -= new DataStore.OnlineEntrySourceUpdateDelegate(Manager_OnlineEntrySourceAddedEvent);
            DataStoreComponent.DataStore.OnlineEntrySourceRemovedEvent -= new DataStore.OnlineEntrySourceUpdateDelegate(Manager_OnlineEntrySourceRemovedEvent);

            base.UnInitializeControl();
        }

        void DataStore_OperationalStatusChangedEvent(IOperational parameter1, OperationalStateEnum parameter2)
        {

        }

        void Manager_EntryAddedEvent(DataStore manager, DataStoreEntry entry)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        void Manager_EntryRemovedEvent(DataStore manager, DataStoreEntry entry)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            if (DataStoreComponent.DataStore == null)
            {
                return;
            }

            lock (DataStoreComponent.DataStore)
            {
                int i = 0;
                foreach(DataStoreEntry entry in DataStoreComponent.DataStore.Entries)
			    {
                    ListViewItem item;
                    if (listViewProviderEntries.Items.Count <= i)
                    {
                        item = new ListViewItem();
                        listViewProviderEntries.Items.Add(item);
                    }
                    else
                    {
                        item = listViewProviderEntries.Items[i];
                    }

                    SetItemAsEntryLocal(item, entry);
                    i++;
                }

                while (listViewProviderEntries.Items.Count > DataStoreComponent.DataStore.EntriesCount)
                {
                    listViewProviderEntries.Items.RemoveAt(listViewProviderEntries.Items.Count - 1);
                }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="entry"></param>
        void SetItemAsEntryLocal(ListViewItem item, DataStoreEntry entry)
        {
            if (listViewProviderEntries.Groups[entry.Symbol.Source] == null)
            {
                listViewProviderEntries.Groups.Add(entry.Symbol.Source, entry.Symbol.Source);
            }

            listViewProviderEntries.Groups[entry.Symbol.Source].Items.Add(item);

            while (item.SubItems.Count < 6)
            {
                item.SubItems.Add("");
            }

            item.Text = entry.Symbol.Name;
            item.Tag = entry;
            if (entry.Period.HasValue)
            {
                item.SubItems[1].Text = entry.Period.Value.ToString();
            }
            else
            {
                item.SubItems[1].Text = "-";
            }

            item.SubItems[2].Text = entry.StartTime.ToString();
            item.SubItems[3].Text = entry.EndTime.ToString();

            // Quote count.
            item.SubItems[4].Text = entry.QuoteCount.ToString();

            item.SubItems[5].Text = entry.Description;
        }

        private void listViewProviderEntries_SizeChanged(object sender, EventArgs e)
        {
            listViewProviderEntries.Columns[listViewProviderEntries.Columns.Count - 1].Width = -2;
            //listViewProviderEntries.Columns[listViewProviderEntries.Columns.Count - 1].Width -= 2;
        }

        private void toolStripButtonShowEntryData_Click(object sender, EventArgs e)
        {
            if (listViewProviderEntries.SelectedItems.Count == 0)
            {
                return;
            }

            if (listViewProviderEntries.SelectedItems.Count > 1)
            {
                MessageBox.Show("Select only one entry to show its data.", "Note", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataStoreEntry entry = (DataStoreEntry)listViewProviderEntries.SelectedItems[0].Tag;
            if (entry.Period.HasValue == false || entry.DataType != DataStoreEntry.EntryDataTypeEnum.DataBar)
            {
                return;
            }

            DataReaderWriter<DataBar> readerWriter = entry.GetDataBarReaderWriter();
            if (readerWriter == null)
            {
                MessageBox.Show("Failed to read entry file.", "Open Forex Platform", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<DataBar> barData = readerWriter.Read();
            
            RawDataTradeChartSeries series = new RawDataTradeChartSeries(entry.Symbol.Name);
            series.Initialize(barData, entry.Period.Value);

            ChartForm form = new ChartForm("Entry [" + entry.Symbol.Name + "," + entry.Period.ToString() + "] Data");
            form.Chart.MasterPane.Add(series);
            form.Chart.MasterPane.FitDrawingSpaceToScreen(true, true);

            form.Show(this.ParentForm);
        }

        private void toolStripMenuItemFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All supported|*.csv;*.hst|CSV (*.csv)|*.csv|HST (*.hst)|*.hst";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _waitControl.Message = "Importing data, please wait.";
                _waitControl.SetActiveState(true);

                GeneralHelper.FireAndForget(delegate()
                {
                    string errors = string.Empty;

                    foreach (string filePath in ofd.FileNames)
                    {
                        if (DataStoreComponent.DataStore.AddEntryFromLocalFile(filePath) == null)
                        {
                            errors += "Failed to create entry from file [" + filePath + "]." + Environment.NewLine;
                        }
                    }

                    _waitControl.SetActiveState(false);
                    WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);

                    if (string.IsNullOrEmpty(errors) == false)
                    {
                        MessageBox.Show(errors, "Error(s) in Importing Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
        }

        private void toolStripButtonRemoveEntry_Click(object sender, EventArgs e)
        {
            if (listViewProviderEntries.SelectedItems.Count == 0)
            {
                return;
            }

            if (MessageBox.Show("Deleting quote entries will delete all quote data associated with these entries. Are you sure?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }

            foreach (ListViewItem item in listViewProviderEntries.SelectedItems)
            {
                DataStoreComponent.DataStore.RemoveEntry((DataStoreEntry)item.Tag);
            }

            UpdateUI();

        }

        private void fromOnlineSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HostingForm form = new HostingForm("Online Sources", new OnlineDataStoresControl() { Manager = DataStoreComponent.DataStore });
            form.Show(this.ParentForm);
        }

    }
}
