using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Provides flags that specify the capabilities of a given source.
    /// </summary>
    public enum SourceTypeEnum
    {
        None = 0,
        BackTesting = 1, // Local
        Live = 2, // Remote
        DataProvider = 4,
        OrderExecution = 8,
        HighPriority = 16, // This flag is assigned to live sources, with high informational value 
      HighPriorityLiveFullProvider = DataProvider | OrderExecution | Live | HighPriority
    }

    /// <summary>
    /// Specify the interface to managing expert sessions. Use by the expert to create
    /// destroy etc. trading sessions (remote or local).
    /// </summary>
    public interface ISourceManager
    {
        ///// <summary>
        ///// Available Data Sources on the platform.
        ///// </summary>
        //List<ComponentId> DataSourcesIDs { get; }

        ///// <summary>
        ///// Available Execution Source on the platform.
        ///// </summary>
        //List<ComponentId> OrderExecutionSourcesIDs { get; }

        /// <summary>
        /// 
        /// </summary>
        ISourceOrderExecution ObtainOrderExecutionProvider(ComponentId sourceId, ComponentId dataSourceId);

        /// <summary>
        /// New.
        /// </summary>
        ISourceOrderExecution GetOrderExecutionProvider(ComponentId source);

        /// <summary>
        /// Will provide existing quotation provider, or create a new one.
        /// </summary>
        IQuoteProvider ObtainQuoteProvider(ComponentId source, Symbol symbol);

        /// <summary>
        /// 
        /// </summary>
        IDataBarHistoryProvider ObtainDataBarHistoryProvider(ComponentId source, Symbol symbol, TimeSpan period);

        /// <summary>
        /// Will provide existing tick history provider, or create a new one.
        /// </summary>
        IDataTickHistoryProvider ObtainDataTickHistoryProvider(ComponentId source, Symbol symbol);

        /// <summary>
        /// 
        /// </summary>
        ISourceDataDelivery GetDataDelivery(ComponentId source);

        /// <summary>
        /// Will retrieve or create a new one.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        ISourceDataDelivery ObtainDataDelivery(ComponentId source);

        /// <summary>
        /// 
        /// </summary>
        SortedDictionary<int, List<ComponentId>> GetCompatibleOrderExecutionSources(ComponentId dataSourceId, Symbol symbol, SourceTypeEnum sourceType);

        /// <summary>
        /// 
        /// </summary>
        Dictionary<Symbol, TimeSpan[]> SearchSymbols(ComponentId source, string symbolMatch);

        /// <summary>
        /// 
        /// </summary>
        DataSessionInfo? GetSymbolDataSessionInfo(ComponentId id, Symbol symbol);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filteringSourceType"></param>
        /// <param name="partialMatch"></param>
        /// <returns></returns>
        List<ComponentId> GetSources(SourceTypeEnum filteringSourceType, bool partialMatch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        SourceTypeEnum? GetSourceTypeFlags(ComponentId sourceId, SourceTypeEnum? filter);

    }
}
