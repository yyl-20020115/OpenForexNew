using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// This class brings together the (Data/OrderSink)OrderExecutionProvider(s) and the ChartSeries, allowing the dataDelivery to be rendered.
    /// </summary>
    [Serializable]
    public class ProviderTradeChartSeries : TradeChartSeries
    {
        volatile ISessionDataProvider _dataProvider;
        
        volatile IDataBarHistoryProvider _dataBars = null;
        /// <summary>
        /// 
        /// </summary>
        protected IDataBarHistoryProvider CurrentDataBarProvider
        {
            get 
            { 
                return _dataBars; 
            }

            set
            {
                lock (this)
                {
                    if (_dataBars != null)
                    {
                        _dataBars.DataBarHistoryUpdateEvent -= new DataBarHistoryUpdateDelegate(DataBarHistory_DataBarHistoryUpdateEvent);
                        _dataBars = null;
                    }

                    _dataBars = value;

                    if (_dataBars != null)
                    {
                        _period = _dataBars.Period;
                        _dataBars.DataBarHistoryUpdateEvent += new DataBarHistoryUpdateDelegate(DataBarHistory_DataBarHistoryUpdateEvent);
                    }
                    else
                    {
                        _period = null;
                    }
                }
            }
        }

        ISourceOrderExecution _orderExecutionProvider;

        Image _imageUp;
        Image _imageDown;
        Image _imageCross;

        volatile bool _showOrderArrow = true;

        /// <summary>
        /// Show the orders arrow below the bar.
        /// </summary>
        public bool ShowOrderArrow
        {
            get { return _showOrderArrow; }
            set { _showOrderArrow = value; }
        }

        volatile bool _showOrderSpot = true;
        
        /// <summary>
        /// Show the circle and line on the spot of the very order price.
        /// </summary>
        public bool ShowOrderSpot
        {
            get { return _showOrderSpot; }
            set { _showOrderSpot = value; }
        }

        volatile bool _showClosedOrdersTracing = true;

        /// <summary>
        /// Show order tracing line - from open to close.
        /// </summary>
        public bool ShowClosedOrdersTracing
        {
            get { return _showClosedOrdersTracing; }
            set { _showClosedOrdersTracing = value; }
        }

        volatile bool _showPendingOrdersTracing = true;
        
        /// <summary>
        /// Show order tracing line for open orders - from open price to current price.
        /// </summary>
        public bool ShowPendingOrdersTracing
        {
            get { return _showPendingOrdersTracing; }
            set { _showPendingOrdersTracing = value; }
        }

        Pen _buyDashedPen = new Pen(Color.Green);
        Pen _sellDashedPen = new Pen(Color.Red);

        volatile bool _showCurrentAskLine = true;
        public bool ShowCurrentAskLine
        {
            get { return _showCurrentAskLine; }
            set { _showCurrentAskLine = value; }
        }

        volatile bool _showCurrentBidLine = true;
        public bool ShowCurrentBidLine
        {
            get { return _showCurrentBidLine; }
            set { _showCurrentBidLine = value; }
        }

        Pen _priceLevelPen = new Pen(Color.Gray);
        public Pen PriceLevelPen
        {
            set { lock (this) { _priceLevelPen = value; } }
        }

        volatile DataBar.DataValueEnum _lineDrawingSource = DataBar.DataValueEnum.Close;
        public DataBar.DataValueEnum LineDrawingSource
        {
            get { return _lineDrawingSource; }
            set { _lineDrawingSource = value; }
        }

        volatile Order _selectedOrder = null;
        /// <summary>
        /// 
        /// </summary>
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set { _selectedOrder = value; }
        }

        /// <summary>
        /// Count of items in this series.
        /// </summary>
        public override int ItemsCount
        {
            get
            {
                if (CurrentDataBarProvider != null)
                {
                    return CurrentDataBarProvider.BarCount;
                }

                return 0;
            }
        }

        volatile float _minValue = float.MaxValue;
        volatile float _maxValue = float.MinValue;

        volatile float _minVolume = float.MaxValue;
        volatile float _maxVolume = float.MinValue;

        /// <summary>
        /// Used for selection, initialized on drawing the order stpos.
        /// </summary>
        Dictionary<Order, RectangleF> _ordersArrows = new Dictionary<Order, RectangleF>();

        public delegate void SelectedOrderChangedDelegate(Order previousSelectedOrder, Order newSelectedOrder);
        public event SelectedOrderChangedDelegate SelectedOrderChangedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ProviderTradeChartSeries(string name)
            : base(name)
        {
            base.Name = name;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public ProviderTradeChartSeries(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _showOrderArrow = info.GetBoolean("showOrderArrow");
            _showOrderSpot = info.GetBoolean("showOrderSpot");
            _showClosedOrdersTracing = info.GetBoolean("showClosedOrdersTracing");
            _showPendingOrdersTracing = info.GetBoolean("showPendingOrdersTracing");

            _buyDashedPen = (Pen)info.GetValue("buyDashedPen", typeof(Pen));
            _sellDashedPen = (Pen)info.GetValue("sellDashedPen", typeof(Pen));

            _showCurrentAskLine = info.GetBoolean("showCurrentAskLine");
            _showCurrentBidLine = info.GetBoolean("showCurrentBidLine");

            _priceLevelPen = (Pen)info.GetValue("priceLevelPen", typeof(Pen));

            _lineDrawingSource = (DataBar.DataValueEnum)info.GetInt32("lineDrawingSource");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("showOrderArrow", _showOrderArrow);
            info.AddValue("showOrderSpot", _showOrderSpot);
            info.AddValue("showClosedOrdersTracing", _showClosedOrdersTracing);
            info.AddValue("showPendingOrdersTracing", _showPendingOrdersTracing);

            info.AddValue("buyDashedPen", _buyDashedPen);
            info.AddValue("sellDashedPen", _sellDashedPen);

            info.AddValue("showCurrentAskLine", _showCurrentAskLine);
            info.AddValue("showCurrentBidLine", _showCurrentBidLine);

            info.AddValue("priceLevelPen", _priceLevelPen);

            info.AddValue("lineDrawingSource", (int)_lineDrawingSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(ISessionDataProvider dataProvider, ISourceOrderExecution orderExecutionProvider)
        {
            if (dataProvider == null)
            {
                return;
            }

            lock (this)
            {
                _dataProvider = dataProvider;
                if (_dataProvider != null)
                {
                    _dataProvider.CurrentDataBarProviderChangedEvent += new DataProviderUpdateDelegate(_dataProvider_CurrentDataBarProviderChangedEvent);
                    CurrentDataBarProvider = _dataProvider.DataBars;

                    if (_dataProvider.Quotes != null)
                    {
                        _dataProvider.Quotes.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(Quotes_QuoteUpdateEvent);
                    }
                }

                _orderExecutionProvider = orderExecutionProvider;
                if (_orderExecutionProvider != null)
                {
                    IOrderSink executor = _orderExecutionProvider;
                    if (executor != null)
                    {
                        executor.OrdersUpdatedEvent += new OrdersUpdateDelegate(executor_OrderUpdatedEvent);
                    }

                    ITradeEntityManagement management = _orderExecutionProvider.TradeEntities;
                    if (management != null)
                    {
                        management.OrdersAddedEvent += new OrderManagementOrdersUpdateDelegate(management_OrdersAddedEvent);
                        management.OrdersRemovedEvent += new OrderManagementOrdersUpdateDelegate(management_OrdersRemovedEvent);
                        management.OrdersUpdatedEvent += new OrderManagementOrdersUpdateTypeDelegate(management_OrderUpdatedEvent);
                    }
                }

                ComponentResourceManager resources = new ComponentResourceManager(typeof(ProviderTradeChartSeries));
                _imageDown = ((Image)(resources.GetObject("imageDown")));
                _imageUp = ((Image)(resources.GetObject("imageUp")));
                _imageCross = ((Image)(resources.GetObject("imageCross")));

                _buyDashedPen.DashPattern = new float[] { 5, 5 };
                _buyDashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;

                _priceLevelPen.DashPattern = new float[] { 3, 3 };
                _priceLevelPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;

                _sellDashedPen.DashPattern = new float[] { 5, 5 };
                _sellDashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            }
        }

        void Quotes_QuoteUpdateEvent(IQuoteProvider provider)
        {
            // Changing the name of the trade chart series is done in the PlatformExpertSessionControl,
            // since it requires the session status.

            RaiseSeriesValuesUpdated(true);
        }

        void _dataProvider_CurrentDataBarProviderChangedEvent(ISessionDataProvider dataProvider)
        {
            CurrentDataBarProvider = dataProvider.DataBars;
        }

        void management_OrderUpdatedEvent(ITradeEntityManagement provider, AccountInfo account, Order[] orders, Order.UpdateTypeEnum[] updatesType)
        {
            //RaiseSeriesValuesUpdated(true);
        }

        void executor_OrderUpdatedEvent(IOrderSink executor, AccountInfo account, string[] previousOrdersIds, OrderInfo[] orderInfos, Order.UpdateTypeEnum[] updatesType)
        {
            //RaiseSeriesValuesUpdated(true);
        }

        void management_OrdersAddedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            RaiseSeriesValuesUpdated(true);
        }

        void management_OrdersRemovedEvent(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> order)
        {
            RaiseSeriesValuesUpdated(true);
        }

        void DataBarHistory_DataBarHistoryUpdateEvent(IDataBarHistoryProvider provider, DataBarUpdateType updateType, int updatedBarsCount)
        {
            if (updateType != DataBarUpdateType.CurrentBarUpdate)
            {// Do the full update only on actual bar update.
                UpdateValues(updatedBarsCount);
            }
            else
            {
                RaiseSeriesValuesUpdated(true);
            }
        }

        void UpdateValues(int updateBarsCount)
        {
            IDataBarHistoryProvider provider = CurrentDataBarProvider;
            if (provider == null)
            {
                return;
            }

            lock (provider)
            {
                for (int i = provider.BarCount - 1; i >= 0 && i >= provider.BarCount - 1 - updateBarsCount; i--)
                {
                    DataBar bar = provider.BarsUnsafe[i];
                    float volume = (float)bar.Volume;
                    
                    _minVolume = Math.Min(_minVolume, (float)volume);
                    _maxVolume = Math.Max(_maxVolume, (float)volume);

                    _maxValue = Math.Max((float)bar.High, _maxValue);
                    _minValue = Math.Min((float)bar.Low, _minValue);
                }
            }

            RaiseSeriesValuesUpdated(true);
        }

        public void UnInitialize()
        {
            lock (this)
            {
                _ordersArrows.Clear();
                GeneralHelper.FireAndForget(SelectedOrderChangedEvent, _selectedOrder, null);
                _selectedOrder = null;

                CurrentDataBarProvider = null;
                if (_dataProvider != null)
                {
                    if (_dataProvider.Quotes != null)
                    {
                        _dataProvider.Quotes.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(Quotes_QuoteUpdateEvent);
                    }

                    _dataProvider.CurrentDataBarProviderChangedEvent -= new DataProviderUpdateDelegate(_dataProvider_CurrentDataBarProviderChangedEvent);
                    _dataProvider = null;
                }
                
                if (_orderExecutionProvider != null)
                {
                    IOrderSink executor = _orderExecutionProvider;
                    ITradeEntityManagement management = _orderExecutionProvider.TradeEntities;

                    if (executor != null)
                    {
                        executor.OrdersUpdatedEvent -= new OrdersUpdateDelegate(executor_OrderUpdatedEvent);
                    }

                    if (management != null)
                    {
                        management.OrdersAddedEvent -= new OrderManagementOrdersUpdateDelegate(management_OrdersAddedEvent);
                        management.OrdersRemovedEvent -= new OrderManagementOrdersUpdateDelegate(management_OrdersRemovedEvent);
                        management.OrdersUpdatedEvent -= new OrderManagementOrdersUpdateTypeDelegate(management_OrderUpdatedEvent);
                    }

                    _orderExecutionProvider = null;
                }
                
                _buyDashedPen.Dispose();
                _sellDashedPen.Dispose();
            }
        }

        protected override void OnAddedToChart()
        {
            UpdateValues(int.MaxValue);
        }

        protected override void OnRemovedFromChart()
        {
            UnInitialize();
        }

        public override void OnShowChartContextMenu(ContextMenuStrip menuStrip, ToolStripMenuItem selectedObjectsMenuItem)
        {
            base.OnShowChartContextMenu(menuStrip, selectedObjectsMenuItem);
        }

        public override void OnChartContextMenuItemClicked(ToolStripItem item)
        {
            base.OnChartContextMenuItemClicked(item);
        }

        public override bool TrySelect(System.Drawing.Drawing2D.Matrix transformationMatrix, PointF drawingSpaceSelectionPoint, float absoluteSelectionMargin, bool canLoseSelection)
        {
            if (base.TrySelect(transformationMatrix, drawingSpaceSelectionPoint, absoluteSelectionMargin, canLoseSelection))
            {
                return true;
            }

            ISourceOrderExecution provider = _orderExecutionProvider;

            List<Order> orders = new List<Order>();

            if (provider != null && provider.TradeEntities != null && _dataProvider != null)
            {// Try select orders.
                lock (provider.TradeEntities)
                {
                    orders.AddRange(provider.TradeEntities.GetOrdersBySymbol(_dataProvider.SessionInfo.Symbol));
                }
            }

            lock (this)
            {
                foreach (Order order in orders)
                {
                    if (_ordersArrows.ContainsKey(order))
                    {
                        if (_ordersArrows[order].Contains(drawingSpaceSelectionPoint))
                        {
                            GeneralHelper.FireAndForget(SelectedOrderChangedEvent, _selectedOrder, order);
                            _selectedOrder = order;
                            break;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Main drawing routine.
        /// </summary>
        public override void Draw(ChartPane managingPane, GraphicsWrapper g, int unitsUnification, 
            RectangleF clippingRectangle, float itemWidth, float itemMargin)
        {
            //TracerHelper.Trace(TracerHelper.GetCallingMethod(2).Name);

            if (Visible == false)
            {
                return;
            }

            if (_dataProvider == null)
            {
                return;
            }

            IDataBarHistoryProvider dataBarProvider = CurrentDataBarProvider;
            if (dataBarProvider == null)
            {
                return;
            }

            lock (dataBarProvider)
            {
                base.Draw(g, dataBarProvider.BarsUnsafe, unitsUnification, clippingRectangle, itemWidth, itemMargin, _maxVolume, null);
            }

            // Draw ask/bid line.
            if (_dataProvider.OperationalState == CommonSupport.OperationalStateEnum.Operational && _dataProvider.Quotes != null 
                && _dataProvider.Quotes.Bid.HasValue && _dataProvider.Quotes.Ask.HasValue)
            {
                if (_showCurrentAskLine)
                {
                    float price = (float)_dataProvider.Quotes.Ask;
                    g.DrawLine(_priceLevelPen, clippingRectangle.X, price, clippingRectangle.X + clippingRectangle.Width, price);
                }

                if (_showCurrentBidLine)
                {
                    float price = (float)_dataProvider.Quotes.Bid;
                    g.DrawLine(_priceLevelPen, clippingRectangle.X, price, clippingRectangle.X + clippingRectangle.Width, price);
                }
            }

            List<Order> ordersOpening;

            // Draw orders locations on chart.
            lock(this)
            {
                if (_orderExecutionProvider == null)
                {
                    return;
                }
            }
            // Render orders.
            ordersOpening = new List<Order>();

            ITradeEntityManagement history = _orderExecutionProvider.TradeEntities;
            if (history != null && _dataProvider != null)
            {
                lock (history)
                {
                    ordersOpening.AddRange(history.GetOrdersBySymbol(_dataProvider.SessionInfo.Symbol));
                }
            }

            // Use for orders closes.
            List<Order> ordersClosing = new List<Order>();
            foreach (Order order in ordersOpening)
            {
                if (order.State == OrderStateEnum.Closed)
                {// Only add orders already closed.
                    ordersClosing.Add(order);
                }
            }

            // This is used later on, since ordersClosing is modified.
            List<Order> ordersClosed = new List<Order>(ordersClosing);

            // TradeEntities opening at current bar.
            List<Order> pendingOpeningOrders = new List<Order>();
            // Order closing at current bar.
            List<Order> pendingClosingOrders = new List<Order>();

            PointF drawingPoint = new PointF();
            int startIndex, endIndex;
            GetDrawingRangeIndecesFromClippingRectange(clippingRectangle, drawingPoint, unitsUnification, out startIndex, out endIndex, itemWidth, itemMargin);

            lock (dataBarProvider)
            {
                float lastBarX = (itemMargin + itemWidth) * dataBarProvider.BarCount;
                for (int i = startIndex; i < endIndex && i < dataBarProvider.BarCount
                    && (ordersOpening.Count > 0 || ordersClosing.Count > 0); i++)
                {// Foreach bar, draw orders (and closeVolume).

                    while (ordersOpening.Count > 0)
                    {// All orders before now.
                        if (ordersOpening[0].OpenTime < (dataBarProvider.BarsUnsafe[i].DateTime - Period))
                        {// Order before time period.
                            if ((ordersOpening[0].State == OrderStateEnum.Executed /*||
                                ordersOpening[0].State == OrderInformation.OrderStateEnum.Submitted*/) 
                                && _showPendingOrdersTracing)
                            {// Since it is an open pending order, we shall also need to draw it as well.
                                pendingOpeningOrders.Add(ordersOpening[0]);
                            }
                            ordersOpening.RemoveAt(0);
                            continue;
                        }

                        if (ordersOpening[0].OpenTime > dataBarProvider.BarsUnsafe[i].DateTime)
                        {// Order after time period - look no further.
                            break;
                        }

                        // Order open is within the current period.
                        // Only if order is part of the current period - add to pending.
                        pendingOpeningOrders.Add(ordersOpening[0]);
                        ordersOpening.RemoveAt(0);
                    }

                    for (int j = ordersClosing.Count - 1; j >= 0; j--)
                    {
                        if (ordersClosing[j].CloseTime >= (dataBarProvider.BarsUnsafe[i].DateTime - dataBarProvider.Period) &&
                            ordersClosing[j].CloseTime <= dataBarProvider.BarsUnsafe[i].DateTime)
                        {// Order close is within the current period.
                            pendingClosingOrders.Add(ordersClosing[j]);
                            ordersClosing.RemoveAt(j);
                        }
                    }

                    drawingPoint.X = i * (itemMargin + itemWidth);
                    DrawOrders(g, i, drawingPoint, itemWidth, itemMargin, pendingOpeningOrders, pendingClosingOrders, dataBarProvider.BarsUnsafe[i], lastBarX);
                    pendingOpeningOrders.Clear();
                    pendingClosingOrders.Clear();
                }

                if (_showClosedOrdersTracing && dataBarProvider.BarCount > 0 && startIndex < dataBarProvider.BarCount)
                {// Since a closed order may be before or after (or during) the curren set of periods - make a special search and render for them.
                    endIndex = Math.Max(0, endIndex);
                    endIndex = Math.Min(dataBarProvider.BarCount - 1, endIndex);

                    foreach (Order order in ordersClosed)
                    {
                        if (order.OpenTime.HasValue 
                            && order.CloseTime.HasValue
                            && order.OpenTime.Value <= dataBarProvider.BarsUnsafe[endIndex].DateTime
                            && order.CloseTime.Value >= dataBarProvider.BarsUnsafe[startIndex].DateTime - dataBarProvider.Period)
                        {
                            int openIndex = dataBarProvider.GetIndexAtTime(order.OpenTime.Value);
                            int closeIndex = dataBarProvider.GetIndexAtTime(order.CloseTime.Value);

                            Pen pen = _buyDashedPen;
                            if (order.IsBuy == false)
                            {
                                pen = _sellDashedPen;
                            }

                            Decimal? doubleOpenValue = order.OpenPrice;
                            Decimal? doubleCloseValue = order.ClosePrice;

                            if (doubleOpenValue.HasValue == false)
                            {
                                SystemMonitor.Error("Invalid open price value for closed order to draw.");
                                continue;
                            }

                            if (doubleCloseValue.HasValue == false)
                            {
                                SystemMonitor.Error("Invalid close price value for closed order to draw.");
                                continue;
                            }

                            g.DrawLine(pen, new PointF(openIndex * (itemWidth + itemMargin), (float)doubleOpenValue),
                                new PointF(closeIndex * (itemWidth + itemMargin), (float)doubleCloseValue));
                        }
                    }
                }

            } // Lock
        }

        void DrawOrders(GraphicsWrapper g, int index, PointF drawingPoint, float itemWidth, float itemMargin, 
            List<Order> openingOrders, List<Order> closingOrders, DataBar orderBarData, float lastBarX)
        {
            // Width is same as items in real coordinates.
            float actualImageHeight = _imageDown.Height / Math.Abs(g.DrawingSpaceTransformClone.Elements[3]);

            float yToXScaling = Math.Abs(g.DrawingSpaceTransformClone.Elements[0] / g.DrawingSpaceTransformClone.Elements[3]);
            PointF updatedImageDrawingPoint = drawingPoint;
            foreach (Order order in openingOrders)
            {
                DrawOrder(g, ref updatedImageDrawingPoint, order, itemWidth, itemMargin, yToXScaling, orderBarData, lastBarX, true);
            }

            foreach (Order order in closingOrders)
            {
                DrawOrder(g, ref updatedImageDrawingPoint, order, itemWidth, itemMargin, yToXScaling, orderBarData, lastBarX, false);
            }

        }


        void DrawOrder(GraphicsWrapper g, ref PointF updatedImageDrawingPoint, Order order, float itemWidth, float itemMargin,
            float yToXScaling, DataBar orderBarData, float lastBarX, bool drawOpening)
        {
            Image image = _imageUp;
            Brush brush = Brushes.Green;
            Pen dashedPen = _buyDashedPen;
            Pen pen = Pens.GreenYellow;
            if (order.IsBuy == false)
            {
                image = _imageDown;
                brush = Brushes.Red;
                pen = Pens.Red;
                dashedPen = _sellDashedPen;
            }

            if (drawOpening == false)
            {
                image = _imageCross;
            }

            if (order.OpenPrice.HasValue == false)
            {
                SystemMonitor.OperationError("Order with no open price assigned for drawing.", TracerItem.PriorityEnum.Low);
                return;
            }

            float price = (float)order.OpenPrice.Value;
            if (drawOpening == false)
            {
                if (order.ClosePrice.HasValue == false)
                {
                    return;
                }

                price = (float)order.ClosePrice.Value;
            }

            if (drawOpening && _showPendingOrdersTracing && 
                (order is ActiveOrder && order.State == OrderStateEnum.Executed)
                && _dataProvider.Quotes.Bid.HasValue 
                && _dataProvider.Quotes.Ask.HasValue)
            {// Open orders tracking line.
                PointF point1 = new PointF(updatedImageDrawingPoint.X + itemWidth / 2f, updatedImageDrawingPoint.Y + price);
                float sellPrice = (float)_dataProvider.Quotes.Bid;
                if (order.IsBuy == false)
                {
                    sellPrice = (float)_dataProvider.Quotes.Ask;
                }
                PointF point2 = new PointF(lastBarX - itemWidth / 2f, updatedImageDrawingPoint.Y + sellPrice);
                g.DrawLine(dashedPen, point1, point2);
            }

            //if (drawOpening && _showClosedOrdersTracing && order.IsOpen == false)
            //{// Closed order tracking.
            // Close order tracing is implemented in main draw function.
            //}

            if (_showOrderSpot)
            {
                PointF basePoint = new PointF(updatedImageDrawingPoint.X, updatedImageDrawingPoint.Y + price);
                float height = (yToXScaling * itemWidth);
                if (order.IsBuy == false)
                {
                    height = -height;
                }

                if (drawOpening)
                {
                    g.FillPolygon(brush, new PointF[] { basePoint, new PointF(basePoint.X + itemWidth, basePoint.Y), 
                        new PointF(basePoint.X + (itemWidth / 2f), basePoint.Y + height) });
                    g.DrawPolygon(Pens.White, new PointF[] { basePoint, new PointF(basePoint.X + itemWidth, basePoint.Y), 
                        new PointF(basePoint.X + (itemWidth / 2f), basePoint.Y + height) });

                    float drawToLeft = (float)(1.5 * itemWidth);
                    float drawToRight = (float)(2.5 * itemWidth);

                    // Take profit level.
                    if (order.TakeProfit.HasValue
                        && order.TakeProfit.Value != 0)
                    {
                        g.DrawLine(pen, updatedImageDrawingPoint.X - drawToLeft, updatedImageDrawingPoint.Y + (float)order.TakeProfit,
                            updatedImageDrawingPoint.X + drawToRight, updatedImageDrawingPoint.Y + (float)order.TakeProfit);

                        g.DrawLine(pen, updatedImageDrawingPoint.X + itemWidth / 2f, updatedImageDrawingPoint.Y + (float)order.TakeProfit,
                            updatedImageDrawingPoint.X + itemWidth / 2f, updatedImageDrawingPoint.Y + (float)order.TakeProfit - height);
                    }

                    // Stop loss level.
                    if (order.StopLoss.HasValue
                        && order.StopLoss.Value != 0)
                    {
                        g.DrawLine(pen, updatedImageDrawingPoint.X - drawToLeft, updatedImageDrawingPoint.Y + (float)order.StopLoss,
                            updatedImageDrawingPoint.X + drawToRight, updatedImageDrawingPoint.Y + (float)order.StopLoss);

                        g.DrawLine(pen, updatedImageDrawingPoint.X + itemWidth / 2f, updatedImageDrawingPoint.Y + (float)order.StopLoss,
                            updatedImageDrawingPoint.X + itemWidth / 2f, updatedImageDrawingPoint.Y + (float)order.StopLoss + height);
                    }
                }
                else
                {
                    g.DrawRectangle(Pens.White, basePoint.X, basePoint.Y, 
                        itemWidth, yToXScaling * itemWidth);
                }

            }

            float imageHeight = 2 * (yToXScaling * itemWidth);
            if (_showOrderArrow)
            {
                float x = updatedImageDrawingPoint.X - (itemWidth / 2f);
                float y = updatedImageDrawingPoint.Y + (float)orderBarData.Low - (yToXScaling * itemWidth);
                float width = 2 * itemWidth;
                float height = -imageHeight;
                GraphicsWrapper.NormalizedRectangle(ref x, ref y, ref width, ref height);
                RectangleF rectange = new RectangleF(x, y, width, height);

                // Draw up image.
                g.DrawImage(image, rectange.X, rectange.Y, rectange.Width, rectange.Height);

                if (order == _selectedOrder)
                {// This is selected order.
                    g.DrawRectangle(Pens.White, rectange);
                }

                _ordersArrows[order] = rectange;

                updatedImageDrawingPoint.Y -= 1.2f * imageHeight;
            }

        }

        /// <summary>
        /// Override this routine, to add the price labels at the space outside of the current drawing pane.
        /// </summary>
        /// <param name="managingPane"></param>
        /// <param name="g"></param>
        public override void DrawInitialActualSpaceOverlays(ChartPane managingPane, GraphicsWrapper g)
        {
            base.DrawInitialActualSpaceOverlays(managingPane, g);

            if (_dataProvider == null || _dataProvider.OperationalState != OperationalStateEnum.Operational)
            {
                return;
            }

            decimal? ask = _dataProvider.Quotes.Ask;
            decimal? bid = _dataProvider.Quotes.Bid;

            if (ask.HasValue == false
                || bid.HasValue == false)
            {
                return;
            }

            // Measure based on a default format of 6 symbols.
            SizeF size = g.MeasureString("00.0000", managingPane.LabelsFont);

            Brush fillBrush = managingPane.LabelsFill;
            if (fillBrush == null)
            {
                fillBrush = managingPane.ActualDrawingSpaceAreaFill;
            }

            if (_showCurrentAskLine)
            {
                PointF position = managingPane.GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(0, (float)_dataProvider.Quotes.Ask), true);
                position.X = managingPane.Width - managingPane.ActualDrawingSpaceAreaMarginRight;
                position.Y -= size.Height;

                if (fillBrush != null)
                {
                    g.FillRectangle(fillBrush, new RectangleF(position.X, position.Y, size.Width, size.Height));
                }

                if (managingPane.LabelsFont != null && managingPane.LabelsFontBrush != null)
                {
                    g.DrawString(ask.Value.ToString(_dataProvider.SessionInfo.ValueFormat), managingPane.LabelsFont, managingPane.LabelsFontBrush, position);
                    g.DrawRectangle(managingPane.ActualDrawingSpaceAreaBorderPen, new RectangleF(position.X, position.Y, size.Width/* - 2*/, size.Height));
                }

            }

            if (_showCurrentBidLine)
            {
                PointF position = managingPane.GraphicsWrapper.DrawingSpaceToActualSpace(new PointF(0, (float)_dataProvider.Quotes.Bid), true);
                position.X = managingPane.Width - managingPane.ActualDrawingSpaceAreaMarginRight;

                if (fillBrush != null)
                {
                    g.FillRectangle(fillBrush, new RectangleF(position.X, position.Y, size.Width, size.Height));
                }

                if (managingPane.LabelsFont != null && managingPane.LabelsFontBrush != null)
                {
                    g.DrawString(bid.Value.ToString(_dataProvider.SessionInfo.ValueFormat), managingPane.LabelsFont, managingPane.LabelsFontBrush, position);
                    g.DrawRectangle(managingPane.ActualDrawingSpaceAreaBorderPen, new RectangleF(position.X, position.Y, size.Width/* - 2*/, size.Height));
                }
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
            if (_dataProvider == null)
            {
                return;
            }

            if (startIndex.HasValue == false && endIndex.HasValue == false)
            {// We can use the stored values to optimize performance in this case.
                minimum = _minValue;
                maximum = _maxValue;
                return;
            }

            if (startIndex.HasValue == false)
            {
                startIndex = 0;
            }

            IDataBarHistoryProvider dataBarProvider = CurrentDataBarProvider;
            if (dataBarProvider == null)
            {
                return;
            }

            if (endIndex.HasValue == false)
            {
                endIndex = dataBarProvider.BarCount;
            }

            lock (dataBarProvider)
            {
                for (int i = startIndex.Value; i < endIndex && i < dataBarProvider.BarCount; i++)
                {
                    if (dataBarProvider.BarsUnsafe[i].HasDataValues)
                    {
                        minimum = (float)Math.Min((float)dataBarProvider.BarsUnsafe[i].Low, minimum);
                        maximum = (float)Math.Max((float)dataBarProvider.BarsUnsafe[i].High, maximum);
                    }
                }
            }
        }

        public override DateTime? GetTimeAtIndex(int index)
        {
            if (_dataProvider == null)
            {
                return null;
            }

            IDataBarHistoryProvider dataBarProvider = CurrentDataBarProvider;
            if (dataBarProvider == null)
            {
                return null;
            }

            lock (dataBarProvider)
            {
                return dataBarProvider.BarsUnsafe[index].DateTime;
            }
        }

        public override void SaveToFile(string fileName)
        {
            if (_dataProvider == null)
            {
                return;
            }

            IDataBarHistoryProvider dataBarProvider = CurrentDataBarProvider;
            if (dataBarProvider == null)
            {
                return;
            }

            lock (dataBarProvider)
            {
                CSVDataBarReaderWriter reader = new CSVDataBarReaderWriter(fileName, CommonFinancial.CSVDataBarReaderWriter.DataFormat.CSVHistoricalFileDefault);
                reader.Write(dataBarProvider.BarsUnsafe);
            }
        }
        
        protected override float GetDrawingValueAt(int index, object tag)
        {
            if (_dataProvider == null)
            {
                return 0;
            }

            IDataBarHistoryProvider dataBarProvider = CurrentDataBarProvider;
            if (dataBarProvider == null)
            {
                return 0;
            }

            // The lock on the dataDelivery provider must already be on.
            return (float)dataBarProvider.BarsUnsafe[index].GetValue(_lineDrawingSource);
        }
    }
}
