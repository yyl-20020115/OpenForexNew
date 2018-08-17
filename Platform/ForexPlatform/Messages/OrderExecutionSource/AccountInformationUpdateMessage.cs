using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Message is sent when there is an update of account information in a given source.
    /// Not every account update may be registered, so this might be on a time basis, or 
    /// per request.
    /// </summary>
    [Serializable]
    public class AccountInformationUpdateMessage : AccountResponseMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public AccountInformationUpdateMessage(AccountInfo account, bool operationResult)
            : base(account, operationResult)
        {
        }
    }
}
