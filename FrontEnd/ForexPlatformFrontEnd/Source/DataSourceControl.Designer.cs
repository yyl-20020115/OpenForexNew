namespace ForexPlatformFrontEnd.UI
{
    partial class DataSourceControl
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
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sessionInfosControl1 = new ForexPlatformFrontEnd.SessionInfosControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabelStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelName = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sessionInfosControl1
            // 
            this.sessionInfosControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionInfosControl1.Location = new System.Drawing.Point(0, 25);
            this.sessionInfosControl1.Margin = new System.Windows.Forms.Padding(2);
            this.sessionInfosControl1.Name = "sessionInfosControl1";
            this.sessionInfosControl1.SessionManager = null;
            this.sessionInfosControl1.Size = new System.Drawing.Size(800, 575);
            this.sessionInfosControl1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelStatus,
            this.toolStripSeparator1,
            this.toolStripLabelName});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabelStatus
            // 
            this.toolStripLabelStatus.Name = "toolStripLabelStatus";
            this.toolStripLabelStatus.Size = new System.Drawing.Size(11, 22);
            this.toolStripLabelStatus.Text = "-";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelName
            // 
            this.toolStripLabelName.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabelName.Name = "toolStripLabelName";
            this.toolStripLabelName.Size = new System.Drawing.Size(11, 22);
            this.toolStripLabelName.Text = "-";
            // 
            // DataSourceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sessionInfosControl1);
            this.Controls.Add(this.toolStrip1);
            this.ImageName = "cube_green.png";
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DataSourceControl";
            this.Load += new System.EventHandler(this.DataProviderSourceControl_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ForexPlatformFrontEnd.SessionInfosControl sessionInfosControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelName;

    }
}
