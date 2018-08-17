//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Text;
//using System.Windows.Forms;
//using ForexPlatform;
//using CommonSupport;
//using CommonFinancial;

//namespace ForexPlatformFrontEnd
//{
//    /// <summary>
//    /// Control visualizes the usage of master orders and application of MasterTradingExpert.
//    /// </summary>
//    public partial class MasterOrderControl : CommonBaseControl, IQuotationProvider
//    {
//        MasterTradingExpert _expert;

//        // No entries means all are visibile.
//        List<OrderInformation.StateEnum> _orderFilterStates = new List<OrderInformation.StateEnum>();


//        public OperationalStateEnum OperationalState
//        {
//            get { return _expert.OperationalState; }
//        }

//        List<ExpertSession> SelectedSessions
//        {
//            get
//            {
//                List<ExpertSession> result = new List<ExpertSession>();
//                foreach (ListViewItem item in listViewSessions.Items)
//                {
//                    if (item.Checked)
//                    {
//                        result.Add(item.Tag as ExpertSession);
//                    }
//                }

//                return result;
//            }
//        }

//        SessionInfo SessionInfo
//        {
//            get
//            {
//                if (_expert != null && _expert.SessionManager != null && _expert.SessionManager.Sessions.Count > 0)
//                {
//                    return _expert.SessionManager.Sessions[0].SessionInfo;
//                }
//                return new SessionInfo();
//            }
//        }

//        /// <summary>
//        /// The current time on the quotation provider.
//        /// </summary>
//        DateTime SourceTime 
//        {
//            get
//            {

//            }
//        }

//        public event OperationalStatusChangedDelegate OperationalStatusChangedEvent;
//        public event QuoteUpdateDelegate QuoteUpdateEvent;

//        /// <summary>
//        /// 
//        /// </summary>
//        public MasterOrderControl()
//        {
//            InitializeComponent();
//        }

//        protected override void OnLoad(EventArgs e)
//        {
//            base.OnLoad(e);

//            foreach (OrderInformation.StateEnum value in Enum.GetValues(typeof(OrderInformation.StateEnum)))
//            {
//                ToolStripMenuItem item = new ToolStripMenuItem(value.ToString());
//                item.Tag = value;
//                toolStripDropDownButtonShow.DropDownItems.Add(item);
//            }

//            listViewOrders.Groups.Clear();
//            listViewOrders.Groups.Add(OrderInformation.StateEnum.Opened.ToString(), OrderInformation.StateEnum.Opened.ToString());
//            listViewOrders.Groups.Add(OrderInformation.StateEnum.PlacedPending.ToString(), OrderInformation.StateEnum.PlacedPending.ToString());
//            listViewOrders.Groups.Add(OrderInformation.StateEnum.Closed.ToString(), OrderInformation.StateEnum.Closed.ToString());
            
//            foreach (string name in Enum.GetNames(typeof(OrderInformation.StateEnum)))
//            {
//                listViewOrders.Groups.Add(name, name);
//            }

//            foreach (string name in Enum.GetNames(typeof(MasterOrder.VolumeDistributionEnum)))
//            {
//                toolStripComboBoxVolumeDistribution.Items.Add(name);
//            }

//            toolStripComboBoxVolumeDistribution.SelectedIndex = 1;
//            toolStripComboBoxVolumeDistribution.Enabled = false;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public bool Initialize(MasterTradingExpert expert)
//        {
//            _expert = expert;
//            _expert.OperationalStatusChangedEvent += OperationalStatusChangedEvent;

//            _expert.SessionManager.SessionCreatedEvent += new GeneralHelper.GenericDelegate<IExpertSessionManager, ExpertSession>(SessionManager_SessionCreatedEvent);
//            _expert.SessionManager.SessionDestroyingEvent += new GeneralHelper.GenericDelegate<IExpertSessionManager, ExpertSession>(SessionManager_SessionDestroyingEvent);

//            foreach (ExpertSession session in _expert.SessionManager.Sessions)
//            {
//                if (session.OperationalState == OperationalStateEnum.Operational &&
//                    session.DataProvider.OperationalState == OperationalStateEnum.Operational)
//                {
//                    session.DataProvider.RequestValuesUpdate();
//                }
//            }

//            UpdateUI();

//            return true;
//        }

