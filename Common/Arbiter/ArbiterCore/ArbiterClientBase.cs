using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using CommonSupport;
using System.IO;

namespace Arbiter
{
    /// <summary>
    /// This is a helper abstract base class for the new ArbiterClients. It is not mandatory to use,
    /// as the arbiter works with the IArbiterClientEx directly.
    /// </summary>
    [Serializable]
    public abstract class ArbiterClientBase : IArbiterClient, ISerializable
    {
        volatile Arbiter _arbiter;
        /// <summary>
        /// Get the instance of the Arbiter class, that this client is assigned to.
        /// </summary>
        public Arbiter Arbiter
        {
            get { return _arbiter; }
          set { this._arbiter = value; }
        }

        protected ArbiterClientId _subscriptionClientId;
        protected volatile MessageFilter _messageFilter;
        
        private bool _singleThreadOnly = false;

        /// <summary>
        /// Gets or sets the name of this client instance.
        /// </summary>
        public string Name
        {
            get { return _subscriptionClientId.ClientName; }
            set { _subscriptionClientId.ClientName = value; }
        }

        /// <summary>
        /// Get the general state of this client, has it been initialized to an Arbiter.
        /// </summary>
        public bool IsArbiterInitialized
        {
            get { return _arbiter != null; }
        }

        #region IArbiterClient Members

        /// <summary>
        /// Get the clients subscription ID, unique for the Arbiter (and in general), 
        /// it allows other clients to communicate with this module using messages.
        /// </summary>
        public ArbiterClientId SubscriptionClientID
        {
            get { return _subscriptionClientId; }
        }

        /// <summary>
        /// Get the client requestMessage filter. The filter defines what messages are allowed to be received by this client.
        /// </summary>
        public MessageFilter SubscriptionMessageFilter
        {
            get { return _messageFilter; }
        }

        /// <summary>
        /// Gets the mode in which this client is running. Single thread means the arbiter will only allow one thread to enter this client at one time.
        /// </summary>
        public bool SingleThreadMode
        {
            get { return _singleThreadOnly; }
        }

        public abstract void ReceiveExecution(ExecutionEntity entity);
        public abstract void ReceiveExecutionWithReply(ExecutionEntityWithReply entity);
        public abstract void ReceiveConversationTimedOut(Conversation conversation);
        public abstract Message ReceiveDirectCall(Message message);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of this client module.</param>
        /// <param name="singleThreadOnly">Should the module be entered with single or multiple Arbiter threads.</param>
        public ArbiterClientBase(string idName, bool singleThreadOnly)
        {
            if (string.IsNullOrEmpty(idName))
            {// Try to establish user friendly name, or if not available, use class name.
                idName = UserFriendlyNameAttribute.GetTypeAttributeName(this.GetType());
            }

            _subscriptionClientId = new ArbiterClientId(idName, this.GetType(), this);
            _messageFilter = new MessageFilter(true);
            _singleThreadOnly = singleThreadOnly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of this client module.</param>
        /// <param name="singleThreadOnly">Should the module be entered with single or multiple Arbiter threads.</param>
        public ArbiterClientBase(bool singleThreadOnly)
        {
            string name = UserFriendlyNameAttribute.GetTypeAttributeName(this.GetType());

            // Make sure not to use the default struct parameterless constructor.
            _subscriptionClientId = new ArbiterClientId(name, this.GetType(), this);

            _messageFilter = new MessageFilter(true);
            _singleThreadOnly = singleThreadOnly;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="singleThreadOnly"></param>
        public ArbiterClientBase(SerializationInfo info, StreamingContext context)
        {
            _subscriptionClientId = (ArbiterClientId)info.GetValue("clientId", typeof(ArbiterClientId));
            _messageFilter = (MessageFilter)info.GetValue("messageFilter", typeof(MessageFilter));
        }

        /// <summary>
        /// Do not use this in normal circumstances, this is automatically assigned.
        /// Allows to assign element with an existing ID, in order to be recognized in existing paths after de-persistence.
        /// </summary>
        /// <param name="id"></param>
        public bool SetArbiterSubscriptionClientId(ArbiterClientId id)
        {
            if (_arbiter != null)
            {
                SystemMonitor.Error("CAn not assign arbiter subscription id, after item already added to arbiter.");
                return false;
            }

            lock (this)
            {
                _subscriptionClientId = id;
            }

            return true;
        }

        #region ISerializable Members

        /// <summary>
        /// When overriding always make sure to call the parent baseMethod too.
        /// </summary>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("clientId", _subscriptionClientId);
            info.AddValue("messageFilter", _messageFilter);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public virtual bool ArbiterInitialize(Arbiter arbiter)
        {
            lock (this)
            {
                if (_arbiter != null)
                {
                    SystemMonitor.Error("Arbiter value initialized more than once in a client.");
                }

                _arbiter = arbiter;
            }

            return true;
        }

        /// <summary>
        /// Override to handle an uninitializations needed.
        /// </summary>
        public virtual bool ArbiterUnInitialize()
        {
            lock (this)
            {
                _arbiter = null;
            }
            return true;
        }




    }
}
