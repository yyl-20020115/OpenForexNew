using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Simple volume indicators - display the volume for the given period.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Volume")]
    public class VolumeCustom : CustomPlatformIndicator
    {

        DataBar.DataValueEnum _dataSource = DataBar.DataValueEnum.Volume;
        /// <summary>
        /// 
        /// </summary>
        public DataBar.DataValueEnum DataSource
        {
            get { return _dataSource; }

            set 
            { 
                _dataSource = value;
                Results.Clear();
                OnCalculate(false, DataBarUpdateType.NewPeriod);
            }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public VolumeCustom()
            : base(UserFriendlyNameAttribute.GetTypeAttributeName(typeof(VolumeCustom)), true, false, new string[] { "Volume" })
        {
            base._results.SetResultSetChartType("Volume", LinesChartSeries.ChartTypeEnum.Histogram);
        }

        protected override void OnCalculate(bool fullRecalculation, DataBarUpdateType? updateType)
        {
            if (fullRecalculation)
            {
                double[] values1 = DataProvider.GetValuesAsDouble(_dataSource, 0, DataProvider.BarCount);

                Results.AddSetValues("Volume", 0, values1.Length, true, values1);
            }
            else
            {
                // How many steps to overlap for safety.
                int overlap = Math.Max(0, Math.Min(5, Results.SetLength - 1));

                if (Results.SetLength > DataProvider.BarCount)
                {// This happens sometimes and causes exception.
                    Results.ClipTo(DataProvider.BarCount);
                    return;
                }

                int startIndex = Math.Max(0, Results.SetLength - overlap);
                int count = DataProvider.BarCount - startIndex;
                
                double[] values1 = DataProvider.GetValuesAsDouble(_dataSource, startIndex, count);

                Results.AddSetValues("Volume", startIndex, count, true, values1);
            }
        }

        /// <summary>
        /// Simple clone implementation will not clone results and signals, only parameters.
        /// </summary>
        /// <returns></returns>
        public override PlatformIndicator OnSimpleClone()
        {
            VolumeCustom indicator = new VolumeCustom();
            return indicator;
        }
    }
}
