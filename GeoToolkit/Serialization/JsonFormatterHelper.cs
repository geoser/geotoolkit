using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
#if DEBUG
using NUnit.Framework;
#endif

namespace GeoToolkit.Serialization
{
    public static class JsonFormatterHelper
    {
        public static string Serialize(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            using (var stream = new MemoryStream())
            {
                new DataContractJsonSerializer(obj.GetType()).WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static T Deserialize<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException("json");

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                return new DataContractJsonSerializer(typeof (T)).ReadObject(ms) as T;
        }
    }

#if DEBUG
    [TestFixture]
    public class JsonFormatterHelperTest
    {
        [Serializable]
        public class TestClass
        {
            public int TestField1 { get; set; }
            public string TestField2 { get; set; }
            public DateTime TestField3 { get; set; }
        }

        [Test]
        public void SerializeTest()
        {
            var testObj = new TestClass
                                {
                                    TestField1 = 3,
                                    TestField2 = "Тест ;№%:",
                                    TestField3 = DateTime.Now
                                };

            var serialized = JsonFormatterHelper.Serialize(testObj);

            Assert.IsNotNullOrEmpty(serialized);

            var deserialized = JsonFormatterHelper.Deserialize<TestClass>(serialized);

            Assert.IsNotNull(deserialized);

            Assert.AreEqual(testObj.TestField1, deserialized.TestField1);
            Assert.AreEqual(testObj.TestField2, deserialized.TestField2);
            Assert.Less((testObj.TestField3 - deserialized.TestField3).TotalMilliseconds, 1);
        }
    }
#endif
}