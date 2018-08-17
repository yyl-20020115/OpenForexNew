using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// A second generation of the Arbiter model, this one designed with speed in mind.
    /// 
    /// It is also advisable to rely on the .NET framework thread pool since it is planned
    /// a huge speed up in .NET 4.0.
    /// </summary>
    public class ArbiterSlim : IDisposable
    {
        public const int InvalidClientIndex = -1;

        Arbiter _arbiter;

        volatile bool _isDisposed = false;

        volatile ThreadPoolFastEx _threadPool;
        /// <summary>
        /// 
        /// </summary>
        public ThreadPoolFastEx ThreadPool
        {
            get { return _threadPool; }
        }

        /// <summary>
        /// Hot swapping - this is the fastes way of all to access a client, 
        /// without holding an actual reference, and no locks either.
        /// The client Id must contain the index of the list.
        /// This index will never change, since we shall only add items to the list.
        /// 
        /// To evade locking - when adding new items, simply replace the list with a new one.
        /// </summary>
        volatile List<IArbiterSlimClient> _clientsHotSwap = new List<IArbiterSlimClient>();

        volatile Dictionary<Guid, int> _idToIndexHotSwap = new Dictionary<Guid, int>();

        //volatile bool _tracing = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ArbiterSlim(Arbiter arbiter)
        {
            _arbiter = arbiter;
            _threadPool = new ThreadPoolFastEx(arbiter.Name + ".Slim");
            arbiter.ClientAddedEvent += new ClientManagerClientUpdateDelegate(arbiter_ClientAddedEvent);
            arbiter.ClientRemovedEvent += new ClientManagerClientUpdateDelegate(arbiter_ClientRemovedEvent);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _isDisposed = true;

            ThreadPoolFastEx threadPool = _threadPool;
            if (threadPool != null)
            {
                threadPool.Dispose();
                _threadPool = null;
            }
            
            Arbiter arbiter = _arbiter;
            if (arbiter != null)
            {
                arbiter.ClientAddedEvent -= new ClientManagerClientUpdateDelegate(arbiter_ClientAddedEvent);
                arbiter.ClientRemovedEvent -= new ClientManagerClientUpdateDelegate(arbiter_ClientRemovedEvent);
                _arbiter = null;
            }
        }

        #endregion

        void arbiter_ClientAddedEvent(IArbiterClientManager manager, IArbiterClient client)
        {
            AddClient(client.Slim);
        }

        void arbiter_ClientRemovedEvent(IArbiterClientManager manager, IArbiterClient client)
        {
            RemoveClient(client.Slim);
        }

        /// <summary>
        /// Will return negative value (for ex. -1, or see InvalidClientIndex) to indicate not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetClientIndexById(ComponentId id)
        {
            Dictionary<Guid, int> idsToIndex = _idToIndexHotSwap;
            if (idsToIndex.ContainsKey(id.Guid))
            {
                return idsToIndex[id.Guid];
            }

            return InvalidClientIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        public IArbiterSlimClient GetClientByIndex(int id)
        {
            List<IArbiterSlimClient> clients = _clientsHotSwap;
            if (clients.Count <= id || id < 0)
            {
                return null;
            }

            return clients[id];
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AddClient(IArbiterSlimClient client)
        {
            int index = -1;

            lock (this)
            {// We shall perform a swap of the clients collection, to ensure all operations
                // continue while we are doing the add of the client.

                if (GetClientIndexById(client.Client.SubscriptionClientID.Id) >= 0)
                {// Already added.
                    return false;
                }

                // Since once deployed, collection is never modified, reading like this is safe.
                // Just make sure to not use _clients multiple times, rather grab a reference first.
                List<IArbiterSlimClient> clientsHotSwap = new List<IArbiterSlimClient>(_clientsHotSwap);
                clientsHotSwap.Add(client);
                index = clientsHotSwap.Count - 1;

                Dictionary<Guid, int> idToIndexHotSwap = new Dictionary<Guid, int>(_idToIndexHotSwap);
                // This type of assignment will also work with multiple entries.
                idToIndexHotSwap[client.Client.SubscriptionClientID.Id.Guid] = index;

                // Instant swaps.
                _clientsHotSwap = clientsHotSwap;
                _idToIndexHotSwap = idToIndexHotSwap;
            }

            client.OnAddToArbiter(this, index);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemoveClient(IArbiterSlimClient client)
        {
            List<IArbiterSlimClient> clientsHotSwap = _clientsHotSwap;

            int id = client.ArbiterSlimIndex;
            if (clientsHotSwap.Count <= id || id < 0)
            {
                SystemMonitor.OperationError("Failed to remove client from ArbiterSlim.");
                return false;
            }

            if (clientsHotSwap[id].Client != client.Client)
            {
                SystemMonitor.OperationError("Client [" + client.Client.SubscriptionClientID.ToString() + "] does not belong to this arbiter with this ID.");
                return false;
            }

            lock (this)
            {
                clientsHotSwap[id] = null;
                _idToIndexHotSwap[client.Client.SubscriptionClientID.Id.Guid] = -1;
            }

            client.OnRemoveFromArbiter();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Send(int senderID, Envelope envelope)
        {
            if (_isDisposed)
            {// Possible to get disposed while operating here.
                return false;
            }

            IArbiterSlimClient client = GetClientByIndex(envelope.ReceiverArbiterSlimIndex);
            if (client == null)
            {
                SystemMonitor.OperationError("Failed to find arbiter slim client [" + envelope.ReceiverArbiterSlimIndex.ToString() + "].");
                return false;
            }

            return client.Receive(envelope);
        }

    }
}
