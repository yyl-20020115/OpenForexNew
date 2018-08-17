using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// Helper delegate.
    /// </summary>
    public delegate void EnvelopeUpdateDelegate(ArbiterSlimActiveClientStub stub, Envelope envelope);

    /// <summary>
    /// Base interface for client implementations of the Arbiter Slim client.
    /// </summary>
    public interface IArbiterSlimClient
    {
        IArbiterClient Client { get; }
        ArbiterSlim ArbiterSlim { get; }
        int ArbiterSlimIndex { get; }

        event EnvelopeUpdateDelegate EnvelopeReceivedEvent;
        event EnvelopeUpdateDelegate EnvelopeExecuteEvent;

        bool Receive(Envelope envelope);
        bool Send(Envelope envelope);

        bool OnAddToArbiter(ArbiterSlim arbiter, int arbiterIndex);
        void OnRemoveFromArbiter();
    }
}
