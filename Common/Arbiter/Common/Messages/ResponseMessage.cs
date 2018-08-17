using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Base class for responding messages (not mandatory).
    /// Did operation succeeded.
    /// </summary>
    [Serializable]
    public class ResponseMessage : TransportMessage
    {
        string _requestMessageTypeName = string.Empty;
        /// <summary>
        /// This is automatically assigned by the Arbiter Transportation framework (TransportClient)
        /// and states the name of the class that this responce message is responding to (if available).
        /// </summary>
        public string RequestMessageTypeName
        {
            get { return _requestMessageTypeName; }
            set { _requestMessageTypeName = value; }
        }

        bool _operationResult = true;
        public bool OperationResult
        {
            get { return _operationResult; }
            set { _operationResult = value; }
        }

        string _operationResultMessage = null;
        public string OperationResultMessage
        {
            get { return _operationResultMessage; }
            set { _operationResultMessage = value; }
        }

        public string ResultMessage
        {
            get { return _operationResultMessage; }
            set { _operationResultMessage = value; }
        }

        Exception _exception = null;
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ResponseMessage()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public ResponseMessage(bool operationResult)
        {
            _operationResult = operationResult;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationResult"></param>
        public ResponseMessage(bool operationResult, string errorMessage)
        {
            _operationResult = operationResult;
            _operationResultMessage = errorMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        public ResponseMessage(bool operationResult, Exception exception)
        {
            _operationResult = operationResult;
            _exception = exception;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string GetErrorMessage()
        {
            if (string.IsNullOrEmpty(OperationResultMessage) == false)
            {
                return OperationResultMessage;
            }
            else if (Exception != null)
            {
                return Exception.Message;
            }
            else
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CopyParameters(ResponseMessage message)
        {
            this._operationResult = message.OperationResult;
            _exception = message.Exception;
            _operationResultMessage = message.OperationResultMessage;
        }
    }
}
