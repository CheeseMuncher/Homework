using FluentAssertions;
using MetaEdit;
using System;
using Xunit;

namespace MetaEditTests
{
    public class TotalRecallConventionTests : TestFixture<TotalRecallConvention>
    {
        [Fact]
        public void Separators_ReturnsExpectedSeparators()
        {
            // Act
            var result = Sut.Separators;

            // Assert
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
            // Act
            Action action = () => Sut.GetCallType("Test");

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetDateTime_ConvertsDateCorrectly()
        {
            // Arrange
            var input1 = "2020-01-14@19-29-20";
            var input2 = "2021-02-15@18-28-19";

            // Act
            var result1 = Sut.GetDateTime(input1);
            var result2 = Sut.GetDateTime(input2);

            // Assert
            var expected1 = new DateTime(2020, 01, 14, 19, 29, 20);
            var expected2 = new DateTime(2021, 02, 15, 18, 28, 19);
            result1.Should().Be(expected1);
            result2.Should().Be(expected2);
        }
    }
}