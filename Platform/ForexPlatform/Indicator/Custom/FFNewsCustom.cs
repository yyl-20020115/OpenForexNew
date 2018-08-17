using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using CommonFinancial;
using System.Drawing;
using System.Net;
using System.Xml;
using System.Globalization;

namespace ForexPlatform
{
    /// <summary>
    /// Indicator provides Forex Factory XML news dataDelivery directly to the chart.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Forex Factory News Source ")]
    public class FFNewsCustom : CustomPlatformIndicator
    {
        public enum EventImpact
        {
            Low,
            Medium,
            High,
            Holiday,
            Unknown
        }

        [Serializable]
        public struct NewsEvent
        {
            public string Title;
            public string Country;
            
            /// <summary>
            /// If this is true, the Date contains both valid date and time; otherwise only valid date.
            /// </summary>
            public bool HasTime;

            public DateTime DateTime;
            public EventImpact Impact;
            public string Forecast;
            public string Previous;
        }

        string _xmlAddress = "http://cloud.forexfactory.com/ffcal_week_this.xml";
        /// <summary>
        /// Address of the forex factory news xml.
        /// </summary>
        public string XmlAddress
        {
            get { return _xmlAddress; }
            set { _xmlAddress = value; }
        }

        List<NewsEvent> _events = new List<NewsEvent>();
        
        [NonSerialized]
        volatile bool _isUpdating = false;

        List<NewsEvent> _visibleNewsEvents = new List<NewsEvent>();
        public List<NewsEvent> VisibleNewsEvents
        {
            get { lock (this) { return _visibleNewsEvents; } }
        }

        volatile bool _includeHigh = true;
        public bool IncludeHigh
        {
            get { return _includeHigh; }
            set { _includeHigh = value; }
        }

        volatile bool _includeMedium = true;
        public bool IncludeMedium
        {
            get { return _includeMedium; }
            set { _includeMedium = value; }
        }

        volatile bool _includeLow = false;
        public bool IncludeLow
        {
            get { return _includeLow; }
            set { _includeLow = value; }
        }

        Font TitleFont
        {
            get { return ChartSeries.CustomMessagesFont; }
            set { ChartSeries.CustomMessagesFont = value; }
        }

        SolidBrush _titleBrush = (SolidBrush)Brushes.LightSeaGreen;
        public SolidBrush TitleBrush
        {
            get { return _titleBrush; }
            set { _titleBrush = value; }
        }

        SolidBrush _impactBrush = (SolidBrush)Brushes.Red;
        public SolidBrush ImpactBrush
        {
            get { return _impactBrush; }
            set { _impactBrush = value; }
        }

        SolidBrush _previousBrush = (SolidBrush)Brushes.Sienna;
        public SolidBrush PreviousBrush
        {
            get { return _previousBrush; }
            set { _previousBrush = value; }
        }

        SolidBrush _forecastBrush = (SolidBrush)Brushes.Green;
        public SolidBrush ForecastBrush
        {
            get { return _forecastBrush; }
            set { _forecastBrush = value; }
        }

