using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using CommonSupport;
using System.ComponentModel;
using System.Windows.Forms;

namespace CommonFinancial
{
    /// <summary>
    /// Slave chart pane, serves as a secondary charting area to a master one.
    /// Usually used for showing synchronized additional information to the data shown in the master chart pane.
    /// </summary>
    public class SlaveChartPane : ChartPane
    {
        /// <summary>
        /// If the pane has a master pane, how should it synchronize (in both directions, or x only).
        /// Synchronizing in X only is usefull when slave pane has a different Y scale than the master pane.
        /// </summary>
        public enum MasterPaneSynchronizationModeEnum
        {
            None,
            XAxis
        }

        /// <summary>
        /// When synchronizing with master, autonomous autoscroll to end is disabled.
        /// </summary>
        public override bool AutoScrollToEnd
        {
            get
            {
                return base.AutoScrollToEnd;
            }

            set
            {// When synchronizing with master, autonomous autoscroll to end is disabled.
                if (_masterPaneSynchronizationMode == MasterPaneSynchronizationModeEnum.None)
                {
                    base.AutoScrollToEnd = value;
                }
                else
                {
                    base.AutoScrollToEnd = false;
                }
            }
        }

        bool _showMasterSynchronizationImage = true;
        /// <summary>
        /// Should the pane be showing the image specifying if it is synchronzed with master.
        /// </summary>
        public bool ShowMasterSynchronizationImage
        {
            get { return _showMasterSynchronizationImage; }
            set { _showMasterSynchronizationImage = value; }
        }

        /// <summary>
        /// Image used when a pane is slave pane, to signify if pane is precisely aligned with master or not.
        /// </summary>
        Image _masterSynchronizationImage;
        bool _synchronizedWithMaster = false;

        volatile MasterPaneSynchronizationModeEnum _masterPaneSynchronizationMode = MasterPaneSynchronizationModeEnum.XAxis;
        public MasterPaneSynchronizationModeEnum MasterPaneSynchronizationMode
        {
            get { return _masterPaneSynchronizationMode; }
            set { _masterPaneSynchronizationMode = value; }
        }


        MasterChartPane _masterPane;
        /// <summary>
        /// If this pane needs to synchronize its zoom, pan, 
        /// etc. with another pane, assign master pane here.
        /// </summary>
        public MasterChartPane MasterPane
        {
            get { return _masterPane; }
            set
            {
                if (_masterPane != null)
                {
                    _masterPane.DrawingSpaceViewTransformationChangedEvent -= new DrawingSpaceViewTransformationChangedDelegate(_masterPane_DrawingSpaceViewTransformationChangedEvent);
                    _masterPane.AppearanceSchemeChangedEvent -= new AppearanceSchemeChangedDelegate(_masterPane_AppearanceSchemeChangedEvent);
                    _masterPane.Crosshair.CrossHairShowEvent -= new CommonFinancial.CrossHairRender.CrossHairShowDelegate(_masterPane_CrossHairShowEvent);
                    _masterPane.ParametersUpdatedEvent -= new ParametersUpdatedDelegate(_masterPane_ParametersUpdatedEvent);
                }

                _masterPane = value;
                if (_masterPane != null)
                {
                    _masterPane.Crosshair.CrossHairShowEvent += new CommonFinancial.CrossHairRender.CrossHairShowDelegate(_masterPane_CrossHairShowEvent);
                    _masterPane.DrawingSpaceViewTransformationChangedEvent += new DrawingSpaceViewTransformationChangedDelegate(_masterPane_DrawingSpaceViewTransformationChangedEvent);
                    _masterPane.AppearanceSchemeChangedEvent += new AppearanceSchemeChangedDelegate(_masterPane_AppearanceSchemeChangedEvent);
                    _masterPane.ParametersUpdatedEvent += new ParametersUpdatedDelegate(_masterPane_ParametersUpdatedEvent);

                    this.SetAppearanceScheme(_masterPane.AppearanceScheme);

                    SynchronizeWithMasterPane();
                }
            }
        }

        Point? _lastCrossHairPosition;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SlaveChartPane()
        {
            this.DrawingSpaceViewTransformationChangedEvent += new DrawingSpaceViewTransformationChangedDelegate(SlaveChartPane_DrawingSpaceViewTransformationChangedEvent);
            SetAppearanceMode(AppearanceModeEnum.SuperCompact);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            UpdateDrawingSpace();
            SynchronizeWithMasterPane();

            base.AutoScrollToEnd = false;
        }

        public override void CalculateActualDrawingSpaceAreaMarginTopAndBottom()
        {
            base.CalculateActualDrawingSpaceAreaMarginTopAndBottom();

            if (ShowSeriesLabels)
            {
                _actualDrawingSpaceAreaMarginTop = 28;
            }
        }

