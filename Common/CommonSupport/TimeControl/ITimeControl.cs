using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonSupport
{
    /// <summary>
    /// Delegate for current step changed for ITimeControl.
    /// </summary>
    /// <param name="control"></param>
    public delegate void ITimeControlStepUpdateDelegate(ITimeControl control);

    /// <summary>
    /// Allows to perform time control on providers, executioners etc.
    /// Operations like stepping forwards and back are defined here, as well as the number of total periods available for stepping.
    /// </summary>
    public interface ITimeControl
    {
        /// <summary>
        /// CompletionEvent raised when current step has been changed.
        /// </summary>
        event ITimeControlStepUpdateDelegate CurrentStepChangedEvent;

        /// <summary>
        /// Total count of all steps in the item controlled.
        /// May be null if item does not have establish total number of steps.
        /// </summary>
        int? TotalStepsCount { get; }

        /// <summary>
        /// Index of the current period, starting from 0.
        /// May be null if current step is not establish for item.
        /// </summary>
        int? CurrentStep { get; }

        /// <summary>
        /// A serie of slave controls that follow the steps of this control.
        /// </summary>
        ReadOnlyCollection<ITimeControl> SlaveControls { get; }

        /// <summary>
        /// 
        /// </summary>
        bool AddSlaveControl(ITimeControl control);

        /// <summary>
        /// 
        /// </summary>
        bool RemoveSlaveControl(ITimeControl control);

        /// <summary>
        /// Is forward steping supported, based on current step position.
        /// </summary>
        bool CanStepBack { get; }
        
        /// <summary>
        /// Is steping back supported, based on the current step position.
        /// </summary>
        bool CanStepForward { get; }

        /// <summary>
        /// Is restarting supported.
        /// </summary>
        bool CanRestart { get; }
        
        /// <summary>
        /// Interval between steps of each period. May be null to signify unestablished or not applicable value.
        /// Some time controlees may not have fixed intervals, and be in floating interval, this case TimeSpan.Zero is returned.
        /// </summary>
        TimeSpan? Period { get; }

        /// <summary>
        /// Step forwards, advancing to next period.
        /// </summary>
        /// <returns>Will return false if stepping has failed; otherwise true.</returns>
        bool StepForward();

        /// <summary>
        /// Step forwards, advancing with steps periods ahead.
        /// </summary>
        /// <returns>Will return false if stepping has failed; otherwise true.</returns>
        bool StepForward(int steps);

        /// <summary>
        /// Step backwards, returning to previous period. Some time controllers may not support this.
        /// </summary>
        /// <returns>Will return false if stepping has failed; otherwise true.</returns>
        bool StepBack();

        /// <summary>
        /// Step backwards, returning to previous period. Some time controllers may not support this.
        /// </summary>
        /// <returns>Will return false if stepping has failed; otherwise true.</returns>
        bool StepBack(int steps);

        /// <summary>
        /// Skip to the index given. If the index is before current, operation may be denied by item 
        /// controlled and false will be returned. Also if index is more that total index count result may be false.
        /// </summary>
        /// <param name="index">Index to step to.</param>
        /// <returns>True to signify success in stepping to index; otherwise false to signify error.</returns>
        bool StepTo(int index);

        /// <summary>
        /// Do all steps to the end.
        /// </summary>
        /// <returns></returns>
        bool StepToEnd();

        /// <summary>
        /// Step back all the steps to the start.
        /// </summary>
        /// <returns></returns>
        bool Restart();
    }
}
