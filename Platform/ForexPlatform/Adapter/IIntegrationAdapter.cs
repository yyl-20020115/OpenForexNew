using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Helper delegate.
    /// </summary>
    public delegate void IntegrationAdapterUpdateDelegate(IIntegrationAdapter adapter);

    /// <summary>
    /// Base interface for integration adapters.
    /// </summary>
    public interface IIntegrationAdapter : IArbiterClient, IOperational, IDisposable
    {
        bool IsStarted { get; }

        /// <summary>
        /// Adapter notifies of persistance data updated.
        /// </summary>
        event IntegrationAdapterUpdateDelegate PersistenceDataUpdateEvent;

        /// <summary>
        /// Adapter starts operation.
        /// </summary>
        bool Start(Platform platform, out string operationResultMessage);

        /// <summary>
        /// Adapter stops operation.
        /// </summary>
        bool Stop(out string operationResultMessage);
    }
}
