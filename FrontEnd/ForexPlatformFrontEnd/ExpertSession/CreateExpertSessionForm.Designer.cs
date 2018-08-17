namespace ForexPlatformFrontEnd
{
    partial class CreateExpertSessionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateExpertSessionForm));
            this.createExpertSessionControl = new ForexPlatformFrontEnd.CreateExpertSessionControl2();
            this.SuspendLayout();
            // 
            // createExpertSessionControl
            // 
            this.createExpertSessionControl.AllowBackTestingSession = true;
            this.createExpertSessionControl.AllowGraphicsSession = true;
            this.createExpertSessionControl.AllowLiveSession = true;
            this.createExpertSessionControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.createExpertSessionControl.Host = null;
            this.createExpertSessionControl.Location = new System.Drawing.Point(0, 0);
            this.createExpertSessionControl.Margin = new System.Windows.Forms.Padding(2);
            this.createExpertSessionControl.MinimumSize = new System.Drawing.Size(400, 117);
            this.createExpertSessionControl.Name = "createExpertSessionControl";
            this.createExpertSessionControl.Size = new System.Drawing.Size(667, 423);
            this.createExpertSessionControl.TabIndex = 0;
            // 
            // CreateExpertSessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(667, 423);
            this.Controls.Add(this.createExpertSessionControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CreateExpertSessionForm";
            this.Text = "Initialize Expert";
            this.Load += new System.EventHandler(this.CreateExpertSessionForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateExpertSessionForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public CreateExpertSessionControl2 createExpertSessionControl;

    }
}