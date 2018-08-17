using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CommonSupport;
using CommonFinancial;
using System.Runtime.Serialization;

namespace ForexPlatform
{
    /// <summary>
    /// Platform indicator child class, it takes care of importing all the TALib indicators into the platform.
    /// An instance of this class can have any underlying type of indicator from the TaLib library.
    /// </summary>
    [Serializable]
    public class GenericTALibIndicator : PlatformIndicator
    {
        /// <summary>
        /// Dynamically collected using reflection.
        /// </summary>
        List<ParameterInfo> _inputDefaultArrayParameters = new List<ParameterInfo>();

        /// <summary>
        /// Dynamically collected using reflection.
        /// </summary>
        List<ParameterInfo> _intputParameters = new List<ParameterInfo>();

        /// <summary>
        /// Dynamically collected using reflection.
        /// </summary>
        List<ParameterInfo> _outputArraysParameters = new List<ParameterInfo>();

        MethodInfo _methodInfo;
        MethodInfo _lookbackMethodInfo;

        /// <summary>
        /// Default null means indicator does not use it.
        /// </summary>
        DataBar.DataValueEnum? _realInputArraySource = null;
        /// <summary>
        /// Used to fill the double[] realIn (or real0In) array if the indicator requires it.
        /// </summary>
        public DataBar.DataValueEnum? RealInputArraySource
        {
            get { lock (this) { return _realInputArraySource; } }
            set { lock (this) { _realInputArraySource = value; } }
        }

        /// <summary>
        /// Default null means indicator does not use it.
        /// </summary>
        DataBar.DataValueEnum? _real1InputArraySource = null;
        /// <summary>
        /// Used to fill the double[] real1In array if the indicator requires it.
        /// </summary>
        public DataBar.DataValueEnum? Real1InputArraySource
        {
            get { lock (this) { return _real1InputArraySource; } }
            set { lock (this) { _real1InputArraySource = value; } }
        }

        int? _lookbackCount = null;

        /// <summary>
        /// Private constructor, the class instances must be created trough the static baseMethod CreateInstance().
        /// </summary>
        GenericTALibIndicator(string name, string description, bool? isTradeable, bool? isScaledToQuotes, string[] resultSetNames)
            : base(name, isTradeable, isScaledToQuotes, resultSetNames)
        {
            _description = description;
        }

        /// <summary>
        /// Initialization.
        /// </summary>
        public bool SetInitialParameters(object[] values, string[] names)
        {
            lock (this)
            {
                if (_intputParameters.Count != values.Length)
                {
                    return false;
                }

                for (int i = 0; i < values.Length; i++)
                {
                    if (_intputParameters[i].ParameterType != values[i].GetType())
                    {
                        return false;
                    }

                    names[i] = _intputParameters[i].Name;
                }

                Parameters.SetCore(names, values);
            }
            return true;
        }

