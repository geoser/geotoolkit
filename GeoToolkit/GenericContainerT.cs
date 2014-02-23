using System;

namespace GeoToolkit
{
    [Serializable]
    public class GenericContainer<T> where T : class
    {
        private T _value;

        public GenericContainer(T value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Value = value;
        }

        public T Value
        {
            get { return _value; }
            set
            {
                if (value == _value)
                    return;

                _value = value;

                InvokeValueChanged(EventArgs.Empty);
            }
        }

        public event EventHandler ValueChanged;

        protected void InvokeValueChanged(EventArgs e)
        {
            var handler = ValueChanged;
            if (handler != null) handler(this, e);
        }

        public override string ToString()
        {
            return Value != null ? Value.ToString() : base.ToString();
        }

        public bool Equals(GenericContainer<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GenericContainer<T>)) return false;
            return Equals((GenericContainer<T>) obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public static bool operator ==(GenericContainer<T> left, GenericContainer<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GenericContainer<T> left, GenericContainer<T> right)
        {
            return !Equals(left, right);
        }
    }
}