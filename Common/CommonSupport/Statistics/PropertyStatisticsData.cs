using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CommonSupport
{
    /// <summary>
    /// Statistics data of array that is related to the values of a given property of an object.
    /// </summary>
    public class PropertyStatisticsData
    {
        public PropertyInfo Info;

        List<double> _values = new List<double>();
        public List<double> Values
        {
            get { return _values; }
        }

        double _maximumValue = double.MinValue;
        public double MaximumValue
        {
            get { return _maximumValue; }
        }

        double _minimumValue = double.MaxValue;
        public double MinimumValue
        {
            get { return _minimumValue; }
        }

        double _averageValue = double.NaN;
        public double AverageValue
        {
            get { return _averageValue; }
        }

        double _positivePercentage = double.NaN;
        public double PositivePercentage
        {
            get { return _positivePercentage; }
        }

        double _negativePercentage = double.NaN;
        public double NegativePercentage
        {
            get { return _negativePercentage; }
        }

        int _consecutivePositives = 0;
        public int ConsecutivePositives
        {
            get
            {
                return _consecutivePositives;
            }
        }

        double _sum;
        public double Sum
        {
            get { return _sum; }
        }

        int _consecutiveNegatives = 0;
        public int ConsecutiveNegatives
        {
            get
            {
                return _consecutiveNegatives;
            }
        }

        double _consecutiveNegativesSum = 0;
        public double ConsecutiveNegativesSum
        {
            get { return _consecutiveNegativesSum; }
        }

        double _consecutivePositivesSum = 0;
        public double ConsecutivePositivesSum
        {
            get { return _consecutivePositivesSum; }
        }

        public static string[] ColumnHeaders
        {
            get
            {
                return new string[] { "Property Name", "Count", "Sum", "Avrg", "Min", "Max", "+%", "-%", "Consec +", "Consec -", "Consec + Sum", "Consec - Sum" };
            }
        }

        /// <summary>
        /// Use this instead of Info Name, since this contains the set name as well.
        /// </summary>
        public string Name
        {
            get
            {
                if (ParentSet != null)
                {
                    return ParentSet.Name + "." + Info.Name;
                }
                return Info.Name;
            }
        }

        public string[] Columns
        {
            get
            {
                return new string[] { Name, _values.Count.ToString(), _sum.ToString(), Math.Round(AverageValue, 3).ToString(), Math.Round(MinimumValue, 3).ToString(), Math.Round(MaximumValue, 3).ToString(), Math.Round(PositivePercentage, 3).ToString(), Math.Round(NegativePercentage, 3).ToString(), _consecutivePositives.ToString(), _consecutiveNegatives.ToString(), _consecutivePositivesSum.ToString(), _consecutiveNegativesSum.ToString() };
            }
        }

        MultipleItemStatisticsSet _parentSet;
        /// <summary>
        /// The set that contains this data. Can be null.
        /// </summary>
        public MultipleItemStatisticsSet ParentSet
        {
            get { return _parentSet; }
        }

        /// <summary>
        /// MultipleItemStatisticsSet parentSet CAN be NULL.
        /// </summary>
        public PropertyStatisticsData(MultipleItemStatisticsSet parentSet, PropertyInfo info, object[] items)
        {
            _parentSet = parentSet;
            Info = info;

            if (items.Length == 0)
            {
                return;
            }

            int positiveCount = 0;
            int negativeCount = 0;

            int consecutivePositive = 0;
            int consecutiveNegative = 0;

            double consecutivePositivesSum = 0;
            double consecutiveNegativesSum = 0;

            MethodInfo getMethodInfo = info.GetGetMethod(false);
            foreach (object item in items)
            {
                object objResult = getMethodInfo.Invoke(item, null);
                double value;
                bool result = double.TryParse(objResult.ToString(), out value);

                _values.Add(value);
       
                System.Diagnostics.Debug.Assert(result, "Parsing value failed.");

                _maximumValue = Math.Max(value, _maximumValue);
                _minimumValue = Math.Min(value, _minimumValue);
                if (value > 0)
                {
                    positiveCount++;
                    
                    consecutivePositive++;
                    consecutivePositivesSum += value;

                    consecutiveNegative = 0;
                    consecutiveNegativesSum = 0;
                }
                else if (value < 0)
                {
                    negativeCount++;

                    consecutiveNegative++;
                    consecutiveNegativesSum += value;

                    consecutivePositive = 0;
                    consecutivePositivesSum = 0;
                }
                _sum += value;

                _consecutivePositivesSum = Math.Max(_consecutivePositivesSum, consecutivePositivesSum);
                _consecutiveNegativesSum = Math.Min(_consecutiveNegativesSum, consecutiveNegativesSum);

                _consecutivePositives = Math.Max(_consecutivePositives, consecutivePositive);
                _consecutiveNegatives = Math.Max(_consecutiveNegatives, consecutiveNegative);
            }

            _positivePercentage = 100 * (double)positiveCount / (double)items.Length;
            _negativePercentage = 100 * (double)negativeCount / (double)items.Length;

            _averageValue = _sum / items.Length;
        }
    }
}
