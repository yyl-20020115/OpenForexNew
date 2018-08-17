using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace CommonSupport
{
    /// <summary>
    /// Class helps in accessing high performace Windows timers.
    /// Represents a high-resolution stopwatch. It can be used to measure 
    /// very small intervals of time. Keep in mind this is a slow approach, if u need speed go for the timeGetTime (??) package.
    /// </summary>

    public class HighPerformanceTimer
    {
#if WINDOWS

        #region Imports
        /// <summary>
        /// The QueryPerformanceCounter function retrieves the current 
        /// value of the high-resolution performance counter.
        /// </summary>
        /// <param name="x">
        /// Pointer to a variable that receives the 
        /// current performance-counter value, in counts.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is 
        /// nonzero.
        /// </returns>
        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        /// <summary>
        /// The QueryPerformanceFrequency function retrieves the 
        /// frequency of the high-resolution performance counter, 
        /// if one exists. The frequency cannot change while the 
        /// system is running.
        /// </summary>
        /// <param name="x">
        /// Pointer to a variable that receives 
        /// the current performance-counter frequency, in counts 
        /// per second. If the installed hardware does not support 
        /// a high-resolution performance counter, this parameter 
        /// can be zero.
        /// </param>
        /// <returns>
        /// If the installed hardware supports a 
        /// high-resolution performance counter, the return value 
        /// is nonzero.
        /// </returns>
        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        #endregion

        private long _startTime;
        private long _stopTime;
        
        static private long _frequency;

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// A long that holds the start time.
        /// </value>
        private long StartTime
        {
            get
            {
                return _startTime;
            }
        }

        /// <summary>
        /// Gets or sets the frequency of the high-resolution 
        /// performance counter.
        /// </summary>
        /// <value>
        /// A long that holds the frequency of the 
        /// high-resolution performance counter.
        /// </value>
        private static long Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
            }
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static HighPerformanceTimer()
        {
            if (QueryPerformanceFrequency(out _frequency) == false)
            {
                // high-performance counter not supported 
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public HighPerformanceTimer()
        {
            _startTime = 0;
            _stopTime = 0;

        }

        /// <summary>
        /// 
        /// </summary>
        public static double GetCurrentTime()
        {
            long value = GetValue();
            return value / _frequency;
        }

        #region Methods

        /// <summary>
        /// Resets the stopwatch. This method should be called 
        /// when you start measuring.
        /// </summary>
        public void Start()
        {
            QueryPerformanceCounter(out _startTime);
        }

        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution 
        /// performance counter.
        /// </exception>
        public void Reset()
        {
            _startTime = GetValue();
        }

        // Stop the timer
        public void Stop()
        {
            QueryPerformanceCounter(out _stopTime);
        }

        /// <summary>
        /// Returns the duration of the timer (in seconds).
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)(_stopTime - _startTime) / (double)_frequency;
            }
        }

        /// <summary>
        /// Returns the time that has passed since the Reset() 
        /// method was called.
        /// </summary>
        /// <remarks>
        /// The time is returned in tenths-of-a-millisecond. 
        /// If the Peek method returns '10000', it means the interval 
        /// took exactely one second.
        /// </remarks>
        /// <returns>
        /// A long that contains the time that has passed 
        /// since the Reset() method was called.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution performance counter.
        /// </exception>
        public long PeekMillisecond10th()
        {
            return (long)(((GetValue() - StartTime)
                / (double)Frequency) * 10000);
        }

        /// <summary>
        /// Milliseconds
        /// </summary>
        /// <returns></returns>
        public long PeekMillisecond()
        {
            return (long)(((GetValue() - StartTime)
                / (double)Frequency) * 1000);
        }

        /// <summary>
        /// Microsecond
        /// </summary>
        /// <returns></returns>
        public long PeekMicrosecond()
        {
            return (long)(((GetValue() - StartTime) 
                / (double)Frequency) * 1000000);
        }

        /// <summary>
        /// Nanosecond
        /// </summary>
        /// <returns></returns>
        public long PeekNanosecond()
        {
            return (long)(((GetValue() - StartTime) 
                / (double)Frequency));
        }

        /// <summary>
        /// Get the value in seconds and reset the counter.
        /// </summary>
        /// <returns></returns>
        public double GetSecondsReset()
        {
            long valLong = GetValue() - StartTime;
            Reset();

            return (double)valLong / (double)Frequency;
        }


        /// <summary>
        /// Retrieves the current value of the high-resolution 
        /// performance counter.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The system does not have a high-resolution 
        /// performance counter.
        /// </exception>
        /// <returns>
        /// A long that contains the current performance-counter 
        /// value, in counts.
        /// </returns>
        public static long GetValue()
        {
            long ret = 0;
            if (QueryPerformanceCounter(out ret) == false)
            {
                throw new NotSupportedException("Error while querying the high-resolution performance counter.");
            }
            return ret;
        }

        #endregion

#endif
    }

}
