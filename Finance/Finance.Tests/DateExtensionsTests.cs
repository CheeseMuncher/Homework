using Finance.Utils;
using FluentAssertions;
using System;
using Xunit;

namespace Finance.Tests;

public class DateExtensionsTests : TestFixture
{
    [Theory]
    [InlineData(DayOfWeek.Saturday)]
    [InlineData(DayOfWeek.Sunday)]
    public void IsWeekday_ReturnsFalse_IfWeekend(DayOfWeek dayOfWeek)
    {
        // Arrange
        var date = Create<DateTime>();
        date = date.AddDays((int)dayOfWeek - (int)date.DayOfWeek);        

        // Act
        var result = date.IsWeekday();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(DayOfWeek.Monday)]
    [InlineData(DayOfWeek.Tuesday)]
    [InlineData(DayOfWeek.Wednesday)]
    [InlineData(DayOfWeek.Thursday)]
    [InlineData(DayOfWeek.Friday)]
    public void IsWeekday_ReturnsTrue_IfWeekday(DayOfWeek dayOfWeek)
    {
        // Arrange
        var date = Create<DateTime>();
        date = date.AddDays((int)dayOfWeek - (int)date.DayOfWeek);

        // Act
        var result = date.IsWeekday();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsXmas_ReturnsFalse_IfNotXmasDay()
    {
        // Arrange
        var date = Create<DateTime>();
        date = date.AddDays(date.Day == 25 ? 1 : 0);

        // Act
        var result = date.IsXmas();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsXmas_ReturnsTrue_IfXmasDay()
    {
        // Arrange
        var date = new DateTime(Create<int>(), 12, 25);

        // Act
        var result = date.IsXmas();

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(2022, 01, 02)] // Monday
    [InlineData(2018, 01, 02)] // Tuesday
    [InlineData(2019, 01, 02)] // Wednesday
    [InlineData(2020, 01, 02)] // Thursday
    [InlineData(2015, 01, 02)] // Friday
    [InlineData(2011, 01, 01)] // Saturday
    [InlineData(2011, 01, 02)] // Sunday
    [InlineData(2022, 01, 01)] // Sunday
    [InlineData(2019, 01, 07)] // Monday
    [InlineData(2021, 01, 04)] // Monday
    public void IsFirstWorkday_ReturnsFalse_IfNotFirstWorkday(int year, int month, int day)
    {
        // Arrange
        var date = new DateTime(year, month, day);

        // Act
        var result = date.IsFirstWorkday();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(2018, 01, 01)] // Monday
    [InlineData(2019, 01, 01)] // Tuesday
    [InlineData(2020, 01, 01)] // Wednesday
    [InlineData(2015, 01, 01)] // Thursday
    [InlineData(2021, 01, 01)] // Friday
    [InlineData(2017, 01, 02)] // Monday
    [InlineData(2022, 01, 03)] // Monday
    public void IsFirstWorkday_ReturnsTrue_IfFirstWorkday(int year, int month, int day)
    {
        // Arrange
        var date = new DateTime(year, month, day);

        // Act
        var result = date.IsFirstWorkday();

        // Assert
        result.Should().BeTrue();
    }

}