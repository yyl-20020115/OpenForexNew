using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Arbiter;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Provides an implementation of a proxy integration adapter.
    /// It manages the routing of messages to the proxy adapter (arbiter integration server)
    /// trough a local client (arbiter integration client), as well as 
    /// register itself as a source locally so that the source mechanism can 
    /// work, but some messages are directly proxy forwarded to proxy adapter,
    /// so no thread blocking occurs here.
    /// </summary>
    public class ProxyIntegrationAdapter : OperationalTransportClient
    {
        protected TransportIntegrationServer _integrationServer;

        TransportInfo _subscriberTransportMessageInfo = null;

        public Uri ServerIntegrationUri
        {
            get
            {
                if (_integrationServer != null)
                {
                    return _integrationServer.Address;
                }

                SystemMonitor.Error("Integration server not initialized, returned Uri empty.");
                return new Uri("");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProxyIntegrationAdapter(Uri serverIntegrationUri)
            : base(typeof(ProxyIntegrationAdapter).Name, false)
        {
            TracerHelper.Tracer.Clear(true);

            // Locate the trace file, next to the MT4IntegrationDLL, that is deployed inside the
            // corresponding MT4 expert/libraries folder.
            FileTracerItemSink sink = new FileTracerItemSink(TracerHelper.Tracer);

            sink.FilePath = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Experts"), 
                "OFxP.Expert." + GeneralHelper.GetFileCompatibleDateTime(DateTime.Now) + ".log");

            TracerHelper.Tracer.Add(sink);

            TracerHelper.TraceEntry();

            // This.
            Filter.Enabled = true;
            Filter.AllowOnlyAddressedMessages = false;
            Filter.AllowChildrenTypes = true;
            Filter.AllowedNonAddressedMessageTypes.Add(typeof(TransportMessage));

            Arbiter.Arbiter arbiter = new Arbiter.Arbiter("IntegrationServer.Arbiter");
            arbiter.AddClient(this);

            // Integration server.
            _integrationServer = new TransportIntegrationServer(serverIntegrationUri);
            if (arbiter.AddClient(_integrationServer) == false)
            {// Failed to add/initialize integration server.
                _integrationServer = null;
                MessageBox.Show("Failed to initialize OFxP Integration Server." + System.Environment.NewLine + "Possibly another integration with the same address [" + serverIntegrationUri.ToString() + "] already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ChangeOperationalState(OperationalStateEnum.Operational);

        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~ProxyIntegrationAdapter()
        {
            TracerHelper.TraceEntry();
        }

        /// <summary>
        /// Un initialize manager, notify subsribers, clear resources.
        /// </summary>
        public void UnInitialize()
        {
            TracerHelper.TraceEntry();

            ChangeOperationalState(OperationalStateEnum.UnInitialized);

            lock (this)
            {
                if (_subscriberTransportMessageInfo != null)
                {// Send update to subscriber to let him now we are closing down.
                    this.SendResponding(_subscriberTransportMessageInfo, new SubscriptionTerminatedMessage());

                    _subscriberTransportMessageInfo = null;
                }
            }

            // Give time to the termination message to arrive.
            System.Threading.Thread.Sleep(1000);

            if (_integrationServer != null && Arbiter != null)
            {
                Arbiter.RemoveClient(_integrationServer);
            }
        }

        public virtual void Dispose()
        {
        }

        #region Arbiter Messaging

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void SendToSubscriber(TransportMessage message)
        {
            TracerHelper.TraceEntry(message.GetType().Name);
            
            lock (this)
            {
                if (_subscriberTransportMessageInfo != null)
                {
                    this.SendResponding(_subscriberTransportMessageInfo, message);
                }
            }

            TracerHelper.TraceExit();
        }

        /// <summary>
        /// Helper, unused.
        /// </summary>
        /// <param name="inputMessage"></param>
        /// <returns></returns>
        protected bool VerifySubscriber(TransportMessage inputMessage)
        {
            if (_subscriberTransportMessageInfo == null)
            {
                return false;
            }

            lock (this)
            {
                return _subscriberTransportMessageInfo.CheckOriginalSender(inputMessage.TransportInfo);
            }
        }

        public void RegisterSource()
        {
            RegisterSourceMessage message = new RegisterSourceMessage(true);
            message.SourceType = SourceTypeEnum.DataProvider | SourceTypeEnum.Live | SourceTypeEnum.HighPriority;
            message.RequestResponse = false;

            SendAddressed(_integrationServer.SubscriptionClientID, message);

            message = new RegisterSourceMessage(true);
            message.SourceType = SourceTypeEnum.OrderExecution | SourceTypeEnum.Live;
            message.RequestResponse = false;

            SendAddressed(_integrationServer.SubscriptionClientID, message);
        }

        /// <summary>
        /// Receive a request from a client to be subscribed to the sessions events on this server.
        /// </summary>
        [MessageReceiver]
        ResponseMessage Receive(SubscribeMessage message)
        {
            TracerHelper.TraceEntry();

            lock (this)
            {
                if (_subscriberTransportMessageInfo != null)
                {// Notify existing subscriber he lost control.
                    this.SendResponding(_subscriberTransportMessageInfo, new SubscriptionTerminatedMessage());
                    _subscriberTransportMessageInfo = null;
                }

                _subscriberTransportMessageInfo = message.TransportInfo.Clone();
            }

            RegisterSource();

            if (message.RequestResponse)
            {
                return new ResponseMessage(true);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        [MessageReceiver]
        ResponseMessage Receive(UnSubscribeMessage message)
        {
            lock (this)
            {
                _subscriberTransportMessageInfo = null;
            }

            if (message.RequestResponse)
            {
                return new ResponseMessage(true);
            }

            return null;
        }

        #endregion
    }
}
