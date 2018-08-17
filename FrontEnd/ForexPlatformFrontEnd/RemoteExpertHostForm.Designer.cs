namespace ForexPlatformFrontEnd
{
    partial class RemoteExpertHostForm
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteExpertHostForm));
          this.ConnectCheckBox = new System.Windows.Forms.CheckBox();
          this.URLTextBox = new System.Windows.Forms.TextBox();
          this.URLLabel = new System.Windows.Forms.Label();
          this.statusStrip1 = new System.Windows.Forms.StatusStrip();
          this.MessageToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
          this.SessionCheckBox = new System.Windows.Forms.CheckBox();
          this.SessionNameLabel = new System.Windows.Forms.Label();
          this.SessionNameTextBox = new System.Windows.Forms.TextBox();
          this.SymbolLabel = new System.Windows.Forms.Label();
          this.SymbolsComboBox = new System.Windows.Forms.ComboBox();
          this.LotLabel = new System.Windows.Forms.Label();
          this.LotTextBox = new System.Windows.Forms.TextBox();
          this.DecimalPlacesTextBox = new System.Windows.Forms.TextBox();
          this.DecimalPlacesLabel = new System.Windows.Forms.Label();
          this.label1 = new System.Windows.Forms.Label();
          this.ExpertNameTextBox = new System.Windows.Forms.TextBox();
          this.statusStrip1.SuspendLayout();
          this.SuspendLayout();
          // 
          // ConnectCheckBox
          // 
          this.ConnectCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.ConnectCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
          this.ConnectCheckBox.AutoSize = true;
          this.ConnectCheckBox.Location = new System.Drawing.Point(563, 7);
          this.ConnectCheckBox.Name = "ConnectCheckBox";
          this.ConnectCheckBox.Size = new System.Drawing.Size(57, 22);
          this.ConnectCheckBox.TabIndex = 0;
          this.ConnectCheckBox.Text = "Connect";
          this.ConnectCheckBox.UseVisualStyleBackColor = true;
          this.ConnectCheckBox.CheckedChanged += new System.EventHandler(this.ConnectCheckBox_CheckedChanged);
          // 
          // URLTextBox
          // 
          this.URLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.URLTextBox.Location = new System.Drawing.Point(259, 9);
          this.URLTextBox.Name = "URLTextBox";
          this.URLTextBox.Size = new System.Drawing.Size(297, 21);
          this.URLTextBox.TabIndex = 1;
          // 
          // URLLabel
          // 
          this.URLLabel.AutoSize = true;
          this.URLLabel.Location = new System.Drawing.Point(182, 12);
          this.URLLabel.Name = "URLLabel";
          this.URLLabel.Size = new System.Drawing.Size(71, 12);
          this.URLLabel.TabIndex = 2;
          this.URLLabel.Text = "Server URL:";
          // 
          // statusStrip1
          // 
          this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MessageToolStripStatusLabel});
          this.statusStrip1.Location = new System.Drawing.Point(0, 74);
          this.statusStrip1.Name = "statusStrip1";
          this.statusStrip1.Size = new System.Drawing.Size(629, 22);
          this.statusStrip1.TabIndex = 3;
          this.statusStrip1.Text = "statusStrip1";
          // 
          // MessageToolStripStatusLabel
          // 
          this.MessageToolStripStatusLabel.Name = "MessageToolStripStatusLabel";
          this.MessageToolStripStatusLabel.Size = new System.Drawing.Size(61, 17);
          this.MessageToolStripStatusLabel.Text = "Message";
          // 
          // SessionCheckBox
          // 
          this.SessionCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.SessionCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
          this.SessionCheckBox.AutoSize = true;
          this.SessionCheckBox.Enabled = false;
          this.SessionCheckBox.Location = new System.Drawing.Point(563, 35);
          this.SessionCheckBox.Name = "SessionCheckBox";
          this.SessionCheckBox.Size = new System.Drawing.Size(57, 22);
          this.SessionCheckBox.TabIndex = 4;
          this.SessionCheckBox.Text = "Session";
          this.SessionCheckBox.UseVisualStyleBackColor = true;
          this.SessionCheckBox.CheckedChanged += new System.EventHandler(this.SessionCheckBox_CheckedChanged);
          // 
          // SessionNameLabel
          // 
          this.SessionNameLabel.AutoSize = true;
          this.SessionNameLabel.Location = new System.Drawing.Point(2, 40);
          this.SessionNameLabel.Name = "SessionNameLabel";
          this.SessionNameLabel.Size = new System.Drawing.Size(83, 12);
          this.SessionNameLabel.TabIndex = 5;
          this.SessionNameLabel.Text = "Session Name:";
          // 
          // SessionNameTextBox
          // 
          this.SessionNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.SessionNameTextBox.Location = new System.Drawing.Point(93, 37);
          this.SessionNameTextBox.Name = "SessionNameTextBox";
          this.SessionNameTextBox.Size = new System.Drawing.Size(83, 21);
          this.SessionNameTextBox.TabIndex = 6;
          this.SessionNameTextBox.Text = "Default";
          // 
          // SymbolLabel
          // 
          this.SymbolLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.SymbolLabel.AutoSize = true;
          this.SymbolLabel.Location = new System.Drawing.Point(182, 40);
          this.SymbolLabel.Name = "SymbolLabel";
          this.SymbolLabel.Size = new System.Drawing.Size(47, 12);
          this.SymbolLabel.TabIndex = 7;
          this.SymbolLabel.Text = "Symbol:";
          // 
          // SymbolsComboBox
          // 
          this.SymbolsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.SymbolsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.SymbolsComboBox.FormattingEnabled = true;
          this.SymbolsComboBox.Items.AddRange(new object[] {
            "EURUSD",
            "GBPUSD",
            "USDJPY",
            "USDCAD",
            "USDCHF",
            "AUDCAD",
            "AUDJPY",
            "AUDUSD",
            "EURAUD",
            "GBPAUD",
            "AUDNZD",
            "EURNZD",
            "NZDJPY",
            "NZDUSD",
            "CADCHF",
            "CADJPY",
            "CHFAUD",
            "CHFJPY",
            "EURCAD",
            "EURCHF",
            "EURGBP",
            "EURJPY",
            "GBPCAD",
            "GBPCHF",
            "GBPJPY",
            "USDCZK",
            "BRTUSD",
            "GASUSD",
            "OILUSD",
            "XAGUSD",
            "XAUUSD"});
          this.SymbolsComboBox.Location = new System.Drawing.Point(235, 37);
          this.SymbolsComboBox.MaxDropDownItems = 12;
          this.SymbolsComboBox.Name = "SymbolsComboBox";
          this.SymbolsComboBox.Size = new System.Drawing.Size(66, 20);
          this.SymbolsComboBox.TabIndex = 8;
          // 
          // LotLabel
          // 
          this.LotLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.LotLabel.AutoSize = true;
          this.LotLabel.Location = new System.Drawing.Point(307, 41);
          this.LotLabel.Name = "LotLabel";
          this.LotLabel.Size = new System.Drawing.Size(29, 12);
          this.LotLabel.TabIndex = 9;
          this.LotLabel.Text = "Lot:";
          // 
          // LotTextBox
          // 
          this.LotTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.LotTextBox.Location = new System.Drawing.Point(342, 37);
          this.LotTextBox.Name = "LotTextBox";
          this.LotTextBox.Size = new System.Drawing.Size(53, 21);
          this.LotTextBox.TabIndex = 10;
          this.LotTextBox.Text = "10000";
          this.LotTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
          this.LotTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.LotTextBox_Validating);
          // 
          // DecimalPlacesTextBox
          // 
          this.DecimalPlacesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.DecimalPlacesTextBox.Location = new System.Drawing.Point(503, 37);
          this.DecimalPlacesTextBox.Name = "DecimalPlacesTextBox";
          this.DecimalPlacesTextBox.Size = new System.Drawing.Size(53, 21);
          this.DecimalPlacesTextBox.TabIndex = 12;
          this.DecimalPlacesTextBox.Text = "4";
          this.DecimalPlacesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
          this.DecimalPlacesTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.DecimalPlacesTextBox_Validating);
          // 
          // DecimalPlacesLabel
          // 
          this.DecimalPlacesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
          this.DecimalPlacesLabel.AutoSize = true;
          this.DecimalPlacesLabel.Location = new System.Drawing.Point(402, 42);
          this.DecimalPlacesLabel.Name = "DecimalPlacesLabel";
          this.DecimalPlacesLabel.Size = new System.Drawing.Size(95, 12);
          this.DecimalPlacesLabel.TabIndex = 11;
          this.DecimalPlacesLabel.Text = "Decimal Places:";
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(4, 12);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(77, 12);
          this.label1.TabIndex = 17;
          this.label1.Text = "Expert Name:";
          // 
          // ExpertNameTextBox
          // 
          this.ExpertNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.ExpertNameTextBox.Location = new System.Drawing.Point(93, 9);
          this.ExpertNameTextBox.Name = "ExpertNameTextBox";
          this.ExpertNameTextBox.Size = new System.Drawing.Size(83, 21);
          this.ExpertNameTextBox.TabIndex = 16;
          this.ExpertNameTextBox.Text = "AIExpert";
          // 
          // RemoteExpertHostForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(629, 96);
          this.Controls.Add(this.label1);
          this.Controls.Add(this.ExpertNameTextBox);
          this.Controls.Add(this.DecimalPlacesTextBox);
          this.Controls.Add(this.DecimalPlacesLabel);
          this.Controls.Add(this.LotTextBox);
          this.Controls.Add(this.LotLabel);
          this.Controls.Add(this.SymbolsComboBox);
          this.Controls.Add(this.SymbolLabel);
          this.Controls.Add(this.SessionNameTextBox);
          this.Controls.Add(this.SessionNameLabel);
          this.Controls.Add(this.SessionCheckBox);
          this.Controls.Add(this.statusStrip1);
          this.Controls.Add(this.URLLabel);
          this.Controls.Add(this.URLTextBox);
          this.Controls.Add(this.ConnectCheckBox);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.Margin = new System.Windows.Forms.Padding(2);
          this.MaximizeBox = false;
          this.Name = "RemoteExpertHostForm";
          this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
          this.Tag = "";
          this.Text = "Open Forex Platform Expert Platform";
          this.TopMost = true;
          this.statusStrip1.ResumeLayout(false);
          this.statusStrip1.PerformLayout();
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ConnectCheckBox;
        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.Label URLLabel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel MessageToolStripStatusLabel;
        private System.Windows.Forms.CheckBox SessionCheckBox;
        private System.Windows.Forms.Label SessionNameLabel;
        private System.Windows.Forms.TextBox SessionNameTextBox;
        private System.Windows.Forms.Label SymbolLabel;
        private System.Windows.Forms.ComboBox SymbolsComboBox;
        private System.Windows.Forms.Label LotLabel;
        private System.Windows.Forms.TextBox LotTextBox;
        private System.Windows.Forms.TextBox DecimalPlacesTextBox;
        private System.Windows.Forms.Label DecimalPlacesLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ExpertNameTextBox;



    }
}