using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonSupport;
using ForexPlatform;

namespace ForexPlatformFrontEnd
{
    public partial class PlatformNewsControl : PlatformComponentControl
    {
        public PlatformNews PlatformNews
        {
            get { return (PlatformNews)base.Component; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformNewsControl() : base(null)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformNewsControl(PlatformNews news)
            : base(news)
        {
            InitializeComponent();
            this.newsManagerControl1.Manager = news.NewsManager;
        }

    }
}
