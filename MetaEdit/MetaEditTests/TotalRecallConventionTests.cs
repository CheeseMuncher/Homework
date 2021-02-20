using FluentAssertions;
using MetaEdit;
using MetaEdit.Conventions;
using System;
using Xunit;

namespace MetaEditTests
{
    public class TotalRecallConventionTests : TestFixture<TotalRecallConvention>
    {
        [Fact]
        public void Extensions_ReturnsExpectedExtensions()
        {
            // Act
            var result = Sut.Extensions;

            // Assert
            result.Count.Should().Be(2);
            result.Should().Contain(".amr", ".mp3");
        }

        [Fact]
        public void Separators_ReturnsExpectedSeparators()
        {
            // Act
            var result = Sut.Separators;

            // Assert
            result.Count.Should().Be(3);
            result.Should().Contain("_");
            result.Should().Contain(" (");
            result.Should().Contain(")");
        }

        [Fact]
        public void Convention_ReturnsExpectedConvention()
        {
            // Act
            var result = Sut.Convention;

            // Assert
            result.Length.Should().Be(4);
            result[0].Should().Be(nameof(CallData.CallTime));
            result[1].Should().Be(nameof(CallData.CallType));
            result[2].Should().Be(nameof(CallData.ContactName));
            result[3].Should().Be(nameof(CallData.ContactNumber));
        }

        [Theory]
        [InlineData("In", CallType.Received)]
        [InlineData("Out", CallType.Dialed)]
        public void GetCallType_ConvertsInputCorrectly(string input, CallType expected)
        {
            // Act
            var result = Sut.GetCallType(input);

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
            ex.Message.Should().Contain(nameof(TotalRecallConvention));
            ex.Message.Should().Contain(nameof(TotalRecallConvention.GetCallType));
            ex.Message.Should().Contain(input);
        }

        [Theory]
        [InlineData("2020-01-14@19-29-20", 2020, 01, 14, 19, 29, 20)]
        [InlineData("2021-02-15@18-28-19", 2021, 02, 15, 18, 28, 19)]
        public void GetDateTime_ConvertsDateCorrectly(string input, int year, int month, int day, int hour, int minute, int second)
        {
            // Act
            var result = Sut.GetDateTime(input);

            // Assert
            result.Should().Be(new DateTime(year, month, day, hour, minute, second));
        }

        [Theory]
        [InlineData("1 h 24 min", 0, 1, 24, 0, 0)]
        [InlineData("1 min 12 s", 0, 0, 1, 12, 0)]
        [InlineData("59 s 800 ms", 0, 0, 0, 59, 800)]
        [InlineData("500 ms", 0, 0, 0, 0, 500)]
        public void GetTimeSpan_ConvertsTimeSpanCorrectly(string input, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            // Act
            var result = Sut.GetTimeSpan(input);

            // Assert
            result.Should().Be(new TimeSpan(days, hours, minutes, seconds, milliseconds));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Cheese")]
        public void GetTimeSpan_HandlesDodgyInputs(string input)
        {
            // Act
            var result = Sut.GetTimeSpan(input);

            // Assert
            result.Should().Be(new TimeSpan());
        }
    }
}