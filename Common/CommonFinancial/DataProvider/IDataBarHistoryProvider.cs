using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Collections.ObjectModel;

namespace CommonFinancial
{
    public enum DataBarUpdateType
    {// In order of importance.
        Initial,
        NewPeriod,
        HistoryUpdate,
        CurrentBarUpdate
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void DataBarHistoryUpdateDelegate(IDataBarHistoryProvider provider, DataBarUpdateType updateType, int updatedBarsCount);

    /// <summary>
    /// Interface defines how access to quotation history dataDelivery should be done.
    /// [Interface old name was IQuotationHistoryProvider]
    /// </summary>
    public interface IDataBarHistoryProvider : IOperational, IDisposable
    {
      int DataBarLimit { get; set; }
        /// <summary>
        /// Amount if bars to obtain from history requests.
        /// </summary>
        int DefaultHistoryBarsCount { get; set; }

        /// <summary>
        /// The count of stored bar dataDelivery units inside this provider.
        /// </summary>
        int BarCount { get; }

        /// <summary>
        /// Information on the session we are providing for.
        /// </summary>
        DataSessionInfo DataSessionInfo { get; }

        /// <summary>
        /// Time interval between quotes. May be null when value is not applicable or unknown.
        /// </summary>
        TimeSpan? Period { get; }

        /// <summary>
        /// Date and time of the first bar of this provider.
        /// </summary>
        DateTime? FirstBarTime { get; }

        /// <summary>
        /// Date and time of the last bar of this provider.
        /// </summary>
        DateTime? LastBarTime { get; }

        /// <summary>
        /// A list of all the dataDelivery units of this provider. Since it provides full access for speed efficiency
        /// evade changing this list.
        /// </summary>
        ReadOnlyCollection<DataBar> BarsUnsafe { get; }

        /// <summary>
        /// The currently pending dataDelivery unit.
        /// </summary>
        DataBar? Current { get; }

        /// <summary>
        /// Indicator manager handles indicators operations.
        /// </summary>
        IndicatorManager Indicators { get; }

        /// <summary>
        /// Extract dataDelivery values from the dataDelivery stored in this provider in double[] form. Specify the type 
        /// of field to use as basis for the extraction.
        /// </summary>
        decimal[] GetValues(DataBar.DataValueEnum valueEnum);

        /// <summary>
        /// See GetDataValues(BarData.DataValueSourceEnum valueEnum).
        /// </summary>
        decimal[] GetValues(DataBar.DataValueEnum valueEnum, int startingIndex, int indexCount);

        /// <summary>
        /// Extract dataDelivery values from the dataDelivery stored in this provider in double[] form. Specify the type 
        /// of field to use as basis for the extraction.
        /// This override allows the acquisition of dataDelivery converted to double values.
        /// </summary>
        double[] GetValuesAsDouble(DataBar.DataValueEnum valueEnum);

        /// <summary>
        /// See GetDataValues(BarData.DataValueSourceEnum valueEnum).
        /// </summary>
        double[] GetValuesAsDouble(DataBar.DataValueEnum valueEnum, int startingIndex, int indexCount);

        /// <summary>
        /// Get the corresponding bar index at given time. It is not a simple calculation, since some
        /// bars can be missing, also there are time periods when trading is not performed (like weekends, etc.)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        int GetIndexAtTime(DateTime time);

        /// <summary>
        /// CompletionEvent raised when update to the dataDelivery history has occured.
        /// </summary>
        event DataBarHistoryUpdateDelegate DataBarHistoryUpdateEvent;
    }
}
