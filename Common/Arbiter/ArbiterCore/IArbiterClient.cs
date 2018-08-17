using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Arbiter
{
    /// <summary>
    /// The inheritor must make sure to thread protect all the methods from the interface, as there might be many threads simultaniously invoking them.
    /// </summary>
    public interface IArbiterClient
    {
        /// <summary>
        /// Name of the client, usually the same as the name of the Id.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Don't change this during operation, should be the same for the instance.
        /// </summary>
        ArbiterClientId SubscriptionClientID
        {
            get;
        }

        /// <summary>
        /// Implementor to provide the filter for the messages.
        /// </summary>
        MessageFilter SubscriptionMessageFilter
        {
            get;
        }

        /// <summary>
        /// Does the module support multple threads entering or only one thread per time.
        /// </summary>
        bool SingleThreadMode
        {
            get;
        }

        /// <summary>
        /// Receive the arbiter instance and remember it, as you will need to use it.
        /// </summary>
        bool ArbiterInitialize(Arbiter arbiter);

        /// <summary>
        /// 
        /// </summary>
        bool ArbiterUnInitialize();

        /// <summary>
        /// Used for the (high speed) direct call transport mechanism.
        /// </summary>
        Message ReceiveDirectCall(Message message);

        /// <summary>
        /// Typical execution.
        /// </summary>
        /// <param name="execution"></param>
        void ReceiveExecution(ExecutionEntity entity);

        /// <summary>
        /// This execution gives the option of reply.
        /// </summary>
        /// <param name="execution"></param>
        void ReceiveExecutionWithReply(ExecutionEntityWithReply entity);

        /// <summary>
        /// The handler must use the IConversation conversation only during the invocation of this function.
        /// </summary>
        /// <param name="requestMessage"></param>
        void ReceiveConversationTimedOut(Conversation conversation);
    }
}
