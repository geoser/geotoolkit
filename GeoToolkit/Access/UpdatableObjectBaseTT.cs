using System;
using GeoToolkit.Logging;

namespace GeoToolkit.Access
{
    public abstract class UpdatableObjectBase<TRes, TPar> : UpdatableObjectBase, IUpdatable<TRes, TPar>
    {
        private readonly object _syncRoot = new object();

        public event EventHandler<UpdatingEventArgs<TPar>> Updating;
        public event EventHandler<UpdatedEventArgs<TRes>> Updated;

        public override sealed object Update(object param)
        {
            return param == null ? Update(default(TPar)) : Update((TPar)param);
        }

        public override sealed object UpdateSyncronized(object param)
        {
            return param == null ? UpdateSyncronized(default(TPar)) : UpdateSyncronized((TPar)param);
        }

        public virtual TRes Update(TPar param = default(TPar))
        {
            if (!Equals(param, default(TPar)))
                this.LogDebugFormat("Update with param: {0}", param);
            else
                this.LogDebug("Update without param");

            if (!UpdateAllowed)
                throw new InvalidOperationException("Update is not allowed");

            lock (_syncRoot)
            {
                Buzy = true;

                var cancelArgs = new UpdatingEventArgs<TPar>(param);

                try
                {
                    InvokeUpdating(cancelArgs);

                    if (cancelArgs.Cancel)
                        throw new UpdatingCancelledException(cancelArgs.CancelReason);

                    this.LogDebug("Call OnUpdate");
                    TRes result = OnUpdate(param);

                    InvokeUpdated(new UpdatedEventArgs<TRes>(result));

                    LastUpdated = DateTime.Now;
                    IsUpdated = true;

                    return result;
                }
                finally
                {
                    Buzy = false;
                }
            }
        }

        public virtual TRes UpdateSyncronized(TPar param = default(TPar))
        {
            lock (_syncRoot)
            {
                SleepUntilNextAllowed();
                return Update(param);
            }
        }

        protected abstract TRes OnUpdate(TPar param = default(TPar));

        private void InvokeUpdating(UpdatingEventArgs<TPar> e)
        {
            this.LogDebug("Invoke updating");

            var handler = Updating;
            if (handler != null) handler(this, e);
        }

        private void InvokeUpdated(UpdatedEventArgs<TRes> e)
        {
            this.LogDebug("Invoke Updated");

            var handler = Updated;
            if (handler != null) handler(this, e);
        }
    }
}