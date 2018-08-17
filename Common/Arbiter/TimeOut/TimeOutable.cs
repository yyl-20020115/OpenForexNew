using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    public abstract class TimeOutable
    {
        bool _isAlive = true;
        /// <summary>
        /// When an entities dies, it gets auto removed from TimeOutMonitors.
        /// </summary>
        public bool IsAlive
        {
            get { return _isAlive; }
        }

        System.DateTime _constructionTime = DateTime.Now;
        public System.DateTime ConstructionTime
        {
            get { return _constructionTime; }
        }

        TimeSpan _timeOut = TimeSpan.Zero;
        public TimeSpan TimeOut
        {
            get { return _timeOut; }
        }

        bool _timedOutOnDeath = false;
        public bool TimedOut
        {
            get
            {
                lock (this)
                {
                    if (_isAlive)
                    {
                        return (_timeOut != TimeSpan.Zero && (DateTime.Now - _constructionTime) >= _timeOut);
                    }
                    else
                    {
                        return _timedOutOnDeath;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeOutable(TimeSpan timeOut)
        {
            _timeOut = timeOut;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Die()
        {
            _timedOutOnDeath = TimedOut;
            _isAlive = false;
        }

        /// <summary>
        /// Will set the timeout to now, so that it becomes timed out.
        /// </summary>
        public void SetTimedOut()
        {
            lock (this)
            {
                _timeOut = DateTime.Now - _constructionTime;
            }
        }
    }
}
