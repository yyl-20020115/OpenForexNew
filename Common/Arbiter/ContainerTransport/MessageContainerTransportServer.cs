using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Xml;
using Arbiter.MessageContainerTransport.GZipEncoder;
using CommonSupport;


namespace Arbiter.MessageContainerTransport
{
    /// <summary>
    /// UseSynchronizationContext - this is done to evade locking up on WinForms.
    /// ConcurrencyMode - this is how to receive calls. Default is single.
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    public class MessageContainerTransportServer : IMessageContainerTransport, IDisposable
    {
        /// <summary>
        /// 8 MEG buffer, if it occurs that a requestMessage is bigger than this it will cause an exception.
        /// </summary>
        public static int TransportMaxBufferSize = 18 * 1024 * 1024;
        static int _maximumAllowedMessageSize = TransportMaxBufferSize - 1024;
        public static int MaximumAllowedMessageSize
        {
            get {
                if (EnableGZipTransportEncoding)
                {// GZip is capable of puting out the entire requestMessage size.
                    return _maximumAllowedMessageSize;
                }
                else
                {// Normal is only able to provide half of that (due to some reason, bigger messages get lost.
                    return _maximumAllowedMessageSize / 2;
                }
            }
        }

        /// <summary>
        /// Should the transport be encoded. This is good over the internet, 
        /// but can slow down transfer on local machine and network due to higher CPU load for large files.
        /// </summary>
        public static bool EnableGZipTransportEncoding = false;

        ServiceHost _serviceHost;
        CallbackContextManager<IMessageContainerTransport> _contextManager = new CallbackContextManager<IMessageContainerTransport>();

        public event HandlerDelegate<string, MessageContainer> MessageContainerReceivedEvent;

        public event HandlerDelegate<string> ClientConnected;
        public event HandlerDelegate<string> ClientDisConnected;

        Timer _reconectionTimer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageContainerTransportServer()
        {
        }

        public void Dispose()
        {
            UnInitialize();
        }

        public bool Initialize(Uri serverUriBase)
        {
            TracerHelper.TraceEntry();

            lock (this)
            {
                SystemMonitor.CheckThrow(_serviceHost == null, "Already initialized.");
                try
                {
                    _serviceHost = new ServiceHost(this);
                    _serviceHost.AddServiceEndpoint(typeof(IMessageContainerTransport), CreateBinding(), serverUriBase);
                    _serviceHost.Open();

                    _reconectionTimer = new Timer(new TimerCallback(VerifyClientConnections), null, 0, 30000);
                }
                catch (Exception ex)
                {
                    SystemMonitor.Error("Failed to initialize service host [" + ex.Message + "]");
                    return false;
                }
            }

            return true;
        }

        public bool UnInitialize()
        {
            TracerHelper.TraceEntry();

            lock (this)
            {
                if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed)
                {
                    _serviceHost.BeginClose(TimeSpan.FromSeconds(2), null, null);
                    _serviceHost.Abort();
                    _serviceHost.Close();
                    _serviceHost = null;
                }

                if (_reconectionTimer != null)
                {
                    _reconectionTimer.Dispose();
                    _reconectionTimer = null;
                }
            }

            return true;
        }

