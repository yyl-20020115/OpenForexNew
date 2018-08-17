using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Threading;

namespace CommonFinancial
{
    /// <summary>
    /// Contains statistics regarding the overall performance results of a given Account.
    /// Usefull when many orders are placed to an account to evaluate the qualities of the 
    /// strategy that placed those orders.
    /// </summary>
    [Serializable]
    public class AccountStatistics : IDisposable
    {
        [NonSerialized]
        Account _account;

        object _syncRoot = new object();

        public bool IsInitialized
        {
            get { return _account != null; }
        }

        DateTime? _firstOrderTime = null;
        public DateTime? FirstOrderTime
        {
            get { return _firstOrderTime; }
        }

        DateTime? _lastOrderTime = null;
        public DateTime? LastOrderTime
        {
            get { return _lastOrderTime; }
        }

        Decimal? _initialEquity = null;
        public Decimal? InitialEquity
        {
            get { return _initialEquity; }
        }

        Decimal? _performancePercentage = null;
        /// <summary>
        /// Can be negative.
        /// </summary>
        public Decimal? PerformancePercentage
        {
            get { return _performancePercentage; }
        }

        Decimal? _overalDrawDownPercentage = null;
        /// <summary>
        /// Relative to the initial equity.
        /// </summary>
        public Decimal? OveralDrawDownPercentage
        {
            get { return _overalDrawDownPercentage; }
        }

        Decimal? _largestDrawDownPercentage = null;
        
        /// <summary>
        /// Relative to the maximum result achieved.
        /// </summary>
        public Decimal? LargestDrawDownPercentage
        {
            get { return _largestDrawDownPercentage; }
        }


        Decimal? _bestPerformancePercentage = null;
        /// <summary>
        /// Can be negative.
        /// </summary>
        public Decimal? BestPerformancePercentage
        {
            get { return _bestPerformancePercentage; }
        }

        Decimal? _worstPerformancePercentage = null;
        /// <summary>
        /// Can be negative.
        /// </summary>
        public Decimal? WorstPerformancePercentage
        {
            get { return _worstPerformancePercentage; }
        }

        Decimal? _maxEquity = null;
        public Decimal? MaxEquity
        {
            get { return _maxEquity; }
        }

        Decimal? _minEquity = null;
        public Decimal? MinEquity
        {
            get { return _minEquity; }
        }

        int _totalTrades = 0;
        public int TotalTrades
        {
            get { return _totalTrades; }
        }

        int _buyTrades = 0;
        public int BuyTrades
        {
            get { return _buyTrades; }
        }

        int _sellTrades = 0;
        public int SellTrades
        {
            get { return _sellTrades; }
        }

        int _winningTrades = 0;
        public int WinningTrades
        {
            get { return _winningTrades; }
        }

        int _losingTrades = 0;
        public int LosingTrades
        {
            get { return _losingTrades; }
        }

        Decimal _profitOverall = 0;
        public Decimal ProfitOverall
        {
            get
            {
                return _profitOverall;
            }
        }

        Decimal _winnersProfit = 0;
        public Decimal WinnersProfit
        {
            get { return _winnersProfit; }
        }

        Decimal _losersLoss = 0;
        public Decimal LosersLoss
        {
            get { return _losersLoss; }
        }

        Decimal? _biggestWinner = null;
        public Decimal? BiggestWinner
        {
            get { return _biggestWinner; }
        }

        Decimal? _biggestLoser = null;
        public Decimal? BiggestLoser
        {
            get { return _biggestLoser; }
        }

        int _maxConsecutiveWinners = 0;
        public int MaxConsecutiveWinners
        {
            get { return _maxConsecutiveWinners; }
        }

        int _currentConsecutiveWinners = 0;
        public int CurrentConsecutiveWinners
        {
            get { return _currentConsecutiveWinners; }
        }

        int _maxConsecutiveLosers = 0;
        public int MaxConsecutiveLosers
        {
            get { return _maxConsecutiveLosers; }
        }

        int _currentConsecutiveLosers = 0;
        public int CurrentConsecutiveLosers
        {
            get { return _currentConsecutiveLosers; }
        }

        Dictionary<DateTime, Decimal> _equityHistory;

