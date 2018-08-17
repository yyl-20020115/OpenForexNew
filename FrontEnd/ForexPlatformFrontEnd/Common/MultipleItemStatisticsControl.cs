using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
    public partial class MultipleItemStatisticsControl : UserControl
    {

        List<MultipleItemStatisticsSet> _itemStatisticsSets = new List<MultipleItemStatisticsSet>();

        /// <summary>
        /// 
        /// </summary>
        public MultipleItemStatisticsControl()
        {
            InitializeComponent();
        }

        private void MultipleItemStatisticsControl_Load(object sender, EventArgs e)
        {
            listViewItemSelection.Clear();
            listViewItems.Clear();
        
            foreach (string name in PropertyStatisticsData.ColumnHeaders)
            {
                ColumnHeader columnHeader = listViewItemSelection.Columns.Add(name);
                columnHeader.Width = 120;
            }

            // AddElement 0 and level lines.
            LevelLinesObject lines = new LevelLinesObject();
            lines.Levels.Add(0);
            chartControl.MasterPane.CustomObjectsManager.Add(lines);
        }

        public void AddMultipleItemStatisticsSet(MultipleItemStatisticsSet newSet)
        {
            _itemStatisticsSets.Add(newSet);

            comboBoxSelectedSet.Items.Add(newSet.Name);

            // Update the UI to comply with the new set of dataDelivery.
            foreach (PropertyStatisticsData data in newSet.PropertiesData)
            {
                ListViewItem listViewItemSelectionItem = new ListViewItem(data.Columns);
                // AddElement the name of the set here.
                listViewItemSelectionItem.Tag = data;
                listViewItemSelection.Items.Add(listViewItemSelectionItem);
            }

            if (listViewItemSelection.Columns.Count > 0)
            {
                listViewItemSelection.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        protected void PresentDataSeries(string seriesName, double[] dataValues)
        {
            if (checkBoxShowMA.Checked)
            {// Show the MA
                double[] maValues = MathHelper.CalculateQuickMA(dataValues, (int)this.numericUpDownMAPeriods.Value);
                chartControl.MasterPane.Add(new LinesChartSeries(seriesName + "MA" + ((int)this.numericUpDownMAPeriods.Value).ToString(), LinesChartSeries.ChartTypeEnum.Line, GeneralHelper.DoublesToFloats(maValues)));
            }

            chartControl.MasterPane.Add(new LinesChartSeries(seriesName, LinesChartSeries.ChartTypeEnum.Line, GeneralHelper.DoublesToFloats(dataValues)));
        }

        private void buttonPresent_Click(object sender, EventArgs e)
        {
            chartControl.Clear();
            foreach (ListViewItem item in this.listViewItemSelection.SelectedItems)
            {
                PropertyStatisticsData data = item.Tag as PropertyStatisticsData;
                PresentDataSeries(data.Name, data.Values.ToArray());
            }

        }

        private void buttonPresentSorted_Click(object sender, EventArgs e)
        {
            chartControl.Clear();

            foreach (ListViewItem item in this.listViewItemSelection.SelectedItems)
            {
                PropertyStatisticsData data = item.Tag as PropertyStatisticsData;
                double[] result = GeneralHelper.Sort(data.Values, false);
                PresentDataSeries(data.Name, result);
            }

        }

        private void buttonPresentDistribution_Click(object sender, EventArgs e)
        {
            chartControl.Clear();

            foreach (ListViewItem item in this.listViewItemSelection.SelectedItems)
            {
                PropertyStatisticsData data = item.Tag as PropertyStatisticsData;
                Dictionary<double, double> distribution = MathHelper.CalculateValueDistribution(data.Values, 128);
                PresentDataSeries(data.Name, GeneralHelper.EnumerableToArray(distribution.Values));
            }
            
        }

        private void comboBoxSelectedSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            MultipleItemStatisticsSet set = _itemStatisticsSets[comboBoxSelectedSet.SelectedIndex];
            listViewItems.Clear();

            for (int dataIndex = 0; dataIndex < set.PropertiesData.Count; dataIndex++)
			{
                PropertyStatisticsData data = set.PropertiesData[dataIndex];

                listViewItems.Columns.Add(data.Name);
                
                for (int i = 0; i < data.Values.Count; i++)
                {
                    ListViewItem item;
                    if (listViewItems.Items.Count < i + 1)
                    {// Create the item and make sure it has enough sub items for all the corresponding columns.
                        item = new ListViewItem(new string[set.PropertiesData.Count]);
                        listViewItems.Items.Add(item);
                    }
                    else
                    {
                        item = listViewItems.Items[i];
                    }

                    item.SubItems[dataIndex].Text = data.Values[i].ToString();
                }
            }

            if (listViewItems.Columns.Count > 0)
            {
                for (int i = 0; i < listViewItems.Columns.Count; i++)
                {
                    listViewItems.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
            }


        }

        private void listViewItemSelection_DoubleClick(object sender, EventArgs e)
        {
            buttonPresent_Click(sender, e);
        }



    }
}
