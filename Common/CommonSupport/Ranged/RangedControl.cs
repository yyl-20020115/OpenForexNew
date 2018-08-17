using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class RangedControl : UserControl
    {
        public int Start
        {
            get { return this.trackBarStart.Value;  }
            set { this.trackBarStart.Value = value; }
        }

        public int End
        {
            get { return this.trackBarEnd.Value; }
            set { this.trackBarEnd.Value = value; }
        }

        public int Count
        {
            get { return this.trackBarEnd.Value - this.trackBarStart.Value;  }
            set { this.trackBarEnd.Value = this.trackBarStart.Value + value;  }
        }

        public int Max
        {
            get { return this.trackBarStart.Maximum; }
            set 
            {
                this.trackBarStart.Maximum = value;
                this.trackBarEnd.Maximum = value;
            }
        }

        IRanged _rangedObject = null;
        public IRanged RangedObject
        {
            get { return _rangedObject; }
            set 
            { 
                _rangedObject = value;
                UIApplyRangedObject(_rangedObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RangedControl()
        {
            InitializeComponent();
        }

        protected void UIApplyRangedObject(IRanged ranged)
        {
            this.trackBarStart.Enabled = (ranged != null);
            this.trackBarEnd.Enabled = (ranged != null);

            if (ranged != null)
            {
                this.trackBarStart.Maximum = ranged.MaxRange;
                this.trackBarEnd.Maximum = ranged.MaxRange;

                this.trackBarStart.Value = ranged.StartIndex;
                this.trackBarEnd.Value = ranged.StartIndex + ranged.Count;
            }
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            this.numericUpDownStart.Value = this.trackBarStart.Value;
            this.numericUpDownEnd.Value = this.trackBarEnd.Value;

            this.labelCount.Text = "Count : " + this.Count;

            if (_rangedObject != null)
            {
                _rangedObject.StartIndex = this.trackBarStart.Value;
                _rangedObject.Count = this.trackBarEnd.Value - this.trackBarStart.Value;
            }
        }

    }
}
