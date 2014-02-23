using System;

namespace GeoToolkit
{
	[Serializable]
	public class PendingToken
	{
		private static readonly TimeSpan DEFAULT_PENDING_TIME = TimeSpan.Zero;

		private DateTime _completeTime;
		private DateTime _lastCompleteTime;
		private TimeSpan _lockTime = TimeSpan.FromSeconds(2);
		private bool _pending = true;
		private DateTime _startTime = DateTime.Now;

		public PendingToken(bool pending, TimeSpan lockTime)
		{
			_lockTime = lockTime;
			_pending = pending;
			_completeTime = DateTime.MinValue;
			_lastCompleteTime = DateTime.MinValue;
		}

		public PendingToken(bool pending) : this(pending, DEFAULT_PENDING_TIME)
		{
		}

		public PendingToken() : this(true)
		{
		}

		public PendingToken(TimeSpan lockTime) : this(true, lockTime)
		{
		}

		public TimeSpan LockTime
		{
			get { return _lockTime; }
			set { _lockTime = value; }
		}

		public bool Locked
		{
			get { return CompleteTime.Add(LockTime) > DateTime.Now; }
		}

		public bool Pending
		{
			get { return _pending; }
			set { _pending = value; }
		}

		public DateTime StartTime
		{
			get { return _startTime; }
		}

		public DateTime CompleteTime
		{
			get { return _completeTime; }
		}

		public DateTime LastCompleteTime
		{
			get { return _lastCompleteTime; }
		}

		public bool PendingOrLocked
		{
			get { return Pending || Locked; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="PendingTokenLockedException"></exception>
		public void Start()
		{
			Start(false);
		}

		public void Start(bool ignoreLocked)
		{
			if (!ignoreLocked && PendingOrLocked)
				throw new PendingTokenLockedException();

			if (ignoreLocked && Pending)
				throw new PendingTokenLockedException();

			_startTime = DateTime.Now;
			_completeTime = DateTime.MinValue;
			_pending = true;
		}

		public void Complete()
		{
			if (!Pending)
				return;

			_completeTime = DateTime.Now;
			_lastCompleteTime = _completeTime;
			_pending = false;
		}

		public static implicit operator bool(PendingToken token)
		{
			return token.PendingOrLocked;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1} - {2})", _pending.ToString(),
			                     _startTime.ToString("dd:MM:yyyy hh:mm:ss:fff"),
			                     _completeTime.ToString("dd:MM:yyyy hh:mm:ss:fff"));
		}
	}
}