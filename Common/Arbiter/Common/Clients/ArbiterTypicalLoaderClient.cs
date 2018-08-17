//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Reflection;

//namespace Arbiter
//{
//    /// <summary>
//    /// The purpose of this class is to implement a typical loader - 
//    /// specify the type of the class you need instantiated and the requestMessage filter that says when to create one.
//    /// The class will obey the single instance model. It will remember the instance created ID and monitor it, without referencing it.
//    /// If it goes away a new one will be created.
//    /// </summary>
//    public class ArbiterTypicalLoaderClient : ArbiterClientBase
//    {
//        /// <summary>
//        /// Here we keep all of the instantiated objects as well as info for all that need to be created.
//        /// </summary>
//        List<LoadingSet> _couples = new List<LoadingSet>();
        
//        protected class LoadingSet
//        {
//            public LoadingSet(MessageFilter loadingMessageFilter, Type instantiatedType)
//            {
//                LoadingMessageFilter = loadingMessageFilter;
//                InstantiatedType = instantiatedType;
//                ID = null;
//            }

//            public MessageFilter LoadingMessageFilter;
//            public Type InstantiatedType;
//            public ArbiterClientID? ID;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public ArbiterTypicalLoaderClient(string name)
//            : base(name, true)
//        {
//            _messageFilter.Enabled = false;
//        }

//        /// <summary>
//        /// The passed in type needs to be inheritor of the IArbiterClient and also the constructor 
//        /// needs to be parameterless or with one string parameter (name).
//        /// </summary>
//        public void AddLoadingCouple(MessageFilter loadingMessageFilter, Type instantiatedType)
//        {
//            if (instantiatedType.IsAbstract || instantiatedType.GetInterface(typeof(IArbiterClient).Name) == null ||
//                ( (instantiatedType.GetConstructor(new Type[] { }) == null) 
//                && (instantiatedType.GetConstructor(new Type[] { typeof(string) }) == null) ))
//            {
//                throw new Exception("The passed in type is not an arbiter client type or is abstract or it has no empty constructor or (string) constructor.");
//            }

//            _couples.Add(new LoadingSet(loadingMessageFilter, instantiatedType));
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override void ReceiveExecution(ExecutionEntity entity)
//        {
//            RunInstantiations(entity);
//        }

//        public override void ReceiveExecutionWithReply(ExecutionEntityWithReply entity)
//        {
//            RunInstantiations(entity);
//        }

//        public override void ReceiveConversationTimedOut(Conversation conversation)
//        {
//            throw new Exception("TypicalLoader does not use TimeOuts.");
//        }

//        protected void RunInstantiations(ExecutionEntity entity)
//        {
//            foreach (LoadingSet couple in _couples)
//            {
//                if (couple.LoadingMessageFilter.MessageAllowed(entity.Message) == false)
//                {
//                    continue;
//                }

//                // Creating a new instance of the specified type.
//                if (couple.ID != null && _arbiter.GetClientByID(couple.ID.Value) != null)
//                {// There is a live instance of this client, that is handling the case, continue.
//                    continue;
//                }

//                // We need to create a new instance, and register it to the arbiter, and remember its ID.

//                IArbiterClient client = null;
//                if (couple.InstantiatedType.GetConstructor(new Type[] { }) != null)
//                {// Parameterless constructor.
//                    client = (IArbiterClient)Activator.CreateInstance(couple.InstantiatedType);
//                }
//                else if (couple.InstantiatedType.GetConstructor(new Type[] { typeof(string) }) != null)
//                {// Single string parameter constructor.
//                    client = (IArbiterClient)Activator.CreateInstance(couple.InstantiatedType, new object[] { couple.InstantiatedType.Name + ".Loaded" });
//                }
                
//                couple.ID = client.SubscriptionClientID;

//                if (_arbiter.AddClient(client) == false ||
//                    _arbiter.GetClientByID(couple.ID.Value) == null)
//                {// We need to verify all went in properly, otherwise we are facing a problem. 
//                    // We can not know if the client will not change its ID, so check just to be sure.
//                    throw new Exception("Proper constructor not found.");
//                }

//                TraceHelper.Trace("Arbiter::RunInstantiations created a new object instance [" + client.GetType().Name + "].");
//            }// foreach
//        }

//    }
//}
