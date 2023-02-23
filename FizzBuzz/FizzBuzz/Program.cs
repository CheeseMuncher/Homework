// See https://aka.ms/new-console-template for more information
var fizzBuzz = new FizzBuzz.FizzBuzz();
foreach(var i in Enumerable.Range(1, 100))
{
    Console.WriteLine(fizzBuzz.Evaluate(i));
}
