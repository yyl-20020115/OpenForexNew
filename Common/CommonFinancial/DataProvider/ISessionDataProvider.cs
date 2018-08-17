using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonSupport;
using CommonFinancial;

namespace CommonFinancial
{
    /// <summary>
    /// Delegates for the events in the ISessionDataProvider class.
    /// </summary>
    /// <param name="dataProvider"></param>
    public delegate void DataProviderUpdateDelegate(ISessionDataProvider dataProvider);
    public delegate void DataProviderBarProviderUpdateDelegate(ISessionDataProvider dataProvider, IDataBarHistoryProvider provider);

    /// <summary>
    /// Interface specifies how a session dataDelivery provider should look like. It may support bar or tick dataDelivery history, 
    /// or none, only providing quotations.
    /// Also in some cases it may not have any QuoteProvider, when it provides access to static historic dataDelivery.
    /// </summary>
    public interface ISessionDataProvider : IDisposable, IOperational
    {
        /// <summary>
        /// Name of this provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Provides access to the current price information (may be null if only historical information is available).
        /// </summary>
        IQuoteProvider Quotes { get; }
        
        /// <summary>
        /// Provides access to quote history (if available). May be null (in cases there is no dataDelivery history).
        /// </summary>
        IDataTickHistoryProvider DataTicks { get; }

        /// <summary>
        /// The *currently selected* dataDelivery bars provider.
        /// The instance behind this can change with calls to SetCurrentDataBarsProvider() etc.
        /// May be null.
        /// Provides access to quote history (if available). May be null (in cases there is no dataDelivery history).
        /// </summary>
        IDataBarHistoryProvider DataBars { get; }

        /// <summary>
        /// This is where time control how be exercised on a provider. 
        /// If provider does not support time control, member is null.
        /// </summary>
        ITimeControl TimeControl { get; }

        /// <summary>
        /// Session orderInfo for the session related to this dataDelivery provider.
        /// </summary>
        DataSessionInfo SessionInfo { get; }

        /// <summary>
        /// 
        /// </summary>
        ComponentId SourceId { get; }

        // <summary>
        // Session orderInfo for the session related to this dataDelivery provider.
        // </summary>
        RuntimeDataSessionInformation RuntimeSessionInformation { get; }


        /// <summary>
        /// Pass null to ask the dataDelivery provider to reuse its previously used session.
        /// </summary>
        bool Initialize(DataSessionInfo? sessionInfo);

        /// <summary>
        /// 
        /// </summary>
        void UnInitialize();

        #region Data Bar OrderExecutionProvider Management

        /// <summary>
        /// The currently selected dataDelivery bar provider has been changed.
        /// </summary>
        event DataProviderUpdateDelegate CurrentDataBarProviderChangedEvent;

        /// <summary>
        /// 
        /// </summary>
        event DataProviderBarProviderUpdateDelegate DataBarProviderCreatedEvent;
        
        /// <summary>
        /// 
        /// </summary>
        event DataProviderBarProviderUpdateDelegate DataBarProviderDestroyedEvent;

        /// <summary>
        /// 
        /// </summary>
        TimeSpan[] AvailableDataBarProviderPeriods { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period">Pass null for none.</param>
        /// <returns></returns>
        bool SetCurrentDataBarProvider(TimeSpan? period);

        /// <summary>
        /// All of the existing dataDelivery bars sets.
        /// </summary>
        IDataBarHistoryProvider GetDataBarProvider(TimeSpan period);

        /// <summary>
        /// Create a new provider for given period, if available.
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        bool ObtainDataBarProvider(TimeSpan period);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        bool ReleaseDataBarProvider(TimeSpan period);

        #endregion
    }
}
