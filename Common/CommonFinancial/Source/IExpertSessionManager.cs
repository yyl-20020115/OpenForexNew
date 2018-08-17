using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Interface defines the major functionalities an expert manager needs to provide
    /// (managing the sources and expert sessions).
    /// </summary>
    public interface ISourceAndExpertSessionManager : ISourceManager
    {
        /// <summary>
        /// All sessions currently in the manager.
        /// </summary>
        ExpertSession[] SessionsArray { get; }

        /// <summary>
        /// All sessions currently in the manager.
        /// </summary>
        int SessionCount { get; }

        #region Events

        event GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager> SessionsUpdateEvent;

        event GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager, ExpertSession> SessionCreatedEvent;
        event GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager, ExpertSession> SessionDestroyedEvent;
        event GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager, ExpertSession> SessionDestroyingEvent;

        #endregion

        /// <summary>
        /// Create a session by coupling dataDelivery and order execution provider.
        /// </summary>
        /// <param name="dataProvider">Mandatory, must be a valid dataDelivery provider.</param>
        /// <param name="orderExecutionProvider">Can be null, to specify dataDelivery only session, or a valid order execution provider.</param>
        /// <returns></returns>
        bool RegisterExpertSession(ExpertSession session);

        /// <summary>
        /// Destroy a session.
        /// </summary>
        /// <param name="session"></param>
        void UnRegisterExpertSession(ExpertSession session);

        /// <summary>
        /// Obtain a session instance by its Info.
        /// </summary>
        ExpertSession GetExpertSession(DataSessionInfo info);

        /// <summary>
        /// Obtain a session based on a symbol and source id of the data source.
        /// </summary>
        ExpertSession GetExpertSession(ComponentId source, Symbol symbol);

        /// <summary>
        /// Obtain a session based on a symbol and source id of the execution source.
        /// </summary>
        ExpertSession GetExpertSessionByExecutionSource(ComponentId executionSourceId, Symbol symbol);
    }
}
