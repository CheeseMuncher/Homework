using System.IO;
using System.Linq;
using System.Reflection;
using Finance.Data;
using Finance.Utils;
using FluentAssertions;
using Google.Apis.Sheets.v4;
using Moq;
using Xunit;

namespace Finance.Tests;

public class GoogleDataClientTests : TestFixture<GoogleDataClient>
{
    private readonly Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();
    private readonly Mock<IGoogleRequestFactory> _mockRequestFactory = new Mock<IGoogleRequestFactory>();

    public GoogleDataClientTests()
    {
        Inject(_mockFileIO);
        Inject(_mockRequestFactory);
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
        _mockFileIO.Verify(f => f.BuildCredentialFromFile(Path.Combine("./Secrets", fileName), scopes), Times.Once);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]    
    public void Connect_InitialisesSheetService()
    {
        // Arrange
        // Act
        Sut.Connect(Create<string>(), Create<string[]>());

        // Assert
        var serviceField = typeof(GoogleDataClient)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single(field => field.Name == "_sheetsService")
            .GetValue(Sut);

        serviceField.Should().NotBeNull();
        var service = serviceField as SheetsService;
        service.Should().NotBeNull(); 
        service.ApplicationName.Should().Be("Finance");        
    }
    
    [Fact]    
    public void FetchLedgerData_InvokesRequestFactory_WithCorrectArgs()
    {
        // Arrange
        Sut.Connect(Create<string>(), Create<string[]>());
        _mockRequestFactory
            .Setup(rf => rf.GetSheetData(It.IsAny<SheetsService>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((SheetsService ss, string s, string r) => ss.Spreadsheets.Values.Get(s, r));


        // Act
        Sut.FetchLedgerData();

        // Assert
        _mockRequestFactory.Verify(f => f.GetSheetData(
            It.IsAny<SheetsService>(), GoogleSecrets.LedgerSpreadsheetId, "Ledger"), Times.Once);
        _mockRequestFactory.VerifyNoOtherCalls();
    }
}