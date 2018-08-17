using System;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Message used for transporting trading values.
    /// </summary>
    [Serializable]
    public class DataHistoryUpdateMessage : DataSessionResponseMessage
    {
        DataHistoryUpdate _update = null;
        public DataHistoryUpdate Update
        {
            get { return _update; }
            set { _update = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataHistoryUpdateMessage(DataSessionInfo session, bool operationResult)
            : base(session, operationResult)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        public DataHistoryUpdateMessage(DataSessionInfo session, DataHistoryUpdate update, bool operationResult)
            : base(session, operationResult)
        {
            _update = update;
        }
    }
}
