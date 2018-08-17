using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    public abstract class EvaluativeOccurence
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        ExpertSession _platformSession;
        public ExpertSession PlatformSession
        {
            get { return _platformSession; }
        }

        /// <summary>
        /// 
        /// </summary>
        public EvaluativeOccurence(ExpertSession platformSession)
        {
            _platformSession = platformSession;
        }

        public EvaluativeOccurenceResultSet EvaluatePositives(int startingIndex, int count)
        {
            EvaluativeOccurenceResultSet result = OnEvaluatePositives(startingIndex, count);
            System.Diagnostics.Debug.Assert(result.CheckPositiveValues(), "Value has been found to be below zero and this was not expected. An occurence is producing invalid results.");
            return result;
        }

        public EvaluativeOccurenceResultSet EvaluateNegatives(int startingIndex, int count)
        {
            EvaluativeOccurenceResultSet result = OnEvaluateNegatives(startingIndex, count);
            System.Diagnostics.Debug.Assert(result.CheckPositiveValues(), "Value has been found to be below zero and this was not expected. An occurence is producing invalid results.");
            return result;
        }

        protected abstract EvaluativeOccurenceResultSet OnEvaluatePositives(int startingIndex, int count);
        protected abstract EvaluativeOccurenceResultSet OnEvaluateNegatives(int startingIndex, int count);

        /// <summary>
        /// 
        /// </summary>
        static public string PrepareFormationName(List<double[]> indicatorsValues, int backwardsTimePeriod)
        {
            StringBuilder sb = new StringBuilder(1024);
            for (int i = indicatorsValues.Count - 1; i >= 0 && i >= indicatorsValues.Count - backwardsTimePeriod; i--)
            {
                foreach (float value in indicatorsValues[i])
                {
                    sb.Append(((int)value).ToString());
                }
                sb.Append(".");
            }

            return sb.ToString();
        }


        /// <summary>
        /// WARNING : SLOW.
        /// </summary>
        static public double[] GetIndicatorsValuesAtActualIndex(int actualIndex, Indicator[] indicators)
        {
            double[] result = new double[indicators.Length];
            SystemMonitor.NotImplementedWarning();
            //for (int i = 0; i < indicators.Length; i++)
            //{
            //    result[i] = indicators[i].Results.Signals[actualIndex];
            //}
            return result;
        }

    }
}
