namespace CommonSupport
{
    partial class DragStripControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DragStripControl));
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.centerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.topToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.leftToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rightToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.floatingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.centerToolStripMenuItem,
            this.toolStripSeparator4,
            this.topToolStripMenuItem1,
            this.leftToolStripMenuItem1,
            this.rightToolStripMenuItem1,
            this.bottomToolStripMenuItem1,
            this.toolStripSeparator5,
            this.floatingToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            this.contextMenuStripMain.Size = new System.Drawing.Size(173, 170);
            this.contextMenuStripMain.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripMain_Opening);
            // 
            // centerToolStripMenuItem
            // 
            this.centerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("centerToolStripMenuItem.Image")));
            this.centerToolStripMenuItem.Name = "centerToolStripMenuItem";
            this.centerToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.centerToolStripMenuItem.Text = "Dock Center Pane";
            this.centerToolStripMenuItem.Click += new System.EventHandler(this.centerToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(169, 6);
            // 
            // topToolStripMenuItem1
            // 
            this.topToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("topToolStripMenuItem1.Image")));
            this.topToolStripMenuItem1.Name = "topToolStripMenuItem1";
            this.topToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.topToolStripMenuItem1.Text = "Dock Top Pane";
            this.topToolStripMenuItem1.Click += new System.EventHandler(this.topToolStripMenuItem1_Click);
            // 
            // leftToolStripMenuItem1
            // 
            this.leftToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("leftToolStripMenuItem1.Image")));
            this.leftToolStripMenuItem1.Name = "leftToolStripMenuItem1";
            this.leftToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.leftToolStripMenuItem1.Text = "Dock Left Pane";
            this.leftToolStripMenuItem1.Click += new System.EventHandler(this.leftToolStripMenuItem1_Click);
            // 
            // rightToolStripMenuItem1
            // 
            this.rightToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("rightToolStripMenuItem1.Image")));
            this.rightToolStripMenuItem1.Name = "rightToolStripMenuItem1";
            this.rightToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.rightToolStripMenuItem1.Text = "Dock Right Pane";
            this.rightToolStripMenuItem1.Click += new System.EventHandler(this.rightToolStripMenuItem1_Click);
            // 
            // bottomToolStripMenuItem1
            // 
            this.bottomToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("bottomToolStripMenuItem1.Image")));
            this.bottomToolStripMenuItem1.Name = "bottomToolStripMenuItem1";
            this.bottomToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.bottomToolStripMenuItem1.Text = "Dock Bottom Pane";
            this.bottomToolStripMenuItem1.Click += new System.EventHandler(this.bottomToolStripMenuItem1_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(169, 6);
            // 
            // floatingToolStripMenuItem
            // 
            this.floatingToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("floatingToolStripMenuItem.Image")));
            this.floatingToolStripMenuItem.Name = "floatingToolStripMenuItem";
            this.floatingToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.floatingToolStripMenuItem.Text = "Floating";
            this.floatingToolStripMenuItem.Click += new System.EventHandler(this.floatingToolStripMenuItem_Click);
            // 
            // DragStripControl
            // 
            this.ContextMenuStrip = this.contextMenuStripMain;
            this.Name = "DragStripControl";
            this.contextMenuStripMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripMain;
        private System.Windows.Forms.ToolStripMenuItem topToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem leftToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem rightToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem bottomToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem centerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem floatingToolStripMenuItem;

    }
}
