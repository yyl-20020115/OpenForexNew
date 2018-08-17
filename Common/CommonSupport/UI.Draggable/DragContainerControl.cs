using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CommonSupport.Properties;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.ObjectModel;

namespace CommonSupport
{
    // Give the control a designer category, to ensure design-time compatability
    [DesignerCategoryAttribute("Component")]
    // Make sure we can add child controls at runtime
    // See Microsoft Knowledge Base Article - 813450
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))] 
    public partial class DragContainerControl : UserControl
    {
        List<DragControl> _dragControls = new List<DragControl>();
        public ReadOnlyCollection<DragControl> DragControls
        {
            get { return _dragControls.AsReadOnly(); }
        }

        bool _allowFloating = true;
        /// <summary>
        /// Is floating for controls contained allowed.
        /// </summary>
        public bool AllowFloating
        {
            get { return _allowFloating; }
            set { _allowFloating = value; }
        }

        Dictionary<DragControl, Splitter> _splitters = new Dictionary<DragControl, Splitter>();

        public delegate void DragControlUpdatedDelegate(DragContainerControl container, DragControl control);

        public event DragControlUpdatedDelegate DragControlAdded;
        public event DragControlUpdatedDelegate DragControlRemoved;

        /// <summary>
        /// A label displayed in the most background of the main area, good for general idea what the current
        /// container space is representing.
        /// </summary>
        public string MainAreaText
        {
            get { return labelText.Text; }
            set { labelText.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DragContainerControl()
        {
            InitializeComponent();
            AllowDrop = true;

            // We also need to handle the drag cases of the top level drag labels form.
            _dragLabelsForm.DragDrop += new DragEventHandler(_dragLabelsForm_DragDrop);
            _dragLabelsForm.DragOver += new DragEventHandler(_dragLabelsForm_DragOver);
        }

        void _dragLabelsForm_DragOver(object sender, DragEventArgs e)
        {
            OnDragOver(e);
        }

        void _dragLabelsForm_DragDrop(object sender, DragEventArgs e)
        {
            OnDragDrop(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearControls()
        {
            foreach (DragControl control in _dragControls.ToArray())
            {
                DoRemoveDragControl(control);
            }
        }

        /// <summary>
        /// Control loading.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdatePanels();
        }

        /// <summary>
        /// Each dock style corresponds to an area, None being floating controls
        /// that are directly assigned to the container (this control).
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        List<DragControl> GetAreaControls(DockStyle area)
        {
            ControlCollection controlsCollection = null;
            switch (area)
            {
                case DockStyle.Bottom:
                    controlsCollection = panelBottom.Controls;
                    break;
                case DockStyle.Fill:
                    controlsCollection = panelCenter.Controls;
                    break;
                case DockStyle.Left:
                    controlsCollection = panelLeft.Controls;
                    break;
                case DockStyle.None:
                    controlsCollection = this.Controls;
                    break;
                case DockStyle.Right:
                    controlsCollection = panelRight.Controls;
                    break;
                case DockStyle.Top:
                    controlsCollection = panelTop.Controls;
                    break;
            }

            List<DragControl> result = new List<DragControl>();
            foreach (Control control in controlsCollection)
            {
                if (control is DragControl)
                {
                    result.Add(control as DragControl);
                }
            }

            return result;
        }

        /// <summary>
        /// Save the state of the controls - placement, size etc.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="panel"></param>
        void SavePanelState(SerializationInfoEx info, Panel panel)
        {
            info.AddValue(panel.Name + ".Width", panel.Width);
            info.AddValue(panel.Name + ".Height", panel.Height);
            info.AddValue(panel.Name + ".Visible", panel.Visible);
        }

        /// <summary>
        /// Restore state of the controls.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="panel"></param>
        void RestorePanelState(SerializationInfoEx info, Panel panel)
        {
            if (info.ContainsValue(panel.Name + ".Width"))
            {
                panel.Width = info.GetInt32(panel.Name + ".Width");
            }

            if (info.ContainsValue(panel.Name + ".Height"))
            {
                panel.Height = info.GetInt32(panel.Name + ".Height");
            }

            if (info.ContainsValue(panel.Name + ".Visible"))
            {
                panel.Visible = info.GetBoolean(panel.Name + ".Visible");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DragControl GetDragControlByGuid(Guid guid)
        {
            foreach (DragControl control in _dragControls)
            {
                if (control.Guid == guid)
                {
                    return control;
                }
            }

            return null;
        }

        /// <summary>
        /// Save state of controls (size, location, dock etc.)
        /// </summary>
        /// <param name="info"></param>
        public void SaveState(SerializationInfoEx info)
        {
            // Save panels.
            SavePanelState(info, panelTop);
            SavePanelState(info, panelBottom);
            SavePanelState(info, panelLeft);
            SavePanelState(info, panelRight);
            SavePanelState(info, panelCenter);

            // Save draggable controls.
            DockStyle[] values = (DockStyle[])Enum.GetValues(typeof(DockStyle));
            foreach (DockStyle style in values)
            {
                List<SerializationInfoEx> infos = new List<SerializationInfoEx>();
                foreach (DragControl control in GetAreaControls(style))
                {
                    //if (control.ControlContained == null)
                    //{// Skip empty drag controls.
                    //    continue;
                    //}

                    SerializationInfoEx controlInfo = new SerializationInfoEx();
                    control.SaveState(controlInfo);
                    infos.Add(controlInfo);
                }
                info.AddValue(style.ToString() + "Infos", infos);
            }

        }

        /// <summary>
        /// Restore state of controls. Controls are created and left invisible untill assigned.
        /// </summary>
        /// <param name="info"></param>
        public void RestoreState(SerializationInfoEx info)
        {
            // Clear off any existing drag control from any area.
            DockStyle[] values = (DockStyle[])Enum.GetValues(typeof(DockStyle));
            foreach (DockStyle style in values)
            {
                foreach (DragControl control in GetAreaControls(style))
                {
                    RemoveDragControl(control);
                }
            }

            // Restore draggable controls.
            foreach (DockStyle style in values)
            {
                string fieldName = style.ToString() + "Infos";
                if (info.ContainsValue(fieldName) == false)
                {
                    continue;
                }
                List<SerializationInfoEx> infos = info.GetValue<List<SerializationInfoEx>>(fieldName);
                foreach (SerializationInfoEx controlInfo in infos)
                {
                    DragControl control = new DragControl();
                    control.RestoreState(controlInfo);
                    control.Visible = false;
                    AddDragControl(control);

                    if (style != DockStyle.None)
                    {
                        DockControl(control, style);
                    }
                }
            }

            // Restore panes.
            RestorePanelState(info, panelTop);
            RestorePanelState(info, panelBottom);
            RestorePanelState(info, panelLeft);
            RestorePanelState(info, panelRight);
            RestorePanelState(info, panelCenter);
        }


        public void AddDragControl(DragControl control)
        {
            if (_dragControls.Contains(control))
            {
                return;
            }

            if (Controls.Contains(control) == false)
            {
                Controls.Add(control);
            }

            // Default add floating.
            _dragControls.Add(control);
            control.DragContainer = this;

            control.dragStrip.DoubleClick += new EventHandler(dragStrip_DoubleClick);
            control.dragStrip.CloseEvent += new DragStripControl.CloseDelegate(dragStrip_CloseEvent);
            control.dragStrip.DockPaneEvent += new DragStripControl.DockDelegate(dragStrip_DockPaneEvent);

            control.BringToFront();

            UpdatePanels();

            if (DragControlAdded != null)
            {
                DragControlAdded(this, control);
            }
        }

        void dragStrip_DockPaneEvent(DragControl control, DockStyle style)
        {
            DockControl(control, style);
        }

        /// <summary>
        /// 
        /// </summary>
        void DoRemoveDragControl(DragControl control)
        {
            control.dragStrip.DoubleClick -= new EventHandler(dragStrip_DoubleClick);
            control.dragStrip.CloseEvent -= new DragStripControl.CloseDelegate(dragStrip_CloseEvent);
            control.dragStrip.DockPaneEvent -= new DragStripControl.DockDelegate(dragStrip_DockPaneEvent);
            control.DragContainer = null;

            ClearControlSplitter(control);

            control.Parent.Controls.Remove(control);

            _dragControls.Remove(control);
        }

        /// <summary>
        /// This is the way to remove a control.
        /// Removing from "Controls" will not do.
        /// </summary>
        public void RemoveDragControl(DragControl control)
        {
            if (_dragControls.Contains(control) == false)
            {
                return;
            }

            DoRemoveDragControl(control);

            UpdatePanels();

            if (DragControlRemoved != null)
            {
                DragControlRemoved(this, control);
            }
        }

        public bool ControlIsDocked(DragControl control)
        {
            return control.Dock != DockStyle.None; //(control.Parent != this);
        }

        public DragControl GetContainerFill(Control panel)
        {
            foreach (Control control in panel.Controls)
            {
                if (control.Dock == DockStyle.Fill && control is DragControl)
                {
                    return (DragControl)control;
                }
            }
            return null;
        }


        void SetControlSplitter(Control container, DragControl control)
        {
            if (control.Dock != DockStyle.Fill)
            {
                Splitter splitter = new SplitterEx();
                splitter.Width = 6;
                splitter.Height = 6;

                splitter.Dock = GetContainerDefaultDockStyle(container);
                splitter.BackColor = SystemColors.Control;
                container.Controls.Add(splitter);
                _splitters[control] = splitter;
            }
        }

        /// <summary>
        /// Add a control to a given control containing pane.
        /// </summary>
        void AddControlToContainer(Control container, DragControl control)
        {
            if (container.Controls.Contains(control) == false)
            {// Not already added.
                DragControl currentFill = GetContainerFill(container);

                if (currentFill != null && currentFill != control)
                {// A fill already exists

                    //if (addAsFill)
                    //{// But we need to swap since the new one want to be fill now.
                    //    control.Dock = DockStyle.Fill;

                    //    // Remove now so we can add the new one properly.
                    //    container.Controls.Remove(currentFill);
                        
                    //    WinFormsHelper.BeginManagedInvoke(this, delegate()
                    //    {// And this will be executed as soon as we finish the current operation trough the Invoke mechanism.
                    //        currentFill.Height = 100;
                    //        AddControlToContainer(container, currentFill, false);
                    //    });

                    //}
                    //else
                    //{
                        control.Dock = GetContainerDefaultDockStyle(container);
                    //}
                }
                else
                {
                    control.Dock = DockStyle.Fill;
                }
            }

            SetControlSplitter(container, control);
            container.Controls.Add(control);
        }

        void ClearControlSplitter(DragControl control)
        {
            if (_splitters.ContainsKey(control) && _splitters[control] != null)
            {
                _splitters[control].Parent.Controls.Remove(_splitters[control]);
                _splitters.Remove(control);
            }
        }

        public void DockControl(DragControl dragControl, DockStyle dockPane)
        {
            if (_dragControls.Contains(dragControl) == false)
            {
                SystemMonitor.Warning("Miuse of method.");
                return;
            }

            ClearControlSplitter(dragControl);

            switch (dockPane)
            {
                case DockStyle.None:
                    {// Floating.
                        if (AllowFloating)
                        {
                            dragControl.Dock = DockStyle.None;
                            dragControl.Parent = this;
                            dragControl.BringToFront();
                        }
                        else
                        {
                            AddControlToContainer(panelCenter, dragControl);
                        }
                    }
                    break;
                case DockStyle.Bottom:
                    AddControlToContainer(panelBottom, dragControl);
                    break;
                case DockStyle.Fill:
                    AddControlToContainer(panelCenter, dragControl);
                    break;
                case DockStyle.Left:
                    AddControlToContainer(panelLeft, dragControl);
                    break;
                case DockStyle.Right:
                    AddControlToContainer(panelRight, dragControl);
                    break;
                case DockStyle.Top:
                    AddControlToContainer(panelTop, dragControl);
                    break;
                default:
                    break;
            }

            UpdatePanels();
        }

        void UpdatePanels()
        {
            panelBottom.Visible = panelBottom.Controls.Count != 0;
            panelLeft.Visible = panelLeft.Controls.Count != 0;
            panelRight.Visible = panelRight.Controls.Count != 0;
            panelTop.Visible = panelTop.Controls.Count != 0;
            panelCenter.Visible = panelCenter.Controls.Count != 0;

            splitterBottom.Visible = panelBottom.Controls.Count != 0;
            splitterLeft.Visible = panelLeft.Controls.Count != 0;
            splitterRight.Visible = panelRight.Controls.Count != 0;
            splitterTop.Visible = panelTop.Controls.Count != 0;

            UpdateContainerControls(panelCenter);
            UpdateContainerControls(panelBottom);
            UpdateContainerControls(panelLeft);
            UpdateContainerControls(panelRight);
            UpdateContainerControls(panelTop);

            foreach (Control control in this.Controls)
            {
                if (control is DragControl && ((DragControl)control).Dock == DockStyle.None)
                {
                    control.BringToFront();
                }
            }
        }

        void UpdateContainerControls(Control container)
        {
            if (container.Visible == false)
            {
                return;
            }

            if (GetContainerFill(container) != null)
            {
                return;
            }

            foreach (Control control in container.Controls)
            {
                if (control is DragControl)
                {// Assign the first found to be the new fill.
                    ClearControlSplitter(control as DragControl);
                    control.Dock = DockStyle.Fill;
                    break;
                }
            }
        }


        void dragStrip_CloseEvent(DragControl control)
        {
            RemoveDragControl(control);
        }

        void dragStrip_DoubleClick(object sender, EventArgs e)
        {
            DragControl control = ((DragStripControl)sender).Parent as DragControl;

            if (control.Dock == DockStyle.Fill && AllowFloating)
            {
                DockControl(control, DockStyle.None);
            }
            else if (control.Dock == DockStyle.None)
            {
                DockControl(control, DockStyle.Fill);
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            // Confirm draggable operation.
            drgevent.Effect = DragDropEffects.All;

            Control control = GetDragControlFromData(drgevent);
            
            DoBeginDrag(control);
        }

        volatile bool _isDragging = false;
        DragLabelsForm _dragLabelsForm = new DragLabelsForm();

        public void DoBeginDrag(Control control)
        {
            if (_isDragging)
            {
                return;
            }

            _isDragging = true;

            Point location = Parent.PointToScreen(this.Location);
            location.X += 2;
            location.Y += 2;
            _dragLabelsForm.Location = location;

            Size size = this.Size;
            size.Width -= 4;
            size.Height -= 4;
            _dragLabelsForm.Size = size;

            _dragLabelsForm.Show();
        }

        /// <summary>
        /// Perform end drag operation.
        /// </summary>
        /// <returns>The style if must dock otherwise null.</returns>
        public DockStyle? DoEndDrag(Control control, Point location)
        {
            if (_isDragging == false)
            {// We have dragged a toolbar item left or right, no actual component drag occured.
                return DockStyle.None;
            }

            _isDragging = false;

            DockStyle? dockStyle = _dragLabelsForm.GetDragPosition(location);
            _dragLabelsForm.Hide();

            if (control is DragControl && dockStyle.HasValue)
            {
                DockControl((DragControl)control, dockStyle.Value);
            }

            return dockStyle;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            Control control = GetDragControlFromData(drgevent);
            Point location = new Point(drgevent.X, drgevent.Y);
            //if (control is DragControl)
            {
                DoEndDrag(control, location);
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);

            // If we lost drag since we are over the labels form, keep dragging.
            if (_dragLabelsForm.IsDragOver(Cursor.Position))
            {// Entered a zone of the form.
                return;
            }

            DoEndDrag(this, Cursor.Position);
        }

        public void DoDragOver(Point screenPoint, Control control)
        {
            if (control is DragControl)
            {
                Point p = this.PointToClient(screenPoint);

                control.Location = new Point(
                        p.X - ((DragControl)control).DraggableLocation.X/* - 2*/,
                        p.Y - ((DragControl)control).DraggableLocation.Y/* - 2*/);
            }

            _dragLabelsForm.HandleDragOver(screenPoint);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            drgevent.Effect = DragDropEffects.Move;

            Control control = GetDragControlFromData(drgevent);
            DoDragOver(new Point(drgevent.X, drgevent.Y), control);
        }

        /// <summary>
        /// 
        /// </summary>
        public DragControl GetDragControlFromContainedControl(Control control)
        {
            if (control == null)
            {
                return null;
            }

            foreach(DragControl dragControl in _dragControls)
            {
                if (dragControl.ControlContained == control)
                {
                    return dragControl;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        Control GetDragControlFromData(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(int)))
            {
                int index = (int)drgevent.Data.GetData(typeof(int));
                foreach (Control control in Controls)
                {
                    if (control is DragControl)
                    {
                        if (control.GetHashCode() == index)
                        {
                            return (DragControl)control;
                        }
                    }
                }
            }
            else if (drgevent.Data.GetDataPresent(typeof(Control)))
            {// Dragging a new control from somewhere.
                return (Control)drgevent.Data.GetData(typeof(Control));
            }

            return null;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        DockStyle GetContainerDefaultDockStyle(Control container)
        {
            if (container == panelCenter || container == panelLeft || container == panelRight)
            {
                return DockStyle.Bottom;
            }
            else if (container == panelBottom || container == panelTop)
            {
                return DockStyle.Right;
            }

            System.Diagnostics.Debug.Fail("Unexpected.");
            return DockStyle.None;
        }

        private void DragContainerControl_BackColorChanged(object sender, EventArgs e)
        {
            splitterBottom.BackColor = SystemColors.Control;
            splitterLeft.BackColor = SystemColors.Control;
            splitterRight.BackColor = SystemColors.Control;
            splitterTop.BackColor = SystemColors.Control;

            foreach (Splitter splitter in _splitters.Values)
            {
                splitter.BackColor = SystemColors.Control;
            }

        }

    }
}
