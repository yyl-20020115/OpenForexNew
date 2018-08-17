using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Arbiter
{
    class BackgroundWorkerEx : BackgroundWorker
    {
        ExecutionEntity _entity;
        internal ExecutionEntity Entity
        {
            get { return _entity; }
        }

        public BackgroundWorkerEx(ExecutionEntity entity)
        {
            _entity = entity;
        }
    }
}
