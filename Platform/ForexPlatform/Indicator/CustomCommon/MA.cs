//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;


//namespace ForexPlatform
//{
//    [UserFriendlyName("Moving Average")]
//    public class MA : Indicator
//    {
//        private int _period = 12;
//        public int Period
//        {
//            get { return _period; }
//            set { _period = value; }
//        }

//        public enum MAStates
//        {
//            Default = 0,
//            Cross0Up = 1,
//            Cross0Down = 2
//        }

//        double[] _closeResultValues;

//        /// <summary>
//        /// 
//        /// </summary>
//        public MA()
//            : base(true, true)
//        {
//        }

//        //public override Type StateEnumType
//        //{
//        //    get { return typeof(MAStates); }
//        //}

//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {
//            SystemMonitor.CheckNotImplementedCritical(startingIndex == 0);

//            _closeResultValues = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Close, startingIndex, indecesCount);
//            double[] ma = new double[_closeResultValues.Length];

//            int beginIndex, number;
//            TicTacTec.TA.Library.Core.RetCode code =
//                TicTacTec.TA.Library.Core.Sma(0, indecesCount - 1, _closeResultValues, Period,
//                    out beginIndex, out number, ma);

//            System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//            Results.AddSetValues("MA", beginIndex, number, ma);
//        }

//        public override float OnResultAnalysisCrossingFound(int line1index, double line1value, int line2index, double line2value, bool direction, double currentSignalPositionValue)
//        {
//            if (line1index == 0 && line2index == 1)
//            {
//                if (direction)
//                {
//                    return (int)MAStates.Cross0Up;
//                }
//                else
//                {
//                    return (int)MAStates.Cross0Down;
//                }
//            }
//            else
//            {
//                System.Diagnostics.Debug.Fail("Not expected.");
//            }

//            return (int)MAStates.Default;
//        }

//        protected override double[][] ProvideSignalAnalysisLines()
//        {
//            return new double[][] { Results.ResultSets[0].Values, _closeResultValues };
//        }

//    }
//}
