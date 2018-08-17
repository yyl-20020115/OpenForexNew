using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommonFinancial
{
    public class EvaluationResultSetFilter
    {
        bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        bool _combinedFiltering = false;
        public bool CombinedFiltering
        {
            get { return _combinedFiltering; }
            set { _combinedFiltering = value; }
        }

        public double PositivePerPosition;
        public double NegativePerPosition;
        
        public int PositiveCount;
        public int NegativeCount;
        
        public double PositiveSum;
        public double NegativeSum;

        int _totalItems = 0;
        public int TotalItems
        {
            get { return _totalItems; }
        }

        int _filteredOutItems = 0;
        public int FilteredOutItems
        {
            get { return _filteredOutItems; }
        }

        public enum FilteringResultEnum
        {
            Disabled,
            PositivePass,
            NegativePass,
            CombinedPass,
            Failed
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="analysisPlatform">The form to do testing against.</param>
        public EvaluationResultSetFilter()
        {
        }

        public static EvaluationResultSet[] FilterResultSets(IEnumerable<EvaluationResultSet> resultSets, EvaluationResultSetFilter filter)
        {
            List<EvaluationResultSet> resultResultSets = new List<EvaluationResultSet>();
            foreach (EvaluationResultSet resultSet in resultSets)
            {
                resultResultSets.Add(resultSet);
            }

            return resultResultSets.ToArray();
        }

        /// <summary>
        /// Will return true if instance failed the filtering conditions.
        /// </summary>
        public bool FilterOut(EvaluationResultSet evaluationResultSet)
        { 
            FilteringResultEnum result = FilterOutDetailed(evaluationResultSet);
            return !(result == FilteringResultEnum.Disabled || result == FilteringResultEnum.CombinedPass || result == FilteringResultEnum.PositivePass || result == FilteringResultEnum.NegativePass);
        }


        /// <summary>
        /// Will return true if the instance failed the filtering conditions.
        /// </summary>
        public FilteringResultEnum FilterOutDetailed(EvaluationResultSet evaluationResultSet)
        {// For a condition to fail, it has to fail to both - positive and negative check.

            if (Enabled == false)
            {// Filtering is disabled.
                return FilteringResultEnum.Disabled;
            }

            _totalItems++;

            bool positiveFailResult =  
                (evaluationResultSet.CalculatePositivePerPosition() < PositivePerPosition ||
                evaluationResultSet.CalculatePositiveSum() < PositiveSum ||
                evaluationResultSet.PositiveEntries.Count < PositiveCount );

            bool negativeFailResult =                 
                (evaluationResultSet.CalculateNegativePerPosition() > NegativePerPosition ||
                evaluationResultSet.CalculateNegativeSum() > NegativeSum ||
                evaluationResultSet.NegativeEntries.Count < NegativeCount);

            // By default - we need both to be failed, for the item to be flushed.
            bool failResult = positiveFailResult && negativeFailResult;

            if (CombinedFiltering)
            {// When combined mode is on, we need only one to be failed - as we require both for pass result.
                failResult = positiveFailResult || negativeFailResult;
            }
            
            if (failResult)
            {
                _filteredOutItems++;
            }

            if (!positiveFailResult && !negativeFailResult)
            {
                return FilteringResultEnum.CombinedPass;
            }

            if (!positiveFailResult)
            {
                return FilteringResultEnum.PositivePass;
            }

            if (!negativeFailResult)
            {
                return FilteringResultEnum.NegativePass;
            }

            return FilteringResultEnum.Failed;
        }

    }
}
