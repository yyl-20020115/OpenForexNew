using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class ProfilerControl : UserControl
    {
        public ProfilerControl()
        {
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
#if DEBUG
            if (Profiler.Entries != null)
            {
                this.listBoxMain.Items.Clear();
                string[] names = Enum.GetNames(typeof(Profiler.ProfilerEntry));
                for (int i = 0; i < Profiler.Entries.Length; i++)
                {
                    string message = names[i] + ": entries [" + Profiler.Entries[i] + "] time ["+Profiler.Times[i]/10000+"]";
                    this.listBoxMain.Items.Add(message);
                }
            }
#endif
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (Profiler.Entries != null)
            {
                for (int i = 0; i < Profiler.Entries.Length; i++)
                {
                    Profiler.Entries[i] = 0;
                    Profiler.Times[i] = 0;
                }
            }
#endif

        }


    }
}
