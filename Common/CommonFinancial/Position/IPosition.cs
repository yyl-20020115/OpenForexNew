using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public delegate void UpdateDelegate(IPosition position);
    public delegate void OperationUpdateDelegate(IPosition position, string orderId);

    /// <summary>
    /// Base interface for trading position.
    /// </summary>
    public interface IPosition : IDisposable
    {
        Symbol Symbol
        {
            get;
        }

        decimal Volume
        {
            get;
        }

        decimal? BasePrice
        {
            get;
        }

        decimal Result
        {
            get;
        }

        Tracer Tracer
        {
            get;
            set;
        }

        #region Events

        event UpdateDelegate UpdateEvent;

        #endregion

        #region Construction and Instance Control

        bool Initialize();

        void UnInitialize();

        #endregion

        #region OrderBasedPosition Management

        ///// <summary>
        ///// Full submit of orders with a full set of parameters.
        ///// </summary>
        ///// <returns>If success, returns the ID of the newly placed order.</returns>
        //string Submit(OrderTypeEnum orderType, int minVolume, decimal? price, decimal? slippage,
        //    decimal? takeProfit, decimal? stopLoss, out string operationResultMessage);

        ///// <summary>
        ///// Submit a request for a market close partial or full closeVolume of the current position.
        ///// </summary>
        //string SubmitClose(int? minVolume, out string operationResultMessage);

        ///// <summary>
        ///// Open orders in position management
        ///// </summary>
        //bool CancelPendingOrder(string openOrderId, out string operationResultMessage);

        #endregion
    }
}
