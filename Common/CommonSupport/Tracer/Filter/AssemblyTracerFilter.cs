//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Reflection;
//using System.Collections.ObjectModel;

//namespace CommonSupport
//{
//    /// <summary>
//    /// Filter for tracer item based on assembly of origin.
//    /// </summary>
//    [Serializable]
//    class AssemblyTracerFilter : TracerFilter
//    {
//        List<Assembly> _filteredOutAssemblies = new List<Assembly>();
//        /// <summary>
//        /// 
//        /// </summary>
//        public Assembly[] FilteredOutAssembliesArray
//        {
//            get { lock (this) { return _filteredOutAssemblies.ToArray(); } }
//        }

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="tracer"></param>
//        public AssemblyTracerFilter(Tracer tracer)
//            : base(tracer)
//        {
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public void SetAssemblyFiltered(Assembly assembly, bool filtered)
//        {
//            lock (this)
//            {
//                if (filtered)
//                {
//                    if (_filteredOutAssemblies.Contains(assembly) == false)
//                    {
//                        _filteredOutAssemblies.Add(assembly);
//                    }
//                }
//                else
//                {
//                    _filteredOutAssemblies.Remove(assembly);
//                }
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override bool FilterItem(TracerItem item)
//        {
//            foreach (Assembly assembly in FilteredOutAssembliesArray)
//            {
//                if (item.Assembly == assembly)
//                {
//                    return false;
//                }
//            }
//            return true;
//        }
//    }
//}
