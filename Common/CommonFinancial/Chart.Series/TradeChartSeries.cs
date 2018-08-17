using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Thread safe.
    /// </summary>
    [Serializable]
    public abstract class TradeChartSeries : TimeBasedChartSeries
    {
        public enum ChartTypeEnum
        {
            CandleStick,
            BarChart,
            ColoredArea,
            Histogram,
            Line,
        }

        public override string[] ChartTypes
        {
            get { return Enum.GetNames(typeof(ChartTypeEnum)); }
        }

        public override string SelectedChartType
        {
            get
            {
                return Enum.GetName(typeof(ChartTypeEnum), _chartType);
            }
        }

        volatile ChartTypeEnum _chartType = ChartTypeEnum.BarChart;
        public ChartTypeEnum ChartType
        {
            get { return _chartType; }
            set { _chartType = value; }
        }

        SolidBrush _risingBarFill = null;
        public SolidBrush RisingBarFill
        {
            set { _risingBarFill = value; }
            get { return _risingBarFill; }
        }

        SolidBrush _fallingBarFill = (SolidBrush)Brushes.White;
        public SolidBrush FallingBarFill
        {
            set { _fallingBarFill = value; }
            get { return _fallingBarFill; }
        }

        Pen _risingBarPen = Pens.Green;
        public Pen RisingBarPen
        {
            set { _risingBarPen = value; }
            get { return _risingBarPen; }
        }

        Pen _fallingBarPen = Pens.Green;
        public Pen FallingBarPen
        {
            set { _fallingBarPen = value; }
            get { return _fallingBarPen; }
        }

        Pen _timeGapsLinePen = new Pen(Color.DarkRed);
        /// <summary>
        /// Time gaps occur when there is space between one period and the next.
        /// </summary>
        public Pen TimeGapsLinePen
        {
            set { _timeGapsLinePen = value; }
            get { return _timeGapsLinePen; }
        }

        bool _showVolume = false;
        public bool ShowVolume
        {
            get { return _showVolume; }
            set { _showVolume = value; }
        }

        Pen _volumePen = Pens.Gainsboro;
        public Pen VolumePen
        {
            get { return _volumePen; }
            set { _volumePen = value; }
        }

        SolidBrush _volumeBrush = (SolidBrush)Brushes.Gainsboro;
        public SolidBrush VolumeBrush
        {
            set { _volumeBrush = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TradeChartSeries(string name)
            : base(name)
        {
            _timeGapsLinePen.DashPattern = new float[] { 10, 10 };
            _timeGapsLinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
        }

        /// <summary>
        /// 
        /// </summary>
        public TradeChartSeries(string name, ChartTypeEnum chartType)
            : base(name)
        {
            _chartType = chartType;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public TradeChartSeries(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _chartType = (ChartTypeEnum)info.GetInt32("chartType");
            _risingBarFill = (SolidBrush)info.GetValue("risingBarFill", typeof(SolidBrush));
            _fallingBarFill = (SolidBrush)info.GetValue("fallingBarFill", typeof(SolidBrush));
            _risingBarPen = (Pen)info.GetValue("risingBarPen", typeof(Pen));
            _fallingBarPen = (Pen)info.GetValue("fallingBarPen", typeof(Pen));
            _timeGapsLinePen = (Pen)info.GetValue("timeGapsLinePen", typeof(Pen));
            _showVolume = info.GetBoolean("showVolume");
            _volumePen = (Pen)info.GetValue("volumePen", typeof(Pen));
            _volumeBrush = (SolidBrush)info.GetValue("volumeBrush", typeof(SolidBrush));
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("chartType", (int)_chartType);
            info.AddValue("risingBarFill", _risingBarFill);
            info.AddValue("fallingBarFill", _fallingBarFill);
            info.AddValue("risingBarPen", _risingBarPen);
            info.AddValue("fallingBarPen", _fallingBarPen);
            info.AddValue("timeGapsLinePen", _timeGapsLinePen);
            info.AddValue("showVolume", _showVolume);
            info.AddValue("volumePen", _volumePen);
            info.AddValue("volumeBrush", _volumeBrush);
        }

        /// <summary>
        /// Drawing routine, invoke from child class to draw dataDelivery bars in common uniform way.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="unitsUnification">Draw a few units as one. Used when zooming is too big to show each unit - so unify them. 1 means no unification, 10 means unify 10 units together</param>
        /// <param name="clippingRectangle"></param>
        /// <param name="itemWidth"></param>
        /// <param name="itemMargin"></param>
        public void Draw(GraphicsWrapper g, ReadOnlyCollection<DataBar> dataBars,
            int unitsUnification, RectangleF clippingRectangle, float itemWidth, float itemMargin, float maxVolume, object tag)
        {
            if (Visible == false)
            {
                return;
            }

            PointF drawingPoint = new PointF();
            lock (this)
            {
                if (_chartType == ChartTypeEnum.Line)
                {
                    base.DrawItemSet(LinesChartSeries.ChartTypeEnum.Line,
                        g, _risingBarPen, null, unitsUnification, clippingRectangle, itemWidth, itemMargin, null);
                }
                else if (_chartType == ChartTypeEnum.Histogram)
                {
                    base.DrawItemSet(LinesChartSeries.ChartTypeEnum.Histogram, 
                        g, _risingBarPen, _risingBarFill, unitsUnification, clippingRectangle, itemWidth, itemMargin, tag);
                }
                else if (_chartType == ChartTypeEnum.ColoredArea)
                {
                    base.DrawItemSet(LinesChartSeries.ChartTypeEnum.ColoredArea,
                        g, _risingBarPen, _risingBarFill, unitsUnification, clippingRectangle, itemWidth, itemMargin, tag);
                }
                else if (_chartType == ChartTypeEnum.CandleStick || _chartType == ChartTypeEnum.BarChart)
                {// Unit unification is done trough combining many bars together.
                    List<DataBar> combinationDatas = new List<DataBar>();
                    bool timeGapFound = false;

                    int startIndex, endIndex;
                    GetDrawingRangeIndecesFromClippingRectange(clippingRectangle, drawingPoint, unitsUnification, out startIndex, out endIndex, itemWidth, itemMargin);

                    float volumeDisplayRange = clippingRectangle.Height / 4f;
                    decimal volumeDisplayMultiplicator = 0;

                    if (maxVolume > 0)
                    {
                        volumeDisplayMultiplicator = (decimal)(volumeDisplayRange / maxVolume);
                    }

                    for (int i = startIndex; i < endIndex && i < dataBars.Count; i++)
                    {
                        combinationDatas.Add(dataBars[i]);

                        if (unitsUnification == 1 && ShowTimeGaps && i > 0 && dataBars[i].DateTime - dataBars[i - 1].DateTime != _period)
                        {
                            timeGapFound = true;
                        }

                        if (i % unitsUnification == 0)
                        {
                            DataBar combinedData = DataBar.CombinedBar(combinationDatas.ToArray());
                            combinationDatas.Clear();
                            if (combinedData.HasDataValues)
                            //&& drawingPoint.X >= clippingRectangle.X 
                            //&& drawingPoint.X <= clippingRectangle.X + clippingRectangle.Width)
                            {
                                if (timeGapFound && ShowTimeGaps && _timeGapsLinePen != null)
                                {// Draw time gap.
                                    timeGapFound = false;
                                    g.DrawLine(_timeGapsLinePen, new PointF(drawingPoint.X, clippingRectangle.Y), new PointF(drawingPoint.X, clippingRectangle.Y + clippingRectangle.Height));

                                    //g.DrawLine(_timeGapsLinePen, new PointF(drawingPoint.X, clippingRectangle.Y), new PointF(drawingPoint.X, (float)(dataDelivery.High)));
                                    //g.DrawLine(_timeGapsLinePen, new PointF(drawingPoint.X, (float)(dataDelivery.High + dataDelivery.BarTotalLength / 2f)), new PointF(drawingPoint.X, clippingRectangle.Y + clippingRectangle.Height));
                                }

                                if (_chartType == ChartTypeEnum.CandleStick)
                                {
                                    DrawCandleStick(g, ref drawingPoint, combinedData, itemWidth, itemMargin, unitsUnification);
                                }
                                else
                                {
                                    DrawBar(g, ref drawingPoint, combinedData, itemWidth, itemMargin, unitsUnification);
                                }
                                
                                // Draw closeVolume for this bar.
                                if (_showVolume)
                                {
                                    float actualHeight = (float)(combinedData.Volume * volumeDisplayMultiplicator);
                                    g.DrawLine(_volumePen, drawingPoint.X, clippingRectangle.Y, drawingPoint.X, clippingRectangle.Y + actualHeight);
                                }

                            }

                            drawingPoint.X = (i + 1) * (itemMargin + itemWidth);
                        }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Enter locked.
        /// </summary>
        void DrawBar(GraphicsWrapper g, ref PointF startingPoint, DataBar barData, float itemWidth, float itemMargin, int itemUnitification)
        {
            float xMiddle = startingPoint.X + itemWidth / 2;
            float xHalfWidth = itemWidth / 2;

            Pen pen = _risingBarPen;
            if (barData.IsRising == false)
            {
                pen = _fallingBarPen;
            }

            if (pen == null)
            {
                return;
            }

            float yDisplacement = startingPoint.Y;

            g.DrawLine(pen, xMiddle, yDisplacement + (float)barData.Low, xMiddle, yDisplacement + (float)barData.High);
            g.DrawLine(pen, xMiddle, yDisplacement + (float)barData.Open, xMiddle - xHalfWidth, yDisplacement + (float)barData.Open);
            g.DrawLine(pen, xMiddle, yDisplacement + (float)barData.Close, xMiddle + xHalfWidth, yDisplacement + (float)barData.Close);
        }

        /// <summary>
        /// Enter locked.
        /// </summary>
        void DrawCandleStick(GraphicsWrapper g, ref PointF startingPoint, DataBar barData, float itemWidth, float itemMargin, int itemUnitification)
        {
            if (barData.IsRising)
            {
                if (_risingBarFill != null)
                {
                    g.FillRectangle(_risingBarFill, startingPoint.X, startingPoint.Y + (float)barData.Open, itemWidth, (float)barData.AbsoluteBodyHeight);
                }

                if (_risingBarPen != null)
                {
                    if (itemWidth > 4)
                    {
                        g.DrawRectangle(_risingBarPen, startingPoint.X, startingPoint.Y + (float)barData.Open, itemWidth, (float)barData.AbsoluteBodyHeight);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Green, startingPoint.X, startingPoint.Y + (float)barData.Open, itemWidth, (float)barData.AbsoluteBodyHeight);
                    }

                    // Draw a horizontal line for 0 height bars.
                    if (barData.AbsoluteBodyHeight == 0)
                    {
                        g.DrawLine(_fallingBarPen, startingPoint.X, startingPoint.Y + (float)barData.Close, startingPoint.X + itemWidth, startingPoint.Y + (float)barData.Close);
                    }

                    // Lower shadow
                    g.DrawLine(_risingBarPen, 
                        startingPoint.X + itemWidth / 2, 
                        startingPoint.Y + (float)barData.Low, 
                        startingPoint.X + itemWidth / 2, 
                        startingPoint.Y + (float)barData.Open);

                    // Upper shadow
                    g.DrawLine(_risingBarPen,
                        startingPoint.X + itemWidth / 2, 
                        (float)(startingPoint.Y + (float)barData.High),
                        startingPoint.X + itemWidth / 2,
                        (float)(startingPoint.Y + (float)barData.Close));
                }
            }
            else
            {
                if (_fallingBarFill != null)
                {
                    g.FillRectangle(_fallingBarFill, startingPoint.X, startingPoint.Y + (float)barData.Close, itemWidth, (float)barData.AbsoluteBodyHeight);
                }

                if (_fallingBarPen != null)
                {
                    if (itemWidth >= 4)
                    {// Only if an item is clearly visible, show the border, otherwise, hide to improve. overal visibility.
                        // Showing this border adds nice detail on close zooming, but not useful otherwise.
                        g.DrawRectangle(_fallingBarPen, startingPoint.X, startingPoint.Y + (float)barData.Close, itemWidth, (float)barData.AbsoluteBodyHeight);
                    }

                    // Draw a horizontal line for 0 height bars.
                    if (barData.AbsoluteBodyHeight == 0)
                    {
                        g.DrawLine(_fallingBarPen, startingPoint.X, startingPoint.Y + (float)barData.Close, startingPoint.X + itemWidth, startingPoint.Y + (float)barData.Close);
                    }

                    // Lower shadow
                    g.DrawLine(_fallingBarPen, 
                        startingPoint.X + itemWidth / 2, 
                        startingPoint.Y + (float)barData.Low, 
                        startingPoint.X + itemWidth / 2, 
                        startingPoint.Y + (float)barData.Close);

                    // Upper shadow
                    g.DrawLine(_fallingBarPen, 
                        startingPoint.X + itemWidth / 2,
                        (float)(startingPoint.Y + (float)barData.High),
                        startingPoint.X + itemWidth / 2,
                        (float)(startingPoint.Y + (float)barData.Open));
                }
            }
        }

        public override void SetSelectedChartType(string chartType)
        {
            _chartType = (ChartTypeEnum)Enum.Parse(typeof(ChartTypeEnum), chartType);
        }

    }
}
