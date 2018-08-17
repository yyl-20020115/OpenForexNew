using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using CommonSupport;
using System.Threading;
using System.Configuration;

namespace Arbiter.MessageContainerTransport
{
    /// <summary>
    /// UseSynchronizationContext - this is done to evade locking up on WinForms.
    /// ConcurrencyMode - this is how to receive calls. Default is single.
    /// </summary>
    [CallbackBehavior(UseSynchronizationContext=false)]
    public class MessageContainerTransportClient : IMessageContainerTransport, IDisposable
    {
        TimeSpan _defaultConnectionTimeOut = TimeSpan.FromSeconds(15);

        DateTime _lastServerCall = DateTime.Now;

        TimeSpan _keepAliveTimeOut = TimeSpan.FromSeconds(15);

        TimeSpan _maximumConnectionTimeOut = TimeSpan.FromMinutes(2);
        public TimeSpan MaximumConnectionTimeOut
        {
            get { lock (this) { return _maximumConnectionTimeOut; } }
            set { lock (this) { _maximumConnectionTimeOut = value; } }
        }

        volatile DuplexChannelFactory<IMessageContainerTransport> _channelFactory;
        volatile IMessageContainerTransport _proxyServerInterface;

        volatile string _connectionSessionID;
        /// <summary>
        /// ID of the current connection session.
        /// </summary>
        public string ConnectionSessionID
        {
            get { return _connectionSessionID; }
        }

        /// <summary>
        /// CompletionEvent is raised each time a connection attempt has finished (success or not).
        /// </summary>
        System.Threading.ManualResetEvent _connectingEvent = new System.Threading.ManualResetEvent(false);

        public bool IsConnected
        {
            get
            {
                lock (this)
                {
                    return (_proxyServerInterface != null
                        && ((IChannel)_proxyServerInterface).State == CommunicationState.Opened);
                }
            }
        }

        /// <summary>
        /// CompletionEvent used to signal the operation of the single static connection thread.
        /// </summary>
        static List<MessageContainerTransportClient> Clients = new List<MessageContainerTransportClient>();

        public delegate void ConnectionStatusChangedDelegate(bool connected);
        public event ConnectionStatusChangedDelegate ConnectionStatusChangedEvent;
        public event HandlerDelegate<MessageContainer> MessageContainerReceivedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serverAddressUri"></param>
        public MessageContainerTransportClient()
        {
        }

        public void Dispose()
        {
            UnInitialize();
        }

        public bool Initialize(Uri serverAddressUri)
        {
            lock (this)
            {
                EndpointAddress serverAddress = new EndpointAddress(serverAddressUri);

                try
                {
                    // If you get a configuration exception here in debug mode, just *ignore it* (disable it from Debug >> Exceptions), 
                    // normal execution exceptions is just one more of the WCFs excellent features:
                    // see more here: http://ari-techno.blogspot.com/2008/08/this-element-is-not-currently.html

                    _channelFactory = new DuplexChannelFactory<IMessageContainerTransport>(
                        this, MessageContainerTransportServer.CreateBinding(), serverAddress);
                }
                catch (ConfigurationErrorsException)
                {// This is a dummy catch, just to shut the debugger up.
                }

                lock (Clients)
                {
                    if (Clients.Contains(this) == false)
                    {
                        Clients.Add(this);

                        if (Clients.Count == 1)
                        {// We are the single new starting client, fire the connection monitoring thread.
                            GeneralHelper.FireAndForget(ConnectionMonitorThread);
                        }
                    }
                }
                
            }

            return true;
        }

        /// <summary>
        /// Uninitialize the client from operation.
        /// </summary>
        /// <returns></returns>
        public bool UnInitialize()
        {
            lock (Clients)
            {
                Clients.Remove(this);
            }

            lock (this)
            {
                if (_proxyServerInterface != null)
                {
                    try
                    {
                        ((IChannel)_proxyServerInterface).Opened -= new EventHandler(MessageContainerTransportClient_Opened);
                        ((IChannel)_proxyServerInterface).Opening -= new EventHandler(MessageContainerTransportClient_Opening);
                        ((IChannel)_proxyServerInterface).Faulted -= new EventHandler(MessageContainerTransportClient_Faulted);
                        ((IChannel)_proxyServerInterface).Closing -= new EventHandler(MessageContainerTransportClient_Closing);

                        ((IChannel)_proxyServerInterface).Abort();
                    }
                    catch(Exception ex)
                    {
                        SystemMonitor.OperationError(string.Empty, ex);
                    }
                    finally
                    {
                        _proxyServerInterface = null;
                    }
                }

                if (_channelFactory != null)
                {
                    try
                    {
                        _channelFactory.Abort();
                    }
                    catch(Exception ex)
                    {
                        SystemMonitor.OperationError(string.Empty, ex);
                    }
                    finally
                    {
                        _channelFactory = null;
                    }
                }

                _connectingEvent.Reset();
            }

            return true;
        }
        

