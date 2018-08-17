using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;
using ForexPlatform;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control provides general controls for visualizing the dataDelivery in an Expert Session.
    /// </summary>
    public partial class PlatformExpertSessionControl : CommonBaseControl
    {
        PlatformExpertSession _session;

        List<ToolStripItem> timeControlToolStripItems = new List<ToolStripItem>();

        OrdersControl _correspondingOrdersControl = null;
        /// <summary>
        /// Since the Expert Session control and the orders control can work in cooperation
        /// to show selected orders details, assign this to make them work together.
        /// </summary>
        public OrdersControl CorrespondingOrdersControl
        {
            get { return _correspondingOrdersControl; }
            set 
            {
                if (this.DesignMode)
                {
                    return;
                }

                if (_correspondingOrdersControl != null)
                {
                    _correspondingOrdersControl.SelectedOrderChangedEvent -= new OrdersControl.SelectedOrderChangedDelegate(ordersControl1_SelectedOrderChangedEvent);
                }

                _correspondingOrdersControl = value; 
            }
        }

        volatile IDataBarHistoryProvider _dataBars = null;
        /// <summary>
        /// 
        /// </summary>
        protected IDataBarHistoryProvider CurrentDataBarProvider
        {
            get { return _dataBars; }

            set 
            {
                if (_dataBars != null)
                {
                    _dataBars.Indicators.IndicatorAddedEvent -= new CommonFinancial.IndicatorManager.IndicatorUpdateDelegate(_session_IndicatorAddedEvent);
                    _dataBars.Indicators.IndicatorRemovedEvent -= new CommonFinancial.IndicatorManager.IndicatorUpdateDelegate(_session_IndicatorRemovedEvent);
                    _dataBars.Indicators.IndicatorUnInitializedEvent -= new CommonFinancial.IndicatorManager.IndicatorUpdateDelegate(_session_IndicatorUnInitializedEvent);
                    _dataBars.DataBarHistoryUpdateEvent -= new DataBarHistoryUpdateDelegate(DataBarHistory_DataBarHistoryUpdateEvent);
                    _dataBars = null;
                }

                _dataBars = value;

                if (_dataBars != null)
                {
                    _dataBars.Indicators.IndicatorAddedEvent += new CommonFinancial.IndicatorManager.IndicatorUpdateDelegate(_session_IndicatorAddedEvent);
                    _dataBars.Indicators.IndicatorRemovedEvent += new CommonFinancial.IndicatorManager.IndicatorUpdateDelegate(_session_IndicatorRemovedEvent);
                    // Capture uninit, since removal is not done on shutdown (due to serialization).
                    _dataBars.Indicators.IndicatorUnInitializedEvent += new CommonFinancial.IndicatorManager.IndicatorUpdateDelegate(_session_IndicatorUnInitializedEvent);
                    _dataBars.DataBarHistoryUpdateEvent += new DataBarHistoryUpdateDelegate(DataBarHistory_DataBarHistoryUpdateEvent);

                    foreach (PlatformIndicator indicator in _dataBars.Indicators.IndicatorsArray)
                    {
                        ChartPane indicatorPane = null;
                        foreach (ChartPane pane in chartControl.Panes)
                        {
                            if (pane.StateId == indicator.ChartSeries.ChartPaneStateId)
                            {
                                indicatorPane = pane;
                                break;
                            }
                        }

                        if (indicatorPane == null)
                        {
                            if (indicator.ScaledToQuotes.HasValue && indicator.ScaledToQuotes.Value)
                            {// Create in master pane.
                                indicatorPane = chartControl.MasterPane;
                                this.chartControl.MasterPane.Add(indicator.ChartSeries);
                            }
                            else
                            {// Create in slave pane.
                                indicatorPane = this.chartControl.CreateSlavePane(indicator.Name, SlaveChartPane.MasterPaneSynchronizationModeEnum.XAxis, 120);
                            }
                        }

                        control_AddIndicatorEvent(indicator, indicatorPane);
                    }

                    if (_dataBars.BarCount > 0)
                    {
                        WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.GenericDelegate<bool, bool>(chartControl.MasterPane.FitDrawingSpaceToScreen), true, true);
                        chartControl.Invalidate();
                    }
                }
            }
        }


        public PlatformExpertSession Session
        {
            get 
            { 
                return _session;
            }

            set 
            {
                if (value == _session && this.DesignMode)
                {
                    return;
                }

                if (_session != null)
                {
                    _session.ChartControlPersistence = new SerializationInfoEx();
                    SaveState();

                    if (_session.DataProvider != null)
                    {
                        if (_session.DataProvider.Quotes != null)
                        {
                            _session.DataProvider.Quotes.QuoteUpdateEvent -= new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
                        }

                        if (_session.DataProvider.DataTicks != null)
                        {
                            _session.DataProvider.DataTicks.DataTickHistoryUpdateEvent -= new DataTickHistoryUpdateDelegate(DataTickHistory_DataTickHistoryUpdateEvent);
                        }

                        _session.DataProvider.CurrentDataBarProviderChangedEvent -= new DataProviderUpdateDelegate(DataProvider_CurrentDataBarProviderChangedEvent);
                    }

                    if (_session.MainChartSeries != null)
                    {
                        _session.MainChartSeries.SelectedOrderChangedEvent -= new ProviderTradeChartSeries.SelectedOrderChangedDelegate(series_SelectedOrderChangedEvent);
                    }

                    _session.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(_session_OperationalStatusChangedEvent);
                }

                CurrentDataBarProvider = null;

                _session = value;
                this.chartControl.MasterPane.Clear(true, true);
                LoadTimeManagementToolStripItems(false);

                timeManagementToolStrip1.Controller = null;
                //timeManagementToolStrip1.Controlees.Clear();

                if (_correspondingOrdersControl != null)
                {
                    _correspondingOrdersControl.SingleSession = value;
                }

                if (_session != null)
                {
                    _session.MainChartSeries.SelectedOrderChangedEvent += new ProviderTradeChartSeries.SelectedOrderChangedDelegate(series_SelectedOrderChangedEvent);

                    //_mainSeries.LoadFromFile(_sessionInfo.SessionDataProvider, _sessionInfo.OrderExecutionProvider);
                    if (_session.ChartControlPersistence != null)
                    {
                        this.chartControl.RestoreState(_session.ChartControlPersistence);
                    }

                    //this.Invoke(new GeneralHelper.GenericDelegate<string>(chartControl.MainPane.SetChartName), "Loading dataDelivery from source...");
                    //this.Invoke(new GeneralHelper.DefaultDelegate(UpdateMasterChartName));
                    WinFormsHelper.BeginManagedInvoke(this, UpdateMasterChart);

                    this.chartControl.MasterPane.Add(_session.MainChartSeries, false, false);

                    _session.DataProvider.Quotes.RequestQuoteUpdate(false);

                    // Make sure this subscription is after series was initialized, as it also subscribes.
                    if (_session.DataProvider.Quotes != null)
                    {
                        _session.DataProvider.Quotes.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
                    }

                    if (_session.DataProvider.DataTicks != null)
                    {
                        _session.DataProvider.DataTicks.DataTickHistoryUpdateEvent += new DataTickHistoryUpdateDelegate(DataTickHistory_DataTickHistoryUpdateEvent);
                    }

                    _session.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_session_OperationalStatusChangedEvent);

                    CurrentDataBarProvider = _session.DataProvider.DataBars;

                    _session.DataProvider.CurrentDataBarProviderChangedEvent += new DataProviderUpdateDelegate(DataProvider_CurrentDataBarProviderChangedEvent);

                    if (_session.OrderExecutionProvider != null && _session.OrderExecutionProvider.TimeControl != null && _session.DataProvider.TimeControl != null)
                    {// We can apply time tradeEntities to this sessionInformation.
                        LoadTimeManagementToolStripItems(true);

                        // Data provider is leading the time control.
                        timeManagementToolStrip1.Controller = _session.DataProvider.TimeControl;

                        // ActiveOrder execution only gets notifications of what is going on.
                        //timeManagementToolStrip1.Controlees.AddElement(_sessionInfo.OrderExecutionProvider.TimeControl);
                    }

                }
                else
                {
                    WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateMasterChart);
                }
            }
        }

        public bool ShowChartControl
        {
            get { return this.chartControl.Visible; }
            set { this.chartControl.Visible = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformExpertSessionControl()
        {
            InitializeComponent();

            foreach (ToolStripItem item in timeManagementToolStrip1.Items)
            {
                timeControlToolStripItems.Add(item);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            UpdateMasterChart();
            timeManagementToolStrip1.Visible = false;
        }

        public override void SaveState()
        {
            if (_session != null)
            {
                if (_session.ChartControlPersistence == null)
                {
                    SystemMonitor.Warning("Session chart control persistence not assigned.");
                }
                else
                {
                    chartControl.SaveState(_session.ChartControlPersistence);
                }
            }
        }

        /// <summary>
        /// Loads / unloads the time tradeEntities toolstrip items on to the main toolstrip.
        /// </summary>
        /// <param name="load"></param>
        void LoadTimeManagementToolStripItems(bool load)
        {
            foreach (ToolStripItem item in timeControlToolStripItems)
            {
                if (load)
                {
                    toolStripMain.Items.Add(item);
                }
                else
                {
                    toolStripMain.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExpertSessionControl_Load(object sender, EventArgs e)
        {
            if (_correspondingOrdersControl != null)
            {
                _correspondingOrdersControl.SelectedOrderChangedEvent += new OrdersControl.SelectedOrderChangedDelegate(ordersControl1_SelectedOrderChangedEvent);
            }

            chartControl.MasterPane.KeyPress += new KeyPressEventHandler(MasterPane_KeyPress);
        }

        /// <summary>
        /// 
        /// </summary>
        void DataProvider_CurrentDataBarProviderChangedEvent(ISessionDataProvider dataProvider)
        {
            CurrentDataBarProvider = dataProvider.DataBars;
        }

        /// <summary>
        /// 
        /// </summary>
        void MasterPane_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UnInitializeControl()
        {
            if (_correspondingOrdersControl != null)
            {
                _correspondingOrdersControl.SelectedOrderChangedEvent -= new OrdersControl.SelectedOrderChangedDelegate(ordersControl1_SelectedOrderChangedEvent);
                _correspondingOrdersControl.UnInitializeControl();
                _correspondingOrdersControl = null;
            }

            Session = null;

            base.UnInitializeControl();
        }

        void _session_IndicatorAddedEvent(IndicatorManager indicators, Indicator indicator)
        {
        }

        void _session_IndicatorRemovedEvent(IndicatorManager indicators, Indicator indicator)
        {
            foreach (ChartPane pane in this.chartControl.Panes)
            {
                if (pane.Remove(((PlatformIndicator)indicator).ChartSeries))
                {// Found and removed.
                    if (pane.Series.Length == 0)
                    {
                        chartControl.RemoveSlavePane((SlaveChartPane)pane);
                    }
                    break;
                }
            }
        }

        void _session_IndicatorUnInitializedEvent(IndicatorManager indicators, Indicator indicator)
        {
            //PlatformIndicator platformIndicator = (PlatformIndicator)indicator;
            //platformIndicator.ChartSeries.Visible = false;
        }


        void series_SelectedOrderChangedEvent(Order previousSelectedOrder, Order newSelectedOrder)
        {
            if (_correspondingOrdersControl != null)
            {
                _correspondingOrdersControl.SelectOrder(newSelectedOrder);
            }
        }

        void ordersControl1_SelectedOrderChangedEvent(ExpertSession session, Order newSelectedOrder)
        {
            if (_session.MainChartSeries != null)
            {
                _session.MainChartSeries.SelectedOrder = newSelectedOrder;
                chartControl.MasterPane.Refresh();
            }
        }

        /// <summary>
        /// Non UI thread.
        /// </summary>
        void UpdateMasterChart()
        {
            this.chartControl.Visible = _session != null;
            toolStripButtonIndicators.Visible = _session != null;

            if (_session == null || _session.DataProvider == null)
            {
                return;
            }

            string name = _session.Info.Symbol.Name;

            if (CurrentDataBarProvider != null && CurrentDataBarProvider.Period.HasValue)
            {
                name += " [M" + CurrentDataBarProvider.Period.Value.TotalMinutes.ToString() + "]";
            }

            if (_session.DataProvider != null && _session.DataProvider.Quotes != null
                && _session.DataProvider.Quotes.OperationalState == OperationalStateEnum.Operational)
            {
                name += ", " + GeneralHelper.ToString(_session.DataProvider.Quotes.Bid, _session.Info.ValueFormat) + " / " + GeneralHelper.ToString(_session.DataProvider.Quotes.Ask, _session.Info.ValueFormat);
                name += ", " + _session.DataProvider.DataBars.BarCount.ToString() + " bars";
            }
            else
            {
                name = name + ", " + _session.OperationalState.ToString();
            }

            if (_session.OperationalState != OperationalStateEnum.Operational)
            {
                name = name + ", " + _session.OperationalState.ToString();
            }

            WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.GenericDelegate<string>(chartControl.MasterPane.SetChartName), name);
        }

        void _session_OperationalStatusChangedEvent(IOperational session, OperationalStateEnum previousState)
        {// Session has changed operation mode - show in the chart name.

            if (_session == null)
            {
                SystemMonitor.Warning("Control session already cleared.");
                return;
            }

            WinFormsHelper.BeginManagedInvoke(this, delegate()
            {
                if (session == _session)
                {
                    UpdateMasterChart();
                }
                else
                {
                    SystemMonitor.CheckError(_session == null || session == null, "Session mismatch.");
                }
            });

            // We can not afford delayed execution here, since on closing the application, persisting is needed
            // and by than Invoke requests are not handled any more. 
            if (previousState == OperationalStateEnum.Operational)
            {// All coming out of operation state must be persisted.
                // Save UI dataDelivery to sessionInformation object.
                if (_session.ChartControlPersistence != null)
                {
                    _session.ChartControlPersistence.Clear();
                }
                else
                {
                    _session.ChartControlPersistence = new SerializationInfoEx();
                }

                chartControl.SaveState(_session.ChartControlPersistence);
            }
        }

        void DataTickHistory_DataTickHistoryUpdateEvent(IDataTickHistoryProvider provider, DataTickUpdateType updateType)
        {
            if (updateType == DataTickUpdateType.Initial)
            {// Only executes on initial adding of many items.
                WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.GenericDelegate<bool, bool>(chartControl.MasterPane.FitDrawingSpaceToScreen), true, true);
            }
        }

        void DataBarHistory_DataBarHistoryUpdateEvent(IDataBarHistoryProvider provider, DataBarUpdateType updateType, int updatedBarsCount)
        {
            if (updateType == DataBarUpdateType.Initial)
            {// Only executes on initial adding of many items.
                WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.GenericDelegate<bool, bool>(chartControl.MasterPane.FitDrawingSpaceToScreen), true, true);
            }
        }
        
        void Quote_QuoteUpdateEvent(IQuoteProvider provider)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateMasterChart);
        }

        private void toolStripButtonIndicators_Click(object sender, EventArgs e)
        {
            if (Session == null)
            {
                return;
            }

            ExpertSessionIndicatorsControl control = new ExpertSessionIndicatorsControl(_session, chartControl.Panes);
            control.AddIndicatorEvent += new ExpertSessionIndicatorsControl.AddIndicatorDelegate(control_AddIndicatorEvent);
            control.RemoveIndicatorEvent += new ExpertSessionIndicatorsControl.RemoveIndicatorDelegate(control_RemoveIndicatorEvent);
            HostingForm form = new HostingForm("Session " + _session.Info.Name + " Indicators", control);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.ShowDialog();
        }

        void control_RemoveIndicatorEvent(PlatformIndicator indicator)
        {
        }

        void control_AddIndicatorEvent(PlatformIndicator indicator, ChartPane pane)
        {
            if (pane == null)
            {
                pane = chartControl.CreateSlavePane(indicator.Name, SlaveChartPane.MasterPaneSynchronizationModeEnum.XAxis, this.Height / 4);
                pane.RightMouseButtonSelectionMode = ChartPane.SelectionModeEnum.HorizontalZoom;
                pane.Add(indicator.ChartSeries);
                // Establish proper Y axis values.
                //pane.FitDrawingSpaceToScreen(true, true);
            }
            else
            {
                pane.Add(indicator.ChartSeries);
            }
        }

    }
}
