namespace RomanNumerals;

public class RomanConverter
{
    private readonly string[] _digits = new [] { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
    private readonly string[] _tens = new [] { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
    private readonly string[] _hundreds = new [] { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
    private readonly string[] _thousands = new [] { "", "M", "MM" };

    public string ConvertToRoman(int input)
    {
        var digits = Enumerable.Range(0, 4).Select(i => GetDigit(input, i)).ToArray();
        return $"{_thousands[digits[3]]}{_hundreds[digits[2]]}{_tens[digits[1]]}{_digits[digits[0]]}";
    }

    private int GetDigit(int input, int magnitude) => (input / (int)Math.Pow(10, magnitude)) % 10;
}