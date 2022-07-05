using FluentAssertions;
using MetaEdit;
using MetaEdit.Conventions;
using System;
using System.Linq;
using Xunit;

namespace MetaEditTests.Conventions
{
    public class SuperBackupConventionTests : TestFixture<SuperBackupConvention>
    {
        [Fact]
        public void Extensions_ReturnsExpectedExtensions()
        {
            // Act
            var result = Sut.Extensions;

            // Assert
            result.Count.Should().Be(1);
            result.Single().Should().Be(".csv");
        }

        [Fact]
        public void Separators_ReturnsExpectedSeparators()
        {
            // Act
            var result = Sut.Separators;
            result.Single().Should().Be(",");

            // Assert
            result.Count.Should().Be(1);
            result.Should().Contain(",");
        }

        [Fact]
        public void Convention_ReturnsExpectedConvention()
        {
            // Act
            var result = Sut.Convention;

            // Assert
            result.Length.Should().Be(5);
            result[0].Should().Be(nameof(CallData.ContactName));
            result[1].Should().Be(nameof(CallData.ContactNumber));
            result[2].Should().Be(nameof(CallData.CallTime));
            result[3].Should().Be(nameof(CallData.CallType));
            result[4].Should().Be(nameof(CallData.CallDuration));
        }

        [Theory]
        [InlineData(CallType.Received)]
        [InlineData(CallType.Dialed)]
        [InlineData(CallType.Missed)]
        public void GetCallType_ConvertsInputCorrectly(CallType expected)
        {
            // Act
            var result = Sut.GetCallType($"{expected}");

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetCallType_ThrowsArgumentException_IfInputUnexpected()
        {
            // Arrange
            var input = Create<string>();

            // Act
            Action action = () => Sut.GetCallType(input);

            // Assert
            var ex = Assert.Throws<ArgumentException>(action);
            ex.Message.Should().Contain(nameof(SuperBackupConvention));
            ex.Message.Should().Contain(nameof(SuperBackupConvention.GetCallType));
            ex.Message.Should().Contain(input);
        }

        [Theory]
        [InlineData("06 Jun 2020 15:59", 2020, 06, 06, 15, 59, 00)]
        [InlineData("23 Jan 2021 09:58", 2021, 01, 23, 09, 58, 00)]
        public void GetDateTime_ConvertsDateCorrectly(string input, int year, int month, int day, int hour, int minute, int second)
        {
            // Act
            var result = Sut.GetDateTime(input);

            // Assert
            result.Should().Be(new DateTime(year, month, day, hour, minute, second));
        }

        [Fact]
        public void GetTimeSpan_ConvertsTimeSpanCorrectly()
        {
            // Arrange
            var input = "14:07:20";

            // Act
            var result = Sut.GetTimeSpan(input);

            // Assert
            var expected = new TimeSpan(14, 7, 20);
            result.Should().Be(expected);
        }
    }
}