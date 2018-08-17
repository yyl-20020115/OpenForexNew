using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public class HelpHandler : AutomatedControlEventHandler
    {
        public override void KeyUp(Control control, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                MessageBox.Show("Help for control [" + control.Name + "]");
            }
        }

        public override void KeyDown(Control control, KeyEventArgs e)
        {
        }

        public override void KeyPress(Control control, KeyPressEventArgs e)
        {
        }
    }
}
