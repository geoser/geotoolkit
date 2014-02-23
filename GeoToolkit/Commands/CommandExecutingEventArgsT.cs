namespace GeoToolkit.Commands
{
	public class CommandExecutingEventArgs<T> : CommandExecutingEventArgs
	{
		public new T Context
		{
			get { return (T)base.Context; }
		}

		public CommandExecutingEventArgs()
		{
		}

		public CommandExecutingEventArgs(T context) : base(context)
		{
		}
	}
}