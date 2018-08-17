using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommonSupport;
using CommonFinancial;
using ForexPlatform;
//using RtRNG;
using System.IO;
namespace ForexPlatformFrontEnd
{


  [Serializable]
  [UserFriendlyName("AI Expert")]
  public class AIPlatformManagedExpert : PlatformManagedExpert
  {
    public static int DefaultQuotesLimit = 16 * 1024 * 1024; //16M

    protected List<Quote?> quotes = null;

    protected Quote? last = null;

    protected decimal bought = decimal.Zero;

    protected decimal sold = decimal.Zero;

    protected decimal energyBought = decimal.Zero;

    protected decimal energySold = decimal.Zero;

    protected decimal energyTotalExpected = decimal.Zero;

    protected decimal energyReminsExpected = decimal.Zero;

    protected int exchangeTotalCount = 0;
    protected int raisedCount = 0;

    protected bool loaded = false;

    protected Thread thread = null;

    protected AutoResetEvent quitevent = null;

    protected AutoResetEvent notifyevent = null;



    protected decimal energyAsk = decimal.Zero;

    protected decimal energyBid = decimal.Zero;

    protected int quotesLimit = DefaultQuotesLimit;

    protected Random random = null;

    protected StreamWriter writer = null;

    public virtual List<Quote?> Quotes
    {
      get
      {
        return this.quotes ?? (this.quotes = new List<Quote?>());
      }
    }
    public virtual int QuotesLimit
    {
      get
      {
        return this.quotesLimit;
      }
      set
      {
        this.quotesLimit = value >= 0 ? value : 0;
      }
    }

    public virtual Random Random
    {
      get
      {
        return this.random ?? (this.random = new Random((int)DateTime.Now.Ticks));
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public AIPlatformManagedExpert(ISourceAndExpertSessionManager sessionManager, string name)
      : base(sessionManager, name)
    {

    }

    /// <summary>
    /// Handle startup actions.
    /// Return false if you fail to initialize.
    /// </summary>
    protected override bool OnStart()
    {
      this.writer = new StreamWriter(new FileStream("output.txt", FileMode.Create, FileAccess.ReadWrite,FileShare.Read));

      if((this.thread = new Thread(new ThreadStart(this.ThreadCore)))!=null)
      {
        this.notifyevent = new AutoResetEvent(false);
        this.quitevent = new AutoResetEvent(false);

        this.thread.Start();
      }


      return base.OnStart();
    }

    protected override void OnStop()
    {
      if (this.writer != null)
      {
        this.writer.Close();
        this.writer = null;
      }

      if (this.thread != null)
      {
        if(this.quitevent!=null)
        {
          this.quitevent.Set();
        }

        this.thread.Join();

        this.thread = null;
      }
      base.OnStop();
    }


    protected virtual decimal GetEnergy(Quote? quote, Quote? last, bool askbid)
    {
      decimal result = 0m;

      if (quote != null && last != null)
      {
        if (askbid)
        {
          // delta(E) = 0.5 * delta(m)v^2= volume * ((price - last) /duration)^2
          result = this.GetSquare(this.GetDelta(quote.Value.Ask, last.Value.Ask), this.GetDelta(quote.Value.Time, last.Value.Time)) * quote.Value.Volume.Value;
        }
        else
        {
          result = this.GetSquare(this.GetDelta(quote.Value.Bid, last.Value.Bid), this.GetDelta(quote.Value.Time, last.Value.Time)) * quote.Value.Volume.Value;
        }
      }

      return result;
    }

    protected virtual decimal GetDelta(decimal? a, decimal? b)
    {

      return (a.HasValue && b.HasValue) ? (decimal)(a.Value - b.Value) : 0;
    }

    protected virtual double GetDelta(DateTime? a, DateTime? b)
    {
      double minutes = (a.HasValue && b.HasValue) ? (a.Value - b.Value).TotalMinutes : 0.0;

      return minutes;
    }

    protected virtual decimal GetSquare(decimal a, double b)
    {
      if (a != 0m && b != 0.0)
      {
        double ax = (double)a;

        double rx = (ax / b) * (ax / b) * Math.Sign(ax);

        return new decimal(rx);
      }
      return 0m;
    }

    protected virtual bool AddQuote(Quote? quote)
    {
      if (quote != null)
      {
        if (this.last.HasValue && this.last.Value.IsFullySet && quote.Value.IsFullySet)
        {
          if (quote.Value.Time.Value == this.last.Value.Time.Value)
          {
            return false;
          }
        }

        this.Quotes.Add(quote);

        if (Quotes.Count >= this.QuotesLimit * 2)
        {
          this.Quotes.RemoveRange(0, this.Quotes.Count - this.QuotesLimit);
        }

        this.energyAsk += this.GetEnergy(quote, this.last, true);
        this.energyBid += this.GetEnergy(quote, this.last, false);

        this.last = quote;

        return true;
      }
      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    protected override void OnQuoteUpdate(IQuoteProvider provider)
    {
      lock (this)
      {
        if (provider != null)
        {
          if (this.AddQuote(provider.CurrentQuote))
          {
            if(this.writer!=null)
            {
              this.Decide(provider.CurrentQuote);

              this.writer.WriteLine("{0}, {1}, {2}", provider.Time, this.energyAsk, this.energyBid);
            }
          }
        }
      }
    }

    protected override void OnDataBarPeriodUpdate(IDataBarHistoryProvider provider, DataBarUpdateType updateType, int updatedBarsCount)
    {
      lock (this)
      {
        if (provider != null)
        {
          //Console.WriteLine("Bars: {0}", provider.BarCount);

          //Keep bars have no more limit than the quotes
          if (provider.DataBarLimit > this.QuotesLimit)
          {
            provider.DataBarLimit = this.QuotesLimit;
          }
        }
      }
    }

    protected virtual bool GetCoinSide(decimal part, decimal total)
    {
      total = total == decimal.Zero ? decimal.One : total;

      decimal percent = part / total;

      ulong result = (ulong)this.Random.Next();

      return result < ulong.MaxValue * percent;
    }

    protected virtual void ThreadCore()
    {
      if (this.quitevent != null)
      {
        while (!this.quitevent.WaitOne(TimeSpan.Zero))
        {
          
          //this.Decide(this.last);

        }
      }
    }
    protected string order = string.Empty;

    protected virtual void Decide(Quote? quote)
    {
      if (quote != null)
      {
        if (!this.loaded) //如果尚未买入，则考虑如何买入
        {
          this.loaded = true;
          //计算出从卖出到现在的价格的能量增量。

          this.order = this.OpenBuyOrder(10000);

        }
        else //如果已经买入，则考虑如何卖出
        {
          //计算出从买入到现在的的能量残余（买入的时候，已经计算了能量期望值在买入能量里面）

          decimal energyRemins = this.energyBid - this.energyBought;

          decimal? result = this.GetOrderResult(this.order, Order.ResultModeEnum.Currency);

          //如果已经获利
          if (result.HasValue && result.Value > 0)
          {
            Console.WriteLine("Get Money: {0}", result.Value);

            //最简单形式
            this.CloseOrder(this.order);

            this.loaded = false;
          }
        }
      }
    }


  }

}

