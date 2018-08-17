using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    public interface IDynamicPropertyContainer
    {
        string[] GetGenericDynamicPropertiesNames();
        object GetGenericDynamicPropertyValue(string name);
        Type GetGenericDynamicPropertyType(string name);
        bool SetGenericDynamicPropertyValue(string name, object value);

        void PropertyChanged();
    }
}
