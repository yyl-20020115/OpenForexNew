using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonFinancial
{
    class ImagesChartSeries : IndexBasedChartSeries
    {
        PointF _imagesDisplacement = new PointF();
        /// <summary>
        /// What is the default displacement for images - allowing to render them besides/above etc. the graphics.
        /// </summary>
        public PointF ImagesDisplacement
        {
            get
            {
                lock (this)
                {
                    return _imagesDisplacement;
                }
            }
            set
            {
                lock (this)
                {
                    _imagesDisplacement = value;
                }
            }
        }

        List<double> _values = new List<double>();

        public override int ItemsCount
        {
            get { return _values.Count; }
        }

        Dictionary<float, Image> _images = new Dictionary<float, Image>();
        public Dictionary<float, Image> Images
        {
            get
            {
                lock (this)
                {
                    return _images;
                }
            }
        }

        public override void Draw(ChartPane managingPane, GraphicsWrapper g, int unitsUnification, RectangleF clippingRectangle, float itemWidth, float itemMargin)
        {
            PointF drawingPoint = new PointF();

            if (this.Visible == false)
            {
                return;
            }

            lock (this)
            {
                Image image;
                foreach (float value in _values)
                {// Images mode does not apply unit unification
                    if (double.IsNaN(value) == false && drawingPoint.X >= clippingRectangle.X && drawingPoint.X <= clippingRectangle.X + clippingRectangle.Width
                        && _images.ContainsKey((int)value))
                    {
                        image = _images[(int)value];
                        g.DrawImage(image, drawingPoint.X + _imagesDisplacement.X - image.Width / 2, drawingPoint.Y + _imagesDisplacement.Y + (float)value);
                    }

                    drawingPoint.X += itemMargin + itemWidth;
                }
            }
        }


        public override void SetSelectedChartType(string chartType)
        {
        }

        public override void SaveToFile(string fileName)
        {
        }

        public override void GetTotalMinimumAndMaximum(int? startIndex, int? endIndex, ref float minimum, ref float maximum)
        {
            if (startIndex.HasValue == false)
            {
                startIndex = 0;
            }

            if (endIndex.HasValue == false)
            {
                endIndex = _values.Count;
            }

            for (int i = startIndex.Value; i < endIndex.Value && i < _values.Count; i++)
            {
                double value = _values[i];
                if (double.IsNaN(value) == false && double.IsInfinity(value) == false && double.MinValue != value
                    && double.MaxValue != value)
                {
                    minimum = (float)Math.Min(minimum, value);
                    maximum = (float)Math.Max(maximum, value);
                }
            }
        }

        public override void DrawSeriesIcon(GraphicsWrapper g, Rectangle rectangle)
        {
            base.DrawIcon(g, Pens.White, Brushes.Black, rectangle);
        }

        protected override float GetDrawingValueAt(int index, object tag)
        {
            return float.NaN;
        }
    }
}
