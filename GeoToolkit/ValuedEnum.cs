using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace GeoToolkit
{
    public static class ValuedEnum
    {
        private const FieldAttributes _ENUM_FIELDS = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal;

        private static readonly ConcurrentDictionary<Type, Dictionary<string, string>> _valueCache =
            new ConcurrentDictionary<Type, Dictionary<string, string>>();

        public static string ToValueString(this Enum value)
        {
            return GetStringValue(value.ToString(), GetEnumValues(value.GetType()));
        }

        public static string GetValue<T>(T enumValue) where T : struct
        {
            return ValuedEnum<T>.GetValue(enumValue);
        }

        public static string[] GetValues<T>() where T : struct
        {
            return ValuedEnum<T>.GetValues();
        }

        internal static Dictionary<string, string> GetEnumValues(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsEnum)
                throw new ArgumentException(String.Format("Type {0} is not enum", type.FullName));

            Dictionary<string, string> result;

            if (_valueCache.TryGetValue(type, out result))
                return result;

            return _valueCache
                .GetOrAdd(type,
                          t =>
                              {
                                  var res = new Dictionary<string, string>();
                                  FieldInfo[] fields = t.GetFields();

                                  foreach (FieldInfo fieldInfo in fields)
                                  {
                                      if ((fieldInfo.Attributes & _ENUM_FIELDS) != _ENUM_FIELDS)
                                          continue;

                                      var enumAttributes = 
                                          Attribute.GetCustomAttributes(fieldInfo, typeof (EnumValueAttribute));

                                      res.Add(fieldInfo.Name, enumAttributes.Length == 0
                                                                  ? fieldInfo.Name
                                                                  : ((EnumValueAttribute) enumAttributes[0]).StringValue);
                                  }

                                  return res;
                              });
        }

        internal static string GetStringValue(string value, Dictionary<string, string> enumValues)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (enumValues == null)
                throw new ArgumentNullException("enumValues");

            var builder = new StringBuilder();

            foreach (string v in value.Replace(" ", string.Empty).Split(','))
                builder.AppendFormat("{0}, ", enumValues.ContainsKey(v) ? enumValues[v] : v);

            return builder.Length > 0 ? builder.ToString().TrimEnd(' ', ',') : string.Empty;
        }

        public static TEnum Parse<TEnum>(string value) where TEnum : struct
        {
            return ValuedEnum<TEnum>.Parse(value);
        }

        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct
        {
            return ValuedEnum<TEnum>.TryParse(value, out result);
        }
    }

#if DEBUG

    [TestFixture]
    public class ValuedEnumTest
    {
        private enum TestEnum1
        {
            [EnumValue("Test value 1")]
            Test1,

            [EnumValue("Test value 2")]
            Test2
        }

        [Flags]
        private enum TestEnum2
        {
            [EnumValue("Test value 1")]
            Test1 = 1,

            [EnumValue("Test value 2")]
            Test2 = 2
        }

        [Flags]
        private enum TestEnum3
        {
            [EnumValue("Test value 1")]
            Test1 = 1,

            Test2 = 2,

            [EnumValue("Test value 3")]
            Test3 = 4,
        }

        [Test]
        public void EnumTest()
        {
            Assert.AreEqual("Test value 2", TestEnum1.Test2.ToValueString());
            Assert.AreEqual("Test value 1", TestEnum1.Test1.ToValueString());

            Assert.AreEqual("Test value 1, Test value 2", (TestEnum2.Test1 | TestEnum2.Test2).ToValueString());

            Assert.AreEqual("Test value 1, Test2", (TestEnum3.Test1 | TestEnum3.Test2).ToValueString());
            Assert.AreEqual("Test value 1, Test value 3", (TestEnum3.Test1 | TestEnum3.Test3).ToValueString());
            Assert.AreEqual("Test2, Test value 3", (TestEnum3.Test2 | TestEnum3.Test3).ToValueString());

            var strings = ValuedEnum<TestEnum3>.GetValues();
            Assert.IsTrue(strings.Contains("Test value 1"));
            Assert.IsTrue(strings.Contains("Test2"));
            Assert.IsTrue(strings.Contains("Test value 3"));

            var strings1 = ValuedEnum.GetValues<TestEnum3>();
            Assert.AreEqual(strings, strings1);

            Assert.AreEqual(TestEnum3.Test3, ValuedEnum.Parse<TestEnum3>("Test value 3"));
        }
    }

#endif
}