using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Needed to rectify problems in the GDI+ drawing mechanism.
    /// All coordinate dependant operations must be wrapped.
    /// Since GDI+ makes many problems when it works with very small (0.001 or less) or very big (100000) values,
    /// all conversions to actual real space are done by for it, by this class.
    /// This way the Graphics transformation is kept at initial state.
    /// </summary>
    public class GraphicsWrapper
    {
        /// <summary>
        /// This must not be exposed, since it is changed upon every draw.
        /// </summary>
        Graphics _g;

        public SmoothingMode SmoothingMode
        {
            get 
            {
                Graphics g = _g;
                if (g == null)
                {
                    return SmoothingMode.Default;
                }

                lock (this) 
                { 
                    return g.SmoothingMode; 
                } 
            }

            set 
            {
                Graphics g = _g;
                if (g == null)
                {
                    return;
                }

                lock (this) 
                { 
                    g.SmoothingMode = value; 
                } 
            }
        }

        /// <summary>
        /// Visible clipping bounds currently applied for graphics.
        /// </summary>
        public RectangleF VisibleClipBounds
        {
            get 
            {
                Graphics g = _g;
                if (g == null)
                {
                    return RectangleF.Empty;
                }

                lock (this) 
                { 
                    return g.VisibleClipBounds; 
                } 
            }
        }

        protected Matrix _drawingSpaceTransform = new Matrix();
        /// <summary>
        /// Transforms from drawing to actual space.
        /// </summary>
        public Matrix DrawingSpaceTransformClone
        {
            get { lock (this) { return _drawingSpaceTransform.Clone(); } }
        }

        volatile bool _drawingSpaceMode = false;
        /// <summary>
        /// Should subsequent drawing be done in drawing space.
        /// Will auto reset on ResetTransform();
        /// </summary>
        public bool DrawingSpaceMode
        {
            get { return _drawingSpaceMode; }
            set { _drawingSpaceMode = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GraphicsWrapper()
        {
            lock (this)
            {
                // Reverted Y axis, since Win32 drawing is top to bottom, and charting is other way round.
                _drawingSpaceTransform.Scale(1, -1);
            }
        }

        public void SynchronizeDrawingSpaceXAxis(GraphicsWrapper masterWrapper)
        {
            lock (this)
            {
                // The current transformation matrix.
                float[] elements = _drawingSpaceTransform.Elements;

                // Modify the matrix only in X direction.
                elements[0] = masterWrapper.DrawingSpaceTransformClone.Elements[0];
                elements[4] = masterWrapper.DrawingSpaceTransformClone.Elements[4];

                _drawingSpaceTransform = new Matrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
            }
        }

        public void SetGraphics(Graphics g)
        {
            lock (this)
            {
                _g = g;
                if (_g != null)
                {
                    _g.ResetTransform();
                }
                _drawingSpaceMode = false;
            }
        }

        public void ScaleDrawingSpaceTransform(float x, float y)
        {
            lock (this)
            {
                _drawingSpaceTransform.Scale(x, y);
            }
        }

        public void ScaleDrawingSpaceTransform(float x, float y, MatrixOrder order)
        {
            lock (this)
            {
                _drawingSpaceTransform.Scale(x, y, order);
            }
        }

        public void TranslateDrawingSpaceTransfrom(float x, float y)
        {
            lock (this)
            {
                _drawingSpaceTransform.Translate(x, y);
            }
        }

        public void TranslateDrawingSpaceTransfrom(float x, float y, MatrixOrder order)
        {
            lock (this)
            {
                _drawingSpaceTransform.Translate(x, y, order);
            }
        }

        public void ResetClip()
        {
            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                g.ResetClip();
            }
        }

        public void SetClip(Rectangle rectangle)
        {
            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                Convert(ref rectangle);
                g.SetClip(rectangle);
            }
        }

        public void SetClip(RectangleF rectangle)
        {
            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                Convert(ref rectangle);
                g.SetClip(rectangle);
            }
        }


        public void Convert(ref Rectangle rectangle)
        {
            lock (this)
            {
                if (_drawingSpaceMode == false)
                {
                    return;
                }
            }

            RectangleF result = DrawingSpaceToActualSpace(rectangle);
            rectangle.X = (int)result.X;
            rectangle.Y = (int)result.Y;
            rectangle.Width = (int)result.Width;
            result.Height = (int)result.Height;
        }

        public void Convert(ref RectangleF rectangle)
        {
            lock (this)
            {
                if (_drawingSpaceMode == false)
                {
                    return;
                }
            }

            RectangleF result = DrawingSpaceToActualSpace(rectangle);
            rectangle.Location = result.Location;
            rectangle.Size = result.Size;
        }

        public void Convert(ref float x, ref float y)
        {
            lock (this)
            {
                if (_drawingSpaceMode == false)
                {
                    return;
                }
            }

            PointF result = DrawingSpaceToActualSpace(new PointF(x, y), true);
            x = result.X;
            y = result.Y;

            if ((x < float.MinValue && x > float.MaxValue) || 
                (y <= float.MinValue && y >= float.MaxValue))
            {
                SystemMonitor.Error("X/Y invalid value.");
                x = 0;
                y = 0;
            }

        }

        public void Convert(ref Point point)
        {
            lock (this)
            {
                if (_drawingSpaceMode == false)
                {
                    return;
                }
            }

            PointF newPoint = DrawingSpaceToActualSpace(point, true);
            point.X = (int)newPoint.X;
            point.Y = (int)newPoint.Y;
        }

        public void Convert(ref PointF point)
        {
            lock (this)
            {
                if (_drawingSpaceMode == false)
                {
                    return;
                }
            }

            PointF newPoint = DrawingSpaceToActualSpace(point, true);
            point.X = newPoint.X;
            point.Y = newPoint.Y;
        }

        /// <summary>
        /// Optimized version of the transform point method of the Matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="point"></param>
        public void TransformPoint(System.Drawing.Drawing2D.Matrix matrix, ref PointF point)
        {
            // Formula.
            // p.x = mat[0][0] * pt.x + mat[0][1] * pt.y + mat[0][2];
            // p.y = mat[1][0] * pt.x + mat[1][1] * pt.y + mat[1][2];

            float[] elements = matrix.Elements;
            // *Important* stores the value, otherwise changed in first calculation.
            float x = point.X;
            point.X = elements[0] * point.X + elements[2] * point.Y + elements[4];
            point.Y = elements[1] * x + elements[3] * point.Y + elements[5];
        }

        /// <summary>
        /// Only the scale and rotate.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="point"></param>
        public void TransformVector(System.Drawing.Drawing2D.Matrix matrix, ref PointF point)
        {
            // Formula.
            // p.x = mat[0][0] * pt.x + mat[0][1] * pt.y;
            // p.y = mat[1][0] * pt.x + mat[1][1] * pt.y;

            float[] elements = matrix.Elements;
            // *Important* stores the value, otherwise changed in first calculation.
            float x = point.X;
            point.X = elements[0] * point.X + elements[2] * point.Y;
            point.Y = elements[1] * x + elements[3] * point.Y;
        }


        public void ReverseConvert(ref SizeF size)
        {
            lock (this)
            {
                if (_drawingSpaceMode == false)
                {
                    return;
                }
            }

            PointF point = ActualSpaceToDrawingSpace(new PointF(size.Width, size.Height), false);
            size.Width = point.X;
            size.Height = point.Y;
        }

        public SizeF MeasureString(string text, Font font)
        {
            Graphics g = _g;
            if (g == null)
            {
                return SizeF.Empty;
            }

            lock (this)
            {
                // How about compensating font size?
                SizeF result = g.MeasureString(text, font);
                ReverseConvert(ref result);
                return result;
            }
        }

        static public void NormalizedRectangle(ref float x, ref float y, ref float width, ref float height)
        {
            if (width < 0)
            {
                x += width;
                width = -width;
            }

            if (height < 0)
            {
                y += height;
                height = -height;
            }
        }


        public void DrawString(string text, Font font, Brush brush, PointF point)
        {
            DrawString(text, font, brush, point.X, point.Y);
        }

        public void DrawString(string text, Font font, Brush brush, float x, float y)
        {
            //// How about compensating font size?
            //Convert(ref x, ref y);
            //GraphicsState state = _g.Save();
            //_g.ScaleTransform(1, _drawingSpaceTransform.Elements[3]);
            //_g.DrawString(text, font, brush, new PointF(x, y * _drawingSpaceTransform.Elements[3]));
            //_g.Restore(state);

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                // How about compensating font size?
                Convert(ref x, ref y);
                g.DrawString(text, font, brush, x, y);
            }
        }

        public void DrawRectangle(Pen pen, Rectangle rectangle)
        {
            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                Convert(ref rectangle);
                g.DrawRectangle(pen, rectangle);
            }
        }


        public void DrawRectangle(Pen pen, RectangleF rectangle)
        {
            DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            RectangleF rectangle = new RectangleF(x, y, width, height);
            Convert(ref rectangle);

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                g.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            Convert(ref x1, ref y1);
            Convert(ref x2, ref y2);
            
            Graphics g = _g;
            if (g == null || pen == null)
            {
                return;
            }
            
            lock (this)
            {
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        public void DrawLine(Pen pen, PointF point1, PointF point2)
        {
            Convert(ref point1);
            Convert(ref point2);

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                g.DrawLine(pen, point1, point2);
            }
        }

        public void DrawPolygon(Pen pen, PointF[] inputPoints)
        {
            PointF[] points = (PointF[])inputPoints.Clone();

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Convert(ref points[i]);
                }
                g.DrawPolygon(pen, points);
            }
        }

        public void FillPolygon(Brush brush, PointF[] inputPoints)
        {
            PointF[] points = (PointF[])inputPoints.Clone();
            
            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Convert(ref points[i]);
                }
                g.FillPolygon(brush, points);
            }
        }

        public void FillRectangle(Brush brush, Rectangle rectangle)
        {
            Convert(ref rectangle);

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                g.FillRectangle(brush, rectangle);
            }
        }

        public void FillRectangle(Brush brush, RectangleF rectangle)
        {
            Convert(ref rectangle);

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                g.FillRectangle(brush, rectangle);
            }
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            RectangleF rectangle = new RectangleF(x, y, width, height);
            Convert(ref rectangle);

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                g.FillRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

        public void DrawImage(Image image, float x, float y)
        {
            DrawImage(image, x, y, image.Width, image.Height);
        }

        public void DrawImage(Image image, PointF point)
        {
            DrawImage(image, point.X, point.Y, image.Width, image.Height);
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            // Here image sizes are compensated.
            RectangleF rectangle = new RectangleF(x, y, width, height);
            Convert(ref rectangle);

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                g.DrawImage(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

        public void DrawImageUnscaledAndClipped(Image image, Rectangle rectangle)
        {
            Convert(ref rectangle);

            Graphics g = _g;
            if (g == null)
            {
                return;
            }

            lock (this)
            {
                g.DrawImageUnscaledAndClipped(image, rectangle);
            }
        }

        /// <summary>
        /// This operates in 1 of 2 modes: 
        /// translation mode (translate the point including scale, and rotation)
        /// vector mode (rotate and scale vector)
        /// </summary>
        public PointF ActualSpaceToDrawingSpace(PointF point, bool translationMode)
        {
            lock (this)
            {
                // Must have external array, for the function to work.
                

                Matrix matrix;
                matrix = _drawingSpaceTransform.Clone();
                if (matrix.IsInvertible == false)
                {// Since some modes result in none invertible matrix, just abandon this calculation.
                    return new PointF();
                }

                matrix.Invert();

                if (translationMode)
                {
                    TransformPoint(matrix, ref point);
                    //matrix.TransformPoints(array);
                }
                else
                {
                    //matrix.TransformVectors(array);
                    TransformVector(matrix, ref point);
                }

                return point;
            }
        }

        public RectangleF ActualSpaceToDrawingSpace(RectangleF rectangle)
        {
            lock (this)
            {
                PointF llCorner = ActualSpaceToDrawingSpace(rectangle.Location, true);
                PointF urCorner = new PointF(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
                urCorner = ActualSpaceToDrawingSpace(urCorner, true);

                // Normalize.
                if (llCorner.X > urCorner.X)
                {
                    float cache = llCorner.X;
                    llCorner.X = urCorner.X;
                    urCorner.X = cache;
                }

                if (llCorner.Y > urCorner.Y)
                {
                    float cache = llCorner.Y;
                    llCorner.Y = urCorner.Y;
                    urCorner.Y = cache;
                }

                return new RectangleF(llCorner.X, llCorner.Y, urCorner.X - llCorner.X, urCorner.Y - llCorner.Y);
            }
        }


        public RectangleF DrawingSpaceToActualSpace(RectangleF rectangle)
        {
            lock (this)
            {
                PointF llCorner = DrawingSpaceToActualSpace(rectangle.Location, true);
                PointF urCorner = new PointF(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
                urCorner = DrawingSpaceToActualSpace(urCorner, true);

                // Normalize.
                if (llCorner.X > urCorner.X)
                {
                    float cache = llCorner.X;
                    llCorner.X = urCorner.X;
                    urCorner.X = cache;
                }

                if (llCorner.Y > urCorner.Y)
                {
                    float cache = llCorner.Y;
                    llCorner.Y = urCorner.Y;
                    urCorner.Y = cache;
                }

                return new RectangleF(llCorner.X, llCorner.Y, urCorner.X - llCorner.X, urCorner.Y - llCorner.Y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PointF DrawingSpaceToActualSpace(PointF point, bool translationMode)
        {
            lock (this)
            {
                //PointF[] points = new PointF[] { point };
                if (translationMode)
                {
                    //DrawingSpaceTransformClone.TransformPoints(points);
                    TransformPoint(_drawingSpaceTransform, ref point);
                }
                else
                {
                    //DrawingSpaceTransformClone.TransformVectors(points);
                    TransformVector(_drawingSpaceTransform, ref point);
                }

                //return points[0];
                return point;
            }
        }

    }
}
