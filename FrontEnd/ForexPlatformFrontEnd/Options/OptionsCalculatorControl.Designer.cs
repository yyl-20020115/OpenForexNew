namespace ForexPlatformFrontEnd
{
    partial class OptionsCalculatorControl
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
            this.chartControl1 = new CommonFinancial.ChartControl();
            this.buttonPut = new System.Windows.Forms.Button();
            this.numericUpDownStart = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownEnd = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStrike = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownPrice = new System.Windows.Forms.NumericUpDown();
            this.buttonCall = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStrike)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // chartControl1
            // 
            this.chartControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chartControl1.Location = new System.Drawing.Point(2, 66);
            this.chartControl1.Margin = new System.Windows.Forms.Padding(2);
            this.chartControl1.Name = "chartControl1";
            this.chartControl1.Size = new System.Drawing.Size(796, 532);
            this.chartControl1.TabIndex = 0;
            // 
            // buttonPut
            // 
            this.buttonPut.Location = new System.Drawing.Point(190, 3);
            this.buttonPut.Name = "buttonPut";
            this.buttonPut.Size = new System.Drawing.Size(75, 23);
            this.buttonPut.TabIndex = 1;
            this.buttonPut.Text = "Put";
            this.buttonPut.UseVisualStyleBackColor = true;
            this.buttonPut.Click += new System.EventHandler(this.buttonPut_Click);
            // 
            // numericUpDownStart
            // 
            this.numericUpDownStart.Location = new System.Drawing.Point(551, 3);
            this.numericUpDownStart.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.numericUpDownStart.Name = "numericUpDownStart";
            this.numericUpDownStart.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownStart.TabIndex = 3;
            // 
            // numericUpDownEnd
            // 
            this.numericUpDownEnd.Location = new System.Drawing.Point(677, 3);
            this.numericUpDownEnd.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numericUpDownEnd.Name = "numericUpDownEnd";
            this.numericUpDownEnd.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownEnd.TabIndex = 4;
            this.numericUpDownEnd.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // numericUpDownStrike
            // 
            this.numericUpDownStrike.DecimalPlaces = 4;
            this.numericUpDownStrike.Location = new System.Drawing.Point(64, 3);
            this.numericUpDownStrike.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.numericUpDownStrike.Name = "numericUpDownStrike";
            this.numericUpDownStrike.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownStrike.TabIndex = 5;
            this.numericUpDownStrike.Enter += new System.EventHandler(this.numericUpDownStrike_Enter);
            // 
            // numericUpDownPrice
            // 
            this.numericUpDownPrice.DecimalPlaces = 4;
            this.numericUpDownPrice.Location = new System.Drawing.Point(64, 29);
            this.numericUpDownPrice.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.numericUpDownPrice.Name = "numericUpDownPrice";
            this.numericUpDownPrice.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownPrice.TabIndex = 6;
            this.numericUpDownPrice.Enter += new System.EventHandler(this.numericUpDownPrice_Enter);
            // 
            // buttonCall
            // 
            this.buttonCall.Location = new System.Drawing.Point(190, 32);
            this.buttonCall.Name = "buttonCall";
            this.buttonCall.Size = new System.Drawing.Size(75, 23);
            this.buttonCall.TabIndex = 7;
            this.buttonCall.Text = "Call";
            this.buttonCall.UseVisualStyleBackColor = true;
            this.buttonCall.Click += new System.EventHandler(this.buttonCall_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Strike";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Price";
            // 
            // OptionsCalculatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCall);
            this.Controls.Add(this.numericUpDownPrice);
            this.Controls.Add(this.numericUpDownStrike);
            this.Controls.Add(this.numericUpDownEnd);
            this.Controls.Add(this.numericUpDownStart);
            this.Controls.Add(this.buttonPut);
            this.Controls.Add(this.chartControl1);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStrike)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommonFinancial.ChartControl chartControl1;
        private System.Windows.Forms.Button buttonPut;
        private System.Windows.Forms.NumericUpDown numericUpDownStart;
        private System.Windows.Forms.NumericUpDown numericUpDownEnd;
        private System.Windows.Forms.NumericUpDown numericUpDownStrike;
        private System.Windows.Forms.NumericUpDown numericUpDownPrice;
        private System.Windows.Forms.Button buttonCall;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
