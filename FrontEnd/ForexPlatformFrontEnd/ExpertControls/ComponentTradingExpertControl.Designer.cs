namespace ForexPlatformFrontEnd
{
    partial class ComponentTradingExpertControl
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
            this.dragContainerControl1 = new CommonSupport.DragContainerControl();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddTradingSession = new System.Windows.Forms.ToolStripButton();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // dragContainerControl1
            // 
            this.dragContainerControl1.AllowDrop = true;
            this.dragContainerControl1.BackColor = System.Drawing.SystemColors.Control;
            this.dragContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dragContainerControl1.Location = new System.Drawing.Point(0, 25);
            this.dragContainerControl1.Name = "dragContainerControl1";
            this.dragContainerControl1.Size = new System.Drawing.Size(800, 575);
            this.dragContainerControl1.TabIndex = 0;
            // 
            // toolStripMain
            // 
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddTradingSession});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.Size = new System.Drawing.Size(800, 25);
            this.toolStripMain.TabIndex = 20;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // toolStripButtonAddTradingSession
            // 
            this.toolStripButtonAddTradingSession.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD2;
            this.toolStripButtonAddTradingSession.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddTradingSession.Name = "toolStripButtonAddTradingSession";
            this.toolStripButtonAddTradingSession.Size = new System.Drawing.Size(124, 22);
            this.toolStripButtonAddTradingSession.Text = "Add Trading Session";
            this.toolStripButtonAddTradingSession.Click += new System.EventHandler(this.toolStripButtonCreate_Click);
            // 
            // ComponentTradingExpertControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dragContainerControl1);
            this.Controls.Add(this.toolStripMain);
            this.ImageName = "FACTORY.PNG";
            this.Name = "ComponentTradingExpertControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommonSupport.DragContainerControl dragContainerControl1;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddTradingSession;
    }
}
