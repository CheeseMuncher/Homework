using FluentAssertions;
using MetaEdit;
using MetaEdit.Conventions;
using System;
using System.Linq;
using Xunit;

namespace MetaEditTests.Conventions
{
    public class AllCallRecorderConventionTests : TestFixture<AllCallRecorderConvention>
    {
        [Fact]
        public void Extensions_ReturnsExpectedExtensions()
        {
            // Act
            var result = Sut.Extensions;

            // Assert
            result.Count.Should().Be(1);
            result.Single().Should().Be(".3gp");
        }

        [Fact]
        public void Separators_ReturnsExpectedSeparators()
        {
            // Act
            var result = Sut.Separators;

            // Assert
            result.Count.Should().Be(1);
            result.Single().Should().Be("o");
        }

        [Fact]
        public void Convention_ReturnsExpectedConvention()
        {
            // Act
            var result = Sut.Convention;

            // Assert
            result.Length.Should().Be(2);
            result[0].Should().Be(nameof(CallData.CallTime));
            result[1].Should().Be(nameof(CallData.ContactNumber));
        }

        [Theory]
        [InlineData("120101010233", 2012, 01, 01, 01, 02, 33)]
        public void GetDateTime_ConvertsDateCorrectly(string input, int year, int month, int day, int hour, int minute, int second)
        {
            // Act
            var result = Sut.GetDateTime(input);

            // Assert
            result.Should().Be(new DateTime(year, month, day, hour, minute, second));
        }
    }
}