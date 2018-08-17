using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;
using ForexPlatform;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// A managed expert control allows the exposure of an expert.
    /// The default operation is on one sessionInformation only.
    /// </summary>
    public partial class ManagedExpertControl : CommonBaseControl
    {
        Expert _expert;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ManagedExpertControl(PlatformManagedExpert expert)
        {
            InitializeComponent();
            tracerControl.Tracer = expert.Tracer;

            SetExpert(expert);
        }

        /// <summary>
        /// 
        /// </summary>
        public ManagedExpertControl(ManualTradeExpert expert)
        {
            InitializeComponent();
            
            tracerControl.Visible = true;
            //toolStripButtonTrace.Visible = false;
            splitter1.Visible = false;

            SetExpert(expert);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            toolStripAdditional.Visible = false;

            ordersControlExpertSession.AllowOrderManagement = true;

            // Move all the items from the additional toolstrip to the one on the expertSessionControl.
            List<ToolStripItem> additionalItems = new List<ToolStripItem>();
            foreach (ToolStripItem item in toolStripAdditional.Items)
            {
                additionalItems.Add(item);
            }

            for (int i = additionalItems.Count - 1; i >= 0; i--)
            {
                expertSessionControl.toolStripMain.Items.Insert(0, additionalItems[i]);
            }

            // SystemMonitor.CheckError(this.Parent is ExpertHostControl);
            
            // Match orders control and chart orders.
            expertSessionControl.CorrespondingOrdersControl = ordersControlExpertSession;
        }

        public override void UnInitializeControl()
        {
            if (_expert != null && _expert.Manager != null && _expert.Manager.SessionsArray.Length > 0
                && _expert.Manager.SessionsArray[0].OrderExecutionProvider != null)
            {
                _expert.Manager.SessionsArray[0].OrderExecutionProvider.AccountInfoUpdateEvent -= new AccountInfoUpdateDelegate(OrderExecutionProvider_AccountInfoUpdateEvent);
            }

            tracerControl.Tracer = null;
            SetExpert(null);

            base.UnInitializeControl();
        }

        void SetExpert(Expert expert)
        {
            SystemMonitor.CheckError(expert == null || _expert == null || expert == _expert, "Change expert mode not supported yet. Use one control per expert.");
            _expert = expert;
            
            if (_expert == null)
            {
                expertSessionControl.Session = null;
                accountControl1.Account = null;
                expertSessionControl.CorrespondingOrdersControl = null;
                return;
            }

            toolStripButtonInitialize.Enabled = _expert.Manager != null && _expert.Manager.SessionCount == 0;
            expertSessionControl.CorrespondingOrdersControl = ordersControlExpertSession;

            if (_expert.Manager != null)
            {
                int height = 0;
                if (((ExpertHost)_expert.Manager).UISerializationInfo.TryGetValue<int>("tabControl1.Height", ref height))
                {
                    tabControl1.Height = height;
                }
            }

            if (_expert.Manager != null && _expert.Manager.SessionCount > 0)
            {
                expertSessionControl.Session = (PlatformExpertSession)_expert.Manager.SessionsArray[0];
                //toolStripButtonOrders.Checked = expertSessionControl.Session.OrderExecutionProvider != null;

                positionsControl1.Initialize(_expert.Manager, _expert.Manager.GetDataDelivery(_expert.Manager.SessionsArray[0].DataProvider.SourceId), _expert.Manager.SessionsArray[0].OrderExecutionProvider);

                if (_expert.Manager.SessionsArray[0].OrderExecutionProvider != null)
                {
                    _expert.Manager.SessionsArray[0].OrderExecutionProvider.AccountInfoUpdateEvent += new AccountInfoUpdateDelegate(OrderExecutionProvider_AccountInfoUpdateEvent);
                    if (_expert.Manager.SessionsArray[0].OrderExecutionProvider.DefaultAccount != null)
                    {
                        accountControl1.Account = _expert.Manager.SessionsArray[0].OrderExecutionProvider.DefaultAccount;
                    }
                }

                tabControl1.Visible = _expert.Manager.SessionsArray[0].OrderExecutionProvider != null;
                accountControl1.Visible = tabControl1.Visible;
                if (tabControl1.Visible == false)
                {
                    tabControl1.SelectedTab = tabControl1.TabPages[2];
                }
            }
            else
            {
                //toolStripButtonOrders.Checked = false;
                expertSessionControl.Session = null;
            }

        }

        void OrderExecutionProvider_AccountInfoUpdateEvent(IOrderSink provider, AccountInfo accountInfo)
        {
            if (accountControl1.Account == null)
            {
                accountControl1.Account = _expert.Manager.SessionsArray[0].OrderExecutionProvider.DefaultAccount;
            }
        }

        private void toolStripButtonInitialize_Click(object sender, EventArgs e)
        {
            Initialize();
        }

        public void Initialize()
        {
            CreateExpertSessionForm form = new CreateExpertSessionForm((LocalExpertHost)_expert.Manager);
            if (form.ShowDialog() == DialogResult.OK && _expert.Manager.SessionCount > 0)
            {
                toolStripButtonInitialize.Enabled = false;
                SetExpert(_expert);

                if (_expert.Manager.SessionCount > 0)
                {
                    bool showOrders = _expert.Manager.SessionsArray[0].OrderExecutionProvider != null;
                    splitter1.Visible = showOrders;
                    splitter2.Visible = showOrders;
                    tabControl1.Visible = showOrders;
                }
            }
        }

        private void toolStripButtonProperties_Click(object sender, EventArgs e)
        {
            PropertiesForm form = new PropertiesForm("Expert Properties", _expert);
            form.ShowDialog();
        }

        private void toolStripButtonOrders_CheckStateChanged(object sender, EventArgs e)
        {
            //ordersControlExpertSession.Visible = toolStripButtonOrders.Checked;
            //this.accountControl1.Visible = ordersControlExpertSession.Visible;
            //splitter2.Visible = toolStripButtonOrders.Checked;
        }

        private void toolStripButtonTrace_CheckStateChanged(object sender, EventArgs e)
        {
            //tracerControl.Visible = toolStripButtonTrace.Checked;
            //this.splitter1.Visible = toolStripButtonTrace.Checked;
        }

        private void toolStripButtonChart_CheckStateChanged(object sender, EventArgs e)
        {
            expertSessionControl.ShowChartControl = toolStripButtonChart.Checked;
        }

        public override void SaveState()
        {
            base.SaveState();
            expertSessionControl.SaveState();
        }

        private void tabControl1_SizeChanged(object sender, EventArgs e)
        {
            if (_expert != null && _expert.Manager != null)
            {
                ((ExpertHost)_expert.Manager).UISerializationInfo.AddValue("tabControl1.Height", tabControl1.Height);
            }
        }

        private void tabControl1_VisibleChanged(object sender, EventArgs e)
        {
            splitter1.Visible = tabControl1.Visible;
        }

        private void accountControl1_VisibleChanged(object sender, EventArgs e)
        {
            splitter2.Visible = accountControl1.Visible;
        }

        private void ManagedExpertControl_SizeChanged(object sender, EventArgs e)
        {
            tabControl1.Height = this.Height / 3;
        }

    }
}
