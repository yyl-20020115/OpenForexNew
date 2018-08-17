using System;
using System.Collections.Generic;
using System.Text;

namespace CommonFinancial
{
    /// <summary>
    /// Structure contains information specific to a trading session.
    /// Any trading session contains a trading BaseCurrency as well as trading 
    /// timer interval (variable or fixed) and additional information.
    /// </summary>
    [Serializable]
    public struct DataSessionInfo : IComparable<DataSessionInfo>, IEquatable<DataSessionInfo>
    {
        Guid _id;
        /// <summary>
        /// Unique GUID of this session.
        /// </summary>
        public Guid Guid
        {
            get { return _id; }
        }

        string _name;
        /// <summary>
        /// Name of the session.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        Symbol _symbol;
        /// <summary>
        /// Trading baseCurrency information, for this session.
        /// </summary>
        public Symbol Symbol
        {
            get { return _symbol; }
        }

        decimal _lotSize;
        public decimal LotSize
        {
            get { return _lotSize; }
        }

        int _decimalPlaces;
        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
        }

        /// <summary>
        /// Minimum increment of trading.
        /// </summary>
        public decimal DecimalIncrement
        {
            get
            {
                if (_decimalPlaces == 0)
                {
                    return 1;
                }

                return (decimal)Math.Pow(0.1, DecimalPlaces);
            }
        }

        string _valueFormat;

        /// <summary>
        /// String format of the values (for ex. "#0.####"), depending on session precision in PointDigits.
        /// </summary>
        public string ValueFormat
        {
            get 
            {
                return _valueFormat; 
            }
        }

        /// <summary>
        /// Is this session orderInfo emtpy.
        /// </summary>
        public bool IsEmtpy
        {
            get
            {
                return _id == Guid.Empty || this.Equals(Empty);
            }
        }

        /// <summary>
        /// Empty Info instance.
        /// </summary>
        public static DataSessionInfo Empty
        {
            get { return new DataSessionInfo(Guid.Empty, string.Empty, Symbol.Empty, 0, 0); }
        }

        /// <summary>
        /// Constructor, full.
        /// </summary>
        public DataSessionInfo(Guid id, string name, Symbol symbol, decimal lotSize, int decimalPlaces)
        {
            _id = id;
            _symbol = symbol;
            _decimalPlaces = decimalPlaces;
            _lotSize = lotSize;
            _name = name;
            _valueFormat = "#0.";

            for (int i = 0; i < _decimalPlaces; i++)
            {
                _valueFormat += "0";
            }
        }

        /// <summary>
        /// Do those sessions match loosely and is it possible to map one for the other 
        /// (are they interchangeable and a version of the same trading instrument and time frame).
        /// This will not compare IDs but symbols, time intervals etc.
        /// </summary>
        public static bool CheckSessionsLooseMatch(DataSessionInfo info1, DataSessionInfo info2)
        {
            return (info1.Name == info2.Name
                && info1.Symbol.CompareTo(info2.Symbol) == 0
                && info1.LotSize == info2.LotSize
                // TODO: verify mapping will still operate properly after period changed.
                /*&& info1.Period == info2.Period*/);
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public bool CheckLooseMatch(DataSessionInfo other)
        {
            return CheckSessionsLooseMatch(this, other);
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal ConvertPointsToValue(decimal points)
        {
            return points * (decimal)Math.Pow(10, _decimalPlaces);
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConvertVolumeToUnits(decimal lots)
        {
            return (int)((decimal)_lotSize * lots); 
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal ConvertVolumeToLots(int units)
        {
            return (int)((decimal)units / (decimal)_lotSize);
        }

        #region IComparable<Info> Members

        public int CompareTo(DataSessionInfo other)
        {
            int compare = _id.CompareTo(other._id);
            if (compare != 0)
            {
                return compare;
            }

            compare = _symbol.CompareTo(other._symbol);
            if (compare != 0)
            {
                return compare;
            }

            compare = _decimalPlaces.CompareTo(other._decimalPlaces);
            if (compare != 0)
            {
                return compare;
            }

            compare = _lotSize.CompareTo(other._lotSize);
            if (compare != 0)
            {
                return compare;
            }

            return _name.CompareTo(other._name);
        }

        #endregion

        #region IEquatable<DataSessionInfo> Members

        public bool Equals(DataSessionInfo other)
        {
            return CompareTo(other) == 0;
        }

        #endregion
    }
}
