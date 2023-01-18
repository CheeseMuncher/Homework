using FluentAssertions;
using RomanNumerals;
using Xunit;

namespace RomanNumeralsTests;

public class RomanConverterTests
{
    private static readonly Random Random = new();
    private static readonly int[] _digits = new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private static int RandomDigit() => _digits.OrderBy(_ => Random.Next()).First();
    private static string RomanDigits(IEnumerable<object[]> range, int digit) => digit == 0 ? "" : range.ToArray()[digit - 1][1].ToString();

    private readonly RomanConverter _converter = new ();

    [Theory]
    [MemberData(nameof(Digits))]
    public void ConvertToRoman_ConvertsDigitsCorrectly(int input, string output)
    {
        _converter.ConvertToRoman(input).Should().Be(output);
    }

    [Theory]
    [MemberData(nameof(AllDoubleDigits))]
    public void ConvertToRoman_ConvertsDoubleDigitsCorrectly(int input, string output)
    {
        _converter.ConvertToRoman(input).Should().Be(output);
    }

    [Theory]
    [MemberData(nameof(NineTripleDigits))]
    public void ConvertToRoman_ConvertsTripleDigitsCorrectly(int input, string output)
    {
        _converter.ConvertToRoman(input).Should().Be(output);
    }

    [Theory]
    [MemberData(nameof(NineTripleDigits))]
    public void ConvertToRoman_ConvertsNumbersAbove1000Correctly(int input, string output)
    {        
        _converter.ConvertToRoman(input + 1000).Should().Be("M" + output);
    }

    [Fact]    
    public void ConvertToRoman_Converts2000Correctly()
    {        
        _converter.ConvertToRoman(2000).Should().Be("MM");
    }

    private static object[] AddTestData(object[] first, object[] second) => new object[] { (int)first[0] + (int)second[0], (string)first[1] + (string)second[1] };

    public static IEnumerable<object[]> Digits()
    {
        yield return new object[] { 1, "I" };
        yield return new object[] { 2, "II" };
        yield return new object[] { 3, "III" };
        yield return new object[] { 4, "IV" };
        yield return new object[] { 5, "V" };
        yield return new object[] { 6, "VI" };
        yield return new object[] { 7, "VII" };
        yield return new object[] { 8, "VIII" };
        yield return new object[] { 9, "IX" };
    }

    private static IEnumerable<object[]> MultiplesOfTen()
    {
        yield return new object[] { 10, "X" };
        yield return new object[] { 20, "XX" };
        yield return new object[] { 30, "XXX" };
        yield return new object[] { 40, "XL" };
        yield return new object[] { 50, "L" };
        yield return new object[] { 60, "LX" };
        yield return new object[] { 70, "LXX" };
        yield return new object[] { 80, "LXXX" };
        yield return new object[] { 90, "XC" };
    }

    private static IEnumerable<object[]> MultiplesOfOneHundred()
    {
        yield return new object[] { 100, "C" };
        yield return new object[] { 200, "CC" };
        yield return new object[] { 300, "CCC" };
        yield return new object[] { 400, "CD" };
        yield return new object[] { 500, "D" };
        yield return new object[] { 600, "DC" };
        yield return new object[] { 700, "DCC" };
        yield return new object[] { 800, "DCCC" };
        yield return new object[] { 900, "CM" };
    }

    public static IEnumerable<object[]> AllDoubleDigits()
    {
        foreach (var multiple in MultiplesOfTen())
        {
            yield return new object[] { multiple[0], multiple[1] };
            foreach (var digit in Digits())
            {
                yield return AddTestData(multiple, digit);
            }
        }
    }

    public static IEnumerable<object[]> NineTripleDigits()
    {
        foreach (var hundreds in MultiplesOfOneHundred())
        {
            yield return AddTestData(hundreds, RandomUnder100());
        }
    }

    private static object[] RandomUnder100()
    {
        var tens = RandomDigit();
        var digit = RandomDigit();
        return new object[] { tens * 10 + digit, RomanDigits(MultiplesOfTen(), tens) + RomanDigits(Digits(), digit) };
    }
}
