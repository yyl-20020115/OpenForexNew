using Support.UI;
namespace Support
{
    partial class GraphControlEx
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphControlEx));
            Dundas.Charting.WinControl.LineAnnotation lineAnnotation1 = new Dundas.Charting.WinControl.LineAnnotation();
            Dundas.Charting.WinControl.ChartArea chartArea1 = new Dundas.Charting.WinControl.ChartArea();
            Dundas.Charting.WinControl.Legend legend1 = new Dundas.Charting.WinControl.Legend();
            Dundas.Charting.WinControl.Series series1 = new Dundas.Charting.WinControl.Series();
            Dundas.Charting.WinControl.Series series2 = new Dundas.Charting.WinControl.Series();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoomRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPrevious = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemNext = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxStartIndex = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxEndIndex = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxSliceSize = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItemTotalDataSize = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxTotalDataSize = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripMenuItemStatusText = new System.Windows.Forms.ToolStripMenuItem();
            this.chartMain = new Support.UI.ChartCustom();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartMain)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemUpdate,
            this.toolStripMenuItem3,
            this.toolStripMenuItemClear,
            this.toolStripMenuItemZoomRestore,
            this.addToolStripZoomOut,
            this.toolStripMenuItemPrevious,
            this.toolStripMenuItemNext,
            this.toolStripMenuItem1,
            this.toolStripTextBoxStartIndex,
            this.toolStripMenuItem2,
            this.toolStripTextBoxEndIndex,
            this.toolStripMenuItem5,
            this.toolStripTextBoxSliceSize,
            this.toolStripMenuItemTotalDataSize,
            this.toolStripTextBoxTotalDataSize,
            this.toolStripMenuItemStatusText});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 25);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // toolStripMenuItemUpdate
            // 
            this.toolStripMenuItemUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripMenuItemUpdate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemUpdate.Image")));
            this.toolStripMenuItemUpdate.Name = "toolStripMenuItemUpdate";
            this.toolStripMenuItemUpdate.Size = new System.Drawing.Size(28, 21);
            this.toolStripMenuItemUpdate.Text = "toolStripMenuItem4";
            this.toolStripMenuItemUpdate.Click += new System.EventHandler(this.toolStripMenuItemUpdate_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Enabled = false;
            this.toolStripMenuItem3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem3.Image")));
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(79, 21);
            this.toolStripMenuItem3.Text = "Add Item";
            // 
            // toolStripMenuItemClear
            // 
            this.toolStripMenuItemClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemClear.Image")));
            this.toolStripMenuItemClear.Name = "toolStripMenuItemClear";
            this.toolStripMenuItemClear.Size = new System.Drawing.Size(60, 21);
            this.toolStripMenuItemClear.Text = "Clear";
            this.toolStripMenuItemClear.Click += new System.EventHandler(this.toolStripMenuItemClear_Click);
            // 
            // toolStripMenuItemZoomRestore
            // 
            this.toolStripMenuItemZoomRestore.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemZoomRestore.Image")));
            this.toolStripMenuItemZoomRestore.Name = "toolStripMenuItemZoomRestore";
            this.toolStripMenuItemZoomRestore.Size = new System.Drawing.Size(74, 21);
            this.toolStripMenuItemZoomRestore.Text = "Zoom all";
            this.toolStripMenuItemZoomRestore.Click += new System.EventHandler(this.toolStripMenuItemZoomRestore_Click);
            // 
            // addToolStripZoomOut
            // 
            this.addToolStripZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("addToolStripZoomOut.Image")));
            this.addToolStripZoomOut.Name = "addToolStripZoomOut";
            this.addToolStripZoomOut.Size = new System.Drawing.Size(80, 21);
            this.addToolStripZoomOut.Text = "Zoom out";
            this.addToolStripZoomOut.Click += new System.EventHandler(this.addToolStripZoomOut_Click);
            // 
            // toolStripMenuItemPrevious
            // 
            this.toolStripMenuItemPrevious.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemPrevious.Image")));
            this.toolStripMenuItemPrevious.Name = "toolStripMenuItemPrevious";
            this.toolStripMenuItemPrevious.Size = new System.Drawing.Size(76, 21);
            this.toolStripMenuItemPrevious.Text = "Previous";
            this.toolStripMenuItemPrevious.Click += new System.EventHandler(this.toolStripMenuItemPrevious_Click);
            // 
            // toolStripMenuItemNext
            // 
            this.toolStripMenuItemNext.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemNext.Image")));
            this.toolStripMenuItemNext.Name = "toolStripMenuItemNext";
            this.toolStripMenuItemNext.Size = new System.Drawing.Size(58, 21);
            this.toolStripMenuItemNext.Text = "Next";
            this.toolStripMenuItemNext.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolStripMenuItemNext.Click += new System.EventHandler(this.toolStripMenuItemNext_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Enabled = false;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(72, 21);
            this.toolStripMenuItem1.Text = "Start index";
            // 
            // toolStripTextBoxStartIndex
            // 
            this.toolStripTextBoxStartIndex.MaxLength = 32567;
            this.toolStripTextBoxStartIndex.Name = "toolStripTextBoxStartIndex";
            this.toolStripTextBoxStartIndex.ReadOnly = true;
            this.toolStripTextBoxStartIndex.Size = new System.Drawing.Size(45, 21);
            this.toolStripTextBoxStartIndex.Text = "0";
            this.toolStripTextBoxStartIndex.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Enabled = false;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(66, 21);
            this.toolStripMenuItem2.Text = "End index";
            // 
            // toolStripTextBoxEndIndex
            // 
            this.toolStripTextBoxEndIndex.MaxLength = 32567;
            this.toolStripTextBoxEndIndex.Name = "toolStripTextBoxEndIndex";
            this.toolStripTextBoxEndIndex.ReadOnly = true;
            this.toolStripTextBoxEndIndex.Size = new System.Drawing.Size(45, 21);
            this.toolStripTextBoxEndIndex.Text = "0";
            this.toolStripTextBoxEndIndex.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Enabled = false;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(61, 21);
            this.toolStripMenuItem5.Text = "Slice size";
            // 
            // toolStripTextBoxSliceSize
            // 
            this.toolStripTextBoxSliceSize.MaxLength = 32567;
            this.toolStripTextBoxSliceSize.Name = "toolStripTextBoxSliceSize";
            this.toolStripTextBoxSliceSize.ReadOnly = true;
            this.toolStripTextBoxSliceSize.Size = new System.Drawing.Size(45, 21);
            this.toolStripTextBoxSliceSize.Text = "0";
            // 
            // toolStripMenuItemTotalDataSize
            // 
            this.toolStripMenuItemTotalDataSize.Enabled = false;
            this.toolStripMenuItemTotalDataSize.Name = "toolStripMenuItemTotalDataSize";
            this.toolStripMenuItemTotalDataSize.Size = new System.Drawing.Size(89, 21);
            this.toolStripMenuItemTotalDataSize.Text = "Total data size";
            // 
            // toolStripTextBoxTotalDataSize
            // 
            this.toolStripTextBoxTotalDataSize.Name = "toolStripTextBoxTotalDataSize";
            this.toolStripTextBoxTotalDataSize.ReadOnly = true;
            this.toolStripTextBoxTotalDataSize.Size = new System.Drawing.Size(45, 21);
            this.toolStripTextBoxTotalDataSize.Text = "0";
            // 
            // toolStripMenuItemStatusText
            // 
            this.toolStripMenuItemStatusText.Enabled = false;
            this.toolStripMenuItemStatusText.Name = "toolStripMenuItemStatusText";
            this.toolStripMenuItemStatusText.Size = new System.Drawing.Size(103, 21);
            this.toolStripMenuItemStatusText.Text = "Minimum zoom : 5";
            // 
            // chartMain
            // 
            lineAnnotation1.ClipToChartArea = "Default";
            lineAnnotation1.EndCap = Dundas.Charting.WinControl.LineAnchorCap.Arrow;
            lineAnnotation1.LineWidth = 4;
            lineAnnotation1.Name = "Line1";
            lineAnnotation1.StartCap = Dundas.Charting.WinControl.LineAnchorCap.Arrow;
            this.chartMain.Annotations.Add(lineAnnotation1);
            chartArea1.AxisX.LabelStyle.Interval = 0;
            chartArea1.AxisX.LabelStyle.IntervalOffset = 0;
            chartArea1.AxisX.LabelStyle.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX.LabelStyle.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.Interval = 0;
            chartArea1.AxisX.MajorGrid.IntervalOffset = 0;
            chartArea1.AxisX.MajorGrid.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorTickMark.Interval = 0;
            chartArea1.AxisX.MajorTickMark.IntervalOffset = 0;
            chartArea1.AxisX.MajorTickMark.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorTickMark.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX.View.Zoomable = false;
            chartArea1.AxisX2.LabelStyle.Interval = 0;
            chartArea1.AxisX2.LabelStyle.IntervalOffset = 0;
            chartArea1.AxisX2.LabelStyle.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.LabelStyle.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.MajorGrid.Interval = 0;
            chartArea1.AxisX2.MajorGrid.IntervalOffset = 0;
            chartArea1.AxisX2.MajorGrid.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.MajorGrid.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.MajorTickMark.Interval = 0;
            chartArea1.AxisX2.MajorTickMark.IntervalOffset = 0;
            chartArea1.AxisX2.MajorTickMark.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.MajorTickMark.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY.LabelStyle.Format = "#.####";
            chartArea1.AxisY.LabelStyle.Interval = 0;
            chartArea1.AxisY.LabelStyle.IntervalOffset = 0;
            chartArea1.AxisY.LabelStyle.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY.LabelStyle.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY.MajorGrid.Interval = 0;
            chartArea1.AxisY.MajorGrid.IntervalOffset = 0;
            chartArea1.AxisY.MajorGrid.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY.MajorGrid.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY.MajorTickMark.Interval = 0;
            chartArea1.AxisY.MajorTickMark.IntervalOffset = 0;
            chartArea1.AxisY.MajorTickMark.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY.MajorTickMark.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.LabelStyle.Interval = 0;
            chartArea1.AxisY2.LabelStyle.IntervalOffset = 0;
            chartArea1.AxisY2.LabelStyle.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.LabelStyle.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.MajorGrid.Interval = 0;
            chartArea1.AxisY2.MajorGrid.IntervalOffset = 0;
            chartArea1.AxisY2.MajorGrid.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.MajorGrid.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.MajorTickMark.Interval = 0;
            chartArea1.AxisY2.MajorTickMark.IntervalOffset = 0;
            chartArea1.AxisY2.MajorTickMark.IntervalOffsetType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.MajorTickMark.IntervalType = Dundas.Charting.WinControl.DateTimeIntervalType.Auto;
            chartArea1.CursorX.LineStyle = Dundas.Charting.WinControl.ChartDashStyle.Dash;
            chartArea1.CursorX.UserEnabled = true;
            chartArea1.CursorX.UserSelection = true;
            chartArea1.CursorY.LineStyle = Dundas.Charting.WinControl.ChartDashStyle.Dash;
            chartArea1.CursorY.UserEnabled = true;
            chartArea1.Name = "Default";
            this.chartMain.ChartAreas.Add(chartArea1);
            this.chartMain.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            legend1.Docking = Dundas.Charting.WinControl.LegendDocking.Top;
            legend1.MaxAutoSize = 80F;
            legend1.Name = "Default";
            legend1.TableStyle = Dundas.Charting.WinControl.LegendTableStyle.Wide;
            this.chartMain.Legends.Add(legend1);
            this.chartMain.Location = new System.Drawing.Point(0, 25);
            this.chartMain.Name = "chartMain";
            this.chartMain.Palette = Dundas.Charting.WinControl.ChartColorPalette.Pastel;
            series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            series1.ChartType = "Spline";
            series1.Name = "Series1";
            series2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            series2.ChartType = "Line";
            series2.Name = "Series2";
            this.chartMain.Series.Add(series1);
            this.chartMain.Series.Add(series2);
            this.chartMain.Size = new System.Drawing.Size(800, 575);
            this.chartMain.TabIndex = 4;
            this.chartMain.Text = "chart1";
            this.chartMain.TextAntiAliasingQuality = Dundas.Charting.WinControl.TextAntiAliasingQuality.Normal;
            this.chartMain.SelectionRangeChanged += new Dundas.Charting.WinControl.Chart.CursorEventHandler(this.chartMain_SelectionRangeChanged);
            this.chartMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chartMain_MouseUp);
            this.chartMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chartMain_MouseDown);
            this.chartMain.Click += new System.EventHandler(this.chartMain_Click);
            // 
            // GraphControlEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chartMain);
            this.Controls.Add(this.menuStrip);
            this.Name = "GraphControlEx";
            this.Size = new System.Drawing.Size(800, 600);
            this.Load += new System.EventHandler(this.GraphControlEx_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripZoomOut;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxStartIndex;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxEndIndex;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPrevious;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNext;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxSliceSize;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTotalDataSize;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxTotalDataSize;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClear;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoomRestore;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStatusText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUpdate;
        private ChartCustom chartMain;
    }
}
