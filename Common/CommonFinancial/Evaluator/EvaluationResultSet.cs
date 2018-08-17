using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CommonSupport;

namespace CommonFinancial
{
    public class EvaluationResultSet
    {
        public class EvaluationEntry
        {
            long _index = 0;
            [ItemStatisticsAttribute]
            public long Index
            {
                get { return _index; }
                set { _index = value; }
            }

            double _value = 0;
            [ItemStatisticsAttribute]
            public double Value
            {
                get { return _value; }
                set { _value = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public EvaluationEntry(long index, double value)
            {
                _index = index;
                _value = value;
            }
        }

        List<EvaluationEntry> _positiveEntries = new List<EvaluationEntry>();
        public List<EvaluationEntry> PositiveEntries
        {
            get { return _positiveEntries; }
        }

        List<EvaluationEntry> _negativeEntries = new List<EvaluationEntry>();
        public List<EvaluationEntry> NegativeEntries
        {
            get { return _negativeEntries; }
        }

        EvaluativeOccurence _evaluativeItem;
        public EvaluativeOccurence EvaluativeItem
        {
            get { return _evaluativeItem; }
        }

        public enum SortResultsEnum
        {
            OriginalOrder,
            Average,
            Percentage,
            Sum,
            Count
        }

        /// <summary>
        /// 
        /// </summary>
        public EvaluationResultSet(EvaluativeOccurence evaluativeItem)
        {
            _evaluativeItem = evaluativeItem;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _positiveEntries.Clear();
            _negativeEntries.Clear();
        }

        public double CalculatePositiveSum()
        {
            double sum = 0;
            foreach(EvaluationEntry entry in _positiveEntries)
            {
                sum += entry.Value;
            }
            return sum;
        }

        public double CalculateNegativeSum()
        {
            double sum = 0;
            foreach (EvaluationEntry entry in _negativeEntries)
            {
                sum += entry.Value;
            }
            return sum;
        }

        public double CalculatePositivePerPosition()
        {
            if (_positiveEntries.Count > 0)
            {
                return CalculatePositiveSum() / _positiveEntries.Count;
            }
            return 0;
        }

        public double CalculateNegativePerPosition()
        {
            if (_negativeEntries.Count > 0)
            {
                return CalculateNegativeSum() / _negativeEntries.Count;
            }

            return 0;
        }

        public double CalculatePositiveWinningPercentage()
        {
            if (PositiveEntries.Count == 0)
            {
                return 0;
            }

            int positiveWinning = 0;
            foreach (EvaluationEntry entry in _positiveEntries)
            {
                if (entry.Value > 0)
                {
                    positiveWinning++;
                }
            }

            return (float)positiveWinning * 100 / (float)this.PositiveEntries.Count;
        }


        public double CalcualteNegativeWinningPercentage()
        {
            if (this.NegativeEntries.Count == 0)
            {
                return 0;
            }

            int negativeWinning = 0;
            foreach(EvaluationEntry entry in _negativeEntries)
            {
                if (entry.Value < 0)
                {
                    negativeWinning++;
                }
            }

            return (float)negativeWinning * 100 / (float)this.NegativeEntries.Count;
        }

        public virtual string Print()
        {
            return
            "+[avrg:" + (float)CalculatePositivePerPosition() + " (" + (float)CalculatePositiveSum() + ", " + (float)_positiveEntries.Count + ") \t\t " +
            "-[avrg:" + (float)CalculateNegativePerPosition() + " (" + (float)CalculateNegativeSum() + ", " + (float)_negativeEntries.Count + ")";
        }

        /// <summary>
        /// Static baseMethod allowing us to sort result sets.
        /// </summary>
        static public EvaluationResultSet[] SortResultSets(IEnumerable<EvaluationResultSet> resultSets, 
            bool positiveSet, SortResultsEnum sortCriteria)
        {
            SortedDictionary<Decimal, EvaluationResultSet> dictionary = new SortedDictionary<Decimal, EvaluationResultSet>();
            foreach (EvaluationResultSet resultSet in resultSets)
            {
                // All the different values are coded in the double.
                // As the last part is a unique index, so that if values match, sorting will still work properly.
                double sortBasedValue = 0;
                if (sortCriteria == SortResultsEnum.OriginalOrder)
                {
                    sortBasedValue = dictionary.Count;
                }
                else if (sortCriteria == SortResultsEnum.Average)
                {
                    if (positiveSet)
                    {
                        sortBasedValue = resultSet.CalculatePositivePerPosition();
                    }
                    else
                    {
                        sortBasedValue = resultSet.CalculateNegativePerPosition();
                    }
                }
                else if (sortCriteria == SortResultsEnum.Count)
                {
                    if (positiveSet)
                    {
                        sortBasedValue = resultSet.PositiveEntries.Count;
                    }
                    else
                    {
                        sortBasedValue = resultSet.NegativeEntries.Count;
                    }
                }
                else if (sortCriteria == SortResultsEnum.Percentage)
                {
                    if (positiveSet)
                    {
                        sortBasedValue = resultSet.CalculatePositiveWinningPercentage();
                    }
                    else
                    {
                        sortBasedValue = resultSet.CalcualteNegativeWinningPercentage();
                    }
                }
                else if (sortCriteria == SortResultsEnum.Sum)
                {
                    if (positiveSet)
                    {
                        sortBasedValue = resultSet.CalculatePositiveSum();
                    }
                    else
                    {
                        sortBasedValue = resultSet.CalculateNegativeSum();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.Fail("Sorting case not handled.");
                }

                dictionary.Add(new Decimal(100000000 * sortBasedValue) + new Decimal(dictionary.Count), resultSet);
            }

            EvaluationResultSet[] result = new EvaluationResultSet[dictionary.Count];
            dictionary.Values.CopyTo(result, 0);
            return result;
        }
    }
}
