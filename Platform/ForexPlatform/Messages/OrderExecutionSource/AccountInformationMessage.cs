using System;
using System.Collections.Generic;
using System.Text;
using CommonFinancial;
using Arbiter;

namespace ForexPlatform
{
    /// <summary>
    /// Message allows to obtain updated account information.
    /// </summary>
    [Serializable]
    public class AccountInformationMessage : AccountRequestMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public AccountInformationMessage(AccountInfo info)
            : base(info)
        {
        }
    }
}
