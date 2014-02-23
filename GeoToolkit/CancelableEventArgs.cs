using System.ComponentModel;

namespace GeoToolkit
{
    public class CancelableEventArgs : CancelEventArgs
    {
        public string CancelReason { get; set; }
        public object Tag { get; set; }

        public CancelableEventArgs() : this(false, string.Empty)
        {
        }

        public CancelableEventArgs(bool cancel) : this(cancel, string.Empty)
        {
        }

        public CancelableEventArgs(bool cancel, string cancelReason) : base(cancel)
        {
            CancelReason = cancelReason;
        }
    }
}