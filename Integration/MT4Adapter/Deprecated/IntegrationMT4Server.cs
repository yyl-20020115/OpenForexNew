//using System;
//using System.Collections.Generic;
//using Arbiter;
//using CommonSupport;
//using ForexPlatform;
//using CommonFinancial;
//using System.Windows.Forms;
//using System.Reflection;
//using System.IO;

//namespace MT4Adapter
//{
//    /// <summary>
//    /// A single integration manager server multiple sessions (each session corresponding to a client).
//    /// The manager is the only class handling messages coming from the clients.
//    /// </summary>
//    public class IntegrationMT4Server : TransportClient
//    {
//        //TransportIntegrationServer _integrationServer;

//        //public Uri ServerIntegrationUri
//        //{
//        //    get
//        //    {
//        //        lock (this)
//        //        {
//        //            return _integrationServer.Address;
//        //        }
//        //    }
//        //}

//        //TransportInfo _subscriberTransportMessageInfo = null;

//        ///// <summary>
//        ///// Expert ID vs Session instance.
//        ///// </summary>
//        //Dictionary<DataSessionInfo, IntegrationMT4ServerSession> _sessions = new Dictionary<DataSessionInfo, IntegrationMT4ServerSession>();

//        ///// <summary>
//        ///// Dummy session receives any messages that were meant for some session not found.
//        ///// </summary>
//        //IntegrationMT4ServerSession _dummySession = new IntegrationMT4ServerSession("DUMMY", "DUMMY", 1, 1, 1);
//        //public IntegrationMT4ServerSession DummySession
//        //{
//        //    get { lock (this) { return _dummySession; } }
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        public IntegrationMT4Server(Uri serverIntegrationUri)
//            : base("IntegrationServer[" + serverIntegrationUri.AbsoluteUri + "]", true)
//        {
//            //TracerHelper.Tracer.Clear(true);

//            //// Locate the trace file, next to the MT4IntegrationDLL, that is deployed inside the
//            //// corresponding MT4 expert/libraries folder.
//            //FileTracerItemSink sink = new FileTracerItemSink(TracerHelper.Tracer);

//            //sink.FilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "OFxP.Expert.log");
//            //TracerHelper.Tracer.Add(sink);

//            ////TracerHelper.Trace("STARTUP:" + Application.StartupPath);
//            ////TracerHelper.Trace("STARTUP:" + Assembly.GetExecutingAssembly().Location);
//            ////TracerHelper.Trace("STARTUP:" + );

//            //TracerHelper.TraceEntry();

//            ////GeneralHelper.FireAndForget(delegate()
//            ////{
//            ////    TracerControl tracerControl = new TracerControl();
//            ////    Tracer tracer = new Tracer();
//            ////    TracerHelper.Tracer = tracer;
//            ////    tracerControl.Tracer = tracer;

//            ////    HostingForm form = new HostingForm("Tracer", tracerControl);
//            ////    form.ShowDialog();
//            ////}
//            ////);

//            //// This.
//            //Filter.Enabled = true;
//            //Filter.AllowOnlyAddressedMessages = false;
//            //Filter.AllowChildrenTypes = true;
//            //Filter.AllowedNonAddressedMessageTypes.Add(typeof(TransportMessage));

//            //Arbiter.Arbiter arbiter = new Arbiter.Arbiter("IntegrationServer.Arbiter");
//            //arbiter.AddClient(this);

//            //// Integration server.
//            //_integrationServer = new TransportIntegrationServer(serverIntegrationUri);
//            //if (arbiter.AddClient(_integrationServer) == false)
//            //{// Failed to add/initialize integration server.
//            //    _integrationServer = null;
//            //    MessageBox.Show("Failed to initialize OFxP MT4 Integration Server." + System.Environment.NewLine + "Possibly another integration with the same address [" + serverIntegrationUri.ToString() + "] already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            //    return;
//            //}
//        }

