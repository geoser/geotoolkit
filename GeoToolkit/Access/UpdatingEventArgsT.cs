namespace GeoToolkit.Access
{
    public class UpdatingEventArgs<T> : UpdatingEventArgs
    {
        public new T Param
        {
            get
            {
                if (base.Param == null)
                    return default(T);

                return (T) base.Param;
            }
            set { base.Param = value; }
        }

        public UpdatingEventArgs(T param) : base(param)
        {
        }

        public static UpdatingEventArgs<T> CreateCancel(T param, string cancelReason)
        {
            return new UpdatingEventArgs<T>(param) { CancelReason = cancelReason};
        }
    }
}