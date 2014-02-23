using System;
using System.Threading;
using GeoToolkit.Logging;

namespace GeoToolkit.Access
{
    public abstract class UpdatableObjectBase : IUpdatable
    {
        private TimeSpan _updateInterval = TimeSpan.MinValue;

        protected UpdatableObjectBase()
        {
            LastUpdated = DateTime.MinValue;
        }

        public abstract object Update(object param);
        public abstract object UpdateSyncronized(object param);

        public bool Buzy { get; protected set; }

        public bool IsUpdated { get; protected set; }

        public DateTime LastUpdated { get; protected set; }

        public TimeSpan UpdateInterval
        {
            get { return _updateInterval != TimeSpan.MinValue ? _updateInterval : DefaultUpdateInterval; }
            set { _updateInterval = value; }
        }

        public abstract TimeSpan DefaultUpdateInterval { get; }

        public bool UpdateAllowed
        {
            get
            {
                if (Buzy)
                    return false;

                if (!IsUpdated)
                    return true;

                var nowTimeShifted = DateTime.Now.AddMilliseconds(50);
                if (nowTimeShifted > LastUpdated.Add(UpdateInterval))
                    return true;

                return false;
            }
        }

        public void SleepUntilNextAllowed()
        {
            if (Buzy)
                throw new InvalidOperationException("Thread cannot sleep while accessor is buzy");

            if (!IsUpdated)
                return;

            if (LastUpdated.Add(UpdateInterval) <= DateTime.Now)
                return;

            var sleepTime = LastUpdated.Add(UpdateInterval).Subtract(DateTime.Now);

            this.LogDebugFormat("Sleep for {0} ms", sleepTime.Milliseconds);
            Thread.Sleep(sleepTime);
        }
    }
}
