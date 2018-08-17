using System;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Interface specifies what properties and methods an ActiveOrder Execution OrderExecutionProvider should provide.
    /// This allows the upper layers to access order execution functionality, disregarding order
    /// execution specifics.
    /// </summary>
    public interface ISourceOrderExecution : IOrderSink, IDisposable
    {
        /// <summary>
        /// ActiveOrder execution account instance for this provider.
        /// The account that handles the orders of the provider.
        /// </summary>
        Account[] Accounts { get; }

        /// <summary>
        /// New. Default account used. To be replaced when full support for multiple accounts on a source is implemented.
        /// </summary>
        Account DefaultAccount { get; }

        ///// <summary>
        ///// Is the execution is currently processing some other operation.
        ///// </summary>
        //bool IsBusy
        //{
        //    get;
        //}

        /// <summary>
        /// 
        /// </summary>
        ITradeEntityManagement TradeEntities { get; }

        /// <summary>
        /// 
        /// </summary>
        ComponentId SourceId { get; }

        /// <summary>
        /// This is where time control how be exercised on a provider. 
        /// If provider does not support time control, member is null.
        /// </summary>
        ITimeControl TimeControl { get; }
    }
}
