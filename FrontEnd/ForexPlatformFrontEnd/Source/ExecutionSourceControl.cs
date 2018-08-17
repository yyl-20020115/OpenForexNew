using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonSupport;
using ForexPlatform;

namespace ForexPlatformFrontEnd
{
    public partial class ExecutionSourceControl : PlatformComponentControl
    {
        OrderExecutionSource ExecutionSource
        {
            get { return (OrderExecutionSource)base.Component; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ExecutionSourceControl()
            : base(null)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public ExecutionSourceControl(OrderExecutionSource orderExecutionSource)
            : base(orderExecutionSource)
        {
            InitializeComponent();
            this.Name = orderExecutionSource.Name;
            this.sessionInfosControl1.SessionManager = ExecutionSource;
        }

        /// <summary>
        /// Constructor needed for the runtime reflection operations.
        /// </summary>
        public ExecutionSourceControl(RemoteExecutionSource orderExecutionSource)
            : base(orderExecutionSource)
        {
            InitializeComponent();
            this.Name = orderExecutionSource.Name;
            this.sessionInfosControl1.SessionManager = ExecutionSource;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ExecutionSource.OperationalStatusChangedEvent += new OperationalStatusChangedDelegate(ExecutionSource_OperationalStatusChangedEvent);
            UpdateUI();
        }

        void ExecutionSource_OperationalStatusChangedEvent(IOperational parameter1, OperationalStateEnum parameter2)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.DefaultDelegate(UpdateUI));
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            //this.toolStripLabelSource.Text = ExecutionSource.Name + " is " + ExecutionSource.OperationalState.ToString();
            //if (ExecutionSource.OperationalState != OperationalStateEnum.Operational)
            //{
            //    toolStripLabelSource.ForeColor = Color.DarkRed;
            //}
            //else
            //{
            //    toolStripLabelSource.ForeColor = Color.DarkGreen;
            //}
        }

    }
}
