using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using CommonSupport;
using ForexPlatform;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
	/// <summary>
    /// Summary description for FormMoneyManager.
	/// </summary>
    /// 
    [UserFriendlyName("Money Management Results Test")]
    public class MoneyManagerControl : TesterControl
	{
		private System.Windows.Forms.ListBox listBoxResults;
        private System.Windows.Forms.Button buttonRunSingleTest;
		private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonRunMultipleTests;
        private System.Windows.Forms.Label label5;
        private PropertyGrid propertyGridTester;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private Button buttonShowDistribution;
        private Label label1;
        private NumericUpDown numericUpDownIterationsCount;

        List<double[]> _results = new List<double[]>();

		public MoneyManagerControl()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: AddElement any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required baseMethod for Designer support - do not modify
		/// the contents of this baseMethod with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.listBoxResults = new System.Windows.Forms.ListBox();
            this.buttonRunSingleTest = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonRunMultipleTests = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.propertyGridTester = new System.Windows.Forms.PropertyGrid();
            this.buttonShowDistribution = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownIterationsCount = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIterationsCount)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxResults
            // 
            this.listBoxResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxResults.BackColor = System.Drawing.Color.WhiteSmoke;
            this.listBoxResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxResults.HorizontalScrollbar = true;
            this.listBoxResults.ItemHeight = 16;
            this.listBoxResults.Location = new System.Drawing.Point(10, 37);
            this.listBoxResults.Name = "listBoxResults";
            this.listBoxResults.Size = new System.Drawing.Size(502, 482);
            this.listBoxResults.TabIndex = 0;
            this.listBoxResults.SelectedIndexChanged += new System.EventHandler(this.listBoxResults_SelectedIndexChanged);
            // 
            // buttonRunSingleTest
            // 
            this.buttonRunSingleTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRunSingleTest.Location = new System.Drawing.Point(521, 6);
            this.buttonRunSingleTest.Name = "buttonRunSingleTest";
            this.buttonRunSingleTest.Size = new System.Drawing.Size(267, 27);
            this.buttonRunSingleTest.TabIndex = 1;
            this.buttonRunSingleTest.Text = "Test 1 iteration";
            this.buttonRunSingleTest.Click += new System.EventHandler(this.buttonRunSingleTest_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(320, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(192, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Clear results";
            this.button2.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonRunMultipleTests
            // 
            this.buttonRunMultipleTests.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRunMultipleTests.Location = new System.Drawing.Point(686, 39);
            this.buttonRunMultipleTests.Name = "buttonRunMultipleTests";
            this.buttonRunMultipleTests.Size = new System.Drawing.Size(102, 26);
            this.buttonRunMultipleTests.TabIndex = 12;
            this.buttonRunMultipleTests.Text = "iterations";
            this.buttonRunMultipleTests.Click += new System.EventHandler(this.buttonRunMultipleTest_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.Location = new System.Drawing.Point(12, 534);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(723, 51);
            this.label5.TabIndex = 13;
            this.label5.Text = "Note: zero results shows how many accounts were 0 in the end of the test, loses s" +
                "hows how many accounts were on a loss (less than starting capital) in the end of" +
                " the test";
            // 
            // propertyGridTester
            // 
            this.propertyGridTester.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridTester.Location = new System.Drawing.Point(518, 71);
            this.propertyGridTester.Name = "propertyGridTester";
            this.propertyGridTester.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGridTester.Size = new System.Drawing.Size(270, 514);
            this.propertyGridTester.TabIndex = 15;
            this.propertyGridTester.ToolbarVisible = false;
            // 
            // buttonShowDistribution
            // 
            this.buttonShowDistribution.Enabled = false;
            this.buttonShowDistribution.Location = new System.Drawing.Point(10, 8);
            this.buttonShowDistribution.Name = "buttonShowDistribution";
            this.buttonShowDistribution.Size = new System.Drawing.Size(192, 23);
            this.buttonShowDistribution.TabIndex = 16;
            this.buttonShowDistribution.Text = "Show results distribution";
            this.buttonShowDistribution.Click += new System.EventHandler(this.buttonShowDistribution_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(518, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "Test";
            // 
            // numericUpDownIterationsCount
            // 
            this.numericUpDownIterationsCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownIterationsCount.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownIterationsCount.Location = new System.Drawing.Point(560, 42);
            this.numericUpDownIterationsCount.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.numericUpDownIterationsCount.Name = "numericUpDownIterationsCount";
            this.numericUpDownIterationsCount.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownIterationsCount.TabIndex = 18;
            this.numericUpDownIterationsCount.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // MoneyManagerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.numericUpDownIterationsCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonShowDistribution);
            this.Controls.Add(this.propertyGridTester);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonRunMultipleTests);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonRunSingleTest);
            this.Controls.Add(this.listBoxResults);
            this.ImageName = "Help.png";
            this.Name = "MoneyManagerForm";
            this.Size = new System.Drawing.Size(800, 600);
            this.Name = "Money Management Results Test";
            this.Load += new System.EventHandler(this.FormMoneyManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIterationsCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		RiskRewardTester _tester = new RiskRewardTester();

		private void buttonRunSingleTest_Click(object sender, System.EventArgs e)
		{
            double result = _tester.RunTest();
			listBoxResults.Items.Add("Test iteration ran result is: " + result.ToString());
		}

        private void FormMoneyManager_Load(object sender, EventArgs e)
        {
            propertyGridTester.SelectedObject = _tester;
        }

		private void buttonRunMultipleTest_Click(object sender, System.EventArgs e)
		{
            int iterations = (int)this.numericUpDownIterationsCount.Value;

			int resultingZeros = 0;
			int resultingLoses = 0;
			double sumTotal = 0;

            double[] results = new double[iterations];

			for(int i=0; i<iterations; i++)
			{
                double testValue = _tester.RunTest();
                
                results[i] = testValue;

                sumTotal += testValue;
                
                if (testValue == 0)
                {
                    resultingZeros++;
                }
                
                if (testValue < _tester.StartingCapital)
                {
                    resultingLoses++;
                }
			}

            _results.Add(results);

			double averageResult = sumTotal / iterations;
			listBoxResults.Items.Add(iterations.ToString()+" Test iterations ran - results: zeroes[" + resultingZeros.ToString() +", % " +(100*((float)resultingZeros/(float)iterations)).ToString() 
				+ "]  loses[" + resultingLoses.ToString() + ", % "+(100*((float)resultingLoses/(float)iterations)).ToString()
				+ "] average[" + ((float)averageResult).ToString() + "]");
		}

		
		private void buttonClear_Click(object sender, System.EventArgs e)
		{
			listBoxResults.Items.Clear();
            _results.Clear();
		}

        private void listBoxResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonShowDistribution.Enabled = listBoxResults.SelectedItems.Count > 0;
        }

        private void buttonShowDistribution_Click(object sender, EventArgs e)
        {
            ChartForm form = new ChartForm("Distribution of results");
            form.Show();

            Dictionary<double, double> distributedResults = MathHelper.CalculateValueDistribution(_results[listBoxResults.SelectedIndex], 256);
            double[] distributedValues = GeneralHelper.EnumerableToArray(distributedResults.Values);
            double[] distributedValuesMA = MathHelper.CalculateQuickMA(distributedValues, 12);



            form.Chart.MasterPane.Add(new LinesChartSeries("Value distribution", LinesChartSeries.ChartTypeEnum.Histogram, GeneralHelper.DoublesToFloats(distributedValues)), true, false);
            form.Chart.MasterPane.Add(new LinesChartSeries("Value distribution MA 12", LinesChartSeries.ChartTypeEnum.Line, GeneralHelper.DoublesToFloats(distributedValuesMA)), true, false);
        }


	}
}
