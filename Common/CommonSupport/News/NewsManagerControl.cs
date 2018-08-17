using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CommonSupport;

namespace CommonSupport
{
    /// <summary>
    /// News item visualization control.
    /// </summary>
    public partial class NewsManagerControl : UserControl
    {
        enum Mode
        {
            Default,
            Searching,
            Marking
        }

        enum ShowMode
        {
            Default,
            Deleted,
            Read,
            UnRead,
            Favourite
        }

        ShowMode _showMode = ShowMode.Default;

        Mode _mode = Mode.Default;

        volatile NewsManager _manager;
        public NewsManager Manager
        {
            get 
            { 
                return _manager; 
            }

            set
            {
                if (_manager != null)
                {
                    _manager.SourceAddedEvent -= new NewsManager.NewsSourceUpdateDelegate(_manager_SourceAddedEvent);
                    _manager.SourceRemovedEvent -= new NewsManager.NewsSourceUpdateDelegate(_manager_SourceRemovedEvent);

                    _manager.SourceItemsAddedEvent -= new NewsManager.NewsSourceItemsUpdateDelegate(_manager_SourceItemsAddedEvent);

                    _manager.UpdatingStartedEvent -= new NewsManager.GeneralUpdateDelegate(_manager_UpdatingStartedEvent);
                    _manager.UpdatingFinishedEvent -= new NewsManager.GeneralUpdateDelegate(_manager_UpdatingFinishedEvent);

                    _manager = null;
                }

                _manager = value;

                if (_manager != null)
                {
                    _manager.SourceAddedEvent += new NewsManager.NewsSourceUpdateDelegate(_manager_SourceAddedEvent);
                    _manager.SourceRemovedEvent += new NewsManager.NewsSourceUpdateDelegate(_manager_SourceRemovedEvent);

                    _manager.SourceItemsAddedEvent += new NewsManager.NewsSourceItemsUpdateDelegate(_manager_SourceItemsAddedEvent);

                    _manager.UpdatingStartedEvent += new NewsManager.GeneralUpdateDelegate(_manager_UpdatingStartedEvent);
                    _manager.UpdatingFinishedEvent += new NewsManager.GeneralUpdateDelegate(_manager_UpdatingFinishedEvent);

                    WinFormsHelper.BeginManagedInvoke(this, UpdateUI);
                }
            }
        }

        volatile int _maximumItemsShown;
        public int MaximumItemsShown
        {
            get { return _maximumItemsShown; }
            set { _maximumItemsShown = value; }
        }

        EventSource _selectedSource = null;

        List<FinancialNewsEvent> _items = new List<FinancialNewsEvent>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public NewsManagerControl()
        {
            InitializeComponent();

            listView.AutoScrollSlack = 0;

            lock (listView)
            {
                listView.AdvancedColumnManagementUnsafe.Add(1, new VirtualListViewEx.ColumnManagementInfo() { FillWhiteSpace = true });
                listView.AdvancedColumnManagementUnsafe.Add(2, new VirtualListViewEx.ColumnManagementInfo() { MinWidth = 110, AutoResizeMode = ColumnHeaderAutoResizeStyle.ColumnContent });
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            _maximumItemsShown = 10000;

            base.OnLoad(e);
        }

        private void OnLoadEvent(object sender, EventArgs e)
        {
            UpdateUI();
        }

        void _manager_SourceItemsAddedEvent(NewsManager manager, EventSource source, IEnumerable<EventBase> events)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromSeconds(1), UpdateUI);
        }

        void _manager_UpdatingFinishedEvent(NewsManager manager)
        {// Needed for the update status icon.
            WinFormsHelper.BeginManagedInvoke(this, UpdateUI);
        }

        void _manager_UpdatingStartedEvent(NewsManager manager)
        {// Needed for the update status icon.
            WinFormsHelper.BeginManagedInvoke(this, UpdateUI);
        }

        bool IsSourceSelected(EventSource source)
        {
            return _selectedSource == null || source == _selectedSource;
        }

        bool IsChannelSelected(string channelName)
        {
            return true;
            //return _selectedChannel == null || _selectedChannel == channel; 
        }

