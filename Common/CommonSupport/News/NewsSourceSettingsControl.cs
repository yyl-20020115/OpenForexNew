using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class NewsSourceSettingsControl : UserControl
    {
        EventSource _source;

        /// <summary>
        /// 
        /// </summary>
        public NewsSourceSettingsControl()
        {
            InitializeComponent();
        }

        public NewsSourceSettingsControl(EventSource source)
        {
            InitializeComponent();

            _source = source;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            listViewFeedChannels.Items.Clear();
            foreach (EventSourceChannel channel in _source.Channels)
            {
                ListViewItem item = listViewFeedChannels.Items.Add(channel.Name);
                item.Checked = channel.Enabled;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            List<EventSourceChannel> channels = _source.Channels;
            for (int i = 0; i < channels.Count; i++)
            {
                if (listViewFeedChannels.Items[i].Checked != channels[i].Enabled)
                {
                    channels[i].Enabled = listViewFeedChannels.Items[i].Checked;
                }
            }

            this.Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }


    }
}
