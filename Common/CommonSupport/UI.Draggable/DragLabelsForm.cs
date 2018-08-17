using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class DragLabelsForm : Form
    {
        Label[] _labels = new Label[] { };

        const int BorderingWidth = 2;

        Pen _borderPen = new Pen(Brushes.Olive, BorderingWidth);
        Pen _borderPen2 = new Pen(Brushes.DarkKhaki, BorderingWidth);

        Rectangle _boundRectangle1 = new Rectangle();
        Rectangle _boundRectangle2 = new Rectangle();

        /// <summary>
        /// 
        /// </summary>
        public DragLabelsForm()
        {
            InitializeComponent();

            AllowDrop = true;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _labels = new Label[5];
            _labels[0] = labelDockTop;
            _labels[1] = labelDockRight;
            _labels[2] = labelDockLeft;
            _labels[3] = labelDockCenter;
            _labels[4] = labelDockBottom;
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);

            HandleDragOver(Cursor.Position);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            // Confirm draggable operation.
            drgevent.Effect = DragDropEffects.All;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _boundRectangle1 = new Rectangle(6, 6, this.Width - 12, this.Height - 12);
            _boundRectangle2 = new Rectangle(8, 8, this.Width - 16, this.Height - 16);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawRectangle(_borderPen, _boundRectangle1);
            e.Graphics.DrawRectangle(_borderPen2, _boundRectangle2);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

        }

        //protected override void OnDragOver(DragEventArgs drgevent)
        //{
        //    base.OnDragOver(drgevent);
        //}

        public void HandleDragOver(Point position)
        {
            position = this.PointToClient(position);
            foreach (Label label in _labels)
            {
                if (label.Bounds.Contains(position))
                {
                    label.BackColor = Color.Orange;
                }
                else
                {
                    label.BackColor = Color.FloralWhite;
                }
            }
        }

        public bool IsDragOver(Point location)
        {
            if (GetDragPosition(location).HasValue)
            {
                return true;
            }

            location = this.PointToClient(location);

            // Extra margin to extend the encompassing rectangles a bit, required since there seems to be a 1px bug in the original drag enter - leave algorythm.
            int extraMargin = 1;

            Rectangle topRect = new Rectangle(_boundRectangle1.X - extraMargin, _boundRectangle1.Y - extraMargin, _boundRectangle1.Width + 2 * extraMargin, 2 * BorderingWidth + 2 * extraMargin);
            Rectangle bottomRect = new Rectangle(_boundRectangle1.X - extraMargin, _boundRectangle1.Bottom - 2 * BorderingWidth - extraMargin, _boundRectangle1.Width + 2 * extraMargin, 2 * BorderingWidth + 2 * extraMargin);

            Rectangle leftRect = new Rectangle(_boundRectangle1.X - extraMargin, _boundRectangle1.Y - extraMargin, 2 * BorderingWidth + 2 * extraMargin, _boundRectangle1.Height + 2 * extraMargin);
            Rectangle rightRect = new Rectangle(_boundRectangle1.Right - 2 * BorderingWidth - extraMargin, _boundRectangle1.Y - extraMargin, 2 * BorderingWidth + 2 * extraMargin, _boundRectangle1.Height + 2 * extraMargin);

            return topRect.Contains(location) || bottomRect.Contains(location) || leftRect.Contains(location) || rightRect.Contains(location);
        }


        /// <summary>
        /// 
        /// </summary>
        public DockStyle? GetDragPosition(Point location)
        {
            location = this.PointToClient(location);

            DockStyle? dockStyle = null;
            if (labelDockLeft.Bounds.Contains(location))
            {
                dockStyle = DockStyle.Left;
            }
            else if (labelDockRight.Bounds.Contains(location))
            {
                dockStyle = DockStyle.Right;
            }
            else if (labelDockTop.Bounds.Contains(location))
            {
                dockStyle = DockStyle.Top;
            }
            else if (labelDockBottom.Bounds.Contains(location))
            {
                dockStyle = DockStyle.Bottom;
            }
            else if (labelDockCenter.Bounds.Contains(location))
            {
                dockStyle = DockStyle.Fill;
            }

            return dockStyle;
        }
    }
}
