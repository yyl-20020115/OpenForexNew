// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
// This is based on the LP TimerTest, by Luc Pattyn, at codeproject
// ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace CommonSupport
{
    /// <summary>
    /// Class uses the windows multimedia precision timing mechanisms, to provide precision timing for events.
    /// Class can be used in 2 ways - subscribe for the event, or watch the Update Event with a thread waiting for it.
    /// </summary>
    public class HighPrecisionEventTimer
    {
#if WINDOWS

        // WIN32 CONSTANTS IMPORTED
        
        // Event occurs once, after uDelay milliseconds
        const int TIME_ONESHOT = 0x0000; // Program timer for single event.
        
        // Event occurs every uDelay milliseconds.
        const int TIME_PERIODIC = 0x0001; // Program for continuous periodic event.

        // When the timer expires, the system calls the function pointed to by the lpTimeProc parameter. This is the default value.
        const int TIME_CALLBACK_FUNCTION = 0x0000;  // Callback is function.
        
        // When the timer expires, the system calls the SetEvent function to set the event pointed to by the lpTimeProc parameter. The dwUser parameter is ignored.
        const int TIME_CALLBACK_EVENT_SET = 0x0010;  // Callback is event - use SetEvent.
        
        //When the timer expires, the system calls PulseEvent function to pulse the event pointed to by the lpTimeProc parameter. The dwUser parameter is ignored.
        const int TIME_CALLBACK_EVENT_PULSE = 0x0020;  // Callback is event - use PulseEvent.
        
        //Passing this flag prevents an event from occurring after the timeKillEvent function is called.
        const int TIME_KILL_SYNCHRONOUS = 0x0100; // This flag prevents the event from occurring.

        /// <summary>
        /// Constructor.
        /// </summary>
        public HighPrecisionEventTimer()
        {
            // Establish the timing intervals capabilities - not needed right now but nice to have.
			TimeCaps timeCaps = new TimeCaps(0, 0);
			
            // This function queries the timer device to determine its resolution.
            if (timeGetDevCaps(out timeCaps, Marshal.SizeOf(timeCaps)) != 0)
            {
                throw new Exception("HighPrecisionEventTimer failed to aquire parameters from the OS.");
            }
            
			_minimumValue = timeCaps.Minimum;
			_maximumValue = timeCaps.Maximum;

            _internalAntiGCDelegateInstance = new TimerEventHandler(TickEventHandlerMethod);
        }
        
        /// <summary>
        /// Class can be used in 2 ways - subscribe for the event.
        /// </summary>
        public delegate void EventOccuredDelegate();
        public event EventOccuredDelegate EventOccuredEvent;

        /// <summary>
        /// UNTESTED !!
        /// Or watch this event with a thread waiting for it.
        /// </summary>
        public ManualResetEvent UpdateEvent = new ManualResetEvent(false);


        delegate void TimerEventHandler(uint id, uint msg, ref int userCtx, int rsv1, int rsv2);
        // VERY IMPORTANT!!! We need to keep a reference as otherwise the GC will delete the delegate object
        // that is waiting for the call, coming in from unmanaged code.
        TimerEventHandler _internalAntiGCDelegateInstance;
        
        uint _minimumValue = 0;
        uint _maximumValue = 0;

        uint _timerId = 0;

        public bool IsRunning
        {
            get { return _timerId != 0; }
        }

        [DllImport("Winmm.dll")]
        private static extern int timeGetTime();

		[DllImport("Winmm.dll")]
		private static extern uint timeGetDevCaps(out TimeCaps timeCaps, int size);

		[DllImport("Winmm.dll", SetLastError=true)]
		private static extern uint timeSetEvent(int msDelay, int msResolution, TimerEventHandler handler, ref int userCtx, int eventType);

		[DllImport("Winmm.dll", SetLastError=true)]
        private static extern uint timeKillEvent(uint timerEventId);

        /// <summary>
        /// Structure used to query information in the timeGetDevCaps
        /// </summary>
        struct TimeCaps 
        {
			public uint Minimum;
			public uint Maximum;

			public TimeCaps(uint minimum, uint maximum) 
            {
				Minimum=minimum;
				Maximum=maximum;
			}
		}

		public void StartPeriodic(int intervalMs) 
        {
            int dummyData = 0;	// dummy data

            _timerId = timeSetEvent(intervalMs, intervalMs, _internalAntiGCDelegateInstance,
                ref dummyData, TIME_PERIODIC | TIME_KILL_SYNCHRONOUS);

            if (_timerId == 0)
            {
                throw new Exception("HighPrecisionEventTimer failed to set event.");
            }
		}

        public void StartOnce(int intervalMs)
        {
            int dummyData = 0;	// dummy data

            // No need for this, oneshot timers do not need to be stopped.
            _timerId = 0;

            uint funcResult = timeSetEvent(intervalMs, intervalMs, _internalAntiGCDelegateInstance,
                ref dummyData, TIME_ONESHOT | TIME_KILL_SYNCHRONOUS);

            if (funcResult == 0)
            {
                throw new Exception("HighPrecisionEventTimer failed to set event.");
            }
        }

        public void Stop()
        {
            if (_timerId != 0)
            {
                timeKillEvent(_timerId);
            }
            
            _timerId = 0;
        }

		void TickEventHandlerMethod(uint id, uint msg, ref int userCtx, int rsv1, int rsv2) 
        {
            if (EventOccuredEvent != null)
            {
                EventOccuredEvent();
            }

            lock (UpdateEvent)
            {
                UpdateEvent.Set();
            }
		}


#endif

    }
}
