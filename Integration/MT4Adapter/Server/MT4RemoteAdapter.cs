using System;
using System.Collections.Generic;
using System.Text;

namespace MT4Adapter
{
    /// <summary>
    /// Final layer of the adapter.
    /// The class layers are as follows:
    /// 
    /// ProxyIntegrationAdapter inherits OperationalTransportClient
    /// MT4RemoteAdapterCommon inherits ProxyIntegrationAdapter
    /// MT4RemoteAdapterData inherits MT4RemoteAdapterCommon
    /// MT4RemoteAdapterOrders inherits MT4RemoteAdapterData
    /// MT4RemoteAdapter inherits MT4RemoteAdapterOrders
    /// 
    /// </summary>
    public class MT4RemoteAdapter : MT4RemoteAdapterOrders
    {
        /// <summary>
        /// 
        /// </summary>
        public MT4RemoteAdapter(Uri serverAddress)
           : base(serverAddress)
        {
        }
    }
}
