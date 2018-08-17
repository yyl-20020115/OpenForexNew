using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace CommonSupport
{
    /// <summary>
    /// Class contains method to assist in mathematical operations.
    /// </summary>
    public static class MathHelper
    {
        public enum DirectionEnum
        {
            Equal = 0,
            Up,
            Down
        }

        /// <summary>
        /// Round a numeric value to a base.
        /// </summary>
        public static int RoundTo(int value, int roundBase)
        {
            double temp = (double)value / (double)roundBase;
            return (int)((int)temp * roundBase);
        }

        /// <summary>
        /// Will round the value to a given count of resulting symbols.
        /// </summary>
        /// <returns></returns>
        public static double RoundToSymbolsCount(double inputValue, int requiredSymbolsCount)
        {
            int symbols = (int)Math.Ceiling(Math.Log10(inputValue));
            // Values below 0 mean spaces already existing after the zero point - discard them.
            symbols = Math.Max(0, symbols);
            return Math.Round(inputValue, requiredSymbolsCount - symbols);

            //double symbolsd = Math.Log10(inputDataUnits[0].Close);
            //double symbols2d = Math.Log10(1.2);
            //int symbols2 = (int)Math.Ceiling(Math.Log10(1.2));
            //double symbols3d = Math.Log10(10.2);
            //int symbols3 = (int)Math.Ceiling(Math.Log10(10.2));
        }

        /// <summary>
        /// Perform a complex calculation to establish the distance between these points in absolute coordinates,
        /// considering both X and Y scaling.
        /// </summary>
        public static float GetAbsoluteDistance(Matrix transformationMatrix, PointF point1, PointF point2)
        {
            float xDifference = Math.Abs(point1.X - point2.X);
            float yDifference = Math.Abs(point1.Y - point2.Y);

            // Apply scale.
            xDifference = xDifference * transformationMatrix.Elements[0];
            yDifference = yDifference * transformationMatrix.Elements[3];

            return (float)Math.Sqrt(xDifference * xDifference + yDifference * yDifference);
        }

        /// <summary>
        /// Will show where line 1 crosses line 2; 1 for cross up, 2 for cross down, 0 for no crossing.
        /// </summary>
        static public double[] CreateLineCrossings(double[] line1, double[] line2)
        {
            System.Diagnostics.Debug.Assert(line1.Length == line2.Length);

            double[] results = new double[line2.Length];

            for (int k = 0; k < line1.Length; k++)
            {
                if (k == 0)
                {
                    //if (line1[k] == line2[k])
                    //{
                    //    results[k] = 1;
                    //}
                }
                else
                {
                    if ((line1[k - 1] >= line2[k - 1] && line1[k] <= line2[k]))
                    {
                        results[k] = 2;
                    }
                    else if (line1[k - 1] <= line2[k - 1] && line1[k] >= line2[k])
                    {
                        results[k] = 1;
                    }
                }
            }

            return results;
        }


        /// <summary>
        /// 
        /// </summary>
        static public double[] CreateFixedLineResultLength(double value, int count)
        {
            double[] line = new double[count];
            for (int i = 0; i < count; i++)
            {
                line[i] = value;
            }
            return line;
        }


        /// <summary>
        /// Calculates a "connection value" line between the 2 values (like a vector connecting 2 points).
        /// </summary>
        static public double[] CreateConnectionValues(double initialValue, double secondaryValue, int connectionValuesCount)
        {
            double[] results = new double[connectionValuesCount];
            //System.Diagnostics.Debug.Assert(connectionValuesCount >= 2);

            results[0] = initialValue;
            results[connectionValuesCount - 1] = secondaryValue;

            double stepValue = (secondaryValue - initialValue) / (connectionValuesCount - 1);
            for (int i = 1; i < connectionValuesCount - 1; i++)
            {
                results[i] = results[i - 1] + stepValue;
            }

            return results;
        }

        static public DirectionEnum[] EstablishDirection(double[] values)
        {
            DirectionEnum[] results = new DirectionEnum[values.Length];

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i - 1] > values[i])
                {
                    results[i] = DirectionEnum.Down;
                }
                else if (values[i - 1] < values[i])
                {
                    results[i] = DirectionEnum.Up;
                }
            }

            return results;
        }


        /// <summary>
        /// This is a quick approximation of the Moving average.
        /// The formula used here is as follows : 
        /// An estimate of the moving average if the bin size for moving average is n may be obtained by:
        /// NewAverage = (((n-1) * OldAverage) + newValue)/n
        /// This works once the bin is full (sample number >= n). The bin partially full is often dealt with by using a seed value for the initial moving average (OldAverage) and then using this calculation.
        /// This assumes normal distribution of values etc.
        /// </summary>
        static public double[] CalculateQuickMA(double[] values, int period)
        {
            double[] results = new double[values.Length];
            double average = 0;
            for (int i = 0; i < values.Length; i++)
            {
                average = (((period - 1) * average) + values[i]) / period;
                results[i] = average;
            }
            return results;
        }
        
        /// <summary>
        /// Provides a way to see the distribution of the values in periodsCount separate periods.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="periodsCount"></param>
        /// <returns></returns>
        static public Dictionary<double, double> CalculateValueDistribution(IEnumerable<double> values, int periodsCount)
        {
            List<double> list = new List<double>(values);
            list.Sort();

            double min = list[0];
            double max = list[list.Count - 1];

            double periodSize = (max - min) / periodsCount;

            Dictionary<double, double> results = new Dictionary<double,double>();

            int j=0;
            for (double i=min; i<max; i+=periodSize)
            {
                int count = 0;
                for (; j < list.Count; j++)
                {
                    if (list[j] <= i + periodSize)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }

                results.Add(i, count);
            }

            return results;
        }

    }
}
