using FluentAssertions;
using MetaEdit;
using MetaEdit.Conventions;
using MetaEdit.Decoding;
using System;
using Xunit;

namespace MetaEditTests
{
    public class SuperBackupDecoderTests : TestFixture<SuperBackupDecoder>
    {
        public SuperBackupDecoderTests()
        {
            Inject(new SuperBackupConvention() as IDecodeConvention);
        }

        [Fact]
        public void DecodeFileName_DecodesFullFileNameCorrectly()
        {
            // Arrange
            var input = "NFU Mutual,01295811090,07 Sep 2020 16:47,Dialed,00:24:58";

            // Act
            var result = Sut.DecodeFileName(input);

            // Assert
            result.CallTime.Should().Be(new DateTime(2020, 09, 07, 16, 47, 00));
            result.CallType.Should().Be(CallType.Dialed);
            result.ContactName.Should().Be("NFU Mutual");
            result.ContactNumber.Should().Be("01295811090");
            result.CallDuration.Should().Be(TimeSpan.Parse("00:24:58"));
        }

        [Fact]
        public void DecodeFileName_DecodesPartialFileNameCorrectly()
        {
            // Arrange
            var input = "01223203138,01223203138,23 Jan 2021 09:58,Received,00:00:30";

            // Act
            var result = Sut.DecodeFileName(input);

            // Assert
            result.CallTime.Should().Be(new DateTime(2021, 01, 23, 09, 58, 00));
            result.CallType.Should().Be(CallType.Received);
            result.ContactName.Should().BeNull();
            result.ContactNumber.Should().Be("01223203138");
            result.CallDuration.Should().Be(TimeSpan.Parse("00:00:30"));
        }

        [Fact]
        public void DecodeFileName_ThrowsIfMultipleParamsPassedIn()
        {
            // Arrange
            var input = "01223203138,01223203138,23 Jan 2021 09:58,Received,00:00:30";
            var param1 = Create<string>();

            // Act
            Action action = () => Sut.DecodeFileName(input, param1);

            // Assert
            var ex = Assert.Throws<ArgumentException>(action);
            ex.Message.Should().Contain(nameof(SuperBackupDecoder));
            ex.Message.Should().Contain(nameof(SuperBackupDecoder.DecodeFileName));
            ex.Message.Should().Contain(input);
            ex.Message.Should().Contain("1");
            ex.Message.Should().Contain(param1);
        }
    }
}