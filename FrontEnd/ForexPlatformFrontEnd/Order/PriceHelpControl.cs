using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ForexPlatformFrontEnd
{
    public partial class PriceHelpControl : UserControl
    {
        NumericUpDown _sourceControl;

        public int[] PredefinedValueLevels = new int[] { 10, 25, 40, 50, 75, 100, 125, 150, 200, 250, 500, 1000 };

        /// <summary>
        /// Designer needed constructor.
        /// </summary>
        public PriceHelpControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Actual constructor.
        /// </summary>
        /// <param name="sourceControl"></param>
        public PriceHelpControl(NumericUpDown sourceControl)
        {
            InitializeComponent();
            _sourceControl = sourceControl;
        }

        private void PriceHelpControl_Leave(object sender, EventArgs e)
        {
            buttonX_Click(sender, e);
        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                this.Parent.Controls.Remove(this);
            }
        }

        /// <summary>
        /// Show the control at a given location.
        /// </summary>
        /// <param name="referenceControl">The control to use as location reference, must have parent to add this to.</param>
        public static void ShowHelperControlAt(Point location, Control parentControl, NumericUpDown sourceNumericControl)
        {
            if (sourceNumericControl.Enabled == false)
            {
                return;
            }

            PriceHelpControl control = new PriceHelpControl(sourceNumericControl);
            parentControl.Controls.Add(control);
            control.Location = location;
            control.BringToFront();
            control.Focus();
        }

        private void PriceHelpControl_Load(object sender, EventArgs e)
        {
            listViewPointsAbove.Items.Clear();
            listViewPointsBelow.Items.Clear();
            foreach (int value in PredefinedValueLevels)
            {
                ListViewItem item = new ListViewItem(new string[] { "", value.ToString() });
                item.ForeColor = Color.Green;
                item.Tag = value;
                listViewPointsAbove.Items.Add(item);

                item = new ListViewItem(new string[] { "", value.ToString() });
                item.ForeColor = Color.Red;
                item.Tag = value;
                listViewPointsBelow.Items.Add(item);
            }
        }

        //decimal GetPoints()
        //{
        //    string text = "0";//listViewPoints.SelectedItems
        //        //listBoxPoints.SelectedItem.ToString();
        //    text = text.Replace("points", "");
        //    decimal val = Decimal.Parse(text);
        //    val = val * (decimal)Math.Pow(0.1, _sourceControl.DecimalPlaces);
        //    return val;
        //}

        private void listViewPointsAbove_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewPointsAbove.SelectedItems.Count > 0)
            {
                decimal value = (int)listViewPointsAbove.SelectedItems[0].Tag * (decimal)Math.Pow(0.1, _sourceControl.DecimalPlaces);
                value = Math.Min(value, _sourceControl.Maximum);
                _sourceControl.Value += value;
                buttonX_Click(sender, e);
            }
        }

        private void listViewPointsBelow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewPointsBelow.SelectedItems.Count > 0)
            {
                decimal value = (int)listViewPointsBelow.SelectedItems[0].Tag * (decimal)Math.Pow(0.1, _sourceControl.DecimalPlaces);
                value = Math.Max(_sourceControl.Minimum, value);
                _sourceControl.Value -= value;
                buttonX_Click(sender, e);
            }
        }

    }
}
