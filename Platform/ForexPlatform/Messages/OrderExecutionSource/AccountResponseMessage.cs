using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Base class responce for an operation on an account.
    /// </summary>
    [Serializable]
    public class AccountResponseMessage : ResponseMessage
    {
        AccountInfo _accountInfo = AccountInfo.Empty;

        public AccountInfo AccountInfo
        {
            get { return _accountInfo; }
            set { _accountInfo = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountResponseMessage(AccountInfo accountInfo, bool operationResult)
            : base(operationResult)
        {
            _accountInfo = accountInfo;
        }
   }
}
