namespace CommonSupport
{
    partial class DragContainerControl
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
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.labelText = new System.Windows.Forms.Label();
            this.splitterRight = new CommonSupport.SplitterEx();
            this.splitterBottom = new CommonSupport.SplitterEx();
            this.splitterLeft = new CommonSupport.SplitterEx();
            this.splitterTop = new CommonSupport.SplitterEx();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 121);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(200, 479);
            this.panelLeft.TabIndex = 0;
            // 
            // panelRight
            // 
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(600, 121);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(200, 373);
            this.panelRight.TabIndex = 1;
            // 
            // panelBottom
            // 
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(206, 500);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(594, 100);
            this.panelBottom.TabIndex = 2;
            // 
            // panelTop
            // 
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 115);
            this.panelTop.TabIndex = 3;
            // 
            // panelCenter
            // 
            this.panelCenter.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(206, 121);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(388, 373);
            this.panelCenter.TabIndex = 8;
            // 
            // labelText
            // 
            this.labelText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelText.Enabled = false;
            this.labelText.Font = new System.Drawing.Font("Tahoma", 26.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelText.ForeColor = System.Drawing.Color.Gray;
            this.labelText.Location = new System.Drawing.Point(3, 13);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(794, 576);
            this.labelText.TabIndex = 21;
            this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitterRight
            // 
            this.splitterRight.BackColor = System.Drawing.SystemColors.Control;
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(594, 121);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(6, 373);
            this.splitterRight.TabIndex = 7;
            this.splitterRight.TabStop = false;
            // 
            // splitterBottom
            // 
            this.splitterBottom.BackColor = System.Drawing.SystemColors.Control;
            this.splitterBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterBottom.Location = new System.Drawing.Point(206, 494);
            this.splitterBottom.Name = "splitterBottom";
            this.splitterBottom.Size = new System.Drawing.Size(594, 6);
            this.splitterBottom.TabIndex = 5;
            this.splitterBottom.TabStop = false;
            // 
            // splitterLeft
            // 
            this.splitterLeft.BackColor = System.Drawing.SystemColors.Control;
            this.splitterLeft.Location = new System.Drawing.Point(200, 121);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(6, 479);
            this.splitterLeft.TabIndex = 4;
            this.splitterLeft.TabStop = false;
            // 
            // splitterTop
            // 
            this.splitterTop.BackColor = System.Drawing.SystemColors.Control;
            this.splitterTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitterTop.Location = new System.Drawing.Point(0, 115);
            this.splitterTop.Name = "splitterTop";
            this.splitterTop.Size = new System.Drawing.Size(800, 6);
            this.splitterTop.TabIndex = 6;
            this.splitterTop.TabStop = false;
            // 
            // DragContainerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.splitterBottom);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.splitterLeft);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.splitterTop);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.labelText);
            this.Name = "DragContainerControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.BackColorChanged += new System.EventHandler(this.DragContainerControl_BackColorChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.Label labelText;
        private SplitterEx splitterLeft;
        private SplitterEx splitterBottom;
        private SplitterEx splitterTop;
        private SplitterEx splitterRight;
    }
}
