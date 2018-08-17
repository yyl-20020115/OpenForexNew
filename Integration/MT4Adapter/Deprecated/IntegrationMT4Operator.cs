//using System;
//using System.Collections.Generic;
//using System.Text;
//using Arbiter;
//using CommonSupport;
//using ForexPlatform;
//using CommonFinancial;
//using System.Threading;
//using System.Configuration;
//using System.Runtime.Serialization;

//namespace MT4Adapter
//{
//    /// <summary>
//    /// MT4 Integration Operator allows integrating to the MT4, provides data for order and execution sources.
//    /// A single operator can connect to multiple servers trough connections. Each connection corresponds to
//    /// one server, and maintains many sessions.
//    /// </summary>
//    [Serializable]
//    [UserFriendlyName("Meta Trader 4 Integration")]
//    public class IntegrationMT4Operator : PlatformOperator
//    {
//        /// <summary>
//        /// Low level component since others are likely to depend on it.
//        /// </summary>
//        public override int ComponentLevel
//        {
//            get
//            {
//                return 10;
//            }
//        }

//        public override bool MultipleInstancesAllowed
//        {
//            get
//            {
//                return false;
//            }
//        }

//        List<IntegrationMT4OperatorConnection> _connectionInstances = new List<IntegrationMT4OperatorConnection>();
//        public List<IntegrationMT4OperatorConnection> ConnectionInstances
//        {
//            get { lock (this) { return _connectionInstances; } }
//        }

//        public delegate void ConnectionStatusChangedDelegate(IntegrationMT4OperatorConnection connection);
//        public event ConnectionStatusChangedDelegate ConnectionStatusChangedEvent;

//        /// <summary>
//        /// 
//        /// </summary>
//        public IntegrationMT4Operator()
//            : base(false)
//        {
//            // This.
//            Filter.Enabled = true;
//            Filter.AllowOnlyAddressedMessages = false;
//            Filter.AllowChildrenTypes = true;
//            Filter.AllowedMessageTypes.Add(typeof(TransportMessage));

//            Name = UserFriendlyNameAttribute.GetClassAttributeName(typeof(IntegrationMT4Operator));
//            base.DefaultTimeOut = TimeSpan.FromSeconds(15);
//        }

//        /// <summary>
//        /// Deserializing contructor.
//        /// </summary>
//        public IntegrationMT4Operator(SerializationInfo info, StreamingContext context)
//            : base(info, context)
//        {
//            _connectionInstances = (List<IntegrationMT4OperatorConnection>)info.GetValue("connectionInstances", typeof(List<IntegrationMT4OperatorConnection>));
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        void newConnection_OperationalStatusChangedEvent(IOperational connection, OperationalStateEnum parameter2)
//        {
//            if (ConnectionStatusChangedEvent != null)
//            {
//                ConnectionStatusChangedEvent((IntegrationMT4OperatorConnection)connection);
//            }
//        }

//        /// <summary>
//        /// Add a new connection to this uri. Will spawn a thread to initialize this connection.
//        /// Calls can be synchronous.
//        /// </summary>
//        /// <param name="uriString"></param>
//        /// <returns></returns>
//        public bool AddConnection(string uriString)
//        {
//            Uri uri = new Uri(uriString);
//            IntegrationMT4OperatorConnection newConnection;

//            lock (this)
//            {
//                foreach (IntegrationMT4OperatorConnection connection in _connectionInstances)
//                {
//                    if (uri == connection.IntegrationUri)
//                    {// Connection to this address already exists.
//                        SystemMonitor.OperationError("Connection to this address already exists [" + uriString + "].");
//                        return false;
//                    }
//                }

//                newConnection = new IntegrationMT4OperatorConnection(uri);
//                newConnection.OperationalStatusChangedEvent += new OperationalStatusChangedDelegate(newConnection_OperationalStatusChangedEvent);
//                this.Arbiter.AddClient(newConnection);
//                newConnection.Initialize(this);

//                _connectionInstances.Add(newConnection);
//            }

//            this.RaisePersistenceDataUpdatedEvent();
//            return true;
//        }

//        /// <summary>
//        /// Remove the connection from operation.
//        /// </summary>
//        /// <param name="connection"></param>
//        /// <returns></returns>
//        public bool RemoveConnection(IntegrationMT4OperatorConnection connection)
//        {
//            lock (this)
//            {
//                if (_connectionInstances.Contains(connection) == false)
//                {
//                    SystemMonitor.OperationError("Connection instance not found.");
//                    return false;
//                }
//            }
            
//            connection.OperationalStatusChangedEvent -= new OperationalStatusChangedDelegate(newConnection_OperationalStatusChangedEvent);
//            connection.UnInitialize();

//            Arbiter.RemoveClient(connection);
            
//            lock(this)
//            {
//                _connectionInstances.Remove(connection);
//            }

//            connection.Dispose();
//            return true;
//        }

//        /// <summary>
//        /// Serialization routine.
//        /// </summary>
//        public override void GetObjectData(SerializationInfo info, StreamingContext context)
//        {
//            base.GetObjectData(info, context);

//            lock (this)
//            {
//                info.AddValue("connectionInstances", _connectionInstances);
//            }
//        }

//        /// <summary>
//        /// Disposing the object and all related data.
//        /// </summary>
//        protected override void OnDispose()
//        {
//            base.OnDispose();

//            IntegrationMT4OperatorConnection[] connections;
//            lock (this)
//            {
//                connections = _connectionInstances.ToArray();
//                _connectionInstances.Clear();
//            }

//            foreach (IntegrationMT4OperatorConnection connection in _connectionInstances)
//            {
//                connection.Dispose();
//            }
//        }

//        protected override bool OnInitialize(Platform platform)
//        {
//            if (base.OnInitialize(platform) == false)
//            {
//                return false;
//            }

//            ChangeOperationalState(OperationalStateEnum.Operational);

//            IntegrationMT4OperatorConnection[] connections;
//            lock (this)
//            {
//                connections = _connectionInstances.ToArray();
//            }

//            for (int i = 0; i < connections.Length; i++)
//            {
//                IntegrationMT4OperatorConnection connection = connections[i];
//                connection.OperationalStatusChangedEvent += new OperationalStatusChangedDelegate(newConnection_OperationalStatusChangedEvent);
//                Arbiter.AddClient(connection);

//                connection.Initialize(this);
//            }

//            return true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        protected override bool OnUnInitialize()
//        {
//            base.OnUnInitialize();

//            ChangeOperationalState(OperationalStateEnum.NotOperational);

//            IntegrationMT4OperatorConnection[] connections;
//            lock (this)
//            {
//                connections = _connectionInstances.ToArray();
//            }

//            foreach (IntegrationMT4OperatorConnection connection in connections)
//            {
//                connection.OperationalStatusChangedEvent -= new OperationalStatusChangedDelegate(newConnection_OperationalStatusChangedEvent);
//                connection.UnInitialize();
//                Arbiter.RemoveClient(connection);
//            }

//            return true;
//        }


//    }
//}
