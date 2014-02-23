namespace GeoToolkit.Access
{
    public abstract class UpdatableObjectBase<TRes> : UpdatableObjectBase<TRes, object>, IUpdatable<TRes>
    {
        protected abstract TRes OnUpdate();

        protected override TRes OnUpdate(object param = null)
        {
            return OnUpdate();
        }

        TRes IUpdatable<TRes, object>.Update(object param)
        {
            return Update(param);
        }

        TRes IUpdatable<TRes, object>.UpdateSyncronized(object param)
        {
            return UpdateSyncronized(param);
        }
    }
}