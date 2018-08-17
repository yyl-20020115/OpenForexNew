using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// Class helps with operations on system performance counters.
    /// </summary>
    public class PerformanceCounterHelper
    {
        static bool _countersAllowed = true;
        
        /// <summary>
        /// Some environments should not use those.
        /// </summary>
        public static bool CountersAllowed
        {
            get { return PerformanceCounterHelper._countersAllowed; }
            set { PerformanceCounterHelper._countersAllowed = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PerformanceCounterHelper()
        {
            //CounterCreationData xcounter = new CounterCreationData();
            //xcounter.CounterName = "Xtreme counter";
            //xcounter.CounterType = PerformanceCounterType.NumberOfItems32;
            //counters.Add(xcounter);

            //string categoryName = Application.ProductName + ".TestCounters1";
            //if (PerformanceCounterCategory.Exists(categoryName) == false)
            //{
            //    CounterCreationDataCollection counters = new CounterCreationDataCollection();

            //    PerformanceCounterCategory category = PerformanceCounterCategory.Create(categoryName, 
            //        "Category for the counters of " + Application.ProductName, PerformanceCounterCategoryType.SingleInstance,
            //        counters);

            //    CounterCreationData xcounter = new CounterCreationData();
            //    xcounter.CounterName = "Xtreme counter";
            //    xcounter.CounterType = PerformanceCounterType.NumberOfItems32;

            //    counters.Add(xcounter);

            //    //category.
            //}


            //PerformanceCounter _TotalOperations = new PerformanceCounter();
            //_TotalOperations.
            //_TotalOperations.CategoryName = "MyCategory";
            //_TotalOperations.CounterName = "Xtreme counter";
            //_TotalOperations.MachineName = ".";
            //_TotalOperations.ReadOnly = false;
        }

        public static PerformanceCounter CreateCounter(string name, string description, PerformanceCounterType type)
        {
            if (_countersAllowed == false)
            {
                return null;
            }

            string categoryName = Application.ProductName + "." + name;
            if (PerformanceCounterCategory.Exists(categoryName) == false)
            {
                CounterCreationDataCollection counters = new CounterCreationDataCollection();

                CounterCreationData counterData = new CounterCreationData();
                counterData.CounterName = name;
                counterData.CounterHelp = description;
                counterData.CounterType = type;

                counters.Add(counterData);

                PerformanceCounterCategory category = PerformanceCounterCategory.Create(categoryName, "Category for the counters of " + Application.ProductName, 
                    PerformanceCounterCategoryType.SingleInstance, counters);
            }


            PerformanceCounter counter = new PerformanceCounter(categoryName, name, string.Empty, false);
            return counter;
        }
    }
}
