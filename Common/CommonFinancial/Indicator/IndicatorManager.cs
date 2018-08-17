using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Collections.ObjectModel;

namespace CommonFinancial
{
    /// <summary>
    /// Manages the operation and storage of trading indicators.
    /// </summary>
    [Serializable]
    public class IndicatorManager
    {
        List<Evaluator> _evaluators = new List<Evaluator>();
        /// <summary>
        /// All evaluators currently assigned to this session.
        /// </summary>
        public ReadOnlyCollection<Evaluator> Evaluators
        {
            get { lock (this) { return _evaluators.AsReadOnly(); } }
        }

        ListUnique<Indicator> _indicators = new ListUnique<Indicator>();
        /// <summary>
        /// All indicators currently assigned to this session.
        /// </summary>
        public ReadOnlyCollection<Indicator> IndicatorsUnsafe
        {
            get { lock (this) { return _indicators.AsReadOnly(); } }
        }

        /// <summary>
        /// 
        /// </summary>
        public Indicator[] IndicatorsArray
        {
            get { lock (this) { return _indicators.ToArray(); } }
        }

        IDataBarHistoryProvider _dataBarProvider;

        public delegate void IndicatorUpdateDelegate(IndicatorManager session, Indicator indicator);

        [field: NonSerialized]
        public event IndicatorUpdateDelegate IndicatorAddedEvent;

        [field: NonSerialized]
        public event IndicatorUpdateDelegate IndicatorRemovedEvent;

                /// <summary>
        /// Uninitialization of indicators occurs both - on removing it and on uninit of the session.
        /// Note that remove of indicator does not occur on uninit of the session, due to serialization needs.
        /// It is prefferably to catch this instead of the remove.
        /// </summary>
        [field: NonSerialized]
        public event IndicatorUpdateDelegate IndicatorUnInitializedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public IndicatorManager(IDataBarHistoryProvider dataProvider)
        {
            _dataBarProvider = dataProvider;
        }

        public bool Initialize()
        {
            //foreach (Evaluator evaluator in _evaluators)
            //{
            //    evaluator.Initialize(this);
            //}

            foreach (Indicator indicator in _indicators)
            {
                indicator.Initialize(_dataBarProvider);
            }

            return true;
        }

        public void UnInitialize()
        {
            foreach (Indicator indicator in _indicators)
            {
                indicator.UnInitialize();
            }

        }

        protected void UnInitializeIndicator(Indicator indicator)
        {
            indicator.UnInitialize();

            if (IndicatorUnInitializedEvent != null)
            {
                IndicatorUnInitializedEvent(this, indicator);
            }
        }


        /// <summary>
        /// Add a new indicator.
        /// </summary>
        /// <param name="indicator"></param>
        /// <returns></returns>
        public bool AddIndicator(Indicator indicator)
        {
            if (indicator.Initialize(_dataBarProvider) == false)
            {
                indicator.Dispose();
                return false;
            }

            indicator.Calculate(true, null);
            bool addResult;
            lock (this)
            {
                addResult = _indicators.Add(indicator);
            }

            if (addResult && IndicatorAddedEvent != null)
            {
                IndicatorAddedEvent(this, indicator);
            }

            return true;
        }

        public void RemoveIndicator(Indicator indicator)
        {
            indicator.UnInitialize();

            bool removeResult;
            lock (this)
            {
                removeResult = _indicators.Remove(indicator);
            }

            if (IndicatorUnInitializedEvent != null)
            {
                IndicatorUnInitializedEvent(this, indicator);
            }

            if (removeResult && IndicatorRemovedEvent != null)
            {
                IndicatorRemovedEvent(this, indicator);
            }
        }

        /// <summary>
        /// Keep in mind many indicators with the same name can be present at the same time, 
        /// so this will return the first found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Indicator GetFirstIndicatorByName(string name)
        {
            lock (this)
            {
                foreach (Indicator indicator in _indicators)
                {
                    if (indicator.Name.ToLower() == name.ToLower())
                    {
                        return indicator;
                    }
                }
            }

            return null;
        }


    }
}
