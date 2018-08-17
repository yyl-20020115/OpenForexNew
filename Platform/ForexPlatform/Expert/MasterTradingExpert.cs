//using System;
//using System.Runtime.Serialization;
//using CommonFinancial;
//using CommonSupport;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;

//namespace ForexPlatform
//{
//    /// <summary>
//    /// Master trading allows the execution of orders simultaniously to multiple executioners.
//    /// It partially implements the ISourceOrderExecution, ISessionDataProvider to allow the order
//    /// creation user interface to operate.
//    /// </summary>
//    [Serializable]
//    [UserFriendlyName("Master Trading [Prototype]")]
//    public class MasterTradingExpert : Expert
//    {
//        List<MasterOrder> _masterOrders = new List<MasterOrder>();
//        public ReadOnlyCollection<MasterOrder> MasterOrders
//        {
//            get { lock (this) { return _masterOrders.AsReadOnly(); } }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public MasterTradingExpert(/*ISourceManager sessionManager, string name*/)
//            : base(null, string.Empty/*sessionManager, name*/)
//        {
//        }

//        public bool AddOrder(MasterOrder order)
//        {
//            _masterOrders.Add(order);
//            return true;
//        }

//        public bool RemoveOrder(MasterOrder order)
//        {
//            return _masterOrders.Remove(order);
//        }
//    }
//}
