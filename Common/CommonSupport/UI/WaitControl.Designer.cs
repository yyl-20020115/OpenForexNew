namespace CommonSupport
{
    partial class WaitControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaitControl));
            this.labelMain = new System.Windows.Forms.Label();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMain
            // 
            this.labelMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelMain.Location = new System.Drawing.Point(0, 0);
            this.labelMain.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMain.Name = "labelMain";
            this.labelMain.Size = new System.Drawing.Size(533, 20);
            this.labelMain.TabIndex = 0;
            this.labelMain.Text = "label1";
            this.labelMain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMain.VisibleChanged += new System.EventHandler(this.labelMain_VisibleChanged);
            // 
            // timerUI
            // 
            this.timerUI.Enabled = true;
            this.timerUI.Interval = 1000;
            this.timerUI.Tick += new System.EventHandler(this.timerUI_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // WaitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelMain);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "WaitControl";
            this.Size = new System.Drawing.Size(533, 130);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelMain;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
