using System;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control provides functionality to modify existing order parameters.
    /// </summary>
    public partial class ModifyOrderControl : UserControl
    {
        IQuoteProvider _dataProvider;

        Order _order;

        /// <summary>
        /// Mode of operation for this control.
        /// </summary>
        public enum ModeEnum
        {
            Modify,
            Close
        }

        // Use these instead of direct access, since direct access causes the control to change its cursor position, and this is not allowed since it confuses the user.
        decimal _numericUpDownTakeProfitValue = 0;
        decimal _numericUpDownStopLossValue = 0;
        decimal _numericUpDownVolumeValue = 0;
        decimal _numericUpDownPendingPrice = 0;

        DataSessionInfo _session;

        ModeEnum _mode;

        ISourceOrderExecution _provider;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ModifyOrderControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Detailed constructor.
        /// </summary>
        public ModifyOrderControl(ModeEnum mode, Order order)
        {
            InitializeComponent();
            _provider = order.OrderExecutionProvider;
            _mode = mode;
            _order = order;
            _dataProvider = order.QuoteProvider;
            _dataProvider.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(_dataProvider_QuoteUpdateEvent);
            _session = order.SessionInfo.Value;

        }

        void _dataProvider_QuoteUpdateEvent(IQuoteProvider provider)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
 	        base.OnHandleDestroyed(e);
            UnInitialize();
        }

        public void UnInitialize()
        {
            if (_dataProvider != null)
            {
                _dataProvider.QuoteUpdateEvent -= new QuoteProviderUpdateDelegate(_dataProvider_QuoteUpdateEvent);
                _dataProvider = null;
            }

            _order = null;
        }

        void LoadOrderUI()
        {
            textBoxSymbol.Text = _order.Symbol.Name;
            this.textBoxId.Text = _order.Id;

            if (_order.State != OrderStateEnum.Submitted)
            {
                if (numericUpDownVolume.Maximum != GetOrderVolumeForUI())
                {
                    numericUpDownVolume.Maximum = GetOrderVolumeForUI();
                }
            }

            groupBoxPending.Visible = _order.IsDelayed;
            if (_order.IsDelayed)
            {
                numericUpDownPendingPrice.Enabled = _order.State == OrderStateEnum.Submitted && _order.OpenPrice.HasValue;
                if (_order.OpenPrice.HasValue)
                {
                    numericUpDownPendingPrice.Value = _order.OpenPrice.Value;
                }
            }
            
            decimal? stopLoss = _order.StopLoss;
            decimal? takeProfit = _order.TakeProfit;

            checkBoxStopLoss.Checked = stopLoss.HasValue;
            checkBoxTakeProfit.Checked = takeProfit.HasValue;

            if (checkBoxStopLoss.Checked)
            {
                numericUpDownStopLoss.Value = stopLoss.Value;
            }

            if (checkBoxTakeProfit.Checked)
            {
                numericUpDownTakeProfit.Value = takeProfit.Value;
            }

        }

        private void ModifyOrderControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            this.buttonModify.Left = buttonClose.Left;
            this.buttonModify.Width = buttonClose.Width;

            if (numericUpDownVolume.Minimum != 0)
            {
                numericUpDownVolume.Minimum = 0;
            }

            if (numericUpDownVolume.Enabled != (_mode == ModeEnum.Modify))
            {
                numericUpDownVolume.Enabled = (_mode == ModeEnum.Modify);
            }

            if (numericUpDownVolume.DecimalPlaces != _session.DecimalPlaces)
            {
                numericUpDownVolume.DecimalPlaces = _session.DecimalPlaces;
            }
          
            if (_mode == ModeEnum.Modify)
            {
                numericUpDownVolume.Value = GetOrderVolumeForUI();
            }
            else
            {
                numericUpDownVolume.Value = 0;
            }

            if (_session.Symbol.IsForexPair)
            {
                labelVolume.Text = "Lots";
                numericUpDownVolume.Increment = 0.1m;
                if (numericUpDownVolume.DecimalPlaces != 2)
                {
                    numericUpDownVolume.DecimalPlaces = 2;
                }
            }
            else
            {
                labelVolume.Text = "Units";

                numericUpDownVolume.Increment = 1;

                if (numericUpDownVolume.DecimalPlaces != 0)
                {
                    numericUpDownVolume.DecimalPlaces = 0;
                }
            }

            checkBoxStopLoss.Enabled = (_mode == ModeEnum.Modify);
            checkBoxTakeProfit.Enabled = (_mode == ModeEnum.Modify);

            numericUpDownPendingPrice.DecimalPlaces = _session.DecimalPlaces;
            numericUpDownPendingPrice.Increment = _session.DecimalIncrement;

            numericUpDownTakeProfit.DecimalPlaces = _session.DecimalPlaces;
            numericUpDownTakeProfit.Increment = _session.DecimalIncrement;

            numericUpDownStopLoss.DecimalPlaces = _session.DecimalPlaces;
            numericUpDownStopLoss.Increment = _session.DecimalIncrement;

            LoadOrderUI();

            UpdateUI();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.buttonModify.Left = buttonClose.Left;
            this.buttonModify.Width = buttonClose.Width;
        }

        bool _isUpdating = false;

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            _isUpdating = true;

            #region Perform UI logic

            decimal? ask = _dataProvider.Ask;
            decimal? bid = _dataProvider.Bid;

            labelPrice.Text = GeneralHelper.ToString(ask, _session.ValueFormat) + " / " + GeneralHelper.ToString(bid, _session.ValueFormat);

            bool volumeModificationNeeded = GetOrderVolumeFromUI() != _order.CurrentVolume;

            checkBoxTakeProfit.Enabled = (_mode == ModeEnum.Modify) && false == volumeModificationNeeded;
            checkBoxStopLoss.Enabled = (_mode == ModeEnum.Modify) && false == volumeModificationNeeded;

            if (volumeModificationNeeded)
            {
                checkBoxTakeProfit.Checked = false;
                checkBoxStopLoss.Checked = false;
            }

            if (numericUpDownStopLoss.Enabled != checkBoxStopLoss.Checked)
            {
                this.numericUpDownStopLoss.Enabled = checkBoxStopLoss.Checked;
            }
            if (numericUpDownTakeProfit.Enabled != checkBoxTakeProfit.Checked)
            {
                this.numericUpDownTakeProfit.Enabled = checkBoxTakeProfit.Checked;
            }

            bool stopLossAllowBuy = true;
            bool stopLossAllowSell = true;

            if (numericUpDownStopLoss.Enabled == false)
            {
                numericUpDownStopLoss.Value = 0;
            }
            else
            {
                stopLossAllowBuy = _numericUpDownStopLossValue < _dataProvider.Bid;
                stopLossAllowSell = _numericUpDownStopLossValue > _dataProvider.Ask;
            }

            bool takeProfitAllowBuy = true, takeProfitAllowSell = true;
            if (numericUpDownTakeProfit.Enabled == false)
            {
                numericUpDownTakeProfit.Value = 0;
            }
            else
            {
                takeProfitAllowBuy = _numericUpDownTakeProfitValue > _dataProvider.Ask;
                takeProfitAllowSell = _numericUpDownTakeProfitValue < _dataProvider.Bid;
            }

            buttonClose.Visible = (_numericUpDownVolumeValue == 0);
            buttonModify.Visible = !buttonClose.Visible;

            buttonModify.Enabled = (_numericUpDownVolumeValue != GetOrderVolumeForUI()) ||
                _numericUpDownStopLossValue != _order.StopLoss ||
                _numericUpDownTakeProfitValue != _order.TakeProfit ||
                (numericUpDownPendingPrice.Visible && _numericUpDownPendingPrice != _order.OpenPrice);

            #endregion

            _isUpdating = false;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdating)
            {
                return;
            }

            decimal? ask = _dataProvider.Ask;
            decimal? bid = _dataProvider.Bid;

            if (ask.HasValue == false || bid.HasValue == false)
            {
                return;
            }

            if (sender == checkBoxTakeProfit && checkBoxTakeProfit.Checked)
            {
                numericUpDownTakeProfit.Value = (ask.Value + bid.Value) / 2m;
            }

            if (_order.StopLoss.HasValue == false && checkBoxStopLoss.Checked)
            {
                numericUpDownStopLoss.Value = (ask.Value + bid.Value) / 2m;
            }

            UpdateUI();
        }

        /// <summary>
        /// Obtain the currently assigned slippage (null if none).
        /// </summary>
        /// <returns></returns>
        decimal? GetSlippage()
        {
            if (numericUpDownSlippage.Enabled)
            {
                return numericUpDownSlippage.Value;
            }
            return null;
        }

        decimal GetOrderVolumeForUI()
        {
            if (_session.Symbol.IsForexPair)
            {
                return (decimal)_order.CurrentVolume / _session.LotSize;
            }

            return _order.CurrentVolume;
        }

        int GetOrderVolumeFromUI()
        {
            if (_session.Symbol.IsForexPair)
            {
                return (int)(this._numericUpDownVolumeValue * _session.LotSize);
            }

            return (int)this._numericUpDownVolumeValue;
        }

        protected void ModifyOrder(bool close)
        {
            // ActiveOrder closing.
            if (close)
            {
                string operationResultMessage;
                if (_order.Close(out operationResultMessage))
                {
                    // ActiveOrder closed, hide.
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Failed to close order [" + operationResultMessage + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // If closing an order, no additional operations are performed.
                return;
            }

            // Volume modification.
            bool volumeModificationNeeded = GetOrderVolumeFromUI() != _order.CurrentVolume;
            if (volumeModificationNeeded)
            {// Some modification in closeVolume was requested.
                bool result = true;
                string operationResultMessage = string.Empty;

                if (_numericUpDownVolumeValue > GetOrderVolumeForUI())
                {
                    result = _order.IncreaseVolume(GetOrderVolumeFromUI() - _order.CurrentVolume, GetSlippage(), null, out operationResultMessage);
                }
                else if (_numericUpDownVolumeValue < GetOrderVolumeForUI())
                {
                    result = _order.DecreaseVolume(_order.CurrentVolume - GetOrderVolumeFromUI(), GetSlippage(), null, out operationResultMessage);
                }

                if (result == false)
                {
                    MessageBox.Show("Failed to modify order volume [" + operationResultMessage + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Showed error; no more operations are taken; continue showing dialog.
                    return;
                }
            }

            // Additional parameters modification.
            decimal? stopLoss = null;
            decimal? takeProfit = null;
            decimal? openTargetPrice = null;
            bool updateRequired = false;

            updateRequired = _order.StopLossAssigned != checkBoxStopLoss.Checked
                || (checkBoxStopLoss.Checked && _order.StopLoss != numericUpDownStopLoss.Value)
                || (_order.TakeProfitAssigned != checkBoxTakeProfit.Checked
                || (checkBoxTakeProfit.Checked && _order.TakeProfit != numericUpDownTakeProfit.Value));

            updateRequired = updateRequired ||
                (
                    groupBoxPending.Visible 
                    && _order.State == OrderStateEnum.Submitted
                    && _order.OpenPrice != numericUpDownPendingPrice.Value
                );

            if (checkBoxStopLoss.Checked)
            {
                stopLoss = numericUpDownStopLoss.Value;
            }
            else
            {
                if (_order.StopLoss.HasValue)
                {// Set to no value.
                    updateRequired = true;
                    stopLoss = decimal.MinValue;
                }
            }

            if (checkBoxTakeProfit.Checked)
            {
                takeProfit = numericUpDownTakeProfit.Value;
            }
            else
            {
                if (_order.TakeProfit.HasValue)
                {// Set to no value.
                    updateRequired = true;
                    takeProfit = decimal.MinValue;
                }
            }

            if (groupBoxPending.Visible 
                && _order.State == OrderStateEnum.Submitted
                && _order.OpenPrice != numericUpDownPendingPrice.Value)
            {
                updateRequired = true;
                openTargetPrice = numericUpDownPendingPrice.Value;
            }

            if (updateRequired)
            {
                string operationResultMessage = string.Empty;
                if (_order.ModifyRemoteParameters(stopLoss, takeProfit, openTargetPrice, out operationResultMessage) == false)
                {
                    if (volumeModificationNeeded)
                    {// We succeeded closeVolume modification, and failed additional params, so show the operationResult.
                        MessageBox.Show("Operation partial success (volume was modified); failed to modify order additional parameters[" + operationResultMessage + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Failed to modify order [" + operationResultMessage + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
            }

            // Operation ended OK, close.
            this.Hide();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _numericUpDownTakeProfitValue = numericUpDownTakeProfit.Value;
            _numericUpDownStopLossValue = numericUpDownStopLoss.Value;

            UpdateUI();
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            ModifyOrder(false);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            ModifyOrder(true);
        }

        private void numericUpDownVolume_ValueChanged(object sender, EventArgs e)
        {
            _numericUpDownVolumeValue = numericUpDownVolume.Value;
            UpdateUI();
        }

        private void numericUpDownPendingPrice_ValueChanged(object sender, EventArgs e)
        {
            _numericUpDownPendingPrice = numericUpDownPendingPrice.Value;
            UpdateUI();
        }

        private void buttonStopLossPriceHelper_Click(object sender, EventArgs e)
        {
            PriceHelpControl.ShowHelperControlAt(((Button)sender).Location, this, numericUpDownStopLoss);
        }

        private void buttonTakeProfitPriceHelper_Click(object sender, EventArgs e)
        {
            PriceHelpControl.ShowHelperControlAt(((Button)sender).Location, this, numericUpDownTakeProfit);
        }

        private void buttonOpeningPricePriceHelper_Click(object sender, EventArgs e)
        {
            PriceHelpControl.ShowHelperControlAt(((Button)sender).Location, this, numericUpDownPendingPrice);
        }

        private void numericUpDownVolume_KeyDown(object sender, KeyEventArgs e)
        {
            _numericUpDownPendingPrice = numericUpDownPendingPrice.Value;
            UpdateUI();
        }

    }
}
