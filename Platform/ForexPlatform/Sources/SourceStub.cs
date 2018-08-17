using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;
using System.Runtime.Serialization;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Base class for source stubs.
    /// </summary>
    [Serializable]
    public abstract class SourceStub : OperationalTransportClient
    {
        SourceTypeEnum _sourceType;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SourceStub(string name, SourceTypeEnum sourceType)
            : base(name, false)
        {
            _sourceType = sourceType;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SourceStub(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _sourceType = (SourceTypeEnum)info.GetValue("sourceType", typeof(SourceTypeEnum));
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("sourceType", _sourceType);
        }

        /// <summary>
        /// Start operating.
        /// </summary>
        public virtual bool Start()
        {
            //ChangeOperationalState(OperationalStateEnum.Operational);
            return true;
        }

        /// <summary>
        /// Stop operating.
        /// </summary>
        public virtual void Stop()
        {
            //ChangeOperationalState(OperationalStateEnum.NotOperational);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool ArbiterInitialize(Arbiter.Arbiter arbiter)
        {
            if (base.ArbiterInitialize(arbiter) == false)
            {
                return false;
            }

            //ChangeOperationalState(OperationalStateEnum.Operational);

            RegisterSourceMessage register = new RegisterSourceMessage(_sourceType, true);
            register.RequestResponse = false;
            this.Send(register);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool ArbiterUnInitialize()
        {
            // Unregister.
            RegisterSourceMessage register = new RegisterSourceMessage(false);
            register.RequestResponse = false;
            this.Send(register);

            //ChangeOperationalState(OperationalStateEnum.UnInitialized);
            return base.ArbiterUnInitialize();
        }


    }
}
