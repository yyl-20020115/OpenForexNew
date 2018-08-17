using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control allows to visualize order execution source positions.
    /// </summary>
    public partial class PositionsControl : CommonBaseControl
    {
        ISourceOrderExecution _provider;
        ISourceAndExpertSessionManager _manager;
        ISourceDataDelivery _dataDelivery;

        /// <summary>
        /// 
        /// </summary>
        protected Position SelectedPosition
        {
            get
            {
                if (listView.SelectedItems.Count == 0)
                {
                    return null;
                }

                return ((Position)listView.SelectedItems[0].Tag);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PositionsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(ISourceAndExpertSessionManager manager, ISourceDataDelivery dataDelivery, ISourceOrderExecution provider)
        {
            _manager = manager;
            _provider = provider;
            _dataDelivery = dataDelivery;
            
            if (_provider != null)
            {
                _provider.TradeEntities.PositionsUpdatedEvent += new PositionsUpdateDelegate(TradeEntities_PositionsUpdatedEvent);
                _provider.TradeEntities.PositionsAddedEvent += new PositionsUpdateDelegate(TradeEntities_PositionsAddedEvent);
                _provider.TradeEntities.PositionsRemovedEvent += new PositionsUpdateDelegate(TradeEntities_PositionsRemovedEvent);
                _provider.PositionsUpdateEvent += new PositionUpdateDelegate(_provider_PositionUpdateEvent);
            }

            UpdateUI();

            if (listView.Items.Count > 0 && listView.SelectedItems.Count == 0)
            {
                listView.Items[0].Selected = true;
            }
        }

        public void UnInitialize()
        {
            if (_provider != null)
            {
                _provider.TradeEntities.PositionsUpdatedEvent -= new PositionsUpdateDelegate(TradeEntities_PositionsUpdatedEvent);
                _provider.TradeEntities.PositionsAddedEvent -= new PositionsUpdateDelegate(TradeEntities_PositionsAddedEvent);
                _provider.TradeEntities.PositionsRemovedEvent -= new PositionsUpdateDelegate(TradeEntities_PositionsRemovedEvent);
                _provider.PositionsUpdateEvent -= new PositionUpdateDelegate(_provider_PositionUpdateEvent);
            }

            _manager = null;
            _provider = null;
            _dataDelivery = null;
        }

        public override void UnInitializeControl()
        {
            UnInitialize();

            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);

            base.UnInitializeControl();
        }

        void TradeEntities_PositionsRemovedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<IPosition> positions)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        void TradeEntities_PositionsAddedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<IPosition> positions)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        void TradeEntities_PositionsUpdatedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<IPosition> positions)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        void _provider_PositionUpdateEvent(IOrderSink provider, AccountInfo accountInfo, PositionInfo[] positionInfo)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        void UpdateUI()
        {
            this.Enabled = _provider != null;
            if (_provider == null)
            {
                return;
            }

            List<Position> positions = _provider.TradeEntities.Positions;
            //lock (_provider.TradeEntities)
            //{
            //    positions = GeneralHelper.EnumerableToList<Position>(_provider.TradeEntities.PositionsUnsafe);
            //}

            if (_provider.DefaultAccount != null)
            {
                if (toolStripComboBoxAccount.Items.Count == 0)
                {
                    toolStripComboBoxAccount.Items.Add(_provider.DefaultAccount.Info.Name);
                    toolStripComboBoxAccount.SelectedIndex = 0;
                }
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if (listView.Items.Count <= i)
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems.Clear();
                    for (int j = 0; j < listView.Columns.Count; j++)
                    {
                        item.SubItems.Add(string.Empty);
                    }

                    listView.Items.Add(item);
                }

                SetItemAsPosition(listView.Items[i], positions[i]);
            }

            while (listView.Items.Count > positions.Count)
            {
                listView.Items.RemoveAt(listView.Items.Count - 1);
            }
        }

        void SetItemAsPosition(ListViewItem item, Position position)
        {
            if (item.SubItems[0].Text != position.Symbol.Name)
            {
                // Symbol
                item.SubItems[0].Text = position.Symbol.Name;
            }
            
            string volume = position.Volume.ToString("### ### ##0.");
            if (item.SubItems[1].Text != volume)
            {
                // Volume
                item.SubItems[1].Text = volume;
            }

            string marketValue = GeneralHelper.ToString(position.Info.MarketValue, "### ### ##0.");
            if (item.SubItems[2].Text != marketValue)
            {
                // Mkt Value
                item.SubItems[2].Text = marketValue;
            }

            string pending  = GeneralHelper.ToString(position.Info.PendingBuyVolume);
            if (item.SubItems[2].Text != pending)
            {
                // Pending
                item.SubItems[3].Text = pending;
            }

            string basePrice = GeneralHelper.ToString(position.BasePrice);
            if (item.SubItems[4].Text != basePrice)
            {
                // Base Price
                item.SubItems[4].Text = basePrice;
            }

            if (item.SubItems[5].Text != GeneralHelper.ToString(position.Result))
            {
                // Open Result
                item.SubItems[5].Text = GeneralHelper.ToString(position.Result);
            }
            
            if (item.SubItems[6].Text != GeneralHelper.ToString(position.Info.ClosedResult))
            {
                // Closed Result
                item.SubItems[6].Text = GeneralHelper.ToString(position.Info.ClosedResult);
            }

            if (item.SubItems[7].Text != GeneralHelper.ToString(position.Price))
            {
                // Current Price
                item.SubItems[7].Text = GeneralHelper.ToString(position.Price);
            }

            if (item.Tag != position)
            {
                item.Tag = position;
            }
        }

        private void toolStripButtonNewOrder_Click(object sender, EventArgs e)
        {
            if (_manager == null)
            {
                return;
            }

            Position position = SelectedPosition;
            if (position == null)
            {
                MessageBox.Show("Select a position to put order to.");
                return;
            }

            if (position.DataDelivery.OperationalState != OperationalStateEnum.Operational
                || position.OrderExecutionProvider.OperationalState != OperationalStateEnum.Operational)
            {
                MessageBox.Show("Position data or order execution provider not operational.");
                return;
            }

            IQuoteProvider quotes = _manager.ObtainQuoteProvider(position.DataDelivery.SourceId, position.Symbol);
            DataSessionInfo? info = _manager.GetSymbolDataSessionInfo(position.DataDelivery.SourceId, position.Symbol);

            if (info.HasValue == false)
            {
                MessageBox.Show("Failed to establish position session.", Application.ProductName + " - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            NewOrderControl control = new NewOrderControl(quotes, info.Value, true, true);
            
            control.CreatePlaceOrderEvent += new NewOrderControl.CreatePlaceOrderDelegate(SubmitPositionOrder);

            HostingForm f = new HostingForm("New Order", control);
            f.FormBorderStyle = FormBorderStyle.FixedSingle;
            f.MaximizeBox = false;
            f.ShowDialog();
            control.CreatePlaceOrderEvent -= new NewOrderControl.CreatePlaceOrderDelegate(SubmitPositionOrder);
        }

        bool SubmitPositionOrder(OrderTypeEnum orderType, bool synchronous, int volume, decimal? allowedSlippage, decimal? desiredPrice,
            decimal? sourceTakeProfit, decimal? sourceStopLoss, bool allowExistingOrdersManipulation, string comment, out string operationResultMessage)
        {
            SystemMonitor.CheckThrow(SelectedPosition != null, "Selected session must be not null to create orders.");

            Position position = SelectedPosition.OrderExecutionProvider.TradeEntities.ObtainPositionBySymbol(SelectedPosition.Symbol);

            if (position == null)
            {
                operationResultMessage = "Failed to find position order.";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            string submitResult = string.Empty;
            if (synchronous == false)
            {
                submitResult = position.Submit(orderType, volume,
                    desiredPrice, allowedSlippage, sourceTakeProfit, sourceStopLoss, out operationResultMessage);
            }
            else
            {
                PositionExecutionInfo info;

                if (allowExistingOrdersManipulation)
                {
                    int directionalVolume = OrderInfo.TypeIsBuy(orderType) ? volume : -volume;
                    submitResult = position.ExecuteMarketBalanced(directionalVolume, desiredPrice, allowedSlippage, TimeSpan.FromSeconds(15), 
                        out info, out operationResultMessage);
                }
                else
                {
                    submitResult = position.ExecuteMarket(orderType, volume, desiredPrice, allowedSlippage, sourceTakeProfit,
                        sourceStopLoss, TimeSpan.FromSeconds(15), out info, out operationResultMessage);
                }
            }

            if (string.IsNullOrEmpty(submitResult))
            {
                SystemMonitor.Error("Failed to place order [" + operationResultMessage + "]");
                return false;
            }

            return true;
        }

        private void toolStripButtonAddSymbol_Click(object sender, EventArgs e)
        {
            if (_dataDelivery != null && _provider != null)
            {
                Dictionary<Symbol, TimeSpan[]> symbols = _dataDelivery.SearchSymbols(toolStripTextBoxSymbol.Text);

                if (symbols == null || symbols.Count == 0)
                {
                    MessageBox.Show("Failed to find position.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (symbols.Count > 1)
                {
                    MessageBox.Show("More than one symbol found containing [" + toolStripTextBoxSymbol.Text + "]. Please specify symbol more precisely.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _provider.TradeEntities.ObtainPositionBySymbol(GeneralHelper.EnumerableToList<Symbol>(symbols.Keys)[0]);
            }

        }

        private void toolStripButtonClose_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView.SelectedItems)
            {
                Position position = (Position)item.Tag;
                //if (position == null)
                //{
                //    MessageBox.Show("Select a position to put order to.");
                //    return;
                //}

                if (position.DataDelivery.OperationalState != OperationalStateEnum.Operational
                    || position.OrderExecutionProvider.OperationalState != OperationalStateEnum.Operational)
                {
                    MessageBox.Show("Position data or order execution provider not operational.");
                    return;
                }

                string operationResultMessage;
                PositionExecutionInfo executionResult;
                if (string.IsNullOrEmpty(position.ExecuteMarketBalancedClose(null, TimeSpan.FromSeconds(30), out executionResult, out operationResultMessage)))
                {
                    MessageBox.Show("Failed to close position - " + operationResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (executionResult.Result == PositionExecutionInfo.ExecutionResultEnum.PartialSuccess)
                {
                    MessageBox.Show("Partial position close [" + executionResult.VolumeExecuted.ToString() + " out of " + executionResult.VolumeRequested.ToString() + "].", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        private void listContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                e.Cancel = true;
            }
        }

        private void removeFromListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_provider == null)
            {
                return;
            }

            foreach (ListViewItem item in this.listView.SelectedItems)
            {
                Position position = (Position)item.Tag;
                _provider.TradeEntities.RemovePosition(position);
            }
        }
    }
}
