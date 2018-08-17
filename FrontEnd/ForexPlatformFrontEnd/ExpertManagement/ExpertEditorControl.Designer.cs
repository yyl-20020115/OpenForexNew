namespace ForexPlatformFrontEnd
{
    partial class ExpertEditorControl
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpertEditorControl));
            AIMS.Libraries.CodeEditor.WinForms.LineMarginRender lineMarginRender2 = new AIMS.Libraries.CodeEditor.WinForms.LineMarginRender();
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBuild = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonProperties = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCodeSnippets = new System.Windows.Forms.ToolStripDropDownButton();
            this.insertIndicatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syntaxDocument = new AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument(this.components);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.codeEditorControl = new AIMS.Libraries.CodeEditor.CodeEditorControl();
            this.listViewMessages = new System.Windows.Forms.ListView();
            this.Messages = new System.Windows.Forms.ColumnHeader();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStripLabelFileName = new System.Windows.Forms.ToolStripLabel();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEdit,
            this.toolStripSeparator4,
            this.toolStripButtonSave,
            toolStripSeparator3,
            this.toolStripButtonBuild,
            this.toolStripButtonProperties,
            this.toolStripButtonCodeSnippets,
            toolStripSeparator1,
            this.toolStripLabelFileName});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(800, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButtonEdit
            // 
            this.toolStripButtonEdit.CheckOnClick = true;
            this.toolStripButtonEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEdit.Image")));
            this.toolStripButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Size = new System.Drawing.Size(45, 22);
            this.toolStripButtonEdit.Text = "Edit";
            this.toolStripButtonEdit.Visible = false;
            this.toolStripButtonEdit.CheckStateChanged += new System.EventHandler(this.toolStripButtonEdit_CheckStateChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.Image = global::ForexPlatformFrontEnd.Properties.Resources.disk_blue;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(51, 22);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonBuild
            // 
            this.toolStripButtonBuild.Image = global::ForexPlatformFrontEnd.Properties.Resources.CUBES;
            this.toolStripButtonBuild.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBuild.Name = "toolStripButtonBuild";
            this.toolStripButtonBuild.Size = new System.Drawing.Size(49, 22);
            this.toolStripButtonBuild.Text = "Build";
            this.toolStripButtonBuild.Click += new System.EventHandler(this.toolStripButtonBuild_Click);
            // 
            // toolStripButtonProperties
            // 
            this.toolStripButtonProperties.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonProperties.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonProperties.Image")));
            this.toolStripButtonProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonProperties.Name = "toolStripButtonProperties";
            this.toolStripButtonProperties.Size = new System.Drawing.Size(76, 22);
            this.toolStripButtonProperties.Text = "Properties";
            this.toolStripButtonProperties.Click += new System.EventHandler(this.toolStripButtonProperties_Click);
            // 
            // toolStripButtonCodeSnippets
            // 
            this.toolStripButtonCodeSnippets.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonCodeSnippets.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertIndicatorToolStripMenuItem});
            this.toolStripButtonCodeSnippets.Enabled = false;
            this.toolStripButtonCodeSnippets.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCodeSnippets.Image")));
            this.toolStripButtonCodeSnippets.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCodeSnippets.Name = "toolStripButtonCodeSnippets";
            this.toolStripButtonCodeSnippets.Size = new System.Drawing.Size(132, 22);
            this.toolStripButtonCodeSnippets.Text = "Insert Code Snippet";
            this.toolStripButtonCodeSnippets.Visible = false;
            // 
            // insertIndicatorToolStripMenuItem
            // 
            this.insertIndicatorToolStripMenuItem.Name = "insertIndicatorToolStripMenuItem";
            this.insertIndicatorToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.insertIndicatorToolStripMenuItem.Text = "Insert Indicator";
            // 
            // syntaxDocument
            // 
            this.syntaxDocument.Lines = new string[] {
        ""};
            this.syntaxDocument.MaxUndoBufferSize = 1000;
            this.syntaxDocument.Modified = false;
            this.syntaxDocument.UndoStep = 0;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "ok");
            this.imageList.Images.SetKeyName(1, "error");
            // 
            // codeEditorControl
            // 
            this.codeEditorControl.ActiveView = AIMS.Libraries.CodeEditor.WinForms.ActiveView.BottomRight;
            this.codeEditorControl.AutoListPosition = null;
            this.codeEditorControl.AutoListSelectedText = "";
            this.codeEditorControl.AutoListVisible = false;
            this.codeEditorControl.CopyAsRTF = false;
            this.codeEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditorControl.Document = this.syntaxDocument;
            this.codeEditorControl.FileName = null;
            this.codeEditorControl.InfoTipCount = 1;
            this.codeEditorControl.InfoTipPosition = null;
            this.codeEditorControl.InfoTipSelectedIndex = 1;
            this.codeEditorControl.InfoTipVisible = false;
            lineMarginRender2.Bounds = new System.Drawing.Rectangle(19, 0, 19, 16);
            this.codeEditorControl.LineMarginRender = lineMarginRender2;
            this.codeEditorControl.Location = new System.Drawing.Point(0, 25);
            this.codeEditorControl.LockCursorUpdate = false;
            this.codeEditorControl.Name = "codeEditorControl";
            this.codeEditorControl.Saved = false;
            this.codeEditorControl.ShowScopeIndicator = false;
            this.codeEditorControl.Size = new System.Drawing.Size(800, 490);
            this.codeEditorControl.SmoothScroll = false;
            this.codeEditorControl.SplitviewH = -4;
            this.codeEditorControl.SplitviewV = -4;
            this.codeEditorControl.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(219)))), ((int)(((byte)(214)))));
            this.codeEditorControl.TabIndex = 0;
            this.codeEditorControl.Text = "codeEditorControl1";
            this.codeEditorControl.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            this.codeEditorControl.TextChanged += new System.EventHandler(this.codeEditorControl_TextChanged);
            // 
            // listViewMessages
            // 
            this.listViewMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Messages});
            this.listViewMessages.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listViewMessages.FullRowSelect = true;
            this.listViewMessages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewMessages.HideSelection = false;
            this.listViewMessages.Location = new System.Drawing.Point(0, 523);
            this.listViewMessages.MultiSelect = false;
            this.listViewMessages.Name = "listViewMessages";
            this.listViewMessages.Size = new System.Drawing.Size(800, 70);
            this.listViewMessages.SmallImageList = this.imageList;
            this.listViewMessages.TabIndex = 0;
            this.listViewMessages.UseCompatibleStateImageBehavior = false;
            this.listViewMessages.View = System.Windows.Forms.View.Details;
            this.listViewMessages.SelectedIndexChanged += new System.EventHandler(this.listViewMessages_SelectedIndexChanged);
            // 
            // Messages
            // 
            this.Messages.Text = "Compilation Messages";
            this.Messages.Width = 783;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 515);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(800, 8);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // toolStripLabelFileName
            // 
            this.toolStripLabelFileName.Name = "toolStripLabelFileName";
            this.toolStripLabelFileName.Size = new System.Drawing.Size(63, 22);
            this.toolStripLabelFileName.Text = "{File Name}";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ExpertEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.codeEditorControl);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.listViewMessages);
            this.Controls.Add(this.toolStrip);
            this.ImageName = "breakpoint_selection.png";
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ExpertEditorControl";
            this.Size = new System.Drawing.Size(800, 593);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton toolStripButtonBuild;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButtonCodeSnippets;
        private AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument syntaxDocument;
        private System.Windows.Forms.ToolStripButton toolStripButtonEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private AIMS.Libraries.CodeEditor.CodeEditorControl codeEditorControl;
        private System.Windows.Forms.ListView listViewMessages;
        private System.Windows.Forms.ColumnHeader Messages;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripButton toolStripButtonProperties;
        private System.Windows.Forms.ToolStripMenuItem insertIndicatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabelFileName;
    }
}
