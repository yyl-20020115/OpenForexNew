using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport.UI
{
    public partial class ManagedVirtualListView : UserControl
    {
        public int VirtualListSize
        {
            get
            {
                return this.vScrollBar.Maximum;
            }

            set
            {
                this.vScrollBar.Maximum = value;
                //base.VirtualListSize = value;
                //UpdateColumnWidths();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ManagedVirtualListView()
        {
            InitializeComponent();
        }
    }
}
