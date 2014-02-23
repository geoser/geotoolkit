namespace GeoToolkit.Access
{
    public class UpdatedEventArgs<T> : UpdatedEventArgs
    {
        public new T Result
        {
            get
            {
                if (base.Result == null)
                    return default(T);

                return (T) base.Result;
            }
        }

        public UpdatedEventArgs(T result) : base(result)
        {
        }
    }
}