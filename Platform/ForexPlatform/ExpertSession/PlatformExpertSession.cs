using System;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatform
{
    /// <summary>
    /// Child of the ExpertSession, purpose is to perform platform specific actions in relation to the
    /// operation of the expert sessionInformation (like register/unregister components to the Arbiter).
    /// </summary>
    [Serializable]
    public class PlatformExpertSession : ExpertSession
    {
        ProviderTradeChartSeries _mainChartSeries;
        /// <summary>
        /// UI.
        /// </summary>
        public ProviderTradeChartSeries MainChartSeries
        {
            get { return _mainChartSeries; }
        }

        SerializationInfoEx _chartControlPersistence = null;
        /// <summary>
        /// UI.
        /// </summary>
        public SerializationInfoEx ChartControlPersistence
        {
            get { return _chartControlPersistence; }
            set { _chartControlPersistence = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        public PlatformExpertSession(DataSessionInfo sessionInfo)
            : base(sessionInfo)
        {
            _mainChartSeries = new ProviderTradeChartSeries(sessionInfo.Name);
            _mainChartSeries.ChartType = TradeChartSeries.ChartTypeEnum.CandleStick;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Initialize(DataSessionInfo? sessionInfo)
        {
            if (base.Initialize(sessionInfo) == false)
            {
                return false;
            }

            _mainChartSeries.Initialize(DataProvider, OrderExecutionProvider);

            return true;
        }

        /// <summary>
        /// Bringing down sessionInformation.
        /// </summary>
        /// <param name="arbiter"></param>
        public override void UnInitialize()
        {
            base.UnInitialize();
        }
    }
}
