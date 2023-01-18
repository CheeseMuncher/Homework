using FluentAssertions;
using RomanNumerals;
using Xunit;

namespace RomanNumeralsTests;

public class InputValidatorTests
{
    private readonly InputValidator _validator = new ();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("some text")]
    [InlineData("12.34")]
    public void IsValid_ReturnsFalseWithMessage_IfInputNotAnInteger(string input)
    {
        // Act
        var result = _validator.IsValid(input);

        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Input was not an integer, only integers are supported");
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("2001")]
    [InlineData("2002")]
    public void IsValid_ReturnsFalseWithMessage_IfInputIntegerOutsideRange(string input)
    {
        // Act
        var result = _validator.IsValid(input);

        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Input was outside supported range, only integers between 1 and 2000 are supported");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("10")]
    [InlineData("2000")]
    public void IsValid_ReturnsTrueWithoutMessage_IfInputValid(string input)
    {
        // Act
        var result = _validator.IsValid(input);

        result.IsValid.Should().BeTrue();
        result.Message.Should().BeEmpty();
    }
}
