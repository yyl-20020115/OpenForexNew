using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Indicator signals are typically a set of values that occur when the indicator results correspond to some special
    /// value or condition that is considered to be a signal.
    /// </summary>
    [Serializable]
    public class IndicatorSignals : ISerializable
    {
        Indicator _indicator = null;
        int _actualResultSetStartingIndex = 0;
        double[] _signals = new double[] { };

        /// <summary>
        /// 
        /// </summary>
        public double[] Signals
        {
            get
            {
                lock (this)
                {
                    return _signals;
                }
            }

            set
            {
                lock (this)
                {
                    _signals = value;
                }
            }
        }

        protected int ActualResultsLength
        {
            get { return _signals.Length; }
        }

        /// <summary>
        /// In an extremum search, how many periods before and after the extremum are required to indicate an extremum as one.
        /// </summary>
        int _extremumAnalysisOutlinePeriod = 7;
        public int ExtremumAnalysisOutlinePeriod
        {
            get { return _extremumAnalysisOutlinePeriod; }
            set { _extremumAnalysisOutlinePeriod = value; }
        }

        public Type SignalStatesEnumType
        {
            get { return null; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IndicatorSignals(Indicator indicator)
        {
            _indicator = indicator;
        }

        /// <summary>
        /// Deserialization routine.
        /// </summary>
        public IndicatorSignals(SerializationInfo info, StreamingContext context)
        {
            _indicator = (Indicator)info.GetValue("indicator", typeof(Indicator));
            _actualResultSetStartingIndex = 0;
            _signals = new double[] { };
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("indicator", _indicator);
        }

        /// <summary>
        /// Provide the system with a way to know what is the scope of the indicator signals.
        /// </summary>
        public void GetStateValues(out string[] names, out int[] values)
        {
            names = null;
            values = null;
            //lock (this)
            //{

            //    if (SignalStatesEnumType == null)
            //    {
            //        names = new string[0];
            //        values = new int[0];
            //        return;
            //    }

            //    names = Enum.GetNames(SignalStatesEnumType);
            //    Array valuesArray = Enum.GetValues(SignalStatesEnumType);
            //    values = new int[valuesArray.Length];
            //    int i = 0;
            //    foreach (object value in valuesArray)
            //    {
            //        values[i] = (int)value;
            //        i++;
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public void PerformCrossingResultAnalysis(double[][] inputLines)
        {
            //            lock (this)
            //            {

            //                if (inputLines == null || inputLines.Length == 0 || inputLines[0].Length == 0)
            //                {
            //                    return;
            //                }

            //                for (int i = 0; i < inputLines.Length; i++)
            //                {
            //                    for (int j = i + 1; j < inputLines.Length; j++)
            //                    {
            //                        double[] line1 = inputLines[i];
            //                        double[] line2 = inputLines[j];

            //                        if (line1.Length == 0 || line2.Length == 0)
            //                        {
            //                            continue;
            //                        }

            //                        System.Diagnostics.Debug.Assert(line1.Length == line2.Length);

            //                        // Do not look for signals before the ResultSetStartingIndex, as those signals are invalid.
            //                        for (int k = _actualResultSetStartingIndex; k < line1.Length; k++)
            //                        {
            //                            if (k == 0)
            //                            {
            //                                if (line1[k] == line2[k])
            //                                {
            //                                    _signals[k] = _indicator.OnResultAnalysisCrossingFound(i, line1[k], j, line2[k], true, _signals[k]);
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if ((line1[k - 1] >= line2[k - 1] && line1[k] <= line2[k]))
            //                                {
            //                                    _signals[k] = _indicator.OnResultAnalysisCrossingFound(i, line1[k], j, line2[k], false, _signals[k]);
            //                                }
            //                                else if (line1[k - 1] <= line2[k - 1] && line1[k] >= line2[k])
            //                                {
            //                                    _signals[k] = _indicator.OnResultAnalysisCrossingFound(i, line1[k], j, line2[k], true, _signals[k]);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }

            //#if DEBUG // Verify the signals results agains the limits placed by the enum provided.
            //                string[] names;
            //                int[] values;
            //                GetStateValues(out names, out values);

            //                for (int i = 0; i < _signals.Length; i++)
            //                {
            //                    bool found = false;
            //                    foreach (int value in values)
            //                    {
            //                        if (_signals[i] == value)
            //                        {
            //                            found = true;
            //                            break;
            //                        }
            //                    }

            //                    System.Diagnostics.Debug.Assert(found, "Provided result is out of bounds. Possible calculation error.");
            //                }
            //#endif

            //            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double[] CreateFixedLineResultLength(double value)
        {
            //return MathHelper.CreateFixedLineResultLength(value,  ResultSets[0].Values.Length);
            SystemMonitor.NotImplementedCritical();
            return null;
        }

        /// <summary>
        /// Keep this in mind : this will search backwards for an extremum, but will not mark anything if any other signals are found on the way.
        /// The baseMethod of searching is going back and looking for a point that is the biggest/smallest in the ExtremumAnalysisOutlinePeriod count ticks
        /// before and after.
        /// </summary>
        private void AnalyseIndexExtremums(int inspectedLineIndex, int index, double[] values)
        {
            //lock (this)
            //{

            //    // check in the look back perdio, that there is no signal like us.

            //    double max = double.MinValue;
            //    double min = double.MaxValue;
            //    int lastExtremeMaxIndexFound = 0;
            //    int lastExtremeMinIndexFound = 0;

            //    // There is no use to look back before (index - (2 * ExtremumAnalysisOutlinePeriod + 2)).
            //    for (int i = index; i >= _actualResultSetStartingIndex && i >= index - (2 * ExtremumAnalysisOutlinePeriod + 2); i--)
            //    {
            //        if (_signals[i] != 0 && lastExtremeMaxIndexFound == 0 && lastExtremeMinIndexFound == 0)
            //        {// Some signal is already found, stop search.
            //            return;
            //        }

            //        double current = values[i];

            //        if (i <= index - ExtremumAnalysisOutlinePeriod)
            //        { // OK, we are in the zone to start looking now.
            //            if (current < min)
            //            {
            //                lastExtremeMinIndexFound = i;
            //            }
            //            if (current > max)
            //            {
            //                lastExtremeMaxIndexFound = i;
            //            }
            //        }

            //        if (lastExtremeMinIndexFound - i >= ExtremumAnalysisOutlinePeriod)
            //        {// Extreme minimum found.
            //            _signals[index] = _indicator.OnResultAnalysisExtremumFound(inspectedLineIndex, _resultSets[inspectedLineIndex].Values[lastExtremeMinIndexFound], false, _signals[index]);
            //            return;
            //        }

            //        if (lastExtremeMaxIndexFound - i >= ExtremumAnalysisOutlinePeriod)
            //        {// Extreme maximum found.
            //            _signals[index] = _indicator.OnResultAnalysisExtremumFound(inspectedLineIndex, _resultSets[inspectedLineIndex].Values[lastExtremeMaxIndexFound], true, _signals[index]);
            //            return;
            //        }

            //        max = Math.Max(current, max);
            //        min = Math.Min(current, min);
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public void PerformExtremumResultAnalysis()
        {
            //lock (this)
            //{

            //    for (int i = 0; i < _resultSets.Count; i++)
            //    {
            //        System.Diagnostics.Debug.Assert(_resultSets[i].Values.Length == _resultSets[0].Values.Length);
            //        for (int k = _actualResultSetStartingIndex; k < _resultSets[0].Values.Length; k++)
            //        {
            //            AnalyseIndexExtremums(i, k, _resultSets[i].Values);
            //        }
            //    }
            //}
        }

    }
}
