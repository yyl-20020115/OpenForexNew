namespace ForexPlatformFrontEnd
{
    partial class PositionsControl
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Orders", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("History", System.Windows.Forms.HorizontalAlignment.Left);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader16 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader20 = new System.Windows.Forms.ColumnHeader();
            this.listContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.submitOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.closePositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripLabelAccount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxAccount = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxSymbol = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonAddSymbol = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonNewOrder = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClose = new System.Windows.Forms.ToolStripButton();
            this.removeFromListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.listContextMenuStrip.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listView
            // 
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18,
            this.columnHeader19,
            this.columnHeader20});
            this.listView.ContextMenuStrip = this.listContextMenuStrip;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            listViewGroup1.Header = "Orders";
            listViewGroup1.Name = "Orders";
            listViewGroup2.Header = "History";
            listViewGroup2.Name = "History";
            this.listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 25);
            this.listView.Margin = new System.Windows.Forms.Padding(0);
            this.listView.MinimumSize = new System.Drawing.Size(450, 22);
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(798, 361);
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 25;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Symbol";
            this.columnHeader13.Width = 100;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Volume";
            this.columnHeader14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader14.Width = 80;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Mkt. Value";
            this.columnHeader15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader15.Width = 80;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Pending";
            this.columnHeader16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader16.Width = 80;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Base Price";
            this.columnHeader17.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader17.Width = 90;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Open Result";
            this.columnHeader18.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader18.Width = 90;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Closed Result";
            this.columnHeader19.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader19.Width = 90;
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "Price";
            this.columnHeader20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader20.Width = 120;
            // 
            // listContextMenuStrip
            // 
            this.listContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.submitOrderToolStripMenuItem,
            this.toolStripSeparator3,
            this.closePositionToolStripMenuItem,
            this.toolStripSeparator4,
            this.removeFromListToolStripMenuItem});
            this.listContextMenuStrip.Name = "listContextMenuStrip";
            this.listContextMenuStrip.Size = new System.Drawing.Size(169, 104);
            this.listContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.listContextMenuStrip_Opening);
            // 
            // submitOrderToolStripMenuItem
            // 
            this.submitOrderToolStripMenuItem.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD;
            this.submitOrderToolStripMenuItem.Name = "submitOrderToolStripMenuItem";
            this.submitOrderToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.submitOrderToolStripMenuItem.Text = "Submit Order";
            this.submitOrderToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonNewOrder_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(165, 6);
            // 
            // closePositionToolStripMenuItem
            // 
            this.closePositionToolStripMenuItem.Image = global::ForexPlatformFrontEnd.Properties.Resources.DELETE;
            this.closePositionToolStripMenuItem.Name = "closePositionToolStripMenuItem";
            this.closePositionToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.closePositionToolStripMenuItem.Text = "Close Position";
            this.closePositionToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonClose_Click);
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelAccount,
            this.toolStripComboBoxAccount,
            toolStripSeparator6,
            this.toolStripLabel1,
            this.toolStripTextBoxSymbol,
            this.toolStripButtonAddSymbol,
            this.toolStripSeparator2,
            this.toolStripButtonNewOrder,
            this.toolStripButtonClose,
            toolStripSeparator1});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(798, 25);
            this.toolStripMain.TabIndex = 26;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // toolStripLabelAccount
            // 
            this.toolStripLabelAccount.Enabled = false;
            this.toolStripLabelAccount.Name = "toolStripLabelAccount";
            this.toolStripLabelAccount.Size = new System.Drawing.Size(46, 22);
            this.toolStripLabelAccount.Text = "Account";
            // 
            // toolStripComboBoxAccount
            // 
            this.toolStripComboBoxAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxAccount.DropDownWidth = 450;
            this.toolStripComboBoxAccount.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.toolStripComboBoxAccount.Name = "toolStripComboBoxAccount";
            this.toolStripComboBoxAccount.Size = new System.Drawing.Size(240, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(41, 22);
            this.toolStripLabel1.Text = "Symbol";
            // 
            // toolStripTextBoxSymbol
            // 
            this.toolStripTextBoxSymbol.Name = "toolStripTextBoxSymbol";
            this.toolStripTextBoxSymbol.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripButtonAddSymbol
            // 
            this.toolStripButtonAddSymbol.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD2;
            this.toolStripButtonAddSymbol.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddSymbol.Name = "toolStripButtonAddSymbol";
            this.toolStripButtonAddSymbol.Size = new System.Drawing.Size(46, 22);
            this.toolStripButtonAddSymbol.Text = "Add";
            this.toolStripButtonAddSymbol.ToolTipText = "Add New Position Symbol";
            this.toolStripButtonAddSymbol.Click += new System.EventHandler(this.toolStripButtonAddSymbol_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonNewOrder
            // 
            this.toolStripButtonNewOrder.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD;
            this.toolStripButtonNewOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNewOrder.Name = "toolStripButtonNewOrder";
            this.toolStripButtonNewOrder.Size = new System.Drawing.Size(130, 22);
            this.toolStripButtonNewOrder.Text = "Submit Position Order";
            this.toolStripButtonNewOrder.Click += new System.EventHandler(this.toolStripButtonNewOrder_Click);
            // 
            // toolStripButtonClose
            // 
            this.toolStripButtonClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonClose.Image = global::ForexPlatformFrontEnd.Properties.Resources.DELETE;
            this.toolStripButtonClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClose.Name = "toolStripButtonClose";
            this.toolStripButtonClose.Size = new System.Drawing.Size(93, 22);
            this.toolStripButtonClose.Text = "Close Position";
            this.toolStripButtonClose.Click += new System.EventHandler(this.toolStripButtonClose_Click);
            // 
            // removeFromListToolStripMenuItem
            // 
            this.removeFromListToolStripMenuItem.Name = "removeFromListToolStripMenuItem";
            this.removeFromListToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.removeFromListToolStripMenuItem.Text = "Remove from List";
            this.removeFromListToolStripMenuItem.ToolTipText = "Remove position from list (it will auto re-appear if orders were placed to it).";
            this.removeFromListToolStripMenuItem.Click += new System.EventHandler(this.removeFromListToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(165, 6);
            // 
            // PositionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStripMain);
            this.Name = "PositionsControl";
            this.Size = new System.Drawing.Size(798, 386);
            this.listContextMenuStrip.ResumeLayout(false);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.ColumnHeader columnHeader20;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripLabel toolStripLabelAccount;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxAccount;
        private System.Windows.Forms.ToolStripButton toolStripButtonNewOrder;
        private System.Windows.Forms.ToolStripButton toolStripButtonClose;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxSymbol;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddSymbol;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ContextMenuStrip listContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem submitOrderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem closePositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem removeFromListToolStripMenuItem;
    }
}
