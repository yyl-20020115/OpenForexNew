using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonSupport
{
    class CoordinateSystem 
    {
        public CoordinateSystem()
        {
        }

        public void Render(System.Drawing.Graphics graphics)
        {
            Pen pen = new System.Drawing.Pen(System.Drawing.Color.Salmon);

            graphics.DrawLine(pen, 0, 0, 200, 0);
            graphics.DrawLine(pen, 0, 0, 0, 200);
            graphics.DrawRectangle(pen, 0, 0, 20, 20);
            graphics.DrawRectangle(pen, -20, -20, 20, 20);
        }
    }
}
