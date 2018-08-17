using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CommonSupport
{
	/// <summary>
	/// By default the Shape class provides functionality for a rectangle. Done so as it is very common and reusable.
	/// </summary>
	public abstract class Shape
	{
        public Shape(Stencil stencil)
		{
            _stencil = stencil;
		}

        public delegate void OnRenderDelegate(System.Drawing.Graphics g);
        public event OnRenderDelegate OnRenderEvent;

        // Stencil is the object containing multiple shapes.
        Stencil _stencil;
        public Stencil Stencil
        {
            get { return _stencil; }
        }

        Shape _parent;
        public Shape Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public System.Drawing.Color SystemColor
        {
            get
            {
                return System.Drawing.Color.Gainsboro;
            }
        }
        
        System.Drawing.Drawing2D.Matrix _transformationMatrix = new System.Drawing.Drawing2D.Matrix();
        public System.Drawing.Drawing2D.Matrix TransformationMatrix
        {
            get
            {
                System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
                if (Parent != null)
                {
                    matrix.Multiply(Parent.TransformationMatrix);
                }
                matrix.Translate(Position.X, Position.Y);
                return matrix;
            }
        }

        public const int DefaultControlSize = 3;
        public const int DefaultControlBorderSize = 3;

        public ShapeConnectionPoint[] ConnectionPoints
        {
            get
            {
                return new ShapeConnectionPoint[] { new ShapeConnectionPoint(this) };
            }
        }

        SizeF _size = new SizeF(DefaultControlSize, DefaultControlSize);
        public SizeF Size
        {
            get { return _size; }
            set { _size = value; }
        }

        PointF _position = new PointF();
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

		bool _isVisualModified = true;
		public bool IsVisualModified
		{
			get
			{
				return _isVisualModified;
			}
			set
			{
                _isVisualModified = value;
			}
		}

		bool _isMouseOver = false;
		public bool IsMouseOver
		{
			get
			{
				return _isMouseOver;
			}
		}

        Color _backgroundColor = Color.Gray;
		public Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				_backgroundColor = value;
				IsVisualModified = true;
			}
		}

		Color _borderColor = Color.Green;
		public Color BorderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
				IsVisualModified = true;
			}
		}
		
		bool _visible = true;
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				IsVisualModified = true;
			}
		}

        Pen _mainPen = new Pen(Color.IndianRed);
        public Pen MainPen
        {
            get { return _mainPen; }
        }

        Pen _borderPen = new Pen(Color.Lavender);
        public Pen BorderPen
        {
            get { return _borderPen; }
        }

        SolidBrush _mainBrush = new SolidBrush(Color.DarkGreen);
        public SolidBrush MainBrush
        {
            get { return _mainBrush; }
        }

        SolidBrush _mainTextBrush = new SolidBrush(System.Drawing.Color.Fuchsia);
        public SolidBrush MainTextBrush
        {
            get { return _mainTextBrush; }
        }

		public virtual void MouseEnter()
		{
			_isMouseOver = true;
		}

		public virtual void MouseLeave()
		{
			_isMouseOver = false;
		}

        public virtual bool IsPointInside(PointF point)
        {
            return (new System.Drawing.RectangleF(Position.X, Position.Y, Size.Width, Size.Height))
                .IntersectsWith(new System.Drawing.RectangleF((int)point.X, (int)point.Y, 1, 1));
        }

        public virtual void Render(System.Drawing.Graphics graphics)
        {
            if (OnRenderEvent != null)
            {
                OnRenderEvent(graphics);
            }

            foreach (ShapeConnectionPoint connectionPoint in ConnectionPoints)
            {
                graphics.DrawRectangle(new Pen(SystemColor),
                    connectionPoint.RelativePosition.X, connectionPoint.RelativePosition.Y,
                    connectionPoint.RelativePosition.X + Size.Width, connectionPoint.RelativePosition.Y + Size.Height);
            }

        }
	}
}