        public static Binding CreateBinding()
        {
            TracerHelper.Trace("");

            Binding result;

            TimeSpan reliableSessionTimeOut = TimeSpan.FromSeconds(10);

            //if (EnableGZipTransportEncoding)
            //{// Apply custom GZip binding, allowing minimized requestMessage size. The only way to apply GZip is to use custom binding.

            //    CustomBinding customBinding = new CustomBinding();

            //    //ReliableSessionBindingElement reliableSessionElement = new ReliableSessionBindingElement(true);
            //    //reliableSessionElement.InactivityTimeout = reliableSessionTimeOut;
            //    //customBinding.Elements.Add(reliableSessionElement);

            //    GZipMessageEncodingBindingElement encoding = new GZipMessageEncodingBindingElement();
            //    customBinding.Elements.Add(encoding);

            //    TcpTransportBindingElement tcpTransport = new TcpTransportBindingElement();
            //    customBinding.Elements.Add(tcpTransport);

            //    // To access the Reader Quotas, we need to use this Binding context technique:
            //    // as seen here: http://blogs.msdn.com/drnick/archive/2006/08/10/More-Binding-Polymorphism.aspx
            //    BindingContext bindingContext = new BindingContext(customBinding, new BindingParameterCollection());
            //    XmlDictionaryReaderQuotas quotas = bindingContext.GetInnerProperty<XmlDictionaryReaderQuotas>();

            //    // Extend the reading quotas to allow transfer of larger information pieces.
            //    quotas.MaxDepth = 2147483647;
            //    quotas.MaxStringContentLength = 2147483647;
            //    quotas.MaxArrayLength = 2147483647;

            //    tcpTransport.TransferMode = TransferMode.Buffered;
            //    tcpTransport.MaxReceivedMessageSize = TransportMaxBufferSize;
            //    tcpTransport.MaxBufferSize = TransportMaxBufferSize;

            //    result = customBinding;
            //}
            //else
            {// Apply default binary encoding.
                
                // This is the way to do it - binary encoding, if using a CustomBinding element like the previous case.
                //BinaryMessageEncodingBindingElement encoding = new BinaryMessageEncodingBindingElement();
                //customBinding.Elements.Add(encoding);

                //encoding.MaxReadPoolSize = 16;
                //encoding.MaxSessionSize = TransportMaxBufferSize;
                //encoding.MaxWritePoolSize = 16;
                //encoding.MessageVersion = MessageVersion.Default;
                //XmlDictionaryReaderQuotas quotas = encoding.ReaderQuotas;

                // Simple tcp binding.
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, false);
                binding.ReceiveTimeout = TimeSpan.MaxValue;

                binding.ReliableSession.Enabled = true;
                binding.ReliableSession.InactivityTimeout = reliableSessionTimeOut;
                binding.CloseTimeout = TimeSpan.FromSeconds(20);

                // TimeSpan.MaxValue is interpreted with a rounding error, so use 24 days instead
                binding.SendTimeout = TimeSpan.FromSeconds(60);

                binding.ReaderQuotas.MaxDepth = 2147483647;
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.ReaderQuotas.MaxArrayLength = 2147483647;

                // We can not have streamed mode and be duplex mode in the same time.
                binding.TransferMode = TransferMode.Buffered;

                binding.MaxReceivedMessageSize = TransportMaxBufferSize;
                binding.MaxBufferSize = TransportMaxBufferSize;

                result = binding;
            }


