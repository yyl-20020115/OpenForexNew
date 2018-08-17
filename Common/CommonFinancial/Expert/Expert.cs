using System;
using Arbiter;
using System.Collections.Generic;
using CommonSupport;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// The base class for all Experts in the platform. An Expert is typically the main unit making the trading decisions.
    /// An expert is the piece of code that has all the information for taking a decision on a trade. There are however some small
    /// exceptions, like a manual trading expert - this requires user input, or an integration expert - this may export the needed 
    /// decision making information to some external module or class.
    /// </summary>
    [Serializable]
    public abstract class Expert : Operational, IDisposable
    {
        protected volatile string _name;
        public string Name
        {
            get { return _name; }
        }

        volatile ISourceAndExpertSessionManager _manager;
        public ISourceAndExpertSessionManager Manager
        {
            get { return _manager; }
        }

        public delegate void ExpertUpdateDelegate(Expert expert);

        [field: NonSerialized]
        public event ExpertUpdateDelegate PersistenceDataUpdateEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Expert(ISourceAndExpertSessionManager sessionManager, string name)
        {
            _manager = sessionManager;
            _name = name;
        }

        #region IDeserializationCallback Members

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            _operationalState = OperationalStateEnum.NotOperational;
        }

        #endregion

        /// <summary>
        /// Allows the children implementation to specify specific operation persistence moments.
        /// </summary>
        protected void RaisePersistenceDataUpdateEvent()
        {
            if (PersistenceDataUpdateEvent != null)
            {
                PersistenceDataUpdateEvent(this);
            }
        }


        /// <summary>
        /// A single instance must be able to handle multiple Initialize-Uninitialize cycles.
        /// And also must be able to operate a Initialize-Uninitialize-SaveState-Deserialize-Initialize etc. cycles.
        /// </summary>
        public bool Initialize()
        {
            if (OnInitialize())
            {
                ChangeOperationalState(OperationalStateEnum.Operational);
                return true;
            }

            return false;
        }

        /// <summary>
        /// A single instance must be able to handle multiple Initialize-Uninitialize cycles.
        /// </summary>
        public bool UnInitialize()
        {
            if (OnUnInitialize())
            {
                ChangeOperationalState(OperationalStateEnum.UnInitialized);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ChangeOperationalState(OperationalStateEnum.Disposed);
            _manager = null;
            _name = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool OnInitialize()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool OnUnInitialize()
        {
            return true;
        }


    }
}
