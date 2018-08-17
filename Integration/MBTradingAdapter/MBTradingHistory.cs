using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using MBTHISTLib;
using CommonFinancial;
using System.Threading;

namespace MBTradingAdapter
{
    /// <summary>
    /// Manages history storage and access for the MBTrading adapter.
    /// </summary>
    public class MBTradingHistory : IDisposable, OperationPerformerStub.IImplementation
    {
        BackgroundMessageLoopOperator _messageLoopOperator;

        internal MbtHistMgr _historyClient;

        OperationPerformerStub _operationStub;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MBTradingHistory(BackgroundMessageLoopOperator messageLoopOperator)
        {
            _messageLoopOperator = messageLoopOperator;
            _operationStub = new OperationPerformerStub(this);
        }

        public bool Initialize(MbtHistMgr historyClient)
        {
            SystemMonitor.CheckError(_messageLoopOperator.InvokeRequred == false, "Init must better be called on message loop method.");

            _historyClient = historyClient;

            // *Never* subscribe to COM events of a property, always make sure to hold the object alive
            // http://www.codeproject.com/Messages/2189754/Re-Losing-COM-events-handler-in-Csharp-client.aspx

            _historyClient.OnDataEvent += new _IMbtHistEvents_OnDataEventEventHandler(_historyClient_OnDataEvent);
            _historyClient.OnError += new _IMbtHistEvents_OnErrorEventHandler(_historyClient_OnError);

            return true;
        }

        /// <summary>
        /// Get time converted to MBT format.
        /// </summary>
        /// <returns></returns>
        int? GetConvertedTime(DataHistoryOperation operation)
        {
            // Rules:
            //Day bars:
            //-3 = Calendar year
            //-2 = Calendar month
            //-1 = Calendar week
            //0 or 1 = One day
            //2 or more = days in bar with current day as last day

            //Min bars:
            //0 or 60 = One minute
            //Otherwise, the number of seconds desired, E.g.:
            //20 = 20 second bars
            //90 = one and-a-half minute bars
            //600 = ten minute bars 

            if (operation.Request.IsDayBased)
            {
                if (operation.Request.Period == TimeSpan.FromDays(365))
                {// Year.
                    return -3;
                }

                if (operation.Request.Period == TimeSpan.FromDays(31))
                {// Month.
                    return -2;
                }

                if (operation.Request.Period == TimeSpan.FromDays(7) || operation.Request.Period == TimeSpan.FromDays(5))
                {// Week.
                    return -1;
                }

                if (operation.Request.Period >= TimeSpan.FromDays(1) && operation.Request.Period <= TimeSpan.FromDays(4))
                {// Day (1,2,3 or 4)
                    return 1;
                }
            }

            if (operation.Request.IsMinuteBased)
            {
                if (operation.Request.Period >= TimeSpan.FromMinutes(1) && operation.Request.Period < TimeSpan.FromHours(24)) 
                {
                    return (int)operation.Request.Period.TotalSeconds;
                }
            }

            SystemMonitor.InvalidCall("Do not call time for tick based.");
            return null;

        }

        /// <summary>
        /// Submit a dataDelivery retrieval request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Place(DataHistoryOperation operation)
        {
            return _operationStub.PlaceOperation(operation, true);
        }

        /// <summary>
        /// 
        /// </summary>
        void _historyClient_OnDataEvent(int lRequestId, object pHist, enumHistEventType evt)
        {
            List<DataBar> extractedBars = new List<DataBar>();
            if (pHist is MbtHistDayBar)
            {
                MbtHistDayBar bars = (MbtHistDayBar)pHist;
                bars.First();

                while (bars.Eof == false)
                {
                    decimal open = (decimal)bars.Open;
                    decimal close = (decimal)bars.Close;
                    decimal high = (decimal)bars.High;
                    decimal low = (decimal)bars.Low;

                    extractedBars.Insert(0, new DataBar(bars.CloseDate, open, high, low, close, bars.TotalVolume));
                    bars.Next();
                }
            }
            else if (pHist is MbtHistMinBar)
            {
                MbtHistMinBar bars = (MbtHistMinBar)pHist;
                bars.First();

                while (bars.Eof == false)
                {
                    decimal open = (decimal)bars.Open;
                    decimal close = (decimal)bars.Close;
                    decimal high = (decimal)bars.High;
                    decimal low = (decimal)bars.Low;

                    extractedBars.Insert(0, new DataBar(bars.UTCDateTime, open, high, low, close, bars.TotalVolume));
                    bars.Next();
                }
            }

            DataHistoryOperation operation = (DataHistoryOperation)_operationStub.GetOperationById(lRequestId.ToString());
            if (operation != null)
            {
                _operationStub.CompleteOperation(lRequestId.ToString(), new DataHistoryUpdate(operation.Request.Period, extractedBars));
            }
        }

        void _historyClient_OnError(int lRequestId, object pHist, enumHistErrorType err)
        {
            SystemMonitor.OperationError("History error [" + err.ToString() + "]");
        }

        public void Dispose()
        {
            if (_historyClient != null)
            {
                _historyClient.OnDataEvent -= new _IMbtHistEvents_OnDataEventEventHandler(_historyClient_OnDataEvent);
                _historyClient.OnError -= new _IMbtHistEvents_OnErrorEventHandler(_historyClient_OnError);
                _historyClient = null;
            }

            if (_messageLoopOperator != null)
            {
                _messageLoopOperator = null;
            }
        }

        #region IImplementation Members

        public bool StartOperation(OperationInformation operationInformation)
        {
            DataHistoryOperation operation = (DataHistoryOperation)operationInformation;

            bool result = true;
            _messageLoopOperator.Invoke(delegate()
            {// Placing the request on the stolen main application thread, since we need the requestMessage pump for this to work properly.
                MbtHistMgr historyClient = _historyClient;
                if (historyClient == null)
                {
                    result = false;
                    return;
                }

                if (operation.Request.IsMinuteBased == false && operation.Request.IsDayBased == false)
                {
                    result = false;
                    return;
                }

                int? time = GetConvertedTime(operation);

                if (operation.Request.IsDayBased)
                {
                    MbtHistDayBar histBar;
                    histBar = historyClient.CreateHistDayBar();
                    histBar.Clear();
                    histBar.SendRequest(operation.Symbol, Int32.Parse(operation.Id), time.Value,
                        new DateTime(0), new DateTime(0), operation.Request.MaxValuesRetrieved.HasValue ? operation.Request.MaxValuesRetrieved.Value : int.MaxValue, true);
                }
                else if (operation.Request.IsMinuteBased)
                {
                    MbtHistMinBar histBar = historyClient.CreateHistMinBar();
                    histBar.Clear();
                    histBar.SendRequest(operation.Symbol, Int32.Parse(operation.Id), time.Value,
                        new DateTime(0), new DateTime(0), operation.Request.MaxValuesRetrieved.HasValue ? operation.Request.MaxValuesRetrieved.Value : int.MaxValue, true);
                }
            });

            return result;
        }

        #endregion
    }
}
