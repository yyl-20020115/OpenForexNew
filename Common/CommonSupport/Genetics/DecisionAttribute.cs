//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CommonSupport
//{
//    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
//    public class DecisionAttribute : Attribute
//    {
//        public DecisionAttribute(int expressionsCount)
//        {
//            _expressionsCount = expressionsCount;
//        }

//        public DecisionAttribute(int expressionsCount, int weight)
//        {
//            _expressionsCount = expressionsCount;
//            _weight = weight;
//        }

//        int _weight = 1;
//        /// <summary>
//        /// The weight of the attribute signifies how often it will be selected in the expression tree.
//        /// The higher the weight, the higher the chance for selection.
//        /// For ex. weight 5 means this will be selected as much as 5 others with weight 1 - all together.
//        /// </summary>
//        /// <param name="weight"></param>
//        public int Weight
//        {
//            get
//            {
//                return _weight;
//            }
//        }

//        int _expressionsCount;
//        /// <summary>
//        /// The number of possible results this decision provides.
//        /// </summary>
//        public int ExpressionsCount
//        {
//            get { return _expressionsCount; }
//        }

//    }
//}