        void MessageContainerTransportClient_Opening(object sender, EventArgs e)
        {
        }

        void MessageContainerTransportClient_Opened(object sender, EventArgs e)
        {
            // This event is NOT properly raised, it is reaised also on failed connection.
        }

        void MessageContainerTransportClient_Closing(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_connectionSessionID) == false && ConnectionStatusChangedEvent != null)
            {
                ConnectionStatusChangedEvent(false);
            }
            _connectionSessionID = string.Empty;
        }

        void MessageContainerTransportClient_Faulted(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_connectionSessionID) == false && ConnectionStatusChangedEvent != null)
            {
                ConnectionStatusChangedEvent(false);
            }
            _connectionSessionID = string.Empty;
        }

        /// <summary>
        /// Monitor connection state and recconect if needed.
        /// </summary>
        static protected void ConnectionMonitorThread()
        {
            while (GeneralHelper.ApplicationClosing == false)
            {
                Thread.Sleep(500);

                MessageContainerTransportClient[] clients;

                lock (Clients)
                {
                    if (Clients.Count == 0)
                    {
                        return;
                    }

                    clients = Clients.ToArray();
                }

                foreach (MessageContainerTransportClient client in clients)
                {
                    bool keepAliveTimeOut;
                    IMessageContainerTransport messageInterface;
                    lock (client)
                    {
                        keepAliveTimeOut = ((DateTime.Now - client._lastServerCall) >= client._keepAliveTimeOut);
                        messageInterface = client._proxyServerInterface;
                    }

                    if (keepAliveTimeOut && client.IsConnected && messageInterface != null)
                    {
                        try
                        {
                            TracerHelper.Trace("Pinging...");
                            bool result = messageInterface.Ping();
                            lock (client)
                            {
                                client._lastServerCall = DateTime.Now;
                                if (result == false)
                                {// Server returned false.
                                    TracerHelper.Trace("Closing client interface...");

                                    ((IChannel)client._proxyServerInterface).Close(TimeSpan.FromSeconds(2));
                                    ((IChannel)client._proxyServerInterface).Abort();
                                    client._proxyServerInterface = null;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            SystemMonitor.OperationWarning("Connection to server failed [" + ex.Message + "].");
                        }
                    }

                    lock (client)
                    {
                        if (client._channelFactory == null)
                        {// Client has no factory, disposed or not initialized, skip it.
                            continue;
                        }

                        CommunicationState state = CommunicationState.Closed;
                        if (client._proxyServerInterface != null)
                        {
                            state = ((IChannel)client._proxyServerInterface).State;
                        }

                        // Synchronized entering here.
                        if (client._proxyServerInterface != null
                            && state != CommunicationState.Closed
                                && state != CommunicationState.Closing
                                && state != CommunicationState.Faulted
                            )
                        {// Interface OK, continue.
                            continue;
                        }
                    }

                    CreateClientProxyInterface(client);
                
                }// For each client
            
            }// While
        }

        /// <summary>
        /// Helper.
        /// </summary>
        static void CreateClientProxyInterface(MessageContainerTransportClient client)
        {
            TracerHelper.TraceEntry();

            if (client._proxyServerInterface != null &&
                client._connectingEvent.WaitOne(client._defaultConnectionTimeOut) == false)
            {
                SystemMonitor.OperationError("Timed out, since another creation is in progress.");
                return;
            }

            SystemMonitor.CheckOperationWarning(string.IsNullOrEmpty(client._connectionSessionID), "Session seems to be active.");

            client._connectingEvent.Reset();

            try
            {
                IMessageContainerTransport serverInterface;
                DuplexChannelFactory<IMessageContainerTransport> channelFactory = null;

                lock (client)
                {
                    if (client._proxyServerInterface != null)
                    {
                        ((IChannel)client._proxyServerInterface).Opened -= new EventHandler(client.MessageContainerTransportClient_Opened);
                        ((IChannel)client._proxyServerInterface).Opening -= new EventHandler(client.MessageContainerTransportClient_Opening);
                        ((IChannel)client._proxyServerInterface).Faulted -= new EventHandler(client.MessageContainerTransportClient_Faulted);
                        ((IChannel)client._proxyServerInterface).Closing -= new EventHandler(client.MessageContainerTransportClient_Closing);
                    }

                    client._connectionSessionID = string.Empty;
                    if (client._channelFactory == null)
                    {
                        return;
                    }

                    channelFactory = client._channelFactory;
                }

                serverInterface = channelFactory.CreateChannel();

                ((IChannel)serverInterface).Opened += new EventHandler(client.MessageContainerTransportClient_Opened);
                ((IChannel)serverInterface).Opening += new EventHandler(client.MessageContainerTransportClient_Opening);
                ((IChannel)serverInterface).Faulted += new EventHandler(client.MessageContainerTransportClient_Faulted);
                ((IChannel)serverInterface).Closing += new EventHandler(client.MessageContainerTransportClient_Closing);
                
                lock(client)
                {
                    client._proxyServerInterface = serverInterface;
                }

                ((IChannel)serverInterface).BeginOpen(TimeSpan.FromSeconds(10), BeginOpenAsyncCallback, client);

                //client._connectionSessionID = serverInterface.ClientRegister();
                //SystemMonitor.Report("Client registered sessionID [" + client.ConnectionSessionID + "]");
                
                //// Release this as early as possible, since it might be needed before the event is raised.
                //client._connectingEvent.Set();

                //if (client.ConnectionStatusChangedEvent != null)
                //{
                //    GeneralHelper.FireAndForget(delegate() { client.ConnectionStatusChangedEvent(true); });
                //}
            }
            catch (Exception ex)
            {
                TracerHelper.Trace("Failed to create proxy interface [" + ex.Message + "]");
            }
            finally
            {// Always make sure to leave the baseMethod with a released event.
                //client._connectingEvent.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void BeginOpenAsyncCallback(IAsyncResult ar)
        {
            MessageContainerTransportClient client = (MessageContainerTransportClient)ar.AsyncState;

            try
            {
                IMessageContainerTransport serverInterface;
                lock (client)
                {
                    serverInterface = client._proxyServerInterface;
                }

                if (serverInterface == null)
                {
                    return;
                }

                if (ar.IsCompleted)
                {
                    client._connectionSessionID = serverInterface.ClientRegister();
                    SystemMonitor.Report("Client registered sessionID [" + client.ConnectionSessionID + "]");

                    //// Release this as early as possible, since it might be needed before the event is raised.
                    //client._connectingEvent.Set();
                }

                ((IChannel)serverInterface).EndOpen(ar);
            }
            catch (Exception ex)
            {
                TracerHelper.Trace("Failed to create proxy interface [" + ex.Message + "]");
            }
            finally
            {// This will get executed even if the return statement is hit.
                client._connectingEvent.Set();
            }

            if (client.ConnectionStatusChangedEvent != null)
            {
                GeneralHelper.FireAndForget(delegate() { client.ConnectionStatusChangedEvent(true); });
            }

        }

        public void ChangeServerAddressUri(Uri serverAddressUri)
        {
            lock (this)
            {
                EndpointAddress serverAddress = new EndpointAddress(serverAddressUri);
                _channelFactory = new DuplexChannelFactory<IMessageContainerTransport>(
                    this, MessageContainerTransportServer.CreateBinding(), serverAddress);
                _proxyServerInterface = null;
            }
        }

        #region IMessageContainerTransport Members

        /// <summary>
        /// Send container over to the other side.
        /// </summary>
        /// <param name="messageContainer"></param>
        /// <returns></returns>
        public virtual bool SendMessageContainer(MessageContainer messageContainer)
        {
            TracerHelper.TraceEntry("sending [" + messageContainer.MessageStreamLength + "], sessionID[" + this.ConnectionSessionID + "]");

            SystemMonitor.CheckThrow(messageContainer.MessageStreamLength <= MessageContainerTransportServer.MaximumAllowedMessageSize, "MessageContainer message is too big. Message will not be sent.");
            if (_connectingEvent.WaitOne(_defaultConnectionTimeOut) == false)
            {
                SystemMonitor.OperationError("Failed to send message due to initialization timeout.", TracerItem.PriorityEnum.Medium);
                return false;
            }

            try
            {
                IMessageContainerTransport messageInterface;
                lock (this)
                {
                    if (_proxyServerInterface == null)
                    {
                        TracerHelper.TraceError("Failed to send to server, interface not created.");
                        return false;
                    }
                    messageInterface = _proxyServerInterface;
                }

                messageInterface.ReceiveMessageContainer(messageContainer);
                lock (this)
                {
                    _lastServerCall = DateTime.Now;
                }
            }
            catch (Exception exception)
            {
                TracerHelper.TraceError("Failed to send to server, reason [" + exception.Message + "].");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Receive container from the server.
        /// </summary>
        /// <param name="messageContainer"></param>
        public virtual void ReceiveMessageContainer(MessageContainer messageContainer)
        {
            //_connectionSessionID = OperationContext.Current.SessionId;
            if (MessageContainerReceivedEvent != null)
            {
                TracerHelper.Trace("receiving [" + messageContainer.MessageStreamLength + "], sessionID[" + this.ConnectionSessionID + "]");
                MessageContainerReceivedEvent(messageContainer);
            }
            else
            {
                SystemMonitor.OperationError("container missed [" + messageContainer.MessageStreamLength + "], sessionID[" + this.ConnectionSessionID + "]");
            }

            lock (this)
            {
                _lastServerCall = DateTime.Now;
            }
        }

        public bool Ping()
        {
            lock (this)
            {
                _lastServerCall = DateTime.Now;
            }

            return true;
        }

        public string ClientRegister()
        {
            throw new Exception("The method or operation is intentionally not implemented and should not be invoked.");
        }

        #endregion

    }
}
