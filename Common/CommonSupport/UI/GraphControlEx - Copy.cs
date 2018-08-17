using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Dundas.Charting.WinControl;

namespace Support
{
    /// <summary>
    /// To do - on adding a new serie, resize all the others to comply.
    /// </summary>
    
    public partial class GraphControlEx : UserControl
    {
        const int MinimumZoom = 5;

        /// <summary>
        /// The border size to take in account, when placing the chart areas.
        /// In percentage.
        /// </summary>
        const int ChartAreasBorderSize = 2;

        public enum SeriesDataLineType
        {
            Point,
            FastPoint,
            Bubble,
            Line,
            Spline,
            StepLine,
            FastLine,
            Bar,
            StackedBar,
            StackedBar100,
            Column,
            StackedColumn,
            StackedColumn100,
            Area,
            SplineArea,
            StackedArea,
            StackedArea100,
            Pie,
            Doughnut,
            Stock,
            CandleStick,
            Range,
            SplineRange,
            Gantt,
            RangeColumn,
            Radar,
            Polar,
            ErrorBar,
            BoxPlot,
            Renko,
            ThreeLineBreak,
            Kagi,
            PointAndFigure,
            Funnel,
            Pyramid,

            Custom_FatLine,
            Default = FastLine,
        }

        string _labelFormatString = "00.000";
        public string LabelFormatString
        {
            get { return _labelFormatString; }
            set { _labelFormatString = value; }
        }

        public class SeriesData
        {
            public bool IsGraphical = false;
            public string SeriesName;
            public string ChartAreaName;
            public double[][] ValuesArrays;
        }

        Point? _rightMouseDownPoint = null;

        List<SeriesData> _seriesDatas = new List<SeriesData>();

        bool _graphicalSeriesShowLabels = true;
        public bool GraphicalSeriesShowLabels
        {
            get { return _graphicalSeriesShowLabels; }
            set { _graphicalSeriesShowLabels = value; }
        }

        bool _graphicaSeriesHideZeroes = true;
        public bool GraphicaSeriesHideZeroes
        {
            get { return _graphicaSeriesHideZeroes; }
            set { _graphicaSeriesHideZeroes = value; }
        }

        public int SeriesCount
        {
            get { return _seriesDatas.Count; }
        }

        public int StartIndex
        {
            get { return int.Parse(this.toolStripTextBoxStartIndex.Text); }
            set { toolStripTextBoxStartIndex.Text = value.ToString(); }
        }

        public int EndIndex
        {
            get { return int.Parse(this.toolStripTextBoxEndIndex.Text); }
            set { toolStripTextBoxEndIndex.Text = value.ToString(); }
        }

        public int SliceSize
        {
            get { return int.Parse(toolStripTextBoxSliceSize.Text); }
            set { toolStripTextBoxSliceSize.Text = value.ToString(); }
        }

        public int LongestDataSize
        {
            get
            {
                int max = 0;
                foreach (SeriesData data in _seriesDatas)
                {
                    foreach (double[] array in data.ValuesArrays)
                    {
                        max = Math.Max(max, array.Length);
                    }
                }

                toolStripTextBoxTotalDataSize.Text = max.ToString();
                return max;
            }
        }

        private void GraphControlEx_Load(object sender, EventArgs e)
        {
            this.chartMain.Series.Clear();
            SetupChartArea(MainChartArea);
        }

        protected ChartArea MainChartArea
        {
            get { return this.chartMain.ChartAreas[0]; }
        }

