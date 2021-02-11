﻿using FluentAssertions;
using MetaEdit;
using MetaEdit.Conventions;
using System;
using Xunit;

namespace MetaEditTests
{
    public class CallDataDecoderTests : TestFixture<CallDataDecoder>
    {
        public CallDataDecoderTests()
        {
            Inject(new TotalRecallConvention() as IDecodeConvention);
        }

        [Fact]
        public void DecodeFileName_DecodesFullFileNameCorrectly()
        {
            // Arrange
            var duration = "59 s 800 ms";
            var input = "2022-01-14@19-29-20_In_John.M.Smith (07123456789)";

            // Act
            var result = Sut.DecodeFileName(input, duration);

            // Assert
            result.CallTime.Should().Be(new DateTime(2022, 01, 14, 19, 29, 20));
            result.CallType.Should().Be(CallType.Received);
            result.ContactName.Should().Be("John.M.Smith");
            result.ContactNumber.Should().Be("07123456789");
            result.CallDuration.Should().Be(TimeSpan.Parse("00:00:59.800"));
        }

        [Fact]
        public void DecodeFileName_DecodesPartialFileNameCorrectly()
        {
            // Arrange
            var input = "2020-01-14@19-29-20_Out_Unknown.Contact (PrivateNumber)";

            // Act
            var result = Sut.DecodeFileName(input);

            // Assert
            result.CallTime.Should().Be(new DateTime(2020, 01, 14, 19, 29, 20));
            result.CallType.Should().Be(CallType.Dialed);
            result.ContactName.Should().BeNull();
            result.ContactNumber.Should().BeNull();
            result.CallDuration.Should().Be(TimeSpan.FromMilliseconds(0));
        }

        [Fact]
        public void DecodeFileName_ThrowsIfMultipleParamsPassedIn()
        {
            // Arrange
            var input = "2022-01-14@19-29-20_In_John.M.Smith (07123456789)";

            // Act
            Action action = () => Sut.DecodeFileName(input, "test1", "test2");

            // Assert
            Assert.Throws<ArgumentException>(action);
        }
    }
}