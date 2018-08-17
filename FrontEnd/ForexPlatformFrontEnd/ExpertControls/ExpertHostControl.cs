using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
//using Arbiter;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// The ActualExpertHostControl is the corresponding UI control to the ActualExpertHost class. 
    /// It provides a way for the expert host to appear as an UI element.
    /// </summary>
    public partial class ExpertHostControl : PlatformComponentControl
    {
        ExpertHost _expertHost;
        public ExpertHost ExpertHost
        {
            get { return _expertHost; }
        }

        CommonBaseControl _expertControl;

        /// <summary>
        /// 
        /// </summary>
        public ExpertHostControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public ExpertHostControl(LocalExpertHost expertHost)
            : base(expertHost)
        {
            InitializeComponent();

            Initialize(expertHost);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expertHost"></param>
        void Initialize(ExpertHost expertHost)
        {
            this.Name = expertHost.Name;
            
            _expertHost = expertHost;
            _expertHost.OperationalStateChangedEvent += new OperationalStateChangedDelegate(expertHost_OperationalStatusChangedEvent);
            
            // Create early here, to be able to use the image name.
            _expertControl = CommonBaseControl.CreateCorrespondingControl(_expertHost.Expert, true);
            if (_expertControl != null)
            {
                this.ImageName = _expertControl.ImageName;
            }

            //_host.SessionsUpdateEvent += new GeneralHelper.GenericDelegate<ISourceManager>(_expertHost_SessionsUpdateEvent);
            //_host.SourcesUpdateEvent += new GeneralHelper.GenericDelegate<ISourceManager>(_expertHost_SourcesUpdateEvent);
            //DoUpdateUI();
        }

        void expertHost_OperationalStatusChangedEvent(IOperational host, OperationalStateEnum previousState)
        {
        }

        private void ExpertHostControl_Load(object sender, EventArgs e)
        {
            if (_expertControl == null)
            {
                labelMain.Text = _expertHost.ExpertName + ", " + _expertHost.Expert.GetType().Name + " has no user interface component.";
                return;
            }

            _expertControl.CreateControl();
            _expertControl.Dock = DockStyle.Fill;
            _expertControl.Parent = this;
            _expertControl.BringToFront();
        }

        public override void SaveState()
        {
            if (_expertControl != null)
            {
                _expertControl.SaveState();
            }
        }

        public override void UnInitializeControl()
        {
            if (_expertControl != null)
            {
                _expertControl.UnInitializeControl();
            }

            if (_expertHost != null)
            {
                //_host.SessionsUpdateEvent -= new GeneralHelper.GenericDelegate<ISourceManager>(_expertHost_SessionsUpdateEvent);
                //_host.SourcesUpdateEvent -= new GeneralHelper.GenericDelegate<ISourceManager>(_expertHost_SourcesUpdateEvent);
                _expertHost.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(expertHost_OperationalStatusChangedEvent);
                _expertHost = null;
            }

            base.UnInitializeControl();
        }

    }
}
