using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace GeoToolkit
{
    public class StringValidator
    {
        public readonly string[] RequiredWords;
        public readonly string[] RestrictedWords;

        public bool WholeWords;
        public bool IgnoreCase;
        public RequiredModeEnum RequiredMode = RequiredModeEnum.All;

        public StringValidator(string[] restricted, string[] required)
        {
            if (restricted == null)
                throw new ArgumentNullException("restricted");

            if (required == null)
                throw new ArgumentNullException("required");

            if (restricted.Any(s => string.IsNullOrEmpty(s)))
                throw new ArgumentException("Non of the restricted array item can be null or empty", "restricted");

            if (required.Any(s => string.IsNullOrEmpty(s)))
                throw new ArgumentException("Non of the required array item can be null or empty", "required");

            RestrictedWords = restricted;
            RequiredWords = required;
        }

        public StringValidator(string[] restricted) : this(restricted, new string[0])
        {
        }

        public bool Validate(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str == string.Empty && RequiredWords.Length > 0)
                return false;

            return CompareRegex(str);
        }

        private bool CompareRegex(string str)
        {
            var options = RegexOptions.CultureInvariant | (IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            if (RestrictedWords.Any(s => Regex.IsMatch(str, WholeWords ? @"\b" + s + @"\b" : s, options)))
                return false;

            switch (RequiredMode)
            {
                case RequiredModeEnum.All:
                    if (RequiredWords.Any(s => !Regex.IsMatch(str, WholeWords ? @"\b" + s + @"\b" : s, options)))
                        return false;
                    break;
                case RequiredModeEnum.Any:
                    if (RequiredWords.All(s => !Regex.IsMatch(str, WholeWords ? @"\b" + s + @"\b" : s, options)))
                        return false;
                    break;
            }

            return true;
        }

        public static bool ValidateRestricted(string str, params string[] restricted)
        {
            return new StringValidator(restricted).Validate(str);
        }

        public static bool ValidateRestricted(string str, string[] restricted, 
            bool ignoreCase = false, bool wholeWord = false)
        {
            return new StringValidator(restricted)
                       {
                           IgnoreCase = ignoreCase,
                           WholeWords = wholeWord
                       }
                .Validate(str);
        }

        public static bool ValidateRequired(string str, params string[] required)
        {
            return new StringValidator(new string[0], required).Validate(str);
        }

        public static bool ValidateRequired(string str, string[] required, 
            bool ignoreCase = false, bool wholeWord = false, 
            RequiredModeEnum requiredMode = RequiredModeEnum.All)
        {
            return new StringValidator(new string[0], required)
                       {
                           IgnoreCase = ignoreCase,
                           WholeWords = wholeWord,
                           RequiredMode = requiredMode
                       }
                .Validate(str);
        }

        public enum RequiredModeEnum
        {
            Any,
            All
        }
    }

    [TestFixture]
    public class StringValidatorTest
    {
        [Test]
        public void StringValidatorTestTest()
        {
            var validator = new StringValidator(new[] {"res1", "res2"}, new[] {"req1", "req2"});
            validator.WholeWords = true;

            Assert.IsFalse(validator.Validate("res1"));
            Assert.IsFalse(validator.Validate("res2"));
            Assert.IsFalse(validator.Validate("res1  res2 req1 req2"));
            Assert.IsTrue(validator.Validate("res1res2 req1 req2"));
            Assert.IsFalse(validator.Validate("res1res2 Req1 req2"));
            Assert.IsFalse(validator.Validate("res1" + Environment.NewLine + "res2"));
            Assert.IsFalse(validator.Validate("some text"));
            Assert.IsFalse(validator.Validate("req1req2"));
            Assert.IsFalse(validator.Validate(" sadefwe req1 ewr"));

            Assert.IsTrue(validator.Validate("req1 req2"));
            Assert.IsTrue(validator.Validate(" sdfs  req1 req2  wefsadfa"));

            validator.WholeWords = false;

            Assert.IsFalse(validator.Validate("res1"));
            Assert.IsFalse(validator.Validate("res2"));
            Assert.IsFalse(validator.Validate("res1 res2 req1 req2"));
            Assert.IsFalse(validator.Validate("res1res2 req1 req2"));
            Assert.IsFalse(validator.Validate("res1" + Environment.NewLine + "res2"));
            Assert.IsFalse(validator.Validate("some text"));
            Assert.IsTrue(validator.Validate("req1req2"));
            Assert.IsFalse(validator.Validate(" sadefwe req1 ewr"));

            Assert.IsTrue(validator.Validate("req1 req2"));
            Assert.IsTrue(validator.Validate(" sdfs  req1 req2  wefsadfa"));

            validator.IgnoreCase = true;

            Assert.IsFalse(validator.Validate("reS1, Req1, rEq2"));

            validator.RequiredMode = StringValidator.RequiredModeEnum.Any;

            Assert.IsTrue(validator.Validate("req2"));

            validator = new StringValidator(new string[] {"res1"});

            Assert.IsTrue(validator.Validate("test"));
            Assert.IsFalse(validator.Validate("asdf we wefs res1test"));

            validator.WholeWords = true;
            Assert.IsTrue(validator.Validate("lskdfj er res1test"));

            Assert.IsTrue(_nameValidator.Validate("Форкаст"));
        }

        private static readonly string[] _restrictedWords =
            new[]
                {
                    "To Be Placed", "AvB", "Stall", "Place", "RFC", "F/C", "Daily Win Dist", "Nov",
                    "1f", "2f", "3f", "4f", "5f", "6f", "7f", "1m1f", "1m2f", "1m3f"
                };

        private static readonly StringValidator _nameValidator =
            new StringValidator(_restrictedWords)
            {
                IgnoreCase = true,
                WholeWords = true
            };
    }
}