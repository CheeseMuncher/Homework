using RomanNumerals;

var validator = new InputValidator();
var input = "";
var validationOutput = validator.IsValid(input);

while (!validationOutput.IsValid)
{
    Console.WriteLine("Type an integer between 1 and 2000 and press enter");
    input = Console.ReadLine();
    validationOutput = validator.IsValid(input);
    if (!validationOutput.IsValid)
        Console.WriteLine(validationOutput.Message);
    else
        Console.WriteLine(new RomanConverter().ConvertToRoman(int.Parse(input)));
}
