using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Collections.ObjectModel;

namespace CommonFinancial
{
    public enum DataTickUpdateType
    {// In order of importance.
        Initial,
        HistoryUpdate,
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void DataTickHistoryUpdateDelegate(IDataTickHistoryProvider provider, DataTickUpdateType updateType);

    /// <summary>
    /// Interface defines how access to quotation history dataDelivery should be done.
    /// [Interface old name was IQuotationHistoryProvider]
    /// </summary>
    public interface IDataTickHistoryProvider : IOperational
    {
        /// <summary>
        /// The count of stored dataDelivery units inside this provider.
        /// </summary>
        int TickCount { get; }

        /// <summary>
        /// Date and time of the first tick of this provider.
        /// </summary>
        DateTime? FirstTime { get; }

        /// <summary>
        /// Date and time of the last tick of this provider.
        /// </summary>
        DateTime? LastTime { get; }

        /// <summary>
        /// A list of all the dataDelivery units of this provider. Since it provides full access for speed efficiency
        /// evade changing this list.
        /// </summary>
        ReadOnlyCollection<DataTick> TicksUnsafe { get; }

        /// <summary>
        /// The currently pending dataDelivery unit.
        /// </summary>
        DataTick? Current { get; }

        /// <summary>
        /// Extract dataDelivery values from the dataDelivery stored in this provider in double[] form. Specify the type 
        /// of field to use as basis for the extraction.
        /// </summary>
        decimal[] GetValues(DataTick.DataValueEnum valueEnum);

        /// <summary>
        /// See GetDataValues(DataTick.DataValueSourceEnum valueEnum).
        /// </summary>
        decimal[] GetValues(DataTick.DataValueEnum valueEnum, int startingIndex, int indexCount);

        /// <summary>
        /// Extract dataDelivery values from the dataDelivery stored in this provider in double[] form. Specify the type 
        /// of field to use as basis for the extraction.
        /// This override allows the acquisition of dataDelivery converted to double values.
        /// </summary>
        double[] GetValuesAsDouble(DataTick.DataValueEnum valueEnum);

        /// <summary>
        /// See GetDataValues(DataTick.DataValueSourceEnum valueEnum).
        /// </summary>
        double[] GetValuesAsDouble(DataTick.DataValueEnum valueEnum, int startingIndex, int indexCount);

        /// <summary>
        /// Get the corresponding bar index at given time. It is not a simple calculation, since some
        /// bars can be missing, also there are time periods when trading is not performed (like weekends, etc.)
        /// </summary>
        int GetIndexAtTime(DateTime time);

        /// <summary>
        /// CompletionEvent raised when update to the dataDelivery history has occured.
        /// </summary>
        event DataTickHistoryUpdateDelegate DataTickHistoryUpdateEvent;
    }
}
