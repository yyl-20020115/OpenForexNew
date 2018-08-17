//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Drawing;
//using System.ComponentModel;

//namespace CommonFinancial
//{
//    /// <summary>
//    /// Class is a chart object, marking (showing) orders executed on given locations.
//    /// </summary>
//    public class OrdersMarkerObject : CustomObject
//    {
//        Image _imageUp;
//        Image _imageDown;

//        ISourceOrderExecution _provider;

//        TradeChartSeries _tradeSeries;

//        DateTime? _startingTime;

//        /// <summary>
//        /// 
//        /// </summary>
//        public OrdersMarkerObject()
//        {
//            DrawingOrder = DrawingOrderEnum.PostSeries;

//            ComponentResourceManager resources = new ComponentResourceManager(typeof(ChartPane));
//            _imageUp = (Image)(resources.GetObject("imageArrowUpGreen"));
//            _imageDown = (Image)(resources.GetObject("imageArrowDownRed"));
//        }

//        public bool Initialize(TradeChartSeries tradeSeries, ISourceOrderExecution provider)
//        {
//            _tradeSeries = tradeSeries;
//            _provider = provider;

//            //_provider.OrderAddedEvent += new CommonSupport.GeneralHelper.GenericDelegate<ActiveOrder>(_provider_OrderAddedEvent);
//            //_provider.OrderRemovedEvent += new CommonSupport.GeneralHelper.GenericDelegate<ActiveOrder>(_provider_OrderRemovedEvent);
//            //_provider.OrdersUpdatedEvent += new CommonSupport.GeneralHelper.GenericDelegate<ActiveOrder>(_provider_OrderUpdatedEvent);
//            return true;
//        }

//        //void _provider_OrderUpdatedEvent(ActiveOrder parameter1)
//        //{
//        //}

//        //void _provider_OrderRemovedEvent(ActiveOrder parameter1)
//        //{
//        //}

//        //void _provider_OrderAddedEvent(ActiveOrder parameter1)
//        //{
//        //}

//        public void UnInitialize()
//        {
//            if (_provider != null)
//            {
//                //_provider.OrderAddedEvent -= new CommonSupport.GeneralHelper.GenericDelegate<ActiveOrder>(_provider_OrderAddedEvent);
//                //_provider.OrderRemovedEvent -= new CommonSupport.GeneralHelper.GenericDelegate<ActiveOrder>(_provider_OrderRemovedEvent);
//                //_provider.OrdersUpdatedEvent -= new CommonSupport.GeneralHelper.GenericDelegate<ActiveOrder>(_provider_OrderUpdatedEvent);

//                _provider = null;
//            }
//        }

//        public override void Draw(Graphics g, PointF? mousePosition, RectangleF clippingRectangle, RectangleF drawingSpace)
//        {
//            if (_tradeSeries.DataBarsCount > 0)
//            {
//                _startingTime = _tradeSeries.DataBars[0].DateTime;
//            }

//            if (_startingTime.HasValue == false)
//            {
//                return;
//            }






//            List<BarData> datas = new List<BarData>();
//            bool timeGapFound = false;

//            int startIndex, endIndex;
//            GetDrawingRangeIndecesFromClippingRectange(clippingRectangle, drawingPoint, unitsUnification, out startIndex, out endIndex, itemWidth, itemMargin);

//            for (int i = startIndex; i < endIndex && i < _dataBars.Count; i++)
//            {
//                datas.Add(_dataBars[i]);

//                if (unitsUnification == 1 && ShowTimeGaps && i > 0 && _dataBars[i].DateTime - _dataBars[i - 1].DateTime != _period)
//                {
//                    timeGapFound = true;
//                }

//                if (i % unitsUnification == 0)
//                {
//                    BarData combinedData = BarData.CombinedBar(datas.ToArray());
//                    datas.Clear();
//                    if (combinedData.HasDataValues)
//                    //&& drawingPoint.X >= clippingRectangle.X 
//                    //&& drawingPoint.X <= clippingRectangle.X + clippingRectangle.Width)
//                    {
//                        if (timeGapFound && ShowTimeGaps && _timeGapsLinePen != null)
//                        {
//                            timeGapFound = false;
//                            g.DrawLine(_timeGapsLinePen, new PointF(drawingPoint.X, clippingRectangle.Y), new PointF(drawingPoint.X, clippingRectangle.Y + clippingRectangle.Height));

//                            //g.DrawLine(_timeGapsLinePen, new PointF(drawingPoint.X, clippingRectangle.Y), new PointF(drawingPoint.X, (float)(dataDelivery.High)));
//                            //g.DrawLine(_timeGapsLinePen, new PointF(drawingPoint.X, (float)(dataDelivery.High + dataDelivery.BarTotalLength / 2f)), new PointF(drawingPoint.X, clippingRectangle.Y + clippingRectangle.Height));
//                        }

//                        if (_chartType == ChartTypeEnum.CandleStick)
//                        {
//                            DrawCandleStick(g, ref drawingPoint, combinedData, itemWidth * unitsUnification, itemMargin * unitsUnification);
//                        }
//                        else
//                        {
//                            DrawBar(g, ref drawingPoint, combinedData, itemWidth, itemMargin);
//                        }
//                    }
//                }

//                drawingPoint.X = (i + 1) * (itemMargin + itemWidth);
//            }
//        }
//    }
//}
