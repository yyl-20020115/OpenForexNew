using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonFinancial
{
    /// <summary>
    /// Static chart object, draw lines at the levels given (for ex. 0 level, 50% level etc.)
    /// </summary>
    [Serializable]
    public class LevelLinesObject : CustomObject
    {
        List<double> _levels = new List<double>();
        public List<double> Levels
        {
            get { return _levels; }
        }

        Pen _pen = Pens.DarkGray;
        public Pen Pen
        {
            set { _pen = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public LevelLinesObject()
            : base()
        {
            this.DrawingOrder = DrawingOrderEnum.PreSeries;
        }

        public override void Draw(GraphicsWrapper g, PointF? mousePosition, RectangleF clippingRectangle, RectangleF drawingSpace)
        {
            if (Visible == false)
            {
                return;
            }

            foreach (double level in _levels)
            {
                if (level >= drawingSpace.Y && level <= drawingSpace.Y + drawingSpace.Height)
                {
                    g.DrawLine(_pen, drawingSpace.X, (float)level, drawingSpace.X + drawingSpace.Width, (float)level);
                }
            }
        }
    }
}