        public float[] EquityHistoryValues
        {
            get
            {
                lock (_syncRoot)
                {
                    return GeneralHelper.DecimalsToFloats(_equityHistory.Values, _equityHistory.Count);
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountStatistics()
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_equityHistory != null)
                {
                    _equityHistory.Clear();
                    _equityHistory = null;
                }
            }
        }

        #endregion

        public bool Initialize(Account account)
        {
            SystemMonitor.CheckWarning(IsInitialized == false, "Instance already initialized.");

            if (IsInitialized)
            {
                return false;
            }

            lock (_syncRoot)
            {
                _equityHistory = new Dictionary<DateTime, Decimal>();

                _account = account;
                _account.UpdatedEvent += new Account.AccountUpdateDelegate(_account_UpdateEvent);
                _account.OperationalStateChangedEvent += new CommonSupport.OperationalStateChangedDelegate(_account_OperationalStatusChangedEvent);
                
                if (account.Info.Equity.HasValue)
                {
                    _initialEquity = account.Info.Equity.Value;
                }
                else
                {
                    _initialEquity = null;
                }

                IOrderSink executor = account.OrderExecutionProvider;
                if (executor != null)
                {
                    executor.OrdersUpdatedEvent += new OrdersUpdateDelegate(OrderExecutionProvider_OrderUpdatedEvent);
                    //OrderExecutionProvider_OrderUpdatedEvent(executor, _account.Info, new string[] { }, new OrderInfo[] { }, new Order.UpdateTypeEnum[] { });
                }
            }

            return true;
        }

        public void UnInitialize()
        {
            bool update = false;
            lock (_syncRoot)
            {
                if (_account != null)
                {
                    _account.UpdatedEvent -= new Account.AccountUpdateDelegate(_account_UpdateEvent);
                    _account.OperationalStateChangedEvent -= new CommonSupport.OperationalStateChangedDelegate(_account_OperationalStatusChangedEvent);

                    IOrderSink executor = _account.OrderExecutionProvider;
                    if (executor != null)
                    {
                        executor.OrdersUpdatedEvent -= new OrdersUpdateDelegate(OrderExecutionProvider_OrderUpdatedEvent);
                        update = true;
                    }
                    _account = null;
                }
            }

            if (update)
            {
                UpdateOrdersStatistics();
            }
        }

        void _account_OperationalStatusChangedEvent(IOperational account, OperationalStateEnum previousState)
        {
            if (account.OperationalState == CommonSupport.OperationalStateEnum.Operational)
            {
                if (_account.Info.Balance.HasValue)
                {
                    _initialEquity = _account.Info.Balance.Value;
                }
            }
        }

        public void UpdateValues()
        {
            lock (_syncRoot)
            {
                if (_account.Info.Equity.HasValue)
                {
                    _maxEquity = GeneralHelper.Max(_account.Info.Equity, _maxEquity);
                    _minEquity = GeneralHelper.Min(_account.Info.Equity, _minEquity);
                }

                if (_initialEquity != 0)
                {
                    if (_account.Info.Equity.HasValue)
                    {
                        _performancePercentage = (100 * _account.Info.Equity.Value / _initialEquity) - 100;
                    }

                    if (_minEquity.HasValue && _maxEquity.HasValue && _initialEquity.HasValue)
                    {
                        _bestPerformancePercentage = GeneralHelper.Max((100 * _maxEquity.Value / _initialEquity.Value) - 100, _bestPerformancePercentage);
                        _worstPerformancePercentage = GeneralHelper.Min((100 * _minEquity.Value / _initialEquity.Value) - 100, _worstPerformancePercentage);

                        _overalDrawDownPercentage = GeneralHelper.Max((100 * (_initialEquity.Value - _minEquity.Value) / _initialEquity.Value), _overalDrawDownPercentage);
                        _largestDrawDownPercentage = GeneralHelper.Max((100 * (_maxEquity.Value - _minEquity.Value) / _initialEquity.Value), _largestDrawDownPercentage);
                    }
                }
                else
                {
                    _performancePercentage = null;
                    _bestPerformancePercentage = null;
                    _worstPerformancePercentage = null;

                    _overalDrawDownPercentage = null;
                    _largestDrawDownPercentage = null;
                }

                if (_account.Info.Equity.HasValue)
                {
                    _profitOverall = _account.Info.Equity.Value;
                    _equityHistory[DateTime.Now] = _account.Info.Equity.Value;
                }

            }
        }

