using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MT4Adapter
{
    public partial class MT4IntegrationWizardControl
    {
        private System.Windows.Forms.Button buttonDeploy;
        private System.Windows.Forms.TextBox textBoxMT4Directory;
        private System.Windows.Forms.Button buttonBrowse;
        private ToolTip toolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label label1;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MT4IntegrationWizardControl));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDeploy = new System.Windows.Forms.Button();
            this.textBoxMT4Directory = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonDeploySelected = new System.Windows.Forms.Button();
            this.buttonClearHistory = new System.Windows.Forms.Button();
            this.listViewHistory = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.groupBoxHistory = new System.Windows.Forms.GroupBox();
            this.groupBoxHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Meta Trader 4 Folder";
            // 
            // buttonDeploy
            // 
            this.buttonDeploy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeploy.AutoSize = true;
            this.buttonDeploy.Enabled = false;
            this.buttonDeploy.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDeploy.Image = ((System.Drawing.Image)(resources.GetObject("buttonDeploy.Image")));
            this.buttonDeploy.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonDeploy.Location = new System.Drawing.Point(541, 14);
            this.buttonDeploy.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDeploy.Name = "buttonDeploy";
            this.buttonDeploy.Size = new System.Drawing.Size(106, 23);
            this.buttonDeploy.TabIndex = 8;
            this.buttonDeploy.Text = "Deploy";
            this.buttonDeploy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonDeploy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip.SetToolTip(this.buttonDeploy, "Deploy to Folder");
            this.buttonDeploy.UseVisualStyleBackColor = true;
            this.buttonDeploy.Click += new System.EventHandler(this.buttonDeploy_Click);
            // 
            // textBoxMT4Directory
            // 
            this.textBoxMT4Directory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMT4Directory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxMT4Directory.Location = new System.Drawing.Point(4, 18);
            this.textBoxMT4Directory.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxMT4Directory.Name = "textBoxMT4Directory";
            this.textBoxMT4Directory.Size = new System.Drawing.Size(503, 20);
            this.textBoxMT4Directory.TabIndex = 9;
            this.textBoxMT4Directory.TextChanged += new System.EventHandler(this.textBoxMT4Directory_TextChanged);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.AutoSize = true;
            this.buttonBrowse.Location = new System.Drawing.Point(511, 15);
            this.buttonBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(26, 23);
            this.buttonBrowse.TabIndex = 10;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.AutoSize = true;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonClose.Location = new System.Drawing.Point(541, 300);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(106, 23);
            this.buttonClose.TabIndex = 12;
            this.buttonClose.Text = "Close";
            this.toolTip.SetToolTip(this.buttonClose, "Close");
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonDeploySelected
            // 
            this.buttonDeploySelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonDeploySelected.AutoSize = true;
            this.buttonDeploySelected.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDeploySelected.Image = ((System.Drawing.Image)(resources.GetObject("buttonDeploySelected.Image")));
            this.buttonDeploySelected.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.buttonDeploySelected.Location = new System.Drawing.Point(524, 223);
            this.buttonDeploySelected.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDeploySelected.Name = "buttonDeploySelected";
            this.buttonDeploySelected.Size = new System.Drawing.Size(111, 23);
            this.buttonDeploySelected.TabIndex = 14;
            this.buttonDeploySelected.Text = "Deploy Selected";
            this.buttonDeploySelected.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.buttonDeploySelected.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip.SetToolTip(this.buttonDeploySelected, "Deploy to Selected Folders");
            this.buttonDeploySelected.UseVisualStyleBackColor = true;
            this.buttonDeploySelected.Click += new System.EventHandler(this.buttonDeploySelected_Click);
            // 
            // buttonClearHistory
            // 
            this.buttonClearHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClearHistory.AutoSize = true;
            this.buttonClearHistory.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonClearHistory.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.buttonClearHistory.Location = new System.Drawing.Point(5, 224);
            this.buttonClearHistory.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClearHistory.Name = "buttonClearHistory";
            this.buttonClearHistory.Size = new System.Drawing.Size(109, 23);
            this.buttonClearHistory.TabIndex = 16;
            this.buttonClearHistory.Text = "Clear History";
            this.buttonClearHistory.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip.SetToolTip(this.buttonClearHistory, "Clear Selected History Folders");
            this.buttonClearHistory.UseVisualStyleBackColor = true;
            this.buttonClearHistory.Click += new System.EventHandler(this.buttonClearHistory_Click);
            // 
            // listViewHistory
            // 
            this.listViewHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewHistory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewHistory.CheckBoxes = true;
            this.listViewHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewHistory.FullRowSelect = true;
            this.listViewHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewHistory.Location = new System.Drawing.Point(6, 19);
            this.listViewHistory.Name = "listViewHistory";
            this.listViewHistory.Size = new System.Drawing.Size(629, 200);
            this.listViewHistory.TabIndex = 13;
            this.listViewHistory.UseCompatibleStateImageBehavior = false;
            this.listViewHistory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Folder";
            this.columnHeader1.Width = 641;
            // 
            // groupBoxHistory
            // 
            this.groupBoxHistory.Controls.Add(this.buttonClearHistory);
            this.groupBoxHistory.Controls.Add(this.buttonDeploySelected);
            this.groupBoxHistory.Controls.Add(this.listViewHistory);
            this.groupBoxHistory.Location = new System.Drawing.Point(5, 43);
            this.groupBoxHistory.Name = "groupBoxHistory";
            this.groupBoxHistory.Size = new System.Drawing.Size(641, 252);
            this.groupBoxHistory.TabIndex = 17;
            this.groupBoxHistory.TabStop = false;
            this.groupBoxHistory.Text = "Folders History";
            // 
            // MT4IntegrationWizardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupBoxHistory);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxMT4Directory);
            this.Controls.Add(this.buttonDeploy);
            this.Controls.Add(this.label1);
            this.Name = "MT4IntegrationWizardControl";
            this.Size = new System.Drawing.Size(649, 325);
            this.Load += new System.EventHandler(this.MT4IntegrationWizardControl_Load);
            this.groupBoxHistory.ResumeLayout(false);
            this.groupBoxHistory.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Button buttonClose;
        private ListView listViewHistory;
        private Button buttonDeploySelected;
        private Button buttonClearHistory;
        private ColumnHeader columnHeader1;
        private GroupBox groupBoxHistory;
    }
}