        /// <summary>
        /// Static construction routine; needed since the way TaLib indicators are constructed is dynamically.
        /// </summary>
        public static GenericTALibIndicator CreateInstance(MethodInfo methodInfo, MethodInfo lookbackMethodInfo, string description, bool? isTradeable, bool? isScaledToQuotes)
        {
            if (methodInfo == null)
            {
                return null;
            }

            Type returnType = methodInfo.ReturnType;

            if (returnType != typeof(TicTacTec.TA.Library.Core.RetCode))
            {
                return null;
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length < 5)
            {
                return null;
            }

            int index = 0;

            if (parameters[index].ParameterType != typeof(int) ||
                parameters[index].Name != "startIdx")
            {
                return null;
            }
            index++;

            if (parameters[index].ParameterType != typeof(int) ||
                parameters[index].Name != "endIdx")
            {
                return null;
            }
            index++;

            List<ParameterInfo> indicatorParameters = new List<ParameterInfo>();
            while (parameters.Length > index && parameters[index].ParameterType == typeof(double[]))
            {
                if (parameters[index].Name != "inReal" &&
                    parameters[index].Name != "inReal0" &&
                    parameters[index].Name != "inReal1" &&
                    parameters[index].Name != "inHigh" &&
                    parameters[index].Name != "inLow" &&
                    parameters[index].Name != "inOpen" &&
                    parameters[index].Name != "inClose" && 
                    parameters[index].Name != "inVolume"
                    )
                {
                    return null;
                }

                indicatorParameters.Add(parameters[index]);
                index++;
            }

            // optIn parameters
            List<ParameterInfo> indicatorInputParameters = new List<ParameterInfo>();
            while(parameters.Length > index && parameters[index].Name.StartsWith("optIn"))
            {
                if (parameters[index].ParameterType == typeof(int) ||
                    parameters[index].ParameterType != typeof(double) ||
                    parameters[index].ParameterType != typeof(TicTacTec.TA.Library.Core.MAType))
                {
                    indicatorInputParameters.Add(parameters[index]);
                }
                else
                {// Invalid type.
                    return null;
                }
                index++;
            }

            if (parameters.Length <= index || parameters[index].IsOut == false
                || parameters[index].Name != "outBegIdx")
            {
                return null;
            }

            index++;

            if (parameters.Length <= index || parameters[index].IsOut == false
                || parameters[index].Name != "outNBElement")
            {
                return null;
            }

            index++;

            List<ParameterInfo> indicatorOutputArrayParameters = new List<ParameterInfo>();
            List<string> indicatorOutputArrayParametersNames = new List<string>();
            while (parameters.Length > index)
            {
                if (parameters[index].Name.StartsWith("out") == false)
                {
                    return null;
                }

                if (parameters[index].ParameterType == typeof(double[])
                    || parameters[index].ParameterType == typeof(int[]))
                {
                    indicatorOutputArrayParametersNames.Add(parameters[index].Name);
                    indicatorOutputArrayParameters.Add(parameters[index]);
                }
                else 
                {
                    return null;
                }
                
                index++;
            }

            if (parameters.Length != index)
            {// Parameters left unknown.
                return null;
            }

            GenericTALibIndicator indicator = new GenericTALibIndicator(methodInfo.Name, description, isTradeable, isScaledToQuotes, indicatorOutputArrayParametersNames.ToArray());
            indicator._inputDefaultArrayParameters.AddRange(indicatorParameters);
            indicator._outputArraysParameters.AddRange(indicatorOutputArrayParameters);
            indicator._intputParameters.AddRange(indicatorInputParameters);
            indicator._methodInfo = methodInfo;
            indicator._lookbackMethodInfo = lookbackMethodInfo;

            return indicator;
        }

        /// <summary>
        /// Helper, calculate the lookback, it needs to be done every time as it depends on the parameters as well.
        /// The look back is how many history bars back does the indicator need to establish the current value.
        /// </summary>
        protected void EstablishLookback()
        {
            if (_lookbackCount.HasValue == false)
            {
                lock (this)
                {
                    _lookbackCount = (int)_lookbackMethodInfo.Invoke(null, _parameters.CoreValues.ToArray());
                }
            }
        }

