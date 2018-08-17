using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Arbiter;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Implementation of integration adapter, that forwards all requests as operations to a remote handler.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Proxy Integration Adapter")]
    public class ProxyIntegrationAdapterClient : OperationalTransportClient, IIntegrationAdapter
    {
        ManualResetEvent _initializationEvent = new ManualResetEvent(true);

        TransportInfo _proxyTransportInfo = null;

        volatile Uri _integrationUri = new Uri("net.tcp://localhost:13123/TradingAPI");
        /// <summary>
        /// 
        /// </summary>
        public Uri IntegrationUri
        {
            get { return _integrationUri; }
            set
            {
                _integrationUri = value;
                if (value != null)
                {
                    this.Name = "ProxyIntegrationAdapter " + _integrationUri.ToString();
                }
                else
                {
                    this.Name = "ProxyIntegrationAdapter";
                }
            }
        }

        TransportIntegrationClient _integrationClient;

        ArbiterClientId _integrationClientId = ArbiterClientId.Empty;

      public TransportInfo ProxyTransportInfo
      {
        get{
          return this._proxyTransportInfo;
        }
      }
 

        /// <summary>
        /// 
        /// </summary>
       public ArbiterClientId IntegrationClientId
        {
            get
            {
                lock (this)
                {
                    if (_integrationClient == null)
                    {
                        return ArbiterClientId.Empty;
                    }

                    return _integrationClient.SubscriptionClientID;
                }
            }
        }

        /// <summary>
        /// Persistence managed by controller, adapter always begins in Non started mode.
        /// </summary>
        volatile bool _isStarted = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsStarted
        {
            get { return _isStarted; }
        }

        /// <summary>
        /// 
        /// </summary>
        public event IntegrationAdapterUpdateDelegate PersistenceDataUpdateEvent;


        #region Instance Control

        /// <summary>
        /// 
        /// </summary>
        public ProxyIntegrationAdapterClient()
            : base(UserFriendlyNameAttribute.GetTypeAttributeName(typeof(ProxyIntegrationAdapterClient)), false)
        {
            this.DefaultTimeOut = TimeSpan.FromSeconds(60);
        }

        public ProxyIntegrationAdapterClient(Uri _integrationUri)
          : this()
        {
          this._integrationUri = _integrationUri;
        }

        /// <summary>
        /// 
        /// </summary>
        public ProxyIntegrationAdapterClient(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            IntegrationUri = (Uri)info.GetValue("integrationUri", typeof(Uri));
            _integrationClientId = (ArbiterClientId)info.GetValue("integrationClientId", typeof(ArbiterClientId));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            lock (this)
            {
                info.AddValue("integrationUri", IntegrationUri);
                info.AddValue("integrationClientId", _integrationClientId);
            }
        }

        public override bool ArbiterInitialize(Arbiter.Arbiter arbiter)
        {

            return base.ArbiterInitialize(arbiter);
        }

        public override bool ArbiterUnInitialize()
        {
            RegisterSourceMessage register = new RegisterSourceMessage(false);
            register.RequestResponse = false;
            this.Send(register);

            if (_integrationClient != null)
            {
                lock (this)
                {
                    if (Arbiter != null)
                    {
                        Arbiter.RemoveClient(_integrationClient);
                    }

                    _integrationClient.ConnectionStatusChangedEvent -= new TransportIntegrationClient.ConnectionStatusChangedDelegate(_integrationClient_ConnectionStatusChangedEvent);
                    _integrationClient = null;
                }
            }

            return base.ArbiterUnInitialize();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        void InitializeConnection()
        {
            _initializationEvent.Reset();

            if (OperationalState == OperationalStateEnum.Operational
                || OperationalState == OperationalStateEnum.Initializing
                || _integrationClient == null)
            {
                SystemMonitor.OperationWarning("Initialize called while operational or initializing.");
                _initializationEvent.Set();
                return;
            }

            ChangeOperationalState(OperationalStateEnum.Initializing);

            ResponseMessage result = this.SendAndReceiveAddressed<ResponseMessage>(
                IntegrationClientId, new SubscribeMessage());

            if (result == null || result.OperationResult == false)
            {
                SystemMonitor.OperationError("Failed to subscribe.", TracerItem.PriorityEnum.Medium);
                ChangeOperationalState(OperationalStateEnum.NotOperational);
                _initializationEvent.Set();

                return;
            }

            _initializationEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connected"></param>
        void _integrationClient_ConnectionStatusChangedEvent(bool connected)
        {
            if (connected)
            {
                if (OperationalState != OperationalStateEnum.Operational)
                {
                    if (_initializationEvent.WaitOne(0))
                    {// Try to recconect.
                        InitializeConnection();
                    }
                }
            }
            else
            {// Connection just went down.
                if (OperationalState == OperationalStateEnum.Operational)
                {
                    ChangeOperationalState(OperationalStateEnum.NotOperational);
                    //string operationResultMessage;
                    //Stop(out operationResultMessage);
                }
            }
        }

        #region IIntegrationAdapter Members

        public bool Start(Platform platform, out string operationResultMessage)
        {// This will start the integration client.
            if (Arbiter == null && platform!=null && platform.Arbiter!=null)
            {
              platform.Arbiter.AddClient(this);
            }

            if (IsStarted)
            {
                operationResultMessage = "Adapter already started.";
                return false;
            }

            if (_integrationClient == null)
            {
                _integrationClient = new TransportIntegrationClient(_integrationUri);
            }

            if (_integrationClient != null && _integrationClientId.IsEmpty == false)
            {
                _integrationClient.SetArbiterSubscriptionClientId(_integrationClientId);
            }

            _integrationClient.MandatoryRequestMessageReceiverID = this.SubscriptionClientID;

            _integrationClient.ConnectionStatusChangedEvent += new TransportIntegrationClient.ConnectionStatusChangedDelegate(_integrationClient_ConnectionStatusChangedEvent);
            Arbiter.AddClient(_integrationClient);
            _integrationClientId = _integrationClient.SubscriptionClientID;

            _isStarted = true;

            ChangeOperationalState(OperationalStateEnum.Initialized);

            operationResultMessage = string.Empty;
            return true;
        }

        public bool Stop(out string operationResultMessage)
        {// This will stop the integration client and abort the connection.
            if (OperationalState == OperationalStateEnum.Operational)
            {
                UnSubscribeMessage message = new UnSubscribeMessage();
                message.RequestResponse = false;

                this.SendAddressed(IntegrationClientId, message);
            }

            ChangeOperationalState(OperationalStateEnum.NotOperational);

            if (_integrationClient != null)
            {
                _integrationClient.ConnectionStatusChangedEvent -= new TransportIntegrationClient.ConnectionStatusChangedDelegate(_integrationClient_ConnectionStatusChangedEvent);
                Arbiter.RemoveClient(_integrationClient);
            }

            _isStarted = false;

            operationResultMessage = string.Empty;
            return true;
        }


        #endregion

        #region Messages

        /// <summary>
        /// Intercept and handle this message here, since we control the Id level registrations
        /// locally, since this class pretends to be the sources themselves.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [MessageReceiver]
        protected GetDataSourceSymbolCompatibleResponseMessage Receive(GetDataSourceSymbolCompatibleMessage message)
        {
            int result = 0;
            if (message.DataSourceId == this.SubscriptionClientID.Id)
            {
                result = int.MaxValue;
            }

            return new GetDataSourceSymbolCompatibleResponseMessage(true) { CompatibilityLevel = result };
        }


        [MessageReceiver]
        protected void Receive(SubscriptionTerminatedMessage message)
        {
            ChangeOperationalState(OperationalStateEnum.NotOperational);
        }

        [MessageReceiver]
        void Receive(RegisterSourceMessage message)
        {
            _proxyTransportInfo = message.TransportInfo;

            RegisterSourceMessage localRegistrationMessage = new RegisterSourceMessage(
                message.SourceType.Value, message.Register);

            localRegistrationMessage.RequestResponse = false;

            this.Send(localRegistrationMessage);

            if (OperationalState != OperationalStateEnum.Operational)
            {
                if (OperationalState == OperationalStateEnum.Initializing 
                    || OperationalState == OperationalStateEnum.Initialized)
                {

                    ChangeOperationalState(OperationalStateEnum.Operational);
                }
                else
                {
                    SystemMonitor.OperationError("Adapter received a subscription confirmation, but not in expected state.");
                }
            }
        }

        [MessageReceiver]
        protected void Receive(ResponseMessage message)
        {
            if (Arbiter == null)
            {
                SystemMonitor.OperationWarning("Received message [" + message.GetType().Name + "] while already out of arbiter.");
                return;
            }

            ProxyForwardingSend(message);
        }

        /// <summary>
        /// Implement a conditional proxy of incoming messages, based on state of connection.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [MessageReceiver]
        ResponseMessage ReceiveProxy(RequestMessage message)
        {
            if (Arbiter == null || IntegrationClientId.IsEmpty || _proxyTransportInfo == null)
            {
                SystemMonitor.OperationWarning("Message [" + message.GetType().Name + "] received but integration not ready.");
                if (message.RequestResponse)
                {
                    return new ResponseMessage(false);
                }
                else
                {
                    return null;
                }
            }

            if (this.OperationalState != OperationalStateEnum.Operational)
            {
                if (message.RequestResponse)
                {
                    return new ResponseMessage(false, "Adapter not operational.");
                }
                else
                {
                    return null;
                }
            }

            ProxySend(IntegrationClientId, message);
            return null;
        }

        [MessageReceiver]
        GetExecutionSourceParametersResponseMessage Receive(GetExecutionSourceParametersMessage message)
        {
            return new GetExecutionSourceParametersResponseMessage(true);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

    }
}
