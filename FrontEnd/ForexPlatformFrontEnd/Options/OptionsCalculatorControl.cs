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
    /// 
    /// </summary>
    [UserFriendlyName("Option Calculator")]
    public partial class OptionsCalculatorControl : TesterControl
    {
        /// <summary>
        /// 
        /// </summary>
        public OptionsCalculatorControl()
        {
            InitializeComponent();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public OptionsCalculatorControl()
        //{
        //    InitializeComponent();
        //}

        private void numericUpDownStrike_Enter(object sender, EventArgs e)
        {
            numericUpDownStrike.Select(0, 10);
        }

        private void numericUpDownPrice_Enter(object sender, EventArgs e)
        {
            numericUpDownPrice.Select(0, 10);
        }

        private void buttonPut_Click(object sender, EventArgs e)
        {
            float[] values = new float[(int)numericUpDownEnd.Value - (int)numericUpDownStart.Value];
            float price = (float)numericUpDownPrice.Value;
            float strike = (float)numericUpDownStrike.Value;

            for (int i = 0; i < values.Length; i++)
            {
                float difference = price + strike - i;
                if (difference > 0)
                {
                    values[i] = difference / (float)numericUpDownPrice.Value;
                }
            }

            LinesChartSeries series = new LinesChartSeries();
            series.Name = "call " + price.ToString("#.00") + ";" + strike.ToString("#.00");
            series.AddValueSet(values);

            chartControl1.MasterPane.Add(series);
        }

        private void buttonCall_Click(object sender, EventArgs e)
        {
            float[] values = new float[(int)numericUpDownEnd.Value - (int)numericUpDownStart.Value];
            float price = (float)numericUpDownPrice.Value;
            float strike = (float)numericUpDownStrike.Value;

            for (int i = 0; i < values.Length; i++)
            {
                float difference = i - price - strike;
                if (difference > 0)
                {
                    values[i] = difference / (float)numericUpDownPrice.Value;
                }
            }

            LinesChartSeries series = new LinesChartSeries();
            series.Name = "call " + price.ToString("#.00") + ";" + strike.ToString("#.00");
            series.AddValueSet(values);
            
            chartControl1.MasterPane.Add(series);
        }
    }
}
