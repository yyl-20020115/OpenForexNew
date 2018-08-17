using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;


namespace CommonFinancial
{
    /// <summary>
    /// Results are established from start index with count, there is nothing additionaly added in the beggining.
    /// </summary>
    [Serializable]
    public abstract class Evaluator : IRanged
    {
        [NonSerialized]
        ExpertSession _session;
        public ExpertSession Session
        {
            get { return _session; }
        }

        public abstract string FullName
        {
            get;
        }

        public int MaxRange
        {
            get { return Session.DataProvider.DataBars.BarCount; }
        }

        int _startingIndex = 0;
        public int StartIndex
        {
            get { return _startingIndex; }
            set { _startingIndex = value; }
        }

        int _count = 0;
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Evaluator()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Initialize(ExpertSession session)
        {
            lock (this)
            {
                _session = session;
                _count = session.DataProvider.DataBars.BarCount;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void UnInitialize()
        {
            lock (this)
            {
                _session = null;
            }
        }

        /// <summary>
        /// Calculation helper function.
        /// Evaluator will always calculate for the entire range and evaluate for the limited range.
        /// </summary>
        public void Calculate()
        {
            OnCalculate(0, Session.DataProvider.DataBars.BarCount);
        }

        protected abstract void OnCalculate(int startingIndex, int indecesCount);

        /// <summary>
        /// Will evaluate the given IEvaluative instance agains this Evaluator and provide the result.
        /// </summary>
        /// <returns></returns>
        public abstract EvaluationResultSet Evaluate(EvaluativeOccurence evaluative);


    }
}
