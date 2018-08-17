using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Serves as basis for manual trading.
    /// </summary>
    public partial class ManualMultiTradeExpertControl : CommonBaseControl
    {
        ManualMultiTradeExpert _expert;

        /// <summary>
        /// 
        /// </summary>
        public ManualMultiTradeExpertControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public ManualMultiTradeExpertControl(ManualMultiTradeExpert expert)
        {
            InitializeComponent();

            _expert = expert;
            _expert.Manager.SessionsUpdateEvent += new CommonSupport.GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(SessionManager_SessionUpdateEvent);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //toolStripAdvanced.Visible = toolStripButtonAdvanced.Checked;

            // Clear temp items.
            for (int i = toolStripMain.Items.Count - 1; i >= 0; i--)
            {
                ToolStripItem item = toolStripMain.Items[i];
                if (item.Tag as string == "del")
                {
                    toolStripMain.Items.Remove(item);
                }
            }

            UpdateUI();
        }

        void SessionManager_SessionUpdateEvent(ISourceAndExpertSessionManager sessionManager)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        List<ToolStripButton> GetPanelsButtons()
        {
            List<ToolStripButton> buttons = new List<ToolStripButton>();
            foreach (ToolStripItem item in toolStripMain.Items)
            {
                if (item is ToolStripButton && item.Tag is Panel)
                {
                    buttons.Add((ToolStripButton)item);
                }
            }
            return buttons;
        }

        Panel CreateSessionPanel(PlatformExpertSession session)
        {
            Panel panel = new Panel();

            // Session not found, create new button for it.
            PlatformExpertSessionControl sessionChartControl = new PlatformExpertSessionControl();
            sessionChartControl.Dock = DockStyle.Fill;
            sessionChartControl.CreateControl();
            sessionChartControl.Session = session;

            panel.Controls.Add(sessionChartControl);

            if (session.OrderExecutionProvider != null)
            {
                Splitter splitter = new SplitterEx();
                splitter.Dock = DockStyle.Bottom;
                panel.Controls.Add(splitter);
                splitter.Height = 6;

                OrdersControl ordersControl = new OrdersControl();
                ordersControl.Dock = DockStyle.Bottom;
                ordersControl.CreateControl();
                ordersControl.SingleSession = session;
                ordersControl.VisibleChanged += delegate(object sender, EventArgs e)
                {
                    splitter.Visible = ordersControl.Visible;
                };
                //ordersControl.Manager = this._expert.Manager;
                panel.Controls.Add(ordersControl);

                sessionChartControl.CorrespondingOrdersControl = ordersControl;

                Splitter splitter2 = new Splitter();
                splitter2.Dock = DockStyle.Bottom;
                panel.Controls.Add(splitter2);

                AccountControl accountControl = new AccountControl();
                accountControl.Dock = DockStyle.Bottom;
                accountControl.CreateControl();
                accountControl.Account = session.OrderExecutionProvider.DefaultAccount;
                panel.Controls.Add(accountControl);
            }

            panel.Tag = sessionChartControl;

            panel.Visible = false;
            panel.Dock = DockStyle.Fill;
            
            return panel;
        }

        void DestroySessionPanel(Panel panel)
        {
            if (panel.Tag is PlatformExpertSessionControl)
            {
                PlatformExpertSessionControl sessionControl = (PlatformExpertSessionControl)panel.Tag;
                
                if (sessionControl.CorrespondingOrdersControl != null)
                {
                    sessionControl.CorrespondingOrdersControl.SessionManager = null;
                    sessionControl.CorrespondingOrdersControl = null;
                }

                sessionControl.Session = null;
                sessionControl.Dispose();

                panel.Tag = null;
            }
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            if (_expert == null || _expert.Manager == null)
            {
                return;
            }

            ExpertSession[] sessions;
            lock (_expert.Manager)
            {
                sessions = GeneralHelper.EnumerableToArray<ExpertSession>(_expert.Manager.SessionsArray);
            }

            List<ToolStripButton> panelsButtons = GetPanelsButtons();
            foreach (PlatformExpertSession session in sessions)
            {
                // Search in already existing.
                bool sessionFound = false;
                foreach (ToolStripButton button in panelsButtons)
                {
                    Panel containingPanel = (Panel)button.Tag;
                    if (containingPanel.Tag is PlatformExpertSessionControl)
                    {
                        PlatformExpertSessionControl sessionControl = (PlatformExpertSessionControl)containingPanel.Tag;
                        if (sessionControl.Session == session)
                        {// Session found and presented, continue.
                            panelsButtons.Remove(button);
                            sessionFound = true;
                            break;
                        }
                    }
                }

                if (sessionFound)
                {
                    continue;
                }

                // Create new sessionInformation.
                Panel panel = CreateSessionPanel(session);
                panel.Parent = this;
                panel.BringToFront();
                panel.CreateControl();

                ToolStripButton newButton = new ToolStripButton(session.Info.Name, ForexPlatformFrontEnd.Properties.Resources.PIN_GREY);
                newButton.Tag = panel;
                newButton.Click += new EventHandler(toolStripButton_Click);
                this.toolStripMain.Items.Add(newButton);

                // Show newly created sessionInformation.
                toolStripButton_Click(newButton, EventArgs.Empty);
            }

            // Those buttons no longer have corresponding sessions and must be removed.
            foreach(ToolStripButton button in panelsButtons)
            {
                Panel panel = button.Tag as Panel;
                if (panel.Tag is PlatformExpertSessionControl)
                {
                    DestroySessionPanel(panel);
                    this.Controls.Remove(panel);

                    button.Click -= new EventHandler(toolStripButton_Click);
                    toolStripMain.Items.Remove(button);
                }
            }

            // Make sure there is at least one visibile sessionInformation if there are any.
            panelsButtons = GetPanelsButtons();
            bool visibleSessionFound = false;
            foreach (ToolStripButton button in panelsButtons)
            {
                if (((Panel)(button.Tag)).Visible)
                {
                    visibleSessionFound = true;
                    break;
                }
            }

            if (visibleSessionFound == false && panelsButtons.Count > 0)
            {// Show the first available sessionInformation.
                toolStripButton_Click(panelsButtons[0], EventArgs.Empty);
            }

            //toolStripAdvanced.Visible = toolStripButtonAdvanced.Checked;
        }

        public override void UnInitializeControl()
        {
            if (_expert != null)
            {
                if (_expert.Manager != null)
                {
                    _expert.Manager.SessionsUpdateEvent -= new CommonSupport.GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(SessionManager_SessionUpdateEvent);
                }
                _expert = null;
            }

            base.UnInitializeControl();
        }

        private void toolStripButtonCreateSession_Click(object sender, EventArgs e)
        {
            CreateExpertSessionForm form = new CreateExpertSessionForm((LocalExpertHost)_expert.Manager);
            form.ShowDialog();
        }

        private void toolStripButtonDeleteSession_Click(object sender, EventArgs e)
        {
            foreach (ToolStripButton button in GetPanelsButtons())
            {
                Panel panel = (Panel)button.Tag;
                if (button.Checked == false)
                {
                    continue;
                }

                if (panel.Tag is PlatformExpertSessionControl)
                {
                    PlatformExpertSessionControl control = panel.Tag as PlatformExpertSessionControl;
                    _expert.Manager.UnRegisterExpertSession(control.Session);
                }
                else
                {// This is a stand alone component - just remove it.
                    this.Controls.Remove(panel);

                    button.Click -= new EventHandler(toolStripButton_Click);
                    toolStripMain.Items.Remove(button);
                }

                break;
            }
        }

        private void toolStripButton_Click(object sender, EventArgs e)
        {
            // Iterate 1st time to active newly assigned control.
            foreach (ToolStripButton button in GetPanelsButtons())
            {
                if (button == sender)
                {
                    button.Checked = true;
                    ((Panel)(button.Tag)).Visible = true;
                }
            }

            // Iterate 2nd time for better UI transition.
            foreach (ToolStripButton button in GetPanelsButtons())
            {
                if (button != sender)
                {
                    button.Checked = false;
                    ((Panel)(button.Tag)).Visible = false;
                }
            }
        }

    }
}
