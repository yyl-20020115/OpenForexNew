using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// Control used to hide underlying controls on a parent control, when an operation that stops access to them is needed.
    /// </summary>
    public partial class WaitControl : UserControl
    {
        /// <summary>
        /// Invocation safety.
        /// </summary>
        public string Message
        {
            get { return this.labelMain.Text; }
            set 
            {
                if (this.InvokeRequired == false)
                {
                    labelMain.Text = value;
                }
                else
                {
                    WinFormsHelper.BeginManagedInvoke(this, delegate() { labelMain.Text = value; });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Active
        {
            get
            {
                return this.Visible;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public WaitControl()
        {
            InitializeComponent();
            this.Visible = false;
        }

        /// <summary>
        /// Invocation safety.
        /// </summary>
        /// <param name="activate"></param>
        public void SetActiveState(bool activate)
        {
            if (this.InvokeRequired == false)
            {
                if (this.IsHandleCreated == false)
                {
                    this.CreateControl();
                }

                this.Visible = true;
                DoActivate(activate);
            }
            else
            {
                WinFormsHelper.BeginManagedInvoke(this, delegate()
                {
                    if (this.IsHandleCreated == false)
                    {
                        this.CreateControl();
                    }

                    DoActivate(activate);
                });
            }
        }

        /// <summary>
        /// Helper.
        /// </summary>
        /// <param name="activate"></param>
        void DoActivate(bool activate)
        {
            if (activate)
            {
                this.Left = 0;
                this.Top = 0;
                this.Width = Parent.Width;
                this.Height = Parent.Height;
                this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                this.BringToFront();
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }

        private void timerUI_Tick(object sender, EventArgs e)
        {
            pictureBox1.Visible = !pictureBox1.Visible;
        }

        private void labelMain_VisibleChanged(object sender, EventArgs e)
        {
            timerUI.Enabled = this.Visible;
        }
    }
}
