using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoToolkit
{
	[Serializable]
	public sealed class ValuedEnum<TEnum> : IEquatable<ValuedEnum<TEnum>>, IEquatable<TEnum> 
		where TEnum : struct
	{
        private static readonly Dictionary<string, string> _enumValues;
	    private string _valueString;

	    public TEnum Value { get; private set; }

	    static ValuedEnum()
	    {
	        _enumValues = ValuedEnum.GetEnumValues(typeof(TEnum));
	    }

	    public ValuedEnum(TEnum enumValue)
		{
			Value = enumValue;
		}

	    public static string GetValue(TEnum value)
	    {
	        return ((ValuedEnum<TEnum>)value).ToValueString();
	    }

	    public static string GetValue(ValuedEnum<TEnum> value)
	    {
	        return value.ToValueString();
	    }

	    public string ToValueString()
	    {
	        return _valueString ?? (_valueString = ValuedEnum.GetStringValue(Value.ToString(), _enumValues));
	    }

	    public override string ToString()
	    {
	        return Value.ToString();
	    }

        public static string[] GetValues()
        {
            return _enumValues.Values.ToArray();
        }

        public static TEnum Parse(string value)
        {
            foreach (KeyValuePair<string, string> enumValue in _enumValues)
                if (enumValue.Value == value)
                    return (TEnum) Enum.Parse(typeof (TEnum), enumValue.Key);

            throw new InvalidOperationException(string.Format("Cannot parse value {0} to type {1}",
                                                              value, typeof (TEnum)));
        }

	    #region IEquatable

        public bool Equals(TEnum value)
	    {
	        return Equals(Value, value);
	    }

	    public bool Equals(ValuedEnum<TEnum> valuedEnum)
	    {
	        return Equals(Value, valuedEnum.Value);
	    }

	    public override bool Equals(object obj)
	    {
	        if (!(obj is ValuedEnum<TEnum> || obj is TEnum))
	            return false;
	        return Equals((ValuedEnum<TEnum>)obj);
	    }

	    public override int GetHashCode()
	    {
	        return Value.GetHashCode();
	    }

	    #endregion

	    #region Orerators

	    public static implicit operator ValuedEnum<TEnum>(TEnum enumValue)
	    {
	        return new ValuedEnum<TEnum>(enumValue);
	    }

	    public static implicit operator TEnum(ValuedEnum<TEnum> valued)
	    {
	        return valued.Value;
	    }

	    public static bool operator ==(ValuedEnum<TEnum> left, ValuedEnum<TEnum> right)
	    {
	        return Equals(left, right);
	    }

	    public static bool operator !=(ValuedEnum<TEnum> left, ValuedEnum<TEnum> right)
	    {
	        return !Equals(left, right);
	    }

	    public static bool operator ==(TEnum left, ValuedEnum<TEnum> right)
	    {
	        return Equals(left, right.Value);
	    }

	    public static bool operator !=(TEnum left, ValuedEnum<TEnum> right)
	    {
	        return !Equals(left, right.Value);
	    }

	    public static bool operator ==(ValuedEnum<TEnum> left, TEnum right)
	    {
	        return Equals(left.Value, right);
	    }

	    public static bool operator !=(ValuedEnum<TEnum> left, TEnum right)
	    {
	        return !Equals(left.Value, right);
	    }

	    #endregion
	}
}