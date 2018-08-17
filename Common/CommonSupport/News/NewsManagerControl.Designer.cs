using System.Windows.Forms;
namespace CommonSupport
{
    partial class NewsManagerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required baseMethod for Designer support - do not modify 
        /// the contents of this baseMethod with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewsManagerControl));
            this.toolStripLabelSources = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabelUpdating = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonUpdate = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDetails = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMarkRead = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMarkAllRead = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButtonShowMode = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRead = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonChannel = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonSource = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripTextBoxSearch = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMark = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSearchClear = new System.Windows.Forms.ToolStripButton();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView = new CommonSupport.VirtualListViewEx();
            this.columnHeaderGeneral = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTitle = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDateTime = new System.Windows.Forms.ColumnHeader();
            this.newsItemControl1 = new CommonSupport.NewsItemControl();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelSources
            // 
            this.toolStripLabelSources.Name = "toolStripLabelSources";
            this.toolStripLabelSources.Size = new System.Drawing.Size(53, 22);
            this.toolStripLabelSources.Text = "Feeds [0]";
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelSources,
            this.toolStripLabelUpdating,
            toolStripSeparator1,
            this.toolStripButtonUpdate,
            toolStripSeparator4,
            this.toolStripButtonDetails,
            toolStripSeparator6,
            this.toolStripButtonMarkRead,
            this.toolStripButtonMarkAllRead,
            toolStripSeparator3,
            this.toolStripDropDownButtonShowMode,
            toolStripSeparator2,
            this.toolStripDropDownButtonChannel,
            this.toolStripDropDownButtonSource,
            this.toolStripButtonSettings,
            toolStripSeparator5,
            this.toolStripTextBoxSearch,
            this.toolStripButtonSearch,
            this.toolStripButtonMark,
            this.toolStripButtonSearchClear});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(802, 25);
            this.toolStrip.TabIndex = 17;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripLabelUpdating
            // 
            this.toolStripLabelUpdating.AutoSize = false;
            this.toolStripLabelUpdating.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripLabelUpdating.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLabelUpdating.Image")));
            this.toolStripLabelUpdating.Name = "toolStripLabelUpdating";
            this.toolStripLabelUpdating.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabelUpdating.ToolTipText = "Updating";
            // 
            // toolStripButtonUpdate
            // 
            this.toolStripButtonUpdate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUpdate.Image")));
            this.toolStripButtonUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUpdate.Name = "toolStripButtonUpdate";
            this.toolStripButtonUpdate.Size = new System.Drawing.Size(62, 22);
            this.toolStripButtonUpdate.Text = "Update";
            this.toolStripButtonUpdate.Click += new System.EventHandler(this.toolStripButtonUpdate_Click);
            // 
            // toolStripButtonDetails
            // 
            this.toolStripButtonDetails.Checked = true;
            this.toolStripButtonDetails.CheckOnClick = true;
            this.toolStripButtonDetails.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonDetails.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDetails.Name = "toolStripButtonDetails";
            this.toolStripButtonDetails.Size = new System.Drawing.Size(72, 22);
            this.toolStripButtonDetails.Text = "Show Details";
            this.toolStripButtonDetails.Click += new System.EventHandler(this.toolStripButtonDetails_Click);
            // 
            // toolStripButtonMarkRead
            // 
            this.toolStripButtonMarkRead.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMarkRead.Image")));
            this.toolStripButtonMarkRead.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMarkRead.Name = "toolStripButtonMarkRead";
            this.toolStripButtonMarkRead.Size = new System.Drawing.Size(78, 22);
            this.toolStripButtonMarkRead.Text = "Mark Read";
            this.toolStripButtonMarkRead.ToolTipText = "Mark Selected Items as Read";
            this.toolStripButtonMarkRead.Visible = false;
            this.toolStripButtonMarkRead.Click += new System.EventHandler(this.toolStripButtonMarkRead_Click);
            // 
            // toolStripButtonMarkAllRead
            // 
            this.toolStripButtonMarkAllRead.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMarkAllRead.Name = "toolStripButtonMarkAllRead";
            this.toolStripButtonMarkAllRead.Size = new System.Drawing.Size(76, 22);
            this.toolStripButtonMarkAllRead.Text = "Mark All Read";
            this.toolStripButtonMarkAllRead.ToolTipText = "Mark All Items as Read";
            this.toolStripButtonMarkAllRead.Click += new System.EventHandler(this.toolStripButtonMarkAllRead_Click);
            // 
            // toolStripDropDownButtonShowMode
            // 
            this.toolStripDropDownButtonShowMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator8,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItemRead});
            this.toolStripDropDownButtonShowMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonShowMode.Name = "toolStripDropDownButtonShowMode";
            this.toolStripDropDownButtonShowMode.Size = new System.Drawing.Size(42, 22);
            this.toolStripDropDownButtonShowMode.Text = "View";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem1.Text = "Default";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.buttonShowModeDefault_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(128, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem2.Image")));
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem2.Text = "Favourite";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.buttonShowModeFavourite_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem3.Image")));
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem3.Text = "Deleted";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.buttonShowModeDeleted_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem4.Image")));
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem4.Text = "Unread";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.buttonShowModeUnread_Click);
            // 
            // toolStripMenuItemRead
            // 
            this.toolStripMenuItemRead.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemRead.Image")));
            this.toolStripMenuItemRead.Name = "toolStripMenuItemRead";
            this.toolStripMenuItemRead.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItemRead.Text = "Read";
            this.toolStripMenuItemRead.Click += new System.EventHandler(this.buttonShowModeRead_Click);
            // 
            // toolStripDropDownButtonChannel
            // 
            this.toolStripDropDownButtonChannel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonChannel.Name = "toolStripDropDownButtonChannel";
            this.toolStripDropDownButtonChannel.Size = new System.Drawing.Size(59, 22);
            this.toolStripDropDownButtonChannel.Text = "Channel";
            this.toolStripDropDownButtonChannel.Visible = false;
            this.toolStripDropDownButtonChannel.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripDropDownButtonChannel_DropDownItemClicked);
            this.toolStripDropDownButtonChannel.DropDownOpening += new System.EventHandler(this.toolStripDropDownButtonChannel_DropDownOpening);
            // 
            // toolStripDropDownButtonSource
            // 
            this.toolStripDropDownButtonSource.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonSource.Image")));
            this.toolStripDropDownButtonSource.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonSource.Name = "toolStripDropDownButtonSource";
            this.toolStripDropDownButtonSource.Size = new System.Drawing.Size(60, 22);
            this.toolStripDropDownButtonSource.Text = "Feed";
            this.toolStripDropDownButtonSource.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripDropDownButtonSource_DropDownItemClicked);
            this.toolStripDropDownButtonSource.DropDownOpening += new System.EventHandler(this.toolStripDropDownButtonSource_DropDownOpening);
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSettings.Image")));
            this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSettings.Name = "toolStripButtonSettings";
            this.toolStripButtonSettings.Size = new System.Drawing.Size(66, 22);
            this.toolStripButtonSettings.Text = "Settings";
            this.toolStripButtonSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripButtonSettings.Click += new System.EventHandler(this.toolStripButtonSettings_Click);
            // 
            // toolStripTextBoxSearch
            // 
            this.toolStripTextBoxSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolStripTextBoxSearch.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStripTextBoxSearch.Name = "toolStripTextBoxSearch";
            this.toolStripTextBoxSearch.Size = new System.Drawing.Size(140, 25);
            this.toolStripTextBoxSearch.ToolTipText = "Enter string to search or mark";
            this.toolStripTextBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBoxSearch_KeyDown);
            // 
            // toolStripButtonSearch
            // 
            this.toolStripButtonSearch.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSearch.Image")));
            this.toolStripButtonSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearch.Name = "toolStripButtonSearch";
            this.toolStripButtonSearch.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonSearch.Text = "Search";
            this.toolStripButtonSearch.Click += new System.EventHandler(this.toolStripButtonSearch_Click);
            // 
            // toolStripButtonMark
            // 
            this.toolStripButtonMark.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMark.Image")));
            this.toolStripButtonMark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMark.Name = "toolStripButtonMark";
            this.toolStripButtonMark.Size = new System.Drawing.Size(50, 22);
            this.toolStripButtonMark.Text = "Mark";
            this.toolStripButtonMark.Click += new System.EventHandler(this.toolStripButtonMark_Click);
            // 
            // toolStripButtonSearchClear
            // 
            this.toolStripButtonSearchClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSearchClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSearchClear.Image")));
            this.toolStripButtonSearchClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearchClear.Name = "toolStripButtonSearchClear";
            this.toolStripButtonSearchClear.Size = new System.Drawing.Size(36, 22);
            this.toolStripButtonSearchClear.Text = "Clear";
            this.toolStripButtonSearchClear.ToolTipText = "Clear Search or Mark Results";
            this.toolStripButtonSearchClear.Click += new System.EventHandler(this.toolStripButtonSearchClear_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "square.filled.lightred.png");
            this.imageList.Images.SetKeyName(1, "square.filled.red.png");
            this.imageList.Images.SetKeyName(2, "rssfeed-icon-14x14.png");
            this.imageList.Images.SetKeyName(3, "star_yellow.png");
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 464);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(802, 4);
            this.splitter1.TabIndex = 20;
            this.splitter1.TabStop = false;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem7,
            this.toolStripSeparator9,
            this.toolStripMenuItem6,
            this.toolStripSeparator7,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(152, 104);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem5.Image")));
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem5.Text = "Favourite";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem7.Image")));
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem7.Text = "Not Favourite";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(148, 6);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem6.Image")));
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem6.Text = "Mark as Read";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripButtonMarkRead_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(148, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // listView
            // 
            this.listView.AutoScroll = false;
            this.listView.AutoScrollSlack = 3;
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderGeneral,
            this.columnHeaderTitle,
            this.columnHeaderDateTime});
            this.listView.ContextMenuStrip = this.contextMenuStrip;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView.ForeColor = System.Drawing.Color.MidnightBlue;
            this.listView.FullRowSelect = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 25);
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(802, 439);
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 22;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.VirtualMode = true;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listViewItems_SelectedIndexChanged);
            this.listView.DoubleClick += new System.EventHandler(this.listViewItems_DoubleClick);
            this.listView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.objectListView_RetrieveVirtualItem);
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            // 
            // columnHeaderGeneral
            // 
            this.columnHeaderGeneral.Text = "";
            this.columnHeaderGeneral.Width = 35;
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "Title";
            this.columnHeaderTitle.Width = 602;
            // 
            // columnHeaderDateTime
            // 
            this.columnHeaderDateTime.Text = "Date";
            // 
            // newsItemControl1
            // 
            this.newsItemControl1.BackColor = System.Drawing.SystemColors.Window;
            this.newsItemControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.newsItemControl1.Location = new System.Drawing.Point(0, 468);
            this.newsItemControl1.Margin = new System.Windows.Forms.Padding(2);
            this.newsItemControl1.Name = "newsItemControl1";
            this.newsItemControl1.NewsItem = null;
            this.newsItemControl1.Size = new System.Drawing.Size(802, 134);
            this.newsItemControl1.TabIndex = 21;
            // 
            // NewsManagerControl
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.newsItemControl1);
            this.Name = "NewsManagerControl";
            this.Size = new System.Drawing.Size(802, 602);
            this.Load += new System.EventHandler(this.OnLoadEvent);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabelUpdating;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonSource;
        private System.Windows.Forms.ToolStripLabel toolStripLabelSources;
        private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton toolStripButtonMarkAllRead;
        //private NewsItemControl newsItemControl1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxSearch;
        private System.Windows.Forms.ToolStripButton toolStripButtonSearch;
        private System.Windows.Forms.ToolStripButton toolStripButtonSearchClear;
        private NewsItemControl newsItemControl1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonChannel;
        private System.Windows.Forms.ToolStripButton toolStripButtonMark;
        private VirtualListViewEx listView;
        private ColumnHeader columnHeaderGeneral;
        private ColumnHeader columnHeaderTitle;
        private ColumnHeader columnHeaderDateTime;
        private ToolStripButton toolStripButtonUpdate;
        private ToolStripButton toolStripButtonMarkRead;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripButton toolStripButtonDetails;
        private ToolStripDropDownButton toolStripDropDownButtonShowMode;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItemRead;

    }
}
