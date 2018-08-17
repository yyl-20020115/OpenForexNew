using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using CommonSupport;
using TicTacTec.TA.Library;
using CommonFinancial;

namespace ForexPlatform
{
    /// <summary>
    /// Manages reflection based creation of all the available indicator types (taken from TALIB, those with code etc.)
    /// Singleton thread safe implementation of Indicator Manager class.
    /// The manager implements the "Prototype Design Pattern" for the managemen of indicators.
    /// </summary>
    public sealed class IndicatorFactory
    {
        #region Singleton Implementation

        static readonly IndicatorFactory _instance = new IndicatorFactory();
        public static IndicatorFactory Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Explicit static constructor to tell C# compiler, not to mark type as beforefieldinit
        /// </summary>
        static IndicatorFactory()
        {
        }

        #endregion

        Dictionary<IndicatorGroup, Dictionary<string, PlatformIndicator>> _indicatorsGroups = new Dictionary<IndicatorGroup, Dictionary<string, PlatformIndicator>>();

        public enum IndicatorGroup
        {
            // Custom indicators implementations, inside the platform.
            Custom,
            // Ta lib technical indicators.
            TaLib,
            // Ta lib candle stick formartions indication.
            TaLibCandleStickFormation
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private IndicatorFactory()
        {
            // LoadFromFile custom indicators.
            InitializeCustomPlatformIndicators();

            // Init ta lib indicators.
            InitializeTaLibIndicators();
        }

        private void InitializeTaLibIndicators()
        {
            // Start init for ta lib indicators.
            Dictionary<string, MethodInfo> methodsVerified = new Dictionary<string, MethodInfo>();
            lock (this)
            {
                // Setup the unstable period; unstable periods usually stretch to the start, however 
                // we can approximate to 120 bars; this will extend the lookback of the unstable functions 120.
                // see here: http://www.tadoc.org/forum/index.php?topic=858.0
                TicTacTec.TA.Library.Core.SetUnstablePeriod(TicTacTec.TA.Library.Core.FuncUnstId.FuncUnstAll, 120);

                MethodInfo[] methods = typeof(TicTacTec.TA.Library.Core).GetMethods();

                foreach (MethodInfo methodInfo in methods)
                {
                    if (methodInfo.ReturnType == typeof(Core.RetCode))
                    {
                        if (methodsVerified.ContainsKey(methodInfo.Name))
                        {// Establish the double[] input baseMethod of the two and use that.

                            ParameterInfo[] parameters = methodsVerified[methodInfo.Name].GetParameters();
                            ParameterInfo[] newParameters = methodInfo.GetParameters();

                            SystemMonitor.CheckThrow(parameters.Length == newParameters.Length);

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (newParameters[i].ParameterType == typeof(Single[])
                                    && parameters[i].ParameterType == typeof(Double[]))
                                {// Current is double version of the function, new is single, stop parameter check.
                                    break;
                                }

                                if (newParameters[i].ParameterType == typeof(Double[])
                                    && parameters[i].ParameterType == typeof(Single[]))
                                {// Current is single version of the function, new is double, swap and stop parameter check.
                                    methodsVerified[methodInfo.Name] = methodInfo;
                                    break;
                                }
                            }

                            
                        }
                        else
                        {
                            methodsVerified.Add(methodInfo.Name, methodInfo);
                        }

                    }
                }
            }

            _indicatorsGroups.Add(IndicatorGroup.TaLib, new Dictionary<string, PlatformIndicator>());

            // List here dataDelivery for all the indicators that are to be imported from TaLIB.

            #region Group TALin

            InitializeTaLibIndicator(methodsVerified["Acos"], "Vector Trigonometric ACos", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Ad"], "Chaikin A/D Line", null, null, null, null, null, null);

            InitializeTaLibIndicator(methodsVerified["Add"], "Vector Arithmetic Add", null, null, null, null, DataBar.DataValueEnum.Open, DataBar.DataValueEnum.Close);

            InitializeTaLibIndicator(methodsVerified["AdOsc"], "Chaikin A/D Oscillator", null, null, null, null, null, null, 10, 20);

            InitializeTaLibIndicator(methodsVerified["Adx"], "Average Directional Movement Index", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Adxr"], "Average Directional Movement Index Rating", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Apo"], "Absolute Price Oscillator", null, null, null, null, DataBar.DataValueEnum.Close, null, 10, 20, Core.MAType.Ema);

            InitializeTaLibIndicator(methodsVerified["Aroon"], "Aroon", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["AroonOsc"], "Aroon Oscillator", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Asin"], "Vector Trigonometric ASin", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Atan"], "Vector Trigonometric ATan", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Atr"], "Average True Range", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["AvgPrice"], "Average Price", null, null, null, null, null, null);

