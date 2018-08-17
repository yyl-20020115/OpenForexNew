using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// Notify a subscriber his subscription has been terminated.
    /// </summary>
    [Serializable]
    public class SubscriptionTerminatedMessage : TransportMessage
    {
    }
}