//        void SessionManager_SessionCreatedEvent(IExpertSessionManager parameter1, ExpertSession session)
//        {
//            session.DataProvider.RequestValuesUpdate();
//            session.DataProvider.ValuesUpdateEvent += new QuoteUpdateDelegate(DataProvider_ValuesUpdateEvent);
//            session.OperationalStatusChangedEvent += new OperationalStatusChangedDelegate(session_OperationalStatusChangedEvent);

//            UpdateUI();
//        }

//        void DataProvider_ValuesUpdateEvent(IQuotationProvider provider, DataBarUpdateType updateType, int count, int stepsRemaining)
//        {
//            WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.DefaultDelegate(UpdateUI));
//        }

//        void session_OperationalStatusChangedEvent(IOperational parameter1, OperationalStateEnum parameter2)
//        {
//            WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.DefaultDelegate(UpdateUI));
//        }

//        void session_OperationalStatusChangedEvent()
//        {
            
//        }

//        void SessionManager_SessionDestroyingEvent(IExpertSessionManager parameter1, ExpertSession session)
//        {
//            UpdateUI();
//            session.OperationalStatusChangedEvent -= new OperationalStatusChangedDelegate(session_OperationalStatusChangedEvent);
//            session.DataProvider.ValuesUpdateEvent -= new QuoteUpdateDelegate(DataProvider_ValuesUpdateEvent);
//        }

//        public override void  UnInitializeControl()
//        {
//            if (_expert != null)
//            {
//                _expert.OperationalStatusChangedEvent -= OperationalStatusChangedEvent;
//                if (_expert.SessionManager != null)
//                {
//                    _expert.SessionManager.SessionCreatedEvent -= new GeneralHelper.GenericDelegate<IExpertSessionManager, ExpertSession>(SessionManager_SessionCreatedEvent);
//                    _expert.SessionManager.SessionDestroyedEvent -= new GeneralHelper.GenericDelegate<IExpertSessionManager, ExpertSession>(SessionManager_SessionDestroyingEvent);
//                }
//                _expert = null;
//            }
//            base.UnInitializeControl();
//        }

//        private void toolStripButtonNewOrder_Click(object sender, EventArgs e)
//        {
//            if (SelectedSessions.Count == 0)
//            {
//                MessageBox.Show("Select at least one slave session/account to place order to.");
//                return;
//            }

//            NewOrderControl control = new NewOrderControl(this, SessionInfo);
//            control.CreatePlaceOrderEvent += new NewOrderControl.CreatePlaceOrderDelegate(
//                delegate(OrderTypeEnum orderType, decimal volume, decimal allowedSlippage, 
//                decimal desiredPrice, decimal sourceTakeProfit, decimal sourceStopLoss, string comment, out string operationResultMessage)
//                {
//                    MasterOrder.VolumeDistributionEnum volumeDistribution = (MasterOrder.VolumeDistributionEnum)Enum.GetValues(typeof(MasterOrder.VolumeDistributionEnum)).GetValue(toolStripComboBoxVolumeDistribution.SelectedIndex);

//                    List<ExpertSession> selectedSessions = SelectedSessions;

//                    MasterOrder order = new MasterOrder(selectedSessions, volumeDistribution, true);
//                    if (order.Place(orderType, volume, allowedSlippage, desiredPrice, 
//                        sourceTakeProfit, sourceStopLoss, comment, out operationResultMessage) == false)
//                    {
//                        order.UnInitialize();
//                        operationResultMessage = "Failed to open order [" + operationResultMessage + "].";
//                        return false;
//                    }

//                    _expert.AddOrder(order);

//                    if (order.OpenedSlaveOrders.Count < selectedSessions.Count)
//                    {
//                        MessageBox.Show("Some orders were not placed [" + (selectedSessions.Count - order.OpenedSlaveOrders.Count).ToString() + "] failed out of [" + selectedSessions.Count.ToString() + "]" + Environment.NewLine + operationResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
//                    }

//                    UpdateUI();

//                    return true;
//                });

//            HostingForm f = new HostingForm("New Order", control);
//            f.FormBorderStyle = FormBorderStyle.FixedSingle;
//            f.ShowDialog();

//        }

//        bool IsOrderVisible(MasterOrder order)
//        {
//            if (_orderFilterStates.Count == 0)
//            {
//                return true;
//            }

