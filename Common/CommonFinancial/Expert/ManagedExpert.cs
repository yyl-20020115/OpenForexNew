//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CommonFinancial
//{
//    /// <summary>
//    /// Managed expert notified this is manageable by the expert management utility.
//    /// It allows a simplified access to the abilities the platform provides and is the
//    /// best starting point for implementing a simple to medium complex trading strategy.
//    /// </summary>
//    public abstract class ManagedExpert : Expert
//    {
//        int _currentSessionIndex = -1;

//        ExpertSession CurrentSession
//        {
//            get { return null; }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public ManagedExpert(ISourceManager manager, string name)
//            : base(manager, name)
//        {
//        }

//        void SetCurrentSession()
//        {
//        }

//        /// <summary>
//        /// This will create the indicator if needed.
//        /// </summary>
//        /// <param name="indicatorName"></param>
//        /// <returns></returns>
//        protected Indicator GetIndicator(string name)
//        {
//            if (CurrentSession == null)
//            {
//                return null;
//            }

//            Indicator indicator = CurrentSession.GetFirstIndicatorByName(name);
            
//            if (indicator == null)
//            {
//                IndicatorManager
//            }
//        }
//    }
//}
