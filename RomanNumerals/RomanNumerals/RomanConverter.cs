namespace RomanNumerals;

public class RomanConverter
{
    private readonly string[] _digits = new [] { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
    private readonly string[] _tens = new [] { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
    private readonly string[] _hundreds = new [] { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };

    public string ConvertToRoman(int input)
    {
        var digits = _digits[input % 10];
        var tens = _tens[(input / 10) % 10];
        var hundreds = _hundreds[(input / 100) % 10];
        var prefix = input > 999 ? "M" : "";
        return $"{prefix}{hundreds}{tens}{digits}";
    }
}