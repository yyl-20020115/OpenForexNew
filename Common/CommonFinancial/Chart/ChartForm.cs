using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonFinancial
{
    /// <summary>
    /// Form hosts an integrated chart control.
    /// </summary>
    public partial class ChartForm : Form
    {
        public ChartControl Chart
        {
            get { return chartControl; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        public ChartForm(string title)
        {
            InitializeComponent();
            this.Text = title;
        }
    }
}
