using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class HostingForm : Form
    {
        Control _control;
        public Control Control
        {
            get { return _control; }
        }

        public bool ShowCloseButton
        {
            get { return panelButtonClose.Visible; }
            set 
            { 
                panelButtonClose.Visible = value;
            }
        }

        /// <summary>
        /// Needed to prevent a bug with the showing of control, and hiding instantly.
        /// </summary>
        bool _isOperational = false;

        bool _autoAssignOwnerForm = true;
        /// <summary>
        /// Should the main application form be used automatically as owner form for this form.
        /// Assignment happens OnLoad, so make sure to set this before that.
        /// </summary>
        public bool AutoAssignOwnerForm
        {
            get { return _autoAssignOwnerForm; }
            set { _autoAssignOwnerForm = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public HostingForm(string formText)
        {
            InitializeComponent();

            this.Text = formText;
        }

        /// <summary>
        /// 
        /// </summary>
        public HostingForm(string formText, Control control)
        {
            InitializeComponent();

            this.Text = Application.ProductName;
            if (string.IsNullOrEmpty(formText) == false)
            {
                this.Text = this.Text + " - " + formText;
            }

            _control = control;
        }

        private void HostingForm_Load(object sender, EventArgs e)
        {
            if (_control == null)
            {
                return;
            }

            this.Owner = Application.OpenForms[0];

            // Not before that.
            _control.VisibleChanged += new EventHandler(_control_VisibleChanged);

            _control.Visible = true;

            this.SuspendLayout();

            Controls.Add(_control);
            this.Width = _control.Width + 20;

            _control.Left = 0;
            _control.Top = 0;

            this.ResumeLayout();

            int additionalHeight = 30;
            if (panelButtonClose.Visible)
            {
                additionalHeight += panelButtonClose.Height;
            }

            if (VScroll)
            {// Apply maximum height of 85% screen height, or _control heigth, whichever is smaller.
                this.Height = Math.Min(_control.Height + additionalHeight, (int)(Screen.PrimaryScreen.Bounds.Height * 0.85));
            }
            _control.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _control.BringToFront();

            _isOperational = true;
        }


        void _control_VisibleChanged(object sender, EventArgs e)
        {
            if (_isOperational && _control.Visible == false && this.Visible == true)
            {
                this.Close();
            }
        }

        public static HostingForm ShowHostingForm(Control control, string controlName)
        {
            HostingForm form = CreateHostingFormControl(control, controlName);
            form.Show();
            return form;
        }

        /// <summary>
        /// 
        /// </summary>
        public static HostingForm CreateHostingFormControl(Control control, string controlName)
        {
            Size requiredSize = control.Size;
            HostingForm hostingForm = new HostingForm(controlName);
            hostingForm.Controls.Add(control);
            control.Dock = System.Windows.Forms.DockStyle.Fill;
            hostingForm.Size = requiredSize;
            return hostingForm;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            // If shown with Show() dialog result is not considered.
            this.Close();
        }

    }
}