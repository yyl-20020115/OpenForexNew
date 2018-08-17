using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonFinancial;
using ForexPlatform;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control provides information for multiple order execution accountInfos. It can be displayed
    /// in compact mode (1 account showing, and a combo to allow selection) or full mode (all accountInfos showing).
    /// This control can be used in 2 modes - trough the expert host variable (attach directly to expert host)
    /// or by directly adding/removing order execution providers.
    /// </summary>
    public partial class AccountsControl : UserControl
    {
        List<ISourceOrderExecution> _providers = new List<ISourceOrderExecution>();
        List<AccountControl> _accountControls = new List<AccountControl>();

        bool _compactMode = true;
        int _fullModeHeight = 140;

        ISourceAndExpertSessionManager _sessionManager;
        /// <summary>
        /// 
        /// </summary>
        public ISourceAndExpertSessionManager SessionManager
        {
            get { return _sessionManager; }
            set 
            {
                if (_sessionManager != null)
                {
                    _sessionManager.SessionsUpdateEvent -= new CommonSupport.GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(_sessionManager_SessionsUpdateEvent);
                }

                _sessionManager = value;

                if (_sessionManager != null)
                {
                    _sessionManager.SessionsUpdateEvent += new CommonSupport.GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(_sessionManager_SessionsUpdateEvent);
                }

                _sessionManager_SessionsUpdateEvent(_sessionManager);
            }
        }

        void _sessionManager_SessionsUpdateEvent(ISourceAndExpertSessionManager host)
        {
            _providers.Clear();
            if (host != null)
            {
                foreach (ExpertSession session in _sessionManager.SessionsArray)
                {
                    if (session.OrderExecutionProvider != null)
                    {
                        _providers.Add(session.OrderExecutionProvider);
                    }
                }
            }
            UpdateUI();
        }

        /// <summary>
        /// 
        /// </summary>
        public AccountsControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateUI();
        }

        public bool AddProvider(ISourceOrderExecution provider)
        {
            if (_providers.Contains(provider))
            {
                return false;
            }

            _providers.Add(provider);
            UpdateUI();

            return true;
        }

        public bool RemoveProvider(ISourceOrderExecution provider)
        {
            bool result = _providers.Remove(provider);
            UpdateUI();
            return result;
        }

        void AddAccountControl(Account account)
        {
            AccountControl control = new AccountControl();
            control.Account = account;
            _accountControls.Add(control);
            this.Controls.Add(control);
        }

        void RemoveAccountControl(AccountControl control)
        {
            control.Account = null;
            _accountControls.Remove(control);
            this.Controls.Remove(control);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            // Update the combobox items.
            toolStripComboBoxAccounts.Visible = _compactMode;
            toolStripLabelAccount.Visible = _compactMode;

            if (_compactMode == false)
            {
                this.MinimumSize = new Size(0, 0);
                this.MaximumSize = new Size(0, 0);

                this.Height = _fullModeHeight + 2;
            }
            else
            {
                this.Height = toolStripMain.Height + 2;
                if (_accountControls.Count > 0)
                {
                    this.Height += _accountControls[0].Height;
                }

                this.MinimumSize = new Size(0, this.Height);
                this.MaximumSize = new Size(0, this.Height);
            }

            for (int i = 0; i < _providers.Count; i++)
            {
                string name = _providers[i].DefaultAccount.Info.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = "Unknown";
                }

                if (toolStripComboBoxAccounts.Items.Count <= i)
                {
                    toolStripComboBoxAccounts.Items.Add(name);
                }
                else
                {
                    toolStripComboBoxAccounts.Items[i] = name;
                }
            }

            while (toolStripComboBoxAccounts.Items.Count > _providers.Count)
            {// Remove unneeded items.
                toolStripComboBoxAccounts.Items.RemoveAt(toolStripComboBoxAccounts.Items.Count - 1);
            }

            // Update the account controls.
            if (_compactMode)
            {
                for (int i = _accountControls.Count - 1; i >= 0 ; i--)
                {// Remove any control in excess of 1.
                    if (i > 0 || _providers.Count == 0)
                    {
                        RemoveAccountControl(_accountControls[i]);
                    }
                }

                if (_accountControls.Count == 0 && _providers.Count > 0)
                {// AddElement the one control.
                    AddAccountControl(_providers[0].DefaultAccount);
                }

                if (_accountControls.Count > 0 && toolStripComboBoxAccounts.SelectedIndex >= 0)
                {
                    _accountControls[0].Account = _providers[toolStripComboBoxAccounts.SelectedIndex].DefaultAccount;
                }
            }
            else
            {
                for (int i = 0; i < _providers.Count; i++)
                {
                    if (_accountControls.Count <= i)
                    {// AddElement new account control.
                        AddAccountControl(_providers[i].DefaultAccount);
                    }
                }

                while (_accountControls.Count > _providers.Count)
                {// Remove not needed account control.
                    RemoveAccountControl(_accountControls[_accountControls.Count - 1]);
                }
            }
        }

        private void toolStripButtonSwitchMode_Click(object sender, EventArgs e)
        {
            if (_compactMode)
            {
                _compactMode = false;
                UpdateUI();
            }
            else
            {
                _compactMode = true;
                _fullModeHeight = this.Height;
                UpdateUI();
            }

        }

        private void toolStripComboBoxAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }
}
