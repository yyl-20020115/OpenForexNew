using System.Windows.Forms;
namespace CommonSupport
{
    partial class NewsItemControl
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabelFullStory = new System.Windows.Forms.LinkLabel();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.WhiteSmoke;
            this.labelTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTitle.ForeColor = System.Drawing.Color.LightSlateGray;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(800, 20);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "[Title]";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnHandleMouseAction);
            this.labelTitle.Click += new System.EventHandler(this.labelTitle_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelInfo.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelInfo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelInfo.Location = new System.Drawing.Point(0, 20);
            this.labelInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(800, 20);
            this.labelInfo.TabIndex = 3;
            this.labelInfo.Tag = "published [Date] by [Author]";
            this.labelInfo.Text = "published 12.08.2008, 19:31 by Pavmarat Shankisirimi";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelInfo.Click += new System.EventHandler(this.labelInfo_Click);
            // 
            // toolTip
            // 
            this.toolTip.IsBalloon = true;
            // 
            // linkLabelFullStory
            // 
            this.linkLabelFullStory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelFullStory.AutoSize = true;
            this.linkLabelFullStory.BackColor = System.Drawing.Color.WhiteSmoke;
            this.linkLabelFullStory.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabelFullStory.LinkColor = System.Drawing.Color.Navy;
            this.linkLabelFullStory.Location = new System.Drawing.Point(708, 4);
            this.linkLabelFullStory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabelFullStory.Name = "linkLabelFullStory";
            this.linkLabelFullStory.Size = new System.Drawing.Size(90, 13);
            this.linkLabelFullStory.TabIndex = 4;
            this.linkLabelFullStory.TabStop = true;
            this.linkLabelFullStory.Text = "Read full story";
            this.linkLabelFullStory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFullStory_LinkClicked);
            // 
            // textBoxText
            // 
            this.textBoxText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxText.Location = new System.Drawing.Point(3, 43);
            this.textBoxText.Multiline = true;
            this.textBoxText.Name = "textBoxText";
            this.textBoxText.Size = new System.Drawing.Size(794, 129);
            this.textBoxText.TabIndex = 8;
            // 
            // NewsItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.textBoxText);
            this.Controls.Add(this.linkLabelFullStory);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.labelTitle);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "NewsItemControl";
            this.Size = new System.Drawing.Size(800, 175);
            this.Load += new System.EventHandler(this.RSSItemControl_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnHandleMouseAction);
            this.Resize += new System.EventHandler(this.RSSItemControl_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.LinkLabel linkLabelFullStory;
        private TextBox textBoxText;

    }
}
