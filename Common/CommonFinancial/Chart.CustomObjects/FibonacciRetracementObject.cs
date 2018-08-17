using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CommonSupport;
using System.Drawing.Drawing2D;

namespace CommonFinancial
{
    /// <summary>
    /// Fibbonacci retracement object draws line on the chart at specific fibonacci levels.
    /// </summary>
    [Serializable]
    public class FibonacciRetracementObject : DynamicCustomObject
    {
        Color _penColor = Color.White;
        public Color PenColor
        {
            set 
            { 
                _penColor = value;
                _solidLinePen = new Pen(_penColor);
                _dashedLinePen = new Pen(_penColor);
                _dashedLinePen.DashPattern = new float[] { 3, 3 };
                _dashedLinePen.DashStyle = DashStyle.Custom;
            }
        }

        Pen _dashedLinePen;
        Pen _solidLinePen;

        public override bool IsBuilding
        {
            get 
            {
                return _controlPoints.Count < 2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FibonacciRetracementObject(string name)
            : base(name)
        {
            this.DrawingOrder = DrawingOrderEnum.PostSeries;
        }

        public override RectangleF GetContainingRectangle(RectangleF drawingSpace)
        {
            if (_controlPoints.Count < 2)
            {
                return RectangleF.Empty;
            }

            // Base baseMethod relies on control points and is good in this scenario.
            return base.GetContainingRectangle(drawingSpace);
        }

        public override bool TrySelect(Matrix transformationMatrix, PointF drawingSpaceSelectionPoint, float absoluteSelectionMargin, bool canLoseSelection)
        {
            if (IsBuilding)
            {
                return false;
            }

            if (TrySelectControlPoints(transformationMatrix, drawingSpaceSelectionPoint, absoluteSelectionMargin) > 0
                || TrySelectLine(transformationMatrix, true, _controlPoints[0], _controlPoints[1], drawingSpaceSelectionPoint, absoluteSelectionMargin))
            {
                Selected = true;
                return true;
            }

            if (canLoseSelection)
            {
                Selected = false;
            }

            return false;
        }

        public override bool AddBuildingPoint(PointF point)
        {
            _controlPoints.Add(point);
            return !IsBuilding;
        }

        public override void Draw(GraphicsWrapper g, PointF? mousePosition, RectangleF clippingRectangle, RectangleF drawingSpace)
        {
            if (Visible == false)
            {
                return;
            }

            if (_controlPoints.Count < 1)
            {
                return;
            }

            PointF point1 = _controlPoints[0];
            PointF point2;
            if (_controlPoints.Count < 2)
            {
                point2 = mousePosition.Value;
            }
            else
            {
                point2 = _controlPoints[1];
            }

            // Clipping opitmization.
            RectangleF rectangle = new RectangleF(
                Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y),
                Math.Abs(point2.X - point1.X), Math.Abs(point2.Y - point1.Y));

            if (rectangle.IntersectsWith(clippingRectangle) == false)
            {
                return;
            }

            // Draw base line.
            g.DrawLine(_dashedLinePen, point1, point2);

            float baseLevel = point1.Y;
            float height = point2.Y - point1.Y;

            // Draw fibbonacci levels.
            float[] levels = new float[] { 0, 23.6f, 38.2f, 50, 61.8f, 100 };
            for (int i = 0; i < levels.Length; i++)
            {
                float actualLevel = baseLevel + height * levels[i] / 100f;
                g.DrawLine(_solidLinePen, point1.X, actualLevel, point2.X, actualLevel);
                g.DrawString(levels[i].ToString(), DefaultDynamicObjectFont, Brushes.White, point1.X, actualLevel);
            }

            

            if (Selected)
            {
                DrawControlPoints(g);
            }

        }
    }
}