//        ///// <summary>
//        ///// Un initialize manager, notify subsribers, clear resources.
//        ///// </summary>
//        //public void UnInitialize()
//        //{
//        //    TracerHelper.TraceEntry();
//        //    if (_subscriberTransportMessageInfo != null)
//        //    {// Send update to subscriber to let him now we are closing down.
//        //        this.SendResponding(_subscriberTransportMessageInfo, new SubscriptionTerminatedMessage());
//        //    }
            
//        //    // Give time to the termination message to arrive.
//        //    System.Threading.Thread.Sleep(1000);
//        //}

//        ///// <summary>
//        ///// Destructor.
//        ///// </summary>
//        //~IntegrationMT4Server()
//        //{
//        //    TracerHelper.TraceEntry();
//        //}

//        ///// <summary>
//        ///// Obtain a session instance by its expert ID.
//        ///// </summary>
//        ///// <param name="expertId"></param>
//        ///// <returns></returns>
//        //public IntegrationMT4ServerSession GetSessionById(string expertId)
//        //{
//        //    // TODO : here mapping needs to be done to send requests from multi experts to same session
//        //    lock (this)
//        //    {
//        //        foreach (DataSessionInfo info in _sessions.Keys)
//        //        {
//        //            if (info.Name == expertId)
//        //            {
//        //                return _sessions[info];
//        //            }
//        //        }
//        //    }

//        //    return null;
//        //}

//        #region Initialization

//        //public bool InitializeIntegrationSession(string expertId, string symbol, int minutes,
//        //    decimal modePoint, decimal modeDigits, decimal modeSpread, decimal modeStopLevel, decimal modeLotSize, decimal modeTickValue,
//        //    decimal modeTickSize, decimal modeSwapLong, decimal modeSwapShort, decimal modeStarting, decimal modeExpiration,
//        //    decimal modeTradeAllowed, decimal modeMinLot, decimal modeLotStep, decimal modeMaxLot, decimal modeSwapType,
//        //    decimal modeProfitCalcMode, decimal modeMarginCalcMode, decimal modeMarginInit, decimal modeMarginMaintenance,
//        //    decimal modeMarginHedged, decimal modeMarginRequired, decimal modeFreezeLevel)
//        //{
//        //    lock (this)
//        //    {
//        //        TracerHelper.Trace(expertId);

//        //        if (GetSessionById(expertId) != null)
//        //        {
//        //            TracerHelper.TraceError("Integration with this name (symbol/period) [" + expertId + "] already exists.");
//        //            return false;
//        //        }

//        //        // Create session with new id.
//        //        IntegrationMT4ServerSession session = new IntegrationMT4ServerSession(expertId, symbol, minutes, modeLotSize, (int)modeDigits);
//        //        Arbiter.AddClient(session);
//        //        _sessions.Add(session.Information.Info, session);
                
//        //        if (_subscriberTransportMessageInfo != null)
//        //        {// Send update message to subscriber.
//        //            session.SubscribedTransportMessageInfo = _subscriberTransportMessageInfo;

//        //            // NOT IMPLEMENTED.
//        //            //SessionsUpdatesMessage message = new SessionsUpdatesMessage(SessionsUpdatesMessage.UpdateTypeEnum.Added, session.Information);
//        //            //this.SendResponding(_subscriberTransportMessageInfo, message);
//        //        }
//        //    }

//        //    return true;
//        //}

//        /// <summary>
//        /// A call coming from the expert, requesting to uninitialize this session.
//        /// </summary>
//        /// <param name="expertId"></param>
//        /// <returns></returns>
//        public bool UnInitializeIntegrationSession(string expertId)
//        {
//            IntegrationMT4ServerSession session = null;
//            lock (this)
//            {
//                TracerHelper.Trace(expertId);

//                session = GetSessionById(expertId);
//                if (session == null)
//                {
//                    TracerHelper.TraceError("Integration with this id [" + expertId + "] can not be found.");
//                    return false;
//                }

//                _sessions.Remove(session.Information.Info);
//            }


