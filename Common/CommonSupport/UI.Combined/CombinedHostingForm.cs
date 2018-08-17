using System;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CombinedHostingForm : Form
    {
        CommonBaseControl _control;

        public CommonBaseControl ContainedControl
        {
            get { return _control; }
            set { _control = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public CombinedHostingForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public CombinedHostingForm(CommonBaseControl control)
        {
            InitializeComponent();

            this.Text = Application.ProductName + " - " + control.Name;
            
            this.Controls.Add(control);
            _control = control;
            
            //this.TopLevel = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _control.Dock = DockStyle.Fill;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        private void CombinedHostingForm_ResizeBegin(object sender, EventArgs e)
        {
            //Opacity = 0.4;
        }

        private void CombinedHostingForm_ResizeEnd(object sender, EventArgs e)
        {
            //Opacity = 1.0;
        }

    }
}
