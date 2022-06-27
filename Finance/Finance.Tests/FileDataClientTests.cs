using Finance.Data;
using Finance.Domain.TraderMade.Models;
using Finance.Domain.Yahoo.Models;
using Finance.Utils;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Moq;
using System;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Finance.Tests;

public class FileDataClientTests : TestFixture<FileDataClient>
{
    private readonly Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();
    private readonly JsonSerializerOptions _jsonOptions;

    public FileDataClientTests()
    {
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new DateTimeConverter());

        _mockFileIO
            .Setup(io => io.FileExists(It.IsAny<string>()))
            .Returns(true);
        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(new [] { "{}" });

        Inject(_mockFileIO.Object);
    }

    [Fact]
    public void GetTraderMadeHistoryData_ReturnsEmptyResponse_IfFileDoesNotExist()
    {
        // Arrange
        _mockFileIO
            .Setup(io => io.FileExists(It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = Sut.GetTraderMadeHistoryData(Create<string>());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetTraderMadeHistoryData_ReadsDataFromCorrectFile()
    {
        // Arrange
        var fileName = Create<string>();
        
        // Act
        Sut.GetTraderMadeHistoryData(fileName);

        // Assert
        _mockFileIO.Verify(io => io.ReadLinesFromFile(fileName), Times.Once);
        _mockFileIO.Verify(io => io.FileExists(fileName), Times.Once);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetTraderMadeHistoryData_DeserialisesFileData()
    {
        // Arrange
        var response = Create<ForexHistoryResponse>();
        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(new [] { JsonSerializer.Serialize(response, _jsonOptions) } );
        
        // Act
        var result = Sut.GetTraderMadeHistoryData(Create<string>());

        // Assert
        result.Should().BeEquivalentTo(response, RelaxDate);
    }

    [Fact]
    public void GetTraderMadeHistoryData_DeserialisesRequestDate()
    {
        // Arrange
        var response = "{\"request_time\": \"Thu, 23 Jun 2022 21:52:33 GMT\"}";
        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(new [] { response });
        
        // Act
        var result = Sut.GetTraderMadeHistoryData(Create<string>());

        // Assert
        result.request_time.Should().Be(new DateTime(2022,06,23,21,52,33));
    }

    [Fact]
    public void GetTraderMadeHistoryData_DeserialisesMultilineFileData()
    {
        // Arrange
        var response = Create<ForexHistoryResponse>();
        var json = JsonSerializer.Serialize(response);
        json = json.Replace("{", "{\n").Replace("}", "}\n");

        _mockFileIO
            .Setup(io => io.ReadLinesFromFile(It.IsAny<string>()))
            .Returns(json.Split("\n").ToArray());
        
        // Act
        var result = Sut.GetTraderMadeHistoryData(Create<string>());

        // Assert
        result.Should().BeEquivalentTo(response, RelaxDate);
    }            

    [Fact]
    public void GetYahooFileHistoryData_ReturnsEmptyResponse_IfFileDoesNotExist()
    {
        // Arrange
        _mockFileIO
            .Setup(io => io.FileExists(It.IsAny<string>()))
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
        _mockFileIO.Verify(io => io.FileExists(fileName), Times.Once);
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
            .Setup(io => io.FileExists(It.IsAny<string>()))
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
        _mockFileIO.Verify(io => io.FileExists(fileName), Times.Once);
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

    private static EquivalencyAssertionOptions<ForexHistoryResponse> ExcludeRequestTime(EquivalencyAssertionOptions<ForexHistoryResponse> options) =>
        options.Excluding(fhr => fhr.request_time);

    private static EquivalencyAssertionOptions<ForexHistoryResponse> RelaxDate(EquivalencyAssertionOptions<ForexHistoryResponse> options) =>
        options
        .Using<ForexHistoryResponse>(x => x.Subject.date.Should().BeCloseTo(x.Expectation.date, TimeSpan.FromSeconds(2)))
        .WhenTypeIs<ForexHistoryResponse>();
}