        /// <summary>
        /// Obtain an array of input values, based in type of array name required from TaLib.
        /// </summary>
        double[] GetInputArrayValues(string valueArrayTypeName, int startingIndex, int indexCount)
        {
            IDataBarHistoryProvider provider = DataProvider;
            if (provider == null)
            {
                return null;
            }

            double[] result = null;
            if (valueArrayTypeName == "inLow")
            {
                result = provider.GetValuesAsDouble(DataBar.DataValueEnum.Low, startingIndex, indexCount);
            }
            else if (valueArrayTypeName == "inHigh")
            {
                result = provider.GetValuesAsDouble(DataBar.DataValueEnum.High, startingIndex, indexCount);
            }
            else if (valueArrayTypeName == "inOpen")
            {
                result = provider.GetValuesAsDouble(DataBar.DataValueEnum.Open, startingIndex, indexCount);
            }
            else if (valueArrayTypeName == "inClose")
            {
                result = provider.GetValuesAsDouble(DataBar.DataValueEnum.Close, startingIndex, indexCount);
            }
            else if (valueArrayTypeName == "inVolume")
            {
                result = provider.GetValuesAsDouble(DataBar.DataValueEnum.Volume, startingIndex, indexCount);
            }
            else if (valueArrayTypeName == "inReal")
            {
                if (_realInputArraySource.HasValue)
                {
                    result = provider.GetValuesAsDouble(_realInputArraySource.Value, startingIndex, indexCount);
                }
                else
                {
                    SystemMonitor.Throw("inReal parameter not assigned.");
                }
            }
            else if (valueArrayTypeName == "inReal0")
            {
                if (_realInputArraySource.HasValue)
                {
                    result = provider.GetValuesAsDouble(_realInputArraySource.Value, startingIndex, indexCount);
                }
                else
                {
                    SystemMonitor.Throw("inReal parameter not assigned.");
                }
            }
            else if (valueArrayTypeName == "inReal1")
            {
                if (_real1InputArraySource.HasValue)
                {
                    result = provider.GetValuesAsDouble(_real1InputArraySource.Value, startingIndex, indexCount);
                }
                else
                {
                    SystemMonitor.Throw("inReal parameter not assigned.");
                }
            }
            else
            {
                SystemMonitor.Throw("Class operation logic error - array type name unknown.");
            }

            return result;
        }

        // Parameters format of TaLibCore functions.
        //int startIdx, - mandatory
        //int endIdx, - mandatory
        //double[] inReal[added 0/1] or/and inOpen or/and inLow or/and inHigh or/and inClose
        //int/double optIn[NAME] or/and another one or none - parameters

        //out int outBegIdx, 
        //out int outNBElement, 
        //double/int[] out[Real/Integer] and or another one

        // Example:
        //TicTacTec.TA.Library.Core.RetCode code = TicTacTec.TA.Library.Core.Sma(0, indecesCount - 1, _closeResultValues, Period, out beginIndex, out number, ma);        }

        /// <summary>
        /// Actual calculation routine.
        /// </summary>
        protected override void OnCalculate(bool fullRecalculation, DataBarUpdateType? updateType)
        {
            // Format of a TA baseMethod.
            //int startIdx, - mandatory
            //int endIdx, - mandatory
            //double[] inReal[added 0/1] or/and inOpen or/and inLow or/and inHigh or/and inClose
            //int/double optIn[NAME] or/and another one or none - parameters

            //out int outBegIdx, 
            //out int outNBElement, 
            //double/int[] out[Real/Integer] and or another one

            // Example:
            //TicTacTec.TA.Library.Core.RetCode code = TicTacTec.TA.Library.Core.Sma(0, indecesCount - 1, _closeResultValues, Period, out beginIndex, out number, ma);        }

            EstablishLookback();

            // Prepare parameters.
            int dataProviderStartIndex = 0;
            
            if (fullRecalculation == false)
            {
                dataProviderStartIndex = Math.Max(0, Results.SetLength - _lookbackCount.Value - 1);
            }

            int startIndex = 0;
            int endIndex = DataProvider.BarCount - dataProviderStartIndex - 1;

            //if (StepByStepCalculation)
            //{
            //    int outBeginIdxPosition;
            //    object[] parameters = SetupParameters(startIndex, endIndex, dataProviderStartIndex, out outBeginIdxPosition);
            //    for (int i = 0; i < DataProvider.BarCount - Results.SetLength; i++)
            //    {
            //        parameters[0] = i;
            //        parameters[1] = endIndex - i;
            //        Calc(parameters, outBeginIdxPosition, dataProviderStartIndex);
            //    }
            //}

            int outBeginIdxPosition;
            object[] parameters = SetupParameters(startIndex, endIndex, dataProviderStartIndex, out outBeginIdxPosition);
            PerformCalculation(parameters, outBeginIdxPosition, dataProviderStartIndex);
        }

