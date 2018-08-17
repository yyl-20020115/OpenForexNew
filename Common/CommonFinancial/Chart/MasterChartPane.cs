using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CommonFinancial
{
    /// <summary>
    /// Chart pane that is the master(leading) for a single chart control; it controls scroll of slave panes 
    /// as well as the main time frame.
    /// </summary>
    public class MasterChartPane : ChartPane
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MasterChartPane()
        {
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

        }

    }
}
