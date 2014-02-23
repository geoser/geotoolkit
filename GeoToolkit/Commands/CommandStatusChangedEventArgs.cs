using System;

namespace GeoToolkit.Commands
{
	public class CommandStatusChangedEventArgs : EventArgs
	{
		private readonly bool _isAvailable;

		public CommandStatusChangedEventArgs(bool isAvailable)
		{
			_isAvailable = isAvailable;
		}

		public bool IsAvailable
		{
			get { return _isAvailable; }
		}
	}
}