using System;
using System.Windows.Forms;
using CommonSupport;
using ForexPlatform;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Base class for platform components controls.
    /// Inheritance from this control is optional.
    /// </summary>
    public partial class PlatformComponentControl : CommonBaseControl
    {
        PlatformComponent _component;
        public PlatformComponent Component
        {
            get { return _component; }
        }

        bool IsInherited
        {
            get
            {
                return this.GetType() != typeof(PlatformComponentControl);
            }
        }

        protected OpenForexPlatformBeta MasterForm
        {
            get
            {
                return (OpenForexPlatformBeta)Application.OpenForms[0];
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlatformComponentControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlatformComponentControl(PlatformComponent component)
        {
            InitializeComponent();

            if (this.DesignMode)
            {
                return;
            }

            _component = component;

            if (_component != null)
            {
                base._persistenceData = _component.UISerializationInfo;
                this.Name = _component.Name;
            }
        }

        private void PlatformComponentControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            SystemMonitor.CheckThrow(Tag == _component, "Tag is expected to be component.");

            labelStatus.Visible = this.IsInherited == false;

            if (this.IsInherited == false)
            {
                _component.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_component_OperationalStateChangedEvent);
                _component_OperationalStateChangedEvent(_component, OperationalStateEnum.Unknown);
            }

            if (_component != null)
            {
                this.Name = _component.Name;
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (this.DesignMode == false)
            {
                if (_component != null)
                {
                    this.Name = _component.Name;
                }
            }
        }

        void _component_OperationalStateChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            WinFormsHelper.BeginManagedInvoke(this, 
                delegate 
                {
                    labelStatus.Text = UserFriendlyNameAttribute.GetTypeAttributeName(operational.GetType()) + " is " + operational.OperationalState.ToString();
                });
        }

        /// <summary>
        /// Gives the control access to the application status strip.
        /// </summary>
        /// <param name="strip">Will be the application status strip or null for none.</param>
        public virtual void SetApplicationStatusStrip(StatusStrip strip)
        {
        }

        public override void UnInitializeControl()
        {
            if (_component != null)
            {
                _component = null;
            }

            base.UnInitializeControl();
        }



    }
}
