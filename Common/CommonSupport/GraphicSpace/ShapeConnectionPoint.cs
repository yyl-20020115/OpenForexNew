using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonSupport
{
    public class ShapeConnectionPoint
    {
        public ShapeConnectionPoint(Shape shape)
        {
            _shape = shape;
        }
        
        Shape _shape;
        public Shape Shape
        {
            get { return _shape; }
        }

        PointF _relativePosition = new PointF();
        public PointF RelativePosition
        {
            get
            {
                return _relativePosition;
            }
            set
            {
                _relativePosition = value;
            }
        }

    }
}
