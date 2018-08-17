using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Class contains information about a trading account.
    /// Comparition between two infos is done only on essential set of parameters (server, baseCurrency, id, name),
    /// not the whole bunch of parameters (balance, credit etc. are ommited from compare).
    /// </summary>
    [Serializable]
    public struct AccountInfo : IComparable<AccountInfo>
    {
        Guid _guid;
        /// <summary>
        /// Unique Guid of account.
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get { return _guid == Guid.Empty; }
        }

        /// <summary>
        /// Empty account orderInfo.
        /// </summary>
        public static AccountInfo Empty
        {
            get { return new AccountInfo() { _guid = Guid.Empty }; }
        }

        #region Custom Operator

        /// <summary>
        /// 
        /// </summary>
        public static bool operator ==(AccountInfo a, AccountInfo b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator !=(AccountInfo a, AccountInfo b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.CompareTo((AccountInfo)obj) == 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        /// <summary>
        /// Identifiable core information about this account.
        /// </summary>
        #region Core Account Information Members

        volatile string _company;
        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }

        Symbol _baseCurrency;
        public Symbol BaseCurrency
        {
            get { return _baseCurrency; }
            set { _baseCurrency = value; }
        }

        volatile string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        volatile string _server;
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        volatile string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        decimal? _balance;
        public decimal? Balance
        {
            get { return _balance; }
            set { _balance = value; }
        }

        //decimal? _initialBalance;
        //public decimal? InitialBalance
        //{
        //    get { return _initialBalance; }
        //    set { _initialBalance = value; }
        //}

        decimal? _credit;
        public decimal? Credit
        {
            get { return _credit; }
            set { _credit = value; }
        }

        decimal? _equity;
        public decimal? Equity
        {
            get { return _equity; }
            set { _equity = value; }
        }

        decimal? _freeMargin;
        public decimal? FreeMargin
        {
            get { return _freeMargin; }
            set { _freeMargin = value; }
        }

        decimal? _leverage;
        public decimal? Leverage
        {
            get { return _leverage; }
            set { _leverage = value; }
        }

        decimal? _margin;
        public decimal? Margin
        {
            get { return _margin; }
            set { _margin = value; }
        }

        private decimal? _profit;
        public decimal? Profit
        {
            get { return _profit; }
            set { _profit = value; }
        }

        /// <summary>
        /// Full parametrized constructor.
        /// </summary>
        public AccountInfo(Guid guid, Decimal balance, Decimal credit, string company,
            Symbol baseCurrency, Decimal equity, Decimal freeMargin,
            Decimal leverage, Decimal margin, string name,
            string id, Decimal profit, string server)
        {
            _guid = guid;
            //_initialBalance = balance;
            _balance = balance;
            _credit = credit;
            _company = company;
            _baseCurrency = baseCurrency;
            _id = id;
            _server = server;
            _name = name;
            _equity = equity;
            _freeMargin = freeMargin;
            _leverage = leverage;
            _margin = margin;
            _profit = profit;

        }

        /// <summary>
        /// This will automatically update the free margin of this info, based on the 
        /// 
        /// </summary>
        public decimal? CalcuateFreeMargin(decimal openPositionsMarketValue)
        {
            if (_margin.HasValue == false || _leverage.HasValue == false)
            {
                return null;
            }

            decimal actualMargin = _margin.Value * _leverage.Value;
            return (actualMargin - openPositionsMarketValue) / _leverage.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update(AccountInfo other)
        {
            if (other.Guid != Guid.Empty)
            {
                if (this.Guid != other.Guid)
                {
                    SystemMonitor.Warning(string.Format("Account update with mismatching Guid, this [{0}] other [{1}].", this._guid.ToString(), other._guid.ToString()));
                    return;
                }
            }

            if (other.Balance.HasValue)
            {
                _balance = other._balance;
            }

            if (other._credit.HasValue)
            {
                _credit = other._credit;
            }

            if (string.IsNullOrEmpty(other._company) == false)
            {
                _company = other._company;
            }

            if (other._baseCurrency.IsEmpty == false)
            {
                _baseCurrency = other._baseCurrency;
            }

            if (string.IsNullOrEmpty(other._id) == false)
            {
                _id = other._id;
            }

            if (string.IsNullOrEmpty(other._server) == false)
            {
                _server = other._server;
            }

            if (string.IsNullOrEmpty(other._name) == false)
            {
                _name = other._name;
            }

            if (other._equity.HasValue)
            {
                _equity = other._equity;
            }

            if (other._freeMargin.HasValue)
            {
                _freeMargin = other._freeMargin;
            }

            if (other._leverage.HasValue)
            {
                _leverage = other._leverage;
            }

            if (other._margin.HasValue)
            {
                _margin = other._margin;
            }

            if (other._profit.HasValue)
            {
                _profit = other._profit;
            }

        }

        #region IComparable<AccountInfo> Members

        public int CompareTo(AccountInfo other)
        {
            int result = _guid.CompareTo(other._guid);
            if (result != 0)
            {
                return result;
            }

            result = GeneralHelper.CompareNullable(_company, other._company);
            if (result != 0)
            {
                return result;
            }

            result = _baseCurrency.CompareTo(other._baseCurrency);
            if (result != 0)
            {
                return result;
            }

            result = GeneralHelper.CompareNullable(_id, other._id);
            if (result != 0)
            {
                return result;
            }

            result = GeneralHelper.CompareNullable(_server, other._server);
            if (result != 0)
            {
                return result;
            }

            return GeneralHelper.CompareNullable(_name, other._name);
        }

        #endregion
    }
}