            InitializeTaLibIndicator(methodsVerified["Bbands"], "Bollinger Bands", null, null, null, null, DataBar.DataValueEnum.Close, null, 14, 10d, 10d, Core.MAType.Ema);

            InitializeTaLibIndicator(methodsVerified["Beta"], "Beta", null, null, null, null, DataBar.DataValueEnum.Open, DataBar.DataValueEnum.Close, 14);

            InitializeTaLibIndicator(methodsVerified["Bop"], "Balance Of Power", null, null, null, null, null, null);

            InitializeTaLibIndicator(methodsVerified["Cci"], "Commodity Channel Index", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Ceil"], "Vector Ceil", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Cmo"], "Chande Momentum Oscillator", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["Correl"], "Pearson's Correlation Coefficient (r)", null, null, null, null, DataBar.DataValueEnum.Open, DataBar.DataValueEnum.Close, 14);

            InitializeTaLibIndicator(methodsVerified["Cos"], "Vector Trigonometric Cos", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Cosh"], "Vector Trigonometric Cosh", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Dema"], "Double Exponential Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["Div"], "Vector Arithmetic Div", null, null, null, null, DataBar.DataValueEnum.Open, DataBar.DataValueEnum.Close);

            InitializeTaLibIndicator(methodsVerified["Dx"], "Directional Movement Index", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Ema"], "Exponential Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["Exp"], "Vector Arithmetic Exp", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Floor"], "Vector Floor", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["HtDcPeriod"], "Hilbert Transform - Dominant Cycle Period", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["HtDcPhase"], "Hilbert Transform - Dominant Cycle Phase", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["HtPhasor"], "Hilbert Transform - Phasor Components", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["HtSine"], "Hilbert Transform - SineWave", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["HtTrendline"], "Hilbert Transform - Instantaneous Trendline", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["HtTrendMode"], "Hilbert Transform - Trend vs Cycle Mode", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Kama"], "Kaufman Adaptive Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["LinearReg"], "Linear Regression", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["LinearRegAngle"], "Linear Regression Angle", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["LinearRegIntercept"], "Linear Regression Intercept", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["LinearRegSlope"], "Linear Regression Slope", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["Ln"], "Vector Log Natural", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Log10"], "Vector Log10", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Macd"], "Moving Average Convergence/Divergence", null, null, null, null, DataBar.DataValueEnum.Close, null, 10, 20, 12);

            InitializeTaLibIndicator(methodsVerified["MacdExt"], "MACD with controllable MA type", null, null, null, null, DataBar.DataValueEnum.Close, null, 10, Core.MAType.Ema, 20, Core.MAType.Ema, 12, Core.MAType.Ema);

            InitializeTaLibIndicator(methodsVerified["MacdFix"], "Moving Average Convergence/Divergence Fix 12/26", null, null, null, null, DataBar.DataValueEnum.Close, null, 10);

            InitializeTaLibIndicator(methodsVerified["Mama"], "MESA Adaptive Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 10d, 20d);

            InitializeTaLibIndicator(methodsVerified["Max"], "Highest value over a specified period", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["MaxIndex"], "Index of highest value over a specified period", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["MedPrice"], "Median Price", null, null, null, null, null, null);

            InitializeTaLibIndicator(methodsVerified["Mfi"], "Money Flow Index", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["MidPoint"], "MidPoint over period", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["MidPrice"], "Midpoint Price over period", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Min"], "Lowest value over a specified period", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["MinIndex"], "Index of lowest value over a specified period", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["MinMax"], "Lowest and highest values over a specified period", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["MinMaxIndex"], "Indexes of lowest and highest values over a specified period", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["MinusDI"], "Minus Directional Indicator", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["MinusDM"], "Minus Directional Movement", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Mom"], "Momentum", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["MovingAverage"], "Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 14, Core.MAType.Sma);

            // Consumes double[] inPeriods, which is to be established programatically only.
            //InitializeTaLibIndicator(methodsVerified["MovingAverageVariablePeriod"], "Moving Average with variable period", null, null, 2, 20, Core.MAType.Ema);

            InitializeTaLibIndicator(methodsVerified["Mult"], "Vector Arithmetic Mult", null, null, null, null, DataBar.DataValueEnum.Open, DataBar.DataValueEnum.Close);

            InitializeTaLibIndicator(methodsVerified["Natr"], "Normalized Average True Range", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Obv"], "On Balance Volume", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["PlusDI"], "Plus Directional Indicator", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["PlusDM"], "Plus Directional Movement", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Ppo"], "Percentage Price Oscillator", null, null, null, null, DataBar.DataValueEnum.Close, null, 12, 24, Core.MAType.Ema);

