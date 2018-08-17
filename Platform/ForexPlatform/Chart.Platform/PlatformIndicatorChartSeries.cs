using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.Serialization;
using CommonFinancial;
using CommonSupport;
using ForexPlatformPersistence;

namespace ForexPlatform
{
    /// <summary>
    /// This class brings together the indicator and the ChartSeries, allowing the dataDelivery to be rendered
    /// and containing render specific information.
    /// </summary>
    [Serializable]
    public class PlatformIndicatorChartSeries : IndexBasedChartSeries, IDynamicPropertyContainer
    {
        PlatformIndicator _indicator = null;
        public PlatformIndicator Indicator
        {
            get { return _indicator; }
        }

        Pen _defaultPen = Pens.WhiteSmoke;
        /// <summary>
        /// 
        /// </summary>
        public Pen DefaultPen
        {
            get { return _defaultPen; }
            set { _defaultPen = value; }
        }

        const int DashSize = 6;

        Pen _defaultDashedPen = new Pen(Color.LightGray, 1);
        /// <summary>
        /// 
        /// </summary>
        public Pen DefaultDahsedPen
        {
            get { return _defaultPen; }
            set { _defaultPen = value; }
        }

        Brush _defaultBrush = Brushes.WhiteSmoke;
        /// <summary>
        /// 
        /// </summary>
        public Brush DefaultBrush
        {
            get { return _defaultBrush; }
            set { _defaultBrush = value; }
        }
        
        const LinesChartSeries.ChartTypeEnum DefaultChartType = LinesChartSeries.ChartTypeEnum.Line;

        public override int ItemsCount
        {
            get
            {
                if (_indicator != null)
                {
                    return Indicator.Results.SetLength;
                }
                else
                {
                    return 0;
                }
            }
        }

        Dictionary<string, Pen> _outputResultSetsPens = new Dictionary<string, Pen>();
        public Dictionary<string, Pen> OutputResultSetsPens
        {
            get { return _outputResultSetsPens; }
        }

        public const string FixedLinePrefix = "FixedLine.Value.";

