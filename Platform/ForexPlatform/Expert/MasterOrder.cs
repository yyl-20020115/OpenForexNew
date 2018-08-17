//using System;
//using System.Collections.Generic;
//using System.Text;
//using CommonFinancial;
//using System.Collections.ObjectModel;
//using CommonSupport;
//using System.Threading;

//namespace ForexPlatform
//{
//    /// <summary>
//    /// Master order class actively manages slave sessions to execute orders action corresponding to this 
//    /// objects actions; this way the master order allows multiple slave-orders to be controlled together
//    /// in a group, by controlling the master order.
//    /// </summary>
//    [Serializable]
//    public class MasterOrder : Order
//    {
//        List<ActiveOrder> _placedSlaveOrders = new List<ActiveOrder>();
//        /// <summary>
//        /// A list of slave orders to this master order, currently open.
//        /// </summary>
//        public ReadOnlyCollection<ActiveOrder> OpenedSlaveOrders
//        {
//            get { return _placedSlaveOrders.AsReadOnly(); }
//        }

//        VolumeDistributionEnum _volumeDistribution;

//        /// <summary>
//        /// The way lots closeVolume is distributed amongs slave accountInfos/orders.
//        /// </summary>
//        public enum VolumeDistributionEnum
//        {
//            PredefinedVolume,
//            TotalVolumeForEachOrder,
//            EqualParts,
//            OnEquityRatio,
//            OnFreeMarginRatio
//        }

//        DateTime _localOpenTime;
//        public DateTime LocalOpenTime
//        {
//            get { return _localOpenTime; }
//        }

//        List<ExpertSession> _slaveSessions = new List<ExpertSession>();

//        Symbol _symbol = new Symbol("MasterMulti", "Master Order / Multi Symbol");
        
//        public new Symbol? Symbol
//        {
//            get { lock (this) { return _symbol; } }
//        }

//        /// <summary>
//        /// Constructor. Set slave sessions, this order will put slave orders to.
//        /// </summary>
//        public MasterOrder(IEnumerable<ExpertSession> slaveSessions, VolumeDistributionEnum volumeDistribution, bool initialize)

//        {
//            _slaveSessions.Clear();
//            _slaveSessions.AddRange(slaveSessions);

//            _volumeDistribution = volumeDistribution;

//            if (initialize)
//            {
//                SystemMonitor.CheckThrow(this.Initialize(), "Failed to initialize order.");
//            }
//        }

//        public override void Dispose()
//        {
//            lock (this)
//            {
//                _slaveSessions = null;
//            }
//        }

//        public bool Initialize()
//        {
//            SystemMonitor.CheckThrow(State == OrderStateEnum.UnInitialized, "Order already initialized.");

//            State = OrderStateEnum.Initialized;
//            return true;
//        }

//        public void UnInitialize()
//        {
//            State = OrderStateEnum.UnInitialized;
//        }

//        public virtual void OnDeserialization(object sender)
//        {
//            State = OrderStateEnum.UnInitialized;
//        }

//        /// <summary>
//        /// Submit a master order. It will attemp to create a slave order on each slave sessionInformation.
//        /// </summary>
//        /// <returns>True at least some orders were opened OK, otherwise false.</returns>
//        public bool Place(OrderTypeEnum orderType, int volume, decimal? allowedSlippage,
//            decimal? desiredPrice, decimal? remoteTakeProfit, decimal? remoteStopLoss, 
//            string comment, out string operationResultMessage)
//        {
//            operationResultMessage = "";
//            SystemMonitor.NotImplementedCritical();
//            return false;

//            //SystemMonitor.CheckThrow(_placedSlaveOrders.Count == 0, "Can not place same master more than once.");

//            //if (OrderInfo.TypeIsDelayed(orderType))
//            //{
//            //    operationResultMessage = "Delayed order types currently not supported in master orders.";
//            //    return false;
//            //}

//            //string operationMessageSum = string.Empty;
//            //int ordersToProcess;
//            //lock (this)
//            //{
//            //    ordersToProcess = _slaveSessions.Count;
//            //}

//            //ManualResetEvent processEvent = new ManualResetEvent(false);

//            //GeneralHelper.GenericDelegate<ExpertSession> createPlaceDelegate = 
//            //    delegate(ExpertSession session)
//            //{
//            //    if (session.DataProvider == null || session.DataProvider.OperationalState != OperationalStateEnum.Operational)
//            //    {
//            //        lock (this)
//            //        {
//            //            operationMessageSum += System.Environment.NewLine + "Session [" + session.Info.Name + "] dataDelivery provider not operational.";
//            //        }
//            //    }
//            //    else
//            //    {
//            //        ActiveOrder order = new ActiveOrder(_ma session.OrderExecutionProvider, true);

//            //        string innerOperationResultMessage;
//            //        bool innerResult = order.Submit(orderType, closeVolume, allowedSlippage, desiredPrice, remoteTakeProfit, remoteStopLoss,
//            //            comment, out innerOperationResultMessage);

//            //        lock (this)
//            //        {// Make sure the operations to the common variables are synchronized.

//            //            if (innerResult)
//            //            {
//            //                _placedSlaveOrders.AddElement(order);
//            //                operationMessageSum += System.Environment.NewLine + "Session [" + session.Info.Name + "], Result [OK]";
//            //            }
//            //            else
//            //            {// AddElement error to composite requestMessage.
//            //                operationMessageSum += System.Environment.NewLine + "Session [" + session.Info.Name + "], Result [" + innerOperationResultMessage + "]";
//            //            }
//            //        }
//            //    }

