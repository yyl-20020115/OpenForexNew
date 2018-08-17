using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// Class represents the window strip at the top of any drag control.
    /// </summary>
    public partial class DragStripControl : UserControl
    {
        LinearGradientBrush _activeCaptionBrush = null;
        Point _mousePosition = Point.Empty;

        Point _mouseDown = Point.Empty;

        public delegate void DockDelegate(DragControl control, DockStyle style);
        public event DockDelegate DockPaneEvent;

        public delegate void CloseDelegate(DragControl control);
        public event CloseDelegate CloseEvent;

        public DragControl ParentControl { get { return this.Parent as DragControl; } }

        volatile bool _hasFocus = false;
        public bool HasFocus
        {
            get { return _hasFocus; }
            set { _hasFocus = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DragStripControl()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.DesignMode)
            {
                return;
            }

            Blend activeBackColorGradientBlend = new Blend(2);
            activeBackColorGradientBlend.Factors = new float[] { 0.5F, 1.0F };
            activeBackColorGradientBlend.Positions = new float[] { 0.0F, 1.0F };

            //_activeCaptionBrush = new LinearGradientBrush(new PointF(0, 0), new PointF(3, 3), SystemColors.ActiveCaption, SystemColors.Control);
    		Color ActiveBackColorGradientBegin = SystemColors.GradientActiveCaption;
            Color ActiveBackColorGradientEnd = SystemColors.ActiveCaption;

            _activeCaptionBrush = new LinearGradientBrush(new Rectangle(0,0, 10, this.Height), ActiveBackColorGradientBegin, ActiveBackColorGradientEnd, LinearGradientMode.Vertical);
            _activeCaptionBrush.Blend = activeBackColorGradientBlend;

            OnResize(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _mouseDown = Point.Empty;
            if (e.X > 20 && e.X < this.Width - 20)
            {
                _mouseDown = e.Location;
            }

            if (this.ParentControl.Dock == DockStyle.None)
            {// Only floating - bring to front on touch.
                this.ParentControl.BringToFront();
            }
        }

        void PerformDragDrop(Point location)
        {
            DragControl parent = (DragControl)this.Parent;
            parent.DraggableLocation = location;

            // Synchronous drag operation.
            int index = this.Parent.GetHashCode();
                //parent.Parent.Controls.IndexOf(parent);
            DoDragDrop(index, DragDropEffects.All);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_mouseDown != Point.Empty && (e.X - _mouseDown.X != 0 || e.Y - _mouseDown.Y != 0) 
                && e.Button == MouseButtons.Left && Parent.Dock == DockStyle.None)
            {
                PerformDragDrop(e.Location);
            }

            _mousePosition = e.Location;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mousePosition = Point.Empty;
            this.Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.DesignMode)
            {
                return;
            }

            if (_activeCaptionBrush != null)
            {
                //_activeCaptionBrush.ResetTransform();
                //_activeCaptionBrush.ScaleTransform(1.7f * this.Width, this.Height, MatrixOrder.Prepend);
            }

            this.Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.X < 18)
            {
                contextMenuStripMain.Show(this.PointToScreen(e.Location));
            }

            if (e.X > this.Width - 20)
            {
                if (CloseEvent != null)
                {
                    CloseEvent(this.Parent as DragControl);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (this.DesignMode)
            {
                return;
            }

            pe.Graphics.FillRectangle(_activeCaptionBrush, new Rectangle(0, 0, this.Width, this.Height));

            int marginLeft = 18;
            bool hasImage = ParentControl.Image != null;
            if (hasImage)
            {
                if (this.Width > 45)
                {
                    pe.Graphics.DrawImage(ParentControl.Image, 18, 1);
                }
                marginLeft = 36;
            }


            if (this.Width > 45)
            {
                RectangleF textLayoutRectangle = new RectangleF(marginLeft, 2, this.Width - marginLeft - 18, this.Height);
                if (_hasFocus)
                {
                    pe.Graphics.DrawString(Parent.Name, SystemFonts.CaptionFont, SystemBrushes.ActiveCaptionText, textLayoutRectangle);
                }
                else
                {
                    pe.Graphics.DrawString(Parent.Name, SystemFonts.CaptionFont, SystemBrushes.InactiveCaptionText, textLayoutRectangle);
                }
            }

            if (_mousePosition == Point.Empty || _mousePosition.X >= 16)
            {
                pe.Graphics.DrawImageUnscaled(Properties.Resources.square, new Point(1, 1));
            }
            else
            {
                pe.Graphics.DrawImageUnscaled(Properties.Resources.square_triangle, new Point(1, 1));
            }

            Rectangle rectangle = new Rectangle(new Point(this.Width - 18, 1), Properties.Resources.DockPane_Close.Size);
            GraphicsHelper.DrawImageColorMapped(pe.Graphics, Properties.Resources.DockPane_Close, rectangle, Color.Black, Color.White);

            if (_mousePosition != Point.Empty && _mousePosition.X > this.Width - 20)
            {
                rectangle.X++;
                rectangle.Width -= 2;
                rectangle.Height -= 2;
                pe.Graphics.DrawRectangle(Pens.White, rectangle);
            }
                 
        }

        /// <summary>
        /// 
        /// </summary>
        private void topToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DockPaneEvent(this.Parent as DragControl, DockStyle.Top);
        }

        private void leftToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DockPaneEvent(this.Parent as DragControl, DockStyle.Left);
        }

        private void bottomToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DockPaneEvent(this.Parent as DragControl, DockStyle.Bottom);
        }

        private void rightToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DockPaneEvent(this.Parent as DragControl, DockStyle.Right);
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockPaneEvent(this.Parent as DragControl, DockStyle.Fill);
        }

        private void floatingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockPaneEvent(this.Parent as DragControl, DockStyle.None);
        }

        private void contextMenuStripMain_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            floatingToolStripMenuItem.Enabled = ParentControl.DragContainer.AllowFloating;
        }


    }
}
