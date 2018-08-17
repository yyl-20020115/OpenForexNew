using ForexPlatform;
using CommonFinancial;
namespace ForexPlatformFrontEnd
{
    partial class MultipleItemStatisticsControl
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
            this.listViewItemSelection = new System.Windows.Forms.ListView();
            this.buttonPresent = new System.Windows.Forms.Button();
            this.buttonPresentSorted = new System.Windows.Forms.Button();
            this.buttonPresentDistribution = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonPresentConsecutivesDistribution = new System.Windows.Forms.Button();
            this.checkBoxShowMA = new System.Windows.Forms.CheckBox();
            this.numericUpDownMAPeriods = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxShow0Line = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageStatistics = new System.Windows.Forms.TabPage();
            this.chartControl = new CommonFinancial.ChartControl();
            this.tabPageItems = new System.Windows.Forms.TabPage();
            this.listViewItems = new System.Windows.Forms.ListView();
            this.comboBoxSelectedSet = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMAPeriods)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageStatistics.SuspendLayout();
            this.tabPageItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewItemSelection
            // 
            this.listViewItemSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewItemSelection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewItemSelection.FullRowSelect = true;
            this.listViewItemSelection.HideSelection = false;
            this.listViewItemSelection.Location = new System.Drawing.Point(5, 6);
            this.listViewItemSelection.Name = "listViewItemSelection";
            this.listViewItemSelection.Size = new System.Drawing.Size(784, 190);
            this.listViewItemSelection.TabIndex = 2;
            this.listViewItemSelection.UseCompatibleStateImageBehavior = false;
            this.listViewItemSelection.View = System.Windows.Forms.View.Details;
            this.listViewItemSelection.DoubleClick += new System.EventHandler(this.listViewItemSelection_DoubleClick);
            // 
            // buttonPresent
            // 
            this.buttonPresent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPresent.Location = new System.Drawing.Point(5, 202);
            this.buttonPresent.Name = "buttonPresent";
            this.buttonPresent.Size = new System.Drawing.Size(75, 23);
            this.buttonPresent.TabIndex = 5;
            this.buttonPresent.Text = "Present";
            this.buttonPresent.UseVisualStyleBackColor = true;
            this.buttonPresent.Click += new System.EventHandler(this.buttonPresent_Click);
            // 
            // buttonPresentSorted
            // 
            this.buttonPresentSorted.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPresentSorted.Location = new System.Drawing.Point(86, 202);
            this.buttonPresentSorted.Name = "buttonPresentSorted";
            this.buttonPresentSorted.Size = new System.Drawing.Size(112, 23);
            this.buttonPresentSorted.TabIndex = 8;
            this.buttonPresentSorted.Text = "Present Sorted";
            this.buttonPresentSorted.UseVisualStyleBackColor = true;
            this.buttonPresentSorted.Click += new System.EventHandler(this.buttonPresentSorted_Click);
            // 
            // buttonPresentDistribution
            // 
            this.buttonPresentDistribution.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPresentDistribution.Location = new System.Drawing.Point(204, 202);
            this.buttonPresentDistribution.Name = "buttonPresentDistribution";
            this.buttonPresentDistribution.Size = new System.Drawing.Size(112, 23);
            this.buttonPresentDistribution.TabIndex = 9;
            this.buttonPresentDistribution.Text = "Present Distribution";
            this.buttonPresentDistribution.UseVisualStyleBackColor = true;
            this.buttonPresentDistribution.Click += new System.EventHandler(this.buttonPresentDistribution_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelStatus.Location = new System.Drawing.Point(0, 0);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(10, 13);
            this.labelStatus.TabIndex = 10;
            this.labelStatus.Text = "-";
            // 
            // buttonPresentConsecutivesDistribution
            // 
            this.buttonPresentConsecutivesDistribution.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPresentConsecutivesDistribution.Location = new System.Drawing.Point(322, 202);
            this.buttonPresentConsecutivesDistribution.Name = "buttonPresentConsecutivesDistribution";
            this.buttonPresentConsecutivesDistribution.Size = new System.Drawing.Size(181, 23);
            this.buttonPresentConsecutivesDistribution.TabIndex = 11;
            this.buttonPresentConsecutivesDistribution.Text = "Present Consecutives Distribution";
            this.buttonPresentConsecutivesDistribution.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowMA
            // 
            this.checkBoxShowMA.AutoSize = true;
            this.checkBoxShowMA.Checked = true;
            this.checkBoxShowMA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowMA.Location = new System.Drawing.Point(508, 205);
            this.checkBoxShowMA.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxShowMA.Name = "checkBoxShowMA";
            this.checkBoxShowMA.Size = new System.Drawing.Size(72, 17);
            this.checkBoxShowMA.TabIndex = 12;
            this.checkBoxShowMA.Text = "Show MA";
            this.checkBoxShowMA.UseVisualStyleBackColor = true;
            // 
            // numericUpDownMAPeriods
            // 
            this.numericUpDownMAPeriods.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownMAPeriods.Location = new System.Drawing.Point(578, 205);
            this.numericUpDownMAPeriods.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownMAPeriods.Name = "numericUpDownMAPeriods";
            this.numericUpDownMAPeriods.Size = new System.Drawing.Size(91, 20);
            this.numericUpDownMAPeriods.TabIndex = 13;
            this.numericUpDownMAPeriods.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(673, 206);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "periods";
            // 
            // checkBoxShow0Line
            // 
            this.checkBoxShow0Line.AutoSize = true;
            this.checkBoxShow0Line.Checked = true;
            this.checkBoxShow0Line.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShow0Line.Location = new System.Drawing.Point(718, 206);
            this.checkBoxShow0Line.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxShow0Line.Name = "checkBoxShow0Line";
            this.checkBoxShow0Line.Size = new System.Drawing.Size(81, 17);
            this.checkBoxShow0Line.TabIndex = 15;
            this.checkBoxShow0Line.Text = "Show 0 line";
            this.checkBoxShow0Line.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageStatistics);
            this.tabControl1.Controls.Add(this.tabPageItems);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 13);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 587);
            this.tabControl1.TabIndex = 16;
            // 
            // tabPageStatistics
            // 
            this.tabPageStatistics.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageStatistics.Controls.Add(this.listViewItemSelection);
            this.tabPageStatistics.Controls.Add(this.checkBoxShow0Line);
            this.tabPageStatistics.Controls.Add(this.buttonPresent);
            this.tabPageStatistics.Controls.Add(this.label1);
            this.tabPageStatistics.Controls.Add(this.chartControl);
            this.tabPageStatistics.Controls.Add(this.numericUpDownMAPeriods);
            this.tabPageStatistics.Controls.Add(this.buttonPresentSorted);
            this.tabPageStatistics.Controls.Add(this.checkBoxShowMA);
            this.tabPageStatistics.Controls.Add(this.buttonPresentDistribution);
            this.tabPageStatistics.Controls.Add(this.buttonPresentConsecutivesDistribution);
            this.tabPageStatistics.Location = new System.Drawing.Point(4, 22);
            this.tabPageStatistics.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageStatistics.Name = "tabPageStatistics";
            this.tabPageStatistics.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageStatistics.Size = new System.Drawing.Size(792, 561);
            this.tabPageStatistics.TabIndex = 0;
            this.tabPageStatistics.Text = "Statistics";
            // 
            // chartControl
            // 
            this.chartControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chartControl.Location = new System.Drawing.Point(5, 231);
            this.chartControl.Margin = new System.Windows.Forms.Padding(4);
            this.chartControl.Name = "chartControl";
            this.chartControl.Size = new System.Drawing.Size(784, 326);
            this.chartControl.TabIndex = 7;
            // 
            // tabPageItems
            // 
            this.tabPageItems.Controls.Add(this.listViewItems);
            this.tabPageItems.Controls.Add(this.comboBoxSelectedSet);
            this.tabPageItems.Location = new System.Drawing.Point(4, 22);
            this.tabPageItems.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageItems.Name = "tabPageItems";
            this.tabPageItems.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageItems.Size = new System.Drawing.Size(792, 561);
            this.tabPageItems.TabIndex = 1;
            this.tabPageItems.Text = "Items";
            this.tabPageItems.UseVisualStyleBackColor = true;
            // 
            // listViewItems
            // 
            this.listViewItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewItems.FullRowSelect = true;
            this.listViewItems.Location = new System.Drawing.Point(2, 23);
            this.listViewItems.Margin = new System.Windows.Forms.Padding(2);
            this.listViewItems.Name = "listViewItems";
            this.listViewItems.Size = new System.Drawing.Size(788, 536);
            this.listViewItems.TabIndex = 0;
            this.listViewItems.UseCompatibleStateImageBehavior = false;
            this.listViewItems.View = System.Windows.Forms.View.Details;
            // 
            // comboBoxSelectedSet
            // 
            this.comboBoxSelectedSet.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxSelectedSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectedSet.FormattingEnabled = true;
            this.comboBoxSelectedSet.Location = new System.Drawing.Point(2, 2);
            this.comboBoxSelectedSet.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxSelectedSet.Name = "comboBoxSelectedSet";
            this.comboBoxSelectedSet.Size = new System.Drawing.Size(788, 21);
            this.comboBoxSelectedSet.TabIndex = 1;
            this.comboBoxSelectedSet.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectedSet_SelectedIndexChanged);
            // 
            // MultipleItemStatisticsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.labelStatus);
            this.Name = "MultipleItemStatisticsControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.Load += new System.EventHandler(this.MultipleItemStatisticsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMAPeriods)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageStatistics.ResumeLayout(false);
            this.tabPageStatistics.PerformLayout();
            this.tabPageItems.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewItemSelection;
        private System.Windows.Forms.Button buttonPresent;
        private ChartControl chartControl;
        private System.Windows.Forms.Button buttonPresentSorted;
        private System.Windows.Forms.Button buttonPresentDistribution;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonPresentConsecutivesDistribution;
        private System.Windows.Forms.CheckBox checkBoxShowMA;
        private System.Windows.Forms.NumericUpDown numericUpDownMAPeriods;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxShow0Line;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageStatistics;
        private System.Windows.Forms.TabPage tabPageItems;
        private System.Windows.Forms.ListView listViewItems;
        private System.Windows.Forms.ComboBox comboBoxSelectedSet;
    }
}
