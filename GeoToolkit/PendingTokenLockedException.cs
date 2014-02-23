using System;

namespace GeoToolkit
{
    [Serializable]
	public class PendingTokenLockedException : Exception
	{
		public PendingTokenLockedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public PendingTokenLockedException(string message) : this(message, null)
		{
		}

		public PendingTokenLockedException() : this("Cannot start locked PendingToken object. Wait till Locked property is false")
		{
		}
	}
}