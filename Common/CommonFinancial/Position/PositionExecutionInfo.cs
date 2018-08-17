using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.ComponentModel;

namespace CommonFinancial
{
    /// <summary>
    /// Information regarding a specific execution on a given position.
    /// </summary>
    [Serializable]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public struct PositionExecutionInfo
    {
        string _executionId;
        public string ExecutionId
        {
            get { return _executionId; }
        }

        ComponentId _dataSourceId;
        public ComponentId DataSourceId
        {
            get { return _dataSourceId; }
            set { _dataSourceId = value; }
        }

        ComponentId _executionSourceId;
        public ComponentId ExecutionSourceId
        {
            get { return _executionSourceId; }
            set { _executionSourceId = value; }
        }

        Symbol _symbol;
        public Symbol Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        public enum ExecutionResultEnum
        {
            Unknown = 0,
            Success,
            PartialSuccess,
            Failure
        }

        OrderTypeEnum _orderType;
        public OrderTypeEnum OrderType
        {
            get { return _orderType; }
            set { _orderType = value; }
        }

        ExecutionResultEnum _result;
        public ExecutionResultEnum Result
        {
            get { return _result; }
            set { _result = value; }
        }

        decimal? _executedPrice;
        public decimal? ExecutedPrice
        {
            get { return _executedPrice; }
            set { _executedPrice = value; }
        }

        int _volumeExecuted;

        public int VolumeExecuted
        {
            get { return _volumeExecuted; }
            set { _volumeExecuted = value; }
        }

        public int VolumeExecutedDirectional
        {
            get 
            {
                if (OrderInfo.TypeIsBuy(_orderType))
                {
                    return _volumeExecuted;
                }
                else
                {
                    return -_volumeExecuted;
                }
            }
        }

        int _volumeRequested;

        public int VolumeRequested
        {
            get { return _volumeRequested; }
            set { _volumeRequested = value; }
        }

        DateTime? _executionTime;
        public DateTime? ExecutionTime
        {
            get { return _executionTime; }
            set { _executionTime = value; }
        }

        public bool IsEmpty
        {
            get
            {
                return _result == ExecutionResultEnum.Unknown && (_executedPrice.HasValue == false || _executedPrice == 0) && _volumeRequested == 0;
            }
        }

        public static PositionExecutionInfo Empty
        {
            get
            {
                return new PositionExecutionInfo();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PositionExecutionInfo(string executionId, ComponentId dataSourceId, ComponentId executionSourceId,
            Symbol symbol, OrderTypeEnum orderType, decimal? price, int volumeRequest, int volumeExecuted, DateTime? time, ExecutionResultEnum result)
        {
            _executionId = executionId;
            _orderType = orderType;
            _dataSourceId = dataSourceId;
            _executionSourceId = executionSourceId;
            _symbol = symbol;
            _result = result;
            _executedPrice = price;
            _volumeRequested = volumeRequest;
            _volumeExecuted = volumeExecuted;
            _executionTime = time;
        }

        /// <summary>
        /// Will calculate the result of this execution up until now, against the values of the provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public decimal? GetPendingResult(IQuoteProvider provider, Order.ResultModeEnum mode, DataSessionInfo info)
        {
            if (this.IsEmpty || _executedPrice.HasValue == false || this._executedPrice == 0 || provider == null)
            {
                return null;
            }

            return Order.GetResult(mode, _executedPrice, null, _volumeExecuted, Symbol, OrderStateEnum.Executed, OrderType, null, Symbol.Empty,
                info.LotSize, info.DecimalPlaces, provider.Ask, provider.Bid);
        }

        public string Print()
        {
            return string.Format("Result[{0}], Symbol [{1}], Price [{2}], Volume Requested [{3}], Volume Executed [{4}], OrderType [{4}], Source [{5}]", _result.ToString(), _symbol.Name, GeneralHelper.ToString(_executedPrice), GeneralHelper.ToString(_volumeRequested), GeneralHelper.ToString(_volumeExecuted), _orderType.ToString(), _executionSourceId.Name);
        }
    }
}
