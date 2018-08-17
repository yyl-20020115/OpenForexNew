using CommonSupport;
using CommonFinancial;
namespace ForexPlatformFrontEnd
{
    partial class ConsecutivesTestForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required baseMethod for Designer support - do not modify
        /// the contents of this baseMethod with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonCalculate = new System.Windows.Forms.Button();
            this.labelConsecutives = new System.Windows.Forms.Label();
            this.numericUpDownTotalCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownIterations = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.graphControl = new CommonFinancial.ChartControl();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIterations)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.AutoSize = true;
            this.buttonCalculate.Location = new System.Drawing.Point(319, 22);
            this.buttonCalculate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonCalculate.Name = "buttonCalculate";
            this.buttonCalculate.Size = new System.Drawing.Size(61, 23);
            this.buttonCalculate.TabIndex = 0;
            this.buttonCalculate.Text = "Calculate";
            this.buttonCalculate.UseVisualStyleBackColor = true;
            this.buttonCalculate.Click += new System.EventHandler(this.buttonCalculate_Click);
            // 
            // labelConsecutives
            // 
            this.labelConsecutives.AutoSize = true;
            this.labelConsecutives.Location = new System.Drawing.Point(2, 2);
            this.labelConsecutives.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelConsecutives.Name = "labelConsecutives";
            this.labelConsecutives.Size = new System.Drawing.Size(71, 13);
            this.labelConsecutives.TabIndex = 1;
            this.labelConsecutives.Text = "Consecutives";
            // 
            // numericUpDownTotalCount
            // 
            this.numericUpDownTotalCount.Location = new System.Drawing.Point(76, 23);
            this.numericUpDownTotalCount.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDownTotalCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownTotalCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTotalCount.Name = "numericUpDownTotalCount";
            this.numericUpDownTotalCount.Size = new System.Drawing.Size(91, 20);
            this.numericUpDownTotalCount.TabIndex = 2;
            this.numericUpDownTotalCount.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Total count";
            // 
            // numericUpDownIterations
            // 
            this.numericUpDownIterations.Location = new System.Drawing.Point(224, 23);
            this.numericUpDownIterations.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDownIterations.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownIterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownIterations.Name = "numericUpDownIterations";
            this.numericUpDownIterations.Size = new System.Drawing.Size(91, 20);
            this.numericUpDownIterations.TabIndex = 4;
            this.numericUpDownIterations.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(170, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Iterations";
            // 
            // graphControl
            // 
            this.graphControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphControl.Location = new System.Drawing.Point(0, 50);
            this.graphControl.Name = "graphControl";
            this.graphControl.Size = new System.Drawing.Size(600, 438);
            this.graphControl.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.buttonCalculate);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.labelConsecutives);
            this.panel1.Controls.Add(this.numericUpDownIterations);
            this.panel1.Controls.Add(this.numericUpDownTotalCount);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 50);
            this.panel1.TabIndex = 7;
            // 
            // ConsecutivesTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graphControl);
            this.Controls.Add(this.panel1);
            this.ImageName = "Help.png";
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ConsecutivesTestForm";
            this.Size = new System.Drawing.Size(600, 488);
            this.Name = "Consecutive Orders Result Test";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIterations)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCalculate;
        private System.Windows.Forms.Label labelConsecutives;
        private System.Windows.Forms.NumericUpDown numericUpDownTotalCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownIterations;
        private System.Windows.Forms.Label label1;
        private ChartControl graphControl;
        private System.Windows.Forms.Panel panel1;
    }
}