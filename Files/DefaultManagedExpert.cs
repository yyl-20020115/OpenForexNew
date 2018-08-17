using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommonSupport;
using CommonFinancial;
using ForexPlatform;

/// <summary>
/// Make sure you inherit PlatformManagedExpert with your expert.
/// </summary>
[Serializable]
[UserFriendlyName("Default Managed Expert")]
public class MyTradeExpert : PlatformManagedExpert
{
    int _buyValue = 60;
    /// <summary>
    /// The value to perform buy operation at.
    /// </summary>
    public int BuyValue
    {
        get { return _buyValue; }
        set { _buyValue = value; }
    }

    int _closePositionValue = 45;
    /// <summary>
    /// The value to close any open position at.
    /// </summary>
    public int ClosePositionValue
    {
        get { return _closePositionValue; }
        set { _closePositionValue = value; }
    }

    int _sellValue = 30;
    /// <summary>
    /// The value to perform sell operation at.
    /// </summary>
    public int SellValue
    {
        get { return _sellValue; }
        set { _sellValue = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public MyTradeExpert(ISourceAndExpertSessionManager sessionManager, string name)
        : base(sessionManager, name)
    {
    }

    /// <summary>
    /// Handle startup actions.
    /// Return false if you fail to initialize.
    /// </summary>
    protected override bool OnStart()
    {
        // Here is an example how to use an indicator.
        // Create / Acquire indicator.
        Indicator indicator = this.ObtainIndicator("Rsi");

        // Change an input parameter.
        indicator.Parameters.SetCore("optInTimePeriod", 20);

        // Done automatically.
        //// Make sure values are updated with new parameter.
        //// This is not mandatory, but needed if you use operationResult values of the 
        //// indicator immediately after assigning its parameters.
        //indicator.Calculate(true);

        return true;
    }

    /// <summary>
    /// Handle all actions need to be done on stopping.
    /// </summary>
    protected override void OnStop()
    {
    }

    /// <summary>
    /// Quote dataDelivery was updated.
    /// </summary>
    protected override void OnDataBarPeriodUpdate(DataBarUpdateType updateType, int updatedBarsCount)
    {
        Indicator indicator = this.ObtainIndicator("Rsi");

        if (indicator.Results.GetValueSetCurrentValue(0).HasValue == false 
            || this.CanPlaceOrders == false)
        {
            return;
        }

        Trace(indicator.Results.GetValueSetCurrentValue(0).Value.ToString());

        if (this.CurrentPositionVolume == 0)
        {

            // Get the value of the [0] index operationResult set of this indicator.
            // Each indicator can have many operationResult sets - each represeting a "line" on the chart.
            if (indicator.Results.GetValueSetCurrentValue(0) > BuyValue)
            {// If the Rsi operationResult is above BuyValue, take some action.
                this.OpenBuyOrder(10000);
            }
            else if (indicator.Results.GetValueSetCurrentValue(0) < SellValue)
            {
                this.OpenSellOrder(10000);
            }
        }
        else
        {// Maybe we need to close an open position.

            if (this.CurrentPositionVolume > 0
                && indicator.Results.GetValueSetCurrentValue(0) < ClosePositionValue)
            {// If the Rsi operationResult is below SellValue, close buy position.
                this.ClosePosition();
            }

            if (this.CurrentPositionVolume < 0
                && indicator.Results.GetValueSetCurrentValue(0) > ClosePositionValue)
            {// If the Rsi operationResult is above SellValue, close sell position.
                this.ClosePosition();
            }
        }
    }
}
