using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Delegates for usage by the ISourceDataDelivery interface.
    /// </summary>
    public delegate void QuoteUpdateDelegate(ISourceDataDelivery dataDelivery, DataSessionInfo session, Quote? quote);
    public delegate void DataHistoryUpdateDelegate(ISourceDataDelivery dataDelivery, DataSessionInfo session, DataHistoryUpdate update);
    public delegate void DataDeliveryUpdateDelegate(ISourceDataDelivery dataDelivery);

    /// <summary>
    /// Defines management of delivery of dataDelivery to the dataDelivery provider.
    /// Supports streaming access to dataDelivery.
    /// </summary>
    public interface ISourceDataDelivery : IOperational, IDisposable
    {
        /// <summary>
        /// Trading quote (ask/bid) update.
        /// </summary>
        event QuoteUpdateDelegate QuoteUpdateEvent;

        /// <summary>
        /// 
        /// </summary>
        event DataHistoryUpdateDelegate DataHistoryUpdateEvent;

        /// <summary>
        /// Id of the source this delivery points to.
        /// </summary>
        ComponentId SourceId { get; }

        /// <summary>
        /// This is where time control how be exercised on a provider. 
        /// If provider does not support time control, member is null.
        /// </summary>
        ITimeControl TimeControl { get; }

        /// <summary>
        /// 
        /// </summary>
        Dictionary<Symbol, TimeSpan[]> SearchSymbols(string symbolMatchPattern);

        /// <summary>
        /// Information for the session we are delivering dataDelivery for.
        /// </summary>
        List<RuntimeDataSessionInformation> GetSymbolsRuntimeSessionInformations(Symbol[] symbols);

        /// <summary>
        /// See GetSymbolsRuntimeInformation.
        /// </summary>
        RuntimeDataSessionInformation GetSymbolRuntimeSessionInformation(Symbol symbol);

        /// <summary>
        /// 
        /// </summary>
        bool SubscribeToData(DataSessionInfo session, bool subscribe, DataSubscriptionInfo subscription);

        /// <summary>
        /// Request an update of current quote.
        /// </summary>
        /// <param name="quoteUpdateDelegate"></param>
        /// <returns></returns>
        bool RequestQuoteUpdate(DataSessionInfo sessionInfo, bool waitResult);

        /// <summary>
        /// Request a retrieval of dataDelivery history.
        /// </summary>
        /// <param name="updateDelegate">Set this to baseMethod to be called upon dataDelivery receival, or null. If null, the default event (BarDataUpdateEvent) will be raised.</param>
        bool RequestDataHistoryUpdate(DataSessionInfo sessionInfo, DataHistoryRequest request, bool waitResult);

        /// <summary>
        /// 
        /// </summary>
        bool Initialize();

        /// <summary>
        /// 
        /// </summary>
        void UnInitialize();
    }
}
