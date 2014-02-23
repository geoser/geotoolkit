using System;

namespace GeoToolkit.Commands
{
	[Serializable]
	public class CommandExecutingException : ApplicationException
	{
		public CommandExecutingException(string message, Exception inner) : base(message, inner)
		{
		}

		public CommandExecutingException(string message) : this(message, null)
		{
		}

		public CommandExecutingException(Exception inner) : this("Command executing", inner)
		{
		}

		public CommandExecutingException() : this((Exception)null)
		{
		}
	}
}