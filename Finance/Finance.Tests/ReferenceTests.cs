using Finance.Utils;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Finance.Tests;

public class ReferenceTests : TestFixture
{
    [Fact]    
    public void GetMarketDays_ReturnsCorrectResult()
    {
        // Arrange
        var start = new DateTime(2021, 12, 01);
        var end = new DateTime(2022, 04, 30);

        // Act
        var result = Reference.GetMarketDays(start, end).ToArray();

        // Assert
        result.Count().Should().Be(105);
        result.Any(date => date.Month == 12 && date.Day == 25).Should().BeFalse();
        result.Any(date => date.Month == 01 && date.Day == 03).Should().BeFalse();
        result.Any(date => date.Month == 04 && date.Day == 15).Should().BeFalse();
    }
}