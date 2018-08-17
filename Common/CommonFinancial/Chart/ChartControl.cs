using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Chart control is the main usable control for charting. It wraps the functionality of one
    /// or more chart panes together and exposes additional controlling UI.
    /// </summary>
    public partial class ChartControl : UserControl
    {
        List<ChartPane> _panes = new List<ChartPane>();

        /// <summary>
        /// All panes, including the Main Pane.
        /// </summary>
        public ReadOnlyCollection<ChartPane> Panes
        {
            get { return _panes.AsReadOnly(); }
        }

        /// <summary>
        /// The main (master) pane.
        /// </summary>
        public MasterChartPane MasterPane
        {
            get { lock (this) { return (MasterChartPane)_panes[0]; } }
        }

        protected Color DrawingColor
        {
            get { return toolStripButtonColor.BackColor; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ChartControl()
        {
            InitializeComponent();

            _panes.Add(masterPane);

            masterPane.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            masterPane.Name = "Master Pane";
            masterPane.DrawingSpaceViewTransformationChangedEvent += new ChartPane.DrawingSpaceViewTransformationChangedDelegate(graphicPane_DrawingSpaceViewTransformationChangedEvent);
            masterPane.DrawingSpaceUpdatedEvent += new ChartPane.DrawingSpaceUpdatedDelegate(graphicPane_DrawingSpaceUpdatedEvent);

            masterPane.CustomObjectsManager.DynamicObjectBuiltEvent += new CustomObjectsManager.DynamicObjectBuiltDelegate(CustomObjectsManager_DynamicObjectBuiltEvent);
            masterPane.ParametersUpdatedEvent += new ChartPane.ParametersUpdatedDelegate(masterPane_ParametersUpdatedEvent);

        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveState(SerializationInfoEx info)
        {
            lock (this)
            {
                info.AddValue("Name", this.Name);
                info.AddValue("hScrollBar.Visible", this.hScrollBar.Visible);
                info.AddValue("vScrollBar.Visible", this.vScrollBar.Visible);

                List<SerializationInfoEx> infos = new List<SerializationInfoEx>();
                foreach (ChartPane pane in _panes)
                {
                    SerializationInfoEx paneInfo = new SerializationInfoEx();
                    pane.SaveState(paneInfo, true);
                    infos.Add(paneInfo);
                }

                info.AddValue("panesStates", infos);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RestoreState(SerializationInfoEx info)
        {
            Clear();

            lock (this)
            {
                foreach (ChartPane pane in GeneralHelper.EnumerableToList<ChartPane>(_panes))
                {
                    if (pane is SlaveChartPane)
                    {
                        this.RemoveSlavePane((SlaveChartPane)pane);
                    }
                }

                this.Name = info.GetString("Name");

                this.hScrollBar.Visible = info.GetBoolean("hScrollBar.Visible");
                this.vScrollBar.Visible = info.GetBoolean("vScrollBar.Visible");
                toolStripButtonShowScrollbars.Checked = this.hScrollBar.Visible;

                List<SerializationInfoEx> infos = info.GetValue<List<SerializationInfoEx>>("panesStates");
                for (int i = 0; i < infos.Count; i++)
                {
                    if (i == 0)
                    {
                        MasterPane.RestoreState(infos[0], true);
                    }
                    else
                    {
                        SlaveChartPane pane = CreateSlavePane("", SlaveChartPane.MasterPaneSynchronizationModeEnum.XAxis, 100);
                        pane.RestoreState(infos[i], true);
                    }
                }
            }
        }

        public void Clear()
        {
            foreach (ChartPane pane in _panes)
            {
                pane.Clear(true, true);
            }
        }

        public SlaveChartPane CreateSlavePane(string chartName, SlaveChartPane.MasterPaneSynchronizationModeEnum masterSynchronizationMode, int height)
        {
            SlaveChartPane pane = new SlaveChartPane();
            _panes.Add(pane);
            pane.Name = "Slave Pane[" + chartName + "]" ;
            pane.ChartName = chartName;

            pane.Dock = DockStyle.Bottom;
            pane.MasterPaneSynchronizationMode = masterSynchronizationMode;

            pane.XAxisLabelsFontBrush = null;

            Splitter splitter = new Splitter();
            splitter.Height = 4;
            splitter.Dock = DockStyle.Bottom;
            this.Controls.Add(splitter);
            splitter.SendToBack();

            pane.Tag = splitter;

            this.Controls.Add(pane);
            pane.SendToBack();
            pane.Height = height;
            pane.MasterPane = MasterPane;

            hScrollBar.SendToBack();
            vScrollBar.SendToBack();

            this.toolStripDynamicObjects.SendToBack();
            this.toolStripMain.SendToBack();

            return pane;
        }

        void masterPane_ParametersUpdatedEvent(ChartPane pane)
        {
            if (this.IsHandleCreated == false)
            {
                return;
            }
            UpdateMasterPaneToolbar();
        }

        void CheckToolStripDropDownButtonItem(ToolStripDropDownItem button, int index)
        {
            for (int i = 0; i < button.DropDownItems.Count; i++)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)button.DropDownItems[i];
                item.CheckState = CheckState.Unchecked;
                if (i == index)
                {
                    item.CheckState = CheckState.Checked;
                }
            }
        }

        void UpdateMasterPaneToolbar()
        {
            CheckToolStripDropDownButtonItem(toolStripDropDownButtonSelectionMode, (int)masterPane.RightMouseButtonSelectionMode);
            CheckToolStripDropDownButtonItem(toolStripDropDownButtonScrollMode, (int)masterPane.ScrollMode);

            CheckToolStripDropDownButtonItem(toolStripMenuItemAppearanceColorScheme, (int)masterPane.AppearanceScheme);
            CheckToolStripDropDownButtonItem(toolStripMenuItemAppearanceYAxisLabelPosition, (int)masterPane.YAxisLabelsPosition);

            toolStripButtonCrosshair.Checked = masterPane.CrosshairVisible;
            toolStripButtonShowLabels.Checked = masterPane.ShowSeriesLabels;
            toolStripButtonLimitView.Checked = masterPane.LimitedView;
            toolStripButtonAutoScrollToEnd.Checked = masterPane.AutoScrollToEnd;
        }


        void CustomObjectsManager_DynamicObjectBuiltEvent(CustomObjectsManager manager, DynamicCustomObject dynamicObject)
        {
            foreach (ToolStripItem item in toolStripDynamicObjects.Items)
            {
                if (item is ToolStripButton)
                {
                    ((ToolStripButton)item).Checked = false;
                }
            }
        }

        /// <summary>
        /// This will not remove the main pane, since it is mandatory.
        /// </summary>
        public bool RemoveSlavePane(SlaveChartPane pane)
        {
            if (_panes.Remove(pane))
            {
                pane.MasterPane = null;
                pane.Clear(true, true);
                
                this.Controls.Remove((Splitter)pane.Tag);
                pane.Tag = null;

                pane.Parent = null;

                return true;
            }
            return false;
        }

        public ChartPane GetPaneByName(string paneName)
        {
            foreach (ChartPane pane in _panes)
            {
                if (pane.Name == paneName)
                {
                    return pane;
                }
            }

            return null;
        }

        private void GraphicControl_Load(object sender, EventArgs e)
        {
            MasterPane.FitDrawingSpaceToScreen(false, true);
            //MainPane.AdditionalDrawingSpaceAreaMarginLeft = 24;

            masterPane.SetAppearanceScheme(masterPane.AppearanceScheme);

            toolStripDropDownButtonSelectionMode.DropDownItems.Clear();
            foreach (string name in Enum.GetNames(typeof(ChartPane.SelectionModeEnum)))
            {
                toolStripDropDownButtonSelectionMode.DropDownItems.Add(GeneralHelper.SeparateCapitalLetters(name));
            }

            // Appearance menu
            toolStripMenuItemAppearanceColorScheme.DropDownItemClicked += new ToolStripItemClickedEventHandler(toolStripDropDownButtonAppearanceScheme_DropDownItemClicked);
            foreach (string name in Enum.GetNames(typeof(ChartPane.AppearanceSchemeEnum)))
            {
                toolStripMenuItemAppearanceColorScheme.DropDownItems.Add(GeneralHelper.SeparateCapitalLetters(name));
            }

            toolStripMenuItemAppearanceYAxisLabelPosition.DropDownItemClicked += new ToolStripItemClickedEventHandler(toolStripMenuItemAppearanceYAxisLabelPosition_DropDownItemClicked);
            foreach(string name in Enum.GetNames(typeof(ChartPane.YAxisLabelPosition)))
            {
                toolStripMenuItemAppearanceYAxisLabelPosition.DropDownItems.Add(GeneralHelper.SeparateCapitalLetters(name));
            }
            
            // 
            toolStripDropDownButtonScrollMode.DropDownItems.Clear();
            foreach (string name in Enum.GetNames(typeof(ChartPane.ScrollModeEnum)))
            {
                toolStripDropDownButtonScrollMode.DropDownItems.Add(GeneralHelper.SeparateCapitalLetters(name));
            }

            UpdateMasterPaneToolbar();
        }

        private void toolStripButtonFitVertical_Click(object sender, EventArgs e)
        {
            MasterPane.FitDrawingSpaceToScreen(false, true, MasterPane.DrawingSpaceDisplayLimit);
            MasterPane.Invalidate();
        }

        private void toolStripButtonFitHorizontal_Click(object sender, EventArgs e)
        {
            MasterPane.FitDrawingSpaceToScreen(true, false);
            MasterPane.Invalidate();
        }

        //private void toolStripComboBoxRightMouseButtonSelectionMode_SelectedIndexChanged(object sender, EventArgs e)
        //{
            //MainPane.RightMouseButtonSelectionMode = (ChartPane.SelectionModeEnum)Enum.Parse(typeof(ChartPane.SelectionModeEnum), toolStripComboBoxRightMouseButtonSelectionMode.SelectedItem.ToString());
        //}

        private void toolStripDropDownButtonSelectionMode_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)e.ClickedItem;
            MasterPane.RightMouseButtonSelectionMode = (ChartPane.SelectionModeEnum)Enum.Parse(typeof(ChartPane.SelectionModeEnum), item.Text.Replace(" ", string.Empty));
            CheckToolStripDropDownButtonItem(toolStripDropDownButtonSelectionMode, (int)MasterPane.RightMouseButtonSelectionMode);
        }

        private void toolStripButtonFit_Click(object sender, EventArgs e)
        {
            MasterPane.FitDrawingSpaceToScreen(true, true);
            MasterPane.Invalidate();
        }

        private void toolStripButtonShowLabels_Click(object sender, EventArgs e)
        {
            MasterPane.ShowSeriesLabels = toolStripButtonShowLabels.Checked;
            MasterPane.Invalidate();
        }

        void toolStripMenuItemAppearanceYAxisLabelPosition_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)e.ClickedItem;
            ChartPane.YAxisLabelPosition positionMode = (ChartPane.YAxisLabelPosition)Enum.Parse(typeof(ChartPane.YAxisLabelPosition), item.Text.Replace(" ", string.Empty));

            MasterPane.YAxisLabelsPosition = positionMode;
            MasterPane.Invalidate();
        }

        private void toolStripDropDownButtonAppearanceScheme_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)e.ClickedItem;
            ChartPane.AppearanceSchemeEnum scheme = (ChartPane.AppearanceSchemeEnum)Enum.Parse(typeof(ChartPane.AppearanceSchemeEnum), item.Text.Replace(" ", string.Empty));

            MasterPane.SetAppearanceScheme(scheme);
            MasterPane.Invalidate();
        }

        private void toolStripDropDownButtonScrollMode_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)e.ClickedItem;
            MasterPane.ScrollMode = (ChartPane.ScrollModeEnum)Enum.Parse(typeof(ChartPane.ScrollModeEnum), item.Text.Replace(" ", string.Empty));

            CheckToolStripDropDownButtonItem(toolStripDropDownButtonScrollMode, (int)MasterPane.ScrollMode);
        }


        private void toolStripButtonLimitView_Click(object sender, EventArgs e)
        {
            MasterPane.LimitedView = toolStripButtonLimitView.Checked;
        }

        private void toolStripButtonCrosshair_Click(object sender, EventArgs e)
        {
            MasterPane.CrosshairVisible = toolStripButtonCrosshair.Checked;
            this.Refresh();
        }

        private void toolStripButtonShowScrollbars_Click(object sender, EventArgs e)
        {
            this.hScrollBar.Visible = toolStripButtonShowScrollbars.Checked;
            this.vScrollBar.Visible = toolStripButtonShowScrollbars.Checked;
        }

        void graphicPane_DrawingSpaceUpdatedEvent(ChartPane pane)
        {// Drawing space or Drawing space view was changed - update.

            RectangleF actualDrawingSpaceView = masterPane.GraphicsWrapper.ActualSpaceToDrawingSpace(masterPane.ActualDrawingSpaceArea);

            int width = (int)(masterPane.DrawingSpaceDisplayLimit.Width - actualDrawingSpaceView.Width);
            int height = (int)(masterPane.DrawingSpaceDisplayLimit.Height - actualDrawingSpaceView.Height);

            bool hScrollRefresh = false;
            bool vScrollRefresh = false;

            if (width > 0)
            {
                if (hScrollBar.Maximum != width)
                {
                    this.hScrollBar.Maximum = width;
                    hScrollRefresh = true;
                }

                if (hScrollBar.LargeChange != hScrollBar.Maximum / 100 + 1)
                {
                    hScrollBar.LargeChange = hScrollBar.Maximum / 100 + 1;
                    hScrollRefresh = true;
                }

                if (hScrollBar.SmallChange != hScrollBar.Maximum / 1000 + 1)
                {
                    hScrollBar.SmallChange = hScrollBar.Maximum / 1000 + 1;
                    hScrollRefresh = true;
                }
            }

            if (this.hScrollBar.Enabled != (width > 0))
            {
                this.hScrollBar.Enabled = (width > 0);
            }

            if (height > 0)
            {
                if (this.vScrollBar.Maximum != height)
                {
                    this.vScrollBar.Maximum = height;
                    vScrollRefresh = true;
                }

                if (vScrollBar.LargeChange != vScrollBar.Maximum / 100 + 1)
                {
                    vScrollBar.LargeChange = vScrollBar.Maximum / 100 + 1;
                    vScrollRefresh = true;
                }

                if (vScrollBar.SmallChange != vScrollBar.Maximum / 1000 + 1)
                {
                    vScrollBar.SmallChange = vScrollBar.Maximum / 1000 + 1;
                    vScrollRefresh = true;
                }
            }

            if (this.vScrollBar.Enabled != false)
            {
                this.vScrollBar.Enabled = false;
            }
            //if (this.vScrollBar.Enabled != (height > 0))
            //{
                //this.vScrollBar.Enabled = (height > 0);
                //vScrollRefresh = true;
            //}

            int xLocation = (int)(actualDrawingSpaceView.X - masterPane.DrawingSpaceDisplayLimit.X);
            int yLocation = (int)(actualDrawingSpaceView.Y - masterPane.DrawingSpaceDisplayLimit.Y);

            if (xLocation > 0)
            {
                if (hScrollBar.Value != Math.Min(hScrollBar.Maximum, xLocation))
                {
                    hScrollBar.Value = Math.Min(hScrollBar.Maximum, xLocation);
                    hScrollRefresh = true;
                }
            }
            else
            {
                if (hScrollBar.Value != 0)
                {
                    hScrollBar.Value = 0;
                    hScrollRefresh = true;
                }
            }

            if (yLocation > 0 && vScrollBar.Maximum - yLocation > 0)
            {
                if (vScrollBar.Value != Math.Min(vScrollBar.Maximum, vScrollBar.Maximum - yLocation))
                {
                    // Y bars operate in the other faship - top is top
                    vScrollBar.Value = Math.Min(vScrollBar.Maximum, vScrollBar.Maximum - yLocation);
                    vScrollRefresh = true;
                }
            }
            else
            {
                if (vScrollBar.Value != 0)
                {
                    vScrollBar.Value = 0;
                    vScrollRefresh = true;
                }
            }

            if (hScrollRefresh)
            {
                hScrollBar.Refresh();
            }

            if (vScrollRefresh)
            {
                vScrollBar.Refresh();
            }

            // Also Update the series in the Save To File button.
            UpdateSaveToFileUI();
        }

        void UpdateSaveToFileUI()
        {
            for (int i = 0; i < MasterPane.Series.Length; i++)
            {
                if (toolStripButtonSaveToFile.DropDownItems.Count < i + 1)
                {
                    ToolStripItem item = toolStripButtonSaveToFile.DropDownItems.Add(MasterPane.Series[i].Name);
                    if (item.Tag is int == false || (int)item.Tag != i)
                    {
                        item.Tag = i;
                    }
                }
                else
                if (toolStripButtonSaveToFile.DropDownItems[i].Name != MasterPane.Series[i].Name)
                {
                    toolStripButtonSaveToFile.DropDownItems[i].Text = MasterPane.Series[i].Name;
                    toolStripButtonSaveToFile.DropDownItems[i].Tag = i;
                }
            }

            for (int i = MasterPane.Series.Length; i < toolStripButtonSaveToFile.DropDownItems.Count; i++)
            {
                toolStripButtonSaveToFile.DropDownItems.RemoveAt(MasterPane.Series.Length);
            }
        }

        void graphicPane_DrawingSpaceViewTransformationChangedEvent(ChartPane pane, System.Drawing.Drawing2D.Matrix previousTransformation, System.Drawing.Drawing2D.Matrix currentTransformation)
        {// Pane has changed its view.
            graphicPane_DrawingSpaceUpdatedEvent(pane);
            toolStripLabelUnitUnification.Text = "Scale: 1 / " + masterPane.CurrentUnitUnification;
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            RectangleF actualDrawingSpaceView = masterPane.GraphicsWrapper.ActualSpaceToDrawingSpace(masterPane.ActualDrawingSpaceArea);

            int yLocation = (int)(actualDrawingSpaceView.Y - masterPane.DrawingSpaceDisplayLimit.Y);
            int yValue = (vScrollBar.Value + yLocation) - vScrollBar.Maximum;

            MasterPane.HandlePan(true, new PointF(0, yValue));
            MasterPane.Invalidate();
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            RectangleF actualDrawingSpaceView = masterPane.GraphicsWrapper.ActualSpaceToDrawingSpace(masterPane.ActualDrawingSpaceArea);

            int xLocation = (int)(actualDrawingSpaceView.X - masterPane.DrawingSpaceDisplayLimit.X);
            int xValue = hScrollBar.Value - xLocation;

            MasterPane.HandlePan(true, new PointF(-xValue, 0));
            MasterPane.Invalidate();
        }

        private void toolStripButtonSaveToFile_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (MasterPane.SeriesCount <= (int)e.ClickedItem.Tag)
            {
                SystemMonitor.Warning("Selected for saving series that does not exist.");
                return;
            }

            ChartSeries series = MasterPane.Series[(int)e.ClickedItem.Tag];

            if (series.ItemsCount == 0)
            {
                MessageBox.Show("This series has no bars to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Select save file";
                dialog.AddExtension = true;
                dialog.RestoreDirectory = true;
                dialog.DefaultExt = "csv";
                dialog.Filter = "Text file (*.csv)|*.csv";
                dialog.FileName = GeneralHelper.RepairFileName(series.Name);

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                series.SaveToFile(dialog.FileName);
            }
        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            MasterPane.ZoomIn(2);
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            MasterPane.ZoomOut(0.5f);
        }

        private void toolStripButtonShowTimeGaps_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ChartSeries series in MasterPane.Series)
            {
                if (series is TimeBasedChartSeries)
                {
                    ((TimeBasedChartSeries)series).ShowTimeGaps = toolStripButtonShowTimeGaps.Checked;
                }
            }

            MasterPane.Invalidate();
        }

        private void toolStripButtonDrawLineSegment_Click(object sender, EventArgs e)
        {
            if (toolStripButtonDrawLineSegment.Checked)
            {
                if (MasterPane.CustomObjectsManager.IsBuildingObject)
                {// Already building.
                    ((ToolStripButton)(sender)).Checked = false;
                    return;
                }

                LineObject line = new LineObject("line", LineObject.ModeEnum.LineSegment);
                line.Pen = new Pen(DrawingColor);
                MasterPane.CustomObjectsManager.BuildDynamicObject(line);
            }
            else
            {
                MasterPane.CustomObjectsManager.StopBuildingDynamicObject();
            }
        }

        private void toolStripButtonDrawLine_Click(object sender, EventArgs e)
        {
            if (toolStripButtonDrawLine.Checked)
            {
                if (MasterPane.CustomObjectsManager.IsBuildingObject)
                {// Already building.
                    ((ToolStripButton)(sender)).Checked = false;
                    return;
                }

                LineObject line = new LineObject("line", LineObject.ModeEnum.Line);
                line.Pen = new Pen(DrawingColor);
                MasterPane.CustomObjectsManager.BuildDynamicObject(line);
            }
            else
            {
                MasterPane.CustomObjectsManager.StopBuildingDynamicObject();
            }
        }

        private void toolStripButtonColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = toolStripButtonColor.BackColor;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                toolStripButtonColor.BackColor = dialog.Color;
            }
        }

        private void toolStripButtonHorizontalLine_Click(object sender, EventArgs e)
        {
            if (toolStripButtonHorizontalLine.Checked)
            {
                if (MasterPane.CustomObjectsManager.IsBuildingObject)
                {// Already building.
                    ((ToolStripButton)(sender)).Checked = false;
                    return;
                }
                
                LineObject line = new LineObject("horizontal line", LineObject.ModeEnum.HorizontalLine);
                line.Pen = new Pen(DrawingColor);
                MasterPane.CustomObjectsManager.BuildDynamicObject(line);
            }
            else
            {
                MasterPane.CustomObjectsManager.StopBuildingDynamicObject();
            }
        }

        private void toolStripButtonVerticalLine_Click(object sender, EventArgs e)
        {
            if (toolStripButtonVerticalLine.Checked)
            {
                if (MasterPane.CustomObjectsManager.IsBuildingObject)
                {// Already building.
                    ((ToolStripButton)(sender)).Checked = false;
                    return;
                }
                
                LineObject line = new LineObject("vertical line", LineObject.ModeEnum.VerticalLine);
                line.Pen = new Pen(DrawingColor);
                MasterPane.CustomObjectsManager.BuildDynamicObject(line);
            }
            else
            {
                MasterPane.CustomObjectsManager.StopBuildingDynamicObject();
            }
        }

        private void toolStripButtonFibonacci_Click(object sender, EventArgs e)
        {
            if (toolStripButtonFibonacci.Checked)
            {
                if (MasterPane.CustomObjectsManager.IsBuildingObject)
                {// Already building.
                    ((ToolStripButton)(sender)).Checked = false;
                    return;
                }

                FibonacciRetracementObject fibonacci = new FibonacciRetracementObject("fibonacci retracement");
                fibonacci.PenColor = DrawingColor;
                MasterPane.CustomObjectsManager.BuildDynamicObject(fibonacci);
            }
            else
            {
                MasterPane.CustomObjectsManager.StopBuildingDynamicObject();
            }
        }

        private void toolStripButtonText_Click(object sender, EventArgs e)
        {
            if (toolStripButtonText.Checked)
            {
                if (MasterPane.CustomObjectsManager.IsBuildingObject)
                {// Already building.
                    ((ToolStripButton)(sender)).Checked = false;
                    return;
                }

                TextObject text = new TextObject("text");
                text.Brush = new SolidBrush(DrawingColor);
                MasterPane.CustomObjectsManager.BuildDynamicObject(text);
            }
            else
            {
                MasterPane.CustomObjectsManager.StopBuildingDynamicObject();
            }

        }

        private void toolStripButtonAutoScrollToEnd_Click(object sender, EventArgs e)
        {
            MasterPane.AutoScrollToEnd = toolStripButtonAutoScrollToEnd.Checked;
        }

        private void hScrollBar_Enter(object sender, EventArgs e)
        {

        }

        private void masterPane_AppearanceSchemeChangedEvent(ChartPane pane, ChartPane.AppearanceSchemeEnum scheme)
        {
            if (masterPane.XAxisLabelsFontBrush is SolidBrush)
            {
                this.toolStripMain.ForeColor = ((SolidBrush)masterPane.XAxisLabelsFontBrush).Color;
            }
        }

        private void toolStripMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButtonArrow_Click(object sender, EventArgs e)
        {
            if (toolStripButtonArrow.Checked)
            {
                if (MasterPane.CustomObjectsManager.IsBuildingObject)
                {// Already building.
                    ((ToolStripButton)(sender)).Checked = false;
                    return;
                }

                ArrowObject arrow = new ArrowObject("arrow");
                
                arrow.Pen = new Pen(DrawingColor);
                arrow.Brush = new SolidBrush(DrawingColor);

                MasterPane.CustomObjectsManager.BuildDynamicObject(arrow);
            }
            else
            {
                MasterPane.CustomObjectsManager.StopBuildingDynamicObject();
            }
        }


    }
}
