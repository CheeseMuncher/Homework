using Finance.Utils;
using FluentAssertions;
using FluentAssertions.Extensions;
using System;
using Xunit;

namespace Finance.Tests;

public class DateTimeExtensionsTests : TestFixture
{
    [Fact]
    public void ToDateTime_ReturnsCorrectDateTime()
    {
        // Arrange
        long tick = 1640474446;        

        // Act
        var result = tick.UnixToDateTime();

        // Assert
        result.Should().Be(25.December(2021).At(23,20,46));
    }

    [Fact]
    public void ToUnixTimeStamp_ReturnsCorrectValue()
    {
        // Arrange
        var instant = new DateTime(2021, 12, 25, 23, 20, 46);

        // Act
        var result = instant.ToUnixTimeStamp();

        // Assert
        result.Should().Be(1640474446);
    }
}