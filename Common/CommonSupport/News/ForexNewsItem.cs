using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Specific forex type news item.
    /// </summary>
    public class ForexNewsItem : EventBase
    {
        /// <summary>
        /// The level of impact of the item.
        /// </summary>
        public enum ImpactEnum
        {
            NA = 0,
            Low = 1,
            Medium = 5,
            High = 10
        }

        volatile ImpactEnum _impact = ImpactEnum.NA;
        /// <summary>
        /// 
        /// </summary>
        [DBPersistenceToXmlData()]
        public ImpactEnum Impact
        {
            get { return _impact; }
            set { _impact = value; }
        }

        volatile string _currency;
        /// <summary>
        /// 
        /// </summary>
        [DBPersistenceToXmlData()]
        public string Currency
        {
            get { return _currency; }
            set { _currency = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ForexNewsItem()
        {
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public override int CompareTo(EventBase other)
        //{
        //    int compare = base.CompareTo(other);
        //    if (compare != 0 && other.GetType() != this.GetType())
        //    {
        //        return compare;
        //    }

        //    ForexNewsItem otherItem = (ForexNewsItem)other;
        //    compare = _impact.CompareTo(otherItem._impact);
        //    if (compare != 0)
        //    {
        //        return compare;
        //    }

        //    return _currency.CompareTo(otherItem._currency);
        //}
    }
}
