using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CommonSupport
{
    /// <summary>
    /// 
    /// </summary>
    public class TracerStatusStripOperator
    {
        System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelReportsLink;
        System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelWarningsLink;
        System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelErrorsLink;
        System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSystemReport;
        System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelReport;

        ToolStripSeparator separator1;
        ToolStripSeparator separator2;

        Tracer _tracer;
        Timer _timer;

        const int MaxPendingItems = 5;

        List<TracerItem> _pendingMessageItems = new List<TracerItem>();
        TimeSpan _previewStayOnInterval = TimeSpan.FromSeconds(7.5);

        DateTime _lastShow = DateTime.Now;

        TracerItem _itemShown = null;

        StatusStrip _targetStatusStrip;

        TracerItemKeeperSink _itemKeeperSink = null;

        Control _ownerControl = null;

        volatile TracerItem.TypeEnum _filterItemType = TracerItem.TypeEnum.Error | TracerItem.TypeEnum.Warning;
        /// <summary>
        /// 
        /// </summary>
        public TracerItem.TypeEnum FilterItemType
        {
            get { return _filterItemType; }
            set { _filterItemType = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TracerStatusStripOperator()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load(Control ownerControl, Timer timer, Tracer tracer, StatusStrip targetStatusStrip)
        {
            _timer = timer;
            _timer.Tick += new EventHandler(_timer_Tick);
            
            _tracer = tracer;
            _tracer.ItemAddedEvent += new Tracer.ItemUpdateDelegate(_tracer_ItemAddedEvent);

            _itemKeeperSink = (TracerItemKeeperSink)_tracer.GetSinkByType(typeof(TracerItemKeeperSink));

            _targetStatusStrip = targetStatusStrip;

            _ownerControl = ownerControl;

            separator1 = new ToolStripSeparator();
            separator2 = new ToolStripSeparator();

            this.toolStripStatusLabelSystemReport = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelReport = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelReportsLink = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelWarningsLink = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelErrorsLink = new System.Windows.Forms.ToolStripStatusLabel();

            // 
            // toolStripStatusLabelSystemReport
            // 
            this.toolStripStatusLabelSystemReport.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.toolStripStatusLabelSystemReport.Name = "toolStripStatusLabelSystemReport";
            this.toolStripStatusLabelSystemReport.Size = new System.Drawing.Size(78, 17);
            this.toolStripStatusLabelSystemReport.Text = "System Report";
            // 
            // toolStripStatusLabelReport
            // 
            //this.toolStripStatusLabelReport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabelReport.Image")));
            this.toolStripStatusLabelReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripStatusLabelReport.IsLink = false;
            this.toolStripStatusLabelReport.LinkColor = System.Drawing.SystemColors.HighlightText;
            this.toolStripStatusLabelReport.Name = "toolStripStatusLabelReport";
            this.toolStripStatusLabelReport.Size = new System.Drawing.Size(127, 17);
            this.toolStripStatusLabelReport.Spring = true;
            this.toolStripStatusLabelReport.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabelReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelReportsLink
            // 
            this.toolStripStatusLabelReportsLink.IsLink = false;
            this.toolStripStatusLabelReportsLink.LinkColor = System.Drawing.SystemColors.HighlightText;
            this.toolStripStatusLabelReportsLink.Name = "toolStripStatusLabelReportsLink";
            this.toolStripStatusLabelReportsLink.Size = new System.Drawing.Size(54, 17);
            this.toolStripStatusLabelReportsLink.Text = "0 Reports";
            // 
            // toolStripStatusLabelWarningsLink
            // 
            this.toolStripStatusLabelWarningsLink.IsLink = false;
            this.toolStripStatusLabelWarningsLink.LinkColor = System.Drawing.SystemColors.HighlightText;
            this.toolStripStatusLabelWarningsLink.Name = "toolStripStatusLabelWarningsLink";
            this.toolStripStatusLabelWarningsLink.Size = new System.Drawing.Size(61, 17);
            this.toolStripStatusLabelWarningsLink.Text = "0 Warnings";
            // 
            // toolStripStatusLabelErrorsLink
            // 
            this.toolStripStatusLabelErrorsLink.IsLink = false;
            this.toolStripStatusLabelErrorsLink.LinkColor = System.Drawing.SystemColors.HighlightText;
            this.toolStripStatusLabelErrorsLink.Name = "toolStripStatusLabelErrorsLink";
            this.toolStripStatusLabelErrorsLink.Size = new System.Drawing.Size(45, 17);
            this.toolStripStatusLabelErrorsLink.Text = "0 Errors";

            targetStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] 
            {
//                separator1,
                this.toolStripStatusLabelSystemReport,
                this.toolStripStatusLabelReport,
                separator2,
                this.toolStripStatusLabelErrorsLink,
                this.toolStripStatusLabelWarningsLink,
                this.toolStripStatusLabelReportsLink,
            });

            ShowItem(null);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnLoad()
        {
            if (_timer != null)
            {
                _timer.Tick -= new EventHandler(_timer_Tick);
                _timer = null;
            }

            if (_tracer != null)
            {
                _tracer.ItemAddedEvent -= new Tracer.ItemUpdateDelegate(_tracer_ItemAddedEvent);
                _tracer = null;
            }

            if (_targetStatusStrip != null)
            {
                _targetStatusStrip.Items.Remove(separator1);
                _targetStatusStrip.Items.Remove(separator2);
                _targetStatusStrip.Items.Remove(this.toolStripStatusLabelSystemReport);
                _targetStatusStrip.Items.Remove(this.toolStripStatusLabelReport);
                _targetStatusStrip.Items.Remove(this.toolStripStatusLabelReportsLink);
                _targetStatusStrip.Items.Remove(this.toolStripStatusLabelWarningsLink);
                _targetStatusStrip.Items.Remove(this.toolStripStatusLabelErrorsLink);

                _targetStatusStrip = null;
            }

            _ownerControl = null;
            _itemKeeperSink = null;
            
            lock (this)
            {
                _pendingMessageItems.Clear();
            }
        }

        /// <summary>
        /// Helper, shows an item to constrol.
        /// Always call on UI thread.
        /// </summary>
        void ShowItem(TracerItem item)
        {
            _itemShown = item;

            if (item == null)
            {
                this.toolStripStatusLabelReport.Text = string.Empty;
            }
            else
            {
                if ((item.FullType & TracerItem.TypeEnum.Error) != 0)
                {
                    toolStripStatusLabelReport.ForeColor = Color.DarkRed;
                }
                else if ((item.FullType & TracerItem.TypeEnum.Warning) != 0)
                {
                    toolStripStatusLabelReport.ForeColor = Color.DarkBlue;
                }
                else
                {
                    toolStripStatusLabelReport.ForeColor = SystemColors.ControlText;
                }

                _lastShow = DateTime.Now;
                this.toolStripStatusLabelReport.Text = "[" + _pendingMessageItems.Count.ToString() + "] " + item.PrintMessage();
            }
        }

        void _tracer_ItemAddedEvent(Tracer tracer, TracerItem item)
        {
            if ((item.FullType & _filterItemType) != 0)
            {
                lock (this)
                {
                    if (_pendingMessageItems.Count > MaxPendingItems)
                    {
                        _pendingMessageItems.RemoveAt(0);
                    }

                    _pendingMessageItems.Add(item);
                }
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if ((_pendingMessageItems.Count > 0 || _itemShown != null) 
                && (DateTime.Now - _lastShow) > _previewStayOnInterval)
            {
                if (_itemShown == null)
                {
                    if (_pendingMessageItems.Count > 0)
                    {
                        TracerItem item = null;
                        lock (this)
                        {
                            item = _pendingMessageItems[0];
                            _pendingMessageItems.Remove(item);
                        }

                        ShowItem(item);
                    }
                }
                else
                {
                    ShowItem(null);
                }
            }

            if (_itemKeeperSink != null)
            {
                string errorsText = string.Empty, warningsText = string.Empty, reportsText = string.Empty;
                lock (_itemKeeperSink)
                {
                    errorsText = "Errors " + (_itemKeeperSink.GetItemsByTypeCount(TracerItem.TypeEnum.Error)).ToString();
                    warningsText = "Warnings " + (_itemKeeperSink.GetItemsByTypeCount(TracerItem.TypeEnum.Warning)).ToString();

                    reportsText = "Reports " + (_itemKeeperSink.FilteredItemsCount).ToString();
                }

                if (toolStripStatusLabelErrorsLink.Text != errorsText)
                {
                    toolStripStatusLabelErrorsLink.Text = errorsText;
                }

                if (toolStripStatusLabelWarningsLink.Text != warningsText)
                {
                    toolStripStatusLabelWarningsLink.Text = warningsText;
                }

                if (toolStripStatusLabelReportsLink.Text != reportsText)
                {
                    toolStripStatusLabelReportsLink.Text = reportsText;
                }
            }
        }
    }
}
