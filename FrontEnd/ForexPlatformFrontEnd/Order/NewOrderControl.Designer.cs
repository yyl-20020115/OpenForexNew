using CommonSupport;
namespace ForexPlatformFrontEnd
{
    partial class NewOrderControl
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
            System.Windows.Forms.Panel panel1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewOrderControl));
            this.comboBoxSymbol = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownStopLoss = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTakeProfit = new System.Windows.Forms.NumericUpDown();
            this.groupBoxPending = new System.Windows.Forms.GroupBox();
            this.buttonPendingPriceHelper = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownPendingPrice = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerExpiry = new System.Windows.Forms.DateTimePicker();
            this.radioButtonStop = new System.Windows.Forms.RadioButton();
            this.radioButtonLimit = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxManagement = new System.Windows.Forms.ComboBox();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxStopLoss = new System.Windows.Forms.CheckBox();
            this.checkBoxTakeProfit = new System.Windows.Forms.CheckBox();
            this.checkBoxSlippage = new System.Windows.Forms.CheckBox();
            this.numericUpDownSlippage = new System.Windows.Forms.NumericUpDown();
            this.checkBoxPendingOrder = new System.Windows.Forms.CheckBox();
            this.labelPrice = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownVolume = new System.Windows.Forms.NumericUpDown();
            this.buttonSell = new System.Windows.Forms.Button();
            this.buttonBuy = new System.Windows.Forms.Button();
            this.buttonStopLossPriceHelper = new System.Windows.Forms.Button();
            this.buttonTakeProfitPriceHelper = new System.Windows.Forms.Button();
            this.labelVolume = new System.Windows.Forms.Label();
            this.checkBoxSynchronous = new System.Windows.Forms.CheckBox();
            this.checkBoxBalancedPositionMode = new System.Windows.Forms.CheckBox();
            panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStopLoss)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTakeProfit)).BeginInit();
            this.groupBoxPending.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPendingPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSlippage)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.DimGray;
            panel1.Location = new System.Drawing.Point(185, 22);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1, 66);
            panel1.TabIndex = 32;
            // 
            // comboBoxSymbol
            // 
            this.comboBoxSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSymbol.Enabled = false;
            this.comboBoxSymbol.FormattingEnabled = true;
            this.comboBoxSymbol.Location = new System.Drawing.Point(311, 5);
            this.comboBoxSymbol.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxSymbol.Name = "comboBoxSymbol";
            this.comboBoxSymbol.Size = new System.Drawing.Size(112, 21);
            this.comboBoxSymbol.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Volume";
            // 
            // numericUpDownStopLoss
            // 
            this.numericUpDownStopLoss.DecimalPlaces = 4;
            this.numericUpDownStopLoss.Enabled = false;
            this.numericUpDownStopLoss.Location = new System.Drawing.Point(78, 28);
            this.numericUpDownStopLoss.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownStopLoss.Maximum = new decimal(new int[] {
            100000,
            0,
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
            this.numericUpDownTakeProfit.Location = new System.Drawing.Point(78, 52);
            this.numericUpDownTakeProfit.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownTakeProfit.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownTakeProfit.Name = "numericUpDownTakeProfit";
            this.numericUpDownTakeProfit.Size = new System.Drawing.Size(81, 20);
            this.numericUpDownTakeProfit.TabIndex = 10;
            this.numericUpDownTakeProfit.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // groupBoxPending
            // 
            this.groupBoxPending.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPending.Controls.Add(this.buttonPendingPriceHelper);
            this.groupBoxPending.Controls.Add(panel1);
            this.groupBoxPending.Controls.Add(this.label7);
            this.groupBoxPending.Controls.Add(this.numericUpDownPendingPrice);
            this.groupBoxPending.Controls.Add(this.label4);
            this.groupBoxPending.Controls.Add(this.dateTimePickerExpiry);
            this.groupBoxPending.Controls.Add(this.radioButtonStop);
            this.groupBoxPending.Controls.Add(this.radioButtonLimit);
            this.groupBoxPending.Enabled = false;
            this.groupBoxPending.Location = new System.Drawing.Point(4, 100);
            this.groupBoxPending.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxPending.Name = "groupBoxPending";
            this.groupBoxPending.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxPending.Size = new System.Drawing.Size(419, 93);
            this.groupBoxPending.TabIndex = 12;
            this.groupBoxPending.TabStop = false;
            this.groupBoxPending.Enter += new System.EventHandler(this.groupBoxPending_Enter);
            // 
            // buttonPendingPriceHelper
            // 
            this.buttonPendingPriceHelper.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonPendingPriceHelper.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonPendingPriceHelper.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonPendingPriceHelper.Image = global::ForexPlatformFrontEnd.Properties.Resources.UP_DOWN;
            this.buttonPendingPriceHelper.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonPendingPriceHelper.Location = new System.Drawing.Point(331, 48);
            this.buttonPendingPriceHelper.Margin = new System.Windows.Forms.Padding(2);
            this.buttonPendingPriceHelper.Name = "buttonPendingPriceHelper";
            this.buttonPendingPriceHelper.Size = new System.Drawing.Size(24, 24);
            this.buttonPendingPriceHelper.TabIndex = 28;
            this.buttonPendingPriceHelper.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonPendingPriceHelper.UseVisualStyleBackColor = true;
            this.buttonPendingPriceHelper.Click += new System.EventHandler(this.buttonPendingPriceHelper_Click);
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(260, 29);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Price";
            // 
            // numericUpDownPendingPrice
            // 
            this.numericUpDownPendingPrice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numericUpDownPendingPrice.DecimalPlaces = 4;
            this.numericUpDownPendingPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.numericUpDownPendingPrice.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericUpDownPendingPrice.Location = new System.Drawing.Point(230, 47);
            this.numericUpDownPendingPrice.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownPendingPrice.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.numericUpDownPendingPrice.Name = "numericUpDownPendingPrice";
            this.numericUpDownPendingPrice.Size = new System.Drawing.Size(97, 26);
            this.numericUpDownPendingPrice.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 71);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Expiry";
            // 
            // dateTimePickerExpiry
            // 
            this.dateTimePickerExpiry.Checked = false;
            this.dateTimePickerExpiry.Enabled = false;
            this.dateTimePickerExpiry.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerExpiry.Location = new System.Drawing.Point(46, 68);
            this.dateTimePickerExpiry.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerExpiry.Name = "dateTimePickerExpiry";
            this.dateTimePickerExpiry.ShowCheckBox = true;
            this.dateTimePickerExpiry.Size = new System.Drawing.Size(104, 20);
            this.dateTimePickerExpiry.TabIndex = 28;
            // 
            // radioButtonStop
            // 
            this.radioButtonStop.AutoSize = true;
            this.radioButtonStop.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioButtonStop.Location = new System.Drawing.Point(10, 47);
            this.radioButtonStop.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonStop.Name = "radioButtonStop";
            this.radioButtonStop.Size = new System.Drawing.Size(46, 17);
            this.radioButtonStop.TabIndex = 2;
            this.radioButtonStop.Text = "Stop";
            this.radioButtonStop.UseVisualStyleBackColor = false;
            this.radioButtonStop.CheckedChanged += new System.EventHandler(this.radioButtonStop_CheckedChanged);
            // 
            // radioButtonLimit
            // 
            this.radioButtonLimit.AutoSize = true;
            this.radioButtonLimit.Checked = true;
            this.radioButtonLimit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioButtonLimit.Location = new System.Drawing.Point(10, 26);
            this.radioButtonLimit.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonLimit.Name = "radioButtonLimit";
            this.radioButtonLimit.Size = new System.Drawing.Size(45, 17);
            this.radioButtonLimit.TabIndex = 1;
            this.radioButtonLimit.TabStop = true;
            this.radioButtonLimit.Text = "Limit";
            this.radioButtonLimit.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(192, 81);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Management";
            this.label5.Visible = false;
            // 
            // comboBoxManagement
            // 
            this.comboBoxManagement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxManagement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxManagement.Enabled = false;
            this.comboBoxManagement.FormattingEnabled = true;
            this.comboBoxManagement.Location = new System.Drawing.Point(265, 79);
            this.comboBoxManagement.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxManagement.Name = "comboBoxManagement";
            this.comboBoxManagement.Size = new System.Drawing.Size(158, 21);
            this.comboBoxManagement.TabIndex = 14;
            this.comboBoxManagement.Visible = false;
            // 
            // textBoxComment
            // 
            this.textBoxComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComment.Location = new System.Drawing.Point(2, 210);
            this.textBoxComment.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxComment.Multiline = true;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(421, 68);
            this.textBoxComment.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, 195);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Comment";
            // 
            // checkBoxStopLoss
            // 
            this.checkBoxStopLoss.AutoSize = true;
            this.checkBoxStopLoss.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBoxStopLoss.Location = new System.Drawing.Point(4, 29);
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
            this.checkBoxTakeProfit.Location = new System.Drawing.Point(4, 53);
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
            this.checkBoxSlippage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBoxSlippage.Location = new System.Drawing.Point(4, 77);
            this.checkBoxSlippage.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSlippage.Name = "checkBoxSlippage";
            this.checkBoxSlippage.Size = new System.Drawing.Size(65, 17);
            this.checkBoxSlippage.TabIndex = 19;
            this.checkBoxSlippage.Text = "Slippage";
            this.checkBoxSlippage.UseVisualStyleBackColor = true;
            this.checkBoxSlippage.CheckedChanged += new System.EventHandler(this.checkBoxSlippage_CheckedChanged);
            // 
            // numericUpDownSlippage
            // 
            this.numericUpDownSlippage.DecimalPlaces = 4;
            this.numericUpDownSlippage.Enabled = false;
            this.numericUpDownSlippage.Location = new System.Drawing.Point(78, 76);
            this.numericUpDownSlippage.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownSlippage.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownSlippage.Name = "numericUpDownSlippage";
            this.numericUpDownSlippage.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownSlippage.TabIndex = 20;
            this.numericUpDownSlippage.Value = new decimal(new int[] {
            5,
            0,
            0,
            262144});
            // 
            // checkBoxPendingOrder
            // 
            this.checkBoxPendingOrder.AutoSize = true;
            this.checkBoxPendingOrder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBoxPendingOrder.Location = new System.Drawing.Point(4, 99);
            this.checkBoxPendingOrder.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxPendingOrder.Name = "checkBoxPendingOrder";
            this.checkBoxPendingOrder.Size = new System.Drawing.Size(92, 17);
            this.checkBoxPendingOrder.TabIndex = 23;
            this.checkBoxPendingOrder.Text = "Pending Order";
            this.checkBoxPendingOrder.UseVisualStyleBackColor = true;
            this.checkBoxPendingOrder.CheckedChanged += new System.EventHandler(this.checkBoxPendingOrder_CheckedChanged);
            // 
            // labelPrice
            // 
            this.labelPrice.BackColor = System.Drawing.Color.Transparent;
            this.labelPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.labelPrice.Location = new System.Drawing.Point(2, 15);
            this.labelPrice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPrice.Name = "labelPrice";
            this.labelPrice.Size = new System.Drawing.Size(230, 32);
            this.labelPrice.TabIndex = 24;
            this.labelPrice.Text = "999.99 / 999.99";
            this.labelPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.labelPrice);
            this.groupBox2.Location = new System.Drawing.Point(189, 26);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(234, 49);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Market Price Bid / Ask";
            // 
            // numericUpDownVolume
            // 
            this.numericUpDownVolume.DecimalPlaces = 2;
            this.numericUpDownVolume.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownVolume.Location = new System.Drawing.Point(50, 5);
            this.numericUpDownVolume.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownVolume.Name = "numericUpDownVolume";
            this.numericUpDownVolume.Size = new System.Drawing.Size(81, 20);
            this.numericUpDownVolume.TabIndex = 27;
            this.numericUpDownVolume.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonSell
            // 
            this.buttonSell.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSell.AutoSize = true;
            this.buttonSell.Image = ((System.Drawing.Image)(resources.GetObject("buttonSell.Image")));
            this.buttonSell.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSell.Location = new System.Drawing.Point(296, 306);
            this.buttonSell.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSell.Name = "buttonSell";
            this.buttonSell.Size = new System.Drawing.Size(125, 38);
            this.buttonSell.TabIndex = 5;
            this.buttonSell.TabStop = false;
            this.buttonSell.Text = "Sell";
            this.buttonSell.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSell.UseVisualStyleBackColor = true;
            this.buttonSell.Click += new System.EventHandler(this.buttonSell_Click);
            // 
            // buttonBuy
            // 
            this.buttonBuy.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonBuy.AutoSize = true;
            this.buttonBuy.Image = global::ForexPlatformFrontEnd.Properties.Resources.arrow_up_green_24;
            this.buttonBuy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonBuy.Location = new System.Drawing.Point(4, 306);
            this.buttonBuy.Margin = new System.Windows.Forms.Padding(2);
            this.buttonBuy.Name = "buttonBuy";
            this.buttonBuy.Size = new System.Drawing.Size(124, 38);
            this.buttonBuy.TabIndex = 4;
            this.buttonBuy.TabStop = false;
            this.buttonBuy.Text = "Buy";
            this.buttonBuy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonBuy.UseVisualStyleBackColor = true;
            this.buttonBuy.Click += new System.EventHandler(this.buttonBuy_Click);
            // 
            // buttonStopLossPriceHelper
            // 
            this.buttonStopLossPriceHelper.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonStopLossPriceHelper.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonStopLossPriceHelper.Image = global::ForexPlatformFrontEnd.Properties.Resources.UP_DOWN;
            this.buttonStopLossPriceHelper.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonStopLossPriceHelper.Location = new System.Drawing.Point(161, 26);
            this.buttonStopLossPriceHelper.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStopLossPriceHelper.Name = "buttonStopLossPriceHelper";
            this.buttonStopLossPriceHelper.Size = new System.Drawing.Size(24, 22);
            this.buttonStopLossPriceHelper.TabIndex = 29;
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
            this.buttonTakeProfitPriceHelper.Location = new System.Drawing.Point(161, 50);
            this.buttonTakeProfitPriceHelper.Margin = new System.Windows.Forms.Padding(2);
            this.buttonTakeProfitPriceHelper.Name = "buttonTakeProfitPriceHelper";
            this.buttonTakeProfitPriceHelper.Size = new System.Drawing.Size(24, 22);
            this.buttonTakeProfitPriceHelper.TabIndex = 30;
            this.buttonTakeProfitPriceHelper.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonTakeProfitPriceHelper.UseVisualStyleBackColor = true;
            this.buttonTakeProfitPriceHelper.Click += new System.EventHandler(this.buttonTakeProfitPriceHelper_Click);
            // 
            // labelVolume
            // 
            this.labelVolume.AutoSize = true;
            this.labelVolume.Location = new System.Drawing.Point(135, 7);
            this.labelVolume.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelVolume.Name = "labelVolume";
            this.labelVolume.Size = new System.Drawing.Size(135, 13);
            this.labelVolume.TabIndex = 31;
            this.labelVolume.Text = "Lots (1 Lot = 100000 Units)";
            // 
            // checkBoxSynchronous
            // 
            this.checkBoxSynchronous.AutoSize = true;
            this.checkBoxSynchronous.Checked = true;
            this.checkBoxSynchronous.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSynchronous.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxSynchronous.Location = new System.Drawing.Point(3, 284);
            this.checkBoxSynchronous.Name = "checkBoxSynchronous";
            this.checkBoxSynchronous.Size = new System.Drawing.Size(134, 17);
            this.checkBoxSynchronous.TabIndex = 32;
            this.checkBoxSynchronous.Text = "Execute Synchronously";
            this.checkBoxSynchronous.UseVisualStyleBackColor = true;
            // 
            // checkBoxBalancedPositionMode
            // 
            this.checkBoxBalancedPositionMode.AutoSize = true;
            this.checkBoxBalancedPositionMode.Checked = true;
            this.checkBoxBalancedPositionMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBalancedPositionMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxBalancedPositionMode.Location = new System.Drawing.Point(143, 284);
            this.checkBoxBalancedPositionMode.Name = "checkBoxBalancedPositionMode";
            this.checkBoxBalancedPositionMode.Size = new System.Drawing.Size(138, 17);
            this.checkBoxBalancedPositionMode.TabIndex = 33;
            this.checkBoxBalancedPositionMode.Text = "Balanced Position Mode";
            this.checkBoxBalancedPositionMode.UseVisualStyleBackColor = true;
            this.checkBoxBalancedPositionMode.CheckedChanged += new System.EventHandler(this.checkBoxAllowExistingOrdersModification_CheckedChanged);
            // 
            // NewOrderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxBalancedPositionMode);
            this.Controls.Add(this.checkBoxSynchronous);
            this.Controls.Add(this.labelVolume);
            this.Controls.Add(this.buttonTakeProfitPriceHelper);
            this.Controls.Add(this.buttonStopLossPriceHelper);
            this.Controls.Add(this.numericUpDownTakeProfit);
            this.Controls.Add(this.numericUpDownVolume);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxPendingOrder);
            this.Controls.Add(this.numericUpDownSlippage);
            this.Controls.Add(this.checkBoxSlippage);
            this.Controls.Add(this.checkBoxTakeProfit);
            this.Controls.Add(this.checkBoxStopLoss);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxManagement);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBoxPending);
            this.Controls.Add(this.numericUpDownStopLoss);
            this.Controls.Add(this.buttonSell);
            this.Controls.Add(this.buttonBuy);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxSymbol);
            this.Controls.Add(this.textBoxComment);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(356, 0);
            this.Name = "NewOrderControl";
            this.Size = new System.Drawing.Size(425, 346);
            this.Load += new System.EventHandler(this.NewOrderControl_Load);
            this.Resize += new System.EventHandler(this.NewOrderControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStopLoss)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTakeProfit)).EndInit();
            this.groupBoxPending.ResumeLayout(false);
            this.groupBoxPending.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPendingPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSlippage)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSymbol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBuy;
        private System.Windows.Forms.Button buttonSell;
        private System.Windows.Forms.NumericUpDown numericUpDownStopLoss;
        private System.Windows.Forms.NumericUpDown numericUpDownTakeProfit;
        private System.Windows.Forms.GroupBox groupBoxPending;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxManagement;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxStopLoss;
        private System.Windows.Forms.CheckBox checkBoxTakeProfit;
        private System.Windows.Forms.CheckBox checkBoxSlippage;
        private System.Windows.Forms.NumericUpDown numericUpDownSlippage;
        private System.Windows.Forms.RadioButton radioButtonLimit;
        private System.Windows.Forms.RadioButton radioButtonStop;
        private System.Windows.Forms.CheckBox checkBoxPendingOrder;
        private System.Windows.Forms.Label labelPrice;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePickerExpiry;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDownPendingPrice;
        private System.Windows.Forms.NumericUpDown numericUpDownVolume;
        private System.Windows.Forms.Button buttonPendingPriceHelper;
        private System.Windows.Forms.Button buttonStopLossPriceHelper;
        private System.Windows.Forms.Button buttonTakeProfitPriceHelper;
        private System.Windows.Forms.Label labelVolume;
        private System.Windows.Forms.CheckBox checkBoxSynchronous;
        private System.Windows.Forms.CheckBox checkBoxBalancedPositionMode;
    }
}
