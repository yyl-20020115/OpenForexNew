using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using Arbiter;
using System.Runtime.Serialization;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Operator manages sources (dataDelivery, order execution) in a platform.
    /// Accepts non addressed requestMessage from the arbiter for source registration purposes.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Sources")]
    [ComponentManagement(true, false, 3, false)]
    public class SourceManagementComponent : ManagementPlatformComponent
    {
        SourceContainer _sources = new SourceContainer();
        /// <summary>
        /// 
        /// </summary>
        public SourceContainer Sources
        {
            get { return _sources; }
        }

        public delegate void SourcesUpdateDelegate(SourceManagementComponent sourceOperator);
        
        [field:NonSerialized]
        public event SourcesUpdateDelegate SourcesUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        public SourceManagementComponent()
            : base(false)
        {
            this.Filter.AllowOnlyAddressedMessages = false;
            this.Filter.AllowedNonAddressedMessageTypes.Add(typeof(RegisterSourceMessage));
            this.Filter.AllowedNonAddressedMessageTypes.Add(typeof(RequestSourcesMessage));
            this.Filter.AllowedNonAddressedMessageTypes.Add(typeof(GetSourceInfoMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        public SourceManagementComponent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Filter.AllowOnlyAddressedMessages = false;
            this.Filter.AllowedNonAddressedMessageTypes.Add(typeof(RegisterSourceMessage));
            this.Filter.AllowedNonAddressedMessageTypes.Add(typeof(RequestSourcesMessage));
            this.Filter.AllowedNonAddressedMessageTypes.Add(typeof(GetSourceInfoMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected override bool OnInitialize(Platform platform)
        {
            ChangeOperationalState(OperationalStateEnum.Operational);

            return base.OnInitialize(platform);
        }

        protected override bool OnUnInitialize()
        {
            ChangeOperationalState(OperationalStateEnum.NotOperational);
            return base.OnUnInitialize();
        }

        [MessageReceiver]
        protected ResponseMessage Receive(RegisterSourceMessage message)
        {
            if (message.Register)
            {// Registration.
                if (message.SourceType.HasValue == false)
                {
                    SystemMonitor.OperationWarning("Source type not specified.");
                    if (message.RequestResponse)
                    {
                        return new ResponseMessage(false) { ResultMessage = "Source type not specified." };
                    }
                }
                
                _sources.Register(new SourceInfo(message.SourceType.Value, message.TransportInfo));
            }
            else
            {// Unregistration.
                _sources.UnRegister(message.SourceType, message.TransportInfo);
            }

            this.Send(new SourcesUpdateMessage(new List<SourceInfo>(_sources.SourcesArray), true));

            if (SourcesUpdateEvent != null)
            {
                SourcesUpdateEvent(this);
            }

            if (message.RequestResponse)
            {
                return new ResponseMessage(true);
            }

            return null;
        }

        [MessageReceiver]
        protected SourcesUpdateMessage Receive(RequestSourcesMessage message)
        {
            if (message.RequestResponse == false)
            {
                return null;
            }
            else
            {
                return new SourcesUpdateMessage(_sources.SearchSources(message.SourceType, message.PartialMatch), true);
            }
        }

        public SourceInfo? GetSourceInfo(ComponentId id)
        {
            return _sources.GetSourceById(id);
        }

        /// <summary>
        /// 
        /// </summary>
        [MessageReceiver]
        protected SourcesUpdateMessage Receive(GetSourceInfoMessage message)
        {
            return new SourcesUpdateMessage(_sources.GetSourcesById(message.Id), true);
        }
    }
}
