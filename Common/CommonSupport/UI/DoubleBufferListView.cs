using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport.UI
{
    /// <summary>
    /// Extends default list view to allow double buffering.
    /// </summary>
    public class DoubleBufferListView : ListView
    {
        public DoubleBufferListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
