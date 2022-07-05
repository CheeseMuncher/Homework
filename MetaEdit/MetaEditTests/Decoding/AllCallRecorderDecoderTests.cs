using FluentAssertions;
using MetaEdit;
using MetaEdit.Conventions;
using MetaEdit.Decoding;
using System;
using Xunit;

namespace MetaEditTests
{
    public class AllCallRecorderDecoderTests : TestFixture<CallDataDecoder>
    {
        public AllCallRecorderDecoderTests()
        {
            Inject(new AllCallRecorderConvention() as IDecodeConvention);
        }

        [Fact]
        public void DecodeFileName_DecodesFullFileNameCorrectly()
        {
            // Arrange
            var duration = "59 s 800 ms";
            var input = "120101010233o00447123123123.3gp";

            // Act
            var result = Sut.DecodeFileName(input, duration);

            // Assert
            result.CallTime.Should().Be(new DateTime(2012, 01, 01, 01, 02, 33));
            result.CallType.Should().Be(CallType.Unknown);
            result.ContactName.Should().BeNull();
            result.ContactNumber.Should().Be("00447123123123");
            result.CallDuration.Should().Be(TimeSpan.Parse("00:00:59.800"));
            result.FileExtension.Should().Be("3gp");
        }

        [Fact]
        public void DecodeFileName_ThrowsIfMultipleParamsPassedIn()
        {
            // Arrange
            var input = "120101010233o00447123123123.3gp";
            var param1 = Create<string>();
            var param2 = Create<string>();

            // Act
            Action action = () => Sut.DecodeFileName(input, param1, param2);

            // Assert
            var ex = Assert.Throws<ArgumentException>(action);
            ex.Message.Should().Contain(nameof(CallDataDecoder));
            ex.Message.Should().Contain(nameof(AllCallRecorderConvention));
            ex.Message.Should().Contain(nameof(CallDataDecoder.DecodeFileName));
            ex.Message.Should().Contain(input);
            ex.Message.Should().Contain("2");
            ex.Message.Should().Contain(param1);
            ex.Message.Should().Contain(param2);
        }
    }
}