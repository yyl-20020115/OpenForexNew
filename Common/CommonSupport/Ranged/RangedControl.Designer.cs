namespace CommonSupport
{
    partial class RangedControl
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
            this.numericUpDownEnd = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStart = new System.Windows.Forms.NumericUpDown();
            this.trackBarEnd = new System.Windows.Forms.TrackBar();
            this.trackBarStart = new System.Windows.Forms.TrackBar();
            this.labelCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarStart)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownEnd
            // 
            this.numericUpDownEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownEnd.Enabled = false;
            this.numericUpDownEnd.Location = new System.Drawing.Point(317, 43);
            this.numericUpDownEnd.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.numericUpDownEnd.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
            this.numericUpDownEnd.Name = "numericUpDownEnd";
            this.numericUpDownEnd.ReadOnly = true;
            this.numericUpDownEnd.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownEnd.TabIndex = 107;
            // 
            // numericUpDownStart
            // 
            this.numericUpDownStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownStart.Enabled = false;
            this.numericUpDownStart.Location = new System.Drawing.Point(317, 8);
            this.numericUpDownStart.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.numericUpDownStart.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
            this.numericUpDownStart.Name = "numericUpDownStart";
            this.numericUpDownStart.ReadOnly = true;
            this.numericUpDownStart.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownStart.TabIndex = 106;
            // 
            // trackBarEnd
            // 
            this.trackBarEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarEnd.LargeChange = 1;
            this.trackBarEnd.Location = new System.Drawing.Point(6, 31);
            this.trackBarEnd.Maximum = 0;
            this.trackBarEnd.Name = "trackBarEnd";
            this.trackBarEnd.Size = new System.Drawing.Size(305, 42);
            this.trackBarEnd.TabIndex = 105;
            this.trackBarEnd.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarEnd.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            // 
            // trackBarStart
            // 
            this.trackBarStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarStart.LargeChange = 1;
            this.trackBarStart.Location = new System.Drawing.Point(6, 8);
            this.trackBarStart.Maximum = 0;
            this.trackBarStart.Name = "trackBarStart";
            this.trackBarStart.Size = new System.Drawing.Size(305, 42);
            this.trackBarStart.TabIndex = 104;
            this.trackBarStart.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            // 
            // label1
            // 
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(3, 67);
            this.labelCount.Name = "label1";
            this.labelCount.Size = new System.Drawing.Size(50, 13);
            this.labelCount.TabIndex = 108;
            this.labelCount.Text = "Count : 0";
            // 
            // RangedControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.trackBarEnd);
            this.Controls.Add(this.numericUpDownEnd);
            this.Controls.Add(this.numericUpDownStart);
            this.Controls.Add(this.trackBarStart);
            this.Name = "RangedControl";
            this.Size = new System.Drawing.Size(400, 80);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownEnd;
        private System.Windows.Forms.NumericUpDown numericUpDownStart;
        private System.Windows.Forms.TrackBar trackBarEnd;
        private System.Windows.Forms.TrackBar trackBarStart;
        private System.Windows.Forms.Label labelCount;
    }
}
