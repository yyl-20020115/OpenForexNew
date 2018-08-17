using System;
using CommonSupport;
using MBTCOMLib;
using MBTORDERSLib;

namespace MBTradingAdapter
{
    ///// <summary>
    ///// Copy of values, from API documentation.
    ///// </summary>
    //public enum enumQuoteServiceFlags
    //{
    //    qsfLevelOne = 1,
    //    qsfLevelTwo = 2,
    //    qsfTimeAndSales = 4,
    //    qsfOptions = 8
    //}

    /// <summary>
    /// Class constrols integration to the MBTrading application.
    /// TODO: release of communication manager, using the Marshal namespace calls
    /// ALL SYMBOLS MUST BE PASSED TO THE API IN "UPPERCASE"
    /// 
    /// Works in a separate thread, since COM EVENTS coming from the API
    /// require a requestMessage loop and the same thread to access them, it takes
    /// care of all this.
    /// </summary>
    public class MBTradingTestConnectionManager : Operational, IDisposable
    {
        const int HostId = 282;

        /// <summary>
        /// Try to access this only from the internal thread.
        /// </summary>
        volatile MbtComMgr _communicationManager;

        BackgroundMessageLoopOperator _messageLoopOperator;

        MbtOrderClient _orderClient = null;

        //volatile MBTradingQuote _quotes;
        //public MBTradingQuote Quotes
        //{
        //    get { return _quotes; }
        //}

        //volatile MBTradingHistory _history;
        //public MBTradingHistory History
        //{
        //    get { return _history; }
        //}

        //volatile MBTradingOrders _orders;
        ///// <summary>
        ///// 
        ///// </summary>
        //public MBTradingOrders Orders
        //{
        //    get { return _orders; }
        //}

        //MBTradingAdapter _adapter;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MBTradingTestConnectionManager()
        {
            ChangeOperationalState(OperationalStateEnum.Constructed);

            _messageLoopOperator = new BackgroundMessageLoopOperator(false);
            //_quotes = new MBTradingQuote(_messageLoopOperator);
            //_history = new MBTradingHistory(_messageLoopOperator);
            //_orders = new MBTradingOrders(_messageLoopOperator);

            _messageLoopOperator.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                //if (_quotes != null)
                //{
                //    _quotes.Dispose();
                //    _quotes = null;
                //}

                //if (_history != null)
                //{
                //    _history.Dispose();
                //    _history = null;
                //}

                //if (_orders != null)
                //{
                //    _orders.UnInitialize();
                //    _orders.Dispose();
                //    _orders = null;
                //}

                //if (_adapter != null)
                //{
                //    _adapter = null;
                //}

                if (_communicationManager != null)
                {
                    //_communicationManager.OnLogonSucceed -= new IMbtComMgrEvents_OnLogonSucceedEventHandler(_communicationManager_OnLogonSucceed);
                    _communicationManager = null;

                }

                if (_messageLoopOperator != null)
                {
                    _messageLoopOperator.Stop();
                    _messageLoopOperator.Dispose();
                    _messageLoopOperator = null;
                }

                ChangeOperationalState(OperationalStateEnum.Disposed);
            }
            
            //GC.Collect();
        }

        /// <summary>
        /// 
        /// </summary>
        void _communicationManager_OnAlertAdded(MbtAlert pAlert)
        {
            SystemMonitor.Report(pAlert.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        void _communicationManager_OnLogonSucceed()
        {
            ChangeOperationalState(OperationalStateEnum.Operational);
            // ---
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Login(string username, string password)
        {
            bool result = false;

            if (OperationalState == OperationalStateEnum.Disposed)
            {
                return false;
            }

            _messageLoopOperator.Invoke(delegate()
            {
                // This is a slow blocking call, make sure to execute outside of lock.
                MbtComMgr manager = new MbtComMgr();
                _communicationManager = manager;
                //_communicationManager.OnLogonSucceed += new IMbtComMgrEvents_OnLogonSucceedEventHandler(_communicationManager_OnLogonSucceed);

                _orderClient = _communicationManager.OrderClient;
                _orderClient.SilentMode = true;

                //_quotes.Initialize(_communicationManager.Quotes);
                //_history.Initialize(_communicationManager.HistMgr);
                ///_orders.Initialize(_adapter, this, _communicationManager);

                //_communicationManager.OnAlertAdded += new IMbtComMgrEvents_OnAlertAddedEventHandler(_communicationManager_OnAlertAdded);
                _communicationManager.EnableSplash(false);

                ChangeOperationalState(OperationalStateEnum.Initialized);
                _communicationManager.EnableSplash(false);
                _communicationManager.SilentMode = true;

                result = _communicationManager.DoLogin(HostId, username, password, "");

            }, TimeSpan.FromSeconds(25));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Logout()
        {
            _orderClient = null;
            //_communicationManager.OnLogonSucceed -= new IMbtComMgrEvents_OnLogonSucceedEventHandler(_communicationManager_OnLogonSucceed);
            _communicationManager = null;
            GC.Collect();
            //_communicationManager.disp
            return true;
        }


    }
}
