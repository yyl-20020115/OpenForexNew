using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using CommonSupport;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// This class brings together the (Data/OrderSink)OrderExecutionProvider(s) and the ChartSeries, allowing the dataDelivery to be rendered.
    /// </summary>
    [Serializable]
    public class RawDataTradeChartSeries : TradeChartSeries
    {
        List<DataBar> _data = new List<DataBar>();
        public List<DataBar> Data
        {
          get { lock(this) { return _data; } }
        }

        public override int ItemsCount
        {
            get
            {
                return _data.Count;
            }
        }

        float _minVolume = float.MaxValue;
        float _maxVolume = float.MinValue;

        /// <summary>
        /// 
        /// </summary>
        public RawDataTradeChartSeries(string name)
            : base(name)
        {
            base.Name = name;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public RawDataTradeChartSeries(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(IEnumerable<DataBar> data, TimeSpan prevalentPeriod)
        {
            lock (this)
            {
                _period = prevalentPeriod;
                _data = new List<DataBar>(data);
            }
        }

        void UpdateValues()
        {
            TracerHelper.TraceEntry();
            lock (this)
            {
                foreach (DataBar data in _data)
                {
                    _minVolume = Math.Min(_minVolume, (float)data.Volume);
                    _maxVolume = Math.Max(_maxVolume, (float)data.Volume);
                }
            }

            RaiseSeriesValuesUpdated(true);
        }

        public void UnInitialize()
        {
        }

        protected override void OnAddedToChart()
        {
            UpdateValues();
        }

        protected override void OnRemovedFromChart()
        {
            UnInitialize();
        }

        /// <summary>
        /// Main drawing routine.
        /// </summary>
        public override void Draw(ChartPane managingPane, GraphicsWrapper g, int unitsUnification,
            RectangleF clippingRectangle, float itemWidth, float itemMargin)
        {
            TracerHelper.Trace(ReflectionHelper.GetCallingMethod(2).Name);

            if (Visible == false)
            {
                return;
            }

            lock (this)
            {
                base.Draw(g, _data.AsReadOnly(), unitsUnification, clippingRectangle, itemWidth, itemMargin, _maxVolume, null);
            }
        }

        public override void DrawSeriesIcon(GraphicsWrapper g, Rectangle rectangle)
        {
            base.DrawIcon(g, RisingBarPen, RisingBarFill, rectangle);
        }

        /// <summary>
        /// Establish the total minimum value of any item in this interval.
        /// </summary>
        /// <param name="startIndex">Inclusive starting index.</param>
        /// <param name="endIndex">Exclusive ending index.</param>
        public override void GetTotalMinimumAndMaximum(int? startIndex, int? endIndex,
            ref float minimum, ref float maximum)
        {
            if (startIndex.HasValue == false)
            {
                startIndex = 0;
            }

            if (endIndex.HasValue == false)
            {
                endIndex = _data.Count;
            }

            lock (this)
            {
                for (int i = startIndex.Value; i < endIndex && i < _data.Count; i++)
                {
                    if (_data[i].HasDataValues)
                    {
                        minimum = (float)Math.Min((float)_data[i].Low, minimum);
                        maximum = (float)Math.Max((float)_data[i].High, maximum);
                    }
                }
            }
        }

        public override DateTime? GetTimeAtIndex(int index)
        {
            if (_data.Count == 0)
            {
                return null;
            }

            lock (this)
            {
                return _data[index].DateTime;
            }
        }

        public override void SaveToFile(string fileName)
        {
            lock (this)
            {
                CSVDataBarReaderWriter reader = new CSVDataBarReaderWriter(fileName, CSVDataBarReaderWriter.DataFormat.CSVHistoricalFileDefault);
                reader.Write(_data.AsReadOnly());
            }
        }

        protected override float GetDrawingValueAt(int index, object tag)
        {
            if (_data.Count == 0)
            {
                return 0;
            }

            // The lock on the dataDelivery provider must already be on.
            return (float)_data[index].GetValue(DataBar.DataValueEnum.Average);
        }
    }
}
