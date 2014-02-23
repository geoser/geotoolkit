using System;
using System.Globalization;
using System.Text.RegularExpressions;
using GeoToolkit.Extensions;
using NUnit.Framework;

namespace GeoToolkit.BuzinessObjects
{
    public struct PhoneNumber
    {
        private static readonly Regex _regex = new Regex(
                @"(((\+|8(\s|-)*10)(\s|-)*(?<countrycode>\d{1,3}))|8?)(\s|-)*(?<prefix>(\((\d(\s|-)*){3}\))|((\d(\s|-)*){3}))?(\s|-)*(?<part1>(\d(\s|-)*){3})(?<part2>(\d(\s|-)*){2})(?<part3>(\d(\s|-)*){2})(\s|-)*$",
                RegexOptions.Compiled);

        public static readonly PhoneNumber Empty = new PhoneNumber();

        private const ushort _CODE_RUSSIA = 7;
        private const ushort _CODE_MOSCOW = 495;

        public PhoneNumber(ushort countryCode, ushort prefix, ushort part1, ushort part2, ushort part3) : this()
        {
            if (countryCode > 999)
                throw new ArgumentException("Country code must be between 0 and 999");

            if (prefix > 999)
                throw new ArgumentException("Prefix must be between 0 and 999", "prefix");

            if (part1 > 999)
                throw new ArgumentException("part1 be between 0 and 999", "prefix");

            if (part2 > 99)
                throw new ArgumentException("part2 must be between 0 and 99", "prefix");

            if (part3 > 999)
                throw new ArgumentException("part3 must be between 0 and 99", "prefix");

            CountryCode = countryCode == default(ushort) ? _CODE_RUSSIA : countryCode;
            Prefix = prefix;
            Part1 = part1;
            Part2 = part2;
            Part3 = part3;
        }

        public ushort CountryCode { get; private set; }
        public ushort Prefix { get; private set; }

        public ushort Part1 { get; private set; }
        public ushort Part2 { get; private set; }
        public ushort Part3 { get; private set; }

        public PhoneNumberPart Parts;

        public bool IsEmpty
        {
            get { return Equals(Empty); }
        }

        public static bool TryParse(string str, out PhoneNumber result)
        {
            result = new PhoneNumber();

            if (string.IsNullOrEmpty(str))
                return false;
            
            Match match = _regex.Match(str);

            if (!match.Success)
                return false;

            string countryCode = match.Groups["countrycode"].Success
                                    ? match.Groups["countrycode"].Value
                                    : string.Empty;
            string prefix = match.Groups["prefix"].Success
                                    ? match.Groups["prefix"].Value
                                    : string.Empty;
            string part1 = match.Groups["part1"].Success
                                ? match.Groups["part1"].Value
                                : string.Empty;
            string part2 = match.Groups["part2"].Success
                                ? match.Groups["part2"].Value
                                : string.Empty;
            string part3 = match.Groups["part3"].Success
                                ? match.Groups["part3"].Value
                                : string.Empty;

            if (countryCode != string.Empty && prefix == string.Empty)
                return false;

            if (countryCode == string.Empty)
                countryCode = _CODE_RUSSIA.ToString(CultureInfo.InvariantCulture);

            if (prefix == string.Empty)
                prefix = _CODE_MOSCOW.ToString(CultureInfo.InvariantCulture);

            result = new PhoneNumber(Convert.ToUInt16(countryCode.ExtractDigits()),
                                     Convert.ToUInt16(prefix.ExtractDigits()),
                                     Convert.ToUInt16(part1.ExtractDigits()),
                                     Convert.ToUInt16(part2.ExtractDigits()),
                                     Convert.ToUInt16(part3.ExtractDigits()))
                         {
                             Parts = PhoneNumberPart.Full
                         };


            return true;
        }

        public static PhoneNumber Parse(string str)
        {
            PhoneNumber result;

            if (TryParse(str, out result))
                return result;

            throw new FormatException("Cannot convert string to phone number");
        }

        public override string ToString()
        {
            return ToString(NumberFormat.Canonical);
        }

        public string ToString(NumberFormat format)
        {
            switch (format)
            {
                case NumberFormat.Canonical:
                    return string.Format("+{0} {1:000} {2:000}{3:00}{4:00}", 
                        CountryCode, Prefix, Part1, Part2, Part3);
                case NumberFormat.UserFrienly:
                    return string.Format("+{0} ({1:000}) {2:000}-{3:00}-{4:00}",
                        CountryCode, Prefix, Part1, Part2, Part3);
                case NumberFormat.Short:
                    return string.Format("{0}{1:000}{2:000}{3:00}{4:00}",
                        CountryCode, Prefix, Part1, Part2, Part3);
                default:
                    throw new ApplicationException("Should not be here");
            }
        }

        public enum NumberFormat
        {
            UserFrienly,
            Canonical,
            Short
        }
    }

    [TestFixture]
    public class PhoneTest
    {
        private static void AssertPhoneNumberInfo(PhoneNumber number, ushort exCountryCode, ushort exPrefix,
                                                  ushort exPart1, ushort exPart2, ushort exPart3)
        {
            Assert.IsNotNull(number);
            Assert.AreEqual(exCountryCode, number.CountryCode);
            Assert.AreEqual(exPrefix, number.Prefix);
            Assert.AreEqual(exPart1, number.Part1);
            Assert.AreEqual(exPart2, number.Part2);
            Assert.AreEqual(exPart3, number.Part3);
        }

        [Test]
        public void ExtractDigits()
        {
            Assert.AreEqual("12345", "1hj234i- 5".ExtractDigits());
        }

        [Test]
        public void ParseNumber()
        {
            AssertPhoneNumberInfo(PhoneNumber.Parse("9 2 6  123 4567"), 7, 926, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("9261234567"), 7, 926, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("79261234567"), 7, 926, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("8-9 2 6  123 45 67"), 7, 926, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("8  10 7-9 2 6  123 45 67"), 7, 926, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("+7-9 2 6  123 45 67"), 7, 926, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse(" +42-9 -2 6  123 45 67"), 42, 926, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse(" +423-9 -2 6  123 45 67"), 423, 926, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("1234567"), 7, 495, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("123-4567"), 7, 495, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("12-345-67"), 7, 495, 123, 45, 67);
            AssertPhoneNumberInfo(PhoneNumber.Parse("123-45-67"), 7, 495, 123, 45, 67);

            var phone = PhoneNumber.Parse("79260230567");

            Assert.AreEqual("+7 (926) 023-05-67", phone.ToString(PhoneNumber.NumberFormat.UserFrienly));

            Assert.IsFalse(PhoneNumber.TryParse("+7926123 45 6", out phone));
        }
    }
}
