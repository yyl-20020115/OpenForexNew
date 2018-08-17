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

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control allows the selection of a trading baseCurrency from a source.
    /// </summary>
    public partial class SymbolSelectControl : UserControl
    {
        bool _immediateSelectionMode = false;

        public bool ImmediateSelectionMode
        {
            get { return _immediateSelectionMode; }
            set 
            { 
                _immediateSelectionMode = value;
                UpdateSelectionModeUI();
            }
        }

        TradePlatformComponent _host;
        /// <summary>
        /// 
        /// </summary>
        public TradePlatformComponent Host
        {
            get { return _host; }
            set 
            { 
                _host = value;
                UpdateSources();
            }
        }

        List<ComponentId> _dataSources = new List<ComponentId>();

        /// <summary>
        /// This is only available when we are using live mode.
        /// </summary>
        List<ComponentId> _orderExecutionSourcesIds = new List<ComponentId>();
        public ComponentId? SelectedDataSourceId
        {
            get
            {
                if (this.comboBoxDataProviderSources.SelectedIndex < 0)
                {
                    return null;
                }

                return _dataSources[comboBoxDataProviderSources.SelectedIndex];
            }
        }

        Dictionary<Symbol, TimeSpan[]> _symbols = new Dictionary<Symbol, TimeSpan[]>();

        /// <summary>
        /// BaseCurrency selected by user.
        /// </summary>
        public List<Symbol> SelectedSymbols
        {
            get
            {
                return GetSelectedSymbols();
            }
        }

        bool _initialSourceUpdate = false;

        /// <summary>
        /// Period for baseCurrency, may be null.
        /// </summary>
        public TimeSpan? SelectedPeriod
        {
            get
            {
                if (listViewPeriod.SelectedItems.Count > 0)
                {
                    return (TimeSpan)listViewPeriod.SelectedItems[0].Tag;
                }

                return null;
            }
        }

        public bool ShowSelectButton
        {
            get
            {
                return buttonSelect.Visible;
            }

            set
            {
                buttonSelect.Visible = value;
            }
        }

        /// <summary>
        /// Allow selection of multiple symbols at the same time, from the same source.
        /// </summary>
        public bool MultiSelect
        {
            get { return listViewSymbols.MultiSelect; }
            set { listViewSymbols.MultiSelect = value; }
        }

        public delegate void SelectedSymbolChangedDelegate(SymbolSelectControl control);
        public event SelectedSymbolChangedDelegate SelectedSymbolsChangedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SymbolSelectControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateSelectionModeUI();

            //_dataProviderSources.Clear();
            //if (comboBoxDataProviderSources.Items.Count > 0)
            //{
            //    comboBoxDataProviderSources.SelectedIndex = 0;
            //}
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
        void UpdateSources()
        {
            _initialSourceUpdate = true;

            _dataSources.Clear();
            comboBoxDataProviderSources.Items.Clear();

            if (_host != null)
            {
                // Update dataDelivery operationResult combo.
                List<ComponentId> ids = new List<ComponentId>();
                ids.AddRange(_host.GetSources(SourceTypeEnum.DataProvider, true));

                foreach (ComponentId id in ids)
                {// First add the high priority operationResult.
                    if ((_host.GetSourceTypeFlags(id, SourceTypeEnum.DataProvider) & SourceTypeEnum.HighPriority) != 0)
                    {
                        _dataSources.Add(id);
                        comboBoxDataProviderSources.Items.Add(id.Name);
                    }
                }

                foreach (ComponentId id in ids)
                {// Then the low priority ones.
                    if ((_host.GetSourceTypeFlags(id, SourceTypeEnum.DataProvider) & SourceTypeEnum.HighPriority) == 0)
                    {
                        _dataSources.Add(id);
                        comboBoxDataProviderSources.Items.Add(id.Name);
                    }
                }

                if (comboBoxDataProviderSources.Items.Count > 0)
                {
                    comboBoxDataProviderSources.SelectedIndex = 0;
                }
            }

            _initialSourceUpdate = false;
        }

        private void comboBoxDataProviderSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonNextSource.Enabled = comboBoxDataProviderSources.SelectedIndex < comboBoxDataProviderSources.Items.Count - 1;
            buttonPreviousSource.Enabled = comboBoxDataProviderSources.SelectedIndex > 0;

            textBoxSymbol.Text = string.Empty;
            buttonSearch_Click(sender, e);

            if (_initialSourceUpdate && _symbols.Count == 0)
            {// Try the next one since this source has no symbols right now, and we are on initial run.
                if (comboBoxDataProviderSources.SelectedIndex < comboBoxDataProviderSources.Items.Count - 1)
                {
                    comboBoxDataProviderSources.SelectedIndex++;
                }
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (Host != null && SelectedDataSourceId.HasValue)
            {
                _symbols = Host.SearchSymbols(SelectedDataSourceId.Value, textBoxSymbol.Text);
                UpdateSymbolsList();
            }
        }

        private void textBoxSymbolSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                buttonSearch_Click(sender, EventArgs.Empty);
            }
        }

        protected void UpdateSymbolsList()
        {
            listViewSymbols.Items.Clear();
            foreach (KeyValuePair<Symbol, TimeSpan[]> pair in _symbols)
            {
                ListViewItem item = new ListViewItem();
                item.Text = pair.Key.Group;
                item.SubItems.Add(pair.Key.Name);
                item.Tag = pair.Key;

                listViewSymbols.Items.Add(item);
            }

            if (listViewSymbols.Items.Count > 0)
            {
                listViewSymbols.SelectedIndices.Add(0);
            }
        }

        List<Symbol> GetSelectedSymbols()
        {
            List<Symbol> result = new List<Symbol>();

            if (listViewSymbols.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listViewSymbols.SelectedItems)
                {
                    result.Add((Symbol)(item.Tag));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(textBoxSymbol.Text.Trim()) == false)
                {
                    result.Add(new Symbol(textBoxSymbol.Text));
                }
            }
            return result;
        }


        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (comboBoxDataProviderSources.SelectedIndex < comboBoxDataProviderSources.Items.Count - 1)
            {
                comboBoxDataProviderSources.SelectedIndex++;
            }
        }

        private void buttonPreviousSource_Click(object sender, EventArgs e)
        {
            if (comboBoxDataProviderSources.SelectedIndex > 0)
            {
                comboBoxDataProviderSources.SelectedIndex--;
            }
        }

        //private void textBoxSymbol_Enter(object sender, EventArgs e)
        //{
        //    textBoxSymbol.SelectAll();
        //}

        //private void textBoxSymbol_MouseDown(object sender, MouseEventArgs e)
        //{
        //    textBoxSymbol.SelectAll();
        //}

        private void textBoxSymbol_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSearch_Click(sender, EventArgs.Empty);
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            if (SelectedSymbolsChangedEvent != null)
            {
                SelectedSymbolsChangedEvent(this);
            }
        }

        private void listViewSymbols_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonSelect_Click(sender, e);
        }

        private void textBoxSymbol_MouseDown(object sender, MouseEventArgs e)
        {
            listViewSymbols.SelectedItems.Clear();
        }

        private void listViewSymbols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSymbols.SelectedItems.Count > 0)
            {
                textBoxSymbol.Text = ((Symbol)listViewSymbols.SelectedItems[0].Tag).Name;
            }
            else
            {
                textBoxSymbol.Text = "";
            }

            UpdateTimePeriods();
            
            if (listViewSymbols.SelectedItems.Count > 0 && _immediateSelectionMode
                && SelectedPeriod.HasValue)
            {
                buttonSelect_Click(sender, e);
            }
        }

        void UpdateTimePeriods()
        {
            // Update time periods.
            listViewPeriod.Items.Clear();

            if (SelectedSymbols.Count == 1)
            {
                if (_symbols.ContainsKey(SelectedSymbols[0]))
                {
                    foreach (TimeSpan span in _symbols[SelectedSymbols[0]])
                    {
                        listViewPeriod.Items.Add(span.ToString()).Tag = span;
                    }
                }
            }
            else
            {
                listViewPeriod.Items.Clear();
            }

            if (listViewPeriod.Items.Count > 0)
            {
                // Select 1H if available by default.
                foreach (ListViewItem item in listViewPeriod.Items)
                {
                    if ((TimeSpan)item.Tag == TimeSpan.FromHours(1))
                    {
                        item.Selected = true;
                        break;
                    }
                }

                if (listViewPeriod.SelectedItems.Count == 0)
                {
                    listViewPeriod.SelectedIndices.Add(0);
                }
            }
        }


        void UpdateSelectionModeUI()
        {
            //SystemMonitor.CheckError(ImmediateSelectionMode == false || MultiSelect == false, "Immediate selection and multi selection mode may not be operating properly together.");

            buttonSelect.Visible = !ImmediateSelectionMode;
            if (buttonSelect.Visible == false)
            {
                listViewPeriod.Height = listViewSymbols.Height;
            }
            else
            {
                listViewPeriod.Height = listViewSymbols.Height - 29;
            }
        }
    }
}
