namespace ForexPlatformFrontEnd
{
    partial class TimeManagementSkipToControl
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeManagementSkipToControl));
            this.buttonSkipTo = new System.Windows.Forms.Button();
            this.numericUpDownSkip = new System.Windows.Forms.NumericUpDown();
            this.labelOf = new System.Windows.Forms.Label();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.buttonSkipToEnd = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxCurrent = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSkip)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(2, 27);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(40, 13);
            label1.TabIndex = 0;
            label1.Text = "Skip to";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(2, 4);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(41, 13);
            label2.TabIndex = 4;
            label2.Text = "Current";
            // 
            // buttonSkipTo
            // 
            this.buttonSkipTo.AutoSize = true;
            this.buttonSkipTo.Image = ((System.Drawing.Image)(resources.GetObject("buttonSkipTo.Image")));
            this.buttonSkipTo.Location = new System.Drawing.Point(175, 23);
            this.buttonSkipTo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonSkipTo.Name = "buttonSkipTo";
            this.buttonSkipTo.Size = new System.Drawing.Size(96, 23);
            this.buttonSkipTo.TabIndex = 1;
            this.buttonSkipTo.Text = "Run";
            this.buttonSkipTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSkipTo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSkipTo.UseVisualStyleBackColor = true;
            this.buttonSkipTo.Click += new System.EventHandler(this.buttonSkipTo_Click);
            // 
            // numericUpDownSkip
            // 
            this.numericUpDownSkip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownSkip.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownSkip.Location = new System.Drawing.Point(48, 25);
            this.numericUpDownSkip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDownSkip.Name = "numericUpDownSkip";
            this.numericUpDownSkip.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownSkip.TabIndex = 2;
            // 
            // labelOf
            // 
            this.labelOf.AutoSize = true;
            this.labelOf.Location = new System.Drawing.Point(172, 4);
            this.labelOf.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelOf.Name = "labelOf";
            this.labelOf.Size = new System.Drawing.Size(19, 13);
            this.labelOf.TabIndex = 3;
            this.labelOf.Text = "of ";
            // 
            // timerUI
            // 
            this.timerUI.Interval = 1000;
            this.timerUI.Tick += new System.EventHandler(this.timerUI_Tick);
            // 
            // buttonSkipToEnd
            // 
            this.buttonSkipToEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSkipToEnd.AutoSize = true;
            this.buttonSkipToEnd.Image = ((System.Drawing.Image)(resources.GetObject("buttonSkipToEnd.Image")));
            this.buttonSkipToEnd.Location = new System.Drawing.Point(4, 97);
            this.buttonSkipToEnd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonSkipToEnd.Name = "buttonSkipToEnd";
            this.buttonSkipToEnd.Size = new System.Drawing.Size(96, 23);
            this.buttonSkipToEnd.TabIndex = 6;
            this.buttonSkipToEnd.Text = "Run To End";
            this.buttonSkipToEnd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSkipToEnd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSkipToEnd.UseVisualStyleBackColor = true;
            this.buttonSkipToEnd.Click += new System.EventHandler(this.buttonSkipToEnd_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.AutoSize = true;
            this.buttonClose.Location = new System.Drawing.Point(202, 97);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(96, 23);
            this.buttonClose.TabIndex = 7;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBoxCurrent
            // 
            this.textBoxCurrent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCurrent.Location = new System.Drawing.Point(48, 2);
            this.textBoxCurrent.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxCurrent.Name = "textBoxCurrent";
            this.textBoxCurrent.ReadOnly = true;
            this.textBoxCurrent.Size = new System.Drawing.Size(120, 20);
            this.textBoxCurrent.TabIndex = 8;
            // 
            // TimeManagementSkipToControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxCurrent);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonSkipToEnd);
            this.Controls.Add(label2);
            this.Controls.Add(this.labelOf);
            this.Controls.Add(this.numericUpDownSkip);
            this.Controls.Add(this.buttonSkipTo);
            this.Controls.Add(label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "TimeManagementSkipToControl";
            this.Size = new System.Drawing.Size(300, 122);
            this.Load += new System.EventHandler(this.TimeManagementSkipToControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSkip)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSkipTo;
        private System.Windows.Forms.NumericUpDown numericUpDownSkip;
        private System.Windows.Forms.Label labelOf;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.Button buttonSkipToEnd;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxCurrent;
    }
}
