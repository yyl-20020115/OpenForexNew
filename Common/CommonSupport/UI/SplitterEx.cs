using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CommonSupport
{
    /// <summary>
    /// Extended splitter.
    /// </summary>
    public class SplitterEx : Splitter
    {
        Pen _pen = new Pen(Color.FromArgb(231, 229, 224));

        /// <summary>
        /// 
        /// </summary>
        public SplitterEx()
        {
        }

        protected void Draw(PaintEventArgs e, bool horizontal)
        {
            int margin = 5;

            if (horizontal)
            {
                Point p1 = new Point(margin, this.Height / 2);
                Point p2 = new Point(this.Width - margin, this.Height / 2);

                e.Graphics.DrawLine(SystemPens.ControlDark, p1.X, p1.Y, p2.X, p2.Y);
                e.Graphics.DrawLine(SystemPens.ControlLightLight, p1.X, p1.Y - 1, p2.X, p2.Y - 1);
                
                //e.Graphics.DrawLine(Pens.DarkGray, p1.X, p1.Y, p2.X, p2.Y);
                //e.Graphics.DrawLine(_pen, p1.X, p1.Y - 1, p2.X, p2.Y - 1);
            }
            else
            {
                Point p1 = new Point(this.Width / 2, margin);
                Point p2 = new Point(this.Width / 2, this.Height - margin);

                e.Graphics.DrawLine(SystemPens.ControlDark, p1.X, p1.Y, p2.X, p2.Y);
                e.Graphics.DrawLine(SystemPens.ControlLightLight, p1.X - 1, p1.Y, p2.X - 1, p2.Y);

                //e.Graphics.DrawLine(Pens.DarkGray, p1.X, p1.Y, p2.X, p2.Y);
                //e.Graphics.DrawLine(_pen, p1.X - 1, p1.Y, p2.X - 1, p2.Y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            switch (this.Dock)
            {
                case DockStyle.Bottom:
                    Draw(e, true);
                    break;
                case DockStyle.Fill:
                    Draw(e, false);
                    break;
                case DockStyle.Left:
                    Draw(e, false);
                    break;
                case DockStyle.None:
                    break;
                case DockStyle.Right:
                    Draw(e, false);
                    break;
                case DockStyle.Top:
                    Draw(e, true);
                    break;
                default:
                    break;
            }
            
        }
    }
}