        bool IsItemSearched(FinancialNewsEvent item)
        {
            return (item.Title.ToLower().Contains(this.toolStripTextBoxSearch.Text.ToLower())
                || item.Description.ToLower().Contains(this.toolStripTextBoxSearch.Text.ToLower()));
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        public void UpdateUI()
        {
            toolStripButtonSettings.Enabled = _manager != null;
            toolStripButtonSearchClear.Enabled = _manager != null;
            toolStripButtonSearch.Enabled = _manager != null;
            toolStripButtonMarkRead.Enabled = _manager != null;
            toolStripLabelSources.Enabled = _manager != null;
            toolStripLabelUpdating.Enabled = _manager != null;
            toolStripButtonUpdate.Enabled = _manager != null;
            toolStripButtonMark.Enabled = _manager != null;
            toolStripDropDownButtonSource.Enabled = _manager != null;

            toolStripButtonDetails.Checked = newsItemControl1.Visible;

            if (_manager == null || this.DesignMode)
            {
                return;
            }

            toolStripLabelSources.Text = "Feeds [" + _manager.NewsSourcesArray.Length + "]";
            toolStripButtonUpdate.Enabled = !_manager.IsUpdating;

            if (_manager.IsUpdating)
            {
                toolStripLabelUpdating.DisplayStyle = ToolStripItemDisplayStyle.Image;
            }
            else
            {
                toolStripLabelUpdating.DisplayStyle = ToolStripItemDisplayStyle.Text;
            }

            // Obtain all items sorted by time.
            SortedList<FinancialNewsEvent, FinancialNewsEvent> combinedSortedItems =
                new SortedList<FinancialNewsEvent, FinancialNewsEvent>();

            foreach (EventSource source in _manager.NewsSourcesArray)
            {
                if ((_selectedSource != null && source != _selectedSource) || source.Enabled == false)
                {// Source filter limitation.
                    continue;
                }

                SortedList<FinancialNewsEvent, FinancialNewsEvent> sourceItems = source.GetAllItems<FinancialNewsEvent>();

                foreach (KeyValuePair<FinancialNewsEvent, FinancialNewsEvent> eventPair in sourceItems)
                {
					if (combinedSortedItems.ContainsKey(eventPair.Key))
					{
                        SystemMonitor.OperationError("Item already added to combined container.");
					}
					else
					{
                        combinedSortedItems[eventPair.Key] = eventPair.Value;
					}
                }
            }

            _items.Clear();

            // Filter
            foreach (FinancialNewsEvent item in combinedSortedItems.Values)
            {
                switch (_showMode)
                {
                    case ShowMode.Default:
                        if (item.IsVisible == false)
                        {// All non deleted.
                            continue;
                        }
                        break;
                    case ShowMode.Deleted:
                        if (item.IsVisible)
                        {// Only deleted.
                            continue;
                        }
                        break;
                    case ShowMode.Read:
                        if (item.IsVisible == false || item.IsRead == false)
                        {// Visible read.
                            continue;
                        }
                        break;
                    case ShowMode.UnRead:
                        if (item.IsVisible == false || item.IsRead)
                        {// Visible non read.
                            continue;
                        }
                        break;
                    case ShowMode.Favourite:
                        if (item.IsVisible == false || item.IsFavourite == false)
                        {// Visible favourites.
                            continue;
                        }
                        break;
                    default:
                        break;
                }

                if (_mode != Mode.Searching || IsItemSearched(item))
                {
                    _items.Insert(0, item);
                }
                if (MaximumItemsShown > 0 && _items.Count >= MaximumItemsShown)
                {
                    break;
                }

                if (MaximumItemsShown > 0 && _items.Count >= MaximumItemsShown)
                {
                    break;
                }
            }

            //_items.Reverse();
            
            listView.VirtualListSize = _items.Count;

            listView.Invalidate();
            listView.UpdateColumnWidths();

            // Needed to update scroll state and evade a bug in the list control (problem in any win list control).
            //this.listView.Scrollable = false;
            //this.listView.Scrollable = true;
        }

        void _manager_SourceRemovedEvent(NewsManager manager, EventSource source)
        {
            WinFormsHelper.BeginManagedInvoke(this, UpdateUI);
        }

        void _manager_SourceAddedEvent(NewsManager manager, EventSource source)
        {
            WinFormsHelper.BeginManagedInvoke(this, UpdateUI);
        }

        private void toolStripButtonSettings_Click(object sender, EventArgs e)
        {
            NewsManagerSettingsControl control = new NewsManagerSettingsControl(this);
            HostingForm form = new HostingForm("News Management Properties", control);
            form.StartPosition = FormStartPosition.WindowsDefaultLocation;
            form.ShowDialog();
        }

        private void toolStripDropDownButtonSource_DropDownOpening(object sender, EventArgs e)
        {
            toolStripDropDownButtonSource.DropDownItems.Clear();
            ToolStripMenuItem itemAll = new ToolStripMenuItem("All Feeds");
            toolStripDropDownButtonSource.DropDownItems.Add(itemAll);
            toolStripDropDownButtonSource.DropDownItems.Add(new ToolStripSeparator());

            foreach (EventSource source in _manager.NewsSourcesArray)
            {
                if (source.Enabled == false)
                {
                    continue;
                }

                string name = source.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = source.Address;
                }

                ToolStripMenuItem item = new ToolStripMenuItem(name);
                item.Checked = (source == _selectedSource);
                item.Tag = source;
                int index = toolStripDropDownButtonSource.DropDownItems.Add(item);
                //toolStripDropDownButtonSource.DropDownItems[index].Tag = feed;
            }
        }

