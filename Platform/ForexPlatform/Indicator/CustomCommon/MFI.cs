//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;
//using CommonFinancial;


//namespace ForexPlatform
//{
//    [UserFriendlyName("Money Flow Index")]
//    public class MFI : Indicator
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
//        public MFI()
//            : base(true, false, new string[] { "MFI" })
//        {

//        }

//        public enum MFIStates
//        {
//            Default = 0,
//            Under20Up = 1,
//            Under20Down = 2,
//            Over80Up = 3,
//            Over80Down = 4
//        }

//        //public override Type StateEnumType
//        //{
//        //    get { return typeof(MFIStates); }
//        //}


//        protected override void OnCalculate(int startingIndex, int indecesCount)
//        {
//            double[] high = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.High, startingIndex, indecesCount);
//            double[] low = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Low, startingIndex, indecesCount);
//            double[] close = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Close, startingIndex, indecesCount);
//            double[] closeVolume = SessionDataProvider.GetDataValues(BarData.DataValueSourceEnum.Volume, startingIndex, indecesCount);

//            double[] operationResult = new double[high.Length];

//            int beginIndex, number;

//            TicTacTec.TA.Library.Core.RetCode code =
//            TicTacTec.TA.Library.Core.Mfi(0, indecesCount - 1, high, low, close, closeVolume, TimePeriod, out beginIndex, out number, operationResult);

//            System.Diagnostics.Debug.Assert(code == TicTacTec.TA.Library.Core.RetCode.Success);

//            Results.SetResultSetValues("MFI", beginIndex, number, operationResult);
//        }

//        public override float OnResultAnalysisCrossingFound(int line1index, double line1value, int line2index, double line2value, bool direction, double currentSignalPositionValue)
//        {
//            if (line1index == 0 && line2index == 1)
//            {
//                if (direction)
//                {
//                    return (int)MFIStates.Over80Up;
//                }
//                else
//                {
//                    return (int)MFIStates.Over80Down;
//                }
//            }
//            else if (line1index == 0 && line2index == 2)
//            {
//                if (direction)
//                {
//                    return (int)MFIStates.Under20Up;
//                }
//                else
//                {
//                    return (int)MFIStates.Under20Down;
//                }
//            }

//            return (int)MFIStates.Default;

//        }

//        protected override double[][] ProvideSignalAnalysisLines()
//        {
//            return new double[][] { Results.ResultSets[0].Values, Results.CreateFixedLineResultLength(20), Results.CreateFixedLineResultLength(80) };
//        }

//    }
//}
