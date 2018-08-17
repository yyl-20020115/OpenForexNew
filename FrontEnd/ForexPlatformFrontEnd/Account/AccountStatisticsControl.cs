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
    /// Control shows statistical information regarding an account performance.
    /// </summary>
    public partial class AccountStatisticsControl : UserControl
    {
        Account _account;

        LinesChartSeries _chartSeries = new LinesChartSeries("Equity");

        /// <summary>
        /// 
        /// </summary>
        public AccountStatisticsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public AccountStatisticsControl(Account account)
        {
            InitializeComponent();

            if (this.DesignMode)
            {
                return;
            }

            _account = account;

            _account.UpdatedEvent += new Account.AccountUpdateDelegate(_account_UpdateEvent);
            chartControl1.MasterPane.Add(_chartSeries);
        }

        public void UnInitializeControl()
        {
            if (this.DesignMode)
            {
                return;
            }

            _account.UpdatedEvent -= new Account.AccountUpdateDelegate(_account_UpdateEvent);

            chartControl1.MasterPane.Remove(_chartSeries);
            _chartSeries.ClearValues();
            _chartSeries = null;
        }

        void _account_UpdateEvent(Account account)
        {
            if (this.DesignMode)
            {
                return;
            }

            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        public void UpdateUI()
        {
            if (this.DesignMode)
            {
                return;
            }

            textBoxTimeFirstOrderOpen.Text = GeneralHelper.GetShortDateTime(_account.Statistics.FirstOrderTime);
            textBoxTimeLastOrderOpen.Text = GeneralHelper.GetShortDateTime(_account.Statistics.LastOrderTime);

            textBoxEquityInitial.Text = GeneralHelper.ToString(_account.Statistics.InitialEquity);

            textBoxPerformanceOverall.Text = GeneralHelper.ToString(_account.Statistics.PerformancePercentage, "#.00");

            textBoxPerformanceBest.Text = GeneralHelper.ToString(_account.Statistics.BestPerformancePercentage, "#.00");
            textBoxPerformanceWorst.Text = GeneralHelper.ToString(_account.Statistics.WorstPerformancePercentage, "#.00");

            textBoxDrawdownOverall.Text = GeneralHelper.ToString(_account.Statistics.OveralDrawDownPercentage, "#.00");
            textBoxDrawdownLargest.Text = GeneralHelper.ToString(_account.Statistics.LargestDrawDownPercentage, "#.00");
            
            textBoxEquityMax.Text = GeneralHelper.ToString(_account.Statistics.MaxEquity);
            textBoxEquityMin.Text = GeneralHelper.ToString(_account.Statistics.MinEquity);

            textBoxTradesTotalCount.Text = _account.Statistics.TotalTrades.ToString();
            textBoxTradesWinnersCount.Text = _account.Statistics.WinningTrades.ToString();
            textBoxTradesLosersCount.Text = _account.Statistics.LosingTrades.ToString();
            textBoxTradesBuyCount.Text = _account.Statistics.BuyTrades.ToString();
            textBoxTradesSellCount.Text = _account.Statistics.SellTrades.ToString();
            
            textBoxProfitLossAllWinners.Text = _account.Statistics.WinnersProfit.ToString();
            textBoxProfitLossAllLosers.Text = _account.Statistics.LosersLoss.ToString();
            textBoxProfitLossOverall.Text = _account.Statistics.ProfitOverall.ToString();
            
            textBoxTradesBiggestWinner.Text = GeneralHelper.ToString(_account.Statistics.BiggestWinner);
            textBoxTradesBiggestLoser.Text = GeneralHelper.ToString(_account.Statistics.BiggestLoser);

            textBoxTradesMaxConsecutiveWinners.Text = GeneralHelper.ToString(_account.Statistics.MaxConsecutiveWinners);
            textBoxTradesMaxConsecutiveLosers.Text = GeneralHelper.ToString(_account.Statistics.MaxConsecutiveLosers);

            if (_chartSeries != null)
            {
                _chartSeries.SetValues(new float[][] { _account.Statistics.EquityHistoryValues });
            }
        }

        private void ExecutionAccountStatisticsControl_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }
}
