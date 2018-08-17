namespace CommonSupport
{
    partial class CombinedContainerControl
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CombinedContainerControl));
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripLabelTitle = new System.Windows.Forms.ToolStripLabel();
            this.toolStripDropDownButtonNew = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripButtonTitlesText = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparatorStart = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.floatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dragContainerControl = new CommonSupport.DragContainerControl();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMain.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            toolStripSeparator2.Visible = false;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            toolStripSeparator3.Visible = false;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            toolStripSeparator4.Visible = false;
            // 
            // toolStripMain
            // 
            this.toolStripMain.AllowDrop = true;
            this.toolStripMain.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonStart,
            this.toolStripLabelTitle,
            toolStripSeparator2,
            this.toolStripDropDownButtonNew,
            toolStripSeparator4,
            this.toolStripButtonTitlesText,
            toolStripSeparator3,
            this.toolStripButtonClose,
            this.toolStripSeparatorStart});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripMain.Size = new System.Drawing.Size(800, 25);
            this.toolStripMain.TabIndex = 0;
            this.toolStripMain.Text = "toolStrip1";
            this.toolStripMain.DragOver += new System.Windows.Forms.DragEventHandler(this.toolStripMain_DragOver);
            this.toolStripMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.toolStripMain_DragEnter);
            this.toolStripMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.toolStripMain_DragDrop);
            // 
            // toolStripButtonStart
            // 
            this.toolStripButtonStart.AutoSize = false;
            this.toolStripButtonStart.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.toolStripButtonStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStart.Image")));
            this.toolStripButtonStart.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Size = new System.Drawing.Size(86, 22);
            this.toolStripButtonStart.Text = "Start";
            // 
            // toolStripLabelTitle
            // 
            this.toolStripLabelTitle.BackColor = System.Drawing.Color.Transparent;
            this.toolStripLabelTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.toolStripLabelTitle.Name = "toolStripLabelTitle";
            this.toolStripLabelTitle.Size = new System.Drawing.Size(67, 22);
            this.toolStripLabelTitle.Text = "Components";
            this.toolStripLabelTitle.ToolTipText = "Components Tab Control";
            this.toolStripLabelTitle.Visible = false;
            // 
            // toolStripDropDownButtonNew
            // 
            this.toolStripDropDownButtonNew.Image = global::CommonSupport.Properties.Resources.ADD2;
            this.toolStripDropDownButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonNew.Name = "toolStripDropDownButtonNew";
            this.toolStripDropDownButtonNew.Size = new System.Drawing.Size(57, 22);
            this.toolStripDropDownButtonNew.Text = "New";
            this.toolStripDropDownButtonNew.ToolTipText = "New Component";
            this.toolStripDropDownButtonNew.Visible = false;
            // 
            // toolStripButtonTitlesText
            // 
            this.toolStripButtonTitlesText.Checked = true;
            this.toolStripButtonTitlesText.CheckOnClick = true;
            this.toolStripButtonTitlesText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonTitlesText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTitlesText.Name = "toolStripButtonTitlesText";
            this.toolStripButtonTitlesText.Size = new System.Drawing.Size(36, 22);
            this.toolStripButtonTitlesText.Text = "Titles";
            this.toolStripButtonTitlesText.ToolTipText = "Show Components Titles";
            this.toolStripButtonTitlesText.Visible = false;
            this.toolStripButtonTitlesText.Click += new System.EventHandler(this.toolStripButtonTitlesText_Click);
            // 
            // toolStripButtonClose
            // 
            this.toolStripButtonClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClose.Image = global::CommonSupport.Properties.Resources.button_cancel_12;
            this.toolStripButtonClose.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClose.Name = "toolStripButtonClose";
            this.toolStripButtonClose.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonClose.Text = "toolStripButton1";
            this.toolStripButtonClose.ToolTipText = "Close Component";
            this.toolStripButtonClose.MouseLeave += new System.EventHandler(this.toolStripButtonClose_MouseLeave);
            this.toolStripButtonClose.MouseEnter += new System.EventHandler(this.toolStripButtonClose_MouseEnter);
            this.toolStripButtonClose.Click += new System.EventHandler(this.toolStripButtonClose_Click);
            // 
            // toolStripSeparatorStart
            // 
            this.toolStripSeparatorStart.Name = "toolStripSeparatorStart";
            this.toolStripSeparatorStart.Size = new System.Drawing.Size(6, 25);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem,
            this.toolStripSeparator1,
            this.floatToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 54);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::CommonSupport.Properties.Resources.DELETE2;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(121, 6);
            // 
            // floatToolStripMenuItem
            // 
            this.floatToolStripMenuItem.Name = "floatToolStripMenuItem";
            this.floatToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.floatToolStripMenuItem.Text = "Float";
            this.floatToolStripMenuItem.Click += new System.EventHandler(this.floatToolStripMenuItem_Click);
            // 
            // dragContainerControl
            // 
            this.dragContainerControl.AllowDrop = true;
            this.dragContainerControl.AllowFloating = false;
            this.dragContainerControl.BackColor = System.Drawing.Color.WhiteSmoke;
            this.dragContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dragContainerControl.Location = new System.Drawing.Point(0, 25);
            this.dragContainerControl.MainAreaText = "";
            this.dragContainerControl.Name = "dragContainerControl";
            this.dragContainerControl.Size = new System.Drawing.Size(800, 575);
            this.dragContainerControl.TabIndex = 1;
            this.dragContainerControl.DragDrop += new System.Windows.Forms.DragEventHandler(this.dragContainerControl_DragDrop);
            this.dragContainerControl.DragControlRemoved += new CommonSupport.DragContainerControl.DragControlUpdatedDelegate(this.dragContainerControl_DragControlRemoved);
            // 
            // CombinedContainerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Controls.Add(this.dragContainerControl);
            this.Controls.Add(this.toolStripMain);
            this.Name = "CombinedContainerControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton toolStripButtonClose;
        public System.Windows.Forms.ToolStrip toolStripMain;
        public System.Windows.Forms.ToolStripLabel toolStripLabelTitle;
        public System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonNew;
        private DragContainerControl dragContainerControl;
        private System.Windows.Forms.ToolStripButton toolStripButtonTitlesText;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem floatToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorStart;
        public System.Windows.Forms.ToolStripDropDownButton toolStripButtonStart;
    }
}
