//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;

//namespace CommonFinancial
//{
//    /// <summary>
//    /// Advanced order class add advanced cpabilities of managing a placed order.
//    /// </summary>
//    [Serializable]
//    [UserFriendlyName("Advanced Active Order")]
//    public class AdvancedActiveOrder : ActiveOrder
//    {
//        /// <summary>
//        /// This allows to define result targets and handle the corresponding event that occurs when the given level is reached (or over).
//        /// Once the level is reached, the result target is deleted from the list.
//        /// </summary>
//        List<Decimal> _resultTargets = new List<Decimal>();
//        public List<Decimal> ResultTargets
//        {
//            get { lock (this) { return _resultTargets; } }
//        }

//        Decimal _localTakeProfit = Decimal.MinValue;
//        public Decimal LocalTakeProfit
//        {
//            get { lock (this) { return _localTakeProfit; } }
//        }

//        Decimal _localStopLoss = Decimal.MinValue;
//        public Decimal LocalStopLoss
//        {
//            get { lock (this) { return _localStopLoss; } }
//        }

//        Decimal _localTrailingStopLoss = Decimal.MinValue;
//        public Decimal ActiveTrailingStopLoss
//        {
//            get { lock (this) { return _localTrailingStopLoss; } }
//        }

//        //DataBarUpdateType? _operateOnUpdate = null;
//        ///// <summary>
//        ///// When should the advanced rules be considered. If this value is null values will be considered on all updates,
//        ///// otherwise only on the specified update type.
//        ///// </summary>
//        //public DataBarUpdateType? OperateOnUpdate
//        //{
//        //    get { lock (this) { return _operateOnUpdate; } }
//        //    set { lock (this) { _operateOnUpdate = value; } }
//        //}

//        protected delegate void OnResultTargetReachedDeleate(Decimal target, Decimal ask, Decimal bid);

//        [field:NonSerialized]
//        protected event OnResultTargetReachedDeleate OnResultTargetReachedEvent;

//        /// <summary>
//        /// 
//        /// </summary>
//        public AdvancedActiveOrder(ISourceOrderExecution executionProvider)
//            : base(executionProvider)
//        {
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        protected override void Quote_QuoteUpdateEvent(IQuoteProvider provider)
//        {
//            base.Quote_QuoteUpdateEvent(provider);

//            if (SessionDataProvider == null || SessionDataProvider.Quotes == null)
//            {
//                return;
//            }

//            decimal? ask = SessionDataProvider.Quotes.Ask;
//            decimal? bid = SessionDataProvider.Quotes.Bid;
//            Decimal? currentResultValue = GetRawResult(this.OpenPrice, this.CurrentVolume, this.State, this.Type, ask, bid, true);

//            if (currentResultValue == null)
//            {
//                return;
//            }

//            lock (this)
//            {
//                // Look for pre-defined result targets.
//                if (_resultTargets.Count != 0)
//                {// We need them sorted.
//                    _resultTargets.Sort();
//                    for (int i = _resultTargets.Count - 1; i >= 0 && currentResultValue < 0; i--)
//                    {// Figure out negative targets, starting with the abs.smallest (closest to 0).
//                        if (_resultTargets[i] < 0)
//                        {
//                            if (currentResultValue <= _resultTargets[i])
//                            {// Target hit, remove and continue.
//                                if (OnResultTargetReachedEvent != null)
//                                {
//                                    OnResultTargetReachedEvent(_resultTargets[i], ask.Value, bid.Value);
//                                    _resultTargets.RemoveAt(i);
//                                }
//                                else
//                                {// Since we missed the smallest target, no need to look further.
//                                    break;
//                                }
//                            }
//                        }
//                    }

//                    _resultTargets.Reverse();
//                    for (int i = _resultTargets.Count - 1; i >= 0 && currentResultValue > 0; i--)
//                    {// Figure out positive targets, starting with the abs.smallest (closest to 0).
//                        if (_resultTargets[i] > 0)
//                        {
//                            if (currentResultValue >= _resultTargets[i])
//                            {// Target hit, remove and continue.
//                                if (OnResultTargetReachedEvent != null)
//                                {
//                                    OnResultTargetReachedEvent(_resultTargets[i], ask.Value, bid.Value);
//                                    _resultTargets.RemoveAt(i);
//                                }
//                                else
//                                {// Since we missed the smallest target, no need to look further.
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//            }// lock

//            if ((LocalTakeProfit != 0 && currentResultValue >= LocalTakeProfit) ||
//                (LocalStopLoss != 0 && currentResultValue <= -LocalStopLoss) ||
//                (ActiveTrailingStopLoss != 0 && OrderMaximumResultAchieved - currentResultValue >= ActiveTrailingStopLoss))
//            {// Close position.
//                Close(ask, bid);
//            }

//        }
//    }
//}
