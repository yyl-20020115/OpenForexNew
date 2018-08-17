using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace CommonFinancial
{
    /// <summary>
    /// Class stores and provides access to dataDelivery incoming from a dataDelivery delivery unit.
    /// </summary>
    [Serializable]
    public sealed class QuoteProvider : Operational, IQuoteProvider
    {
        volatile ISourceDataDelivery _dataDelivery;

        DataSessionInfo _sessionInfo;

        Symbol _symbol;

        Quote? _currentQuote = null;
        /// <summary>
        /// 
        /// </summary>
        public Quote? CurrentQuote
        {
            get { return _currentQuote; }
        }

        public decimal? Ask
        {
            get
            {
                if (_currentQuote.HasValue)
                {
                    return _currentQuote.Value.Ask;
                }
                else
                {
                    return null;
                }
            }
        }

        public decimal? Bid
        {
            get
            {
                if (_currentQuote.HasValue)
                {
                    return _currentQuote.Value.Bid;
                }
                else
                {
                    return null;
                }
            }
        }

        public decimal? Spread
        {
            get
            {
                if (_currentQuote.HasValue)
                {
                    return _currentQuote.Value.Spread;
                }
                else
                {
                    return null;
                }
            }
        }

        DateTime? _lastQuoteTime = null;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastQuoteTime
        {
            get { return _lastQuoteTime; }
        }


        public DateTime? Time
        {
            get
            {
                if (_currentQuote.HasValue)
                {
                    return _currentQuote.Value.Time;
                }
                else
                {
                    return null;
                }
            }
        }

        [field:NonSerialized]
        public event QuoteProviderUpdateDelegate QuoteUpdateEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public QuoteProvider(DataSessionInfo sessionInfo)
        {// Make sure this dataDelivery keeper is in the same state as its dataDelivery delivery.
            _sessionInfo = sessionInfo;
            _symbol = _sessionInfo.Symbol;
        }

        public void SetInitialParameters(ISourceDataDelivery dataDelivery)
        {
            base.StatusSynchronizationSource = dataDelivery;
            _dataDelivery = dataDelivery;

            if (_dataDelivery != null)
            {
                _dataDelivery.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_dataDelivery_OperationalStatusChangedEvent);
                if (_dataDelivery.OperationalState == OperationalStateEnum.Operational)
                {
                    _dataDelivery_OperationalStatusChangedEvent(_dataDelivery, OperationalStateEnum.NotOperational);
                }

                _dataDelivery.QuoteUpdateEvent += new QuoteUpdateDelegate(_dataDelivery_QuoteUpdateEvent);
            }
        }

        void _dataDelivery_OperationalStatusChangedEvent(IOperational operational, OperationalStateEnum previousOperationState)
        {
            if (operational.OperationalState != OperationalStateEnum.Operational)
            {
                return;
            }

            // Re-map the session orderInfo.
            RuntimeDataSessionInformation information = _dataDelivery.GetSymbolRuntimeSessionInformation(_symbol);
            if (information == null)
            {
                SystemMonitor.OperationError("Failed to map session information for quote provider.");
                _sessionInfo = DataSessionInfo.Empty;
            }
            else
            {
                _sessionInfo = information.Info;

                if (_dataDelivery.SubscribeToData(_sessionInfo, true, new DataSubscriptionInfo(true, false, null)))
                {
                    RequestQuoteUpdate(false);
                }
            }

        }

        public void UnInitialize()
        {
            if (_dataDelivery != null)
            {
                _dataDelivery.QuoteUpdateEvent -= new QuoteUpdateDelegate(_dataDelivery_QuoteUpdateEvent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            _currentQuote = null;

            SetInitialParameters(_dataDelivery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool RequestQuoteUpdate(bool waitResult)
        {
            if (_dataDelivery != null)
            {
                return _dataDelivery.RequestQuoteUpdate(_sessionInfo, waitResult);
            }

            return false;
        }

        void _dataDelivery_QuoteUpdateEvent(ISourceDataDelivery dataDelivery, DataSessionInfo session, Quote? quote)
        {
            if (_sessionInfo.Equals(session) == false)
            {
                return;
            }

            lock (this)
            {
                _lastQuoteTime = DateTime.Now;
                _currentQuote = quote;
            }

            if (QuoteUpdateEvent != null)
            {
                QuoteUpdateEvent(this);
            }
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public decimal? GetOrderCloseQuote(bool isBuy)
        {
            return GetOrderCloseQuote(isBuy ? OrderTypeEnum.BUY_MARKET : OrderTypeEnum.SELL_MARKET);
        }

        /// <summary>
        /// Helper.
        /// </summary>
        public decimal? GetOrderCloseQuote(OrderTypeEnum orderType)
        {
            if (this.OperationalState != OperationalStateEnum.Operational
                || this.Ask.HasValue == false
                || this.Bid.HasValue == false)
            {
                return null;
            }

            if (OrderInfo.TypeIsBuy(orderType))
            {
                return this.Bid;
            }
            else
            {
                return this.Ask;
            }
        }

        /// <summary>
        /// Common helper method.
        /// </summary>
        public decimal? GetOrderOpenQuote(bool isBuy)
        {
            return GetOrderOpenQuote(isBuy ? OrderTypeEnum.BUY_MARKET : OrderTypeEnum.SELL_MARKET);
        }

        /// <summary>
        /// Common helper method, establish current placement price for order type.
        /// </summary>
        public decimal? GetOrderOpenQuote(OrderTypeEnum orderType)
        {
            if (this.OperationalState != OperationalStateEnum.Operational
                || this.Ask.HasValue == false
                || this.Bid.HasValue == false)
            {
                return null;
            }

            if (OrderInfo.TypeIsBuy(orderType))
            {
                return this.Ask;
            }
            else
            {
                return this.Bid;
            }
        }


    }
}
