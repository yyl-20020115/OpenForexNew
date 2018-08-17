using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// This indicator is not tradeable since it changes its position and requres future looking.
    /// Indicator written manually no external lib used.
    /// </summary>
    [Serializable]
    [UserFriendlyName("ZigZag Custom")]
    public class ZigZagCustom : CustomPlatformIndicator
    {
        /// <summary>
        /// If not using open close, than we use low and high (default).
        /// </summary>
        bool _useOpenClose = false;
        public bool UseOpenClose
        {
            get { return _useOpenClose; }
            set { _useOpenClose = value; }
        }

        double _significancePercentage = 3;
        public double SignificancePercentage
        {
            get { return _significancePercentage; }
            set { _significancePercentage = value; }
        }

        //int _minimumLength = 5;
        //public int MinimumLength
        //{
        //    get { return _minimumLength; }
        //    set { _minimumLength = value; }
        //}

        /// <summary>
        /// 
        /// </summary>
        public ZigZagCustom()
            : base(typeof(ZigZagCustom).Name, false, true, new string[] { "ZigZag" })
        {
        }

        public enum ZigZagStates
        {
            Default = 0,
            PeakHigh = 1,
            PeakLow = 2
        }

        protected override void OnCalculate(bool fullRecalculation, DataBarUpdateType? updateType)
        {
            double[] values1, values2;

            int startingIndex = 0;
            int indecesCount = DataProvider.BarCount - 1;

            if (UseOpenClose)
            {
                values1 = DataProvider.GetValuesAsDouble(DataBar.DataValueEnum.Open, startingIndex, indecesCount);
                values2 = DataProvider.GetValuesAsDouble(DataBar.DataValueEnum.Close, startingIndex, indecesCount);
            }
            else
            {
                values1 = DataProvider.GetValuesAsDouble(DataBar.DataValueEnum.High, startingIndex, indecesCount);
                values2 = DataProvider.GetValuesAsDouble(DataBar.DataValueEnum.Low, startingIndex, indecesCount);
            }

            System.Diagnostics.Debug.Assert(values1.Length == values2.Length);

            if (indecesCount == 0)
            {
                Results.AddSetValues("ZigZag", startingIndex, indecesCount, true, new double[] { });
                return;
            }

            // This is simple indicator, it can directly write to the signals array.
            double[] signals = new double[indecesCount];
            signals[0] = (double)ZigZagStates.PeakHigh;

            int lastPeakIndex = 0;
            ZigZagStates lastPeakState = ZigZagStates.PeakHigh;

            // Perform actual calculation.
            for (int i = 0; i < values1.Length; i++)
            {
                double requiredDifferenceValue = (SignificancePercentage / 100) * values1[i];

                double high = Math.Max(values1[i], values2[i]);
                double low = Math.Min(values1[i], values2[i]);

                double lastPeakValue = 0;
                if (lastPeakState == ZigZagStates.PeakHigh)
                {
                    lastPeakValue = Math.Max(values1[lastPeakIndex], values2[lastPeakIndex]);
                }
                else if (lastPeakState == ZigZagStates.PeakLow)
                {
                    lastPeakValue = Math.Min(values1[lastPeakIndex], values2[lastPeakIndex]);
                }

                bool newLow = (lastPeakState == ZigZagStates.PeakHigh && (lastPeakValue - low >= requiredDifferenceValue)) || (lastPeakState == ZigZagStates.PeakLow && low < lastPeakValue);
                bool newHigh = (lastPeakState == ZigZagStates.PeakLow && (high - lastPeakValue >= requiredDifferenceValue)) || (lastPeakState == ZigZagStates.PeakHigh && high > lastPeakValue);

                //System.Diagnostics.Debug.Assert(newLow == false || newHigh == false);

                if (newLow && newHigh)
                {// Favor the extension of existing peak in this case.
                    if (lastPeakState == ZigZagStates.PeakHigh)
                    {
                        newLow = false;
                    }
                    else
                    {
                        newHigh = false;
                    }
                }

                if (newHigh)
                {
                    if (lastPeakState == ZigZagStates.PeakHigh)
                    {// Update the high.
                        signals[lastPeakIndex] = (double)ZigZagStates.Default;
                    }
                    else if (lastPeakState == ZigZagStates.PeakLow)
                    {// New high found.
                        lastPeakState = ZigZagStates.PeakHigh;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Fail("Unexpected case.");
                    }

                    lastPeakIndex = i;
                    signals[i] = (double)ZigZagStates.PeakHigh;
                }
                else if (newLow)
                {
                    if (lastPeakState == ZigZagStates.PeakLow)
                    {// Update the low.
                        signals[lastPeakIndex] = (double)ZigZagStates.Default;
                    }
                    else if (lastPeakState == ZigZagStates.PeakHigh)
                    {// New low found.
                        lastPeakState = ZigZagStates.PeakLow;
                    }

                    lastPeakIndex = i;
                    signals[lastPeakIndex] = (double)ZigZagStates.PeakLow;
                }

            }

            // Finish with a signal.
            if (signals[signals.Length - 1] == (double)ZigZagStates.Default)
            {
                if (lastPeakState == ZigZagStates.PeakHigh)
                {
                    signals[signals.Length - 1] = (double)ZigZagStates.PeakLow;
                }
                else
                {
                    signals[signals.Length - 1] = (double)ZigZagStates.PeakHigh;
                }
            }

            // Finally create the results, that shows a line moving between the peak values - like a proper ZigZag indicator.
            lastPeakIndex = 0;
            lastPeakState = ZigZagStates.PeakHigh;
            double[] results = new double[indecesCount];

            for (int i = 1; i < signals.Length; i++)
            {
                if (signals[i] != (double)ZigZagStates.Default)
                {
                    double lastPeakValue, currentPeakValue;

                    if (lastPeakState == ZigZagStates.PeakHigh)
                    {
                        lastPeakValue = Math.Max(values1[lastPeakIndex], values2[lastPeakIndex]);
                        currentPeakValue = Math.Min(values1[i], values2[i]);
                    }
                    else
                    {
                        lastPeakValue = Math.Min(values1[lastPeakIndex], values2[lastPeakIndex]);
                        currentPeakValue = Math.Max(values1[i], values2[i]);
                    }

                    double[] midValues = MathHelper.CreateConnectionValues(lastPeakValue, currentPeakValue, i - lastPeakIndex);
                    midValues.CopyTo(results, lastPeakIndex);

                    lastPeakIndex = i;
                    lastPeakState = (ZigZagStates)signals[i];
                }

            }

            Results.AddSetValues("ZigZag", startingIndex, indecesCount - 1, true, results);
            
            // Only after we add the operationResult set, the signals become available.
            Signals.Signals = signals;
        }

        /// <summary>
        /// Simple clone implementation will not clone results and signals, only parameters.
        /// </summary>
        /// <returns></returns>
        public override PlatformIndicator OnSimpleClone()
        {
            ZigZagCustom result = new ZigZagCustom();
            result._description = this._description;
            result._name = this._name;
            result._significancePercentage = this._significancePercentage;
            result._useOpenClose = this._useOpenClose;

            return result;
        }
    }
}
