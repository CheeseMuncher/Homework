using System.IO;
using System.Linq;
using System.Reflection;
using Finance.Data;
using Finance.Domain.GoogleSheets;
using Finance.Utils;
using FluentAssertions;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Moq;
using Xunit;

namespace Finance.Tests;

public class GoogleDataClientTests : TestFixture<GoogleDataClient>
{
    private readonly Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();
    private readonly Mock<IGoogleRequestFactory> _mockRequestFactory = new Mock<IGoogleRequestFactory>();
    private readonly Mock<ILedgerManager> _mockLedgerManager = new Mock<ILedgerManager>();

    public GoogleDataClientTests()
    {
        _mockFileIO.Setup(mfi => mfi.SecretsFileExists(It.IsAny<string>())).Returns(true);
        Inject(_mockFileIO);
        Inject(_mockRequestFactory);
        Inject(_mockLedgerManager);
    }

    [Fact]    
    public void Connect_ChecksSecretsFileExists()
    {
        // Arrange
        var fileName = Create<string>();

        // Act
        Sut.Connect(fileName, Create<string[]>());

        // Assert
        _mockFileIO.Verify(f => f.SecretsFileExists(fileName), Times.Once);
    }

    [Fact]    
    public void Connect_Throws_IfFileNotFound()
    {
        // Arrange
        var fileName = Create<string>();
        _mockFileIO.Setup(mfi => mfi.SecretsFileExists(fileName)).Returns(false);

        // Act
        var action = () => Sut.Connect(fileName, Create<string[]>());

        // Assert
        action.Should().Throw<FileNotFoundException>().WithMessage("Connect file not found");
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
        _mockFileIO.Verify(f => f.BuildCredentialFromFile(fileName, scopes), Times.Once);
    }

    [Fact]
    public void Connect_NoOtherInvocations()
    {
        // Arrange
        var fileName = Create<string>();

        // Act
        Sut.Connect(fileName, Create<string[]>());

        // Assert
        _mockFileIO.Verify(f => f.SecretsFileExists(fileName), Times.Once);
        _mockFileIO.Verify(f => f.BuildCredentialFromFile(fileName, It.IsAny<string[]>()), Times.Once);
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
        // Act
        Sut.FetchLedgerData();

        // Assert
        _mockRequestFactory.Verify(f => f.GetSheetData(
            It.IsAny<SheetsService>(), GoogleSecrets.LedgerSpreadsheetId, "Ledger"), Times.Once);
        _mockRequestFactory.VerifyNoOtherCalls();
    }

    [Fact]
    public void ClearSheetData_InvokesRequestFactory_WithCorrectArgs()
    {
        // Act
        Sut.ClearSheetData();

        // Assert
        _mockRequestFactory.Verify(f => f.ClearSheetData(
            It.IsAny<SheetsService>(), GoogleSecrets.LedgerSpreadsheetId, "Output"), Times.Once);
        _mockRequestFactory.VerifyNoOtherCalls();
    }

    [Fact]
    public void WriteData_InvokesLedgerManager_BuildLedgerWriteData()
    {
        // Act
        Sut.WriteData();
        
        // Assert
        _mockLedgerManager.Verify(f => f.BuildLedgerWriteData(), Times.Once);
        _mockLedgerManager.VerifyNoOtherCalls();
    }

    [Fact]
    public void WriteData_InvokesRequestFactory_WithCorrectArgs()
    {
        // Arrange
        var data = Create<ValueRange>();
        _mockLedgerManager.Setup(f => f.BuildLedgerWriteData()).Returns(data);

        // Act
        Sut.WriteData();
        
        // Assert
        _mockRequestFactory.Verify(f => f.WriteSheetData(
            It.IsAny<SheetsService>(), GoogleSecrets.LedgerSpreadsheetId, data, "Output"), Times.Once);
        _mockRequestFactory.VerifyNoOtherCalls();
    }
}