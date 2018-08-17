using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ProxySourceTest
{
    /// <summary>
    /// 
    /// </summary>
    public class ProxyIntegrationAdapterImplementation : OperationalTransportClient
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
        public ProxyIntegrationAdapterImplementation(Uri serverIntegrationUri)
            : base(typeof(ProxyIntegrationAdapterImplementation).Name, false)
        {
            TracerHelper.Tracer.Clear(true);

            // Locate the trace file, next to the MT4IntegrationDLL, that is deployed inside the
            // corresponding MT4 expert/libraries folder.
            FileTracerItemSink sink = new FileTracerItemSink(TracerHelper.Tracer);

            sink.FilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "OFxP.Expert.log");
            TracerHelper.Tracer.Add(sink);

            //TracerHelper.Trace("STARTUP:" + Application.StartupPath);
            //TracerHelper.Trace("STARTUP:" + Assembly.GetExecutingAssembly().Location);
            //TracerHelper.Trace("STARTUP:" + );

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
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~ProxyIntegrationAdapterImplementation()
        {
            TracerHelper.TraceEntry();
        }

        /// <summary>
        /// Un initialize manager, notify subsribers, clear resources.
        /// </summary>
        public void UnInitialize()
        {
            TracerHelper.TraceEntry();
            if (_subscriberTransportMessageInfo != null)
            {// Send update to subscriber to let him now we are closing down.
                this.SendResponding(_subscriberTransportMessageInfo, new SubscriptionTerminatedMessage());

                _subscriberTransportMessageInfo = null;
            }

            if (_integrationServer != null && Arbiter != null)
            {
                Arbiter.RemoveClient(_integrationServer);
            }

            // Give time to the termination message to arrive.
            System.Threading.Thread.Sleep(1000);
        }


        public virtual void Dispose()
        {
        }

        #region Arbiter Messaging

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SendToSubscriber(TransportMessage message)
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

        /// <summary>
        /// Receive a request from a client to be subscribed to the sessions events on this server.
        /// </summary>
        [MessageReceiver]
        ResponceMessage Receive(SubscribeMessage message)
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

            if (message.RequestResponce)
            {
                return new ResponceMessage(true);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        [MessageReceiver]
        ResponceMessage Receive(UnSubscribeMessage message)
        {
            lock (this)
            {
                _subscriberTransportMessageInfo = null;
            }

            if (message.RequestResponce)
            {
                return new ResponceMessage(true);
            }

            return null;
        }

        #endregion
    }
}