        /// <summary>
        /// Draw.
        /// </summary>
        protected override void Draw(PaintEventArgs paintArgs)
        {
            base.Draw(paintArgs);

            _lastCrossHairPosition = null;
        }

        /// <summary>
        /// Draw additional image.
        /// </summary>
        protected override void DrawInitialActualSpaceOverlays(GraphicsWrapper g, TimeBasedChartSeries timeBasedSeries)
        {
            base.DrawInitialActualSpaceOverlays(g, timeBasedSeries);

            if (_showMasterSynchronizationImage && _masterSynchronizationImage != null)
            {
                g.DrawImageUnscaledAndClipped(_masterSynchronizationImage, new Rectangle(4, (int)LabelsTopMargin, _masterSynchronizationImage.Width, _masterSynchronizationImage.Height));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void _masterPane_ParametersUpdatedEvent(ChartPane pane)
        {
            SynchronizeWithMasterPane();
        }

        public override void DrawCrossHairAt(Point screenLocation)
        {
            if (_masterPane != null)
            {
                _masterPane.DrawCrossHairAt(screenLocation);
            }
        }

        void _masterPane_CrossHairShowEvent(CrossHairRender render, Point screenLocation)
        {
            Crosshair.DrawAt(this.RectangleToScreen(ActualDrawingSpaceArea), screenLocation, true);

            DrawCrossHairLocationInfo(null, this.PointToClient(screenLocation));
        }

        void _masterPane_AppearanceSchemeChangedEvent(ChartPane pane, AppearanceSchemeEnum scheme)
        {
            this.SetAppearanceScheme(scheme);
            this.Refresh();
            //this.Invalidate();
        }

        void SlaveChartPane_DrawingSpaceViewTransformationChangedEvent(ChartPane pane, Matrix previousTransformation, Matrix currentTransformation)
        {
            UpdateMasterSynchronizationState(false);
        }

        void _masterPane_DrawingSpaceViewTransformationChangedEvent(ChartPane pane, Matrix previousTransformation, Matrix currentTransformation)
        {
            SynchronizeWithMasterPane();
        }

        public void SynchronizeWithMasterPane()
        {
            if (_masterPane == null || _masterPaneSynchronizationMode == MasterPaneSynchronizationModeEnum.None)
            {
                return;
            }

            SystemMonitor.CheckThrow(_masterPaneSynchronizationMode == MasterPaneSynchronizationModeEnum.XAxis, "Mode not supported.");
            GraphicsWrapper.SynchronizeDrawingSpaceXAxis(_masterPane.GraphicsWrapper);

            this.YAxisLabelsPosition = _masterPane.YAxisLabelsPosition;

            //this.AutoScrollToEnd = _masterPane.AutoScrollToEnd;
            this.LimitedView = _masterPane.LimitedView;

            Crosshair.Visible = _masterPane.Crosshair.Visible;

            RectangleF screen = GraphicsWrapper.ActualSpaceToDrawingSpace(ActualDrawingSpaceArea);

            FitHorizontalAreaToScreen(screen);

            UpdateMasterSynchronizationState(true);

            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(100), Refresh);
            //this.Invalidate();
        }

        void UpdateMasterSynchronizationState(bool isSynchronized)
        {
            _synchronizedWithMaster = isSynchronized;

            if (AppearanceMode == AppearanceModeEnum.Normal)
            {
                if (isSynchronized)
                {
                    // Set the synchronization image to green.
                    ComponentResourceManager resources = new ComponentResourceManager(typeof(SlaveChartPane));
                    _masterSynchronizationImage = ((Image)(resources.GetObject("imageComponentGreen")));
                }
                else
                {
                    // Set the synchronization image to green.
                    ComponentResourceManager resources = new ComponentResourceManager(typeof(SlaveChartPane));
                    _masterSynchronizationImage = ((Image)(resources.GetObject("imageComponentRed")));
                }
            }
        }

        public override void Add(ChartSeries series, bool usePaneColorSelector, bool replaceSeriesWithSameName)
        {
            base.Add(series, usePaneColorSelector, replaceSeriesWithSameName);
            SynchronizeWithMasterPane();
        }

        protected override void series_SeriesUpdatedEvent(ChartSeries series, bool updateUI)
        {
            if (updateUI)
            {
                SynchronizeWithMasterPane();
            }
            base.series_SeriesUpdatedEvent(series, updateUI);
        }

        protected override void DrawGraphicSeriesLabels(GraphicsWrapper g, int initialMarginLeft)
        {// Provide additional space for the labels on the left.
            base.DrawGraphicSeriesLabels(g, initialMarginLeft + 16);
        }
    }
}
