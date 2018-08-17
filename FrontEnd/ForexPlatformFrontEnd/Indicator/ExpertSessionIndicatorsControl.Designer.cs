using CommonSupport;
namespace ForexPlatformFrontEnd
{
    partial class ExpertSessionIndicatorsControl
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
            System.Windows.Forms.Label label4;
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.GroupBox groupBox2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpertSessionIndicatorsControl));
            System.Windows.Forms.GroupBox groupBox3;
            this.indicatorControl1 = new CommonSupport.CustomPropertiesControl();
            this.listViewIndicators = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.listViewIndicatorTypes = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.comboBoxChartAreas = new System.Windows.Forms.ComboBox();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(4, 271);
            label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(69, 13);
            label4.TabIndex = 9;
            label4.Text = "Display Pane";
            label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(this.indicatorControl1);
            groupBox1.Location = new System.Drawing.Point(442, 2);
            groupBox1.Margin = new System.Windows.Forms.Padding(2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(2);
            groupBox1.Size = new System.Drawing.Size(196, 489);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "Selected Indicator Properties";
            // 
            // indicatorControl1
            // 
            this.indicatorControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.indicatorControl1.IsReadOnly = false;
            this.indicatorControl1.Location = new System.Drawing.Point(4, 17);
            this.indicatorControl1.Margin = new System.Windows.Forms.Padding(2);
            this.indicatorControl1.Name = "indicatorControl1";
            this.indicatorControl1.SelectedContainerObject = null;
            this.indicatorControl1.SelectedObject = null;
            this.indicatorControl1.Size = new System.Drawing.Size(187, 467);
            this.indicatorControl1.TabIndex = 11;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupBox2.Controls.Add(this.listViewIndicators);
            groupBox2.Controls.Add(this.buttonRemove);
            groupBox2.Location = new System.Drawing.Point(2, 2);
            groupBox2.Margin = new System.Windows.Forms.Padding(2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(2);
            groupBox2.Size = new System.Drawing.Size(433, 171);
            groupBox2.TabIndex = 13;
            groupBox2.TabStop = false;
            groupBox2.Text = "Existing";
            // 
            // listViewIndicators
            // 
            this.listViewIndicators.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewIndicators.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewIndicators.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewIndicators.FullRowSelect = true;
            this.listViewIndicators.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewIndicators.HideSelection = false;
            this.listViewIndicators.Location = new System.Drawing.Point(4, 17);
            this.listViewIndicators.Margin = new System.Windows.Forms.Padding(2);
            this.listViewIndicators.MultiSelect = false;
            this.listViewIndicators.Name = "listViewIndicators";
            this.listViewIndicators.Size = new System.Drawing.Size(424, 123);
            this.listViewIndicators.TabIndex = 3;
            this.listViewIndicators.UseCompatibleStateImageBehavior = false;
            this.listViewIndicators.View = System.Windows.Forms.View.Details;
            this.listViewIndicators.SelectedIndexChanged += new System.EventHandler(this.listViewIndicators_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 384;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemove.AutoSize = true;
            this.buttonRemove.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemove.Image")));
            this.buttonRemove.Location = new System.Drawing.Point(355, 144);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(73, 23);
            this.buttonRemove.TabIndex = 5;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // groupBox3
            // 
            groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupBox3.Controls.Add(this.listViewIndicatorTypes);
            groupBox3.Controls.Add(this.comboBoxChartAreas);
            groupBox3.Controls.Add(this.buttonNew);
            groupBox3.Controls.Add(label4);
            groupBox3.Location = new System.Drawing.Point(2, 179);
            groupBox3.Margin = new System.Windows.Forms.Padding(2);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(2);
            groupBox3.Size = new System.Drawing.Size(433, 339);
            groupBox3.TabIndex = 14;
            groupBox3.TabStop = false;
            groupBox3.Text = "Create New";
            // 
            // listViewIndicatorTypes
            // 
            this.listViewIndicatorTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewIndicatorTypes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewIndicatorTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listViewIndicatorTypes.FullRowSelect = true;
            this.listViewIndicatorTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewIndicatorTypes.HideSelection = false;
            this.listViewIndicatorTypes.Location = new System.Drawing.Point(4, 17);
            this.listViewIndicatorTypes.Margin = new System.Windows.Forms.Padding(2);
            this.listViewIndicatorTypes.MultiSelect = false;
            this.listViewIndicatorTypes.Name = "listViewIndicatorTypes";
            this.listViewIndicatorTypes.Size = new System.Drawing.Size(424, 252);
            this.listViewIndicatorTypes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewIndicatorTypes.TabIndex = 1;
            this.listViewIndicatorTypes.UseCompatibleStateImageBehavior = false;
            this.listViewIndicatorTypes.View = System.Windows.Forms.View.Details;
            this.listViewIndicatorTypes.SelectedIndexChanged += new System.EventHandler(this.listViewIndicatorTypes_SelectedIndexChanged);
            this.listViewIndicatorTypes.DoubleClick += new System.EventHandler(this.listViewIndicatorTypes_DoubleClick);
            this.listViewIndicatorTypes.Enter += new System.EventHandler(this.listViewIndicatorTypes_Enter);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 384;
            // 
            // comboBoxChartAreas
            // 
            this.comboBoxChartAreas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxChartAreas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChartAreas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxChartAreas.FormattingEnabled = true;
            this.comboBoxChartAreas.Items.AddRange(new object[] {
            "New chart"});
            this.comboBoxChartAreas.Location = new System.Drawing.Point(4, 286);
            this.comboBoxChartAreas.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxChartAreas.Name = "comboBoxChartAreas";
            this.comboBoxChartAreas.Size = new System.Drawing.Size(424, 21);
            this.comboBoxChartAreas.TabIndex = 10;
            this.comboBoxChartAreas.SelectedIndexChanged += new System.EventHandler(this.comboBoxChartAreas_SelectedIndexChanged);
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNew.AutoSize = true;
            this.buttonNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonNew.Image")));
            this.buttonNew.Location = new System.Drawing.Point(348, 311);
            this.buttonNew.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(80, 23);
            this.buttonNew.TabIndex = 4;
            this.buttonNew.Text = "Create";
            this.buttonNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.AutoSize = true;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(553, 495);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(80, 23);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // ExpertSessionIndicatorsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(groupBox3);
            this.Controls.Add(groupBox2);
            this.Controls.Add(groupBox1);
            this.Controls.Add(this.buttonClose);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ExpertSessionIndicatorsControl";
            this.Size = new System.Drawing.Size(640, 520);
            this.Load += new System.EventHandler(this.ExpertSessionIndicatorsControl_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ExpertSessionIndicatorsControl_KeyPress);
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewIndicatorTypes;
        private System.Windows.Forms.ListView listViewIndicators;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ComboBox comboBoxChartAreas;
        private CustomPropertiesControl indicatorControl1;
    }
}
