using System;

namespace GeoToolkit.Access
{
    [Serializable]
    public class UpdatingCancelledException : ApplicationException
    {
        public UpdatingCancelledException() : this(string.Empty)
        {
        }

        public UpdatingCancelledException(string cancelReason) 
            : base("Updating cancelled by user code: " + cancelReason)
        {
        }
    }
}