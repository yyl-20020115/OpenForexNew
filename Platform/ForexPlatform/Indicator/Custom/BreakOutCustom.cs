//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;


//namespace ForexPlatform
//{
//    /// <summary>
//    /// This implements a simple version of the break out strategy shown by PeterCrowns of ForexFactory.
//    /// </summary>
//    [UserFriendlyName("Break Out Custom")]
//    public class BreakOutCustom : Indicator
//    {
//        public enum BreakOutStates
//        {
//            Default = 0,
//            BreakOut1 = 1,
//            BreakOut1Loose = 2,
//            BreakOut2 = 3,
//            BreakOut2Loose = 4
//        }

//        public enum StartOfPeriodValue
//        {
//            StartOfHour,
//            StartOfDay,
//            StartOfWeek,
//            StartOfMonth,
//            StartOfYear
//        }

//        StartOfPeriodValue _startOfPeriod = StartOfPeriodValue.StartOfDay;
//        /// <summary>
//        /// Where the breakout should look for a start of period to compare its value.
//        /// </summary>
//        public StartOfPeriodValue StartOfPeriod
//        {
//            get { return _startOfPeriod; }
//            set { _startOfPeriod = value; }
//        }

//        //public Type StateEnumType
//        //{
//        //    get { return typeof(BreakOutStates); }
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        public BreakOutCustom()
//            : base(true, true)
//        {
//        }

//        /// <summary>
//        /// 0 means no pin bar, 1 is pin bar ... 
//        /// </summary>
//        double EvaluateBar(int index)
//        {
//            System.Diagnostics.Debug.Assert(index > 0);
//            BarData prevDataUnit = this.SessionDataProvider.DataUnits[index - 1];
//            BarData dataUnit = this.SessionDataProvider.DataUnits[index];
//            BarData? startOfPeriodValue = null;

//            DayOfWeek lastDayOfWeek = DayOfWeek.Saturday;

//            for (int i = 1; i <= index && startOfPeriodValue.HasValue == false; i++)
//            {// Find the first bar of the day to figure out levels of entry.

//                switch (StartOfPeriod)
//                {
//                    case StartOfPeriodValue.StartOfHour:
//                        {
//                            if (SessionDataProvider.DataUnits[index - i].DateTime.TimeOfDay.Hours != dataUnit.DateTime.TimeOfDay.Hours)
//                            {// 1st of the month.
//                                startOfPeriodValue = SessionDataProvider.DataUnits[index - i];
//                            }
//                        }
//                    break;
//                    case StartOfPeriodValue.StartOfDay:
//                        {
//                            if (SessionDataProvider.DataUnits[index - i].DateTime.DayOfYear != dataUnit.DateTime.DayOfYear)
//                            {
//                                startOfPeriodValue = SessionDataProvider.DataUnits[index - i + 1];
//                            }
//                        }

//                    break;
//                    case StartOfPeriodValue.StartOfWeek:
//                    {
//                        if (SessionDataProvider.DataUnits[index - i].DateTime.DayOfWeek < lastDayOfWeek)
//                        {
//                            lastDayOfWeek = SessionDataProvider.DataUnits[index - i].DateTime.DayOfWeek;
//                        }
//                        else
//                        {
//                            startOfPeriodValue = SessionDataProvider.DataUnits[index - i + 1];
//                        }
//                    }
//                    break;
//                    case StartOfPeriodValue.StartOfMonth:
//                    {
//                        if (SessionDataProvider.DataUnits[index - i].DateTime.Month != dataUnit.DateTime.Month)
//                        {
//                            startOfPeriodValue = SessionDataProvider.DataUnits[index - i + 1];
//                        }
//                    }
//                    break;
//                    case StartOfPeriodValue.StartOfYear:
//                    {
//                        if (SessionDataProvider.DataUnits[index - i].DateTime.Year != dataUnit.DateTime.Year)
//                        {// 1st of the month.
//                            startOfPeriodValue = SessionDataProvider.DataUnits[index - i + 1];
//                        }
//                    }
//                    break;
//                    default:
//                    {
//                        System.Diagnostics.Debug.Fail("UnImplemented.");
//                    }
//                    break;
//                }
//            }

//            if (startOfPeriodValue == null || startOfPeriodValue.Value.DateTime == dataUnit.DateTime)
//            {// Start of day not found or same as current bar.
//                return (double)BreakOutStates.Default;
//            }

//            // This pattern is called "piercing line"
//            bool isBullishBreakOut = (prevDataUnit.BarIsRising == false && dataUnit.BarIsRising && prevDataUnit.BarBottomShadowLength >= dataUnit.BarBottomShadowLength
//                && prevDataUnit.Close <= dataUnit.Open && prevDataUnit.Open > dataUnit.Close && prevDataUnit.AverageOpenClose < dataUnit.Close);
//            if (isBullishBreakOut && dataUnit.Open > startOfPeriodValue.Value.HigherOpenClose)
//            {
//                return (double)BreakOutStates.BreakOut1;
//            }

            
//            bool isBullishBreakOutLoose = (prevDataUnit.BarIsRising == false && dataUnit.BarIsRising && 
//                prevDataUnit.Open > dataUnit.Close);
//            if (isBullishBreakOutLoose && dataUnit.Close > startOfPeriodValue.Value.HigherOpenClose)
//            {
//                return (double)BreakOutStates.BreakOut1Loose;
//            }


//            bool isBearishBreakOut = (prevDataUnit.BarIsRising && dataUnit.BarIsRising == false && prevDataUnit.BarTopShadowLength >= dataUnit.BarTopShadowLength
//                && prevDataUnit.Close >= dataUnit.Open && prevDataUnit.Open < dataUnit.Close && prevDataUnit.AverageOpenClose > dataUnit.Close);
//            if (isBearishBreakOut && dataUnit.Open < startOfPeriodValue.Value.LowerOpenClose)
//            {
//                return (double)BreakOutStates.BreakOut2;
//            }

//            bool isBearishBreakOutLoose = (prevDataUnit.BarIsRising && dataUnit.BarIsRising == false &&
//                prevDataUnit.Open < dataUnit.Close);
//            if (isBearishBreakOutLoose && dataUnit.Close < startOfPeriodValue.Value.LowerOpenClose)
//            {
//                return (double)BreakOutStates.BreakOut2Loose;
//            }
            
//            return (double)BreakOutStates.Default;
//        }

//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {
//            double[] results = new double[indecesCount];
//            for (int i = 1; i < indecesCount - 1; i++)
//            {
//                results[i] = EvaluateBar(i + startingIndex);
//            }

//            Results.SetResultSetValues("default", startingIndex, indecesCount, results);
//            results.CopyTo(Results.Signals, 0);
//        }



//    }
//}
