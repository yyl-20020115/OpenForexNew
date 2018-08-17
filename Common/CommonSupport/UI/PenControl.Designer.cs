namespace CommonSupport
{
    partial class PenControl
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
            this.comboBoxDash = new System.Windows.Forms.ComboBox();
            this.labelColor = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(3, 6);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(74, 17);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Pen Name";
            // 
            // comboBoxDash
            // 
            this.comboBoxDash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDash.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDash.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxDash.FormattingEnabled = true;
            this.comboBoxDash.Location = new System.Drawing.Point(99, 32);
            this.comboBoxDash.Name = "comboBoxDash";
            this.comboBoxDash.Size = new System.Drawing.Size(106, 24);
            this.comboBoxDash.TabIndex = 1;
            this.comboBoxDash.SelectedIndexChanged += new System.EventHandler(this.comboBoxDash_SelectedIndexChanged);
            // 
            // labelColor
            // 
            this.labelColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelColor.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.labelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.labelColor.Location = new System.Drawing.Point(99, 3);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(106, 23);
            this.labelColor.TabIndex = 2;
            this.labelColor.Text = "           ";
            this.labelColor.Click += new System.EventHandler(this.labelColor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Line";
            // 
            // PenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelColor);
            this.Controls.Add(this.comboBoxDash);
            this.Controls.Add(this.labelName);
            this.Name = "PenControl";
            this.Size = new System.Drawing.Size(208, 65);
            this.Load += new System.EventHandler(this.PenControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.ComboBox comboBoxDash;
        private System.Windows.Forms.Label labelColor;
        private System.Windows.Forms.Label label1;
    }
}
