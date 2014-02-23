using System;
using System.ComponentModel;

namespace GeoToolkit.Commands
{
	[Serializable]
	public abstract class CommandBase<T> : CommandBase
	{
	    protected CommandBase() : this(true)
		{
		}

	    protected CommandBase(bool enabled) : base(enabled)
		{
		}

        [Browsable(false)]
        public new T Context
        {
            get
            {
                { return (T)base.Context;}
            }
            set
            {
                base.Context = value;
            }
        }
	}
}