using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonSupport;
using System.Reflection;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control allows creation of new orders.
    /// </summary>
    public partial class NewOrderControl : UserControl
    {
        Type[] _orderTypes;

        IQuoteProvider _dataProvider;
        protected IQuoteProvider DataProvider
        {
            get { return _dataProvider; }
        }

        protected bool StopLossAllowBuy
        {
            get
            {
                decimal? referenceLevel = DataProvider.Bid;
                if (checkBoxPendingOrder.Checked)
                {// Since this is a delayed(pending) order, consider this level.
                    referenceLevel = numericUpDownPendingPrice.Value;
                }

                return numericUpDownStopLoss.Enabled == false ||
                    (referenceLevel.HasValue && _stopLossValue < referenceLevel);
            }
        }

        protected bool StopLossAllowSell
        {
            get
            {
                decimal? referenceLevel = DataProvider.Ask;
                if (checkBoxPendingOrder.Checked)
                {// Since this is a delayed(pending) order, consider this level.
                    referenceLevel = numericUpDownPendingPrice.Value;
                }

                return numericUpDownStopLoss.Enabled == false ||
                    (referenceLevel.HasValue && _stopLossValue > referenceLevel);
            }
        }

        protected bool TakeProfitAllowSell
        {
            get
            {
                decimal? referenceLevel = DataProvider.Bid;
                if (checkBoxPendingOrder.Checked)
                {// Since this is a delayed(pending) order, consider this level.
                    referenceLevel = numericUpDownPendingPrice.Value;
                }

                return numericUpDownTakeProfit.Enabled == false ||
                    (referenceLevel.HasValue && _takeProfitValue < referenceLevel);
            }
        }

        protected bool TakeProfitAllowBuy
        {
            get
            {
                decimal? referenceLevel = DataProvider.Ask;
                if (checkBoxPendingOrder.Checked)
                {// Since this is a delayed(pending) order, consider this level.
                    referenceLevel = numericUpDownPendingPrice.Value;
                }

                return numericUpDownTakeProfit.Enabled == false ||
                    (referenceLevel.HasValue && _takeProfitValue > referenceLevel);
            }
        }

        protected decimal? StopLoss
        {
            get
            {
                if (numericUpDownStopLoss.Enabled == false)
                {
                    return null;
                }

                return numericUpDownStopLoss.Value;
            }
        }

        protected decimal? TakeProfit
        {
            get
            {
                if (numericUpDownTakeProfit.Enabled == false)
                {
                    return null;
                }

                return numericUpDownTakeProfit.Value;
            }

        }

        Image _imageSell;
        Image _imageBuy;

        decimal _takeProfitValue = 0;
        decimal _stopLossValue = 0;
        decimal _volumeValue = 0;

        DataSessionInfo _session;

        bool _balancedPositionModeVisible = true;
        bool _balancePositionModeChecked = true;

        public delegate bool CreatePlaceOrderDelegate(OrderTypeEnum orderType, bool synchronous, int volume, 
            decimal? allowedSlippage, decimal? desiredPrice, decimal? sourceTakeProfit, decimal? sourceStopLoss, 
            bool allowExistingOrdersManipulation, string comment, out string operationResultMessage);

        /// <summary>
        /// Create order event. Exported to allow different types of orders to be created upon request.
        /// Make sure to implement this event before using the control. 
        /// </summary>
        public event CreatePlaceOrderDelegate CreatePlaceOrderEvent;

        /// <summary>
        /// 
        /// </summary>
        public NewOrderControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The control only consumes dataDelivery provider, so that it is not available for sessionless operations too.
        /// </summary>
        public NewOrderControl(IQuoteProvider dataProvider, DataSessionInfo session, 
                    bool balancedPositionModeVisible, bool balancePositionModeChecked)
        {
            InitializeComponent();

            _dataProvider = dataProvider;
            if (_dataProvider != null)
            {
                _dataProvider.QuoteUpdateEvent += _dataProvider_OnValuesUpdateEvent;
            }

            _balancedPositionModeVisible = balancedPositionModeVisible;
            _balancePositionModeChecked = balancePositionModeChecked;

            _session = session;

            numericUpDownStopLoss.Increment = session.DecimalIncrement;
            numericUpDownStopLoss.DecimalPlaces = session.DecimalPlaces;

            numericUpDownSlippage.Increment = session.DecimalIncrement;
            numericUpDownSlippage.DecimalPlaces = session.DecimalPlaces;

            numericUpDownTakeProfit.Increment = session.DecimalIncrement;
            numericUpDownTakeProfit.DecimalPlaces = session.DecimalPlaces;

            numericUpDownPendingPrice.Increment = session.DecimalIncrement;
            numericUpDownPendingPrice.DecimalPlaces = session.DecimalPlaces;
        }

        protected override void  OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            UnInitializeControl();
        }

        public void UnInitializeControl()
        {
            if (DataProvider != null)
            {
                DataProvider.QuoteUpdateEvent -= _dataProvider_OnValuesUpdateEvent;
            }

            _dataProvider = null;
        }

        private void NewOrderControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            _imageSell = buttonSell.Image;
            _imageBuy = buttonBuy.Image;

            List<Type> orderTypes = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(typeof(ActiveOrder),
                ReflectionHelper.GetApplicationEntryAssemblyReferencedAssemblies());

            orderTypes.Add(typeof(ActiveOrder));
            orderTypes.Reverse();
            _orderTypes = orderTypes.ToArray();

            foreach (Type type in _orderTypes)
            {
                string name = type.Name;
                UserFriendlyNameAttribute.GetTypeAttributeValue(type, ref name);
                this.comboBoxManagement.Items.Add(name);
                comboBoxManagement.SelectedIndex = 0;
            }

            if (DataProvider.Ask.HasValue == false || DataProvider.Ask == 0)
            {
                numericUpDownStopLoss.DecimalPlaces = 0;
            }
            else
            {
                numericUpDownStopLoss.DecimalPlaces = 5 - (int)Math.Ceiling(Math.Log10((double)DataProvider.Ask));
            }

            if (_session.Symbol.IsForexPair)
            {
                labelVolume.Text = "Lots (1 Lot = " + _session.LotSize + " Units)";
                numericUpDownVolume.DecimalPlaces = 2;
                numericUpDownVolume.Value = 1;
            }
            else
            {
                labelVolume.Text = "Units";
                numericUpDownVolume.DecimalPlaces = 0;
                numericUpDownVolume.Value = 100;
            }

            numericUpDownStopLoss.Increment = (Decimal)Math.Pow(0.1, numericUpDownStopLoss.DecimalPlaces);

            numericUpDownTakeProfit.DecimalPlaces = numericUpDownStopLoss.DecimalPlaces;
            numericUpDownTakeProfit.Increment = numericUpDownStopLoss.Increment;

            dateTimePickerExpiry.Value = dateTimePickerExpiry.MaxDate;

            comboBoxSymbol.Text = _session.Symbol.Name;

            checkBoxBalancedPositionMode.Visible = _balancedPositionModeVisible;
            checkBoxBalancedPositionMode.Checked = _balancePositionModeChecked;

            NewOrderControl_Resize(sender, e);

            UpdateUI();
        }


        void _dataProvider_OnValuesUpdateEvent(IQuoteProvider dataProvider)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            decimal? ask = _dataProvider.Ask;
            decimal? bid = _dataProvider.Bid;

            if (_dataProvider == null || ask.HasValue == false || bid.HasValue == false)
            {
                labelPrice.Text = "- / -";
                numericUpDownVolume.Enabled = false;
                buttonBuy.Enabled = false;
                buttonSell.Enabled = false;
            }
            else
            {
                // Maximum decimal symbols displayed 6.
                labelPrice.Text = Math.Round(bid.Value, 6) + " / " + Math.Round(ask.Value, 6);
                numericUpDownVolume.Enabled = true;
                buttonBuy.Enabled = true;
                buttonSell.Enabled = true;
            }

            checkBoxPendingOrder.Checked = checkBoxPendingOrder.Checked && false == checkBoxBalancedPositionMode.Checked;
            checkBoxPendingOrder.Enabled = false == checkBoxBalancedPositionMode.Checked;
            checkBoxStopLoss.Checked = checkBoxStopLoss.Checked && false == checkBoxBalancedPositionMode.Checked;
            checkBoxStopLoss.Enabled = false == checkBoxBalancedPositionMode.Checked;
            checkBoxTakeProfit.Checked = checkBoxTakeProfit.Checked && false == checkBoxBalancedPositionMode.Checked;
            checkBoxTakeProfit.Enabled = false == checkBoxBalancedPositionMode.Checked;

            this.numericUpDownSlippage.Enabled = checkBoxSlippage.Checked;
            this.numericUpDownStopLoss.Enabled = checkBoxStopLoss.Checked;
            buttonStopLossPriceHelper.Enabled = this.numericUpDownStopLoss.Enabled;

            this.numericUpDownTakeProfit.Enabled = checkBoxTakeProfit.Checked;
            buttonTakeProfitPriceHelper.Enabled = this.numericUpDownTakeProfit.Enabled;

            if (numericUpDownStopLoss.Enabled == false)
            {
                numericUpDownStopLoss.Value = 0;
            }

            if (numericUpDownTakeProfit.Enabled == false)
            {
                numericUpDownTakeProfit.Value = 0;
            }

            // Mark buy button with a text color to suggest if it is allowed or not.
            if (StopLossAllowBuy && TakeProfitAllowBuy)
            {
                //buttonBuy.ForeColor = SystemColors.ControlText;
                buttonBuy.Image = _imageBuy;
            }
            else
            {
                //buttonBuy.ForeColor = SystemColors.GrayText;
                buttonBuy.Image = null;
            }

            if (StopLossAllowSell && TakeProfitAllowSell)
            {
                //buttonSell.ForeColor = SystemColors.ControlText;
                buttonSell.Image = _imageSell;
            }
            else
            {
                //buttonSell.ForeColor = SystemColors.GrayText;
                buttonSell.Image = null;
            }

            groupBoxPending.Enabled = checkBoxPendingOrder.Checked;

        }


        private void NewOrderControl_Resize(object sender, EventArgs e)
        {
            buttonSell.Width = (this.Width / 2) - 10;
            buttonBuy.Width = (this.Width / 2) - 10;
            buttonSell.Left = (this.Width / 2) + 5;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();

            decimal? ask = DataProvider.Ask;
            decimal? bid = DataProvider.Bid;

            if (ask.HasValue == false || bid.HasValue == false)
            {
                return;
            }

            if (sender == checkBoxStopLoss && checkBoxStopLoss.Checked)
            {// Initial - set a proper dPrice on the numeric.
                numericUpDownStopLoss.Value = (ask.Value + bid.Value) / 2;
            }

            if (sender == checkBoxTakeProfit && checkBoxTakeProfit.Checked)
            {
                numericUpDownTakeProfit.Value = (ask.Value + bid.Value) / 2;
            }

        }

        private void buttonSell_Click(object sender, EventArgs e)
        {
            if (StopLossAllowSell == false)
            {
                MessageBox.Show("Stop Loss level not valid.", "Order", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (TakeProfitAllowSell == false)
            {
                MessageBox.Show("Take Profit level not valid.", "Order", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            CreateOrder(false);
        }

        private void buttonBuy_Click(object sender, EventArgs e)
        {
            if (StopLossAllowBuy == false)
            {
                MessageBox.Show("Stop Loss level not valid.", "Order", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (TakeProfitAllowBuy == false)
            {
                MessageBox.Show("Take Profit level not valid.", "Order", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            CreateOrder(true);
        }

        protected void CreateOrder(bool isBuy)
        {
            OrderTypeEnum orderType = isBuy ? OrderTypeEnum.BUY_MARKET : OrderTypeEnum.SELL_MARKET;
            if (checkBoxPendingOrder.Checked)
            {
                if (radioButtonLimit.Checked)
                {
                    if (isBuy)
                    {
                        orderType = OrderTypeEnum.BUY_LIMIT_MARKET;
                    }
                    else
                    {
                        orderType = OrderTypeEnum.SELL_LIMIT_MARKET;
                    }
                }
                else if (radioButtonStop.Checked)
                {
                    if (isBuy)
                    {
                        orderType = OrderTypeEnum.BUY_STOP_MARKET;
                    }
                    else
                    {
                        orderType = OrderTypeEnum.SELL_STOP_MARKET;
                    }
                }
            }

            decimal? slippage = null;
            if (this.checkBoxSlippage.Checked)
            {
                slippage = this.numericUpDownSlippage.Value;
            }

            decimal? desiredPrice = isBuy ? DataProvider.Ask : DataProvider.Bid;
            string operationResultMessage;

            if (checkBoxPendingOrder.Checked)
            {
                desiredPrice = numericUpDownPendingPrice.Value;
            }

            string comment = textBoxComment.Text;

            if (CreatePlaceOrderEvent(orderType, checkBoxSynchronous.Checked, GetVolume(), slippage, 
                desiredPrice, TakeProfit, StopLoss, checkBoxBalancedPositionMode.Checked, comment, out operationResultMessage) == false)
            {// Show error requestMessage.
                MessageBox.Show(operationResultMessage, "Order Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.Hide();
            }
        }

        int GetVolume()
        {
            if (_session.Symbol.IsForexPair)
            {
                return (int)(this.numericUpDownVolume.Value * _session.LotSize);
            }

            return (int)this.numericUpDownVolume.Value;
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _stopLossValue = numericUpDownStopLoss.Value;
            _takeProfitValue = numericUpDownTakeProfit.Value;
            _volumeValue = numericUpDownVolume.Value;

            UpdateUI();
        }

        private void checkBoxPendingOrder_CheckedChanged(object sender, EventArgs e)
        {
            decimal? ask = DataProvider.Ask;

            if (checkBoxPendingOrder.Checked && ask.HasValue)
            {
                if (radioButtonLimit.Checked)
                {
                    numericUpDownPendingPrice.Value = Math.Round(ask.Value, numericUpDownPendingPrice.DecimalPlaces);
                }
                else if (radioButtonStop.Checked)
                {
                    numericUpDownPendingPrice.Value = Math.Round(ask.Value, numericUpDownPendingPrice.DecimalPlaces);
                }
            }
            else
            {
                numericUpDownPendingPrice.Value = 0;
            }

            UpdateUI();
        }

        private void radioButtonStop_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void groupBoxPending_Enter(object sender, EventArgs e)
        {

        }

        private void buttonPendingPriceHelper_Click(object sender, EventArgs e)
        {
            Point location = ((Button)sender).Location;
            location.X -= 35;
            PriceHelpControl.ShowHelperControlAt(location, this, numericUpDownPendingPrice);
        }

        private void buttonStopLossPriceHelper_Click(object sender, EventArgs e)
        {
            PriceHelpControl.ShowHelperControlAt(((Button)sender).Location, this, numericUpDownStopLoss);
        }

        private void buttonTakeProfitPriceHelper_Click(object sender, EventArgs e)
        {
            PriceHelpControl.ShowHelperControlAt(((Button)sender).Location, this, numericUpDownTakeProfit);
        }

        private void checkBoxAllowExistingOrdersModification_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void checkBoxSlippage_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

    }
}
