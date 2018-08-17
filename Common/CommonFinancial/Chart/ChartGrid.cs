using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Implements drawing of a grid.
    /// </summary>
    [Serializable]
    public class ChartGrid
    {
        float? _horizontalLineSpacing = 100;
        public float? HorizontalLineSpacing
        {
            get { return _horizontalLineSpacing; }
            set 
            {
                if (value <= 0)
                {
                    SystemMonitor.Warning("Invalid value fed to the Grid horizontal spacing.");
                    _horizontalLineSpacing = null;
                }
                else
                {
                    _horizontalLineSpacing = value;
                }
            }
        }

        float? _verticalLineSpacing = 20;
        public float? VerticalLineSpacing
        {
            get { return _verticalLineSpacing; }
            set {
                if (value <= 0)
                {
                    SystemMonitor.Warning("Invalid value fed to the Grid vertical spacing.");
                    _verticalLineSpacing = null;
                }
                else
                {
                    _verticalLineSpacing = value;
                }
            }
        }

        bool _considerScale = true;
        
        /// <summary>
        /// Is the grid considering scale, and not drawing too much of itself.
        /// </summary>
        public bool ConsiderScale
        {
            get { return _considerScale; }
            set { _considerScale = value; }
        }

        Pen _pen = Pens.DimGray;
        public Pen Pen
        {
            set { _pen = (Pen)value.Clone(); }
        }

        bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ChartGrid()
        {
        }

        public void Draw(GraphicsWrapper g, RectangleF clipping, RectangleF space, float totalItemWidth)
        {
            if (Visible == false)
            {
                return;
            }

            if (VerticalLineSpacing.HasValue)
            {
                float actualSpacing = _verticalLineSpacing.Value * totalItemWidth;
                if (ConsiderScale)
                {
                    int xScaling = Math.Abs((int) (1 / g.DrawingSpaceTransformClone.Elements[0]));
                    if (xScaling > 1)
                    {
                        actualSpacing = actualSpacing * xScaling;
                    }
                }

                // Set starting to the closes compatible position.
                float starting = (int)(space.Left / actualSpacing);
                starting = starting * actualSpacing;

                if (((space.Right - starting) / actualSpacing) <= 10000)
                {
                    for (float x = starting; x <= space.Right; x += actualSpacing)
                    {// Vertical lines.
                        if (x >= clipping.X && x <= clipping.X + clipping.Width + actualSpacing)
                        {
                            g.DrawLine(_pen, x, Math.Max(space.Top, clipping.Top), x, Math.Min(space.Bottom, clipping.Bottom));
                        }
                    }
                }
                else
                {
                    SystemMonitor.OperationError("Too many drawing steps planned for grid, drawing skipped.");
                }
            }

            if (HorizontalLineSpacing.HasValue)
            {
                float actualSpacing = _horizontalLineSpacing.Value;
                
                if (ConsiderScale 
                    && double.IsInfinity(g.DrawingSpaceTransformClone.Elements[3]) == false
                    && g.DrawingSpaceTransformClone.Elements[3] != 0)
                {
                    int yScaling = Math.Abs((int) (1 / g.DrawingSpaceTransformClone.Elements[3]));
                    if (yScaling > 1)
                    {
                        actualSpacing = actualSpacing * yScaling;
                    }
                }

                // Set starting to the closes compatible position.
                float starting = (int)(space.Top / actualSpacing);
                starting = starting * actualSpacing;

                if (((space.Bottom - starting) / actualSpacing) <= 10000)
                {
                    for (float y = starting; y <= space.Bottom; y += actualSpacing)
                    {// Horizontal lines.
                        if (y >= clipping.Y && y <= clipping.Y + clipping.Height)
                        {
                            g.DrawLine(_pen, Math.Max(space.Left, clipping.Left), y, Math.Min(space.Right, clipping.Right), y);
                        }
                    }
                }
                else
                {
                    SystemMonitor.OperationError("Too many drawing steps planned for grid, drawing skipped.");
                }
            }
        }
    }
}