//            if (_subscriberTransportMessageInfo != null)
//            {// Send update message to subscriber.
//                // NOT IMPLEMENTED.
//                //SessionsUpdatesMessage message = new SessionsUpdatesMessage(SessionsUpdatesMessage.UpdateTypeEnum.Removed, session.Information);
//                //this.SendResponding(_subscriberTransportMessageInfo, message);
//            }

//            session.UnInitialize();
//            return true;
//        }



//        #endregion

//        ///// <summary>
//        ///// Receive a request from a client to be subscribed to the sessions events on this server.
//        ///// </summary>
//        //[MessageReceiver] 
//        //ResponceMessage Receive(SubscribeToServerMessage message)
//        //{
//        //    TracerHelper.TraceEntry();

//        //    lock (this)
//        //    {
//        //        if (_subscriberTransportMessageInfo != null)
//        //        {// Notify existing subscriber he lost control.
//        //            this.SendResponding(_subscriberTransportMessageInfo, new SubscriptionTerminatedMessage());
//        //            _subscriberTransportMessageInfo = null;
//        //        }

//        //        _subscriberTransportMessageInfo = message.TransportInfo.Clone();

//        //        foreach (IntegrationMT4ServerSession session in GeneralHelper.EnumerableToArray<IntegrationMT4ServerSession>(_sessions.Values))
//        //        {
//        //            session.SubscribedTransportMessageInfo = _subscriberTransportMessageInfo;
//        //        }
//        //    }

//        //    return new ResponceMessage(true);
//        //}

//        //[MessageReceiver]
//        //ResponceMessage Receive(UnSubscribeMessage message)
//        //{
//        //    lock (this)
//        //    {
//        //        _subscriberTransportMessageInfo = null;
//        //        foreach (IntegrationMT4ServerSession session in _sessions.Values)
//        //        {
//        //            session.SubscribedTransportMessageInfo = null;
//        //        }
//        //    }

//        //    if (message.RequestConfirmation == false)
//        //    {
//        //        return null;
//        //    }

//        //    return new ResponceMessage(true);
//        //}

//        //[MessageReceiver]
//        //SessionOperationResponceMessage Receive(SessionOperationMessage message)
//        //{
//        //    SystemMonitor.NotImplementedCritical();
//        //    return null;

//        //    //IntegrationMT4ServerSession session = null;

//        //    //lock (this)
//        //    //{
//        //    //    if (_sessions.ContainsKey(message.DataSessionInfo) == false)
//        //    //    {
//        //    //        SystemMonitor.Error("Failed to find session [" + message.DataSessionInfo.Name + "," + message.DataSessionInfo.Symbol + "].");
//        //    //        return new SessionOperationResponceMessage(message.DataSessionInfo, message.OperationID, false);
//        //    //    }

//        //    //    session = _sessions[message.DataSessionInfo];
//        //    //}

//        //    //return (SessionOperationResponceMessage)session.Receive(message);
//        //}

//        //[MessageReceiver]
//        //SessionsUpdatesMessage Receive(GetSessionsUpdatesMessage message)
//        //{
//        //    TracerHelper.Trace(">> [" + _sessions.Count.ToString() + "]");

//        //    List<RuntimeDataSessionInformation> infos = new List<RuntimeDataSessionInformation>();
//        //    lock (this)
//        //    {
//        //        foreach(IntegrationMT4ServerSession session in _sessions.Values)
//        //        {
//        //            if (message.IsSessionRequested(session.Information.Info))
//        //            {
//        //                infos.Add(session.Information);
//        //            }
//        //        }
//        //    }

//        //    if (message.RequestResponce)
//        //    {// Directly return the sessions in the return message.
//        //        return new SessionsUpdatesMessage(SessionsUpdatesMessage.UpdateTypeEnum.Requested, infos);
//        //    }
//        //    else
//        //    {
//        //        // Notify the new subscriber of all the existing sessions.
//        //        this.SendResponding(_subscriberTransportMessageInfo, new SessionsUpdatesMessage(SessionsUpdatesMessage.UpdateTypeEnum.Added, infos));
//        //        return null;
//        //    }
//        //}

//    }
//}
