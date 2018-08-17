using System;
using System.Collections.Generic;
using System.Text;

namespace CommonFinancial
{
    public class EvaluativeOccurenceResultSet
    {
        public int StartingIndex;
        public int IndexCount;
        public double[] Values;

        public enum CombinationMethodEnum
        {
            Binary, // 0 or 1
            AbsSumming, // X OR Y
            Multiplication // X AND Y
        }

        /// <summary>
        /// 
        /// </summary>
        public EvaluativeOccurenceResultSet(int startingIndex, int indexCount)
        {
            StartingIndex = startingIndex;
            IndexCount = indexCount;
            Values = new double[StartingIndex + indexCount];
        }

        /// <summary>
        /// This automated multiplication of all the values in the resulting array.
        /// </summary>
        public void MultiplyValuesBy(double multiplicator)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] *= multiplicator;
            }
        }

        /// <summary>
        /// Will verify all the values in the Values are positive or zero (>=0), as this is expected from the platform.
        /// </summary>
        public bool CheckPositiveValues()
        {
            foreach (double value in Values)
            {
                if (value < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultFilterOneOrZero">Specify if all results are in 0 or 1 range.</param>
        public static EvaluativeOccurenceResultSet CreateCombinedSet(CombinationMethodEnum combinationMethod, bool resultFilterOneOrZero, EvaluativeOccurenceResultSet[] occurenceSets, int startingIndex, int count)
        {
            //System.Diagnostics.Debug.Fail("FIX ME");

            EvaluativeOccurenceResultSet resultingOccurenceSet = new EvaluativeOccurenceResultSet(startingIndex, count);

            // Result combination.
            foreach (EvaluativeOccurenceResultSet evaluativeOccurenceSet in occurenceSets)
            {
                // Perform combination.
                for (int actualIndex = 0; actualIndex < startingIndex + count; actualIndex++)
                {
                    if (combinationMethod == CombinationMethodEnum.AbsSumming)
                    {
                        resultingOccurenceSet.Values[actualIndex] += Math.Abs(evaluativeOccurenceSet.Values[actualIndex]);
                    }
                    else if (combinationMethod == CombinationMethodEnum.Multiplication)
                    {
                        resultingOccurenceSet.Values[actualIndex] *= evaluativeOccurenceSet.Values[actualIndex];
                    }

                    if (resultFilterOneOrZero && resultingOccurenceSet.Values[actualIndex] != 0)
                    {
                        resultingOccurenceSet.Values[actualIndex] = 1;
                    }
                }
            }

            return resultingOccurenceSet;
        }
    }
}
