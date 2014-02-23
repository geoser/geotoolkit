namespace GeoToolkit.Access
{
    public class AsyncUpdater<TU, TRes> : AsyncUpdater<TU, TRes, object>
        where TU : class, IUpdatable<TRes>
    {
        public AsyncUpdater(TU accessor) : base(accessor)
        {
        }

        public AsyncUpdater()
        {
        }

        protected override object GetUpdatingParameter()
        {
            return null;
        }
    }
}