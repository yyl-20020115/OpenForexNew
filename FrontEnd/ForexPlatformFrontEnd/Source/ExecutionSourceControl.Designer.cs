namespace ForexPlatformFrontEnd
{
    partial class ExecutionSourceControl
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
            this.components = new System.ComponentModel.Container();
            this.sessionInfosControl1 = new ForexPlatformFrontEnd.SessionInfosControl();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // sessionInfosControl1
            // 
            this.sessionInfosControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionInfosControl1.Location = new System.Drawing.Point(0, 0);
            this.sessionInfosControl1.Margin = new System.Windows.Forms.Padding(2);
            this.sessionInfosControl1.Name = "sessionInfosControl1";
            this.sessionInfosControl1.SessionManager = null;
            this.sessionInfosControl1.Size = new System.Drawing.Size(800, 600);
            this.sessionInfosControl1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.sessionInfosControl1, "Sessions available for operation on this source.");
            // 
            // ExecutionSourceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sessionInfosControl1);
            this.ImageName = "cube_yellow.png";
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ExecutionSourceControl";
            this.ResumeLayout(false);

        }

        #endregion

        private ForexPlatformFrontEnd.SessionInfosControl sessionInfosControl1;
        private System.Windows.Forms.ToolTip toolTip1;

    }
}
