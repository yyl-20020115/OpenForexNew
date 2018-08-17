using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ServiceModel;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Common delegates.
    /// </summary>
    public delegate void HandlerDelegate();
    public delegate void HandlerDelegate<TParameterOne>(TParameterOne parameter1);
    public delegate void HandlerDelegate<TParameterOne, TParameterTwo>(TParameterOne parameter1, TParameterTwo parameter2);

    /// <summary>
    /// The main class for the Arbiter model. This is where the execution passes trough.
    /// It accepts messages and routes them to proper receivers.
    /// </summary>
    public class Arbiter : IArbiterClientManager, IDisposable
    {
        TimeOutMonitor _timeOutMonitor = new TimeOutMonitor();
        ExecutionManager _executionManager;

        protected Dictionary<IArbiterClient, MessageFilter> _clientsAndFilters = new Dictionary<IArbiterClient, MessageFilter>();
        protected SortedDictionary<ArbiterClientId, IArbiterClient> _clientsIdsAndClients = new SortedDictionary<ArbiterClientId, IArbiterClient>();

        volatile string _name;
        /// <summary>
        /// Name of this arbiter.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        protected volatile bool _isDisposed = false;

        public event ClientManagerClientUpdateDelegate ClientAddedEvent;
        public event ClientManagerClientUpdateDelegate ClientRemovedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Arbiter(string name)
        {
            _name = name;
            _timeOutMonitor.EntityTimedOutEvent += new HandlerDelegate<TimeOutable>(_timeOutMonitor_EntityTimedOutEvent);
            _executionManager = new ExecutionManager(this);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _isDisposed = true;

            lock (_timeOutMonitor)
            {
                _timeOutMonitor.Dispose();
                _timeOutMonitor = null;
            }

            lock (_executionManager)
            {
                _executionManager.Dispose();
                _executionManager = null;
            }

            lock (_clientsAndFilters)
            {// Remove all clients to allow them to UnInitialize, and if they have any unmanaged resources - release them.
                List<IArbiterClient> clients = new List<IArbiterClient>(_clientsAndFilters.Keys);
                foreach(IArbiterClient client in clients)
                {
                    this.RemoveClient(client);
                }
            }
        }

        #endregion

        ///// <summary>
        ///// The direct call allows to circumvent the many steps (incl serialization) of typical message sending
        ///// and make a direct call to another Arbiter member; thus making a much faster delivery. This path
        ///// has a maximum optimization for speed, so tracing etc. are disabled.
        ///// 
        ///// Also this mechanism only works for TransportClients currently.
        ///// 
        ///// The mechanism does not utilize any new threads, and the execution is performed on the calling thread.
        ///// 
        ///// Direct calls can only be made to participants on the same arbiter, and no addressing is applied
        ///// for the messages.
        ///// </summary>
        //public Message DirectCall(ArbiterClientId senderID, ArbiterClientId receiverID, Message message)
        //{
        //    IArbiterClient receiver = GetClientByID(receiverID, true);
        //    if (receiver == null || receiver is TransportClient == false)
        //    {
        //        SystemMonitor.OperationWarning("Sender [" + senderID.Id.Print() + "] creating conversation message [" + message.GetType().Name + " ] by not present receiver [" + receiverID.Id.Print() + "] or receiver not a TransportClient");
        //        return null;
        //    }

        //    Message response = receiver.ReceiveDirectCall(message);
        //    return response;
        //}

        /// <summary>
        /// Will send a point to point requestMessage and start a conversation that can have many replies.
        /// </summary>
        public ConversationPointToPoint CreateConversation(ArbiterClientId senderID, ArbiterClientId receiverID, Message message, TimeSpan timeout)
        {
            SystemMonitor.CheckError(((TransportMessage)message).TransportInfo.CurrentTransportInfo != null);

            if (message is TransportMessage)
            {
                SystemMonitor.CheckError(((TransportMessage)message).TransportInfo.CurrentTransportInfo != null);
                TracerHelper.Trace("sender[" + senderID.Id.Name + "], receiver [" + receiverID.Id.Name + "], message [" + message.GetType().Name + "] [" + ((TransportMessage)message).TransportInfo.TransportInfoCount + "]");
            }
            else
            {
                TracerHelper.Trace("sender[" + senderID.Id.Name + "], message [" + message.GetType().Name + "]");
            }

            if (GetClientByID(senderID, false) == null)
            {
                SystemMonitor.Error("Creating conversation from a not present sender [" + message.GetType().Name + ", " + senderID.Id.Print() + "].");
                return null;
            }

            if (GetClientByID(receiverID, false) == null)
            {
                SystemMonitor.OperationWarning("Sender [" + senderID.Id.Print() + "] creating conversation message [" + message.GetType().Name +" ] by not present receiver [" + receiverID.Id.Print() + "]");
                return null;
            }

            ConversationPointToPoint conversation = new ConversationPointToPoint(_executionManager, message, senderID, receiverID, timeout);
            if (_isDisposed)
            {// Possible to get disposed while operating here.
                return null;
            }

            lock (_timeOutMonitor)
            {
                _timeOutMonitor.AddEntity(conversation);
            }

            return conversation;
        }

        /// <summary>
        /// Will start a shoutcast mode conversation, the sender sending to all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="requestMessage"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public ConversationMultiPoint CreateConversation(ArbiterClientId senderID, Message message, TimeSpan timeout)
        {
            if (message is TransportMessage)
            {
                SystemMonitor.CheckError(((TransportMessage)message).TransportInfo.CurrentTransportInfo != null);
                TracerHelper.Trace("sender[" + senderID.Id.Name + "], message [" + message.GetType().Name + "] [" + ((TransportMessage)message).TransportInfo.TransportInfoCount + "]");
            }
            else
            {
                TracerHelper.Trace("sender[" + senderID.Id.Name + "], message [" + message.GetType().Name + "]");
            }

            if (GetClientByID(senderID, false) == null)
            {
                SystemMonitor.Error("Creating conversation by not present sender/owner.");
                return null;
            }

            ConversationMultiPoint conversation = new ConversationMultiPoint(_executionManager, message, senderID, GatherMessageClients(message, senderID), timeout);
            
            if (_isDisposed)
            {// Possible to get disposed while operating here.
                return null;
            }
            
            lock (_timeOutMonitor)
            {
                _timeOutMonitor.AddEntity(conversation);
            }
            return conversation;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter0"></param>
        protected void _timeOutMonitor_EntityTimedOutEvent(TimeOutable parameter0)
        {
            Conversation conversation = (Conversation)parameter0;
            IArbiterClient client = GetClientByID(conversation.OwnerID, false);
            if (client != null)
            {
                client.ReceiveConversationTimedOut(conversation);
            }
        }
        
        /// <summary>
        /// Find out what clients need to receive this requestMessage.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public IEnumerable<ArbiterClientId> GatherMessageClients(Message message, ArbiterClientId senderID)
        {
            List<ArbiterClientId> clients = new List<ArbiterClientId>();
            if (_isDisposed)
            {// Possible to get disposed while operating here.
                return null;
            }

            lock (_clientsAndFilters)
            {
                foreach (IArbiterClient client in _clientsAndFilters.Keys)
                {
                    if (_clientsAndFilters[client].MessageAllowed(message))
                    {
                        clients.Add(client.SubscriptionClientID);
                    }
                }
            }
            return clients;
        }

        #region IArbiterClientManager Members

        /// <summary>
        /// Add a client to the arbiter, will raise the ClientAddedEvent if successful.
        /// </summary>
        public bool AddClient(IArbiterClient client)
        {
            if (client == null || GetClientByID(client.SubscriptionClientID, false) != null)
            {
                return false;
            }

            if (_isDisposed)
            {// Possible to get disposed while operating here.
                return false;
            }

            lock (_clientsAndFilters)
            {
                if (_clientsAndFilters.ContainsKey(client))
                {
                    return false;
                }
                _clientsAndFilters.Add(client, client.SubscriptionMessageFilter);
            }

            lock (_clientsIdsAndClients)
            {
                _clientsIdsAndClients.Add(client.SubscriptionClientID, client);
            }

            if (client.ArbiterInitialize(this) == false || _isDisposed)
            {
                lock (_clientsAndFilters)
                {
                    _clientsAndFilters.Remove(client);
                }

                lock (_clientsIdsAndClients)
                {
                    _clientsIdsAndClients.Remove(client.SubscriptionClientID);
                }

                SystemMonitor.OperationError("Client failed arbiter initialialization [" + client.Name + "]");
                return false;
            }

            ClientManagerClientUpdateDelegate clientAddedDelegate = ClientAddedEvent;
            if (clientAddedDelegate != null)
            {
                clientAddedDelegate(this, client);
            }

            return true;
        }

        /// <summary>
        /// Remove the client from the arbiter (will raise the ClientRemovedEvent if successful).
        /// </summary>
        public bool RemoveClient(IArbiterClient client)
        {
            if (_isDisposed)
            {// Possible to get disposed while operating here.
                return false;
            }

            bool result = false;
            lock (_clientsAndFilters)
            {
                client.ArbiterUnInitialize();
                result = _clientsAndFilters.Remove(client) && _clientsIdsAndClients.Remove(client.SubscriptionClientID);
            }

            ClientManagerClientUpdateDelegate clientRemovedDelegate = ClientRemovedEvent;
            if (result && clientRemovedDelegate != null)
            {
                clientRemovedDelegate(this, client);
            }

            return result;
        }

        /// <summary>
        /// Check if there is a client with this ID to this Arbiter instance.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasClient(ArbiterClientId id)
        {
            lock (_clientsIdsAndClients)
            {
                return _clientsIdsAndClients.ContainsKey(id);
            }
        }

        /// <summary>
        /// Allows to retrieve an instance of a client by its ID. Use with caution since it breaks the independence model 
        /// and is applied only in special cases.
        /// </summary>
        public IArbiterClient GetClientByID(ArbiterClientId id)
        {
            return GetClientByID(id, false);
        }

        /// <summary>
        /// Will return NULL if the client has left arbiter. Do not keep references to this as it will keep objects alive!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IArbiterClient GetClientByID(ArbiterClientId id, bool useOptionalReference)
        {
            if (_isDisposed)
            {// Possible to get disposed while operating here.
                return null;
            }

            if (useOptionalReference && id.OptionalReference != null)
            {// Optional reference allows to circumvent the usage of the dictionary, 
                // to establish the corresponding client for this id.
                return id.OptionalReference;
            }

            lock (_clientsIdsAndClients)
            {
                IArbiterClient resultValue;
                if (_clientsIdsAndClients.TryGetValue(id, out resultValue))
                {
                    return resultValue;
                }
                else
                {
                    return null;
                }
            }
        }


        #endregion


    }

}
