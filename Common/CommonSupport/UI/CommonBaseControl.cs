using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// Common base class for all user controls used troughout the system.
    /// </summary>
    public partial class CommonBaseControl : UserControl
    {
        protected string _imageName = "";
        public virtual string ImageName
        {
            get { return _imageName; }
            set { _imageName = value; }
        }

        protected SerializationInfoEx _persistenceData = null;
        public SerializationInfoEx PersistenceData
        {
            get { return _persistenceData; }
        }

        /// <summary>
        /// Contructor.
        /// </summary>
        public CommonBaseControl()
        {
            InitializeComponent();
            this.Name = this.GetType().Name;
        }

        /// <summary>
        /// Instructs the control to save its state data to the PersistenceData container. May be a call from some management component.
        /// </summary>
        public virtual void SaveState()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
        }

        /// <summary>
        /// Perform unloading here.
        /// </summary>
        public virtual void UnInitializeControl()
        {
            Tag = null;
        }

        #region Static

        /// <summary>
        /// Helper, see CreateCorrespondingControl(object component, bool allowComponentBaseTypes)
        /// </summary>
        static public CommonBaseControl CreateCorrespondingControl(object component)
        {
            return CreateCorrespondingControl(component, false);
        }

        /// <summary>
        /// Will create a control using reflection, corresponding to the object passed it. In order for the control to be recognized
        /// it must take as a constructor paramterer the type passed in.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="allowComponentBaseTypes">This indicates whether the search for corresponding control should also cover parent types of the given type</param>
        /// <returns></returns>
        static public CommonBaseControl CreateCorrespondingControl(object component, bool allowComponentBaseTypes)
        {
            Type componentType = component.GetType();
            ListUnique<Assembly> assemblies = ReflectionHelper.GetReferencedAndInitialAssembly(Assembly.GetEntryAssembly());
            assemblies.Add(Assembly.GetAssembly(componentType));
            List<Type> types = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(typeof(CommonBaseControl), true, false, assemblies, new Type[] { componentType });

            if (types.Count == 0 && allowComponentBaseTypes)
            {
                while (componentType != typeof(object) && types.Count == 0)
                {
                    componentType = componentType.BaseType;
                    types = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(typeof(CommonBaseControl), true, false, assemblies, new Type[] { componentType });
                }
            }

            if (types.Count == 0)
            {// Type not found.
                return null;
            }

            string typesNames = string.Empty;
            if (types.Count > 1)
            {
                foreach (Type type in types)
                {
                    typesNames += type.Name + "();";
                }

                SystemMonitor.CheckWarning(types.Count == 1, "More than 1 control found for this type [" + component.GetType().Name + "][" + typesNames + "] of component, creating the first one.");
            }

            // Return the first proper object.
            CommonBaseControl control = (CommonBaseControl)types[0].GetConstructor(new Type[] { componentType }).Invoke(new object[] { component });
            control.Tag = component;
            return control;
        }

        #endregion


    }
}
