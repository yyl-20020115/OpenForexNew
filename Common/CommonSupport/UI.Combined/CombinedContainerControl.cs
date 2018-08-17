using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonSupport.Properties;

namespace CommonSupport
{
    /// <summary>
    /// Provides containment for UI controls. Controls can be in 1 of 2 states
    /// floating or part of the tabs.
    /// 3 types of state for a control - tabbed (ToolStripButton), docked at workspace (ToolStripButton and/or DragControl), floating (Form)
    /// </summary>
    public partial class CombinedContainerControl : UserControl
    {
        BiDictionary<ToolStripButton, CommonBaseControl> _tabbedButtonsControls = new BiDictionary<ToolStripButton, CommonBaseControl>();
        BiDictionary<CombinedHostingForm, CommonBaseControl> _floatingFormsControls = new BiDictionary<CombinedHostingForm, CommonBaseControl>();

        CommonBaseControl _checkedControl = null;

        DragControl _checkedControlContainer = null;

        Point? _lastMouseDown = null;

        ImageList _imageList = null;
        /// <summary>
        /// Image list to server as source for components images.
        /// </summary>
        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }

        ToolStripSeparator _dragSeparator = new ToolStripSeparator();

        public bool ShowComponentsTabsTitles
        {
            get { return toolStripButtonTitlesText.Checked; }
            set 
            { 
                toolStripButtonTitlesText.Checked = value;
                UpdateUI();
            }
        }


        public delegate void ControlUpdateDelegate(CombinedContainerControl containerControl, CommonBaseControl control);

