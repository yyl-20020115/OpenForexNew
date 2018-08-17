using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using CommonSupport;
using Rss;
using System.Web;

namespace CommonSupport
{
    /// <summary>
    /// Control visualizes news items information.
    /// </summary>
    public partial class NewsItemControl : UserControl
    {
        static Pen _penNormal = new Pen(SystemColors.ControlDark);
        static Pen _penUnread = new Pen(Color.DarkRed);

        FinancialNewsEvent _newsItem;
        /// <summary>
        /// Item displayed in source.
        /// </summary>
        public FinancialNewsEvent NewsItem
        {
            get 
            {
                return _newsItem; 
            }

            set 
            {
                lock (this)
                {
                    _newsItem = value;
                    if (_newsItem == null)
                    {
                        return;
                    }

                    labelTitle.Text = _newsItem.Title;

                    if (_newsItem is RssNewsEvent)
                    {
                        labelInfo.Text = (labelInfo.Tag as string).Replace("[Date]", _newsItem.DateTime.ToString()).Replace("[Author]", ((RssNewsEvent)_newsItem).Author);
                    }

                    textBoxText.Text = _newsItem.Description;

                    if (_newsItem.Channel != null)
                    {
                        labelInfo.Text += " " + _newsItem.Channel.Source.Name;
                        //labelInfo.Image = _newsItem.Source.GetShortcutIcon();
                    }

                    labelTitle.ForeColor = SystemColors.ControlText;
                    textBoxText.ForeColor = SystemColors.ControlText;

                    if (string.IsNullOrEmpty(textBoxText.Text.Trim()))
                    {
                        //_compactModeHeight = 55;
                        //this.Height = 55;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public NewsItemControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public NewsItemControl(RssNewsEvent newsItem)
        {
            InitializeComponent();
            NewsItem = newsItem;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_newsItem == null)
            {
                return;
            }

            SizeF infoSize = e.Graphics.MeasureString(labelInfo.Text, labelInfo.Font);
            int yLocation = labelInfo.Top + labelInfo.Height / 2;
            //if (_newsItem.IsRead == false)
            //{
                e.Graphics.DrawLine(_penUnread, 5/*25*/, yLocation, this.Width - infoSize.Width - 10, yLocation);
            //}
            //else
            //{
            //    e.Graphics.DrawLine(_penNormal, 25, yLocation, this.Width - infoSize.Width - 10, yLocation);
            //}

            // Shrink the title text to fit.
            SizeF titleSize = e.Graphics.MeasureString(labelTitle.Text, labelTitle.Font);
            while(titleSize.Width >= labelTitle.Width - 15 && labelTitle.Text.Length > 5)
            {// Cut string until it fits.
                labelTitle.Text = labelTitle.Text.Substring(0, labelTitle.Text.Length - 5) + "...";
                titleSize = e.Graphics.MeasureString(labelTitle.Text, labelTitle.Font);
            }
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {// Go to full / compact mode.
            //if (this.Height == _compactModeHeight)
            //{// Go to full mode.
            //    this.Height = 150;
            //    //this.textBoxDescription.BackColor = SystemColors.Window;
            //}
            //else
            //{// Go to compact mode.
            //    this.Height = _compactModeHeight;
            //    //this.textBoxDescription.BackColor = this.BackColor;
            //}

            //this.Parent.Focus();
        }

        public void ShowItemDetails()
        {
            if (_newsItem != null && string.IsNullOrEmpty(_newsItem.Link) == false)
            {
                GeneralHelper.RunUrl(_newsItem.Link);
            }
        }

        private void linkLabelFullStory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowItemDetails();
        }

        private void RSSItemControl_Load(object sender, EventArgs e)
        {// Initialize in compact mode.
            //this.Height = _compactModeHeight;
            this.labelInfo.Text = "";
            this.labelTitle.Text = "";
            //this.comboBoxVote.SelectedIndex = 0;
            //this.textBoxDescription.BackColor = this.BackColor;
        }

        private void RSSItemControl_Resize(object sender, EventArgs e)
        {
            if (_newsItem != null)
            {
                labelTitle.Text = _newsItem.Title;
            }
        }

        private void OnHandleMouseAction(object sender, MouseEventArgs e)
        {
            //this.Parent.Focus();
        }

        private void labelInfo_Click(object sender, EventArgs e)
        {

        }

        //public List<Control> CreateControlsClone(FeedItem item)
        //{
        //    List<Control> result = new List<Control>();

        //    using (NewsItemControl newsControl = new NewsItemControl(item))
        //    {
        //        foreach (Control control in newsControl.Controls)
        //        {
        //            control.Parent = null;
        //            result.Add(control);
        //        }
        //    }

        //    return result;
        //}

    }


}
