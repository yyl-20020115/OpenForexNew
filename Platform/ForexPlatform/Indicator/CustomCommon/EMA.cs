//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;


//namespace ForexPlatform
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [UserFriendlyName("Exponential Moving Average")]
//    public class EMA : Indicator
//    {
//        int _timePeriod = 17;
//        public int TimePeriod
//        {
//            get { return _timePeriod; }
//            set { _timePeriod = value; }
//        }

//        double[] _closeResultValues;

//        /// <summary>
//        /// 
//        /// </summary>
//        public EMA()
//            : base(true, true, new string[] { "EMA" })
//        {
//        }

//        public enum EMAStates
//        {
//            Default = 0,
//            DownUpCross = 1,
//            UpDownCross = 2,
//        }

//        //public override Type StateEnumType
//        //{
//        //    get { return typeof(EMAStates); }
//        //}

//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {
//            _closeResultValues = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Close, startingIndex, indecesCount);

//            double[] emaResults = new double[indecesCount];
//            int beginIndex = 0;
//            int numberElements = 0;

//            TicTacTec.TA.Library.Core.RetCode code =
//            TicTacTec.TA.Library.Core.Ema(0, indecesCount - 1, _closeResultValues, TimePeriod,
//                out beginIndex, out numberElements, emaResults);

//            System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//            Results.SetResultSetValues("EMA", beginIndex, numberElements, emaResults);
//        }

//        public override float OnResultAnalysisCrossingFound(int line1index, double line1value, int line2index, double line2value,
//            bool direction, double currentSignalPositionValue)
//        {
//            if (line1index == 0 && line2index == 1)
//            {
//                if (direction)
//                {
//                    return (int)EMAStates.DownUpCross;
//                }
//                else
//                {
//                    return (int)EMAStates.UpDownCross;
//                }
//            }

//            return (int)EMAStates.Default;
//        }

//        protected override double[][] ProvideSignalAnalysisLines()
//        {
//            return new double[][] { Results.ResultSets[0].Values, _closeResultValues };
//        }

//    }
//}