        void _account_UpdateEvent(Account account)
        {
            UpdateValues();
        }

        public virtual void OrderExecutionProvider_OrderUpdatedEvent(IOrderSink executor, AccountInfo account, string[] previousOrdersIds, OrderInfo[] providerOrders, ActiveOrder.UpdateTypeEnum[] updatesType)
        {
            UpdateOrdersStatistics();
        }

        void UpdateOrdersStatistics()
        {
            ISourceOrderExecution provider = _account.OrderExecutionProvider;
            if (provider == null)
            {
                SystemMonitor.Warning("Provider not found.");
                return;
            }

            List<Order> orders = provider.TradeEntities.GetOrdersByState(OrderStateEnum.Executed);
            orders.AddRange(provider.TradeEntities.GetOrdersByState(OrderStateEnum.Closed));

            _buyTrades = 0;
            _sellTrades = 0;
            _totalTrades = orders.Count;

            for (int i = 0; i < orders.Count; i++)
            {
                OrderInfo providerOrder = orders[i].Info;
                //ActiveOrder.UpdateTypeEnum updateType = updatesType[i];
                //if (providerOrder.State == OrderStateEnum.UnInitialized)
                //{
                //    continue;
                //}

                //if (updateType == ActiveOrder.UpdateTypeEnum.Executed)
                {
                    //_totalTrades++;

                    if (providerOrder.IsBuy)
                    {
                        Interlocked.Increment(ref _buyTrades);
                    }
                    else
                    {
                        Interlocked.Increment(ref _sellTrades);
                    }
                }

                Order order = _account.TradeEntities.GetOrderById(providerOrder.Id);
                if (order == null)
                {
                    SystemMonitor.Error("Order with id [" + providerOrder.Id + "] not found.");
                    continue;
                }

                if (providerOrder.State == OrderStateEnum.Closed && order is ActiveOrder)
                {
                    if (((ActiveOrder)order).GetResult(ActiveOrder.ResultModeEnum.Raw) > 0)
                    {
                        Interlocked.Increment(ref _currentConsecutiveWinners);
                        Interlocked.Increment(ref _winningTrades);
                        _currentConsecutiveLosers = 0;

                        decimal? result = ((ActiveOrder)order).GetResult(ActiveOrder.ResultModeEnum.Currency);
                        if (result.HasValue)
                        {
                            //_winnersProfit += result.Value;
                            _biggestWinner = GeneralHelper.Max(result.Value, _biggestWinner.Value);
                        }

                    }
                    else if (((ActiveOrder)order).GetResult(ActiveOrder.ResultModeEnum.Raw) < 0)
                    {
                        Interlocked.Increment(ref _currentConsecutiveLosers);
                        _currentConsecutiveWinners = 0;
                        Interlocked.Increment(ref _losingTrades);
                        decimal? result = ((ActiveOrder)order).GetResult(ActiveOrder.ResultModeEnum.Currency);

                        if (result.HasValue)
                        {
                            //_losersLoss += result.Value;
                            _biggestLoser = GeneralHelper.Min(result.Value, _biggestLoser.Value);
                        }
                    }

                    _maxConsecutiveWinners = Math.Max(_currentConsecutiveWinners, _maxConsecutiveWinners);
                    _maxConsecutiveLosers = Math.Max(_currentConsecutiveLosers, _maxConsecutiveLosers);
                }

                // Establish first order open time.
                //if (providerOrder.State == OrderStateEnum.Executed)
                {
                    if (order.OpenTime.HasValue &&
                        (_firstOrderTime.HasValue == false || order.OpenTime < _firstOrderTime))
                    {
                        _firstOrderTime = order.OpenTime.Value;
                    }

                    if (order.OpenTime.HasValue &&
                        (_lastOrderTime.HasValue == false || order.OpenTime.Value > _lastOrderTime))
                    {
                        _lastOrderTime = order.OpenTime.Value;
                    }
                }
            }
        }

    }
}
