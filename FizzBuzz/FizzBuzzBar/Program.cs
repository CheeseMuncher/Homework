using TwistedFizzBuzz;

var _twistedFizzBuzz = new TwistedFizzBuzz.TwistedFizzBuzz();
var rules = new []
{
    new FizzBuzzRule { Multiple = 5, Word = "Fizz" },
    new FizzBuzzRule { Multiple = 9, Word = "Buzz" },
    new FizzBuzzRule { Multiple = 27, Word = "Bar" }
};
_twistedFizzBuzz.SetRules(rules);

foreach(var text in _twistedFizzBuzz.GetRangeOutput(-20,127))
{
    Console.WriteLine(text);
}