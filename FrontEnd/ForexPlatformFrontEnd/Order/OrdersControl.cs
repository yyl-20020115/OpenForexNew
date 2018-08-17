using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonSupport;
using CommonFinancial;
using ForexPlatformFrontEnd.Properties;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control providers information and manual manipulation for a set of orders.
    /// Takes as main input the Manager member (to visualize all its sessions).
    /// </summary>
    public partial class OrdersControl : CommonBaseControl
    {
        int _fullModeHeigth = 0;

        /// <summary>
        /// Should the account selection tools be shown always, or only when more than one account on the sessionInformation manager.
        /// When managing an multi entire ISessionManager, always show, otherwise for a single sessionInformation not needed.
        /// </summary>
        bool _alwaysShowAccountSelectionTools = false;

        bool _allowCompactMode = true;
        /// <summary>
        /// Is the control allowed to enter compact mode.
        /// </summary>
        public bool AllowCompactMode
        {
            get { return _allowCompactMode; }
            set { _allowCompactMode = value; }
        }

        ActiveOrder.ResultModeEnum _resultsDisplayMode = ActiveOrder.ResultModeEnum.Pips;
        public ActiveOrder.ResultModeEnum ResultsDisplayMode
        {
            get { return _resultsDisplayMode; }
        }

        ExpertSession _selectedSession = null;

        bool _compactMode = false;

        List<ExpertSession> _sessions = new List<ExpertSession>();

        ISourceAndExpertSessionManager _sessionManager;
        public ISourceAndExpertSessionManager SessionManager
        {
            get { return _sessionManager; }
            set 
            {
                // Keep it here, before the first return.
                this.Enabled = value != null;

                if (value == _sessionManager)
                {
                    return;
                }

                if (_sessionManager != null)
                {
                    _sessionManager.SessionsUpdateEvent -= new GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(_sessionManager_SessionsUpdateEvent);
                    foreach (ExpertSession session in _sessionManager.SessionsArray)
                    {
                        RemoveSession(session);
                    }
                }

                _sessionManager = value;
                
                SystemMonitor.CheckError(_sessions.Count == 0, "Session management error (all must be removed at this point).");
                _sessions.Clear();

                if (_sessionManager != null)
                {
                    _alwaysShowAccountSelectionTools = true;

                    _sessionManager.SessionsUpdateEvent += new GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(_sessionManager_SessionsUpdateEvent);
                    foreach (ExpertSession session in _sessionManager.SessionsArray)
                    {
                        AddSession(session);
                    }
                }

                UpdateUI();
            }
        }

        /// <summary>
        /// Use this to have the control in sindle sessionInformation mode.
        /// </summary>
        public ExpertSession SingleSession
        {
            get
            {
                if (_sessions.Count == 0 || _sessions.Count > 1)
                {
                    return null;
                }
                return _sessions[0];
            }

            set
            {
                this.Enabled = value != null;

                SystemMonitor.CheckThrow(_sessionManager == null, "Can not mix 2 modes in a orders control.");
                while (_sessions.Count > 0)
                {
                    RemoveSession(_sessions[0]);
                }

                if (value != null)
                {
                    AddSession(value);
                    _selectedSession = value;
                }

                // Make sure those are in this order.
                UpdateUI();
            }
        }

        // No entries means all are visibile.
        List<OrderStateEnum> _orderFilterStates = new List<OrderStateEnum>();

        public bool AllowOrderManagement
        {
            get { return toolStripOrders.Visible; }
            set { toolStripOrders.Visible = value; }
        }

        public delegate void SelectedOrderChangedDelegate(ExpertSession orderSession, Order newSelectedOrder);
        public event SelectedOrderChangedDelegate SelectedOrderChangedEvent;

        public delegate void SelectedSessionChangedDelegate(ExpertSession session);
        /// <summary>
        /// Will be invoked with null parameter in case no particular sessionInformation is selected and all sessions in sessionInformation manager are applicable.
        /// </summary>
        public event SelectedSessionChangedDelegate SelectedSessionChangedEvent;

        /// <summary>
        /// 
        /// </summary>
        public OrdersControl()
        {
            InitializeComponent();
            _fullModeHeigth = this.Height;
        }

        public override void UnInitializeControl()
        {
            this.SessionManager = null;
            base.UnInitializeControl();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            toolStripButtonPopupNotifications.Checked = Settings.Default.DeveloperMode;

            toolStripDropDownButtonShow.DropDownItems.Add("All");
            toolStripDropDownButtonShow.DropDownItems.Add(new ToolStripSeparator());

            toolStripDropDownButtonShow.DropDownItems.Add("Opened and Pending");
            toolStripDropDownButtonShow.DropDownItems.Add(new ToolStripSeparator());

            toolStripDropDownButtonShow.DropDownItems.Add("Opened and Closed");
            toolStripDropDownButtonShow.DropDownItems.Add(new ToolStripSeparator());

            toolStripDropDownButtonShow.DropDownItems.Add("Opened, Pending and Closed");
            toolStripDropDownButtonShow.DropDownItems.Add(new ToolStripSeparator());
            
            foreach (OrderStateEnum value in Enum.GetValues(typeof(OrderStateEnum)))
            {
                ToolStripMenuItem item = new ToolStripMenuItem(value.ToString());
                item.Tag = value;
                toolStripDropDownButtonShow.DropDownItems.Add(item);
            }

            UpdateUI();

            //Manager = null;
        }

        bool IsOrderVisible(Order order)
        {
            if (_orderFilterStates.Count == 0)
            {
                return true;
            }

            foreach(OrderStateEnum state in _orderFilterStates)
            {
                if (order.State == state)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// NON UI Thread.
        /// </summary>
        public void SelectOrder(Order order)
        {
            if (this.IsHandleCreated)
            {
                WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.GenericDelegate<Order>(DoSelectOrder), order);
            }
        }

        protected void DoSelectOrder(Order order)
        {
            listViewOrders.SelectedItems.Clear();
            foreach (ListViewItem item in listViewOrders.Items)
            {
                if (item.Tag == order)
                {
                    item.Selected = true;
                }
            }
        }


        void _sessionManager_SessionsUpdateEvent(ISourceAndExpertSessionManager sessionManager)
        {
            SystemMonitor.CheckThrow(sessionManager == _sessionManager);

            List<ExpertSession> sessions = new List<ExpertSession>(_sessions);
            foreach (ExpertSession session in _sessionManager.SessionsArray)
            {
                if (sessions.Contains(session) == false)
                {// Newly added sessionInformation.
                    AddSession(session);
                }

                sessions.Remove(session);
            }

            foreach (ExpertSession session in sessions)
            {// Those sessions were not present after the manager was updated.
                RemoveSession(session);
            }

            UpdateUI();
        }

        protected bool AddSession(ExpertSession session)
        {
            if (_sessions.Contains(session))
            {
                return false;
            }

            _sessions.Add(session);

            if (session.DataProvider != null && session.DataProvider.Quotes != null)
            {
                session.DataProvider.Quotes.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
            }

            if (session.OrderExecutionProvider != null)
            {
                ITradeEntityManagement management = session.OrderExecutionProvider.TradeEntities;

                if (management != null)
                {
                    management.OrdersAddedEvent += new OrderManagementOrdersUpdateDelegate(history_OrdersAddedEvent);
                    management.OrdersRemovedEvent += new OrderManagementOrdersUpdateDelegate(history_OrdersRemovedEvent);
                    management.OrdersUpdatedEvent += new OrderManagementOrdersUpdateTypeDelegate(management_OrderUpdatedEvent);
                    management.OrdersCriticalInformationChangedEvent += new OrderManagementOrderCriticalModificationDelegate(management_OrdersCriticalInformationChangedEvent);
                }
            }
            return true;
        }

        /// <summary>
        /// An order has been received an update containing critical modification of its information,
        /// so handle this here and show to user.
        /// </summary>
        void management_OrdersCriticalInformationChangedEvent(ITradeEntityManagement provider, AccountInfo account, Order order, OrderInfo updateInfo)
        {
            // Compose the critical update message instantly, as otherwise it is wrong since the order gets instantly updated after this call.
            string criticalUpdateMessage = string.Empty;
            if (order.OpenPrice.HasValue && updateInfo.OpenPrice.HasValue && order.OpenPrice.Value != updateInfo.OpenPrice.Value)
            {
                criticalUpdateMessage = string.Format("Open Price Difference [{0}], Existing [{1}], New [{2}].", order.OpenPrice - updateInfo.OpenPrice, order.OpenPrice.ToString(), updateInfo.OpenPrice) + Environment.NewLine;
            }

            if (order.ClosePrice.HasValue && updateInfo.ClosePrice.HasValue && order.ClosePrice.Value != updateInfo.ClosePrice.Value)
            {
                criticalUpdateMessage += string.Format("Close Price Difference [{0}], Existing [{1}], New [{2}].", order.ClosePrice - updateInfo.ClosePrice, order.ClosePrice.ToString(), updateInfo.ClosePrice) + Environment.NewLine;
            }

            WinFormsHelper.BeginManagedInvoke(this, delegate()
            {
                if (toolStripButtonPopupNotifications.Checked == false)
                {
                    return;
                }

                if (string.IsNullOrEmpty(criticalUpdateMessage) == false)
                {
                    string message = "Account [" + account.Name + "] Order Id[" + order.Id + "] Critical Information Updated" + Environment.NewLine + criticalUpdateMessage;
                    NotificationForm form = new NotificationForm();
                    form.labelMessage.Text = message;
                    form.Text = "Critical Order Update";
                    form.TopMost = true;
                    form.Show();

                    //MessageBox.Show("Critical Order Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        void management_OrderUpdatedEvent(ITradeEntityManagement provider, AccountInfo account, Order[] orders, Order.UpdateTypeEnum[] updateType)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        /// <summary>
        /// NON UI Thread.
        /// </summary>
        void history_OrdersRemovedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        /// <summary>
        /// NON UI Thread.
        /// </summary>
        void history_OrdersAddedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        protected bool RemoveSession(ExpertSession session)
        {
            if (_sessions.Contains(session) == false)
            {
                return false;
            }

            _sessions.Remove(session);

            if (session.DataProvider != null && session.DataProvider.Quotes != null)
            {
                session.DataProvider.Quotes.QuoteUpdateEvent -= new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
            }

            if (session.OrderExecutionProvider != null)
            {
                ITradeEntityManagement management = session.OrderExecutionProvider.TradeEntities;

                if (management != null)
                {
                    management.OrdersAddedEvent -= new OrderManagementOrdersUpdateDelegate(history_OrdersAddedEvent);
                    management.OrdersRemovedEvent -= new OrderManagementOrdersUpdateDelegate(history_OrdersRemovedEvent);
                    management.OrdersUpdatedEvent -= new OrderManagementOrdersUpdateTypeDelegate(management_OrderUpdatedEvent);
                    management.OrdersCriticalInformationChangedEvent -= new OrderManagementOrderCriticalModificationDelegate(management_OrdersCriticalInformationChangedEvent);
                }
            }

            return true;
        }

        
        /// <summary>
        /// Helper.
        /// </summary>
        /// <param name="sessionInformation"></param>
        /// <returns></returns>
        string GetSessionName(ExpertSession session)
        {
            if (session.OperationalState == OperationalStateEnum.NotOperational || session.OperationalState == OperationalStateEnum.Unknown
                || session.OrderExecutionProvider == null || session.OrderExecutionProvider.DefaultAccount== null)
            {
                return "[Invalid Account]";
            }
            else
            {
                return session.Info.Name + "," + session.OrderExecutionProvider.DefaultAccount.Info.Name + ", " + session.OrderExecutionProvider.DefaultAccount.Info.Server;
            }
        }

        /// <summary>
        /// Updates some parts of the UI, since others must be updated on given events only.
        /// </summary>
        public void UpdateUI()
        {
            this.Enabled = ((_selectedSession != null && _selectedSession.OrderExecutionProvider != null) || _sessionManager != null);

            if (this.Enabled == false)
            {
                return;
            }

            // Update SessionsArray UI combobox
            for (int i = 0; i < _sessions.Count + 1; i++)
            {
                if (toolStripComboBoxAccount.Items.Count <= i)
                {
                    toolStripComboBoxAccount.Items.Add("");
                }

                if (i == 0)
                {// All accountInfos go first.
                    if ((string)toolStripComboBoxAccount.Items[0] != "All")
                    {
                        toolStripComboBoxAccount.Items[0] = "All";
                    }
                }
                else
                {
                    string name = GetSessionName(_sessions[i-1]);
                    if ((string)toolStripComboBoxAccount.Items[i] != name)
                    {
                        toolStripComboBoxAccount.Items[i] = name;
                    }
                }

            }

            while (_sessions.Count + 1< toolStripComboBoxAccount.Items.Count)
            {
                toolStripComboBoxAccount.Items.RemoveAt(toolStripComboBoxAccount.Items.Count - 1);
            }

            if (toolStripComboBoxAccount.SelectedIndex < 0)
            {
                toolStripComboBoxAccount.SelectedIndex = 0;
            }

            // Update 
            this.listViewOrders.Visible = _compactMode == false;

            this.toolStripButtonCloseOrder.Visible = _compactMode == false;
            this.toolStripButtonModifyOrder.Visible = _compactMode == false;

            toolStripButtonDisplayMode.Visible = _allowCompactMode;
            toolStripSeparatorDisplayMode.Visible = _allowCompactMode;

            this.toolStripButtonNewOrder.Enabled = _selectedSession != null;
            //this.toolStripButtonObtainOrders.Enabled = true;

            this.toolStripButtonCloseOrder.Enabled = listViewOrders.SelectedItems.Count > 0;
            this.toolStripButtonModifyOrder.Enabled = listViewOrders.SelectedItems.Count > 0;

            if (_compactMode)
            {
                toolStripButtonDisplayMode.Text = "Full Mode";
            }
            else
            {
                toolStripButtonDisplayMode.Text = "Compact Mode";
            }

            if (_alwaysShowAccountSelectionTools)
            {
                toolStripLabelAccount.Visible = true;
                toolStripComboBoxAccount.Visible = true;
                toolStripSeparatorAccount.Visible = true;
            }
            else
            {
                // If only 1 account to manage, do not show
                toolStripLabelAccount.Visible = _sessions.Count > 1;
                toolStripComboBoxAccount.Visible = _sessions.Count > 1;
                toolStripSeparatorAccount.Visible = _sessions.Count > 1;
            }

            List<ListViewItem> items = new List<ListViewItem>();
            foreach (ListViewItem item in listViewOrders.Items)
            {
                items.Add(item);
            }

            foreach (ExpertSession session in _sessions)
            {
                if (IsSessionVisible(session))
                {
                    lock (session)
                    {
                        GeneralHelper.GenericDelegate<Order, AccountInfo> UpdateOrder = delegate(Order order, AccountInfo account)
                        {
                            if (IsOrderVisible(order) == false)
                            {
                                return;
                            }

                            bool orderFound = false;
                            foreach (ListViewItem item in items)
                            {
                                if (item.Tag == order)
                                {
                                    SetItemAsOrder(item, order, account);
                                    orderFound = true;
                                    items.Remove(item);
                                    break;
                                }
                            }

                            if (orderFound == false)
                            {
                                ListViewItem item = new ListViewItem();
                                listViewOrders.Items.Add(item);
                                SetItemAsOrder(item, order, account);
                            }
                        };

                        IOrderSink executor = session.OrderExecutionProvider;
                        ITradeEntityManagement ordersManagement = session.OrderExecutionProvider.TradeEntities;

                        if (ordersManagement != null && session.OrderExecutionProvider.DefaultAccount != null)
                        {
                            //lock (ordersManagement)
                            //{
                                foreach (Order order in ordersManagement.Orders)
                                {
                                    UpdateOrder(order, session.OrderExecutionProvider.DefaultAccount.Info);
                                }
                            //}
                        }
                    }
                }
            }

            foreach (ListViewItem item in items)
            {
                listViewOrders.Items.Remove(item);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        bool IsSessionVisible(ExpertSession session)
        {
            if (session.OrderExecutionProvider == null)
            {// Not order sessions always invisible.
                return false;
            }

            return _selectedSession == null || _selectedSession == session;
        }

        /// <summary>
        /// NON UI Thread.
        /// </summary>
        void Quote_QuoteUpdateEvent(IQuoteProvider provider)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        /// <summary>
        /// NON UI Thread.
        /// </summary>
        //protected void DoObtainAllOrders(bool showError)
        //{
            //foreach (ExpertSession session in _sessions.ToArray())
            //{
            //    if (session.OrderExecutionProvider == null || 
            //        session.OrderExecutionProvider.OperationalState != OperationalStateEnum.Operational)
            //    {
            //        continue;
            //    }

            //    string operationResultMessage;
            //    if (session.OrderExecutionProvider.DefaultAccount != null &&
            //        session.OrderExecutionProvider.DefaultAccount.SynchronizeOrders(null, out operationResultMessage) == false)
            //    {
            //        if (showError)
            //        {
            //            MessageBox.Show("Failed to obtain orders session [" + session.Info.Name + "] [" + operationResultMessage + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            SystemMonitor.OperationError("Failed to obtain orders session [" + session.Info.Name + "] [" + operationResultMessage + "].");
            //        }
            //    }
            //}

            //WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.DefaultDelegate( delegate() { toolStripButtonObtainOrders.Enabled = true; }));
        //}

        protected ListViewItem SetItemAsOrder(ListViewItem item, Order order, AccountInfo account)
        {
            if (item.Tag != order)
            {
                item.Tag = order;
            }

            if (item.UseItemStyleForSubItems != false)
            {
                item.UseItemStyleForSubItems = false;
            }

            OrderStateEnum orderState = order.State;
            string groupName = string.Format("{0} Orders, Server [{1}] Company [{2}] Account [{3} / {4}]", orderState.ToString().ToUpper(), account.Server, account.Company, account.Name, account.Id);

            ListViewGroup group = listViewOrders.Groups[groupName];
            if (group == null)
            {
                group = new ListViewGroup(groupName, groupName);

                if (orderState == OrderStateEnum.Executed)
                {// Insert group at top of list.
                    listViewOrders.Groups.Insert(0, group);
                }
                else if (orderState == OrderStateEnum.Submitted)
                {// ...
                    listViewOrders.Groups.Insert(0, group);
                }
                else
                {
                    listViewOrders.Groups.Add(group);
                }
            }

            if (item.Group != group)
            {
                item.Group = group;
            }

            if (order.IsBuy)
            {
                int index = order.State == OrderStateEnum.Executed ? 0 : 2;
                if (item.ImageIndex != index)
                {
                    item.ImageIndex = index;
                }
            }
            else
            {
                int index = order.State == OrderStateEnum.Executed ? 1 : 3;
                if (item.ImageIndex != index)
                {
                    item.ImageIndex = index;
                }
            }

            while(item.SubItems.Count < 12)
            {
                item.SubItems.Add("");
            }

            //if (order.Type == OrderTypeEnum.UNKNOWN)
            //{// Order type not established yet.
            //    item.SubItems[0].Text = "NA";
            //    return item;
            //}

            string id = order.Id;
            if (id == null)
            {
                id = string.Empty;
            }

            if (item.SubItems[0].Text != id)
            {
                item.SubItems[0].Text = id;
            }

            if (item.SubItems[1].Text != order.Symbol.Name)
            {
                item.SubItems[1].Text = order.Symbol.Name;
            }

            if (order.IsOpenOrPending)
            {
                if (item.SubItems[2].Text != (GeneralHelper.GetShortDateTime(order.OpenTime)))
                {
                    item.SubItems[2].Text = (GeneralHelper.GetShortDateTime(order.OpenTime));
                }
            }
            else
            {
                if (item.SubItems[2].Text != (GeneralHelper.GetShortDateTime(order.OpenTime) + " to " + GeneralHelper.GetShortDateTime(order.CloseTime)))
                {
                    item.SubItems[2].Text = (GeneralHelper.GetShortDateTime(order.OpenTime) + " to " + GeneralHelper.GetShortDateTime(order.CloseTime));
                }
            }

            if (item.SubItems[3].Text != (order.Type.ToString()))
            {
                item.SubItems[3].Text = (order.Type.ToString());
            }

            if (item.SubItems[4].Text != (GeneralHelper.ToString(order.CurrentVolume)))
            {
                item.SubItems[4].Text = GeneralHelper.ToString(order.CurrentVolume);
            }

            if (item.SubItems[5].Text != GeneralHelper.ToString(order.OpenPrice))
            {
                item.SubItems[5].Text = GeneralHelper.ToString(order.OpenPrice);
            }

            string closePriceValue = GeneralHelper.ToString(order.ClosePrice);
            if (item.SubItems[6].Text != closePriceValue)
            {
                item.SubItems[6].Text = closePriceValue;
            }

            if (item.SubItems[7].Text != GeneralHelper.ToString(order.StopLoss))
            {
                item.SubItems[7].Text = GeneralHelper.ToString(order.StopLoss);
            }

            if (item.SubItems[8].Text != GeneralHelper.ToString(order.TakeProfit))
            {
                item.SubItems[8].Text = GeneralHelper.ToString(order.TakeProfit);
            }

            if (item.SubItems[9].Text != ("-"))
            {
                item.SubItems[9].Text = ("-");
            }

            decimal? currentResultPips = order.GetResult(Order.ResultModeEnum.Pips);
            decimal? currentResultBaseCurrency = order.GetResult(Order.ResultModeEnum.Currency);

            string resultString;

            if (currentResultBaseCurrency.HasValue == false || 
                currentResultBaseCurrency.HasValue == false)
            {
                resultString = "NaN";
            }
            else
            {
                resultString = currentResultPips.Value.ToString("0.#") + "p / " + currentResultBaseCurrency.Value.ToString("0.#");
            }

            if (item.SubItems[10].Text != resultString)
            {
                item.SubItems[10].Text = resultString;
            }

            if (currentResultPips < 0)
            {
                if (item.SubItems[10].BackColor != Color.MistyRose)
                {
                    item.SubItems[10].BackColor = Color.MistyRose;
                }
            }
            else
            {
                if (item.SubItems[10].BackColor != Color.Transparent)
                {
                    item.SubItems[10].BackColor = Color.Transparent;
                }
            }

            if (item.SubItems[11].Text != "-")
            {
                item.SubItems[11].Text = ("-");
            }

            return item;
        }

        private void buttonAddOrder_Click(object sender, EventArgs e)
        {
            if (_selectedSession == null)
            {
                MessageBox.Show("Select a session to send order to.");
                return;
            }

            if (_selectedSession.DataProvider.OperationalState != OperationalStateEnum.Operational
                || _selectedSession.OrderExecutionProvider.OperationalState != OperationalStateEnum.Operational)
            {
                MessageBox.Show("Session data or order execution provider not operational.");
                return;
            }

            NewOrderControl control = new NewOrderControl(_selectedSession.DataProvider.Quotes, _selectedSession.Info, false, false);
            control.CreatePlaceOrderEvent += new NewOrderControl.CreatePlaceOrderDelegate(SubmitPositionOrder);

            HostingForm f = new HostingForm("New Order", control);
            f.MaximizeBox = false;
            f.FormBorderStyle = FormBorderStyle.FixedSingle;
            f.ShowDialog();
            control.CreatePlaceOrderEvent -= new NewOrderControl.CreatePlaceOrderDelegate(SubmitPositionOrder);

        }

        bool SubmitPositionOrder(OrderTypeEnum orderType, bool synchronous, int volume, decimal? allowedSlippage, decimal? desiredPrice,
            decimal? sourceTakeProfit, decimal? sourceStopLoss, bool allowExistingOrdersManipulation, string comment, out string operationResultMessage)
        {
            SystemMonitor.CheckThrow(_selectedSession != null, "Selected session must be not null to create orders.");

            Position position = _selectedSession.OrderExecutionProvider.TradeEntities.ObtainPositionBySymbol(_selectedSession.Info.Symbol);

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
                submitResult = position.ExecuteMarket(orderType, volume, desiredPrice, allowedSlippage,
                    sourceTakeProfit, sourceStopLoss, TimeSpan.FromSeconds(15), out info, out operationResultMessage);
            }

            if (string.IsNullOrEmpty(submitResult))
            {
                operationResultMessage = "Failed to initialize order [" +  operationResultMessage + "].";
                SystemMonitor.Error(operationResultMessage);
                return false;
            }

            return true;
        }

        private void listViewOrders_SizeChanged(object sender, EventArgs e)
        {
            //listViewOrders.Columns[listViewOrders.Columns.Count - 1].Width = -2;
        }

        private void buttonShowOrder_Click(object sender, EventArgs e)
        {
            List<Order> orders = GetSelectedOrders();
            if (orders.Count > 1)
            {
                MessageBox.Show("Select one order to modify.");
                return;
            }

            if (orders.Count == 0 || orders[0].IsOpenOrPending == false)
            {
                return;
            }

            PropertiesForm form = new PropertiesForm("Order", orders[0]);
            form.Show();
        }

        List<Order> GetSelectedOrders()
        {
            List<Order> result = new List<Order>();
            foreach (ListViewItem item in listViewOrders.SelectedItems)
            {

                result.Add((Order)item.Tag);
            }

            return result;
        }


        private void buttonModifyOrder_Click(object sender, EventArgs e)
        {
            List<Order> orders = GetSelectedOrders();
            if (orders.Count > 1 || orders.Count == 0)
            {
                MessageBox.Show("Select a single order to modify.");
                return;
            }

            Order selectedOrder = orders[0];

            if (selectedOrder.IsOpenOrPending == false
                || selectedOrder.OrderExecutionProvider == null)
                //|| selectedOrder.OrderExecutionProvider.DataProvider == null
                //|| selectedOrder.OrderExecutionProvider.DataProvider.Quotes == null)
            {
                return;
            }

            ModifyOrderControl control = new ModifyOrderControl(ModifyOrderControl.ModeEnum.Modify, selectedOrder);
            control.Visible = true;

            HostingForm form = new HostingForm("Modify order", control);
            form.ShowDialog();
        }

        private void buttonCloseOrder_Click(object sender, EventArgs e)
        {
            List<Order> orders = GetSelectedOrders();
            if (orders.Count > 1 || orders.Count == 0)
            {
                MessageBox.Show("Select a single order to close.");
                return;
            }

            if (orders[0] is ActiveOrder)
            {
                ActiveOrder order = (ActiveOrder)orders[0];

                if (order.IsOpenOrPending == false)
                {
                    return;
                }

                if (order.State == OrderStateEnum.Submitted)
                {
                    string message;
                    if (order.Cancel(out message))
                    {
                        MessageBox.Show("Order canceled.", "Success", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Order cancel failed [" + message + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    ModifyOrderControl control = new ModifyOrderControl(ModifyOrderControl.ModeEnum.Close, order);
                    HostingForm form = new HostingForm("Modify order", control);
                    form.ShowDialog();
                }
            }
            else if (orders[0] is PassiveOrder)
            {
                PassiveOrder order = (PassiveOrder)orders[0];

                if (order.State != OrderStateEnum.Submitted)
                {
                    MessageBox.Show("Passive orders can only be canceled.");
                    return;
                }
                
                string operationResultMessage = string.Empty;
                if (order.CloseOrCancel(null, null, out operationResultMessage) == false)
                {
                    MessageBox.Show("Failed to cancel order [" + operationResultMessage + "].", "Order Cancel Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }


        private void toolStripLabelOrders_Click(object sender, EventArgs e)
        {
            if (_compactMode)
            {// Full mode, restore.
                this.MaximumSize = new Size(0, 0);
                this.Height = _fullModeHeigth;
            }
            else
            {// Compact mode.
                _fullModeHeigth = this.Height;
                this.Height = toolStripButtonDisplayMode.Height + 2;
                this.MaximumSize = new Size(0, this.Height);
            }

            //listViewOrders.SendToBack();
            _compactMode = !_compactMode;

            UpdateUI();
        }

        private void listViewOrders_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listViewOrders.SelectedItems.Count > 0)
                {
                    Order order = listViewOrders.SelectedItems[0].Tag as Order;
                    closeToolStripMenuItem.Enabled = order.IsOpenOrPending;
                    modifyToolStripMenuItem.Enabled = order.IsOpenOrPending;

                    contextMenuStripOrders.Show(listViewOrders.PointToScreen(e.Location));
                }
            }
        }

        private void listViewOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_sessionManager == null)
            {
                return;
            }

            if (SelectedOrderChangedEvent != null)
            {
                if (listViewOrders.SelectedItems.Count == 0)
                {
                    SelectedOrderChangedEvent(null, null);
                }
                else
                {
                    DataSessionInfo sessionInfo = ((DataSessionInfo?)listViewOrders.SelectedItems[0].Group.Tag).Value;
                    ExpertSession session = null;
                    foreach (ExpertSession sessionItem in _sessions)
                    {
                        if (sessionItem.Info.Equals(sessionInfo))
                        {
                            session = sessionItem;
                            break;
                        }
                    }

                    SystemMonitor.CheckError(session != null, "Session for this session info not found.");

                    Order order = listViewOrders.SelectedItems[0].Tag as Order;

                    SystemMonitor.CheckError(session.OrderExecutionProvider == null || session.OrderExecutionProvider.TradeEntities.ContainsOrder(order), "Wrong session for selected order.");

                    SelectedOrderChangedEvent(session, order);
                }
            }
        }

        private void closeNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Order> orders = GetSelectedOrders();
            
            // Filter orders.
            for (int i = orders.Count - 1; i >= 0; i--)
            {
                if (orders[i].IsOpenOrPending == false || orders[i] is ActiveOrder == false)
                {
                    orders.RemoveAt(i);
                }
            }

            if (orders.Count == 0)
            {
                return;
            }

            if (orders.Count > 1)
            {
                if (MessageBox.Show("Closing [" + orders.Count + "] orders. Are you sure?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    return;
                }
            }

            bool hasFailure = false;
            string compositeMessage = "";
            foreach (ActiveOrder order in orders)
            {
                string message = "";
                if (order.Close(out message))
                {
                    compositeMessage += System.Environment.NewLine + "Order [" + order.Id + "] closed successfully at [" + order.ClosePrice + "]";
                }
                else
                {
                    hasFailure = true;
                    compositeMessage += System.Environment.NewLine + "Order [" + order.Id + "] failed to close, [" + message + "]";
                }
            }

            if (hasFailure)
            {
                MessageBox.Show(compositeMessage, "Operation encountered errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(compositeMessage, "Operation succeeded", MessageBoxButtons.OK);
            }
        }

        private void toolStripComboBoxAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_sessionManager == null)
            {
                return;
            }

            if (toolStripComboBoxAccount.SelectedIndex > 0)
            {// Set select orderInfo, -1 to compensate for the initial "All" field.
                _selectedSession = _sessionManager.SessionsArray[toolStripComboBoxAccount.SelectedIndex - 1];
            }
            else
            {// By default 0 position means all are selected.
                _selectedSession = null;
            }

            UpdateUI();

            if (SelectedSessionChangedEvent != null)
            {
                SelectedSessionChangedEvent(_selectedSession);
            }
        }

        private void toolStripDropDownButtonShow_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _orderFilterStates.Clear();
            if (e.ClickedItem.Tag == null)
            {
                if (toolStripDropDownButtonShow.DropDownItems.IndexOf(e.ClickedItem) == 2)
                {
                    _orderFilterStates.Add(OrderStateEnum.Executed);
                    _orderFilterStates.Add(OrderStateEnum.Submitted);
                }
                else if (toolStripDropDownButtonShow.DropDownItems.IndexOf(e.ClickedItem) == 4)
                {
                    _orderFilterStates.Add(OrderStateEnum.Executed);
                    _orderFilterStates.Add(OrderStateEnum.Closed);
                }
                else if (toolStripDropDownButtonShow.DropDownItems.IndexOf(e.ClickedItem) == 6)
                {
                    _orderFilterStates.Add(OrderStateEnum.Executed);
                    _orderFilterStates.Add(OrderStateEnum.Submitted);
                    _orderFilterStates.Add(OrderStateEnum.Closed);
                }
            }
            else if (e.ClickedItem.Tag is OrderStateEnum)
            {
                _orderFilterStates.Add((OrderStateEnum)e.ClickedItem.Tag);
            }
            UpdateUI();
        }

        private void toolStripDropDownButtonShow_DropDownOpening(object sender, EventArgs e)
        {
            foreach (ToolStripItem toolStripItem in toolStripDropDownButtonShow.DropDownItems)
            {
                if (toolStripItem is ToolStripMenuItem == false)
                {
                    continue;
                }

                ToolStripMenuItem item = (ToolStripMenuItem)toolStripItem;
                if (item.Tag != null && _orderFilterStates.Count == 1)
                {
                    item.Checked = _orderFilterStates.Contains((OrderStateEnum)item.Tag);
                }
                else
                {
                    item.Checked = false;

                    int index = toolStripDropDownButtonShow.DropDownItems.IndexOf(item);
                    if (_orderFilterStates.Count == 0 && index == 0)
                    {
                        item.Checked = true;
                    }
                    else if (_orderFilterStates.Count == 2 
                        && _orderFilterStates.Contains(OrderStateEnum.Executed)
                        && _orderFilterStates.Contains(OrderStateEnum.Submitted)
                        && index == 2)
                    {
                        item.Checked = true;
                    }
                    else if (_orderFilterStates.Count == 2 
                        && _orderFilterStates.Contains(OrderStateEnum.Executed)
                        && _orderFilterStates.Contains(OrderStateEnum.Closed)
                        && index == 4)
                    {
                        item.Checked = true;
                    }
                    else if (_orderFilterStates.Count == 3
                        && _orderFilterStates.Contains(OrderStateEnum.Executed)
                        && _orderFilterStates.Contains(OrderStateEnum.Submitted)
                        && _orderFilterStates.Contains(OrderStateEnum.Closed)
                        && index == 6)
                    {
                        item.Checked = true;
                    }
                }
            }


        }

        private void contextMenuStripOrders_Opening(object sender, CancelEventArgs e)
        {
            List<Order> orders = GetSelectedOrders();

            bool allAreOpen = true;
            bool allAreActive = true;
            bool allAreSubmitted = true;

            closeToolStripMenuItem.Text = "Close / Cancel";


            foreach (Order order in orders)
            {
                if (order is ActiveOrder == false)
                {
                    allAreActive = false;
                }

                if (order.State != OrderStateEnum.Submitted)
                {
                    allAreSubmitted = false;
                }

                if (order.IsOpenOrPending == false)
                {
                    allAreOpen = false;
                    break;
                }
            }

            if (allAreSubmitted)
            {
                closeNowToolStripMenuItemMenu.Enabled = false;
                closeToolStripMenuItem.Enabled = true;
                modifyToolStripMenuItem.Enabled = false;
                detailsToolStripMenuItem.Enabled = false;

                closeToolStripMenuItem.Text = "Cancel";

                return;
            }

            if (allAreOpen == false || allAreActive == false)
            {// Multiple operatios allowed only on opened orders.
                
                closeNowToolStripMenuItemMenu.Enabled = false;
                closeToolStripMenuItem.Enabled = false;
                modifyToolStripMenuItem.Enabled = allAreOpen;
                detailsToolStripMenuItem.Enabled = false;
                
                return;
            }

            closeNowToolStripMenuItemMenu.Enabled = true;
            closeToolStripMenuItem.Enabled = true;
            modifyToolStripMenuItem.Enabled = true;
            detailsToolStripMenuItem.Enabled = true;

            if (orders.Count == 0 || orders.Count > 1)
            {
                closeToolStripMenuItem.Enabled = false;
                modifyToolStripMenuItem.Enabled = false;
                detailsToolStripMenuItem.Enabled = false;
                return;
            }

            if (orders.Count == 1 && orders[0].State == OrderStateEnum.Submitted)
            {
                closeNowToolStripMenuItemMenu.Enabled = false;
                closeToolStripMenuItem.Text = "Cancel";
            }
            else
            {
                closeToolStripMenuItem.Text = "Close";
            }
        }

    }
}