        private void toolStripDropDownButtonSource_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _selectedSource = (EventSource)e.ClickedItem.Tag;
            

            UpdateUI();
        }

        private void toolStripButtonUpdate_Click(object sender, EventArgs e)
        {
            if (_manager != null)
            {
                GeneralHelper.FireAndForget(_manager.UpdateFeeds);
                toolStripButtonUpdate.Enabled = false;
            }
        }

        private void listViewItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count > 0)
            {
                FinancialNewsEvent item = _items[listView.SelectedIndices[0]];
                newsItemControl1.NewsItem = item;
                
                if (item.IsRead == false)
                {
                    item.IsRead = true;
                    
                    item.Channel.HandleItemsUpdated(new FinancialNewsEvent[] { item });
                }
            }
            else
            {
                newsItemControl1.NewsItem = null;
            }
        }

        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            _mode = Mode.Searching;
            UpdateUI();
        }

        private void toolStripButtonSearchClear_Click(object sender, EventArgs e)
        {
            toolStripTextBoxSearch.Text = "";
            _mode = Mode.Default;
               
            UpdateUI();
        }

        private void toolStripTextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toolStripButtonSearch_Click(sender, EventArgs.Empty);
            }
        }

        private void toolStripButtonMark_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolStripTextBoxSearch.Text) == false)
            {
                _mode = Mode.Marking;
            }
            else
            {
                _mode = Mode.Default;
            }

            UpdateUI();
        }

        private void toolStripDropDownButtonChannel_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //_selectedChannel = (RssChannel)e.ClickedItem.Tag;
            UpdateUI();
        }

        private void toolStripDropDownButtonChannel_DropDownOpening(object sender, EventArgs e)
        {
            toolStripDropDownButtonChannel.DropDownItems.Clear();
            ToolStripMenuItem itemAll = new ToolStripMenuItem("All Channels");
            toolStripDropDownButtonChannel.DropDownItems.Add(itemAll);
            toolStripDropDownButtonChannel.DropDownItems.Add(new ToolStripSeparator());

            if (_selectedSource != null)
            {
                foreach (string channel in _selectedSource.ChannelsNames)
                {
                    string name = channel;

                    ToolStripMenuItem item = new ToolStripMenuItem(name);
                    item.Checked = true;
                    item.Tag = channel;
                    int index = toolStripDropDownButtonChannel.DropDownItems.Add(item);
                }
            }
        }

        private void listViewItems_DoubleClick(object sender, EventArgs e)
        {
            newsItemControl1.ShowItemDetails();
        }

        //private void toolStripButtonClear_Click(object sender, EventArgs e)
        //{
        //    listView.VirtualListSize = 0;
        //    listView.Refresh();
        //}

        private void objectListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_items.Count <= e.ItemIndex)
            {
                SystemMonitor.Warning("UI inconsitency.");
                return;
            }

            FinancialNewsEvent item = _items[e.ItemIndex];

            e.Item = new ListViewItem("");
            if (item.IsVisible)
            {
                if (item.IsFavourite)
                {
                    e.Item.ImageIndex = 3;
                }
                else
                if (item.IsRead)
                {
                    e.Item.ImageIndex = 0;
                }
                else
                {
                    e.Item.ImageIndex = 1;
                }
            }

            //if (item.Source != null)
            //{// Use index, since image key does not seem to work.
            //    e.Item.ImageIndex = imageList.Images.IndexOfKey(item.Source.Address);
            //}

            e.Item.SubItems.Add(item.Title);
            e.Item.SubItems.Add(GeneralHelper.GetShortDateTimeNoYear(item.DateTime));

            if (_mode == Mode.Marking && IsItemSearched(item))
            {
                e.Item.BackColor = Color.MistyRose;
            }
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.listViewItems_DoubleClick(sender, e);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                foreach (int index in listView.SelectedIndices)
                {
                    _items[index].IsVisible = false;
                    _items[index].Channel.HandleItemsUpdated(new FinancialNewsEvent[] { _items[index] });
                }
                listView.SelectedIndices.Clear();
                UpdateUI();
            }
        }

        /// <summary>
        /// If items is null, selected items will be gathered and used.
        /// </summary>
        void MakeItems(IEnumerable<FinancialNewsEvent> items, bool markRead, bool markDeleted, bool markFavourite, bool markNotFavourite)
        {
            if (items == null)
            {
                items = new List<FinancialNewsEvent>();
                foreach (int index in this.listView.SelectedIndices)
                {
                    ((List<FinancialNewsEvent>)items).Add(_items[index]);
                }
            }

            Dictionary<EventSourceChannel, List<EventBase>> itemsModified = new Dictionary<EventSourceChannel, List<EventBase>>();
            foreach (FinancialNewsEvent item in items)
            {
                if (markRead && item.IsRead == false)
                {
                    if (itemsModified.ContainsKey(item.Channel) == false)
                    {
                        itemsModified.Add(item.Channel, new List<EventBase>());
                    }

                    if (itemsModified[item.Channel].Contains(item) == false)
                    {
                        itemsModified[item.Channel].Add(item);
                    }
                    item.IsRead = true;
                }

                if (markDeleted && item.IsVisible)
                {
                    if (itemsModified.ContainsKey(item.Channel) == false)
                    {
                        itemsModified.Add(item.Channel, new List<EventBase>());
                    }
                    if (itemsModified[item.Channel].Contains(item) == false)
                    {
                        itemsModified[item.Channel].Add(item);
                    }
                    item.IsVisible = false;
                }

                if (markFavourite && item.IsFavourite == false)
                {
                    if (itemsModified.ContainsKey(item.Channel) == false)
                    {
                        itemsModified.Add(item.Channel, new List<EventBase>());
                    }
                    if (itemsModified[item.Channel].Contains(item) == false)
                    {
                        itemsModified[item.Channel].Add(item);
                    }
                    item.IsFavourite = true;
                }
                else
                if (markNotFavourite && item.IsFavourite)
                {
                    if (itemsModified.ContainsKey(item.Channel) == false)
                    {
                        itemsModified.Add(item.Channel, new List<EventBase>());
                    }
                    if (itemsModified[item.Channel].Contains(item) == false)
                    {
                        itemsModified[item.Channel].Add(item);
                    }
                    item.IsFavourite = false;
                }
            }

            foreach (KeyValuePair<EventSourceChannel, List<EventBase>> channelPair in itemsModified)
            {
				foreach (EventBase item in channelPair.Value)
				{
					SystemMonitor.Report(item.Id.ToString());
				}

                channelPair.Key.HandleItemsUpdated(channelPair.Value);
            }
        }

        private void toolStripButtonMarkAllRead_Click(object sender, EventArgs e)
        {
            MakeItems(_items, true, false, false, false);
            UpdateUI();
        }

        private void toolStripButtonMarkRead_Click(object sender, EventArgs e)
        {
            MakeItems(null, true, false, false, false);
            UpdateUI();
        }

        private void buttonShowModeFavourite_Click(object sender, EventArgs e)
        {
            _showMode = ShowMode.Favourite;
            UpdateUI();
        }

        private void buttonShowModeDeleted_Click(object sender, EventArgs e)
        {
            _showMode = ShowMode.Deleted;
            UpdateUI();
        }

        private void buttonShowModeUnread_Click(object sender, EventArgs e)
        {
            _showMode = ShowMode.UnRead;
            UpdateUI();

        }

        private void buttonShowModeRead_Click(object sender, EventArgs e)
        {
            _showMode = ShowMode.Read;
            UpdateUI();
        }

        private void buttonShowModeDefault_Click(object sender, EventArgs e)
        {
            _showMode = ShowMode.Default;
            UpdateUI();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeItems(null, false, true, false, false);
            listView.SelectedIndices.Clear();
            UpdateUI();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            MakeItems(null, false, false, true, false);
            UpdateUI();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            MakeItems(null, false, false, false, true);
            UpdateUI();
        }

        private void toolStripButtonDetails_Click(object sender, EventArgs e)
        {
            newsItemControl1.Visible = toolStripButtonDetails.Checked;
            splitter1.Visible = newsItemControl1.Visible;
        }

    }
}
