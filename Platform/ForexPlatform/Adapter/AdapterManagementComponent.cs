using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;
using ForexPlatform;
using CommonFinancial;
using System.Threading;
using System.Configuration;
using System.Runtime.Serialization;


namespace ForexPlatform
{
    /// <summary>
    /// Operator manages the creation/destruction and control of integration adapters.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Adapters")]
    [ComponentManagement(false, false, 10, false)]
    public class AdapterManagementComponent : ManagementPlatformComponent
    {// Low level component since others are likely to depend on it.
        List<ComponentId> _startedAdaptersIds = new List<ComponentId>();

        GenericContainer<IIntegrationAdapter> _adapters = new GenericContainer<IIntegrationAdapter>();
        
        /// <summary>
        /// Stores adapters instances.
        /// </summary>
        public GenericContainer<IIntegrationAdapter> Adapters
        {
            get { return _adapters; }
        }

        /// <summary>
        /// 
        /// </summary>
        public AdapterManagementComponent()
            : base(false)
        {
            Name = UserFriendlyNameAttribute.GetTypeAttributeName(typeof(AdapterManagementComponent));
            base.DefaultTimeOut = TimeSpan.FromSeconds(15);

            ChangeOperationalState(OperationalStateEnum.Constructed);

            _adapters.ItemAddedEvent += new GenericContainer<IIntegrationAdapter>.ItemUpdateDelegate(Adapters_ItemAddedEvent);
            _adapters.ItemRemovedEvent += new GenericContainer<IIntegrationAdapter>.ItemUpdateDelegate(Adapters_ItemRemovedEvent);
        }

        /// <summary>
        /// Deserializing contructor.
        /// </summary>
        public AdapterManagementComponent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _adapters = new GenericContainer<IIntegrationAdapter>();

            _adapters.ItemAddedEvent += new GenericContainer<IIntegrationAdapter>.ItemUpdateDelegate(Adapters_ItemAddedEvent);
            _adapters.ItemRemovedEvent += new GenericContainer<IIntegrationAdapter>.ItemUpdateDelegate(Adapters_ItemRemovedEvent);

            _startedAdaptersIds = (List<ComponentId>)info.GetValue("startedAdaptersIds", _startedAdaptersIds.GetType());
            
            // Adapters are serialized one by one, to add safety when deserializing (one adapter will 
            // otherwise block the entire deserialization for the manager).
            int adaptersCount = (int)info.GetValue("adapterCount", typeof(int));
            for (int i = 0; i < adaptersCount; i++)
            {
                try
                {
                    IIntegrationAdapter adapter = (IIntegrationAdapter)info.GetValue("adapter_" + i, typeof(IIntegrationAdapter));
                    _adapters.Add(adapter);
                }
                catch (SerializationException ex)
                {
                    SystemMonitor.OperationError("Failed to deserialize integration adapter [" + ex.Message + "].");
                }
                catch (InvalidCastException ex)
                {
                    SystemMonitor.OperationError("Failed to deserialize integration adapter [" + ex.Message + "].");
                }
            }

            ChangeOperationalState(OperationalStateEnum.Constructed);
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            lock (this)
            {
                info.AddValue("startedAdaptersIds", _startedAdaptersIds);

                IIntegrationAdapter[] adapters = _adapters.ToArray();
                info.AddValue("adapterCount", adapters.Length);
                for (int i = 0; i < adapters.Length; i++)
                {
                    info.AddValue("adapter_" + i, adapters[i]);
                }
            }
        }

        void Adapters_ItemAddedEvent(GenericContainer<IIntegrationAdapter> keeper, IIntegrationAdapter adapter)
        {
            adapter.PersistenceDataUpdateEvent += new IntegrationAdapterUpdateDelegate(adapter_PersistenceDataUpdateEvent);
            if (Arbiter != null)
            {
                Arbiter.AddClient(adapter);
            }

            if (OperationalState != OperationalStateEnum.Unknown)
            {
                RaisePersistenceDataUpdatedEvent();
            }
        }

        void Adapters_ItemRemovedEvent(GenericContainer<IIntegrationAdapter> keeper, IIntegrationAdapter adapter)
        {
            string operationResultMessage;
            adapter.Stop(out operationResultMessage);
            if (Arbiter != null)
            {
                Arbiter.RemoveClient(adapter);
            }
            adapter.PersistenceDataUpdateEvent -= new IntegrationAdapterUpdateDelegate(adapter_PersistenceDataUpdateEvent);
            adapter.Dispose();

            if (OperationalState != OperationalStateEnum.Unknown)
            {
                RaisePersistenceDataUpdatedEvent();
            }
        }

        void adapter_PersistenceDataUpdateEvent(IIntegrationAdapter adapter)
        {
            if (OperationalState != OperationalStateEnum.Unknown)
            {
                RaisePersistenceDataUpdatedEvent();
            }
        }

        /// <summary>
        /// Disposing the object and all related dataDelivery.
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            Adapters.Clear(true);
        }

        /// <summary>
        /// Helper, retrieves adapters from the provided type.
        /// </summary>
        public List<AdapterType> GetAdaptersByType<AdapterType>()
            where AdapterType : class/*, IIntegrationAdapter*/
        {
            List<AdapterType> result = new List<AdapterType>();

            foreach (IIntegrationAdapter adapter in Adapters.ToArray())
            {
                if (adapter is AdapterType)
                {
                    result.Add(adapter as AdapterType);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        protected override bool OnInitialize(Platform platform)
        {
            if (base.OnInitialize(platform) == false)
            {
                return false;
            }

            ChangeOperationalState(OperationalStateEnum.Operational);
            // Make sure to add them instantly since initial subscriptions and requests may start very soon.
            // We can not wait for the fire and forget to perform for that.
            foreach (IIntegrationAdapter adapter in Adapters.ToArray())
            {
                Arbiter.AddClient(adapter);
            }

            GeneralHelper.FireAndForget(delegate()
            {// Begin starting the adapters, once they are added to the Arbiter and ready to begin operation.
                foreach (IIntegrationAdapter adapter in Adapters.ToArray())
                {
                    bool mustStart;
                    lock (this)
                    {
                        mustStart = _startedAdaptersIds.Contains(adapter.SubscriptionClientID.Id);
                    }

                    if (mustStart)
                    {
                        string operationResultMessage;
                        adapter.Start(this.Platform, out operationResultMessage);
                    }
                }

                lock (this)
                {
                    _startedAdaptersIds.Clear();
                }
            });

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool OnUnInitialize()
        {
            lock (this)
            {
                _startedAdaptersIds.Clear();
            }

            foreach (IIntegrationAdapter adapter in Adapters.ToArray())
            {
                string operationResultMessage;
                if (adapter.IsStarted)
                {
                    lock (this)
                    {
                        _startedAdaptersIds.Add(adapter.SubscriptionClientID.Id);
                    }
                }

                adapter.Stop(out operationResultMessage);
                Arbiter.RemoveClient(adapter);
            }

            ChangeOperationalState(OperationalStateEnum.UnInitialized);
            return base.OnUnInitialize();
        }
    }
}