            return result;
        }

        #region IMessageContainerTransport Members

        /// <summary>
        /// Clear and disconnect all failed interfaces.
        /// </summary>
        /// <param name="badInterfaces"></param>
        protected void ProcessBadInterfaces(List<IMessageContainerTransport> badInterfaces)
        {
            foreach (IMessageContainerTransport interfaceContainer in badInterfaces)
            {
                TracerHelper.Trace("Removing bad interface...");
                try
                {
                    string sessionID;
                    OperationContext context;
                    lock (_contextManager)
                    {
                        if (_contextManager.ClientsContextsUnsafe.ContainsKey(interfaceContainer) == false)
                        {// This interface already removed.
                            continue;
                        }

                        context = _contextManager.ClientsContextsUnsafe[interfaceContainer];
                        sessionID = context.SessionId;
                        _contextManager.RemoveContext(interfaceContainer);
                    }

                    if (sessionID == null)
                    {
                        sessionID = string.Empty;
                    }

                    TracerHelper.Trace("Interface removed..." + sessionID);

                    if (ClientDisConnected != null)
                    {
                        ClientDisConnected(sessionID);
                    }

                    if (context.Channel.State == CommunicationState.Opened ||
                        context.Channel.State == CommunicationState.Opening)
                    {
                        TracerHelper.Trace("Closing client channel [" + context.Channel.SessionId + "].");
                        context.Channel.Close(TimeSpan.FromSeconds(2));
                        context.Channel.Abort();
                    }
                }
                catch (Exception ex)
                {
                    TracerHelper.Trace("Error encountered while clearing bad interface [" + ex.Message + "].");
                }
            }
        }

        public void VerifyClientConnections(object param)
        {
            Dictionary<IMessageContainerTransport, OperationContext> clientsContexts;
            lock (_contextManager)
            {
                clientsContexts = new Dictionary<IMessageContainerTransport, OperationContext>(_contextManager.ClientsContextsUnsafe);
            }

            List<IMessageContainerTransport> badInterfaces = new List<IMessageContainerTransport>();
            foreach (IMessageContainerTransport clientCallback in clientsContexts.Keys)
            {
                try
                {
                    if (clientsContexts[clientCallback].Channel.State == CommunicationState.Opened)
                    {
                        clientCallback.Ping();
                    }
                    else
                    {
                        if (clientsContexts[clientCallback].Channel.State == CommunicationState.Faulted ||
                            clientsContexts[clientCallback].Channel.State == CommunicationState.Closed ||
                            clientsContexts[clientCallback].Channel.State == CommunicationState.Closing)
                        {
                            TracerHelper.Trace("Bad interface established, enlisting for removal [" + clientsContexts[clientCallback].Channel.SessionId + "].");
                            badInterfaces.Add(clientCallback);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TracerHelper.Trace("Bad interface established [" + ex.Message + "], enlisting for removal.");
                    badInterfaces.Add(clientCallback);
                }
            }

            ProcessBadInterfaces(badInterfaces);

            TracerHelper.TraceExit();
        }

        /// <summary>
        /// Set operationContextSessionID to "*" to mark all clients.
        /// </summary>
        /// <param name="operationContextSessionID"></param>
        /// <param name="messageContainer"></param>
        public void SendMessageContainer(string operationContextSessionID, MessageContainer messageContainer)
        {
            TracerHelper.TraceEntry("[" + messageContainer.MessageStreamLength + "]");
            if (messageContainer.MessageStreamLength > MaximumAllowedMessageSize)
            {
                TracerHelper.TraceError("Message too bid. Operation failed.");
                return;
                //throw new Exception("Arbiter::MessageContainerTransportServer::MessageContainer requestMessage is too big. Message will not be sent.");
            }

            Dictionary<IMessageContainerTransport, OperationContext> clientsContexts;
            lock (_contextManager)
            {
                clientsContexts = new Dictionary<IMessageContainerTransport,OperationContext>(_contextManager.ClientsContextsUnsafe);
            }

            List<IMessageContainerTransport> badInterfaces = new List<IMessageContainerTransport>();
            foreach (IMessageContainerTransport clientCallback in clientsContexts.Keys)
            {
                // "*" indicates send to all clients
                if (operationContextSessionID == "*" ||
                    clientsContexts[clientCallback].SessionId == operationContextSessionID)
                {
                    try
                    {
                        TracerHelper.Trace("clientCallback [" + operationContextSessionID + "]");
                        clientCallback.ReceiveMessageContainer(messageContainer);
                        TracerHelper.Trace("clientCallback [" + operationContextSessionID + "] invoked");
                    }
                    catch (Exception ex)
                    {
                        TracerHelper.TraceError("Failed to invoke client interface [" + ex.ToString() + "]");
                        badInterfaces.Add(clientCallback);
                    }
                }
            }

            ProcessBadInterfaces(badInterfaces);

            TracerHelper.TraceExit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageContainer"></param>
        public void ReceiveMessageContainer(MessageContainer messageContainer)
        {
            TracerHelper.Trace("[" + messageContainer.MessageStreamLength + "], clientCallback[" + OperationContext.Current.SessionId + "]");
            if (MessageContainerReceivedEvent != null)
            {
                string sessionId = OperationContext.Current.SessionId;
                GeneralHelper.FireAndForget(delegate()
                {
                    try
                    {
                        MessageContainerReceivedEvent(sessionId, messageContainer);
                    }
                    catch (Exception ex)
                    {
                        TracerHelper.TraceError(ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// Communication protocol implementation.
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            // The contexts are not matched like this, and this results in a false even in proper cases.
            //lock (_contextManager)
            //{
            //    bool result = _contextManager.ClientsContextsUnsafe.ContainsValue(OperationContext.Current.ses);
            //    SystemMonitor.CheckOperationError(result, "Client context mismatch.");
            //    return result;
            //}

            return true;
        }

        public string ClientRegister()
        {// A client has called - register it.

            lock (_contextManager)
            {
                _contextManager.AddContext(OperationContext.Current);
            }

            TracerHelper.Trace("[" + OperationContext.Current.SessionId + "]");
            if (ClientConnected != null)
            {
                ClientConnected(OperationContext.Current.SessionId);
            }
            IMessageContainerTransport callbackInterface = OperationContext.Current.GetCallbackChannel<IMessageContainerTransport>();
            return OperationContext.Current.SessionId;
        }

        #endregion

    }
}
