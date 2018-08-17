using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Server as UI component for the Extended Manual Trading Expert.
    /// This is auto create (using reflection) by the platform system in order to provide
    /// a UI for the correspoding expert class.
    /// </summary>
    public partial class ComponentTradingExpertControl : CommonBaseControl
    {
        ComponentTradingExpert _expert;

        public ComponentTradingExpert Expert
        {
            get { return _expert; }
            
            
            set 
            {
                if (_expert != null)
                {
                    if (_expert.Manager != null)
                    {
                        _expert.Manager.SessionsUpdateEvent -= new GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(SessionManager_SessionsUpdateEvent);
                    }
                    _expert.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(_expert_OperationalStatusChangedEvent);
                    _expert = null;
                }

                _expert = value;
                if (_expert != null)
                {
                    _expert.Manager.SessionsUpdateEvent += new GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(SessionManager_SessionsUpdateEvent);
                    _expert.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_expert_OperationalStatusChangedEvent);

                    if (_expert.Host.UISerializationInfo != null)
                    {
                        dragContainerControl1.RestoreState(_expert.Host.UISerializationInfo);
                    }

                    if (_expert.OperationalState == OperationalStateEnum.Operational)
                    {
                        // Match existing sessions to existing drag controls.
                        for (int i = 0; i < _expert.Manager.SessionCount && i < dragContainerControl1.DragControls.Count; i++)
                        {
                            PlatformExpertSession session = (PlatformExpertSession)_expert.Manager.SessionsArray[i];
                            PlatformExpertSessionControl sessionControl = new PlatformExpertSessionControl();
                            sessionControl.CreateControl();
                            sessionControl.Session = session;
                            DragControl control = dragContainerControl1.DragControls[i];
                            control.Text = session.Info.Name;
                            control.ControlContained = sessionControl;
                        }

                        // If there are some sessions with non existing controls - process them.
                        SessionManager_SessionsUpdateEvent(_expert.Manager);
                    }
                }
            }
        }

        /// <summary>
        /// Constructor for UI editor.
        /// </summary>
        public ComponentTradingExpertControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ComponentTradingExpertControl(ComponentTradingExpert expert)
        {
            InitializeComponent();
            
            this.CreateControl();
            Expert = expert;
        }

        /// <summary>
        /// Handle destruction of object to release any resources or links.
        /// </summary>
        /// <param name="e"></param>
        public override void UnInitializeControl()
        {
            Expert = null;
            base.UnInitializeControl();
        }

        /// <summary>
        /// Handle events assignment on load.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            dragContainerControl1.DragControlAdded += new DragContainerControl.DragControlUpdatedDelegate(dragContainerControl1_DragControlAdded);
            dragContainerControl1.DragControlRemoved += new DragContainerControl.DragControlUpdatedDelegate(dragContainerControl1_DragControlRemoved);
        }

        void _expert_OperationalStatusChangedEvent(IOperational expert, OperationalStateEnum previousState)
        {
            if (expert.OperationalState == OperationalStateEnum.Operational)
            {
                SessionManager_SessionsUpdateEvent(_expert.Manager);
            }

            if (previousState == OperationalStateEnum.Operational)
            {// Coming out of operational state - always persist.
                _expert.Host.UISerializationInfo = new SerializationInfoEx();
                dragContainerControl1.SaveState(_expert.Host.UISerializationInfo);
            }
        }

        
        DragControl GetDragControlBySessionInfo(DataSessionInfo sessionInfo)
        {
            foreach (DragControl control in dragContainerControl1.DragControls)
            {
                if (control.ControlContained is PlatformExpertSessionControl)
                {
                    if (((PlatformExpertSessionControl)((DragControl)control).ControlContained).Session.Info.Equals(sessionInfo))
                    {
                        return control;
                    }
                }
            }

            return null;
        }

        void dragContainerControl1_DragControlAdded(DragContainerControl container, DragControl control)
        {
            
        }
        
        /// <summary>
        /// Destroy sessionInformation upon closing its UI component.
        /// </summary>
        void dragContainerControl1_DragControlRemoved(DragContainerControl container, DragControl control)
        {
            if (control.ControlContained is PlatformExpertSessionControl)
            {
                _expert.Manager.UnRegisterExpertSession(((PlatformExpertSessionControl)control.ControlContained).Session);
            }
        }

        /// <summary>
        /// When there is update in sessions - synchronize UI components with available sessions.
        /// </summary>
        /// <param name="parameter1"></param>
        void SessionManager_SessionsUpdateEvent(ISourceAndExpertSessionManager parameter1)
        {
            foreach (PlatformExpertSession session in _expert.Manager.SessionsArray)
            {
                if (GetDragControlBySessionInfo(session.Info) != null)
                {
                    continue;
                }

                PlatformExpertSessionControl sessionControl = new PlatformExpertSessionControl();
                AddComponentControl(sessionControl, session.Info.Name);
                sessionControl.Session = session;
            }
        }

        /// <summary>
        /// Adds a control to a drag pane.
        /// </summary>
        /// <param name="control"></param>
        void AddComponentControl(Control controlContained, string text)
        {
            DragControl control = new DragControl();
            control.Text = text;
            control.ControlContained = controlContained;
            control.CreateControl();
            this.dragContainerControl1.AddDragControl(control);
        }

        /// <summary>
        /// To create new expert sessionInformation, show the creation Form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonCreate_Click(object sender, EventArgs e)
        {
            CreateExpertSessionForm form = new CreateExpertSessionForm((LocalExpertHost)_expert.Manager);
            form.ShowDialog();
        }

    }
}
