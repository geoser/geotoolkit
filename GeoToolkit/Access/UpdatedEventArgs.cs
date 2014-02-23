using System;

namespace GeoToolkit.Access
{
    public class UpdatedEventArgs : EventArgs
    {
        public object Result { get; private set; }

        public UpdatedEventArgs(object result)
        {
            Result = result;
        }
    }
}