        protected Series GetSeriesByName(string seriesName)
        {
            foreach (Series series in this.chartMain.Series)
            {
                if (series.Name == seriesName)
                {
                    return series;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public GraphControlEx()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This is a little special. As there is a problem with having 0 as interval in the Dundas control,
        /// this will make sure the interval is proper and it is not zero.
        /// </summary>
        private void SetSafeChartAreaYAxisRangeIntervalForSeries(SeriesData seriesData, int startIndex, int endIndex)
        {
            double[][] valuesArrays = seriesData.ValuesArrays;

            double min = double.MaxValue;
            double max = double.MinValue;

            for (int k = 0; k < valuesArrays.Length; k++)
            {
                for (int i = startIndex; (i < endIndex || endIndex == 0) && i < valuesArrays[k].Length; i++)
                {// Cycle for the given range, as long as it is valid, and if the endIndex is zero, disregard it.
                    min = Math.Min(min, valuesArrays[k][i]);
                    max = Math.Max(max, valuesArrays[k][i]);
                }
            }

            // Set the chart area limits.
            ChartArea chartArea = GetChartAreaByName(seriesData.ChartAreaName);
            chartArea.AxisY.Minimum = Math.Min(chartArea.AxisY.Minimum, min);
            chartArea.AxisY.Maximum = Math.Max(chartArea.AxisY.Maximum, max);

            if (chartArea.AxisY.Minimum == chartArea.AxisY.Maximum)
            {// We need this otherwise the control goes crash if the interval is zero.
                chartArea.AxisY.Maximum = chartArea.AxisY.Minimum + 1;
            }

        }

        protected void UpdateCharts()
        {
            SliceSize = EndIndex - StartIndex + 1;

            UpdateDynamicChartAreasState();

            // Reset the chart are zooms.
            foreach (ChartArea chartArea in this.chartMain.ChartAreas)
            {
                if (chartArea.Visible == false)
                {// Chart area not visible, skip.
                    continue;
                }

                chartArea.CursorX.SelectionEnd = chartArea.CursorX.SelectionStart;

                chartArea.AxisY.Minimum = float.MaxValue;
                chartArea.AxisY.Maximum = float.MinValue;

                if (StartIndex != 0 || EndIndex != 0)
                {
                    chartArea.AxisX.Minimum = StartIndex;
                    chartArea.AxisX.Maximum = EndIndex;
                }
                else
                {
                    chartArea.AxisX.Minimum = 0;
                    chartArea.AxisX.Maximum = LongestDataSize;
                }
            }

            // Calculate division Count.
            int divisionCount = 1 + LongestDataSize / this.chartMain.Width;

            // Actual data binding here.
            foreach (SeriesData seriesData in this._seriesDatas)
            {
                Series series = GetSeriesByName(seriesData.SeriesName);

                if ( series == null || GetChartAreaByName(series.ChartArea).Visible == false)
                {// This series belongs to chart area that is not visible, skip.
                    continue;
                }

                if (StartIndex == 0 && EndIndex == 0)
                {// Top level mode.
                    //if (divisionCount == 1)
                    //{// Direct mode.
                    //    series.Points.DataBindY(seriesData.ValuesArrays);
                    //}
                    //else
                    {// Economical mode - less data will be shown than available.
                        int[] xValues = new int[seriesData.ValuesArrays[0].Length / divisionCount];
                        double[][] yValues = new double[seriesData.ValuesArrays.Length][];

                        for (int k = 0; k < seriesData.ValuesArrays.Length; k++)
                        {
                            yValues[k] = new double[seriesData.ValuesArrays[k].Length / divisionCount];

                            for (int i = 0; i < seriesData.ValuesArrays[k].Length / divisionCount; i++)
                            {
                                yValues[k][i] = seriesData.ValuesArrays[k][i * divisionCount];
                                xValues[i] = i * divisionCount;
                            }
                        }

                        if (xValues.Length > 0)
                        {
                            series.Points.DataBindXY(xValues, yValues);
                        }
                    }
                }
                else
                {// Range mode - direct dump.
                    divisionCount = 1;

                    int length = EndIndex - StartIndex;

                    int[] xValues = new int[length];
                    double[][] yValues = new double[seriesData.ValuesArrays.Length][];

                    for (int k = 0; k < seriesData.ValuesArrays.Length; k++)
                    {
                        yValues[k] = new double[length];
                        for (int i = StartIndex; i < EndIndex; i++)
                        {
                            xValues[i - StartIndex] = i;
                            if (i >= 0 && i < seriesData.ValuesArrays[k].Length)
                            {
                                yValues[k][i - StartIndex] = seriesData.ValuesArrays[k][i];
                            }
                        }
                    }

                    series.Points.DataBindXY(xValues, yValues);
                }
                
                // Set the proper Y axis range for the given series.
                SetSafeChartAreaYAxisRangeIntervalForSeries(seriesData, StartIndex, EndIndex);

                // Finally we need to do some additional handling in case this is graphical.
                if (seriesData.IsGraphical)
                {// Setup some stuff for the graphical series.
                    // Color specified by the pallete
                    // series.Color = Color.Olive;  
                    series.MarkerSize = 10;
                    series.ShowLabelAsValue = this.GraphicalSeriesShowLabels;
                    series["LabelStyle"] = "Top";
                    series.MarkerStyle = MarkerStyle.Diamond;

                    if (this.GraphicaSeriesHideZeroes)
                    {
                        // 0 -> empty point
                        for (int i = 0; i < series.Points.Count; i++)
                        {
                            if (series.Points[i].YValues[0] == 0)
                            {
                                if (i == 0)
                                {// There is a problem with DUNDAS, when it has all its points empty.
                                    // Read here, for more details : http://support.dundas.com/Default.aspx?article=783
                                    // To evade it, we shall make the first point invisible, rather than empty.
                                    series.Points[i].Color = Color.Transparent;
                                }
                                else
                                {
                                    series.Points[i].Empty = true;
                                }
                            }
                        }
                    }
                }

            }

        }

        protected void UpdateDynamicChartAreasState()
        {
            int visibleChartAreas = 0;

            for (int i = this.chartMain.ChartAreas.Count - 1; i >= 0; i--)
            {
                bool hasSeries = false;
                bool hasActiveSeries = false;
                string areaTitle = "";

                foreach (Series series in this.chartMain.Series)
                {
                    if (series.ChartArea == this.chartMain.ChartAreas[i].Name)
                    {
                        hasSeries = true;
                        if (series.Enabled)
                        {
                            hasActiveSeries = true;
                            areaTitle += series.Name + " ";
                        }
                    }
                }

                if (hasSeries == false && this.chartMain.ChartAreas[i] != MainChartArea)
                {// Remove
                    this.chartMain.ChartAreas.RemoveAt(i);
                }
                else if (hasActiveSeries == false)
                {// Hide.
                    this.chartMain.ChartAreas[i].Visible = false;
                }
                else
                {// Show.
                    visibleChartAreas++;
                    this.chartMain.ChartAreas[i].Visible = true;
                    this.chartMain.ChartAreas[i].AxisY.Title = areaTitle;
                }
            }

            if (visibleChartAreas == 0)
            {
                return;
            }

            int visibleArea = 0;
            // Manually set the positions of all the chart areas - automated layout is disabled.
            for (int j = 0; j < this.chartMain.ChartAreas.Count; j++)
            {
                if (this.chartMain.ChartAreas[j].Visible == false)
                {
                    continue;
                }

                float legendSize = 4 + this.chartMain.Legends[0].Position.Height;

                // Height and width are calculated in percentage.
                float heightPerArea = (100 - 2 * ChartAreasBorderSize - legendSize) / visibleChartAreas;
                //this.chartMain.ChartAreas[j].Position.Auto = false;
                this.chartMain.ChartAreas[j].Position.X = ChartAreasBorderSize;
                this.chartMain.ChartAreas[j].Position.Y = ChartAreasBorderSize + heightPerArea * visibleArea + legendSize;
                this.chartMain.ChartAreas[j].Position.Width = 100 - 2 * ChartAreasBorderSize;
                this.chartMain.ChartAreas[j].Position.Height = heightPerArea;
                visibleArea++;
            }
        }

        protected ChartArea GetChartAreaByName(string areaName)
        {
            foreach (ChartArea chartArea in this.chartMain.ChartAreas)
            {
                if (chartArea.Name == areaName)
                {
                    return chartArea;
                }
            }
            return null;
        }

        protected void SetupChartArea(ChartArea chartArea)
        {
            chartArea.AlignOrientation = AreaAlignOrientation.Vertical;
            chartArea.AlignType = AreaAlignType.Position;
            chartArea.AlignWithChartArea = "Default";
            chartArea.AxisX.StartFromZero = true;
            chartArea.AxisY.StartFromZero = true;
            // Format the AxisY label style so that only 4 digits are visible after the decimal point.
            chartArea.AxisY.LabelStyle.Format = LabelFormatString;
        }

        protected bool CreateNewChartArea(string areaName)
        {
            if (GetChartAreaByName(areaName) != null)
            {
                System.Diagnostics.Debug.Fail("Chart area with that name already exists.");
                return false;
            }

            ChartArea chartArea = this.chartMain.ChartAreas.Add(areaName);
            SetupChartArea(chartArea);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        protected void AddValuesToSeriesData(SeriesData currentSeriesData, double[][] valuesArrays)
        {
            // Copy over the data.
            for (int i = 0; i < valuesArrays.Length; i++)
            {
                double[] existingArray = currentSeriesData.ValuesArrays[i];
                double[] newArray = new double[existingArray.Length + valuesArrays[i].Length];

                existingArray.CopyTo(newArray, 0);
                valuesArrays[i].CopyTo(newArray, existingArray.Length);

                currentSeriesData.ValuesArrays[i] = newArray;
            }
        }

        /// <summary>
        /// Add some additional data to existing data series.
        /// Keep in mind that the other series will be filled with 0, to match the size of the currently extended one.
        /// </summary>
        public bool AddDataSeriesValues(string seiesName, double[][] valuesArrays, bool updateUI)
        {
            SeriesData currentSeriesData = this.GetSeriesDataByName(seiesName);
            if (currentSeriesData == null)
            {
                System.Diagnostics.Debug.Fail("Series was not found.");
                return false;
            }

            if (currentSeriesData.ValuesArrays.Length != valuesArrays.Length)
            {
                System.Diagnostics.Debug.Fail("Input values array is not of the proper size.");
                return false;
            }

            AddValuesToSeriesData(currentSeriesData, valuesArrays);

            // Fill in the other series with zeroes to keep the size in check.
            double[][] zeroFillValues = new double[valuesArrays.Length][];
            for (int i = 0; i < valuesArrays.Length; i++)
            {
                zeroFillValues[i] = new double[valuesArrays[i].Length];
            }

            foreach (SeriesData seriesData in _seriesDatas)
            {
                if (seriesData != currentSeriesData)
                {// Need to update this series as well - adding some empty values in the end.
                    AddValuesToSeriesData(seriesData, zeroFillValues);
                }
            }

            if (updateUI)
            {
                UpdateCharts();
            }

            return true;
        }


        /// <summary>
        /// Add graphical data series to default chart area.
        /// Pass NULL for the fixedSeriesTitle to use default.
        /// </summary>
        public bool AddGraphicalDataSeries(string seriesName, double[][] valuesArrays)
        {
            return AddGraphicalDataSeries(MainChartArea.Name, seriesName, valuesArrays);
        }

        /// <summary>
        /// Add graphical data series.
        /// Pass NULL for the fixedSeriesTitle to use default.
        /// </summary>
        public bool AddGraphicalDataSeries(string areaName, string seriesName, double[][] valuesArrays)
        {
            return AddDataSeries(areaName, seriesName, SeriesDataLineType.Point, valuesArrays, true);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool AddReplaceDataSeries(string seriesName, SeriesDataLineType lineType, double[][] valuesArrays)
        {
            if (GetSeriesDataByName(seriesName) != null)
            {
                this.RemoveDataSeries(seriesName);
            }
            return AddDataSeries(MainChartArea.Name, seriesName, lineType, valuesArrays, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AddReplaceDataSeries(string areaName, string seriesName, SeriesDataLineType lineType, double[][] valuesArrays)
        {
            if (GetSeriesDataByName(seriesName) != null)
            {
                this.RemoveDataSeries(seriesName);
            }
            return AddDataSeries(areaName, seriesName, lineType, valuesArrays, false);
        }

        /// <summary>
        /// Add typical the data series to the default area.
        /// </summary>
        public bool AddDataSeries(string seiesName, SeriesDataLineType lineType, double[][] valuesArrays)
        {
            return AddDataSeries(MainChartArea.Name, seiesName, lineType, valuesArrays, false);
        }

        /// <summary>
        /// Add typical the data series to specified area.
        /// </summary>
        public bool AddDataSeries(string areaName, string seriesName, SeriesDataLineType lineType, double[][] valuesArrays)
        {
            return AddDataSeries(areaName, seriesName, lineType, valuesArrays, false);
        }

        /// <summary>
        /// If no area with this name exists, a new one will be created.
        /// Keep in mind that all data series must have equal sizes, otherwise problems occur on having zoom.
        /// </summary>
        bool AddDataSeries(string areaName, string seriesName, SeriesDataLineType lineType, 
            double[][] valuesArrays, bool graphicalSeries)
        {
            if (GetSeriesDataByName(seriesName) != null)
            {
                System.Diagnostics.Debug.Fail("Data series already exists.");
                return false;
            }

            // We need to make sure all the sizes are equal.
            foreach (double[] doubleArray in valuesArrays)
            {
                int longestDataSize = this.LongestDataSize;
                if (doubleArray.Length != longestDataSize && longestDataSize != 0)
                {// We have a problem, deny adding this series.
                    System.Diagnostics.Debug.Fail("Data series size is wrong.");
                    return false;
                }
            }

            ChartArea chartArea = GetChartAreaByName(areaName);
            if (chartArea == null)
            {
                CreateNewChartArea(areaName);
                chartArea = GetChartAreaByName(areaName);
            }

            chartArea.AxisX.Margin = false;

            //chartArea.AxisX.Minimum = 50;

            SeriesData seriesData = new SeriesData();
            seriesData.IsGraphical = graphicalSeries;
            seriesData.ValuesArrays = valuesArrays;
            seriesData.SeriesName = seriesName;
            seriesData.ChartAreaName = areaName;
            _seriesDatas.Add(seriesData);

            {
                Series series = this.chartMain.Series.Add(seriesName);
                series.ChartArea = areaName;

                if (lineType == SeriesDataLineType.Custom_FatLine)
                {
                    series.BorderWidth = 3;
                    series.ChartType = "Line";
                }
                else
                {
                    series.ChartType = lineType.ToString();
                }

    
                series.Name = seriesName;
                series.ShowInLegend = false;
                series.ShadowOffset = 0;

                // We need to call this in order to have the colors properly applied to this new series.
                this.chartMain.ApplyPaletteColors();
                
                CreateSeriesLegendItem(series);
            }

            UpdateCharts();

            return true;
        }

        void CreateSeriesLegendItem(Series series)
        {
            // Set the custom legend item.
            int index = this.chartMain.Legends[0].CustomItems.Add(series.Color, series.Name + " [On]");
            this.chartMain.Legends[0].CustomItems[index].BorderColor = Color.Transparent;

            this.chartMain.Legends[0].CustomItems[index].MarkerBorderWidth = 3;
            this.chartMain.Legends[0].CustomItems[index].Color = series.Color;
            this.chartMain.Legends[0].CustomItems[index].Tag = series;
            this.chartMain.Legends[0].CustomItems[index].Style = LegendImageStyle.Rectangle;
        }

        void RemoveSeriesLegendItem(Series series)
        {
            for (int i = this.chartMain.Legends[0].CustomItems.Count - 1; i >= 0; i--)
            {
                if (this.chartMain.Legends[0].CustomItems[i].Tag == series)
                {
                    this.chartMain.Legends[0].CustomItems.RemoveAt(i);
                }
            }
        }

        public void RemoveDataSeries(string seriesName)
        {
            // Remove series from chart.
            for (int i = this.chartMain.Series.Count-1; i >= 0; i--)
            {
                if (this.chartMain.Series[i].Name == seriesName)
                {
                    RemoveSeriesLegendItem(this.chartMain.Series[i]);
                    this.chartMain.Series.RemoveAt(i);
                }
            }

            // Remove series from data values array.
            for (int i = _seriesDatas.Count - 1; i >= 0; i--)
            {
                if (this._seriesDatas[i].SeriesName == seriesName)
                {
                    this._seriesDatas.RemoveAt(i);
                }
            }

            // Clean up all chart areas that have no series in them.
            UpdateDynamicChartAreasState();
        }

        public SeriesData GetSeriesDataByName(string seriesName)
        {
            foreach (SeriesData seriesData in _seriesDatas)
            {
                if (seriesData.SeriesName == seriesName)
                {
                    return seriesData;
                }
            }
            return null;
        }

        public void Clear()
        {
            this.chartMain.Series.Clear();
            _seriesDatas.Clear();

            MainChartArea.AxisX.Minimum = 0;
            MainChartArea.AxisX.Maximum = 0;

            StartIndex = 0;
            EndIndex = 0;
            SliceSize = 0;

            this.chartMain.Legends[0].CustomItems.Clear();

            for (int i = this.chartMain.ChartAreas.Count - 1; i >= 0; i--)
            {
                if (this.chartMain.ChartAreas[i] != MainChartArea)
                {
                    this.chartMain.ChartAreas.RemoveAt(i);
                }
            }
        }


        private void ShowDataRange(int selectionStart, int selectionEnd)
        {
            StartIndex = (int)selectionStart;
            EndIndex = (int)selectionEnd;

            if (StartIndex > EndIndex)
            {// Swap.
                int tmp = StartIndex;
                StartIndex = EndIndex;
                EndIndex = tmp;
            }

            if (StartIndex < 0 || EndIndex < 0)
            {
                StartIndex = 0;
                EndIndex = SliceSize - 1;
            }

            if (EndIndex >= LongestDataSize)
            {
                //int actualDifference = EndIndex - StartIndex + 1;
                //StartIndex = LongestDataSize - actualDifference;
                EndIndex = LongestDataSize - 1;
            }

            UpdateCharts();
        }

        private void chartMain_SelectionRangeChanged(object sender, CursorEventArgs e)
        {
            if ((e.NewSelectionStart > 0 && Math.Abs(e.NewSelectionEnd - e.NewSelectionStart) > MinimumZoom) 
                || (e.NewSelectionStart == 0 && e.NewSelectionEnd == 0))
            {
                ShowDataRange((int)e.NewSelectionStart, (int)e.NewSelectionEnd);
            }
        }

        private void toolStripMenuItemNext_Click(object sender, EventArgs e)
        {
            ShowDataRange(StartIndex + SliceSize, StartIndex + 2 * SliceSize);
        }

        private void toolStripMenuItemPrevious_Click(object sender, EventArgs e)
        {
            ShowDataRange(StartIndex - SliceSize, StartIndex);
        }

        private void toolStripMenuItemZoomRestore_Click(object sender, EventArgs e)
        {
            ShowDataRange(0, 0);
        }

        private void addToolStripZoomOut_Click(object sender, EventArgs e)
        {
            int initialSliceSize = SliceSize;
            SliceSize = SliceSize * 2;
            ShowDataRange(StartIndex - initialSliceSize, EndIndex + initialSliceSize);
        }

        private void toolStripMenuItemClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void SetSeriesEnabled(string seriesName, bool enabled)
        {
            Series series = GetSeriesByName(seriesName);
            series.Enabled = enabled;

            // Set the custom legend item to match the state of the series.
            foreach(LegendItem legendItem in this.chartMain.Legends[0].CustomItems)
            {
                if (legendItem.Tag == series)
                {
                    SynchronizeLegendItemStateWithSeries(legendItem);
                }
            }

            UpdateDynamicChartAreasState();
        }

        void SynchronizeLegendItemStateWithSeries(LegendItem legendItem)
        {
            Series series = (Series)legendItem.Tag;

            if (series.Enabled)
            {
                legendItem.Name = series.Name + " [On]";

                // Call this to have the proper color in Series Color.
                this.chartMain.ApplyPaletteColors();
                legendItem.Color = series.Color;

                UpdateDynamicChartAreasState();
            }
            else
            {
                legendItem.Name = series.Name + " [Off]";
                legendItem.Color = Color.FromArgb(50, Color.Black);
            }
        }
        
        private void chartMain_MouseDown(object sender, MouseEventArgs e)
        {
            // Also update the status text.
            this.toolStripMenuItemStatusText.Text = "X:" + MainChartArea.CursorX.Position + " Y:" + MainChartArea.CursorY.Position + " MinimumZoom:" + MinimumZoom;

            // We need to manually update all the cursor positions of all the visible chart areas, 
            // as the auto mode is broken due to the manual positioning of chart areas.
            for (int i = 0; i < this.chartMain.ChartAreas.Count; i++)
            {
                if (this.chartMain.ChartAreas[i].Visible)
                {
                    this.chartMain.ChartAreas[i].CursorX.Position = MainChartArea.CursorX.Position;
                }
            }

            HitTestResult result = chartMain.HitTest(e.X, e.Y);
            if (result != null && result.Object != null)
            {
                // When user hits the LegendItem
                if (result.Object is LegendItem)
                {
                    LegendItem legendItem = (LegendItem)result.Object;
                    Series series = (Series)legendItem.Tag;
                    series.Enabled = !series.Enabled;

                    if (e.Button == MouseButtons.Right)
                    {// Delete
                        //System.Diagnostics.Debug.Fail("UnImplemented.");
                        //if (CustomRemoveEnabled && MessageBox.Show("Are you sure you want to delete this item?", "Delete Item", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        //{
                        //    DeleteSeries(series.Name);
                        //}
                    }
                    else
                    {// Enable<->Disable
                        SynchronizeLegendItemStateWithSeries(legendItem);
                        UpdateDynamicChartAreasState();
                        UpdateCharts();
                    }
                }
                else
                {// Did not hit a legent item.
                    if (e.Button == MouseButtons.Right)
                    {// In this case just do a drag.
                        _rightMouseDownPoint = new Point(e.X, e.Y);
                        this.chartMain.Cursor = Cursors.Hand;
                    }
                }
            }

        }

        private void chartMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (StartIndex >= 0 && EndIndex > 0 && SliceSize > 0 && _rightMouseDownPoint != null)
                {
                    Point position = new Point(e.X, e.Y);
                    int difference = _rightMouseDownPoint.Value.X - e.X;
                    // Calculate how much pixels is a single unit.
                    double chartAreaWidth = this.chartMain.Width * MainChartArea.Position.Width / 112f;
                    double unitsPerPixel = chartAreaWidth / SliceSize;
                    double actualDifference = difference / unitsPerPixel;
                    ShowDataRange(StartIndex + (int)actualDifference, EndIndex + (int)actualDifference);
                }

                this.chartMain.Cursor = Cursors.Default;
                _rightMouseDownPoint = null;
            }
        }

        private void chartMain_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItemUpdate_Click(object sender, EventArgs e)
        {
            UpdateDynamicChartAreasState();
        }


    }
}

