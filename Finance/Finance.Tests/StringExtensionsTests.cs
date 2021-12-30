using Finance.Utils;
using FluentAssertions;
using Xunit;

namespace Finance.Tests;

public class StringExtensionsTests : TestFixture
{
    [Theory]
    [InlineData("GE", "GE")]
    [InlineData("HSBA.L", "HSBA")]
    [InlineData("VAL.L", "VAL")]
    public void HandleIndex_TrimsSuffix(string input, string output)
    {
        // Arrange
        // Act
        var result = input.HandleIndex();

        // Assert
        result.Should().Be(output);
    }
}