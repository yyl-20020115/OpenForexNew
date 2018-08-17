using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonSupport
{
    public class LabelShape : Shape
    {
        public LabelShape(Stencil stencil) : base(stencil)
        {
        }
        
        string _text = "Label shape";
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        Color _color = Color.Blue;
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        static Font DefaultFont = new Font("Tahoma", 10, FontStyle.Regular);
        public Font Font
        {
            get { return DefaultFont; }
        }

        public override void Render(System.Drawing.Graphics graphics)
        {
            base.Render(graphics);
            
            graphics.DrawString(Text, Font, MainTextBrush, new PointF(0, 0));
        }

    }
}
