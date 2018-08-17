using System.Windows.Forms;
namespace ForexPlatformFrontEnd
{
    partial class ExpertManagementControl
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
            System.Windows.Forms.Splitter splitter2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            System.Windows.Forms.Splitter splitter3;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpertManagementControl));
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Imported", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Local", System.Windows.Forms.HorizontalAlignment.Left);
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonExpertList = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonImportedAssemblies = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonImportAssembly = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelRemove = new System.Windows.Forms.ToolStripLabel();
            this.listViewExperts = new System.Windows.Forms.ListView();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.panelExperts = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNewExpert = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteExpert = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButtonRunExpert = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItemRunStandalone = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonEditExpert = new System.Windows.Forms.ToolStripButton();
            this.virtualListViewExAssemblies = new CommonSupport.VirtualListViewEx();
            this.columnHeaderDll = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStripAssemblies = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlExperts = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolStripButtonSourceFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            splitter2 = new System.Windows.Forms.Splitter();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            splitter3 = new System.Windows.Forms.Splitter();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
            this.panelExperts.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStripAssemblies.SuspendLayout();
            this.tabControlExperts.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // splitter2
            // 
            splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            splitter2.Location = new System.Drawing.Point(0, 445);
            splitter2.Name = "splitter2";
            splitter2.Size = new System.Drawing.Size(271, 10);
            splitter2.TabIndex = 6;
            splitter2.TabStop = false;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // splitter3
            // 
            splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            splitter3.Location = new System.Drawing.Point(0, 480);
            splitter3.Name = "splitter3";
            splitter3.Size = new System.Drawing.Size(800, 8);
            splitter3.TabIndex = 6;
            splitter3.TabStop = false;
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
            // splitterLeft
            // 
            this.splitterLeft.Location = new System.Drawing.Point(271, 25);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(6, 455);
            this.splitterLeft.TabIndex = 2;
            this.splitterLeft.TabStop = false;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonExpertList,
            toolStripSeparator3,
            this.toolStripButtonImportedAssemblies,
            toolStripSeparator1,
            this.toolStripButtonImportAssembly,
            this.toolStripLabelRemove,
            this.toolStripSeparator2,
            this.toolStripButtonSourceFile});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(800, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButtonExpertList
            // 
            this.toolStripButtonExpertList.Checked = true;
            this.toolStripButtonExpertList.CheckOnClick = true;
            this.toolStripButtonExpertList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonExpertList.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExpertList.Image")));
            this.toolStripButtonExpertList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExpertList.Name = "toolStripButtonExpertList";
            this.toolStripButtonExpertList.Size = new System.Drawing.Size(64, 22);
            this.toolStripButtonExpertList.Text = "Experts";
            this.toolStripButtonExpertList.CheckStateChanged += new System.EventHandler(this.toolStripButtonExpertList_CheckStateChanged);
            // 
            // toolStripButtonImportedAssemblies
            // 
            this.toolStripButtonImportedAssemblies.Checked = true;
            this.toolStripButtonImportedAssemblies.CheckOnClick = true;
            this.toolStripButtonImportedAssemblies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonImportedAssemblies.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonImportedAssemblies.Image")));
            this.toolStripButtonImportedAssemblies.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonImportedAssemblies.Name = "toolStripButtonImportedAssemblies";
            this.toolStripButtonImportedAssemblies.Size = new System.Drawing.Size(126, 22);
            this.toolStripButtonImportedAssemblies.Text = "Imported Assemblies";
            this.toolStripButtonImportedAssemblies.CheckStateChanged += new System.EventHandler(this.toolStripButtonImportedDlls_CheckStateChanged);
            // 
            // toolStripButtonImportAssembly
            // 
            this.toolStripButtonImportAssembly.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonImportAssembly.Image")));
            this.toolStripButtonImportAssembly.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonImportAssembly.Name = "toolStripButtonImportAssembly";
            this.toolStripButtonImportAssembly.Size = new System.Drawing.Size(107, 22);
            this.toolStripButtonImportAssembly.Text = "Import Assembly";
            this.toolStripButtonImportAssembly.Click += new System.EventHandler(this.toolStripButtonImportAssembly_Click);
            // 
            // toolStripLabelRemove
            // 
            this.toolStripLabelRemove.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabelRemove.Font = new System.Drawing.Font("Tahoma", 4.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStripLabelRemove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLabelRemove.Image")));
            this.toolStripLabelRemove.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripLabelRemove.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.toolStripLabelRemove.Name = "toolStripLabelRemove";
            this.toolStripLabelRemove.Size = new System.Drawing.Size(18, 22);
            this.toolStripLabelRemove.Text = " ";
            this.toolStripLabelRemove.ToolTipText = "Close tab";
            this.toolStripLabelRemove.MouseEnter += new System.EventHandler(this.toolStripLabelRemove_MouseEnter);
            this.toolStripLabelRemove.MouseLeave += new System.EventHandler(this.toolStripLabelRemove_MouseLeave);
            this.toolStripLabelRemove.Click += new System.EventHandler(this.toolStripLabelRemove_Click);
            // 
            // listViewExperts
            // 
            this.listViewExperts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewExperts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName});
            this.listViewExperts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewExperts.FullRowSelect = true;
            listViewGroup3.Header = "Imported";
            listViewGroup3.Name = "Imported";
            listViewGroup4.Header = "Local";
            listViewGroup4.Name = "Local";
            this.listViewExperts.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            this.listViewExperts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewExperts.HideSelection = false;
            this.listViewExperts.Location = new System.Drawing.Point(0, 25);
            this.listViewExperts.MultiSelect = false;
            this.listViewExperts.Name = "listViewExperts";
            this.listViewExperts.Size = new System.Drawing.Size(271, 420);
            this.listViewExperts.TabIndex = 3;
            this.listViewExperts.UseCompatibleStateImageBehavior = false;
            this.listViewExperts.View = System.Windows.Forms.View.Details;
            this.listViewExperts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewExperts_MouseDoubleClick);
            this.listViewExperts.SelectedIndexChanged += new System.EventHandler(this.virtualListViewExExperts_SelectedIndexChanged);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Experts";
            this.columnHeaderName.Width = 200;
            // 
            // panelExperts
            // 
            this.panelExperts.BackColor = System.Drawing.SystemColors.Control;
            this.panelExperts.Controls.Add(this.listViewExperts);
            this.panelExperts.Controls.Add(this.toolStrip1);
            this.panelExperts.Controls.Add(splitter2);
            this.panelExperts.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelExperts.Location = new System.Drawing.Point(0, 25);
            this.panelExperts.Name = "panelExperts";
            this.panelExperts.Size = new System.Drawing.Size(271, 455);
            this.panelExperts.TabIndex = 4;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNewExpert,
            this.toolStripButtonDeleteExpert,
            toolStripSeparator5,
            this.toolStripDropDownButtonRunExpert,
            toolStripSeparator6,
            this.toolStripButtonEditExpert});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(271, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonNewExpert
            // 
            this.toolStripButtonNewExpert.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD;
            this.toolStripButtonNewExpert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNewExpert.Name = "toolStripButtonNewExpert";
            this.toolStripButtonNewExpert.Size = new System.Drawing.Size(48, 22);
            this.toolStripButtonNewExpert.Text = "New";
            this.toolStripButtonNewExpert.Click += new System.EventHandler(this.toolStripButtonNewExpert_Click);
            // 
            // toolStripButtonDeleteExpert
            // 
            this.toolStripButtonDeleteExpert.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonDeleteExpert.Enabled = false;
            this.toolStripButtonDeleteExpert.Image = global::ForexPlatformFrontEnd.Properties.Resources.DELETE2;
            this.toolStripButtonDeleteExpert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteExpert.Name = "toolStripButtonDeleteExpert";
            this.toolStripButtonDeleteExpert.Size = new System.Drawing.Size(58, 22);
            this.toolStripButtonDeleteExpert.Text = "Delete";
            this.toolStripButtonDeleteExpert.Click += new System.EventHandler(this.toolStripButtonDeleteExpert_Click);
            // 
            // toolStripDropDownButtonRunExpert
            // 
            this.toolStripDropDownButtonRunExpert.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRunStandalone});
            this.toolStripDropDownButtonRunExpert.Enabled = false;
            this.toolStripDropDownButtonRunExpert.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonRunExpert.Image")));
            this.toolStripDropDownButtonRunExpert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonRunExpert.Name = "toolStripDropDownButtonRunExpert";
            this.toolStripDropDownButtonRunExpert.Size = new System.Drawing.Size(55, 22);
            this.toolStripDropDownButtonRunExpert.Text = "Run";
            // 
            // toolStripMenuItemRunStandalone
            // 
            this.toolStripMenuItemRunStandalone.Name = "toolStripMenuItemRunStandalone";
            this.toolStripMenuItemRunStandalone.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItemRunStandalone.Text = "Run Standalone";
            this.toolStripMenuItemRunStandalone.Click += new System.EventHandler(this.runStandaloneToolStripMenuItem_Click);
            // 
            // toolStripButtonEditExpert
            // 
            this.toolStripButtonEditExpert.Enabled = false;
            this.toolStripButtonEditExpert.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEditExpert.Image")));
            this.toolStripButtonEditExpert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditExpert.Name = "toolStripButtonEditExpert";
            this.toolStripButtonEditExpert.Size = new System.Drawing.Size(45, 22);
            this.toolStripButtonEditExpert.Text = "Edit";
            this.toolStripButtonEditExpert.Click += new System.EventHandler(this.toolStripButtonEdit_Click);
            // 
            // virtualListViewExAssemblies
            // 
            this.virtualListViewExAssemblies.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.virtualListViewExAssemblies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderDll});
            this.virtualListViewExAssemblies.ContextMenuStrip = this.contextMenuStripAssemblies;
            this.virtualListViewExAssemblies.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.virtualListViewExAssemblies.FullRowSelect = true;
            this.virtualListViewExAssemblies.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.virtualListViewExAssemblies.Location = new System.Drawing.Point(0, 488);
            this.virtualListViewExAssemblies.Name = "virtualListViewExAssemblies";
            this.virtualListViewExAssemblies.Size = new System.Drawing.Size(800, 112);
            this.virtualListViewExAssemblies.TabIndex = 5;
            this.virtualListViewExAssemblies.UseCompatibleStateImageBehavior = false;
            this.virtualListViewExAssemblies.View = System.Windows.Forms.View.Details;
            this.virtualListViewExAssemblies.VirtualMode = true;
            this.virtualListViewExAssemblies.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.virtualListViewExDlls_RetrieveVirtualItem);
            this.virtualListViewExAssemblies.KeyDown += new System.Windows.Forms.KeyEventHandler(this.virtualListViewExAssemblies_KeyDown);
            // 
            // columnHeaderDll
            // 
            this.columnHeaderDll.Text = "Imported Assemblies";
            this.columnHeaderDll.Width = 120;
            // 
            // contextMenuStripAssemblies
            // 
            this.contextMenuStripAssemblies.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.contextMenuStripAssemblies.Name = "contextMenuStripAssemblies";
            this.contextMenuStripAssemblies.Size = new System.Drawing.Size(125, 26);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // tabControlExperts
            // 
            this.tabControlExperts.Controls.Add(this.tabPage1);
            this.tabControlExperts.Controls.Add(this.tabPage2);
            this.tabControlExperts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlExperts.Location = new System.Drawing.Point(277, 25);
            this.tabControlExperts.Name = "tabControlExperts";
            this.tabControlExperts.SelectedIndex = 0;
            this.tabControlExperts.Size = new System.Drawing.Size(523, 455);
            this.tabControlExperts.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(515, 429);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(515, 429);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // toolStripButtonSourceFile
            // 
            this.toolStripButtonSourceFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSourceFile.Image")));
            this.toolStripButtonSourceFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSourceFile.Name = "toolStripButtonSourceFile";
            this.toolStripButtonSourceFile.Size = new System.Drawing.Size(114, 22);
            this.toolStripButtonSourceFile.Text = "Import Source File";
            this.toolStripButtonSourceFile.Click += new System.EventHandler(this.toolStripButtonSourceFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // ExpertManagementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlExperts);
            this.Controls.Add(this.splitterLeft);
            this.Controls.Add(this.panelExperts);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(splitter3);
            this.Controls.Add(this.virtualListViewExAssemblies);
            this.ImageName = "breakpoint_selection.png";
            this.Name = "ExpertManagementControl";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panelExperts.ResumeLayout(false);
            this.panelExperts.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStripAssemblies.ResumeLayout(false);
            this.tabControlExperts.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonExpertList;
        private System.Windows.Forms.ToolStripButton toolStripButtonImportAssembly;
        private ListView listViewExperts;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.Panel panelExperts;
        private CommonSupport.VirtualListViewEx virtualListViewExAssemblies;
        private System.Windows.Forms.ColumnHeader columnHeaderDll;
        private System.Windows.Forms.ToolStripButton toolStripButtonImportedAssemblies;
        private ContextMenuStrip contextMenuStripAssemblies;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonNewExpert;
        private ToolStripButton toolStripButtonDeleteExpert;
        private ToolStripDropDownButton toolStripDropDownButtonRunExpert;
        private ToolStripMenuItem toolStripMenuItemRunStandalone;
        private TabControl tabControlExperts;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private ToolStripButton toolStripButtonEditExpert;
        private Splitter splitterLeft;
        private ToolStripLabel toolStripLabelRemove;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonSourceFile;

    }
}
