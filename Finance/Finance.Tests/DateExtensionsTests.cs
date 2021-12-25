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

    [Theory]
    [InlineData(2021, 12, 20)] // Monday
    [InlineData(2021, 12, 25)] // Saturday
    [InlineData(2022, 12, 25)] // Sunday
    public void IsXmasHoliday_ReturnsFalse_IfNotXmasHoliday(int year, int month, int day)
    {
        // Arrange        
        var date = new DateTime(year, month, day);

        // Act
        var result = date.IsXmasHoliday();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(2020, 12, 25)] // Friday
    [InlineData(2021, 12, 27)] // Monday
    [InlineData(2022, 12, 26)] // Monday
    [InlineData(2023, 12, 25)] // Monday
    public void IsXmasHoliday_ReturnsTrue_IfXmasHoliday(int year, int month, int day)
    {
        // Arrange
        var date = new DateTime(year, month, day);

        // Act
        var result = date.IsXmasHoliday();

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
    public void IsNewYearHoliday_ReturnsFalse_IfNotNewYearHoliday(int year, int month, int day)
    {
        // Arrange
        var date = new DateTime(year, month, day);

        // Act
        var result = date.IsNewYearHoliday();

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
    public void IsNewYearHoliday_ReturnsTrue_IfNewYearHoliday(int year, int month, int day)
    {
        // Arrange
        var date = new DateTime(year, month, day);

        // Act
        var result = date.IsNewYearHoliday();

        // Assert
        result.Should().BeTrue();
    }
}