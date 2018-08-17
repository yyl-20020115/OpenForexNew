//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;

//namespace ForexPlatform
//{
//    /// <summary>
//    /// SlowD or %D is a moving average of the %K or slowK.
//    /// SlowD is the once producing the trading signals.
//    /// </summary>
    
//    [UserFriendlyName("Stochastic")]
//    public class STO : Indicator
//    {
//        int _fastKPeriod = 8;
//        public int FastKPeriod
//        {
//            get { return _fastKPeriod; }
//            set { _fastKPeriod = value; }
//        }

//        int _slowKPeriod = 5;
//        public int SlowKPeriod
//        {
//            get { return _slowKPeriod; }
//            set { _slowKPeriod = value; }
//        }

//        TicTacTec.TA.Library.Core.MAType SlowKMAType = TicTacTec.TA.Library.Core.MAType.Ema;

//        int _slowDPeriod = 3;
//        public int SlowDPeriod
//        {
//            get { return _slowDPeriod; }
//            set { _slowDPeriod = value; }
//        }

//        TicTacTec.TA.Library.Core.MAType SlowDMAType = TicTacTec.TA.Library.Core.MAType.Ema;

//        public enum STOStates
//        {
//            Default = 0,
//            Under20Up = 1,
//            Under20Down = 2,
//            Over80Up = 3,
//            Over80Down = 4
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public STO()
//            : base(true, false, new string[] { "STOSlowK", "STOSlowD"})
//        {
//        }

//        //public override Type StateEnumType
//        //{
//        //    get { return typeof(STOStates); }
//        //}


//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {
//            double[] high = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.High, startingIndex, indecesCount);
//            double[] low = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Low, startingIndex, indecesCount);
//            double[] close = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Close, startingIndex, indecesCount);

//            double[] resultSlowK = new double[high.Length];
//            double[] resultSlowD = new double[high.Length];

//            int beginIndex, number;

//            TicTacTec.TA.Library.Core.RetCode code =
//            TicTacTec.TA.Library.Core.Stoch(0, indecesCount - 1, high, low, close,
//                FastKPeriod, SlowKPeriod, SlowKMAType, SlowDPeriod, SlowDMAType,
//                out beginIndex, out number, resultSlowK, resultSlowD);

//            System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//            Results.SetResultSetValues("STOSlowK", beginIndex, number, resultSlowK);
//            Results.SetResultSetValues("STOSlowD", beginIndex, number, resultSlowD);
//        }

//        public override float OnResultAnalysisCrossingFound(int line1index, double line1value, int line2index, double line2value, bool direction, double currentSignalPositionValue)
//        {
//            if (line1index == 0 && line2index == 1)
//            {
//                if (direction)
//                {
//                    return (int)STOStates.Under20Up;
//                }
//                else
//                {
//                    return (int)STOStates.Under20Down;
//                }
//            }

//            if (line1index == 0 && line2index == 2)
//            {
//                if (direction)
//                {
//                    return (int)STOStates.Over80Up;
//                }
//                else
//                {
//                    return (int)STOStates.Over80Down;
//                }
//            }

//            return (int)STOStates.Default;

//        }

//        protected override double[][] ProvideSignalAnalysisLines()
//        {
//            return new double[][] { Results.ResultSets[1].Values, Results.CreateFixedLineResultLength(20), Results.CreateFixedLineResultLength(80) };
//        }

//    }
//}
