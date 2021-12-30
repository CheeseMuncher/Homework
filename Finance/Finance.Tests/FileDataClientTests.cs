using Finance.Data;
using Finance.Utils;
using Finance.Domain.Yahoo;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Finance.Tests;

public class FileDataClientTests : TestFixture<FileDataClient>
{
    private readonly Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();

    public FileDataClientTests()
    {
        _mockFileIO
            .Setup(io => io.FileExists(It.IsAny<string>()))
            .Returns(true);
        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(new [] { "{}" });

        Inject(_mockFileIO.Object);
    }

    [Fact]
    public void GetYahooFileData_ReturnsEmptyResponse_IfFileDoesNotExist()
    {
        // Arrange
        _mockFileIO
            .Setup(io => io.FileExists(It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = Sut.GetYahooFileData(Create<string>());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetYahooFileData_ReadsDataFromCorrectFile()
    {
        // Arrange
        var fileName = Create<string>();
        
        // Act
        Sut.GetYahooFileData(fileName);

        // Assert
        _mockFileIO.Verify(io => io.ReadLinesFromFile(fileName), Times.Once);
        _mockFileIO.Verify(io => io.FileExists(fileName), Times.Once);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetYahooFileData_DeserialisesFileData()
    {
        // Arrange
        var response = Create<Response>();
        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(new [] { JsonSerializer.Serialize(response) } );
        
        // Act
        var result = Sut.GetYahooFileData(Create<string>());

        // Assert
        result.Should().BeEquivalentTo(response);
    }

    [Fact]
    public void GetYahooFileData_DeserialisesMultilineFileData()
    {
        // Arrange
        var response = Create<Response>();
        var json = JsonSerializer.Serialize(response);
        json = json.Replace("{", "{\n").Replace("}", "}\n");

        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(json.Split("\n").ToArray());
        
        // Act
        var result = Sut.GetYahooFileData(Create<string>());

        // Assert
        result.Should().BeEquivalentTo(response);
    }

            
}