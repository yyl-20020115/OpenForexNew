using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CommonSupport
{
    public partial class BrushControl : UserControl
    {
        Brush _brush;
        public Brush Brush
        {
            get 
            { 
                return _brush; 
            }

            set 
            {
                _brush = value;
                UpdateUI();
            }
        }

        string _brushName;
        public string BrushName
        {
            get 
            { 
                return _brushName;
            }

            set 
            { 
                _brushName = value;
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

        public delegate void BrushChangedDelegate(BrushControl control);
        public event BrushChangedDelegate BrushChangedEvent;

        /// <summary>
        /// 
        /// </summary>
        public BrushControl()
        {
            InitializeComponent();
        }

        private void BrushControl_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_brush != null)
            {
                e.Graphics.FillRectangle(_brush, new RectangleF(labelColor.Location, labelColor.Size));
            }
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            labelName.Text = _brushName;
        }

        private void labelColor_Click(object sender, EventArgs e)
        {
            if (ReadOnly)
            {
                return;
            }

            ColorDialog dialog = new ColorDialog();
            if (_brush is SolidBrush)
            {
                dialog.Color = ((SolidBrush)_brush).Color;
            }
            else if (_brush is LinearGradientBrush)
            {
                dialog.Color = ((LinearGradientBrush)_brush).LinearColors[0];
            }

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _brush = new SolidBrush(dialog.Color);
            }

            UpdateUI();

            if (BrushChangedEvent != null)
            {
                BrushChangedEvent(this);
            }

            this.Refresh();
        }

        private void comboBoxDash_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BrushChangedEvent != null)
            {
                BrushChangedEvent(this);
            }
        }

        private void labelEmpty_Click(object sender, EventArgs e)
        {
            Brush = null;
            if (BrushChangedEvent != null)
            {
                BrushChangedEvent(this);
            }
            this.Refresh();
        }
    }
}
