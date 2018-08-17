using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;
using ForexPlatform;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control designed to help creating expert sessions.
    /// </summary>
    public partial class CreateExpertSessionControl2 : UserControl
    {
        ExpertHost _host;
        public ExpertHost Host
        {
            get { return _host; }
            set 
            { 
                _host = value;
                symbolSelectControl.Host = _host;
            }
        }

        List<ComponentId> _orderExecutionSourcesIds = new List<ComponentId>();
        List<int> _orderExecutionSourcesCompatibilities = new List<int>();

        public bool AllowGraphicsSession
        {
            get { return radioButtonGraphicsOnly.Enabled; }
            set { radioButtonGraphicsOnly.Enabled = value; }
        }

        public bool AllowBackTestingSession
        {
            get { return radioButtonSimulationTrading.Enabled; }
            set { radioButtonSimulationTrading.Enabled = value; }
        }

        public bool AllowLiveSession
        {
            get { return radioButtonLiveTrading.Enabled; }
            set { radioButtonLiveTrading.Enabled = value; }
        }

        public delegate void SessionCreatedDelegate();
        public event SessionCreatedDelegate SessionCreatedEvent;

        /// <summary>
        /// 
        /// </summary>
        public CreateExpertSessionControl2()
        {
            InitializeComponent();
        }
        
        private void CreateExpertSessionControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            symbolSelectControl.MultiSelect = false;

            if (radioButtonLiveTrading.Enabled)
            {
                radioButtonLiveTrading.Checked = true;
            } 
            else if (radioButtonGraphicsOnly.Enabled)
            {
                radioButtonGraphicsOnly.Checked = true;
            }
            else if (radioButtonSimulationTrading.Enabled)
            {
                radioButtonSimulationTrading.Checked = true;
            }

            radioButtonType_CheckedChanged(sender, e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (_host != null)
            {
                _host = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataProviderSession"></param>
        /// <returns>Sorted compatible operationResult, by compatibility.</returns>
        SortedDictionary<int, List<ComponentId>> GetCompatibleOrderExecutionSources()
        {
            SortedDictionary<int, List<ComponentId>> result = new SortedDictionary<int,List<ComponentId>>();

            if (symbolSelectControl.SelectedDataSourceId.HasValue == false
                || symbolSelectControl.SelectedSymbols.Count != 1)
            {
                return result;
            }

            ComponentId dataSourceId = symbolSelectControl.SelectedDataSourceId.Value;
            Symbol symbol = symbolSelectControl.SelectedSymbols[0];

            if (radioButtonGraphicsOnly.Checked)
            {
                return result;
            }

            if (radioButtonSimulationTrading.Checked || radioButtonLiveTrading.Checked)
            {// WARNING, combined IF - CASE, do not refactor lightly!!
                result = _host.GetCompatibleOrderExecutionSources(dataSourceId, symbol, SourceTypeEnum.OrderExecution | SourceTypeEnum.BackTesting);
            }
            
            if (radioButtonLiveTrading.Checked)
            {// WARNING, combined IF - CASE, do not refactor lightly!!
                SortedDictionary<int, List<ComponentId>> tmpResult = _host.GetCompatibleOrderExecutionSources(dataSourceId, symbol, SourceTypeEnum.OrderExecution | SourceTypeEnum.Live);

                foreach (int key in tmpResult.Keys)
                {
                    if (result.ContainsKey(key) == false)
                    {
                        result.Add(key, new List<ComponentId>());
                    }

                    result[key].AddRange(tmpResult[key]);
                }
            }

            //foreach (ComponentId id in candidatesSources)
            //{
            //    int compatibilityLevel = 0;
            //    bool compatible = false;

            //    SourceTypeEnum? type = _host.GetSourceTypeFlags(id, SourceTypeEnum.OrderExecution);
            //    if (type.HasValue == false)
            //    {
            //        continue;
            //    }
                
            //    type = type & SourceTypeEnum.BackTesting;
            //    if (type == SourceTypeEnum.BackTesting)
            //    {
            //        compatibilityLevel = 1;
            //        compatible = true;
            //    }
            //    else
            //    {
            //        compatible = _host.IsDataSourceSymbolCompatible(id, symbolSelectControl.SelectedDataSourceId.Value, 
            //            symbolSelectControl.SelectedSymbols[0], out compatibilityLevel);
            //    }

            //    // If componentId has not matched any continue condition, remote it from list.
            //    if (compatible)
            //    {
            //        if (result.ContainsKey(compatibilityLevel) == false)
            //        {
            //            result[compatibilityLevel] = new List<ComponentId>();
            //        }
            //        result[compatibilityLevel].Register(id);
            //    }
            //}

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxOrderExecutionSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            //buttonCreateSession.Enabled =
            //    (virtualListViewExSessions.SelectedIndices.Count > 0
            //    && (comboBoxOrderExecutionSources.Visible == false || comboBoxOrderExecutionSources.SelectedIndex >= 0)
            //    && comboBoxDataProviderSources.SelectedIndex >= 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreateSession_Click(object sender, EventArgs e)
        {
            if (symbolSelectControl.SelectedSymbols.Count == 0)
            {
                MessageBox.Show("No symbol selected.");
                return;
            }

            ComponentId? orderExecuteId = null;
            int compatibility = 0;
            if ((radioButtonSimulationTrading.Checked || radioButtonLiveTrading.Checked)
                && comboBoxOrderExecutionSources.SelectedIndex < _orderExecutionSourcesIds.Count)
            {
                orderExecuteId = _orderExecutionSourcesIds[comboBoxOrderExecutionSources.SelectedIndex];
                compatibility = _orderExecutionSourcesCompatibilities[comboBoxOrderExecutionSources.SelectedIndex];
            }

            string operationResultMessage;

            DataSessionInfo? sessionInfo = Host.GetSymbolDataSessionInfo(symbolSelectControl.SelectedDataSourceId.Value, 
                symbolSelectControl.SelectedSymbols[0]);

            if (sessionInfo.HasValue == false)
            {
                MessageBox.Show("Failed to initiate session for this symbol.");
                return;
            }
            
            if (_host.GetExpertSession(sessionInfo.Value) != null)
            {
                MessageBox.Show("Session for this symbol/source already exists.");
                return;
            }

            if (compatibility < 100 && radioButtonLiveTrading.Checked)
            {
                if (MessageBox.Show("You are creating a live session with order execution provider [" + orderExecuteId.Value.Name + "], that has low compatilibity level with selected data source. It is likely this is not the default order execution provider for this data source." + Environment.NewLine + Environment.NewLine + "Do you wish to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
            }

            PlatformExpertSession session = _host.CreateExpertSession(sessionInfo.Value, symbolSelectControl.SelectedDataSourceId.Value,
                orderExecuteId, radioButtonSimulationTrading.Checked, out operationResultMessage);

            if (session == null || _host.RegisterExpertSession(session) == false)
            {
                MessageBox.Show("Failed to create session [" + operationResultMessage + "].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // TODO: add support for tick dataDelivery.
                TimeSpan period = TimeSpan.FromHours(1);

                if (symbolSelectControl.SelectedPeriod.HasValue)
                {
                    period = symbolSelectControl.SelectedPeriod.Value;
                }
                else 
                {
                    if (GeneralHelper.EnumerableContains<TimeSpan>(session.DataProvider.AvailableDataBarProviderPeriods, period) == false)
                    {// We can not use 1hour because it is not available.
                        if (session.DataProvider.AvailableDataBarProviderPeriods.Length > 0)
                        {
                            period = session.DataProvider.AvailableDataBarProviderPeriods[0];
                        }
                        else
                        {
                            SystemMonitor.OperationWarning("Failed to establish period for session, going for default (1 hour) although not marked as available.");
                        }
                    }
                }

                if (session.DataProvider.ObtainDataBarProvider(period) == false)
                {
                    SystemMonitor.OperationError("Failed to obtain data bar provide for period [" + period.ToString() + "].");
                }

                if (SessionCreatedEvent != null)
                {
                    SessionCreatedEvent();
                }
            }

            this.Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonType_CheckedChanged(object sender, EventArgs e)
        {
            //_dataProviderSources.Clear();
            //comboBoxDataProviderSources.Items.Clear();

            // Update dataDelivery operationResult combo.
            List<ComponentId> ids = new List<ComponentId>();
            if (radioButtonGraphicsOnly.Checked)
            {
                ids.AddRange(_host.GetSources(SourceTypeEnum.DataProvider, true));
            }
            else if (radioButtonLiveTrading.Checked)
            {
                ids.AddRange(_host.GetSources(SourceTypeEnum.DataProvider | SourceTypeEnum.Live, false));
            }
            else if (radioButtonSimulationTrading.Checked)
            {
                ids.AddRange(_host.GetSources(SourceTypeEnum.DataProvider, true));
            }

            comboBoxOrderExecutionSources.Visible = !radioButtonGraphicsOnly.Checked;
            labelExecutionSource.Visible = comboBoxOrderExecutionSources.Visible;

            //if (virtualListViewExSessions.SelectedIndices.Count > 0)
            //{
            //    int value = virtualListViewExSessions.SelectedIndices[0];
            //    virtualListViewExSessions.SelectedIndices.Clear();
            //    virtualListViewExSessions.SelectedIndices.AddElement(value);
            //    //listViewSourceSessions.Items[value].Selected = true;
            //}

            if (radioButtonGraphicsOnly.Checked)
            {
                textBoxSessionDescription.Text = "This type of session allows viewing and analyzing exiting data. No orders are allowed.";
            }
            else if (radioButtonSimulationTrading.Checked)
            {
                textBoxSessionDescription.Text = "This type of session allows performing a trading simulation on existing (historical) data. The orders placed are simulationary and are kept inside this platform only.";
            }
            else if (radioButtonLiveTrading.Checked)
            {
                textBoxSessionDescription.Text = "This type of session allows performing live money, paper trading or live simulation trading. It receives real time data and sends the orders to be executed by the execution provider source selected.";
            }

            symbolSelectControl_SelectedSymbolChangedEvent(symbolSelectControl);
        }

        private void symbolSelectControl_SelectedSymbolChangedEvent(SymbolSelectControl control)
        {
            _orderExecutionSourcesIds.Clear();
            _orderExecutionSourcesCompatibilities.Clear();
            comboBoxOrderExecutionSources.Items.Clear();

            comboBoxOrderExecutionSources.Enabled = symbolSelectControl.SelectedDataSourceId.HasValue;
            
            if (comboBoxOrderExecutionSources.Enabled)
            {
                SortedDictionary<int, List<ComponentId>> compatibleSources = GetCompatibleOrderExecutionSources();

                foreach (int value in compatibleSources.Keys)
                {
                    if (value > 0)
                    {
                        foreach (ComponentId id in compatibleSources[value])
                        {
                            _orderExecutionSourcesIds.Insert(0, id);
                            _orderExecutionSourcesCompatibilities.Insert(0, value);

                            if (_host.Platform.Settings.DeveloperMode)
                            {// Print some more orderInfo for developers.
                                comboBoxOrderExecutionSources.Items.Insert(0, id.Name + " (Compatibility: " + value.ToString() + ")");
                            }
                            else
                            {
                                comboBoxOrderExecutionSources.Items.Insert(0, id.Name);
                            }

                            if (comboBoxOrderExecutionSources.Items.Count > 0)
                            {
                                comboBoxOrderExecutionSources.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }

            buttonCreateSession.Enabled = symbolSelectControl.SelectedSymbols.Count == 1
                && (comboBoxOrderExecutionSources.Visible == false || comboBoxOrderExecutionSources.SelectedIndex >= 0)
                && symbolSelectControl.SelectedDataSourceId.HasValue;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
