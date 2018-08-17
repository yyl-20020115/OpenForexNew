using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommonFinancial;
using CommonSupport;

namespace MBTradingAdapter
{
    /// <summary>
    /// 
    /// </summary>
    public class DataHistoryOperation : OperationInformation
    {
        DataHistoryRequest _request;
        public DataHistoryRequest Request
        {
            get { return _request; }
        }

        public new DataHistoryUpdate Response
        {
            get { return (DataHistoryUpdate)base.Response; }
        }

        volatile string _symbol;
        public string Symbol
        {
            get { return _symbol; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistoryOperation(string symbol, DataHistoryRequest request)
        {
            _symbol = symbol;
            _request = request;
        }
        
    }
}
