using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GeoToolkit.Commands
{
    [Serializable]
    public abstract class ControlCommandBase<T> : CommandBase<T>
    {
        private readonly List<Component> _components = new List<Component>();
        private readonly object _syncRoot = new object();

        protected ControlCommandBase(bool enabled) : base(enabled)
        {
            UseWaitCursor = true;
        }

        protected ControlCommandBase() : this(true)
        {
        }

        public bool UseWaitCursor { get; set; }

        public IEnumerable<Component> BoundControls
        {
            get { return _components.AsReadOnly(); }
        }

        public void Bind(Button button)
        {
            if (button == null)
                throw new ArgumentNullException("button");

            lock (_syncRoot)
            {
                if (_components.Contains(button))
                    return;

                button.Enabled = Enabled;
                button.Click += ComponentOnClick;
                button.EnabledChanged += ComponentOnEnabledChanged;

                _components.Add(button);
            }
        }

        public void Bind(ToolStripItem toolBoxItem)
        {
            if (toolBoxItem == null)
                throw new ArgumentNullException("toolBoxItem");

            lock (_syncRoot)
            {
                if (_components.Contains(toolBoxItem))
                    return;

                toolBoxItem.Enabled = Enabled;
                toolBoxItem.Click += ComponentOnClick;
                toolBoxItem.EnabledChanged += ComponentOnEnabledChanged;

                _components.Add(toolBoxItem);
            }
        }

        protected void Bind(params Component[] components)
        {
            if (components == null)
                throw new ArgumentNullException("components");

            if (components.Length == 0)
                return;

            foreach (var component in components)
            {
                if (component is Button)
                {
                    Bind((Button)component);
                    continue;
                }

                if (component is ToolStripItem)
                    Bind((ToolStripItem) component);
            }
        }

        public void Unbind(Button button)
        {
            if (button == null)
                throw new ArgumentNullException("button");

            lock (_syncRoot)
            {
                if (!_components.Contains(button))
                    return;

                button.Click -= ComponentOnClick;
                button.EnabledChanged -= ComponentOnEnabledChanged;

                _components.Remove(button);
            }
        }

        public void Unbind(ToolStripItem toolBoxItem)
        {
            if (toolBoxItem == null)
                throw new ArgumentNullException("toolBoxItem");

            lock (_syncRoot)
            {
                if (!_components.Contains(toolBoxItem))
                    return;

                toolBoxItem.Click -= ComponentOnClick;
                toolBoxItem.EnabledChanged -= ComponentOnEnabledChanged;

                _components.Remove(toolBoxItem);
            }
        }

        public void UnbindAll()
        {
            lock (_syncRoot)
            {
                if (_components == null || _components.Count == 0)
                    return;

                do
                {
                    var component = _components[0];

                    if (component is Button)
                        Unbind((Button)component);
                    else if (component is ToolStripItem)
                        Unbind((ToolStripItem)component);
                } while (_components.Count > 0);
            }
        }

        private void ComponentOnEnabledChanged(object sender, EventArgs eventArgs)
        {
            if (sender is Button)
                ControlEnabled(((Button) sender).Enabled);

            if (sender is ToolStripItem)
                ControlEnabled(((ToolStripItem)sender).Enabled);
        }

        private void ComponentOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (UseWaitCursor)
                    Cursor.Current = Cursors.WaitCursor;

                ControlEnabled(false);

                Execute();

                ControlEnabled(Enabled);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        protected override void InvokeEnabledChanged()
        {
            ControlEnabled(Enabled);

            base.InvokeEnabledChanged();
        }

        private void ControlEnabled(bool enabled)
        {
            lock (_syncRoot)
            {
                foreach (var component in _components)
                {
                    if (component is Button)
                        ((Button) component).Enabled = enabled;

                    if (component is ToolStripItem)
                        ((ToolStripItem) component).Enabled = enabled;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            UnbindAll();

            base.Dispose(disposing);
        }
    }
}