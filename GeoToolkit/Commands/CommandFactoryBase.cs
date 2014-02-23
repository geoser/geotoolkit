using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GeoToolkit.Commands
{
	public abstract class CommandFactoryBase<TFactory> : Component
        where TFactory : CommandFactoryBase<TFactory>
	{
        private readonly object _syncRoot = new object();

		private readonly List<CommandBase> _commands = new List<CommandBase>();
        private readonly ConcurrentDictionary<Keys, CommandBase> _keyedCommands = 
            new ConcurrentDictionary<Keys, CommandBase>();

		public IList<CommandBase> Commands
		{
			get { return _commands.AsReadOnly(); }
		}

		public event EventHandler<CommandExecutedEventArgs> CommandExecuted;
		public event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        public CommandBase GetCommandForKey(Keys keyData)
        {
            CommandBase result;

            return _keyedCommands.TryGetValue(keyData, out result) ? result : null;
        }

	    public CommandBase RegisterCommand(CommandBase command)
		{
		    lock (_syncRoot)
		    {
                if (_commands.Contains(command))
                    return command;

		        _commands.Add(command);

		        if (command.Shortcut != 0)
		            _keyedCommands[command.Shortcut] = command;

		        command.BeforeExecute += CommandBeforeExecute;
		        command.AfterExecute += CommandAfterExecute;
		        return command;
		    }
		}

		private void CommandAfterExecute(object sender, Commands.CommandExecutedEventArgs e)
		{
			OnCommandExecuted(sender, e);
		}

		private void CommandBeforeExecute(object sender, EventArgs e)
		{
			OnCommandExecuting(sender);
		}

		protected void OnCommandExecuted(object sender, Commands.CommandExecutedEventArgs e)
		{
		    var handler = CommandExecuted;
		    if (handler != null)
				handler(null, new CommandExecutedEventArgs((CommandBase)sender, true));
		}

	    protected void OnCommandExecuting(object sender)
	    {
	        var handler = CommandExecuting;
	        if (handler != null)
				handler(null, new CommandExecutingEventArgs((CommandBase)sender));
	    }

	    protected override void Dispose(bool disposing)
	    {
	        if (disposing)
	            foreach (var command in Commands)
	                command.Dispose();

            base.Dispose(disposing);
	    }

	    #region Nested type: CommandExecutedEventArgs

		public class CommandExecutedEventArgs : EventArgs
		{
			public readonly CommandBase Command;
			public readonly bool Success;

			public CommandExecutedEventArgs(CommandBase command, bool success)
			{
				Command = command;
				Success = success;
			}
		}

		#endregion

		#region Nested type: CommandExecutingEventArgs

		public class CommandExecutingEventArgs : EventArgs
		{
			public readonly CommandBase Command;

			public CommandExecutingEventArgs(CommandBase command)
			{
				Command = command;
			}
		}

		#endregion
	}
}