using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// Enum with states an object can be in. Only one state at a given moment is typically allowed.
    /// Not every object goes trough all the stages, it is up to itself to determine what state and when to be in.
    /// </summary>
    public enum OperationalStateEnum
    {
        Unknown, // [Reccommendation] State of the item now known.
        Constructed, // [Reccommendation] Object was constructed.
        Initializing, // [Reccommendation] Object is initializing (maybe waiting for additional data).
        Initialized, // [Reccommendation] Object is initialized.
        Operational, // [Reccommendation] Object is ready for operation.
        NotOperational, // [Reccommendation] Object is not ready for operation.
        UnInitialized, // [Reccommendation] Object was uninitialized.
        Disposed // [Reccommendation] Object was disposed.
    }

    /// <summary>
    /// IOperational related delegate.
    /// </summary>
    public delegate void OperationalStateChangedDelegate(IOperational operational, OperationalStateEnum previousOperationState);
    
    /// <summary>
    /// Interface defines an object that has operational and non operation states.
    /// </summary>
    public interface IOperational
    {
        /// <summary>
        /// The current state of the object.
        /// </summary>
      OperationalStateEnum OperationalState { get; }

        /// <summary>
        /// Raised when operator changes state, the second parameter is the *previous operational* state.
        /// </summary>
        event OperationalStateChangedDelegate OperationalStateChangedEvent;

        void ChangeOperationalState(OperationalStateEnum OperationalState);

    }
}
