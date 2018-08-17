namespace CommonSupport
{
    partial class BrushControl
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
            this.labelName = new System.Windows.Forms.Label();
            this.labelColor = new System.Windows.Forms.Label();
            this.labelEmpty = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(2, 5);
            this.labelName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(65, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Brush Name";
            // 
            // labelColor
            // 
            this.labelColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelColor.BackColor = System.Drawing.Color.Transparent;
            this.labelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.labelColor.Location = new System.Drawing.Point(145, 2);
            this.labelColor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(80, 19);
            this.labelColor.TabIndex = 2;
            this.labelColor.Text = "           ";
            this.labelColor.Click += new System.EventHandler(this.labelColor_Click);
            // 
            // labelEmpty
            // 
            this.labelEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEmpty.AutoSize = true;
            this.labelEmpty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelEmpty.ForeColor = System.Drawing.Color.DarkBlue;
            this.labelEmpty.Location = new System.Drawing.Point(105, 5);
            this.labelEmpty.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelEmpty.Name = "labelEmpty";
            this.labelEmpty.Size = new System.Drawing.Size(36, 13);
            this.labelEmpty.TabIndex = 3;
            this.labelEmpty.Text = "Empty";
            this.labelEmpty.Click += new System.EventHandler(this.labelEmpty_Click);
            // 
            // BrushControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelEmpty);
            this.Controls.Add(this.labelColor);
            this.Controls.Add(this.labelName);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "BrushControl";
            this.Size = new System.Drawing.Size(227, 24);
            this.Load += new System.EventHandler(this.BrushControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelColor;
        private System.Windows.Forms.Label labelEmpty;
    }
}
