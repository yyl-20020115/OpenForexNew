using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;
using ForexPlatform;

namespace ForexPlatformFrontEnd
{
    public partial class ExpertSessionIndicatorsControl : UserControl
    {
        // Indicator whaiting to be initialized and used.
        PlatformIndicator _pendingIndicator;

        ExpertSession _session;

        IEnumerable<ChartPane> _chartPanes;

        public delegate void AddIndicatorDelegate(PlatformIndicator indicator, ChartPane pane);
        
        public delegate void RemoveIndicatorDelegate(PlatformIndicator indicator);

        /// <summary>
        /// This will be raised to notify owner the user selected to create the indicator in the given pane.
        /// pane will be null to specify a new pane needs to be created.
        /// </summary>
        public event AddIndicatorDelegate AddIndicatorEvent;

        public event RemoveIndicatorDelegate RemoveIndicatorEvent;
        
        /// <summary>
        /// 
        /// </summary>
        public ExpertSessionIndicatorsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public ExpertSessionIndicatorsControl(ExpertSession session, IEnumerable<ChartPane> chartPanes)
        {
            InitializeComponent();

            _session = session;
            _chartPanes = chartPanes;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            UnInitializeControl();
        }
        
        public void UnInitializeControl()
        {
            listViewIndicators.Clear();
            listViewIndicatorTypes.Clear();
            this.indicatorControl1.SelectedObject = null;
            _session = null;
        }

        private void ExpertSessionIndicatorsControl_Load(object sender, EventArgs e)
        {
            string[] groupsNames = Enum.GetNames(typeof(IndicatorFactory.IndicatorGroup));

            foreach (string groupName in groupsNames)
            {
                listViewIndicatorTypes.Groups.Add(groupName, groupName + " Group");

                IndicatorFactory.IndicatorGroup group = (IndicatorFactory.IndicatorGroup)Enum.Parse(typeof(IndicatorFactory.IndicatorGroup), groupName);
                string[] names = IndicatorFactory.Instance.GetIndicatorsNames(group);
                string[] descriptions = IndicatorFactory.Instance.GetIndicatorsDescriptions(group);

                for (int i = 0; i < names.Length; i++)
                {
                    ListViewItem item = new ListViewItem(names[i] + ", " + descriptions[i]);
                    item.Tag = names[i];
                    item.Group = listViewIndicatorTypes.Groups[groupName];
                    listViewIndicatorTypes.Items.Add(item);
                }
            }

            UpdateUI();
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            listViewIndicators.Items.Clear();
            foreach (Indicator indicator in _session.DataProvider.DataBars.Indicators.IndicatorsArray)
            {
                listViewIndicators.Items.Add(indicator.Name).Tag = indicator;
            }

            buttonNew.Enabled = _pendingIndicator != null;

            comboBoxChartAreas.Items.Clear();
            comboBoxChartAreas.Items.Add("New Chart Area");
            foreach (ChartPane pane in _chartPanes)
            {
                comboBoxChartAreas.Items.Add(pane.Name);
            }
         
            comboBoxChartAreas.SelectedIndex = 0;
            if (listViewIndicatorTypes.SelectedIndices.Count > 0)
            {
                bool? isScaledToQuotes = IndicatorFactory.Instance.GetIndicatorCloneByName(listViewIndicatorTypes.SelectedItems[0].Tag as string).ScaledToQuotes;
                if (isScaledToQuotes.HasValue && isScaledToQuotes.Value)
                {// Since indicator is scaled to quotes, default its pane selection to main pane.
                    comboBoxChartAreas.SelectedIndex = 0;
                }
            }

            if (_pendingIndicator != null && _pendingIndicator.ScaledToQuotes.HasValue && _pendingIndicator.ScaledToQuotes.Value)
            {// By default, scaled to quotes indicators are to be shown in main pane.
                comboBoxChartAreas.SelectedIndex = 1;
            }
        }

