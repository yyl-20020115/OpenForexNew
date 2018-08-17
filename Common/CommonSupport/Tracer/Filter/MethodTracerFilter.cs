using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CommonSupport
{
    /// <summary>
    /// Tracer item filter, allows filtering based on originating assembly, class and (method)}.
    /// </summary>
    [Serializable]
    public class MethodTracerFilter : TracerFilter
    {
        public class AssemblyTracingInformation
        {
            public bool Enabled = true;
            public Dictionary<Type, bool> Types = new Dictionary<Type, bool>();
        }

        Dictionary<Assembly, AssemblyTracingInformation> _assemblies = new Dictionary<Assembly, AssemblyTracingInformation>();
        /// <summary>
        /// Thread safe access.
        /// </summary>
        public List<Assembly> Assemblies
        {
            get 
            { 
                lock (this) 
                {
                    return GeneralHelper.EnumerableToList<Assembly>(_assemblies.Keys);
                } 
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MethodTracerFilter()
        {
            foreach (Assembly assembly in ReflectionHelper.GetApplicationEntryAssemblyAndReferencedAssemblies())
            {
                object[] copyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
                if (copyright != null && copyright.Length > 0)
                {// Skip system assemblies.
                    AssemblyCopyrightAttribute attribute = (AssemblyCopyrightAttribute)copyright[0];
                    if (attribute.Copyright.Contains("Microsoft"))
                    {
                        continue;
                    }
                }

                _assemblies.Add(assembly, new AssemblyTracingInformation());
            }
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public MethodTracerFilter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }


        /// <summary>
        /// 
        /// </summary>
        public AssemblyTracingInformation GetAssemblyInfo(Assembly assembly)
        {
            lock (this)
            {
                if (_assemblies.ContainsKey(assembly))
                {
                    return _assemblies[assembly];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool FilterItem(TracerItem item)
        {
            if (item is MethodTracerItem == false)
            {
                return true;
            }

            bool updated = false;
            bool result = false;
            
            lock (this)
            {
                MethodTracerItem actualItem = (MethodTracerItem)item;
                if (_assemblies.ContainsKey(actualItem.MethodBase.DeclaringType.Assembly) == false)
                {// Add assembly.
                    _assemblies.Add(actualItem.MethodBase.DeclaringType.Assembly, new AssemblyTracingInformation());
                    updated = true;
                }

                if (_assemblies[actualItem.MethodBase.DeclaringType.Assembly].Enabled == false)
                {// Entire assembly stopped.
                    return false;
                }

                if (_assemblies[actualItem.MethodBase.DeclaringType.Assembly].Types.ContainsKey(actualItem.MethodBase.DeclaringType) == false)
                {// Add type.
                    _assemblies[actualItem.MethodBase.DeclaringType.Assembly].Types.Add(actualItem.MethodBase.DeclaringType, true);
                    updated = true;
                    result = true;
                }
                else
                {
                    return _assemblies[actualItem.MethodBase.DeclaringType.Assembly].Types[actualItem.MethodBase.DeclaringType];
                }
            }

            if (updated)
            {
                RaiseFilterUpdatedEvent(true);
            }

            return result;
        }
    }
}
