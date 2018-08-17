using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CommonSupport;
using ForexPlatformPersistence;

namespace ForexPlatform
{
    /// <summary>
    /// Platform news component.
    /// The component provides news apprehention, sorting, searching etc. Makes extensive use
    /// of support classes (both UI and logics).
    /// </summary>
    [Serializable]
    [UserFriendlyName("News Component")]
    [ComponentManagement(false, false, 30, false)]
    public class PlatformNews : PlatformComponent
    {
        /// <summary>
        /// Persisted.
        /// </summary>
        volatile PlatformNewsManager _newsManager;
        public PlatformNewsManager NewsManager
        {
            get { return _newsManager; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PlatformNews()
            : base(true)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public PlatformNews(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _newsManager = (PlatformNewsManager)info.GetValue("platformNewsManager", typeof(PlatformNewsManager));
        }

        /// <summary>
        /// Serialization.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("platformNewsManager", _newsManager);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            ChangeOperationalState(OperationalStateEnum.Disposed);

            _newsManager.Dispose();
            _newsManager = null;
        }

        protected override bool OnInitialize(Platform platform)
        {
            if (base.OnInitialize(platform) == false)
            {
                return false;
            }

            if (_newsManager == null)
            {// Only executed after NO serialization has been done and this is a first component run.
                _newsManager = new PlatformNewsManager();
            }

            GeneralHelper.FireAndForget(delegate()
            {
                _newsManager.Initialize(platform);
            });

            ChangeOperationalState(OperationalStateEnum.Operational);
            return true;
        }

        protected override bool OnUnInitialize()
        {
            bool result = base.OnUnInitialize();

            _newsManager.UnInitialize();
            
            ChangeOperationalState(OperationalStateEnum.UnInitialized);

            return result;
        }

    }
}
