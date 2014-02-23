using System;

namespace GeoToolkit.Access
{
    public class UpdatingEventArgs : EventArgs
    {
        public object Param { get; protected set; }
        public bool Cancel { get; set; }
        public string CancelReason { get; set; }

        public UpdatingEventArgs(object param)
        {
            Param = param;
            Cancel = false;
        }
    }
}