using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace CommonSupport
{
    /// <summary>
    /// Helper static class handling reflection operations.
    /// Make sure to use it extensively, since it contains some dynamic runtime reflection references as well.
    /// </summary>
    public static class ReflectionHelper
    {
        static Dictionary<Assembly, List<Assembly>> _dynamicReferencedAssemblies = new Dictionary<Assembly, List<Assembly>>();

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ReflectionHelper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        /// <summary>
        /// Domain manager has failed to find an assembly (probably a dynamically loaded one) so help him.
        /// Although the assembly is loaded, the manager fails to find it, since its a twat.
        /// </summary>
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GetName().FullName == args.Name)
                {
                    return assembly;
                }
            }
            return null;
        }

        /// <summary>
        /// Add a dynamicly referenced assembly.
        /// </summary>
        public static void AddDynamicReferencedAssembly(Assembly sourceAssembly, Assembly referencedAssembly)
        {
            lock (_dynamicReferencedAssemblies)
            {
                if (_dynamicReferencedAssemblies.ContainsKey(sourceAssembly) == false)
                {
                    _dynamicReferencedAssemblies.Add(sourceAssembly, new List<Assembly>());
                }

                if (_dynamicReferencedAssemblies[sourceAssembly].Contains(referencedAssembly) == false)
                {
                    _dynamicReferencedAssemblies[sourceAssembly].Add(referencedAssembly);
                }
            }
        }

        static public MethodInfo GetMethodInfo(GeneralHelper.DefaultDelegate delegateInstance)
        {
            return delegateInstance.Method;
        }

        static public MethodInfo GetMethodInfo<TParam1>(GeneralHelper.GenericDelegate<TParam1> delegateInstance)
        {
            return delegateInstance.Method;
        }

        static public MethodInfo GetMethodInfo<TParam1, TParam2>(GeneralHelper.GenericDelegate<TParam1, TParam2> delegateInstance)
        {
            return delegateInstance.Method;
        }

        static public MethodInfo GetMethodInfo<TParam1, TParam2, TParam3>(GeneralHelper.GenericDelegate<TParam1, TParam2, TParam3> delegateInstance)
        {
            return delegateInstance.Method;
        }

        /// <summary>
        /// Will return all the properties and methods of a given type, 
        /// that have the designated return type and take no parameter. Used in automated statistics.
        /// </summary>
        /// <param name="individualType"></param>
        /// <returns></returns>
        static public MethodInfo[] GetTypePropertiesAndMethodsByReturnType(Type objectType, Type[] returnTypes)
        {
            List<MethodInfo> resultList = new List<MethodInfo>();

            MethodInfo[] allMethods = objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (MethodInfo methodInfo in allMethods)
            {
                bool doesMatchReturnType = false;
                foreach (Type type in returnTypes)
                {
                    if (methodInfo.ReturnType == type)
                    {
                        doesMatchReturnType = true;
                        break;
                    }
                }

                if (methodInfo.GetParameters().Length == 0 && doesMatchReturnType)
                {// No params, proper return type - this is the one.
                    resultList.Add(methodInfo);
                }
            }

            MethodInfo[] resultArray = new MethodInfo[resultList.Count];
            resultList.CopyTo(resultArray);
            return resultArray;
        }

        /// <summary>
        /// Will create for you the needed instances of the given type children types
        /// with DEFAULT CONSTRUCTORS only, no params.
        /// </summary>
        static public List<TypeRequired> GetTypeChildrenInstances<TypeRequired>(System.Reflection.Assembly assembly)
        {
            return GetTypeChildrenInstances<TypeRequired>(new Assembly[] { assembly });
        }

        /// <summary>
        /// 
        /// </summary>
        static public AttributeType GetTypeCustomAttributeInstace<AttributeType>(Type type, bool inherit)
            where AttributeType : class
        {
            object[] attributes = type.GetCustomAttributes(inherit);
            foreach (object attribute in attributes)
            {
                if (attribute.GetType() == typeof(AttributeType))
                {
                    return (AttributeType) attribute;
                }
            }

            return null;
        }

        ///// <summary>
        ///// See GetTypeMarkedWithCustomAttribute().
        ///// </summary>
        ///// <returns></returns>
        //static public bool IsTypeMarkedWithCustomAttribute(Type type, Type attributeType, bool inherit)
        //{
        //    return IsTypeMarkedWithCustomAttribute(type, attributeType, inherit);
        //}

        /// <summary>
        /// Helper, establish if the given type is marked with this custom attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        static public bool IsTypeMarkedWithCustomAttribute(Type type, Type attributeType, bool inherit)
        {
            object[] attributes = type.GetCustomAttributes(attributeType, inherit);
            return attributes != null && attributes.Length > 0;
        }

        /// <summary>
        /// Will create for you the needed instances of the given type children types
        /// with DEFAULT CONSTRUCTORS only, no params.
        /// </summary>
        static public List<TypeRequired> GetTypeChildrenInstances<TypeRequired>(IEnumerable<Assembly> assemblies)
        {
            List<TypeRequired> resultingInstances = new List<TypeRequired>();

            foreach (Assembly assembly in assemblies)
            {
                // Collect all the types that match the description
                List<Type> blockTypes = ReflectionHelper.GetTypeChildrenTypes(typeof(TypeRequired), assembly);

                foreach (Type blockType in blockTypes)
                {
                    System.Reflection.ConstructorInfo[] constructorInfo = blockType.GetConstructors();

                    if (constructorInfo == null || constructorInfo.Length == 0 ||
                        blockType.IsAbstract || blockType.IsClass == false)
                    {
                        continue;
                    }

                    resultingInstances.Add((TypeRequired)constructorInfo[0].Invoke(null));
                }
            }

            return resultingInstances;
        }

        //static public int GetEnumValueIndex(System.Type enumType, string valueName)
        //{
        //    string[] names = System.Enum.GetNames(enumType);
        //    for (int i = 0; i < names.Length; i++)
        //    {
        //        if (names[i] == valueName)
        //        {
        //            return i;
        //        }
        //    }
        //    throw new Exception("Invalid enum value name passed in.");
        //}

        /// <summary>
        /// Loads all the assemblies in the directory of the executing (entry) assembly and searches 
        /// them for inheritors of the given type. SLOW!
        /// </summary>
        /// <returns></returns>
        //static public List<Type> GetCollectTypeChildrenTypesFromRelatedAssemblies(Type typeSearched)
        //{
        //    List<Type> result = new List<Type>();

        //    // Load all the assemblies in the directory of the current application and try to find
        //    // inheritors of AIndividual in them, then gather those in the list.
        //    string path = Assembly.GetEntryAssembly().Location;
        //    path = path.Remove(path.LastIndexOf('\\'));
        //    string[] dllFiles = System.IO.Directory.GetFiles(path, "*.dll");

        //    foreach (string file in dllFiles)
        //    {
        //        Assembly assembly;
        //        try
        //        {
        //            assembly = Assembly.LoadFile(file);
        //        }
        //        catch (Exception)
        //        {// This DLL was not a proper assembly, disregard.
        //            continue;
        //        }
        //        // Try to find typeSearched inheritors in this assembly.
        //        result.AddRange(ReflectionSupport.GetTypeChildrenTypes(typeSearched, assembly));
        //    }
        //    return result;
        //}

        /// <summary>
        /// Helper method allows to retrieve application entry assembly referenced (static and runtime) assemblies.
        /// </summary>
        static public List<Assembly> GetApplicationEntryAssemblyReferencedAssemblies()
        {
            return GetReferencedAssemblies(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Helper method allows to retrieve application entry assembly as well as its referenced (static and runtime) assemblies.
        /// </summary>
        static public List<Assembly> GetApplicationEntryAssemblyAndReferencedAssemblies()
        {
            return GetReferencedAndInitialAssembly(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Helper method allows to retrieve initial assembly and it referenced (static and runtime) assemblies.
        /// </summary>
        static public ListUnique<Assembly> GetReferencedAndInitialAssembly(Assembly initialAssembly)
        {
            ListUnique<Assembly> assemblies = GetReferencedAssemblies(initialAssembly);
            assemblies.Add(initialAssembly);
            return assemblies;
        }

        /// <summary>
        /// Helper method allows to retrieve initial assembly referenced (static and runtime) assemblies.
        /// </summary>
        static public ListUnique<Assembly> GetReferencedAssemblies(Assembly initialAssembly)
        {
            ListUnique<Assembly> result = new ListUnique<Assembly>();

            AssemblyName[] names = initialAssembly.GetReferencedAssemblies();
            for (int i = 0; i < names.Length; i++)
            {
                result.Add(Assembly.Load(names[i]));
            }

            lock(_dynamicReferencedAssemblies)
            {
                if (_dynamicReferencedAssemblies.ContainsKey(initialAssembly))
                {
                    result.AddRange(_dynamicReferencedAssemblies[initialAssembly]);
                }
            }

            return result;
        }

        /// <summary>
        /// Helper method, allows to retrieve a list of children types to the parent type, from list of referenced assemblies.
        /// </summary>
        static public List<Type> GatherTypeChildrenTypesFromAssemblies(Type parentType, IEnumerable<Assembly> assemblies)
        {
            return GatherTypeChildrenTypesFromAssemblies(parentType, assemblies, false, true);
        }

        /// <summary>
        /// This will look for children types in Entry, Current, Executing, Calling assemly, 
        /// as well as assemblies with names specified and found in the directory of the current application.
        /// </summary>
        /// <returns></returns>
        static public List<Type> GatherTypeChildrenTypesFromAssemblies(Type parentType, IEnumerable<Assembly> assemblies, bool allowOnlyClasses, bool allowAbstracts)
        {
            List<Type> resultingTypes = new List<Type>();
            if (assemblies == null)
            {
                return resultingTypes;
            }

            foreach (Assembly assembly in assemblies)
            {
                List<Type> types = ReflectionHelper.GetTypeChildrenTypes(parentType, assembly);
                foreach (Type type in types)
                {
                    if ((allowOnlyClasses == false || type.IsClass)
                        && (allowAbstracts || type.IsAbstract == false))
                    {
                        resultingTypes.Add(type);
                    }
                }
            }

            return resultingTypes;
        }

        /// <summary>
        /// Extended baseMethod, allows to specify the requrired constructor parameters.
        /// </summary>
        static public List<Type> GatherTypeChildrenTypesFromAssemblies(Type parentType, bool exactParameterTypeMatch, 
            bool allowAbstract, IEnumerable<Assembly> assemblies, Type[] constructorParametersTypes)
        {
            List<Type> candidateTypes = GatherTypeChildrenTypesFromAssemblies(parentType, assemblies);
            List<Type> resultingTypes = new List<Type>();

            if (constructorParametersTypes == null)
            {
                constructorParametersTypes = new Type[] { };
            }

            foreach (Type type in candidateTypes)
            {
                ConstructorInfo constructorInfo = type.GetConstructor(constructorParametersTypes);
                if (constructorInfo != null)
                {
                    bool isValid = true;
                    if (allowAbstract == false)
                    {
                        isValid = type.IsAbstract == false && type.IsClass == true;
                    }

                    if (isValid && exactParameterTypeMatch)
                    {// Perform check for exact type match, evade parent classes.

                        ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
                        for (int i = 0; i < parameterInfos.Length; i++)
                        {
                            if (parameterInfos[i].ParameterType != constructorParametersTypes[i])
                            {// Not good, skip.
                                isValid = false;
                                break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        resultingTypes.Add(type);
                    }
                }
            }

            return resultingTypes;
        }

        /// <summary>
        /// SLOW, see GetCallingMethod(); provides the original calling method outside the owner type
        /// (if such a method exists in the call stack).
        /// </summary>
        /// <param name="stackPop"></param>
        /// <returns></returns>
        public static MethodBase GetExternalCallingMethod(int stackPop, List<Type> ownerTypesIgnored)
        {
            MethodBase baseMethod = ReflectionHelper.GetCallingMethod(stackPop);
            MethodBase method = baseMethod;
            
            int methodIndex = stackPop + 1;
            while (ownerTypesIgnored.Contains(method.DeclaringType))
            {// All calls from system monitor get traced back one additional step backwards.

                method = ReflectionHelper.GetCallingMethod(methodIndex);
                methodIndex++;

                if (method == null)
                {
                    method = baseMethod;
                    break;
                }
            }

            return method;
        }

        /// <summary>
        /// SLOW, retrieves information for a calling baseMethod using the stack.
        /// </summary>
        /// <param name="stackPop">How many steps up the stack to take, before providing the baseMethod info.</param>
        public static MethodBase GetCallingMethod(int stackPop)
        {
            StackTrace _callStack = new StackTrace(); // The call stack  
            if (stackPop >= _callStack.FrameCount)
            {
                return null;
            }

            StackFrame frame = _callStack.GetFrame(stackPop); // The frame that called me.
            return frame.GetMethod(); // The baseMethod that called me.
        }

        /// <summary>
        /// SLOW, retrieves full name for a calling baseMethod using the stack.
        /// </summary>
        /// <param name="stackPop">How many steps up the stack to take, before providing the baseMethod info.</param>
        public static string GetFullCallingMethodName(int stackPop)
        {
            StackTrace _callStack = new StackTrace(); // The call stack  
            StackFrame frame = _callStack.GetFrame(stackPop); // The frame that called me
            MethodBase method = frame.GetMethod(); // The baseMethod that called me

            string assemblyName = method.DeclaringType.Assembly.GetName().Name;

            return assemblyName + "." + method.DeclaringType.Name + "." + method.Name;
        }

        /// <summary>
        /// Collect them from a given assembly.
        /// </summary>
        static public List<Type> GetTypeChildrenTypes(Type typeSearched, System.Reflection.Assembly assembly)
        {
            List<Type> result = new List<Type>();
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                string message = string.Empty;
                foreach (Exception subEx in ex.LoaderExceptions)
                {
                    message += "{" + GeneralHelper.GetExceptionMessage(subEx) + "}";
                }

                SystemMonitor.OperationError("Failed to load assembly types for [" + typeSearched.Name + ", " + assembly.GetName().Name + "] [" + GeneralHelper.GetExceptionMessage(ex) + ", " + message + "].");
                return result;
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError("Failed to load assembly types for [" + typeSearched.Name + ", " + assembly.GetName().Name + "] [" + GeneralHelper.GetExceptionMessage(ex) + "].");
                return result;
            }

            foreach (Type type in types)
            {
                if (typeSearched.IsInterface)
                {
                    List<Type> interfaces = new List<Type>(type.GetInterfaces());
                    if (interfaces.Contains(typeSearched))
                    {
                        result.Add(type);
                    }
                }
                else
                if (type.IsSubclassOf(typeSearched))
                {
                    result.Add(type);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <param name="bindingFlags">For ex. : BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance</param>
        /// <param name="checkParents"></param>
        /// <returns></returns>
        static public List<MethodInfo> GatherTypeMethodsByAttribute(Type inputType, Type attributeType, 
            BindingFlags bindingFlags, bool processParentTypes)
        {
            List<MethodInfo> result = new List<MethodInfo>();

            Type currentType = inputType;

            while (currentType != typeof(object))
            {// Gather current type members, but also gather parents private types, since those will not be available to the child class and will be missed.
                // Also not that the dictionary key mechanism throws if same baseMethod is entered twise - so it is a added safety feature.

                foreach (MethodInfo methodInfo in currentType.GetMethods())
                {
                    object[] customAttributes = methodInfo.GetCustomAttributes(false);

                    if (currentType != inputType && methodInfo.IsPrivate == false)
                    {// Since this is one of the members of the parent classes, make sure to just gather privates.
                        // because all of the parent's protected and public methods are available from the child class.
                        continue;
                    }

                    foreach (object attribute in customAttributes)
                    {
                        if (attribute.GetType() == attributeType)
                        {
                            if (result.Contains(methodInfo) == false)
                            {
                                result.Add(methodInfo);
                            }
                        }
                    }
                }

                currentType = currentType.BaseType;
                if (processParentTypes == false)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Does the type implement the specified interface type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool IsTypeImplementingInterface(Type type, Type interfaceType)
        {
            foreach (Type implementedInterfaceType in type.GetInterfaces())
            {
                if (implementedInterfaceType == interfaceType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the value dynamically from a non-typed object, by trying to access a method or 
        /// property with this name.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="methodName"></param>
        /// <returns>Null if the method was not found.</returns>
        public static object GetDynamicValue(object source, string methodName, params object[] parameters)
        {
            Type[] parametersTypes = null;
            if (parameters != null && parameters.Length > 0)
            {
                parametersTypes = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    parametersTypes[i] = parameters[i].GetType();
                }
            }

            Type type = source.GetType();

            MethodInfo mi;
            if (parametersTypes != null)
            {
                mi = type.GetMethod(methodName, parametersTypes);
            }
            else
            {
                mi = type.GetMethod(methodName);
            }

            if (mi != null)
            {
                return mi.Invoke(source, parameters);
            }

            PropertyInfo pi = type.GetProperty(methodName);
            if (pi == null)
            {
                return null;
            }

            return pi.GetValue(source, null);
        }
    }
}

