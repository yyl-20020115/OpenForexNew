//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonSupport;

//namespace CommonFinancial
//{
//    /// <summary>
//    /// Handles order execution requests coming from an expert session. 
//    /// Makes use of the automatic serialization mode, since it does not need to inherit any
//    /// classes this is not a problem.
//    /// Class follows its executor operational state.
//    /// </summary>
//    [Serializable]
//    [UserFriendlyName("Order Execution OrderExecutionProvider")]
//    public abstract class OrderExecutionProvider : Operational, ISourceOrderExecution, Account.IUpdateImplementation
//    {
//        #region ISourceOrderExecution Members

//        protected volatile Account _account = null;
//        /// <summary>
//        /// Account information related to this provider.
//        /// </summary>
//        public Account Account
//        {
//            get { return _account; }
//        }

//        volatile ISessionDataProvider _sessionDataProvider;
//        public virtual ISessionDataProvider SessionDataProvider
//        {
//            get { return _sessionDataProvider; }
//        }

//        public DataSessionInfo Info
//        {
//            get
//            {
//                if (_sessionDataProvider != null)
//                {
//                    return _sessionDataProvider.Info;
//                }
//                else
//                {
//                    return DataSessionInfo.Empty;
//                }
//            }
//        }

//        volatile IOrderSink _executor;
//        public virtual IOrderSink OrderSink
//        {
//            get { return _executor; }
//        }

//        volatile ITimeControl _timeControl = null;
//        public virtual ITimeControl TimeControl
//        {
//            get 
//            {
//                return _timeControl; 
//            }
//        }


//        volatile IPosition _position;
//        /// <summary>
//        /// 
//        /// </summary>
//        public IPosition Position
//        {
//            get { return _position; }
//        }

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        public OrderExecutionProvider(Account account)
//        {
//            _account = account;
//            _account.SetInitialParameters(this);

//            ChangeOperationalState(OperationalStateEnum.Constructed);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        protected bool SetInitialParameters(IPosition position, ISessionDataProvider dataProvider, IOrderSink executor, ITimeControl timeControl)
//        {
//            _position = position;
//            _sessionDataProvider = dataProvider;
//            _executor = executor;
//            _timeControl = timeControl;

//            ChangeOperationalState(OperationalStateEnum.Initializing);

//            return true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sessionInfo"></param>
//        /// <returns></returns>
//        public virtual bool Initialize()
//        {
//            base.StatusSynchronizationSource = _executor;

//            if (_account != null)
//            {
//                _account.Initialize(this);
//                _account.BeginAccountUpdate();
//            }

//            if (_executor != null)
//            {
//                _executor.Initialize();
//                _executor.AccountInfoUpdateEvent += new AccountInfoUpdateDelegate(_executor_AccountInfoUpdateEvent);
//            }

//            return true;
//        }

//        /// <summary>
//        /// Link executor account orderInfo update to actual account.
//        /// </summary>
//        /// <param name="provider"></param>
//        /// <param name="accountInfo"></param>
//        void _executor_AccountInfoUpdateEvent(IOrderSink provider, AccountInfo accountInfo)
//        {
//            Account account = _account;
//            if (account != null)
//            {
//                account.AdoptInfo(accountInfo);
//            }
//        }

      
//        /// <summary>
//        /// 
//        /// </summary>
//        public void UnInitialize()
//        {
//            if (_account != null)
//            {
//                _account.UnInitialize();
//            }

//            if (_executor != null)
//            {
//                _executor.UnInitialize();
//            }
//        }

//        #endregion
        
//        #region IDisposable Members

//        public void Dispose()
//        {
            
//        }

//        #endregion

//        bool Account.IUpdateImplementation.BeginAccountUpdate(Account account)
//        {
//            IOrderSink executor = _executor;
//            if (executor != null)
//            {
//                AccountInfo? orderInfo = executor.BeginAccountInfoUpdate(account.Info);
//                if (orderInfo.HasValue)
//                {
//                    account.AdoptInfo(orderInfo.Value);
//                    return true;
//                }
//            }

//            return false;
//        }

//    }
//}
