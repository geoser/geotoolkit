using System;
using System.Threading;
using BLToolkit.Reflection;
using BLToolkit.TypeBuilder;
using GeoToolkit.Logging;

namespace GeoToolkit.Access
{
    public abstract class AsyncUpdater<TA, TRes, TPar> : AsyncUpdater
        where TA : class, IUpdatable<TRes, TPar>
    {
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private readonly object _syncRoot = new object();
        private bool _stopFlag;
        private Thread _workingThread;
        private bool _isAccessorAutoCreated;

        public bool AutoCreateAccessor { get; set; }

        protected AsyncUpdater(TA accessor) : this()
        {
            if (accessor == null)
                throw new ArgumentNullException("accessor");

            Accessor = accessor;
        }

        protected AsyncUpdater()
        {
            ThreadStopTimeout = TimeSpan.FromSeconds(10);

            AutoCreateAccessor = true;
        }

        public TimeSpan ThreadStopTimeout { get; set; }

        public override sealed DateTime LastUpdateTime
        {
            get { return Accessor.LastUpdated; }
        }

        /// <summary>
        /// Public constructor for safe use as a component
        /// </summary>
        public TA Accessor { get; set; }

        public override TimeSpan Interval
        {
            get
            {
                AssertAccessor();

                return Accessor.UpdateInterval;
            }
            set
            {
                AssertAccessor();

                Accessor.UpdateInterval = value;
            }
        }

        public event EventHandler<UpdatingEventArgs<TPar>> Updating
        {
            add
            {
                AssertAccessor();

                Accessor.Updating += value;
            }
            remove
            {
                AssertAccessor();

                Accessor.Updating -= value;
            }
        }

        public event EventHandler<UpdatedEventArgs<TRes>> Updated
        {
            add
            {
                AssertAccessor();

                Accessor.Updated += value;
            }
            remove
            {
                AssertAccessor();
                
                Accessor.Updated -= value;
            }
        }

        private void AssertAccessor()
        {
            if (Accessor != null)
                return;
            
            if (AutoCreateAccessor)
            {
                Accessor = TypeFactory.CreateInstance<TA>();
                _isAccessorAutoCreated = true;
                return;
            }

            throw new InvalidOperationException("You must initialize Accessor first");
        }

        private void DoWork()
        {
            while (!_stopFlag)
            {
                _resetEvent.WaitOne(Interval);

                if (_stopFlag)
                    return;

                try
                {
                    TPar updatingParam = GetUpdatingParameter();

                    UpdateSyncronized(updatingParam);
                }
                catch(UpdatingCancelledException e)
                {
                    this.LogWarn(e.Message);
                }
                catch (Exception e)
                {
                    this.LogError("Error during updating", e);
                }

                if (_stopFlag)
                    return;

                _resetEvent.Reset();
            }
        }

        protected abstract TPar GetUpdatingParameter();

        public TRes Update(TPar param = default(TPar))
        {
            AssertAccessor();

            return Accessor.Update(param);
        }

        public TRes UpdateSyncronized(TPar param = default(TPar))
        {
            AssertAccessor();

            try
            {
                Locked = true;

                lock (_syncRoot)
                    return Accessor.UpdateSyncronized(param);
            }
            finally
            {
                Locked = false;
            }
        }

        public override sealed void Start()
        {
            if (IsRunning)
            {
                this.LogWarn("Start method call for running updater. Do nothing...");
                return;
            }

            this.LogInfo("Starting...");

            _stopFlag = false;

            var startingEventArgs = new CancelableEventArgs();

            InvokeStarting(startingEventArgs);

            if (startingEventArgs.Cancel)
            {
                this.LogError("Starting was cancelled by user code: " + startingEventArgs.CancelReason);

                InvokeStartingCanceled(EventArgs.Empty);

                return;
            }

            _workingThread = new Thread(DoWork);
            _workingThread.Start();

            IsRunning = true;

            _resetEvent.Set();

            InvokeStarted(EventArgs.Empty);

            this.LogInfo("Started successfully");
        }

        protected override sealed void Stop(bool disposing)
        {
            this.LogInfo("Stopping...");

            if (!IsRunning)
            {
                this.LogWarn("No need to stop. The updater is already stopped");
                return;
            }

            if (!disposing)
            {
                var stoppingEventArgs = new CancelableEventArgs();
                InvokeStopping(stoppingEventArgs);
                if (stoppingEventArgs.Cancel)
                {
                    this.LogError("Stopping was cancelled by user code");
                    return;
                }
            }

            _stopFlag = true;

            if (_workingThread != null && _workingThread.IsAlive && _workingThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                _resetEvent.Set();

                this.LogDebug("Joining process...");
                if (!_workingThread.Join(ThreadStopTimeout))
                    _workingThread.Abort();

                this.LogInfo("Process stopped");
            }

            this.LogDebug("Invoke Stopped");
            if (!disposing)
                InvokeStopped(EventArgs.Empty);

            this.LogInfo("Stopped successfully");

            IsRunning = false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_isAccessorAutoCreated && Accessor is IDisposable)
                ((IDisposable) Accessor).Dispose();
        }
    }
}