using System;
#if DEBUG
using NUnit.Framework;
#endif

namespace GeoToolkit.FuzzyLogic
{
    public static class FuzzyStringComparer
    {
        /// <summary>
        /// Calculates distance between <paramref name="source"/> and <paramref name="target"/>
        /// required for convertion from the first to the second using Needleman-Wunch algorithm.
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="target">Target string</param>
        /// <param name="gap">Weight factor for insertion and deletion operations.</param>
        /// <param name="comparisonType">Comparison type</param>
        /// <returns></returns>
        public static int GetDistance(string source, string target, int gap, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (target == null)
                throw new ArgumentNullException("target");

            if (source == target)
                return 0;

            if (source == string.Empty)
                return target.Length * gap;

            if (target == string.Empty)
                return source.Length * gap;

            var matrix = new int[source.Length + 1, target.Length + 1];

            int sourceUpperBound = matrix.GetUpperBound(0);
            int targetUpperBound = matrix.GetUpperBound(1);

            for (int i = 0; i <= sourceUpperBound; i++)
                matrix[i, 0] = i;

            for (int j = 0; j <= targetUpperBound; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= sourceUpperBound; i++)
                for (int j = 1; j <= targetUpperBound; j++)
                {
                    int cost = string.Equals(source[i - 1].ToString(), target[j - 1].ToString(), comparisonType) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(
                            matrix[i - 1, j] + gap, // deletion
                            matrix[i, j - 1] + gap), // insertion
                        matrix[i - 1, j - 1] + cost); // substitution
                }

            return matrix[sourceUpperBound, targetUpperBound];
        }

        /// <summary>
        /// Calculates distance between <paramref name="source"/> and <paramref name="target"/>
        /// required for convertion from the first to the second using Levenshtein algorithm.
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="target">Target string</param>
        /// <param name="comparisonType">Comparison type</param>
        /// <returns></returns>
        public static int GetDistance(string source, string target, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            return GetDistance(source, target, 1, comparisonType);
        }
    }

#if DEBUG
    [TestFixture]
    public class FuzzyStringComparerTest
    {
        [Test]
        public void GetDistanceTest()
        {
            Assert.AreEqual(4, FuzzyStringComparer.GetDistance(string.Empty, "test"));
            Assert.AreEqual(4, FuzzyStringComparer.GetDistance("test", string.Empty));
            Assert.AreEqual(0, FuzzyStringComparer.GetDistance(string.Empty, string.Empty));

            Assert.AreEqual(0, FuzzyStringComparer.GetDistance("test", "test"));
            Assert.AreEqual(1, FuzzyStringComparer.GetDistance("test", "test1"));
            Assert.AreEqual(1, FuzzyStringComparer.GetDistance("test", "tes"));
            Assert.AreEqual(1, FuzzyStringComparer.GetDistance("test", "tesr"));
            Assert.AreEqual(3, FuzzyStringComparer.GetDistance("русский", "русс"));
            Assert.AreEqual(2, FuzzyStringComparer.GetDistance("русский", "русшски"));

            Assert.AreEqual(4, FuzzyStringComparer.GetDistance("test", "TEST"));
            Assert.AreEqual(0, FuzzyStringComparer.GetDistance("test", "TEST", StringComparison.InvariantCultureIgnoreCase));
            Assert.AreEqual(4, FuzzyStringComparer.GetDistance("тест", "ТЕСТ"));
            Assert.AreEqual(0, FuzzyStringComparer.GetDistance("тест", "ТЕСТ", StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(8, FuzzyStringComparer.GetDistance(string.Empty, "test", 2));
            Assert.AreEqual(8, FuzzyStringComparer.GetDistance("test", string.Empty, 2));

            Assert.AreEqual(2, FuzzyStringComparer.GetDistance("test", "test1", 2));
            Assert.AreEqual(2, FuzzyStringComparer.GetDistance("test", "tes", 2));
            Assert.AreEqual(1, FuzzyStringComparer.GetDistance("test", "tesr", 2));
            Assert.AreEqual(6, FuzzyStringComparer.GetDistance("русский", "русс", 2));
            Assert.AreEqual(4, FuzzyStringComparer.GetDistance("русский", "русшски", 2));
            Assert.AreEqual(3, FuzzyStringComparer.GetDistance("русский", "русшки", 2));
        }
    }
#endif
}
