using FluentAssertions;
using MetaEdit.Conventions;
using System;
using Xunit;

namespace MetaEditTests.Conventions
{
    public class MediaInfoConventionTests
    {
        [Theory]
        [InlineData("1 h 24 min", 0, 1, 24, 0, 0)]
        [InlineData("1 min 12 s", 0, 0, 1, 12, 0)]
        [InlineData("59 s 800 ms", 0, 0, 0, 59, 800)]
        [InlineData("500 ms", 0, 0, 0, 0, 500)]
        public void GetTimeSpan_ConvertsTimeSpanCorrectly(string input, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            // Act
            var result = MediaInfoConvention.GetTimeSpan(input);

            // Assert
            result.Should().Be(new TimeSpan(days, hours, minutes, seconds, milliseconds));
        }
    }
}