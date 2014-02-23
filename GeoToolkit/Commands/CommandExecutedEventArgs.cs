using System;

namespace GeoToolkit.Commands
{
    public class CommandExecutedEventArgs : EventArgs
    {
        public CommandExecutedEventArgs(object context = null)
        {
            Context = context;
        }

        public CommandExecutedEventArgs() : this(true)
        {
        }

        public object Context { get; private set; }
    }
}