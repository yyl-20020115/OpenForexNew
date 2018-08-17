using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Indicator class child, that joins the platform requirements for an indicator, with the indicator class.
    /// Contains orderInfo for the UI of the indicator (PlatformIndicatorChartSeries).
    /// </summary>
    [Serializable]
    public abstract class PlatformIndicator : Indicator
    {
        volatile protected PlatformIndicatorChartSeries _chartSeries;
        /// <summary>
        /// UI component.
        /// </summary>
        public PlatformIndicatorChartSeries ChartSeries
        {
            get { return _chartSeries; }
        }

        float? _rangeMinimum = null;
        /// <summary>
        /// If the indicator is ranged (has a fixed minimum and maximum value), this must be assigned to minimum.
        /// </summary>
        public float? RangeMinimum
        {
            get { return _rangeMinimum; }
            set { _rangeMinimum = value; }
        }

        float? _rangeMaximum = null;
        /// <summary>
        /// If the indicator is ranged (has a fixed minimum and maximum value), this must be assigned to minimum.
        /// </summary>
        public float? RangeMaximum
        {
            get { return _rangeMaximum; }
            set { _rangeMaximum = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlatformIndicator(string name, bool? isTradeable, bool? isScaledToQuotes, string[] resultSetNames)
            : base(name, isTradeable, isScaledToQuotes, resultSetNames)
        {
            TracerHelper.Trace(this.Name);

            _chartSeries = CreateChartSeries();
            // Needs immediate initialization since it will be added to chart.
            _chartSeries.Initialize(this);
        }

        /// <summary>
        /// Overriden by children implementations to specify indicator implementation specific chart series.
        /// </summary>
        /// <returns></returns>
        protected virtual PlatformIndicatorChartSeries CreateChartSeries()
        {
            return new PlatformIndicatorChartSeries(this.Name);
        }

        /// <summary>
        /// Deserialization callback.
        /// </summary>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            // Needs immediate initialization since it will be added to chart.
            _chartSeries.Initialize(this);
        }

        public override void UnInitialize()
        {
            _chartSeries.UnInitialize();

            base.UnInitialize();
        }

        public PlatformIndicator SimpleClone()
        {
            PlatformIndicator indicator = OnSimpleClone();
            indicator.RangeMaximum = this.RangeMaximum;
            indicator.RangeMinimum = this.RangeMinimum;

            return indicator;
        }

        /// <summary>
        /// Create a shallow copy of the instance.
        /// </summary>
        public abstract PlatformIndicator OnSimpleClone();
    }
}
