using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// Manages automated additional system functionalities applied to many or all of the controls in an application.
    /// The managed is expected to work in a single UI thread cals, so no thread safety is applied.
    /// </summary>
    public class ApplicationControlAutomationManager: IDisposable
    {
        /// <summary>
        /// Controls subscribed for event handling.
        /// </summary>
        List<Control> _controls = new List<Control>();

        /// <summary>
        /// Those are handlers coming from the usage of attributes in classes.
        /// </summary>
        Dictionary<Type, AutomatedControlEventHandler> _attributeHandlers = new Dictionary<Type, AutomatedControlEventHandler>();
        
        /// <summary>
        /// Those are manually added/assigned handlers; control specific handlers, collection is based on control type.
        /// </summary>
        Dictionary<Type, List<AutomatedControlEventHandler>> _controlTypeHandlers = new Dictionary<Type, List<AutomatedControlEventHandler>>();

        /// <summary>
        /// 
        /// </summary>
        public ApplicationControlAutomationManager(Control topLevelControl)
        {
            SystemMonitor.CheckWarning(topLevelControl.Parent == null, "Application control automation manager is expected to start at top level control; some controls may be missed.");
            RegisterControl(topLevelControl);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            while (_controls.Count > 0)
            {
                UnRegisterControl(_controls[_controls.Count - 1]);
            }

            _attributeHandlers.Clear();
            _controlTypeHandlers.Clear();
        }

        /// <summary>
        /// This allows manual registration of handlers for given control type. Useful when control 
        /// type can not be assigned an attribute.
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="handler"></param>
        public bool RegisterControlHandler(Type controlType, AutomatedControlEventHandler handler)
        {
            if (_controlTypeHandlers.ContainsKey(controlType) == false)
            {
                _controlTypeHandlers.Add(controlType, new List<AutomatedControlEventHandler>());
            }

            _controlTypeHandlers[controlType].Add(handler);
            return true;
        }

        /// <summary>
        /// Unregister manuall assigned control type handler.
        /// </summary>
        public bool UnRegisterControlHandler(Type controlType, AutomatedControlEventHandler handler)
        {
            if (_controlTypeHandlers.ContainsKey(controlType) == false)
            {
                _controlTypeHandlers.Add(controlType, new List<AutomatedControlEventHandler>());
            }

            return _controlTypeHandlers[controlType].Remove(handler);
        }


        void RegisterControl(Control control)
        {
            if (_controls.Contains(control))
            {
                SystemMonitor.Warning("Registered control type already registered not found [ " + control.Name + "]");
                return;
            }

            _controls.Add(control);

            control.ControlAdded += new ControlEventHandler(control_ControlAdded);
            control.ControlRemoved += new ControlEventHandler(control_ControlRemoved);

            control.KeyDown += new KeyEventHandler(control_KeyDown);
            control.KeyUp += new KeyEventHandler(control_KeyUp);
            control.KeyPress += new KeyPressEventHandler(control_KeyPress);

            foreach (Control childControl in control.Controls)
            {
                RegisterControl(childControl);
            }
        }

        void UnRegisterControl(Control control)
        {
            if (_controls.Remove(control) == false)
            {
                SystemMonitor.Warning("Unregistered control type not found [ " + control.Name + "]");
                return;
            }

            control.ControlAdded -= new ControlEventHandler(control_ControlAdded);
            control.ControlRemoved -= new ControlEventHandler(control_ControlRemoved);

            control.KeyDown -= new KeyEventHandler(control_KeyDown);
            control.KeyUp -= new KeyEventHandler(control_KeyUp);
            control.KeyPress -= new KeyPressEventHandler(control_KeyPress);

            foreach (Control childControl in control.Controls)
            {
                UnRegisterControl(childControl);
            }
        }

        List<AutomatedControlEventHandler> GetControlHandlers(Control control)
        {
            List<AutomatedControlEventHandler> result = new List<AutomatedControlEventHandler>();

            // Check attribute handlers.
            Type[] handlerTypes = AutomatedControlEventHandlerAttribute.GetClassHandlerTypes(control.GetType());
            foreach (Type handlerType in handlerTypes)
            {
                if (_attributeHandlers.ContainsKey(handlerType) == false)
                {
                    object handler = Activator.CreateInstance(handlerType);
                    if (handler == null)
                    {
                        SystemMonitor.Warning("Failed to create, with default constructor, handler of type [" + handlerType.Name + "]");
                        continue;
                    }
                    _attributeHandlers.Add(handlerType, (AutomatedControlEventHandler)handler);
                }
                result.Add(_attributeHandlers[handlerType]);
            }

            // Chech manually assigned handlers.
            foreach (Type type in _controlTypeHandlers.Keys)
            {
                if (control.GetType() == type || control.GetType().IsSubclassOf(type))
                {
                    result.AddRange(_controlTypeHandlers[type]);
                }
            }

            return result;
        }

        void control_ControlAdded(object sender, ControlEventArgs e)
        {
            RegisterControl(e.Control);
        }

        void control_ControlRemoved(object sender, ControlEventArgs e)
        {
            UnRegisterControl(e.Control);
        }

        void control_KeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (AutomatedControlEventHandler handler in GetControlHandlers((Control)sender))
            {
                handler.KeyPress((Control)sender, e);
            }
        }

        void control_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (AutomatedControlEventHandler handler in GetControlHandlers((Control)sender))
            {
                handler.KeyUp((Control)sender, e);
            }
        }

        void control_KeyDown(object sender, KeyEventArgs e)
        {
            foreach (AutomatedControlEventHandler handler in GetControlHandlers((Control)sender))
            {
                handler.KeyDown((Control)sender, e);
            }
        }


    }
}
