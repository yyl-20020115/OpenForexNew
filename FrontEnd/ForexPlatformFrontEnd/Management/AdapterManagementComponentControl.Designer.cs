namespace ForexPlatformFrontEnd
{
    partial class AdapterManagementComponentControl
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdapterManagementComponentControl));
            this.groupBoxAdapters = new System.Windows.Forms.GroupBox();
            this.listViewIntegrations = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.toolStripIntegrations = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRemoveAdapter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAdapterProperties = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStartAdapter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonStopAdapter = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxAdapterType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonCreate = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.groupBoxAdapters.SuspendLayout();
            this.toolStripIntegrations.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // groupBoxAdapters
            // 
            this.groupBoxAdapters.Controls.Add(this.listViewIntegrations);
            this.groupBoxAdapters.Controls.Add(this.toolStripIntegrations);
            this.groupBoxAdapters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxAdapters.Location = new System.Drawing.Point(0, 31);
            this.groupBoxAdapters.Name = "groupBoxAdapters";
            this.groupBoxAdapters.Size = new System.Drawing.Size(800, 489);
            this.groupBoxAdapters.TabIndex = 19;
            this.groupBoxAdapters.TabStop = false;
            this.groupBoxAdapters.Text = "Adapters";
            // 
            // listViewIntegrations
            // 
            this.listViewIntegrations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewIntegrations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewIntegrations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewIntegrations.FullRowSelect = true;
            this.listViewIntegrations.GridLines = true;
            this.listViewIntegrations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewIntegrations.Location = new System.Drawing.Point(3, 41);
            this.listViewIntegrations.Name = "listViewIntegrations";
            this.listViewIntegrations.Size = new System.Drawing.Size(794, 445);
            this.listViewIntegrations.TabIndex = 20;
            this.listViewIntegrations.UseCompatibleStateImageBehavior = false;
            this.listViewIntegrations.View = System.Windows.Forms.View.Details;
            this.listViewIntegrations.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewIntegrations_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Started";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Status";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 634;
            // 
            // toolStripIntegrations
            // 
            this.toolStripIntegrations.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripIntegrations.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripIntegrations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRemoveAdapter,
            this.toolStripButtonAdapterProperties,
            this.toolStripButtonStartAdapter,
            this.toolStripSeparator1,
            this.toolStripButtonStopAdapter});
            this.toolStripIntegrations.Location = new System.Drawing.Point(3, 16);
            this.toolStripIntegrations.Name = "toolStripIntegrations";
            this.toolStripIntegrations.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripIntegrations.Size = new System.Drawing.Size(794, 25);
            this.toolStripIntegrations.TabIndex = 18;
            this.toolStripIntegrations.Text = "toolStrip1";
            // 
            // toolStripButtonRemoveAdapter
            // 
            this.toolStripButtonRemoveAdapter.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonRemoveAdapter.Image = global::ForexPlatformFrontEnd.Properties.Resources.DELETE2;
            this.toolStripButtonRemoveAdapter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveAdapter.Name = "toolStripButtonRemoveAdapter";
            this.toolStripButtonRemoveAdapter.Size = new System.Drawing.Size(66, 22);
            this.toolStripButtonRemoveAdapter.Text = "Remove";
            this.toolStripButtonRemoveAdapter.ToolTipText = "Stop and remove selected adapter.";
            this.toolStripButtonRemoveAdapter.Click += new System.EventHandler(this.toolStripButtonRemoveAdapter_Click);
            // 
            // toolStripButtonAdapterProperties
            // 
            this.toolStripButtonAdapterProperties.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonAdapterProperties.Image = global::ForexPlatformFrontEnd.Properties.Resources.address_book;
            this.toolStripButtonAdapterProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdapterProperties.Name = "toolStripButtonAdapterProperties";
            this.toolStripButtonAdapterProperties.Size = new System.Drawing.Size(76, 22);
            this.toolStripButtonAdapterProperties.Text = "Properties";
            this.toolStripButtonAdapterProperties.ToolTipText = "Show properties for selected adapter(s).";
            this.toolStripButtonAdapterProperties.Click += new System.EventHandler(this.toolStripButtonAdapterProperties_Click);
            // 
            // toolStripButtonStartAdapter
            // 
            this.toolStripButtonStartAdapter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStartAdapter.Image")));
            this.toolStripButtonStartAdapter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStartAdapter.Name = "toolStripButtonStartAdapter";
            this.toolStripButtonStartAdapter.Size = new System.Drawing.Size(51, 22);
            this.toolStripButtonStartAdapter.Text = "Start";
            this.toolStripButtonStartAdapter.ToolTipText = "Start adapter.";
            this.toolStripButtonStartAdapter.Click += new System.EventHandler(this.toolStripButtonStartAdapter_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonStopAdapter
            // 
            this.toolStripButtonStopAdapter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStopAdapter.Image")));
            this.toolStripButtonStopAdapter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStopAdapter.Name = "toolStripButtonStopAdapter";
            this.toolStripButtonStopAdapter.Size = new System.Drawing.Size(49, 22);
            this.toolStripButtonStopAdapter.Text = "Stop";
            this.toolStripButtonStopAdapter.ToolTipText = "Stop adapter.";
            this.toolStripButtonStopAdapter.Click += new System.EventHandler(this.toolStripButtonStopAdapter_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(800, 6);
            this.splitter1.TabIndex = 22;
            this.splitter1.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            toolStripSeparator5,
            this.toolStripButtonRefresh,
            toolStripSeparator4,
            this.toolStripLabel2,
            this.toolStripComboBoxAdapterType,
            this.toolStripButtonCreate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 24;
            this.toolStrip1.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Enabled = false;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(82, 22);
            this.toolStripLabel1.Text = "Create Adapter";
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(49, 22);
            this.toolStripButtonRefresh.Text = "Refresh";
            this.toolStripButtonRefresh.ToolTipText = "Connect to Selected Addresses";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Enabled = false;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(31, 22);
            this.toolStripLabel2.Text = "Type";
            // 
            // toolStripComboBoxAdapterType
            // 
            this.toolStripComboBoxAdapterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxAdapterType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripComboBoxAdapterType.Name = "toolStripComboBoxAdapterType";
            this.toolStripComboBoxAdapterType.Size = new System.Drawing.Size(180, 25);
            // 
            // toolStripButtonCreate
            // 
            this.toolStripButtonCreate.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD2;
            this.toolStripButtonCreate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreate.Name = "toolStripButtonCreate";
            this.toolStripButtonCreate.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonCreate.Text = "Create";
            this.toolStripButtonCreate.ToolTipText = "Add New Address";
            this.toolStripButtonCreate.Click += new System.EventHandler(this.toolStripButtonCreate_Click);
            // 
            // AdapterManagementComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxAdapters);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.toolStrip1);
            this.ImageName = "GEARS.PNG";
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "AdapterManagementComponentControl";
            this.Size = new System.Drawing.Size(800, 520);
            this.Load += new System.EventHandler(this.AdapterManagementOperatorControl_Load);
            this.groupBoxAdapters.ResumeLayout(false);
            this.groupBoxAdapters.PerformLayout();
            this.toolStripIntegrations.ResumeLayout(false);
            this.toolStripIntegrations.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAdapters;
        private System.Windows.Forms.ToolStrip toolStripIntegrations;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveAdapter;
        private System.Windows.Forms.ListView listViewIntegrations;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreate;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxAdapterType;
        private System.Windows.Forms.ToolStripButton toolStripButtonStartAdapter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonStopAdapter;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdapterProperties;
    }
}
