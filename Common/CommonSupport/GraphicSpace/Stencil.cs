using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace CommonSupport
{
	/// <summary>
	/// Summary description for ObjectItem.
	/// </summary>
	[Serializable]
	public class Stencil
	{
		public Stencil(Space space)
		{
			System.Diagnostics.Debug.Assert(space != null);
            _objectSpace = space;
		}

		public string Name
		{
			get
			{
                if (DocumentTag != null)
                {
                    return "OI[" + DocumentTag.ToString() + "]";
                }
                else
                {
                    return "Object Item";
                }
			}
		}

        protected Object DocumentTag;

		Space _objectSpace;
		public Space Space
		{
			get
			{
                return _objectSpace;
			}
		}

        List<Shape> _shapes = new List<Shape>();
        public List<Shape> Shapes
        {
            get { return _shapes; }
            set { _shapes = value; }
        }

        public Matrix TransformationMatrix
        {
            get
            {
                Matrix matrix = new Matrix();
                matrix.Translate(Position.X, Position.Y);
                return matrix;
            }
        }

        private PointF _position = new PointF();
        public PointF Position
        {
            get { return _position; }
            set { _position = value; }
        }

        bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public bool IsMouseOver
        {
            get
            {
                foreach (Shape shape in Shapes)
                {
                    if (shape.IsMouseOver)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Update the shapes mouse in and mouse out.
        /// </summary>
        /// <param name="mousePosition"></param>
        public void SetMousePosition(PointF mousePosition)
        {
            // Convert to stencil coordinates.
            PointF[] points = new PointF[] { mousePosition };
            Matrix matrix = this.TransformationMatrix.Clone();
            matrix.Invert();
            matrix.TransformPoints(points);

            foreach (Shape shape in Shapes)
            {
                if (shape.IsPointInside(points[0]) && shape.IsMouseOver == false)
                {
                    shape.MouseEnter();
                }

                if (shape.IsPointInside(points[0]) == false && shape.IsMouseOver == true)
                {
                    shape.MouseLeave();
                }
            }
        }

        public bool IsPointInside(PointF point)
        {
            foreach (Shape shape in Shapes)
            {
                if (shape.IsPointInside(point))
                {
                    return true;
                }
            }
            return false;
        }

		/// <summary>
		/// Remove self from object space.
		/// </summary>
		public void Remove()
		{
			if(Space != null)
			{
				Space.RemoveStencil(this);
			}
		}

	}
}
