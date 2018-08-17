using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    
    // MT4 interpretation of stop and limit orders.
    // ----
    // BUY STOP / SELL LIMIT
    // MARKET
    // BUY LIMIT / SELL STOP
    // ----

    /// <summary>
    /// Enum of order types. ActiveOrder type specifies the buy/sell order direction, as well as 
    /// wether to execute the order immediately (BUY_MARKET/SELL_MARKET) or
    /// perform delayed execution at a certain level.
    /// </summary>
    public enum OrderTypeEnum
    {// IMPORTANT: Do not change the values of those since they are matching the MT4 values !!!
        BUY_MARKET = 0, // MT4 variable name OP_BUY
        SELL_MARKET = 1, // MT4 variable name OP_SELL
        
        BUY_LIMIT_MARKET = 2, // Delayed order, buy limit.
        SELL_LIMIT_MARKET = 3, // Delayed order, sell limit.

        // STOP Orders
        // Buy or sell at market once the price reaches or passes through a specified price. 
        // Used by traders who either have a position (long or short) and want to close the 
        // position if it moves against them OR by traders who wish to open a new position once 
        // the currency rises to a specific level. The stop price on a sell stop must be below 
        // the current bid. The stop price on a buy stop must be above the current offer. Stop 
        // orders do not guarantee you an execution at or near the stop price. Once triggered, 
        // the order competes with other incoming market orders.
        // Example: This order type is used mostly for protection. 
        // If we are long the EUR/USD at 1.1888, our concern is to not lose more than 10 pips 
        // to the downside (Pending trading strategy used). So we would enter a sell stop market 
        // order with a stop price of 1.1878
        BUY_STOP_MARKET = 4, // Delayed order, buy stop
        SELL_STOP_MARKET = 5, // Delayed order, sell stop

        UNKNOWN = 9999 // Unknown order type (MT4 is not using this).
    }

    /// <summary>
    /// State of the order, moves from one state to another in its lifecycle.
    /// A state can only have one state at one moment, however the flags are usefull for quieries.
    /// </summary>
    public enum OrderStateEnum
    {
        UnInitialized = 1, // Not initialized.
        Initialized = 2, // Initialized, ready for placement.
        Submitted = 4, // Submitted, Open, placed and pending (delayed order) and waiting to be remotely executed upon conditions met.
        Suspended = 8, // Submitted but suspended (for ex. due to session not active).
        Executed = 16, // Executed, acknowledged.
        Closed = 32, // Closed.
        Canceled = 64, // Canceled.
        Failed = 128, // Placing order failed.
        Unknown = 256 // State not known.
    }

    /// <summary>
    /// Struct contains the core information of an order to a broker.
    /// </summary>
    [Serializable]
    public struct OrderInfo : IComparable<OrderInfo>
    {
        volatile string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        volatile OrderTypeEnum _type;
        public OrderTypeEnum Type
        {
            get { return _type; }
            set { _type = value; }
        }

        volatile OrderStateEnum _state;
        public OrderStateEnum State
        {
            get { return _state; }
            set { _state = value; }
        }

        volatile int _volume;
        public int Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        public int DirectionalVolume
        {
            get 
            {
                if (IsBuy)
                {
                    return _volume;
                }
                else
                {
                    return -_volume;
                }
            }
        }

        Decimal? _openPrice;
        public Decimal? OpenPrice
        {
            get { return _openPrice; }
            set { _openPrice = value; }
        }

        Decimal? _closePrice;
        public Decimal? ClosePrice
        {
            get { return _closePrice; }
            set { _closePrice = value; }
        }

        Decimal? _stopLoss;
        public Decimal? StopLoss
        {
            get { return _stopLoss; }
            set { _stopLoss = value; }
        }

        Decimal? _takeProfit;
        public Decimal? TakeProfit
        {
            get { return _takeProfit; }
            set { _takeProfit = value; }
        }

        /// <summary>
        /// Not including swaps or commissions.
        /// </summary>
        Decimal? _currentProfit;
        public Decimal? CurrentProfit
        {
            get { return _currentProfit; }
        }

        Decimal? _swap;
        public Decimal? Swap
        {
            get { return _swap; }
        }

        Symbol _symbol;
        /// <summary>
        /// Equity symbol for this order.
        /// </summary>
        public Symbol Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        DateTime? _placeTime;
        /// <summary>
        /// The date time the order was placed (submitted and accepted by execution system).
        /// </summary>
        public DateTime? PlaceTime
        {
            get { return _placeTime; }
            set { _placeTime = value; }
        }

        DateTime? _openTime;
        /// <summary>
        /// The date time the order was opened (executed).
        /// </summary>
        public DateTime? OpenTime
        {
            get { return _openTime; }
            set { _openTime = value; }
        }

        DateTime? _closeTime;
        /// <summary>
        /// The date time the order was closed.
        /// </summary>
        public DateTime? CloseTime
        {
            get { return _closeTime; }
            set { _closeTime = value; }
        }

        DateTime? _expiration;
        /// <summary>
        /// Expiration time for pending orders.
        /// </summary>
        public DateTime? Expiration
        {
            get { return _expiration; }
        }

        Decimal? _commission;
        /// <summary>
        /// Current order commission.
        /// </summary>
        public Decimal? Commission
        {
            get { return _commission; }
        }

        volatile string _comment;
        /// <summary>
        /// Comment assigned to the order.
        /// </summary>
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        volatile string _tag;
        /// <summary>
        /// A.K.A Magic number; usually used to track orders in execution systems.
        /// </summary>
        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// Is a Buy order.
        /// </summary>
        public bool IsBuy
        {
            get
            {
                return TypeIsBuy(Type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSell
        {
            get { return !IsBuy; }
        }

        /// <summary>
        /// Is this a delayed type of order. Delayed orders are not executed immediately,
        /// but on achieving a certain condition (price, time etc.)
        /// </summary>
        public bool IsDelayed
        {
            get
            {
                return TypeIsDelayed(Type);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OrderInfo(string orderId)
        {
            _id = orderId;
            _type = OrderTypeEnum.UNKNOWN;
            _state = OrderStateEnum.Unknown;

            _symbol = Symbol.Empty;

            _volume = 0;
            _openPrice = null;
            _closePrice = null;
            _stopLoss = null;
            _takeProfit = null;
            _currentProfit = null;
            _swap = null;
            _openTime = null;
            _placeTime = null;

            _closeTime = null;
            _expiration = null;
            _commission = null;
            _comment = null;
            _tag = null;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public OrderInfo(string orderId, Symbol symbol, OrderTypeEnum orderType, OrderStateEnum state, int volume)
        {
            _id = orderId;
            _symbol = symbol;
            _type = orderType;
            _volume = volume;
            _openPrice = null;
            _closePrice = null;
            _stopLoss = null;
            _takeProfit = null;
            _currentProfit = null;
            _swap = null;
            _openTime = null;
            _placeTime = null;

            _state = state;

            _closeTime = null;
            _expiration = null;
            _commission = null;
            _comment = null;
            _tag = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OrderInfo(string orderId, Symbol symbol, OrderTypeEnum orderType, OrderStateEnum state, int volume, Decimal? openPrice, Decimal? closePrice,
            Decimal? orderStopLoss, Decimal? orderTakeProfit, Decimal? currentProfit, Decimal? orderSwap, DateTime? orderPlatformOpenTime, DateTime? orderPlatformCloseTime,
            DateTime? orderPlatformPlaceTime, DateTime? orderExpiration, Decimal? orderCommission, string orderComment, string tag)
        {
            _id = orderId;
            _symbol = symbol;
            _type = orderType;
            _volume = volume;
            _openPrice = openPrice;
            _closePrice = closePrice;
            _stopLoss = orderStopLoss;
            _takeProfit = orderTakeProfit;
            _currentProfit = currentProfit;
            _swap = orderSwap;
            _openTime = orderPlatformOpenTime;
            _placeTime = orderPlatformPlaceTime;

            _state = state;

            _closeTime = orderPlatformCloseTime;
            _expiration = orderExpiration;
            _commission = orderCommission;
            _comment = orderComment;
            _tag = tag;
        }

        /// <summary>
        /// Print order details to a string.
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            return "orderTicket:[" + _id + "] " +
            "orderSymbol:[" + _symbol.Name + "] " +
            "orderType:[" + _type + "] " +
            "volume:[" + _volume + "] " +
            "openPrice:[" + _openPrice + "] " +
            "closePrice:[" + _closePrice + "] " +
            "orderStopLoss:[" + _stopLoss + "] " +
            "orderTakeProfit:[" + _takeProfit + "] " +
            "currentProfit:[" + _currentProfit + "] " +
            "orderSwap:[" + _swap + "] " +
            "platformOpenTime :[" + _openTime + "] " +
            "platformCloseTime :[" + _closeTime + "] " +
            "orderExpiration :[" + _expiration + "] " +
            "orderCommission:[" + _commission + "] " +
            "orderComment:[" + _comment + "] " +
            "tag:[" + _tag + "]";
        }

        /// <summary>
        /// Check if given order type is buy (or sell).
        /// </summary>
        public static bool TypeIsBuy(OrderTypeEnum orderType)
        {
            SystemMonitor.CheckOperationWarning(orderType != OrderTypeEnum.UNKNOWN, "Order type not established yet.");
            return orderType == OrderTypeEnum.BUY_MARKET || orderType == OrderTypeEnum.BUY_LIMIT_MARKET || orderType == OrderTypeEnum.BUY_STOP_MARKET;
        }

        /// <summary>
        /// Is this order type delayed, otherwise it is for instant execution.
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static bool TypeIsDelayed(OrderTypeEnum orderType)
        {
            SystemMonitor.CheckWarning(orderType != OrderTypeEnum.UNKNOWN, "Order type not established yet.");
            return orderType != OrderTypeEnum.BUY_MARKET
                && orderType != OrderTypeEnum.SELL_MARKET;
        }

        /// <summary>
        /// Determines if the incoming order info is a critical update as compared to the current one.
        /// Critical updates occur when an order has been executed at a price, and later on the price is changed.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsCriticalUpdate(OrderInfo other)
        {
            if (this.State != OrderStateEnum.Executed || other.State != OrderStateEnum.Executed)
            {// Critical can only occur when an executed order has changed price.
                return false;
            }

            return (((OpenPrice.HasValue && other.OpenPrice.HasValue && OpenPrice.Value != other.OpenPrice.Value)
                    || (ClosePrice.HasValue && other.ClosePrice.HasValue && ClosePrice.Value != other.ClosePrice.Value)));
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Update(OrderInfo updateInfo)
        {
            if (updateInfo.Id != _id 
                || (updateInfo.Symbol.Name != _symbol.Name)
                // Can not filter type here, since type may change (for ex. the server may accept market orders as Market_Limit)
                /*|| (updateInfo.Type != OrderTypeEnum.UNKNOWN && updateInfo.Type != Type)*/)
            {
                SystemMonitor.Warning("Mismatching order basic parameters.");
                return false;
            }

            _volume = updateInfo._volume;
            _state = updateInfo._state;

            if (updateInfo._openPrice.HasValue)
            {
                _openPrice = updateInfo._openPrice;
            }

            if (updateInfo._closePrice.HasValue)
            {
                _closePrice = updateInfo._closePrice;
            }

            //if (updateInfo._stopLoss.HasValue)
            {
                _stopLoss = updateInfo._stopLoss;
            }

            //if (updateInfo._takeProfit.HasValue)
            {
                _takeProfit = updateInfo._takeProfit;
            }

            if (updateInfo._currentProfit.HasValue)
            {
                _currentProfit = updateInfo._currentProfit;
            }

            if (updateInfo._swap.HasValue)
            {
                _swap = updateInfo._swap;
            }

            if (updateInfo._closeTime.HasValue)
            {
                _closeTime = updateInfo._closeTime;
            }

            if (updateInfo._openTime.HasValue)
            {
                _openTime = updateInfo._openTime;
            }

            if (updateInfo._expiration.HasValue)
            {
                _expiration = updateInfo._expiration;
            }

            if (updateInfo._commission.HasValue)
            {
                _commission = updateInfo._commission;
            }

            if (updateInfo._comment != null)
            {
                _comment = updateInfo._comment;
            }

            if (updateInfo._tag != null)
            {
                _tag = updateInfo._tag;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public int CompareTo(OrderInfo other)
        {
            return _id.CompareTo(other.Id);
        }

    }
}