        /// <summary>
        /// Helper, prepares the parameters in format to be passed on to TaLib.
        /// </summary>
        object[] SetupParameters(int startIndex, int endIndex, int dataProviderStartIndex, out int beginIdxPosition)
        {
            beginIdxPosition = 0;

            int indexCount = endIndex - startIndex + 1;

            // Consider the operationResult returned.
            List<object> parameters = new List<object>();

            parameters.Add(startIndex);
            parameters.Add(endIndex);

            lock (this)
            {
                foreach (ParameterInfo info in _inputDefaultArrayParameters)
                {
                    parameters.Add(GetInputArrayValues(info.Name, dataProviderStartIndex, indexCount));
                }

                foreach (object parameter in _parameters.CoreValues)
                {
                    parameters.Add(parameter);
                }

                beginIdxPosition = parameters.Count;

                // outBeginIdx
                parameters.Add(0);

                // outNBElemenet
                parameters.Add(0);

                foreach (ParameterInfo info in _outputArraysParameters)
                {
                    if (info.ParameterType == typeof(double[]))
                    {// Passed arrays must be prepared to the proper size.
                        double[] array = new double[indexCount];
                        parameters.Add(array);
                    }
                    else if (info.ParameterType == typeof(int[]))
                    {// Passed arrays must be prepared to the proper size.
                        int[] array = new int[indexCount];
                        parameters.Add(array);
                    }
                    else
                    {
                        SystemMonitor.Throw("Class operation logic error, type not supported.");
                    }
                }

            }

            return parameters.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void PerformCalculation(object[] parametersArray, int beginIdxPosition, int actualStartingIndex)
        {
            // This is how the normal call looks like:
            //    TicTacTec.TA.Library.Core.Adx((int)parameters[0], (int)parameters[1], (double[])parameters[2],
            //    (double[])parameters[3], (double[])parameters[4], (int)parameters[5],
            //    out outBeginIdx, out outNBElemenet, (double[])parameters[8]);

            TicTacTec.TA.Library.Core.RetCode code = (TicTacTec.TA.Library.Core.RetCode)
                _methodInfo.Invoke(null, parametersArray);

            int outBeginIdx = (int)parametersArray[beginIdxPosition];
            int outNBElement = (int)parametersArray[beginIdxPosition + 1];

            lock (this)
            {
                for (int i = 0; outNBElement > 0 && i < _outputArraysParameters.Count; i++)
                {
                    int index = beginIdxPosition + 2 + i;
                    if (parametersArray[index].GetType() == typeof(double[]))
                    {// Double output.
                        Results.AddSetValues(_outputArraysParameters[i].Name, actualStartingIndex + outBeginIdx, outNBElement, true, (double[])parametersArray[index]);
                    }
                    else if (parametersArray[index].GetType() == typeof(int[]))
                    {// Int output.
                        Results.AddSetValues(_outputArraysParameters[i].Name, actualStartingIndex + outBeginIdx, outNBElement, true, GeneralHelper.IntsToDoubles((int[])parametersArray[index]));
                    }
                }

            }
        }

        /// <summary>
        /// Does not include results.
        /// </summary>
        /// <returns></returns>
        public override PlatformIndicator OnSimpleClone()
        {
            TracerHelper.Trace((this.DataProvider != null).ToString());

            GenericTALibIndicator newIndicator = CreateInstance(_methodInfo, _lookbackMethodInfo, Description, Tradeable, ScaledToQuotes);
            newIndicator._inputDefaultArrayParameters = new List<ParameterInfo>(_inputDefaultArrayParameters);
            newIndicator._intputParameters = new List<ParameterInfo>(_intputParameters);
            newIndicator._outputArraysParameters = new List<ParameterInfo>(_outputArraysParameters);
            newIndicator._parameters = (IndicatorParameters)_parameters.Clone();

            newIndicator._realInputArraySource = _realInputArraySource;
            newIndicator._real1InputArraySource = _real1InputArraySource;

            return newIndicator;
        }


    }
}
