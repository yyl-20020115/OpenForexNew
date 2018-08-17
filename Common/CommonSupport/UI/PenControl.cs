using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class PenControl : UserControl
    {
        Pen _pen;
        public Pen Pen
        {
            get 
            { 
                return _pen; 
            }

            set 
            { 
                _pen = (Pen)value.Clone();
                UpdateUI();
            }
        }

        string _penName;
        public string PenName
        {
            get 
            { 
                return _penName;
            }

            set 
            { 
                _penName = value;
                UpdateUI();
            }
        }

        bool _readOnly = false;

        public bool ReadOnly
        {
            get 
            { 
                return _readOnly; 
            }

            set 
            { 
                _readOnly = value;
                UpdateUI();
            }
        }

        public delegate void PenChangedDelegate(PenControl control);
        public event PenChangedDelegate PenChangedEvent;

        /// <summary>
        /// 
        /// </summary>
        public PenControl()
        {
            InitializeComponent();
            comboBoxDash.Items.AddRange(Enum.GetNames(typeof(System.Drawing.Drawing2D.DashStyle)));
        }

        private void PenControl_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            if (_pen == null)
            {
                labelName.Text = "";
                comboBoxDash.SelectedIndex = -1;
                labelColor.BackColor = this.BackColor;
                return;
            }

            labelColor.BackColor = _pen.Color;
            labelName.Text = _penName;
            comboBoxDash.Enabled = !ReadOnly;
            comboBoxDash.SelectedIndex = (int)_pen.DashStyle;
        }

        private void labelColor_Click(object sender, EventArgs e)
        {
            if (ReadOnly)
            {
                return;
            }

            ColorDialog dialog = new ColorDialog();
            dialog.Color = _pen.Color;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _pen.Color = dialog.Color;
            }

            UpdateUI();

            if (PenChangedEvent != null)
            {
                PenChangedEvent(this);
            }
        }

        private void comboBoxDash_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)Enum.Parse(typeof(System.Drawing.Drawing2D.DashStyle), comboBoxDash.SelectedItem.ToString());

            if (PenChangedEvent != null)
            {
                PenChangedEvent(this);
            }
        }
    }
}
