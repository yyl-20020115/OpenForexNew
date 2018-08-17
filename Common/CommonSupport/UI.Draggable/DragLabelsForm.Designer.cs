namespace CommonSupport
{
    partial class DragLabelsForm
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelDockLeft = new System.Windows.Forms.Label();
            this.labelDockCenter = new System.Windows.Forms.Label();
            this.labelDockBottom = new System.Windows.Forms.Label();
            this.labelDockTop = new System.Windows.Forms.Label();
            this.labelDockRight = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelDockLeft
            // 
            this.labelDockLeft.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelDockLeft.BackColor = System.Drawing.Color.Orange;
            this.labelDockLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDockLeft.Image = global::CommonSupport.Properties.Resources.DockIndicator_PanelLeft;
            this.labelDockLeft.Location = new System.Drawing.Point(30, 275);
            this.labelDockLeft.Name = "labelDockLeft";
            this.labelDockLeft.Size = new System.Drawing.Size(100, 50);
            this.labelDockLeft.TabIndex = 28;
            this.labelDockLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelDockCenter
            // 
            this.labelDockCenter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelDockCenter.BackColor = System.Drawing.Color.FloralWhite;
            this.labelDockCenter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDockCenter.Image = global::CommonSupport.Properties.Resources.DockIndicator_PanelFill;
            this.labelDockCenter.Location = new System.Drawing.Point(350, 275);
            this.labelDockCenter.Name = "labelDockCenter";
            this.labelDockCenter.Size = new System.Drawing.Size(100, 50);
            this.labelDockCenter.TabIndex = 27;
            this.labelDockCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelDockBottom
            // 
            this.labelDockBottom.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelDockBottom.BackColor = System.Drawing.Color.FloralWhite;
            this.labelDockBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDockBottom.Image = global::CommonSupport.Properties.Resources.DockIndicator_PanelBottom;
            this.labelDockBottom.Location = new System.Drawing.Point(350, 520);
            this.labelDockBottom.Name = "labelDockBottom";
            this.labelDockBottom.Size = new System.Drawing.Size(100, 50);
            this.labelDockBottom.TabIndex = 26;
            this.labelDockBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelDockTop
            // 
            this.labelDockTop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelDockTop.BackColor = System.Drawing.Color.FloralWhite;
            this.labelDockTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDockTop.Image = global::CommonSupport.Properties.Resources.DockIndicator_PanelTop;
            this.labelDockTop.Location = new System.Drawing.Point(350, 30);
            this.labelDockTop.Name = "labelDockTop";
            this.labelDockTop.Size = new System.Drawing.Size(100, 50);
            this.labelDockTop.TabIndex = 25;
            this.labelDockTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelDockRight
            // 
            this.labelDockRight.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelDockRight.BackColor = System.Drawing.Color.Moccasin;
            this.labelDockRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDockRight.Image = global::CommonSupport.Properties.Resources.DockIndicator_PanelRight;
            this.labelDockRight.Location = new System.Drawing.Point(670, 275);
            this.labelDockRight.Name = "labelDockRight";
            this.labelDockRight.Size = new System.Drawing.Size(100, 50);
            this.labelDockRight.TabIndex = 24;
            this.labelDockRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DragLabelsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.labelDockLeft);
            this.Controls.Add(this.labelDockCenter);
            this.Controls.Add(this.labelDockBottom);
            this.Controls.Add(this.labelDockTop);
            this.Controls.Add(this.labelDockRight);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "DragLabelsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "DragLabelsForm";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelDockLeft;
        private System.Windows.Forms.Label labelDockCenter;
        private System.Windows.Forms.Label labelDockBottom;
        private System.Windows.Forms.Label labelDockTop;
        private System.Windows.Forms.Label labelDockRight;

    }
}