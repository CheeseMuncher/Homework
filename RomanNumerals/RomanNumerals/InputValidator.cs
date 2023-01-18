namespace RomanNumerals;

public class InputValidator
{
    private const int MinValidValue = 1;
    private const int MaxValidValue = 2000;

    public (bool IsValid, string Message) IsValid(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out var value))
            return (false, "Input was not an integer, only integers are supported");

        if (value < MinValidValue || MaxValidValue < value)
            return (false, $"Input was outside supported range, only integers between {MinValidValue} and {MaxValidValue} are supported");

        return (true, "");
    }
}
