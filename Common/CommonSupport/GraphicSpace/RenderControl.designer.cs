namespace CommonSupport
{
    partial class RenderControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenderControl));
            this.buttonZoomAll = new System.Windows.Forms.Button();
            this.comboBoxZoom = new System.Windows.Forms.ComboBox();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonZoomAll
            // 
            this.buttonZoomAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonZoomAll.BackColor = System.Drawing.SystemColors.Control;
            this.buttonZoomAll.Enabled = false;
            this.buttonZoomAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonZoomAll.Image = ((System.Drawing.Image)(resources.GetObject("buttonZoomAll.Image")));
            this.buttonZoomAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonZoomAll.Location = new System.Drawing.Point(365, 4);
            this.buttonZoomAll.Margin = new System.Windows.Forms.Padding(4);
            this.buttonZoomAll.Name = "buttonZoomAll";
            this.buttonZoomAll.Size = new System.Drawing.Size(37, 26);
            this.buttonZoomAll.TabIndex = 15;
            this.buttonZoomAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonZoomAll.UseVisualStyleBackColor = false;
            // 
            // comboBoxZoom
            // 
            this.comboBoxZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxZoom.Items.AddRange(new object[] {
            "10%",
            "25%",
            "50%",
            "75%",
            "100%",
            "125%",
            "150%",
            "200%",
            "300%",
            "400%",
            "500%"});
            this.comboBoxZoom.Location = new System.Drawing.Point(494, 4);
            this.comboBoxZoom.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxZoom.Name = "comboBoxZoom";
            this.comboBoxZoom.Size = new System.Drawing.Size(76, 24);
            this.comboBoxZoom.TabIndex = 14;
            this.comboBoxZoom.SelectedIndexChanged += new System.EventHandler(this.comboBoxZoom_SelectedIndexChanged);
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonZoomOut.BackColor = System.Drawing.SystemColors.Control;
            this.buttonZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("buttonZoomOut.Image")));
            this.buttonZoomOut.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonZoomOut.Location = new System.Drawing.Point(450, 4);
            this.buttonZoomOut.Margin = new System.Windows.Forms.Padding(4);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(37, 26);
            this.buttonZoomOut.TabIndex = 13;
            this.buttonZoomOut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonZoomOut.UseVisualStyleBackColor = false;
            this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonZoomIn.BackColor = System.Drawing.SystemColors.Control;
            this.buttonZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("buttonZoomIn.Image")));
            this.buttonZoomIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonZoomIn.Location = new System.Drawing.Point(408, 4);
            this.buttonZoomIn.Margin = new System.Windows.Forms.Padding(4);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(37, 26);
            this.buttonZoomIn.TabIndex = 12;
            this.buttonZoomIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonZoomIn.UseVisualStyleBackColor = false;
            this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
            // 
            // RenderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonZoomAll);
            this.Controls.Add(this.comboBoxZoom);
            this.Controls.Add(this.buttonZoomOut);
            this.Controls.Add(this.buttonZoomIn);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "RenderControl";
            this.Size = new System.Drawing.Size(576, 447);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonZoomAll;
        private System.Windows.Forms.ComboBox comboBoxZoom;
        private System.Windows.Forms.Button buttonZoomOut;
        private System.Windows.Forms.Button buttonZoomIn;
    }
}
