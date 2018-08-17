using CommonSupport;
namespace ForexPlatformFrontEnd
{
    partial class DataStoreManagementComponentControl
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
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Local", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Download", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataStoreManagementComponentControl));
            this.contextMenuStripEntries = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewProviderEntries = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.toolStripLocalEntry = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRemoveEntry = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButtonImportData = new System.Windows.Forms.ToolStripDropDownButton();
            this.fromOnlineSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonShowEntryData = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStripDataImporter = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.tabControlImporters = new System.Windows.Forms.TabControl();
            this.toolStripButtonAddImporter = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStripEntries.SuspendLayout();
            this.toolStripLocalEntry.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStripDataImporter.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // contextMenuStripEntries
            // 
            this.contextMenuStripEntries.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripSeparator2,
            this.removeToolStripMenuItem});
            this.contextMenuStripEntries.Name = "contextMenuStripEntries";
            this.contextMenuStripEntries.Size = new System.Drawing.Size(125, 54);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Image = global::ForexPlatformFrontEnd.Properties.Resources.CHART;
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.showToolStripMenuItem.Text = "Preview";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonShowEntryData_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(121, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::ForexPlatformFrontEnd.Properties.Resources.DELETE;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonRemoveEntry_Click);
            // 
            // listViewProviderEntries
            // 
            this.listViewProviderEntries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewProviderEntries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader10,
            this.columnHeader9});
            this.listViewProviderEntries.ContextMenuStrip = this.contextMenuStripEntries;
            this.listViewProviderEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewProviderEntries.FullRowSelect = true;
            this.listViewProviderEntries.GridLines = true;
            listViewGroup3.Header = "Local";
            listViewGroup3.Name = "Local";
            listViewGroup4.Header = "Download";
            listViewGroup4.Name = "Download";
            this.listViewProviderEntries.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            this.listViewProviderEntries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewProviderEntries.Location = new System.Drawing.Point(0, 25);
            this.listViewProviderEntries.Name = "listViewProviderEntries";
            this.listViewProviderEntries.Size = new System.Drawing.Size(800, 245);
            this.listViewProviderEntries.TabIndex = 3;
            this.listViewProviderEntries.UseCompatibleStateImageBehavior = false;
            this.listViewProviderEntries.View = System.Windows.Forms.View.Details;
            this.listViewProviderEntries.SizeChanged += new System.EventHandler(this.listViewProviderEntries_SizeChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Local Entry";
            this.columnHeader3.Width = 250;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Period";
            this.columnHeader4.Width = 90;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Start";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "End";
            this.columnHeader2.Width = 140;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Count";
            this.columnHeader10.Width = 80;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Description";
            this.columnHeader9.Width = 190;
            // 
            // toolStripLocalEntry
            // 
            this.toolStripLocalEntry.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripLocalEntry.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRemoveEntry,
            this.toolStripDropDownButtonImportData,
            toolStripSeparator1,
            this.toolStripButtonShowEntryData});
            this.toolStripLocalEntry.Location = new System.Drawing.Point(0, 0);
            this.toolStripLocalEntry.Name = "toolStripLocalEntry";
            this.toolStripLocalEntry.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripLocalEntry.Size = new System.Drawing.Size(800, 25);
            this.toolStripLocalEntry.TabIndex = 4;
            this.toolStripLocalEntry.Text = "toolStrip1";
            // 
            // toolStripButtonRemoveEntry
            // 
            this.toolStripButtonRemoveEntry.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonRemoveEntry.Image = global::ForexPlatformFrontEnd.Properties.Resources.DELETE;
            this.toolStripButtonRemoveEntry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveEntry.Name = "toolStripButtonRemoveEntry";
            this.toolStripButtonRemoveEntry.Size = new System.Drawing.Size(66, 22);
            this.toolStripButtonRemoveEntry.Text = "Remove";
            this.toolStripButtonRemoveEntry.ToolTipText = "Remove local entry and delete its data.";
            this.toolStripButtonRemoveEntry.Click += new System.EventHandler(this.toolStripButtonRemoveEntry_Click);
            // 
            // toolStripDropDownButtonImportData
            // 
            this.toolStripDropDownButtonImportData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromOnlineSourceToolStripMenuItem,
            this.toolStripSeparator3,
            this.toolStripMenuItemFromFile});
            this.toolStripDropDownButtonImportData.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonImportData.Image")));
            this.toolStripDropDownButtonImportData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonImportData.Name = "toolStripDropDownButtonImportData";
            this.toolStripDropDownButtonImportData.Size = new System.Drawing.Size(68, 22);
            this.toolStripDropDownButtonImportData.Text = "Import";
            // 
            // fromOnlineSourceToolStripMenuItem
            // 
            this.fromOnlineSourceToolStripMenuItem.Enabled = false;
            this.fromOnlineSourceToolStripMenuItem.Name = "fromOnlineSourceToolStripMenuItem";
            this.fromOnlineSourceToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.fromOnlineSourceToolStripMenuItem.Text = "From Online Source(s)...";
            this.fromOnlineSourceToolStripMenuItem.Click += new System.EventHandler(this.fromOnlineSourceToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(200, 6);
            // 
            // toolStripMenuItemFromFile
            // 
            this.toolStripMenuItemFromFile.Name = "toolStripMenuItemFromFile";
            this.toolStripMenuItemFromFile.Size = new System.Drawing.Size(203, 22);
            this.toolStripMenuItemFromFile.Text = "From Local File(s)...";
            this.toolStripMenuItemFromFile.Click += new System.EventHandler(this.toolStripMenuItemFromFile_Click);
            // 
            // toolStripButtonShowEntryData
            // 
            this.toolStripButtonShowEntryData.Image = global::ForexPlatformFrontEnd.Properties.Resources.CHART;
            this.toolStripButtonShowEntryData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowEntryData.Name = "toolStripButtonShowEntryData";
            this.toolStripButtonShowEntryData.Size = new System.Drawing.Size(65, 22);
            this.toolStripButtonShowEntryData.Text = "Preview";
            this.toolStripButtonShowEntryData.ToolTipText = "Preview entry data in a popup window.";
            this.toolStripButtonShowEntryData.Click += new System.EventHandler(this.toolStripButtonShowEntryData_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewProviderEntries);
            this.splitContainer1.Panel1.Controls.Add(this.toolStripLocalEntry);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlImporters);
            this.splitContainer1.Panel2.Controls.Add(this.toolStripDataImporter);
            this.splitContainer1.Size = new System.Drawing.Size(800, 600);
            this.splitContainer1.SplitterDistance = 270;
            this.splitContainer1.TabIndex = 5;
            // 
            // toolStripDataImporter
            // 
            this.toolStripDataImporter.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripDataImporter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripLabel1,
            this.toolStripComboBox1,
            this.toolStripButtonAddImporter});
            this.toolStripDataImporter.Location = new System.Drawing.Point(0, 0);
            this.toolStripDataImporter.Name = "toolStripDataImporter";
            this.toolStripDataImporter.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripDataImporter.Size = new System.Drawing.Size(800, 25);
            this.toolStripDataImporter.TabIndex = 5;
            this.toolStripDataImporter.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::ForexPlatformFrontEnd.Properties.Resources.button_cancel_12;
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Remove";
            this.toolStripButton1.ToolTipText = "Remove local entry and delete its data.";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Enabled = false;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(49, 22);
            this.toolStripLabel1.Text = "Importer";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(160, 25);
            // 
            // tabControlImporters
            // 
            this.tabControlImporters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlImporters.Location = new System.Drawing.Point(0, 25);
            this.tabControlImporters.Name = "tabControlImporters";
            this.tabControlImporters.SelectedIndex = 0;
            this.tabControlImporters.Size = new System.Drawing.Size(800, 301);
            this.tabControlImporters.TabIndex = 6;
            // 
            // toolStripButtonAddImporter
            // 
            this.toolStripButtonAddImporter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddImporter.Image")));
            this.toolStripButtonAddImporter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddImporter.Name = "toolStripButtonAddImporter";
            this.toolStripButtonAddImporter.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonAddImporter.Text = "Create";
            // 
            // DataStoreControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.ImageName = "DATA.PNG";
            this.Name = "DataStoreControl";
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.contextMenuStripEntries.ResumeLayout(false);
            this.toolStripLocalEntry.ResumeLayout(false);
            this.toolStripLocalEntry.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.toolStripDataImporter.ResumeLayout(false);
            this.toolStripDataImporter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewProviderEntries;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ToolStrip toolStripLocalEntry;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveEntry;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowEntryData;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEntries;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonImportData;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFromFile;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem fromOnlineSourceToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStripDataImporter;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.TabControl tabControlImporters;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddImporter;
    }
}
