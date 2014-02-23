using System;

namespace GeoToolkit
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EnumValueAttribute : Attribute
	{
		public readonly string StringValue;

		public EnumValueAttribute(string stringValue)
		{
			StringValue = stringValue;
		}
	}
}