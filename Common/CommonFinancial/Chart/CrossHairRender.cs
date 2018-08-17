using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Class handles functionality related to rendering a cross hair using the "ReversibleLine" drawing technicue.
    /// </summary>
    public class CrossHairRender
    {
        Control _ownerControl;

        Point? _lastPointLeft = null;
        Point? _lastPointRight = null;
        Point? _lastPointUp = null;
        Point? _lastPointDown = null;

        volatile bool _drawnHorizontal = false;
        volatile bool _drawnVertical = false;

        public Rectangle? LastRectangle
        {
            get
            {
                if (_lastPointLeft.HasValue && _lastPointRight.HasValue && _lastPointUp.HasValue && _lastPointDown.HasValue)
                {
                    return new Rectangle(_lastPointLeft.Value.X, _lastPointUp.Value.Y, _lastPointRight.Value.X - _lastPointLeft.Value.X, _lastPointDown.Value.Y - _lastPointUp.Value.Y);
                }

                return null;
            }
        }

        public Point? LastPoint
        {
            get
            {
                if (_lastPointUp.HasValue && _lastPointLeft.HasValue)
                {
                    return new Point(_lastPointUp.Value.X, _lastPointLeft.Value.Y);
                }

                return null;
            }
        }

        volatile bool _visible = false;

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public delegate void CrossHairShowDelegate(CrossHairRender renderer, Point location);
        /// <summary>
        /// Event emmited in screen coordinates.
        /// </summary>
        public event CrossHairShowDelegate CrossHairShowEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CrossHairRender(Control ownerControl)
        {
            _ownerControl = ownerControl;
            _ownerControl.Paint += new PaintEventHandler(_ownerControl_Paint);
        }

        void _ownerControl_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle.Width >= _ownerControl.Width
                && e.ClipRectangle.Height >= _ownerControl.Height)
            {
                _drawnHorizontal = false;
                _drawnVertical = false;
                WinFormsHelper.BeginManagedInvoke(_ownerControl, delegate() { Redraw(); });
            }
        }

        void _ownerControl_Invalidated(object sender, InvalidateEventArgs e)
        {
            if (e.InvalidRect.Width >= _ownerControl.Width
                && e.InvalidRect.Height >= _ownerControl.Height)
            {
                _drawnHorizontal = false;
                _drawnVertical = false;
            }
        }

        /// <summary>
        /// Clear the previous crosshair.
        /// </summary>
        public void ClearCrossHair()
        {
            if (_drawnHorizontal)
            {
                _drawnHorizontal = false;
                if (_lastPointLeft.HasValue && _lastPointRight.HasValue)
                {
                    ControlPaint.DrawReversibleLine(_lastPointLeft.Value, _lastPointRight.Value, _ownerControl.BackColor);
                }
            }

            if (_drawnVertical)
            {
                _drawnVertical = false;
                if (_lastPointUp.HasValue && _lastPointDown.HasValue)
                {
                    ControlPaint.DrawReversibleLine(_lastPointUp.Value, _lastPointDown.Value, _ownerControl.BackColor);
                }
            }
        }

        /// <summary>
        /// Redraw the crosshair at last coordinates.
        /// </summary>
        public void Redraw()
        {
            Point? lastPoint = LastPoint;
            if (LastRectangle.HasValue && lastPoint.HasValue)
            {
                DrawAt(LastRectangle.Value, LastPoint.Value, true);
            }
        }

        /// <summary>
        /// Draw the crosshair at given coordinates.
        /// </summary>
        public void DrawAt(Rectangle screenClippingRectangle, Point screenLocation, bool clearPreviousDrawn)
        {
            Point pointLeft = new Point(screenClippingRectangle.X, screenLocation.Y);
            Point pointRight = new Point(screenClippingRectangle.X + screenClippingRectangle.Width, screenLocation.Y);

            Point pointUp = new Point(screenLocation.X, screenClippingRectangle.Y);
            Point pointDown = new Point(screenLocation.X, screenClippingRectangle.Y + screenClippingRectangle.Height);

            if (clearPreviousDrawn)
            {
                ClearCrossHair();
            }

            if (_visible == false)
            {
                return;
            }

            _lastPointLeft = pointLeft;
            _lastPointRight = pointRight;
            _lastPointUp = pointUp;
            _lastPointDown = pointDown;
            
            if (_ownerControl.RectangleToScreen(new Rectangle(0,0, _ownerControl.Width, _ownerControl.Height)).Contains(
                new Point((pointLeft.X + pointRight.X) / 2, pointLeft.Y)))
            {
                _drawnHorizontal = true;
                ControlPaint.DrawReversibleLine(pointLeft, pointRight, _ownerControl.BackColor);
            }

            //if (_ownerControl.PointToScreen(new Point(0, _ownerControl.Top)).y <= pointUp.Y
            //    && _ownerControl.PointToScreen(new Point(0, _ownerControl.Bottom)).y >= pointDown.Y)
            {
                _drawnVertical = true;
                ControlPaint.DrawReversibleLine(pointUp, pointDown, _ownerControl.BackColor);
            }

            if (CrossHairShowEvent != null)
            {
                CrossHairShowEvent(this, screenLocation);
            }

        }

    }
}
