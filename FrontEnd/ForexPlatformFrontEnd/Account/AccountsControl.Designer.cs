namespace ForexPlatformFrontEnd
{
    partial class AccountsControl
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
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSwitchMode = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelAccount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxAccounts = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator2,
            this.toolStripButtonSwitchMode,
            this.toolStripSeparator1,
            this.toolStripLabelAccount,
            this.toolStripComboBoxAccounts});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.Size = new System.Drawing.Size(800, 25);
            this.toolStripMain.TabIndex = 1;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(51, 22);
            this.toolStripLabel1.Text = "Accounts";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSwitchMode
            // 
            this.toolStripButtonSwitchMode.Image = global::ForexPlatformFrontEnd.Properties.Resources.elements1;
            this.toolStripButtonSwitchMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSwitchMode.Name = "toolStripButtonSwitchMode";
            this.toolStripButtonSwitchMode.Size = new System.Drawing.Size(90, 22);
            this.toolStripButtonSwitchMode.Text = "Display Mode";
            this.toolStripButtonSwitchMode.ToolTipText = "Switch between compact and full display mode.";
            this.toolStripButtonSwitchMode.Click += new System.EventHandler(this.toolStripButtonSwitchMode_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelAccount
            // 
            this.toolStripLabelAccount.Name = "toolStripLabelAccount";
            this.toolStripLabelAccount.Size = new System.Drawing.Size(48, 22);
            this.toolStripLabelAccount.Text = "Selected";
            // 
            // toolStripComboBoxAccounts
            // 
            this.toolStripComboBoxAccounts.Enabled = false;
            this.toolStripComboBoxAccounts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripComboBoxAccounts.Name = "toolStripComboBoxAccounts";
            this.toolStripComboBoxAccounts.Size = new System.Drawing.Size(421, 25);
            this.toolStripComboBoxAccounts.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxAccounts_SelectedIndexChanged);
            // 
            // AccountsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.toolStripMain);
            this.Name = "ExecutionAccountsControl";
            this.Size = new System.Drawing.Size(800, 58);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonSwitchMode;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxAccounts;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelAccount;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
