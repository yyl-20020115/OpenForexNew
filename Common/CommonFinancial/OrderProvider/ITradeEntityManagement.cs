using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using CommonSupport;

namespace CommonFinancial
{

    /// <summary>
    /// Delegate for order add/remove etc.
    /// </summary>
    public delegate void OrderManagementOrdersUpdateDelegate(ITradeEntityManagement provider, AccountInfo account, IEnumerable<Order> orders);

    /// <summary>
    /// Delegate for orders updates, with update type specified.
    /// <param name="orders"></param>
    public delegate void OrderManagementOrdersUpdateTypeDelegate(ITradeEntityManagement provider, AccountInfo account, Order[] orders, Order.UpdateTypeEnum[] updatesType);

    /// <summary>
    /// Delegate for critical order modifications event.
    /// </summary>
    public delegate void OrderManagementOrderCriticalModificationDelegate(ITradeEntityManagement provider, AccountInfo account, Order order, OrderInfo updateInfo);

    /// <summary>
    /// 
    /// </summary>
    public delegate void PositionsUpdateDelegate(ITradeEntityManagement provider, AccountInfo account, IEnumerable<IPosition> positions);

    public enum OrderHistoryMode
    {
        FullAccountHistory,
        FullSymbolHistory,
        CurrentSessionOnly // In this mode, the order history only provides orders of the current session
    }
    
    /// <summary>
    /// Defines how access to existing / previous orders history / management (incl. running orders) is provided.
    /// </summary>
    public interface ITradeEntityManagement : IOperational, IDisposable
    {
        /// <summary>
        /// Is the implementor supposed to provide full order history or just orders from the current session.
        /// </summary>
        OrderHistoryMode OrderHistoryMode { get; set; }

        /// <summary>
        /// All orders. TradeEntities are expected to be provided in chronological order.
        /// Unsafe collection, before access, lock owner (ITradeEntityManagement) instance!!
        /// </summary>
        List<Order> Orders { get; }

        /// <summary>
        /// 
        /// </summary>
        List<Position> Positions { get; }

        /// <summary>
        /// TradeEntities added to management.
        /// </summary>
        event OrderManagementOrdersUpdateDelegate OrdersAddedEvent;

        /// <summary>
        /// TradeEntities removed from management.
        /// </summary>
        event OrderManagementOrdersUpdateDelegate OrdersRemovedEvent;

        /// <summary>
        /// Order was updated, see UpdateType for type of update event.
        /// Those are raised by the orders.
        /// </summary>
        event OrderManagementOrdersUpdateTypeDelegate OrdersUpdatedEvent;

        /// <summary>
        /// Event occurs when critical order information (like Open/Close price) has been 
        /// changed trough an update, indicating there is a glitch in some order management
        /// mechanism (this can occur, when a broker changes orders prices, and is here to
        /// monitor for this, not errors in the system).
        /// </summary>
        event OrderManagementOrderCriticalModificationDelegate OrdersCriticalInformationChangedEvent;

        /// <summary>
        /// TradeEntities added to management.
        /// </summary>
        event PositionsUpdateDelegate PositionsAddedEvent;

        /// <summary>
        /// TradeEntities removed from management.
        /// </summary>
        event PositionsUpdateDelegate PositionsRemovedEvent;

        /// <summary>
        /// Order was updated, see UpdateType for type of update event.
        /// Those are raised by the orders.
        /// </summary>
        event PositionsUpdateDelegate PositionsUpdatedEvent;

        /// <summary>
        /// Start the management for operation.
        /// </summary>
        /// <returns></returns>
        bool Initialize();

        /// <summary>
        /// 
        /// </summary>
        void UnInitialize();

        /// <summary>
        /// 
        /// </summary>
        Position GetPositionBySymbol(Symbol symbol, bool allowNameMatchOnly);

        /// <summary>
        /// 
        /// </summary>
        Position ObtainPositionBySymbol(Symbol symbol);

        /// <summary>
        /// 
        /// </summary>
        bool AddPosition(Position position);

        /// <summary>
        /// 
        /// </summary>
        bool RemovePosition(Position position);

        /// <summary>
        /// Helper, returns an order based on its execution platform Id. Usefull for components
        /// that need to persist and recognize orders over restarts.
        /// </summary>
        Order GetOrderById(string id);

        /// <summary>
        /// 
        /// </summary>
        Order[] GetOrdersBySymbol(Symbol symbol);

        /// <summary>
        /// Add order to orders.
        /// </summary>
        void AddOrders(IEnumerable<Order> order);

        /// <summary>
        /// Add single order to orders management.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool AddOrder(Order order);

        /// <summary>
        /// Remove order from management.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        void RemoveOrders(IEnumerable<Order> order);

        /// <summary>
        /// Remove single order from management.
        /// </summary>
        bool RemoveOrder(Order order);

        /// <summary>
        /// Is the given order part of this management.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool ContainsOrder(Order order);

        /// <summary>
        /// Get orders based on their current state. Gets all orders with given state.
        /// The baseMethod is *unsafe* so to iterate the returned result, lock the IOrderManager instance.
        /// </summary>
        ReadOnlyCollection<Order> GetOrdersByStateUnsafe(OrderStateEnum state);

        /// <summary>
        /// Result is safe, so can be manipulated as needed without need of additional locking.
        /// </summary>
        List<Order> GetOrdersByState(OrderStateEnum state);
    }
}
