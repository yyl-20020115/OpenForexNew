//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Arbiter
//{
//    /// <summary>
//    /// Helpers for automating a subscription type routine connection.
//    /// Class is subscription client and provider in the same time.
//    /// </summary>
//    public class SubscriptionClient : TransportClient
//    {
//        Dictionary<Type, bool> _

//        /// <summary>
//        /// 
//        /// </summary>
//        public SubscriptionClient(string name, bool singleThreadMode)
//            : base(name, singleThreadMode)
//        {
//            //Filter.AllowChildrenTypes = true;
//        }

//        protected void SubscribeTo(List<ArbiterClientId?> forwardPathToSubscriptionProvider)
//        {
//        }

//        [MessageReceiver]
//        void Receive(SubscribeMessage message)
//        {
//        }

//        [MessageReceiver]
//        void Receive(UnSubscribeMessage message)
//        {
//        }

//        //[MessageReceiver]
//        //void Receive(SubscriptionDataMessage message)
//        //{
//        //}

//        [MessageReceiver]
//        void Receive(SubscriptionTerminatedMessage message)
//        {
//        }
//    }
//}
