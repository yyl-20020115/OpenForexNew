using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Hosts the indicators calculation results. An indicator typically has one or a few output 
    /// signalling sets of ouput values (lines).
    /// </summary>
    [Serializable]
    public class IndicatorResults : ISerializable
    {
        /// <summary>
        /// Values above this will cause automatic scale down, since otherwise they tend to overload the rendering engine.
        /// </summary>
        const int MaximumValueConstraint = 100000;

        /// <summary>
        /// When data is too big, we need to multiply down to fit into float range.
        /// </summary>
        volatile float _dynamicMultiplicator = 1;

        Dictionary<string, List<double>> _resultSets = new Dictionary<string, List<double>>();
        Dictionary<string, LinesChartSeries.ChartTypeEnum> _resultSetsChartTypes = new Dictionary<string, LinesChartSeries.ChartTypeEnum>();

        /// <summary>
        /// Thread safe access to names of output sets.
        /// </summary>
        public List<string> SetsNamesList
        {
            get { lock (this) { return GeneralHelper.EnumerableToList<string>(_resultSets.Keys); } }
        }

        /// <summary>
        /// Thread unsafe way of accessing output sets names.
        /// </summary>
        public IEnumerable<string> SetsNamesUnsafe
        {
            get { lock (this) { return _resultSets.Keys; } }
        }

        /// <summary>
        /// Values of a set, by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ReadOnlyCollection<double> this[int index]
        {
            get
            {
                lock (this)
                {
                    int i = 0;
                    foreach (string name in _resultSets.Keys)
                    {
                        if (i == index)
                        {
                            return _resultSets[name].AsReadOnly();
                        }
                    }
                }

                return null;
            }
        }
        
        /// <summary>
        /// Values of a set, by name.
        /// </summary>
        public ReadOnlyCollection<double> this[string name]
        {
            get
            {
                lock (this)
                {
                    return _resultSets[name].AsReadOnly();
                }
            }
        }

        /// <summary>
        /// How many result sets are in this container.
        /// </summary>
        public int SetsCount
        {
            get
            {
                lock (this)
                {
                    return _resultSets.Count;
                }
            }
        }

        /// <summary>
        /// What is the length of the sets.
        /// </summary>
        public int SetLength
        {
            get
            {
                lock (this)
                {
                    foreach (List<double> list in _resultSets.Values)
                    {
                        return list.Count;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public IndicatorResults(Indicator indicator, string[] resultSetNames)
        {
            foreach (string name in resultSetNames)
            {
                _resultSets[name] = new List<double>();
            }
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public IndicatorResults(SerializationInfo info, StreamingContext context)
        {
            string[] names = (string[])info.GetValue("resultSetsNames", typeof(string[]));
            foreach (string name in names)
            {
                _resultSets.Add(name, new List<double>());
            }

            _resultSetsChartTypes = (Dictionary<string, LinesChartSeries.ChartTypeEnum>)info.GetValue("resultSetsChartTypes", typeof(Dictionary<string, LinesChartSeries.ChartTypeEnum>));
        }

        /// <summary>
        /// Serialization routine.
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("resultSetsNames", GeneralHelper.EnumerableToArray<string>(_resultSets.Keys));
            info.AddValue("resultSetsChartTypes", _resultSetsChartTypes);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                foreach (string resultSet in _resultSets.Keys)
                {
                    _resultSets[resultSet].Clear();
                }
            }
        }

        /// <summary>
        /// Clip all results to maximum count.
        /// </summary>
        /// <param name="count"></param>
        public void ClipTo(int count)
        {
            lock (this)
            {
                foreach (string resultSet in _resultSets.Keys)
                {
                    if (_resultSets[resultSet].Count > count)
                    {
                        _resultSets[resultSet].RemoveRange(count, _resultSets[resultSet].Count - count);
                    }

                    _resultSets[resultSet].Clear();
                }
            }
        }

        ///// <summary>
        ///// Append the result piece to the named result set.
        ///// </summary>
        //public bool AppendSetValues(string name, double[] inputResultPiece)
        //{
        //    return AddSetValues(name, this[name].Count, inputResultPiece.Length, true, inputResultPiece);
        //}

        /// <summary>
        /// 
        /// </summary>
        //public bool UpdateSetLatestValues(string name, double[] inputResultPiece)
        //{
        //    return AddSetValues(name, Math.Max(0, this.SetLength - inputResultPiece.Length), inputResultPiece.Length, true, inputResultPiece);
        //}

        bool UpdateDynamicMultiplicator(ref double[] values1)
        {
            foreach (double value in values1)
            {
                if ((value * _dynamicMultiplicator) > MaximumValueConstraint)
                {
                    _dynamicMultiplicator *= 0.1f;
                    UpdateDynamicMultiplicator(ref values1);
                    return true;
                }
            }

            return false;
        }

        ///<summary>
        /// This used to handle results.
        /// inputResultPiece stores results, where 0 corresponds to startingIndex; the length of inputResultPiece may be larger than count.
        ///</summary>
        public bool AddSetValues(string name, int startingIndex, int count, bool overrideExistingValues, double[] inputResultPiece)
        {
            float valueMultiplicator = _dynamicMultiplicator;
            UpdateDynamicMultiplicator(ref inputResultPiece);
            
            if (valueMultiplicator != _dynamicMultiplicator)
            {// There was a change in the multiplicator, fix the exisitng values first.
                
                // This is what we need to apply to existing to take it to new level.
                float translationMultiplicator = _dynamicMultiplicator / valueMultiplicator;

                foreach (KeyValuePair<string, List<double>> pair in _resultSets)
                {
                    List<double> list = pair.Value;
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] *= translationMultiplicator;
                    }
                }
            }

            if (_dynamicMultiplicator != 1)
            {
                for (int i = 0; i < inputResultPiece.Length; i++)
			    {
			        inputResultPiece[i] *= _dynamicMultiplicator;
    			}
            }

            lock (this)
            {
                if (_resultSets.ContainsKey(name) == false)
                {
                    SystemMonitor.Error("SetResultSetValues result set [" + name + "] not found.");
                    return false;
                }

                List<double> resultSet = _resultSets[name];

                for (int i = resultSet.Count; i < startingIndex; i++)
                {// Only if there are some empty spaces before the start, fill them with no value.
                    resultSet.Add(double.NaN);
                }

                // Get the dataDelivery from the result it is provided to us.
                for (int i = startingIndex; i < startingIndex + count; i++)
                {
                    if (resultSet.Count <= i)
                    {
                        resultSet.Add(inputResultPiece[i - startingIndex]);
                    }
                    else
                    {
                        if (overrideExistingValues)
                        {
                            resultSet[i] = inputResultPiece[i - startingIndex];
                        }
                    }
                }

            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public double? GetValueSetCurrentValue(int setIndex)
        {
            ReadOnlyCollection<double> set = this[setIndex];
            if (set == null || set.Count == 0)
            {
                return null;
            }
            return set[set.Count - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        public double? GetValueSetCurrentValue(string setName)
        {
            ReadOnlyCollection<double> set = this[setName];
            if (set == null)
            {
                return null;
            }
            return set[set.Count - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetResultSetChartType(string setName, LinesChartSeries.ChartTypeEnum chartType)
        {
            _resultSetsChartTypes[setName] = chartType;
        }

        /// <summary>
        /// Obtain the chart type for this result set.
        /// </summary>
        public LinesChartSeries.ChartTypeEnum? GetResultSetChartType(string setName)
        {
            if (_resultSetsChartTypes.ContainsKey(setName))
            {
                return _resultSetsChartTypes[setName];
            }

            return null;
        }

    }
}