            InitializeTaLibIndicator(methodsVerified["Roc"], "Rate of change : ((price/prevPrice)-1)*100", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["RocP"], "Rate of change Percentage: (price-prevPrice)/prevPrice", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["RocR"], "Rate of change ratio: (price/prevPrice)", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["RocR100"], "Rate of change ratio 100 scale: (price/prevPrice)*100", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            Indicator indicator = InitializeTaLibIndicator(methodsVerified["Rsi"], "Relative Strength Index", null, null, 0, 100, DataBar.DataValueEnum.Close, null, 14);
            indicator.Parameters.SetDynamic(PlatformIndicatorChartSeries.FixedLinePrefix + "High", 70);
            indicator.Parameters.SetDynamic(PlatformIndicatorChartSeries.FixedLinePrefix + "Low", 30);

            InitializeTaLibIndicator(methodsVerified["Sar"], "Parabolic SAR", null, null, null, null, null, null, 1d, 2d);

            InitializeTaLibIndicator(methodsVerified["SarExt"], "Parabolic SAR - Extended", null, null, null, null, null, null, 1d, 2d, 3d, 4d, 5d, 6d, 7d, 8d);

            InitializeTaLibIndicator(methodsVerified["Sin"], "Vector Trigonometric Sin", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Sinh"], "Vector Trigonometric Sinh", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Sma"], "Simple Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["Sqrt"], "Vector Square Root", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["StdDev"], "Standard Deviation", null, null, null, null, DataBar.DataValueEnum.Close, null, 14, 2d);

            InitializeTaLibIndicator(methodsVerified["Stoch"], "Stochastic", null, null, null, null, null, null, 10, 20, Core.MAType.Ema, 22, Core.MAType.Ema);

            InitializeTaLibIndicator(methodsVerified["StochF"], "Stochastic Fast", null, null, null, null, null, null, 10, 20, Core.MAType.Ema);

            InitializeTaLibIndicator(methodsVerified["StochRsi"], "Stochastic Relative Strength Index", null, null, null, null, DataBar.DataValueEnum.Close, null, 14, 10, 20, Core.MAType.Ema);

            InitializeTaLibIndicator(methodsVerified["Sub"], "Vector Arithmetic Substraction", null, null, null, null, DataBar.DataValueEnum.Open, DataBar.DataValueEnum.Close);

            InitializeTaLibIndicator(methodsVerified["Sum"], "Summation", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["T3"], "Triple Exponential Moving Average (T3)", null, null, null, null, DataBar.DataValueEnum.Close, null, 14, 2d);

            InitializeTaLibIndicator(methodsVerified["Tan"], "Vector Trigonometric Tan", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Tanh"], "Vector Trigonometric Tanh", null, null, null, null, DataBar.DataValueEnum.Close, null);

            InitializeTaLibIndicator(methodsVerified["Tema"], "Triple Exponential Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["Trima"], "Triangular Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["Trix"], "1-day Rate-Of-Change (ROC) of a Triple Smooth EMA", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["TrueRange"], "True Range", null, null, null, null, null, null);

            InitializeTaLibIndicator(methodsVerified["Tsf"], "Time Series Forecast", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            InitializeTaLibIndicator(methodsVerified["TypPrice"], "Typical Price", null, null, null, null, null, null);

            InitializeTaLibIndicator(methodsVerified["UltOsc"], "Ultimate Oscillator", null, null, null, null, null, null, 12, 14, 16);

            InitializeTaLibIndicator(methodsVerified["Variance"], "Variance", null, null, null, null, DataBar.DataValueEnum.Close, null, 14, 2d);

            InitializeTaLibIndicator(methodsVerified["WclPrice"], "Weighted Close Price", null, null, null, null, null, null);

            InitializeTaLibIndicator(methodsVerified["WillR"], "Williams' %R", null, null, null, null, null, null, 14);

            InitializeTaLibIndicator(methodsVerified["Wma"], "Weighted Moving Average", null, null, null, null, DataBar.DataValueEnum.Close, null, 14);

            #endregion
        }

        /// <summary>
        /// Helper, loads custom indicators.
        /// </summary>
        void InitializeCustomPlatformIndicators()
        {
            foreach(Assembly assembly in ReflectionHelper.GetApplicationEntryAssemblyReferencedAssemblies())
            {
                CollectCustomIndicatorsFromAssembly(assembly);
            }
        }

        /// <summary>
        /// Will load all the indicators from the assembly.
        /// </summary>
        /// <param name="assembly"></param>
        public void CollectCustomIndicatorsFromAssembly(Assembly assembly)
        {
            List<Type> indicatorTypes = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(typeof(PlatformIndicator),
                true, false, new Assembly[] { assembly }, new Type[] { });

            lock (this)
            {
                if (_indicatorsGroups.ContainsKey(IndicatorGroup.Custom) == false)
                {
                    _indicatorsGroups.Add(IndicatorGroup.Custom, new Dictionary<string, PlatformIndicator>());
                }

                foreach (Type type in indicatorTypes)
                {// Extract user friendly names.

                    string name = type.Name;
                    if (UserFriendlyNameAttribute.GetTypeAttributeValue(type, ref name))
                    {
                        name = type.Name + ", " + name;
                    }

                    if (type.GetConstructor(new Type[] { }) == null)
                    {// Can not create indicator since it needs a parameterless constructor.
                        continue;
                    }

                    PlatformIndicator indicator = (PlatformIndicator)Activator.CreateInstance(type);
                    _indicatorsGroups[IndicatorGroup.Custom][name] = indicator;
                }
            }
        }

        /// <summary>
        /// Helper, initialize a single TaLib indicator.
        /// </summary>
        Indicator InitializeTaLibIndicator(MethodInfo methodInfo, string indicatorName, bool? isTradeable, 
            bool? isScaledToQuotes, float? rangeMinimum, float? rangeMaximum, DataBar.DataValueEnum? inRealSource, DataBar.DataValueEnum? inReal1Source, params object[] inputParameters)
        {
            lock (this)
            {
                MethodInfo lookBackCountMethodInfo = null;
                foreach (MethodInfo info in typeof(TicTacTec.TA.Library.Core).GetMethods())
                {
                    if (info.Name == methodInfo.Name + "Lookback")
                    {
                        lookBackCountMethodInfo = info;
                        break;
                    }
                }

                if (lookBackCountMethodInfo == null)
                {
                    SystemMonitor.Error("Failed to find indicator [" + methodInfo.Name + "] look back method.");
                    return null;
                }

                GenericTALibIndicator indicator = GenericTALibIndicator.CreateInstance(methodInfo, lookBackCountMethodInfo, indicatorName, isTradeable, isScaledToQuotes);

                if (indicator == null)
                {
                    SystemMonitor.Error("Creating indicator [" + indicator.Name + "] failed.");
                    return null;
                }

                string[] names = new string[inputParameters.Length];
                for (int i = 0; i < inputParameters.Length; i++)
			    {
                    names[i] = "Parameter." + inputParameters[i].GetType().Name + "." + i.ToString();
			    }
                
                SystemMonitor.CheckThrow(indicator.SetInitialParameters(inputParameters, names));

                _indicatorsGroups[IndicatorGroup.TaLib].Add(indicator.Name, indicator);

                indicator.RealInputArraySource = inRealSource;
                indicator.Real1InputArraySource = inReal1Source;

                indicator.RangeMinimum = rangeMinimum;
                indicator.RangeMaximum = rangeMaximum;

                return indicator;
            }
        }



        public string[] GetIndicatorsDescriptions(IndicatorGroup indicatorsGroup)
        {
            lock (this)
            {
                if (_indicatorsGroups.ContainsKey(indicatorsGroup) == false)
                {
                    return new string[] { };
                }

                string[] result = new string[_indicatorsGroups[indicatorsGroup].Count];
                int i = 0;
                foreach(string name in _indicatorsGroups[indicatorsGroup].Keys)
                {
                    result[i] = _indicatorsGroups[indicatorsGroup][name].Description;
                    i++;
                }
                return result;
            }
        }
        
        public string[] GetIndicatorsNames(IndicatorGroup indicatorsGroup)
        {
            lock (this)
            {
                if (_indicatorsGroups.ContainsKey(indicatorsGroup))
                {
                    return GeneralHelper.EnumerableToArray<string>(_indicatorsGroups[indicatorsGroup].Keys);
                }
            }

            return new string [] { };
        }

        /// <summary>
        /// Search indicator in all groups.
        /// </summary>
        public PlatformIndicator GetIndicatorCloneByName(string name)
        {
            lock (this)
            {
                foreach (IndicatorGroup group in _indicatorsGroups.Keys)
                {
                    // Case insensitive search.
                    foreach (string indicatorName in _indicatorsGroups[group].Keys)
                    {
                        if (name.ToLower() == indicatorName.ToLower())
                        {
                            return ((PlatformIndicator)_indicatorsGroups[group][indicatorName]).SimpleClone();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public PlatformIndicator GetIndicatorCloneByName(IndicatorGroup indicatorGroup, string name)
        {
            lock (this)
            {
                if (_indicatorsGroups[indicatorGroup].ContainsKey(name))
                {
                    return ((PlatformIndicator)_indicatorsGroups[indicatorGroup][name]).SimpleClone();
                }
            }

            return null;
        }
    }
}