        public const string OutputResultSetChartTypePrefix = "Output.ChartType.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PlatformIndicatorChartSeries(string name)
        {
            _defaultDashedPen.DashPattern = new float[] { DashSize, DashSize };
            Name = name;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public PlatformIndicatorChartSeries(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            SerializationHelper.ManualDeSerialize(info, _outputResultSetsPens);
            
            _defaultPen = (Pen)info.GetValue("defaultPen", typeof(Pen));
            _defaultDashedPen = (Pen)info.GetValue("defaultDashedPen", typeof(Pen));
            
            // Dash Pattern can not be persisted (see surogate for details).
            _defaultDashedPen.DashPattern = new float[] { DashSize, DashSize };
            
            _defaultBrush = (Brush)info.GetValue("defaultBrush", typeof(Brush));
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            SerializationHelper.ManualSerialize(_outputResultSetsPens, info);
            
            info.AddValue("defaultPen", _defaultPen);
            info.AddValue("defaultDashedPen", _defaultDashedPen);
            info.AddValue("defaultBrush", _defaultBrush);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Initialize(PlatformIndicator indicator)
        {
            lock (this)
            {
                _indicator = indicator;
                _indicator.IndicatorCalculatedEvent += new Indicator.IndicatorCalculatedDelegate(_indicator_IndicatorCalculatedEvent);
                _indicator.Parameters.ParameterUpdatedValueEvent += new IndicatorParameters.ParameterUpdatedValueDelegate(Parameters_ParameterUpdatedValueEvent);
                foreach (string setName in indicator.Results.SetsNamesUnsafe)
                {
                    if (_outputResultSetsPens.ContainsKey(setName) == false)
                    {// For newly added names, initialize with default pen color.
                        _outputResultSetsPens.Add(setName, DefaultPen);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnInitialize()
        {
            lock (this)
            {
                if (_indicator != null)
                {
                    if (_indicator.Parameters != null)
                    {
                        _indicator.Parameters.ParameterUpdatedValueEvent -= new IndicatorParameters.ParameterUpdatedValueDelegate(Parameters_ParameterUpdatedValueEvent);
                    }
                    _indicator.IndicatorCalculatedEvent -= new Indicator.IndicatorCalculatedDelegate(_indicator_IndicatorCalculatedEvent);
                    _indicator = null;
                }
            }
        }

        /// <summary>
        /// Export this.
        /// </summary>
        /// <param name="updateUI"></param>
        public new void RaiseSeriesValuesUpdated(bool updateUI)
        {
            base.RaiseSeriesValuesUpdated(updateUI);
        }

        protected override void OnAddedToChart()
        {
            base.RaiseSeriesValuesUpdated(true);
        }

        protected override void OnRemovedFromChart()
        {
            if (_indicator != null)
            {
                _indicator.IndicatorCalculatedEvent -= new Indicator.IndicatorCalculatedDelegate(_indicator_IndicatorCalculatedEvent);
                _indicator = null;
            }
        }

        void Parameters_ParameterUpdatedValueEvent(string name, object value)
        {
        }

        void _indicator_IndicatorCalculatedEvent(Indicator indicator, bool fullRecalculation)
        {
            base.RaiseSeriesValuesUpdated(false);

            if (_indicator != null && _indicator.CustomMessages != null)
            {
                _customMessages = new Dictionary<string, Color>(_indicator.CustomMessages);
            }
        }

        public override void SaveToFile(string fileName)
        {
            SystemMonitor.NotImplementedWarning();
        }

        public override void DrawInitialActualSpaceOverlays(ChartPane managingPane, GraphicsWrapper g)
        {
            base.DrawInitialActualSpaceOverlays(managingPane, g);
        }

        public override void Draw(ChartPane managingPane, GraphicsWrapper g, int unitsUnification, RectangleF clippingRectangle, float itemWidth, float itemMargin)
        {
            if (this.Visible == false || Indicator == null)
            {
                return;
            }

            lock (Indicator.Results)
            {
                foreach (string name in Indicator.Results.SetsNamesUnsafe)
                {
                    LinesChartSeries.ChartTypeEnum? chartType = Indicator.Results.GetResultSetChartType(name);
                    if (chartType.HasValue == false)
                    {// No specific value assigned means go for default.
                        chartType = DefaultChartType;
                    }

                    base.DrawItemSet(chartType.Value, g, _outputResultSetsPens[name], _defaultBrush, 
                        unitsUnification, clippingRectangle, itemWidth, itemMargin, name);
                }
            }

            foreach(string name in Indicator.Parameters.DynamicNames)
            {// Render fixed lines.
                if (name.Contains(FixedLinePrefix))
                {
                    object value = Indicator.Parameters.GetDynamic(name);
                    if (value == null)
                    {
                        continue;
                    }

                    float floatValue = Convert.ToSingle(value);
                    int dashMove = 0;// ((int)x % DashSize);
                    g.DrawLine(_defaultDashedPen, clippingRectangle.X + dashMove, floatValue, clippingRectangle.X + clippingRectangle.Width, floatValue);
                }
            }
        }

        protected override float GetDrawingValueAt(int index, object tag)
        {
            if (Indicator == null)
            {
                return 0;
            }

            return (float)Indicator.Results[(string)tag][index];
        }

        public override void SetSelectedChartType(string chartType)
        {
            //object type = Enum.Parse(typeof(LinesChartSeries.ChartTypeEnum), chartType);
            //_selectedChartType = (LinesChartSeries.ChartTypeEnum)type;
        }

        public override void GetTotalMinimumAndMaximum(int? startIndex, int? endIndex, ref float minimum, ref float maximum)
        {
            if (Indicator == null)
            {
                return;
            }

            if (Indicator.RangeMinimum.HasValue && Indicator.RangeMaximum.HasValue)
            {
                minimum = Indicator.RangeMinimum.Value;
                maximum = Indicator.RangeMaximum.Value;
            }

            if (startIndex.HasValue == false)
            {
                startIndex = 0;
            }

            if (endIndex.HasValue == false)
            {
                endIndex = this.ItemsCount - 1;
            }

            for (int i = startIndex.Value; i <= endIndex.Value; i++)
            {
                foreach (string setName in Indicator.Results.SetsNamesUnsafe)
                {
                    ReadOnlyCollection<double> set = Indicator.Results[setName];
                    if (set.Count > i && double.IsNaN(set[i]) == false)
                    {
                        maximum = (float)Math.Max(set[i], maximum);
                        minimum = (float)Math.Min(set[i], minimum);
                    }
                }
            }
        }

        public override void DrawSeriesIcon(GraphicsWrapper g, Rectangle rectangle)
        {
            foreach (string name in _outputResultSetsPens.Keys)
            {
                base.DrawIcon(g, _outputResultSetsPens[name], Brushes.White, rectangle);
                return;
            }

            // If none are available, draw a simple black and white icon.
            base.DrawIcon(g, Pens.White, Brushes.Black, rectangle);
        }


        #region IDynamicPropertyContainer Members

        /// <summary>
        /// Make this public and accessible.
        /// </summary>
        /// <param name="updateUI"></param>
        public void PropertyChanged()
        {
            RaiseSeriesValuesUpdated(true);
        }

        #endregion


        public string[] GetGenericDynamicPropertiesNames()
        {
            List<string> names = new List<string>();
            names.AddRange(_outputResultSetsPens.Keys);
            names.AddRange(Indicator.Parameters.CoreNames);
            names.AddRange(Indicator.Parameters.DynamicNames);

            foreach (string name in Indicator.Results.SetsNamesList)
            {
                names.Add(OutputResultSetChartTypePrefix + name);
            }

            return names.ToArray();
        }

        public object GetGenericDynamicPropertyValue(string name)
        {
            if (Indicator != null)
            {
                if (Indicator.Parameters.GetCore(name) != null)
                {
                    return Indicator.Parameters.GetCore(name);
                }

                if (Indicator.Parameters.GetDynamic(name) != null)
                {
                    return Indicator.Parameters.GetDynamic(name);
                }
            }

            if (name.Contains(OutputResultSetChartTypePrefix))
            {
                LinesChartSeries.ChartTypeEnum? value = Indicator.Results.GetResultSetChartType(name.Replace(OutputResultSetChartTypePrefix, ""));
                if (value.HasValue)
                {
                    return value;
                }
                else
                {
                    return DefaultChartType;
                }
            }

            if (_outputResultSetsPens.ContainsKey(name) == false)
            {
                SystemMonitor.Error("UI logic error.");
                return null;
            }
            else
            {
                return _outputResultSetsPens[name];
            }
        }

        public Type GetGenericDynamicPropertyType(string name)
        {
            object value = GetGenericDynamicPropertyValue(name);
            if (value != null)
            {
                return value.GetType();
            }

            if (name.Contains(OutputResultSetChartTypePrefix))
            {
                return typeof(LinesChartSeries.ChartTypeEnum);
            }

            if (_outputResultSetsPens.ContainsKey(name))
            {
                _outputResultSetsPens[name].GetType();
            }

            return null;
        }

        public bool SetGenericDynamicPropertyValue(string name, object value)
        {
            if (Indicator.Parameters.ContainsCoreValue(name))
            {
                Indicator.Parameters.SetCore(name, value);
            }
            else if (Indicator.Parameters.ContainsDynamicValue(name))
            {
                Indicator.Parameters.SetDynamic(name, value);
            }
            else if (name.Contains(OutputResultSetChartTypePrefix))
            {
                Indicator.Results.SetResultSetChartType(name.Replace(OutputResultSetChartTypePrefix, string.Empty), (LinesChartSeries.ChartTypeEnum)value);
            }
            else
            {
                _outputResultSetsPens[name] = (Pen)value;
            }

            return true;
        }


    }
}
