using System;
using CommonSupport;
using FXCore;
using CommonFinancial;
using System.Collections.Generic;
using System.Threading;
using ForexPlatform;

namespace FXCMAdapter
{
    /// <summary>
    /// Class constrols integration to the Fxcm Order2Go API.
    /// 
    /// Works in a separate thread, since COM EVENTS coming from the API
    /// require a requestMessage loop and the same thread to access them, it takes
    /// care of all this.
    /// </summary>
    public class FXCMConnectionManager : Operational, IDisposable
    {
        #region Member variables

        BackgroundMessageLoopOperator _messageLoopOperator;

        /// <summary>
        /// Try to access this only from the internal thread.
        /// </summary>
        volatile CoreAut _core;
        volatile TradeDeskAut _desk;
        volatile TradeDeskEventsSinkClass _tradeDeskEventsSink;

        public bool LoggedIn
        {
            get 
            {
                bool result = false;
                BackgroundMessageLoopOperator messageLoopOperator = _messageLoopOperator;
                if (messageLoopOperator != null)
                {
                    messageLoopOperator.Invoke(delegate() { result = _desk.IsLoggedIn(); }, TimeSpan.FromSeconds(15));
                }

                return result;
            }
        }

        int _subscriptionId = -1;

		string _serviceUrl = String.Empty;

        #endregion

        public delegate void ItemUpdateDelegate(FXCMConnectionManager manager, string accountId);

        /// <summary>
        /// Raised on a thread pool call.
        /// </summary>
        public event ItemUpdateDelegate AccountUpdatedEvent;

        public delegate void OrderUpdateDelegate(FXCMConnectionManager manager, string accountId, OrderInfo orderInfo);
        /// <summary>
        /// Raised on a thread pool call.
        /// </summary>
        public event OrderUpdateDelegate OrderUpdatedEvent;

        public delegate void QuoteUpdateDelegate(FXCMConnectionManager manager, string symbol, DataTick dataTick);
        /// <summary>
        /// Raised on a thread pool call.
        /// </summary>
        public event QuoteUpdateDelegate QuoteUpdatedEvent;


        #region Instance control

