using System;

namespace GeoToolkit.Commands
{
	public class CommandExecutingEventArgs : EventArgs
	{
	    public CommandExecutingEventArgs(object context = null)
		{
			Context = context;
		}

	    public CommandExecutingEventArgs() : this(true)
		{
		}

	    public bool Cancel { get; set; }

	    public object Context { get; private set; }
	}
}