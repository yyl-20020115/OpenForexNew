using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    public class Profiler
    {
        public enum ProfilerEntry
        {
            MainForm_UpdateUIComponents,
            AManaged_Update,
            GS_DoWork_SimulationModifiers,
            GS_DoWork_PopulationModifiers,
            GS_DoWork_PopulationIndividuals,
        }

#if DEBUG
        public static Int64[] Entries = new Int64[Enum.GetValues(typeof(ProfilerEntry)).Length];
        public static Int64[] Times = new Int64[Enum.GetValues(typeof(ProfilerEntry)).Length];
        static long _lastTicks = 0;
#endif

        // TODO : CONVERT THE PROFILER TO MEDIA TIMERS
        public static void Enter(ProfilerEntry entry)
        {
#if DEBUG
            Entries[(int)entry]++;
            _lastTicks = System.DateTime.Now.Ticks;
#endif
        }

        public static void Leave(ProfilerEntry entry)
        {
#if DEBUG
            Times[(int)entry] += System.DateTime.Now.Ticks - _lastTicks;
#endif
        }

    }
}
