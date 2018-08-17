using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Drawing;
using System.Collections.ObjectModel;

namespace CommonFinancial
{
    /// <summary>
    /// Thread safe.
    /// </summary>
    public class LinesChartSeries : IndexBasedChartSeries
    {
        public enum ChartTypeEnum
        {
            Line,
            Histogram,
            ColoredArea
        }

        protected List<Pen> _valueSetsPens = new List<Pen>();

        List<float[]> _valueSets = new List<float[]>();
        public ReadOnlyCollection<float[]> ValueSets
        {
            get
            {
                lock (this)
                {
                    return _valueSets.AsReadOnly();
                }
            }
        }

        protected Pen _defaultPen = Pens.White;
        public Pen DefaultPen
        {
            get { return _defaultPen; }
            set { _defaultPen = value; }
        }

        protected Brush _defaultFill = Brushes.WhiteSmoke;
        public Brush DefaultFill
        {
            get { return _defaultFill; }
            set { _defaultFill = value; }
        }

        volatile int _itemsCount = -1;
        public override int ItemsCount
        {
            get { return _itemsCount; }
        }

        volatile ChartTypeEnum _chartType = ChartTypeEnum.Line;
        public ChartTypeEnum ChartType
        {
            get { return _chartType; }
        }

        public override string[] ChartTypes
        {
            get { return Enum.GetNames(typeof(ChartTypeEnum)); }
        }


        public override string SelectedChartType
        {
            get
            {
                return Enum.GetName(typeof(ChartTypeEnum), _chartType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LinesChartSeries()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public LinesChartSeries(string name)
            : base(name)
        {
        }

        public LinesChartSeries(string name, ChartTypeEnum chartType, float[] values)
            : base(name)
        {
            _chartType = chartType;
            AddValueSet(values);
        }

        public void ClearValues()
        {
            lock (this)
            {
                _valueSets.Clear();
                _valueSetsPens.Clear();
            }

            _itemsCount = 0;
        }

        public void AddValueSet(float[] valueSet)
        {
            AddValueSet(valueSet, null);
        }

        public void AddValueSet(float[] valueSet, Pen pen)
        {
            lock (this)
            {
                _valueSets.Add(valueSet);
                _valueSetsPens.Add(pen);
            }

            _itemsCount = Math.Max(valueSet.Length, _itemsCount);

            RaiseSeriesValuesUpdated(true);
        }

        /// <summary>
        /// Helper, for a single value set.
        /// </summary>
        public void SetValues(float[] valueSet)
        {
            SetValues(new float[][] { valueSet });
        }

        /// <summary>
        /// Assign values to these chart series.
        /// </summary>
        /// <param name="valueSets"></param>
        public void SetValues(IEnumerable<float[]> valueSets)
        {
            _itemsCount = 0;

            foreach (float[] valueSet in valueSets)
            {
                _itemsCount = Math.Max(_itemsCount, valueSet.Length);
            }

            lock (this)
            {
                _valueSets = new List<float[]>(valueSets);
                _valueSetsPens = new List<Pen>();
                for (int i = 0; i < _valueSets.Count; i++)
                {
                    _valueSetsPens.Add(_defaultPen);
                }
            }

            RaiseSeriesValuesUpdated(true);
        }

        public override void SaveToFile(string fileName)
        {// TODO, save simple values to file.
        }

        public override void GetTotalMinimumAndMaximum(int? startIndex, int? endIndex,
            ref float minimum, ref float maximum)
        {
            lock (this)
            {
                foreach (float[] valueSet in _valueSets)
                {
                    int actualStartValue = 0;
                    int actualEndIndex = valueSet.Length;
                    if (startIndex.HasValue)
                    {
                        actualStartValue = startIndex.Value;
                    }

                    if (endIndex.HasValue)
                    {
                        actualEndIndex = endIndex.Value;
                    }

                    for (int i = actualStartValue; i < actualEndIndex && i < valueSet.Length; i++)
                    {
                        if (double.IsNaN(valueSet[i]) == false)
                        {
                            minimum = Math.Min((float)valueSet[i], minimum);
                            maximum = Math.Max((float)valueSet[i], maximum);
                        }
                    }
                }
            }
        }

        public override void DrawSeriesIcon(GraphicsWrapper g, Rectangle rectangle)
        {
            base.DrawIcon(g, _defaultPen, _defaultFill, rectangle);
        }

        public override void Draw(ChartPane managingPane, GraphicsWrapper g, int unitsUnification, 
            RectangleF clippingRectangle, float itemWidth, float itemMargin)
        {
            if (this.Visible == false)
            {
                return;
            }

            lock (this)
            {
                for (int i = 0; i < _valueSets.Count; i++)
                {
                    Pen pen = _defaultPen;
                    if (_valueSetsPens[i] == null)
                    {
                        pen = _valueSetsPens[i];
                    }

                    base.DrawItemSet(_chartType, g, pen, _defaultFill, unitsUnification, clippingRectangle, itemWidth, itemMargin, _valueSets[i]);
                }
            } // Lock
        }

        public override void SetSelectedChartType(string chartType)
        {
            _chartType = (ChartTypeEnum)Enum.Parse(typeof(ChartTypeEnum), chartType);
        }

        protected override float GetDrawingValueAt(int index, object tag)
        {
            float[] values = (float[])tag;
            return values[index];
        }
    }
}
