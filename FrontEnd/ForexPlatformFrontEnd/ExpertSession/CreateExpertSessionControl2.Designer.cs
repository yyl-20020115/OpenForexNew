namespace ForexPlatformFrontEnd
{
    partial class CreateExpertSessionControl2
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
            this.labelExecutionSource = new System.Windows.Forms.Label();
            this.comboBoxOrderExecutionSources = new System.Windows.Forms.ComboBox();
            this.textBoxSessionDescription = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.symbolSelectControl = new ForexPlatformFrontEnd.SymbolSelectControl();
            this.radioButtonSimulationTrading = new System.Windows.Forms.RadioButton();
            this.radioButtonGraphicsOnly = new System.Windows.Forms.RadioButton();
            this.radioButtonLiveTrading = new System.Windows.Forms.RadioButton();
            this.buttonCreateSession = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelExecutionSource
            // 
            this.labelExecutionSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelExecutionSource.AutoSize = true;
            this.labelExecutionSource.Location = new System.Drawing.Point(3, 412);
            this.labelExecutionSource.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelExecutionSource.Name = "labelExecutionSource";
            this.labelExecutionSource.Size = new System.Drawing.Size(83, 13);
            this.labelExecutionSource.TabIndex = 16;
            this.labelExecutionSource.Text = "Order Execution";
            this.labelExecutionSource.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBoxOrderExecutionSources
            // 
            this.comboBoxOrderExecutionSources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxOrderExecutionSources.DropDownHeight = 250;
            this.comboBoxOrderExecutionSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOrderExecutionSources.Enabled = false;
            this.comboBoxOrderExecutionSources.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxOrderExecutionSources.FormattingEnabled = true;
            this.comboBoxOrderExecutionSources.IntegralHeight = false;
            this.comboBoxOrderExecutionSources.Location = new System.Drawing.Point(6, 427);
            this.comboBoxOrderExecutionSources.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxOrderExecutionSources.Name = "comboBoxOrderExecutionSources";
            this.comboBoxOrderExecutionSources.Size = new System.Drawing.Size(590, 21);
            this.comboBoxOrderExecutionSources.TabIndex = 15;
            this.comboBoxOrderExecutionSources.SelectedIndexChanged += new System.EventHandler(this.comboBoxOrderExecutionSources_SelectedIndexChanged);
            // 
            // textBoxSessionDescription
            // 
            this.textBoxSessionDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSessionDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxSessionDescription.ForeColor = System.Drawing.SystemColors.InfoText;
            this.textBoxSessionDescription.Location = new System.Drawing.Point(160, 3);
            this.textBoxSessionDescription.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSessionDescription.Multiline = true;
            this.textBoxSessionDescription.Name = "textBoxSessionDescription";
            this.textBoxSessionDescription.ReadOnly = true;
            this.textBoxSessionDescription.Size = new System.Drawing.Size(436, 76);
            this.textBoxSessionDescription.TabIndex = 23;
            this.textBoxSessionDescription.Text = "Note";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.symbolSelectControl);
            this.groupBox1.Location = new System.Drawing.Point(6, 84);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(590, 325);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Symbol";
            // 
            // symbolSelectControl
            // 
            this.symbolSelectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.symbolSelectControl.Host = null;
            this.symbolSelectControl.ImmediateSelectionMode = true;
            this.symbolSelectControl.Location = new System.Drawing.Point(3, 16);
            this.symbolSelectControl.MultiSelect = false;
            this.symbolSelectControl.Name = "symbolSelectControl";
            this.symbolSelectControl.ShowSelectButton = false;
            this.symbolSelectControl.Size = new System.Drawing.Size(584, 306);
            this.symbolSelectControl.TabIndex = 24;
            this.symbolSelectControl.SelectedSymbolsChangedEvent += new ForexPlatformFrontEnd.SymbolSelectControl.SelectedSymbolChangedDelegate(this.symbolSelectControl_SelectedSymbolChangedEvent);
            // 
            // radioButtonSimulationTrading
            // 
            this.radioButtonSimulationTrading.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonSimulationTrading.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioButtonSimulationTrading.Image = global::ForexPlatformFrontEnd.Properties.Resources.currency_dollar_grayscale;
            this.radioButtonSimulationTrading.Location = new System.Drawing.Point(6, 30);
            this.radioButtonSimulationTrading.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonSimulationTrading.Name = "radioButtonSimulationTrading";
            this.radioButtonSimulationTrading.Size = new System.Drawing.Size(150, 23);
            this.radioButtonSimulationTrading.TabIndex = 20;
            this.radioButtonSimulationTrading.Text = " Back Testing";
            this.radioButtonSimulationTrading.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonSimulationTrading.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.radioButtonSimulationTrading.UseVisualStyleBackColor = true;
            this.radioButtonSimulationTrading.CheckedChanged += new System.EventHandler(this.radioButtonType_CheckedChanged);
            // 
            // radioButtonGraphicsOnly
            // 
            this.radioButtonGraphicsOnly.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonGraphicsOnly.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioButtonGraphicsOnly.Image = global::ForexPlatformFrontEnd.Properties.Resources.CHART;
            this.radioButtonGraphicsOnly.Location = new System.Drawing.Point(5, 3);
            this.radioButtonGraphicsOnly.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonGraphicsOnly.Name = "radioButtonGraphicsOnly";
            this.radioButtonGraphicsOnly.Size = new System.Drawing.Size(150, 23);
            this.radioButtonGraphicsOnly.TabIndex = 19;
            this.radioButtonGraphicsOnly.Text = "Graphics";
            this.radioButtonGraphicsOnly.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonGraphicsOnly.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.radioButtonGraphicsOnly.UseVisualStyleBackColor = true;
            this.radioButtonGraphicsOnly.CheckedChanged += new System.EventHandler(this.radioButtonType_CheckedChanged);
            // 
            // radioButtonLiveTrading
            // 
            this.radioButtonLiveTrading.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonLiveTrading.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioButtonLiveTrading.Image = global::ForexPlatformFrontEnd.Properties.Resources.currency_dollar1;
            this.radioButtonLiveTrading.Location = new System.Drawing.Point(5, 57);
            this.radioButtonLiveTrading.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonLiveTrading.Name = "radioButtonLiveTrading";
            this.radioButtonLiveTrading.Size = new System.Drawing.Size(150, 22);
            this.radioButtonLiveTrading.TabIndex = 18;
            this.radioButtonLiveTrading.Text = "Live";
            this.radioButtonLiveTrading.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonLiveTrading.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.radioButtonLiveTrading.UseVisualStyleBackColor = true;
            this.radioButtonLiveTrading.CheckedChanged += new System.EventHandler(this.radioButtonType_CheckedChanged);
            // 
            // buttonCreateSession
            // 
            this.buttonCreateSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateSession.AutoSize = true;
            this.buttonCreateSession.Enabled = false;
            this.buttonCreateSession.Image = global::ForexPlatformFrontEnd.Properties.Resources.CHECK2;
            this.buttonCreateSession.Location = new System.Drawing.Point(368, 452);
            this.buttonCreateSession.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCreateSession.Name = "buttonCreateSession";
            this.buttonCreateSession.Size = new System.Drawing.Size(112, 24);
            this.buttonCreateSession.TabIndex = 17;
            this.buttonCreateSession.Text = "OK";
            this.buttonCreateSession.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCreateSession.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonCreateSession.UseVisualStyleBackColor = true;
            this.buttonCreateSession.Click += new System.EventHandler(this.buttonCreateSession_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonCancel.Location = new System.Drawing.Point(484, 452);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 24);
            this.buttonCancel.TabIndex = 17;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // CreateExpertSessionControl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxSessionDescription);
            this.Controls.Add(this.radioButtonSimulationTrading);
            this.Controls.Add(this.radioButtonGraphicsOnly);
            this.Controls.Add(this.radioButtonLiveTrading);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCreateSession);
            this.Controls.Add(this.labelExecutionSource);
            this.Controls.Add(this.comboBoxOrderExecutionSources);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(450, 168);
            this.Name = "CreateExpertSessionControl2";
            this.Size = new System.Drawing.Size(600, 478);
            this.Load += new System.EventHandler(this.CreateExpertSessionControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonSimulationTrading;
        private System.Windows.Forms.RadioButton radioButtonGraphicsOnly;
        private System.Windows.Forms.RadioButton radioButtonLiveTrading;
        private System.Windows.Forms.ComboBox comboBoxOrderExecutionSources;
        private System.Windows.Forms.Label labelExecutionSource;
        public System.Windows.Forms.Button buttonCreateSession;
        private System.Windows.Forms.TextBox textBoxSessionDescription;
        private SymbolSelectControl symbolSelectControl;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button buttonCancel;

    }
}
