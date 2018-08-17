using System;
using System.Runtime.Serialization;
using CommonSupport;
using ForexPlatform;
using Arbiter;
using CommonFinancial;
using System.Collections.Generic;
using System.Threading;
using FXCore;

namespace FXCMAdapter
{
    /// <summary>
    /// Class provides integration to the FXCM Order2Go API.
    /// Based on contributions by "drginm".
    /// </summary>
    [Serializable]
    [UserFriendlyName("FXCM Integration Adapter")]
    public class FXCMAdapter : StubIntegrationAdapter
    {
        #region Member Variables

        volatile FXCMConnectionManager _manager = null;
        /// <summary>
        /// 
        /// </summary>
        public FXCMConnectionManager Manager
        {
            get { return _manager; }
        }

        volatile string _username = "D3949398001";
        /// <summary>
        /// 
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        volatile string _password = "2811";
        /// <summary>
        /// 
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        volatile string _serviceUrl = "http://www.fxcorporate.com/Hosts.jsp";

        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { _serviceUrl = value; }
        }

        volatile string _accountType = "Demo";

        public string AccountType
        {
            get { return _accountType; }
            set { _accountType = value; }
        }

        volatile FXCMOrders _orders;
        /// <summary>
        /// 
        /// </summary>
        public FXCMOrders Orders
        {
            get { return _orders; }
        }

        volatile FXCMData _data;
        /// <summary>
        /// 
        /// </summary>
        public FXCMData Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Id of the data source component.
        /// </summary>
        public ArbiterClientId? DataSourceId
        {
            get
            {
                if (_dataSourceStub != null)
                {
                    return base._dataSourceStub.SubscriptionClientID;
                }

                return null;
            }
        }

        #endregion

        #region Construction and Instance Control
        /// <summary>
        /// Constructor.
        /// </summary>
        public FXCMAdapter()
        {
            _orderExecutionStub = new OrderExecutionSourceStub("FXCM Adapter Execution", false);
            _dataSourceStub = new DataSourceStub("FXCM Adapter Data", true);

            base.SetStub(_dataSourceStub);
            base.SetStub(_orderExecutionStub);

            Construct();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public FXCMAdapter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _username = info.GetString("username");
            _password = info.GetString("password");
            _serviceUrl = info.GetString("serviceUrl");
            _accountType = info.GetString("accountType");

            Construct();
        }

        bool Construct()
        {
            _data = new FXCMData(this, DataSourceStub);
            _orders = new FXCMOrders(this, OrderExecutionStub);

            StatusSynchronizationEnabled = false;
            StatusSynchronizationSource = null;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            DisposeManager();

            FXCMData data = _data;
            if (data != null)
            {
                data.Dispose();
                _data = null;
            }

            if (_orders != null)
            {
                _orders.Dispose();
                _orders = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("username", _username);
            info.AddValue("password", _password);
            info.AddValue("serviceUrl", _serviceUrl);
            info.AddValue("accountType", _accountType);

            base.GetObjectData(info, context);
        }

        #endregion

        protected override bool OnStart(out string operationResultMessage)
        {
            if (OperationalState == OperationalStateEnum.Operational
                || OperationalState == OperationalStateEnum.Initializing
                || OperationalState == OperationalStateEnum.Initializing)
            {
                operationResultMessage = "Adapter already started.";
                return false;
            }

            if (_manager == null)
            {
                _manager = new FXCMConnectionManager();
            }

            // FXCM API is COM, and sometimes fails to release itself, so make sure we shall not hang.
            SeppukuWatchdog.Activate();

            StatusSynchronizationEnabled = true;
            StatusSynchronizationSource = _manager;

            FXCMConnectionManager manager = _manager;
            if (manager.OperationalState != OperationalStateEnum.Initialized
                && manager.OperationalState != OperationalStateEnum.Constructed)
            {
                operationResultMessage = "The FXCM Adapter can only be started once each application session. Restart the Open Forex Platform to start it again.";
                return false;
            }

            if (manager.Login(Username, Password, _serviceUrl, _accountType, out operationResultMessage) == false)
            {
                operationResultMessage = "Failed to log in to FXCM [" + operationResultMessage + "].";
                SystemMonitor.OperationError(operationResultMessage);
                return false;
            }
            else
            {
                if (_data.Initialize() == false || _orders.Initialize() == false)
                {
                    operationResultMessage = "Failed to initialize FXCMOrders or FXCMData.";
                    SystemMonitor.OperationError(operationResultMessage);
                    return false;
                }

                operationResultMessage = string.Empty;

                StartSources();
                return true;
            }
        }

        protected override bool OnStop(out string operationResultMessage)
        {
            operationResultMessage = string.Empty;
            
            // Stop the orders and data before dumping the manager.
            _orders.UnInitialize();
            _data.UnInitialize();

            DisposeManager();

            base.StatusSynchronizationSource = null;
            StatusSynchronizationEnabled = false;

            ChangeOperationalState(OperationalStateEnum.NotOperational);

            return true;
        }

        protected bool DisposeManager()
        {
            FXCMConnectionManager manager = _manager;
            if (manager != null)
            {
                manager.Logout();
                manager.Dispose();
                manager = null;
                return true;
            }

            return false;
        }

    }
}
