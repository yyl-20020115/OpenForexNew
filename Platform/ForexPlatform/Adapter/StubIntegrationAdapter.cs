using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using Arbiter;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Base class for integration adapters, allowing dataDelivery to be imported
    /// and order execution to be exported from the platform.
    /// </summary>
    [Serializable]
    public abstract class StubIntegrationAdapter : IntegrationAdapter
    {
        protected volatile DataSourceStub _dataSourceStub = null;
        protected DataSourceStub DataSourceStub
        {
            get
            {
                return _dataSourceStub;
            }
        }
        
        protected volatile OrderExecutionSourceStub _orderExecutionStub = null;
        protected OrderExecutionSourceStub OrderExecutionStub
        {
            get { return _orderExecutionStub; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public StubIntegrationAdapter()
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public StubIntegrationAdapter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            try
            {
                _dataSourceStub = (DataSourceStub)info.GetValue("dataSourceStub", typeof(DataSourceStub));
                _orderExecutionStub = (OrderExecutionSourceStub)info.GetValue("orderSourceStub", typeof(OrderExecutionSourceStub));
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError("Failed to deserialize stub integration adapter", ex);
            }
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            lock (this)
            {
                info.AddValue("dataSourceStub", _dataSourceStub);
                info.AddValue("orderSourceStub", _orderExecutionStub);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SetStub(DataSourceStub dataSourceStub)
        {
            if (_dataSourceStub != null)
            {
                return false;
            }

            _dataSourceStub = dataSourceStub;

            return true;
        }

        public bool SetStub(OrderExecutionSourceStub orderExecutionStub)
        {
            if (_orderExecutionStub != null)
            {
                return false;
            }

            _orderExecutionStub = orderExecutionStub;
            return true;
        }

        public override bool ArbiterInitialize(Arbiter.Arbiter arbiter)
        {
            bool result = base.ArbiterInitialize(arbiter);

            // Make sure to add sources as soon as possible, since there might be some requests coming in for them.
            InitializeSources();

            return result;
        }

        public override bool ArbiterUnInitialize()
        {
            InitializeSources();

            return base.ArbiterUnInitialize();
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool InitializeSources()
        {
            if (Arbiter != null && _dataSourceStub != null)
            {
                Arbiter.AddClient(_dataSourceStub);
            }

            if (Arbiter != null && _orderExecutionStub != null)
            {
                Arbiter.AddClient(_orderExecutionStub);
            }

            SystemMonitor.CheckError(Arbiter != null, "Arbiter must be assigned to start sources.");

            return true;
        }

        /// <summary>
        /// Helper, removes sources.
        /// </summary>
        /// <returns></returns>
        protected bool UnInitializeSources()
        {
            if (Arbiter != null && _dataSourceStub != null)
            {
                Arbiter.RemoveClient(_dataSourceStub);
            }

            if (Arbiter != null && _orderExecutionStub != null)
            {
                Arbiter.RemoveClient(_orderExecutionStub);
            }

            SystemMonitor.CheckWarning(Arbiter != null, "Arbiter not assigned.");

            RaisePersistenceDataUpdateEvent();
            return true;
        }

        /// <summary>
        /// Helper, start sources.
        /// </summary>
        protected void StartSources()
        {
            if (_dataSourceStub != null)
            {
                _dataSourceStub.Start();
            }

            if (_orderExecutionStub != null)
            {
                _orderExecutionStub.Start();
            }

        }

        /// <summary>
        /// Helper, stop dataDelivery and order sources.
        /// </summary>
        protected void StopSources()
        {
            if (_dataSourceStub != null)
            {
                _dataSourceStub.Stop();
            }

            if (_orderExecutionStub != null)
            {
                _orderExecutionStub.Stop();
            }
        }


        public override bool Start(Platform platform, out string operationResultMessage)
        {
            if (base.Start(platform, out operationResultMessage) == false)
            {
                StopSources();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Manager requested adapter to stop.
        /// </summary>
        public override bool Stop(out string operationResultMessage)
        {
            StopSources();

            return base.Stop(out operationResultMessage);
        }

    }
}
