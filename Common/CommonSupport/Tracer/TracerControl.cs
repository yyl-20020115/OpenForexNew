using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace CommonSupport
{
    /// <summary>
    /// Control allows the visualization of tracer and tracer items, filters etc.
    /// It is designed to operate on the items of the first tracer item sink keeper in tracer.
    /// </summary>
    public partial class TracerControl : UserControl
    {
        MethodTracerFilter _methodFilter;
        StringTracerFilter _stringFilter;
        StringTracerFilter _stringInputFilter;
        TypeTracerFilter _typeFilter;
        PriorityFilter _priorityFilter;

        const char Separator = ';';

        string _markingMatch = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string MarkingMatch
        {
            get { return _markingMatch; }
            set 
            { 
                _markingMatch = value;

                DoUpdateUI();
                this.Refresh();
            }
        }

        volatile bool _itemsModified = false;

        volatile TracerItemKeeperSink _itemKeeperSink = null;

        volatile Tracer _tracer;
        /// <summary>
        /// The tracer assigned to this control.
        /// </summary>
        public Tracer Tracer
        {
            get { return _tracer; }
            set
            {
                TracerItemKeeperSink itemKeeperSink = _itemKeeperSink;
                if (itemKeeperSink != null)
                {
                    itemKeeperSink.ItemAddedEvent -= new TracerItemKeeperSink.ItemUpdateDelegate(_tracer_ItemAddedEvent);
                    itemKeeperSink.ItemsFilteredEvent -= new TracerItemKeeperSink.TracerUpdateDelegate(_itemKeeperSink_ItemsFilteredEvent);
                    itemKeeperSink.FilterUpdateEvent -= new TracerItemSink.SinkUpdateDelegate(_itemSink_FilterUpdateEvent);
                    itemKeeperSink.ClearFilters();
                    _itemKeeperSink = null;
                }

                if (_tracer != null)
                {
                    _methodFilter = null;
                    _stringFilter = null;
                    
                    _typeFilter = null;
                    _priorityFilter = null;

                    _tracer = null;

                    methodTracerFilterControl1.Filter = null;
                    typeTracerFilterControl1.Filter = null;
                }

                _tracer = value;
                //_mode = ModeEnum.Default;

                if (_tracer != null)
                {
                    _itemKeeperSink = (TracerItemKeeperSink)_tracer.GetSinkByType(typeof(TracerItemKeeperSink));

                    _stringInputFilter = new StringTracerFilter();
                    _tracer.Add(_stringInputFilter);

                    // Attach negative string filter property as data source.
                    _inputExcludeStrip.SetDataSource(_stringInputFilter, 
                        _stringInputFilter.GetType().GetProperty("NegativeFilterStrings"));

                    if (_itemKeeperSink != null)
                    {
                        _itemKeeperSink.ItemAddedEvent += new TracerItemKeeperSink.ItemUpdateDelegate(_tracer_ItemAddedEvent);
                        _itemKeeperSink.ItemsFilteredEvent += new TracerItemKeeperSink.TracerUpdateDelegate(_itemKeeperSink_ItemsFilteredEvent);
                        _itemKeeperSink.FilterUpdateEvent += new TracerItemSink.SinkUpdateDelegate(_itemSink_FilterUpdateEvent);

                        _methodFilter = new MethodTracerFilter();
                        _stringFilter = new StringTracerFilter();
                        _typeFilter = new TypeTracerFilter();
                        _priorityFilter = new PriorityFilter();

                        _itemKeeperSink.AddFilter(_methodFilter);
                        _itemKeeperSink.AddFilter(_stringFilter);
                        _itemKeeperSink.AddFilter(_typeFilter);
                        _itemKeeperSink.AddFilter(_priorityFilter);

                        // Attach positive filter property string data source.
                        _searchingMatchStrip.SetDataSource(_stringFilter,
                            _stringFilter.GetType().GetProperty("PositiveFilterString"));
                        
                        // Attach negative string filter property as data source.
                        _viewExcludeStrip.SetDataSource(_stringFilter, 
                            _stringFilter.GetType().GetProperty("NegativeFilterStrings"));

                        methodTracerFilterControl1.Filter = _methodFilter;
                        typeTracerFilterControl1.Filter = _typeFilter;
                    }

                }

                UpdateUI();
                WinFormsHelper.DirectOrManagedInvoke(this, UpdateFiltersUI);
            }
        }

        /// <summary>
        /// Is the method filter control visible.
        /// </summary>
        public bool ShowMethodFilter
        {
            get { return this.methodTracerFilterControl1.Visible; }
            set { this.methodTracerFilterControl1.Visible = value; }
        }

        /// <summary>
        /// Is the item details (message) pane visible.
        /// </summary>
        public bool ShowDetails
        {
            get { return this.panelSelected.Visible; }
            set { panelSelected.Visible = value; }
        }

        ///// <summary>
        ///// Is the control auto updating upon receiving new messages.
        ///// </summary>
        //public bool AutoUpdate
        //{
        //    get { return toolStripButtonAutoUpdate.Checked; }
        //    set { toolStripButtonAutoUpdate.Checked = true; }
        //}

        /// <summary>
        /// Is the detailed properties of the selected item visibile.
        /// </summary>
        public bool DetailsVisible
        {
            get { return this.propertyGridItem.Visible; }
            set { propertyGridItem.Visible = value; }
        }

        StringsControlToolStripEx _inputExcludeStrip = null;
        StringsControlToolStripEx _viewExcludeStrip = null;
        StringsControlToolStripEx _markingMatchStrip = null;
        StringsControlToolStripEx _searchingMatchStrip = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TracerControl()
        {
            InitializeComponent();
            _tracer = new Tracer();

            listView.VirtualItemsSelectionRangeChanged += new ListViewVirtualItemsSelectionRangeChangedEventHandler(listView_VirtualItemsSelectionRangeChanged);

            lock (listView)
            {
                listView.AdvancedColumnManagementUnsafe.Add(0, new VirtualListViewEx.ColumnManagementInfo() { AutoResizeMode = ColumnHeaderAutoResizeStyle.ColumnContent });
                listView.AdvancedColumnManagementUnsafe.Add(1, new VirtualListViewEx.ColumnManagementInfo() { FillWhiteSpace = true });
            }

            // Search items.
            _searchingMatchStrip = new StringsControlToolStripEx();
            _searchingMatchStrip.Label = "Search";
            // Data source assigned on tracer assignment.
            //toolStripFilters.Items.Add(new ToolStripSeparator());
            WinFormsHelper.MoveToolStripItems(_searchingMatchStrip, toolStripFilters);

            // Mark items.
            _markingMatchStrip = new StringsControlToolStripEx();
            _markingMatchStrip.Label = "Mark";
            _markingMatchStrip.SetDataSource(this, this.GetType().GetProperty("MarkingMatch"));
            toolStripFilters.Items.Add(new ToolStripSeparator()); 
            WinFormsHelper.MoveToolStripItems(_markingMatchStrip, toolStripFilters);

            // View exclude filtering...
            _viewExcludeStrip = new StringsControlToolStripEx();
            _viewExcludeStrip.Label = "Exclude";
            WinFormsHelper.MoveToolStripItems(_viewExcludeStrip, toolStripFilters);

            // Input filtering...
            ToolStripLabel label = new ToolStripLabel("Filter");
            label.ForeColor = SystemColors.GrayText;
            toolStripFilters.Items.Add(new ToolStripSeparator());
            toolStripFilters.Items.Add(label);

            // Input exlude items.
            _inputExcludeStrip = new StringsControlToolStripEx();
            _inputExcludeStrip.Label = "Exclude";
            // Data source assigned on tracer assignment.
            toolStripFilters.Items.Add(new ToolStripSeparator());
            WinFormsHelper.MoveToolStripItems(_inputExcludeStrip, toolStripFilters);
        }

        private void TracerControl_Load(object sender, EventArgs e)
        {
            toolStripButtonDetails.Checked = false;
            toolStripButtonAutoScroll.Checked = listView.AutoScroll;

            toolStripComboBoxPriority.DropDownItems.Add("All").Click += new EventHandler(TracerControlPriorityItem_Click);
            toolStripComboBoxPriority.DropDownItems.Add(new ToolStripSeparator());

            foreach (string name in Enum.GetNames(typeof(CommonSupport.Tracer.TimeDisplayFormatEnum)))
            {
                toolStripDropDownButtonTimeDisplay.DropDownItems.Add(name).Tag = Enum.Parse(typeof(CommonSupport.Tracer.TimeDisplayFormatEnum), name);
            }

            foreach (string name in Enum.GetNames(typeof(TracerItem.PriorityEnum)))
            {
                ToolStripItem item = toolStripComboBoxPriority.DropDownItems.Add(name + " and above");
                item.Tag = Enum.Parse(typeof(TracerItem.PriorityEnum), name);
                item.Click += new EventHandler(TracerControlPriorityItem_Click);
            }

        }

        /// <summary>
        /// Save state.
        /// </summary>
        /// <param name="state"></param>
        public void SaveState(SerializationInfoEx state)
        {
            if (_stringFilter != null)
            {
                state.AddValue("_stringFilter", _stringFilter);
            }

            if (_stringInputFilter != null)
            {
                state.AddValue("_stringInputFilter", _stringInputFilter);
            }

        }

        /// <summary>
        /// Load state.
        /// </summary>
        /// <param name="state"></param>
        public void LoadState(SerializationInfoEx state)
        {
            if (state.ContainsValue("_stringFilter"))
            {
                StringTracerFilter stringFilter = state.GetValue<StringTracerFilter>("_stringFilter");
                if (_stringFilter != null)
                {
                    _stringFilter.CopyDataFrom(stringFilter);
                }
            }

            if (state.ContainsValue("_stringInputFilter"))
            {
                StringTracerFilter stringInputFilter = state.GetValue<StringTracerFilter>("_stringInputFilter");
                if (_stringInputFilter != null)
                {
                    _stringInputFilter.CopyDataFrom(stringInputFilter);
                }
            }

            WinFormsHelper.DirectOrManagedInvoke(this, UpdateFiltersUI);
        }

        void TracerControlPriorityItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            PriorityFilter filter = _priorityFilter;
            if (filter == null)
            {
                return;
            }
            
            if (item.Tag != null)
            {
                filter.MinimumPriority = (TracerItem.PriorityEnum)item.Tag;
            }
            else
            {
                filter.MinimumPriority = TracerItem.PriorityEnum.Trivial;
            }

            UpdateUI();
        }


        void _itemSink_FilterUpdateEvent(TracerItemSink tracer)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, DoUpdateUI);
        }

        const int CleanVirtualItemsCount = 3;

        /// <summary>
        /// 
        /// </summary>
        public void UpdateUI()
        {
            WinFormsHelper.DirectOrManagedInvoke(this, DoUpdateUI);
        }

        void UpdateFiltersUI()
        {
            //if (_stringFilter != null && toolStripTextBoxSearch.Text != _stringFilter.PositiveFilterString)
            //{
            //    toolStripTextBoxSearch.Text = _stringFilter.PositiveFilterString;
            //}
            //else
            //{
            //    toolStripTextBoxSearch.Text = string.Empty;
            //}

            //string excludeText = string.Empty;
            //if (_stringFilter != null && _stringFilter.NegativeFilterStrings != null)
            //{
            //    excludeText = GeneralHelper.ToString(_stringFilter.NegativeFilterStrings, Separator.ToString());
            //}

            //if (toolStripTextBoxExclude.Text != excludeText)
            //{
            //    toolStripTextBoxExclude.Text = excludeText;
            //}

            //excludeText = string.Empty;
            //if (_stringInputFilter != null && _stringInputFilter.NegativeFilterStrings != null)
            //{
            //    excludeText = GeneralHelper.ToString(_stringInputFilter.NegativeFilterStrings, Separator.ToString());
            //}

            _inputExcludeStrip.LoadValues();
            _viewExcludeStrip.LoadValues();
            //_markingMatchStrip.LoadValues();
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void DoUpdateUI()
        {
            try
            {
                if (this.Tracer == null || _itemKeeperSink == null || this.DesignMode)
                {
                    return;
                }

                if (_priorityFilter == null || _priorityFilter.MinimumPriority == TracerItem.PriorityEnum.Minimum)
                {
                    toolStripComboBoxPriority.Text = "Priority [All]";
                }
                else
                {
                    toolStripComboBoxPriority.Text = "Priority [+" + GeneralHelper.SplitCapitalLetters(_priorityFilter.MinimumPriority.ToString()) + "]";
                }

                toolStripDropDownButtonTimeDisplay.Text = "Time [" + GeneralHelper.SplitCapitalLetters(_tracer.TimeDisplayFormat.ToString()) + "]";
                this.toolStripButtonEnabled.Checked = this.Tracer.Enabled;

                // Give some slack for the vlist, since it has problems due to Microsoft List implementation.
                listView.VirtualListSize = _itemKeeperSink.FilteredItemsCount + CleanVirtualItemsCount;
                
            }
            catch (Exception ex)
            {
                SystemMonitor.Error("UI Logic Error [" + ex.Message + "]");
            }
        }

        void listView_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            if (_itemKeeperSink != null)
            {
                _itemKeeperSink.ReFilterItems();
            }

            //DoUpdateUI();
        }

        private void listViewMain_Resize(object sender, EventArgs e)
        {
            this.listView.Columns[listView.Columns.Count - 1].Width = -2;
            this.listView.Invalidate();
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            this.Tracer.Clear(false);

            //_mode = ModeEnum.Default;
            DoUpdateUI();
        }

        private void listViewMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            TracerItem item = null;
            if (_itemKeeperSink != null && listView.SelectedIndices.Count > 0)
            {
                int index = listView.SelectedIndices[0];

                if (index < listView.VirtualListSize - CleanVirtualItemsCount)
                {
                    item = _itemKeeperSink.GetFilteredItem(index);
                }
            }

            LoadTracerItem(item);
        }

        protected void LoadTracerItem(TracerItem item)
        {
            if (item == null)
            {
                textBoxSelectedItemMessage.Text = string.Empty;
            }
            else
            {
                textBoxSelectedItemMessage.Text = item.PrintMessage();
            }
            this.propertyGridItem.SelectedObject = item;
        }

        //private void toolStripTextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        toolStripButtonSearch_Click(sender, EventArgs.Empty);
        //    }
        //}

        //private void toolStripButtonSearchClear_Click(object sender, EventArgs e)
        //{
        //    toolStripTextBoxSearch.Text = "";
        //    toolStripButtonSearch_Click(sender, e);
        //}

        //private void toolStripButtonSearch_Click(object sender, EventArgs e)
        //{
        //    _stringFilter.PositiveFilterString = toolStripTextBoxSearch.Text;
        //}

        protected Color GetPriorityColor(TracerItem.PriorityEnum color)
        {
            switch (color)
            {
                case TracerItem.PriorityEnum.Trivial:
                case TracerItem.PriorityEnum.VeryLow:
                case TracerItem.PriorityEnum.Low:
                case TracerItem.PriorityEnum.Medium:
                    return Color.Transparent;

                case TracerItem.PriorityEnum.High:
                    return Color.MistyRose;
                case TracerItem.PriorityEnum.VeryHigh:
                    return Color.LightSalmon;
                case TracerItem.PriorityEnum.Critical:
                    return Color.Red;
            }

            return Color.Transparent;
        }

        private void listView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem();

            TracerItem tracerItem = null;

            // If we are in the last items, make sure to always leave them blank.
            if (e.ItemIndex <= listView.VirtualListSize - CleanVirtualItemsCount)
            {
                if (_itemKeeperSink != null)
                {
                    tracerItem = _itemKeeperSink.GetFilteredItem(e.ItemIndex);
                }
            }

            if (tracerItem == null)
            {
                e.Item.SubItems.Clear();
                for (int i = 0; i < listView.Columns.Count; i++)
                {
                    e.Item.SubItems.Add(string.Empty);
                }
                return;
            }

            switch (tracerItem.FullType)
            {
                case TracerItem.TypeEnum.MethodEntry:
                    e.Item.ImageIndex = 3;
                    break;
                case TracerItem.TypeEnum.MethodExit:
                    e.Item.ImageIndex = 4;
                    break;
                case TracerItem.TypeEnum.Trace:
                    e.Item.ImageIndex = 0;
                    break;
                case TracerItem.TypeEnum.System:
                    e.Item.ImageIndex = 6;
                    break;
                case TracerItem.TypeEnum.Warning:
                case (TracerItem.TypeEnum.Warning | TracerItem.TypeEnum.Operation):
                    e.Item.ImageIndex = 5;
                    break;
                case (TracerItem.TypeEnum.Error | TracerItem.TypeEnum.Operation):
                case TracerItem.TypeEnum.Error:
                    e.Item.ImageIndex = 2;
                    break;
                default:
                    // If there are only items with no images, the image column width gets
                    // substraced from the Column.0 width and this causes a bug.
                    e.Item.ImageIndex = 0;
                    break;
            }

            if (e.Item.UseItemStyleForSubItems)
            {
                e.Item.UseItemStyleForSubItems = false;
            }

            Color color = GetPriorityColor(tracerItem.Priority);
            if (color != e.Item.SubItems[0].BackColor)
            {
                e.Item.SubItems[0].BackColor = color;
            }

            string day = tracerItem.DateTime.Day.ToString();
            if (tracerItem.DateTime.Day == 1)
            {
                day += "st";
            }
            else if (tracerItem.DateTime.Day == 2)
            {
                day += "nd";
            }
            else if (tracerItem.DateTime.Day == 3)
            {
                day += "rd";
            }
            else
            {
                day += "th";
            }

            string time = string.Empty;
            Tracer tracer = _tracer;
            if (tracer != null)
            {
                if (tracer.TimeDisplayFormat == CommonSupport.Tracer.TimeDisplayFormatEnum.ApplicationTicks)
                {// Application time.
                    time = Math.Round(((decimal)tracerItem.ApplicationTick / (decimal)Stopwatch.Frequency), 6).ToString();
                }
                else if (tracer.TimeDisplayFormat == CommonSupport.Tracer.TimeDisplayFormatEnum.DateTime)
                {// Date time conventional.
                    time = day + tracerItem.DateTime.ToString(", HH:mm:ss:ffff");
                }
                else
                {// Combined.
                    time = day + tracerItem.DateTime.ToString(", HH:mm:ss:ffff");
                    time += " | " + Math.Round(((decimal)tracerItem.ApplicationTick / (decimal)Stopwatch.Frequency), 6).ToString();
                }
            }

            e.Item.Text = tracerItem.Index + ", " + time;

            e.Item.SubItems.Add(tracerItem.PrintMessage());

            if (string.IsNullOrEmpty(_markingMatch) == false)
            {
                if (StringTracerFilter.FilterItem(tracerItem, _markingMatch, null))
                {
                    e.Item.BackColor = Color.PowderBlue;
                }
            }
        }

        void _tracer_ItemAddedEvent(TracerItemKeeperSink sink, TracerItem item)
        {
            if (toolStripButtonAutoUpdate.Checked)
            {// Only updating on new items added, when auto update is enabled.
                _itemsModified = true;
            }
        }

        void _itemKeeperSink_ItemsFilteredEvent(TracerItemKeeperSink tracer)
        {
            _itemsModified = true;
        }

        private void toolStripButtonAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            //timerUpdate.Enabled = toolStripButtonAutoUpdate.Checked;
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (_itemsModified)
            {
                _itemsModified = false;
                DoUpdateUI();
            }
            else
            {
                listView.UpdateAutoScrollPosition();
            }

            //TracerHelper.TraceEntry();
        }

        private void toolStripButtonEnabled_CheckStateChanged(object sender, EventArgs e)
        {
            if (Tracer == null)
            {
                return;
            }

            this.Tracer.Enabled = toolStripButtonEnabled.Checked;
        }

        private void markAllFromThisMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0 || _itemKeeperSink == null)
            {
                return;
            }

            MethodTracerItem item = null;
            item = _itemKeeperSink.GetFilteredItem(listView.SelectedIndices[0]) as MethodTracerItem;

            if (item != null)
            {
                MarkingMatch = item.MethodBase.DeclaringType.Name + "." + item.MethodBase.Name;
            }
        }

        private void markAllFromThisClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0 || _itemKeeperSink == null)
            {
                return;
            }

            MethodTracerItem item = null;
            item = _itemKeeperSink.GetFilteredItem(listView.SelectedIndices[0]) as MethodTracerItem;

            if (item != null)
            {
                MarkingMatch = item.MethodBase.DeclaringType.Module.Name.Substring(0, item.MethodBase.DeclaringType.Module.Name.LastIndexOf(".")) + "." + item.MethodBase.DeclaringType.Name;
            }
        }

        private void markAllFromThisModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0 || _itemKeeperSink == null)
            {
                return;
            }

            MethodTracerItem item = _itemKeeperSink.GetFilteredItem(listView.SelectedIndices[0]) as MethodTracerItem;
            if (item != null)
            {
                MarkingMatch = item.MethodBase.DeclaringType.Module.Name.Substring(0, item.MethodBase.DeclaringType.Module.Name.LastIndexOf(".")) + ".";
            }
        }

        private void ofThisMethodToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0 || _itemKeeperSink == null)
            {
                return;
            }

            MethodTracerItem item = _itemKeeperSink.GetFilteredItem(listView.SelectedIndices[0]) as MethodTracerItem;

            if (item != null)
            {
                _stringFilter.PositiveFilterString = item.MethodBase.DeclaringType.Name + "." + item.MethodBase.Name;
                //toolStripTextBoxSearch.Text = item.MethodBase.DeclaringType.Name + "." + item.MethodBase.Name;
            }

            //toolStripButtonSearch_Click(sender, e);
        }

        private void ofThisClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0 || _itemKeeperSink == null)
            {
                return;
            }

            MethodTracerItem item = _itemKeeperSink.GetFilteredItem(listView.SelectedIndices[0]) as MethodTracerItem;
            if (item != null)
            {
                _stringFilter.PositiveFilterString = item.MethodBase.DeclaringType.Module.Name.Substring(0, item.MethodBase.DeclaringType.Module.Name.LastIndexOf(".")) + "." + item.MethodBase.DeclaringType.Name;
                //toolStripTextBoxSearch.Text = item.MethodBase.DeclaringType.Module.Name.Substring(0, item.MethodBase.DeclaringType.Module.Name.LastIndexOf(".")) + "." + item.MethodBase.DeclaringType.Name;
            }

            //toolStripButtonSearch_Click(sender, e);
        }

        private void ofThisModuleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0 || _itemKeeperSink == null)
            {
                return;
            }

            MethodTracerItem item = _itemKeeperSink.GetFilteredItem(listView.SelectedIndices[0]) as MethodTracerItem;

            if (item != null)
            {
                _stringFilter.PositiveFilterString = "[" + item.MethodBase.DeclaringType.Module.Name.Substring(0, item.MethodBase.DeclaringType.Module.Name.LastIndexOf(".")) + ".";
                //toolStripTextBoxSearch.Text = "[" + item.MethodBase.DeclaringType.Module.Name.Substring(0, item.MethodBase.DeclaringType.Module.Name.LastIndexOf(".")) + ".";
            }

            //toolStripButtonSearch_Click(sender, e);
        }

        private void toolStripButtonAutoScroll_CheckedChanged(object sender, EventArgs e)
        {
            this.listView.AutoScroll = toolStripButtonAutoScroll.Checked;
        }

        private void toolStripButtonDetails_CheckedChanged(object sender, EventArgs e)
        {
            panelSelected.Visible = toolStripButtonDetails.Checked;
            splitterDetails.Visible = panelSelected.Visible;
        }

        private void toolStripDropDownButtonTimeDisplay_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Tracer tracer = _tracer;
            if (tracer != null)
            {
                tracer.TimeDisplayFormat = (CommonSupport.Tracer.TimeDisplayFormatEnum)e.ClickedItem.Tag;
            }

            UpdateUI();
        }

    }
}
