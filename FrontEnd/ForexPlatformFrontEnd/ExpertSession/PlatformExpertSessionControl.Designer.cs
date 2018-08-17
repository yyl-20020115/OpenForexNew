using CommonFinancial;
namespace ForexPlatformFrontEnd
{
    partial class PlatformExpertSessionControl
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlatformExpertSessionControl));
            this.chartControl = new CommonFinancial.ChartControl();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonIndicators = new System.Windows.Forms.ToolStripButton();
            this.timeManagementToolStrip1 = new ForexPlatformFrontEnd.TimeManagementToolStrip();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // chartControl
            // 
            this.chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl.Location = new System.Drawing.Point(0, 25);
            this.chartControl.Margin = new System.Windows.Forms.Padding(2);
            this.chartControl.Name = "chartControl";
            this.chartControl.Size = new System.Drawing.Size(800, 495);
            this.chartControl.TabIndex = 9;
            this.chartControl.Visible = false;
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonIndicators,
            toolStripSeparator1});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.Size = new System.Drawing.Size(800, 25);
            this.toolStripMain.TabIndex = 20;
            // 
            // toolStripButtonIndicators
            // 
            this.toolStripButtonIndicators.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonIndicators.Image")));
            this.toolStripButtonIndicators.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonIndicators.Name = "toolStripButtonIndicators";
            this.toolStripButtonIndicators.Size = new System.Drawing.Size(75, 22);
            this.toolStripButtonIndicators.Text = "Indicators";
            this.toolStripButtonIndicators.Visible = false;
            this.toolStripButtonIndicators.Click += new System.EventHandler(this.toolStripButtonIndicators_Click);
            // 
            // timeManagementToolStrip1
            // 
            this.timeManagementToolStrip1.Controller = null;
            this.timeManagementToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.timeManagementToolStrip1.Enabled = false;
            this.timeManagementToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.timeManagementToolStrip1.Location = new System.Drawing.Point(42, 146);
            this.timeManagementToolStrip1.Name = "timeManagementToolStrip1";
            this.timeManagementToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.timeManagementToolStrip1.Size = new System.Drawing.Size(825, 25);
            this.timeManagementToolStrip1.TabIndex = 19;
            this.timeManagementToolStrip1.Text = "timeManagementToolStrip1";
            // 
            // PlatformExpertSessionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.timeManagementToolStrip1);
            this.Controls.Add(this.chartControl);
            this.Controls.Add(this.toolStripMain);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PlatformExpertSessionControl";
            this.Load += new System.EventHandler(this.ExpertSessionControl_Load);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommonFinancial.ChartControl chartControl;
        private TimeManagementToolStrip timeManagementToolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonIndicators;
        public System.Windows.Forms.ToolStrip toolStripMain;
    }
}
