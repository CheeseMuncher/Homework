using Finance.Utils;
using Google;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Finance.Domain.GoogleSheets;

namespace Finance.Data;

public interface IGoogleDataClient
{
    void Connect(string fileName, string[] scopes);
    ValueRange FetchLedgerData();
    void ClearSheetData();
    void WriteData();
}

public class GoogleDataClient : IGoogleDataClient
{
    private SheetsService? _sheetsService;
    private readonly IFileIO _fileIO;
    private readonly IGoogleRequestFactory _requestFactory;
    private readonly ILedgerManager _ledgerManager;
    
    public GoogleDataClient(IFileIO fileIO, IGoogleRequestFactory requestFactory, ILedgerManager ledgerManager)
    {
        _fileIO = fileIO;
        _requestFactory = requestFactory;
        _ledgerManager = ledgerManager;
    }

    public void Connect(string fileName, string[] scopes)
    {
        if (!_fileIO.SecretsFileExists(fileName))
            throw new FileNotFoundException("Connect file not found");

        var credential = _fileIO.BuildCredentialFromFile(fileName, scopes);
        _sheetsService = new SheetsService(new BaseClientService.Initializer() 
        { 
            HttpClientInitializer = credential, 
            ApplicationName = "Finance"
        });
    }

    public ValueRange FetchLedgerData()
    {
        try
        {
            var request = _requestFactory.GetSheetData(_sheetsService, GoogleSecrets.LedgerSpreadsheetId, "Ledger");
            if (request is not null)
                request.Execute();
        }
        catch(GoogleApiException ex)
        {
            Console.WriteLine($"Google Exception getting sheet data; status code: {ex.HttpStatusCode}, message: {ex.Message}, ");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error getting sheet data; message: {ex.Message}");
        }
        return null;
    }

    public void ClearSheetData()
    {
        var request = _requestFactory.ClearSheetData(_sheetsService, GoogleSecrets.LedgerSpreadsheetId, "Output");
        if (request is not null)
            request.Execute();
    }

    public void WriteData()
    {
        var payload = _ledgerManager.BuildLedgerWriteData();
        var request = _requestFactory.WriteSheetData(_sheetsService, GoogleSecrets.LedgerSpreadsheetId, payload, "Output");
        if (request is not null)
            request.Execute();
    }
}