using System;

namespace GeoToolkit.Commands
{
    [Serializable]
    public class GeneralCommand : GeneralCommand<object>
    {
        public GeneralCommand(Action action)
        {
            Action = o => action();
        }
    }
}