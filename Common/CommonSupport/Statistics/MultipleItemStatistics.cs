using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// A set of item statistics.
    /// </summary>
    public class MultipleItemStatisticsSet
    {
        List<PropertyStatisticsData> _propertiesData = new List<PropertyStatisticsData>();
        public List<PropertyStatisticsData> PropertiesData
        {
            get
            {
                return _propertiesData;
            }
        }

        string _name;
        public string Name
        {
            get { return _name; }
        }

        public int ItemsCount
        {
            get
            {
                if (_propertiesData.Count > 0)
                {
                    return _propertiesData[0].Values.Count;
                }
                return 0;
            }
        }


        public object[] Items
        {
            set
            {
                if (value.Length == 0)
                {
                    return;
                }

                Type t = value[0].GetType();
                List<PropertyInfo> propertyInfos = new List<PropertyInfo>(t.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance));

                for (int i = propertyInfos.Count - 1; i >= 0; i--)
                {
                    if (propertyInfos[i].GetCustomAttributes(typeof(ItemStatisticsAttribute), true).Length != 0)
                    {// This property is not marked with the proper attribute.
                        _propertiesData.Add(new PropertyStatisticsData(this, propertyInfos[i], value));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MultipleItemStatisticsSet(string name, object[] items)
        {
            _name = name;
            Items = items;
        }

        public double[][] CombinedPropertyData()
        {
            return null;
        }
    }
}
