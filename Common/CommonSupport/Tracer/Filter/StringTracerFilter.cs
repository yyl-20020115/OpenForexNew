using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CommonSupport
{
    /// <summary>
    /// Filter for tracer items, based on string contents.
    /// </summary>
    [Serializable]
    public class StringTracerFilter : TracerFilter
    {
        volatile string _positiveFilterString = string.Empty;
        /// <summary>
        /// Only items containing this will be allowed.
        /// </summary>
        public string PositiveFilterString
        {
            get { return _positiveFilterString; }

            set
            {
                if (value != _positiveFilterString)
                {
                    _positiveFilterString = value;
                    RaiseFilterUpdatedEvent(false);
                }
            }
        }

        volatile List<string> _negativeFilterStrings = new List<string>();
        /// <summary>
        /// No items containing any of these will be allowed.
        /// </summary>
        public List<string> NegativeFilterStrings
        {
            get 
            {
                lock (_negativeFilterStrings)
                {
                    return new List<string>(_negativeFilterStrings);
                }
            }

            set 
            {
                _negativeFilterStrings = new List<string>(value);
                RaiseFilterUpdatedEvent(false);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public StringTracerFilter()
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public StringTracerFilter(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _positiveFilterString = info.GetString("_positiveFilterString");
            _negativeFilterStrings = (List<string>)info.GetValue("_negativeFilterStrings", typeof(List<string>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("_positiveFilterString", _positiveFilterString);
            info.AddValue("_negativeFilterStrings", _negativeFilterStrings);
        }

        /// <summary>
        /// Obtain filtering data from other filter.
        /// </summary>
        public override void CopyDataFrom(TracerFilter otherFilter)
        {
            StringTracerFilter filter = otherFilter as StringTracerFilter;
            this._positiveFilterString = filter.PositiveFilterString;
            if (filter.NegativeFilterStrings == null)
            {
                this._negativeFilterStrings = null;
            }
            else
            {
                if (filter.NegativeFilterStrings != null)
                {
                    this._negativeFilterStrings = new List<string>(filter.NegativeFilterStrings);
                }
                else
                {
                    this._negativeFilterStrings = null;
                }
            }

            RaiseFilterUpdatedEvent(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool FilterItem(TracerItem item)
        {
            if (string.IsNullOrEmpty(_positiveFilterString) == false || _negativeFilterStrings != null)
            {
                return FilterItem(item, _positiveFilterString, _negativeFilterStrings);
            }

            return true;
        }

        /// <summary>
        /// Perform the actual filtering of items, agains positive and negative filter strings.
        /// Will *lock the negativeFilterStrings*, if it finds it.
        /// </summary>
        /// <returns></returns>
        public static bool FilterItem(TracerItem item, string positiveFilterString, List<string> negativeFilterStrings)
        {
            string message = item.PrintMessage().ToLower();
            
            // Positive filter check.
            if (string.IsNullOrEmpty(positiveFilterString) == false
                && message.Contains(positiveFilterString.ToLower()) == false)
            {
                return false;
            }

            if (negativeFilterStrings != null)
            {
                lock (negativeFilterStrings)
                {
                    // Negative filter check.
                    foreach (string filter in negativeFilterStrings)
                    {
                        if (string.IsNullOrEmpty(filter) == false
                            && message.Contains(filter.ToLower()))
                        {
                            return false;
                        }
                    }
                }
            }

            // Pass.
            return true;
        }
    }
}
