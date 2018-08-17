using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    class TimeOutMonitor : IDisposable
    {
        System.Timers.Timer _timeOutTimer = new System.Timers.Timer();
        List<TimeOutable> _entities = new List<TimeOutable>();

        public event HandlerDelegate<TimeOutable> EntityTimedOutEvent;

        /// <summary>
        /// 
        /// </summary>
        public TimeOutMonitor()
        {
            _timeOutTimer.AutoReset = true;
            _timeOutTimer.Interval = 500;
            _timeOutTimer.Elapsed += new System.Timers.ElapsedEventHandler(_timeOutTimer_Elapsed);
            _timeOutTimer.Start();
        }

        public void Dispose()
        {
            _timeOutTimer.Stop();
            _timeOutTimer = null;
        }

        /// <summary>
        /// Add entity to be monitored.
        /// </summary>
        /// <param name="envelope"></param>
        public bool AddEntity(TimeOutable entity)
        {
            lock (_entities)
            {
                _entities.Add(entity);
            }

            return true;
        }

        /// <summary>
        /// Remove entity from monitoring.
        /// </summary>
        /// <param name="entity"></param>
        public bool RemoveEntity(TimeOutable entity)
        {
            lock (_entities)
            {
                return _entities.Remove(entity);
            }
        }

        /// <summary>
        /// Perform checks and updates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _timeOutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_entities)
            {
                // Backwards cycling as the collections get modified while cycling.
                for (int i = _entities.Count - 1; i >= 0; i--)
                {
                    TimeOutable entity = _entities[i];
                    if (entity.TimedOut || entity.IsAlive == false)
                    {// If timed out or dead, remove.
                        _entities.Remove(entity);
                        if (entity.TimedOut && EntityTimedOutEvent != null)
                        {// Notify whoever might be interested.
                            EntityTimedOutEvent(entity);
                        }
                    }
                }
            }
        }


    }
}
