using System;
using GeoToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoToolkitTest
{
	/// <summary>
	///This is a test class for GeoToolkit.ValuableEnum&lt;TEnum&gt; and is intended
	///to contain all GeoToolkit.ValuableEnum&lt;TEnum&gt; Unit Tests
	///</summary>
	[TestClass()]
	public class ValuableEnumTest
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		public enum TestEnum
		{
			[EnumValue("ValueAttribute1")]
			Value1,
			Value2,
			[EnumValue("ValueAttribute3")]
			Value3
		}

		[Flags]
		public enum TestEnumFlags
		{
			[EnumValue("ValueAttribute1")]
			Value1 = 1,
			Value2 = 2,
			[EnumValue("ValueAttribute3")]
			Value3 = 4
		}

		/// <summary>
		///A test for ToValueString ()
		///</summary>
		[TestMethod()]
		public void ToValueStringTest()
		{
			Assert.AreEqual("ValueAttribute1", ((ValuedEnum<TestEnum>)TestEnum.Value1).ToValueString());
			Assert.AreEqual("Value2", ((ValuedEnum<TestEnum>)TestEnum.Value2).ToValueString());

			TestEnumFlags testEnumFlags = TestEnumFlags.Value1 | TestEnumFlags.Value2;
			Assert.AreEqual("ValueAttribute1, Value2", ((ValuedEnum<TestEnumFlags>)testEnumFlags).ToValueString());
		}

        public void ParseTest()
        {
            Assert.AreEqual(TestEnum.Value1, ValuedEnum.Parse<TestEnum>("ValueAttribute1"));
            Assert.AreEqual(TestEnum.Value1, ValuedEnum.Parse<TestEnum>("Value1"));
            Assert.AreEqual(TestEnum.Value2, ValuedEnum.Parse<TestEnum>("Value2"));
        }
	}
}