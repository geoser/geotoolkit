using System;
using System.ComponentModel;
using System.Windows.Forms;
using GeoToolkit.Logging;

namespace GeoToolkit.Commands
{
	[Serializable]
	public abstract class CommandBase : Component
	{
		private bool _enabled;
	    private readonly object _syncRoot = new object();
	    private object _context;

	    protected CommandBase() : this(true)
		{
		}

	    protected CommandBase(bool enabled)
		{
			_enabled = enabled;
		}

	    public event EventHandler<CancelEventArgs> ContextChanging;
	    public event EventHandler ContextChanged;

	    public bool Buzy { get; protected set; }

	    [Browsable(false)]
        public virtual object Context
	    {
	        get { return _context; }
	        set
	        {
                if (!OnContextChanging(value))
                    throw new ApplicationException("Changing was cancelled by user code");

	            _context = value;

	            OnContextChanged();
	        }
	    }

	    public virtual Keys Shortcut
		{
			get { return 0; }
		}

	    public virtual string Name
		{
			get { return GetType().Name; }
		}

	    /// <summary>
		/// ƒоступна ли команда дл€ пользовател€
		/// </summary>
		public virtual bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (value == _enabled)
					return;

				_enabled = value;

				InvokeEnabledChanged();
			}
		}

	    private void InvokeContextChanging(CancelEventArgs e)
	    {
	        EventHandler<CancelEventArgs> handler = ContextChanging;
	        if (handler != null) handler(this, e);
	    }

	    private void InvokeContextChanged(EventArgs e)
	    {
	        EventHandler handler = ContextChanged;
	        if (handler != null) handler(this, e);
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns>false if changing should be cancelled</returns>
        protected virtual bool OnContextChanging(object newValue)
        {
            var args = new CancelEventArgs();
            InvokeContextChanging(args);
            return !args.Cancel;
        }

        protected virtual void OnContextChanged()
        {
            InvokeContextChanged(EventArgs.Empty);
        }

	    public void Execute()
		{
            this.LogDebugFormat("Executing command {0} (type {1})...", this, GetType());

            if (!Enabled)
                throw new CommandExecutingException("Cannot execute disabled command");

            if (Buzy)
                throw new InvalidOperationException("Cannot execute buzy command");

            try
            {
                Buzy = true;

                InvokeBeforeExecute();

                var args = new CommandExecutingEventArgs(Context);

                InvokeExecuting(args);

                if (args.Cancel)
                {
                    this.LogWarnFormat("Command {0} cancelled by user code", this);
                    return;
                }

                OnExecute();

                InvokeAfterExecute(new CommandExecutedEventArgs(Context));
            }
            finally
            {
                Buzy = false;
            }
		}

	    protected abstract void OnExecute();

	    public bool TryExecute(object context = null)
        {
            if (!Enabled)
                return false;

            lock (_syncRoot)
            {
                if (!Enabled)
                    return false;

                try
                {
                    Execute();
                    return true;
                }
                catch (Exception e)
                {
                    this.LogError("Cannot execute command: " + this, e);
                    return false;
                }
            }
        }

		#region Events

		protected virtual void InvokeEnabledChanged()
		{
		    var handler = EnabledChanged;
		    if (handler != null)
				handler(this, new CommandStatusChangedEventArgs(Enabled));
		}

	    protected virtual void InvokeBeforeExecute()
	    {
	        var handler = BeforeExecute;
	        if (handler != null)
				handler(this, EventArgs.Empty);
	    }

	    protected virtual void InvokeExecuting(CommandExecutingEventArgs args)
	    {
	        var handler = Executing;
	        if (handler != null)
				handler(this, args);
	    }

        protected virtual void InvokeAfterExecute(CommandExecutedEventArgs args)
	    {
	        var handler = AfterExecute;
	        if (handler != null)
				handler(this, args);
	    }

	    /// <summary>
		/// —татус команды изменилс€
		/// </summary>
		[field: NonSerialized]
		public event EventHandler<CommandStatusChangedEventArgs> EnabledChanged;

		/// <summary>
		/// ¬озникает непосредственно до исполнени€ команды
		/// </summary>
		[field: NonSerialized]
		public event EventHandler BeforeExecute;

		/// <summary>
		/// ¬озникает в момент выполнени€ команды
		/// </summary>
		[field: NonSerialized]
		public event EventHandler<CommandExecutingEventArgs> Executing;

		/// <summary>
		/// ¬озникает сразу после исполнени€ команды
		/// </summary>
		[field: NonSerialized]
        public event EventHandler<CommandExecutedEventArgs> AfterExecute;

		#endregion
	}
}