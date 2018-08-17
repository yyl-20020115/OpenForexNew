//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;

//namespace ForexPlatform
//{
//    /// <summary>
//    /// TODO : verify, test and add feature setting.
//    /// </summary>
//    [UserFriendlyName("Pin Bar Custom")]
//    public class PinBarCustom : Indicator
//    {
        
//        double _surroundingBarAverageMinimumLengthRation = 1.15;
//        public double SurroundingBarAverageMinimumLengthRation
//        {
//            get { return _surroundingBarAverageMinimumLengthRation; }
//            set { _surroundingBarAverageMinimumLengthRation = value; }
//        }
        
//        double _surroudingBarDifferenceRequirementRatio = 0.20;
//        public double SurroudingBarDifferenceRequirementRatio
//        {
//            get { return _surroudingBarDifferenceRequirementRatio; }
//            set { _surroudingBarDifferenceRequirementRatio = value; }
//        }

//        int _lookBackBarsCount = 12;
//        public int LookBackBarsCount
//        {
//            get { return _lookBackBarsCount; }
//            set { _lookBackBarsCount = value; }
//        }

//        //bool _pinBarDirectionValidation = true;
//        ///// <summary>
//        ///// Should the direction of the pin bar itself be considered.
//        ///// </summary>
//        //public bool PinBarDirectionValidation
//        //{
//        //    get { return _pinBarDirectionValidation; }
//        //    set { _pinBarDirectionValidation = value; }
//        //}

//        public enum PinBarStates
//        {
//            Default = 0,
//            PinBar1 = 1,
//            PinBar2 = 2
//        }

//        //public override Type StateEnumType
//        //{
//        //    get { return typeof(PinBarStates); }
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        public PinBarCustom()
//            : base(true, true, new string[] { "PinBar"} )
//        {
//        }

//        /// <summary>
//        /// 0 means no pin bar, 1 is pin bar ... 
//        /// </summary>
//        double EvaluatePinBar(int index)
//        {
//            BarData prevDataUnit = this.SessionDataProvider.DataUnits[index - 1];
//            BarData dataUnit = this.SessionDataProvider.DataUnits[index];
//            BarData nextDataUnit = this.SessionDataProvider.DataUnits[index + 1];

//            double bodyToShadowRatio, prevPinDifference, nextPinDifference;
//            bool nextUnitConditions = false;

//            if (dataUnit.BarIsRising)
//            {// Looking for a bottom pointing pin.
//                bodyToShadowRatio = dataUnit.BarBodyLength / dataUnit.BarBottomShadowLength;
//                prevPinDifference = prevDataUnit.Low - dataUnit.Low;
//                nextPinDifference = nextDataUnit.Low - dataUnit.Low;
//                nextUnitConditions = nextDataUnit.LowerOpenClose > dataUnit.LowerOpenClose;
//            }
//            else
//            {// Looking for a top pointing pin.
//                bodyToShadowRatio = dataUnit.BarBodyLength / dataUnit.BarTopShadowLength;
//                prevPinDifference = dataUnit.High - prevDataUnit.High;
//                nextPinDifference = dataUnit.High - nextDataUnit.High;
//                nextUnitConditions = nextDataUnit.HigherOpenClose < dataUnit.HigherOpenClose;
//            }

//            bool isPinPrerequirements = (bodyToShadowRatio < 1) && (prevPinDifference >= (dataUnit.BarTotalLength * _surroudingBarDifferenceRequirementRatio))
//                && (nextPinDifference >= (dataUnit.BarTotalLength * _surroudingBarDifferenceRequirementRatio))
//                && nextUnitConditions
//                //&& (nextDataUnit.BarIsPositive == dataUnit.BarIsPositive)
//                && (prevDataUnit.BarIsRising != dataUnit.BarIsRising);

//            if (isPinPrerequirements == false)
//            {
//                return 0;
//            }

//            if (index <= _lookBackBarsCount / 2)
//            {// Too few bars have passed - too early to establish a pin bar.
//                return 0;
//            }

//            double barLength = 0;

//            // Check lowest in the last LookBackBarsCount bars.
//            for (int i = index - 1; i >= 0 && i >= index - _lookBackBarsCount; i--)
//            {
//                barLength += SessionDataProvider.DataUnits[i].BarTotalLength;
//                if (dataUnit.BarIsRising)
//                {
//                    if (SessionDataProvider.DataUnits[i].Low <= dataUnit.Low ||
//                        SessionDataProvider.DataUnits[i].LowerOpenClose <= dataUnit.LowerOpenClose)
//                    {// Looking for bottom pointing pin and a lower pin was found last few bars.
//                        return 0;
//                    }
//                }
//                else
//                {
//                    if (SessionDataProvider.DataUnits[i].High >= dataUnit.High ||
//                        SessionDataProvider.DataUnits[i].HigherOpenClose >= dataUnit.HigherOpenClose)
//                    {// Looking for a top pointing pin and a higher pin was found in the last few bars.
//                        return 0;
//                    }
//                }
//            }

//            // This now becomes the average bar length for the last few bars.
//            barLength = barLength / _lookBackBarsCount;
            
//            // Check longer than average last LookBackBarsCount bars.
//            if (dataUnit.BarTotalLength > _surroundingBarAverageMinimumLengthRation * barLength)
//            {
//                if (dataUnit.BarIsRising)
//                {
//                    return (float)PinBarStates.PinBar1;
//                }
//                else
//                {
//                    return (float)PinBarStates.PinBar2;
//                }
//            }
//            return (float)PinBarStates.Default;
//        }

//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {
//            double[] results = new double[indecesCount];
//            for (int i = 1; i < indecesCount - 1; i++)
//            {
//                // Results are given in +1 to compensate looking in the next bar.
//                results[i + 1] = EvaluatePinBar(i + startingIndex);
//            }

//            Results.SetResultSetValues("PinBar", startingIndex, indecesCount, results);
//            results.CopyTo(Results.Signals, 0);
//        }



//    }
//}
