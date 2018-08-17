namespace CommonSupport
{
    partial class NewsManagerSettingsControl
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
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewsManagerSettingsControl));
            this.listViewFeeds = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.textBoxAdd = new System.Windows.Forms.TextBox();
            this.numericUpDownUpdateInterval = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMaxItemsCount = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoUpdate = new System.Windows.Forms.CheckBox();
            this.comboBoxPreconfigured = new System.Windows.Forms.ComboBox();
            this.buttonAddPreconfigured = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpdateInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxItemsCount)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(3, 33);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(78, 13);
            label3.TabIndex = 6;
            label3.Text = "New RSS feed";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(479, 5);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(24, 13);
            label5.TabIndex = 10;
            label5.Text = "sec";
            label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 6);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(88, 13);
            label1.TabIndex = 0;
            label1.Text = "Max items shown";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(3, 62);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(105, 13);
            label4.TabIndex = 12;
            label4.Text = "Pre-configured feeds";
            // 
            // listViewFeeds
            // 
            this.listViewFeeds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFeeds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewFeeds.CheckBoxes = true;
            this.listViewFeeds.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewFeeds.FullRowSelect = true;
            this.listViewFeeds.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewFeeds.HideSelection = false;
            this.listViewFeeds.Location = new System.Drawing.Point(6, 130);
            this.listViewFeeds.Name = "listViewFeeds";
            this.listViewFeeds.Size = new System.Drawing.Size(551, 467);
            this.listViewFeeds.TabIndex = 3;
            this.listViewFeeds.UseCompatibleStateImageBehavior = false;
            this.listViewFeeds.View = System.Windows.Forms.View.Details;
            this.listViewFeeds.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewFeeds_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Feed";
            this.columnHeader1.Width = 250;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Link";
            this.columnHeader2.Width = 160;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonAdd.Image = ((System.Drawing.Image)(resources.GetObject("buttonAdd.Image")));
            this.buttonAdd.Location = new System.Drawing.Point(482, 28);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 4;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDelete.Image = ((System.Drawing.Image)(resources.GetObject("buttonDelete.Image")));
            this.buttonDelete.Location = new System.Drawing.Point(482, 101);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 5;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // textBoxAdd
            // 
            this.textBoxAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAdd.Location = new System.Drawing.Point(111, 30);
            this.textBoxAdd.Name = "textBoxAdd";
            this.textBoxAdd.Size = new System.Drawing.Size(362, 20);
            this.textBoxAdd.TabIndex = 7;
            // 
            // numericUpDownUpdateInterval
            // 
            this.numericUpDownUpdateInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownUpdateInterval.Location = new System.Drawing.Point(353, 3);
            this.numericUpDownUpdateInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownUpdateInterval.Name = "numericUpDownUpdateInterval";
            this.numericUpDownUpdateInterval.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownUpdateInterval.TabIndex = 9;
            this.numericUpDownUpdateInterval.ValueChanged += new System.EventHandler(this.updateInterval_ValueChanged);
            // 
            // numericUpDownMaxItemsCount
            // 
            this.numericUpDownMaxItemsCount.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownMaxItemsCount.Location = new System.Drawing.Point(100, 3);
            this.numericUpDownMaxItemsCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownMaxItemsCount.Name = "numericUpDownMaxItemsCount";
            this.numericUpDownMaxItemsCount.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownMaxItemsCount.TabIndex = 1;
            this.numericUpDownMaxItemsCount.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownMaxItemsCount.ValueChanged += new System.EventHandler(this.numericUpDownMaxItemsCount_ValueChanged);
            // 
            // checkBoxAutoUpdate
            // 
            this.checkBoxAutoUpdate.AutoSize = true;
            this.checkBoxAutoUpdate.Checked = true;
            this.checkBoxAutoUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxAutoUpdate.Location = new System.Drawing.Point(226, 4);
            this.checkBoxAutoUpdate.Name = "checkBoxAutoUpdate";
            this.checkBoxAutoUpdate.Size = new System.Drawing.Size(118, 17);
            this.checkBoxAutoUpdate.TabIndex = 11;
            this.checkBoxAutoUpdate.Text = "Auto update interval";
            this.checkBoxAutoUpdate.UseVisualStyleBackColor = true;
            this.checkBoxAutoUpdate.CheckedChanged += new System.EventHandler(this.updateInterval_ValueChanged);
            // 
            // comboBoxPreconfigured
            // 
            this.comboBoxPreconfigured.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPreconfigured.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPreconfigured.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxPreconfigured.FormattingEnabled = true;
            this.comboBoxPreconfigured.Location = new System.Drawing.Point(111, 59);
            this.comboBoxPreconfigured.Name = "comboBoxPreconfigured";
            this.comboBoxPreconfigured.Size = new System.Drawing.Size(362, 21);
            this.comboBoxPreconfigured.TabIndex = 13;
            // 
            // buttonAddPreconfigured
            // 
            this.buttonAddPreconfigured.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddPreconfigured.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonAddPreconfigured.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddPreconfigured.Image")));
            this.buttonAddPreconfigured.Location = new System.Drawing.Point(482, 57);
            this.buttonAddPreconfigured.Name = "buttonAddPreconfigured";
            this.buttonAddPreconfigured.Size = new System.Drawing.Size(75, 23);
            this.buttonAddPreconfigured.TabIndex = 14;
            this.buttonAddPreconfigured.Text = "Add";
            this.buttonAddPreconfigured.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonAddPreconfigured.UseVisualStyleBackColor = true;
            this.buttonAddPreconfigured.Click += new System.EventHandler(this.buttonAddPreconfigured_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSettings.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSettings.Image = ((System.Drawing.Image)(resources.GetObject("buttonSettings.Image")));
            this.buttonSettings.Location = new System.Drawing.Point(401, 101);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(75, 23);
            this.buttonSettings.TabIndex = 17;
            this.buttonSettings.Text = "Settings";
            this.buttonSettings.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // NewsManagerSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonAddPreconfigured);
            this.Controls.Add(this.comboBoxPreconfigured);
            this.Controls.Add(label4);
            this.Controls.Add(this.checkBoxAutoUpdate);
            this.Controls.Add(label5);
            this.Controls.Add(this.numericUpDownUpdateInterval);
            this.Controls.Add(this.textBoxAdd);
            this.Controls.Add(label3);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listViewFeeds);
            this.Controls.Add(this.numericUpDownMaxItemsCount);
            this.Controls.Add(label1);
            this.Name = "NewsManagerSettingsControl";
            this.Size = new System.Drawing.Size(560, 600);
            this.Load += new System.EventHandler(this.NewsManagerSettingsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpdateInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxItemsCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textBoxAdd;
        private System.Windows.Forms.NumericUpDown numericUpDownUpdateInterval;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView listViewFeeds;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxItemsCount;
        private System.Windows.Forms.CheckBox checkBoxAutoUpdate;
        private System.Windows.Forms.ComboBox comboBoxPreconfigured;
        private System.Windows.Forms.Button buttonAddPreconfigured;
        private System.Windows.Forms.Button buttonSettings;
    }
}
