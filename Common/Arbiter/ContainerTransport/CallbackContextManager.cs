using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Collections.ObjectModel;

namespace Arbiter.MessageContainerTransport
{
    /// <summary>
    /// This class manages the callbacks to the clients of a server. It needs to do so, as the server
    /// does not client enumeration functionality in its ServerHost.
    /// </summary>
    class CallbackContextManager<TInterface>
    {
        Dictionary<TInterface, OperationContext> _clientsContexts = new Dictionary<TInterface, OperationContext>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<TInterface, OperationContext> ClientsContextsUnsafe
        {
            get
            {
                lock (this)
                {
                    return _clientsContexts;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TInterface> CallbackInterfacesUnsafe
        {
            get
            {
                lock (this)
                {
                    return _clientsContexts.Keys;
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CallbackContextManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void AddContext(OperationContext context)
        {
            lock (this)
            {
                if (_clientsContexts.ContainsValue(context) == false)
                {
                    context.Channel.Closed += new EventHandler(Channel_Closed);
                    context.Channel.Closing += new EventHandler(Channel_Closing);
                    context.Channel.Faulted += new EventHandler(Channel_Faulted);
                    TInterface callbackInterface = context.GetCallbackChannel<TInterface>();
                    _clientsContexts.Add(callbackInterface, context);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void RemoveContext(TInterface callbackInterface)
        {
            lock (this)
            {
                _clientsContexts.Remove(callbackInterface);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void RemoveContext(OperationContext inputContext)
        {
            lock (this)
            {
                foreach (TInterface tInterface in _clientsContexts.Keys)
                {
                    if (_clientsContexts[tInterface] == inputContext)
                    {
                        _clientsContexts.Remove(tInterface);
                        return;
                    }
                }
            }
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            // TO DO : improve the handling here, adding removal of the channel context.
            //SystemMonitor.Error("Channel_Faulted");
            //RemoveContext(OperationContext.Current);
        }

        void Channel_Closing(object sender, EventArgs e)
        {
            //SystemMonitor.Error("Channel_Closing");
        }

        void Channel_Closed(object sender, EventArgs e)
        {
            //SystemMonitor.Error("Channel_Closed");
        }
    }
}
