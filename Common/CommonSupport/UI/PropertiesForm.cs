using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class PropertiesForm : Form
    {
        public PropertyGrid PropertyGrid
        {
            get { return this.propertyGrid; }
        }

        public bool ShowOkCancel
        {
            get { return this.panelResultButtons.Visible; }
            set { this.panelResultButtons.Visible = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertiesForm(string title)
        {
            InitializeComponent();

            this.Text = title;
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertiesForm(string title, object selectedObject)
        {
            InitializeComponent();

            this.Text = title;
            this.propertyGrid.SelectedObject = selectedObject;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            this.Focus();
        }

        public new DialogResult ShowDialog()
        {
            this.panelResultButtons.Visible = true;
            return base.ShowDialog();
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            this.panelResultButtons.Visible = true;
            return base.ShowDialog(owner);
        }

    }
}
