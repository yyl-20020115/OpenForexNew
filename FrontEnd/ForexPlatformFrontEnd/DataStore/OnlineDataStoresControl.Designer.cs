namespace ForexPlatformFrontEnd
{
    partial class OnlineDataStoresControl
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Local", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Download", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnlineDataStoresControl));
            this.listViewOnlineEntries = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.toolStripOnline = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripDropDownButtonDownloadToDataStore = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDownloadToFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripOnline.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // listViewOnlineEntries
            // 
            this.listViewOnlineEntries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewOnlineEntries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader7});
            this.listViewOnlineEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewOnlineEntries.FullRowSelect = true;
            this.listViewOnlineEntries.GridLines = true;
            listViewGroup1.Header = "Local";
            listViewGroup1.Name = "Local";
            listViewGroup2.Header = "Download";
            listViewGroup2.Name = "Download";
            this.listViewOnlineEntries.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.listViewOnlineEntries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewOnlineEntries.Location = new System.Drawing.Point(0, 25);
            this.listViewOnlineEntries.Name = "listViewOnlineEntries";
            this.listViewOnlineEntries.Size = new System.Drawing.Size(640, 433);
            this.listViewOnlineEntries.TabIndex = 12;
            this.listViewOnlineEntries.UseCompatibleStateImageBehavior = false;
            this.listViewOnlineEntries.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Online Entry";
            this.columnHeader5.Width = 210;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Period";
            this.columnHeader6.Width = 90;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "File Name";
            this.columnHeader13.Width = 120;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "File Size";
            this.columnHeader14.Width = 120;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Download %";
            this.columnHeader7.Width = 90;
            // 
            // toolStripOnline
            // 
            this.toolStripOnline.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripOnline.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonDownloadToDataStore,
            toolStripSeparator4,
            this.toolStripButtonDownloadToFile});
            this.toolStripOnline.Location = new System.Drawing.Point(0, 0);
            this.toolStripOnline.Name = "toolStripOnline";
            this.toolStripOnline.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripOnline.Size = new System.Drawing.Size(640, 25);
            this.toolStripOnline.TabIndex = 13;
            this.toolStripOnline.Text = "toolStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 458);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(640, 22);
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripDropDownButtonDownloadToDataStore
            // 
            this.toolStripDropDownButtonDownloadToDataStore.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonDownloadToDataStore.Image")));
            this.toolStripDropDownButtonDownloadToDataStore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonDownloadToDataStore.Name = "toolStripDropDownButtonDownloadToDataStore";
            this.toolStripDropDownButtonDownloadToDataStore.Size = new System.Drawing.Size(127, 22);
            this.toolStripDropDownButtonDownloadToDataStore.Text = "Import to Data Store";
            this.toolStripDropDownButtonDownloadToDataStore.ToolTipText = "Import entry to local store.";
            this.toolStripDropDownButtonDownloadToDataStore.Click += new System.EventHandler(this.toolStripDropDownButtonDownloadToDataStore_Click);
            // 
            // toolStripButtonDownloadToFile
            // 
            this.toolStripButtonDownloadToFile.Image = global::ForexPlatformFrontEnd.Properties.Resources.disk_blue;
            this.toolStripButtonDownloadToFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDownloadToFile.Name = "toolStripButtonDownloadToFile";
            this.toolStripButtonDownloadToFile.Size = new System.Drawing.Size(118, 22);
            this.toolStripButtonDownloadToFile.Text = "Download to File...";
            this.toolStripButtonDownloadToFile.Click += new System.EventHandler(this.toolStripButtonDownloadToFile_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Image = global::ForexPlatformFrontEnd.Properties.Resources.STOP;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(38, 17);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // OnlineDataStoresControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewOnlineEntries);
            this.Controls.Add(this.toolStripOnline);
            this.Controls.Add(this.statusStrip1);
            this.Name = "OnlineDataStoresControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.toolStripOnline.ResumeLayout(false);
            this.toolStripOnline.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewOnlineEntries;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ToolStrip toolStripOnline;
        private System.Windows.Forms.ToolStripButton toolStripDropDownButtonDownloadToDataStore;
        private System.Windows.Forms.ToolStripButton toolStripButtonDownloadToFile;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;

    }
}
