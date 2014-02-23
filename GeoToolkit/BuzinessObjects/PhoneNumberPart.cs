using System;

namespace GeoToolkit.BuzinessObjects
{
    [Flags]
    public enum PhoneNumberPart
    {
        None = 0,
        CountryCode = 1,
        Prefix = 2,
        Part1 = 4,
        Part2 = 8,
        Part3 = 16,
        CountryCodeAndPrefix = CountryCode | Prefix,
        Full = CountryCodeAndPrefix | Part1 | Part2 | Part3
    }
}