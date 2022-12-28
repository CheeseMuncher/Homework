using Finance.Utils;
using Google;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;

namespace Finance.Data;

public interface IGoogleDataClient
{
    void Connect(string fileName, string[] scopes);
    ValueRange FetchLedgerData();
}

public class GoogleDataClient : IGoogleDataClient
{
    private SheetsService? _sheetsService;
    private IFileIO _fileIO;
    private IGoogleRequestFactory _requestFactory;
    
    public GoogleDataClient(IFileIO fileIO, IGoogleRequestFactory requestFactory)
    {
        _fileIO = fileIO;
        _requestFactory = requestFactory;
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
            return _requestFactory.GetSheetData(_sheetsService, GoogleSecrets.LedgerSpreadsheetId, "Ledger").Execute();    
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
}