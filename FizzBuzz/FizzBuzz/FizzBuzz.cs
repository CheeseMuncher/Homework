using TwistedFizzBuzz;

namespace FizzBuzz;

public class FizzBuzz
{
    private readonly TwistedFizzBuzz.TwistedFizzBuzz _twistedFizzBuzz = new();

    public FizzBuzz()
    {
        // Just in case someone changes how things are initialised in TwistedFizzBuzz
        _twistedFizzBuzz.SetRules(new [] 
        {
            new FizzBuzzRule { Multiple = 3, Word = "Fizz" },
            new FizzBuzzRule { Multiple = 5, Word = "Buzz" }
        });
    }

    public string Evaluate(int i) => _twistedFizzBuzz.GetRangeOutput(i, i).Single();
}
