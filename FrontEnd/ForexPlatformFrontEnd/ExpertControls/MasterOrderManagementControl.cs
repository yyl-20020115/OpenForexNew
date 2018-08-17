//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Text;
//using System.Windows.Forms;
//using ForexPlatform;
//using CommonFinancial;
//using CommonSupport;

//namespace ForexPlatformFrontEnd
//{
//    /// <summary>
//    /// Allows management of multiple orders on multiple order executioners at the same time.
//    /// </summary>
//    public partial class MasterOrderManagementControl : CommonBaseControl
//    {
//        MasterTradingExpert _expert;

//        /// <summary>
//        /// 
//        /// </summary>
//        public MasterOrderManagementControl()
//        {
//            InitializeComponent();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public MasterOrderManagementControl(MasterTradingExpert expert)
//        {
//            InitializeComponent();

//            _expert = expert;
//        }

//        protected override void OnLoad(EventArgs e)
//        {
//            base.OnLoad(e);

//            this.executionAccountsControl1.SessionManager = _expert.SessionManager;
//            this.masterOrderControl1.Initialize(_expert);

//            //ordersControl1.AllowOrderManagement = true;
//            //ordersControlMaster.AllowOrderManagement = true;

//            //ExpertSession masterSession = new ExpertSession(new SessionInfo(Guid.NewGuid(), "Master Session", new Symbol(Guid.NewGuid(), "One/Multiple", "Any"), new TimeSpan(1, 0, 0), 1, 1));
//            //masterSession.SetInitialParameters(new MasterTradingDataProvider(), new MasterTradingOrderExecutionProvider());

//            //ordersControlMaster.SingleSession = masterSession;
//            ordersControl1.SessionManager = _expert.SessionManager;
//        }


//    }
//}
