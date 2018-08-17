using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class FourDimentionDrawControl : UserControl
    {
        List<List<double[][]>> _values = new List<List<double[][]>>();

        public delegate void ItemDelegate(int i4d, int i3d, int i2d, int i1d);
        public event ItemDelegate ItemPointedEvent;
        public event ItemDelegate ItemSelectedEvent;

        public int Maximum4thDSliceIndex
        {
            get
            {
                return trackBarHorizontal.Maximum;
            }
        }

        public int Maximum3thDSliceIndex
        {
            get
            {
                return trackBarVertical.Maximum;
            }
        }

        public int Selected4thDSliceIndex
        {
            get { return trackBarHorizontal.Value; }
            set
            {
                trackBarHorizontal.Value = value;
            }
        }

        public int Selected3thDSliceIndex
        {
            get 
            { 
                return trackBarVertical.Value; 
            }
            set 
            {
                trackBarVertical.Value = value;
            }
        }

        public List<List<double[][]>> Values4D
        {
            set
            {
                _values = value;
                UpdateUI();
            }
        }

        public List<double[][]> Values3D
        {
            set
            {
                _values.Clear();
                _values.Add(value);
                UpdateUI();
            }
        }

        public double[][] Values2D
        {
            set
            {
                _values.Clear();
                List<double[][]> list = new List<double[][]>();
                list.Add(value);
                _values.Add( list ) ;
                UpdateUI();
            }
        }

        public double[] Values1D
        {
            set
            {
                _values.Clear();
                List<double[][]> list = new List<double[][]>();
                list.Add(new double[][] {value });
                _values.Add(list);
                UpdateUI();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public FourDimentionDrawControl()
        {
            InitializeComponent();
        }

        private void TwoDPlusControl_Load(object sender, EventArgs e)
        {
            trackBarHorizontal.Enabled = false;
            trackBarVertical.Enabled = false;

            twoDDrawControl.RenderingMode = TwoDimentionDrawControl.RenderingModeEnum.BiColor;
            twoDDrawControl.RectangleClickedEvent += new TwoDimentionDrawControl.RectangleDelegate(twoDDrawControl_RectangleClickedEvent);
            twoDDrawControl.RectanglePointedEvent += new TwoDimentionDrawControl.RectangleDelegate(twoDDrawControl_RectanglePointedEvent);
        }

        void twoDDrawControl_RectanglePointedEvent(int xIndex, int yIndex)
        {
            if (ItemPointedEvent != null)
            {
                ItemPointedEvent(Selected4thDSliceIndex, Selected3thDSliceIndex, xIndex, yIndex);
            }

            UpdateStatusLabel();
        }

        void twoDDrawControl_RectangleClickedEvent(int xIndex, int yIndex)
        {
            if (ItemSelectedEvent != null)
            {
                ItemSelectedEvent(Selected4thDSliceIndex, Selected3thDSliceIndex, xIndex, yIndex);
            }

            UpdateStatusLabel();
        }

        void UpdateStatusLabel()
        {
            double value = double.NaN;
            if (Selected4thDSliceIndex >= 0 && Selected4thDSliceIndex < _values.Count 
                && Selected3thDSliceIndex >= 0 && Selected3thDSliceIndex < _values[Selected4thDSliceIndex].Count
                && twoDDrawControl.SelectedXIndex >= 0 && twoDDrawControl.SelectedXIndex < _values[Selected4thDSliceIndex][Selected3thDSliceIndex].Length
                && twoDDrawControl.SelectedYIndex >= 0 && twoDDrawControl.SelectedYIndex < _values[Selected4thDSliceIndex][Selected3thDSliceIndex][twoDDrawControl.SelectedXIndex].Length)
            {
                value = _values[Selected4thDSliceIndex][Selected3thDSliceIndex][twoDDrawControl.SelectedXIndex][twoDDrawControl.SelectedYIndex];
            }

            labelStatus.Text = "4:[" + Selected4thDSliceIndex + "] 3:[" + Selected3thDSliceIndex + "] 2:[" + twoDDrawControl.SelectedXIndex + "]" + " 1:[" + twoDDrawControl.SelectedYIndex+ "] v[" + value + "]" + 
            " max 4:[" + Maximum4thDSliceIndex + "] max 3:[" + Maximum3thDSliceIndex + "] Current slice Min [" + twoDDrawControl.MinValue + "] Max [" + twoDDrawControl.MaxValue + "]";
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            if (Maximum4thDSliceIndex == -1)
            {
                trackBarHorizontal.Enabled = false;
            }
            else
            {
                trackBarHorizontal.Enabled = true;
                trackBarHorizontal.Maximum = _values.Count - 1;
            }

            if (Maximum3thDSliceIndex == -1)
            {
                trackBarVertical.Enabled = false;
            }
            else
            {
                trackBarVertical.Enabled = true;
                trackBarVertical.Maximum = _values[Selected4thDSliceIndex].Count - 1;
            }

            twoDDrawControl.MinValue = double.MaxValue;
            twoDDrawControl.MaxValue = double.MinValue;

            // 4D searching for min and max.
            foreach (List<double[][]> list in _values)
            {
                foreach (double[][] values in list)
                {
                    foreach (double[] subValues in values)
                    {
                        foreach (double value in subValues)
                        {
                            twoDDrawControl.MinValue = Math.Min(twoDDrawControl.MinValue, value);
                            twoDDrawControl.MaxValue = Math.Max(twoDDrawControl.MaxValue, value);
                        }
                    }
                }
            }

            twoDDrawControl.Values = _values[Selected4thDSliceIndex][Selected3thDSliceIndex];
            this.twoDDrawControl.Refresh();

            UpdateStatusLabel();
        }

        private void trackBarHorizontal_ValueChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void trackBarVertical_ValueChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void checkBoxColorMode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxColorMode.Checked)
            {
                twoDDrawControl.RenderingMode = TwoDimentionDrawControl.RenderingModeEnum.BiColor;
            }
            else
            {
                twoDDrawControl.RenderingMode = TwoDimentionDrawControl.RenderingModeEnum.Normal;
            }
        }

    }
}
