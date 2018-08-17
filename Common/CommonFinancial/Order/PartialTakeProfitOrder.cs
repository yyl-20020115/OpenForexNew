//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;

//namespace CommonFinancial
//{
//    [UserFriendlyName("Partal take profit order")]
//    [Serializable]
//    public class PartialTakeProfitOrder : AdvancedActiveOrder
//    {
//        /// <summary>
//        /// First double is the level to activate the partial profit taking, 
//        /// the second is the closeVolume of closeVolume (in percents of initial closeVolume) to be closed at this level.
//        /// Those must be correct (never sum more than 100), and keep in mind also the base class TP and SL, 
//        /// and trailing TP operate parallel to this.
//        /// </summary>
//        Dictionary<Decimal, Decimal> _targetLevelToVolumePercentage = new Dictionary<Decimal, Decimal>();

//        /// <summary>
//        /// 
//        /// </summary>
//        public PartialTakeProfitOrder(ISourceOrderExecution executionProvider)
//            : base(executionProvider)
//        {
//            base.OnResultTargetReachedEvent += new OnResultTargetReachedDeleate(PartialTPActiveOrder_OnResultTargetReachedEvent);
//        }

//        public override void OnDeserialization(object sender)
//        {
//            base.OnDeserialization(sender);
//            base.OnResultTargetReachedEvent += new OnResultTargetReachedDeleate(PartialTPActiveOrder_OnResultTargetReachedEvent);
//        }

//        void PartialTPActiveOrder_OnResultTargetReachedEvent(Decimal target, Decimal ask, Decimal bid)
//        {
//            SystemMonitor.NotImplementedCritical();
//            //System.Diagnostics.Debug.Assert(_targetLevelToVolumePercentage.ContainsKey(target));
//            //base.DecreaseVolume(ask, bid, (_targetLevelToVolumePercentage[target] * base.InitialVolume) / 100);
//        }

//        /// <summary>
//        /// First double is the level to activate the partial profit taking, 
//        /// the second is the closeVolume of closeVolume (in percents of initial closeVolume) to be closed at this level.
//        /// Those must be correct (never sum more than 100), and keep in mind also the base class TP and SL, 
//        /// and trailing TP operate parallel to this.
//        /// Also only one entry per given target level.
//        /// </summary>
//        public void AddPartialProfitTarget(Decimal targetLevel, Decimal percentageOfInitialVolume)
//        {
//            Decimal percentageSum = 0;
//            foreach (Decimal value in _targetLevelToVolumePercentage.Values)
//            {
//                percentageSum += value;
//            }

//            if (_targetLevelToVolumePercentage.ContainsKey(targetLevel) || targetLevel <= 0 || 
//                percentageOfInitialVolume > 100 || percentageSum + percentageOfInitialVolume > 100)
//            {
//                throw new Exception("Profit targets can not be met, since they are more than 100% or entry level is invalid or already present.");
//            }

//            _targetLevelToVolumePercentage.Add(targetLevel, percentageOfInitialVolume);
//            base.ResultTargets.Add(targetLevel);
//        }
//    }
//}
