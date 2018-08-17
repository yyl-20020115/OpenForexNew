using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// Class serves as basis for implementing keyboard shortcut handling operations.
    /// Also the class name ControlEventHandler is part of the .NET framework, so keep the automated part.
    /// </summary>
    public abstract class AutomatedControlEventHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public AutomatedControlEventHandler()
        {
        }

        public abstract void KeyUp(Control control, KeyEventArgs e);
        public abstract void KeyDown(Control control, KeyEventArgs e);
        public abstract void KeyPress(Control control, KeyPressEventArgs e);
    }
}
