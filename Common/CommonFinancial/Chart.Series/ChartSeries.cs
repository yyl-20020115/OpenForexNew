using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CommonSupport;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Note - all the Brush and Pens do not have Get accessors on purpose, since defaults can not be modified. 
    /// So to change them directly use the "set".
    /// Thread safe.
    /// </summary>
    [Serializable]
    public abstract class ChartSeries : ISerializable
    {
        volatile string _name = "";
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        volatile bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        Guid _chartPaneStateId;
        /// <summary>
        /// 
        /// </summary>
        public Guid ChartPaneStateId
        {
            get { lock (this) { return _chartPaneStateId; } }
        }

        protected Dictionary<string, Color> _customMessages = new Dictionary<string, Color>();
        /// <summary>
        /// A set of custom messages the displayed to the user. 
        /// Set color to Color.Empty for default requestMessage color.
        /// </summary>
        public Dictionary<string, Color> CustomMessages
        {
            get { lock (this) { return _customMessages; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string[] ChartTypes 
        { 
            get 
            { 
                return new string[] { "Default" } ; 
            } 
        }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual string SelectedChartType
        {
            get
            {
                return "Default";
            }
        }

        public abstract int ItemsCount { get; }

        public delegate void SeriesUpdatedDelegate(ChartSeries series, bool updateUI);

        public event SeriesUpdatedDelegate SeriesUpdatedEvent;

        Font _customMessagesFont = null;
        /// <summary>
        /// Change this if you want this chart series custom messages to be displayed with this special font.
        /// Set to null to use default.
        /// </summary>
        public Font CustomMessagesFont
        {
            get { lock (this) { return _customMessagesFont; } }
            set { lock (this) { _customMessagesFont = value; } }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ChartSeries()
        {
        }

        /// <summary>
        /// Constructor with name.
        /// </summary>
        public ChartSeries(string name)
        {
            _name = name;
        }


        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public ChartSeries(SerializationInfo info, StreamingContext context)
        {
            _name = info.GetString("name");
            _visible = info.GetBoolean("visible");
            _customMessagesFont = (Font)info.GetValue("customMessagesFont", typeof(Font));
            _customMessages = (Dictionary<string, Color>)info.GetValue("customMessages", typeof(Dictionary<string, Color>));
            _chartPaneStateId = (Guid)info.GetValue("chartPaneStateId", typeof(Guid));
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", _name);
            info.AddValue("visible", _visible);
            info.AddValue("customMessages", _customMessages);
            info.AddValue("customMessagesFont", _customMessagesFont);
            info.AddValue("chartPaneStateId", _chartPaneStateId);
        }

        /// <summary>
        /// Notified when series added to chart.
        /// </summary>
        /// <param name="chartPaneStateId"></param>
        public void AddedToChart(Guid chartPaneStateId)
        {
            lock (this)
            {
                _chartPaneStateId = chartPaneStateId;
            }
            OnAddedToChart();
        }

        protected virtual void OnAddedToChart()
        {
        }

        public void RemovedFromChart()
        {
            lock (this)
            {
                _chartPaneStateId = Guid.Empty;
            }
            OnRemovedFromChart();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformationMatrix"></param>
        /// <param name="drawingSpaceSelectionPoint"></param>
        /// <param name="absoluteSelectionMargin"></param>
        /// <param name="canLoseSelection"></param>
        /// <returns>Return true to mark - selected.</returns>
        public virtual bool TrySelect(Matrix transformationMatrix, PointF drawingSpaceSelectionPoint, float absoluteSelectionMargin, bool canLoseSelection)
        {
            return false;
        }

        /// <summary>
        /// Override to participate in the showing of the general chart context menu.
        /// </summary>
        public virtual void OnShowChartContextMenu(ContextMenuStrip menuStrip, ToolStripMenuItem selectedObjectsMenuItem)
        {
        }

        public virtual void OnChartContextMenuItemClicked(ToolStripItem item)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnRemovedFromChart()
        {
        }

        public abstract void SetSelectedChartType(string chartType);

        public abstract void SaveToFile(string fileName);

        /// <summary>
        /// Establish the total minimum value of any item in this interval.
        /// </summary>
        /// <param name="startIndex">Inclusive starting index. Will be null to specify global start.</param>
        /// <param name="endIndex">Exclusive ending index. Will be null to specify global end.</param>
        public abstract void GetTotalMinimumAndMaximum(int? startIndex, int? endIndex, ref float minimum, ref float maximum);

        protected void RaiseSeriesValuesUpdated(bool updateUI)
        {
            if (SeriesUpdatedEvent != null)
            {
                SeriesUpdatedEvent(this, updateUI);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual float CalculateTotalWidth(float itemWidth, float itemMargin)
        {
            return (itemWidth + itemMargin) * ItemsCount;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        protected PointF DrawCustomMessage(GraphicsWrapper g, Font font, Brush brush, string message, PointF drawingLocation)
        {
            if (font != null && brush != null)
            {
                SizeF size = g.MeasureString(message, font);
                g.DrawString(message, font, brush, drawingLocation);
                drawingLocation.Y += size.Height;
            }
            return drawingLocation;
        }

        /// <summary>
        /// This is a chance for the Series to draw custom messages on the given location (typically top left corner).
        /// The result must be the next valid drawing point down (how much space was used for drawing by current series).
        /// </summary>
        public virtual PointF DrawCustomMessages(ChartPane managingPane, GraphicsWrapper g, PointF drawingLocation)
        {
            if (Visible == false)
            {
                return drawingLocation;
            }

            foreach (KeyValuePair<string, Color> pair in CustomMessages)
            {
                Brush brush = managingPane.TitleFontBrush;
                if (pair.Value.IsEmpty == false)
                {
                    brush = new SolidBrush(pair.Value);
                }

                Font font = managingPane.TitleFont;
                if (CustomMessagesFont != null)
                {
                    font = CustomMessagesFont;
                }

                drawingLocation = DrawCustomMessage(g, font, brush, pair.Key, drawingLocation);
            }

            return drawingLocation;
        }

        /// <summary>
        /// Allows the chart series to render information on the external overlaying part of the chart pane.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clippingRectangle">Rectangle describing the entire drawing area.</param>
        public virtual void DrawInitialActualSpaceOverlays(ChartPane managingPane, GraphicsWrapper g)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void DrawSeriesIcon(GraphicsWrapper g, Rectangle rectangle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="unitsUnification">Draw a few units as one. Used when zooming is too big to show each unit - so unify them. 1 means no unification, 10 means unify 10 units together</param>
        /// <param name="clippingRectangle"></param>
        /// <param name="itemWidth"></param>
        /// <param name="itemMargin"></param>
        public abstract void Draw(ChartPane managingPane, GraphicsWrapper g, int unitsUnification, RectangleF clippingRectangle, float itemWidth, float itemMargin);

        /// <summary>
        /// Used by the drawing functions to allow the child to dynamically deside the type and source of drawing, and not force any constraints in underlying dataDelivery type.
        /// </summary>
        protected abstract float GetDrawingValueAt(int index, object tag);

        /// <summary>
        /// Indeces returned are not checked for maximum value, so they may be beyond the available dataDelivery.
        /// </summary>
        protected void GetDrawingRangeIndecesFromClippingRectange(RectangleF clippingRectangle, PointF baseDrawingPoint, 
            int unitsUnification, out int startingIndex, out int endingIndex, float itemWidth, float itemMargin)
        {
            float xStartLocation = Math.Max(0, baseDrawingPoint.X + clippingRectangle.X);
            float xEndLocation = baseDrawingPoint.X + clippingRectangle.X + clippingRectangle.Width;

            startingIndex = (int)(xStartLocation / (itemWidth + itemMargin));
            // Make it round to unitsUnification, round downwards.
            startingIndex = startingIndex / unitsUnification;
            startingIndex = startingIndex * unitsUnification;

            endingIndex = (int)(xEndLocation / (itemWidth + itemMargin));
            // Make it round to unitsUnification, but round upwards.
            int leftOver = endingIndex % unitsUnification;
            endingIndex = endingIndex / unitsUnification;
            endingIndex = endingIndex * unitsUnification;
            if (leftOver > 0)
            {
                endingIndex += unitsUnification;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void DrawItemSet(LinesChartSeries.ChartTypeEnum type, GraphicsWrapper g, Pen pen, Brush fill, int unitsUnification,
            RectangleF clippingRectangle, float itemWidth, float itemMargin, object tag)
        {
            PointF drawingPoint = new PointF();

            int startIndex, endIndex;
            GetDrawingRangeIndecesFromClippingRectange(clippingRectangle, drawingPoint, unitsUnification, out startIndex, out endIndex, itemWidth, itemMargin);

            // Need to go up to (ItemsCount + unitsUnification) since the size of the step is (unitsUnification).
            for (int i = startIndex + unitsUnification - 1; i < endIndex && i < ItemsCount + unitsUnification; i += unitsUnification)
            {
                int actualIndex = i;
                int actualPreviousIndex = Math.Max(0, i - unitsUnification);

                if (actualIndex < 0 || actualIndex >= ItemsCount)
                {// Cycle conditions are loose and this is possible.
                    continue;
                }

                switch (type)
                {
                    case LinesChartSeries.ChartTypeEnum.ColoredArea:
                        DrawColorAreaItem(g, ref drawingPoint, pen, fill, actualIndex, 
                            actualPreviousIndex, itemWidth, itemMargin, tag);
                        break;

                    case LinesChartSeries.ChartTypeEnum.Histogram:
                        DrawHistogramBar(g, ref drawingPoint, pen, fill, actualIndex, 
                            actualPreviousIndex, itemWidth, itemMargin, unitsUnification, tag);
                        break;

                    case LinesChartSeries.ChartTypeEnum.Line:
                        {
                            float previousValue = GetDrawingValueAt(actualPreviousIndex, tag);
                            float value = GetDrawingValueAt(actualIndex, tag);

                            if (float.IsNaN(previousValue) == false && float.IsNaN(value) == false
                                && float.IsInfinity(previousValue) == false && float.IsInfinity(value) == false)
                            {
                                g.DrawLine(pen, drawingPoint.X, drawingPoint.Y + previousValue, drawingPoint.X + itemMargin + itemWidth, drawingPoint.Y + value);
                            }
                            
                            break;
                        }
                    default:
                        break;
                }

                drawingPoint.X = (i + 1) * (itemMargin + itemWidth);
            }
        
        }
        ///// <summary>
        ///// Helper method, draws histogram bars.
        ///// </summary>
        //protected void DrawHistogramBars(GraphicsWrapper g, Pen pen, Brush fill, int unitsUnification,
        //    RectangleF clippingRectangle, float itemWidth, float itemMargin, object tag)
        //{
        //    PointF drawingPoint = new PointF();

        //    int startIndex, endIndex;
        //    GetDrawingRangeIndecesFromClippingRectange(clippingRectangle, drawingPoint, unitsUnification, out startIndex, out endIndex, itemWidth, itemMargin);

        //    for (int i = startIndex + unitsUnification - 1; i < endIndex && i < MaximumIndex + unitsUnification - 1; i += unitsUnification)
        //    {
        //        int actualIndex = i;
        //        int actualPreviousIndex = i - unitsUnification;

        //        if (actualIndex >= MaximumIndex)
        //        {
        //            actualIndex = MaximumIndex - 1;
        //        }

        //        DrawHistogramBar(g, ref drawingPoint, pen, fill, actualIndex, actualPreviousIndex,
        //            itemWidth, itemMargin, unitsUnification, tag);

        //        drawingPoint.X = i * (itemMargin + itemWidth);
        //    }
        //}

        //protected void DrawColoredArea(GraphicsWrapper g, Pen pen, Brush fill, int unitsUnification, 
        //    RectangleF clippingRectangle, float itemWidth, float itemMargin, object tag)
        //{
        //    PointF drawingPoint = new PointF();

        //    int startIndex, endIndex;
        //    GetDrawingRangeIndecesFromClippingRectange(clippingRectangle, drawingPoint, unitsUnification, out startIndex, out endIndex, itemWidth, itemMargin);

        //    for (int i = startIndex + unitsUnification - 1; i < endIndex && i < MaximumIndex + unitsUnification - 1; i += unitsUnification)
        //    {
        //        int actualIndex = i;
        //        int actualPreviousIndex = i - unitsUnification;

        //        if (actualIndex >= MaximumIndex)
        //        {
        //            actualIndex = MaximumIndex - 1;
        //        }

        //        DrawColorAreaItem(g, ref drawingPoint, pen, fill, actualIndex, 
        //            actualPreviousIndex, itemWidth, itemMargin, tag);

        //        drawingPoint.X = i * (itemMargin + itemWidth);
        //    }
        //}

        protected void DrawColorAreaItem(GraphicsWrapper g, ref PointF drawingPoint, Pen pen, Brush fill, 
            int index, int previousItemIndex, float itemWidth, float itemMargin, object tag)
        {
            float indexValue = GetDrawingValueAt(index, tag);
            float previousItemIndexValue = GetDrawingValueAt(previousItemIndex, tag);

            int unificationCount = index - previousItemIndex;

            for (int i = previousItemIndex; i <= index; i++)
            {
                if (float.IsNaN(previousItemIndexValue))
                {
                    previousItemIndexValue = GetDrawingValueAt(i, tag);
                } else if (float.IsNaN(indexValue))
                {
                    indexValue = GetDrawingValueAt(i, tag);
                }
            }

            if (float.IsNaN(indexValue) || float.IsNaN(previousItemIndexValue))
            {// Failed to find reasonable values to draw.
                return;
            }

            if (fill != null)
            {
                g.FillPolygon(fill, new PointF[] { 
                    drawingPoint, 
                    new PointF(drawingPoint.X + (itemMargin + itemWidth) * unificationCount, drawingPoint.Y), 
                    new PointF(drawingPoint.X + (itemMargin + itemWidth) * unificationCount, drawingPoint.Y + indexValue), 
                    new PointF(drawingPoint.X, drawingPoint.Y + previousItemIndexValue) });
            }

            if (pen != null)
            {
                g.DrawLine(pen, drawingPoint.X, drawingPoint.Y + previousItemIndexValue,
                    drawingPoint.X + (itemMargin + itemWidth) * unificationCount, drawingPoint.Y + indexValue);
            }

        }


        void DrawHistogramBar(GraphicsWrapper g, ref PointF drawingPoint, Pen pen, Brush fill, 
            int index, int previousItemIndex, float itemWidth, float itemMargin, int unitsUnification, object tag)
        {
            float y = drawingPoint.Y;

            float height = 0;
            if (unitsUnification > 1)
            {
                double heightSum = 0;
                int actualSumCount = 0;

                for (int i = previousItemIndex; i <= index; i++)
                {
                    float value = GetDrawingValueAt(i, tag);
                    if (float.IsNaN(value)
                        || float.IsInfinity(value))
                    {
                        continue;
                    }

                    heightSum += value;
                    actualSumCount++;
                }

                if (actualSumCount == 0)
                {
                    return;
                }

                height = (float)(heightSum / actualSumCount);
            }
            else
            {
                height = GetDrawingValueAt(index, tag);
            }


            if (height < 0)
            {
                y += height;
                height = -height;
            }

            if (fill != null)
            {
                g.FillRectangle(fill, drawingPoint.X, y, (itemWidth) * unitsUnification, height);
            }
        }

        protected void DrawIcon(GraphicsWrapper g, Pen pen, Brush fill, Rectangle rectangle)
        {
            if (Visible == false)
            {
                return;
            }

            if (fill != null)
            {
                g.FillRectangle(fill, rectangle);
            }

            if (pen != null)
            {
                g.DrawRectangle(pen, rectangle);
            }
        }


    }
}
