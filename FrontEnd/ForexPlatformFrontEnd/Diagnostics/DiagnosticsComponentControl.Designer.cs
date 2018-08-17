namespace ForexPlatformFrontEnd
{
    partial class DiagnosticsComponentControl
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
            this.tracerControl1 = new CommonSupport.TracerControl();
            this.applicationDiagnosticsInformationControl1 = new CommonSupport.DiagnosticsControl();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tracerControl1
            // 
            this.tracerControl1.DetailsVisible = false;
            this.tracerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tracerControl1.Location = new System.Drawing.Point(0, 0);
            this.tracerControl1.Name = "tracerControl1";
            this.tracerControl1.ShowDetails = false;
            this.tracerControl1.ShowMethodFilter = true;
            this.tracerControl1.Size = new System.Drawing.Size(800, 520);
            this.tracerControl1.TabIndex = 0;
            this.tracerControl1.Tracer = null;
            // 
            // applicationDiagnosticsInformationControl1
            // 
            this.applicationDiagnosticsInformationControl1.ImageName = "";
            this.applicationDiagnosticsInformationControl1.Location = new System.Drawing.Point(271, 112);
            this.applicationDiagnosticsInformationControl1.Margin = new System.Windows.Forms.Padding(2);
            this.applicationDiagnosticsInformationControl1.MinimumSize = new System.Drawing.Size(24, 24);
            this.applicationDiagnosticsInformationControl1.Name = "applicationDiagnosticsInformationControl1";
            this.applicationDiagnosticsInformationControl1.Size = new System.Drawing.Size(350, 184);
            this.applicationDiagnosticsInformationControl1.TabIndex = 1;
            // 
            // timerUI
            // 
            this.timerUI.Enabled = true;
            this.timerUI.Interval = 500;
            // 
            // DiagnosticsComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.applicationDiagnosticsInformationControl1);
            this.Controls.Add(this.tracerControl1);
            this.ImageName = "TV.PNG";
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DiagnosticsComponentControl";
            this.Size = new System.Drawing.Size(800, 520);
            this.Load += new System.EventHandler(this.DiagnosticsControl_Load);
            this.Controls.SetChildIndex(this.tracerControl1, 0);
            this.Controls.SetChildIndex(this.applicationDiagnosticsInformationControl1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommonSupport.TracerControl tracerControl1;
        private CommonSupport.DiagnosticsControl applicationDiagnosticsInformationControl1;
        private System.Windows.Forms.Timer timerUI;
    }
}
