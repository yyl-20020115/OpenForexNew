//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;


//namespace ForexPlatform
//{
//    [UserFriendlyName("Relative Strength Index")]
//    public class RSI : Indicator
//    {
//        int _timePeriod = 14;
//        public int TimePeriod
//        {
//            get { return _timePeriod; }
//            set { _timePeriod = value; }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public RSI()
//            : base(true, false)
//        {
//        }

//        public enum RSIStates
//        {
//            Default = 0,
//            Under30Up = 1,
//            Under30Down = 2,
//            Over70Up = 3,
//            Over70Down = 4
//        }

//        //public override Type StateEnumType
//        //{
//        //    get { return typeof(RSIStates); }
//        //}

//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {// Calculate operationResult.
//            double[] closes = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Close, startingIndex, indecesCount);
//            double[] operationResult = new double[closes.Length];

//            int beginIndex, number;

//            TicTacTec.TA.Library.Core.RetCode code =
//            TicTacTec.TA.Library.Core.Rsi(0, indecesCount - 1, closes, TimePeriod,
//                    out beginIndex, out number, operationResult);

//            System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//            Results.SetResultSetValues("RSI", beginIndex, number, operationResult);
//        }


//        protected override double[][] ProvideSignalAnalysisLines()
//        {
//            return new double[][] { Results.ResultSets[0].Values, Results.CreateFixedLineResultLength(30), Results.CreateFixedLineResultLength(70) };
//        }


//        /// <summary>
//        /// Line indeces always come from lower to biger.
//        /// </summary>
//        public override float OnResultAnalysisCrossingFound(int line1index, double line1value, int line2index, double line2value, bool direction, double currentSignalPositionValue)
//        {// Calculating the operationResult signals.

//            if ((line1index == 0 && line2index == 1))
//            {
//                if (direction)
//                {
//                    return (int)RSIStates.Under30Up;
//                }
//                else
//                {
//                    return (int)RSIStates.Under30Down;
//                }
//            }

//            if ((line1index == 0 && line2index == 2))
//            {
//                if (direction)
//                {
//                    return (int)RSIStates.Over70Up;
//                }
//                else
//                {
//                    return (int)RSIStates.Over70Down;
//                }
//            }

//            return (int)RSIStates.Default;
//        }

//    }
//}
