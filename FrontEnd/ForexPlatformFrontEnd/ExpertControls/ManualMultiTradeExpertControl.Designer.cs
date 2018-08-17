namespace ForexPlatformFrontEnd
{
    partial class ManualMultiTradeExpertControl
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
            System.Windows.Forms.ToolStripLabel toolStripLabel2;
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManualMultiTradeExpertControl));
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonCloseSession = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCreateSession = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonWindowed = new System.Windows.Forms.ToolStripButton();
            toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            label1 = new System.Windows.Forms.Label();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.ForeColor = System.Drawing.SystemColors.GrayText;
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new System.Drawing.Size(48, 22);
            toolStripLabel2.Text = "Sessions";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(2, 25);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(403, 13);
            label1.TabIndex = 18;
            label1.Text = "This expert has no sessions. To create a session click \"New\" in the Sessions Menu" +
                ".";
            // 
            // toolStripMain
            // 
            this.toolStripMain.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripLabel2,
            this.toolStripButtonCloseSession,
            this.toolStripSeparator2,
            this.toolStripButtonCreateSession,
            this.toolStripSeparator4,
            this.toolStripButton1,
            this.toolStripButtonWindowed});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.Size = new System.Drawing.Size(800, 25);
            this.toolStripMain.TabIndex = 16;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // toolStripButtonCloseSession
            // 
            this.toolStripButtonCloseSession.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonCloseSession.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCloseSession.Image")));
            this.toolStripButtonCloseSession.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCloseSession.Name = "toolStripButtonCloseSession";
            this.toolStripButtonCloseSession.Size = new System.Drawing.Size(53, 22);
            this.toolStripButtonCloseSession.Text = "Close";
            this.toolStripButtonCloseSession.Click += new System.EventHandler(this.toolStripButtonDeleteSession_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCreateSession
            // 
            this.toolStripButtonCreateSession.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD2;
            this.toolStripButtonCreateSession.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateSession.Name = "toolStripButtonCreateSession";
            this.toolStripButtonCreateSession.Size = new System.Drawing.Size(48, 22);
            this.toolStripButtonCreateSession.Text = "New";
            this.toolStripButtonCreateSession.Click += new System.EventHandler(this.toolStripButtonCreateSession_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::ForexPlatformFrontEnd.Properties.Resources.PIN_GREY;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(96, 22);
            this.toolStripButton1.Tag = "del";
            this.toolStripButton1.Text = "AUDUSD[M60]";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButtonWindowed
            // 
            this.toolStripButtonWindowed.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonWindowed.Enabled = false;
            this.toolStripButtonWindowed.Image = global::ForexPlatformFrontEnd.Properties.Resources.photo_scenery;
            this.toolStripButtonWindowed.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonWindowed.Name = "toolStripButtonWindowed";
            this.toolStripButtonWindowed.Size = new System.Drawing.Size(77, 22);
            this.toolStripButtonWindowed.Text = "Windowed";
            this.toolStripButtonWindowed.Visible = false;
            // 
            // ManualTradeExpertControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(label1);
            this.Controls.Add(this.toolStripMain);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ManualTradeExpertControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreateSession;
        private System.Windows.Forms.ToolStripButton toolStripButtonCloseSession;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButtonWindowed;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

    }
}
