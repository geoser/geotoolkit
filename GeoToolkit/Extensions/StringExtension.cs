using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace GeoToolkit.Extensions
{
    public static class StringExtension
    {
        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            return source.IndexOf(value, comparison) >= 0;
        }

        public static string CutLeft(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            if (source.Length >= length)
                return source.Substring(0, length);

            return source;
        }

        /// <summary>
        /// Возвращает строку, содержащую только цифры из исходной строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ExtractDigits(this string str)
        {
            return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"\D", string.Empty);
        }

        /// <summary>
        /// Преобразовывает киррилические символы в сходные по написанию латинские.
        /// Символы, не имеющие аналогов по написанию, оставляются без изменений.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TranslitSimilar(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var dic =
                new Dictionary<char, char>
                    {
                        {'а', 'a'},
                        {'в', 'b'},
                        {'е', 'e'},
                        {'к', 'k'},
                        {'м', 'm'},
                        {'н', 'n'},
                        {'о', 'o'},
                        {'р', 'p'},
                        {'с', 'c'},
                        {'т', 't'},
                        {'у', 'y'},
                        {'х', 'x'}
                    };

            var sb = new StringBuilder(str.ToLowerInvariant());

            foreach (var kvPair in dic)
                sb.Replace(kvPair.Key, kvPair.Value);

            return sb.ToString();
        }
    }

    [TestFixture]
    public class StringExtensionTest
    {
        [Test]
        public void CutLeftTest()
        {
            const string str = "1234567890";

            Assert.AreEqual("12345", str.CutLeft(5));
            Assert.IsNull(((string)null).CutLeft(5));
            Assert.IsEmpty(string.Empty.CutLeft(5));
            Assert.AreEqual(str, str.CutLeft(15));
        }
    }
}