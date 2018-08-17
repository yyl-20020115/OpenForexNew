//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonFinancial;
//using CommonSupport;

//namespace ForexPlatform
//{
//    /// <summary>
//    /// Implementation using this stub inherit this interface and are queried for dataDelivery trough it, when dataDelivery neeeded.
//    /// </summary>
//    public interface DataSourceStub.IImplementation : IOperational
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        string Name { get; }

//        /// <summary>
//        /// Get symbols based on search criteria.
//        /// </summary>
//        Dictionary<Symbol, TimeSpan[]> SearchSymbols(string symbolMatch, int resultLimit);

//        /// <summary>
//        /// 
//        /// </summary>
//        RuntimeDataSessionInformation GetSymbolSessionRuntimeInformation(Symbol symbols);

//        /// <summary>
//        /// Provide an update of dataDelivery history, based on the request.
//        /// </summary>
//        DataHistoryUpdate GetDataHistoryUpdate(DataSessionInfo session, DataHistoryRequest request);

//        /// <summary>
//        /// Provide pending quote.
//        /// </summary>
//        Quote? GetQuoteUpdate(DataSessionInfo session);

//        /// <summary>
//        /// 
//        /// </summary>
//        void SessionDataSubscriptionUpdate(DataSessionInfo session, bool subscribe, DataSubscriptionInfo? info);
//    }
//}
