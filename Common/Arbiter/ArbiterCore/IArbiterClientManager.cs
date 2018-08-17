using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// Helper delegate.
    /// </summary>
    public delegate void ClientManagerClientUpdateDelegate(IArbiterClientManager manager, IArbiterClient client);

    /// <summary>
    /// Base interface for arbiter client manager.
    /// </summary>
    public interface IArbiterClientManager
    {
        /// <summary>
        /// Add a client to manager.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        bool AddClient(IArbiterClient client);

        /// <summary>
        /// Remove client from manager.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        bool RemoveClient(IArbiterClient client);

        /// <summary>
        /// Obtain a client instance based on its Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IArbiterClient GetClientByID(ArbiterClientId id);

        event ClientManagerClientUpdateDelegate ClientAddedEvent;
        event ClientManagerClientUpdateDelegate ClientRemovedEvent;
    }
}
