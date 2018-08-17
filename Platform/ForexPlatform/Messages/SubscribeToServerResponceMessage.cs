using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Message to subsribe to a integration manager. Once subsribed, operations can be performed.
    /// </summary>
    [Serializable]
    public class SubscribeToServerResponceMessage : TransportMessage
    {
        ArbiterClientId? _dataSourceId;

        public ArbiterClientId? DataSourceId
        {
          get { return _dataSourceId; }
          set { _dataSourceId = value; }
        }
        
        ArbiterClientId? _orderExecutionSourceId;

public ArbiterClientId? OrderExecutionSourceId
{
  get { return _orderExecutionSourceId; }
  set { _orderExecutionSourceId = value; }
}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="knownSessions">The subscribee sends all sessions that it has knowledge about; in case those need to be mapped to new sessions.</param>
        public SubscribeToServerMessage()
        {
        }
    }
}
