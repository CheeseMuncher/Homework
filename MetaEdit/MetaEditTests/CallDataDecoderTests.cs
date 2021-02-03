using FluentAssertions;
using MetaEdit;
using Xunit;

namespace MetaEditTests
{
    public class CallDataDecoderTests : TestFixture<CallDataDecoder>
    {
        [Fact]
        public void Sut_ImplementsContract()
        {
            (Sut is IFileNameDecoder<CallData>).Should().BeTrue();
        }
    }
}