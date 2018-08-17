using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
  /// <summary>
  /// Platform Managed Expert notified this is manageable by the expert tradeEntities utility.
  /// It allows a simplified access to the abilities the platform provides and is the
  /// best starting point for implementing a simple to medium complex trading strategy.
  /// </summary>
  [Serializable]
  public abstract class PlatformManagedExpert : Expert
  {
    /// <summary>
    /// Managed expert has a dedicated tracer.
    /// </summary>
    [NonSerialized]
    Tracer _tracer = new Tracer();
    public Tracer Tracer
    {
      get { return _tracer; }
    }

    protected List<string> workingOrderIds = null;

    public virtual List<string> WorkingOrderIds
    {
      get
      {
        return this.workingOrderIds ?? (this.workingOrderIds = new List<string>());
      }
    }

    volatile bool _started = false;

    volatile int _currentSessionIndex = -1;

    [Browsable(false)]
    public ExpertSession CurrentSession
    {
      get
      {
        if (_currentSessionIndex < 0 || _currentSessionIndex >= Manager.SessionCount)
        {
          return null;
        }

        return Manager.SessionsArray[_currentSessionIndex];
      }
    }

    /// <summary>
    /// Indicates if the expert is capable of placing orders.
    /// </summary>
    public bool CanPlaceOrders
    {
      get
      {
        return CurrentSession != null && CurrentSession.OrderExecutionProvider != null && CurrentSession.OrderExecutionProvider.OperationalState == OperationalStateEnum.Operational;
      }
    }

    /// <summary>
    /// Active volume of the currently available position (or 0 if no position available or no volume assigned).
    /// </summary>
    public decimal CurrentPositionVolume
    {
      get
      {
        if (CurrentSession != null && CurrentSession.Position != null)
        {
          return CurrentSession.Position.Volume;
        }

        return 0;
      }
    }

    /// <summary>
    /// The position that this expert is currently managing.
    /// </summary>
    public Position Position
    {
      get
      {
        Position position = CurrentSession.OrderExecutionProvider.TradeEntities.ObtainPositionBySymbol(CurrentSession.Info.Symbol);
        if (position != null)
        {
          position.Tracer = Tracer;
        }
        return position;
      }
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public PlatformManagedExpert(ISourceAndExpertSessionManager manager, string name)
      : base(manager, name)
    {
      Manager.SessionsUpdateEvent += new GeneralHelper.GenericDelegate<ISourceAndExpertSessionManager>(SessionManager_SessionsUpdateEvent);
      _tracer.Add(new TracerItemKeeperSink(_tracer));
    }

 

    protected override bool OnUnInitialize()
    {
      base.OnUnInitialize();

      Stop();
      return true;
    }

    public override void OnDeserialization(object sender)
    {
      base.OnDeserialization(sender);
      _tracer = new Tracer();
      _tracer.Add(new TracerItemKeeperSink(_tracer));
    }

    void SessionManager_SessionsUpdateEvent(ISourceManager parameter1)
    {
      TracerHelper.Trace(_tracer, "");

      if (_currentSessionIndex == -1 && Manager.SessionCount > 0)
      {
        _currentSessionIndex = 0;

        if (CurrentSession.OperationalState == OperationalStateEnum.Operational)
        {
          if (CurrentSession.DataProvider.DataBars != null)
          {
            Start();
          }
          else
          {
            CurrentSession.DataProvider.DataBarProviderCreatedEvent += new DataProviderBarProviderUpdateDelegate(DataProvider_DataBarProviderCreatedEvent);
          }
        }
      }
    }

    void DataProvider_DataBarProviderCreatedEvent(ISessionDataProvider dataProvider, IDataBarHistoryProvider provider)
    {
      CurrentSession.DataProvider.DataBarProviderCreatedEvent -= new DataProviderBarProviderUpdateDelegate(DataProvider_DataBarProviderCreatedEvent);
      Start();
    }


    void DataBarHistory_DataBarHistoryUpdateEvent(IDataBarHistoryProvider provider, DataBarUpdateType updateType, int updatedBarsCount)
    {
      OnDataBarPeriodUpdate(provider, updateType, updatedBarsCount);
    }

    void Quote_QuoteUpdateEvent(IQuoteProvider provider)
    {
      OnQuoteUpdate(provider);
    }

    private void Start()
    {
      TracerHelper.TraceEntry(_tracer);

      if (_started == false)
      {
        _started = true;
        OnStart();

        if (CurrentSession.DataProvider != null && CurrentSession.DataProvider.Quotes != null)
        {
          CurrentSession.DataProvider.Quotes.QuoteUpdateEvent += new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
        }

        if (CurrentSession.DataProvider != null && CurrentSession.DataProvider.DataBars != null)
        {
          CurrentSession.DataProvider.DataBars.DataBarHistoryUpdateEvent += new DataBarHistoryUpdateDelegate(DataBarHistory_DataBarHistoryUpdateEvent);
        }
      }
    }

    private void Stop()
    {
      TracerHelper.TraceEntry(_tracer);
      if (this._started)
      {
        if (CurrentSession.DataProvider != null && CurrentSession.DataProvider.Quotes != null)
        {
          CurrentSession.DataProvider.Quotes.QuoteUpdateEvent -= new QuoteProviderUpdateDelegate(Quote_QuoteUpdateEvent);
        }

        if (CurrentSession.DataProvider != null && CurrentSession.DataProvider.DataBars != null)
        {
          CurrentSession.DataProvider.DataBars.DataBarHistoryUpdateEvent -= new DataBarHistoryUpdateDelegate(DataBarHistory_DataBarHistoryUpdateEvent);
        }

        this.OnStop();
        this._started = false;
      }
    }

    protected void Trace(string message)
    {
      TracerHelper.DoTrace(Tracer, TracerItem.TypeEnum.Trace, TracerItem.PriorityEnum.Low, message);
    }

    protected virtual bool OnStart()
    {
      return true;
    }

    protected virtual void OnStop()
    {
    }

    protected virtual void OnQuoteUpdate(IQuoteProvider provider)
    {
    }

    protected virtual void OnDataBarPeriodUpdate(IDataBarHistoryProvider provider, DataBarUpdateType updateType, int updatedBarsCount)
    {
    }

    /// <summary>
    /// Will create a new instance of the indicator with this name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Indicator ObtainIndicator(string name)
    {
      if (CurrentSession == null || CurrentSession.DataProvider.DataBars == null)
      {
        return null;
      }

      Indicator indicator = CurrentSession.DataProvider.DataBars.Indicators.GetFirstIndicatorByName(name);
      if (indicator != null)
      {
        return indicator;
      }

      indicator = IndicatorFactory.Instance.GetIndicatorCloneByName(name);
      if (indicator == null)
      {
        SystemMonitor.OperationError("Failed to find or create indicator [" + name + "]");
        return null;
      }

      CurrentSession.DataProvider.DataBars.Indicators.AddIndicator(indicator);
      return indicator;
    }
    public decimal? GetOrderResult(string orderId, Order.ResultModeEnum mode)
    {
      Position position = this.Position;

      if (position != null)
      {
        ISourceOrderExecution provider = this.Position.OrderExecutionProvider;
        if (provider != null)
        {
          Order order = provider.TradeEntities.GetOrderById(orderId);

          if (order != null)
          {
            return order.GetResult(mode);
          }
        }
      }
      return null;
    }
    public string OpenBuyOrder(int volume)
    {
      string operationResultMessage;
      string orderId = null;

      Position position = Position;

      if (position != null)
      {
        orderId = position.Submit(OrderTypeEnum.BUY_MARKET, volume, null, null, null, null, out operationResultMessage);
        if(!string.IsNullOrEmpty(orderId))
        {
          this.WorkingOrderIds.Add(orderId);
        }
      }
      else
      {
        TracerHelper.TraceOperationError(_tracer, "Failed to find corresponding position.");
      }

      return orderId;
    }

    public string OpenSellOrder(int volume)
    {
      string operationResultMessage;
      string orderId = null;

      Position position = Position;

      if (position != null)
      {
        orderId = position.Submit(OrderTypeEnum.SELL_MARKET, volume, null, null, null, null, out operationResultMessage);
        if (!string.IsNullOrEmpty(orderId))
        {
          this.WorkingOrderIds.Add(orderId);
        }
      }
      else
      {
        TracerHelper.TraceOperationError(_tracer, "Failed to find corresponding position.");
      }


      return orderId;
    }
    public virtual bool CloseOrder(string orderid)
    {
      Position position = this.Position;

      if(position!=null)
      {
        ISourceOrderExecution provider = this.Position.OrderExecutionProvider;
        if (provider !=null)
        {
          Order order = provider.TradeEntities.GetOrderById(orderid);

          if(order!=null)
          {
            this.WorkingOrderIds.Remove(orderid);
            if(order.Close())
            {
              return true;
            }

          }
        }
      }
      return false;
    }

    public void ClosePosition()
    {
      Position position = this.Position;

      if (position != null)
      {
        ISourceOrderExecution provider = this.Position.OrderExecutionProvider;
        if (provider != null)
        {
        
          for(int i = this.WorkingOrderIds.Count-1 ;i>=0 ;i--)
          {
            string orderId = this.WorkingOrderIds[i];

            if(!string.IsNullOrEmpty(orderId))
            {
              Order order = provider.TradeEntities.GetOrderById(orderId);

              if (order != null)
              {
                order.Close();
              }
            }
          }
          this.WorkingOrderIds.Clear();
        }
      }
    }
    //public void ClosePosition()
    //{
    //  string operationResultMessage;
    //  Position position = Position;

    //  if (position != null)
    //  {
    //    position.SubmitClose(null, out operationResultMessage);
    //  }
    //  else
    //  {
    //    TracerHelper.TraceOperationError(_tracer, "Failed to find corresponding position.");
    //  }
    //}
  }
}