        volatile int _eventsShownCount = 3;
        /// <summary>
        /// The count of event shown. Closest events are shown, disregarding from the past or the future.
        /// </summary>
        public int EventsShownCount
        {
          get { return _eventsShownCount; }
          set { _eventsShownCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FFNewsCustom()
            : base(typeof(FFNewsCustom).Name,  false, true, new string[] { "FFNews" })
        {
            ChartSeries.CustomMessages.Add("ForexFactory news source not responding [http://www.forexfactory.com/ff_calendar_thisweek.xml].", Color.Empty);
            ChartSeries.CustomMessagesFont = new Font("Tahoma", 10, FontStyle.Bold);
            Parameters.ParameterUpdatedValueEvent += new IndicatorParameters.ParameterUpdatedValueDelegate(Parameters_ParameterUpdatedValueEvent);
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
            _isUpdating = false;
            _events.Clear();
            Parameters.ParameterUpdatedValueEvent += new IndicatorParameters.ParameterUpdatedValueDelegate(Parameters_ParameterUpdatedValueEvent);
        }

        void Parameters_ParameterUpdatedValueEvent(string name, object value)
        {
            UpdateVisibleEvents();
        }

        /// <summary>
        /// Override to change the chart series type attached.
        /// </summary>
        protected override PlatformIndicatorChartSeries CreateChartSeries()
        {
            return new FFNewsPlatformIndicatorChartSeries();
        }

        /// <summary>
        /// Simple clone implementation will not clone results and signals, only parameters.
        /// </summary>
        /// <returns></returns>
        public override PlatformIndicator OnSimpleClone()
        {
            FFNewsCustom result = new FFNewsCustom();
            result._description = this._description;
            result._name = this._name;

            return result;
        }

        protected override void OnCalculate(bool fullRecalculation, DataBarUpdateType? updateType)
        {
            if (_isUpdating == false)
            {
                GeneralHelper.FireAndForget(Update);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateVisibleEvents()
        {
            lock (this)
            {
                // Remove the initial loading requestMessage.
                ChartSeries.CustomMessages.Clear();

                SortedDictionary<TimeSpan, List<NewsEvent>> sortedEvents = new SortedDictionary<TimeSpan, List<NewsEvent>>();

                // Get all items sorted by closest to now (in UTC).
                for (int i = 0; i < _events.Count; i++)
                {
                    NewsEvent eventItem = _events[i];
                    if (eventItem.HasTime == false
                        || (eventItem.Impact == EventImpact.Low && IncludeLow == false)
                        || (eventItem.Impact == EventImpact.Medium && IncludeMedium == false)
                        || (eventItem.Impact == EventImpact.High && IncludeHigh == false)

                        )
                    {// Skip non timed items.
                        continue;
                    }

                    TimeSpan span = DateTime.UtcNow - _events[i].DateTime;
                    span = TimeSpan.FromSeconds(Math.Abs(span.TotalSeconds));

                    if (sortedEvents.ContainsKey(span) == false)
                    {
                        sortedEvents.Add(span, new List<NewsEvent>());
                    }

                    sortedEvents[span].Add(_events[i]);
                }

                // Establish the needed count of items, and are they previous or future.
                List<NewsEvent> prevEvents = new List<NewsEvent>();
                List<NewsEvent> postEvents = new List<NewsEvent>();
                foreach (KeyValuePair<TimeSpan, List<NewsEvent>> pair in sortedEvents)
                {
                    foreach (NewsEvent eventItem in pair.Value)
                    {
                        if (prevEvents.Count + postEvents.Count >= _eventsShownCount)
                        {// This must be before initial pass, to make sure 0 number is also correct.
                            break;
                        }

                        if (eventItem.DateTime < DateTime.UtcNow)
                        {
                            prevEvents.Add(eventItem);
                        }
                        else
                        {
                            postEvents.Add(eventItem);
                        }
                    }
                }

                _visibleNewsEvents.Clear();

                // Show the items.
                foreach (NewsEvent item in prevEvents)
                {
                    _visibleNewsEvents.Add(item);
                }

                foreach (NewsEvent item in postEvents)
                {
                    _visibleNewsEvents.Add(item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            TracerHelper.TraceEntry();
            if (_isUpdating == true)
            {
                return;
            }

            _isUpdating = true;

            List<NewsEvent> events = new List<NewsEvent>();

            WebRequest webRequest = WebRequest.Create(_xmlAddress);
            webRequest.Timeout = 5000;

            WebResponse response = webRequest.GetResponse();
            if (response.ContentLength > 0)
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(response.GetResponseStream());
                 
                    XmlNode weeklyEvents = document.ChildNodes[1];
                    foreach (XmlNode eventNode in weeklyEvents.ChildNodes)
                    {
                        events.Add(ParseEventXml(eventNode));
                    }
                }
                catch (Exception ex)
                {
                    SystemMonitor.Error("Failed to obtain XML stream [" + ex.Message + "].");
                }
            }

            lock (this)
            {
                _events.Clear();
                _events.AddRange(events);
            }

            UpdateVisibleEvents();

            ChartSeries.RaiseSeriesValuesUpdated(true);
            _isUpdating = false;
        }

        NewsEvent ParseEventXml(XmlNode eventNode)
        {
            string title = eventNode.ChildNodes[0].InnerText;
            string country = eventNode.ChildNodes[1].InnerText;
            string date = eventNode.ChildNodes[2].InnerText;
            string time = eventNode.ChildNodes[3].InnerText;
            string impact = eventNode.ChildNodes[4].InnerText;
            string forecast = eventNode.ChildNodes[5].InnerText;
            string previous = eventNode.ChildNodes[6].InnerText;

            EventImpact impactValue = EventImpact.Unknown;
            try
            {
                impactValue = (EventImpact)Enum.Parse(typeof(EventImpact), impact, true);
            }
            catch
            {
                SystemMonitor.OperationError("Failed to parse news event impact [" + impact + "]");
            }


            string dateTimeString = time + " " + date;
            // AddElement extra space before the am or pm, since parse needs it.
            dateTimeString = dateTimeString.ToLower().Replace("am", " am");
            dateTimeString = dateTimeString.ToLower().Replace("pm", " pm");

            // Perform custom date time format and fix (month-day-year).
            DateTimeFormatInfo info = new DateTimeFormatInfo();
            info.LongTimePattern = "MM-dd-yyyy";

            DateTime dateTimeValue = DateTime.Parse(dateTimeString, info);

            return new NewsEvent()
            {
                Country = country,
                Title = title,
                Forecast = forecast,
                Impact = impactValue,
                Previous = previous,
                DateTime = dateTimeValue,
                HasTime = string.IsNullOrEmpty(time) == false
            };
        }


    }
}
