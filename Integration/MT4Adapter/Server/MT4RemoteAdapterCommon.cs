using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using CommonFinancial;
using Arbiter;
using ForexPlatform;

namespace MT4Adapter
{
    /// <summary>
    /// Common layer of the adapter handles. It predefines common stuff for the communication back to the MT4 expert.
    /// </summary>
    public class MT4RemoteAdapterCommon : ProxyIntegrationAdapter
    {
        public const string SeparatorSymbol = ";";

        OperationPerformerStub _stub = new OperationPerformerStub();

        /// <summary>
        /// 
        /// </summary>
        public MT4RemoteAdapterCommon(Uri uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Helper, allows to extract the name of a symbol from the expert Id name.
        /// </summary>
        /// <param name="expertId"></param>
        /// <returns></returns>
        public string ExpertIdToSymbolName(string expertId)
        {
            return expertId.Substring(0, expertId.IndexOf("("));
        }

        // >>
        public void ErrorOccured(int operationResult, string errorMessage)
        {
            TracerHelper.TraceEntry(errorMessage);

            try
            {
                //SessionErrorOccuredMessage message = new SessionErrorOccuredMessage(_sessionInformation.Info, operationResult, errorMessage);
                //SendToSubscriber(message);
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

        }

        /// <summary>
        /// Gather all operations of this type and provide them here.
        /// </summary>
        /// <typeparam name="RequiredType"></typeparam>
        /// <returns></returns>
        protected List<OperationInformation> GetPendingMessagesOperations<RequiredType>(bool gatherStarted)
            where RequiredType : RequestMessage
        {
            List<OperationInformation> result = new List<OperationInformation>();
            try
            {
                lock (this)
                {
                    foreach (OperationInformation info in _stub.PendingOperationsArray)
                    {
                        if (info.Request is RequiredType && (gatherStarted || info.IsStarted == false))
                        {
                            result.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        protected RequiredType GetPendingMessage<RequiredType>(bool startOperation, out int id)
            where RequiredType : RequestMessage
        {
            id = 0;

            try
            {
                lock (this)
                {
                    foreach (OperationInformation info in _stub.PendingOperationsArray)
                    {
                        if (info.Request is RequiredType && info.IsStarted == false)
                        {
                            TracerHelper.Trace("Executing " + info.Request.GetType().Name + " requested was " + typeof(RequiredType).Name);
                            
                            id = Int32.Parse(info.Id);

                            if (startOperation)
                            {
                                info.Start();

                                if (((RequestMessage)info.Request).PerformSynchronous == false)
                                {// Finish of operations that do not require responces as soon as they are started.
                                    TracerHelper.Trace("Completing " + info.Request.GetType().Name);
                                    _stub.CompleteOperation(info.Id, null);
                                }
                            }


                            return (RequiredType)info.Request;
                        }
                    }
                }
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

            return null;
        }

        protected OperationInformation GetOperationById(int id)
        {
            lock (this)
            {
                return _stub.GetOperationById(id.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool CompleteOperation(int operationId, ResponseMessage message)
        {
            lock (this)
            {
                return _stub.CompleteOperation(operationId.ToString(), message);
            }
        }

        [MessageReceiver]
        protected ResponseMessage Receive(RequestMessage message)
        {
            TracerHelper.TraceEntry(message.GetType().Name);

            try
            {
                OperationInformation info = new OperationInformation();
                info.Request = message;

                if (message.PerformSynchronous)
                {
                    ResponseMessage result;
                    if (_stub.PerformOperation<ResponseMessage>(info, null, true, out result))
                    {// Operation performed successfully.
                        TracerHelper.Trace("Operation [" + message.GetType().Name + "] performed successfully.");
                        return result;
                    }
                    else
                    {
                        TracerHelper.TraceOperationError("Operation [" + message.GetType().Name + "] timed out.");
                    }
                }
                else
                {
                    _stub.PlaceOperation(info, true);
                    
                    if (message.RequestResponse)
                    {
                        return new ResponseMessage(true);
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {// Make sure we handle any possible unexpected exceptions, as otherwise they bring the
                // entire package (MT4 included) down with a bad error.
                SystemMonitor.Error(ex.Message);
            }

            return null;
        }

        #region Helpers

        ///// <summary>
        ///// Convert symbol name for understanding by the MT4.
        ///// </summary>
        ///// <param name="symbol"></param>
        ///// <returns></returns>
        //public static string ConvertSymbol(Symbol symbol)
        //{
        //    return symbol.Name.Replace("/", "").ToUpper();
        //}

        /// <summary>
        /// Helper.
        /// </summary>
        public static decimal? Convert(decimal valueType)
        {
            if (valueType == 0)
            {
                return null;
            }
            return valueType;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public static int? Convert(int valueType)
        {
            if (valueType == 0)
            {
                return null;
            }
            return valueType;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public static decimal TranslateModificationValue(decimal? value)
        {
            decimal result = 0;
            if (value.HasValue)
            {
                if (value.Value == decimal.MaxValue || value.Value == decimal.MinValue)
                {// Nan means, set to not assigned.
                    result = -1;
                }
                else
                {// Normal value assignment.
                    result = value.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public static long TranslationModificationValue(long? intValue)
        {
            long result = 0;
            if (intValue.HasValue)
            {
                if (long.MinValue == intValue.Value)
                {// Nan means, set to not assigned.
                    result = -1;
                }
                else
                {// Normal value assignment.
                    result = intValue.Value;
                }
            }

            return result;
        }

        #endregion

    }
}
