//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Runtime.Serialization;
//using CommonSupport;
//using System.Drawing;

//namespace CommonFinancial
//{
//    public class MultiProviderTradeChartSeries : TradeChartSeries
//    {
//        List<ISessionDataProvider> _providers = new List<ISessionDataProvider>();

//        ISourceManager _manager;
//        public ISourceManager Manager
//        {
//            get
//            {
//                lock (this)
//                {
//                    return _manager;
//                }
//            }

//            set
//            {
//                lock (this)
//                {
//                    if (_manager != null)
//                    {
//                        _manager.SessionCreatedEvent -= new CommonSupport.GeneralHelper.GenericDelegate<ISourceManager, ExpertSession>(_manager_SessionCreatedEvent);
//                        _manager.SessionDestroyingEvent -= new CommonSupport.GeneralHelper.GenericDelegate<ISourceManager, ExpertSession>(_manager_SessionDestroyingEvent);

//                        foreach (ISessionDataProvider provider in _providers.ToArray())
//                        {
//                            RemoveProvider(provider);
//                        }

//                        SystemMonitor.CheckError(_providers.Count == 0, "Operation logic error.");
//                    }

//                    _manager = value;

//                    if (_manager != null)
//                    {
//                        _manager.SessionCreatedEvent += new CommonSupport.GeneralHelper.GenericDelegate<ISourceManager, ExpertSession>(_manager_SessionCreatedEvent);
//                        _manager.SessionDestroyingEvent += new CommonSupport.GeneralHelper.GenericDelegate<ISourceManager, ExpertSession>(_manager_SessionDestroyingEvent);

//                        foreach (ISessionDataProvider provider in _providers)
//                        {
//                            AddProvider(provider);
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override int MaximumIndex
//        {
//            get
//            {
//                int result = 0;
//                lock (this)
//                {
//                    foreach (ISessionDataProvider provider in _providers)
//                    {
//                        result = Math.Max(provider.DataUnitCount, result);
//                    }
//                }
//                return result;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public MultiProviderTradeChartSeries(string name)
//            : base(name)
//        {
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="orderInfo"></param>
//        /// <param name="context"></param>
//        public MultiProviderTradeChartSeries(SerializationInfo orderInfo, StreamingContext context)
//            : base(orderInfo, context)
//        {
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="orderInfo"></param>
//        /// <param name="context"></param>
//        public override void GetObjectData(SerializationInfo orderInfo, StreamingContext context)
//        {
//            base.GetObjectData(orderInfo, context);
//        }

//        protected override void OnAddedToChart()
//        {
//            RaiseSeriesValuesUpdated(true);
//        }
        
//        public override void DrawSeriesIcon(GraphicsWrapper g, Rectangle rectangle)
//        {
//            base.DrawIcon(g, RisingBarPen, RisingBarFill, rectangle);
//        }

//        /// <summary>
//        /// Establish the total minimum value of any item in this interval.
//        /// </summary>
//        /// <param name="startIndex">Inclusive starting index.</param>
//        /// <param name="endIndex">Exclusive ending index.</param>
//        public override void GetTotalMinimumAndMaximum(int startIndex, int endIndex,
//            ref float minimum, ref float maximum)
//        {
//            lock (this)
//            {
//                foreach (ISessionDataProvider provider in _providers)
//                {
//                    for (int i = startIndex; i < endIndex && i < provider.DataUnits.Count; i++)
//                    {
//                        if (provider.DataUnits[i].HasDataValues)
//                        {
//                            minimum = (float)Math.Min(provider.DataUnits[i].Low, minimum);
//                            maximum = (float)Math.Max(provider.DataUnits[i].High, maximum);
//                        }
//                    }
//                }
//            }
//        }


//        protected override void OnRemovedFromChart()
//        {
//            base.OnRemovedFromChart();
//            Manager = null;
//        }

//        void AddProvider(ISessionDataProvider provider)
//        {
//            lock (this)
//            {
//                if (_providers.Contains(provider) == false)
//                {
//                    _providers.Add(provider);
//                    provider.ValuesUpdateEvent += new ValuesUpdatedDelegate(_provider_ValuesUpdateEvent);
//                }
//            }
//        }

//        void RemoveProvider(ISessionDataProvider provider)
//        {
//            lock (this)
//            {
//                if (_providers.Contains(provider))
//                {
//                    provider.ValuesUpdateEvent -= new ValuesUpdatedDelegate(_provider_ValuesUpdateEvent);
//                    _providers.Remove(provider);
//                }
//            }
//        }

//        void _manager_SessionDestroyingEvent(ISourceManager parameter1, ExpertSession session)
//        {
//            RemoveProvider(session.SessionDataProvider);
//        }

//        void _manager_SessionCreatedEvent(ISourceManager parameter1, ExpertSession session)
//        {
//            AddProvider(session.SessionDataProvider);
//        }

//        void _provider_ValuesUpdateEvent(IQuoteProvider dataProvider, DataProviderUpdateType updateType, int updatedItemsCount, int stepsRemaining)
//        {
//            if (stepsRemaining == 0)
//            {
//                RaiseSeriesValuesUpdated(true);
//            }
//        }

//        public override void Draw(ChartPane managingPane, GraphicsWrapper g, int unitsUnification, System.Drawing.RectangleF clippingRectangle, float itemWidth, float itemMargin)
//        {
            
//        }
//    }
//}
