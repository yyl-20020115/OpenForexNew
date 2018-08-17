using System;
using System.Collections.Generic;
using System.Text;

namespace CommonFinancial
{
    /// <summary>
    /// Struct contains core information of a position.
    /// </summary>
    [Serializable]
    public struct PositionInfo
    {
        Symbol _symbol;
        /// <summary>
        /// Symbol of position.
        /// </summary>
        public Symbol Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        /// <summary>
        /// Directional position closeVolume.
        /// </summary>
        decimal? _volume;
        /// <summary>
        /// Total closeVolume of items position.
        /// Below zero to indicate a short (sell) position.
        /// </summary>
        public decimal? Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        decimal? _pendingSellVolume;
        public decimal? PendingSellVolume
        {
            get { return _pendingSellVolume; }
            set { _pendingSellVolume = value; }
        }

        decimal? _pendingBuyVolume;
        public decimal? PendingBuyVolume
        {
            get { return _pendingBuyVolume; }
            set { _pendingBuyVolume = value; }
        }

        decimal? _price;
        public decimal? Price
        {
            get { return _price; }
            set { _price = value; }
        }

        decimal? _basis;
        /// <summary>
        /// Base combined price for position.
        /// </summary>
        public decimal? Basis
        {
            get { return _basis; }
            set { _basis = value; }
        }

        decimal? _result;
        /// <summary>
        /// Open Profit/Loss (PnL).
        /// </summary>
        public decimal? Result
        {
            get { return _result; }
            set { _result = value; }
        }

        decimal? _commission;
        /// <summary>
        /// 
        /// </summary>
        public decimal? Commission
        {
            get { return _commission; }
            set { _commission = value; }
        }

        decimal? _closedResult;
        /// <summary>
        /// 
        /// </summary>
        public decimal? ClosedResult
        {
            get { return _closedResult; }
            set { _closedResult = value; }
        }

        decimal? _marketValue;
        /// <summary>
        /// 
        /// </summary>
        public decimal? MarketValue
        {
            get
            {
                return _marketValue;
            }
            set
            {
                _marketValue = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _symbol.IsEmpty;
            }
        }

        public static PositionInfo Empty
        {
            get
            {
                return new PositionInfo();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PositionInfo(Symbol symbol, decimal? volume, decimal? result, decimal? pendingBuyVolume, decimal? pendingSellVolume, decimal? price, decimal? basis, 
            decimal? closedResult, decimal? marketValue, decimal? commission)
        {
            _symbol = symbol;
            _volume = volume;
            _result = result;
            _price = price;
            _basis = basis;
            _closedResult = closedResult;
            _marketValue = marketValue;
            _commission = commission;
            _pendingBuyVolume = pendingBuyVolume;
            _pendingSellVolume = pendingSellVolume;
        }

        /// <summary>
        /// Update current instance with new information.
        /// </summary>
        public void Update(PositionInfo other)
        {
            _symbol = other.Symbol;

            if (other.Volume.HasValue)
            {
                _volume = other.Volume;
            }

            if (other.Result.HasValue)
            {
                _result = other.Result;
            }

            if (other.PendingBuyVolume.HasValue)
            {
                _pendingBuyVolume = other.PendingBuyVolume;
            }

            if (other.Price.HasValue)
            {
                _price = other.Price;
            }

            if (other.Basis.HasValue)
            {
                _basis = other.Basis;
            }

            if (other.ClosedResult.HasValue)
            {
                _closedResult = other.ClosedResult;
            }

            if (other.MarketValue.HasValue)
            {
                _marketValue = other.MarketValue;
            }

            if (other._commission.HasValue)
            {
                _commission = other._commission;
            }

            if (other._pendingBuyVolume.HasValue)
            {
                _pendingBuyVolume = other._pendingBuyVolume;
            }

            if (other._pendingSellVolume.HasValue)
            {
                _pendingSellVolume = other._pendingSellVolume;
            }

        }

    }
}