        private void listViewIndicators_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewIndicators.SelectedItems.Count > 0)
            {
                indicatorControl1.IsReadOnly = false;
                PlatformIndicator indicator = (PlatformIndicator)_session.DataProvider.DataBars.Indicators.IndicatorsArray[listViewIndicators.SelectedItems[0].Index];
                this.indicatorControl1.SelectedObject = indicator.ChartSeries;
            }
            else
            {
                indicatorControl1.SelectedObject = null;
            }
        }

        private void listViewIndicatorTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewIndicatorTypes.SelectedItems.Count > 0)
            {
                string name = listViewIndicatorTypes.SelectedItems[0].Tag as string;

                // Just create the indicator, no need to set it up, this will be handled by the session itself.
                _pendingIndicator = (PlatformIndicator)IndicatorFactory.Instance.GetIndicatorCloneByName(name);

                foreach (string setName in GeneralHelper.EnumerableToArray<string>(_pendingIndicator.ChartSeries.OutputResultSetsPens.Keys))
                {
                    _pendingIndicator.ChartSeries.OutputResultSetsPens[setName] = Pens.WhiteSmoke;
                }

                indicatorControl1.IsReadOnly = true;
                indicatorControl1.SelectedObject = _pendingIndicator.ChartSeries;
            }
            else
            {
                indicatorControl1.SelectedObject = null;
                _pendingIndicator = null;
            }

            UpdateUI();
        }

        ChartPane GetSelectedPane()
        {
            if (comboBoxChartAreas.SelectedIndex == 0)
            {// 0 position reserved for new pane.
                return null;
            }

            int requiredIndex = comboBoxChartAreas.SelectedIndex - 1;

            int currentPaneIndex = 0;
            foreach(ChartPane pane in _chartPanes)
            {
            //    if ((_pendingIndicator.IsScaledToQuotes.HasValue 
            //        && _pendingIndicator.IsScaledToQuotes.Value) || pane is SlaveChartPane)
            //    {
                    if (requiredIndex == currentPaneIndex)
                    {
                        return pane;
                    }
                    currentPaneIndex++;
            //    }
            }

            SystemMonitor.Throw("Unexpected, corresponding chart pane not found.");
            return null;
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            if (listViewIndicatorTypes.SelectedItems.Count == 0 || _pendingIndicator == null)
            {
                return;
            }

            // Get rid of the _pending indicator reference as soon as possible.
            PlatformIndicator pendingIndicator = _pendingIndicator;
            _pendingIndicator = null;

            _session.DataProvider.DataBars.Indicators.AddIndicator(pendingIndicator);

            if (AddIndicatorEvent != null)
            {
                AddIndicatorEvent(pendingIndicator, GetSelectedPane());
            }

            UpdateUI();

            //// Select the newly created indicator.
            //this.listViewIndicators.Items[listViewIndicators.Items.Count - 1].Selected = true;
            //this.listViewIndicators.Focus();

            //_pendingIndicator = null;
        
            // Invoke the creation of a new _pendingIndicator
            //listViewIndicatorTypes_SelectedIndexChanged(sender, e);
            
            // Select the newly created indicator.
            if (listViewIndicators.Items.Count > 0)
            {
                listViewIndicators.Items[listViewIndicators.Items.Count - 1].Selected = true;
            }

            listViewIndicators.Focus();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listViewIndicators.SelectedIndices.Count > 0)
            {
                PlatformIndicator indicator = (PlatformIndicator)listViewIndicators.SelectedItems[0].Tag;
                _session.DataProvider.DataBars.Indicators.RemoveIndicator(indicator);
                
                //RemoveIndicatorEvent(indicator);
            }

            UpdateUI();
        }

        private void listViewIndicatorTypes_DoubleClick(object sender, EventArgs e)
        {
            buttonNew_Click(sender, e);
        }

        private void ExpertSessionIndicatorsControl_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxChartAreas_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listViewIndicatorTypes_Enter(object sender, EventArgs e)
        {
            // This needed to display the prototype indicator in the properties pane again,
            // if we left and now came back but did not change the selected index.
            listViewIndicatorTypes_SelectedIndexChanged(sender, e);
        }

        
    }
}
