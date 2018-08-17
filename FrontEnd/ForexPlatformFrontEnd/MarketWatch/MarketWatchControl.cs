using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
//using Arbiter;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// The ActualExpertHostControl is the corresponding UI control to the ActualExpertHost class. 
    /// It provides a way for the expert host to appear as an UI element.
    /// </summary>
    public partial class MarketWatchControl : PlatformComponentControl
    {
        new MarketWatchComponent Component
        {
            get { return base.Component as MarketWatchComponent; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MarketWatchControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public MarketWatchControl(MarketWatchComponent component)
            : base(component)
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Component != null)
            {
                Component.QuotesUpdateEvent += new MarketWatchComponent.QuotesUpdateDelegate(component_QuotesUpdateEvent);
            }
        }

        void component_QuotesUpdateEvent(MarketWatchComponent component, MarketWatchComponent.SymbolInformation information)
        {
            //WinFormsHelper.BeginFilteredManagedInvoke(this, DoUpdateUI);
        }

        void UpdateUI()
        {
            if (Component == null)
            {
                return;
            }

            lock (Component)
            {
                int index = 0;
                foreach (MarketWatchComponent.SymbolInformation information in Component.SymbolsUnsafe)
                {
                    if (listViewQuotes.Items.Count < index + 1)
                    {
                        ListViewItem item = new ListViewItem("-");
                        item.SubItems.Add("-");
                        item.SubItems.Add("-");
                        item.SubItems.Add("-");
                        item.SubItems.Add("-");
                        item.SubItems.Add("-");
                        listViewQuotes.Items.Add(item);
                    }

                    SetItemAsQuote(listViewQuotes.Items[index], information);
                    index++;
                }

                for (int i = listViewQuotes.Items.Count - 1; i >= 0; i--)
                {
                    if (i > index)
                    {
                        listViewQuotes.Items.RemoveAt(i);
                    }
                }
            }

        }

        void SetItemAsQuote(ListViewItem item, MarketWatchComponent.SymbolInformation information)
        {
            if (item.Text != information.Symbol.Name)
            {
                item.Text = information.Symbol.Name;
            }

            item.UseItemStyleForSubItems = false;
            item.Tag = information.Symbol;

            Color subItemColor = listViewQuotes.BackColor;

            if (information.IsUpDownTimedOut == false)
            {
                subItemColor = information.IsUpFromPrevious ? Color.YellowGreen : Color.DarkSalmon;
            }

            if (item.SubItems[1].BackColor != subItemColor)
            {
                item.SubItems[1].BackColor = subItemColor;
                item.SubItems[2].BackColor = subItemColor;
            }

            string subItemText1 = "-";
            string subItemText2 = "-";

            if (information.Quote.HasValue)
            {
                subItemText1 = information.Quote.Value.Ask.ToString();
                subItemText2 = information.Quote.Value.Bid.ToString();

                item.SubItems[3].Text = GeneralHelper.ToString(information.Quote.Value.Spread);
                item.SubItems[4].Text = GeneralHelper.ToString(information.Quote.Value.High);
                item.SubItems[5].Text = GeneralHelper.ToString(information.Quote.Value.Low);
            }

            if (item.SubItems[1].Text != subItemText1)
            {
                item.SubItems[1].Text = subItemText1;
            }

            if (item.SubItems[2].Text != subItemText2)
            {
                item.SubItems[2].Text = subItemText2;
            }

        }


        public override void UnInitializeControl()
        {
            base.UnInitializeControl();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewQuotes.SelectedItems)
            {
                Component.RemoveSymbol((Symbol)item.Tag);
                listViewQuotes.Items.Remove(item);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SymbolSelectControl control = new SymbolSelectControl();
            HostingForm form = new HostingForm("Select symbol", control);
            control.ShowSelectButton = true;
            control.Host = Component;
            control.SelectedSymbolsChangedEvent += new SymbolSelectControl.SelectedSymbolChangedDelegate(control_SelectedSymbolsChangedEvent);

            form.Show();
        }

        void control_SelectedSymbolsChangedEvent(SymbolSelectControl control)
        {
            Dictionary<Symbol, string> failedSymbols = new Dictionary<Symbol, string>();

            if (control.SelectedSymbols.Count > 0 && control.SelectedDataSourceId.HasValue)
            {
                foreach (Symbol symbol in control.SelectedSymbols)
                {
                    string operationResultMessage;
                    if (Component.AddSymbol(control.SelectedDataSourceId.Value, symbol, out operationResultMessage) == false)
                    {
                        failedSymbols.Add(symbol, operationResultMessage);
                    }
                }
            }

            if (failedSymbols.Count > 0)
            {
                string message = "Failed to add symbols: " + Environment.NewLine;
                foreach (Symbol symbol in failedSymbols.Keys)
                {
                    message += symbol.Name + " [" + failedSymbols[symbol] + "]" + Environment.NewLine;
                }

                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listViewQuotes_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void timerUI_Tick(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void tradeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        void CreateSessionPane(bool trading)
        {
            if (listViewQuotes.SelectedItems.Count < 1 || Component.Delivery == null || Component.DataSourceId.HasValue == false)
            {
                return;
            }

            ListViewItem item = listViewQuotes.SelectedItems[0];
            LocalExpertHost host = new LocalExpertHost(item.Text, typeof(ManualTradeExpert));
            Component.Platform.RegisterComponent(host);

            RuntimeDataSessionInformation info = Component.Delivery.GetSymbolRuntimeSessionInformation((Symbol)item.Tag);

            if (info == null)
            {
                SystemMonitor.OperationError("Failed to obtain symbol session, operation can not be performed.");
                MessageBox.Show("Failed to obtain symbol session, operation can not be performed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string operationResultMessage;

            ComponentId? tradeSourceId = null;
            if (trading)
            {
                SortedDictionary<int, List<ComponentId>> compatibleExecutioners = 
                    Component.GetCompatibleOrderExecutionSources(Component.DataSourceId.Value, info.Info.Symbol, SourceTypeEnum.Live | SourceTypeEnum.OrderExecution);

                if (compatibleExecutioners.Count == 0 ||
                        compatibleExecutioners[GeneralHelper.EnumerableFirstThrows<int>(compatibleExecutioners.Keys)].Count == 0)
                {
                    MessageBox.Show("Failed to find order execution source for this symbol. Trading can not be initiated.");
                    return;
                }

                tradeSourceId = compatibleExecutioners[GeneralHelper.EnumerableFirstThrows<int>(compatibleExecutioners.Keys)][0];
            }

            PlatformExpertSession session = host.CreateExpertSession(info.Info, Component.DataSourceId.Value,
                tradeSourceId, false, out operationResultMessage);

            if (session == null)
            {
                SystemMonitor.OperationError(operationResultMessage);
                MessageBox.Show(operationResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (host.RegisterExpertSession(session))
            {
                // Try to run a 1hour chart, or if not, any chart available.
                bool oneHourFound = false;
                foreach (TimeSpan span in session.DataProvider.AvailableDataBarProviderPeriods)
                {
                    if (span == TimeSpan.FromHours(1))
                    {
                        session.DataProvider.ObtainDataBarProvider(TimeSpan.FromHours(1));
                        oneHourFound = true;
                        break;
                    }
                }

                if (oneHourFound == false && session.DataProvider.AvailableDataBarProviderPeriods.Length > 0)
                {
                    session.DataProvider.ObtainDataBarProvider(session.DataProvider.AvailableDataBarProviderPeriods[0]);
                }

                // Allow the pending control requests to be created and performed before floating it.
                this.BeginInvoke(new GeneralHelper.GenericDelegate<LocalExpertHost>(SetControlFloat), host);
            }
            else
            {
                SystemMonitor.OperationError("Failed to register session.");
                MessageBox.Show("Failed to register session.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chartTradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateSessionPane(true);
        }

        private void viewChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateSessionPane(false);
        }

        void SetControlFloat(LocalExpertHost host)
        {
            CommonBaseControl control = MasterForm.combinedContainerControl.GetControlByTag(host);
            if (control != null)
            {
                MasterForm.combinedContainerControl.SetControlFloating(control);
            }
            else
            {
                SystemMonitor.Error("Failed to find host control to float.");
            }
        }

    }
}
