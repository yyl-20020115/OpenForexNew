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
    public abstract class IntegrationAdapter : OperationalTransportClient, IIntegrationAdapter
    {
        /// <summary>
        /// What is the advised precision for account values calculations (profit, margin etc.)
        /// </summary>
        public const int AdvisedAccountDecimalsPrecision = 2;

        volatile bool _isStarted = false;
        /// <summary>
        /// The adapter manager controls starting and stopping of adapters.
        /// </summary>
        public bool IsStarted
        {
            get { return _isStarted; }
        }

        volatile Platform _platform = null;
        /// <summary>
        /// Platform instance *only available* while started.
        /// </summary>
        protected Platform Platform
        {
            get { return _platform; }
        }

        public event IntegrationAdapterUpdateDelegate PersistenceDataUpdateEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public IntegrationAdapter()
            : base("Integration Adapter", false)
        {
            this.Name = UserFriendlyNameAttribute.GetTypeAttributeName(this.GetType());

            base.DefaultTimeOut = TimeSpan.FromSeconds(20);
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public IntegrationAdapter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected void RaisePersistenceDataUpdateEvent()
        {
            if (PersistenceDataUpdateEvent != null)
            {
                PersistenceDataUpdateEvent(this);
            }
        }

        /// <summary>
        /// Manager requires adapter to start.
        /// </summary>
        public virtual bool Start(Platform platform, out string operationResultMessage)
        {
            if (IsStarted)
            {
                operationResultMessage = "Adapter already started.";
                return false;
            }

            _platform = platform;
            _isStarted = true;
            if (OnStart(out operationResultMessage))
            {
                return true;
            }

            _isStarted = false;
            return false;
        }

        /// <summary>
        /// Called when the adapter is called to start.
        /// Try not to put blocking calls here, since this may be executed on the UI thread.
        /// </summary>
        protected abstract bool OnStart(out string operationResultMessage);

        /// <summary>
        /// Manager requested adapter to stop.
        /// </summary>
        public virtual bool Stop(out string operationResultMessage)
        {
            if (_isStarted)
            {
                _isStarted = false;
                _platform = null;

                bool result = OnStop(out operationResultMessage);
                //UnInitializeSources();
                return result;
            }
            else
            {
                operationResultMessage = "Adapter not started.";
                return false;
            }
        }

        /// <summary>
        /// Child classes override this to specify OnStop behaviour.
        /// </summary>
        protected abstract bool OnStop(out string operationResultMessage);

        //#region Arbiter Messages Sent To Subscribers/SourceContainer

        //protected virtual void SendUpdateResponce(OperationResponceMessage requestMessage, bool toDataSource, bool toOrderSource)
        //{
        //    // Make sure initialization has already passed successfully.
        //    if (_initializationEvent.WaitOne(DefaultTimeOut, true) == false)
        //    {// Time out.
        //        SystemMonitor.OperationError("Time out occured.", SystemMonitor.TracerItem.PriorityEnum.Medium);
        //        return;
        //    }

        //    ArbiterClientId? dataSourceId = _dataSourceId;
        //    ArbiterClientId? orderExecutionSourceId = _orderExecutionSourceId;

        //    if (toDataSource && dataSourceId.HasValue)
        //    {
        //        if (ParentOperator.Platform != null &&
        //            ParentOperator.Platform.GetComponentOperationalState(dataSourceId.Value.Id) != OperationalStateEnum.Operational)
        //        {
        //            return;
        //        }

        //        if (requestMessage.OperationId == -1)
        //        {// Send as a general update for everyone.
        //            this.SendAddressed(dataSourceId.Value, requestMessage);
        //        }
        //        else
        //        {// Send as a respond to whoever requested it.
        //            SystemMonitor.NotImplementedCritical();
        //            //Receive(requestMessage);
        //        }
        //    }

        //    if (toOrderSource && orderExecutionSourceId.HasValue)
        //    {
        //        if (ParentOperator.Platform != null &&
        //            ParentOperator.Platform.GetComponentOperationalState(orderExecutionSourceId.Value.Id) != OperationalStateEnum.Operational)
        //        {
        //            return;
        //        }

        //        if (requestMessage.OperationId == -1)
        //        {// Send as a general update for everyone.
        //            this.SendAddressed(orderExecutionSourceId.Value, requestMessage);
        //        }
        //        else
        //        {// Send as a respond to whoever requested it.
        //            SystemMonitor.NotImplementedCritical();
        //            //Receive(requestMessage);
        //        }
        //    }
        //}

    }
}
