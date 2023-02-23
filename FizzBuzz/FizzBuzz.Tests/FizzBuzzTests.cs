namespace FizzBuzz.Tests;

public class FizzBuzzTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(11)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(16)]
    public void Evaluate_ReturnsInput_IfNotMultiple(int input)
    {
        var result = FizzBuzz.Evaluate(input);
        result.Should().Be(input.ToString());
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    [InlineData(12)]
    [InlineData(18)]
    public void Evaluate_ReturnsFizz_IfMultipleOfThreeOnly(int input)
    {
        var result = FizzBuzz.Evaluate(input);
        result.Should().Be("Fizz");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void Evaluate_ReturnsBuzz_IfMultipleOfFiveOnly(int input)
    {
        var result = FizzBuzz.Evaluate(input);
        result.Should().Be("Buzz");
    }


    [Theory]
    [InlineData(15)]
    [InlineData(30)]
    public void Evaluate_ReturnsFizzBuzz_IfMultipleOfThreeAndFive(int input)
    {
        var result = FizzBuzz.Evaluate(input);
        result.Should().Be("FizzBuzz");
    }    

}