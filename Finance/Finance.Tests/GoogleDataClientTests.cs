using System.Linq;
using System.Reflection;
using Finance.Data;
using Finance.Utils;
using FluentAssertions;
using Moq;
using Xunit;

namespace Finance.Tests;

public class GoogleDataClientTests : TestFixture<GoogleDataClient>
{
    private readonly Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();

    public GoogleDataClientTests()
    {
        Inject(_mockFileIO);
    }

    [Fact]    
    public void Connect_InvokesFileIO_WithCorrectArgs()
    {
        // Arrange
        var fileName = Create<string>();
        var scopes = Create<string[]>();

        // Act
        Sut.Connect(fileName, scopes);

        // Assert
        _mockFileIO.Verify(f => f.BuildCredentialFromFile(fileName,scopes), Times.Once);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]    
    public void Connect_InitialisesSheetService()
    {
        // Arrange
        var fileName = Create<string>();
        var scopes = Create<string[]>();

        // Act
        Sut.Connect(Create<string>(), Create<string[]>());

        // Assert
        var sheetService = typeof(GoogleDataClient)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single(field => field.Name == "_sheetsService")
            .GetValue(Sut);

        sheetService.Should().NotBeNull();
    }
}