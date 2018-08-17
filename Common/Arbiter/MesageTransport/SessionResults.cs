using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommonSupport;

namespace Arbiter.Transport
{
    /// <summary>
    /// The Guid for the session is kept in the Dictionary.
    /// </summary>
    class SessionResults
    {
        int _responsesRequired;
        Type _responseTypeRequired;

        public AutoResetEvent _sessionEndEvent = new AutoResetEvent(false);
        public AutoResetEvent SessionEndEvent
        {
            get 
            {
                return _sessionEndEvent; 
            }
        }


        List<TransportMessage> _responsesReceived = new List<TransportMessage>();
        public List<TransportMessage> Responses
        {
            get 
            {
                lock (_responsesReceived)
                {
                    return _responsesReceived;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responcesRequired"></param>
        /// <param name="responceTypeRequired"></param>
        public SessionResults(int responsesRequired, Type responseTypeRequired)
        {
            _responsesRequired = responsesRequired;
            _responseTypeRequired = responseTypeRequired;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReceiveResponse(TransportMessage message)
        {
            if (message != null 
                && message.GetType() != _responseTypeRequired 
                && message.GetType().IsSubclassOf(_responseTypeRequired) == false)
            {
                SystemMonitor.Error("Session received invalid responce message type [expected (" + _responseTypeRequired.Name + "), received(" + message.GetType().Name + ")]. Message ignored.");
            }
            else
            {
                if (message == null)
                {// We received a NULL signalling to stop wait and abort session.
                    _sessionEndEvent.Set();
                    return;
                }
                else
                {
                    lock (_responsesReceived)
                    {
                        if (_responsesReceived.Count < _responsesRequired)
                        {
                            _responsesReceived.Add(message);

                            if (_responsesReceived.Count == _responsesRequired)
                            {
                                _sessionEndEvent.Set();
                            }
                        }
                        else
                        {// One more requestMessage responce received.
                            TracerHelper.TraceError("Session received too many response messages. Message ignored.");
                        }
                    }
                }
            }
        }
    }

}
