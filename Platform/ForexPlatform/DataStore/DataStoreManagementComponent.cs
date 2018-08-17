using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CommonFinancial;
using CommonSupport;
using ForexPlatformPersistence;

namespace ForexPlatform
{
    /// <summary>
    /// Component manages stored historical quotes dataDelivery.
    /// Since a Platform has only one dataDelivery store (and it is always operating), 
    /// this component here only uses the Platform manager, it does not create or
    /// modify it.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Data Store")]
    [ComponentManagement(true, false, 5, false)]
    public class DataStoreManagementComponent : ManagementPlatformComponent
    {
        /// <summary>
        /// The actual dataDelivery storage manager.
        /// </summary>
        public DataStore DataStore
        {
            get
            {
                if (OperationalState == OperationalStateEnum.Operational && Platform != null)
                {
                    return DataStore.Instance;
                }

                return null;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataStoreManagementComponent() : base(false)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="context"></param>
        public DataStoreManagementComponent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Serialization.
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected override bool OnInitialize(Platform platform)
        {
            base.OnInitialize(platform);
            ChangeOperationalState(OperationalStateEnum.Operational);

            DataStore.Instance.Initialize(platform.Settings);
            return true;
        }


        protected override bool OnUnInitialize()
        {
            ChangeOperationalState(OperationalStateEnum.NotOperational);
            
            DataStore.Instance.UnInitialize();
            return base.OnUnInitialize();
        }



    }
}
