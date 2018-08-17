using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CommonSupport;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CommonFinancial
{
    /// <summary>
    /// Draws text on the chart's drawing space.
    /// </summary>
    [Serializable]
    public class TextObject : DynamicCustomObject
    {
        Brush _brush = Brushes.White;
        public Brush Brush
        {
            set 
            {
                _brush = value;
            }
        }

        bool _isBuildingText = true;
        public override bool IsBuilding
        {
            get 
            {
                return _controlPoints.Count < 1 || _isBuildingText;
            }
        }

        string _text = "";
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        RectangleF _rectangle = RectangleF.Empty;

        /// <summary>
        /// 
        /// </summary>
        public TextObject(string name)
            : base(name)
        {
            this.DrawingOrder = DrawingOrderEnum.PostSeries;
        }

        public override bool AddBuildingKey(KeyEventArgs key)
        {
            if (key.KeyCode == System.Windows.Forms.Keys.Enter)
            {// Finish building.
                _isBuildingText = false;
            }

            return !IsBuilding;
        }

        public override bool AddBuildingKey(KeyPressEventArgs key)
        {
            if (Char.IsLetterOrDigit(key.KeyChar) || Char.IsNumber(key.KeyChar)
                || Char.IsPunctuation(key.KeyChar) || Char.IsWhiteSpace(key.KeyChar))
            {
                _text += key.KeyChar;
            }
            
            this.Manager.HandleDynamicObjectUpdated(this);
            return !IsBuilding;
        }

        public override bool TrySelect(Matrix transformationMatrix, PointF drawingSpaceSelectionPoint, float absoluteSelectionMargin, bool canLoseSelection)
        {
            if (IsBuilding)
            {
                _isBuildingText = false;
            }

            if (_rectangle.Contains(drawingSpaceSelectionPoint))
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

        public override RectangleF GetContainingRectangle(RectangleF drawingSpace)
        {
            if (_controlPoints.Count < 1)
            {
                return RectangleF.Empty;
            }

            // Base baseMethod relies on control points and is good in this scenario.
            return base.GetContainingRectangle(drawingSpace);
        }

        public override bool AddBuildingPoint(PointF point)
        {
            _controlPoints.Add(point);
            return !IsBuilding;
        }

        public override void Draw(GraphicsWrapper g, PointF? mousePosition, RectangleF clippingRectangle, RectangleF drawingSpace)
        {
            if (Visible == false || _controlPoints.Count < 1)
            {
                return;
            }
            
            if (clippingRectangle.Contains(_controlPoints[0]) == false)
            {// Clipping opitmization.
                return;
            }

            SizeF size = g.MeasureString(_text, DefaultDynamicObjectFont);
            if (size.Height < 0)
            {
                _rectangle = new RectangleF(_controlPoints[0].X, _controlPoints[0].Y + size.Height, size.Width, -size.Height);
            }
            else
            {
                _rectangle = new RectangleF(_controlPoints[0].X, _controlPoints[0].Y, size.Width, size.Height);
            }

            g.DrawString(_text, DefaultDynamicObjectFont, _brush, _controlPoints[0]);

            if (Selected || IsBuilding)
            {
                g.DrawLine(Pens.Red, _rectangle.Location, new PointF(_rectangle.X + _rectangle.Width, _rectangle.Y));
            }
            
            // Rounding rectangle.
            //g.DrawRectangle(Pens.White, _rectangle);

            if (Selected)
            {
                DrawControlPoints(g);
            }

        }
    }
}
