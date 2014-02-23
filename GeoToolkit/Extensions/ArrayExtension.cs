using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using System.Linq;

namespace GeoToolkit.Extensions
{
    public static class ArrayExtension
    {
        public static string GetDescription<T>(this IEnumerable<T> array, 
            string linePrefix = null, 
            string lineSuffix = null)
        {
            return GetDescription(array, item => true, linePrefix, lineSuffix);
        }

        public static string GetDescription<T>(this IEnumerable<T> array,
            Predicate<T> predicate, 
            string linePrefix = null, 
            string lineSuffix = null)
        {
            return GetDescription(array, predicate, o => string.Empty, o => string.Empty, linePrefix, lineSuffix);
        }

        public static string GetDescription<T>(this IEnumerable<T> array,
            Func<T, string> itemPrefixFactory,
            Func<T, string> itemSuffixFactory,
            string itemLinePrefix = null, 
            string itemLineSuffix = null)
        {
            return GetDescription(array, item => true, itemPrefixFactory, itemSuffixFactory, itemLinePrefix, itemLineSuffix);
        }

        public static string GetDescription<T>(this IEnumerable<T> array,
            Predicate<T> predicate, 
            Func<T, string> itemPrefixFactory,
            Func<T, string> itemSuffixFactory,
            string itemLinePrefix = null, 
            string itemLineSuffix = null)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (array.Count() == 0)
                return string.Empty;

            var linePrefix = itemLinePrefix ?? string.Empty;
            var lineSuffix = itemLineSuffix ?? string.Empty;

            var result = new StringBuilder();

            foreach (var obj in array)
            {
                if (predicate != null && !predicate(obj))
                    continue;

                var objStr = obj != null ? obj.ToString() : "[null]";

                var prefix = itemPrefixFactory != null ? itemPrefixFactory(obj) ?? string.Empty : string.Empty;
                var suffix = itemSuffixFactory != null ? itemSuffixFactory(obj) ?? string.Empty : string.Empty;

                result.Append(prefix);

                foreach (var objLine in Regex.Split(objStr, Environment.NewLine))
                {
                    if (objLine == string.Empty)
                        continue;

                    result
                        .Append(linePrefix)
                        .Append(objLine)
                        .AppendLine(lineSuffix);
                }

                result.Append(suffix);
            }

            return result.ToString();
        }
    }

    [TestFixture]
    internal class ArrayExtensionTest
    {
        private class InnerClass
        {
            public string Name { get; set; }

            public string[] Field = new[] { "inner1", "inner2" };

            public override string ToString()
            {
                return "Inner: " + Environment.NewLine + Field.GetDescription("    ", string.Empty);
            }
        }

        [Test]
        public void GetDesctiptionTest()
        {
            var arr = new[] {"test1", "test2", "test3"};

            var description = arr.GetDescription("_", "?");

            var expected =
                @"_test1?
_test2?
_test3?
";

            Assert.AreEqual(expected, description);

            var arr2 = new[] {new InnerClass { Name = "Name1" }, new InnerClass { Name = "Name2" }};

            description = arr2.GetDescription("    ", "?");

            expected =
                @"    Inner: ?
        inner1?
        inner2?
    Inner: ?
        inner1?
        inner2?
";

            Assert.AreEqual(expected, description);

            description = arr2.GetDescription(@class => @class.Name + ". " + Environment.NewLine,
                                              innerClass => "__" + Environment.NewLine,
                                              "**", "?");

            expected =
                @"Name1. 
**Inner: ?
**    inner1?
**    inner2?
__
Name2. 
**Inner: ?
**    inner1?
**    inner2?
__
";

            Assert.AreEqual(expected, description);
        }
    }
}
