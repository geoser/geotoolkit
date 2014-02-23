using System;
using System.ComponentModel;
using GeoToolkit.Logging;

namespace GeoToolkit.Access
{
    public abstract class AsyncUpdater : Component
    {
        public abstract DateTime LastUpdateTime { get; }
        public bool IsRunning { get; protected set; }
        public abstract TimeSpan Interval { get; set; }
        public bool Locked { get; protected set; }

        public event EventHandler<CancelableEventArgs> Starting;
        public event EventHandler StartingCanceled;
        public event EventHandler Started;
        public event EventHandler<CancelableEventArgs> Stopping;
        public event EventHandler Stopped;

        public abstract void Start();
        protected abstract void Stop(bool disposing);

        public void Stop()
        {
            Stop(false);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (IsRunning)
                    Stop(true);
            }
            catch
            {
            }

            base.Dispose(disposing);
        }

        #region Event Invokators

        protected virtual void InvokeStarting(CancelableEventArgs e)
        {
            this.LogDebug("Invoke Starting");

            EventHandler<CancelableEventArgs> handler = Starting;
            if (handler != null) handler(this, e);
        }

        protected virtual void InvokeStartingCanceled(EventArgs e)
        {
            EventHandler handler = StartingCanceled;
            if (handler != null) handler(this, e);
        }

        protected virtual void InvokeStarted(EventArgs e)
        {
            this.LogDebug("Invoke Started");

            var handler = Started;
            if (handler != null) handler(this, e);
        }

        protected virtual void InvokeStopping(CancelableEventArgs e)
        {
            var handler = Stopping;
            if (handler != null) handler(this, e);
        }

        protected virtual void InvokeStopped(EventArgs e)
        {
            var handler = Stopped;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}