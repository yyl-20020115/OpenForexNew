using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CommonSupport;
using System.Drawing.Drawing2D;

namespace CommonFinancial
{
    /// <summary>
    /// Arrow chart object. Draws a pointed arrow.
    /// </summary>
    [Serializable]
    public class ArrowObject : DynamicCustomObject
    {
        Pen _pen = Pens.White;
        public Pen Pen
        {
            get { return _pen; }
            set { _pen = value; }
        }

        Brush _brush = Brushes.White;
        public Brush Brush
        {
            get { return _brush; }
            set { _brush = value; }
        }

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
        public ArrowObject(string name)
            : base(name)
        {
        }

        public override bool TrySelect(Matrix transformationMatrix, PointF drawingSpaceSelectionPoint, float absoluteSelectionMargin, bool canLoseSelection)
        {
            if (IsBuilding)
            {
                return false;
            }

            PointF point1 = _controlPoints[0];
            PointF point2 = _controlPoints[1];

            if (TrySelectControlPoints(transformationMatrix, drawingSpaceSelectionPoint, absoluteSelectionMargin) > 0
                || TrySelectLine(transformationMatrix, true, point1, point2, drawingSpaceSelectionPoint, absoluteSelectionMargin))
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

            if (mousePosition.HasValue)
            {
                point2 = mousePosition.Value;
            }
            else
            {
                point2 = point1;
            }

            if (_controlPoints.Count == 2)
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

            SimpleLine line = new SimpleLine(point1, point2);
            PointF vec = new PointF(line.XDelta, line.YDelta);

            vec = g.DrawingSpaceToActualSpace(vec, false);
            vec = new PointF(vec.Y, vec.X);

            // Rotate
            PointF vec1 = SimpleLine.RotatePoint(vec, -(float)Math.PI/2 + 0.3f);
            PointF vec2 = SimpleLine.RotatePoint(vec, -(float)Math.PI/2 -0.3f);

            // Scale
            vec1 = new PointF(vec1.X * 0.35f, vec1.Y * 0.35f);
            vec2 = new PointF(vec2.X * 0.35f, vec2.Y * 0.35f);

            vec1 = g.ActualSpaceToDrawingSpace(vec1, false);
            vec2 = g.ActualSpaceToDrawingSpace(vec2, false);

            g.DrawLine(_pen, point1, point2);
            g.FillPolygon(_brush, new PointF[] { point2, new PointF(point2.X + vec1.X, point2.Y + vec1.Y), 
                new PointF(point2.X + vec2.X, point2.Y + vec2.Y) });

            if (Selected)
            {
                DrawControlPoints(g);
            }

        }
    }
}
