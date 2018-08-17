using System;
using System.Drawing;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// Parent container class for child controls that can be contained/manipulated in a drag container control.
    /// </summary>
    public partial class DragControl : UserControl
    {
        Point _mouseLocation = Point.Empty;

        public enum OperationEnum
        {
            None,
            ReSizeHorizontal,
            ReSizeVertical,
            ReSizeAll
        }

        OperationEnum _operation = OperationEnum.None;

        [NonSerialized]
        Image _image;

        Guid _guid = Guid.NewGuid();
        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        /// <summary>
        /// Showing this image is to be implemented.
        /// </summary>
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        bool _showDragStrip = true;

        /// <summary>
        /// Show or hide the control drag strip at the top.
        /// </summary>
        public bool ShowDragStrip
        {
            get { return _showDragStrip; }
            set 
            { 
                _showDragStrip = value;
                dragStrip.Visible = value;
            }
        }

        public Point DraggableLocation { get; set; }

        Control _controlContained = null;

        public Control ControlContained
        {
            get { return _controlContained; }
            set 
            {
                if (_controlContained == value)
                {
                    return;
                }

                Control previousControl = _controlContained;
                
                _controlContained = value;
                if (_controlContained != null)
                {
                    _controlContained.Parent = this;
                    _controlContained.Dock = DockStyle.None;

                    this.Visible = true;
                }
                
                // Make sure to remove the prev control last, to evade flicker on change.
                if (previousControl != null)
                {
                    previousControl.Parent = null;
                }

                OnResize(EventArgs.Empty);
                this.Invalidate();
            }
        }

        public DragContainerControl DragContainer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DragControl()
        {
            InitializeComponent();

            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.MinimumSize = new Size(18, 20);

            dragStrip.Visible = _showDragStrip;
            OnResize(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void SaveState(SerializationInfoEx info)
        {
            info.AddValue("guid", _guid);
            info.AddValue("location", Location);
            info.AddValue("text", Text);
            info.AddValue("name", Name);
            info.AddValue("dock", Dock);
            info.AddValue("size", Size);
            info.AddValue("showDragStrip", _showDragStrip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void RestoreState(SerializationInfoEx info)
        {
            Guid = info.GetValue<Guid>("guid");
            Location = info.GetValue<Point>("location");
            Text = info.GetString("text");
            Name = info.GetString("name");
            Dock = info.GetValue<DockStyle>("dock");
            Size = info.GetValue<Size>("size");
            if (info.ContainsValue("showDragStrip"))
            {
                ShowDragStrip = info.GetBoolean("showDragStrip");
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();

            if (ControlContained == null)
            {
                return;
            }

            ControlContained.Parent = this;

            int margin = 4;

            ControlContained.Left = margin;

            int dragStripHeight = 0;
            if (ShowDragStrip)
            {
                dragStripHeight = dragStrip.Height;
            }

            ControlContained.Top = dragStripHeight + 2;

            
            if (this.Dock == DockStyle.None)
            {// Free float state.

                if (this.Width < ControlContained.MinimumSize.Width)
                {
                    this.Width = ControlContained.MinimumSize.Width;
                }

                if (this.Height < ControlContained.MinimumSize.Height)
                {
                    this.Height = ControlContained.MinimumSize.Height;
                }

                ControlContained.Width = this.Width - (2 * margin);
                ControlContained.Height = this.Height - 2 - margin - dragStripHeight;
            }
            else
            {
                ControlContained.Width = this.Width - margin;
                ControlContained.Height = this.Height - margin - dragStripHeight;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }


        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (ShowDragStrip)
            {
                this.dragStrip.HasFocus = true;
                this.dragStrip.Invalidate();
            }

            if (this.Dock == DockStyle.None)
            {
                this.BringToFront();
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (ShowDragStrip)
            {
                this.dragStrip.HasFocus = false;
                this.dragStrip.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(this.BackColor);

            Pen pen1 = Pens.Gray;
            Pen pen2 = Pens.DarkGray;
            Pen pen3 = Pens.DimGray;

            if (this.Dock == DockStyle.None)
            {
                e.Graphics.DrawRectangle(pen2, 1, 1, this.Width - 3, this.Height - 3);
                e.Graphics.DrawRectangle(pen3, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        const int ResizeMargin = 15;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.Dock != DockStyle.None)
            {// Only floating controls can be resized this way.
                return;
            }

            if (e.Location.X > this.Width - ResizeMargin)
            {
                Cursor.Current = Cursors.SizeWE;
            }

            if (e.Location.Y > this.Height - ResizeMargin)
            {
                Cursor.Current = Cursors.SizeNS;
            }

            if (e.Location.X > this.Width - ResizeMargin
                && e.Location.Y > this.Height - ResizeMargin)
            {
                Cursor.Current = Cursors.SizeNWSE;
            }

            if (_operation == OperationEnum.ReSizeHorizontal
                || _operation == OperationEnum.ReSizeAll)
            {
                this.Width += e.X - _mouseLocation.X;
            }

            if (_operation == OperationEnum.ReSizeVertical
                || _operation == OperationEnum.ReSizeAll)
            {
                this.Height += e.Y - _mouseLocation.Y;
            }

            _mouseLocation = e.Location;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _mouseLocation = e.Location;

            if (this.Dock != DockStyle.None)
            {// Only free floating are allowed to enter this type of operation.
                return;
            }

            if (e.Location.X > this.Width - ResizeMargin)
            {
                _operation = OperationEnum.ReSizeHorizontal;
            }
            
            if (e.Location.Y > this.Height - ResizeMargin)
            {
                _operation = OperationEnum.ReSizeVertical;
            }

            if (e.Location.X > this.Width - ResizeMargin && 
                e.Location.Y > this.Height - ResizeMargin)
            {
                _operation = OperationEnum.ReSizeAll;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_operation != OperationEnum.None)
            {
                _operation = OperationEnum.None;
            }
        }

    }
}
