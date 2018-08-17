namespace CommonSupport
{
    partial class MethodTracerFilterControl
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
            System.Windows.Forms.ToolStripLabel toolStripButton1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MethodTracerFilterControl));
            this.treeView = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonUnCheckAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCheckAll = new System.Windows.Forms.ToolStripButton();
            toolStripButton1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton1.ForeColor = System.Drawing.SystemColors.GrayText;
            toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new System.Drawing.Size(59, 22);
            toolStripButton1.Text = "Class Filter";
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.CheckBoxes = true;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.FullRowSelect = true;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(322, 375);
            this.treeView.TabIndex = 0;
            this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripButton1,
            this.toolStripButtonUnCheckAll,
            this.toolStripButtonCheckAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(322, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonUnCheckAll
            // 
            this.toolStripButtonUnCheckAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonUnCheckAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUnCheckAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUnCheckAll.Image")));
            this.toolStripButtonUnCheckAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUnCheckAll.Name = "toolStripButtonUnCheckAll";
            this.toolStripButtonUnCheckAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUnCheckAll.Text = "None";
            this.toolStripButtonUnCheckAll.ToolTipText = "Check None";
            this.toolStripButtonUnCheckAll.Click += new System.EventHandler(this.toolStripButtonUnCheckAll_Click);
            // 
            // toolStripButtonCheckAll
            // 
            this.toolStripButtonCheckAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonCheckAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCheckAll.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCheckAll.Image")));
            this.toolStripButtonCheckAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCheckAll.Name = "toolStripButtonCheckAll";
            this.toolStripButtonCheckAll.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCheckAll.Text = "All";
            this.toolStripButtonCheckAll.ToolTipText = "Check All";
            this.toolStripButtonCheckAll.Click += new System.EventHandler(this.toolStripButtonCheckAll_Click);
            // 
            // MethodTracerFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MethodTracerFilterControl";
            this.Size = new System.Drawing.Size(322, 400);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCheckAll;
        private System.Windows.Forms.ToolStripButton toolStripButtonUnCheckAll;
    }
}
