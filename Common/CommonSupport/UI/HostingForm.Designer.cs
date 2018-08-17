namespace CommonSupport
{
    partial class HostingForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required baseMethod for Designer support - do not modify
        /// the contents of this baseMethod with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostingForm));
            this.buttonClose = new System.Windows.Forms.Button();
            this.panelButtonClose = new System.Windows.Forms.Panel();
            this.panelButtonClose.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(37, 5);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // panelButtonClose
            // 
            this.panelButtonClose.Controls.Add(this.buttonClose);
            this.panelButtonClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtonClose.Location = new System.Drawing.Point(0, 54);
            this.panelButtonClose.Name = "panelButtonClose";
            this.panelButtonClose.Size = new System.Drawing.Size(115, 31);
            this.panelButtonClose.TabIndex = 0;
            this.panelButtonClose.Visible = false;
            // 
            // HostingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(115, 85);
            this.Controls.Add(this.panelButtonClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HostingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HostingForm";
            this.Load += new System.EventHandler(this.HostingForm_Load);
            this.panelButtonClose.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelButtonClose;
        private System.Windows.Forms.Button buttonClose;

    }
}