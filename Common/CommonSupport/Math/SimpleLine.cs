using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonSupport
{
    /// <summary>
    /// A simple fast line class to help basic drawing calculations.
    /// Basic equation for a line: Ax+By=C
    /// </summary>
    public class SimpleLine
    {
        PointF _point1;
        PointF _point2;

        public float XDelta
        {
            get { return B; }
        }

        public float YDelta
        {
            get { return A; }
        }


        /// <summary>
        /// A = y2-y1
        /// </summary>
        public float A
        {
            get { return _point2.Y - _point1.Y; }
        }

        /// <summary>
        /// B = x1-x2
        /// </summary>
        public float B
        {
            get { return _point1.X - _point2.X; }
        }

        /// <summary>
        /// C = A*x1+B*y1
        /// </summary>
        public float C
        {
            get { return A * _point1.X + B * _point1.Y; }
        }

        public float Angle
        {
            get
            {
                return (float)Math.Acos(A * B);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public SimpleLine(PointF point1, PointF point2)
        {
            _point1 = point1;
            _point2 = point2;
        }

        /// <summary>
        /// http://www.geocities.com/siliconvalley/2151/math2d.html
        /// </summary>
        /// <param name="rotationAngle"></param>
        static public PointF RotatePoint(PointF point, float rotationAngle)
        {
            float x = (float)Math.Cos(rotationAngle) * point.X - (float)Math.Sin(rotationAngle) * point.Y;
            float y = (float)Math.Sin(rotationAngle) * point.X + (float)Math.Cos(rotationAngle) * point.Y;
            return new PointF(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherLine"></param>
        /// <returns>Returns null to indicate lines are invalid or parralel.</returns>
        public PointF? Intersection(SimpleLine otherLine)
        {
            float det = this.A * otherLine.B - otherLine.A * this.B;
            if(det == 0)
            {
                return null;
            }

            return new PointF( (otherLine.B * this.C - this.B * otherLine.C) / det, 
                (this.A * otherLine.C - otherLine.A * this.C) / det);
        }

        /// <summary>
        /// Formula used as shown here: http://local.wasp.uwa.edu.au/~pbourke/geometry/pointline/
        /// u = ( (x3 - x1)(x2 - x1) + (y3 - y1)(y2 - y1) )/ sqr||p2 - p1||
        /// x = x1 + u(x2 - x1);
        /// y = y1 + u(y2 - y1);
        /// </summary>
        public float? DistanceToPoint(PointF point, out float lineSegmentLocation, out PointF intersectionPoint)
        {
            if (A == 0 && B == 0)
            {// No line.
                lineSegmentLocation = 0;
                intersectionPoint = new PointF();
                return null;
            }

            lineSegmentLocation = (point.X - _point1.X) * (_point2.X - _point1.X) + (point.Y - _point1.Y) * (_point2.Y - _point1.Y);
            float xSquare = (float)Math.Pow(_point2.X - _point1.X, 2);
            float ySquare = (float)Math.Pow(_point2.Y - _point1.Y, 2);

            lineSegmentLocation = lineSegmentLocation / (xSquare + ySquare);

            float intersectionPointX = _point1.X + lineSegmentLocation * (_point2.X - _point1.X);
            float intersectionPointY = _point1.Y + lineSegmentLocation * (_point2.Y - _point1.Y);
            intersectionPoint = new PointF(intersectionPointX, intersectionPointY);

            return (float)Math.Sqrt(Math.Pow(point.X - intersectionPoint.X, 2) + Math.Pow(point.Y - intersectionPoint.Y, 2));
        }
    }
}
