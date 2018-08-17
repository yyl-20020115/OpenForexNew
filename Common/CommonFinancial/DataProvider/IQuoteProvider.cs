using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Delegate for providing events of type value update. Above the standard orderInfo also shows orderInfo
    /// of how many total steps the current update is taking (in cases many steps are done in subsequence one after the other).
    /// </summary>
    /// <param name="stepsRemaining">Multi step mode occurs when many steps are done one after the other in fast succession. Usefull for UI not to update itself.</param>
    public delegate void QuoteProviderUpdateDelegate(IQuoteProvider provider);

    /// <summary>
    /// The most basic dataDelivery provision interface. Provides only current trading dataDelivery quotations.
    /// </summary>
    public interface IQuoteProvider : IOperational
    {
        /// <summary>
        /// 
        /// </summary>
        decimal? Ask { get; }

        /// <summary>
        /// 
        /// </summary>
        decimal? Bid { get; }

        /// <summary>
        /// 
        /// </summary>
        decimal? Spread { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime? Time { get; }

        /// <summary>
        /// 
        /// </summary>
        Quote? CurrentQuote { get; }

        /// <summary>
        /// The last time a quote was received (local time).
        /// </summary>
        DateTime? LastQuoteTime { get; }

        /// <summary>
        /// Update type, updated items count.
        /// </summary>
        event QuoteProviderUpdateDelegate QuoteUpdateEvent;

        /// <summary>
        /// Request update of values.
        /// </summary>
        bool RequestQuoteUpdate(bool waitResult);

        /// <summary>
        /// Helper, helps establish the current opening price for the provided order type.
        /// </summary>
        decimal? GetOrderOpenQuote(OrderTypeEnum orderType);

        /// <summary>
        /// Helper.
        /// </summary>
        decimal? GetOrderCloseQuote(OrderTypeEnum orderType);

        /// <summary>
        /// Helper.
        /// </summary>
        decimal? GetOrderOpenQuote(bool isBuy);

        /// <summary>
        /// Helper
        /// </summary>
        decimal? GetOrderCloseQuote(bool isBuy);

    }
}
