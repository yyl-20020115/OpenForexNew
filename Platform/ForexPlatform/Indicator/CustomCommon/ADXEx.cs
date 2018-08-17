//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;

//namespace ForexPlatform
//{
//    /// <summary>
//    /// Values are 0-100. 
//    /// Interpretation by : http://stockcharts.com/school/doku.php?componentId=chart_school:technical_indicators:average_directional_
//    /// </summary>
//    [UserFriendlyName("Directional Movement - Average Index Extended")]
//    public class ADXEx : Indicator
//    {
//        // 14 period ADX.
//        private int _timePeriod = 14;
//        public int TimePeriod
//        {
//            get { return _timePeriod; }
//            set { _timePeriod = value; }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public ADXEx()
//            : base(true, false)
//        {
//        }

//        public enum ADXStates
//        {
//            Default = 0, // <20 indicates non trending market with low volumes;
//            Cross20Up = 1, // cross of 20 is the possible start of a trend;
//            Cross40Up = 2,
//            Cross40Down = 3,
//            CrossPDINDIUp = 4,
//            CrossPDINDIDown = 5
//        }

//        //public override Type StateEnumType
//        //{
//        //    get { return typeof(ADXStates); }
//        //}

//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {
//            double[] high = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.High, startingIndex, indecesCount);
//            double[] low = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Low, startingIndex, indecesCount);
//            double[] close = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Close, startingIndex, indecesCount);

//            double[] operationResult = new double[high.Length];

//            int beginIndex, number;
//            TicTacTec.TA.Library.Core.RetCode code;

//            {// ADX
//                code = TicTacTec.TA.Library.Core.Adx(0, indecesCount - 1, high, low, close, TimePeriod,
//                        out beginIndex, out number, operationResult);
//                System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//                Results.SetResultSetValues("ADX", beginIndex, number, operationResult);
//            }

//            {// Positive DI.
//                double[] positiveDIResult = new double[high.Length];
//                code = TicTacTec.TA.Library.Core.PlusDI(0, indecesCount - 1, high, low, close, TimePeriod,
//                    out beginIndex, out number, positiveDIResult);
//                System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//                Results.SetResultSetValues("PositiveDI", beginIndex, number, positiveDIResult);
//            }

//            {// Negative DI.
//                double[] negativeDIResult = new double[high.Length];
//                code = TicTacTec.TA.Library.Core.MinusDI(0, indecesCount - 1, high, low, close, TimePeriod,
//                    out beginIndex, out number, negativeDIResult);
//                System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//                Results.SetResultSetValues("NegativeDI", beginIndex, number, negativeDIResult);
//            }

//        }

//        public override float OnResultAnalysisCrossingFound(int line1index, double line1value, int line2index, double line2value, bool direction, double currentSignalPositionValue)
//        {
//            if (line1index == 3 && line2index == 4 && direction)
//            {
//                return (int)ADXStates.CrossPDINDIUp;
//            }
//            if (line1index == 3 && line2index == 4 && direction == false)
//            {
//                return (int)ADXStates.CrossPDINDIDown;
//            }
//            if (line1index == 0 && line2index == 1 && direction)
//            {
//                return (int)ADXStates.Cross20Up;
//            }
//            if (line1index == 0 && line2index == 2 && direction)
//            {
//                return (int)ADXStates.Cross40Up;
//            }
//            if (line1index == 0 && line2index == 2 && direction == false)
//            {
//                return (int)ADXStates.Cross40Down;
//            }

//            return (int)ADXStates.Default;
//        }

//        protected override double[][] ProvideSignalAnalysisLines()
//        {
//            return new double[][] { Results.ResultSets[0].Values, Results.CreateFixedLineResultLength(20), Results.CreateFixedLineResultLength(40), Results.ResultSets[1].Values, Results.ResultSets[2].Values };
//        }

//        public override float OnResultAnalysisExtremumFound(int lineIndex, double lineValue, bool direction, double currentPositionSignalValue)
//        {
//            return 0;
//        }


//    }
//}
