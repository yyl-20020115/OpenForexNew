using CommonSupport;
namespace ForexPlatformFrontEnd
{
    partial class ModifyOrderControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModifyOrderControl));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.numericUpDownStopLoss = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTakeProfit = new System.Windows.Forms.NumericUpDown();
            this.checkBoxStopLoss = new System.Windows.Forms.CheckBox();
            this.checkBoxTakeProfit = new System.Windows.Forms.CheckBox();
            this.checkBoxSlippage = new System.Windows.Forms.CheckBox();
            this.numericUpDownSlippage = new System.Windows.Forms.NumericUpDown();
            this.labelPrice = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonModify = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxId = new System.Windows.Forms.TextBox();
            this.textBoxSymbol = new System.Windows.Forms.TextBox();
            this.numericUpDownVolume = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownPendingPrice = new System.Windows.Forms.NumericUpDown();
            this.groupBoxPending = new System.Windows.Forms.GroupBox();
            this.buttonOpeningPricePriceHelper = new System.Windows.Forms.Button();
            this.buttonStopLossPriceHelper = new System.Windows.Forms.Button();
            this.buttonTakeProfitPriceHelper = new System.Windows.Forms.Button();
            this.labelVolume = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStopLoss)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTakeProfit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSlippage)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPendingPrice)).BeginInit();
            this.groupBoxPending.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Symbol";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Volume";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Image = ((System.Drawing.Image)(resources.GetObject("buttonClose.Image")));
            this.buttonClose.Location = new System.Drawing.Point(2, 211);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(413, 26);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Visible = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // numericUpDownStopLoss
            // 
            this.numericUpDownStopLoss.DecimalPlaces = 4;
            this.numericUpDownStopLoss.Enabled = false;
            this.numericUpDownStopLoss.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericUpDownStopLoss.Location = new System.Drawing.Point(79, 50);
            this.numericUpDownStopLoss.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownStopLoss.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownStopLoss.Name = "numericUpDownStopLoss";
            this.numericUpDownStopLoss.Size = new System.Drawing.Size(81, 20);
            this.numericUpDownStopLoss.TabIndex = 8;
            this.numericUpDownStopLoss.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownTakeProfit
            // 
            this.numericUpDownTakeProfit.DecimalPlaces = 4;
            this.numericUpDownTakeProfit.Enabled = false;
            this.numericUpDownTakeProfit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericUpDownTakeProfit.Location = new System.Drawing.Point(79, 74);
            this.numericUpDownTakeProfit.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownTakeProfit.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numericUpDownTakeProfit.Name = "numericUpDownTakeProfit";
            this.numericUpDownTakeProfit.Size = new System.Drawing.Size(81, 20);
            this.numericUpDownTakeProfit.TabIndex = 10;
            this.numericUpDownTakeProfit.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // checkBoxStopLoss
            // 
            this.checkBoxStopLoss.AutoSize = true;
            this.checkBoxStopLoss.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBoxStopLoss.Location = new System.Drawing.Point(5, 50);
            this.checkBoxStopLoss.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxStopLoss.Name = "checkBoxStopLoss";
            this.checkBoxStopLoss.Size = new System.Drawing.Size(71, 17);
            this.checkBoxStopLoss.TabIndex = 17;
            this.checkBoxStopLoss.Text = "Stop Loss";
            this.checkBoxStopLoss.UseVisualStyleBackColor = true;
            this.checkBoxStopLoss.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxTakeProfit
            // 
            this.checkBoxTakeProfit.AutoSize = true;
            this.checkBoxTakeProfit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBoxTakeProfit.Location = new System.Drawing.Point(5, 74);
            this.checkBoxTakeProfit.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxTakeProfit.Name = "checkBoxTakeProfit";
            this.checkBoxTakeProfit.Size = new System.Drawing.Size(76, 17);
            this.checkBoxTakeProfit.TabIndex = 18;
            this.checkBoxTakeProfit.Text = "Take Profit";
            this.checkBoxTakeProfit.UseVisualStyleBackColor = true;
            this.checkBoxTakeProfit.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxSlippage
            // 
            this.checkBoxSlippage.AutoSize = true;
            this.checkBoxSlippage.Enabled = false;
            this.checkBoxSlippage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBoxSlippage.Location = new System.Drawing.Point(5, 99);
            this.checkBoxSlippage.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSlippage.Name = "checkBoxSlippage";
            this.checkBoxSlippage.Size = new System.Drawing.Size(65, 17);
            this.checkBoxSlippage.TabIndex = 19;
            this.checkBoxSlippage.Text = "Slippage";
            this.checkBoxSlippage.UseVisualStyleBackColor = true;
            this.checkBoxSlippage.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // numericUpDownSlippage
            // 
            this.numericUpDownSlippage.DecimalPlaces = 4;
            this.numericUpDownSlippage.Enabled = false;
            this.numericUpDownSlippage.Location = new System.Drawing.Point(79, 98);
            this.numericUpDownSlippage.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownSlippage.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownSlippage.Name = "numericUpDownSlippage";
            this.numericUpDownSlippage.Size = new System.Drawing.Size(81, 20);
            this.numericUpDownSlippage.TabIndex = 20;
            // 
            // labelPrice
            // 
            this.labelPrice.BackColor = System.Drawing.Color.Transparent;
            this.labelPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.labelPrice.Location = new System.Drawing.Point(2, 15);
            this.labelPrice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPrice.Name = "labelPrice";
            this.labelPrice.Size = new System.Drawing.Size(216, 40);
            this.labelPrice.TabIndex = 24;
            this.labelPrice.Text = "---.-- / ---.--";
            this.labelPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.labelPrice);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(197, 48);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(220, 57);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Market Price";
            // 
            // textBoxComment
            // 
            this.textBoxComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxComment.Location = new System.Drawing.Point(3, 161);
            this.textBoxComment.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxComment.Multiline = true;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(412, 46);
            this.textBoxComment.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 146);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Comment";
            // 
            // buttonModify
            // 
            this.buttonModify.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonModify.AutoSize = true;
            this.buttonModify.Image = ((System.Drawing.Image)(resources.GetObject("buttonModify.Image")));
            this.buttonModify.Location = new System.Drawing.Point(3, 211);
            this.buttonModify.Margin = new System.Windows.Forms.Padding(2);
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.Size = new System.Drawing.Size(225, 26);
            this.buttonModify.TabIndex = 26;
            this.buttonModify.Text = "Modify";
            this.buttonModify.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonModify.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonModify.UseVisualStyleBackColor = true;
            this.buttonModify.Visible = false;
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 4);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Id";
            // 
            // textBoxId
            // 
            this.textBoxId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxId.Location = new System.Drawing.Point(183, 2);
            this.textBoxId.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxId.Name = "textBoxId";
            this.textBoxId.ReadOnly = true;
            this.textBoxId.Size = new System.Drawing.Size(232, 20);
            this.textBoxId.TabIndex = 28;
            // 
            // textBoxSymbol
            // 
            this.textBoxSymbol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSymbol.Location = new System.Drawing.Point(47, 2);
            this.textBoxSymbol.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSymbol.Name = "textBoxSymbol";
            this.textBoxSymbol.ReadOnly = true;
            this.textBoxSymbol.Size = new System.Drawing.Size(113, 20);
            this.textBoxSymbol.TabIndex = 29;
            // 
            // numericUpDownVolume
            // 
            this.numericUpDownVolume.DecimalPlaces = 2;
            this.numericUpDownVolume.Enabled = false;
            this.numericUpDownVolume.Location = new System.Drawing.Point(79, 26);
            this.numericUpDownVolume.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownVolume.Maximum = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.numericUpDownVolume.Name = "numericUpDownVolume";
            this.numericUpDownVolume.Size = new System.Drawing.Size(81, 20);
            this.numericUpDownVolume.TabIndex = 32;
            this.numericUpDownVolume.ValueChanged += new System.EventHandler(this.numericUpDownVolume_ValueChanged);
            this.numericUpDownVolume.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownVolume_KeyDown);
            // 
            // numericUpDownPendingPrice
            // 
            this.numericUpDownPendingPrice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownPendingPrice.DecimalPlaces = 4;
            this.numericUpDownPendingPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.numericUpDownPendingPrice.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericUpDownPendingPrice.Location = new System.Drawing.Point(37, 17);
            this.numericUpDownPendingPrice.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPendingPrice.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownPendingPrice.Name = "numericUpDownPendingPrice";
            this.numericUpDownPendingPrice.Size = new System.Drawing.Size(121, 26);
            this.numericUpDownPendingPrice.TabIndex = 37;
            this.numericUpDownPendingPrice.ValueChanged += new System.EventHandler(this.numericUpDownPendingPrice_ValueChanged);
            // 
            // groupBoxPending
            // 
            this.groupBoxPending.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPending.Controls.Add(this.buttonOpeningPricePriceHelper);
            this.groupBoxPending.Controls.Add(this.numericUpDownPendingPrice);
            this.groupBoxPending.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxPending.Location = new System.Drawing.Point(197, 109);
            this.groupBoxPending.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxPending.Name = "groupBoxPending";
            this.groupBoxPending.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxPending.Size = new System.Drawing.Size(218, 50);
            this.groupBoxPending.TabIndex = 39;
            this.groupBoxPending.TabStop = false;
            this.groupBoxPending.Text = "Pending Opening Price";
            // 
            // buttonOpeningPricePriceHelper
            // 
            this.buttonOpeningPricePriceHelper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpeningPricePriceHelper.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOpeningPricePriceHelper.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOpeningPricePriceHelper.Image = global::ForexPlatformFrontEnd.Properties.Resources.UP_DOWN;
            this.buttonOpeningPricePriceHelper.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonOpeningPricePriceHelper.Location = new System.Drawing.Point(162, 19);
            this.buttonOpeningPricePriceHelper.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOpeningPricePriceHelper.Name = "buttonOpeningPricePriceHelper";
            this.buttonOpeningPricePriceHelper.Size = new System.Drawing.Size(24, 22);
            this.buttonOpeningPricePriceHelper.TabIndex = 43;
            this.buttonOpeningPricePriceHelper.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonOpeningPricePriceHelper.UseVisualStyleBackColor = true;
            this.buttonOpeningPricePriceHelper.Click += new System.EventHandler(this.buttonOpeningPricePriceHelper_Click);
            // 
            // buttonStopLossPriceHelper
            // 
            this.buttonStopLossPriceHelper.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonStopLossPriceHelper.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonStopLossPriceHelper.Image = global::ForexPlatformFrontEnd.Properties.Resources.UP_DOWN;
            this.buttonStopLossPriceHelper.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonStopLossPriceHelper.Location = new System.Drawing.Point(164, 48);
            this.buttonStopLossPriceHelper.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStopLossPriceHelper.Name = "buttonStopLossPriceHelper";
            this.buttonStopLossPriceHelper.Size = new System.Drawing.Size(24, 22);
            this.buttonStopLossPriceHelper.TabIndex = 40;
            this.buttonStopLossPriceHelper.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonStopLossPriceHelper.UseVisualStyleBackColor = true;
            this.buttonStopLossPriceHelper.Click += new System.EventHandler(this.buttonStopLossPriceHelper_Click);
            // 
            // buttonTakeProfitPriceHelper
            // 
            this.buttonTakeProfitPriceHelper.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonTakeProfitPriceHelper.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonTakeProfitPriceHelper.Image = global::ForexPlatformFrontEnd.Properties.Resources.UP_DOWN;
            this.buttonTakeProfitPriceHelper.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonTakeProfitPriceHelper.Location = new System.Drawing.Point(164, 74);
            this.buttonTakeProfitPriceHelper.Margin = new System.Windows.Forms.Padding(2);
            this.buttonTakeProfitPriceHelper.Name = "buttonTakeProfitPriceHelper";
            this.buttonTakeProfitPriceHelper.Size = new System.Drawing.Size(24, 22);
            this.buttonTakeProfitPriceHelper.TabIndex = 42;
            this.buttonTakeProfitPriceHelper.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonTakeProfitPriceHelper.UseVisualStyleBackColor = true;
            this.buttonTakeProfitPriceHelper.Click += new System.EventHandler(this.buttonTakeProfitPriceHelper_Click);
            // 
            // labelVolume
            // 
            this.labelVolume.AutoSize = true;
            this.labelVolume.Location = new System.Drawing.Point(164, 30);
            this.labelVolume.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelVolume.Name = "labelVolume";
            this.labelVolume.Size = new System.Drawing.Size(16, 13);
            this.labelVolume.TabIndex = 43;
            this.labelVolume.Text = "---";
            // 
            // ModifyOrderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelVolume);
            this.Controls.Add(this.numericUpDownStopLoss);
            this.Controls.Add(this.buttonTakeProfitPriceHelper);
            this.Controls.Add(this.buttonStopLossPriceHelper);
            this.Controls.Add(this.groupBoxPending);
            this.Controls.Add(this.numericUpDownTakeProfit);
            this.Controls.Add(this.numericUpDownVolume);
            this.Controls.Add(this.textBoxSymbol);
            this.Controls.Add(this.textBoxId);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonModify);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.numericUpDownSlippage);
            this.Controls.Add(this.checkBoxSlippage);
            this.Controls.Add(this.checkBoxTakeProfit);
            this.Controls.Add(this.checkBoxStopLoss);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxComment);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(400, 199);
            this.Name = "ModifyOrderControl";
            this.Size = new System.Drawing.Size(424, 239);
            this.Load += new System.EventHandler(this.ModifyOrderControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStopLoss)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTakeProfit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSlippage)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPendingPrice)).EndInit();
            this.groupBoxPending.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.NumericUpDown numericUpDownStopLoss;
        private System.Windows.Forms.NumericUpDown numericUpDownTakeProfit;
        private System.Windows.Forms.CheckBox checkBoxStopLoss;
        private System.Windows.Forms.CheckBox checkBoxTakeProfit;
        private System.Windows.Forms.CheckBox checkBoxSlippage;
        private System.Windows.Forms.NumericUpDown numericUpDownSlippage;
        private System.Windows.Forms.Label labelPrice;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonModify;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxId;
        private System.Windows.Forms.TextBox textBoxSymbol;
        private System.Windows.Forms.NumericUpDown numericUpDownVolume;
        private System.Windows.Forms.NumericUpDown numericUpDownPendingPrice;
        private System.Windows.Forms.GroupBox groupBoxPending;
        private System.Windows.Forms.Button buttonStopLossPriceHelper;
        private System.Windows.Forms.Button buttonTakeProfitPriceHelper;
        private System.Windows.Forms.Button buttonOpeningPricePriceHelper;
        private System.Windows.Forms.Label labelVolume;
    }
}
