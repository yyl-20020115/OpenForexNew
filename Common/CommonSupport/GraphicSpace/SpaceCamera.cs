using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CommonSupport
{
    public class SpaceCamera
    {
        public Matrix TransformationMatrix
        {
            get
            {
                Matrix matrix = new Matrix();
                matrix.Translate(_position.X, _position.Y);
                matrix.Scale(_zoom, _zoom);
                return matrix;
            }
        }

        PointF _position = new PointF(0, 0);
        public PointF Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        float _zoom = 1;
        public float Zoom
        {
            set
            {
                _zoom = value;
            }
        }

        float _width = 0;
        public float Width
        {
            //get
            //{
            //    return _width;
            //}
            set
            {
                _width = value;
            }
        }

        float _height = 0;
        public float Height
        {
            //get
            //{
            //    return _height;
            //}
            set
            {
                _height = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SpaceCamera()
        {
        }

        /// <summary>
        /// Shows the given point center screen.
        /// </summary>
        /// <param name="point"></param>
        public void ShowPoint(PointF point)
        {
            Position = new PointF(point.X + _width / 2, point.Y + _height / 2);
        }

        public PointF ReverseScalePoint(PointF point)
        {
            point.X = point.X / this._zoom;
            point.Y = point.Y / this._zoom;
            return point;
        }

        /// <summary>
        /// Reverse transforms the point.
        /// </summary>
        public PointF ReverseTransformPoint(PointF point)
        {
            Matrix matrix = TransformationMatrix.Clone();
            matrix.Invert();
            PointF[] resultPoints = new PointF[] { point };
            matrix.TransformPoints(resultPoints);
            return resultPoints[0];
        }

    }
}
