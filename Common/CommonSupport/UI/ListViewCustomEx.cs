//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;

//namespace CommonSupport
//{
//    [Serializable]
//    public partial class ListViewCustomEx : UserControl
//    {
//        [Serializable]
//        public class ListViewExColumn : MarshalByRefObject, ICloneable
//        {
//            /// <summary>
//            /// 
//            /// </summary>
//            public ListViewExColumn()
//            {
//            }

//            string _header;
//            public string Header
//            {
//                get { return _header; }
//                set { _header = value; }
//            }

//            int _width = 40;
//            public int Width
//            {
//                get { return _width; }
//                set { _width = value; }
//            }


//            #region ICloneable Members

//            public object Clone()
//            {
//                return new ListViewExColumn() { _header = this._header, _width = this._width };
//            }

//            #endregion
//        }

//        List<ListViewExColumn> _colums = new List<ListViewExColumn>();
        
//        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//        public List<ListViewExColumn> Colums
//        {
//            get { return _colums; }
//            set { _colums = value; }
//        }

//        public ImageList ImageList { get; set; }

//        List<string> _items = new List<string>();
//        public List<string> Items
//        {
//            get { return _items; }
//        }

//        Brush _brush = Brushes.Black;

//        int _margin = 2;

//        VScrollBar _vScrollBar = new VScrollBar();

//        int _headerHeight = 17;

//        Brush _headerBrush = SystemBrushes.Control;

//        Pen _headerBorderPen = SystemPens.WindowFrame;

//        /// <summary>
//        /// 
//        /// </summary>
//        public ListViewCustomEx()
//        {
//            InitializeComponent();

//            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
//            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
//            this.SetStyle(ControlStyles.UserPaint, true);

//            Font = new Font("Tahoma", 8);
//        }

//        protected override void OnCreateControl()
//        {
//            base.OnCreateControl();

//            _vScrollBar.Dock = DockStyle.Right;
//            _vScrollBar.ValueChanged += new EventHandler(_vScrollBar_ValueChanged);
//            this.Controls.Add(_vScrollBar);

//        }

//        void _vScrollBar_ValueChanged(object sender, EventArgs e)
//        {
//            this.Refresh();
//        }

//        protected override void OnMouseWheel(MouseEventArgs e)
//        {
//            base.OnMouseWheel(e);
//            int value = -e.Delta / 30;
//            if (_vScrollBar.Value + value < 0)
//            {
//                _vScrollBar.Value = 0;
//            }
//            else if (_vScrollBar.Value + value > _vScrollBar.Maximum)
//            {
//                _vScrollBar.Value = _vScrollBar.Maximum;
//            }
//            else
//            {
//                _vScrollBar.Value += value;
//            }

//            //_vScrollBar.Value = Math.Max(_vScrollBar.Value + e.Delta, 0);
//        }

//        protected override void OnPaint(PaintEventArgs pe)
//        {
//            base.OnPaint(pe);

//            if (Items == null)
//            {
//                return;
//            }

//            //pe.Graphics.ResetClip();

//            pe.Graphics.Clear(SystemColors.Window);

//            _vScrollBar.Maximum = Items.Count;

//            SizeF size = pe.Graphics.MeasureString("ABCDEFGH", Font);
//            for (int i = _vScrollBar.Value; i < Items.Count; i++)
//            {
//                PointF location = new PointF(5, _headerHeight + _margin + (i - _vScrollBar.Value) * (size.Height + _margin));

//                DrawRow(pe, i, location);

//                if (location.Y > this.Height)
//                {
//                    break;
//                }
//            }

//            DrawHeader(pe);

//            // Border.
//            pe.Graphics.DrawRectangle(SystemPens.WindowFrame, 0, 0, this.Width, this.Height);
//        }

//        void DrawHeader(PaintEventArgs pe)
//        {
//            SizeF size = pe.Graphics.MeasureString("ABCDEFGH", Font);

//            pe.Graphics.FillRectangle(_headerBrush, 0, 0, this.Width - 1, _headerHeight);
//            pe.Graphics.DrawRectangle(_headerBorderPen, 0, 0, this.Width - 1, _headerHeight);

//            int x = 0;
//            int y = (int)((_headerHeight - size.Height) / 2) + 1;
//            foreach (ListViewExColumn column in _colums)
//            {
//                pe.Graphics.DrawString(column.Header, SystemFonts.DefaultFont, SystemBrushes.ControlText, new RectangleF(x, y, column.Width, _headerHeight - 1));
//                pe.Graphics.DrawLine(_headerBorderPen, x, 0, x, _headerHeight);
//                x += column.Width;
//            }
//        }

//        void DrawRow(PaintEventArgs pe, int index, PointF location)
//        {
//            pe.Graphics.DrawString(Items[index], Font, _brush, location);
//        }
//    }
//}
