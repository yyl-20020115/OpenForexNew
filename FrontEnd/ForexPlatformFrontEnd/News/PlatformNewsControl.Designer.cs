namespace ForexPlatformFrontEnd
{
    partial class PlatformNewsControl
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
            this.newsManagerControl1 = new CommonSupport.NewsManagerControl();
            this.SuspendLayout();
            // 
            // newsManagerControl1
            // 
            this.newsManagerControl1.AutoScroll = true;
            this.newsManagerControl1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.newsManagerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newsManagerControl1.Location = new System.Drawing.Point(0, 0);
            this.newsManagerControl1.Manager = null;
            this.newsManagerControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.newsManagerControl1.MaximumItemsShown = 50;
            this.newsManagerControl1.Name = "newsManagerControl1";
            this.newsManagerControl1.Size = new System.Drawing.Size(800, 520);
            this.newsManagerControl1.TabIndex = 0;
            // 
            // PlatformNewsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.newsManagerControl1);
            this.Text = "Financial news.";
            this.ImageName = "environment.png";
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PlatformNewsControl";
            this.Size = new System.Drawing.Size(800, 520);
            this.Name = "News";
            this.ResumeLayout(false);

        }

        #endregion

        private CommonSupport.NewsManagerControl newsManagerControl1;

    }
}
