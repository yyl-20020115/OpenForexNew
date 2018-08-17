using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommonFinancial
{
    public class ChartSeriesColorSelector
    {
        int _seriesIndex = 0;
        public int SeriesIndex
        {
            get { return _seriesIndex; }
            set { _seriesIndex = value; }
        }

        public ChartSeriesColorSelector()
        {
        }

        public void SetupSeries(ChartSeries inputSeries)
        {
            if (inputSeries is TradeChartSeries)
            {
                TradeChartSeries series = (TradeChartSeries)inputSeries;
                switch (_seriesIndex)
                {
                    case 0:
                        {
                            series.RisingBarPen = Pens.DarkSeaGreen;
                            series.RisingBarFill = null;
                            series.FallingBarPen = Pens.IndianRed;
                            series.FallingBarFill = (SolidBrush)Brushes.IndianRed;
                        }
                        break;
                    case 1:
                        {
                            series.RisingBarPen = Pens.LightBlue;
                            series.RisingBarFill = null;
                            series.FallingBarPen = Pens.LightBlue;
                            series.FallingBarFill = (SolidBrush)Brushes.LightBlue;
                        }
                        break;
                    case 2:
                        {
                            series.RisingBarPen = Pens.LightSlateGray;
                            series.RisingBarFill = null;
                            series.FallingBarPen = Pens.LightSlateGray;
                            series.FallingBarFill = (SolidBrush)Brushes.LightSlateGray;
                        }
                        break;
                    case 3:
                        {
                            series.RisingBarPen = Pens.Lavender;
                            series.RisingBarFill = null;
                            series.FallingBarPen = Pens.Lavender;
                            series.FallingBarFill = (SolidBrush)Brushes.Lavender;
                        }
                        break;
                    case 4:
                        {
                            series.RisingBarPen = Pens.OrangeRed;
                            series.RisingBarFill = null;
                            series.FallingBarPen = Pens.OrangeRed;
                            series.FallingBarFill = (SolidBrush)Brushes.OrangeRed;
                        }
                        break;
                    case 5:
                        {
                            series.RisingBarPen = Pens.Gold;
                            series.RisingBarFill = null;
                            series.FallingBarPen = Pens.Gold;
                            series.FallingBarFill = (SolidBrush)Brushes.Gold;
                        }
                        break;
                    case 6:
                        {
                            series.RisingBarPen = Pens.GreenYellow;
                            series.RisingBarFill = null;
                            series.FallingBarPen = Pens.GreenYellow;
                            series.FallingBarFill = (SolidBrush)Brushes.GreenYellow;
                        }
                        break;
                    case 7:
                        {
                            series.RisingBarPen = Pens.BlueViolet;
                            series.RisingBarFill = null;
                            series.FallingBarPen = Pens.BlueViolet;
                            series.FallingBarFill = (SolidBrush)Brushes.BlueViolet;
                        }
                        break;

                }
            }

            _seriesIndex++;
        }
    }
}
