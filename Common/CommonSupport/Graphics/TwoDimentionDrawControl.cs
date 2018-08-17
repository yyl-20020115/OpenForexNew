using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class TwoDimentionDrawControl : Control
    {
        Point _lastMousePosition = Point.Empty;

        int _pointedRectangleXIndex = -1;
        int _pointedRectangleYIndex = -1;

        public enum RenderingModeEnum
        {
            Normal,
            // Positive values are green, negatives are red.
            BiColor
        }

        RenderingModeEnum _renderingMode = RenderingModeEnum.Normal;
        public RenderingModeEnum RenderingMode
        {
            get { return _renderingMode; }
            set { _renderingMode = value; }
        }

        public int SelectedXIndex
        {
            get
            {
                return _pointedRectangleXIndex;
            }
        }

        public int SelectedYIndex
        {
            get
            {
                return _pointedRectangleYIndex;
            }
        }

        double _minValue = double.MaxValue;
        public double MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        double _maxValue = double.MinValue;
        public double MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        double[][] _values = new double[][] { };
        // No get, as modifications may alter state.
        public double[][] Values
        {
            set 
            { 
                _values = value;

                if (value == null)
                {
                    return;
                }

                double localMinValue = double.MaxValue;
                double localMaxValue = double.MinValue;
                // Establish min and max value.
                for (int i = 0; i < value.Length; i++)
                {
                    for (int j = 0; j < value[i].Length; j++)
                    {
                        localMaxValue = Math.Max(localMaxValue, value[i][j]);
                        localMinValue = Math.Min(localMinValue, value[i][j]);
                    }
                }

                //System.Diagnostics.Debug.Assert(localMinValue >= _minValue, "Min value for 2D Draw control not properly assigned.");
                //System.Diagnostics.Debug.Assert(localMaxValue <= _maxValue, "Max value for 2D Draw control not properly assigned.");
            }
        }

        public delegate void RectangleDelegate(int xIndex, int yIndex);
        public event RectangleDelegate RectangleClickedEvent;
        public event RectangleDelegate RectanglePointedEvent;


        /// <summary>
        /// 
        /// </summary>
        public TwoDimentionDrawControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
        }

        Rectangle[][] GenerateRectangles()
        {
            Rectangle[][] rectangles = new Rectangle[_values.Length][];

            if (_values.Length == 0)
            {
                return rectangles;
            }

            double partX = (double)this.Width / (double)_values.Length;
            double partY = (double)this.Height / (double)_values[0].Length;

            double x = 0;
            for (int xi = 0; xi < _values.Length; xi++)
            {
                rectangles[xi] = new Rectangle[_values[xi].Length];
                double y = 0;
                for (int yi = 0; yi < _values[xi].Length; yi++)
                {
                    rectangles[xi][yi] = new Rectangle((int)Math.Round(x), (int)Math.Round(y), (int)partX, (int)partY);
                    y += partY;
                }
                x += partX;
            }

            return rectangles;
        }

        void GetRectangleIndexAt(Point point, out int xRectangle, out int yRectangle)
        {// TODO : optimize
            Rectangle[][] rectangles = GenerateRectangles();

            xRectangle = -1;
            yRectangle = -1;

            for (int xi = 0; xi < _values.Length; xi++)
            {
                for (int yi = 0; yi < _values[xi].Length; yi++)
                {
                    if (rectangles[xi][yi].Contains(point))
                    {
                        xRectangle = xi;
                        yRectangle = yi;
                        return;
                    }
                }
            }
            return;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _lastMousePosition = Point.Empty;

            this.Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int xIndex, yIndex;
            GetRectangleIndexAt(e.Location, out xIndex, out yIndex);

            if (RectangleClickedEvent != null)
            {
                RectangleClickedEvent(xIndex, yIndex);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _lastMousePosition = e.Location;
            // This will establish the pointer rectangle indeces.
            this.Refresh();

            if (_pointedRectangleXIndex != -1 && _pointedRectangleYIndex != -1
                && RectanglePointedEvent != null)
            {
                RectanglePointedEvent(_pointedRectangleXIndex, _pointedRectangleYIndex);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.DrawRectangle(Pens.Black, 0,0, this.Width-1, this.Height-1);

            if (this.DesignMode)
            {
                pe.Graphics.FillRectangle(Brushes.Black, pe.ClipRectangle);
                return;
            }

            if (_values == null)
            {
                return;
            }

            Rectangle[][] rectangles = GenerateRectangles();

            _pointedRectangleXIndex = -1;
            _pointedRectangleYIndex = -1;

            SolidBrush brush = new SolidBrush(Color.Black);

            double totalDifference = _maxValue - _minValue;
            // Use the zero value to establish the intensity of the color in color mode.
            int zeroValue = (int)((( -_minValue) / (totalDifference + 1)) * 255);

            // Max 2 when zeroValue is 127.5, when zeroValue == 0, is 1. This is used to brighten up the colors.
            double compensationCoeffecient = 2 - ((double)zeroValue - 127.5) / 127.5;

            // Perform drawing.
            for (int xi = 0; xi < _values.Length; xi++)
            {
                System.Diagnostics.Debug.Assert(double.IsNaN(_maxValue) == false && double.IsNaN(_minValue) == false);
                for (int yi = 0; yi < _values[xi].Length; yi++)
                {
                    int colorValue = (int)(((_values[xi][yi] - _minValue) / (totalDifference + 1)) * 255);
                    colorValue = Math.Min(255, colorValue);
                    colorValue = Math.Max(0, colorValue);
                    
                    if (RenderingMode == RenderingModeEnum.Normal)
                    {
                        brush.Color = Color.FromArgb(colorValue, colorValue, colorValue);
                    }
                    else if (RenderingMode == RenderingModeEnum.BiColor)
                    {
                        if (colorValue >= zeroValue)
                        {// Positive, Green.
                            brush.Color = Color.FromArgb(0, (int)Math.Min(255, (colorValue - zeroValue) * compensationCoeffecient), 0);
                        }
                        else
                        {// Negative, Red.
                            brush.Color = Color.FromArgb((int)Math.Min(255, (zeroValue - colorValue) * compensationCoeffecient), 0, 0);
                        }
                    }

                    pe.Graphics.FillRectangle(brush, rectangles[xi][yi].X, rectangles[xi][yi].Y, rectangles[xi][yi].Width + 1, rectangles[xi][yi].Height+1);

                    if (_lastMousePosition != Point.Empty && 
                        rectangles[xi][yi].Contains(_lastMousePosition))
                    {
                        _pointedRectangleXIndex = xi;
                        _pointedRectangleYIndex = yi;
                    }
                }
            }

            if (_pointedRectangleXIndex != -1 && _pointedRectangleYIndex != -1)
            {
                pe.Graphics.DrawRectangle(Pens.Red, rectangles[_pointedRectangleXIndex][_pointedRectangleYIndex]);
            }
        }
    }
}

