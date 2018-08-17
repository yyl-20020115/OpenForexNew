using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Class takes care of holding orders and keeping track of their state and events.
    /// </summary>
    [Serializable]
    public sealed class TradeEntityKeeper : Operational, ITradeEntityManagement
    {
        [NonSerialized]
        Dictionary<string, Order> _orders = new Dictionary<string, Order>();

        [NonSerialized]
        Dictionary<OrderStateEnum, ListUnique<Order>> _ordersByState = new Dictionary<OrderStateEnum, ListUnique<Order>>();

        [NonSerialized]
        Dictionary<Symbol, ListUnique<Order>> _ordersBySymbol = new Dictionary<Symbol, ListUnique<Order>>();

        [NonSerialized]
        Dictionary<Symbol, Position> _positions = new Dictionary<Symbol, Position>();

        volatile OrderHistoryMode _orderHistoryMode = OrderHistoryMode.FullSymbolHistory;

        /// <summary>
        /// Mode of operation, what orders from history are delivered and what are not.
        /// </summary>
        public OrderHistoryMode OrderHistoryMode
        {
            get { return _orderHistoryMode; }
            set { _orderHistoryMode = value; }
        }

        ISourceManager _manager;

        ISourceOrderExecution _provider;

        ISourceDataDelivery _delivery;

        /// <summary>
        /// 
        /// </summary>
        public List<Order> Orders
        {
            get 
            {
                lock (this)
                {
                    return GeneralHelper.EnumerableToList<Order>(_orders.Values);
                }
            }
        }

        /// <summary>
        /// Thread safe call, result is a copy and may be manipulated as needed.
        /// </summary>
        public List<Position> Positions
        {
            get 
            {
                lock (this)
                {
                    return GeneralHelper.EnumerableToList(_positions.Values);
                }
            }
        }

        //public IEnumerable<Position> PositionsUnsafe
        //{
        //    get { return _positions.Values; }
        //}


        [field: NonSerialized]
        public event OrderManagementOrdersUpdateDelegate OrdersAddedEvent;

        [field: NonSerialized]
        public event OrderManagementOrdersUpdateDelegate OrdersRemovedEvent;

        [field: NonSerialized]
        public event OrderManagementOrdersUpdateTypeDelegate OrdersUpdatedEvent;

        [field: NonSerialized]
        public event OrderManagementOrderCriticalModificationDelegate OrdersCriticalInformationChangedEvent;

        [field: NonSerialized]
        public event PositionsUpdateDelegate PositionsAddedEvent;

        [field: NonSerialized]
        public event PositionsUpdateDelegate PositionsRemovedEvent;

        [field: NonSerialized]
        public event PositionsUpdateDelegate PositionsUpdatedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TradeEntityKeeper()
        {
            ChangeOperationalState(OperationalStateEnum.Constructed);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool SetInitialParameters(ISourceManager manager, ISourceOrderExecution provider,
            ISourceDataDelivery delivery)
        {
            //SystemMonitor.CheckError(_provider == null, "OrderExecutionProvider already assigned.");

            _manager = manager;
            _provider = provider;
            _delivery = delivery;

            if (_provider == null)
            {
                SystemMonitor.Warning("Failed to properly initialize entity keeper.");
            }

            return true;
        }

        /// <summary>
        /// Main initialization point.
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            if (_manager == null)
            {
                return false;
            }

            lock (this)
            {
                foreach (OrderStateEnum state in Enum.GetValues(typeof(OrderStateEnum)))
                {
                    _ordersByState[state] = new ListUnique<Order>();
                }
            }

            _provider.OrdersUpdatedEvent += new OrdersUpdateDelegate(_executor_OrderUpdatedEvent);
            _provider.PositionsUpdateEvent += new PositionUpdateDelegate(_provider_PositionsUpdateEvent);

            ChangeOperationalState(OperationalStateEnum.Operational);

            return true;
        }

        void _provider_PositionsUpdateEvent(IOrderSink provider, AccountInfo accountInfo, PositionInfo[] positionsInfos)
        {
            foreach (PositionInfo info in positionsInfos)
            {
                if (info.Symbol.IsEmpty)
                {
                    SystemMonitor.Warning("Empty symboled position.");
                    continue;
                }

                Position position = ObtainPositionBySymbol(info.Symbol);
                if (position != null)
                {
                    position.UpdateInfo(info);
                }
            }
        }

        void _executor_OrderUpdatedEvent(IOrderSink providerSink, AccountInfo accountInfo, string[] previousOrdersIds, 
            OrderInfo[] ordersInfos, Order.UpdateTypeEnum[] updatesType)
        {
            ISourceOrderExecution provider = _provider;
            
            if (providerSink != _provider)
            {
                SystemMonitor.Warning("Provider mismatch.");
                return;
            }

            List<Order> updatedOrders = new List<Order>();
            List<Order.UpdateTypeEnum> updatedOrdersUpdateTypes = new List<Order.UpdateTypeEnum>();

            for (int i = 0; i < ordersInfos.Length; i++)
            {
                if (string.IsNullOrEmpty(ordersInfos[i].Id))
                {
                    SystemMonitor.Warning("Order update of order with no ID.");
                    continue;
                }

                if (previousOrdersIds.Length > i && previousOrdersIds[i] != ordersInfos[i].Id 
                    && string.IsNullOrEmpty(previousOrdersIds[i]) == false)
                {// Order Id has changed, remove old order.
                    Order superceededOrder = GetOrderById(previousOrdersIds[i]);
                    RemoveOrder(superceededOrder);
                }

                Order order = GetOrderById(ordersInfos[i].Id);
                if (order == null)
                {// Create new order based on incoming information.

                    if (provider.SupportsActiveOrderManagement)
                    {
                        order = new ActiveOrder(_manager, provider, _delivery.SourceId, true);
                    }
                    else
                    {
                        order = new PassiveOrder(_manager, _delivery.SourceId, provider.SourceId);
                    }

                    order.AdoptInfo(ordersInfos[i]);

                    if (AddOrder(order) == false)
                    {
                        SystemMonitor.OperationError("Failed to add order to keeper (id [" + order.Id + "] already used for another order).", TracerItem.PriorityEnum.Medium);
                    }
                }
                else
                {// Existing order, to be updated.

                    OrderInfo info = ordersInfos[i];

                    // First, check for critical modifications (price changes).
                    if (order.Info.IsCriticalUpdate(info))
                    {
                        SystemMonitor.Report(string.Format("Order has received a critical data modication Id[{0}], Open[{1} / {2}], Close[{3} / {4}].", order.Id, order.OpenPrice.ToString(), info.OpenPrice.ToString(), order.ClosePrice.ToString(),
                            info.ClosePrice.ToString()), TracerItem.PriorityEnum.High);

                        if (OrdersCriticalInformationChangedEvent != null)
                        {
                            OrdersCriticalInformationChangedEvent(this, accountInfo, order, info);
                        }
                    }

                    if (order.UpdateInfo(info) == false)
                    {
                        SystemMonitor.OperationError("Failed to update order [" + order.Id + "].");
                        continue;
                    }

                    lock (this)
                    {
                        // Remove from any of the sub arrays it may be in.
                        foreach (OrderStateEnum state in Enum.GetValues(typeof(OrderStateEnum)))
                        {
                            if (_ordersByState.ContainsKey(state) && _ordersByState[state].Contains(order))
                            {
                                _ordersByState[state].Remove(order);
                            }
                        }

                        _ordersByState[info.State].Add(order);
                    }

                    updatedOrders.Add(order);
                    updatedOrdersUpdateTypes.Add(updatesType[i]);
                }
            }

            if (updatedOrders.Count > 0 && OrdersUpdatedEvent != null)
            {
                OrdersUpdatedEvent(this, accountInfo, updatedOrders.ToArray(), updatedOrdersUpdateTypes.ToArray() );
            }
        }

        public void UnInitialize()
        {
            lock (this)
            {
                _orders.Clear();

                _ordersBySymbol.Clear();
                _ordersByState.Clear();
            }

            ChangeOperationalState(OperationalStateEnum.UnInitialized);
        }

        /// <summary>
        /// 
        /// </summary>
        public Order[] GetOrdersBySymbol(Symbol symbol)
        {
            lock (this)
            {
                if (_ordersBySymbol.ContainsKey(symbol))
                {
                    return _ordersBySymbol[symbol].ToArray();
                }
            }

            return new Order[] { };
        }

        /// <summary>
        /// Returns an unsafe collection of orders from this state.
        /// </summary>
        public ReadOnlyCollection<Order> GetOrdersByStateUnsafe(OrderStateEnum state)
        {
            lock (this)
            {
                if (_ordersByState.ContainsKey(state) == false)
                {
                    return null;

                }
                return _ordersByState[state].AsReadOnly();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Order> GetOrdersByState(OrderStateEnum state)
        {
            List<Order> result = new List<Order>();
            lock (this)
            {
                foreach (OrderStateEnum orderState in Enum.GetValues(typeof(OrderStateEnum)))
                {
                    OrderStateEnum filteredState = orderState & state;
                    if (filteredState == orderState && _ordersByState.ContainsKey(orderState))
                    {
                        result.AddRange(_ordersByState[orderState]);
                    }
                }
            }

            return result;
        }

        #region IDeserializationCallback Members

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            _orders = new Dictionary<string, Order>();
            _ordersByState = new Dictionary<OrderStateEnum, ListUnique<Order>>();
            _ordersBySymbol = new Dictionary<Symbol, ListUnique<Order>>();
            _positions = new Dictionary<Symbol, Position>();

            ChangeOperationalState(OperationalStateEnum.Constructed);
        }

        #endregion

        void order_OrderUpdatedEvent(Order order, Order.UpdateTypeEnum updateType)
        {
            if (OrdersUpdatedEvent != null)
            {
                OrdersUpdatedEvent(this, _provider.DefaultAccount.Info, new Order[] { order }, new Order.UpdateTypeEnum[] { updateType });
            }
        }

        void position_UpdateEvent(IPosition position)
        {
            if (PositionsUpdatedEvent != null && _provider.DefaultAccount != null)
            {
                PositionsUpdatedEvent(this, _provider.DefaultAccount.Info, new IPosition[] { position });
            }
        }

        /// <summary>
        /// Check for all orders if order is in this management.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool ContainsOrder(Order order)
        {
            lock (this)
            {
                return _orders.ContainsValue(order);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AddOrder(Order order)
        {
            AddOrders(new Order[] { order });
            return true;
        }

        /// <summary>
        /// Add orders.
        /// </summary>
        public void AddOrders(IEnumerable<Order> orders)
        {
            //foreach (Order order in orders)
            //{
            //    lock (this)
            //    {
            //        if (string.IsNullOrEmpty(order.Id) == false &&
            //            (_orders.ContainsKey(order.Id) || _orders.ContainsValue(order)))
            //        {
            //            SystemMonitor.OperationWarning("Can not add order [" + order.Id + "] since (order or id) already added.");
            //            //return false;
            //        }
            //    }
            //}
            
            foreach (Order order in orders)
            {
                if (string.IsNullOrEmpty(order.Id))
                {
                    SystemMonitor.Warning("Adding an order with no Id, order skipped.");
                    continue;
                }

                if (order.Symbol.IsEmpty)
                {
                    SystemMonitor.Warning("Adding an order with no Symbol assigned, order skipped.");
                    continue;
                }

                lock (this)
                {
                    if (_orders.ContainsKey(order.Id))
                    {
                        continue;
                    }

                    _orders.Add(order.Id, order);
                    if (_ordersByState.ContainsKey(order.State))
                    {
                        _ordersByState[order.State].Add(order);
                    }
                    else
                    {
                        SystemMonitor.Error("State for this order [" + order.State + "] not found.");
                    }

                    if (_ordersBySymbol.ContainsKey(order.Symbol) == false)
                    {
                        _ordersBySymbol.Add(order.Symbol, new ListUnique<Order>());
                    }

                    if (_ordersBySymbol.ContainsKey(order.Symbol))
                    {
                        _ordersBySymbol[order.Symbol].Add(order);
                    }
                    else
                    {
                        SystemMonitor.Error("Order symbol not in list [" + order.Symbol + "] not found.");
                    }
                }

                order.OrderUpdatedEvent += new Order.OrderUpdatedDelegate(order_OrderUpdatedEvent);

                // Make sure there is a position for this order.
                ObtainPositionBySymbol(order.Symbol);
            }

            if (OrdersAddedEvent != null)
            {
                if (_provider.DefaultAccount == null)
                {
                    SystemMonitor.OperationError("Orders update arrived too soon.", TracerItem.PriorityEnum.Medium);
                }
                else
                {
                    OrdersAddedEvent(this, _provider.DefaultAccount.Info, orders);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemoveOrder(Order order)
        {
            RemoveOrders(new Order[] { order });
            return true;
        }

        /// <summary>
        /// Remove order from history.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public void RemoveOrders(IEnumerable<Order> orders)
        {
            lock (this)
            {
                foreach (Order order in orders)
                {
                    if (_orders.Remove(order.Id))
                    {
                        order.OrderUpdatedEvent -= new Order.OrderUpdatedDelegate(order_OrderUpdatedEvent);

                        // Remove from any of the sub arrays it may be in.
                        foreach (OrderStateEnum state in Enum.GetValues(typeof(OrderStateEnum)))
                        {
                            _ordersByState[state].Remove(order);
                        }

                        if (order.Symbol.IsEmpty == false && _ordersBySymbol.ContainsKey(order.Symbol))
                        {
                            _ordersBySymbol[order.Symbol].Remove(order);
                        }
                    }
                }
            }

            if (OrdersRemovedEvent != null)
            {
                OrdersRemovedEvent(this, _provider.DefaultAccount.Info, orders);
            }
        }

        public Order GetOrderById(string id)
        {
            if (string.IsNullOrEmpty(id) || OperationalState != OperationalStateEnum.Operational)
            {
                return null;
            }

            Order order = null;
            lock (this)
            {
                if (_orders.ContainsKey(id))
                {
                    order = _orders[id];
                }
            }

            if (order != null)
            {
                SystemMonitor.CheckError(order.Id == id, "Mismatch in order Id containment.");
            }

            return order;
        }

        /// <summary>
        /// Get existing or create a new position.
        /// </summary>
        public Position ObtainPositionBySymbol(Symbol symbol)
        {
            Position position = GetPositionBySymbol(symbol, true);
            if (position == null)
            {
                if (_delivery == null)
                {
                    SystemMonitor.Warning("Failed to get required delivery to compose a position.");
                    return null;
                }

                if (_provider.SupportsActiveOrderManagement)
                {
                    position = new ActivePosition();
                }
                else
                {
                    position = new PassivePosition();
                }

                if (position.SetInitialParameters(_manager, _provider, _delivery, symbol) == false)
                {
                    SystemMonitor.Warning("Failed to create a position due to set initial error.");
                    return null;
                }

                if (position.Initialize() == false)
                {
                    SystemMonitor.Warning("Failed to initialize a position.");
                    return null;
                }

                if (AddPosition(position) == false)
                {// For some reason adding failed, so dispose the current position
                    // and try again to return one, maybe it was added in the mean time.
                    position.UnInitialize();
                    position.Dispose();
                    position = null;
                    
                    return GetPositionBySymbol(symbol, true);
                }
            }

            return position;
        }


        /// <summary>
        /// 
        /// </summary>
        public Position GetPositionBySymbol(Symbol symbol, bool allowNameMatchOnly)
        {
            lock (this)
            {
                if (_positions.ContainsKey(symbol))
                {
                    return _positions[symbol];
                }

                if (allowNameMatchOnly)
                {
                    foreach (Symbol symbolItem in _positions.Keys)
                    {
                        if (symbol.Name == symbolItem.Name)
                        {
                            return _positions[symbolItem];
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AddPosition(Position position)
        {
            if (position.Symbol.IsEmpty || string.IsNullOrEmpty(position.Symbol.Name))
            {
                SystemMonitor.Warning("Added a position with empty symbol.");
                return false;
            }

            lock (this)
            {
                if (_positions.ContainsKey(position.Symbol))
                {
                    SystemMonitor.Warning("Added a position, but symbol already has a position.");
                    return false;
                }

                _positions.Add(position.Symbol, position);
            }

            position.UpdateEvent += new UpdateDelegate(position_UpdateEvent);

            if (PositionsAddedEvent != null)
            {
                if (_provider == null || _provider.DefaultAccount == null)
                {
                    SystemMonitor.Error("Provider or account not yet assigned, position add event not launched.");
                }
                else
                {
                    PositionsAddedEvent(this, _provider.DefaultAccount.Info, new Position[] { position });
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemovePosition(Position position)
        {
            lock (this)
            {
                if (_positions.Remove(position.Symbol))
                {
                    position.UpdateEvent -= new UpdateDelegate(position_UpdateEvent);
                    return true;    
                }
            }

            return false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _manager = null;
            _provider = null;
            _delivery = null;
        }

        #endregion

    }
}
