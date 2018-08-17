using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using CommonSupport;

namespace CommonFinancial
{
    /// <summary>
    /// Delegate for order updates.
    /// </summary>
    public delegate void OrdersUpdateDelegate(IOrderSink provider, AccountInfo accountInfo, string[] previousOrderIds, OrderInfo[] orderInfos, Order.UpdateTypeEnum[] updateType);
    
    /// <summary>
    /// Delegate for update of positions.
    /// </summary>
    public delegate void PositionUpdateDelegate(IOrderSink provider, AccountInfo accountInfo, PositionInfo[] positionInfo);
    
    /// <summary>
    /// Delegate for update of account information.
    /// </summary>
    public delegate void AccountInfoUpdateDelegate(IOrderSink provider, AccountInfo accountInfo);

    /// <summary>
    /// Interface defines how a class that handles order placement and management should look like.
    /// It is source based, and supports multiple sessions.
    /// </summary>
    public interface IOrderSink : IOperational
    {
        /// <summary>
        /// Order was updated, see UpdateType for type of update event.
        /// </summary>
        event OrdersUpdateDelegate OrdersUpdatedEvent;

        /// <summary>
        /// Position was updated (if executor has positions)
        /// </summary>
        event PositionUpdateDelegate PositionsUpdateEvent;

        /// <summary>
        /// CompletionEvent occurs when there is an update in account information, received from source.
        /// </summary>
        event AccountInfoUpdateDelegate AccountInfoUpdateEvent;

        /// <summary>
        /// Does the executor allow to manage active live orders or not.
        /// If a source is considered to not support per order management, position management is considered.
        /// </summary>
        bool SupportsActiveOrderManagement
        {
            get;
        }

        /// <summary>
        /// Some brokers tread slippage different to others (and there is no way to tell)
        /// so we have the option to modify slippage with this multiplicator upon sending,
        /// where needed.
        /// </summary>
        Decimal SlippageMultiplicator
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        bool Initialize();

        /// <summary>
        /// UnInitialize provider.
        /// </summary>
        void UnInitialize();

        /// <summary>
        /// Obtain latest account information from source.
        /// </summary>
        /// <returns></returns>
        bool BeginAccountInfoUpdate(AccountInfo accountInfo);

        /// <summary>
        /// Get an updated version of all account infos.
        /// </summary>
        bool GetAvailableAccountInfos(out AccountInfo[] accounts);

        /// <summary>
        /// 
        /// </summary>
        bool GetOrdersInformation(AccountInfo accountInfo, string[] orderIds,
            out OrderInfo[] informations, out string operationResultMessage);

        /// <summary>
        /// 
        /// </summary>
        bool BeginOrdersInformationUpdate(AccountInfo accountInfo, string[] orderIds, 
            out string operationResultMessage);

        #region Order Management

        /// <summary>
        /// 
        /// </summary>
        string SubmitOrder(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType, int volume, Decimal? allowedSlippage, Decimal? desiredPrice,
            Decimal? takeProfit, Decimal? stopLoss, string comment, out string operationResultMessage);

        /// <summary>
        /// Allows opening of orders for immediate or delayed execution.
        /// This may execute (open) the order synchronously, if the execution provider supports it
        /// and if the order is immediate, not delayed order.
        /// </summary>
        /// <param name="resultingOrderState">Result indicates the state that the order should be considered, once leaving this baseMethod. Some orders can be executed immediately, while orders only get placed (delayed orders).</param>
        /// <param name="operationTimeOut">What is the maximum operation timed allowed.</param>
        /// <returns>Returns true if order is placed OK.</returns>
        bool SynchronousExecute(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType, int volume, Decimal? allowedSlippage, Decimal? desiredPrice,
            Decimal? takeProfit, Decimal? stopLoss, string comment, TimeSpan operationTimeOut, out OrderInfo? info, out string operationResultMessage);

        /// <summary>
        /// SynchronousExecute with no timeOut parameter.
        /// </summary>
        bool SynchronousExecute(AccountInfo account, Order order, Symbol symbol, OrderTypeEnum orderType, int volume, Decimal? allowedSlippage, Decimal? desiredPrice,
            Decimal? takeProfit, Decimal? stopLoss, string comment, out OrderInfo? info, out string operationResultMessage);

        /// <summary>
        /// Change order execution related parameters.
        /// </summary>
        /// <param name="stopLoss">Applicable to open or pending orders only, pass null to skip, Decimal.MinValue to signify no value.</param>
        /// <param name="takeProfit">Applicable to open or pending orders only, pass null to skip, Decimal.MinValue to signify no value.</param>
        /// <param name="targetOpenPrice">Applicable to pending and opened orders only (pass null to skip)</param>
        /// <returns>Returns true if the operation was successful and order was modified.</returns>
        bool ModifyOrder(AccountInfo account, Order order, Decimal? stopLoss, Decimal? takeProfit, Decimal? targetOpenPrice,
            out string modifiedId, out string operationResultMessage);

        /// <summary>
        /// Decrease already existing order closeVolume. This is in effect a partial close of the order.
        /// </summary>
        bool DecreaseOrderVolume(AccountInfo account, Order order, Decimal volumeDecreasal, Decimal? allowedSlippage,
            Decimal? desiredPrice, out Decimal decreasalPrice, out string modifiedId, out string operationResultMessage);

        /// <summary>
        /// Increase of closeVolume allowed on pending orders exclusively.
        /// </summary>
        bool IncreaseOrderVolume(AccountInfo account, Order order, Decimal volumeIncrease, Decimal? allowedSlippage,
            Decimal? desiredPrice, out Decimal increasalPrice, out string modifiedId, out string operationResultMessage);

        /// <summary>
        /// Cancel pending order.
        /// </summary>
        bool CancelPendingOrder(AccountInfo account, Order order, out string modifiedId, out string operationResultMessage);

        /// <summary>
        /// Close existing opened order.
        /// </summary>
        bool CloseOrder(AccountInfo account, Order order, Decimal? allowedSlippage, Decimal? desiredPrice, out Decimal closingPrice,
            out DateTime closingTime, out string modifiedId, out string operationResultMessage);

        #endregion
    }
}
