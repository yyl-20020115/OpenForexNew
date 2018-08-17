using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CommonSupport
{
    public partial class RenderControl : UserControl
    {
        public RenderControl()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles(); 
        }

        /// <summary>
        /// Smooth drag and pan.
        /// </summary>
        bool SmoothDragging = true;
        bool SuperSmoothRefresh = true;
        const float PanSpeedMultiplicator = 1;
        PointF _mouseDownPosition = new PointF(float.MaxValue, float.MaxValue);

        public delegate void ObjectsAtMousePosDelegate(List<Stencil> objects);
        public event ObjectsAtMousePosDelegate ObjectsAtMousePosEvent;

        CoordinateSystem _coordinateSystem = new CoordinateSystem();

        Space _space;
        public Space Space
        {
            get
            {
                if (this.DesignMode && _space == null)
                {// Assign a default design time object space, to prevent crash.
                    _space = new Space();
                }
                return _space;
            }
            set
            {
                _space = value;
                _space.Camera.Width = this.Width;
                _space.Camera.Height = this.Height;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.DesignMode == false)
            {
                Invalidate();
            }
        }

        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            if (comboBoxZoom.SelectedIndex < comboBoxZoom.Items.Count - 1)
            {
                comboBoxZoom.SelectedIndex++;
            }
            Invalidate();
        }

        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            if (comboBoxZoom.SelectedIndex > 0)
            {
                comboBoxZoom.SelectedIndex--;
            }
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _mouseDownPosition = new PointF(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                // Set mouse pos, so that the ObjectsAtMousePosition are fresh.
                Space.SetMousePosition(Space.DrawSpaceToObjectSpace(new PointF(e.X, e.Y)));

                if (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Shift)
                {
                    List<Stencil> items = new List<Stencil>(Space.SelectedStencils);
                    foreach(Stencil item in Space.ObjectsAtMousePosition)
                    {
                        if (items.Contains(item) == false)
                        {
                            items.Add(item);
                            //break;
                        }
                    }
                    
                    Space.SetSelectedStencilsFromList(items);
                }
                else
                {
                    if (Space.ObjectsAtMousePosition.Count > 0)
                    {
                        Space.SelectedStencils = new Stencil[] { Space.ObjectsAtMousePosition[0] };
                    }
                    else
                    {
                        Space.SelectedStencils = new Stencil[] { };
                    }

                    //if (Space.AllObjectsAtMousePositionAreSelected == false)
                    //{// Only when click-selecting new objects, break the current selection.
                        //Space.SelectedStencils = Space.ObjectsAtMousePosition;
                    //}
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                Space.SetMousePosition(Space.DrawSpaceToObjectSpace(new PointF(e.X, e.Y)));
                
                if (Space.ObjectsAtMousePosition.Count > 0)
                {
                    throw new Exception("Unimplemented");
                    //ShowObjectContextMenu(sender, e);
                }

                if (Space.AllObjectsAtMousePositionAreSelected == false && Space.ObjectsAtMousePosition.Count > 0)
                {// Only when click-selecting new objects, break the current selection.
                    Space.SetSelectedStencilsFromList(Space.ObjectsAtMousePosition);
                }
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _mouseDownPosition = new PointF(float.MaxValue, float.MaxValue);

            if (e.Button == MouseButtons.Left && Space.SelectedStencils.Length > 0)
            {// Move selected objects.
                Cursor = System.Windows.Forms.Cursors.SizeAll;
                foreach (Stencil stencil in Space.SelectedStencils)
                {
                    PointF newPosition = stencil.Position;
                    stencil.Position = Space.Snap.RepositionPoint(newPosition);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Cursor = System.Windows.Forms.Cursors.Default;
         
            Space.SetMousePosition(Space.DrawSpaceToObjectSpace(new PointF(e.X, e.Y)));



            if (ObjectsAtMousePosEvent != null)
            {
                ObjectsAtMousePosEvent(Space.ObjectsAtMousePosition);
            }

            bool refreshNeeded = SuperSmoothRefresh;
            if (_mouseDownPosition.X != float.MaxValue)
            {
                PointF deltaPoint = new PointF(e.X - _mouseDownPosition.X, e.Y - _mouseDownPosition.Y);

                if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
                {// Pan the Camera.
                    Cursor = System.Windows.Forms.Cursors.Hand;

                    deltaPoint.X *= PanSpeedMultiplicator;
                    deltaPoint.Y *= PanSpeedMultiplicator;

                    Space.Camera.Position = new PointF(Space.Camera.Position.X + deltaPoint.X, 
                        Space.Camera.Position.Y + deltaPoint.Y);
                }

                if (e.Button == MouseButtons.Left && Space.SelectedStencils.Length > 0)
                {// Move selected objects.
                    Cursor = System.Windows.Forms.Cursors.SizeAll;

                    // Compensate the camera zoom.
                    deltaPoint = Space.Camera.ReverseScalePoint(deltaPoint);

                    foreach(Stencil stencil in Space.SelectedStencils)
                    {
                        stencil.Position = new PointF(stencil.Position.X + deltaPoint.X, stencil.Position.Y + deltaPoint.Y);
                    }
                }

                if (SmoothDragging)
                {
                    refreshNeeded = true;
                }
                _mouseDownPosition = new PointF(e.X, e.Y);
            }

            if (refreshNeeded)
            {
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
         
            if (e.Delta > 0)
            {
                for (int i = 0; i < (e.Delta / 120); i++)
                {
                    buttonZoomIn_Click(null, EventArgs.Empty);
                }
            }

            if (e.Delta < 0)
            {
                for (int i = 0; i < -(e.Delta / 120); i++)
                {
                    buttonZoomOut_Click(null, EventArgs.Empty);
                }
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            comboBoxZoom.SelectedIndex = 4;
            Space.Camera.ShowPoint(new PointF(0, 0));
        }

        private void comboBoxZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            string zoomString = comboBoxZoom.SelectedItem.ToString().Replace("%", "");
            decimal zoom = System.Convert.ToDecimal(zoomString, new System.Globalization.NumberFormatInfo());
            Space.Camera.Zoom = (float)zoom / 100.0f;
            this.Focus();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Space != null)
            {
                Space.Camera.Width = this.Width;
                Space.Camera.Height = this.Height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Space == null)
            {
                return;
            }

            e.Graphics.Clear(Space.BackgroundColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Matrix spaceMatrix = Space.Camera.TransformationMatrix.Clone();
            //spaceMatrix.Translate(Space.Camera.Position.X, Space.Camera.Position.Y);
            //spaceMatrix.Scale(Space.Camera.Zoom, Space.Camera.Zoom);

            foreach (Stencil stencil in Space.Stencils)
            {// Render each shape.

                e.Graphics.Transform = spaceMatrix;

                Matrix stencilMatrix = spaceMatrix.Clone();
                stencilMatrix.Multiply(stencil.TransformationMatrix);

                foreach (Shape shape in stencil.Shapes)
                {
                    Matrix shapeMatrix = stencilMatrix.Clone();
                    shapeMatrix.Multiply(shape.TransformationMatrix);
                    e.Graphics.Transform = shapeMatrix;
                    shape.Render(e.Graphics);
                }
            }

            e.Graphics.Transform = spaceMatrix;
            _coordinateSystem.Render(e.Graphics);
        }
    }
}
