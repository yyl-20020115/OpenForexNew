//using System;
//using System.Collections.Generic;
//using System.Text;
//using ForexPlatform;
//using Arbiter;
//using CommonFinancial;

//namespace ProxySourceTest
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class TestAdapterImplementation : ProxyIntegrationAdapterImplementation
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public TestAdapterImplementation()
//            : base(new Uri("net.tcp://localhost:13123/TradingAPI"))
//        {
//        }

//        public void RegisterSource()
//        {
//            RegisterSourceMessage message = new RegisterSourceMessage(true);
//            message.SourceType = SourceTypeEnum.DataProvider | SourceTypeEnum.Live | SourceTypeEnum.HighPriority;
//            SendAddressed(_integrationServer.SubscriptionClientID, message);
//        }
//    }
//}