        public event ControlUpdateDelegate ContainedControlRemovedEvent;
        public event ControlUpdateDelegate SelectedTabbedControlChangedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CombinedContainerControl()
        {
            InitializeComponent();
            toolStripMain.AllowDrop = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            toolStripLabelTitle.Visible = false;
            //this.ParentForm.Activated += new EventHandler(ParentForm_Activated);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RestoreState(SerializationInfoEx info)
        {
            dragContainerControl.ClearControls();

            dragContainerControl.RestoreState(info);

            //dragContainerControl.ClearControls();

            if (info.ContainsValue("combinedContainerControl.ShowComponentsTitles"))
            {
                toolStripButtonTitlesText.Checked = info.GetBoolean("combinedContainerControl.ShowComponentsTitles");
            }

            // Establish the CheckedControlContainer
            if (info.ContainsValue("combinedContainerControl.CheckedControlContainerGuid"))
            {
                Guid guid = info.GetValue<Guid>("combinedContainerControl.CheckedControlContainerGuid");
                _checkedControlContainer = dragContainerControl.GetDragControlByGuid(guid);
                if (_checkedControlContainer != null)
                {
                    _checkedControlContainer.ShowDragStrip = false;
                }
            }
        }

        /// <summary>
        /// Call this to persist state data to controls. Controls contain their own settings (partial).
        /// </summary>
        public void SaveState(SerializationInfoEx info)
        {
            // Remove the checked control from drag controls, since it does not need to be stored.
            ChangeCheckedControl(null);

            // Clean of any leftover controls before persisting.
            foreach (DragControl control in GeneralHelper.EnumerableToArray<DragControl>(dragContainerControl.DragControls))
            {
                if (control.ControlContained == null && control != _checkedControlContainer)
                {
                    dragContainerControl.RemoveDragControl(control);
                }
            }

            // Also lets store the _checkedControlContainer
            if (_checkedControlContainer != null)
            {
                info.AddValue("combinedContainerControl.CheckedControlContainerGuid", _checkedControlContainer.Guid);
            }

            info.AddValue("combinedContainerControl.ShowComponentsTitles", toolStripButtonTitlesText.Checked);

            dragContainerControl.SaveState(info);

            foreach (ToolStripButton component in _tabbedButtonsControls.Keys)
            {
                PersistControlData(_tabbedButtonsControls[component], component);
            }

            foreach (CombinedHostingForm form in _floatingFormsControls.Keys)
            {
                PersistControlData(_floatingFormsControls[form], form);
            }

            foreach (DragControl dragControl in dragContainerControl.DragControls)
            {
                if (dragControl.ControlContained != null)
                {
                    PersistControlData((CommonBaseControl)dragControl.ControlContained, dragControl);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        Image GetControlImage(CommonBaseControl control)
        {
            if (_imageList == null || _imageList.Images.ContainsKey(control.ImageName) == false)
            {
                return null;
            }
            else
            {
                return _imageList.Images[control.ImageName];
            }
        }

        /// <summary>
        /// Main UI update baseMethod.
        /// </summary>
        void UpdateUI()
        {
            //this.toolStripLabelTitle.Text = "Components [Tabbed:" + _tabbedButtonsControls.Count.ToString() + " Docked:" + (dragContainerControl.DragControls.Count - 1).ToString() + " Floating:" + _floatingFormsControls.Count + "]";

            foreach (ToolStripButton button in _tabbedButtonsControls.Keys)
            {
                button.Checked = _tabbedButtonsControls.GetByKey(button) == _checkedControl;

                if (toolStripButtonTitlesText.Checked || button.Checked)
                {
                    button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                }
                else
                {
                    button.DisplayStyle = ToolStripItemDisplayStyle.Image;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommonBaseControl GetControlByTag(object tag)
        {
            foreach (CommonBaseControl control in _tabbedButtonsControls.Values)
            {
                if (control.Tag == tag)
                {
                    return control;
                }
            }

            foreach (CommonBaseControl control in _floatingFormsControls.Values)
            {
                if (control.Tag == tag)
                {
                    return control;
                }
            }

            foreach (DragControl control in dragContainerControl.DragControls)
            {
                if (control.ControlContained != null && control.ControlContained.Tag == tag)
                {
                    return (CommonBaseControl)control.ControlContained;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ChangeCheckedControl(CommonBaseControl control)
        {
            if (_checkedControl == control)
            {
                return;
            }

            if (_checkedControlContainer == null)
            {// Creating a new checked container.
                _checkedControlContainer = new DragControl();
                _checkedControlContainer.ShowDragStrip = false;
                dragContainerControl.AddDragControl(_checkedControlContainer);
                dragContainerControl.DockControl(_checkedControlContainer, DockStyle.Fill);
            }
            
            if (control == null)
            {
                _checkedControlContainer.ControlContained = null;
            }
            else
            {
                _checkedControlContainer.ControlContained = control;
                _checkedControlContainer.Name = control.Name;
            }

            if (_checkedControl != null && _checkedControl.PersistenceData != null && control != null)
            {
                _checkedControl.PersistenceData.AddValue("combinedContainer.Checked", false);
            }

            _checkedControl = control;

            if (_checkedControl != null && _checkedControl.PersistenceData != null)
            {
                _checkedControl.PersistenceData.AddValue("combinedContainer.Checked", true);
            }

            if (SelectedTabbedControlChangedEvent != null)
            {
                SelectedTabbedControlChangedEvent(this, _checkedControl);
            }

            UpdateUI();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearControls()
        {
            _checkedControl = null;
            _floatingFormsControls.Clear();
            _tabbedButtonsControls.Clear();

            dragContainerControl.ClearControls();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemoveControl(CommonBaseControl control)
        {
            // We shall not invoke control.UnInitializeControl(); here, since user may ultimately wish to use the control
            // further in another context.

            DoRemoveControl(control);

            if (ContainedControlRemovedEvent != null)
            {
                ContainedControlRemovedEvent(this, control);
            }

            // Raise control removed event.
            return true;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        void SetControlToDragControl(CommonBaseControl control, DragControl dragControl, Point location)
        {
            SystemMonitor.CheckError(dragControl.ControlContained == null, "Drag control already has a contained control.");

            dragControl.ControlContained = control;
            dragControl.Name = control.Name;
            dragControl.Location = location;

            dragControl.Image = GetControlImage(control);
            dragControl.dragStrip.DoubleClick += new EventHandler(dragStrip_DoubleClick);

            UpdateUI();
        }

        /// <summary>
        /// Add a floating control to the workspace area.
        /// location considered only for floating (dockStyle = none)
        /// </summary>
        public DragControl AddWorkspaceControl(CommonBaseControl control, DockStyle dockPane, Point location)
        {
            if (dragContainerControl.GetDragControlFromContainedControl(control) != null)
            {
                SystemMonitor.Warning("Misuse.");
                return null;
            }

            DragControl dragControl = new DragControl();
            SetControlToDragControl(control, dragControl, location);

            dragContainerControl.AddDragControl(dragControl);
            dragContainerControl.DockControl(dragControl, dockPane);

            UpdateUI();

            return dragControl;
        }

        void dragStrip_DoubleClick(object sender, EventArgs e)
        {
            CommonBaseControl control = ((DragStripControl)sender).ParentControl.ControlContained as CommonBaseControl; 
            ((DragStripControl)sender).ParentControl.ControlContained = null;
            dragContainerControl.RemoveDragControl(((DragStripControl)sender).ParentControl);
            SetControlFloating(control);
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddControl(CommonBaseControl control)
        {
            Point? location = null;
            Size? size = null;

            if (control.PersistenceData != null && control.PersistenceData.ContainsValue("combinedContainer.Size"))
            {
                size = control.PersistenceData.GetValue<Size>("combinedContainer.Size");
            }

            if (control.PersistenceData != null && control.PersistenceData.ContainsValue("combinedContainer.Location"))
            {
                location = control.PersistenceData.GetValue<Point>("combinedContainer.Location");
            }

            if (control.PersistenceData != null && control.PersistenceData.ContainsValue("combinedContainer.Floating"))
            {
                CombinedHostingForm form = SetControlFloating(control);
                if (location.HasValue)
                {
                    form.Location = location.Value;
                }

                // Sometimes persistence may be wrong, so make sure to bring control to view when added.
                form.Left = Math.Max(form.Left, 0);
                form.Top = Math.Max(form.Top, 0);

                form.Left = Math.Min(Screen.PrimaryScreen.WorkingArea.Width, form.Left);
                form.Top = Math.Min(Screen.PrimaryScreen.WorkingArea.Height, form.Top);

                form.Width = Math.Max(form.Width, 200);
                form.Height = Math.Max(form.Height, 150);

                if (size.HasValue)
                {
                    form.Size = size.Value;
                }

            }
            else if (control.PersistenceData != null && control.PersistenceData.ContainsValue("combinedContainer.Tabbed"))
            {
                AddTabbedControl(control);
            }
            else if (control.PersistenceData != null && control.PersistenceData.ContainsValue("combinedContainer.Docked")
                && control.PersistenceData.ContainsValue("combinedContainer.Guid"))
            {
                Guid guid = control.PersistenceData.GetValue<Guid>("combinedContainer.Guid");
                DragControl dragControl = dragContainerControl.GetDragControlByGuid(guid);
                if (dragControl == null)
                {
                    SystemMonitor.OperationError("Guid drag control not found. Using a default new one.");
                    AddTabbedControl(control);
                }
                else
                {// Reuse the existing drag control and place the new control inside it.
                    SetControlToDragControl(control, dragControl, Point.Empty);
                    dragControl.Visible = true;
                }
            }
            else
            {// By default add tabbed.
                AddTabbedControl(control);
            }
        }

        //void ParentForm_Activated(object sender, EventArgs e)
        //{
        //    if (this.Visible == false)
        //    {
        //        return;
        //    }

        //    if (SelectedTabbedControlChangedEvent != null && _checkedControl != null)
        //    {
        //        SelectedTabbedControlChangedEvent(this, _checkedControl);
        //    }
        //}

        void combinedHostingForm_Activated(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                return;
            }

            if (sender is CombinedHostingForm && SelectedTabbedControlChangedEvent != null)
            {
                SelectedTabbedControlChangedEvent(this, _floatingFormsControls[sender as CombinedHostingForm]);
            }

        }

        void PersistControlData(CommonBaseControl control, Component containingComponent)
        {
            control.SaveState();

            Point? location = null;
            Size? size = null;

            if (containingComponent == null)
            {
                return;
            }

            // Since this info will be cleared on next line, store here for reference.
            bool isChecked = control.PersistenceData.ContainsValue("combinedContainer.Checked") &&
                control.PersistenceData.GetBoolean("combinedContainer.Checked");

            // Clear all previous persistence info related to "combinedContainer".
            control.PersistenceData.ClearByNamePart("combinedContainer.");

            if (containingComponent is ToolStripButton || 
                control == _checkedControl)
            {
                control.PersistenceData.AddValue("combinedContainer.Tabbed", true);
                control.PersistenceData.AddValue("combinedContainer.Checked", isChecked);
                control.PersistenceData.AddValue("combinedContainer.TabIndex", toolStripMain.Items.IndexOf((ToolStripButton)containingComponent));
            }
            else if (containingComponent is CombinedHostingForm)
            {
                location = ((CombinedHostingForm)containingComponent).Location;
                size = ((CombinedHostingForm)containingComponent).Size;
                control.PersistenceData.AddValue("combinedContainer.Floating", true);
            }
            else if (containingComponent is DragControl)
            {
                control.PersistenceData.AddValue("combinedContainer.Docked", true);
                control.PersistenceData.AddValue("combinedContainer.Guid", ((DragControl)containingComponent).Guid);
            }
            else
            {
                SystemMonitor.Warning("Unrecognized case.");
                return;
            }

            if (location.HasValue)
            {
                control.PersistenceData.AddValue("combinedContainer.Location", location.Value);
            }

            if (size.HasValue)
            {
                control.PersistenceData.AddValue("combinedContainer.Size", size.Value);
            }

        }

        /// <summary>
        /// Add a control to the tabbed controls list.
        /// </summary>
        public void AddTabbedControl(CommonBaseControl control)
        {
            if (_tabbedButtonsControls.ContainsValue(control))
            {
                SystemMonitor.Error("Control already added.");
                return;
            }

            #region Establish Index
            // Since the components enter one by one, it will happen that the index of the incoming control
            // is beyond the current maximum, so we look for the existing controls, one by one, and see where
            // the current one fits - this way they are restored places one by one and everyone is properly placed.

            int? index = null;
            if (control.PersistenceData.ContainsValue("combinedContainer.TabIndex"))
            {
                index = control.PersistenceData.GetValue<int>("combinedContainer.TabIndex");
            }
            
            int? actualIndex = null;
            if (index.HasValue)
            {
                foreach (ToolStripItem item in toolStripMain.Items)
                {
                    int itemIndex = 0;
                    if (item is ToolStripButton)
                    {
                        if (_tabbedButtonsControls.ContainsKey(item as ToolStripButton))
                        {
                            CommonBaseControl itemControl = _tabbedButtonsControls[item as ToolStripButton];
                            if (itemControl != null && itemControl.PersistenceData.ContainsValue("combinedContainer.TabIndex"))
                            {
                                itemIndex = itemControl.PersistenceData.GetValue<int>("combinedContainer.TabIndex");
                            }
                        }
                    }

                    if (index.Value <= itemIndex)
                    {
                        actualIndex = toolStripMain.Items.IndexOf(item);
                        break;
                    }
                }
            }
            #endregion

            string nameRepaired = control.Name.Replace("&", "&&");

            ToolStripButton controlButton = new ToolStripButton(nameRepaired);
            Image image = GetControlImage(control);
            if (image == null)
            {
                image = Resources.dot;
            }

            controlButton.ToolTipText = control.Text;
            controlButton.Image = image;
            controlButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            controlButton.CheckOnClick = false;
            controlButton.Checked = false;
            controlButton.MouseDown += new MouseEventHandler(controlButton_MouseDown);
            controlButton.MouseUp += new MouseEventHandler(controlButton_MouseUp);
            controlButton.MouseMove += new MouseEventHandler(controlButton_MouseMove);

            if (actualIndex.HasValue)
            {
                toolStripMain.Items.Insert(actualIndex.Value, controlButton);
            }
            else
            {
                toolStripMain.Items.Add(controlButton);
            }

            _tabbedButtonsControls.Add(controlButton, control);

            bool isChecked = false;
            if (control.PersistenceData != null && control.PersistenceData.ContainsValue("combinedContainer.Checked"))
            {
                isChecked = control.PersistenceData.GetBoolean("combinedContainer.Checked");
            }

            if (_checkedControl == null || isChecked)
            {
                ChangeCheckedControl(control);
            }

            UpdateUI();
        }

        void controlButton_MouseUp(object sender, MouseEventArgs e)
        {
            _lastMouseDown = null;
            ChangeCheckedControl(_tabbedButtonsControls.GetByKey(sender as ToolStripButton));
        }

        void controlButton_MouseMove(object sender, MouseEventArgs e)
        {
            ToolStripButton item = (ToolStripButton)sender;
            if (_lastMouseDown.HasValue && 
                (Math.Abs(_lastMouseDown.Value.X - e.Location.X) > 5 || Math.Abs(_lastMouseDown.Value.Y - e.Location.Y) > 5)
                && e.Button == MouseButtons.Left && _tabbedButtonsControls.ContainsKey(item))
            {
                _lastMouseDown = null;
                //dragContainerControl.DoBeginDrag(_tabbedButtonsControls.GetByKey(item));
                this.DoDragDrop(item, DragDropEffects.Move);
            }

        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
        }


        /// <summary>
        /// 
        /// </summary>
        void controlButton_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripButton item = (ToolStripButton)sender;

            if (e.Button == MouseButtons.Right)
            {
                ChangeCheckedControl(_tabbedButtonsControls.GetByKey(item));
                contextMenuStrip1.Show(System.Windows.Forms.Cursor.Position);
            }
            else
            {
                _lastMouseDown = e.Location;
            }

            UpdateUI();
        }

        /// <summary>
        /// Actual remove the control from managed controls.
        /// </summary>
        void DoRemoveControl(CommonBaseControl control)
        {
            if (control == _checkedControl)
            {
                // Try to change the current checked control.
                foreach (ToolStripButton button in _tabbedButtonsControls.Keys)
                {
                    if (_tabbedButtonsControls.GetByKey(button) != control)
                    {
                        ChangeCheckedControl(_tabbedButtonsControls.GetByKey(button));
                        break;
                    }
                }

                if (control == _checkedControl)
                {// If we failed to change it to another one, change to null.
                    ChangeCheckedControl(null);
                }
            }


            if (_tabbedButtonsControls.ContainsValue(control))
            {
                toolStripMain.Items.Remove(_tabbedButtonsControls.GetByValue(control));
                _tabbedButtonsControls.RemoveByValue(control);
            }

            if (dragContainerControl.GetDragControlFromContainedControl(control) != null)
            {
                dragContainerControl.RemoveDragControl(dragContainerControl.GetDragControlFromContainedControl(control));
            }

            if (_floatingFormsControls.ContainsValue(control))
            {
                _floatingFormsControls.GetByValue(control).Close();
                _floatingFormsControls.RemoveByValue(control);
            }

            UpdateUI();
        }

        private void toolStripButtonClose_MouseEnter(object sender, EventArgs e)
        {
            toolStripButtonClose.Image = Resources.button_cancel_12_b;
        }

        private void toolStripButtonClose_MouseLeave(object sender, EventArgs e)
        {
            toolStripButtonClose.Image = Resources.button_cancel_12;
        }

        /// <summary>
        /// Resets the user interface placement of controls, by sending them all to the tab control.
        /// </summary>
        public void ResetControlPlacement()
        {
            foreach (CombinedHostingForm form in GeneralHelper.EnumerableToList<CombinedHostingForm>(_floatingFormsControls.Keys))
            {
                CommonBaseControl control = form.ContainedControl;
                control.Parent = null;

                // This makes sure no ask dialogs are shown or closing the forms.
                form.ContainedControl = null;
                
                DoRemoveControl(control);

                AddTabbedControl(control);
            }

            SystemMonitor.CheckError(_floatingFormsControls.Count == 0, "Not all floating forms were replaced with tabbed controls.");

            foreach (DragControl drag in GeneralHelper.EnumerableToList<DragControl>(dragContainerControl.DragControls))
            {
                if (drag == _checkedControlContainer)
                {
                    continue;
                }

                CommonBaseControl control = (CommonBaseControl)drag.ControlContained;
                control.Parent = null;

                // This makes sure no ask dialogs are shown or closing the forms.
                drag.ControlContained = null;

                dragContainerControl.RemoveDragControl(drag);

                AddTabbedControl(control);
            }

            //SystemMonitor.CheckError(dragContainerControl.DragControls.Count == 1, "Not all docked forms were replaced with tabbed controls.");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="location">Location to float to, in screen coordinates.</param>
        /// <param name="newWindow"></param>
        public CombinedHostingForm SetControlFloating(CommonBaseControl control)
        {
            DoRemoveControl(control);

            CombinedHostingForm form = new CombinedHostingForm(control);
            _floatingFormsControls.Add(form, control);

            form.Location = Cursor.Position;
            form.ResizeBegin += new EventHandler(combinedHostingForm_ResizeBegin);
            form.LocationChanged += new EventHandler(combinedHostingForm_LocationChanged);
            //form.MdiParent = this.ParentForm;
            form.ResizeEnd += new EventHandler(combinedHostingForm_ResizeEnd);
            form.FormClosing += new FormClosingEventHandler(combinedHostingForm_FormClosing);
            form.Activated += new EventHandler(combinedHostingForm_Activated);

            form.ShowInTaskbar = true;
            form.Show(this.ParentForm);
            form.BringToFront();

            UpdateUI();

            return form;
        }

        /// <summary>
        /// Removes the currently selected checked control, if one is present.
        /// </summary>
        public void RemoveCurrentCheckedControl()
        {
            if (_checkedControl != null)
            {
                RemoveControl(_checkedControl);
            }
        }

        void combinedHostingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CombinedHostingForm form = sender as CombinedHostingForm;
            if (form.ContainedControl == null)
            {
                _floatingFormsControls.RemoveByKey(form);
                return;
            }

            // Unsubscribe events.
            form.ResizeBegin -= new EventHandler(combinedHostingForm_ResizeBegin);
            form.ResizeEnd -= new EventHandler(combinedHostingForm_ResizeEnd);

            form.FormClosing -= new FormClosingEventHandler(combinedHostingForm_FormClosing);
            form.Activated -= new EventHandler(combinedHostingForm_Activated);

            if (e.CloseReason != CloseReason.UserClosing)
            {// We are doing a major application close, do not ask anything.
                // Persist data form this form since it is being closed and will no longer have it properly assigned.
                PersistControlData(form.ContainedControl, form);
                CommonBaseControl control = form.ContainedControl;
                form.ContainedControl = null;
                
                // Clear from list of internal controls.
                DoRemoveControl(control);

                return;
            }


            DialogResult result = MessageBox.Show("Remove component?" + Environment.NewLine + "+ Click [Yes] to Remove" + Environment.NewLine + "- Click [No] to Send to Tabbed", "Open Forex Platform", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {// Remove.
                CommonBaseControl containedControl = form.ContainedControl;
                form.ContainedControl = null;
                RemoveControl(containedControl);
            }
            else if (result == DialogResult.No)
            {// Send to tabbed.
                _floatingFormsControls.RemoveByKey(form);
                form.Controls.Clear();
                AddTabbedControl(form.ContainedControl);
            }
            else if (result == DialogResult.Cancel)
            {// Keep it unchanged.
                e.Cancel = true;
            }

        }

        void combinedHostingForm_LocationChanged(object sender, EventArgs e)
        {
            dragContainerControl.DoDragOver(Cursor.Position, null);
        }

        void combinedHostingForm_ResizeBegin(object sender, EventArgs e)
        {
            foreach (CombinedHostingForm hostingForm in _floatingFormsControls.Keys)
            {
                //hostingForm.ContainedControl.Visible = false;
                hostingForm.Opacity = 1;
            }

            dragContainerControl.DoBeginDrag(((CombinedHostingForm)sender).ContainedControl);
        }

        void combinedHostingForm_ResizeEnd(object sender, EventArgs e)
        {
            CombinedHostingForm form = (CombinedHostingForm)sender;

            foreach (CombinedHostingForm hostingForm in _floatingFormsControls.Keys)
            {
                //hostingForm.ContainedControl.Visible = true;
                hostingForm.Opacity = 1;
            }

            DockStyle? dockStyle = dragContainerControl.DoEndDrag(form.ContainedControl, Cursor.Position);
            if (dockStyle.HasValue)
            {
                AddWorkspaceControl(form.ContainedControl, dockStyle.Value, Point.Empty);
                form.ContainedControl = null;
                form.Close();
            }
        }


        private void dragContainerControl_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ToolStripButton)))
            {
                ToolStripButton button = (ToolStripButton)e.Data.GetData(typeof(ToolStripButton));
                button.Enabled = false;
             
                CommonBaseControl control = _tabbedButtonsControls.GetByKey(button);

                toolStripMain.Items.Remove(_dragSeparator);

                DockStyle? dockStyle = dragContainerControl.DoEndDrag(control, new Point(e.X, e.Y));
                if (dockStyle == null)
                {
                    SetControlFloating(control);
                }
                else
                {
                    DoRemoveControl(control);
                    AddWorkspaceControl(control, dockStyle.Value, Point.Empty);
                }
            }
        }

        private void dragContainerControl_DragControlRemoved(DragContainerControl container, DragControl dragControl)
        {
            if (dragControl.ControlContained == _checkedControl)
            {
                ChangeCheckedControl(null);
                return;
            }

            if (dragControl.ControlContained == null)
            {
                return;
            }

            DialogResult result = MessageBox.Show("Remove component?" + Environment.NewLine + "- Click [Yes] to Remove" + Environment.NewLine + "- Click [No] to Send to Tabbed", "Open Forex Platform", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {// Remove.
                CommonBaseControl control = (CommonBaseControl)dragControl.ControlContained;
                dragControl.ControlContained = null;
                RemoveControl(control);
            }
            else if (result == DialogResult.No)
            {// Send to tabbed.
                DoRemoveControl((CommonBaseControl)dragControl.ControlContained);
                AddTabbedControl((CommonBaseControl)dragControl.ControlContained);
            }
        }

        private void toolStripMain_DragEnter(object sender, DragEventArgs e)
        {
            base.OnDragEnter(e);

            // Confirm draggable operation.
            e.Effect = DragDropEffects.All;
        }

        private void toolStripButtonTitlesText_Click(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void toolStripButtonClose_Click(object sender, EventArgs e)
        {
            RemoveCurrentCheckedControl();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonClose_Click(sender, e);
        }

        private void floatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_checkedControl != null)
            {
                SetControlFloating(_checkedControl);
            }
        }

        private void toolStripMain_DragDrop(object sender, DragEventArgs e)
        {// Dropped an item on tool strip, maybe we need to move it.

            //Point location = toolStripMain.PointToClient(new Point(e.X, e.Y));
            //ToolStripItem item = toolStripMain.GetItemAt(location);
            //int index = toolStripMain.Items.IndexOf(item);

            //if (item != null && index >= 0 && e.Data.GetDataPresent(typeof(ToolStripButton)))
            //{
            //    ToolStripButton button = (ToolStripButton)e.Data.GetData(typeof(ToolStripButton));
            //    toolStripMain.Items.Remove(button);
            //    toolStripMain.Items.Insert(index, button);
            //}

            //toolStripMain.Items.Remove(_dragSeparator);
            
            // This call not needed (?!)
            dragContainerControl.DoEndDrag(null, new Point());
        }

        private void toolStripMain_DragOver(object sender, DragEventArgs e)
        {
            ToolStripButton button = (ToolStripButton)e.Data.GetData(typeof(ToolStripButton));
            ToolStripItem draggableItem = button;

            Point location = toolStripMain.PointToClient(new Point(e.X, e.Y));
            ToolStripItem item = toolStripMain.GetItemAt(location);
            int index = toolStripMain.Items.IndexOf(item);

            if (item == draggableItem || item is ToolStripButton == false
                || _tabbedButtonsControls.ContainsKey(item as ToolStripButton) == false)
            {
                return;
            }

            if (item != null && index >= 0 && e.Data.GetDataPresent(typeof(ToolStripButton)))
            {
                //toolStripMain.Items.Remove(draggableItem);
                //toolStripMain.Items.Insert(index, draggableItem);

                bool isBeforeItem = location.X - item.Bounds.X <= item.ContentRectangle.Width / 2;

                if (toolStripMain.Items.Contains(draggableItem) == false)
                {
                    if (isBeforeItem == false)
                    {
                        index++;
                    }

                    //toolStripMain.Items.Remove(draggableItem);
                    toolStripMain.Items.Insert(index, draggableItem);
                }
                else
                {
                    int draggableItemIndex = toolStripMain.Items.IndexOf(draggableItem);

                    if (isBeforeItem)
                    {
                        if (draggableItemIndex == index - 1)
                        {
                            return;
                        }

                        // *Do not* move this line of code since its dependent with above and below ones.
                        toolStripMain.Items.Remove(draggableItem);
                        
                        toolStripMain.Items.Insert(index, draggableItem);
                    }
                    else
                    {
                        if (draggableItemIndex == index + 1)
                        {
                            return;
                        }
                        
                        // *Do not* move this line of code since its dependent with above and below ones.
                        toolStripMain.Items.Remove(draggableItem);

                        if (index >= toolStripMain.Items.Count)
                        {
                            toolStripMain.Items.Add(draggableItem);
                        }
                        else
                        {// Here index is not + 1, since we have removed the draggable item and these indexes are now -1.
                            toolStripMain.Items.Insert(index, draggableItem);
                        }
                    }
                }
            }

            //Point location = PointToClient(new Point(e.X, e.Y));
            //ToolStripItem item = toolStripMain.GetItemAt(location);
            //int index = toolStripMain.Items.IndexOf(item);

            //if (item != null && index >= 0 && e.Data.GetDataPresent(typeof(ToolStripButton)))
            //{
            //    ToolStripButton button = (ToolStripButton)e.Data.GetData(typeof(ToolStripButton));
            //    if (item != button && toolStripMain.Items.IndexOf(button) != index)
            //    {
            //        toolStripMain.Items.Remove(button);
            //        toolStripMain.Items.Insert(index, button);
            //    }
            //}

            //toolStripMain.Items.Remove(_dragSeparator);
            //dragContainerControl.DoEndDrag(null, new Point());

        }

    }
}