//            foreach (OrderInformation.StateEnum state in _orderFilterStates)
//            {
//                if (order.State == state)
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        ListViewItem SetItemAsSession(ListViewItem item, ExpertSession session)
//        {
//            // Must be at the top.
//            item.SubItems.Clear();

//            item.Tag = session;
//            item.Text = session.OrderExecutionProvider.Account.Name;
//            item.Checked = true;

//            item.SubItems.Add(session.SessionInfo.Name);

//            item.SubItems.Add(session.DataProvider.Bid.ToString());
//            item.SubItems.Add(session.DataProvider.Ask.ToString());

//            if (session.OrderExecutionProvider.OperationalState != OperationalStateEnum.Operational)
//            {
//                item.BackColor = Color.Salmon;
//            }

//            return item;
//        }

//        ListViewItem SetItemAsOrder(ListViewItem item, MasterOrder order)
//        {
//            if (item.Tag != order)
//            {
//                item.Tag = order;
//            }

//            if (item.UseItemStyleForSubItems != false)
//            {
//                item.UseItemStyleForSubItems = false;
//            }

//            ListViewGroup group = listViewOrders.Groups[order.State.ToString()];
//            if (item.Group != group)
//            {
//                item.Group = group;
//            }

//            int imageIndex = 0;
//            if (order.IsBuy)
//            {
//                imageIndex = order.State == OrderInformation.StateEnum.Opened ? 0 : 2;
//            }
//            else
//            {
//                imageIndex = order.State == OrderInformation.StateEnum.Opened ? 1 : 3;
//            }

//            if (order.State != OrderInformation.StateEnum.Opened &&
//                order.State != OrderInformation.StateEnum.Closed &&
//                order.State != OrderInformation.StateEnum.PlacedPending &&
//                order.State != OrderInformation.StateEnum.Canceled)
//            {
//                imageIndex = 4;
//            }

//            if (item.ImageIndex != imageIndex)
//            {
//                item.ImageIndex = imageIndex;
//            }

//            while (item.SubItems.Count < 10)
//            {
//                item.SubItems.Add("");
//            }

//            string id = order.Id.Print();
//            if (id == null)
//            {
//                id = string.Empty;
//            }

//            if (item.SubItems[0].Text != id)
//            {
//                item.SubItems[0].Text = id;
//            }

//            if (item.SubItems[1].Text != order.Symbol.Value.Name)
//            {
//                item.SubItems[1].Text = order.Symbol.Value.Name;
//            }

//            if (item.SubItems[2].Text != GeneralHelper.GetShortDateTime(order.LocalOpenTime))
//            {
//                item.SubItems[2].Text = GeneralHelper.GetShortDateTime(order.LocalOpenTime);
//            }

//            if (item.SubItems[3].Text != (order.Type.ToString()))
//            {
//                item.SubItems[3].Text = (order.Type.ToString());
//            }

//            if (item.SubItems[4].Text != (order.CurrentVolume.ToString()))
//            {
//                item.SubItems[4].Text = (order.CurrentVolume.ToString());
//            }

//            if (item.SubItems[5].Text != (order.OpenPrice.ToString()))
//            {
//                item.SubItems[5].Text = (order.OpenPrice.ToString());
//            }

//            if (item.SubItems[6].Text != (order.ClosePrice.ToString()))
//            {
//                item.SubItems[6].Text = (order.ClosePrice.ToString());
//            }

//            if (item.SubItems[7].Text != (order.RemoteStopLoss.ToString()))
//            {
//                item.SubItems[7].Text = (order.RemoteStopLoss.ToString());
//            }

//            if (item.SubItems[8].Text != (order.RemoteTakeProfit.ToString()))
//            {
//                item.SubItems[8].Text = (order.RemoteTakeProfit.ToString());
//            }

//            decimal currentResultPips = order.GetResult(Order.ResultModeEnum.Pips);
//            decimal currentResultBaseCurrency = order.GetResult(Order.ResultModeEnum.Currency);

//            if (item.SubItems[9].Text != currentResultPips.ToString() + "p / " + currentResultBaseCurrency)
//            {
//                item.SubItems[9].Text = currentResultPips.ToString() + "p / " + currentResultBaseCurrency;
//            }

//            if (currentResultPips < 0)
//            {
//                if (item.SubItems[9].BackColor != Color.MistyRose)
//                {
//                    item.SubItems[9].BackColor = Color.MistyRose;
//                }
//            }
//            else
//            {
//                if (item.SubItems[9].BackColor != Color.Transparent)
//                {
//                    item.SubItems[9].BackColor = Color.Transparent;
//                }
//            }

