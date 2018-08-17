using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonSupport;
using System.IO;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control visualizes and manages online dataDelivery store entries.
    /// </summary>
    public partial class OnlineDataStoresControl : UserControl
    {
        DataStore _manager;

        public DataStore Manager
        {
            get 
            { 
                return _manager; 
            }

            set 
            {
                if (_manager != null)
                {
                    _manager = null;
                }

                _manager = value;

                if (_manager != null)
                {
                }

                WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
            }
        }

        List<OnlineEntrySource> SelectedOnlineEntries
        {
            get
            {
                List<OnlineEntrySource> sources = new List<OnlineEntrySource>();
                foreach (ListViewItem item in listViewOnlineEntries.SelectedItems)
                {
                    sources.Add(item.Tag as OnlineEntrySource);
                }

                return sources;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public OnlineDataStoresControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateUI();
        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateUI()
        {
            if (_manager == null)
            {
                return;
            }

            lock (Manager)
            {
                for (int i = 0; i < Manager.OnlineEntrySources.Count; i++)
                {
                    ListViewItem item;
                    if (listViewOnlineEntries.Items.Count <= i)
                    {
                        item = new ListViewItem();
                        listViewOnlineEntries.Items.Add(item);
                    }
                    else
                    {
                        item = listViewOnlineEntries.Items[i];
                    }

                    SetItemAsOnlineEntry(item, Manager.OnlineEntrySources[i]);
                }

                while (listViewOnlineEntries.Items.Count > Manager.OnlineEntrySources.Count)
                {
                    listViewOnlineEntries.Items.RemoveAt(listViewOnlineEntries.Items.Count - 1);
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="entry"></param>
        void SetItemAsOnlineEntry(ListViewItem item, OnlineEntrySource entry)
        {
            if (listViewOnlineEntries.Groups[entry.Source] == null)
            {
                listViewOnlineEntries.Groups.Add(entry.Source, entry.Source);
            }

            listViewOnlineEntries.Groups[entry.Source].Items.Add(item);

            if (item.Text != entry.SymbolName)
            {
                item.Text = entry.SymbolName;
            }

            Color foreColor = SystemColors.ControlText;
            if (entry.IsDownloading)
            {
                foreColor = Color.Red;
            }

            if (item.ForeColor != foreColor)
            {
                item.ForeColor = foreColor;
            }

            if (item.Tag != entry)
            {
                if (item.Tag != null)
                {
                    OnlineEntrySource previousEntry = item.Tag as OnlineEntrySource;
                    previousEntry.DataDownloadUpdateEvent -= new OnlineEntrySource.EntrySourceUpdateDelegate(entry_DataDownloadUpdateEvent);
                    item.Tag = null;
                }

                item.Tag = entry;
                entry.DataDownloadUpdateEvent += new OnlineEntrySource.EntrySourceUpdateDelegate(entry_DataDownloadUpdateEvent);
            }

            while (item.SubItems.Count < 5)
            {
                item.SubItems.Add("");
            }

            if (item.SubItems[1].Text != item.SubItems[1].Text)
            {
                item.SubItems[1].Text = entry.Period.ToString();
            }

            if (item.SubItems[2].Text != Path.GetFileName(entry.Uri))
            {
                item.SubItems[2].Text = Path.GetFileName(entry.Uri);
            }

            if (item.SubItems[3].Text != "-")
            {
                item.SubItems[3].Text = "-";
            }

            if (item.SubItems[4].Text != entry.DownloadProgressPercentage.ToString())
            {
                // Quote count.
                item.SubItems[4].Text = entry.DownloadProgressPercentage.ToString();
            }

        }

        void entry_DataDownloadUpdateEvent(OnlineEntrySource entrySource)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }


        private void toolStripButtonDownloadToFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                foreach (OnlineEntrySource source in SelectedOnlineEntries)
                {
                    if (source.IsDownloading == false)
                    {
                        source.BeginDownload(false, fbd.SelectedPath);
                        source.DataDownloadedEvent += source_DataDownloadedEventToDirectory;
                    }
                }
            }

        }

        private void toolStripDropDownButtonDownloadToDataStore_Click(object sender, EventArgs e)
        {
            foreach (OnlineEntrySource source in SelectedOnlineEntries)
            {
                if (source.IsDownloading == false)
                {
                    source.BeginDownload(true, string.Empty);
                    source.DataDownloadedEvent += source_DataDownloadedEventToDataStore;
                }
            }
        }


        void source_DataDownloadedEventToDirectory(OnlineEntrySource source)
        {
            source.DataDownloadedEvent -= new OnlineEntrySource.EntrySourceUpdateDelegate(source_DataDownloadedEventToDataStore);
            source.EndDownload(false);
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        void source_DataDownloadedEventToDataStore(OnlineEntrySource source)
        {
            source.DataDownloadedEvent -= new OnlineEntrySource.EntrySourceUpdateDelegate(source_DataDownloadedEventToDataStore);

            if (source.DownloadSucceeded)
            {
                foreach (string filePath in source.DownloadedTempFilesPaths)
                {
                    Manager.AddEntryFromLocalFile(filePath);
                }

                WinFormsHelper.BeginManagedInvoke(this, delegate()
                {
                    toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    toolStripStatusLabel1.Text = "Download of [" + source.Uri.ToString() + "] successful.";
                });
            }
            else
            {
                WinFormsHelper.BeginManagedInvoke(this, delegate()
                {
                    toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    toolStripStatusLabel1.Text = "Download of [" + source.Uri.ToString() + "] failed.";
                });
            }

            source.EndDownload(true);
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }
    }
}
