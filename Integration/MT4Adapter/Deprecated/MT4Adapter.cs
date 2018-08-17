//using System;
//using System.Runtime.Serialization;
//using Arbiter;
//using CommonSupport;

//namespace MT4Adapter
//{
//    /// <summary>
//    /// MT4 Integration Operator allows integrating to the MT4, provides data for order and execution sources.
//    /// A single operator can connect to multiple servers trough connections. Each connection corresponds to
//    /// one server, and maintains many sessions.
//    /// </summary>

//    /// <summary>
//    /// A connection to the MT4 server, this class handles all messages related to operating a MT4 Server Integration instance.
//    /// </summary>
//    [Serializable]
//    [UserFriendlyName("MT4 Integration Adapter")]
//    public class MT4Adapter : ProxyIntegrationAdapter
//    {
//        volatile Uri _integrationUri;
//        public Uri IntegrationUri
//        {
//            get { return _integrationUri; }
//            set
//            {
//                _integrationUri = value;
//                if (value != null)
//                {
//                    this.Name = UserFriendlyNameAttribute.GetClassAttributeName(typeof(MT4Adapter)) + _integrationUri.ToString();
//                }
//                else
//                {
//                    this.Name = "MT4 Adapter";
//                }
//            }
//        }

//        TransportIntegrationClient _integrationClient;
//        /// <summary>
//        /// 
//        /// </summary>
//        ArbiterClientId IntegrationClientId
//        {
//            get
//            {
//                lock (this)
//                {
//                    if (_integrationClient == null)
//                    {
//                        return ArbiterClientId.Empty;
//                    }

//                    return _integrationClient.SubscriptionClientID;
//                }
//            }
//        }

//        /// <summary>
//        /// Default constructor.
//        /// </summary>
//        public MT4Adapter()
//        {
//        }

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        public MT4Adapter(AdapterManagementOperator parentOperator)
//        {
//            SetInitialParameters(parentOperator);
//        }

//        /// <summary>
//        /// Deserializing contructor.
//        /// </summary>
//        public MT4Adapter(SerializationInfo info, StreamingContext context)
//            : base(info, context)
//        {
//            lock (this)
//            {
//                _integrationUri = (Uri)info.GetValue("integrationUri", typeof(Uri));
//            }
//        }

//        /// <summary>
//        /// Serialization routine.
//        /// </summary>
//        public override void GetObjectData(SerializationInfo info, StreamingContext context)
//        {
//            base.GetObjectData(info, context);
//            lock (this)
//            {
//                info.AddValue("integrationUri", IntegrationUri);
//            }
//        }

//        /// <summary>
//        /// Initializes a connection. Called only once in the beggining of operation.
//        /// </summary>
//        public override bool SetInitialParameters(AdapterManagementOperator parentOperator)
//        {
//            if (ParentOperator != null)
//            {
//                return ParentOperator == parentOperator;
//            }

//            if (base.SetInitialParameters(parentOperator) == false)
//            {
//                return false;
//            }

//            if (_integrationUri == null)
//            {
//                Uri addressUri;
//                if (Uri.TryCreate(parentOperator.Platform.Settings.DefaultMT4IntegrationAddress, UriKind.RelativeOrAbsolute, out addressUri) == false)
//                {
//                    SystemMonitor.Warning("Failed to create uri from address.");
//                }
//                else
//                {
//                    _integrationUri = addressUri;
//                }
//            }

//            this.Name = UserFriendlyNameAttribute.GetClassAttributeName(typeof(MT4Adapter)) + _integrationUri.ToString();

//            ChangeOperationalState(OperationalStateEnum.Initialized);

//            return true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        void InitializeConnection()
//        {
//            _initializationEvent.Reset();

//            if (OperationalState == OperationalStateEnum.Operational
//                || OperationalState == OperationalStateEnum.Initializing
//                || _integrationClient == null)
//            {
//                SystemMonitor.OperationWarning("Initialize called while operational or initializing.");
//                _initializationEvent.Set();
//                return;
//            }

