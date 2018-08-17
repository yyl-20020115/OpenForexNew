//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CommonSupport
//{
//    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
//    public class TerminalAttribute : Attribute
//    {
//        public TerminalAttribute()
//        {
//        }

//        public TerminalAttribute(int weight, bool isBlank)
//        {
//            _weight = weight;
//            _isBlank = isBlank;
//        }
        
//        public TerminalAttribute(int weight)
//        {
//            _weight = weight;
//        }

//        bool _isBlank = false;
//        /// <summary>
//        /// Blanks mean - execute nothing.
//        /// </summary>
//        public bool IsBlank
//        {
//            get { return _isBlank; }
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
//    }
//}
