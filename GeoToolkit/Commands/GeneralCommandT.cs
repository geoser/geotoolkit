using System;

namespace GeoToolkit.Commands
{
    [Serializable]
    public class GeneralCommand<T> : ControlCommandBase<T>
    {
        protected GeneralCommand()
        {
        }

        public GeneralCommand(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Action = action;
        }

        public Action<T> Action { get; protected set; }

        protected override void OnExecute()
        {
            Action(Context);
        }
    }
}