using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.Serialization;
using CommonFinancial;
using CommonSupport;
using ForexPlatformPersistence;

namespace ForexPlatform
{
    /// <summary>
    /// This class brings together the indicator and the ChartSeries, allowing the dataDelivery to be rendered
    /// and containing render specific information.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Forex Factory Platform Indicator Chart Series")]
    public class FFNewsPlatformIndicatorChartSeries : PlatformIndicatorChartSeries
    {
        new FFNewsCustom Indicator
        {
            get
            {
                return (FFNewsCustom)base.Indicator;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FFNewsPlatformIndicatorChartSeries()
            : base(UserFriendlyNameAttribute.GetTypeAttributeName(typeof(FFNewsPlatformIndicatorChartSeries)))
        {
        }

        /// <summary>
        /// Deserialization.
        /// </summary>
        public FFNewsPlatformIndicatorChartSeries(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Serialization.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override PointF DrawCustomMessages(ChartPane managingPane, GraphicsWrapper g, PointF drawingLocation)
        {
            if (Visible == false)
            {
                return drawingLocation;
            }

            // Draw any standard messages first (if any).
            drawingLocation = base.DrawCustomMessages(managingPane, g, drawingLocation);

            Font font = managingPane.TitleFont;
            if (CustomMessagesFont != null)
            {
                font = CustomMessagesFont;
            }

            foreach (FFNewsCustom.NewsEvent eventItem in Indicator.VisibleNewsEvents)
            {
                TimeSpan span = (eventItem.DateTime - DateTime.UtcNow);
                int hours = (int)Math.Abs(Math.Floor(span.TotalHours));

                string message;
                if (span.TotalSeconds < 0)
                {
                    message = hours.ToString() + " hrs " + Math.Abs(span.Minutes).ToString() + " mins since " + eventItem.Country + ": " + eventItem.Title;
                }
                else
                {
                    message = hours.ToString() + " hrs " + span.Minutes.ToString() + " mins until " + eventItem.Country + ": " + eventItem.Title;
                }
                drawingLocation = DrawCustomMessage(g, font, Indicator.TitleBrush, message, drawingLocation);

                float drawingLocationOriginalX = drawingLocation.X;
                SizeF size = new SizeF();
                if (font != null && Indicator.ImpactBrush != null)
                {
                    // Draw impact part.
                    string impactString = "Impact: " + eventItem.Impact.ToString();
                    size = g.MeasureString(impactString, font);
                    g.DrawString(impactString, font, Indicator.ImpactBrush, drawingLocation);
                    drawingLocation.X += size.Width;
                }

                // Draw previous part.
                if (string.IsNullOrEmpty(eventItem.Previous) == false)
                {
                    if (font != null && Indicator.PreviousBrush != null)
                    {
                        string previousString = "Previous: " + eventItem.Previous;
                        size = g.MeasureString(previousString, font);
                        g.DrawString(previousString, font, Indicator.PreviousBrush, drawingLocation);
                        drawingLocation.X += size.Width;
                    }
                }

                if (string.IsNullOrEmpty(eventItem.Forecast) == false)
                {
                    if (font != null && Indicator.ForecastBrush != null)
                    {
                        string forecastString = "Forecast: " + eventItem.Forecast;
                        size = g.MeasureString(forecastString, font);
                        g.DrawString(forecastString, font, Indicator.ForecastBrush, drawingLocation);
                        drawingLocation.X += size.Width;
                    }
                }

                drawingLocation.X = drawingLocationOriginalX;
                drawingLocation.Y += size.Height;
            }

            return drawingLocation;
        }
    }
}