//            //    lock (this)
//            //    {
//            //        ordersToProcess--;
//            //        if (ordersToProcess == 0)
//            //        {// The last one should set the event to notify the main thread.
//            //            processEvent.Set();
//            //        }
//            //    }
//            //};

//            //lock (this)
//            //{
//            //    foreach (ExpertSession session in _slaveSessions)
//            //    {
//            //        if (session.OrderExecutionProvider != null)
//            //        {
//            //            GeneralHelper.FireAndForget(createPlaceDelegate, session);
//            //        }
//            //    }
//            //}

//            //// Wait for all orders placement to end.
//            //if (ordersToProcess > 0)
//            //{
//            //    processEvent.WaitOne();
//            //}
//            //operationResultMessage = operationMessageSum;
            
//            //lock (this)
//            //{
//            //    _initialVolume = closeVolume;
//            //    _info.Volume = closeVolume;
//            //    _info.OpenPrice = 0;
//            //    _info.Type = orderType;

//            //    _info.StopLoss = remoteStopLoss;
//            //    _info.TakeProfit = remoteTakeProfit;
//            //    _localOpenTime = DateTime.Now;

//            //    if (IsDelayed)
//            //    {
//            //        State = OrderStateEnum.Submitted;
//            //    }
//            //    else
//            //    {
//            //        State = OrderStateEnum.Executed;
//            //    }

//            //    return _placedSlaveOrders.Count > 0;
//            //}
//        }

//        /// <summary>
//        /// Will attemp to close all slave orders simultaniously.
//        /// </summary>
//        /// <returns>True if at least one slave order was closed.</returns>
//        public bool Close(out string operationResultMessage)
//        {
//            int result = 0;

//            string operationMessageSum = string.Empty;
//            int ordersToProcess;
//            lock(this)
//            {
//                ordersToProcess = _placedSlaveOrders.Count;
//            }

//            ManualResetEvent processEvent = new ManualResetEvent(false);

//            GeneralHelper.GenericDelegate<ActiveOrder> closeDelegate =
//                delegate(ActiveOrder order)
//                {
//                    string innerOperationResultMessage;

//                    bool innerResult = order.Close(out innerOperationResultMessage);

//                    lock (this)
//                    {
//                        ordersToProcess--;
//                        if (innerResult)
//                        {
//                            result++;
//                            _placedSlaveOrders.Remove(order);
//                            operationMessageSum += System.Environment.NewLine + /*"Order [" + order.OrderExecutionProvider..Info.Name + */ "], Result [OK]";
//                        }
//                        else
//                        {
//                            operationMessageSum += System.Environment.NewLine + /*"Order [" + order.OrderExecutionProvider.Info.Name +*/ "], Result [" + innerOperationResultMessage + "]";
//                        }

//                        if (ordersToProcess == 0)
//                        {// The last one should set the event to notify the main thread.
//                            processEvent.Set();
//                        }
//                    }
//                };

//            lock (this)
//            {
//                foreach (ActiveOrder order in _placedSlaveOrders)
//                {// Delegate will remove orders from _openedSlaveOrders, but only after lock is released, so no problem to do it like this.
//                    GeneralHelper.FireAndForget(closeDelegate, order);
//                }
//            }

//            // Wait for all orders placement to end.
//            processEvent.WaitOne();

//            lock (this)
//            {
//                if (_placedSlaveOrders.Count == 0)
//                {
//                    State = OrderStateEnum.Closed;
//                }
//            }

//            operationResultMessage = operationMessageSum;
//            return result > 0;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override decimal? GetResult(ActiveOrder.ResultModeEnum resultMode)
//        {
//            if (_placedSlaveOrders == null || _placedSlaveOrders.Count == 0)
//            {
//                return null;
//            }

//            decimal result = 0;
//            lock (this)
//            {
//                foreach (ActiveOrder order in _placedSlaveOrders)
//                {
//                    decimal? orderResult = order.GetResult(resultMode);
//                    if (orderResult.HasValue)
//                    {
//                        result += orderResult.Value;
//                    }
//                }
//            }

//            return result;
//        }

//        public bool Cancel(out string operationResultMessage)
//        {
//            operationResultMessage = string.Empty;
//            SystemMonitor.NotImplementedCritical();
//            return false;
//        }

//        public bool IncreaseVolume(decimal volumeIncrease, decimal? allowedSlippage, decimal? desiredPrice, out string operationResultMessage)
//        {
//            operationResultMessage = string.Empty;
//            SystemMonitor.NotImplementedCritical();
//            return false;
//        }

//        public bool DecreaseVolume(decimal volumeDecrease, decimal? allowedSlippage, decimal? desiredPrice, out string operationResultMessage)
//        {
//            operationResultMessage = string.Empty;
//            SystemMonitor.NotImplementedCritical();
//            return false;
//        }

//        public override bool ModifyRemoteParameters(decimal? remoteStopLoss, decimal? remoteTakeProfit, decimal? remoteTargetOpenPrice, out string operationResultMessage)
//        {
//            operationResultMessage = string.Empty;
//            SystemMonitor.NotImplementedCritical();
//            return false;
//        }

//        public override string Print(bool fullPrint)
//        {
//            SystemMonitor.NotImplementedWarning();
//            return string.Empty;
//        }
//    }


//}
