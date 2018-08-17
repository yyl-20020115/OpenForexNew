//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;


//namespace ForexPlatform
//{
//    /// <summary>
//    /// Average True Range (ATR) indicator measures a security's volatility.
//    /// </summary>
//    [UserFriendlyName("Average True Range")]
//    public class ATR : Indicator
//    {
//        // The range for the last day or so.
//        int _timePeriod = 14;
//        public int TimePeriod
//        {
//            get { return _timePeriod; }
//            set { _timePeriod = value; }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public ATR()
//            : base(true, false, new string[] { "ATR" })
//        {
//        }

//        public enum ATRStates
//        {
//            Default = 0,
//            MaxExtreme = 1,
//            MinExtreme = 2
//        }

//        //public override Type StateEnumType
//        //{
//        //    get { return typeof(ATRStates); }
//        //}

//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {
//            double[] high = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.High, startingIndex, indecesCount);
//            double[] low = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Low, startingIndex, indecesCount);
//            double[] close = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Close, startingIndex, indecesCount);

//            double[] operationResult = new double[high.Length];

//            int beginIndex, number;

//            TicTacTec.TA.Library.Core.RetCode code = TicTacTec.TA.Library.Core.Atr(0, indecesCount - 1, high, low, close, TimePeriod,
//                    out beginIndex, out number, operationResult);

//            System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//            Results.SetResultSetValues("ATR", beginIndex, number, operationResult);
//        }

//        public override float OnResultAnalysisExtremumFound(int lineIndex, double lineValue, bool direction, double currentPositionSignalValue)
//        {
//            System.Diagnostics.Debug.Assert(lineIndex == 0);

//            if (direction)
//            {
//                return (int)ATRStates.MaxExtreme;
//            }
//            else
//            {
//                return (int)ATRStates.MinExtreme;
//            }
//        }

//    }
//}
