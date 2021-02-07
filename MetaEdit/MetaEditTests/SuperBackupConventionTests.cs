using FluentAssertions;
using MetaEdit;
using MetaEdit.Conventions;
using System;
using Xunit;

namespace MetaEditTests
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
            result.Should().Contain(".csv");
        }

        [Fact]
        public void Separators_ReturnsExpectedSeparators()
        {
            // Act
            var result = Sut.Separators;

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
            // Act
            Action action = () => Sut.GetCallType("Test");

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetDateTime_ConvertsDateCorrectly()
        {
            // Arrange
            var input1 = "06 Jun 2020 15:59";
            var input2 = "23 Jan 2021 09:58";

            // Act
            var result1 = Sut.GetDateTime(input1);
            var result2 = Sut.GetDateTime(input2);

            // Assert
            var expected1 = new DateTime(2020, 06, 06, 15, 59, 00);
            var expected2 = new DateTime(2021, 01, 23, 09, 58, 00);
            result1.Should().Be(expected1);
            result2.Should().Be(expected2);
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