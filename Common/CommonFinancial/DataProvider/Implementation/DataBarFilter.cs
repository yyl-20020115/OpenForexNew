using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Filters bar dataDelivery against spikes etc.
    /// </summary>
    public class DataBarFilter
    {
        volatile bool _enabled = true;
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        int _maximumConsecutiveSpikes = 5;

        public int MaximumConsecutiveSpikes
        {
            get { return _maximumConsecutiveSpikes; }
            set { _maximumConsecutiveSpikes = value; }
        }

        decimal _maximumAllowedBarToBarPercentageForex = 3;
        /// <summary>
        /// Forex is considered to fluctuate less.
        /// </summary>
        public decimal MaximumAllowedBarToBarPercentageForex
        {
            get { return _maximumAllowedBarToBarPercentageForex; }
            set { _maximumAllowedBarToBarPercentageForex = value; }
        }

        decimal _maximumAllowedBarToBarPercentageStock = 300;
        /// <summary>
        /// Stocks tend to fluctuate more
        /// </summary>
        public decimal MaximumAllowedBarToBarPercentageStock
        {
            get { return _maximumAllowedBarToBarPercentageStock; }
            set { _maximumAllowedBarToBarPercentageStock = value; }
        }
        
        decimal MaxDivergenceCoefficientForex
        {
            get { return _maximumAllowedBarToBarPercentageForex / 100m; }
        }

        decimal MaxDivergenceCoefficientStock
        {
            get { return _maximumAllowedBarToBarPercentageStock / 100m; }
        }

        DataBarHistoryProvider _provider;
        /// <summary>
        /// 
        /// </summary>
        public DataBarFilter(DataBarHistoryProvider provider)
        { 
            _provider = provider;
        }

        /// <summary>
        /// 
        /// </summary>
        public void FilterUpdate(ISourceDataDelivery dataDelivery, DataSessionInfo session, DataHistoryUpdate update)
        {
            if (_enabled == false)
            {
                return;
            }

            decimal? lastHigh = null;
            decimal? lastLow = null;

            decimal divergence = MaxDivergenceCoefficientStock;

            if (session.Symbol.IsForexPair)
            {
                divergence = MaxDivergenceCoefficientForex;
            }

            int consecutiveSpikes = 0;
            for (int i = 0; i < update.DataBarsUnsafe.Count; i++)
			{
                if (lastHigh.HasValue && lastLow.HasValue)
                {
                    DataBar bar = update.DataBarsUnsafe[i];

                    if ( (bar.High > lastHigh.Value * (1 + divergence) || bar.Low < lastLow.Value / (1 + divergence))
                        && consecutiveSpikes <= _maximumConsecutiveSpikes)
                    {// Bar spike detected.
                        consecutiveSpikes++;

                        SystemMonitor.Report("Spike detected in data [" + session.Symbol.Name + "]. Bar values limited to previous [" + lastHigh.Value + ", " + bar.High + "; " + lastLow.Value + ", " + bar.Low + "].");

                        update.DataBarsUnsafe[i] = new DataBar(bar.DateTime,
                            GeneralHelper.LimitRange(bar.Open, lastLow.Value, lastHigh.Value),
                            GeneralHelper.LimitRange(bar.High, lastLow.Value, lastHigh.Value),
                            GeneralHelper.LimitRange(bar.Low, lastLow.Value, lastHigh.Value),
                            GeneralHelper.LimitRange(bar.Close, lastLow.Value, lastHigh.Value),
                            bar.Volume);
                    }
                    else
                    {
                        consecutiveSpikes = 0;
                    }
                }

                lastHigh = update.DataBarsUnsafe[i].High;
                lastLow = update.DataBarsUnsafe[i].Low;
			}
            
        }
    }
}
