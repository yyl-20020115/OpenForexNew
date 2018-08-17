//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Arbiter.Integration
//{
//    public class IntegrationServerTransportMessageFilter : TransportMessageFilter
//    {
//        Dictionary<string, string> _tagToClientID = new Dictionary<string, string>();
//        Dictionary<string, string> _clientIDToTag = new Dictionary<string, string>();

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="ownerID"></param>
//        public IntegrationServerTransportMessageFilter(ArbiterClientID ownerID) 
//            : base(ownerID)
//        {
//        }

//        public string GetTagByClientID(string clientID)
//        {
//            lock (this)
//            {
//                if (_clientIDToTag.ContainsKey(clientID))
//                {
//                    return _clientIDToTag[clientID];
//                }
//                SystemMonitor.Error("Empty result.");
//                return "";
//            }
//        }

//        public string GetClientIDByTag(string tag)
//        {
//            lock (this)
//            {
//                if (_tagToClient.ContainsKey(tag) == false)
//                {
//                    SystemMonitor.Error("Empty result."); 
//                    return "";
//                }
//                return _tagToClient[tag].Guid.ToString();
//            }
//        }

//        public void AddClient(string clientTag, string clientID)
//        {
//            lock (this)
//            {
//                if (_tagToClient.ContainsKey(clientTag) == false && _clientIDToTag.ContainsKey(clientID.Guid.ToString()) == false)
//                {
//                    _tagToClient.Add(clientTag, clientID);
//                    _clientIDToTag.Add(clientID, clientTag);
//                }
//            }
//        }


//        public void RemoveByClientTag(string clientTag)
//        {
//            lock (this)
//            {
//                string clientID = GetClientIDByTag(clientTag);
//                _tagToClient.Remove(clientTag);
//                _clientIDToTag.Remove(clientID);
//            }
//        }

//        protected override bool IsAddressedToMe(TransportMessage requestMessage)
//        {
//            if (requestMessage.CurrentTransportInfo.Value.ReceiverID == null ||
//                requestMessage.CurrentTransportInfo.Value.ReceiverID.HasValue == false)
//            {
//                return false;
//            }

//            if (base.IsAddressedToMe(requestMessage))
//            {
//                return true;
//            }

//            lock(this)
//            {
//                return _clientIDToTag.ContainsKey(requestMessage.CurrentTransportInfo.Value.ReceiverID.Value.Guid.ToString());
//            }
//        }
//    }
//}
