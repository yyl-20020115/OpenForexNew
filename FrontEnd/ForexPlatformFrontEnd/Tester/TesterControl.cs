using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    public class TesterControl : CommonBaseControl
    {
        /// <summary>
        /// 
        /// </summary>
        public TesterControl()
        {
            base._persistenceData = new SerializationInfoEx();
        }
    }
}
