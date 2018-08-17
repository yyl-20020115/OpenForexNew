using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
    [UserFriendlyName("Consecutive Orders Result Test")]
    public partial class ConsecutivesTestForm : TesterControl
    {
        Random _random = new Random();

        public ConsecutivesTestForm()
        {
            InitializeComponent();
        }

        public void PerformConsecutivesIteration(out int maxPositives, out int maxNegatives)
        {
            maxPositives = 0;
            maxNegatives = 0;
            int positives = 0;
            int negatives = 0;

            for (int i = 0; i < (int)numericUpDownTotalCount.Value; i++)
            {
                // 0 - 99 is positives, -1 to -100 is negatives (100 count each)
                int value = _random.Next(-100, 100);

                if (value >= 0)
                {
                    positives++;
                    negatives = 0;
                }
                else
                {
                    positives = 0;
                    negatives++;
                }

                maxPositives = Math.Max(maxPositives, positives);
                maxNegatives = Math.Max(maxNegatives, negatives);
            }
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            List<double> positiveResults = new List<double>();
            List<double> negativeResults = new List<double>();

            for (int i = 0; i < (int)numericUpDownIterations.Value; i++)
            {
                int positives, negatives;
                PerformConsecutivesIteration(out positives, out negatives);

                if (positiveResults.Count < positives + 1)
                {
                    positiveResults.AddRange(new double[positives - positiveResults.Count + 1]);
                    negativeResults.AddRange(new double[positives - negativeResults.Count + 1]);
                }

                if (negativeResults.Count < negatives + 1)
                {
                    positiveResults.AddRange(new double[negatives - positiveResults.Count + 1]);
                    negativeResults.AddRange(new double[negatives - negativeResults.Count + 1]);
                }

                positiveResults[positives] = positiveResults[positives] + 1;
                negativeResults[negatives] = negativeResults[negatives] + 1;
            }

            graphControl.Clear();
            LinesChartSeries series1 = new LinesChartSeries("+", LinesChartSeries.ChartTypeEnum.Histogram, GeneralHelper.DoublesToFloats(positiveResults.ToArray()));
            series1.DefaultPen = Pens.Green;
            graphControl.MasterPane.Add(series1);

            LinesChartSeries series2 = new LinesChartSeries("-", LinesChartSeries.ChartTypeEnum.Histogram, GeneralHelper.DoublesToFloats(negativeResults.ToArray()));
            series2.DefaultPen = Pens.Red;
            graphControl.MasterPane.Add(series2);

            graphControl.MasterPane.FitDrawingSpaceToScreen(true, true);
        }
    }
}