        /// <summary>
        /// Constructor.
        /// </summary>
        public FXCMConnectionManager()
        {
            ChangeOperationalState(OperationalStateEnum.Constructed);

            _messageLoopOperator = new BackgroundMessageLoopOperator(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ChangeOperationalState(OperationalStateEnum.Disposed);

            _desk = null;
            _core = null;
            _tradeDeskEventsSink = null;

            BackgroundMessageLoopOperator messageLoopOperator = _messageLoopOperator;
            if (messageLoopOperator != null)
            {
                messageLoopOperator.Stop();
                messageLoopOperator.Dispose();
            }

            _messageLoopOperator = null;

            GC.Collect();
        }

        #endregion

        /// <summary>
        /// Perform a login.
        /// </summary>
        public bool Login(string username, string password, string serviceUrl, string accountType, out string operationResultMessage)
        {
            _messageLoopOperator.Start();

            if (OperationalState != OperationalStateEnum.Initialized
                 && OperationalState != OperationalStateEnum.Initializing
                && OperationalState != OperationalStateEnum.Constructed)
            {
                operationResultMessage = "Login already started.";
                return false;
            }

			this._serviceUrl = serviceUrl;

            string operationResultMessageCopy = string.Empty;

            ChangeOperationalState(OperationalStateEnum.Initializing);

            object result = false;
            GeneralHelper.GenericReturnDelegate<bool> del = delegate()
            {
                if (_core == null)
                {
                    _core = new FXCore.CoreAutClass();
                    _desk = (FXCore.TradeDeskAut)_core.CreateTradeDesk("trader");
                }

                try
                {
                    _desk.Login(username, password, serviceUrl, accountType);
                    
                    Managed_Subscribe();

                    SystemMonitor.Report("FXCM Service subscribed.");
                    ChangeOperationalState(OperationalStateEnum.Operational);
                }
                catch (Exception exception)
                {
                    operationResultMessageCopy = "Failed to log in [" + exception.Message + "].";
                    SystemMonitor.OperationError(operationResultMessageCopy);
                    ChangeOperationalState(OperationalStateEnum.NotOperational);
                }

                return _desk.IsLoggedIn();
            };

            if (_messageLoopOperator.Invoke(del, TimeSpan.FromSeconds(180), out result) == false || (bool)result == false)
            {
                ChangeOperationalState(OperationalStateEnum.NotOperational);
                operationResultMessage = operationResultMessageCopy;
                return false;
            }

            operationResultMessage = operationResultMessageCopy;
            return (bool)result;
        }

        /// <summary>
        /// Perform logout.
        /// </summary>
        public bool Logout()
        {
            ChangeOperationalState(OperationalStateEnum.NotOperational);

            GeneralHelper.DefaultDelegate del = delegate()
            {
                if (_core != null && _desk.IsLoggedIn())
                {
                    Managed_Unsubscribe();

                    _desk.Logout();

                    _desk = null;
                    _core = null;
                }
            };

            BackgroundMessageLoopOperator messageLoopOperator = _messageLoopOperator;

            if (messageLoopOperator == null || messageLoopOperator.Invoke(del, TimeSpan.FromSeconds(180)) == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public OrderInfo? SubmitOrder(AccountInfo accountInfo, Symbol symbol, OrderTypeEnum orderType, int volume,
            decimal? allowedSlippage, decimal? desiredPrice, decimal? takeProfit, decimal? stopLoss, out string operationResultMessage)
        {
            OrderInfo? order = null;
            operationResultMessage = string.Empty;
            string operationResultMessageCopy = string.Empty;

            GeneralHelper.DefaultDelegate delegateInstance = delegate()
            {
                try
                {

                    TradeDeskAut desk = _desk;
                    if (desk == null)
                    {
                        return;
                    }

                    bool isBuy = OrderInfo.TypeIsBuy(orderType);
                    double realValue = isBuy ? (double)GetInstrumentData(symbol.Name, "Ask") : (double)GetInstrumentData(symbol.Name, "Bid");

                    object orderId, psd;
                    object ocoStopOrderId = null, ocoProfitOrderId = null;

                    desk.CreateFixOrder(desk.FIX_OPENMARKET,
                                                    "",
                                                    realValue,
                                                    realValue,
                                                    (string)GetInstrumentData(symbol.Name, "QuoteID"),
                                                    accountInfo.Id,
                                                    symbol.Name,
                                                    isBuy,
                                                    volume,
                                                    "",
                                                    out orderId,
                                                    out psd);

                    order = new OrderInfo(orderId.ToString(), symbol, orderType,
                                                    OrderStateEnum.Executed, volume, new decimal(realValue), null,
                                                    null, null, null, null, null, null, null, null, null, "ok", "1");

                    //check bid value...
                    if (stopLoss.HasValue)
                    {
                        double dValue = Decimal.ToDouble(stopLoss.Value);

                        desk.CreateFixOrder(desk.FIX_ENTRYSTOP,
                                                        "",
                                                        dValue,
                                                        dValue,
                                                        (string)GetInstrumentData(symbol.Name, "QuoteID"),
                                                        accountInfo.Id,
                                                        symbol.Name,
                                                        !isBuy,
                                                        volume,
                                                        "",
                                                        out ocoStopOrderId,
                                                        out psd);
                    }

                    if (takeProfit.HasValue)
                    {
                        double dValue = Decimal.ToDouble(takeProfit.Value);

                        desk.CreateFixOrder(desk.FIX_ENTRYLIMIT,
                                                        "",
                                                        dValue,
                                                        dValue,
                                                        (string)GetInstrumentData(symbol.Name, "QuoteID"),
                                                        accountInfo.Id,
                                                        symbol.Name,
                                                        !isBuy,
                                                        volume,
                                                        "",
                                                        out ocoProfitOrderId,
                                                        out psd);
                    }

                    //Create OCO.
                    if (stopLoss.HasValue && takeProfit.HasValue)
                    {
                        bool creationOk = true;
                        FXCore.OrdersIDEnumAut group = new FXCore.OrdersIDEnumAut();
                        group.Add((string)ocoProfitOrderId);
                        group.Add((string)ocoStopOrderId);
                        object _result = null;
                        object ocoid = null;
                        desk.CreateOCO(group, out _result, out ocoid);

                        FXCore.OrdersBatchResultEnumAut result = (FXCore.OrdersBatchResultEnumAut)_result;
                        for (int i = 1; i <= result.Count; i++)
                        {
                            FXCore.OrderBatchResultAut res = (FXCore.OrderBatchResultAut)result.Item(i);
                            creationOk = creationOk && res.Success;

                            if (!creationOk)
                            {
                                //Ups throw an exception!!!  But what about the current order.
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    operationResultMessageCopy = GeneralHelper.GetExceptionMessage(ex);
                    SystemMonitor.OperationError(operationResultMessageCopy);
                }
            };

            BackgroundMessageLoopOperator messageOperator = _messageLoopOperator;
            if (messageOperator != null)
            {
                messageOperator.Invoke(delegateInstance, TimeSpan.FromSeconds(120));
            }
            operationResultMessage = operationResultMessageCopy;

            return order;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<AccountInfo> GetAvailableAccounts(int advisedDecimalsPrecision)
        {
            List<AccountInfo> result = new List<AccountInfo>();

            TradeDeskAut desk = _desk;
            if (desk == null)
            {
                return result;
            }

            GeneralHelper.DefaultDelegate delegateInstance = delegate()
            {
                try
                {
                    // Perform update.
                    TableAut accountsTable = (FXCore.TableAut)desk.FindMainTable("accounts");
                    foreach (RowAut item in (RowsEnumAut)accountsTable.Rows)
                    {
                        string id = (string)item.CellValue("AccountID");

                        if (string.IsNullOrEmpty(id))
                        {
                            SystemMonitor.OperationWarning("Account with null/empty id found.");
                            continue;
                        }

                        AccountInfo info = new AccountInfo();
                        // Have the accounts with default empty Guids.
                        //info.Guid = Guid.Empty;
                        info.Id = id;

                        info.Name = "FXCM." + (string)item.CellValue("AccountName");
                        info.Balance = Math.Round(new decimal((double)item.CellValue("Balance")), advisedDecimalsPrecision);
                        info.Equity = Math.Round(new decimal((double)item.CellValue("Equity")), advisedDecimalsPrecision);
                        info.Margin = Math.Round(new decimal((double)item.CellValue("UsableMargin")), advisedDecimalsPrecision);
                        info.Profit = Math.Round(new decimal((double)item.CellValue("GrossPL")), advisedDecimalsPrecision);
                        info.FreeMargin = Math.Round(new decimal((double)item.CellValue("UsableMargin")), advisedDecimalsPrecision);
						info.Company = "FXCM";
						info.Server = this._serviceUrl;

                        result.Add(info);
                    }
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationError(GeneralHelper.GetExceptionMessage(ex));
                }
            };

            BackgroundMessageLoopOperator messageLoopOperator = _messageLoopOperator;
            if (messageLoopOperator != null)
            {
                messageLoopOperator.Invoke(delegateInstance, TimeSpan.FromSeconds(60));
            }

            return result;
        }

        List<PositionInfo> Managed_ProcessPositions()
        {
            List<PositionInfo> result = new List<PositionInfo>();

            TradeDeskAut desk = _desk;
            if (desk == null)
            {
                return result;
            }

            TableAut tradesTable = (TableAut)desk.FindMainTable("summary");
            foreach (RowAut row in (RowsEnumAut)tradesTable.Rows)
            {
                // The unique number of the instrument that has open position(s).
                string offerID = (string)row.CellValue("OfferID");

                // The sequence number of the instrument in the list of instruments displayed to the Trading Station user.
                int defaultSortOrder = (int)row.CellValue("DefaultSortOrder");

                // The symbol indicating the instrument. For example, EUR/USD, USD/JPY, GBP/USD.
                string instrument = (string)row.CellValue("Instrument");

                // The current profit and loss on all positions opened in the instrument with the direction "sell". Commissions and interest are not taken into consideration. The SellNetP/L is expressed in the account currency.
                double sellNetPL = (double)row.CellValue("SellNetPL");

                // The trade amount of all positions opened in the instrument with the direction "sell".
                //The SellAmountK is expressed in the base currency of the instrument. For example, the value 50 for EUR/USD means that the total trade amount of all sell positions opened in EUR/USD is 50,000 Euros.
                double sellAmountK = (double)row.CellValue("SellAmountK");

                // The average open price of positions opened in the instrument with the direction "sell".
                double sellAverageOpen = (double)row.CellValue("SellAvgOpen");

                // The current market price at which all sell positions opened in the instrument can be closed.
                double buyClose = (double)row.CellValue("BuyClose");

                // The current market price at which all buy positions opened in the instrument can be closed.
                double sellClose = (double)row.CellValue("SellClose");

                // The average open price of positions opened in the instrument with the direction "buy".
                double buyAverageOpen = (double)row.CellValue("BuyAvgOpen");

                // The trade amount of all positions opened in the instrument with the direction "buy". 
                //The BuyAmountK is expressed in the base currency of the instrument. For example, the value 50 for EUR/USD means that the total trade amount of all buy positions opened in EUR/USD is 50,000 Euros.
                double buyAmountK = (double)row.CellValue("BuyAmountK");

                // The current profit and loss on all positions opened in the instrument with the direction "buy". Commissions and interest are not taken into consideration. The BuyNetP/L is expressed in the account currency.
                double buyNetPL = (double)row.CellValue("BuyNetPL");

                // The trade amount of all positions (both buy and sell) opened in the instrument. The AmountK is expressed in the base currency of the instrument. For example, the value 50 for EUR/USD means that the total trade amount of all positions opened in EUR/USD is 50,000 Euros.
                double amountK = (double)row.CellValue("AmountK");

                // The current profit and loss on all positions (both buy and sell) opened in the instrument, including commissions and interest. The GrossPL is expressed in the account currency.
                double grossPL = (double)row.CellValue("GrossPL");

                // The current profit and loss on all positions (both buy and sell) opened in the instrument, without taking into consideration the commissions and interest. The NetPL is expressed in the account currency.
                double NetPL = (double)row.CellValue("NetPL");

                // Not currently available.
                // The current profit and loss (in pips) on all positions opened in the instrument with the direction "sell". Commissions and interest are not taken into consideration.
                //double sellNetPLPip = (double)row.CellValue("SellNetPLPip");

                // Not currently available.
                // The current profit and loss (in pips) on all positions opened in the instrument with the direction "buy". Commissions and interest are not taken into consideration.
                //double buyNetPLPip = (double)row.CellValue("BuyNetPLPip");

                PositionInfo info = new PositionInfo(new Symbol(Symbol.SymbolGroup.Forex, instrument), (decimal)(amountK * 1000),
                    (decimal)(buyAmountK > 0 ? Math.Round(buyNetPL, IntegrationAdapter.AdvisedAccountDecimalsPrecision) : Math.Round(sellNetPL, IntegrationAdapter.AdvisedAccountDecimalsPrecision)), null, null, null,
                    (decimal)(buyAmountK > 0 ? buyAverageOpen : sellAverageOpen), (decimal)(Math.Round(grossPL, IntegrationAdapter.AdvisedAccountDecimalsPrecision)), null, null);

                result.Add(info);
            }

            return result;
        }

        void Managed_ProcessOrders()
        {
            //TableAut tradesTable = (TableAut)desk.FindMainTable("trades");
            //foreach (RowAut row in (RowsEnumAut)tradesTable.Rows)
            //{
            //    try
            //    {
            //        // The unique number of the open position. The number is unique within the connection (Real or Demo).
            //        string tradeId = (string)row.CellValue("TradeID");

            //        // The unique number of the account the position is opened on. The number is unique within the connection (Real or Demo). In Order2Go, the account is always referred to by the Account ID.
            //        string accountId = (string)row.CellValue("AccountID");

            //        //  The unique name of the account the position is opened on. This is the name that is displayed to the Trading Station user. The name is unique within the connection (Real or Demo).
            //        string accountName = (string)row.CellValue("AccountName");

            //        // The unique number of the instrument the position is opened in.
            //        string offerID = (string)row.CellValue("OfferID");

            //        // The symbol indicating the instrument the position is opened in. For example, EUR/USD, USD/JPY, GBP/USD.
            //        string symbol = (string)row.CellValue("Instrument");

            //        // The amount of the open position expressed in the base currency. For example, the value 20,000 for EUR/USD indicates that the total amount of the position is 20,000 Euros, for USD/JPY - 20,000 US dollars, etc.
            //        int volume = (int)row.CellValue("Lot");

            //        // The amount of the open position (in thousands) as specified by the Trading Station user. For example, the value 20 for EUR/USD indicates that the total amount of the position is 20,000 Euros, for USD/JPY - 20,000 US dollars, etc.
            //        double volumeK = (double)row.CellValue("AmountK");

            //        // The trade operation the position is opened by. Possible values: "B" - buy, "S" - sell.
            //        string side = (string)row.CellValue("BS");

            //        // The price the position is opened at.
            //        double open = (double)row.CellValue("Open");

            //        // The price at which the position can be closed at the current moment.
            //        double closingPrice = (double)row.CellValue("Close");

            //        // The price of the associated stop order (loss limit level). If there is no associated stop order, the property has the value "0".
            //        double stop = (double)row.CellValue("Stop");

            //        // The distance (in pips) between the current market price and the price at which the price of the trailing stop order will be changed to the next level. This is applicable only to stop orders with the trailing stop feature activated. The property does not make sense for dynamic trailing stop.
            //        double untTrlMove = (double)row.CellValue("UntTrlMove");

            //        // The price of the associated limit order (profit limit level). If there is no associated limit order, the property has the value "0".
            //        double limit = (double)row.CellValue("Limit");

            //        // The current profit/loss on the position in pips.
            //        double pl = (double)row.CellValue("PL");

            //        // The current profit/loss on the position in the account currency.
            //        double grossPl = (double)row.CellValue("GrossPL");

            //        // The commission, i.e. the amount of funds that is subtracted from the account for various reasons which are defined individually for each particular account by the terms and conditions of the trading agreement. The commission is expressed in the account currency.
            //        double commission = (double)row.CellValue("Com");

            //        // The interest, i.e. the cumulative amount of funds that is added to/subtracted from the account for holding the position overnight. The interest is expressed in the account currency.
            //        double interest = (double)row.CellValue("Int");

            //        // The date and time when the position was opened. The date and time are in the UTC time.
            //        DateTime openTime = (DateTime)row.CellValue("Time");

            //        // The type of the account the position is opened on. Possible values: "32" Trading account. "36" Managed account. "38" Controlled account.
            //        string accountType = (string)row.CellValue("Kind");

            //        // Defines whether the position is opened with the direction set to "buy". Possible values: "True" - buy; "False" - sell.
            //        bool isBuy = (bool)row.CellValue("IsBuy");

            //        // The unique number of the pair of prices (Bid and Ask) the position is opened at.
            //        string quoteId = (string)row.CellValue("QuoteID");

            //        // The unique number of the order the position is opened by. The number is unique within the connection (Real or Demo).
            //        string openOrderId = (string)row.CellValue("OpenOrderID");

            //        // The unique identifier of the order request the position is opened by. The key is unique within the connection (Real or Demo).
            //        string openOrderReqID = (string)row.CellValue("OpenOrderReqID");

            //        // The text comment added to the order the position is opened by.
            //        string comment = (string)row.CellValue("QTXT");

            //        // Not available in current version.
            //        // The unique number of the associated stop order. The number is unique within the connection (Real or Demo). If there is no associated stop order, the property has the empty value.
            //        //string stopOrderId = (string)row.CellValue("StopOrderID");

            //        // Not available in current version.
            //        // The unique number of the associated limit order. The number is unique within.
            //        //string limitOrderId = (string)row.CellValue("LimitOrderID");

            //    }
            //    catch (Exception ex)
            //    {
            //        SystemMonitor.OperationError("Failed to establish FXCM position data.", ex);
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Pass empty or null to extract all positions.</param>
        public List<PositionInfo> GetPositions(/*string requestedSymbol*/)
        {
            List<PositionInfo> result = new List<PositionInfo>();
            GeneralHelper.DefaultDelegate delegateInstance = delegate()
            {
                try
                {
                    result = Managed_ProcessPositions();
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationError("Failed to process FXCM positions.", ex);
                }
            };

            BackgroundMessageLoopOperator messageLoopOperator = _messageLoopOperator;
            if (messageLoopOperator != null)
            {
                messageLoopOperator.Invoke(delegateInstance, TimeSpan.FromSeconds(60));
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<DataBar> GetHistory(string forexPair, string period, DateTime lowerBound, DateTime upperBound)
        {
            bool keepIterating = true;
            List<DataBar> candlestickHistoryList = new List<DataBar>();

            GeneralHelper.DefaultDelegate delegateInstance = delegate()
            {
                TradeDeskAut desk = _desk;
                if (desk == null)
                {
                    return;
                }

                FXCore.IMarketRateEnumAut japaneseCandlestick;
                while (keepIterating)
                {
                    japaneseCandlestick = (FXCore.IMarketRateEnumAut)desk.GetPriceHistoryUTC(forexPair, period, lowerBound, upperBound, -1, false, true);

                    foreach (FXCore.IMarketRateAut marketRate in japaneseCandlestick)
                    {
                        DataBar dataBar = new DataBar(marketRate.StartDate, new decimal(marketRate.AskOpen), 
                            new decimal(marketRate.AskHigh), new decimal(marketRate.AskLow), new decimal(marketRate.AskClose), 0);

                        candlestickHistoryList.Add(dataBar);

                        lowerBound = marketRate.StartDate;
                    }

                    Thread.Sleep(100);

                    int i = japaneseCandlestick.Size;

                    keepIterating = i > 1 && upperBound.CompareTo(lowerBound) >= 0;

                    lowerBound.AddTicks(1);
                }
            };

            BackgroundMessageLoopOperator messageLoopOperator = _messageLoopOperator;
            if (messageLoopOperator != null)
            {
                messageLoopOperator.Invoke(delegateInstance, TimeSpan.FromSeconds(60));
            }

            return candlestickHistoryList;
        }

        public object GetInstrumentData(string forexPair, string columnName)
        {
            object columnData = null;

            GeneralHelper.DefaultDelegate delegateInstance = delegate()
            {
                TradeDeskAut desk = _desk;
                if (desk == null)
                {
                    return;
                }

                FXCore.TableAut offersTable = (FXCore.TableAut)desk.FindMainTable("offers");
                if (offersTable == null)
                {
                    return;
                }

                switch (forexPair)
                {
                    case "EUR/USD":
                        columnData = offersTable.CellValue(1, columnName);
                        break;
                    case "USD/JPY":
                        columnData = offersTable.CellValue(2, columnName);
                        break;
                    case "GBP/USD":
                        columnData = offersTable.CellValue(3, columnName);
                        break;
                    case "USD/CHF":
                        columnData = offersTable.CellValue(4, columnName);
                        break;
                    case "EUR/CHF":
                        columnData = offersTable.CellValue(5, columnName);
                        break;
                    case "AUD/USD":
                        columnData = offersTable.CellValue(6, columnName);
                        break;
                    case "USD/CAD":
                        columnData = offersTable.CellValue(7, columnName);
                        break;
                    case "NZD/USD":
                        columnData = offersTable.CellValue(8, columnName);
                        break;
                    case "EUR/GBP":
                        columnData = offersTable.CellValue(9, columnName);
                        break;
                    case "EUR/JPY":
                        columnData = offersTable.CellValue(10, columnName);
                        break;
                    case "GBP/JPY":
                        columnData = offersTable.CellValue(11, columnName);
                        break;
                    case "GBP/CHF":
                        columnData = offersTable.CellValue(12, columnName);
                        break;
                }
            };

            BackgroundMessageLoopOperator messageLoopOperator = _messageLoopOperator;
            if (messageLoopOperator != null)
            {
                messageLoopOperator.Invoke(delegateInstance, TimeSpan.FromSeconds(60));
            }

            return columnData;
        }

        /// <summary>
        /// Managed thread entrance only.
        /// </summary>
        void Managed_Subscribe()
        {
            if (_tradeDeskEventsSink == null)
            {
                _tradeDeskEventsSink = new TradeDeskEventsSinkClass();
                _tradeDeskEventsSink.ITradeDeskEvents_Event_OnRowChanged += new ITradeDeskEvents_OnRowChangedEventHandler(Managed_tdSink_ITradeDeskEvents_Event_OnRowChanged);
                _tradeDeskEventsSink.ITradeDeskEvents_Event_OnSessionStatusChanged += new ITradeDeskEvents_OnSessionStatusChangedEventHandler(Managed_tradeDeskEventsSink_ITradeDeskEvents_Event_OnSessionStatusChanged);
            }

            TradeDeskAut desk = _desk;
            if (_desk == null)
            {
                return;
            }

            _subscriptionId = desk.Subscribe(_tradeDeskEventsSink);
        }

        /// <summary>
        /// Managed thread entrance only.
        /// </summary>
        void Managed_Unsubscribe()
        {
            _tradeDeskEventsSink.ITradeDeskEvents_Event_OnRowChanged -= new ITradeDeskEvents_OnRowChangedEventHandler(Managed_tdSink_ITradeDeskEvents_Event_OnRowChanged);
            _tradeDeskEventsSink.ITradeDeskEvents_Event_OnSessionStatusChanged -= new ITradeDeskEvents_OnSessionStatusChangedEventHandler(Managed_tradeDeskEventsSink_ITradeDeskEvents_Event_OnSessionStatusChanged);

            if (_subscriptionId != -1)
            {
                TradeDeskAut desk = _desk;
                if (desk == null)
                {
                    return;
                }

                desk.Unsubscribe(_subscriptionId);
                _tradeDeskEventsSink = null;
            }
        }

        /// <summary>
        /// Managed thread entrance only.
        /// </summary>
        void Managed_tradeDeskEventsSink_ITradeDeskEvents_Event_OnSessionStatusChanged(string sStatus)
        {// TODO: handle changes in status of connection to the server.

            //The session can have one of the following statuses:
            //Disconnected
            // The connection to the trade server is not established. Methods of the TradeDeskAut except CheckVersion or Login must not be used.
            //Connecting
            // The connection to the trade server is being established. Methods of the TradeDeskAut must not be used.
            //Connected
            // The connection to the trade server is established and the tables are loaded. All methods of the TradeDeskAut except Login may be used.
            //Reconnecting
            // The connection to the server has been lost and the is being restored. Methods of the TradeDeskAut or subsequent objects must not be used. As you can see in the diagram below, this status can be reached because of connection problem as well as because of server forced re-login. You can use TradeDeskAut.LastError property to distinguish these variant. In case the reconnect is started because of connection problem the property will consist of an empty string. In case the reconnect is forced by the server, the property will consist of the server message.
            //Disconnecting
            // The connection to the trade server is being terminated and all session-related resources is being freed. Methods of the TradeDeskAut or subsequent objects must not be used.
        }

        /// <summary>
        /// Helper, convert to known order information.
        /// </summary>
        /// <returns></returns>
        OrderInfo? Managed_ExtractOrderInfo(RowAut orderInstrumentRow, out string accountId)
        {
            accountId = (string)orderInstrumentRow.CellValue("AccountID");
            OrderInfo info = new OrderInfo();


            return null;
        }

        /// <summary>
        /// Managed thread entrance only.
        /// </summary>
        void Managed_tdSink_ITradeDeskEvents_Event_OnRowChanged(object tableDisp, string rowID)
        {
            if (LoggedIn)
            {
                TradeDeskAut desk = _desk;
                if (desk == null)
                {
                    return;
                }

                try
                {
                    FXCore.ITableAut t = (FXCore.ITableAut)tableDisp;
                    if ("offers".Equals(t.Type))
                    {
                        TableAut offersTable = (TableAut)desk.FindMainTable("offers");
                        RowAut instrumentRow = (RowAut)offersTable.FindRow("OfferID", rowID, 0);

                        DataTick dataTick = new DataTick();
                        dataTick.Ask = (decimal)((double)instrumentRow.CellValue("Ask"));
                        dataTick.Bid = (decimal)((double)instrumentRow.CellValue("Bid"));
                        dataTick.DateTime = (DateTime)instrumentRow.CellValue("Time");

                        QuoteUpdateDelegate delegateInstance = QuoteUpdatedEvent;
                        if (delegateInstance != null)
                        {
                            GeneralHelper.FireAndForget(delegateInstance, this, (string)instrumentRow.CellValue("Instrument"), dataTick);
                        }
                    }
                    else if ("orders".Equals(t.Type))
                    {
                        // Orders table empty?
                        //TableAut offersTable = (TableAut)desk.FindMainTable("orders");
                        //RowAut instrumentRow = (RowAut)offersTable.FindRow("OrderID", rowID, 0);

                        //string accountId;
                        //OrderInfo? info = Managed_ExtractOrderInfo(instrumentRow, out accountId);

                        //OrderUpdateDelegate delegateInstance = OrderUpdatedEvent;
                        //if (info.HasValue && delegateInstance != null)
                        //{
                        //    GeneralHelper.FireAndForget(delegateInstance, this, accountId, info.Value);
                        //}
                    }
                    else if ("accounts".Equals(t.Type))
                    {
                        ItemUpdateDelegate delegateInstance = AccountUpdatedEvent;
                        if (delegateInstance != null)
                        {
                            GeneralHelper.FireAndForget(delegateInstance, this, rowID);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    SystemMonitor.Error("Failed to handle OnRow event", ex);
                }
            }
        }

    }
}
