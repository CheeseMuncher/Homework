using Finance.Utils;
using FluentAssertions;
using Xunit;

namespace Finance.Tests;

public class DecimalExtensionsExtensionsTests : TestFixture
{
    [Theory]
    [InlineData(0.123456789, 6, 0.123457)]
    [InlineData(0.123456111, 6, 0.123456)]
    [InlineData(0.123456789, 7, 0.1234568)]
    public void IsWeekday_ReturnsFalse_IfWeekend(decimal input, int digits, decimal output)
    {
        // Arrange
        // Act
        var result = input.RoundToSignificantDigits(digits);

        // Assert
        result.Should().Be(output);
    }
}