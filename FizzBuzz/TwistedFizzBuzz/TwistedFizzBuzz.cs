using System.Text;
using System.Text.Json;

namespace TwistedFizzBuzz;

public class TwistedFizzBuzz
{
    private JsonSerializerOptions options = new ()
    {
        PropertyNameCaseInsensitive = true
    };

    private FizzBuzzRule[] Rules = new [] 
    {
        new FizzBuzzRule { Multiple = 3, Word = "Fizz" },
        new FizzBuzzRule { Multiple = 5, Word = "Buzz" }
    };

    public IEnumerable<string> GetRangeOutput(int minimum, int maximum)
    {
        var reverse = maximum - minimum < 0;
        var range = reverse 
            ? Enumerable.Range(0, minimum - maximum + 1).Select(i => minimum - i)
            : Enumerable.Range(minimum, maximum - minimum + 1);

        foreach(var i in range)
            yield return Evaluate(i);
    }

    public IEnumerable<string> GetSetOutput(int[] input)
    {
        foreach(var i in input)
            yield return Evaluate(i);
    }

    public void SetRules(FizzBuzzRule[] rules)
    {
        Rules = rules;
    }

    public bool SetRules(string json)
    {
        try
        {
            var rule = JsonSerializer.Deserialize<FizzBuzzRule>(json, options);
            if (rule is null || rule.Word is null || rule.Multiple == 0)
                throw new JsonException($"Unable to deserialise {json} to {nameof(FizzBuzzRule)} or input is invalid");

            SetRules(new [] { rule });
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error setting rules from json: {ex.Message}");
            return false;
        }
    }

    private string Evaluate(int input)
    {
        var sb = new StringBuilder();
        foreach(var rule in Rules)
            if (input % rule.Multiple == 0)
                sb.Append(rule.Word);

        return sb.Length == 0 ? $"{input}" : sb.ToString();
    }
}