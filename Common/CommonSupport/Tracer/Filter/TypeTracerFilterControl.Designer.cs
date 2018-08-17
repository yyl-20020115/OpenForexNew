namespace CommonSupport
{
    partial class TypeTracerFilterControl
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
            System.Windows.Forms.ToolStripLabel toolStripLabel1;
            System.Windows.Forms.ToolStripLabel toolStripLabel2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TypeTracerFilterControl));
            this.listViewTypes = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.imageListMain = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonCheckNone = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCheckAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCheckImportant = new System.Windows.Forms.ToolStripButton();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.ForeColor = System.Drawing.SystemColors.GrayText;
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(58, 22);
            toolStripLabel1.Text = "Type Filter";
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            toolStripLabel2.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new System.Drawing.Size(36, 22);
            toolStripLabel2.Text = "Check";
            // 
            // listViewTypes
            // 
            this.listViewTypes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewTypes.CheckBoxes = true;
            this.listViewTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTypes.FullRowSelect = true;
            this.listViewTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewTypes.LargeImageList = this.imageListMain;
            this.listViewTypes.Location = new System.Drawing.Point(0, 25);
            this.listViewTypes.Name = "listViewTypes";
            this.listViewTypes.Size = new System.Drawing.Size(269, 283);
            this.listViewTypes.SmallImageList = this.imageListMain;
            this.listViewTypes.TabIndex = 0;
            this.listViewTypes.UseCompatibleStateImageBehavior = false;
            this.listViewTypes.View = System.Windows.Forms.View.Details;
            this.listViewTypes.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewTypes_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Item Type";
            this.columnHeader1.Width = 160;
            // 
            // imageListMain
            // 
            this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
            this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMain.Images.SetKeyName(0, "ERROR.PNG");
            this.imageListMain.Images.SetKeyName(1, "WARNING.PNG");
            this.imageListMain.Images.SetKeyName(2, "navigate_right.png");
            this.imageListMain.Images.SetKeyName(3, "navigate_left.png");
            this.imageListMain.Images.SetKeyName(4, "dot.gray.png");
            this.imageListMain.Images.SetKeyName(5, "CONSOLE.PNG");
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCheckNone,
            this.toolStripButtonCheckAll,
            this.toolStripButtonCheckImportant,
            toolStripLabel1,
            toolStripLabel2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(269, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonCheckNone
            // 
            this.toolStripButtonCheckNone.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonCheckNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCheckNone.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCheckNone.Image")));
            this.toolStripButtonCheckNone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCheckNone.Name = "toolStripButtonCheckNone";
            this.toolStripButtonCheckNone.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCheckNone.Text = "None";
            this.toolStripButtonCheckNone.ToolTipText = "Check None";
            this.toolStripButtonCheckNone.Click += new System.EventHandler(this.toolStripButtonCheckNone_Click);
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
            // toolStripButtonCheckImportant
            // 
            this.toolStripButtonCheckImportant.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonCheckImportant.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCheckImportant.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCheckImportant.Image")));
            this.toolStripButtonCheckImportant.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCheckImportant.Name = "toolStripButtonCheckImportant";
            this.toolStripButtonCheckImportant.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCheckImportant.Text = "Important";
            this.toolStripButtonCheckImportant.ToolTipText = "Check Important";
            this.toolStripButtonCheckImportant.Click += new System.EventHandler(this.toolStripButtonCheckImportant_Click);
            // 
            // TypeTracerFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewTypes);
            this.Controls.Add(this.toolStrip1);
            this.Name = "TypeTracerFilterControl";
            this.Size = new System.Drawing.Size(269, 308);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewTypes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ImageList imageListMain;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCheckAll;
        private System.Windows.Forms.ToolStripButton toolStripButtonCheckNone;
        private System.Windows.Forms.ToolStripButton toolStripButtonCheckImportant;
    }
}
