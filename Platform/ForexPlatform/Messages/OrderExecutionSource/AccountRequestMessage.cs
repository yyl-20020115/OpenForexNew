using System;
using System.Collections.Generic;
using System.Text;
using Arbiter;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Base class for messages perfomed on an account.
    /// </summary>
    [Serializable]
    public class AccountRequestMessage : RequestMessage
    {
        AccountInfo _accountInfo = AccountInfo.Empty;
        /// <summary>
        /// 
        /// </summary>
        public AccountInfo AccountInfo
        {
            get { return _accountInfo; }
            set { _accountInfo = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public AccountRequestMessage(AccountInfo accountInfo)
            : base()
        {
            _accountInfo = accountInfo;
        }
    }
}
