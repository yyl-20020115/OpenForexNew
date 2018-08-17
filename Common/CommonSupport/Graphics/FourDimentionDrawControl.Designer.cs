namespace CommonSupport
{
    partial class FourDimentionDrawControl
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
            this.trackBarHorizontal = new System.Windows.Forms.TrackBar();
            this.trackBarVertical = new System.Windows.Forms.TrackBar();
            this.label3D = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.twoDDrawControl = new TwoDimentionDrawControl();
            this.checkBoxColorMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHorizontal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVertical)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarHorizontal
            // 
            this.trackBarHorizontal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarHorizontal.Location = new System.Drawing.Point(32, 4);
            this.trackBarHorizontal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trackBarHorizontal.Name = "trackBarHorizontal";
            this.trackBarHorizontal.Size = new System.Drawing.Size(764, 56);
            this.trackBarHorizontal.TabIndex = 1;
            this.trackBarHorizontal.ValueChanged += new System.EventHandler(this.trackBarHorizontal_ValueChanged);
            // 
            // trackBarVertical
            // 
            this.trackBarVertical.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarVertical.Location = new System.Drawing.Point(740, 63);
            this.trackBarVertical.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trackBarVertical.Name = "trackBarVertical";
            this.trackBarVertical.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarVertical.Size = new System.Drawing.Size(56, 672);
            this.trackBarVertical.TabIndex = 2;
            this.trackBarVertical.ValueChanged += new System.EventHandler(this.trackBarVertical_ValueChanged);
            // 
            // label3D
            // 
            this.label3D.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3D.AutoSize = true;
            this.label3D.Location = new System.Drawing.Point(741, 43);
            this.label3D.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3D.Name = "label3D";
            this.label3D.Size = new System.Drawing.Size(26, 17);
            this.label3D.TabIndex = 3;
            this.label3D.Text = "3D";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "4D";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(11, 43);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(13, 17);
            this.labelStatus.TabIndex = 5;
            this.labelStatus.Text = "-";
            // 
            // twoDDrawControl
            // 
            this.twoDDrawControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.twoDDrawControl.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.twoDDrawControl.Location = new System.Drawing.Point(4, 63);
            this.twoDDrawControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.twoDDrawControl.MaxValue = -1.7976931348623157E+308;
            this.twoDDrawControl.MinValue = 1.7976931348623157E+308;
            this.twoDDrawControl.Name = "twoDDrawControl";
            this.twoDDrawControl.RenderingMode = TwoDimentionDrawControl.RenderingModeEnum.Normal;
            this.twoDDrawControl.Size = new System.Drawing.Size(728, 672);
            this.twoDDrawControl.TabIndex = 0;
            this.twoDDrawControl.Text = "_2DDrawControl1";
            // 
            // checkBoxColorMode
            // 
            this.checkBoxColorMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxColorMode.AutoSize = true;
            this.checkBoxColorMode.Checked = true;
            this.checkBoxColorMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxColorMode.Location = new System.Drawing.Point(616, 38);
            this.checkBoxColorMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxColorMode.Name = "checkBoxColorMode";
            this.checkBoxColorMode.Size = new System.Drawing.Size(114, 21);
            this.checkBoxColorMode.TabIndex = 6;
            this.checkBoxColorMode.Text = "BiColor mode";
            this.checkBoxColorMode.UseVisualStyleBackColor = true;
            this.checkBoxColorMode.CheckedChanged += new System.EventHandler(this.checkBoxColorMode_CheckedChanged);
            // 
            // FourDDrawControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxColorMode);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3D);
            this.Controls.Add(this.trackBarVertical);
            this.Controls.Add(this.trackBarHorizontal);
            this.Controls.Add(this.twoDDrawControl);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FourDDrawControl";
            this.Size = new System.Drawing.Size(800, 738);
            this.Load += new System.EventHandler(this.TwoDPlusControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHorizontal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVertical)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TwoDimentionDrawControl twoDDrawControl;
        private System.Windows.Forms.TrackBar trackBarHorizontal;
        private System.Windows.Forms.TrackBar trackBarVertical;
        private System.Windows.Forms.Label label3D;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.CheckBox checkBoxColorMode;
    }
}
