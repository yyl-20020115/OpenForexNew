using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    /// <summary>
    /// This is a very early version of an external profiling mechanism, communicating to 
    /// profiling app with windows messages; has support for both managed and unmanaged code.
    /// </summary>
    public class ProfilerHelper
    {
        /// <summary>
        /// Send a measurement to profiling app.
        /// </summary>
        public static void SendMeasure()
        {
            ////long freq = 2666700000;
            //long count1/*, count2, count3*/;
            ////CommonSupport.HighPerformanceTimer.QueryPerformanceFrequency(out freq);

            //HighPerformanceTimer.QueryPerformanceCounter(out count1);

            //uint xHigh = (uint)((ulong)count1 / uint.MaxValue);
            //uint xLow = (uint)((ulong)count1 % uint.MaxValue);

            ////if (xLow > int.MaxValue)
            ////{
            ////    //xLow -= int.MaxValue;
            ////}

            ////IntPtr ptr = (IntPtr)(xLow);

            //Win32Helper.SendMessage((IntPtr)3278846, 9997, new IntPtr(xHigh), new IntPtr(xLow));


            ////CommonSupport.HighPerformanceTimer.QueryPerformanceCounter(out count2);
            ////CommonSupport.HighPerformanceTimer.QueryPerformanceCounter(out count3);

            ////double combined = (double)count1 / (double)freq;
            ////Debug.Write("<< " + count1 + " @ " + freq + " : " + combined + Environment.NewLine);

            ////combined = (double)count2 / (double)freq;
            ////Debug.Write("<< " + count2 + " @ " + freq + " : " + combined + Environment.NewLine);

            ////combined = (double)count3 / (double)freq;
            ////Debug.Write("<< " + count3 + " @ " + freq + " : " + combined + Environment.NewLine);
        }
    }
}