//            SystemMonitor.CheckError(ParentOperator != null, "Operator not assigned.");
//            ChangeOperationalState(OperationalStateEnum.Initializing);

//            ResultTransportMessage result = this.SendAndReceiveAddressed<ResultTransportMessage>(
//                _integrationClient.SubscriptionClientID, new SubscribeToServerMessage());

//            if (result == null || result.OperationResult == false)
//            {
//                SystemMonitor.OperationError("Failed to subscribe.");
//                ChangeOperationalState(OperationalStateEnum.NotOperational);
//                _initializationEvent.Set();

//                return;
//            }

//            StartSources();

//            lock (this)
//            {
//                this.OperationHandlerPathUnsafe.Clear();
//                this.OperationHandlerPathUnsafe.Add(_integrationClient.SubscriptionClientID);
//            }

//            _initializationEvent.Set();

//            ChangeOperationalState(OperationalStateEnum.Operational);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="connected"></param>
//        void _integrationClient_ConnectionStatusChangedEvent(bool connected)
//        {
//            if (connected)
//            {
//                if (OperationalState != OperationalStateEnum.Operational)
//                {
//                    if (_initializationEvent.WaitOne(0))
//                    {// Try to recconect.
//                        InitializeConnection();
//                    }
//                }
//            }
//            else
//            {// Connection just went down.
//                if (OperationalState == OperationalStateEnum.Operational)
//                {
//                    string operationResultMessage;
//                    Stop(out operationResultMessage);
//                }
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="message"></param>
//        [MessageReceiver]
//        protected override void Receive(SubscriptionTerminatedMessage message)
//        {
//            base.Receive(message);
//            ChangeOperationalState(OperationalStateEnum.NotOperational);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override bool ArbiterUnInitialize()
//        {
//            if (_integrationClient != null)
//            {
//                lock (this)
//                {
//                    if (Arbiter != null)
//                    {
//                        Arbiter.RemoveClient(_integrationClient);
//                    }
//                    _integrationClient.ConnectionStatusChangedEvent -= new TransportIntegrationClient.ConnectionStatusChangedDelegate(_integrationClient_ConnectionStatusChangedEvent);
//                    _integrationClient = null;
//                }
//            }

//            return base.ArbiterUnInitialize();
//        }

//        public override bool OnStart(out string operationResultMessage)
//        {// This will start the integration client.
//            if (Arbiter == null)
//            {
//                operationResultMessage = "Arbiter not assigned.";
//                return false;
//            }

//            if (_integrationClient == null)
//            {
//                _integrationClient = new TransportIntegrationClient(_integrationUri);
//                _integrationClient.MandatoryRequestMessageReceiverID = this.SubscriptionClientID;
//            }

//            ChangeOperationalState(OperationalStateEnum.Initialized);

//            _integrationClient.ConnectionStatusChangedEvent += new TransportIntegrationClient.ConnectionStatusChangedDelegate(_integrationClient_ConnectionStatusChangedEvent);
//            Arbiter.AddClient(_integrationClient);

//            operationResultMessage = string.Empty;
//            return true;
//        }

//        public override bool OnStop(out string operationResultMessage)
//        {// This will stop the integration client and abort the connection.
//            if (OperationalState == OperationalStateEnum.Operational)
//            {
//                UnSubscribeMessage message = new UnSubscribeMessage();
//                message.RequestConfirmation = false;
//                this.SendAddressed(IntegrationClientId, message);
//            }

//            ChangeOperationalState(OperationalStateEnum.NotOperational);

//            _integrationClient.ConnectionStatusChangedEvent -= new TransportIntegrationClient.ConnectionStatusChangedDelegate(_integrationClient_ConnectionStatusChangedEvent);
//            Arbiter.RemoveClient(_integrationClient);

//            operationResultMessage = string.Empty;
//            return true;
//        }
//    }
//}
