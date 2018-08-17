using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// News source for specific forex news.
    /// </summary>
    [EventItemType(typeof(ForexNewsItem))]
    public class ForexNewsSource : EventSource
    {
        /// <summary>
        /// 
        /// </summary>
        public ForexNewsSource()
        {
        }

        protected override void OnUpdate()
        {
            
        }
    }
}
