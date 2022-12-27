using Finance.Data;
using Finance.Utils;
using Finance.Domain.Yahoo.Models;
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
            .Setup(io => io.DataFileExists(It.IsAny<string>()))
            .Returns(true);
        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(new [] { "{}" });

        Inject(_mockFileIO.Object);
    }

    [Fact]
    public void GetYahooFileHistoryData_ReturnsEmptyResponse_IfFileDoesNotExist()
    {
        // Arrange
        _mockFileIO
            .Setup(io => io.DataFileExists(It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = Sut.GetYahooFileHistoryData(Create<string>());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetYahooFileHistoryData_ReadsDataFromCorrectFile()
    {
        // Arrange
        var fileName = Create<string>();
        
        // Act
        Sut.GetYahooFileHistoryData(fileName);

        // Assert
        _mockFileIO.Verify(io => io.ReadLinesFromFile(fileName), Times.Once);
    }

    [Fact]
    public void GetYahooFileHistoryData_NoOtherCalls()
    {
        // Arrange
        var fileName = Create<string>();
        
        // Act
        Sut.GetYahooFileHistoryData(fileName);

        // Assert
        _mockFileIO.Verify(io => io.ReadLinesFromFile(fileName), Times.Once);
        _mockFileIO.Verify(io => io.DataFileExists(fileName), Times.Once);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetYahooFileHistoryData_DeserialisesFileData()
    {
        // Arrange
        var response = Create<HistoryResponse>();
        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(new [] { JsonSerializer.Serialize(response) } );
        
        // Act
        var result = Sut.GetYahooFileHistoryData(Create<string>());

        // Assert
        result.Should().BeEquivalentTo(response);
    }

    [Fact]
    public void GetYahooFileHistoryData_DeserialisesMultilineFileData()
    {
        // Arrange
        var response = Create<HistoryResponse>();
        var json = JsonSerializer.Serialize(response);
        json = json.Replace("{", "{\n").Replace("}", "}\n");

        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(json.Split("\n").ToArray());
        
        // Act
        var result = Sut.GetYahooFileHistoryData(Create<string>());

        // Assert
        result.Should().BeEquivalentTo(response);
    }            

    [Fact]
    public void GetYahooFileChartData_ReturnsEmptyResponse_IfFileDoesNotExist()
    {
        // Arrange
        _mockFileIO
            .Setup(io => io.DataFileExists(It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = Sut.GetYahooFileChartData(Create<string>());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetYahooFileChartData_ReadsDataFromCorrectFile()
    {
        // Arrange
        var fileName = Create<string>();
        
        // Act
        Sut.GetYahooFileChartData(fileName);

        // Assert
        _mockFileIO.Verify(io => io.ReadLinesFromFile(fileName), Times.Once);
        _mockFileIO.Verify(io => io.DataFileExists(fileName), Times.Once);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetYahooFileChartData_DeserialisesFileData()
    {
        // Arrange
        var response = Create<ChartResponse>();
        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(new [] { JsonSerializer.Serialize(response) } );
        
        // Act
        var result = Sut.GetYahooFileChartData(Create<string>());

        // Assert
        result.Should().BeEquivalentTo(response);
    }

    [Fact]
    public void GetYahooFileChartData_DeserialisesMultilineFileData()
    {
        // Arrange
        var response = Create<ChartResponse>();
        var json = JsonSerializer.Serialize(response);
        json = json.Replace("{", "{\n").Replace("}", "}\n");

        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(json.Split("\n").ToArray());
        
        // Act
        var result = Sut.GetYahooFileChartData(Create<string>());

        // Assert
        result.Should().BeEquivalentTo(response);
    }            
}