//            return item;

//        }

//        private void toolStripButtonCloseMasterOrder_Click(object sender, EventArgs e)
//        {
//            if (listViewOrders.SelectedItems.Count > 1)
//            {
//                MessageBox.Show("Select one master order to close.");
//                return;
//            }

//            if (listViewOrders.SelectedItems.Count == 0)
//            {
//                return;
//            }

//            ListViewItem item = listViewOrders.SelectedItems[0];
//            MasterOrder order = item.Tag as MasterOrder;

//            if (order.State != OrderInformation.StateEnum.Opened && 
//                order.State != OrderInformation.StateEnum.PlacedPending)
//            {
//                return;
//            }

//            string operationResultMessage = string.Empty;
//            bool result = false;
//            if (order.State == OrderInformation.StateEnum.Opened)
//            {
//                result = order.Close(out operationResultMessage);
//            }
//            else if (order.State == OrderInformation.StateEnum.PlacedPending)
//            {
//                result = order.Cancel(out operationResultMessage);
//            }

//            if (result == false)
//            {
//                MessageBox.Show("Failed to close order [" + operationResultMessage + "]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }

//            UpdateUI();
//        }

//        /// <summary>
//        /// Update user interface based on the underlying information.
//        /// </summary>
//        void UpdateUI()
//        {
//            // Orders
//            int orderIndex = 0;
//            foreach (MasterOrder order in _expert.MasterOrders)
//            {
//                if (IsOrderVisible(order) == false)
//                {
//                    continue;
//                }

//                ListViewItem item;
//                if (listViewOrders.Items.Count <= orderIndex)
//                {
//                    item = new ListViewItem();
//                    listViewOrders.Items.Add(item);
//                }
//                else
//                {
//                    item = listViewOrders.Items[orderIndex];
//                }

//                SetItemAsOrder(item, order);

//                orderIndex++;
//            }

//            while (listViewOrders.Items.Count > orderIndex)
//            {
//                listViewOrders.Items.RemoveAt(listViewOrders.Items.Count - 1);
//            }

//            // Sessions
//            int sessionIndex = 0;
//            foreach (ExpertSession session in _expert.SessionManager.Sessions)
//            {
//                ListViewItem item;
//                if (listViewSessions.Items.Count <= sessionIndex)
//                {
//                    item = new ListViewItem();
//                    listViewSessions.Items.Add(item);
//                }
//                else
//                {
//                    item = listViewSessions.Items[sessionIndex];
//                }

//                SetItemAsSession(item, session);

//                sessionIndex++;
//            }

//            while (listViewSessions.Items.Count > sessionIndex)
//            {
//                listViewSessions.Items.RemoveAt(listViewSessions.Items.Count - 1);
//            }
//        }

//        #region IDataProvider Members

//        public decimal Ask
//        {
//            get 
//            {
//                if (_expert == null || _expert.SessionManager.Sessions.Count == 0)
//                {
//                    return decimal.MinValue;
//                }

//                return _expert.SessionManager.Sessions[0].DataProvider.Ask;
//            }
//        }

//        public decimal Bid
//        {
//            get 
//            {
//                if (_expert == null || _expert.SessionManager.Sessions.Count == 0)
//                {
//                    return decimal.MinValue;
//                }

//                return _expert.SessionManager.Sessions[0].DataProvider.Bid;
//            }
//        }

//        public decimal Spread
//        {
//            get 
//            {
//                if (_expert == null || _expert.SessionManager.Sessions.Count == 0)
//                {
//                    return decimal.MinValue;
//                }

//                return _expert.SessionManager.Sessions[0].DataProvider.Spread;
//            }
//        }

//        public DateTime LastDataBarTime
//        {
//            get { return DateTime.Now; }
//        }

//        public bool RequestQuoteUpdate()
//        {
//            return false;
//        }

//        #endregion

//        private void toolStripButtonAddSession_Click(object sender, EventArgs e)
//        {
//            CreateExpertSessionForm form = new CreateExpertSessionForm((LocalExpertHost)_expert.SessionManager);
//            form.createExpertSessionControl.AllowGraphicsSession = false;
//            form.createExpertSessionControl.AllowBackTestingSession = false;

//            form.ShowDialog();
//        }


//    }
//}
