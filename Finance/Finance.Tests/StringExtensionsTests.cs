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
    [InlineData("GBPUSD=X", "GBPUSD")]
    public void HandleSuffix_TrimsSuffix(string input, string output)
    {
        // Arrange
        // Act
        var result = input.HandleSuffix();

        // Assert
        result.Should().Be(output);
    }
}