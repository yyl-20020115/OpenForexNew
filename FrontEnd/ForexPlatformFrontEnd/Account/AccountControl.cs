using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Class visualizes an account.
    /// </summary>
    public partial class AccountControl : UserControl
    {
        Account _executionAccount;
        
        /// <summary>
        /// Account to visualize.
        /// </summary>
        public Account Account
        {
            get { return _executionAccount; }
            set 
            {
                if (_executionAccount != null)
                {
                    _executionAccount.UpdatedEvent -= new Account.AccountUpdateDelegate(_executionAccount_UpdateEvent);
                    _executionAccount = null;
                }

                _executionAccount = value;
                if (_executionAccount != null)
                {
                    _executionAccount.UpdatedEvent += new Account.AccountUpdateDelegate(_executionAccount_UpdateEvent);
                }

                WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
            }
        }

        ISourceOrderExecution _provider = null;
        public ISourceOrderExecution Provider
        {
            get
            {
                return _provider;
            }

            set
            {
                if (_provider != null)
                {
                    _provider.AccountInfoUpdateEvent -= new AccountInfoUpdateDelegate(_provider_AccountInfoUpdateEvent);
                }

                _provider = value;

                if (_provider != null)
                {
                    _provider.AccountInfoUpdateEvent += new AccountInfoUpdateDelegate(_provider_AccountInfoUpdateEvent);
                }

                WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public AccountControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateUI();
        }

        void _provider_AccountInfoUpdateEvent(IOrderSink provider, AccountInfo accountInfo)
        {
            if (((ISourceOrderExecution)provider).DefaultAccount != Account)
            {
                Account = ((ISourceOrderExecution)provider).DefaultAccount;
            }

            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        void _executionAccount_UpdateEvent(Account accountInfo)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            if (_executionAccount != null)
            {
                this.toolStripStatusLabelAccount.Text = "Account [" + _executionAccount.Info.Name + "]";
            }
            else
            {
                this.toolStripStatusLabelAccount.Text = "Account NA";
            }

            this.Enabled = _executionAccount != null;

            toolStripButtonStatistics.Enabled = _executionAccount != null;
            toolStripButtonProperties.Enabled = _executionAccount != null;

            if (_executionAccount == null)
            {
                toolStripStatusLabelBalance.Text = "-";
                toolStripStatusLabelEquity.Text = "-";
                toolStripStatusLabelFreeMargin.Text = "-";
                toolStripStatusLabelMargin.Text = "-";
                toolStripStatusLabelMarginLevel.Text = "-";
                toolStripStatusLabelProfit.Text = "-";
            }
            else
            {
                toolStripStatusLabelBalance.Text = GeneralHelper.ToString(_executionAccount.Info.Balance, "#.00");
                toolStripStatusLabelEquity.Text = GeneralHelper.ToString(_executionAccount.Info.Equity, "#.00");
                toolStripStatusLabelMargin.Text = GeneralHelper.ToString(_executionAccount.Info.Margin, "#.00");
                toolStripStatusLabelFreeMargin.Text = GeneralHelper.ToString(_executionAccount.Info.FreeMargin, "#.00");
                toolStripStatusLabelMarginLevel.Text = GeneralHelper.ToString(_executionAccount.Info.FreeMargin, "#.00");
                toolStripStatusLabelProfit.Text = GeneralHelper.ToString(_executionAccount.Info.Profit, "#.00");
            }
        }

        private void toolStripButtonStatistics_Click(object sender, EventArgs e)
        {
            if (_executionAccount != null)
            {
                AccountStatisticsControl control = new AccountStatisticsControl(_executionAccount);
                HostingForm form = new HostingForm("Account Performance Statistics", control);
                form.Show();
            }
        }

        private void toolStripButtonProperties_Click(object sender, EventArgs e)
        {
            if (_executionAccount != null)
            {
                PropertiesForm form = new PropertiesForm("Account Information Properties", _executionAccount.Info);
                form.ShowDialog();
            }
        }
    }
}
