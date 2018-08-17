using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CommonSupport;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace CommonFinancial
{
    /// <summary>
    /// Note - all the Brush and Pens do not have Get accessors on purpose, since defaults can not be modified. 
    /// So to change them directly use the "set".
    /// Implements custom serialization routine, so that its settings, id etc. can be preserved.
    /// </summary>
    public partial class ChartPane : ContainerControl
    {
        const int MinimumAbsoluteSelectionWidth = 18;
        /// <summary>
        /// The larger, the slower panning with keys is.
        /// </summary>
        const float KeyboardPanCoefficient = 30;

        public enum SelectionModeEnum
        {
            Select,
            RectangleZoom,
            HorizontalZoom,
            VerticalZoom,
            None
        }

        public enum ScrollModeEnum
        {
            HorizontalScroll,
            HorizontalScrollAndFit,
            VerticalScroll,
            HorizontalZoom,
            VerticalZoom,
            ZoomToMouse,
            None
        }

        public enum AppearanceModeEnum
        {
            Normal,
            Compact, // Compact mode removes allot of the additional space and items around the chart pane.
            SuperCompact // Same like compact, only more.
        }

        public enum AppearanceSchemeEnum
        {
            Default,
            Custom,
            Fast,
            Trade,
            TradeWhite,
            Dark,
            DarkNatural,
            Light,
            LightNatural,
            LightNaturalFlat,
            Alfonsina,
            Ground
        }

        public enum YAxisLabelPosition
        {
            Left,
            Right,
            Both,
            None
        }

        #region General

        Guid _stateId = Guid.Empty;
        /// <summary>
        /// Needed for persistence relation purposes.
        /// </summary>
        public Guid StateId
        {
            get { return _stateId; }
        }

        volatile AppearanceModeEnum _appearanceMode = AppearanceModeEnum.Normal;
        public AppearanceModeEnum AppearanceMode
        {
            get { return _appearanceMode; }
            set
            {
                SetAppearanceMode(value);
            }
        }

        volatile AppearanceSchemeEnum _appearanceScheme = AppearanceSchemeEnum.Default;
        public AppearanceSchemeEnum AppearanceScheme
        {
            get { return _appearanceScheme; }
            set 
            { 
                SetAppearanceScheme(value);
            }
        }

        bool _autoScrollToEnd = true;
        public virtual bool AutoScrollToEnd
        {
            get { return _autoScrollToEnd; }
            set { _autoScrollToEnd = value; }
        }

        /// <summary>
        /// Needed to synchronize the view to the drawing space when resizing (show the same thing after resized)
        /// </summary>
        Size _lastControlSize = Size.Empty;
        Rectangle _lastActualDrawingSpaceArea = Rectangle.Empty;

        ComponentResourceManager _resources;

        ContextMenuStrip _seriesTypeDynamicContextMenu;

        ChartSeriesColorSelector _colorSelector = new ChartSeriesColorSelector();
        public ChartSeriesColorSelector ColorSelector
        {
            get { return _colorSelector; }
        }

        /// <summary>
        /// Space between labels on X Axis (in number of items displayed).
        /// </summary>
        float _xAxisLabelSpacing = 20;

        /// <summary>
        /// Space between labels on Y Axis (auto assigned).
        /// </summary>
        float _autoYAxisLabelSpacing;

        float? _maximumXZoom = 2f;
        public float? MaximumXZoom
        {
            get { return _maximumXZoom; }
            set 
            {
                _maximumXZoom = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }
            }
        }

        string _chartName = "";
        public string ChartName
        {
            get { return _chartName; }
            set 
            { 
                SetChartName(value);
            }
        }

        Font _titleFont;
        public Font TitleFont
        {
            get {  return _titleFont; }
            set 
            {  
                _titleFont = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }
            }
        }

        Brush _titleFontBrush;
        public Brush TitleFontBrush
        {
            get
            {
                return _titleFontBrush;
            }

            set 
            {  
                _titleFontBrush = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }
            }
        }

        Brush _fill;
        public Brush Fill
        {
            get
            {
                return _fill;
            }

            set 
            { 
                _fill = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        Font _axisLabelsFont;
        public Font AxisLabelsFont
        {
            get {  return _axisLabelsFont; }
            set 
            {  
                _axisLabelsFont = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        Brush _xAxisLabelsFontBrush;
        public Brush XAxisLabelsFontBrush
        {
            get
            {
                return _xAxisLabelsFontBrush;
            }

            set 
            { 
                _xAxisLabelsFontBrush = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        YAxisLabelPosition _yAxisLabelsPosition;
        /// <summary>
        /// OrderBasedPosition of the Y axis labels.
        /// </summary>
        public YAxisLabelPosition YAxisLabelsPosition
        {
            get { return _yAxisLabelsPosition; }
            set 
            { 
                _yAxisLabelsPosition = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        Brush _yAxisLabelsFontBrush;
        public Brush YAxisLabelsFontBrush
        {
            get
            {
                return _yAxisLabelsFontBrush;
            }

            set 
            { 
                _yAxisLabelsFontBrush = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        string _xAxisLabelsFormat;
        public string XAxisLabelsFormat
        {
            get {  return _xAxisLabelsFormat; }
            set 
            {  
                _xAxisLabelsFormat = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        // This must auto adjust to format the number properly and always fit in 6 spaces.
        // Specify positive, negative and zero formats.
        volatile string _yAxisLabelsFormat = " #0.####;-#0.####; Zero";

        public string YAxisLabelsFormat
        {
            get { return _yAxisLabelsFormat; }
            set { _yAxisLabelsFormat = value; }
        }


        Rectangle[] _currentLabelsRectangles = new Rectangle[] { };

        Font _labelsFont;
        public Font LabelsFont
        {
            get {  return _labelsFont; }
            set 
            {  
                _labelsFont = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        Brush _labelsFontBrush;
        public Brush LabelsFontBrush
        {
            get
            {
                return _labelsFontBrush;
            }

            set 
            {  
                _labelsFontBrush = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        Brush _labelsFill;
        public Brush LabelsFill
        {
            get
            {
                return _labelsFill;
            }

            set 
            {  
                _labelsFill = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        float _labelsTopMargin = 8;
        public float LabelsTopMargin
        {
            get { return _labelsTopMargin; }
            set { _labelsTopMargin = value; }
        }

        float _labelsMargin;
        public float LabelsMargin
        {
            get {  return _labelsMargin; }
            set 
            {  
                _labelsMargin = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        /// <summary>
        /// X Axis labels active.
        /// </summary>
        protected bool XAxisLabels
        {
            get
            {
                return _appearanceMode == AppearanceModeEnum.Normal;
            }
        }

        /// <summary>
        /// Y Axis labels active.
        /// </summary>
        protected bool YAxisLabels
        {
            get
            {
                return true;
            }
        }


        bool _showSeriesLabels = true;
        public bool ShowSeriesLabels
        {
            get { return _showSeriesLabels; }
            set 
            {
                _showSeriesLabels = value;

                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        bool _showClippingRectangle = false;
        public bool ShowClippingRectangle
        {
            get {  return _showClippingRectangle; }
            set 
            {  
                _showClippingRectangle = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        bool _unitUnificationOptimizationEnabled = true;
        /// <summary>
        /// Should unit unification be performed to inteligently combine the drawing of many units together to speed un drawing.
        /// </summary>
        public bool UnitUnificationOptimizationEnabled
        {
            get { return _unitUnificationOptimizationEnabled; }
            set 
            { 
                _unitUnificationOptimizationEnabled = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        public int CurrentUnitUnification
        {
            get
            {
                if (_unitUnificationOptimizationEnabled == false)
                {
                    return 1;
                }
                
                return 1 + (int)(0.2 / Math.Abs(GraphicsWrapper.DrawingSpaceTransformClone.Elements[0]));
            }
        }

        SmoothingMode _smoothingMode = SmoothingMode.None;
        public SmoothingMode SmoothingMode
        {
            get {  return _smoothingMode; }
            set 
            {  
                _smoothingMode = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        CustomObjectsManager _customObjectsManager;
        public CustomObjectsManager CustomObjectsManager
        {
            get { return _customObjectsManager; }
        }

        Image _customObjectDrawingImage;

        volatile CrossHairRender _crosshair;
        internal CrossHairRender Crosshair
        {
            get { return _crosshair; }
        }

        #endregion

        #region Selection

        float _defaultAbsoluteSelectionMargin = 7;
        /// <summary>
        /// In pixels, the selection margin to any object or control point of object.
        /// </summary>
        public float DefaultAbsoluteSelectionMargin
        {
            get { return _defaultAbsoluteSelectionMargin; }
            set { _defaultAbsoluteSelectionMargin = value; }
        }

        /// <summary>
        /// What area of the drawing space the user has selected with the mouse.
        /// </summary>
        public RectangleF? CurrentUserSelectedRectangle
        {
            get
            {
                if (_rightMouseButtonSelectionMode == SelectionModeEnum.None || _lastDrawingSpaceMouseRightButtonPosition.HasValue == false 
                    || _currentDrawingSpaceMousePosition.HasValue == false)
                {
                    return null;
                }

                // Normalize.
                PointF lowerLeftPoint = new PointF(Math.Min(_lastDrawingSpaceMouseRightButtonPosition.Value.X, _currentDrawingSpaceMousePosition.Value.X), Math.Min(_lastDrawingSpaceMouseRightButtonPosition.Value.Y, _currentDrawingSpaceMousePosition.Value.Y));
                PointF upperRightPoint = new PointF(Math.Max(_lastDrawingSpaceMouseRightButtonPosition.Value.X, _currentDrawingSpaceMousePosition.Value.X), Math.Max(_lastDrawingSpaceMouseRightButtonPosition.Value.Y, _currentDrawingSpaceMousePosition.Value.Y));

                return new RectangleF(lowerLeftPoint.X, lowerLeftPoint.Y, upperRightPoint.X - lowerLeftPoint.X, upperRightPoint.Y - lowerLeftPoint.Y);
            }
        }

        /// <summary>
        /// What selection is, after considering the selection mode.
        /// </summary>
        public RectangleF? CurrentSelectionRectangle
        {
            get
            {
                if (CurrentUserSelectedRectangle == null)
                {
                    return null;
                }
                
                RectangleF selectionRectangle = CurrentUserSelectedRectangle.Value;

                if (selectionRectangle.X < _drawingSpace.X)
                {
                    selectionRectangle.Width -= _drawingSpace.X - selectionRectangle.X;
                    selectionRectangle.X = _drawingSpace.X;
                }

                if (selectionRectangle.X + selectionRectangle.Width > _drawingSpace.X + _drawingSpace.Width)
                {
                    selectionRectangle.Width = _drawingSpace.X + _drawingSpace.Width - selectionRectangle.X;
                }

                if (selectionRectangle.Y < _drawingSpace.Y)
                {
                    selectionRectangle.Height -= _drawingSpace.Y - selectionRectangle.Y;
                    selectionRectangle.Y = _drawingSpace.Y;
                }

                if (selectionRectangle.Y + selectionRectangle.Height > _drawingSpace.Y + _drawingSpace.Height)
                {
                    selectionRectangle.Height = _drawingSpace.Y + _drawingSpace.Height - selectionRectangle.Y;
                }

                switch (_rightMouseButtonSelectionMode)
                {
                    case SelectionModeEnum.RectangleZoom:
                        break;
                    case SelectionModeEnum.Select:
                        selectionRectangle.Width = 0;
                        selectionRectangle.Height = 0;
                        break;
                    case SelectionModeEnum.HorizontalZoom:
                        selectionRectangle.Y = _drawingSpace.Y;
                        selectionRectangle.Height = _drawingSpace.Height;
                        break;
                    case SelectionModeEnum.VerticalZoom:
                        selectionRectangle.X = _drawingSpace.X;
                        selectionRectangle.Width = _drawingSpace.Width;
                        break;
                    case SelectionModeEnum.None:
                    default:
                        break;
                }

                return selectionRectangle;
            }
        }

        ScrollModeEnum _scrollMode = ScrollModeEnum.HorizontalScrollAndFit;
        public ScrollModeEnum ScrollMode
        {
            get { return _scrollMode; }
            set 
            {
                _scrollMode = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        SelectionModeEnum _rightMouseButtonSelectionMode = SelectionModeEnum.HorizontalZoom;
        public SelectionModeEnum RightMouseButtonSelectionMode
        {
            get {  return _rightMouseButtonSelectionMode; }
            set 
            {
                _rightMouseButtonSelectionMode = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        Pen _selectionPen;
        public Pen SelectionPen
        {
            set 
            {
                _selectionPen = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        Brush _selectionFill;
        public Brush SelectionFill
        {
            set 
            {  
                _selectionFill = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        #endregion

        #region Actual Space

        int _additionalDrawingSpaceAreaMarginLeft = 0;
        public int AdditionalDrawingSpaceAreaMarginLeft
        {
            get { return _additionalDrawingSpaceAreaMarginLeft; }
            set { _additionalDrawingSpaceAreaMarginLeft = value; }
        }

        int _additionalDrawingSpaceAreaMarginRight = 0;
        public int AdditionalDrawingSpaceAreaMarginRight
        {
            get { return _additionalDrawingSpaceAreaMarginRight; }
            set { _additionalDrawingSpaceAreaMarginRight = value; }
        }

        protected int _actualDrawingSpaceAreaMarginLeft;
        public int ActualDrawingSpaceAreaMarginLeft
        {
            get { return _actualDrawingSpaceAreaMarginLeft; }
        }

        protected int _actualDrawingSpaceAreaMarginTop;
        public int ActualDrawingSpaceAreaMarginTop
        {
            get { return _actualDrawingSpaceAreaMarginTop; }
        }

        protected int _actualDrawingSpaceAreaMarginRight;
        public int ActualDrawingSpaceAreaMarginRight
        {
            get { return _actualDrawingSpaceAreaMarginRight; }
        }

        protected int _actualDrawingSpaceAreaMarginBottom;
        public int ActualDrawingSpaceAreaMarginBottom
        {
            get { return _actualDrawingSpaceAreaMarginBottom; }
        }

        Rectangle _actualDrawingSpaceArea;
        public Rectangle ActualDrawingSpaceArea
        {
            get {  return _actualDrawingSpaceArea; }
        }

        Pen _actualDrawingSpaceAreaBorderPen;
        public Pen ActualDrawingSpaceAreaBorderPen
        {
            get
            {
                return _actualDrawingSpaceAreaBorderPen;
            }

            set 
            {
                _actualDrawingSpaceAreaBorderPen = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        Brush _actualDrawingSpaceAreaFill;
        public Brush ActualDrawingSpaceAreaFill
        {
            get
            {
                return _actualDrawingSpaceAreaFill;
            }

            set 
            {
                _actualDrawingSpaceAreaFill = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        ChartGrid _actualSpaceGrid = new ChartGrid();
        public ChartGrid ActualSpaceGrid
        {
            get { return _actualSpaceGrid; }
        }

        #endregion

        #region Drawing Space

        RectangleF _drawingSpace;
        public RectangleF DrawingSpace
        {
            get {  return _drawingSpace; }
        }

        bool _limitedView = true;
        
        /// <summary>
        /// Should we be able to see only the drawing area.
        /// </summary>
        public bool LimitedView
        {
            get { return _limitedView; }
            set 
            { 
                _limitedView = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }
 
            }
        }

        RectangleF _drawingSpaceDisplayLimit;
        public RectangleF DrawingSpaceDisplayLimit
        {
            get { return _drawingSpaceDisplayLimit; }

        }

        ChartGrid _drawingSpaceGrid = new ChartGrid();
        public ChartGrid DrawingSpaceGrid
        {
            get {  return _drawingSpaceGrid; }
        }

        /// <summary>
        /// Add an extra layer over GDI+ to handle some bugs of GDI+.
        /// </summary>
        GraphicsWrapper _graphicsWrapper = new GraphicsWrapper();
        public GraphicsWrapper GraphicsWrapper
        {
            get { return _graphicsWrapper; }
        }

        #endregion

        #region User Input

        bool _isControlKeyDown = false;
        public bool IsControlKeyDown
        {
            get { return _isControlKeyDown; }
        }

        bool _isShiftKeyDown = false;
        public bool IsShiftKeyDown
        {
            get { return _isShiftKeyDown; }
        }

        PointF? _currentDrawingSpaceMousePosition;
        public PointF? CurrentDrawingSpaceMousePosition
        {
            get { return _currentDrawingSpaceMousePosition; }
        }

        PointF? _lastDrawingSpaceMouseRightButtonPosition;

        /// <summary>
        /// Updated also on mouse move to handle drag.
        /// </summary>
        PointF? _lastDrawingSpaceMouseDownLeftButton;

        PointF? _lastDrawingSpaceMouseDownMiddleButton;

        public bool CrosshairVisible
        {
            get { return _crosshair.Visible; }
            set 
            {
                _crosshair.Visible = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        ContextMenuStrip _chartContextMenu;
        ToolStripMenuItem _autoScrollToEndContextMenuItem;
        ToolStripMenuItem _crossHairContextMenuItem;
        ToolStripMenuItem _labelsContextMenuItem;
        ToolStripMenuItem _limitViewContextMenuItem;
        ToolStripMenuItem _selectedObjectsContextMenuItem;
        ToolStripMenuItem _seriesPropertiesContextMenuItem;

        #endregion
        
        #region Series

        /// <summary>
        /// Series not persisted internally.
        /// </summary>
        ListUnique<ChartSeries> _series = new ListUnique<ChartSeries>();
        public ChartSeries[] Series
        {
            get {  return _series.ToArray(); }
        }

        public int SeriesCount
        {
            get { return _series.Count; }
        }

        float _seriesItemWidth;
        /// <summary>
        /// All series must share item sizes.
        /// </summary>
        public float SeriesItemWidth
        {
            get {  return _seriesItemWidth; }
            set 
            {
                _seriesItemWidth = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        float _seriesItemMargin;
        /// <summary>
        /// All series must share item sizes.
        /// </summary>
        public float SeriesItemMargin
        {
            get {  return _seriesItemMargin; }
            set 
            {  
                _seriesItemMargin = value;
                if (ParametersUpdatedEvent != null)
                {
                    ParametersUpdatedEvent(this);
                }

            }
        }

        #endregion

        /// <summary>
        /// The DrawingSpace transformation was modified, so now a different part of the drawing space is visible.
        /// </summary>
        /// <param name="pane"></param>
        public delegate void DrawingSpaceViewTransformationChangedDelegate(ChartPane pane, Matrix previousTransformation, Matrix currentTransformation);
        public event DrawingSpaceViewTransformationChangedDelegate DrawingSpaceViewTransformationChangedEvent;

        public delegate void DrawingSpaceUpdatedDelegate(ChartPane pane);
        public event DrawingSpaceUpdatedDelegate DrawingSpaceUpdatedEvent;

        public delegate void AppearanceSchemeChangedDelegate(ChartPane pane, AppearanceSchemeEnum scheme);
        public event AppearanceSchemeChangedDelegate AppearanceSchemeChangedEvent;

        public delegate void ParametersUpdatedDelegate(ChartPane pane);
        public event ParametersUpdatedDelegate ParametersUpdatedEvent;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ChartPane()
        {
            InitializeElements();
            _crosshair = new CrossHairRender(this);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }

        /// <summary>
        /// 
        /// </summary>
        void InitializeElements()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            
            // Establishing a brand new state.
            _stateId = Guid.NewGuid();

            _drawingSpace = new RectangleF(0, 0, 0, 0);
            _drawingSpaceDisplayLimit = _drawingSpace;

            _actualSpaceGrid.Visible = false;
            _actualSpaceGrid.Pen = new Pen(Color.Gray);

            _yAxisLabelsPosition = YAxisLabelPosition.Right;

            _actualDrawingSpaceAreaMarginLeft = 15 + _additionalDrawingSpaceAreaMarginLeft;
            _actualDrawingSpaceAreaMarginTop = 60;
            _actualDrawingSpaceAreaMarginRight = 45 + _additionalDrawingSpaceAreaMarginRight;
            _actualDrawingSpaceAreaMarginBottom = 20;

            _resources = new ComponentResourceManager(typeof(ChartPane));

            _seriesTypeDynamicContextMenu = new ContextMenuStrip();
            

            _chartContextMenu = new ContextMenuStrip();
            _chartContextMenu.Opening += new CancelEventHandler(_chartContextMenu_Opening);

            ToolStripMenuItem item;

            item = new ToolStripMenuItem("Zoom In", ((Image)(_resources.GetObject("imageZoomIn"))), new EventHandler(ZoomInChartContextMenuItem_Click));
            _chartContextMenu.Items.Add(item);
            item = new ToolStripMenuItem("Zoom Out", ((Image)(_resources.GetObject("imageZoomOut"))), new EventHandler(ZoomOutChartContextMenuItem_Click));
            _chartContextMenu.Items.Add(item);
            _chartContextMenu.Items.Add(new ToolStripSeparator());

            item = new ToolStripMenuItem("Fit To Screen", ((Image)(_resources.GetObject("imageLayoutCenter"))), new EventHandler(FitToScreenChartContextMenuItem_Click));
            _chartContextMenu.Items.Add(item);
            item = new ToolStripMenuItem("Fit Horizontal", ((Image)(_resources.GetObject("imageLayoutHorizontal"))), new EventHandler(FitHorizontalChartContextMenuItem_Click));
            _chartContextMenu.Items.Add(item);
            item = new ToolStripMenuItem("Fit Vertical", ((Image)(_resources.GetObject("imageLayoutVertical"))), new EventHandler(FitVerticalChartContextMenuItem_Click));
            _chartContextMenu.Items.Add(item);
            _chartContextMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem selectionContextMenuItem = new ToolStripMenuItem("Selection");
            foreach(string name in Enum.GetNames(typeof(SelectionModeEnum)))
            {
                ToolStripItem subItem = selectionContextMenuItem.DropDownItems.Add(name, null, new EventHandler(SelectionChartContextMenuItem_Click));
                subItem.Tag = name;
            }
            _chartContextMenu.Items.Add(selectionContextMenuItem);
            _chartContextMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem scrollContextMenuItem = new ToolStripMenuItem("Scroll");
            foreach(string name in Enum.GetNames(typeof(ScrollModeEnum)))
            {
                ToolStripItem subItem = scrollContextMenuItem.DropDownItems.Add(name, null, new EventHandler(ScrollChartContextMenuItem_Click));
                subItem.Tag = name;
            }
            _chartContextMenu.Items.Add(scrollContextMenuItem);
            _chartContextMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem appearanceContextMenuItem = new ToolStripMenuItem("Appearance");
            foreach(string name in Enum.GetNames(typeof(AppearanceSchemeEnum)))
            {
                ToolStripItem subItem = appearanceContextMenuItem.DropDownItems.Add(GeneralHelper.SeparateCapitalLetters(name), null, new EventHandler(AppearanceChartContextMenuItem_Click));
                subItem.Tag = name;
            }

            _chartContextMenu.Items.Add(appearanceContextMenuItem);
            _chartContextMenu.Items.Add(new ToolStripSeparator());

            _autoScrollToEndContextMenuItem = new ToolStripMenuItem("Auto Scroll to End", ((Image)(_resources.GetObject("imageScrollToEnd"))), new EventHandler(AutoScrollToEndChartContextMenuItem_Click));
            _autoScrollToEndContextMenuItem.CheckOnClick = true;
            _autoScrollToEndContextMenuItem.Checked = this.AutoScrollToEnd;
            _chartContextMenu.Items.Add(_autoScrollToEndContextMenuItem);

            _crossHairContextMenuItem = new ToolStripMenuItem("Crosshair", ((Image)(_resources.GetObject("imageTarget"))), new EventHandler(CrosshairChartContextMenuItem_Click));
            _crossHairContextMenuItem.CheckOnClick = true;
            _crossHairContextMenuItem.Checked = false;
            _chartContextMenu.Items.Add(_crossHairContextMenuItem);

            _labelsContextMenuItem = new ToolStripMenuItem("Labels", ((Image)(_resources.GetObject("imageText"))), new EventHandler(LabelsChartContextMenuItem_Click));
            _labelsContextMenuItem.CheckOnClick = true;
            _labelsContextMenuItem.Checked = true;
            _chartContextMenu.Items.Add(_labelsContextMenuItem);

            _limitViewContextMenuItem = new ToolStripMenuItem("Limit View", ((Image)(_resources.GetObject("imageElementSelection"))), new EventHandler(LimitViewChartContextMenuItem_Click));
            _limitViewContextMenuItem.CheckOnClick = true;
            _limitViewContextMenuItem.Checked = true;
            _chartContextMenu.Items.Add(_limitViewContextMenuItem);

            _chartContextMenu.Items.Add(new ToolStripSeparator());

            _seriesPropertiesContextMenuItem = new ToolStripMenuItem("Series Properties", null);
            _chartContextMenu.Items.Add(_seriesPropertiesContextMenuItem);

            _chartContextMenu.Items.Add(new ToolStripSeparator());

            _selectedObjectsContextMenuItem = new ToolStripMenuItem("Selected Object(s) Properties");
            _selectedObjectsContextMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(_selectedObjectsContextMenuItem_DropDownItemClicked);
            _chartContextMenu.Items.Add(_selectedObjectsContextMenuItem);

            _customObjectsManager = new CustomObjectsManager();
            _customObjectsManager.Initialize(this);
            
            _customObjectDrawingImage = ((Image)(_resources.GetObject("imageBrush")));
        }

        void _chartContextMenu_Opening(object sender, CancelEventArgs e)
        {
            _autoScrollToEndContextMenuItem.CheckOnClick = true;
            _autoScrollToEndContextMenuItem.Checked = this.AutoScrollToEnd;

            _limitViewContextMenuItem.CheckOnClick = true;
            _limitViewContextMenuItem.Checked = this.LimitedView;

            _crossHairContextMenuItem.CheckOnClick = true;
            _crossHairContextMenuItem.Checked = this.CrosshairVisible;

            _labelsContextMenuItem.CheckOnClick = true;
            _labelsContextMenuItem.Checked = this.ShowSeriesLabels;
        }


        /// <summary>
        /// Add chart series to pane.
        /// </summary>
        /// <param name="series"></param>
        public virtual void Add(ChartSeries series)
        {
            this.Add(series, false, false);
        }

        /// <summary>
        /// Add chart series to pane.
        /// </summary>
        public virtual void Add(ChartSeries series, bool usePaneColorSelector, bool replaceSeriesWithSameName)
        {
            if (replaceSeriesWithSameName)
            {
                this.RemoveByName(series.Name);
            }

            if (_series.Contains(series))
            {// Already present.
                return;
            }

            if (usePaneColorSelector)
            {
                _colorSelector.SetupSeries(series);
            }

            //if (series.ChartType == ChartSeries.ChartTypeEnum.ColoredArea)
            //{// Colored areas better be inserted first, in rendering they will not overlap other drawings.
            //    _series.Insert(0, series);
            //}
            //else
            //{
                _series.Add(series);
            //}

            series.SeriesUpdatedEvent += new ChartSeries.SeriesUpdatedDelegate(series_SeriesUpdatedEvent);
            series.AddedToChart(this.StateId);

            UpdateDrawingSpace();

            this.Invalidate();
        }

        /// <summary>
        /// Retrieve the first series found with this name.
        /// </summary>
        /// <param name="seriesName"></param>
        /// <returns></returns>
        ChartSeries GetSeriesByName(string seriesName)
        {
            for (int i = _series.Count - 1; i >= 0; i--)
            {
                if (_series[i].Name == seriesName)
                {
                    return _series[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Will remove all series with this name.
        /// </summary>
        public virtual void RemoveByName(string seriesName)
        {
            for (int i = _series.Count - 1; i >= 0; i--)
            {
                if (_series[i].Name == seriesName)
                {
                    Remove(_series[i]);
                }
            }
        }

        public virtual bool Remove(ChartSeries series)
        {
            if (_series.Remove(series))
            {
                series.SeriesUpdatedEvent -= new ChartSeries.SeriesUpdatedDelegate(series_SeriesUpdatedEvent);
                series.RemovedFromChart();

                UpdateDrawingSpace();
                this.Invalidate();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Needed since we use invoke to access it.
        /// </summary>
        public void SetChartName(string name)
        {
            _chartName = name;
            if (ParametersUpdatedEvent != null)
            {
                ParametersUpdatedEvent(this);
            }

        }

        protected virtual void series_SeriesUpdatedEvent(ChartSeries series, bool updateUI)
        {
            //TracerHelper.TraceEntry();
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(100), UpdateDrawingSpace);
            
            if (updateUI)
            {
                WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(100), Invalidate);
            }
        }

        public void Clear(bool clearSeries, bool clearCustomObjects)
        {
            _chartName = "";

            for (int i = _series.Count - 1; clearSeries && i >= 0; i--)
            {
                Remove(_series[i]);
            }

            if (clearCustomObjects)
            {
                _customObjectsManager.Clear();
            }
        }

        /// <summary>
        /// Does NOT consider series visibility.
        /// </summary>
        protected void GetSingleSeriesMinMax(ChartSeries series, int? startIndex, int? endIndex, ref float min, ref float max)
        {
            if (series.Visible)
            {
                series.GetTotalMinimumAndMaximum(startIndex, endIndex, ref min, ref max);
            }

            if (min < float.MaxValue && float.IsNaN(min) == false
                && max > float.MinValue && float.IsNaN(max) == false)
            {
                // Provide a 5% advance on min and max.
                float differencePart = Math.Abs(max - min) / 20f;
                max += differencePart;
                min -= differencePart;
            }
        }

        protected virtual void UpdateDrawingSpace()
        {
            RectangleF? newDrawingSpace = null;

            float globalMin = float.MaxValue;
            float globalVisibleMin = float.MaxValue;
            float globalMax = float.MinValue;
            float globalVisibleMax = float.MinValue;

            // Establish drawing space.
            foreach (ChartSeries series in _series)
            {
                float yMin = float.MaxValue;
                float yMax = float.MinValue;

                GetSingleSeriesMinMax(series, null, null, ref yMin, ref yMax);

                globalMin = Math.Min(yMin, globalMin);
                globalMax = Math.Max(yMax, globalMax);

                if (series.Visible)
                {
                    globalVisibleMin = Math.Min(yMin, globalMin);
                    globalVisibleMax = Math.Max(yMax, globalMax);
                }

                if (newDrawingSpace.HasValue)
                {
                    yMin = Math.Min(yMin, newDrawingSpace.Value.Y);
                    yMax = Math.Max(yMax, newDrawingSpace.Value.Height + newDrawingSpace.Value.Y);
                }

                // Series can provide
                if (float.IsInfinity(yMin) == false
                    && float.IsNaN(yMin) == false
                    && float.MinValue != yMin
                    && float.MaxValue != yMin
                    && float.IsInfinity(yMax) == false
                    && float.IsNaN(yMax) == false
                    && float.MinValue != yMax
                    && float.MaxValue != yMax)
                {
                    if (newDrawingSpace.HasValue == false)
                    {
                        newDrawingSpace = new RectangleF();
                    }

                    float width = Math.Max(series.CalculateTotalWidth(_seriesItemWidth, _seriesItemMargin), newDrawingSpace.Value.Width);

                    RectangleF space = new RectangleF(0, yMin, width, yMax - yMin);

                    if (space.Y > 0)
                    {
                        space.Height += space.Y;
                        space.Y = 0;
                    }

                    if (space.Y + space.Height < 0)
                    {
                        float difference = - space.Height - space.Y;
                        space.Height += difference;
                    }

                    newDrawingSpace = space;
                }
             
            }

            if (newDrawingSpace.HasValue == false)
            {
                return;
            }
            _drawingSpace = newDrawingSpace.Value;
            _drawingSpaceDisplayLimit = _drawingSpace;


            if (_autoScrollToEnd)
            {// Now scroll to the end of the updated space.
                RectangleF area = GraphicsWrapper.ActualSpaceToDrawingSpace(_actualDrawingSpaceArea);
                
                // Scroll to end, but also fit zoom, so that all the dataDelivery at the ending is visible.
                // To achieve it all - use a selection of the ending zone.
                float width = area.Width;
                if (width > _drawingSpaceDisplayLimit.Width)
                {
                    width = _drawingSpaceDisplayLimit.Width;
                }
                FitHorizontalAreaToScreen(new RectangleF(_drawingSpaceDisplayLimit.X + _drawingSpaceDisplayLimit.Width - width, _drawingSpaceDisplayLimit.Y, width, _drawingSpaceDisplayLimit.Height));
            }

            // Establish the drawing space limit.
            if (_series.Count > 0)
            {
                if (globalVisibleMax == float.MinValue || globalVisibleMin == float.MaxValue)
                {// Normalize values, when no visible series, select as if all are visible.
                    if (globalMin == float.MaxValue || globalMax == float.MinValue)
                    {
                        globalVisibleMin = 0;
                        globalVisibleMax = 1;
                    }
                    else
                    {
                        globalVisibleMin = globalMin;
                        globalVisibleMax = globalMax;
                    }
                }

                _drawingSpaceDisplayLimit.Y = globalVisibleMin;
                _drawingSpaceDisplayLimit.Height = globalVisibleMax - globalVisibleMin;
            }

            //UpdateYAxisSpacings();

            if (DrawingSpaceUpdatedEvent != null)
            {
                DrawingSpaceUpdatedEvent(this);
            }
        }

        void UpdateYAxisSpacings()
        {
            // Each label needs ~20 px of space, make sure this number is always one or more (height can become negative).
            int prefferedLabelsCount = Math.Max(1, _actualDrawingSpaceArea.Height / 20);
            PointF newHeight = _graphicsWrapper.ActualSpaceToDrawingSpace(new PointF(0, _actualDrawingSpaceArea.Height), false);

            // Establish the proper label spacing.
            double temp = Math.Abs(newHeight.Y / prefferedLabelsCount); //(_drawingSpaceDisplayLimit.Height / 10);
            double power = 0;
            if (temp < 1)
            {
                while (temp > 0 && temp < 0.1)
                {
                    temp *= 10;
                    power++;
                }
            }
            else
            {
                while (temp >= 10)
                {
                    temp /= 10;
                    power--;
                }
            }

            // Round to 0.2, 0.5 or 1.
            if (temp < (0.2 + 0.5) / 2f)
            {
                temp = 0.2;
            }
            else if (temp < (0.5 + 1) / 2f)
            {
                temp = 0.5;
            }
            else if (temp <= 1.5)
            {
                temp = 1;
            }
            else if (temp <= 3)
            {
                temp = 2;
            }
            else if (temp <= 7)
            {
                temp = 5;
            }
            else
            {
                temp = 10;
            }

            _autoYAxisLabelSpacing = (float)temp * (float)Math.Pow(10, -power);
            _drawingSpaceGrid.HorizontalLineSpacing = _autoYAxisLabelSpacing * 2;
        }

        protected override void OnPaint(PaintEventArgs paintArgs)
        {
            base.OnPaint(paintArgs);
            Draw(paintArgs);
        }

        protected virtual void Draw(PaintEventArgs paintArgs)
        {
            _graphicsWrapper.SetGraphics(paintArgs.Graphics);

            if (this.DesignMode)
            {
                return;
            }

            _graphicsWrapper.SmoothingMode = _smoothingMode;
            if (_fill != null)
            {
                _graphicsWrapper.FillRectangle(_fill, _graphicsWrapper.VisibleClipBounds);
            }

            // The leading date series, can be null if there is no dated series in the chart.
            TimeBasedChartSeries timeBasedSeries = null;

            // Series.
            foreach (ChartSeries series in _series)
            {
                if (series is TimeBasedChartSeries)
                {
                    if (timeBasedSeries != null)
                    {// There is another dataDelivery series already. Two dataDelivery series are not handled.
                        SystemMonitor.Throw("Two data series in a single chart pane are currently not supported.");
                    }
                    timeBasedSeries = (TimeBasedChartSeries)series;
                }
            }

            DrawInitialActualSpaceOverlays(_graphicsWrapper, timeBasedSeries);

            if (paintArgs.ClipRectangle.IntersectsWith(_actualDrawingSpaceArea))
            {
                // Drawing area background.
                _graphicsWrapper.FillRectangle(_actualDrawingSpaceAreaFill, _actualDrawingSpaceArea);

                // Clip.
                _graphicsWrapper.ResetClip();
                _graphicsWrapper.SetClip(_actualDrawingSpaceArea);

                // Drawing space.
                _graphicsWrapper.DrawingSpaceMode = true;

                DrawDrawingSpace(_graphicsWrapper);

                DrawSelection(_graphicsWrapper);

                // Actual space.
                _graphicsWrapper.DrawingSpaceMode = false;

                // Unclip.
                _graphicsWrapper.ResetClip();
            }

            DrawPostActualSpaceOverlays(_graphicsWrapper);

            if (_crosshair.Visible && _crosshair.LastPoint.HasValue)
            {
                DrawCrossHairLocationInfo(paintArgs.Graphics, this.PointToClient(_crosshair.LastPoint.Value));
            }

            _graphicsWrapper.SetGraphics(null);
        }

        void DrawSelection(GraphicsWrapper g)
        {
            if (CurrentSelectionRectangle.HasValue == false)
            {
                return;
            }

            RectangleF selectionRectangle = CurrentSelectionRectangle.Value;
            
            if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0 &&
                _lastDrawingSpaceMouseRightButtonPosition.HasValue && _currentDrawingSpaceMousePosition.HasValue)
            {
                if (_selectionPen != null)
                {
                    g.DrawRectangle(_selectionPen, selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width, selectionRectangle.Height);
                }

                if (_selectionFill != null)
                {
                    g.FillRectangle(_selectionFill, selectionRectangle);
                }
            }
        }


        protected virtual void DrawGraphicSeriesLabels(GraphicsWrapper g, int initialMarginLeft)
        {
            _currentLabelsRectangles = new Rectangle[_series.Count];

            for (int i = 0; i < _series.Count; i++)
            {
                if (i == 0)
                {
                    _currentLabelsRectangles[0].X = initialMarginLeft;
                }
                else
                {
                    _currentLabelsRectangles[i].X = _currentLabelsRectangles[i - 1].Right + (int)_labelsMargin;
                }

                _currentLabelsRectangles[i].Y = (int)_labelsTopMargin;

                SizeF seriesSize = g.MeasureString(_series[i].Name, _labelsFont);
                _currentLabelsRectangles[i].Size = new Size((int)seriesSize.Width, (int)seriesSize.Height);

                int iconWidth = 18;

                // Add space for series icon
                _currentLabelsRectangles[i].Width += iconWidth;

                if (_labelsFill != null)
                {
                    g.FillRectangle(_labelsFill, _currentLabelsRectangles[i]);
                }

                if (_labelsFont != null)
                {
                    g.DrawString(_series[i].Name, _labelsFont, _labelsFontBrush, _currentLabelsRectangles[i].X + iconWidth, _currentLabelsRectangles[i].Y);
                }

                _series[i].DrawSeriesIcon(g, new Rectangle(_currentLabelsRectangles[i].X + 2, _currentLabelsRectangles[i].Y + 2, 14, _currentLabelsRectangles[i].Height - 4));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void CalculateActualDrawingSpaceAreaMarginTopAndBottom()
        {
            if (XAxisLabels)
            {
                _actualDrawingSpaceAreaMarginBottom = 20;
            }
            else
            {
                _actualDrawingSpaceAreaMarginBottom = 8;
                if (_appearanceMode == AppearanceModeEnum.SuperCompact)
                {
                    _actualDrawingSpaceAreaMarginBottom = 2;
                }
            }

            if (ShowSeriesLabels)
            {
                _actualDrawingSpaceAreaMarginTop = 48;
            }
            else
            {
                switch (_appearanceMode)
                {
                    case AppearanceModeEnum.Normal:
                        _actualDrawingSpaceAreaMarginTop = 30;
                        break;
                    case AppearanceModeEnum.Compact:
                        _actualDrawingSpaceAreaMarginTop = 30;
                        break;
                    case AppearanceModeEnum.SuperCompact:
                        _actualDrawingSpaceAreaMarginTop = 2;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Initial part of drawing, called by Draw().
        /// Method takes care of setting up the basic drawing parameters of the chart, required for the drawing
        /// to be done; also draws the system overlays.
        /// </summary>
        protected virtual void DrawInitialActualSpaceOverlays(GraphicsWrapper g, TimeBasedChartSeries timeBasedSeries)
        {
            // Since this is dependant on current graphics scaling, also recalculate it here now.
            UpdateYAxisSpacings();

            CalculateActualDrawingSpaceAreaMarginTopAndBottom();

            if (XAxisLabels)
            {// X Axis Labels

                float totalItemWidth = _seriesItemWidth + _seriesItemMargin;
                float actualXSpacing = _xAxisLabelSpacing * totalItemWidth;

                // Consider X axis label scaling.
                int xScaling = Math.Abs((int)(1 / _graphicsWrapper.DrawingSpaceTransformClone.Elements[0]));
                if (xScaling > 1)
                {
                    actualXSpacing = actualXSpacing * xScaling;
                }

                // Set starting to the closes compatible positionactualXSpacing
                // TODO : this can be optimized further by narrowing the range of xStart to end
                float xStart = (int)(_drawingSpaceDisplayLimit.X / actualXSpacing);
                xStart = xStart * actualXSpacing;

                SizeF xAxisLabelTypicalSize = g.MeasureString("00/00/000 00:00", _axisLabelsFont);

                for (float i = xStart; i < _drawingSpaceDisplayLimit.X + _drawingSpaceDisplayLimit.Width; i += actualXSpacing)
                {
                    PointF point = GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(i, 0), true);

                    if (point.X > (_actualDrawingSpaceArea.X - 2)
                        && point.X < (_actualDrawingSpaceArea.X + _actualDrawingSpaceArea.Width + 2 - xAxisLabelTypicalSize.Width))
                    {
                        int index = (int)(i / totalItemWidth);
                        string message = string.Empty;
                        if (timeBasedSeries != null)
                        {// If there is a leading dateAssignedSeries show labels based on its timing.
                            if (index < timeBasedSeries.ItemsCount)
                            {
                                message = GeneralHelper.GetShortDateTime(timeBasedSeries.GetTimeAtIndex(index));
                            }
                        }
                        else
                        {
                            message = index.ToString(_xAxisLabelsFormat);
                        }

                        if (_axisLabelsFont != null && _xAxisLabelsFontBrush != null)
                        {
                            g.DrawString(message, _axisLabelsFont, _xAxisLabelsFontBrush, point.X, _actualDrawingSpaceArea.Y + _actualDrawingSpaceArea.Height);
                        }

                        // Draw the small line indicating where the string applies for.
                        g.DrawLine(_actualDrawingSpaceAreaBorderPen, point.X, _actualDrawingSpaceArea.Y + _actualDrawingSpaceArea.Height, point.X, _actualDrawingSpaceArea.Y + _actualDrawingSpaceArea.Height + 5);
                    }
                }
            } // X Axis Labels.

            _actualDrawingSpaceAreaMarginLeft = _additionalDrawingSpaceAreaMarginLeft + 5;
            _actualDrawingSpaceAreaMarginRight = _additionalDrawingSpaceAreaMarginRight + 5;

            if (YAxisLabels)
            {// Y Axis Labels.

                int yAxisLabelsWidth = 0;

                // Set starting to the closes compatible positionactualYSpacing
                int maxDecimalPlaces = _autoYAxisLabelSpacing.ToString().Length - 1;

                float yStart = (int)(_drawingSpaceDisplayLimit.Y / _autoYAxisLabelSpacing);
                yStart = yStart * _autoYAxisLabelSpacing;

                // Round off to a fixed number of post decimal point digits, will only work for values under 1
                yStart = (float)Math.Round(yStart, maxDecimalPlaces);

                // This must auto adjust to format the number properly and always fit in 6 spaces.
                // Specify positive, negative and zero formats.
                //_yAxisLabelsFormat = " #0.###;-#0.###; Zero";

                int separatorPosition = _yAxisLabelsFormat.IndexOf(";", 0) - 1;

                // The default is 6 positions total for the y axis labels.
                yAxisLabelsWidth = ((int)g.MeasureString(_yAxisLabelsFormat.Substring(0, separatorPosition), _axisLabelsFont).Width);

                // Calculate the current margin and confirm with any controling subscriber.
                int labelSpacingMargin = yAxisLabelsWidth;

                if (_yAxisLabelsPosition == YAxisLabelPosition.Left ||
                    _yAxisLabelsPosition == YAxisLabelPosition.Both)
                {
                    _actualDrawingSpaceAreaMarginLeft += labelSpacingMargin;
                }

                if (_yAxisLabelsPosition == YAxisLabelPosition.Right ||
                    _yAxisLabelsPosition == YAxisLabelPosition.Both)
                {
                    _actualDrawingSpaceAreaMarginRight += labelSpacingMargin;
                }

                if (_yAxisLabelsPosition != YAxisLabelPosition.None)
                {
                    // A maximum of 10000 steps allowed for this drawing, otherwise some bug is probably present.
                    if ((_drawingSpaceDisplayLimit.Y + _drawingSpaceDisplayLimit.Height - yStart) / _autoYAxisLabelSpacing < 10000)
                    {
                        // Pass 2 - actually draw the labels and label lines at the established and confirmed location.
                        for (float i = yStart; i < _drawingSpaceDisplayLimit.Y + _drawingSpaceDisplayLimit.Height; i += _autoYAxisLabelSpacing)
                        {
                            float iRound = (float)Math.Round(i, maxDecimalPlaces);
                            PointF point = GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(0, iRound), true);

                            if (point.Y <= _actualDrawingSpaceArea.Y - 5 ||
                                point.Y >= _actualDrawingSpaceArea.Y + _actualDrawingSpaceArea.Height)
                            {
                                continue;
                            }

                            // Draw labels on the left.
                            if (_yAxisLabelsPosition == YAxisLabelPosition.Left || _yAxisLabelsPosition == YAxisLabelPosition.Both)
                            {
                                if (_axisLabelsFont != null && _yAxisLabelsFontBrush != null)
                                {
                                    g.DrawString((iRound).ToString(_yAxisLabelsFormat), _axisLabelsFont, _yAxisLabelsFontBrush, _actualDrawingSpaceAreaMarginLeft - yAxisLabelsWidth - 3, point.Y);
                                }

                                // Draw the small line indicating where the string applies for.
                                g.DrawLine(_actualDrawingSpaceAreaBorderPen, _actualDrawingSpaceAreaMarginLeft - 5, point.Y, _actualDrawingSpaceAreaMarginLeft, point.Y);
                            }

                            // Draw labels on the right.
                            if (_yAxisLabelsPosition == YAxisLabelPosition.Right || _yAxisLabelsPosition == YAxisLabelPosition.Both)
                            {
                                if (_axisLabelsFont != null && _yAxisLabelsFontBrush != null)
                                {
                                    g.DrawString((iRound).ToString(_yAxisLabelsFormat), _axisLabelsFont, _yAxisLabelsFontBrush,
                                        this.Width - yAxisLabelsWidth - 3, point.Y);
                                }

                                if (point.Y >= _actualDrawingSpaceArea.Y)
                                {
                                    // Draw the small line indicating where the string applies for.
                                    g.DrawLine(_actualDrawingSpaceAreaBorderPen,
                                        this.Width - yAxisLabelsWidth - 6, point.Y,
                                        this.Width - yAxisLabelsWidth - 3, point.Y);
                                }
                            }
                        }
                    }
                    else
                    {
                        SystemMonitor.OperationError("Too many steps in drawing planned.");
                    }
                }
            }

            foreach (ChartSeries series in _series)
            {
                series.DrawInitialActualSpaceOverlays(this, g);
            }

            UpdateActualDrawingSpaceArea();

            // Actual space, drawing area, grid.
            _actualSpaceGrid.Draw(g, _actualDrawingSpaceArea, _actualDrawingSpaceArea, 1);

            if (ShowSeriesLabels)
            {
                DrawGraphicSeriesLabels(g, _actualDrawingSpaceArea.Left);
            }

            // Show 
            if (_customObjectsManager.IsBuildingObject)
            {
                g.DrawImageUnscaledAndClipped(_customObjectDrawingImage, new Rectangle(4, (int)LabelsTopMargin, _customObjectDrawingImage.Width, _customObjectDrawingImage.Height));
            }
        }

        protected virtual void DrawPostActualSpaceOverlays(GraphicsWrapper g)
        {
            if (_titleFont != null && _titleFontBrush != null)
            {
                // Title
                SizeF titleSize = g.MeasureString(_chartName, _titleFont);
                Rectangle titleRectangle = new Rectangle(_actualDrawingSpaceArea.Left, _actualDrawingSpaceArea.Top, (int)titleSize.Width, (int)titleSize.Height);
                g.DrawString(_chartName, _titleFont, _titleFontBrush, titleRectangle.Location);


                PointF drawingLocation = new PointF(_actualDrawingSpaceArea.Left, titleRectangle.Height + _actualDrawingSpaceArea.Top);
                // After the title, render any messages the chart series might have.
                foreach (ChartSeries series in _series)
                {
                    drawingLocation = series.DrawCustomMessages(this, g, drawingLocation);
                }
            }

            if (_actualDrawingSpaceAreaBorderPen != null)
            {
                // Border
                g.DrawRectangle(_actualDrawingSpaceAreaBorderPen, _actualDrawingSpaceArea.X - 1, _actualDrawingSpaceArea.Y - 1, _actualDrawingSpaceArea.Width + 1, _actualDrawingSpaceArea.Height + 1);
            }
        }

        void DrawDrawingSpace(GraphicsWrapper g)
        {
            RectangleF drawingSpaceClipping = _actualDrawingSpaceArea;
            drawingSpaceClipping.X -= _seriesItemMargin + _seriesItemWidth;
            drawingSpaceClipping.Y -= _seriesItemMargin + _seriesItemWidth;

            drawingSpaceClipping.Width += 2 * (_seriesItemMargin + _seriesItemWidth);
            drawingSpaceClipping.Height += 2 * (_seriesItemMargin + _seriesItemWidth);

            drawingSpaceClipping = GraphicsWrapper.ActualSpaceToDrawingSpace(drawingSpaceClipping);
            //drawingSpaceClipping.Y = DrawingSpace.Y - 10;
            //drawingSpaceClipping.Height = DrawingSpace.Height + 10;

            // Grid.
            _drawingSpaceGrid.Draw(g, drawingSpaceClipping, _drawingSpace, _seriesItemMargin + _seriesItemWidth);

            // Show clipping rectangle.
            if (ShowClippingRectangle)
            {
                Pen clippingRectanglePen = (Pen)Pens.DarkGray.Clone();
                clippingRectanglePen.DashStyle = DashStyle.Dash;

                g.DrawRectangle(clippingRectanglePen, drawingSpaceClipping.X, drawingSpaceClipping.Y, drawingSpaceClipping.Width, drawingSpaceClipping.Height);
            }

            // Draw custom objects - pre series.
            _customObjectsManager.Draw(g, drawingSpaceClipping, CustomObject.DrawingOrderEnum.PreSeries);

            // Series.
            foreach (ChartSeries series in _series)
            {
                series.Draw(this, g, CurrentUnitUnification, drawingSpaceClipping, _seriesItemWidth, _seriesItemMargin);
            }

            // Draw custom objects - post series.
            _customObjectsManager.Draw(g, drawingSpaceClipping, CustomObject.DrawingOrderEnum.PostSeries);


        }

        /// <summary>
        /// Called every time size chanes and on each draw too.
        /// </summary>
        protected virtual void UpdateActualDrawingSpaceArea()
        {
            if (this.Width <= 0 || this.Height <= 0)
            {
                return;
            }

            Rectangle newArea = new Rectangle(_actualDrawingSpaceAreaMarginLeft, _actualDrawingSpaceAreaMarginTop,
                this.Width - _actualDrawingSpaceAreaMarginLeft - _actualDrawingSpaceAreaMarginRight, 
                this.Height - _actualDrawingSpaceAreaMarginTop - _actualDrawingSpaceAreaMarginBottom);

            _actualDrawingSpaceArea = newArea;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Size.Width <= 0 || Size.Height <= 0)
            {
                return;
            }

            UpdateActualDrawingSpaceArea();

            if (_lastControlSize.IsEmpty == false)
            {// Not the first run.
                
                float xScaling = (float)_actualDrawingSpaceArea.Width / (float)_lastActualDrawingSpaceArea.Width;
                float yScaling = (float)_actualDrawingSpaceArea.Height / (float)_lastActualDrawingSpaceArea.Height;
                
                // Default - base zooming on left.
                PointF pointActualSpace = new PointF(_actualDrawingSpaceArea.Left, _actualDrawingSpaceArea.Top);
                PointF drawSpacePoint = GraphicsWrapper.ActualSpaceToDrawingSpace(pointActualSpace, true);

                this.HandleScale(drawSpacePoint, xScaling, yScaling);
            }

            //// If there is a problem here, and it requires the rescalings done 2 times, for the effect to become evident now.
            //{// Second run.
            //    // Default - base zooming on left.
            //    PointF pointActualSpace = new PointF(_actualDrawingSpaceArea.Left, _actualDrawingSpaceArea.Top);
            //    PointF drawSpacePoint = GraphicsWrapper.ActualSpaceToDrawingSpace(pointActualSpace, true);
            //    this.HandleScale(drawSpacePoint, 1, 1);
            //}
            
            _lastActualDrawingSpaceArea = _actualDrawingSpaceArea;
            _lastControlSize = this.Size;
            this.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (_actualDrawingSpaceArea.Contains(e.Location) == false)
            {// Not in drawing area.
                return;
            }

            if (ScrollModeEnum.None == ScrollMode)
            {// Do nothing on scroll.
                return;
            }

            float delta = e.Delta;
            float ScalingFactor = 1000;

            // Normalize delta.
            delta = (float)Math.Max(delta, -0.8 * ScalingFactor);
            delta = (float)Math.Min(delta, 0.8 * ScalingFactor);

            float scaleApplied = 1 + delta / ScalingFactor;

            // Default - base zooming on left.
            PointF pointActualSpace = new PointF(_actualDrawingSpaceArea.Left, _actualDrawingSpaceArea.Top);
            PointF drawSpacePoint = GraphicsWrapper.ActualSpaceToDrawingSpace(pointActualSpace, true);

            ScrollModeEnum scrollMode = ScrollMode;
            if (_isControlKeyDown && _isShiftKeyDown)
            {
                scrollMode = ScrollModeEnum.ZoomToMouse;
            }
            else if (_isControlKeyDown)
            {
                scrollMode = ScrollModeEnum.HorizontalZoom;
            }
            else if (_isShiftKeyDown)
            {
                scrollMode = ScrollModeEnum.VerticalZoom;
            }


            if (ScrollModeEnum.HorizontalZoom == scrollMode)
            {// Scale horizontal.

                HandleScale(drawSpacePoint, scaleApplied, 1);
            }
            else if (ScrollModeEnum.VerticalZoom == scrollMode)
            {// Scale vertical.

                HandleScale(drawSpacePoint, 1, scaleApplied);
            }
            else if (ScrollModeEnum.HorizontalScroll == scrollMode
                    || ScrollModeEnum.HorizontalScrollAndFit == scrollMode)
            {
                // Elements 0 gives the zoom in X, so divide to scroll faster on faster zooms.
                float zoomElement = _graphicsWrapper.DrawingSpaceTransformClone.Elements[0];
                Point dragVector = new Point((int)(-e.Delta / zoomElement), 0);
                HandlePan(true, dragVector);

                if (ScrollModeEnum.HorizontalScrollAndFit == scrollMode)
                {// After scrolling also fit the dataDelivery
                    RectangleF area = GraphicsWrapper.ActualSpaceToDrawingSpace(_actualDrawingSpaceArea);
                    FitHorizontalAreaToScreen(area);
                }

            }
            else if (ScrollModeEnum.VerticalScroll == scrollMode)
            {
                // Elements 0 gives the zoom in X, so divide to scroll faster on faster zooms.
                float zoomElement = _graphicsWrapper.DrawingSpaceTransformClone.Elements[0];
                Point dragVector = new Point(0, (int)(-e.Delta / zoomElement));
                HandlePan(_limitedView, dragVector);
            }
            else if (ScrollModeEnum.ZoomToMouse == scrollMode)
            {// Base zooming on mouse coordinates.
                drawSpacePoint = GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);

                HandleScale(drawSpacePoint, scaleApplied, scaleApplied);
            }
                    
            this.Refresh();
        }

        void MenuSeriesItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ChartSeries series;
            if (item.OwnerItem != null)
            {
                series = (ChartSeries)item.OwnerItem.Tag;
            }
            else
            {
                series = (ChartSeries)item.Owner.Tag;
            }

            series.SetSelectedChartType((string)item.Tag);

            this.Refresh();
        }

        void seriesPropertiesMenuItem_Click(object sender, EventArgs e)
        {
            ChartSeries series = (ChartSeries)(((ToolStripItem)sender).Tag);
            CustomPropertiesControl control = new CustomPropertiesControl();
            control.SelectedObject = series;
            HostingForm form = new HostingForm(series.Name + " Properties", control);
            
            form.MaximizeBox = false;
            form.ShowCloseButton = true;
            form.Show();

            form.HandleDestroyed += delegate(object inSender, EventArgs inE)
            {// On close, update to catch any changes.
                this.Invalidate();
            };
        }

        /// <summary>
        /// Helper function, show main chart pane context menu.
        /// </summary>
        /// <param name="position"></param>
        void ShowChartContextMenu(Point position)
        {
            _crossHairContextMenuItem.Checked = this.CrosshairVisible;
            _labelsContextMenuItem.Checked = this.ShowSeriesLabels;
            _limitViewContextMenuItem.Checked = this.LimitedView;

            if (_customObjectsManager.SelectedDynamicCustomObjects.Count > 0)
            {
                _selectedObjectsContextMenuItem.Enabled = true;

                _selectedObjectsContextMenuItem.DropDownItems.Clear();
                
                foreach (DynamicCustomObject dynamicObject in _customObjectsManager.SelectedDynamicCustomObjects)
                {
                    ToolStripItem item = _selectedObjectsContextMenuItem.DropDownItems.Add(dynamicObject.Name, null);
                    item.Tag = dynamicObject;
                }

                // Allow the chart series to interact with the menu as well, since they may also be showing some dynamic content.
                foreach (ChartSeries series in _series)
                {
                    series.OnShowChartContextMenu(_chartContextMenu, _selectedObjectsContextMenuItem);
                }
            }
            else
            {
                _selectedObjectsContextMenuItem.Enabled = false;
            }

            _seriesPropertiesContextMenuItem.DropDownItems.Clear();
            foreach (ChartSeries series in _series)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)_seriesPropertiesContextMenuItem.DropDownItems.Add(series.Name);
                SetupMenuToSeriesProperties(series, null, item);
            }

            _chartContextMenu.Show(this, position);
        }

        void LabelsChartContextMenuItem_Click(object param, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)param;
            this.ShowSeriesLabels = item.Checked;
            if (ParametersUpdatedEvent != null)
            {
                ParametersUpdatedEvent(this);
            }
            this.Refresh();
        }

        void LimitViewChartContextMenuItem_Click(object param, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)param;
            this.LimitedView = item.Checked;
            if (ParametersUpdatedEvent != null)
            {
                ParametersUpdatedEvent(this);
            }
            this.Refresh();
        }

        void AutoScrollToEndChartContextMenuItem_Click(object param, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)param;
            this.AutoScrollToEnd = item.Checked;
            if (ParametersUpdatedEvent != null)
            {
                ParametersUpdatedEvent(this);
            }
            this.Refresh();
        }

        void CrosshairChartContextMenuItem_Click(object param, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)param;
            this.CrosshairVisible = item.Checked;
            if (ParametersUpdatedEvent != null)
            {
                ParametersUpdatedEvent(this);
            }
            this.Refresh();
        }

        void _selectedObjectsContextMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            if (item.Tag is DynamicCustomObject)
            {
                DynamicCustomObject dynamicObject = (DynamicCustomObject)item.Tag;
                CustomPropertiesControl control = new CustomPropertiesControl();
                control.SelectedObject = dynamicObject;
                control.AutoSize = true;
                HostingForm form = new HostingForm("Properties", control);
                form.Width = 400;
                form.Height = 600;
                form.Show();
            }
            else
            {// Try the chart series - this is an item of theirs.
                lock (this)
                {
                    foreach (ChartSeries series in _series)
                    {
                        series.OnChartContextMenuItemClicked(item);
                    }
                }
            }

        }

        void FitToScreenChartContextMenuItem_Click(object param, EventArgs e)
        {
            this.FitDrawingSpaceToScreen(true, true);
            this.Refresh();
        }

        void FitHorizontalChartContextMenuItem_Click(object param, EventArgs e)
        {
            this.FitDrawingSpaceToScreen(true, false);
            this.Refresh();
        }
        
        void FitVerticalChartContextMenuItem_Click(object param, EventArgs e)
        {
            this.FitDrawingSpaceToScreen(false, false);
            this.Refresh();
        }

        void ZoomInChartContextMenuItem_Click(object param, EventArgs e)
        {
            this.ZoomIn(2);
        }

        void ZoomOutChartContextMenuItem_Click(object param, EventArgs e)
        {
            this.ZoomOut(0.5f);
        }

        void AppearanceChartContextMenuItem_Click(object param, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)param;
            this.SetAppearanceScheme((AppearanceSchemeEnum)Enum.Parse(typeof(AppearanceSchemeEnum), item.Tag as string));
            this.Refresh();
        }
        
        void ScrollChartContextMenuItem_Click(object param, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)param;
            this.ScrollMode = (ScrollModeEnum)Enum.Parse(typeof(ScrollModeEnum), item.Tag as string);
            if (ParametersUpdatedEvent != null)
            {
                ParametersUpdatedEvent(this);
            }
        }
        
        void SelectionChartContextMenuItem_Click(object param, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)param;
            this.RightMouseButtonSelectionMode = (SelectionModeEnum)Enum.Parse(typeof(SelectionModeEnum), item.Tag as string);
            if (ParametersUpdatedEvent != null)
            {
                ParametersUpdatedEvent(this);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (_customObjectsManager != null && _customObjectsManager.OnMouseDoubleClick(e))
            {// Handled by custom objects manager.
                return;
            }

            PointF drawingSpaceLocation = GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);
            foreach (ChartSeries series in _series)
            {
                series.TrySelect(GraphicsWrapper.DrawingSpaceTransformClone, drawingSpaceLocation, DefaultAbsoluteSelectionMargin, true);
            }
        }

        /// <summary>
        /// Helper function allowing to setup a menu with context items related to properties of this series.
        /// Pass EITHER a menu to get filled with the items OR an item to get filled with subItems. No need to pass both.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="menu"></param>
        void SetupMenuToSeriesProperties(ChartSeries series, ToolStrip menu, ToolStripMenuItem menuItem)
        {
            SystemMonitor.CheckError(menu == null || menuItem == null, "Pass only a menu or a menuItem.");

            string[] seriesTypeNames = series.ChartTypes;

            if (menu != null)
            {
                menu.Items.Clear();
                menu.Tag = series;
            }

            if (menuItem != null)
            {
                menuItem.DropDownItems.Clear();
                menuItem.Tag = series;
            }

            for (int j = 0; j < seriesTypeNames.Length; j++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(seriesTypeNames[j]);
                
                if (menu != null)
                {
                    menu.Items.Add(item);
                }

                if (menuItem != null)
                {
                    menuItem.DropDownItems.Add(item);
                }

                item.Click += new EventHandler(MenuSeriesItem_Click);
                item.Checked = series.SelectedChartType == seriesTypeNames[j];

                if (item.Checked)
                {// Mark the currently selected item
                    item.Image = (Image)(_resources.GetObject("imageDot"));
                }
                item.Tag = seriesTypeNames[j];
            }

            ToolStripItem propertiesMenuItem = null;
            if (menu != null)
            {
                menu.Items.Add(new ToolStripSeparator());
                propertiesMenuItem = menu.Items.Add("Properties");
            }

            if (menuItem != null)
            {
                menuItem.DropDownItems.Add(new ToolStripSeparator());
                propertiesMenuItem = menuItem.DropDownItems.Add("Properties");
            }
            
            if (propertiesMenuItem != null)
            {
                propertiesMenuItem.Click += new EventHandler(seriesPropertiesMenuItem_Click);
                propertiesMenuItem.Tag = series;
            }
        }

        /// <summary>
        /// Mouse clicked.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (ShowSeriesLabels)
            {
                for (int i = 0; i < _currentLabelsRectangles.Length; i++)
                {// Find if click was inside one of the labels.
                    if (_currentLabelsRectangles[i].Contains(e.Location))
                    {// Label clicked.
                        if (e.Button == MouseButtons.Left)
                        {// Show/Hide series.
                            _series[i].Visible = !_series[i].Visible;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {// Prepare context sensitive right click menu items.
                            SetupMenuToSeriesProperties(_series[i], _seriesTypeDynamicContextMenu, null);
                            _seriesTypeDynamicContextMenu.Show(this, e.Location);
                        }

                        this.Invalidate();
                        return;
                    }
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                if (CurrentUserSelectedRectangle.HasValue)
                {
                    PointF actualSpace = GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(CurrentUserSelectedRectangle.Value.Width, CurrentUserSelectedRectangle.Value.Height), false);

                    if (actualSpace.X < MinimumAbsoluteSelectionWidth)
                    {// Selection applied is too small and a context menu is displayed.
                        ShowChartContextMenu(e.Location);
                    }
                }
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_customObjectsManager != null && _customObjectsManager.OnMouseDown(e))
            {
                return;
            }

            this.Focus();

            if (_actualDrawingSpaceArea.Contains(e.Location) == false)
            {// Not clicked in drawing area.
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                _lastDrawingSpaceMouseDownLeftButton = GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);
            }
            if (e.Button == MouseButtons.Right)
            {
                _lastDrawingSpaceMouseRightButtonPosition = GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                _lastDrawingSpaceMouseDownMiddleButton = GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_customObjectsManager != null && _customObjectsManager.OnMouseUp(e))
            {
                return;
            }

            _currentDrawingSpaceMousePosition = GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);

            if (e.Button == MouseButtons.Left)
            {
                _lastDrawingSpaceMouseDownLeftButton = null;
            }
            if (e.Button == MouseButtons.Right)
            {
                if (CurrentSelectionRectangle.HasValue)
                {
                    PointF actualSpace = GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(CurrentUserSelectedRectangle.Value.Width, CurrentUserSelectedRectangle.Value.Height), false);

                    if (actualSpace.X < MinimumAbsoluteSelectionWidth
                        || float.IsNaN(CurrentUserSelectedRectangle.Value.Height)
                        || float.IsInfinity(CurrentUserSelectedRectangle.Value.Height))
                    {// Minimum selection applied.
                        this.Invalidate();
                    }
                    else
                    {
                        HandleSelect(CurrentSelectionRectangle.Value);
                    }
                }
                _lastDrawingSpaceMouseRightButtonPosition = null;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                _lastDrawingSpaceMouseDownMiddleButton = null;
            }
        }

        /// <summary>
        /// Draw information related to the position of the crosshair.
        /// </summary>
        /// <param name="g">The graphics to be used for the drawing, or null to create new one using CreateGraphics()</param>
        protected void DrawCrossHairLocationInfo(Graphics g, Point mouseLocation)
        {
            if (g == null)
            {
                // Show cross hair location requestMessage.
                // TODO: Do not use createGraphics...
                g = this.CreateGraphics();
            }

            PointF drawingPoint = GraphicsWrapper.ActualSpaceToDrawingSpace(mouseLocation, true);
            float totalItemWidth = _seriesItemWidth + _seriesItemMargin;
            int index = (int)(drawingPoint.X / totalItemWidth);

            // The leading date series, can be null if there is no dated series in the chart.
            TimeBasedChartSeries timeBasedSeries = null;
            // Series.
            foreach (ChartSeries series in _series)
            {
                if (series is TimeBasedChartSeries)
                {
                    // There is another dataDelivery series already. Two dataDelivery series are not handled.
                    SystemMonitor.CheckThrow(timeBasedSeries == null, "Two time based data series in a single chart pane are currently not supported.");
                    timeBasedSeries = (TimeBasedChartSeries)series;
                }
            }

            string message = string.Empty;
            if (timeBasedSeries != null)
            {// If there is a leading dateAssignedSeries show labels based on its timing.
                if (index >= 0 && index < timeBasedSeries.ItemsCount )
                {
                    message = GeneralHelper.GetShortDateTime(timeBasedSeries.GetTimeAtIndex(index));
                }
            }
            else
            {
                message = index.ToString(_xAxisLabelsFormat);
            }

            // Y value outside of chart area.
            if (mouseLocation.Y < 0 || mouseLocation.Y > this.Height)
            {
                message = message + ", NA";
            }
            else
            {
                message = message + ", " + drawingPoint.Y.ToString(_yAxisLabelsFormat);
            }

            SizeF stringSize;
            if (this is MasterChartPane)
            {
                stringSize = g.MeasureString(message, _axisLabelsFont);
            }
            else
            {// Make srue to have the string cover everything needed, and some more.
                stringSize = g.MeasureString("00.0000, 00.0000", _axisLabelsFont);
            }

            g.FillRectangle(_fill, new Rectangle(_actualDrawingSpaceArea.X + _actualDrawingSpaceArea.Width - (int)stringSize.Width - 10, (int)_labelsTopMargin, (int)stringSize.Width + 10, (int)stringSize.Height));
            
            Brush brush = _xAxisLabelsFontBrush;
            if (brush == null)
            {
                brush = SystemBrushes.ControlText;
            }

            g.DrawString(message, _axisLabelsFont, brush, _actualDrawingSpaceArea.X + _actualDrawingSpaceArea.Width - stringSize.Width, _labelsTopMargin);
        }

        /// <summary>
        /// Render crosshair at screen location.
        /// </summary>
        public virtual void DrawCrossHairAt(Point screenLocation)
        {
            _crosshair.DrawAt(this.RectangleToScreen(_actualDrawingSpaceArea), screenLocation, true);
            DrawCrossHairLocationInfo(null, this.PointToClient(screenLocation));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_customObjectsManager != null && _customObjectsManager.OnMouseMove(e))
            {// CompletionEvent handled by user objects handler.
                return;
            }

            if (_crosshair.Visible && _actualDrawingSpaceArea.Contains(e.Location))
            {
                DrawCrossHairAt(this.PointToScreen(e.Location));
            }

            Cursor = Cursors.Default;
            
            // Show hand on label point.
            if (ShowSeriesLabels)
            {
                foreach (Rectangle rectangle in _currentLabelsRectangles)
                {
                    if (rectangle.Contains(e.Location))
                    {
                        Cursor = Cursors.Hand;
                        break;
                    }
                }
            }

            _currentDrawingSpaceMousePosition = GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);

            if ((_rightMouseButtonSelectionMode == SelectionModeEnum.RectangleZoom 
                || _rightMouseButtonSelectionMode == SelectionModeEnum.HorizontalZoom
                || _rightMouseButtonSelectionMode == SelectionModeEnum.VerticalZoom)
                && _actualDrawingSpaceArea.Contains(e.Location)
                && _lastDrawingSpaceMouseRightButtonPosition != null )
            {// Mark current drawing point for selection purposes.
                this.Invalidate();
            }

            if (e.Button == MouseButtons.Left && _lastDrawingSpaceMouseDownLeftButton != null)
            {
                Cursor = Cursors.Hand;

                PointF dragVector = new PointF(_currentDrawingSpaceMousePosition.Value.X - _lastDrawingSpaceMouseDownLeftButton.Value.X, _currentDrawingSpaceMousePosition.Value.Y - _lastDrawingSpaceMouseDownLeftButton.Value.Y);
                HandlePan(_limitedView && _isControlKeyDown == false, dragVector);

                _currentDrawingSpaceMousePosition = GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);
                _lastDrawingSpaceMouseDownLeftButton = _currentDrawingSpaceMousePosition;

                this.Refresh();
            }

            
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            // This helps with the cross hair bug.
            //this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_customObjectsManager != null
                && _customObjectsManager.OnMouseLeave(e))
            {// CompletionEvent handled by user objects handler.
                return;
            }

            this.Cursor = Cursors.Default;

            // This helps with the cross hair bug.
            //this.Invalidate();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (_customObjectsManager != null && _customObjectsManager.OnKeyPress(e))
            {// CompletionEvent handled by user objects handler.
                return;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Override typical behaviour to make sure arrow keys are passed as well.
            if (keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down)
            {// Do not pass to parent but pass on to OnKeyDown by returning true.
                return false;
            }
            else
            {// We needed the exception for arrow keys only so process as normal.
                return base.ProcessDialogKey(keyData);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_customObjectsManager != null && _customObjectsManager.OnKeyDown(e))
            {// CompletionEvent handled by user objects handler.
                e.Handled = true;
                e.SuppressKeyPress = true;
                
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {// Deny selection.
                _lastDrawingSpaceMouseRightButtonPosition = null;
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.Invalidate();
            }
            
            if (e.KeyCode == Keys.Left)
            {
                PointF partScreenWidth = new PointF(_actualDrawingSpaceArea.Width / KeyboardPanCoefficient, 0);
                partScreenWidth = _graphicsWrapper.ActualSpaceToDrawingSpace(partScreenWidth, false);
                HandlePan(true, partScreenWidth);
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.Refresh();
            }
            else if (e.KeyCode == Keys.Right)
            {
                PointF partScreenWidth = new PointF(-_actualDrawingSpaceArea.Width / KeyboardPanCoefficient, 0);
                partScreenWidth = _graphicsWrapper.ActualSpaceToDrawingSpace(partScreenWidth, false);
                HandlePan(true, partScreenWidth);
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.Refresh();
            }
            else if (e.KeyCode == Keys.Up)
            {
                PointF partScreenWidth = new PointF(0, _actualDrawingSpaceArea.Height / KeyboardPanCoefficient);
                partScreenWidth = _graphicsWrapper.ActualSpaceToDrawingSpace(partScreenWidth, false);
                HandlePan(true, partScreenWidth);
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.Refresh();
            }
            else if (e.KeyCode == Keys.Down)
            {
                PointF partScreenWidth = new PointF(0, -_actualDrawingSpaceArea.Height / KeyboardPanCoefficient);
                partScreenWidth = _graphicsWrapper.ActualSpaceToDrawingSpace(partScreenWidth, false);
                HandlePan(true, partScreenWidth);
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.Refresh();
            }

            // We do not use the Alt keys, since it is for menues in windows, and causes weird focus losing.
            _isControlKeyDown = (e.Modifiers & Keys.Control) != 0;
            _isShiftKeyDown = (e.Modifiers & Keys.Shift) != 0;

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (_customObjectsManager != null && _customObjectsManager.OnKeyUp(e))
            {// CompletionEvent handled by user objects handler.
                return;
            }

            if (e.KeyCode == Keys.Escape && _crosshair.Visible)
            {// Escape stops the crosshair mode.
                CrosshairVisible = false;
                this.Invalidate();
            }

            // We do not use the Alt keys, since it is for menues in windows, and causes weird focus losing.
            _isControlKeyDown = (e.Modifiers & Keys.Control) != 0;
            _isShiftKeyDown = (e.Modifiers & Keys.Shift) != 0;

        }

        /// <summary>
        /// 
        /// </summary>
        public void FitDrawingSpaceToScreen(bool horizontal, bool vertical)
        {
            // 
            UpdateActualDrawingSpaceArea();
            // Needed since the drawing space may contain some invisible series and since we shall zoom it all - first make sure it is ok.
            UpdateDrawingSpace();

            FitDrawingSpaceToScreen(horizontal, vertical, DrawingSpaceDisplayLimit);
        }

        /// <summary>
        /// If fittingSpace is null, DrawingSpaceDisplayLimit is used (zoom all).
        /// </summary>
        public void FitDrawingSpaceToScreen(bool horizontal, bool vertical, RectangleF fittingSpaceRectangle)
        {
            Matrix initialTransform = (Matrix)_graphicsWrapper.DrawingSpaceTransformClone.Clone();
            if (fittingSpaceRectangle.Width == 0 || fittingSpaceRectangle.Height == 0)
            {
                return;
            }

            if (float.IsInfinity(fittingSpaceRectangle.Width) || float.IsNaN(fittingSpaceRectangle.Width)
                || float.IsInfinity(fittingSpaceRectangle.Height) || float.IsNaN(fittingSpaceRectangle.Height)
                || fittingSpaceRectangle.Height == 0 
                || fittingSpaceRectangle.Width == 0)
            {
                MessageBox.Show("Chart pane error, invalid parameters input. Operation will not continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (vertical)
            {
                // Those are reversed, since they reverse their positions entering real space (scaled -1 on Y).
                PointF drawingSpaceBottomPoint = GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(0, fittingSpaceRectangle.Height + fittingSpaceRectangle.Y), true);
                PointF drawingSpaceTopPoint = GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(0, fittingSpaceRectangle.Y), true);

                PointF areaTopPoint = new PointF(0, ActualDrawingSpaceArea.Height + ActualDrawingSpaceArea.Y);
                PointF areaBottomPoint = new PointF(0, ActualDrawingSpaceArea.Y);

                float areaDifference = areaTopPoint.Y - areaBottomPoint.Y;
                float spaceDifference = drawingSpaceTopPoint.Y - drawingSpaceBottomPoint.Y;

                // Scale to size.
                _graphicsWrapper.ScaleDrawingSpaceTransform(1, areaDifference / spaceDifference);

                // Move to place.
                PointF translatedTopPoint = GraphicsWrapper.ActualSpaceToDrawingSpace(areaTopPoint, true);
                _graphicsWrapper.TranslateDrawingSpaceTransfrom(0, translatedTopPoint.Y - fittingSpaceRectangle.Y);
            }

            if (horizontal)
            {
                PointF drawingSpaceLeftPoint = GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(fittingSpaceRectangle.X, 0), true);
                PointF drawingSpaceRightPoint = GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(fittingSpaceRectangle.X + fittingSpaceRectangle.Width, 0), true);

                PointF areaVerticalLeftPoint = new PointF(ActualDrawingSpaceArea.X, 0);
                PointF areaVerticalRightPoint = new PointF(ActualDrawingSpaceArea.X + ActualDrawingSpaceArea.Width, 0);

                float areaDifference = areaVerticalRightPoint.X - areaVerticalLeftPoint.X;
                float spaceDifference = drawingSpaceRightPoint.X - drawingSpaceLeftPoint.X;

                //TracerHelper.Trace(ActualDrawingSpaceArea.X.ToString() + ", " + ActualDrawingSpaceArea.Width.ToString());

                // Scale to size.
                _graphicsWrapper.ScaleDrawingSpaceTransform(areaDifference / spaceDifference, 1);

                if (_maximumXZoom.HasValue 
                    && _graphicsWrapper.DrawingSpaceTransformClone.Elements[0] > _maximumXZoom.Value)
                {// Limit x scaling to 1.
                    _graphicsWrapper.ScaleDrawingSpaceTransform(_maximumXZoom.Value / _graphicsWrapper.DrawingSpaceTransformClone.Elements[0], 1);
                }

                // Move to place.
                PointF areaVerticalLeftPointTranslated = GraphicsWrapper.ActualSpaceToDrawingSpace(areaVerticalLeftPoint, true);
                _graphicsWrapper.TranslateDrawingSpaceTransfrom(areaVerticalLeftPointTranslated.X - fittingSpaceRectangle.X, 0);
            }

            if (_graphicsWrapper.DrawingSpaceTransformClone != initialTransform && this.DrawingSpaceViewTransformationChangedEvent != null)
            {
                DrawingSpaceViewTransformationChangedEvent(this, initialTransform, _graphicsWrapper.DrawingSpaceTransformClone);
            }
        }

        protected void FitHorizontalAreaToScreen(RectangleF areaSelectionRectangle)
        {
            if (_maximumXZoom.HasValue && areaSelectionRectangle.Width < _actualDrawingSpaceArea.Width / _maximumXZoom)
            {// Limit to maximum zoom to calculate properly mins and maxes.
                areaSelectionRectangle.Width = _actualDrawingSpaceArea.Width / _maximumXZoom.Value;
            }

            if (LimitedView)
            {
                if (areaSelectionRectangle.X + areaSelectionRectangle.Width > _drawingSpace.X + _drawingSpace.Width)
                {// Limit to the right, make sure not to show any empty space on the right in this case.
                    areaSelectionRectangle.X = _drawingSpace.X + _drawingSpace.Width - areaSelectionRectangle.Width;
                }

                // Selection must be above the minimum.
                areaSelectionRectangle.X = Math.Max(areaSelectionRectangle.X, _drawingSpace.X);
                if (areaSelectionRectangle.X + areaSelectionRectangle.Width > _drawingSpace.X + _drawingSpace.Width)
                {
                    areaSelectionRectangle.Width = _drawingSpace.X + _drawingSpace.Width - areaSelectionRectangle.X;
                }
            }


            float xStart = areaSelectionRectangle.X;
            float xEnd = areaSelectionRectangle.X + areaSelectionRectangle.Width;

            xStart = Math.Max(0, xStart / (_seriesItemWidth + _seriesItemMargin));
            xEnd = Math.Max(0, xEnd / (_seriesItemWidth + _seriesItemMargin));

            float yMin = float.MaxValue;
            float yMax = float.MinValue;
            foreach (ChartSeries series in _series)
            {
                if (series.Visible)
                {
                    GetSingleSeriesMinMax(series, (int)xStart, (int)xEnd, ref yMin, ref yMax);
                }
            }

            foreach (DynamicCustomObject dynamicObject in _customObjectsManager.DynamicCustomObjects)
            {
                RectangleF containingRectange = dynamicObject.GetContainingRectangle(_drawingSpace);
                if (dynamicObject.Visible && containingRectange.IntersectsWith(areaSelectionRectangle))
                {
                    yMax = Math.Max(yMax, containingRectange.Y + containingRectange.Height);
                    yMin = Math.Min(yMin, containingRectange.Y);
                }
            }

            if (yMin == float.MaxValue || yMax == float.MinValue)
            {
                return;
            }

            areaSelectionRectangle.Y = yMin;
            areaSelectionRectangle.Height = yMax - yMin;

            // Zoom to selection.
            FitDrawingSpaceToScreen(true, true, areaSelectionRectangle);
        }

        /// <summary>
        /// The input selection rectangle must be in drawing space.
        /// </summary>
        protected void HandleSelect(RectangleF selectionRectangle)
        {
            if (float.IsInfinity(selectionRectangle.Width) || float.IsNaN(selectionRectangle.Width)
                || float.IsInfinity(selectionRectangle.Height) || float.IsNaN(selectionRectangle.Height)
                || selectionRectangle.Height == 0 
                || selectionRectangle.Width == 0)
            {
                if (RightMouseButtonSelectionMode == SelectionModeEnum.HorizontalZoom
                    || RightMouseButtonSelectionMode == SelectionModeEnum.VerticalZoom
                    || RightMouseButtonSelectionMode == SelectionModeEnum.RectangleZoom)
                {
                    MessageBox.Show("Chart pane error, invalid parameters input. Operation will not continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            if (_rightMouseButtonSelectionMode == SelectionModeEnum.HorizontalZoom)
            {// Establish the range of the dataDelivery in this rectangle, and zoom on it only.
                FitHorizontalAreaToScreen(selectionRectangle);
            }
            else
            {
                // Zoom to selection.
                FitDrawingSpaceToScreen(true, true, selectionRectangle);
            }

            this.Invalidate();
        }

        public void ZoomIn(float xScaleIncrease)
        {
            // Zoom operation performed based on the right end corner, since this is where the 
            // most current price (user focus) is usually at.
            PointF pointActualSpace = new PointF(ActualDrawingSpaceArea.Left + ActualDrawingSpaceArea.Width /*/ 2*/, ActualDrawingSpaceArea.Top + ActualDrawingSpaceArea.Height / 2);
            PointF drawSpacePoint = GraphicsWrapper.ActualSpaceToDrawingSpace(pointActualSpace, true);
            HandleScale(drawSpacePoint, xScaleIncrease, 1);
            Invalidate();
        }

        public void ZoomOut(float xScaleReduction)
        {
            // Zoom operation performed based on the right end corner, since this is where the 
            // most current price (user focus) is usually at.
            PointF pointActualSpace = new PointF(ActualDrawingSpaceArea.Left + ActualDrawingSpaceArea.Width /*/ 2*/, ActualDrawingSpaceArea.Top + ActualDrawingSpaceArea.Height / 2);
            PointF drawSpacePoint = GraphicsWrapper.ActualSpaceToDrawingSpace(pointActualSpace, true);
            HandleScale(drawSpacePoint, xScaleReduction, 1);
            Invalidate();
        }

        public virtual void HandleScale(PointF scalingCenter, float xScale, float yScale)
        {
            if (xScale == 0 || float.IsInfinity(xScale) || float.IsNaN(xScale)
                || yScale == 0 || float.IsInfinity(yScale) || float.IsNaN(yScale))
            {
                SystemMonitor.Warning("Invalid scaling values.");
                return;
            }

            Matrix initialTransform = (Matrix)_graphicsWrapper.DrawingSpaceTransformClone.Clone();

            _graphicsWrapper.TranslateDrawingSpaceTransfrom(scalingCenter.X, scalingCenter.Y, MatrixOrder.Prepend);
            _graphicsWrapper.ScaleDrawingSpaceTransform(xScale, yScale, MatrixOrder.Prepend);

            if (_maximumXZoom.HasValue && _graphicsWrapper.DrawingSpaceTransformClone.Elements[0] > _maximumXZoom.Value)
            {// Limit x scaling to 1.
                _graphicsWrapper.ScaleDrawingSpaceTransform(_maximumXZoom.Value / _graphicsWrapper.DrawingSpaceTransformClone.Elements[0], 1);
            }

            _graphicsWrapper.TranslateDrawingSpaceTransfrom(-scalingCenter.X, -scalingCenter.Y, MatrixOrder.Prepend);

            if (ScrollModeEnum.HorizontalScrollAndFit == _scrollMode)
            {// After scaling also fit the dataDelivery if we are in fit mode.
                RectangleF area = GraphicsWrapper.ActualSpaceToDrawingSpace(_actualDrawingSpaceArea);
                FitHorizontalAreaToScreen(area);
            }
            
            if (DrawingSpaceViewTransformationChangedEvent != null)
            {
                DrawingSpaceViewTransformationChangedEvent(this, initialTransform, _graphicsWrapper.DrawingSpaceTransformClone);
            }
        }

        public virtual void HandlePan(bool applySpaceLimit, PointF dragVector)
        {
            Matrix initialTransform = (Matrix)_graphicsWrapper.DrawingSpaceTransformClone.Clone();

            _graphicsWrapper.TranslateDrawingSpaceTransfrom(dragVector.X, dragVector.Y);

            RectangleF screen = GraphicsWrapper.ActualSpaceToDrawingSpace(_actualDrawingSpaceArea);

            if (applySpaceLimit)
            {

                // Owner allowed drawing limits.
                if (screen.X < _drawingSpaceDisplayLimit.X)
                {// X left
                    _graphicsWrapper.TranslateDrawingSpaceTransfrom(-_drawingSpaceDisplayLimit.X + screen.X, 0);
                }
                else if (screen.X + screen.Width > _drawingSpaceDisplayLimit.X + _drawingSpaceDisplayLimit.Width)
                {// X right.
                    if (screen.Width < _drawingSpaceDisplayLimit.Width)
                    {
                        _graphicsWrapper.TranslateDrawingSpaceTransfrom(screen.X + screen.Width - _drawingSpaceDisplayLimit.X - _drawingSpaceDisplayLimit.Width, 0);
                    }
                    else if (dragVector.X < 0)
                    {// In this special case, negate the already made translation.
                        _graphicsWrapper.TranslateDrawingSpaceTransfrom(-dragVector.X, 0);
                    }
                }

                if (screen.Y + screen.Height > _drawingSpaceDisplayLimit.Y + _drawingSpaceDisplayLimit.Height)
                {// Y top.
                    _graphicsWrapper.TranslateDrawingSpaceTransfrom(0, -_drawingSpaceDisplayLimit.Y - _drawingSpaceDisplayLimit.Height + screen.Y + screen.Height);
                }
                else if (screen.Y < _drawingSpaceDisplayLimit.Y)
                {// Y bottom.
                    if (screen.Height < _drawingSpaceDisplayLimit.Height)
                    {
                        _graphicsWrapper.TranslateDrawingSpaceTransfrom(0, screen.Y - _drawingSpaceDisplayLimit.Y);
                    }
                    else if (dragVector.Y > 0)
                    {// In this special case, negate the already made translation.
                        _graphicsWrapper.TranslateDrawingSpaceTransfrom(0, -dragVector.Y);
                    }
                }
            }

            if (DrawingSpaceViewTransformationChangedEvent != null)
            {
                DrawingSpaceViewTransformationChangedEvent(this, initialTransform, _graphicsWrapper.DrawingSpaceTransformClone);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetAppearanceMode(AppearanceModeEnum mode)
        {
            _appearanceMode = mode;

            if (mode == AppearanceModeEnum.SuperCompact)
            {
                _showSeriesLabels = false;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Set one of the predefined appearance schemes.
        /// </summary>
        public void SetAppearanceScheme(AppearanceSchemeEnum scheme)
        {
            if (scheme == AppearanceSchemeEnum.Custom)
            {// Changes nothing
                return;
            }

            _appearanceScheme = scheme;

            _seriesItemMargin = 2;
            _seriesItemWidth = 6;

            _actualDrawingSpaceAreaBorderPen = Pens.Gray;
            _actualSpaceGrid.Visible = false;
            _axisLabelsFont = new Font("Tahoma", 8);

            _drawingSpaceGrid.Visible = true;

            _labelsFont = new Font("Tahoma", 8);

            _labelsMargin = 10;
            //_labelsTopMargin = 8;

            _showClippingRectangle = false;

            _titleFont = new Font("Tahoma", 10);

            Point gradientBrushPoint1 = new Point();
            Point gradientBrushPoint2 = new Point(0, Screen.PrimaryScreen.Bounds.Height);

            foreach (ChartSeries series in _series)
            {
                if (series is ProviderTradeChartSeries)
                {
                    ProviderTradeChartSeries providerSeries = (ProviderTradeChartSeries)series;
                    providerSeries.FallingBarFill = (SolidBrush)Brushes.White;
                }
            }

            switch (scheme)
            {

                case AppearanceSchemeEnum.Trade:
                    {
                        _actualDrawingSpaceAreaFill = Brushes.Black;

                        _xAxisLabelsFontBrush = Brushes.DarkGray;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;
                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = Brushes.Black;

                        _labelsFill = Brushes.DarkGray;
                        _labelsFontBrush = Brushes.Black;
                        _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.DarkGray;
                    }
                    break;

                case AppearanceSchemeEnum.TradeWhite:
                        {
                            foreach (ChartSeries series in _series)
                            {
                                if (series is ProviderTradeChartSeries)
                                {
                                    ProviderTradeChartSeries providerSeries = (ProviderTradeChartSeries)series;
                                    providerSeries.FallingBarFill = (SolidBrush)Brushes.LightSalmon;
                                }
                            }

                            _actualDrawingSpaceAreaFill = Brushes.WhiteSmoke;

                            _xAxisLabelsFontBrush = Brushes.Gray;
                            _yAxisLabelsFontBrush = Brushes.Gray;
                            _drawingSpaceGrid.Pen = Pens.DimGray;
                            _fill = Brushes.WhiteSmoke;

                            _labelsFill = null;
                            _labelsFontBrush = Brushes.Black;
                            
                            _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                            _selectionPen = Pens.Gray;

                            _titleFontBrush = Brushes.DarkGray;
                        }
                    break;

                case AppearanceSchemeEnum.Fast:
                    {
                        _actualDrawingSpaceAreaFill = Brushes.Black;

                        _xAxisLabelsFontBrush = Brushes.WhiteSmoke;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;
                        
                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = Brushes.Black;

                        _labelsFill = Brushes.Gainsboro;
                        _labelsFontBrush = Brushes.Black;
                        _selectionFill = null;
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.White;
                    }
                    break;

                case AppearanceSchemeEnum.Default:
                    {
                        _actualDrawingSpaceAreaFill = Brushes.Black;
                        
                        _xAxisLabelsFontBrush = Brushes.WhiteSmoke;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;
                        
                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = Brushes.Black;

                        _labelsFill = Brushes.Gainsboro;
                        _labelsFontBrush = Brushes.Black;
                        _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.White;
                    }
                    break;

                case AppearanceSchemeEnum.Dark:
                    {
                        _actualDrawingSpaceAreaFill = new LinearGradientBrush(gradientBrushPoint1, gradientBrushPoint2, Color.FromArgb(52, 52, 64), Color.FromArgb(84, 77, 84));

                        _xAxisLabelsFontBrush = Brushes.Gainsboro;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;

                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = new SolidBrush(Color.FromArgb(53, 39, 54));

                        _labelsFill = new LinearGradientBrush(new Point(0, 0), new Point(0, 30), Color.Gainsboro, Color.FromArgb(53, 39, 54));
                        _labelsFontBrush = Brushes.Black;
                        _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.Gainsboro;
                    }
                    break;

                case AppearanceSchemeEnum.Light:
                    {
                        _actualDrawingSpaceAreaFill = new LinearGradientBrush(gradientBrushPoint1, gradientBrushPoint2, Color.FromArgb(255, 246, 254), Color.FromArgb(166, 177, 147));

                        _xAxisLabelsFontBrush = Brushes.WhiteSmoke;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;

                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = new SolidBrush(Color.FromArgb(122, 125, 112));

                        _labelsFill = new LinearGradientBrush(new Point(0, 0), new Point(0, 30), Color.Gainsboro, Color.FromArgb(122, 125, 112));
                        _labelsFontBrush = Brushes.Black;
                        _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.Black;
                    }
                    break;

                case AppearanceSchemeEnum.DarkNatural:
                    {
                        _actualDrawingSpaceAreaFill = new LinearGradientBrush(gradientBrushPoint1, gradientBrushPoint2, Color.FromArgb(93, 88, 70), Color.FromArgb(5, 41, 46));

                        _xAxisLabelsFontBrush = Brushes.LightGray;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;

                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = new LinearGradientBrush(gradientBrushPoint1, gradientBrushPoint2, Color.FromArgb(93, 88, 70), Color.FromArgb(5, 41, 46));

                        _labelsFill = null;
                        _labelsFontBrush = Brushes.LightGray;

                        _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.LightGray;
                    }
                    break;
                
                case AppearanceSchemeEnum.LightNatural:
                case AppearanceSchemeEnum.LightNaturalFlat:
                    {
                        
                        if (scheme == AppearanceSchemeEnum.LightNatural)
                        {
                            _actualDrawingSpaceAreaFill = new LinearGradientBrush(gradientBrushPoint1, gradientBrushPoint2, Color.FromArgb(223, 203, 164), Color.FromArgb(173, 165, 130));
                        }
                        else
                        {
                            _actualDrawingSpaceAreaFill = new SolidBrush(Color.FromArgb(223, 203, 164));
                        }

                        _xAxisLabelsFontBrush = Brushes.Black;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;
                        
                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = _actualDrawingSpaceAreaFill;

                        _labelsFill = null;
                        _labelsFontBrush = Brushes.Black;

                        _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.Black;
                    }
                    break;
                case AppearanceSchemeEnum.Alfonsina:
                    {
                        _actualDrawingSpaceAreaFill = new LinearGradientBrush(gradientBrushPoint1, gradientBrushPoint2, Color.FromArgb(214, 201, 141), Color.FromArgb(171, 161, 118));

                        _xAxisLabelsFontBrush = Brushes.WhiteSmoke;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;

                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = new LinearGradientBrush(new Point(), new Point(0, 2000), Color.FromArgb(94, 90, 66), Color.FromArgb(5, 35, 40));
                            
                        _labelsFill = null;
                        _labelsFontBrush = Brushes.WhiteSmoke;

                        _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.Black;
                    }
                    break;
                case AppearanceSchemeEnum.Ground:
                    {
                        _actualDrawingSpaceAreaFill = new LinearGradientBrush(gradientBrushPoint1, gradientBrushPoint2, Color.FromArgb(173, 144, 110), Color.FromArgb(151, 120, 95));

                        _xAxisLabelsFontBrush = Brushes.WhiteSmoke;
                        _yAxisLabelsFontBrush = _xAxisLabelsFontBrush;
                        
                        _drawingSpaceGrid.Pen = Pens.DimGray;
                        _fill = new SolidBrush(Color.FromArgb(118, 85, 73));

                        _labelsFill = null;
                        _labelsFontBrush = Brushes.WhiteSmoke;

                        _selectionFill = new SolidBrush(Color.FromArgb(70, 15, 15, 15));
                        _selectionPen = Pens.Gray;

                        _titleFontBrush = Brushes.WhiteSmoke;
                    }
                    break;
                    
                default:
                    break;
            }

            if (AppearanceSchemeChangedEvent != null)
            {
                AppearanceSchemeChangedEvent(this, scheme);
            }

            if (ParametersUpdatedEvent != null)
            {
                ParametersUpdatedEvent(this);
            }

        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="context"></param>
        public void SaveState(SerializationInfoEx info, bool saveCustomObjects)
        {
            info.AddValue("Name", this.Name);
            info.AddValue("Height", this.Height);

            info.AddValue("_stateId", _stateId);
            info.AddValue("_titleFont", _titleFont);

            info.AddValue("_titleFontBrush", _titleFontBrush);
            info.AddValue("_fill", _fill);
            
            info.AddValue("_axisLabelsFont", _axisLabelsFont);
            info.AddValue("_xAxisLabelsFontBrush", _xAxisLabelsFontBrush);
            info.AddValue("_yAxisLabelsPosition", _yAxisLabelsPosition);
            info.AddValue("_yAxisLabelsFontBrush", _yAxisLabelsFontBrush);
            info.AddValue("_xAxisLabelsFormat", _xAxisLabelsFormat);
            info.AddValue("_yAxisLabelsFormat", _yAxisLabelsFormat);
            info.AddValue("_labelsFont", _labelsFont);

            info.AddValue("_labelsFontBrush", _labelsFontBrush);
            info.AddValue("_labelsFill", _labelsFill);
            info.AddValue("_labelsTopMargin", _labelsTopMargin);
            info.AddValue("_labelsMargin", _labelsMargin);
            info.AddValue("_showSeriesLabels", _showSeriesLabels);
            info.AddValue("_showClippingRectangle", _showClippingRectangle);
            info.AddValue("_unitUnificationOptimizationEnabled", _unitUnificationOptimizationEnabled);
            info.AddValue("_smoothingMode", _smoothingMode);
            info.AddValue("_defaultAbsoluteSelectionMargin", _defaultAbsoluteSelectionMargin);
            info.AddValue("_scrollMode", _scrollMode);
            info.AddValue("_rightMouseButtonSelectionMode", _rightMouseButtonSelectionMode);
            info.AddValue("_selectionPen", _selectionPen);
            info.AddValue("_selectionFill", _selectionFill);
            info.AddValue("_additionalDrawingSpaceAreaMarginLeft", _additionalDrawingSpaceAreaMarginLeft);
            info.AddValue("_additionalDrawingSpaceAreaMarginRight", _additionalDrawingSpaceAreaMarginRight);
            info.AddValue("_actualDrawingSpaceAreaMarginLeft", _actualDrawingSpaceAreaMarginLeft);
            info.AddValue("_actualDrawingSpaceAreaMarginTop", _actualDrawingSpaceAreaMarginTop);
            info.AddValue("_actualDrawingSpaceAreaMarginRight", _actualDrawingSpaceAreaMarginRight);
            info.AddValue("_actualDrawingSpaceAreaMarginBottom", _actualDrawingSpaceAreaMarginBottom);
            info.AddValue("_actualDrawingSpaceAreaBorderPen", _actualDrawingSpaceAreaBorderPen);
            info.AddValue("_actualDrawingSpaceAreaFill", _actualDrawingSpaceAreaFill);
            info.AddValue("_limitedView", _limitedView);
            info.AddValue("_seriesItemWidth", _seriesItemWidth);
            info.AddValue("_seriesItemMargin", _seriesItemMargin);

            info.AddValue("customObjectsSaved", saveCustomObjects);
            if (saveCustomObjects)
            {
                _customObjectsManager.SaveState(info);
            }

            info.AddValue("_actualSpaceGrid", _actualSpaceGrid);
            info.AddValue("_drawingSpaceGrid", _drawingSpaceGrid);

            info.AddValue("_chartName", _chartName);
            info.AddValue("_appearanceScheme", _appearanceScheme);
            info.AddValue("_autoScrollToEnd", _autoScrollToEnd);
            
            if (_maximumXZoom.HasValue)
            {
                info.AddValue("_maximumXZoom", _maximumXZoom.Value);
            }

            info.AddValue("_xAxisLabelSpacing", _xAxisLabelSpacing);
        }


        public void RestoreState(SerializationInfoEx info, bool restoreCustomObjects)
        {
            this.Name = info.GetString("Name");
            this.Height = info.GetInt32("Height");

            _stateId = info.GetValue<Guid>("_stateId");
            _titleFont = info.GetValue<Font>("_titleFont");

            _titleFontBrush = info.GetValue<Brush>("_titleFontBrush");
            _fill = info.GetValue<Brush>("_fill");
            _axisLabelsFont = info.GetValue<Font>("_axisLabelsFont");
            _xAxisLabelsFontBrush = info.GetValue<Brush>("_xAxisLabelsFontBrush");
            _yAxisLabelsPosition = info.GetValue<YAxisLabelPosition>("_yAxisLabelsPosition");
            _yAxisLabelsFontBrush = info.GetValue<Brush>("_yAxisLabelsFontBrush");

            _xAxisLabelsFormat = info.GetString("_xAxisLabelsFormat");
            _yAxisLabelsFormat = info.GetString("_yAxisLabelsFormat");

            _labelsFont = info.GetValue<Font>("_labelsFont");
            _labelsFontBrush = info.GetValue<Brush>("_labelsFontBrush");
            _labelsFill = info.GetValue<Brush>("_labelsFill");

            _labelsTopMargin = info.GetSingle("_labelsTopMargin");
            _labelsMargin = info.GetSingle("_labelsMargin");

            _showSeriesLabels = info.GetBoolean("_showSeriesLabels");
            _showClippingRectangle = info.GetBoolean("_showClippingRectangle");
            _unitUnificationOptimizationEnabled = info.GetBoolean("_unitUnificationOptimizationEnabled");

            _smoothingMode = info.GetValue<SmoothingMode>("_smoothingMode");
            _defaultAbsoluteSelectionMargin = info.GetSingle("_defaultAbsoluteSelectionMargin");

            _scrollMode = info.GetValue<ScrollModeEnum>("_scrollMode");
            _rightMouseButtonSelectionMode = info.GetValue<SelectionModeEnum>("_rightMouseButtonSelectionMode");

            _selectionPen = info.GetValue<Pen>("_selectionPen");
            _selectionFill = info.GetValue<Brush>("_selectionFill");

            _additionalDrawingSpaceAreaMarginLeft = info.GetInt32("_additionalDrawingSpaceAreaMarginLeft");
            _additionalDrawingSpaceAreaMarginRight = info.GetInt32("_additionalDrawingSpaceAreaMarginRight");

            _actualDrawingSpaceAreaMarginLeft = info.GetInt32("_actualDrawingSpaceAreaMarginLeft");
            _actualDrawingSpaceAreaMarginTop = info.GetInt32("_actualDrawingSpaceAreaMarginTop");
            _actualDrawingSpaceAreaMarginRight = info.GetInt32("_actualDrawingSpaceAreaMarginRight");
            _actualDrawingSpaceAreaMarginBottom = info.GetInt32("_actualDrawingSpaceAreaMarginBottom");

            _actualDrawingSpaceAreaBorderPen = info.GetValue<Pen>("_actualDrawingSpaceAreaBorderPen");
            _actualDrawingSpaceAreaFill = info.GetValue<Brush>("_actualDrawingSpaceAreaFill");
            _limitedView = info.GetBoolean("_limitedView");

            _seriesItemWidth = info.GetSingle("_seriesItemWidth");
            _seriesItemMargin = info.GetSingle("_seriesItemMargin");


            if (restoreCustomObjects && info.GetBoolean("customObjectsSaved"))
            {// Restore custom objects.
                _customObjectsManager.RestoreState(info);
            }
            else
            {// New clear custom objects.
                _customObjectsManager.Clear();
            }

            _actualSpaceGrid = info.GetValue<ChartGrid>("_actualSpaceGrid");
            _drawingSpaceGrid = info.GetValue<ChartGrid>("_drawingSpaceGrid");
            _chartName = info.GetString("_chartName");
            _appearanceScheme = info.GetValue<AppearanceSchemeEnum>("_appearanceScheme");

            _autoScrollToEnd = info.GetBoolean("_autoScrollToEnd");
            
            if (info.ContainsValue("_maximumXZoom"))
            {
                _maximumXZoom = info.GetSingle("_maximumXZoom");
            }
            else
            {
                _maximumXZoom = null;
            }

            _xAxisLabelSpacing = info.GetSingle("_xAxisLabelSpacing");

            if (AppearanceSchemeChangedEvent != null)
            {
                AppearanceSchemeChangedEvent(this, _appearanceScheme);
            }
        }

    }
}
